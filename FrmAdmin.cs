using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmAdmin : Form
    {
        // 1. Declaramos la variable privada para guardar los datos del usuario logueado en la memoria de este formulario
        private ResultadosSQL.UsuarioLoginResult _usuarioActual;

        public FrmAdmin(ResultadosSQL.UsuarioLoginResult usuario)
        {
            InitializeComponent();

            // 2. Atrapamos el objeto que nos envió el Login y lo guardamos
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

            // pnlContenedor debe ser el nombre del panel central
            pnlContenedor.Controls.Add(formHijo);
            pnlContenedor.Tag = formHijo;
            formHijo.BringToFront();
            formHijo.Show();
        }

        private void btnPersonalMedico_Click(object sender, EventArgs e)
        {
            // Se utiliza exclusivamente FrmGestionUsuarios2 pasando la sesión activa del administrador
            AbrirFormularioHijo(new FrmGestionUsuarios2(_usuarioActual));
        }

        private void btnSalas_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmSalasAdmin());
        }

        private void btnEspecialidades_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmGestionEspecialidades());
        }

        private void btnUsuarios2_Click(object sender, EventArgs e)
        {
            // Abre la versión 2.0 pasando la sesión activa para control de auto-eliminación
            AbrirFormularioHijo(new FrmGestionUsuarios2(_usuarioActual));
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            // 1. Buscamos la ventana original de Login que está en la memoria y la mostramos
            Application.OpenForms["FrmLogin"].Show();

            // 2. Cerramos la ventana actual por completo para liberar la memoria
            this.Close();
        }
    }
}