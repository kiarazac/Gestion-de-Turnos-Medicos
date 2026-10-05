using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gestion_de_Turnos_Medicos.Negocio;
using Gestion_de_Turnos_Medicos.ResultadosSQL;
using Gestion_de_Turnos_Medicos.Servicios;

namespace Gestion_de_Turnos_Medicos
{
    /// <summary>
    /// Formulario de consulta histórica y reportería clínica para el profesional médico.
    /// Permite filtrar, auditar y exportar todas las consultas y diagnósticos realizados por el médico autenticado.
    /// </summary>
    public partial class FrmMisAtenciones : Form
    {
        private readonly HistoriaClinicaBLL _historiaClinicaBLL = new HistoriaClinicaBLL();
        private readonly UsuarioLoginResult? _usuarioActual;

        private List<AtencionMedicoDTO> _todasLasAtenciones = new List<AtencionMedicoDTO>();

        public FrmMisAtenciones() : this(null)
        {
        }

        public FrmMisAtenciones(UsuarioLoginResult? usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;

            this.Load += FrmMisAtenciones_Load;
            this.cmbPeriodo.SelectedIndexChanged += CmbPeriodo_SelectedIndexChanged;
            this.btnFiltrar.Click += BtnFiltrar_Click;
            this.btnExportar.Click += BtnExportar_Click;
            this.txtBuscar.TextChanged += TxtBuscar_TextChanged;
            this.dgvAtenciones.SelectionChanged += DgvAtenciones_SelectionChanged;
        }

        private void FrmMisAtenciones_Load(object? sender, EventArgs e)
        {
            ConfigurarGrilla();

            if (_usuarioActual != null)
            {
                lblTituloHeader.Text = $"Mis Atenciones Realizadas — Dr. {_usuarioActual.Nombre} {_usuarioActual.Apellido}";
                lblSubtituloHeader.Text = "Consultas médicas, diagnósticos y recetas prescriptas en consultorio";
            }

            // Inicializar fechas predeterminadas (Últimos 7 días)
            cmbPeriodo.SelectedIndex = 1; // "Últimos 7 días"
            CargarAtencionesDesdeBD();
        }

        private void ConfigurarGrilla()
        {
            dgvAtenciones.AutoGenerateColumns = false;
            dgvAtenciones.Columns.Clear();
            dgvAtenciones.RowHeadersVisible = false;
            dgvAtenciones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAtenciones.MultiSelect = false;
            dgvAtenciones.ReadOnly = true;
            dgvAtenciones.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);

            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFecha",
                HeaderText = "Fecha / Hora",
                DataPropertyName = "Fecha",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" },
                Width = 125
            });

            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTurno",
                HeaderText = "N° Turno",
                DataPropertyName = "NroOrden",
                Width = 85
            });

            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPaciente",
                HeaderText = "Paciente",
                DataPropertyName = "PacienteCompleto",
                Width = 180
            });

            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDni",
                HeaderText = "DNI",
                DataPropertyName = "DniPaciente",
                Width = 90
            });

            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colObraSocial",
                HeaderText = "Cobertura",
                DataPropertyName = "ObraSocial",
                Width = 120
            });

            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTipo",
                HeaderText = "Modalidad",
                DataPropertyName = "TipoTurno",
                Width = 95
            });

            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDiagnostico",
                HeaderText = "Diagnóstico",
                DataPropertyName = "DiagRapido",
                Width = 200
            });

            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSala",
                HeaderText = "Consultorio",
                DataPropertyName = "NombreSala",
                Width = 110
            });
        }

        private void CmbPeriodo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            DateTime hoy = DateTime.Today;

            switch (cmbPeriodo.SelectedIndex)
            {
                case 0: // Hoy
                    dtpDesde.Value = hoy;
                    dtpHasta.Value = hoy;
                    dtpDesde.Enabled = false;
                    dtpHasta.Enabled = false;
                    break;

                case 1: // Últimos 7 días
                    dtpDesde.Value = hoy.AddDays(-7);
                    dtpHasta.Value = hoy;
                    dtpDesde.Enabled = false;
                    dtpHasta.Enabled = false;
                    break;

                case 2: // Este mes
                    dtpDesde.Value = new DateTime(hoy.Year, hoy.Month, 1);
                    dtpHasta.Value = hoy;
                    dtpDesde.Enabled = false;
                    dtpHasta.Enabled = false;
                    break;

                case 3: // Todos los registros
                    dtpDesde.Value = new DateTime(2020, 1, 1);
                    dtpHasta.Value = hoy.AddDays(1);
                    dtpDesde.Enabled = false;
                    dtpHasta.Enabled = false;
                    break;

                case 4: // Rango personalizado
                    dtpDesde.Enabled = true;
                    dtpHasta.Enabled = true;
                    break;
            }

            if (cmbPeriodo.SelectedIndex != 4)
            {
                CargarAtencionesDesdeBD();
            }
        }

        private void BtnFiltrar_Click(object? sender, EventArgs e)
        {
            CargarAtencionesDesdeBD();
        }

        private void CargarAtencionesDesdeBD()
        {
            if (_usuarioActual == null || _usuarioActual.IdUsuario <= 0)
            {
                lblTotalAtenciones.Text = "No se pudo identificar la sesión del profesional médico.";
                return;
            }

            DateTime fechaDesde = dtpDesde.Value.Date;
            DateTime fechaHasta = dtpHasta.Value.Date.AddDays(1).AddTicks(-1);

            try
            {
                // Invocación a través de la Capa de Negocio (BLL)
                _todasLasAtenciones = _historiaClinicaBLL.ObtenerAtencionesPorMedico(
                    _usuarioActual.IdUsuario, fechaDesde, fechaHasta);

                AplicarFiltroEnMemoria();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar las atenciones médicas:\n" + ex.Message,
                    "Error de Consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtBuscar_TextChanged(object? sender, EventArgs e)
        {
            AplicarFiltroEnMemoria();
        }

        private void AplicarFiltroEnMemoria()
        {
            string texto = txtBuscar.Text.Trim();

            var lista = _todasLasAtenciones;

            if (!string.IsNullOrWhiteSpace(texto))
            {
                lista = lista.Where(a =>
                    (!string.IsNullOrEmpty(a.NombrePaciente) && a.NombrePaciente.Contains(texto, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(a.ApellidoPaciente) && a.ApellidoPaciente.Contains(texto, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(a.DniPaciente) && a.DniPaciente.Contains(texto, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(a.DiagRapido) && a.DiagRapido.Contains(texto, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(a.NroOrden) && a.NroOrden.Contains(texto, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            dgvAtenciones.DataSource = null;
            dgvAtenciones.DataSource = lista;

            lblTotalAtenciones.Text = $"Total de atenciones encontradas: {lista.Count} (de {_todasLasAtenciones.Count} en el período)";

            ActualizarDetalle();
        }

        private void DgvAtenciones_SelectionChanged(object? sender, EventArgs e)
        {
            ActualizarDetalle();
        }

        private void ActualizarDetalle()
        {
            if (dgvAtenciones.CurrentRow != null && dgvAtenciones.CurrentRow.DataBoundItem is AtencionMedicoDTO atencion)
            {
                txtDetalleEvolucion.Text = !string.IsNullOrWhiteSpace(atencion.DiagRapido)
                    ? atencion.DiagRapido
                    : atencion.DescripHistoriaClinica;

                txtDetalleReceta.Text = !string.IsNullOrWhiteSpace(atencion.RecetaMedicamentos)
                    ? atencion.RecetaMedicamentos
                    : "(Sin receta o indicaciones farmacológicas prescriptas)";
            }
            else
            {
                txtDetalleEvolucion.Clear();
                txtDetalleReceta.Clear();
            }
        }

        private void BtnExportar_Click(object? sender, EventArgs e)
        {
            var atenciones = dgvAtenciones.DataSource as List<AtencionMedicoDTO>;
            if (atenciones == null || atenciones.Count == 0)
            {
                MessageBox.Show("No hay registros de atenciones para exportar en este momento.",
                    "Exportar Reporte", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                string fechaArchivo = DateTime.Now.ToString("yyyyMMdd_HHmm");
                sfd.Filter = "Libro de Excel (*.xlsx)|*.xlsx|Archivo CSV (*.csv)|*.csv|Archivo de Texto (*.txt)|*.txt";
                sfd.FilterIndex = 1;
                sfd.FileName = $"Reporte_Atenciones_Dr_{_usuarioActual?.Apellido ?? "Medico"}_{fechaArchivo}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string extension = Path.GetExtension(sfd.FileName).ToLowerInvariant();

                        if (extension == ".xlsx")
                        {
                            ExportadorExcel.ExportarAtencionesMedico(
                                sfd.FileName,
                                atenciones,
                                dtpDesde.Value.Date,
                                dtpHasta.Value.Date,
                                _usuarioActual);
                        }
                        else if (extension == ".csv")
                        {
                            ExportarCsv(sfd.FileName, atenciones);
                        }
                        else
                        {
                            ExportarTxt(sfd.FileName, atenciones);
                        }

                        MessageBox.Show($"Reporte exportado exitosamente en:\n{sfd.FileName}",
                            "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al exportar el archivo:\n" + ex.Message,
                            "Error de Exportación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportarTxt(string rutaArchivo, List<AtencionMedicoDTO> lista)
        {
            var sb = new StringBuilder();
            sb.AppendLine("================================================================================");
            sb.AppendLine("                 CENTRO MÉDICO — REPORTE DE ATENCIONES MÉDICAS                  ");
            sb.AppendLine("================================================================================");
            sb.AppendLine($"PROFESIONAL MÉDICO : Dr. {_usuarioActual?.Nombre} {_usuarioActual?.Apellido}");
            sb.AppendLine($"FECHA DE EMISIÓN   : {DateTime.Now:dd/MM/yyyy HH:mm:ss} hs");
            sb.AppendLine($"PERÍODO CONSULTADO : {dtpDesde.Value:dd/MM/yyyy} al {dtpHasta.Value:dd/MM/yyyy}");
            sb.AppendLine($"TOTAL ATENCIONES   : {lista.Count}");
            sb.AppendLine("================================================================================");
            sb.AppendLine();

            int contador = 1;
            foreach (var a in lista)
            {
                sb.AppendLine($"[{contador:D3}] ----------------------------------------------------------------------------");
                sb.AppendLine($"FECHA / HORA : {a.Fecha:dd/MM/yyyy HH:mm} hs | TURNO: {a.NroOrden} | SALA: {a.NombreSala}");
                sb.AppendLine($"PACIENTE     : {a.PacienteCompleto} | DNI: {a.DniPaciente} | COBERTURA: {a.ObraSocial}");
                sb.AppendLine($"MODALIDAD    : {a.TipoTurno} | ESPECIALIDAD: {a.Especialidad}");
                sb.AppendLine($"DIAGNÓSTICO  : {a.DiagRapido}");
                if (!string.IsNullOrWhiteSpace(a.RecetaMedicamentos))
                {
                    sb.AppendLine($"RECETA       : {a.RecetaMedicamentos}");
                }
                sb.AppendLine();
                contador++;
            }

            sb.AppendLine("================================================================================");
            sb.AppendLine("                            FIN DEL REPORTE                                     ");
            sb.AppendLine("================================================================================");

            File.WriteAllText(rutaArchivo, sb.ToString(), Encoding.UTF8);
        }

        private void ExportarCsv(string rutaArchivo, List<AtencionMedicoDTO> lista)
        {
            var sb = new StringBuilder();
            sb.AppendLine("sep=;");
            sb.AppendLine("IdHistoria;Fecha;NroOrden;Paciente;DNI;ObraSocial;TipoTurno;Especialidad;Sala;Diagnostico;Receta");

            foreach (var a in lista)
            {
                string diagnostico = (a.DiagRapido ?? "").Replace(";", ",").Replace("\r\n", " ");
                string receta = (a.RecetaMedicamentos ?? "").Replace(";", ",").Replace("\r\n", " ");

                sb.AppendLine($"{a.IdHistoria};{a.Fecha:dd/MM/yyyy HH:mm};{a.NroOrden};{a.PacienteCompleto};{a.DniPaciente};{a.ObraSocial};{a.TipoTurno};{a.Especialidad};{a.NombreSala};{diagnostico};{receta}");
            }

            File.WriteAllText(rutaArchivo, sb.ToString(), new UTF8Encoding(true));
        }
    }
}
