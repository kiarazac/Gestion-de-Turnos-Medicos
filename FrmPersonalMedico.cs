using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Gestion_de_Turnos_Medicos
{
    public partial class Pantalla_Principal_PERSONAL_MEDICO : Form
    {
        // 1. Declaramos la variable privada para guardar los datos del médico logueado
        private ResultadosSQL.UsuarioLoginResult _usuarioActual;

        public Pantalla_Principal_PERSONAL_MEDICO(ResultadosSQL.UsuarioLoginResult usuario)
        {
            InitializeComponent();

            // 2. Atrapamos el objeto que nos envió el Login y lo guardamos para usarlo en esta sesión
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

        // Evento del botón mis_Salas: abre el formulario MisSalas_PM como hijo
        private void mis_Salas_Click(object sender, EventArgs e)
        {
            // NOTA: Para listar solo las salas de este médico, deberás modificar el constructor 
            // de MisSalas_PM para que reciba al usuario, así: AbrirFormularioHijo(new MisSalas_PM(_usuarioActual));
            AbrirFormularioHijo(new MisSalas_PM());
        }

        private void lista_turnos_atención_Click(object sender, EventArgs e)
        {
            // NOTA: Para llamar pacientes y crear la historia clínica, también necesitarás los datos del médico.
            // Cuando actualices esa pantalla, lo llamarás así: AbrirFormularioHijo(new FrmListaTurnosAtencion(_usuarioActual));
            AbrirFormularioHijo(new FrmListaTurnosAtencion());
        }

        private void salir_Click(object sender, EventArgs e)
        {
            // 1. Buscamos la ventana original de Login que está en la memoria y la mostramos
            if (Application.OpenForms["FrmLogin"] != null)
            {
                Application.OpenForms["FrmLogin"].Show();
            }

            // 2. Cerramos la ventana actual de Turnos por completo para liberar la memoria
            this.Close();
        }
    }
}