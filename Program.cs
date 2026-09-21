namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Clase principal que define el punto de entrada de la aplicación Windows Forms.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// Configura el entorno de alta resolución (DPI), estilos visuales e inicia el ciclo de vida del formulario de Login.
        /// </summary>
        /// <remarks>
        /// El atributo [STAThread] (Single-Threaded Apartment) es mandatorio para aplicaciones Windows Forms
        /// para permitir la correcta interacción con componentes COM del sistema operativo (cuadros de diálogo de archivos, portapapeles, etc.).
        /// </remarks>
        [STAThread]
        static void Main()
        {
            // Inicializa la configuración de la aplicación (DPI, renderizado de texto, fuentes del sistema)
            ApplicationConfiguration.Initialize();

            // Inicia el bucle de mensajes de Windows ejecutando el formulario de autenticación inicial
            Application.Run(new FrmLogin());
        }
    }
}