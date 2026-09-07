using System;
using System.Collections.Generic;
using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.ResultadosSQL;

namespace Gestion_de_Turnos_Medicos.Negocio
{
    // La Capa de Negocio (BLL) funciona como puente entre la Interfaz Gráfica (Formularios) y la Capa de Datos (DAL)[cite: 2].
    // Su propósito es procesar las reglas de dominio médico y administrativo antes de interactuar con la base de datos[cite: 2].
    public class SalaBLL
    {
        // Instancia privada de la clase DAL. Los formularios de la UI nunca ven ni tocan esta instancia[cite: 2].
        private readonly SalaDAL _salaDAL = new SalaDAL();

        // Método que la interfaz llama para obtener las salas. Pasa la responsabilidad a la DAL.
        public List<SalaDTO> ObtenerSalas(int? idUsuario = null)
        {
            return _salaDAL.ObtenerSalas(idUsuario);
        }

        // Método invocado cuando un médico intenta abrir una sala al iniciar su jornada[cite: 1].
        public void AbrirSala(int idSala, int idUsuario)
        {
            // Validación de dominio: comprobamos que la UI no envíe identificadores erróneos (como 0 o negativos) antes de golpear SQL Server[cite: 2].
            if (idSala <= 0 || idUsuario <= 0)
                throw new ArgumentException("Los identificadores de sala y usuario son requeridos para abrir la sala.");

            // Si pasa las validaciones, ordenamos a la DAL que ejecute el procedimiento.
            _salaDAL.AbrirSala(idSala, idUsuario);
        }

        // Método invocado al finalizar la jornada médica[cite: 1].
        // En SalaBLL.cs
        public void CerrarSala(int idSala)
        {
            if (idSala <= 0)
                throw new ArgumentException("El identificador de la sala es requerido.");

            _salaDAL.CerrarSala(idSala);
        }

        public void RegistrarSala(string nombreSala, string estadoSala, List<int>? idsMedicos = null)
        {
            if (string.IsNullOrWhiteSpace(nombreSala))
                throw new ArgumentException("El nombre de la sala es obligatorio.");

            if (string.IsNullOrWhiteSpace(estadoSala))
                estadoSala = "Cerrada"; // Estado por defecto

            int idSala = _salaDAL.InsertarSala(nombreSala.Trim(), estadoSala);
            if (idsMedicos != null && idSala > 0)
            {
                foreach (var idMed in idsMedicos)
                {
                    _salaDAL.AsignarSalaMedico(idSala, idMed);
                }
            }
        }

        public void RegistrarSala(string nombreSala, string estadoSala)
        {
            RegistrarSala(nombreSala, estadoSala, null);
        }

        public void EliminarSala(int idSala)
        {
            if (idSala <= 0)
                throw new ArgumentException("ID de sala inválido.");

            _salaDAL.EliminarSala(idSala);
        }

        public void AsignarSalaMedico(int idSala, int idUsuario, string descripcionAtencion)
        {
            if (idSala <= 0 || idUsuario <= 0)
                throw new ArgumentException("Debe seleccionar una sala y un profesional médico.");

            _salaDAL.AsignarSalaMedico(idSala, idUsuario, descripcionAtencion);
        }

        public void ActualizarEstadoSala(int idSala, string nuevoEstado)
        {
            if (idSala <= 0)
                throw new ArgumentException("El ID de la sala no es válido.");

            if (string.IsNullOrWhiteSpace(nuevoEstado))
                throw new ArgumentException("El nuevo estado de la sala es obligatorio.");

            _salaDAL.ActualizarEstadoSala(idSala, nuevoEstado);
        }
    }
}