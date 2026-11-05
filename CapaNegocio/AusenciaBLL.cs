using System;
using System.Collections.Generic;
using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos.Negocio
{
    /// <summary>
    /// Capa de lógica de negocio para las ausencias programadas de los profesionales médicos.
    /// Las ausencias se cargan por día completo y con al menos un día de anticipación para poder notificar a los pacientes.
    /// </summary>
    public class AusenciaBLL
    {
        private readonly AusenciaDAL _ausenciaDAL = new AusenciaDAL();

        /// <summary>
        /// Valida el rango y registra un día de ausencia por cada fecha entre <paramref name="desde"/> y <paramref name="hasta"/>.
        /// </summary>
        /// <param name="idUsuario">Médico autenticado.</param>
        /// <param name="desde">Primer día de ausencia (mínimo mañana).</param>
        /// <param name="hasta">Último día de ausencia.</param>
        /// <param name="motivo">Motivo opcional (máx. 200 caracteres).</param>
        /// <returns>Días registrados y turnos que quedaron sin profesional.</returns>
        /// <exception cref="ArgumentException">Se lanza si el médico o el rango de fechas son inválidos.</exception>
        public ResultadoAusenciaDTO RegistrarAusencias(int idUsuario, DateTime desde, DateTime hasta, string? motivo)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("No se pudo identificar al profesional autenticado.");

            if (desde.Date <= DateTime.Today)
                throw new ArgumentException("Las ausencias deben registrarse con al menos un día de anticipación (a partir de mañana).");

            if (hasta.Date < desde.Date)
                throw new ArgumentException("La fecha hasta debe ser igual o posterior a la fecha desde.");

            motivo = string.IsNullOrWhiteSpace(motivo) ? null : motivo.Trim();
            if (motivo?.Length > 200)
                throw new ArgumentException("El motivo no puede superar los 200 caracteres.");

            return _ausenciaDAL.RegistrarAusencias(idUsuario, desde, hasta, motivo);
        }

        /// <summary>
        /// Obtiene las ausencias vigentes (desde hoy) del médico.
        /// </summary>
        /// <param name="idUsuario">Médico autenticado.</param>
        public List<AusenciaMedicaDTO> ListarAusenciasPorMedico(int idUsuario)
        {
            return _ausenciaDAL.ListarAusenciasPorMedico(idUsuario);
        }

        /// <summary>
        /// Anula una ausencia propia cuya fecha sea a partir de mañana.
        /// </summary>
        /// <param name="idAusencia">Ausencia a anular.</param>
        /// <param name="idUsuario">Médico autenticado.</param>
        /// <exception cref="ArgumentException">Se lanza si los identificadores son inválidos.</exception>
        public void AnularAusencia(int idAusencia, int idUsuario)
        {
            if (idAusencia <= 0 || idUsuario <= 0)
                throw new ArgumentException("La ausencia seleccionada no es válida.");

            _ausenciaDAL.AnularAusencia(idAusencia, idUsuario);
        }
    }
}
