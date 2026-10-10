using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    /// <summary>
    /// Capa de acceso a datos (DAL) responsable del módulo de tesorería y cierre de caja diario.
    /// Ejecuta los procedimientos almacenados de conciliación de turnos y recaudación monetaria.
    /// </summary>
    public class CajaDAL
    {
        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteCierreCajaDiario</c> para obtener
        /// las métricas cuantitativas y montos consolidados recaudados en la fecha especificada.
        /// </summary>
        /// <param name="fecha">Fecha de la jornada a liquidar.</param>
        /// <param name="soloEmitidosHoy">Si es true, consolida según la fecha de emisión/creación del turno.</param>
        /// <returns>Objeto <see cref="ReporteCierreCajaResumenDTO"/> con los totales de caja.</returns>
        public ReporteCierreCajaResumenDTO ObtenerResumenCierreCaja(DateTime fecha, bool soloEmitidosHoy = false)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pFecha = new SqlParameter("@Fecha", fecha.Date);
                var pSoloEmitidos = new SqlParameter("@SoloEmitidosHoy", soloEmitidosHoy);

                try
                {
                    return context.Database
                        .SqlQueryRaw<ReporteCierreCajaResumenDTO>("EXEC sp_ReporteCierreCajaDiario @Fecha, @SoloEmitidosHoy", pFecha, pSoloEmitidos)
                        .AsEnumerable()
                        .FirstOrDefault() ?? new ReporteCierreCajaResumenDTO { FechaCaja = fecha.Date };
                }
                catch (SqlException)
                {
                    var pFecha2 = new SqlParameter("@Fecha", fecha.Date);
                    return context.Database
                        .SqlQueryRaw<ReporteCierreCajaResumenDTO>("EXEC sp_ReporteCierreCajaDiario @Fecha", pFecha2)
                        .AsEnumerable()
                        .FirstOrDefault() ?? new ReporteCierreCajaResumenDTO { FechaCaja = fecha.Date };
                }
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteCierreCajaDetalle</c> para recuperar
        /// la nómina completa y discriminada de turnos y montos cobrados,
        /// con soporte para filtrar por fecha de emisión o fecha programada y por especialidad.
        /// </summary>
        /// <param name="fecha">Fecha de la jornada a auditar.</param>
        /// <param name="especialidad">Nombre de la especialidad o 'Emergencia' (opcional, null para todas).</param>
        /// <param name="soloEmitidosHoy">Si es true, filtra por fecha en que el turno fue emitido (FechaCreacion).</param>
        /// <returns>Lista de <see cref="ReporteCierreCajaDetalleDTO"/> con cada turno emitido/cobrado.</returns>
        public List<ReporteCierreCajaDetalleDTO> ObtenerDetalleCierreCaja(DateTime fecha, string? especialidad = null, bool soloEmitidosHoy = false)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pFecha = new SqlParameter("@Fecha", fecha.Date);
                var pEsp = new SqlParameter("@Especialidad", (object?)especialidad ?? DBNull.Value);
                var pSoloEmitidos = new SqlParameter("@SoloEmitidosHoy", soloEmitidosHoy);

                try
                {
                    return context.Database
                        .SqlQueryRaw<ReporteCierreCajaDetalleDTO>(
                            "EXEC sp_ReporteCierreCajaDetalle @Fecha, @Especialidad, @SoloEmitidosHoy",
                            pFecha, pEsp, pSoloEmitidos)
                        .ToList();
                }
                catch (SqlException)
                {
                    // Fallback directo a través de EF Core LINQ para garantizar que si el SP no está actualizado en SQL Server,
                    // la consulta filtre correctamente por fecha de creación o fecha programada.
                    var query = context.Turnos
                        .Include(t => t.Paciente)
                            .ThenInclude(p => p.ObraSocial)
                        .Include(t => t.Especialidad)
                        .Include(t => t.Usuario)
                        .Where(t => t.Activo && t.Estado != "Cancelado");

                    if (soloEmitidosHoy)
                    {
                        query = query.Where(t => t.FechaCreacion.Date == fecha.Date);
                    }
                    else
                    {
                        query = query.Where(t => t.Fecha.Date == fecha.Date);
                    }

                    if (!string.IsNullOrWhiteSpace(especialidad) &&
                        !especialidad.Equals("Todas", StringComparison.OrdinalIgnoreCase) &&
                        !especialidad.Equals("Todas las Especialidades", StringComparison.OrdinalIgnoreCase))
                    {
                        if (especialidad.Equals("Emergencia", StringComparison.OrdinalIgnoreCase) ||
                            especialidad.Equals("Guardia", StringComparison.OrdinalIgnoreCase))
                        {
                            query = query.Where(t => t.TipoTurno == "Emergencia");
                        }
                        else
                        {
                            query = query.Where(t => t.Especialidad.Nombre == especialidad);
                        }
                    }

                    var turnosEntidades = query
                        .OrderBy(t => t.Horario)
                        .ThenBy(t => t.IdTurno)
                        .ToList();

                    return turnosEntidades.Select(t =>
                    {
                        bool esPart = t.Paciente?.ObraSocial == null ||
                                      string.IsNullOrWhiteSpace(t.Paciente.ObraSocial.Nombre) ||
                                      t.Paciente.ObraSocial.Nombre.IndexOf("Particular", StringComparison.OrdinalIgnoreCase) >= 0;

                        return new ReporteCierreCajaDetalleDTO
                        {
                            IdTurno = t.IdTurno,
                            NroOrden = t.NroOrden ?? "S/N",
                            Fecha = t.Fecha.Date,
                            Horario = t.Horario,
                            FechaEmision = t.FechaCreacion,
                            PacienteCompleto = t.Paciente != null ? $"{t.Paciente.Apellido}, {t.Paciente.Nombre}" : "Sin Paciente",
                            DniPaciente = t.Paciente?.Dni ?? "--",
                            ObraSocial = t.Paciente?.ObraSocial?.Nombre ?? "Particular / Sin Obra Social",
                            EsParticular = esPart,
                            TipoTurno = t.TipoTurno ?? "Consulta",
                            Especialidad = t.Especialidad?.Nombre ?? "General",
                            MontoCobrado = t.Monto,
                            Estado = t.Estado ?? "En Espera",
                            MedicoAsignado = t.Usuario != null ? $"Dr. {t.Usuario.Apellido} {t.Usuario.Nombre}" : "Sin Asignar"
                        };
                    }).ToList();
                }
            }
        }
    }
}
