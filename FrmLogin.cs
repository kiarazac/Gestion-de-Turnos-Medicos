using System.Text.RegularExpressions; //Librería para expresiones regulares y validación de datos

namespace Gestion_de_Turnos_Medicos
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Guardamos los valores de las cajas de texto
            string nombre = txtNombre.Text;
            string apellido = txtApellido.Text;
            string email = txtCorreo.Text;
            string contrasena = txtContraseña.Text;

            // 2. Validamos campos vacíos 
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(contrasena))
            {
                MessageBox.Show("Por favor, completa todos los campos.", "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Validamos el Nombre usando nuestra clase para validaciones
            if (!Validaciones.EsNombreValido(nombre))
            {
                MessageBox.Show("El nombre no puede contener números ni símbolos especiales.", "Nombre inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Validaciones.EsNombreValido(apellido))
            {
                MessageBox.Show("El apellido no puede contener números ni símbolos especiales.", "Apellido inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 4. Validamos el Correo 
            if (!Validaciones.EsEmailValido(email))
            {
                MessageBox.Show("Por favor, ingresa un correo válido (@gmail.com, @hotmail.com o @outlook.com).", "Correo inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 5. Si todo está correcto
            MessageBox.Show("¡Datos correctos! Evaluando rol del usuario...", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Aquí irá la lógica para abrir FrmAdmin, FrmRecepcion o FrmMedico
            // --- CÓDIGO PARA CAMBIAR DE VENTANA ---

            // 1. Creamos una "instancia" (una copia lista para usarse) de tu nueva ventana
            FrmRecepcionista ventanaTurnos = new FrmRecepcionista();

            // 2. Le decimos a esa nueva ventana que se muestre en pantalla
            ventanaTurnos.Show();

            // 3. Ocultamos la ventana actual (el Login) para que no moleste en el fondo
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
    
