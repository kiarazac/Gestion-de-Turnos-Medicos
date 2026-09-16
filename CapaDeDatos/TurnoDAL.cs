using Gestion_de_Turnos_Medicos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    public class TurnoDAL
    {
        public List<SintomaDTO> ObtenerSintomas()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database.SqlQueryRaw<SintomaDTO>("EXEC sp_ObtenerSintomas").ToList();
            }
        }

        // 1. Crea la emergencia base y atrapa el ID real (reemplaza a RegistrarTurnoEmergencia)
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

        // 2. Guarda la relación en la tabla intermedia
        public void GuardarTurnoSintoma(int idTurno, int idSintoma)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);
                var pIdSintoma = new SqlParameter("@IdSintoma", idSintoma);

                context.Database.ExecuteSqlRaw("EXEC sp_GuardarTurnoSintoma @IdTurno, @IdSintoma", pIdTurno, pIdSintoma);
            }
        }

        public int ObtenerGravedadDeSintoma(int idSintoma)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdSintoma = new SqlParameter("@IdSintoma", idSintoma);

                var resultado = context.Database
                    .SqlQueryRaw<GravedadSintomaDTO>("EXEC sp_ObtenerGravedadSintoma @IdSintoma", pIdSintoma)
                    .AsEnumerable()
                    .FirstOrDefault();

                // 1. Extraemos el texto de la base de datos (o "Baja" si viene nulo)
                string gravedadTexto = resultado != null ? resultado.Gravedad : "Baja";

                // 2. Lo traducimos al número de prioridad (1=Alta, 2=Media, 3=Baja)
                if (gravedadTexto.Equals("Alta", StringComparison.OrdinalIgnoreCase))
                    return 1;

                if (gravedadTexto.Equals("Media", StringComparison.OrdinalIgnoreCase))
                    return 2;

                return 3;
            }
        }



        // Corregido al SP sp_LlamarSiguientePaciente con sus parámetros exactos[cite: 3].
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

        // Obtiene los horarios libres para una especialidad y fecha determinadas[cite: 3].
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

        // Registra un turno programado devolviendo el ID autogenerado y su número de orden temporal[cite: 3].
        public ResultadoTurnoDTO CrearTurnoEspecialidad(int idPaciente, string nombreEspecialidad, DateTime fecha, string horario, string estado = "En Espera")
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdPaciente = new SqlParameter("@IdPaciente", idPaciente);
                var pEspecialidad = new SqlParameter("@NombreEspecialidad", nombreEspecialidad);
                var pFecha = new SqlParameter("@Fecha", fecha.Date);
                var pHorario = new SqlParameter("@Horario", horario);
                var pEstado = new SqlParameter("@Estado", (object?)estado ?? "En Espera");

                var res = context.Database
                    .SqlQueryRaw<ResultadoTurnoDTO>("EXEC sp_CrearTurnoEspecialidad @IdPaciente, @NombreEspecialidad, @Fecha, @Horario",
                        pIdPaciente, pEspecialidad, pFecha, pHorario)
                    .AsEnumerable()
                    .FirstOrDefault();

                return res ?? new ResultadoTurnoDTO();
            }
        }

        // Dentro de CapaDeDatos -> TurnoDAL.cs

        public List<TurnoEmergenciaDTO> ListarTurnosEmergencia()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database.SqlQueryRaw<TurnoEmergenciaDTO>("EXEC sp_ListarTurnosEmergencia").ToList();
            }
        }

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

        public List<TurnoAtencionDTO> ListarTurnosAtencion()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database.SqlQueryRaw<TurnoAtencionDTO>("EXEC sp_ListarTurnosAtencion").ToList();
            }
        }

        public void IniciarAtencionTurno(int idTurno)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);
                context.Database.ExecuteSqlRaw("EXEC sp_IniciarAtencionTurno @IdTurno", pIdTurno);
            }
        }

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

        public List<TurnoPantallaDTO> ObtenerTurnosPantallaPublica()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database.SqlQueryRaw<TurnoPantallaDTO>("EXEC sp_ObtenerTurnosPantallaPublica").ToList();
            }
        }

        public List<TurnoGeneralPantallaDTO> ObtenerTurnosPantallaGeneral()
        {
            using (var context = new dbTurnosMedicos())
            {
                return context.Database
                    .SqlQueryRaw<TurnoGeneralPantallaDTO>("EXEC sp_ListarTurnosGeneralesPantalla")
                    .ToList();
            }
        }

        // Lista los turnos en espera para una especialidad específica[cite: 3].
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

        public PacienteDTO BuscarPacientePorDNI(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("Debe indicar un DNI válido para la búsqueda.");

            // Ejecutamos el Stored Procedure mapeando directamente el resultado al DTO
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