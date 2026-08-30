using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmTurnoEspecialidad : Form
    {
        public FrmTurnoEspecialidad()
        {
            InitializeComponent();
        }

        private void FrmTurnoEspecialidad_Load(object sender, EventArgs e)
        {
            // 1. Configurar calendario
            calFechaTurno.MinDate = DateTime.Today;

            // 2. Cargar las especialidades en el nuevo ComboBox
            cmbEspecialidad.Items.Add("Seleccione especialidad...");
            cmbEspecialidad.Items.Add("Cardiología");
            cmbEspecialidad.Items.Add("Pediatría");
            cmbEspecialidad.Items.Add("Traumatología");

            // Seleccionar por defecto la primera opción
            cmbEspecialidad.SelectedIndex = 0;
        }

        private void calFechaTurno_DateChanged(object sender, DateRangeEventArgs e)
        {
            cmbHorarios.Items.Clear();

            // Validar que el recepcionista haya elegido una especialidad primero
            if (cmbEspecialidad.SelectedIndex <= 0)
            {
                MessageBox.Show("Por favor, seleccione una especialidad antes de elegir la fecha.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string especialidad = cmbEspecialidad.SelectedItem.ToString();
            DateTime fechaElegida = e.Start.Date;
            DateTime hoy = DateTime.Today;

            // Simular consulta a DB: SELECT Hora FROM Turnos WHERE Especialidad = @esp AND Fecha = @fecha
            if (especialidad == "Cardiología" && fechaElegida == hoy.AddDays(1))
            {
                cmbHorarios.Items.Add("08:00");
                cmbHorarios.Items.Add("08:30");
            }
            else if (especialidad == "Cardiología" && fechaElegida == hoy.AddDays(3))
            {
                cmbHorarios.Items.Add("16:00");
            }
            else if (especialidad == "Pediatría" && fechaElegida == hoy.AddDays(2))
            {
                cmbHorarios.Items.Add("09:00");
                cmbHorarios.Items.Add("09:30");
                cmbHorarios.Items.Add("10:00");
            }
            else if (especialidad == "Traumatología" && fechaElegida == hoy.AddDays(1))
            {
                cmbHorarios.Items.Add("14:00");
            }
            else
            {
                MessageBox.Show("No hay turnos disponibles para esta fecha y especialidad.", "Sin disponibilidad", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (cmbHorarios.Items.Count > 0)
            {
                cmbHorarios.SelectedIndex = 0;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 1. Limpiamos las fechas y horarios de la selección anterior
            calFechaTurno.RemoveAllBoldedDates();
            cmbHorarios.Items.Clear();

            string especialidad = cmbEspecialidad.SelectedItem.ToString();
            DateTime hoy = DateTime.Today;

            // 2. Simular consulta a DB: ¿Qué días atiende esta especialidad?
            if (especialidad == "Cardiología")
            {
                calFechaTurno.BoldedDates = new DateTime[] { hoy.AddDays(1), hoy.AddDays(3) };
            }
            else if (especialidad == "Pediatría")
            {
                calFechaTurno.BoldedDates = new DateTime[] { hoy.AddDays(2), hoy.AddDays(5) };
            }
            else if (especialidad == "Traumatología")
            {
                calFechaTurno.BoldedDates = new DateTime[] { hoy.AddDays(1) };
            }

            // 3. Forzar al calendario a repintarse con los nuevos días en negrita
            calFechaTurno.UpdateBoldedDates();
        }
    }
}
