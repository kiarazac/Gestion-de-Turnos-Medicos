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
        /// Ejecuta el procedimiento almacenado <c>sp_ReporteCierreCajaDetalle</c> para recuperar
        /// la caja de una fecha: turnos emitidos ese día (sin importar para qué día están programados)
        /// y devoluciones registradas ese día, con filtro opcional por especialidad.
        /// </summary>
        /// <param name="fecha">Fecha de emisión a auditar.</param>
        /// <param name="especialidad">Nombre de la especialidad o 'Emergencia' (opcional, null para todas).</param>
        /// <returns>Lista de <see cref="ReporteCierreCajaDetalleDTO"/> con cada turno emitido/cobrado.</returns>
        public List<ReporteCierreCajaDetalleDTO> ObtenerDetalleCierreCaja(DateTime fecha, string? especialidad = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pFecha = new SqlParameter("@Fecha", fecha.Date);
                var pEsp = new SqlParameter("@Especialidad", (object?)especialidad ?? DBNull.Value);

                try
                {
                    return context.Database
                        .SqlQueryRaw<ReporteCierreCajaDetalleDTO>(
                            "EXEC sp_ReporteCierreCajaDetalle @Fecha, @Especialidad",
                            pFecha, pEsp)
                        .ToList();
                }
                catch (SqlException)
                {
                    // Fallback directo a través de EF Core LINQ por si el SP no está actualizado en SQL Server:
                    // turnos emitidos en la fecha (sin las filas de devolución, que solo arma el SP).
                    var query = context.Turnos
                        .Include(t => t.Paciente)
                            .ThenInclude(p => p.ObraSocial)
                        .Include(t => t.Especialidad)
                        .Include(t => t.Usuario)
                        .Where(t => t.Activo && t.Estado != "Cancelado")
                        .Where(t => t.FechaCreacion.Date == fecha.Date);

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
                            TipoTurno = t.TipoTurno ?? "Especialidad",
                            Especialidad = t.Especialidad?.Nombre ?? "General",
                            MontoCobrado = t.Monto,
                            Estado = t.Estado ?? "En Espera",
                            MedicoAsignado = t.Usuario != null ? $"Dr. {t.Usuario.Apellido} {t.Usuario.Nombre}" : "Sin Asignar"
                        };
                    }).ToList();
                }
            }
        }

        /// <summary>
        /// Ejecuta <c>sp_ListarDevoluciones</c>: devoluciones registradas entre las fechas dadas.
        /// </summary>
        public List<DevolucionDTO> ListarDevoluciones(DateTime desde, DateTime hasta)
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database
                    .SqlQueryRaw<DevolucionDTO>("EXEC sp_ListarDevoluciones @FechaDesde, @FechaHasta",
                        new SqlParameter("@FechaDesde", desde.Date), new SqlParameter("@FechaHasta", hasta.Date))
                    .ToList();
            }
        }

        /// <summary>
        /// Ejecuta <c>sp_MarcarDevolucionRevisada</c>: registra la revisión de la devolución por el gerente.
        /// </summary>
        public void MarcarDevolucionRevisada(int idDevolucion, int? idUsuario)
        {
            using (var context = new dbTurnosMedicos())
            {
                context.Database.ExecuteSqlRaw("EXEC sp_MarcarDevolucionRevisada @IdDevolucion, @IdUsuario",
                    new SqlParameter("@IdDevolucion", idDevolucion),
                    new SqlParameter("@IdUsuario", (object?)idUsuario ?? DBNull.Value));
            }
        }
    }
}
