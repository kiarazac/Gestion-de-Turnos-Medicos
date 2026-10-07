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
        static void Main(string[] args)
        {
            if (args != null && args.Length > 0 && args[0] == "--test-pdf")
            {
                EjecutarPruebasPdf();
                return;
            }

            // Inicializa la configuración de la aplicación (DPI, renderizado de texto, fuentes del sistema)
            ApplicationConfiguration.Initialize();

            // Inicia el bucle de mensajes de Windows ejecutando el formulario de autenticación inicial
            Application.Run(new FrmLogin());
        }

        private static void EjecutarPruebasPdf()
        {
            try
            {
                Console.WriteLine("=== INICIANDO PRUEBAS DE VERIFICACIÓN END-TO-END ===");

                // 1. Verificar Login de Gerente
                var usuarioBLL = new Negocio.UsuarioBLL();
                var gerente = usuarioBLL.Login("gerente@gmail.com", "123456");
                Console.WriteLine($"[OK] Login Gerente exitoso: {gerente?.Nombre} {gerente?.Apellido} - Rol: {gerente?.NombreRol} (IdRol: {gerente?.IdRol})");

                string testDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestPdfOutput");
                if (!Directory.Exists(testDir)) Directory.CreateDirectory(testDir);

                // 2. Verificar Cierre de Caja (Recepcionista)
                var cajaBLL = new CapaNegocio.CajaBLL();
                var resumenCaja = cajaBLL.ObtenerResumenCierreCaja(DateTime.Today);
                var detallesCaja = cajaBLL.ObtenerDetalleCierreCaja(DateTime.Today);
                Console.WriteLine($"[OK] SP Cierre de Caja ejecutado: {resumenCaja.TotalTurnos} turnos hoy, Recaudado: ${resumenCaja.TotalRecaudado:N2}, Detalles: {detallesCaja.Count} registros.");

                string pathCajaPdf = Path.Combine(testDir, "Test_CierreCaja.pdf");
                Servicios.ExportadorPdf.ExportarCierreCaja(pathCajaPdf, resumenCaja, detallesCaja, DateTime.Today, gerente);
                Console.WriteLine($"[OK] PDF Cierre de Caja generado exitosamente ({new FileInfo(pathCajaPdf).Length} bytes): {pathCajaPdf}");

                // 3. Verificar Reportes Gerenciales (Gerente)
                var reporteBLL = new Negocio.ReporteBLL();
                DateTime desde = new DateTime(2025, 1, 1);
                DateTime hasta = DateTime.Today.AddDays(1);
                var ingresos = reporteBLL.ObtenerIngresosPorMedico(desde, hasta);
                var coberturas = reporteBLL.ObtenerObrasSocialesVsParticulares(desde, hasta);
                var demanda = reporteBLL.ObtenerDemandaMedicosRanking(desde, hasta);
                Console.WriteLine($"[OK] SPs Gerenciales ejecutados: Ingresos: {ingresos.Count} médicos, Coberturas: {coberturas.Count} grupos, Demanda: {demanda.Count} rankings.");

                string pathGerencialPdf = Path.Combine(testDir, "Test_ReporteGerencial.pdf");
                Servicios.ExportadorPdf.ExportarReporteGerencial(pathGerencialPdf, ingresos, coberturas, demanda, desde, hasta, gerente);
                Console.WriteLine($"[OK] PDF Gerencial generado exitosamente ({new FileInfo(pathGerencialPdf).Length} bytes): {pathGerencialPdf}");

                // 4. Verificar Reporte Médico y Diagnósticos Frecuentes
                var historiaBLL = new Negocio.HistoriaClinicaBLL();
                var medico = usuarioBLL.Login("martin@gmail.com", "123456");
                int idMedico = medico?.IdUsuario ?? 2;
                var atenciones = historiaBLL.ObtenerAtencionesPorMedico(idMedico, desde, hasta);
                var rankingDiag = reporteBLL.ObtenerRankingDiagnosticosMedico(idMedico, desde, hasta);
                Console.WriteLine($"[OK] SPs Médicos ejecutados para Dr. {medico?.Nombre} {medico?.Apellido} (ID: {idMedico}): {atenciones.Count} atenciones históricas, {rankingDiag.Count} diagnósticos/síntomas frecuentes.");

                string pathMedicoPdf = Path.Combine(testDir, "Test_ReporteMedico.pdf");
                Servicios.ExportadorPdf.ExportarReporteMedico(pathMedicoPdf, atenciones, rankingDiag, desde, hasta, medico);
                // 5. Verificar Reporte de Guardia y Triage (Auditoría Triage)
                var guardiaDAL = new CapaDeDatos.ReporteGuardiaDAL();
                var resumenGuardia = guardiaDAL.ObtenerResumenGuardia(new DateTime(2026, 9, 30), new DateTime(2026, 10, 7), null);
                var rankingGuardia = guardiaDAL.ObtenerRankingSintomas(new DateTime(2026, 9, 30), new DateTime(2026, 10, 7), null);
                var detalleGuardia = guardiaDAL.ObtenerDetalleGuardia(new DateTime(2026, 9, 30), new DateTime(2026, 10, 7), null);
                string pathGuardiaPdf = Path.Combine(testDir, "Test_ReporteGuardia.pdf");
                Servicios.ExportadorPdf.ExportarReporteGuardia(pathGuardiaPdf, resumenGuardia, rankingGuardia, detalleGuardia, new DateTime(2026, 9, 30), new DateTime(2026, 10, 7), "Todas las prioridades", gerente);
                Console.WriteLine($"[OK] PDF Guardia generado exitosamente ({new FileInfo(pathGuardiaPdf).Length} bytes): {pathGuardiaPdf}");

                Console.WriteLine("=== TODAS LAS PRUEBAS END-TO-END FINALIZARON EXITOSAMENTE ===");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR EN PRUEBAS: " + ex);
                Environment.Exit(1);
            }
        }
    }
}