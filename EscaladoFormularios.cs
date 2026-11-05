using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Adapta los formularios hijos embebidos en los paneles contenedores (Admin, Gerente, Recepción, Personal Médico)
    /// al tamaño de pantalla disponible: escala posiciones, tamaños y fuentes de forma proporcional al espacio del contenedor
    /// y se reajusta cuando la ventana cambia de tamaño.
    /// </summary>
    public static class EscaladoFormularios
    {
        /// <summary>
        /// Embebe <paramref name="hijo"/> en <paramref name="contenedor"/> y lo escala desde su tamaño de diseño al espacio disponible.
        /// </summary>
        /// <param name="hijo">Formulario a mostrar dentro del contenedor.</param>
        /// <param name="contenedor">Panel central del formulario principal.</param>
        public static void Embeber(Form hijo, Control contenedor)
        {
            Size tamañoDiseño = hijo.ClientSize; // Se toma antes de quitar el borde, que agranda el área cliente
            hijo.TopLevel = false;
            hijo.WindowState = FormWindowState.Normal; // Maximizado ya ocuparía el contenedor y no se escalaría su contenido
            hijo.FormBorderStyle = FormBorderStyle.None;
            hijo.MinimumSize = Size.Empty; // Un mínimo de diseño impediría achicarlo en pantallas chicas
            hijo.ClientSize = tamañoDiseño;
            hijo.Location = Point.Empty;

            contenedor.Controls.Add(hijo);
            hijo.BringToFront();
            hijo.Show(); // El Load corre con el tamaño de diseño; luego se escala lo que haya creado
            hijo.AutoScaleMode = AutoScaleMode.None; // Si no, cambiar la fuente reescalaría los controles por segunda vez

            Ajustar(hijo, contenedor);

            EventHandler alRedimensionar = (s, e) => Ajustar(hijo, contenedor);
            contenedor.Resize += alRedimensionar;
            hijo.FormClosed += (s, e) => contenedor.Resize -= alRedimensionar;
        }

        /// <summary>
        /// Escala el formulario desde su tamaño actual al área cliente del contenedor y lo deja acoplado (Dock = Fill).
        /// </summary>
        private static void Ajustar(Form hijo, Control contenedor)
        {
            Size destino = contenedor.ClientSize;
            if (hijo.IsDisposed || destino.Width <= 0 || destino.Height <= 0)
                return; // Ventana minimizada: se conserva la escala actual

            // Se desacopla para que el tamaño actual refleje la escala vigente y los anclajes no se apliquen dos veces
            hijo.Dock = DockStyle.None;
            Size actual = hijo.ClientSize;

            if (actual.Width > 0 && actual.Height > 0 && actual != destino)
            {
                float fx = (float)destino.Width / actual.Width;
                float fy = (float)destino.Height / actual.Height;

                hijo.SuspendLayout();
                hijo.Scale(new SizeF(fx, fy));
                EscalarFuentes(hijo, Math.Min(fx, fy)); // Factor uniforme para que el texto no se deforme
                hijo.ResumeLayout(true);
            }

            hijo.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Escala la fuente del control y, recursivamente, las fuentes propias de sus hijos
        /// (los que heredan la fuente del contenedor la toman ya escalada).
        /// </summary>
        private static void EscalarFuentes(Control control, float factor)
        {
            control.Font = Escalar(control.Font, factor);
            EscalarFuentesGrilla(control, factor);
            EscalarFuentesHijos(control, factor);
        }

        private static void EscalarFuentesHijos(Control control, float factor)
        {
            foreach (Control hijo in control.Controls)
            {
                if (!ReferenceEquals(hijo.Font, control.Font))
                    hijo.Font = Escalar(hijo.Font, factor);

                EscalarFuentesGrilla(hijo, factor);
                EscalarFuentesHijos(hijo, factor);
            }
        }

        /// <summary>
        /// Las grillas definen fuentes en sus estilos de celda, que no heredan la fuente del control.
        /// </summary>
        private static void EscalarFuentesGrilla(Control control, float factor)
        {
            if (control is not DataGridView grilla)
                return;

            if (grilla.DefaultCellStyle.Font != null)
                grilla.DefaultCellStyle.Font = Escalar(grilla.DefaultCellStyle.Font, factor);
            if (grilla.ColumnHeadersDefaultCellStyle.Font != null)
                grilla.ColumnHeadersDefaultCellStyle.Font = Escalar(grilla.ColumnHeadersDefaultCellStyle.Font, factor);

            grilla.RowTemplate.Height = Math.Max(18, (int)Math.Round(grilla.RowTemplate.Height * factor));
            grilla.ColumnHeadersHeight = Math.Max(18, (int)Math.Round(grilla.ColumnHeadersHeight * factor));
        }

        private static Font Escalar(Font fuente, float factor)
        {
            return new Font(fuente.FontFamily, Math.Max(6f, fuente.Size * factor), fuente.Style, fuente.Unit);
        }
    }
}
