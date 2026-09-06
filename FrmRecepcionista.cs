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
        // 1. Declaramos la variable privada para guardar los datos del recepcionista logueado
        private ResultadosSQL.UsuarioLoginResult _usuarioActual;

        public FrmRecepcionista(ResultadosSQL.UsuarioLoginResult usuario)
        {
            InitializeComponent();

            // 2. Atrapamos el objeto que nos envió el Login y lo guardamos para esta sesión
            _usuarioActual = usuario;
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
            AbrirFormularioHijo(new FrmTurnoEmergencia());
        }

        private void asign_turnosEspecialidad_Click_1(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmTurnoEspecialidad());
        }

        private void lista_turnos_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmListaTurnos());
        }

        private void salir_Click(object sender, EventArgs e)
        {
            // 1. Buscamos la ventana original de Login que está en la memoria y la mostramos
            if (Application.OpenForms["FrmLogin"] != null)
            {
                Application.OpenForms["FrmLogin"].Show();
            }

            // 2. Cerramos la ventana actual por completo
            this.Close();
        }
    }
}