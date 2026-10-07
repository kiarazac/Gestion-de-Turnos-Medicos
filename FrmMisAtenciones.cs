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
        private readonly ReporteBLL _reporteBLL = new ReporteBLL();
        private readonly TurnoBLL _turnoBLL = new TurnoBLL();
        private readonly UsuarioLoginResult? _usuarioActual;

        private List<AtencionMedicoDTO> _todasLasAtenciones = new List<AtencionMedicoDTO>();
        private List<ReporteMedicoGravedadDTO> _rankingGravedad = new List<ReporteMedicoGravedadDTO>();
        private List<ReporteMedicoSintomaDTO> _rankingSintomas = new List<ReporteMedicoSintomaDTO>();
        private bool _esMedicoClinico = false;

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
            this.dgvAtenciones.CellFormatting += DgvAtenciones_CellFormatting;
            this.dgvRankingGravedad.CellFormatting += DgvRankingGravedad_CellFormatting;
            this.dgvRankingSintomas.CellFormatting += DgvRankingSintomas_CellFormatting;
        }

        private void FrmMisAtenciones_Load(object? sender, EventArgs e)
        {
            ConfigurarGrillaAtenciones();
            ConfigurarGrillasTriage();

            if (_usuarioActual != null)
            {
                lblTituloHeader.Text = $"Mis Atenciones Realizadas — Dr. {_usuarioActual.Nombre} {_usuarioActual.Apellido}";
                
                // Determinar si el médico tratante está habilitado para atender emergencias (Especialidad Clínico)
                _esMedicoClinico = _turnoBLL.PuedeAtenderEmergencias(_usuarioActual.IdUsuario);

                if (_esMedicoClinico)
                {
                    lblSubtituloHeader.Text = "Consultas ambulatorias, diagnósticos, recetas prescriptas y reportería de urgencias en guardia";
                }
                else
                {
                    lblSubtituloHeader.Text = "Consultas de especialidad, diagnósticos clínicos y recetas prescriptas en consultorio";
                    // Si NO es médico clínico, se retira la pestaña de guardia/triage adaptando la vista exclusivamente a su rol
                    tabMisAtenciones.TabPages.Remove(tabTriageClinico);
                }
            }

            // Inicializar fechas predeterminadas (Últimos 7 días)
            cmbPeriodo.SelectedIndex = 1; // "Últimos 7 días"
            CargarAtencionesDesdeBD();
        }

        private void ConfigurarGrillaAtenciones()
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
                Width = 120
            });

            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTurno",
                HeaderText = "N° Turno",
                DataPropertyName = "NroOrden",
                Width = 75
            });

            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPaciente",
                HeaderText = "Paciente",
                DataPropertyName = "PacienteCompleto",
                Width = 160
            });

            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDni",
                HeaderText = "DNI",
                DataPropertyName = "DniPaciente",
                Width = 85
            });

            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colObraSocial",
                HeaderText = "Cobertura",
                DataPropertyName = "ObraSocial",
                Width = 110
            });

            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTipo",
                HeaderText = "Modalidad",
                DataPropertyName = "TipoTurno",
                Width = 90
            });

            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colGravedad",
                HeaderText = "Triage / Severidad",
                DataPropertyName = "GravedadTriage",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDiagnostico",
                HeaderText = "Diagnóstico Emitido",
                DataPropertyName = "DiagRapido",
                Width = 240
            });

            dgvAtenciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSala",
                HeaderText = "Consultorio / Sala",
                DataPropertyName = "NombreSala",
                Width = 110
            });
        }

        private void ConfigurarGrillasTriage()
        {
            // Grilla de Ranking de Gravedad
            dgvRankingGravedad.AutoGenerateColumns = false;
            dgvRankingGravedad.Columns.Clear();
            dgvRankingGravedad.RowHeadersVisible = false;
            dgvRankingGravedad.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRankingGravedad.ReadOnly = true;
            dgvRankingGravedad.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgvRankingGravedad.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colGravNombre",
                HeaderText = "Nivel de Prioridad",
                DataPropertyName = "Gravedad",
                Width = 150
            });

            dgvRankingGravedad.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colGravTurnos",
                HeaderText = "Turnos Atendidos",
                DataPropertyName = "CantidadTurnos",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvRankingGravedad.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colGravPorcentaje",
                HeaderText = "% Prevalencia",
                DataPropertyName = "Porcentaje",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N1", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            // Grilla de Ranking de Síntomas
            dgvRankingSintomas.AutoGenerateColumns = false;
            dgvRankingSintomas.Columns.Clear();
            dgvRankingSintomas.RowHeadersVisible = false;
            dgvRankingSintomas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRankingSintomas.ReadOnly = true;
            dgvRankingSintomas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgvRankingSintomas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSintRank",
                HeaderText = "#",
                Width = 35,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvRankingSintomas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSintDescrip",
                HeaderText = "Síntoma Clínico Manifestado",
                DataPropertyName = "Sintoma",
                Width = 230
            });

            dgvRankingSintomas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSintGravedad",
                HeaderText = "Severidad",
                DataPropertyName = "Gravedad",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvRankingSintomas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSintCasos",
                HeaderText = "Pacientes",
                DataPropertyName = "CantidadCasos",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvRankingSintomas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSintPorcentaje",
                HeaderText = "% Frecuencia",
                DataPropertyName = "Porcentaje",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N1", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
        }

        private void DgvAtenciones_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvAtenciones.Columns[e.ColumnIndex].Name == "colGravedad" && e.Value != null)
            {
                string val = e.Value.ToString() ?? "";
                if (val.Equals("Alta", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.BackColor = Color.FromArgb(254, 226, 226); // Rojo pastel
                    e.CellStyle.ForeColor = Color.FromArgb(153, 27, 27);
                    e.CellStyle.Font = new Font(dgvAtenciones.Font, FontStyle.Bold);
                }
                else if (val.Equals("Media", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.BackColor = Color.FromArgb(254, 240, 138); // Amarillo pastel
                    e.CellStyle.ForeColor = Color.FromArgb(133, 77, 14);
                    e.CellStyle.Font = new Font(dgvAtenciones.Font, FontStyle.Bold);
                }
                else if (val.Equals("Baja", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.BackColor = Color.FromArgb(220, 252, 231); // Verde pastel
                    e.CellStyle.ForeColor = Color.FromArgb(22, 101, 52);
                    e.CellStyle.Font = new Font(dgvAtenciones.Font, FontStyle.Bold);
                }
                else
                {
                    e.CellStyle.ForeColor = Color.FromArgb(148, 163, 184); // Gris
                }
            }
        }

        private void DgvRankingGravedad_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvRankingGravedad.Columns[e.ColumnIndex].Name == "colGravNombre" && e.Value != null)
            {
                string val = e.Value.ToString() ?? "";
                if (val.Equals("Alta", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.ForeColor = Color.FromArgb(220, 38, 38);
                    e.CellStyle.Font = new Font(dgvRankingGravedad.Font, FontStyle.Bold);
                }
                else if (val.Equals("Media", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.ForeColor = Color.FromArgb(202, 138, 4);
                    e.CellStyle.Font = new Font(dgvRankingGravedad.Font, FontStyle.Bold);
                }
                else if (val.Equals("Baja", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.ForeColor = Color.FromArgb(22, 101, 52);
                    e.CellStyle.Font = new Font(dgvRankingGravedad.Font, FontStyle.Bold);
                }
            }
        }

        private void DgvRankingSintomas_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvRankingSintomas.Columns[e.ColumnIndex].Name == "colSintGravedad" && e.Value != null)
            {
                string val = e.Value.ToString() ?? "";
                if (val.Equals("Alta", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.BackColor = Color.FromArgb(254, 226, 226);
                    e.CellStyle.ForeColor = Color.FromArgb(153, 27, 27);
                    e.CellStyle.Font = new Font(dgvRankingSintomas.Font, FontStyle.Bold);
                }
                else if (val.Equals("Media", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.BackColor = Color.FromArgb(254, 240, 138);
                    e.CellStyle.ForeColor = Color.FromArgb(133, 77, 14);
                    e.CellStyle.Font = new Font(dgvRankingSintomas.Font, FontStyle.Bold);
                }
                else if (val.Equals("Baja", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.BackColor = Color.FromArgb(220, 252, 231);
                    e.CellStyle.ForeColor = Color.FromArgb(22, 101, 52);
                    e.CellStyle.Font = new Font(dgvRankingSintomas.Font, FontStyle.Bold);
                }
            }
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

                if (_esMedicoClinico)
                {
                    _rankingGravedad = _reporteBLL.ObtenerRankingGravedadMedico(
                        _usuarioActual.IdUsuario, fechaDesde, dtpHasta.Value.Date);

                    _rankingSintomas = _reporteBLL.ObtenerRankingSintomasMedico(
                        _usuarioActual.IdUsuario, fechaDesde, dtpHasta.Value.Date);

                    dgvRankingGravedad.DataSource = null;
                    dgvRankingGravedad.DataSource = _rankingGravedad;

                    dgvRankingSintomas.DataSource = null;
                    dgvRankingSintomas.DataSource = _rankingSintomas;

                    for (int i = 0; i < dgvRankingSintomas.Rows.Count; i++)
                    {
                        dgvRankingSintomas.Rows[i].Cells["colSintRank"].Value = (i + 1).ToString();
                    }
                }

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
                sfd.Filter = "Documento Oficial PDF (*.pdf)|*.pdf";
                sfd.FilterIndex = 1;
                sfd.FileName = $"Reporte_Atenciones_Dr_{_usuarioActual?.Apellido ?? "Medico"}_{fechaArchivo}.pdf";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;

                        ExportadorPdf.ExportarReporteMedico(
                            sfd.FileName,
                            atenciones,
                            _esMedicoClinico ? _rankingGravedad : null,
                            _esMedicoClinico ? _rankingSintomas : null,
                            dtpDesde.Value.Date,
                            dtpHasta.Value.Date,
                            _usuarioActual);

                        var resp = MessageBox.Show(
                            $"Reporte oficial en PDF generado exitosamente en:\n{sfd.FileName}\n\n¿Desea abrir el archivo ahora?",
                            "Exportación Exitosa", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        if (resp == DialogResult.Yes)
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al exportar el archivo PDF:\n" + ex.Message,
                            "Error de Exportación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
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
