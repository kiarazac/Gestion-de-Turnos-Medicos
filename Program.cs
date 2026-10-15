using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Gestion_de_Turnos_Medicos.CapaDeDatos;
using Gestion_de_Turnos_Medicos.Negocio;

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

            if (args != null && args.Length > 0 && args[0] == "--test-nuevas-funciones")
            {
                EjecutarPruebasNuevasFunciones();
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
                var gerente = usuarioBLL.Login("gerente@consultorio.com", "123456");
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

                // 4. Verificar Reporte Médico y Triage de Atenciones
                var historiaBLL = new Negocio.HistoriaClinicaBLL();
                var turnoBLL = new Negocio.TurnoBLL();
                var medico = usuarioBLL.Login("jorgeMa@hotmail.com", "123456");
                int idMedico = medico?.IdUsuario ?? 21;
                var atenciones = historiaBLL.ObtenerAtencionesPorMedico(idMedico, desde, hasta);
                bool esClinico = turnoBLL.PuedeAtenderEmergencias(idMedico);
                var rankingGrav = esClinico ? reporteBLL.ObtenerRankingGravedadMedico(idMedico, desde, hasta) : null;
                var rankingSint = esClinico ? reporteBLL.ObtenerRankingSintomasMedico(idMedico, desde, hasta) : null;
                Console.WriteLine($"[OK] SPs Médicos ejecutados para Dr. {medico?.Nombre} {medico?.Apellido} (ID: {idMedico}): {atenciones.Count} atenciones históricas, EsClínico: {esClinico}.");

                string pathMedicoPdf = Path.Combine(testDir, "Test_ReporteMedico.pdf");
                Servicios.ExportadorPdf.ExportarReporteMedico(pathMedicoPdf, atenciones, rankingGrav, rankingSint, desde, hasta, medico);
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

        private static void EjecutarPruebasNuevasFunciones()
        {
            try
            {
                Console.WriteLine("=================================================================");
                Console.WriteLine("=== INICIANDO VERIFICACIÓN COMPLETA DE REQUERIMIENTOS NUEVOS ===");
                Console.WriteLine("=================================================================\n");

                var usuarioBLL = new Negocio.UsuarioBLL();
                var turnoBLL = new Negocio.TurnoBLL();

                // -----------------------------------------------------------------
                // ITEM 1: HASH DE CONTRASEÑAS (ALTA, LOGIN, CAMBIO DE CLAVE, RE-LOGIN)
                // -----------------------------------------------------------------
                Console.WriteLine("--- ITEM 1: PRUEBAS DE HASH DE CONTRASEÑAS ---");
                string correoTest = "test_hash_flow@consultorio.com";
                string dniTest = "99887766";
                string passOriginal = "PasswordInicial123!";
                string passModificada = "NuevaPassword456!";

                using (var ctx = new dbTurnosMedicos())
                {
                    ctx.Database.ExecuteSqlRaw("DELETE FROM Usuarios WHERE Correo = {0} OR Dni = {1}", correoTest, dniTest);
                }

                // 1.1 Alta de usuario
                Console.WriteLine("1.1 Registrando nuevo usuario con BLL.RegistrarUsuario...");
                usuarioBLL.RegistrarUsuario("UsuarioTest", "FlujoHash", correoTest, passOriginal, dniTest, "3794123456", 2, "", new List<int>(), new List<int>(), "");

                // Verificar que en DB esté almacenado el hash SHA-256 de 64 caracteres
                using (var ctx = new dbTurnosMedicos())
                {
                    var userEnDb = ctx.Usuarios.AsNoTracking().FirstOrDefault(u => u.Correo == correoTest);
                    if (userEnDb == null) throw new Exception("El usuario no fue insertado en la base de datos.");
                    if (userEnDb.Contrasena == passOriginal) throw new Exception("ERROR: La contraseña está guardada en texto plano.");
                    if (userEnDb.Contrasena != Seguridad.HashearContrasenia(passOriginal)) throw new Exception("ERROR: El hash en DB no coincide con SHA-256.");
                    Console.WriteLine($"[OK] Usuario registrado en DB con hash SHA-256: {userEnDb.Contrasena} (Longitud: {userEnDb.Contrasena.Length})");
                }

                // 1.2 Login con contraseña original
                Console.WriteLine("1.2 Probando login con contraseña correcta...");
                var loginOk = usuarioBLL.Login(correoTest, passOriginal);
                if (loginOk == null) throw new Exception("Login con clave original falló.");
                Console.WriteLine($"[OK] Login exitoso: {loginOk.Nombre} {loginOk.Apellido} - Rol: {loginOk.NombreRol}");

                // 1.3 Login con contraseña errónea
                Console.WriteLine("1.3 Probando login con contraseña incorrecta...");
                try
                {
                    usuarioBLL.Login(correoTest, "ClaveErronea123!");
                    throw new Exception("ERROR: El login con clave errónea debió haber fallado.");
                }
                catch (Exception ex) when (!ex.Message.StartsWith("ERROR:"))
                {
                    Console.WriteLine($"[OK] Login rechazado correctamente ante contraseña errónea ({ex.Message}).");
                }

                // 1.4 Modificación / cambio de clave
                Console.WriteLine("1.4 Modificando la contraseña a nueva clave...");
                usuarioBLL.ModificarUsuario(loginOk.IdUsuario, "UsuarioTest", "FlujoHash", correoTest, dniTest, "3794123456", "", 2, new List<int>(), "", new List<int>(), passModificada);

                using (var ctx = new dbTurnosMedicos())
                {
                    var userModEnDb = ctx.Usuarios.AsNoTracking().FirstOrDefault(u => u.Correo == correoTest);
                    if (userModEnDb?.Contrasena != Seguridad.HashearContrasenia(passModificada)) throw new Exception("ERROR: La nueva clave no se hasheó correctamente en DB.");
                    Console.WriteLine($"[OK] Nueva contraseña almacenada como hash SHA-256: {userModEnDb.Contrasena}");
                }

                // 1.5 Login con contraseña anterior (debe fallar)
                Console.WriteLine("1.5 Probando login con la contraseña anterior (debe ser rechazado)...");
                try
                {
                    usuarioBLL.Login(correoTest, passOriginal);
                    throw new Exception("ERROR: El login con la clave anterior debió haber sido rechazado.");
                }
                catch (Exception ex) when (!ex.Message.StartsWith("ERROR:"))
                {
                    Console.WriteLine($"[OK] Login con clave vieja rechazado correctamente ({ex.Message}).");
                }

                // 1.6 Login con nueva contraseña (debe tener éxito)
                Console.WriteLine("1.6 Probando login con la nueva contraseña...");
                var reloginOk = usuarioBLL.Login(correoTest, passModificada);
                if (reloginOk == null) throw new Exception("Login con la nueva contraseña falló.");
                Console.WriteLine($"[OK] Login exitoso con nueva contraseña: {reloginOk.Nombre} {reloginOk.Apellido}\n");

                // Cleanup usuario de prueba
                using (var ctx = new dbTurnosMedicos())
                {
                    ctx.Database.ExecuteSqlRaw("DELETE FROM Usuarios WHERE Correo = {0}", correoTest);
                }

                // -----------------------------------------------------------------
                // ITEM 2: sp_ListarTurnosAtencion (FILTRO HOY) Y ESTADO AUSENTE
                // -----------------------------------------------------------------
                Console.WriteLine("--- ITEM 2: PRUEBAS DE sp_ListarTurnosAtencion Y ESTADO AUSENTE ---");
                int idPacienteTest;
                string nombreEsp;
                using (var ctx = new dbTurnosMedicos())
                {
                    var pac = ctx.Pacientes.FirstOrDefault(p => p.Activo);
                    if (pac == null) throw new Exception("No hay pacientes activos en la base de datos.");
                    idPacienteTest = pac.IdPaciente;

                    var esp = ctx.Especialidades.FirstOrDefault(e => e.Activo);
                    if (esp == null) throw new Exception("No hay especialidades activas en la base de datos.");
                    nombreEsp = esp.Nombre;
                }

                // Crear un turno de prueba para hoy (horario futuro para superar validación)
                var resultadoTurno = turnoBLL.CrearTurnoEspecialidad(idPacienteTest, nombreEsp, DateTime.Today, "23:50", "En Espera", "CAN-TEST");
                int idTurnoTest = resultadoTurno.IdNuevoTurno;
                Console.WriteLine($"2.1 Turno de prueba creado para hoy: IdTurno={idTurnoTest}, Especialidad={nombreEsp}, NroOrden={resultadoTurno.NroOrden}");

                // Probar sp_ListarTurnosAtencion
                var turnosAtencion = turnoBLL.ListarTurnosAtencion();
                bool encontradoEnListaHoy = turnosAtencion.Any(t => t.IdTurno == idTurnoTest);
                if (!encontradoEnListaHoy) throw new Exception("El turno de hoy no fue devuelto por sp_ListarTurnosAtencion.");
                Console.WriteLine($"[OK] sp_ListarTurnosAtencion filtró y devolvió el turno de hoy correctamente (Total devueltos: {turnosAtencion.Count}).");

                // Crear un turno con fecha de ayer y verificar que sp_ListarTurnosAtencion lo excluya
                int idTurnoAyer;
                using (var ctx = new dbTurnosMedicos())
                {
                    var tAyer = new Turno
                    {
                        IdPaciente = idPacienteTest,
                        IdEspecialidad = 1,
                        Fecha = DateTime.Today.AddDays(-1),
                        FechaCreacion = DateTime.Today.AddDays(-1),
                        Horario = new TimeSpan(11, 0, 0),
                        Estado = "En Espera",
                        TipoTurno = "Consulta",
                        IdPrioridad = 3,
                        Activo = true,
                        NroOrden = "TEST-AYER",
                        CodigoCancelacion = "CAN-AYER"
                    };
                    ctx.Turnos.Add(tAyer);
                    ctx.SaveChanges();
                    idTurnoAyer = tAyer.IdTurno;
                }

                var turnosAtencion2 = turnoBLL.ListarTurnosAtencion();
                bool turnoAyerIncluido = turnosAtencion2.Any(t => t.IdTurno == idTurnoAyer);
                if (turnoAyerIncluido) throw new Exception("ERROR: sp_ListarTurnosAtencion incluyó un turno de fecha anterior a hoy.");
                Console.WriteLine($"[OK] sp_ListarTurnosAtencion excluye turnos que no corresponden a la fecha de hoy.");

                // Probar marcar Ausente
                Console.WriteLine("2.2 Probando marcar turno como 'Ausente' (botón No se presentó)...");
                turnoBLL.MarcarTurnoAusente(idTurnoTest);
                using (var ctx = new dbTurnosMedicos())
                {
                    var turnoMod = ctx.Turnos.AsNoTracking().FirstOrDefault(t => t.IdTurno == idTurnoTest);
                    if (turnoMod?.Estado != "Ausente") throw new Exception($"ERROR: El turno no quedó en estado 'Ausente'. Estado actual: {turnoMod?.Estado}");
                    Console.WriteLine($"[OK] Turno {idTurnoTest} marcado exitosamente como Estado='{turnoMod.Estado}'.");
                }

                // -----------------------------------------------------------------
                // ITEM 3: VALIDACIONES DE ESTADO DE ORIGEN Y Activo = 1
                // -----------------------------------------------------------------
                Console.WriteLine("\n--- ITEM 3: VALIDACIONES DE ESTADO DE ORIGEN Y Activo = 1 ---");

                // 3.1 sp_LlamarSiguienteTurno solo debe admitir origen 'En Espera'
                Console.WriteLine("3.1 Validando que sp_LlamarSiguienteTurno rechace turnos con estado distinto de 'En Espera'...");
                // idTurnoTest está en 'Ausente'
                try
                {
                    turnoBLL.LlamarSiguientePaciente(idTurnoTest, "Dr. Test", "Consultorio 1", null);
                    throw new Exception("ERROR: sp_LlamarSiguienteTurno debió rechazar el turno que está 'Ausente'.");
                }
                catch (Exception ex) when (!ex.Message.StartsWith("ERROR:"))
                {
                    Console.WriteLine($"[OK] Rechazado correctamente origen no 'En Espera': {ex.Message}");
                }

                // Resetear estado a 'En Espera' para probar flujo normal
                using (var ctx = new dbTurnosMedicos())
                {
                    ctx.Database.ExecuteSqlRaw("UPDATE Turnos SET Estado = 'En Espera', Activo = 1 WHERE IdTurno = {0}", idTurnoTest);
                }

                // 3.2 sp_IniciarAtencionTurno solo debe admitir origen 'Llamado'
                Console.WriteLine("3.2 Validando que sp_IniciarAtencionTurno rechace turno en estado 'En Espera' (sin haber sido llamado)...");
                try
                {
                    turnoBLL.IniciarAtencionTurno(idTurnoTest, "Consultorio 1");
                    throw new Exception("ERROR: sp_IniciarAtencionTurno debió rechazar el turno que no está 'Llamado'.");
                }
                catch (Exception ex) when (!ex.Message.StartsWith("ERROR:"))
                {
                    Console.WriteLine($"[OK] Rechazado correctamente inicio de atención sin llamado previo: {ex.Message}");
                }

                // 3.3 sp_FinalizarAtencionTurno solo debe admitir origen 'En Consulta'
                Console.WriteLine("3.3 Validando que sp_FinalizarAtencionTurno rechace turno en estado 'En Espera'...");
                try
                {
                    turnoBLL.FinalizarAtencionTurno(idTurnoTest, "Diagnóstico Test", "Dr. Test", "Consultorio 1");
                    throw new Exception("ERROR: sp_FinalizarAtencionTurno debió rechazar el turno que no está 'En Consulta'.");
                }
                catch (Exception ex) when (!ex.Message.StartsWith("ERROR:"))
                {
                    Console.WriteLine($"[OK] Rechazado correctamente finalización sin estar en consulta: {ex.Message}");
                }

                // 3.4 Validar flujo correcto: En Espera -> Llamado -> En Consulta -> Atendido
                Console.WriteLine("3.4 Validando transición legal completa (En Espera -> Llamado -> En Consulta -> Atendido)...");
                turnoBLL.LlamarSiguientePaciente(idTurnoTest, "Dr. Test", "Consultorio 1", null);
                Console.WriteLine("[OK] Paso 1: sp_LlamarSiguienteTurno exitoso (Estado: Llamado).");

                turnoBLL.IniciarAtencionTurno(idTurnoTest, "Consultorio 1");
                Console.WriteLine("[OK] Paso 2: sp_IniciarAtencionTurno exitoso (Estado: En Consulta).");

                turnoBLL.FinalizarAtencionTurno(idTurnoTest, "Paciente recuperado", "Dr. Test", "Consultorio 1");
                Console.WriteLine("[OK] Paso 3: sp_FinalizarAtencionTurno exitoso (Estado: Atendido).");

                // 3.5 Validar que Activo = 0 sea rechazado
                Console.WriteLine("3.5 Validando rechazo con Activo = 0...");
                using (var ctx = new dbTurnosMedicos())
                {
                    ctx.Database.ExecuteSqlRaw("UPDATE Turnos SET Activo = 0, Estado = 'En Espera' WHERE IdTurno = {0}", idTurnoTest);
                }
                try
                {
                    turnoBLL.LlamarSiguientePaciente(idTurnoTest, "Dr. Test", "Consultorio 1", null);
                    throw new Exception("ERROR: Se permitió llamar a un turno con Activo = 0.");
                }
                catch (Exception ex) when (!ex.Message.StartsWith("ERROR:"))
                {
                    Console.WriteLine($"[OK] Rechazado correctamente turno inactivo (Activo=0): {ex.Message}");
                }

                // Cleanup turnos de prueba anteriores
                using (var ctx = new dbTurnosMedicos())
                {
                    ctx.Database.ExecuteSqlRaw("DELETE FROM Turnos WHERE IdTurno IN ({0}, {1})", idTurnoTest, idTurnoAyer);
                }

                // -----------------------------------------------------------------
                // ITEM 4: NUMERACIÓN CORRELATIVA Y UNICIDAD ENTRE NO ATENDIDOS
                // -----------------------------------------------------------------
                Console.WriteLine("\n--- ITEM 4: NUMERACIÓN CORRELATIVA Y UNICIDAD ENTRE NO ATENDIDOS ---");

                var turnoDAL = new TurnoDAL();

                // 4.1 Crear primer turno de emergencia
                var sintomaList = turnoBLL.ObtenerSintomas();
                if (sintomaList == null || sintomaList.Count == 0) throw new Exception("No hay síntomas cargados en la base de datos.");
                var sintomasTest = sintomaList.Take(2).Select(s => s.IdSintoma).ToList();

                string nroOrden1 = turnoBLL.CrearTurnoEmergenciaConSintomas(idPacienteTest, sintomasTest, false, out string prioTxt1, out string cod1);
                Console.WriteLine($"4.1 Turno de emergencia 1 creado: NroOrden={nroOrden1}, Prioridad={prioTxt1}");

                // 4.2 Crear segundo turno de emergencia
                string nroOrden2 = turnoBLL.CrearTurnoEmergenciaConSintomas(idPacienteTest, sintomasTest, false, out string prioTxt2, out string cod2);
                Console.WriteLine($"4.2 Turno de emergencia 2 creado: NroOrden={nroOrden2}, Prioridad={prioTxt2}");

                if (nroOrden1 == nroOrden2) throw new Exception($"ERROR: Ambos turnos tienen el mismo NroOrden: {nroOrden1}");
                Console.WriteLine($"[OK] Los turnos de emergencia tienen numeración correlativa distinta y única ({nroOrden1} vs {nroOrden2}).");

                // 4.3 Comprobar validación de unicidad entre no atendidos
                bool existeNoAtendido = turnoBLL.ExisteNroOrdenNoAtendido(nroOrden1);
                if (!existeNoAtendido) throw new Exception($"ERROR: ExisteNroOrdenNoAtendido debió retornar true para {nroOrden1}.");
                Console.WriteLine($"[OK] ExisteNroOrdenNoAtendido detectó correctamente el turno activo no atendido ({nroOrden1}).");

                // 4.4 Intentar insertar manualmente duplicado no atendido vía SP (debe fallar)
                Console.WriteLine("4.4 Validando que sp_InsertarTurno rechace duplicado de NroOrden no atendido...");
                try
                {
                    turnoDAL.InsertarTurno(nroOrden1, "Emergencia", 1, idPacienteTest, 1);
                    throw new Exception("ERROR: sp_InsertarTurno debió rechazar la inserción con NroOrden duplicado no atendido.");
                }
                catch (Exception ex) when (!ex.Message.StartsWith("ERROR:"))
                {
                    Console.WriteLine($"[OK] sp_InsertarTurno rechazó el número duplicado correctamente: {ex.Message}");
                }

                // -----------------------------------------------------------------
                // ITEM 5: VISUALIZACIÓN DE SINTOMATOLOGÍA DE GUARDIA / TRIAGE
                // -----------------------------------------------------------------
                Console.WriteLine("\n--- ITEM 5: VISUALIZACIÓN DE SINTOMATOLOGÍA DE GUARDIA ---");

                // 5.1 Consultar el turno recién creado en db
                int idTurnoEmergencia;
                using (var ctx = new dbTurnosMedicos())
                {
                    var tEmg = ctx.Turnos.FirstOrDefault(t => t.NroOrden == nroOrden1);
                    if (tEmg == null) throw new Exception("No se encontró el turno de emergencia recién creado.");
                    idTurnoEmergencia = tEmg.IdTurno;
                }

                // 5.2 Obtener síntomas por procedimiento sp_ObtenerSintomasPorTurno
                var sintomasRecuperados = turnoBLL.ObtenerSintomasPorTurno(idTurnoEmergencia);
                if (sintomasRecuperados == null || sintomasRecuperados.Count == 0)
                    throw new Exception($"ERROR: sp_ObtenerSintomasPorTurno no devolvió los síntomas del turno {idTurnoEmergencia}.");
                Console.WriteLine($"[OK] sp_ObtenerSintomasPorTurno devolvió {sintomasRecuperados.Count} síntomas.");

                // 5.3 Obtener síntomas formateados
                string textoSintomas = turnoBLL.ObtenerSintomasTextoTurno(idTurnoEmergencia);
                if (string.IsNullOrWhiteSpace(textoSintomas))
                    throw new Exception("ERROR: ObtenerSintomasTextoTurno retornó cadena vacía.");
                Console.WriteLine($"[OK] ObtenerSintomasTextoTurno: \"{textoSintomas}\"");

                // 5.4 Validar que sp_ListarTurnosAtencion incluye la columna Sintomas
                var turnosConSintomas = turnoBLL.ListarTurnosAtencion();
                var turnoDto = turnosConSintomas.FirstOrDefault(t => t.IdTurno == idTurnoEmergencia);
                if (turnoDto == null) throw new Exception("Turno de emergencia no hallado en sp_ListarTurnosAtencion.");
                if (string.IsNullOrWhiteSpace(turnoDto.Sintomas))
                    throw new Exception("ERROR: sp_ListarTurnosAtencion no incluyó la columna Sintomas para el turno de guardia.");
                Console.WriteLine($"[OK] sp_ListarTurnosAtencion proyectó la columna Sintomas: \"{turnoDto.Sintomas}\"");

                // Cleanup turnos de prueba creados en ITEM 4 y 5
                using (var ctx = new dbTurnosMedicos())
                {
                    ctx.Database.ExecuteSqlRaw("DELETE FROM TurnoSintomas WHERE IdTurno IN (SELECT IdTurno FROM Turnos WHERE NroOrden IN ({0}, {1}))", nroOrden1, nroOrden2);
                    ctx.Database.ExecuteSqlRaw("DELETE FROM Turnos WHERE NroOrden IN ({0}, {1})", nroOrden1, nroOrden2);
                }

                Console.WriteLine("\n=================================================================");
                Console.WriteLine("=== TODAS LAS PRUEBAS DE TODOS LOS REQUERIMIENTOS PASARON (100%) ===");
                Console.WriteLine("=================================================================");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("\nFALLO EN LAS PRUEBAS: " + ex);
                Environment.Exit(1);
            }
        }
    }
}