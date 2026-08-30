using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmRecepcionista : Form
    {
        public FrmRecepcionista()
        {
            InitializeComponent();
        }

        // Variable para recordar qué formulario está abierto actualmente
        private Form formularioActivo = null;

        private void AbrirFormularioHijo(Form formHijo)
        {
            // Si ya hay un formulario abierto, lo cerramos para no superponerlos
            if (formularioActivo != null)
            {
                formularioActivo.Close();
            }

            formularioActivo = formHijo;

            // Configuramos el formulario hijo para que se comporte como un control interno
            formHijo.TopLevel = false;
            formHijo.FormBorderStyle = FormBorderStyle.None;
            formHijo.Dock = DockStyle.Fill;

            panelContenedor.Controls.Add(formHijo);
            panelContenedor.Tag = formHijo;
            formHijo.BringToFront();
            formHijo.Show();
        }
        private void asign_turnosEmergencia_Click(object sender, EventArgs e)
        {
            // Llamamos a la función y le pasamos una nueva instancia de tu ventana
            AbrirFormularioHijo(new FrmTurnoEmergencia());
        }
        

        private void salir_Click(object sender, EventArgs e)
        {
            // 1. Buscamos la ventana original de Login que está en la memoria y la mostramos
            Application.OpenForms["FrmLogin"].Show();

            // 2. Cerramos la ventana actual de Turnos (esta sí la cerramos por completo)
            this.Close();
        }

        private void lista_turnos_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmListaTurnos());
        }

        private void asign_turnosEspecialidad_Click_1(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmTurnoEspecialidad());
        }
    }
}