
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Reporting.WinForms;

using TechSolutions.Negocio.Servicios;
using TechSolutions.Entidades.Ventas;

namespace TechSolutions.Presentacion
{
    public class FrmReporteVentas : Form
    {
        private readonly VentaServicio ventaServicio;
        private List<Venta> ventasCargadas;

        private DataGridView dgvVentas;
        private Label lblTitulo;
        private Label lblTotalVentas;
        private Label lblTotalRecaudado;

        private Button btnActualizar;
        private Button btnCerrar;
        private Button btnVerDetalles;
        private Button btnImprimir;
        private Button btnExportarPdf;
        private Button btnBuscar;
        private Button btnVerReporte;

        private DateTimePicker dtpFechaInicio;
        private DateTimePicker dtpFechaFin;

        private PrintDocument documentoImpresion;
        private int indiceVentaImpresion;
        private int numeroPagina;
        private bool resumenImpreso;
        private bool filtroFechasAplicado;
        private DateTime fechaInicioFiltro;
        private DateTime fechaFinFiltro;

        private Color azulOscuro = Color.FromArgb(23, 37, 61);
        private Color azul = Color.FromArgb(37, 99, 180);
        private Color fondo = Color.FromArgb(242, 245, 250);
        private Color verde = Color.FromArgb(20, 145, 105);
        private Color texto = Color.FromArgb(45, 55, 72);

        public FrmReporteVentas()
        {
            ventaServicio = new VentaServicio();
            ventasCargadas = new List<Venta>();

            filtroFechasAplicado = false;

            InicializarComponentes();

            documentoImpresion = new PrintDocument();
            documentoImpresion.PrintPage += DocumentoImpresion_PrintPage;

            documentoImpresion.DefaultPageSettings.Landscape = true;
            documentoImpresion.DefaultPageSettings.Margins =
                new Margins(45, 45, 45, 45);

            Shown += async (sender, e) =>
            {
                await CargarVentasAsync();
            };
        }

        private void InicializarComponentes()
        {
            Text = "TechSolutions | Reporte de ventas";
            Size = new Size(1100, 740);
            MinimumSize = new Size(1100, 740);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = fondo;
            Font = new Font("Segoe UI", 10);

            Panel encabezado = new Panel();
            encabezado.Dock = DockStyle.Top;
            encabezado.Height = 115;
            encabezado.BackColor = azulOscuro;
            Controls.Add(encabezado);

            lblTitulo = new Label();
            lblTitulo.Text = "REPORTE DE VENTAS";
            lblTitulo.Font = new Font("Segoe UI", 23, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(30, 18);
            lblTitulo.AutoSize = true;
            encabezado.Controls.Add(lblTitulo);

            Label subtitulo = new Label();
            subtitulo.Text =
                "Consulta el historial de ventas y revisa los ingresos de TechSolutions";
            subtitulo.Font = new Font("Segoe UI", 9);
            subtitulo.ForeColor = Color.FromArgb(195, 210, 230);
            subtitulo.Location = new Point(35, 70);
            subtitulo.AutoSize = true;
            encabezado.Controls.Add(subtitulo);

            Panel tarjetaVentas = CrearTarjeta(
                new Point(25, 140),
                new Size(490, 115),
                azul);

            Controls.Add(tarjetaVentas);

            Label tituloVentas = new Label();
            tituloVentas.Text = "TOTAL DE VENTAS";
            tituloVentas.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            tituloVentas.ForeColor = Color.Gray;
            tituloVentas.Location = new Point(20, 15);
            tituloVentas.AutoSize = true;
            tarjetaVentas.Controls.Add(tituloVentas);

            lblTotalVentas = new Label();
            lblTotalVentas.Text = "0";
            lblTotalVentas.Font = new Font("Segoe UI", 26, FontStyle.Bold);
            lblTotalVentas.ForeColor = azul;
            lblTotalVentas.Location = new Point(20, 42);
            lblTotalVentas.AutoSize = true;
            tarjetaVentas.Controls.Add(lblTotalVentas);

            Panel tarjetaRecaudacion = CrearTarjeta(
                new Point(535, 140),
                new Size(540, 115),
                verde);

            Controls.Add(tarjetaRecaudacion);

            Label tituloRecaudacion = new Label();
            tituloRecaudacion.Text = "TOTAL RECAUDADO";
            tituloRecaudacion.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);
            tituloRecaudacion.ForeColor = Color.Gray;
            tituloRecaudacion.Location = new Point(20, 15);
            tituloRecaudacion.AutoSize = true;
            tarjetaRecaudacion.Controls.Add(tituloRecaudacion);

            lblTotalRecaudado = new Label();
            lblTotalRecaudado.Text = "S/ 0.00";
            lblTotalRecaudado.Font =
                new Font("Segoe UI", 26, FontStyle.Bold);
            lblTotalRecaudado.ForeColor = verde;
            lblTotalRecaudado.Location = new Point(20, 42);
            lblTotalRecaudado.AutoSize = true;
            tarjetaRecaudacion.Controls.Add(lblTotalRecaudado);

            Panel panelFiltros = new Panel();
            panelFiltros.Location = new Point(25, 270);
            panelFiltros.Size = new Size(1050, 65);
            panelFiltros.BackColor = Color.White;
            panelFiltros.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(panelFiltros);

            Label lblDesde = new Label();
            lblDesde.Text = "DESDE:";
            lblDesde.Location = new Point(20, 22);
            lblDesde.AutoSize = true;
            panelFiltros.Controls.Add(lblDesde);

            dtpFechaInicio = new DateTimePicker();
            dtpFechaInicio.Format = DateTimePickerFormat.Short;
            dtpFechaInicio.Location = new Point(85, 17);
            dtpFechaInicio.Size = new Size(140, 30);
            dtpFechaInicio.Value = new DateTime(2000, 1, 1);
            panelFiltros.Controls.Add(dtpFechaInicio);

            Label lblHasta = new Label();
            lblHasta.Text = "HASTA:";
            lblHasta.Location = new Point(260, 22);
            lblHasta.AutoSize = true;
            panelFiltros.Controls.Add(lblHasta);

            dtpFechaFin = new DateTimePicker();
            dtpFechaFin.Format = DateTimePickerFormat.Short;
            dtpFechaFin.Location = new Point(325, 17);
            dtpFechaFin.Size = new Size(140, 30);
            dtpFechaFin.Value = DateTime.Today;
            panelFiltros.Controls.Add(dtpFechaFin);

            btnBuscar = CrearBoton("BUSCAR", azul);
            btnBuscar.Location = new Point(500, 12);
            btnBuscar.Size = new Size(140, 40);
            btnBuscar.Click += BtnBuscar_Click;
            panelFiltros.Controls.Add(btnBuscar);

            btnVerReporte = CrearBoton("VER REPORTE RDLC", verde);
            btnVerReporte.Location = new Point(700, 12);
            btnVerReporte.Size = new Size(190, 40);
            btnVerReporte.Click += BtnVerReporte_Click;
            panelFiltros.Controls.Add(btnVerReporte);

            Panel panelHistorial = new Panel();
            panelHistorial.Location = new Point(25, 350);
            panelHistorial.Size = new Size(1050, 260);
            panelHistorial.BackColor = Color.White;
            panelHistorial.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(panelHistorial);

            Label lblHistorial = new Label();
            lblHistorial.Text = "HISTORIAL DE OPERACIONES";
            lblHistorial.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblHistorial.ForeColor = azulOscuro;
            lblHistorial.Location = new Point(20, 17);
            lblHistorial.AutoSize = true;
            panelHistorial.Controls.Add(lblHistorial);

            Label lblDescripcion = new Label();
            lblDescripcion.Text =
                "Listado de todas las ventas registradas en el sistema";
            lblDescripcion.Font = new Font("Segoe UI", 9);
            lblDescripcion.ForeColor = Color.Gray;
            lblDescripcion.Location = new Point(22, 48);
            lblDescripcion.AutoSize = true;
            panelHistorial.Controls.Add(lblDescripcion);

            dgvVentas = new DataGridView();
            dgvVentas.Location = new Point(20, 85);
            dgvVentas.Size = new Size(1010, 155);
            dgvVentas.ReadOnly = true;
            dgvVentas.AllowUserToAddRows = false;
            dgvVentas.AllowUserToDeleteRows = false;
            dgvVentas.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvVentas.MultiSelect = false;
            dgvVentas.RowHeadersVisible = false;
            dgvVentas.BackgroundColor = Color.White;
            dgvVentas.BorderStyle = BorderStyle.None;
            dgvVentas.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;
            dgvVentas.EnableHeadersVisualStyles = false;
            dgvVentas.ColumnHeadersHeight = 42;
            dgvVentas.RowTemplate.Height = 36;
            dgvVentas.GridColor = Color.FromArgb(230, 234, 240);

            dgvVentas.ColumnHeadersDefaultCellStyle.BackColor = azulOscuro;
            dgvVentas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvVentas.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            dgvVentas.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvVentas.DefaultCellStyle.ForeColor = texto;
            dgvVentas.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(218, 232, 250);
            dgvVentas.DefaultCellStyle.SelectionForeColor = texto;

            dgvVentas.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(246, 248, 252);

            dgvVentas.Columns.Add("Id", "N° VENTA");
            dgvVentas.Columns.Add("UsuarioId", "ID USUARIO");
            dgvVentas.Columns.Add("NombreUsuario", "USUARIO");
            dgvVentas.Columns.Add("Fecha", "FECHA");
            dgvVentas.Columns.Add("Total", "TOTAL");

            dgvVentas.Columns["Id"].FillWeight = 65;
            dgvVentas.Columns["UsuarioId"].FillWeight = 75;
            dgvVentas.Columns["NombreUsuario"].FillWeight = 160;
            dgvVentas.Columns["Fecha"].FillWeight = 140;
            dgvVentas.Columns["Total"].FillWeight = 100;

            panelHistorial.Controls.Add(dgvVentas);

            btnVerDetalles = CrearBoton("VER DETALLES", azul);
            btnVerDetalles.Location = new Point(25, 635);
            btnVerDetalles.Size = new Size(165, 45);
            btnVerDetalles.Click += BtnVerDetalles_Click;
            Controls.Add(btnVerDetalles);

            btnImprimir = CrearBoton(
                "IMPRIMIR REPORTE",
                Color.FromArgb(20, 145, 105));
            btnImprimir.Location = new Point(210, 635);
            btnImprimir.Size = new Size(200, 45);
            btnImprimir.Click += BtnImprimir_Click;
            Controls.Add(btnImprimir);

            btnExportarPdf = CrearBoton(
                "EXPORTAR A PDF",
                Color.FromArgb(180, 75, 45));
            btnExportarPdf.Location = new Point(425, 635);
            btnExportarPdf.Size = new Size(200, 45);
            btnExportarPdf.Click += BtnExportarPdf_Click;
            Controls.Add(btnExportarPdf);

            btnActualizar = CrearBoton("ACTUALIZAR REPORTE", verde);
            btnActualizar.Location = new Point(720, 635);
            btnActualizar.Size = new Size(190, 45);
            btnActualizar.Click += async (sender, e) =>
            {
                await CargarVentasAsync();
            };
            Controls.Add(btnActualizar);

            btnCerrar = CrearBoton(
                "CERRAR",
                Color.FromArgb(95, 105, 120));
            btnCerrar.Location = new Point(925, 635);
            btnCerrar.Size = new Size(150, 45);
            btnCerrar.Click += (sender, e) => Close();
            Controls.Add(btnCerrar);
        }

        private Panel CrearTarjeta(
            Point ubicacion,
            Size tamaño,
            Color color)
        {
            Panel panel = new Panel();
            panel.Location = ubicacion;
            panel.Size = tamaño;
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.FixedSingle;

            Panel linea = new Panel();
            linea.Dock = DockStyle.Left;
            linea.Width = 7;
            linea.BackColor = color;
            panel.Controls.Add(linea);

            return panel;
        }

        private Button CrearBoton(string nombre, Color color)
        {
            Button boton = new Button();
            boton.Text = nombre;
            boton.Size = new Size(160, 40);
            boton.BackColor = color;
            boton.ForeColor = Color.White;
            boton.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.Cursor = Cursors.Hand;
            return boton;
        }

        private async Task CargarVentasAsync()
        {
            btnActualizar.Enabled = false;
            btnBuscar.Enabled = false;
            btnVerDetalles.Enabled = false;

            try
            {
                List<Venta> resultado = await Task.Run(
                    () => ventaServicio.ObtenerTodas());

                if (IsDisposed || Disposing)
                    return;

                ventasCargadas = resultado ?? new List<Venta>();
                filtroFechasAplicado = false;
                MostrarVentasCargadas();
            }
            catch (Exception ex)
            {
                if (!IsDisposed && !Disposing)
                {
                    MessageBox.Show(
                        "Error al cargar las ventas:\n" + ex.Message,
                        "Reporte de ventas",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            finally
            {
                if (!IsDisposed && !Disposing)
                {
                    btnActualizar.Enabled = true;
                    btnBuscar.Enabled = true;
                    btnVerDetalles.Enabled = true;
                }
            }
        }

        private void MostrarVentasCargadas()
        {
            dgvVentas.Rows.Clear();

            foreach (Venta venta in ventasCargadas)
            {
                dgvVentas.Rows.Add(
                    venta.Id,
                    venta.UsuarioId,
                    venta.NombreUsuario,
                    venta.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    venta.Total.ToString("C2"));
            }

            lblTotalVentas.Text = ventasCargadas.Count.ToString();

            decimal recaudacion = ventasCargadas.Sum(v => v.Total);
            lblTotalRecaudado.Text = recaudacion.ToString("C2");
        }

        private async void BtnBuscar_Click(object sender, EventArgs e)
        {
            DateTime inicio = dtpFechaInicio.Value.Date;
            DateTime fin = dtpFechaFin.Value.Date;

            if (inicio > fin)
            {
                MessageBox.Show(
                    "La fecha inicial no puede ser posterior a la fecha final.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            btnBuscar.Enabled = false;
            btnActualizar.Enabled = false;
            btnVerDetalles.Enabled = false;

            try
            {
                List<Venta> resultado = await Task.Run(
                    () => ventaServicio.ObtenerPorFechas(inicio, fin));

                if (IsDisposed || Disposing)
                    return;

                ventasCargadas = resultado ?? new List<Venta>();
                filtroFechasAplicado = true;
                fechaInicioFiltro = inicio;
                fechaFinFiltro = fin;

                MostrarVentasCargadas();

                if (ventasCargadas.Count == 0)
                {
                    MessageBox.Show(
                        "No se encontraron ventas en el periodo seleccionado.",
                        "Resultado de búsqueda",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al filtrar las ventas:\n" + ex.Message,
                    "Reporte de ventas",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed && !Disposing)
                {
                    btnBuscar.Enabled = true;
                    btnActualizar.Enabled = true;
                    btnVerDetalles.Enabled = true;
                }
            }
        }

        // CONSTRUIR DATOS DEL INFORME RDLC
        private DataTable CrearTablaReporte()
        {
            DataTable tabla = new DataTable("DataSet1");

            tabla.Columns.Add("IdVenta", typeof(int));
            tabla.Columns.Add("Fecha", typeof(DateTime));
            tabla.Columns.Add("NombreUsuario", typeof(string));
            tabla.Columns.Add("Producto", typeof(string));
            tabla.Columns.Add("Cantidad", typeof(int));
            tabla.Columns.Add("PrecioUnitario", typeof(decimal));
            tabla.Columns.Add("Subtotal", typeof(decimal));
            tabla.Columns.Add("Total", typeof(decimal));

            foreach (Venta venta in ventasCargadas)
            {
                List<DetalleVenta> detalles =
                    ventaServicio.ObtenerDetalles(venta.Id);

                if (detalles == null || detalles.Count == 0)
                {
                    tabla.Rows.Add(
                        venta.Id,
                        venta.Fecha,
                        venta.NombreUsuario ?? "",
                        "Sin detalles",
                        0,
                        0m,
                        0m,
                        venta.Total);
                    continue;
                }

                foreach (DetalleVenta detalle in detalles)
                {
                    tabla.Rows.Add(
                        venta.Id,
                        venta.Fecha,
                        venta.NombreUsuario ?? "",
                        detalle.NombreProducto ?? "",
                        detalle.Cantidad,
                        detalle.PrecioUnitario,
                        detalle.Subtotal,
                        venta.Total);
                }
            }

            return tabla;
        }

        // ABRIR INFORME RDLC
        private async void BtnVerReporte_Click(object sender, EventArgs e)
        {
            btnVerReporte.Enabled = false;

            try
            {
                if (ventasCargadas == null || ventasCargadas.Count == 0)
                {
                    MessageBox.Show(
                        "No existen ventas para generar el reporte.",
                        "Reporte RDLC",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                DataTable datos = await Task.Run(
                    () => CrearTablaReporte());

                string rutaReporte = Path.Combine(
                    Application.StartupPath,
                    "ReporteVentas.rdlc");

                if (!File.Exists(rutaReporte))
                {
                    MessageBox.Show(
                        "No se encontró ReporteVentas.rdlc.\n" +
                        "Configura Copy to Output Directory en Copy always.",
                        "Reporte RDLC",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                Form ventanaReporte = new Form();
                ventanaReporte.Text = "TechSolutions | Informe de ventas";
                ventanaReporte.Size = new Size(1100, 750);
                ventanaReporte.StartPosition = FormStartPosition.CenterParent;

                ReportViewer visor = new ReportViewer();
                visor.Dock = DockStyle.Fill;
                visor.ProcessingMode = ProcessingMode.Local;
                visor.LocalReport.ReportPath = rutaReporte;
                visor.LocalReport.DataSources.Clear();

                ReportDataSource origen = new ReportDataSource(
                    "DataSet1",
                    datos);

                visor.LocalReport.DataSources.Add(origen);

                ventanaReporte.Controls.Add(visor);

                visor.RefreshReport();
                ventanaReporte.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al generar el informe:\n" + ex.Message,
                    "Reporte RDLC",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed && !Disposing)
                    btnVerReporte.Enabled = true;
            }
        }

        private void ReiniciarImpresion()
        {
            indiceVentaImpresion = 0;
            numeroPagina = 0;
            resumenImpreso = false;
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                if (ventasCargadas == null || ventasCargadas.Count == 0)
                {
                    MessageBox.Show(
                        "No existen ventas para imprimir.",
                        "Impresión",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                ReiniciarImpresion();

                documentoImpresion.DocumentName =
                    "Reporte de ventas TechSolutions";

                documentoImpresion.DefaultPageSettings.Landscape = true;

                using (PrintPreviewDialog vistaPrevia =
                    new PrintPreviewDialog())
                {
                    vistaPrevia.Document = documentoImpresion;
                    vistaPrevia.Width = 1000;
                    vistaPrevia.Height = 750;
                    vistaPrevia.StartPosition =
                        FormStartPosition.CenterParent;
                    vistaPrevia.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al preparar la impresión:\n" + ex.Message,
                    "Impresión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnExportarPdf_Click(object sender, EventArgs e)
        {
            try
            {
                if (ventasCargadas == null || ventasCargadas.Count == 0)
                {
                    MessageBox.Show(
                        "No existen ventas para exportar.",
                        "Exportar PDF",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                string impresoraPDF = "Microsoft Print to PDF";
                bool existeImpresora = false;

                foreach (string impresora in PrinterSettings.InstalledPrinters)
                {
                    if (string.Equals(
                        impresora,
                        impresoraPDF,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        existeImpresora = true;
                        break;
                    }
                }

                if (!existeImpresora)
                {
                    MessageBox.Show(
                        "No se encontró Microsoft Print to PDF.\n" +
                        "Activa esta característica de Windows.",
                        "Impresora PDF no disponible",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                using (SaveFileDialog guardar = new SaveFileDialog())
                {
                    guardar.Title = "Guardar reporte de ventas";
                    guardar.Filter = "Archivo PDF (*.pdf)|*.pdf";
                    guardar.FileName =
                        "ReporteVentas_" +
                        DateTime.Now.ToString("yyyyMMdd_HHmmss") +
                        ".pdf";
                    guardar.DefaultExt = "pdf";
                    guardar.AddExtension = true;
                    guardar.OverwritePrompt = true;

                    if (guardar.ShowDialog(this) != DialogResult.OK)
                        return;

                    PrinterSettings configuracionAnterior =
                        documentoImpresion.PrinterSettings;
                    PrintController controladorAnterior =
                        documentoImpresion.PrintController;

                    try
                    {
                        ReiniciarImpresion();

                        PrinterSettings configuracionPDF =
                            new PrinterSettings();

                        configuracionPDF.PrinterName = impresoraPDF;

                        if (!configuracionPDF.IsValid)
                        {
                            throw new InvalidOperationException(
                                "La impresora PDF no está disponible.");
                        }

                        configuracionPDF.PrintToFile = true;
                        configuracionPDF.PrintFileName = guardar.FileName;

                        documentoImpresion.PrinterSettings =
                            configuracionPDF;
                        documentoImpresion.PrintController =
                            new StandardPrintController();
                        documentoImpresion.DefaultPageSettings.Landscape = true;

                        documentoImpresion.Print();

                        if (!File.Exists(guardar.FileName))
                        {
                            throw new Exception(
                                "No se encontró el archivo PDF generado.");
                        }

                        MessageBox.Show(
                            "¡Reporte exportado correctamente!\n\n" +
                            "Archivo guardado en:\n" + guardar.FileName,
                            "Exportación exitosa",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    finally
                    {
                        documentoImpresion.PrinterSettings =
                            configuracionAnterior;
                        documentoImpresion.PrintController =
                            controladorAnterior;
                        ReiniciarImpresion();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al exportar el PDF:\n" + ex.Message,
                    "Error de exportación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DocumentoImpresion_PrintPage(
            object sender,
            PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;

            float izquierda = e.MarginBounds.Left;
            float derecha = e.MarginBounds.Right;
            float ancho = e.MarginBounds.Width;
            float y = e.MarginBounds.Top;

            using (Font fuenteTitulo = new Font("Arial", 20, FontStyle.Bold))
            using (Font fuenteSubtitulo = new Font("Arial", 10))
            using (Font fuenteEncabezado = new Font("Arial", 9, FontStyle.Bold))
            using (Font fuenteNormal = new Font("Arial", 9))
            using (Font fuenteTotal = new Font("Arial", 12, FontStyle.Bold))
            using (Pen lapiz = new Pen(Color.FromArgb(210, 215, 225), 1))
            using (SolidBrush pincelAzul = new SolidBrush(azulOscuro))
            using (SolidBrush pincelBlanco = new SolidBrush(Color.White))
            using (SolidBrush pincelTexto = new SolidBrush(texto))
            using (SolidBrush pincelGris = new SolidBrush(Color.FromArgb(240, 243, 248)))
            using (SolidBrush pincelVerde = new SolidBrush(verde))
            {
                numeroPagina++;

                g.DrawString("TECHSOLUTIONS", fuenteTitulo, pincelAzul, izquierda, y);
                y += 35;

                g.DrawString("REPORTE DE VENTAS", fuenteSubtitulo, pincelTexto, izquierda, y);
                y += 22;

                g.DrawString(
                    "Fecha de emisión: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    fuenteNormal, pincelTexto, izquierda, y);
                y += 20;

                string periodo = filtroFechasAplicado
                    ? "Periodo: " + fechaInicioFiltro.ToString("dd/MM/yyyy") +
                      " hasta " + fechaFinFiltro.ToString("dd/MM/yyyy")
                    : "Periodo: Todas las ventas registradas";

                g.DrawString(periodo, fuenteNormal, pincelTexto, izquierda, y);
                y += 30;

                float[] anchos =
                {
                    ancho * 0.12f,
                    ancho * 0.14f,
                    ancho * 0.28f,
                    ancho * 0.25f,
                    ancho * 0.21f
                };

                string[] encabezados =
                {
                    "N° VENTA", "ID USUARIO", "USUARIO", "FECHA", "TOTAL"
                };

                float x = izquierda;
                float altoFila = 30;

                for (int i = 0; i < encabezados.Length; i++)
                {
                    RectangleF rectangulo =
                        new RectangleF(x, y, anchos[i], altoFila);

                    g.FillRectangle(pincelAzul, rectangulo);
                    g.DrawString(
                        encabezados[i],
                        fuenteEncabezado,
                        pincelBlanco,
                        rectangulo,
                        new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        });

                    x += anchos[i];
                }

                y += altoFila;
                float limiteInferior = e.MarginBounds.Bottom - 65;

                while (indiceVentaImpresion < ventasCargadas.Count)
                {
                    if (y + altoFila > limiteInferior)
                        break;

                    Venta venta = ventasCargadas[indiceVentaImpresion];
                    x = izquierda;

                    string[] valores =
                    {
                        venta.Id.ToString(),
                        venta.UsuarioId.ToString(),
                        venta.NombreUsuario ?? "",
                        venta.Fecha.ToString("dd/MM/yyyy HH:mm"),
                        venta.Total.ToString("C2")
                    };

                    if (indiceVentaImpresion % 2 == 0)
                    {
                        g.FillRectangle(
                            pincelGris,
                            izquierda,
                            y,
                            ancho,
                            altoFila);
                    }

                    for (int i = 0; i < valores.Length; i++)
                    {
                        RectangleF rectangulo =
                            new RectangleF(x + 4, y, anchos[i] - 8, altoFila);

                        g.DrawString(
                            valores[i],
                            fuenteNormal,
                            pincelTexto,
                            rectangulo,
                            new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center,
                                Trimming = StringTrimming.EllipsisCharacter
                            });

                        x += anchos[i];
                    }

                    g.DrawLine(lapiz, izquierda, y + altoFila, derecha, y + altoFila);
                    y += altoFila;
                    indiceVentaImpresion++;
                }

                if (indiceVentaImpresion < ventasCargadas.Count)
                {
                    e.HasMorePages = true;
                    return;
                }

                if (!resumenImpreso)
                {
                    if (y + 65 > e.MarginBounds.Bottom)
                    {
                        e.HasMorePages = true;
                        return;
                    }

                    y += 20;

                    decimal totalRecaudado =
                        ventasCargadas.Sum(v => v.Total);

                    g.DrawLine(lapiz, izquierda, y, derecha, y);
                    y += 15;

                    g.DrawString(
                        "TOTAL DE VENTAS: " + ventasCargadas.Count,
                        fuenteTotal, pincelAzul, izquierda, y);
                    y += 28;

                    g.DrawString(
                        "TOTAL RECAUDADO: " + totalRecaudado.ToString("C2"),
                        fuenteTotal, pincelVerde, izquierda, y);

                    resumenImpreso = true;
                }

                string pie =
                    "TechSolutions | Reporte de ventas | Página " + numeroPagina;

                SizeF tamañoPie = g.MeasureString(pie, fuenteNormal);

                g.DrawString(
                    pie,
                    fuenteNormal,
                    pincelTexto,
                    derecha - tamañoPie.Width,
                    e.MarginBounds.Bottom + 10);
            }

            e.HasMorePages = false;
        }

        private async void BtnVerDetalles_Click(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow == null)
            {
                MessageBox.Show(
                    "Seleccione una venta primero.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            int ventaId = Convert.ToInt32(
                dgvVentas.CurrentRow.Cells["Id"].Value);

            btnVerDetalles.Enabled = false;

            try
            {
                List<DetalleVenta> detalles = await Task.Run(
                    () => ventaServicio.ObtenerDetalles(ventaId));

                if (IsDisposed || Disposing)
                    return;

                MostrarDetalles(
                    ventaId,
                    detalles ?? new List<DetalleVenta>());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al consultar los detalles:\n" + ex.Message,
                    "Detalles de venta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed && !Disposing)
                    btnVerDetalles.Enabled = true;
            }
        }

        private void MostrarDetalles(
            int ventaId,
            List<DetalleVenta> detalles)
        {
            Form ventana = new Form();

            ventana.Text = "TechSolutions | Detalles de venta";
            ventana.Size = new Size(850, 510);
            ventana.MinimumSize = new Size(850, 510);
            ventana.StartPosition = FormStartPosition.CenterParent;
            ventana.FormBorderStyle = FormBorderStyle.FixedSingle;
            ventana.MaximizeBox = false;
            ventana.BackColor = fondo;

            Panel encabezado = new Panel();
            encabezado.Dock = DockStyle.Top;
            encabezado.Height = 90;
            encabezado.BackColor = azulOscuro;
            ventana.Controls.Add(encabezado);

            Label titulo = new Label();
            titulo.Text = "DETALLE DE LA VENTA N° " + ventaId;
            titulo.Font = new Font("Segoe UI", 17, FontStyle.Bold);
            titulo.ForeColor = Color.White;
            titulo.Location = new Point(25, 25);
            titulo.AutoSize = true;
            encabezado.Controls.Add(titulo);

            Panel panelTabla = new Panel();
            panelTabla.Location = new Point(25, 115);
            panelTabla.Size = new Size(785, 250);
            panelTabla.BackColor = Color.White;
            ventana.Controls.Add(panelTabla);

            DataGridView dgvDetalles = new DataGridView();
            dgvDetalles.Location = new Point(15, 15);
            dgvDetalles.Size = new Size(755, 220);
            dgvDetalles.ReadOnly = true;
            dgvDetalles.AllowUserToAddRows = false;
            dgvDetalles.AllowUserToDeleteRows = false;
            dgvDetalles.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvDetalles.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvDetalles.MultiSelect = false;
            dgvDetalles.RowHeadersVisible = false;
            dgvDetalles.BackgroundColor = Color.White;
            dgvDetalles.BorderStyle = BorderStyle.None;
            dgvDetalles.EnableHeadersVisualStyles = false;
            dgvDetalles.ColumnHeadersHeight = 40;
            dgvDetalles.RowTemplate.Height = 35;
            dgvDetalles.GridColor = Color.FromArgb(230, 234, 240);

            dgvDetalles.ColumnHeadersDefaultCellStyle.BackColor = azulOscuro;
            dgvDetalles.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDetalles.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            dgvDetalles.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvDetalles.DefaultCellStyle.ForeColor = texto;
            dgvDetalles.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(218, 232, 250);
            dgvDetalles.DefaultCellStyle.SelectionForeColor = texto;

            dgvDetalles.Columns.Add("Producto", "PRODUCTO");
            dgvDetalles.Columns.Add("Cantidad", "CANTIDAD");
            dgvDetalles.Columns.Add("Precio", "PRECIO UNITARIO");
            dgvDetalles.Columns.Add("Subtotal", "SUBTOTAL");

            foreach (DetalleVenta detalle in detalles)
            {
                dgvDetalles.Rows.Add(
                    detalle.NombreProducto,
                    detalle.Cantidad,
                    detalle.PrecioUnitario.ToString("C2"),
                    detalle.Subtotal.ToString("C2"));
            }

            panelTabla.Controls.Add(dgvDetalles);

            decimal total = detalles.Sum(d => d.Subtotal);

            Label lblTotal = new Label();
            lblTotal.Text = "TOTAL: " + total.ToString("C2");
            lblTotal.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTotal.ForeColor = verde;
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(480, 385);
            ventana.Controls.Add(lblTotal);

            Button btnCerrarDetalles = CrearBoton(
                "CERRAR",
                Color.FromArgb(95, 105, 120));

            btnCerrarDetalles.Location = new Point(665, 420);
            btnCerrarDetalles.Size = new Size(145, 40);
            btnCerrarDetalles.Click += (s, e) => ventana.Close();

            ventana.Controls.Add(btnCerrarDetalles);

            if (detalles.Count == 0)
            {
                MessageBox.Show(
                    "Esta venta no tiene detalles registrados.",
                    "Información",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            ventana.ShowDialog(this);
        }
    }
}
