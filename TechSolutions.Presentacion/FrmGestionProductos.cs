
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using TechSolutions.Entidades.Productos;
using TechSolutions.Negocio.Servicios;

namespace TechSolutions.Presentacion
{
    public class FrmGestionProductos : Form
    {
        private ProductoServicio servicio;

        private TextBox txtNombre;
        private TextBox txtCategoria;
        private TextBox txtPrecio;
        private TextBox txtStock;

        private DataGridView dgvProductos;

        private Button btnRegistrar;
        private Button btnEditar;
        private Button btnEliminar;
        private Button btnActualizar;
        private Button btnLimpiar;
        private Button btnCerrar;
        private Button btnVistaPrevia;
        private Button btnExportarPDF;

        private Label lblTotal;
        private Label lblStock;
        private Label lblValor;

        private int productoSeleccionado = 0;
        private bool cargandoProductos = false;

        // IMPRESION Y EXPORTACION PDF
        private PrintDocument documentoImpresion;
        private List<Producto> productosImpresion;
        private int indiceImpresion = 0;

        private readonly Color azulOscuro =
            Color.FromArgb(18, 36, 58);

        private readonly Color azul =
            Color.FromArgb(35, 90, 155);

        private readonly Color fondo =
            Color.FromArgb(241, 245, 250);

        public FrmGestionProductos()
        {
            servicio = new ProductoServicio();

            InicializarComponentes();
            ConfigurarImpresion();
            CargarProductos();
        }

        // ==========================================
        // INTERFAZ PRINCIPAL
        // ==========================================

        private void InicializarComponentes()
        {
            Text = "TechSolutions | Gestión de productos";
            ClientSize = new Size(1150, 800);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = fondo;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Font = new Font("Segoe UI", 10);

            // ENCABEZADO
            Panel panelSuperior = new Panel();
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Height = 100;
            panelSuperior.BackColor = azulOscuro;

            Label lblTitulo = new Label();
            lblTitulo.Text = "GESTIÓN DE PRODUCTOS";
            lblTitulo.Font = new Font(
                "Segoe UI", 21, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(35, 15);

            Label lblSubtitulo = new Label();
            lblSubtitulo.Text =
                "Administra el catálogo, precios y existencias de productos";
            lblSubtitulo.Font = new Font("Segoe UI", 10);
            lblSubtitulo.ForeColor =
                Color.FromArgb(190, 205, 225);
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Location = new Point(39, 65);

            Label lblMarca = new Label();
            lblMarca.Text = "INVENTARIO";
            lblMarca.Font = new Font(
                "Segoe UI", 10, FontStyle.Bold);
            lblMarca.ForeColor = Color.White;
            lblMarca.BackColor = azul;
            lblMarca.TextAlign = ContentAlignment.MiddleCenter;
            lblMarca.Size = new Size(140, 40);
            lblMarca.Location = new Point(970, 30);

            panelSuperior.Controls.Add(lblTitulo);
            panelSuperior.Controls.Add(lblSubtitulo);
            panelSuperior.Controls.Add(lblMarca);

            Controls.Add(panelSuperior);

            // TARJETAS DE RESUMEN
            Panel tarjetaTotal = CrearTarjeta(
                "TOTAL DE PRODUCTOS",
                "0",
                Color.FromArgb(35, 90, 155),
                30,
                out lblTotal);

            Panel tarjetaStock = CrearTarjeta(
                "UNIDADES EN STOCK",
                "0",
                Color.FromArgb(35, 150, 105),
                405,
                out lblStock);

            Panel tarjetaValor = CrearTarjeta(
                "VALOR DEL INVENTARIO",
                "S/ 0.00",
                Color.FromArgb(210, 145, 35),
                780,
                out lblValor);

            Controls.Add(tarjetaTotal);
            Controls.Add(tarjetaStock);
            Controls.Add(tarjetaValor);

            // PANEL DE FORMULARIO
            Panel panelFormulario = new Panel();
            panelFormulario.Location = new Point(30, 220);
            panelFormulario.Size = new Size(1090, 190);
            panelFormulario.BackColor = Color.White;

            Label lblFormulario = new Label();
            lblFormulario.Text = "DATOS DEL PRODUCTO";
            lblFormulario.Font = new Font(
                "Segoe UI", 11, FontStyle.Bold);
            lblFormulario.ForeColor = azulOscuro;
            lblFormulario.Location = new Point(20, 15);
            lblFormulario.AutoSize = true;

            panelFormulario.Controls.Add(lblFormulario);

            CrearCampo(
                panelFormulario,
                "Nombre del producto:",
                25, 55,
                out txtNombre);

            CrearCampo(
                panelFormulario,
                "Categoría:",
                290, 55,
                out txtCategoria);

            CrearCampo(
                panelFormulario,
                "Precio (S/):",
                555, 55,
                out txtPrecio);

            CrearCampo(
                panelFormulario,
                "Stock:",
                820, 55,
                out txtStock);

            Controls.Add(panelFormulario);

            // BOTONES DE OPERACIONES
            btnRegistrar = CrearBoton(
                "REGISTRAR",
                Color.FromArgb(35, 150, 105),
                25, 140);

            btnEditar = CrearBoton(
                "EDITAR",
                Color.FromArgb(35, 115, 190),
                210, 140);

            btnEliminar = CrearBoton(
                "ELIMINAR",
                Color.FromArgb(190, 65, 65),
                395, 140);

            btnLimpiar = CrearBoton(
                "LIMPIAR",
                Color.FromArgb(90, 100, 115),
                580, 140);

            panelFormulario.Controls.Add(btnRegistrar);
            panelFormulario.Controls.Add(btnEditar);
            panelFormulario.Controls.Add(btnEliminar);
            panelFormulario.Controls.Add(btnLimpiar);

            btnRegistrar.Click += BtnRegistrar_Click;
            btnEditar.Click += BtnEditar_Click;
            btnEliminar.Click += BtnEliminar_Click;
            btnLimpiar.Click += BtnLimpiar_Click;

            // PANEL DE TABLA
            Panel panelLista = new Panel();
            panelLista.Location = new Point(30, 430);
            panelLista.Size = new Size(1090, 265);
            panelLista.BackColor = Color.White;

            Label lblLista = new Label();
            lblLista.Text = "CATÁLOGO DE PRODUCTOS";
            lblLista.Font = new Font(
                "Segoe UI", 11, FontStyle.Bold);
            lblLista.ForeColor = azulOscuro;
            lblLista.Location = new Point(20, 12);
            lblLista.AutoSize = true;

            dgvProductos = new DataGridView();
            dgvProductos.Location = new Point(15, 50);
            dgvProductos.Size = new Size(1060, 200);

            dgvProductos.BackgroundColor = Color.White;
            dgvProductos.BorderStyle = BorderStyle.None;

            dgvProductos.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvProductos.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvProductos.MultiSelect = false;
            dgvProductos.ReadOnly = true;
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.AllowUserToDeleteRows = false;
            dgvProductos.RowHeadersVisible = false;
            dgvProductos.EnableHeadersVisualStyles = false;
            dgvProductos.AllowUserToResizeRows = false;

            dgvProductos.ColumnHeadersDefaultCellStyle.BackColor =
                azulOscuro;

            dgvProductos.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvProductos.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            dgvProductos.ColumnHeadersHeight = 38;

            dgvProductos.DefaultCellStyle.Font =
                new Font("Segoe UI", 9);

            dgvProductos.DefaultCellStyle.ForeColor =
                Color.FromArgb(45, 55, 70);

            dgvProductos.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(210, 230, 250);

            dgvProductos.DefaultCellStyle.SelectionForeColor =
                Color.Black;

            dgvProductos.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(245, 248, 252);

            dgvProductos.RowTemplate.Height = 35;

            dgvProductos.GridColor =
                Color.FromArgb(225, 230, 238);

            dgvProductos.SelectionChanged +=
                DgvProductos_SelectionChanged;

            panelLista.Controls.Add(lblLista);
            panelLista.Controls.Add(dgvProductos);

            Controls.Add(panelLista);

            // ======================================
            // BOTONES INFERIORES
            // ======================================

            btnActualizar = CrearBoton(
                "ACTUALIZAR LISTA",
                Color.FromArgb(35, 90, 155),
                30, 715);

            btnVistaPrevia = CrearBoton(
                "VISTA PREVIA",
                Color.FromArgb(110, 85, 170),
                390, 715);

            btnExportarPDF = CrearBoton(
                "EXPORTAR PDF",
                Color.FromArgb(35, 135, 120),
                730, 715);

            btnCerrar = CrearBoton(
                "CERRAR",
                Color.FromArgb(190, 65, 65),
                950, 715);

            btnActualizar.Size = new Size(210, 45);
            btnVistaPrevia.Size = new Size(210, 45);
            btnExportarPDF.Size = new Size(200, 45);
            btnCerrar.Size = new Size(170, 45);

            btnActualizar.Click += BtnActualizar_Click;
            btnVistaPrevia.Click += BtnVistaPrevia_Click;
            btnExportarPDF.Click += BtnExportarPDF_Click;
            btnCerrar.Click += (s, e) => Close();

            Controls.Add(btnActualizar);
            Controls.Add(btnVistaPrevia);
            Controls.Add(btnExportarPDF);
            Controls.Add(btnCerrar);
        }

        // ==========================================
        // TARJETAS DE RESUMEN
        // ==========================================

        private Panel CrearTarjeta(
            string titulo,
            string valor,
            Color color,
            int x,
            out Label lblValor)
        {
            Panel tarjeta = new Panel();
            tarjeta.Location = new Point(x, 120);
            tarjeta.Size = new Size(340, 80);
            tarjeta.BackColor = Color.White;

            Panel indicador = new Panel();
            indicador.Location = new Point(0, 0);
            indicador.Size = new Size(7, 80);
            indicador.BackColor = color;

            Label lblTitulo = new Label();
            lblTitulo.Text = titulo;
            lblTitulo.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);
            lblTitulo.ForeColor = Color.Gray;
            lblTitulo.Location = new Point(20, 12);
            lblTitulo.AutoSize = true;

            lblValor = new Label();
            lblValor.Text = valor;
            lblValor.Font = new Font(
                "Segoe UI", 21, FontStyle.Bold);
            lblValor.ForeColor = color;
            lblValor.Location = new Point(20, 34);
            lblValor.AutoSize = true;

            tarjeta.Controls.Add(indicador);
            tarjeta.Controls.Add(lblTitulo);
            tarjeta.Controls.Add(lblValor);

            return tarjeta;
        }

        // ==========================================
        // CAMPOS DEL FORMULARIO
        // ==========================================

        private void CrearCampo(
            Panel contenedor,
            string texto,
            int x,
            int y,
            out TextBox txt)
        {
            Label lbl = new Label();
            lbl.Text = texto;
            lbl.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);
            lbl.ForeColor = azulOscuro;
            lbl.Location = new Point(x, y);
            lbl.AutoSize = true;

            txt = new TextBox();
            txt.Location = new Point(x, y + 27);
            txt.Size = new Size(235, 30);
            txt.Font = new Font("Segoe UI", 10);
            txt.BorderStyle = BorderStyle.FixedSingle;

            contenedor.Controls.Add(lbl);
            contenedor.Controls.Add(txt);
        }

        // ==========================================
        // CREAR BOTONES
        // ==========================================

        private Button CrearBoton(
            string texto,
            Color color,
            int x,
            int y)
        {
            Button boton = new Button();

            boton.Text = texto;
            boton.Location = new Point(x, y);
            boton.Size = new Size(165, 40);
            boton.BackColor = color;
            boton.ForeColor = Color.White;
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);
            boton.Cursor = Cursors.Hand;

            boton.MouseEnter += (s, e) =>
            {
                boton.BackColor =
                    ControlPaint.Dark(color, 0.12f);
            };

            boton.MouseLeave += (s, e) =>
            {
                boton.BackColor = color;
            };

            return boton;
        }

        // ==========================================
        // CARGAR PRODUCTOS
        // ==========================================

        private void CargarProductos()
        {
            try
            {
                cargandoProductos = true;

                List<Producto> lista =
                    servicio.ObtenerTodos();

                dgvProductos.DataSource = null;
                dgvProductos.DataSource = lista;

                if (dgvProductos.Columns["Id"] != null)
                    dgvProductos.Columns["Id"].HeaderText = "Código";

                if (dgvProductos.Columns["Nombre"] != null)
                    dgvProductos.Columns["Nombre"].HeaderText =
                        "Producto";

                if (dgvProductos.Columns["Categoria"] != null)
                    dgvProductos.Columns["Categoria"].HeaderText =
                        "Categoría";

                if (dgvProductos.Columns["Precio"] != null)
                {
                    dgvProductos.Columns["Precio"].HeaderText =
                        "Precio";

                    dgvProductos.Columns["Precio"].DefaultCellStyle.Format =
                        "C2";

                    dgvProductos.Columns["Precio"].DefaultCellStyle.FormatProvider =
                        CultureInfo.GetCultureInfo("es-PE");
                }

                if (dgvProductos.Columns["Stock"] != null)
                    dgvProductos.Columns["Stock"].HeaderText = "Stock";

                dgvProductos.ClearSelection();
                dgvProductos.CurrentCell = null;

                productoSeleccionado = 0;

                ActualizarResumen(lista);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar productos: " + ex.Message,
                    "TechSolutions",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                cargandoProductos = false;
            }
        }

        // ==========================================
        // ACTUALIZAR RESUMEN
        // ==========================================

        private void ActualizarResumen(List<Producto> lista)
        {
            int total = lista.Count;
            int stockTotal = lista.Sum(p => p.Stock);

            decimal valorTotal =
                lista.Sum(p => p.Precio * p.Stock);

            lblTotal.Text = total.ToString();
            lblStock.Text = stockTotal.ToString();

            lblValor.Text = valorTotal.ToString(
                "C2",
                CultureInfo.GetCultureInfo("es-PE"));
        }

        // ==========================================
        // SELECCIONAR PRODUCTO
        // ==========================================

        private void DgvProductos_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (cargandoProductos)
                return;

            if (dgvProductos.SelectedRows.Count == 0)
                return;

            Producto producto =
                dgvProductos.SelectedRows[0].DataBoundItem
                as Producto;

            if (producto == null)
                return;

            productoSeleccionado = producto.Id;

            txtNombre.Text = producto.Nombre;
            txtCategoria.Text = producto.Categoria;

            txtPrecio.Text =
                producto.Precio.ToString(
                    "0.00",
                    CultureInfo.CurrentCulture);

            txtStock.Text = producto.Stock.ToString();
        }

        // ==========================================
        // REGISTRAR PRODUCTO
        // ==========================================

        private void BtnRegistrar_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                Producto producto = ObtenerDatosFormulario();

                servicio.Registrar(producto);

                MessageBox.Show(
                    "Producto registrado correctamente.",
                    "TechSolutions",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LimpiarFormulario();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error al registrar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // ==========================================
        // EDITAR PRODUCTO
        // ==========================================

        private void BtnEditar_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (productoSeleccionado <= 0)
                {
                    MessageBox.Show(
                        "Seleccione un producto de la tabla.");
                    return;
                }

                Producto producto = ObtenerDatosFormulario();

                producto.Id = productoSeleccionado;

                servicio.Actualizar(producto);

                MessageBox.Show(
                    "Producto actualizado correctamente.",
                    "TechSolutions",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LimpiarFormulario();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error al editar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // ==========================================
        // ELIMINAR PRODUCTO
        // ==========================================

        private void BtnEliminar_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (productoSeleccionado <= 0)
                {
                    MessageBox.Show(
                        "Seleccione un producto.");
                    return;
                }

                DialogResult respuesta = MessageBox.Show(
                    "¿Está seguro de eliminar este producto?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (respuesta == DialogResult.Yes)
                {
                    servicio.Eliminar(productoSeleccionado);

                    MessageBox.Show(
                        "Producto eliminado correctamente.",
                        "TechSolutions",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LimpiarFormulario();
                    CargarProductos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo eliminar el producto. " +
                    "Puede estar relacionado con una venta.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // ==========================================
        // ACTUALIZAR LISTA
        // ==========================================

        private void BtnActualizar_Click(
            object sender,
            EventArgs e)
        {
            LimpiarFormulario();
            CargarProductos();
        }

        // ==========================================
        // LIMPIAR FORMULARIO
        // ==========================================

        private void BtnLimpiar_Click(
            object sender,
            EventArgs e)
        {
            LimpiarFormulario();
        }

        // ==========================================
        // OBTENER DATOS DEL FORMULARIO
        // ==========================================

        private Producto ObtenerDatosFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                throw new Exception(
                    "Ingrese el nombre del producto.");

            if (string.IsNullOrWhiteSpace(txtCategoria.Text))
                throw new Exception(
                    "Ingrese la categoría del producto.");

            decimal precio;
            int stock;

            string textoPrecio = txtPrecio.Text.Trim();

            bool precioValido = decimal.TryParse(
                textoPrecio,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out precio);

            if (!precioValido)
            {
                precioValido = decimal.TryParse(
                    textoPrecio,
                    NumberStyles.Number,
                    CultureInfo.GetCultureInfo("es-PE"),
                    out precio);
            }

            if (!precioValido)
                throw new Exception("Ingrese un precio válido.");

            if (precio <= 0)
                throw new Exception(
                    "El precio debe ser mayor que cero.");

            if (!int.TryParse(
                txtStock.Text.Trim(),
                out stock))
            {
                throw new Exception("Ingrese un stock válido.");
            }

            if (stock < 0)
                throw new Exception(
                    "El stock no puede ser negativo.");

            Producto producto = new Producto();

            producto.Nombre = txtNombre.Text.Trim();
            producto.Categoria = txtCategoria.Text.Trim();
            producto.Precio = precio;
            producto.Stock = stock;

            return producto;
        }

        // ==========================================
        // LIMPIAR CAMPOS
        // ==========================================

        private void LimpiarFormulario()
        {
            productoSeleccionado = 0;

            txtNombre.Clear();
            txtCategoria.Clear();
            txtPrecio.Clear();
            txtStock.Clear();

            dgvProductos.ClearSelection();
            dgvProductos.CurrentCell = null;

            txtNombre.Focus();
        }

        // ==========================================
        // CONFIGURAR IMPRESION
        // ==========================================

        private void ConfigurarImpresion()
        {
            documentoImpresion = new PrintDocument();

            documentoImpresion.DocumentName =
                "Inventario de productos - TechSolutions";

            documentoImpresion.DefaultPageSettings.Landscape = true;

            documentoImpresion.DefaultPageSettings.Margins =
                new Margins(45, 45, 45, 45);

            documentoImpresion.PrintPage +=
                DocumentoImpresion_PrintPage;

            documentoImpresion.BeginPrint += (s, e) =>
            {
                indiceImpresion = 0;
            };
        }

        // ==========================================
        // PREPARAR INVENTARIO
        // ==========================================

        private void PrepararInventarioImpresion()
        {
            productosImpresion = servicio.ObtenerTodos()
                .OrderBy(p => p.Nombre)
                .ToList();

            indiceImpresion = 0;
        }

        // ==========================================
        // BOTON VISTA PREVIA
        // ==========================================

        private void BtnVistaPrevia_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                PrepararInventarioImpresion();

                if (productosImpresion == null ||
                    productosImpresion.Count == 0)
                {
                    MessageBox.Show(
                        "No existen productos para mostrar.",
                        "Inventario",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                MostrarVistaPrevia();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al generar la vista previa:\n" +
                    ex.Message,
                    "TechSolutions",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // MOSTRAR VISTA PREVIA
        // ==========================================

        private void MostrarVistaPrevia()
        {
            indiceImpresion = 0;

            using (PrintPreviewDialog vista =
                new PrintPreviewDialog())
            {
                vista.Document = documentoImpresion;
                vista.Width = 1000;
                vista.Height = 700;

                vista.StartPosition =
                    FormStartPosition.CenterScreen;

                vista.ShowIcon = false;

                vista.Text =
                    "Vista previa del inventario - TechSolutions";

                vista.ShowDialog(this);
            }

            indiceImpresion = 0;
        }

        // ==========================================
        // BOTON EXPORTAR PDF
        // ==========================================

        private void BtnExportarPDF_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                PrepararInventarioImpresion();

                if (productosImpresion == null ||
                    productosImpresion.Count == 0)
                {
                    MessageBox.Show(
                        "No existen productos para exportar.",
                        "Inventario",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                using (SaveFileDialog guardar = new SaveFileDialog())
                {
                    guardar.Title = "Exportar inventario a PDF";
                    guardar.Filter = "Archivo PDF (*.pdf)|*.pdf";
                    guardar.FileName =
                        "Reporte_Inventario_TechSolutions_" +
                        DateTime.Now.ToString("yyyyMMdd_HHmm") +
                        ".pdf";

                    guardar.DefaultExt = "pdf";
                    guardar.AddExtension = true;

                    if (guardar.ShowDialog(this) != DialogResult.OK)
                        return;

                    bool impresoraEncontrada = false;

                    foreach (string impresora in
                        PrinterSettings.InstalledPrinters)
                    {
                        if (impresora.Equals(
                            "Microsoft Print to PDF",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            documentoImpresion.PrinterSettings.PrinterName =
                                impresora;

                            impresoraEncontrada = true;
                            break;
                        }
                    }

                    if (!impresoraEncontrada)
                    {
                        MessageBox.Show(
                            "No se encontró Microsoft Print to PDF.\n\n" +
                            "Verifique que la impresora PDF esté habilitada en Windows.",
                            "Impresora PDF no disponible",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }

                    documentoImpresion.PrinterSettings.PrintToFile = true;

                    documentoImpresion.PrinterSettings.PrintFileName =
                        guardar.FileName;

                    documentoImpresion.DefaultPageSettings.Landscape = true;

                    indiceImpresion = 0;

                    documentoImpresion.Print();

                    indiceImpresion = 0;

                    MessageBox.Show(
                        "El reporte de inventario se ha exportado correctamente.\n\n" +
                        "Ubicación:\n" + guardar.FileName,
                        "PDF generado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al exportar el inventario a PDF:\n\n" +
                    ex.Message,
                    "Error de exportación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                indiceImpresion = 0;
            }
        }

        // ==========================================
        // GENERAR PAGINAS DEL INVENTARIO
        // ==========================================

        private void DocumentoImpresion_PrintPage(
            object sender,
            PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle area = e.MarginBounds;

            CultureInfo cultura =
                CultureInfo.GetCultureInfo("es-PE");

            using (Font fuenteTitulo =
                new Font("Arial", 20, FontStyle.Bold))
            using (Font fuenteReporte =
                new Font("Arial", 13, FontStyle.Bold))
            using (Font fuenteSubtitulo =
                new Font("Arial", 10, FontStyle.Regular))
            using (Font fuenteEncabezado =
                new Font("Arial", 9, FontStyle.Bold))
            using (Font fuenteNormal =
                new Font("Arial", 9, FontStyle.Regular))
            using (Font fuenteResumen =
                new Font("Arial", 10, FontStyle.Bold))
            using (Pen lapiz =
                new Pen(Color.FromArgb(190, 200, 215), 1))
            using (SolidBrush pincelAzul =
                new SolidBrush(Color.FromArgb(18, 36, 58)))
            using (SolidBrush pincelGris =
                new SolidBrush(Color.FromArgb(240, 244, 249)))
            using (SolidBrush pincelBlanco =
                new SolidBrush(Color.White))
            using (StringFormat formato =
                new StringFormat())
            {
                formato.Trimming =
                    StringTrimming.EllipsisCharacter;

                int y = area.Top;

                g.DrawString(
                    "TECHSOLUTIONS",
                    fuenteTitulo,
                    pincelAzul,
                    area.Left,
                    y);

                y += 35;

                g.DrawString(
                    "REPORTE GENERAL DE INVENTARIO",
                    fuenteReporte,
                    Brushes.Black,
                    area.Left,
                    y);

                y += 27;

                g.DrawString(
                    "Fecha de emisión: " +
                    DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    fuenteSubtitulo,
                    Brushes.DimGray,
                    area.Left,
                    y);

                y += 30;

                int[] anchos =
                {
                    70,
                    220,
                    190,
                    150,
                    100
                };

                string[] titulos =
                {
                    "CODIGO",
                    "PRODUCTO",
                    "CATEGORIA",
                    "PRECIO",
                    "STOCK"
                };

                int anchoDisponible = area.Width;
                int sumaAnchos = anchos.Sum();

                float escala =
                    (float)anchoDisponible / sumaAnchos;

                int[] anchosFinales = anchos
                    .Select(a => (int)(a * escala))
                    .ToArray();

                anchosFinales[4] +=
                    anchoDisponible - anchosFinales.Sum();

                int x = area.Left;
                int altoFila = 28;

                for (int i = 0; i < titulos.Length; i++)
                {
                    Rectangle rect = new Rectangle(
                        x,
                        y,
                        anchosFinales[i],
                        altoFila);

                    g.FillRectangle(pincelAzul, rect);
                    g.DrawRectangle(lapiz, rect);

                    g.DrawString(
                        titulos[i],
                        fuenteEncabezado,
                        pincelBlanco,
                        rect,
                        formato);

                    x += anchosFinales[i];
                }

                y += altoFila;

                int limiteInferior = area.Bottom - 65;

                while (indiceImpresion <
                       productosImpresion.Count &&
                       y + altoFila <= limiteInferior)
                {
                    Producto producto =
                        productosImpresion[indiceImpresion];

                    string[] valores =
                    {
                        producto.Id.ToString(),
                        producto.Nombre,
                        producto.Categoria,
                        producto.Precio.ToString("C2", cultura),
                        producto.Stock.ToString()
                    };

                    x = area.Left;

                    for (int i = 0; i < valores.Length; i++)
                    {
                        Rectangle rect = new Rectangle(
                            x,
                            y,
                            anchosFinales[i],
                            altoFila);

                        if (indiceImpresion % 2 == 0)
                        {
                            g.FillRectangle(pincelGris, rect);
                        }

                        g.DrawRectangle(lapiz, rect);

                        g.DrawString(
                            valores[i] ?? "",
                            fuenteNormal,
                            Brushes.Black,
                            rect,
                            formato);

                        x += anchosFinales[i];
                    }

                    y += altoFila;
                    indiceImpresion++;
                }

                if (indiceImpresion <
                    productosImpresion.Count)
                {
                    e.HasMorePages = true;
                    return;
                }

                if (y + 90 > area.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }

                y += 20;

                int totalProductos =
                    productosImpresion.Count;

                int totalUnidades =
                    productosImpresion.Sum(p => p.Stock);

                decimal valorTotal =
                    productosImpresion.Sum(
                        p => p.Precio * p.Stock);

                g.DrawLine(
                    lapiz,
                    area.Left,
                    y,
                    area.Right,
                    y);

                y += 15;

                g.DrawString(
                    "RESUMEN DEL INVENTARIO",
                    fuenteResumen,
                    pincelAzul,
                    area.Left,
                    y);

                y += 25;

                g.DrawString(
                    "Total de productos: " + totalProductos,
                    fuenteNormal,
                    Brushes.Black,
                    area.Left,
                    y);

                g.DrawString(
                    "Unidades en stock: " + totalUnidades,
                    fuenteNormal,
                    Brushes.Black,
                    area.Left + 300,
                    y);

                y += 23;

                g.DrawString(
                    "Valor total del inventario: " +
                    valorTotal.ToString("C2", cultura),
                    fuenteResumen,
                    Brushes.DarkGreen,
                    area.Left,
                    y);

                y += 30;

                g.DrawString(
                    "Documento generado por el sistema TechSolutions.",
                    fuenteSubtitulo,
                    Brushes.DimGray,
                    area.Left,
                    y);

                e.HasMorePages = false;
            }
        }
    }
}