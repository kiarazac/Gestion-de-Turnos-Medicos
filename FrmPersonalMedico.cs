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
        public Pantalla_Principal_PERSONAL_MEDICO()
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

        // Evento del botón mis_Salas: abre el formulario MisSalas_PM como hijo
        private void mis_Salas_Click(object sender, EventArgs e)
        {
           
            AbrirFormularioHijo(new MisSalas_PM());
        }

        

        
    }
}
