using Gestion_de_Turnos_Medicos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

namespace Gestion_de_Turnos_Medicos.CapaDeDatos
{
    public class HistoriaClinicaDAL
    {
        // Guarda el registro clínico, anamnesis y recetas al finalizar la atención médica[cite: 3].
        public void InsertarHistoria(string tipoTurno, string diagRapido, string descripHistoriaClinica, string recetaMedicamentos, int idPaciente, int idTurno, int idUsuario)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pTipoTurno = new SqlParameter("@TipoTurno", tipoTurno);
                var pDiagRapido = new SqlParameter("@DiagRapido", (object)diagRapido ?? DBNull.Value);
                var pDescrip = new SqlParameter("@DescripHistoriaClinica", (object)descripHistoriaClinica ?? DBNull.Value);
                var pReceta = new SqlParameter("@RecetaMedicamentos", (object)recetaMedicamentos ?? DBNull.Value);
                var pIdPaciente = new SqlParameter("@IdPaciente", idPaciente);
                var pIdTurno = new SqlParameter("@IdTurno", idTurno);
                var pIdUsuario = new SqlParameter("@IdUsuario", idUsuario);

                context.Database.ExecuteSqlRaw("EXEC sp_InsertarHistoriaClinica @TipoTurno, @DiagRapido, @DescripHistoriaClinica, @RecetaMedicamentos, @IdPaciente, @IdTurno, @IdUsuario",
                    pTipoTurno, pDiagRapido, pDescrip, pReceta, pIdPaciente, pIdTurno, pIdUsuario);
            }
        }

        public List<HistoriaClinicaDTO> ObtenerHistoriaClinicaPaciente(int idPaciente)
        {
            using (var context = new dbTurnosMedicos())
            {
                var pIdPaciente = new SqlParameter("@IdPaciente", idPaciente);

                return context.Database
                    .SqlQueryRaw<HistoriaClinicaDTO>("EXEC sp_ObtenerHistoriaClinicaPaciente @IdPaciente", pIdPaciente)
                    .ToList();
            }
        }
    }
}