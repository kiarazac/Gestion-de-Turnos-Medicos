using Gestion_de_Turnos_Medicos.ResultadosSQL;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    /// <summary>
    /// Capa de acceso a datos (DAL) para la gestión, emisión y ciclo de vida de los turnos médicos,
    /// ejecución de triage, control de llamadas a consultorio y pantallas públicas mediante Stored Procedures.
    /// </summary>
    public class TurnoDAL
    {
        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ObtenerSintomas</c> para recuperar el catálogo de síntomas y gravedades.
        /// </summary>
        /// <returns>Lista de <see cref="SintomaDTO"/>.</returns>
        public List<SintomaDTO> ObtenerSintomas()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database.SqlQueryRaw<SintomaDTO>("EXEC sp_ObtenerSintomas").ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_CrearTurnoEmergencia</c> para registrar un turno de urgencia en base de datos.
        /// </summary>
        /// <param name="idPaciente">Identificador del paciente.</param>
        /// <param name="idPrioridad">Nivel de prioridad calculado (1=Alta, 2=Media, 3=Baja).</param>
        /// <returns>Objeto <see cref="ResultadoTurnoDTO"/> con el ID del turno creado y el número de orden asignado.</returns>
        public ResultadoTurnoDTO CrearTurnoEmergenciaCompleto(int idPaciente, int idPrioridad)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdPaciente = new SqlParameter("@IdPaciente", idPaciente);
                var pIdPrioridad = new SqlParameter("@IdPrioridad", idPrioridad);

                var resultado = context.Database
                    .SqlQueryRaw<ResultadoTurnoDTO>("EXEC sp_CrearTurnoEmergencia @IdPaciente, @IdPrioridad", pIdPaciente, pIdPrioridad)
                    .AsEnumerable()
                    .FirstOrDefault();

                return resultado ?? new ResultadoTurnoDTO();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_GuardarTurnoSintoma</c> para vincular un síntoma reportado con el turno generado.
        /// </summary>
        /// <param name="idTurno">Identificador del turno.</param>
        /// <param name="idSintoma">Identificador del síntoma.</param>
        public void GuardarTurnoSintoma(int idTurno, int idSintoma)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);
                var pIdSintoma = new SqlParameter("@IdSintoma", idSintoma);

                context.Database.ExecuteSqlRaw("EXEC sp_GuardarTurnoSintoma @IdTurno, @IdSintoma", pIdTurno, pIdSintoma);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ObtenerGravedadSintoma</c> y devuelve el valor numérico de prioridad correspondiente.
        /// </summary>
        /// <param name="idSintoma">Identificador del síntoma.</param>
        /// <returns>Nivel numérico de prioridad: 1 para Alta, 2 para Media, 3 para Baja.</returns>
        public int ObtenerGravedadDeSintoma(int idSintoma)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSintoma = new SqlParameter("@IdSintoma", idSintoma);

                var resultado = context.Database
                    .SqlQueryRaw<GravedadSintomaDTO>("EXEC sp_ObtenerGravedadSintoma @IdSintoma", pIdSintoma)
                    .AsEnumerable()
                    .FirstOrDefault();

                string gravedadTexto = resultado != null ? resultado.Gravedad : "Baja";

                if (gravedadTexto.Equals("Alta", StringComparison.OrdinalIgnoreCase))
                    return 1;

                if (gravedadTexto.Equals("Media", StringComparison.OrdinalIgnoreCase))
                    return 2;

                return 3;
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_LlamarSiguienteTurno</c> para convocar al paciente a una sala con su médico asignado.
        /// </summary>
        /// <param name="idTurno">Identificador del turno.</param>
        /// <param name="nombreMedico">Nombre del médico que realiza la llamada.</param>
        /// <param name="salaAsignada">Consultorio o sala asignada para la atención.</param>
        public void LlamarSiguientePaciente(int idTurno, string nombreMedico, string salaAsignada)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);
                var pNombreMedico = new SqlParameter("@NombreMedico", nombreMedico);
                var pSalaAsignada = new SqlParameter("@SalaAsignada", salaAsignada);

                context.Database.ExecuteSqlRaw("EXEC sp_LlamarSiguienteTurno @IdTurno, @NombreMedico, @SalaAsignada",
                    pIdTurno, pNombreMedico, pSalaAsignada);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ObtenerHorariosDisponibles</c> para una especialidad y fecha determinadas.
        /// </summary>
        /// <param name="nombreEspecialidad">Nombre de la especialidad.</param>
        /// <param name="fecha">Fecha requerida.</param>
        /// <returns>Lista de <see cref="HorarioDisponibleDTO"/> con las franjas horarias vacantes.</returns>
        public List<HorarioDisponibleDTO> ObtenerHorariosDisponibles(string nombreEspecialidad, DateTime fecha)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pEspecialidad = new SqlParameter("@NombreEspecialidad", nombreEspecialidad);
                var pFecha = new SqlParameter("@Fecha", fecha.Date);

                return context.Database
                    .SqlQueryRaw<HorarioDisponibleDTO>("EXEC sp_ObtenerHorariosDisponibles @NombreEspecialidad, @Fecha", pEspecialidad, pFecha)
                    .ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_CrearTurnoEspecialidad</c> para registrar un turno programado.
        /// </summary>
        /// <param name="idPaciente">Identificador del paciente.</param>
        /// <param name="nombreEspecialidad">Nombre de la especialidad solicitada.</param>
        /// <param name="fecha">Fecha acordada.</param>
        /// <param name="horario">Horario asignado.</param>
        /// <param name="estado">Estado inicial del turno.</param>
        /// <returns>Objeto <see cref="ResultadoTurnoDTO"/> con el ID del nuevo turno y número de orden.</returns>
        public ResultadoTurnoDTO CrearTurnoEspecialidad(int idPaciente, string nombreEspecialidad, DateTime fecha, string horario, string estado = "En Espera")
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdPaciente = new SqlParameter("@IdPaciente", idPaciente);
                var pEspecialidad = new SqlParameter("@NombreEspecialidad", nombreEspecialidad);
                var pFecha = new SqlParameter("@Fecha", fecha.Date);
                var pHorario = new SqlParameter("@Horario", horario);

                var res = context.Database
                    .SqlQueryRaw<ResultadoTurnoDTO>("EXEC sp_CrearTurnoEspecialidad @IdPaciente, @NombreEspecialidad, @Fecha, @Horario",
                        pIdPaciente, pEspecialidad, pFecha, pHorario)
                    .AsEnumerable()
                    .FirstOrDefault();

                return res ?? new ResultadoTurnoDTO();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ListarTurnosEmergencia</c> para obtener la lista de urgencias registradas.
        /// </summary>
        /// <returns>Lista de <see cref="TurnoEmergenciaDTO"/>.</returns>
        public List<TurnoEmergenciaDTO> ListarTurnosEmergencia()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database.SqlQueryRaw<TurnoEmergenciaDTO>("EXEC sp_ListarTurnosEmergencia").ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ListarTurnosEspecialidad</c> para listar turnos filtrados por nombre de especialidad.
        /// </summary>
        /// <param name="nombreEspecialidad">Nombre de la especialidad.</param>
        /// <returns>Lista de <see cref="TurnoListadoDTO"/>.</returns>
        public List<TurnoListadoDTO> ListarTurnosEspecialidad(string nombreEspecialidad)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pNombreEspecialidad = new SqlParameter("@NombreEspecialidad", nombreEspecialidad);
                return context.Database
                    .SqlQueryRaw<TurnoListadoDTO>("EXEC sp_ListarTurnosEspecialidad @NombreEspecialidad", pNombreEspecialidad)
                    .ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ObtenerListaTurnos</c> con filtros opcionales de especialidad y estado.
        /// </summary>
        /// <param name="idEspecialidad">ID de especialidad (opcional).</param>
        /// <param name="estado">Estado del turno (opcional).</param>
        /// <returns>Lista de <see cref="TurnoListadoDTO"/>.</returns>
        public List<TurnoListadoDTO> ObtenerListaTurnos(int? idEspecialidad = null, string? estado = null)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdEspecialidad = new SqlParameter("@IdEspecialidad", (object)idEspecialidad ?? DBNull.Value);
                var pEstado = new SqlParameter("@Estado", (object?)estado ?? DBNull.Value);

                return context.Database
                    .SqlQueryRaw<TurnoListadoDTO>("EXEC sp_ObtenerListaTurnos @IdEspecialidad, @Estado", pIdEspecialidad, pEstado)
                    .ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ListarTurnosAtencion</c> para obtener la cola de turnos en proceso de atención.
        /// </summary>
        /// <returns>Lista de <see cref="TurnoAtencionDTO"/>.</returns>
        public List<TurnoAtencionDTO> ListarTurnosAtencion()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database.SqlQueryRaw<TurnoAtencionDTO>("EXEC sp_ListarTurnosAtencion").ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_IniciarAtencionTurno</c> marcando el ingreso del paciente al consultorio.
        /// </summary>
        /// <param name="idTurno">ID del turno.</param>
        /// <param name="salaAsignada">Nombre del consultorio o sala.</param>
        public void IniciarAtencionTurno(int idTurno, string salaAsignada)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);
                var pSalaAsignada = new SqlParameter("@SalaAsignada", System.Data.SqlDbType.NVarChar, 100)
                {
                    Value = (object?)salaAsignada ?? DBNull.Value
                };

                context.Database.ExecuteSqlRaw("EXEC sp_IniciarAtencionTurno @IdTurno, @SalaAsignada", pIdTurno, pSalaAsignada);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_FinalizarAtencionTurno</c> para registrar el diagnóstico y concluir la consulta.
        /// </summary>
        /// <param name="idTurno">ID del turno finalizado.</param>
        /// <param name="diagnostico">Conclusiones diagnósticas emitidas por el médico.</param>
        /// <param name="nombreMedico">Nombre del médico tratante.</param>
        /// <param name="salaAsignada">Consultorio donde se prestó la atención.</param>
        public void FinalizarAtencionTurno(int idTurno, string diagnostico, string nombreMedico, string salaAsignada)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);
                var pDiagnostico = new SqlParameter("@Diagnostico", diagnostico);
                var pNombreMedico = new SqlParameter("@NombreMedico", nombreMedico);
                var pSalaAsignada = new SqlParameter("@SalaAsignada", salaAsignada);

                context.Database.ExecuteSqlRaw("EXEC sp_FinalizarAtencionTurno @IdTurno, @Diagnostico, @NombreMedico, @SalaAsignada",
                    pIdTurno, pDiagnostico, pNombreMedico, pSalaAsignada);
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ObtenerTurnosPantallaPublica</c> para poblar la pantalla de llamados en sala de espera.
        /// </summary>
        /// <returns>Lista de <see cref="TurnoPantallaDTO"/>.</returns>
        public List<TurnoPantallaDTO> ObtenerTurnosPantallaPublica()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database.SqlQueryRaw<TurnoPantallaDTO>("EXEC sp_ObtenerTurnosPantallaPublica").ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ListarTurnosGeneralesPantalla</c> para alimentar el tablero general de visualización.
        /// </summary>
        /// <returns>Lista de <see cref="TurnoGeneralPantallaDTO"/>.</returns>
        public List<TurnoGeneralPantallaDTO> ObtenerTurnosPantallaGeneral()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database
                    .SqlQueryRaw<TurnoGeneralPantallaDTO>("EXEC sp_ListarTurnosGeneralesPantalla")
                    .ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_ObtenerTurnosEnEspera</c> para obtener la cola ordenada de una especialidad.
        /// </summary>
        /// <param name="idEspecialidad">Identificador único de la especialidad.</param>
        /// <returns>Lista de <see cref="TurnoEsperaDTO"/>.</returns>
        public List<TurnoEsperaDTO> ObtenerTurnosEnEspera(int idEspecialidad)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdEspecialidad = new SqlParameter("@IdEspecialidad", idEspecialidad);

                return context.Database
                    .SqlQueryRaw<TurnoEsperaDTO>("EXEC sp_ObtenerTurnosEnEspera @IdEspecialidad", pIdEspecialidad)
                    .ToList();
            }
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado <c>sp_BuscarPacientePorDNI</c> para obtener la ficha de un paciente.
        /// </summary>
        /// <param name="dni">Número de DNI del paciente a buscar.</param>
        /// <returns>Objeto <see cref="PacienteDTO"/> si existe; de lo contrario, <c>null</c>.</returns>
        /// <exception cref="ArgumentException">Se lanza si el DNI es nulo o vacío.</exception>
        public PacienteDTO BuscarPacientePorDNI(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("Debe indicar un DNI válido para la búsqueda.");

            string query = "EXEC sp_BuscarPacientePorDNI @DNI";
            var parametro = new Microsoft.Data.SqlClient.SqlParameter("@DNI", dni);

            using (var context = new dbTurnosMedicos())
            {
                var resultado = context.Database.SqlQueryRaw<PacienteDTO>(query, parametro).AsEnumerable().FirstOrDefault();
                return resultado;
            }
        }
    }
}