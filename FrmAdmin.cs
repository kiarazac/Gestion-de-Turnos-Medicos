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
        public FrmAdmin()
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

            // pnlContenedor debe ser el nombre (propiedad Name) del panel central que arrastraste al diseño
            pnlContenedor.Controls.Add(formHijo);
            pnlContenedor.Tag = formHijo;
            formHijo.BringToFront();
            formHijo.Show();
        }

        private void btnPersonalMedico_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmGestionUsuarios());
        }

       

        private void btnSalas_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FrmSalasAdmin());
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

