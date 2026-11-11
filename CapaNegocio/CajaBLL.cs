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
        /// Obtiene la caja de una fecha: turnos emitidos ese día (sin importar para qué día están programados)
        /// y devoluciones registradas ese día, con filtro opcional por especialidad.
        /// </summary>
        /// <param name="fecha">Fecha de emisión a auditar.</param>
        /// <param name="especialidad">Especialidad médica o 'Emergencia' (opcional).</param>
        /// <returns>Lista de <see cref="ReporteCierreCajaDetalleDTO"/> con los registros de cobro.</returns>
        public List<ReporteCierreCajaDetalleDTO> ObtenerDetalleCierreCaja(DateTime fecha, string? especialidad = null)
        {
            return _cajaDAL.ObtenerDetalleCierreCaja(fecha.Date, especialidad);
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

            // Lo cobrado y lo devuelto se informan por separado: una devolución del día puede ser de un turno cobrado otro día,
            // así que restarla de los desgloses los dejaría en negativo. Las devoluciones solo afectan el neto.
            var turnos = detalles.Where(d => !d.EsDevolucion).ToList();
            var devoluciones = detalles.Where(d => d.EsDevolucion).ToList();
            decimal cobrado = turnos.Sum(d => d.MontoCobrado);
            decimal devuelto = -devoluciones.Sum(d => d.MontoCobrado);

            return new ReporteCierreCajaResumenDTO
            {
                FechaCaja = fecha.Date,
                TotalTurnos = turnos.Count,
                TotalCobrado = cobrado,
                TotalDevuelto = devuelto,
                CantidadDevoluciones = devoluciones.Count,
                TotalRecaudado = cobrado - devuelto,
                TurnosParticulares = turnos.Count(d => d.EsParticular),
                MontoParticulares = turnos.Where(d => d.EsParticular).Sum(d => d.MontoCobrado),
                TurnosObraSocial = turnos.Count(d => !d.EsParticular),
                MontoObraSocial = turnos.Where(d => !d.EsParticular).Sum(d => d.MontoCobrado),
                TurnosEmergencia = turnos.Count(d => d.TipoTurno.Equals("Emergencia", StringComparison.OrdinalIgnoreCase)),
                MontoEmergencia = turnos.Where(d => d.TipoTurno.Equals("Emergencia", StringComparison.OrdinalIgnoreCase)).Sum(d => d.MontoCobrado),
                TurnosEspecialidad = turnos.Count(d => !d.TipoTurno.Equals("Emergencia", StringComparison.OrdinalIgnoreCase)),
                MontoEspecialidad = turnos.Where(d => !d.TipoTurno.Equals("Emergencia", StringComparison.OrdinalIgnoreCase)).Sum(d => d.MontoCobrado)
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
            // A la obra social no se le factura un turno cancelado, y las filas de devolución no suman ni restan cobertura.
            return detalles
                .Where(d => !d.EsParticular && !d.EsDevolucion && !string.Equals(d.Estado, "Cancelado", StringComparison.OrdinalIgnoreCase))
                .Sum(d => Math.Max(0m, (d.ArancelBase ?? TurnoBLL.ObtenerArancelBase(d.TipoTurno)) - d.MontoCobrado));
        }
    }
}
