using System;
using System.Collections.Generic;
using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos.CapaNegocio
{
    /// <summary>
    /// Capa de lógica de negocio (BLL) encargada del cierre diario de caja y conciliación de turnos.
    /// Valida parámetros temporales y coordina con <see cref="CajaDAL"/>.
    /// </summary>
    public class CajaBLL
    {
        private readonly CajaDAL _cajaDAL = new CajaDAL();

        /// <summary>
        /// Obtiene las métricas consolidadas del cierre de caja para la fecha dada.
        /// </summary>
        /// <param name="fecha">Fecha a auditar.</param>
        /// <param name="soloEmitidosHoy">Si es true, consolida según la fecha de emisión del turno.</param>
        /// <returns><see cref="ReporteCierreCajaResumenDTO"/> con los totales de caja.</returns>
        public ReporteCierreCajaResumenDTO ObtenerResumenCierreCaja(DateTime fecha, bool soloEmitidosHoy = false)
        {
            return _cajaDAL.ObtenerResumenCierreCaja(fecha.Date, soloEmitidosHoy);
        }

        /// <summary>
        /// Obtiene el detalle nominal de todos los turnos emitidos/atendidos durante la jornada de caja,
        /// con filtro opcional por especialidad y por fecha de emisión.
        /// </summary>
        /// <param name="fecha">Fecha a auditar.</param>
        /// <param name="especialidad">Especialidad médica o 'Emergencia' (opcional).</param>
        /// <param name="soloEmitidosHoy">Si es true, filtra por turnos emitidos/creados en la fecha dada.</param>
        /// <returns>Lista de <see cref="ReporteCierreCajaDetalleDTO"/> con los registros de cobro.</returns>
        public List<ReporteCierreCajaDetalleDTO> ObtenerDetalleCierreCaja(DateTime fecha, string? especialidad = null, bool soloEmitidosHoy = false)
        {
            return _cajaDAL.ObtenerDetalleCierreCaja(fecha.Date, especialidad, soloEmitidosHoy);
        }

        /// <summary>
        /// Devoluciones de dinero (turnos programados cancelados sin reprogramar) registradas en el período, para revisión del gerente.
        /// </summary>
        public List<DevolucionDTO> ListarDevoluciones(DateTime desde, DateTime hasta)
        {
            if (desde.Date > hasta.Date)
                throw new ArgumentException("La fecha 'Desde' no puede ser posterior a la fecha 'Hasta'.");

            return _cajaDAL.ListarDevoluciones(desde.Date, hasta.Date);
        }

        /// <summary>
        /// Marca una devolución como revisada por el gerente.
        /// </summary>
        public void MarcarDevolucionRevisada(int idDevolucion, int? idUsuario)
        {
            if (idDevolucion <= 0)
                throw new ArgumentException("Seleccione una devolución válida.");

            _cajaDAL.MarcarDevolucionRevisada(idDevolucion, idUsuario);
        }

        /// <summary>
        /// Consolida dinámicamente un resumen financiero a partir de una lista filtrada de turnos.
        /// Permite calcular con precisión los KPIs de caja para cualquier subconjunto o especialidad.
        /// </summary>
        /// <param name="fecha">Fecha de consulta.</param>
        /// <param name="detalles">Lista de turnos filtrados.</param>
        /// <returns><see cref="ReporteCierreCajaResumenDTO"/> con los totales calculados.</returns>
        public ReporteCierreCajaResumenDTO CalcularResumenDeDetalles(DateTime fecha, List<ReporteCierreCajaDetalleDTO> detalles)
        {
            if (detalles == null || detalles.Count == 0)
            {
                return new ReporteCierreCajaResumenDTO { FechaCaja = fecha.Date };
            }

            // Las devoluciones (montos negativos) restan de los importes pero no cuentan como turnos
            var turnos = detalles.Where(d => !d.EsDevolucion).ToList();

            return new ReporteCierreCajaResumenDTO
            {
                FechaCaja = fecha.Date,
                TotalTurnos = turnos.Count,
                TotalRecaudado = detalles.Sum(d => d.MontoCobrado),
                TurnosParticulares = turnos.Count(d => d.EsParticular),
                MontoParticulares = detalles.Where(d => d.EsParticular).Sum(d => d.MontoCobrado),
                TurnosObraSocial = turnos.Count(d => !d.EsParticular),
                MontoObraSocial = detalles.Where(d => !d.EsParticular).Sum(d => d.MontoCobrado),
                TurnosEmergencia = turnos.Count(d => d.TipoTurno.Equals("Emergencia", StringComparison.OrdinalIgnoreCase)),
                MontoEmergencia = detalles.Where(d => d.TipoTurno.Equals("Emergencia", StringComparison.OrdinalIgnoreCase)).Sum(d => d.MontoCobrado),
                TurnosEspecialidad = turnos.Count(d => !d.TipoTurno.Equals("Emergencia", StringComparison.OrdinalIgnoreCase)),
                MontoEspecialidad = detalles.Where(d => !d.TipoTurno.Equals("Emergencia", StringComparison.OrdinalIgnoreCase)).Sum(d => d.MontoCobrado)
            };
        }

        /// <summary>
        /// Calcula el monto total que cubren las obras sociales: por cada turno con obra social,
        /// la diferencia entre el arancel completo y lo que abonó el paciente en caja.
        /// Se basa en lo cobrado en cada turno, así el histórico no cambia si luego se edita el % de una obra social.
        /// </summary>
        /// <param name="detalles">Lista de turnos del cierre.</param>
        /// <returns>Monto total cubierto por obras sociales.</returns>
        public static decimal CalcularCoberturaObraSocial(List<ReporteCierreCajaDetalleDTO> detalles)
        {
            // Arancel registrado en el turno; el vigente solo para turnos sin snapshot.
            // Una devolución (importes negativos) revierte la cobertura del turno devuelto.
            return detalles
                .Where(d => !d.EsParticular)
                .Sum(d =>
                {
                    decimal cobertura = Math.Max(0m, Math.Abs(d.ArancelBase ?? TurnoBLL.ObtenerArancelBase(d.TipoTurno)) - Math.Abs(d.MontoCobrado));
                    return d.EsDevolucion ? -cobertura : cobertura;
                });
        }
    }
}
