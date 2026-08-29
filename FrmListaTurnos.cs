using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmListaTurnos : Form
    {
        public FrmListaTurnos()
        {
            InitializeComponent();
        }



        private void FrmListaTurnos_Load(object sender, EventArgs e)
        {
            // Este código simula lo que hará tu base de datos en el futuro
            // El DataGridView recibe los datos en el orden exacto de las columnas que creaste

            dataGridView1.Rows.Add("E-042", "ALTA", "09:15", "En espera", "--");
            dataGridView1.Rows.Add("E-043", "ALTA", "09:20", "En espera", "--");
            dataGridView1.Rows.Add("E-044", "MEDIA", "09:25", "En espera", "--");
            dataGridView1.Rows.Add("E-045", "BAJA", "09:35", "En espera", "--");

            // Simulando el conteo de los paneles superiores
            LtotalAlta.Text = "2";
            LtotalMedia.Text = "1";
            LtotalBaja.Text = "1";

            // Cargar opciones en el selector de especialidades
            cmbEspecialidades.Items.Add("Seleccione una especialidad...");
            cmbEspecialidades.Items.Add("Cardiología");
            cmbEspecialidades.Items.Add("Pediatría");
            cmbEspecialidades.Items.Add("Traumatología");

            // Seleccionar la primera opción por defecto
            cmbEspecialidades.SelectedIndex = 0;
        }

        private void cmbEspecialidades_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 1. Limpiar la tabla antes de cargar nuevos datos
            dgvEspecialidades.Rows.Clear();

            // 2. Obtener la especialidad seleccionada
            string especialidadSeleccionada = cmbEspecialidades.SelectedItem.ToString();

            // 3. Simular la carga desde la base de datos según la selección
            if (especialidadSeleccionada == "Cardiología")
            {
                dgvEspecialidades.Rows.Add("C-012", "29/08/2026", "10:00", "En espera");
                dgvEspecialidades.Rows.Add("C-013", "29/08/2026", "10:30", "En espera");
            }
            else if (especialidadSeleccionada == "Pediatría")
            {
                dgvEspecialidades.Rows.Add("P-005", "29/08/2026", "09:15", "En espera");
            }
            else if (especialidadSeleccionada == "Traumatología")
            {
                dgvEspecialidades.Rows.Add("T-022", "29/08/2026", "11:00", "En espera");
                dgvEspecialidades.Rows.Add("T-023", "29/08/2026", "11:20", "En espera");
                dgvEspecialidades.Rows.Add("T-024", "29/08/2026", "11:40", "Llamado");
            }
        }
    }
}
