
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using TechSolutions.Entidades.Productos;
using TechSolutions.Entidades.Ventas;
using TechSolutions.Negocio.Servicios;

namespace TechSolutions.Presentacion
{
    public class FrmRegistrarVenta : Form
    {
        private int usuarioId;

        private ComboBox cmbProductos;
        private NumericUpDown nudCantidad;
        private DataGridView dgvDetalle;
        private Label lblTotal;
        private Label lblStock;

        private Button btnAgregar;
        private Button btnEliminar;
        private Button btnRegistrar;
        private Button btnCerrar;

        private Button btnVistaPreviaComprobante;
        private Button btnExportarPDF;

        private ProductoServicio productoServicio;
        private VentaServicio ventaServicio;

        private List<Producto> productos;

        // COMPROBANTE
        private PrintDocument documentoComprobante;
        private List<DetalleVenta> detallesComprobante;

        private int numeroVentaComprobante;
        private decimal totalComprobante;
        private DateTime fechaComprobante;
        private int usuarioComprobante;

        private bool comprobanteDisponible = false;

        private Color azulOscuro =
            Color.FromArgb(23, 37, 61);

        private Color azul =
            Color.FromArgb(37, 99, 180);

        private Color fondo =
            Color.FromArgb(242, 245, 250);

        private Color texto =
            Color.FromArgb(45, 55, 72);

        private Color verde =
            Color.FromArgb(20, 145, 105);

        // ==========================================
        // CONSTRUCTOR
        // ==========================================

        public FrmRegistrarVenta(int usuarioId)
        {
            if (usuarioId <= 0)
                throw new ArgumentException(
                    "El identificador del usuario no es válido.");

            this.usuarioId = usuarioId;

            productoServicio = new ProductoServicio();
            ventaServicio = new VentaServicio();

            productos = new List<Producto>();
            detallesComprobante = new List<DetalleVenta>();

            documentoComprobante = new PrintDocument();

            documentoComprobante.DocumentName =
                "Comprobante de venta TechSolutions";

            documentoComprobante.DefaultPageSettings.Margins =
                new Margins(50, 50, 50, 50);

            documentoComprobante.DefaultPageSettings.Landscape =
                false;

            documentoComprobante.PrintPage +=
                DocumentoComprobante_PrintPage;

            InicializarFormulario();
            CargarProductos();
        }

        // ==========================================
        // DISEÑO DEL FORMULARIO
        // ==========================================

        private void InicializarFormulario()
        {
            Text = "TechSolutions | Registrar venta";

            ClientSize = new Size(1100, 790);
            MinimumSize = new Size(1100, 790);

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            BackColor = fondo;
            Font = new Font("Segoe UI", 10);

            // ENCABEZADO

            Panel encabezado = new Panel();

            encabezado.Dock = DockStyle.Top;
            encabezado.Height = 105;
            encabezado.BackColor = azulOscuro;

            Label titulo = new Label();

            titulo.Text = "Nueva venta";
            titulo.Font = new Font("Segoe UI", 23, FontStyle.Bold);
            titulo.ForeColor = Color.White;
            titulo.Location = new Point(30, 17);
            titulo.AutoSize = true;

            Label subtitulo = new Label();

            subtitulo.Text =
                "Selecciona los productos y registra la operación";

            subtitulo.Font = new Font("Segoe UI", 10);
            subtitulo.ForeColor = Color.FromArgb(195, 210, 230);
            subtitulo.Location = new Point(34, 65);
            subtitulo.AutoSize = true;

            encabezado.Controls.Add(titulo);
            encabezado.Controls.Add(subtitulo);

            Controls.Add(encabezado);

            // PANEL PRODUCTOS

            Panel panelProductos = new Panel();

            panelProductos.Location = new Point(25, 130);
            panelProductos.Size = new Size(365, 465);
            panelProductos.BackColor = Color.White;
            panelProductos.BorderStyle = BorderStyle.FixedSingle;

            Controls.Add(panelProductos);

            Label lblSeleccion = new Label();

            lblSeleccion.Text = "SELECCIONAR PRODUCTO";
            lblSeleccion.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblSeleccion.ForeColor = azulOscuro;
            lblSeleccion.Location = new Point(20, 22);
            lblSeleccion.AutoSize = true;

            panelProductos.Controls.Add(lblSeleccion);

            Label lblProducto = new Label();

            lblProducto.Text = "Producto disponible";
            lblProducto.Location = new Point(22, 85);
            lblProducto.AutoSize = true;
            lblProducto.ForeColor = texto;

            panelProductos.Controls.Add(lblProducto);

            cmbProductos = new ComboBox();

            cmbProductos.Location = new Point(22, 115);
            cmbProductos.Size = new Size(315, 35);
            cmbProductos.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProductos.Font = new Font("Segoe UI", 10);

            cmbProductos.SelectedIndexChanged +=
                CmbProductos_SelectedIndexChanged;

            panelProductos.Controls.Add(cmbProductos);

            Label lblCantidad = new Label();

            lblCantidad.Text = "Cantidad";
            lblCantidad.Location = new Point(22, 180);
            lblCantidad.AutoSize = true;
            lblCantidad.ForeColor = texto;

            panelProductos.Controls.Add(lblCantidad);

            nudCantidad = new NumericUpDown();

            nudCantidad.Location = new Point(22, 210);
            nudCantidad.Size = new Size(315, 35);
            nudCantidad.Minimum = 1;
            nudCantidad.Maximum = 1000;
            nudCantidad.Value = 1;
            nudCantidad.Font = new Font("Segoe UI", 11);

            panelProductos.Controls.Add(nudCantidad);

            Panel panelStock = new Panel();

            panelStock.Location = new Point(22, 280);
            panelStock.Size = new Size(315, 65);
            panelStock.BackColor = Color.FromArgb(232, 246, 240);

            panelProductos.Controls.Add(panelStock);

            lblStock = new Label();

            lblStock.Text = "Stock disponible: -";
            lblStock.Location = new Point(12, 18);
            lblStock.Size = new Size(290, 30);
            lblStock.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblStock.ForeColor = verde;

            panelStock.Controls.Add(lblStock);

            btnAgregar = CrearBoton(
                "+ AGREGAR PRODUCTO", azul);

            btnAgregar.Location = new Point(22, 385);
            btnAgregar.Size = new Size(315, 48);
            btnAgregar.Click += BtnAgregar_Click;

            panelProductos.Controls.Add(btnAgregar);

            // PANEL DETALLE

            Panel panelDetalle = new Panel();

            panelDetalle.Location = new Point(410, 130);
            panelDetalle.Size = new Size(665, 465);
            panelDetalle.BackColor = Color.White;
            panelDetalle.BorderStyle = BorderStyle.FixedSingle;

            Controls.Add(panelDetalle);

            Label lblDetalle = new Label();

            lblDetalle.Text = "DETALLE DE LA VENTA";
            lblDetalle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblDetalle.ForeColor = azulOscuro;
            lblDetalle.Location = new Point(20, 22);
            lblDetalle.AutoSize = true;

            panelDetalle.Controls.Add(lblDetalle);

            Label lblAyuda = new Label();

            lblAyuda.Text =
                "Productos agregados a esta operación";

            lblAyuda.Font = new Font("Segoe UI", 9);
            lblAyuda.ForeColor = Color.Gray;
            lblAyuda.Location = new Point(22, 53);
            lblAyuda.AutoSize = true;

            panelDetalle.Controls.Add(lblAyuda);

            dgvDetalle = new DataGridView();

            dgvDetalle.Location = new Point(20, 95);
            dgvDetalle.Size = new Size(625, 340);

            dgvDetalle.ReadOnly = true;
            dgvDetalle.AllowUserToAddRows = false;
            dgvDetalle.AllowUserToDeleteRows = false;

            dgvDetalle.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvDetalle.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvDetalle.MultiSelect = false;
            dgvDetalle.RowHeadersVisible = false;
            dgvDetalle.BackgroundColor = Color.White;
            dgvDetalle.BorderStyle = BorderStyle.None;

            dgvDetalle.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;

            dgvDetalle.EnableHeadersVisualStyles = false;
            dgvDetalle.ColumnHeadersHeight = 42;
            dgvDetalle.RowTemplate.Height = 38;

            dgvDetalle.GridColor =
                Color.FromArgb(230, 234, 240);

            dgvDetalle.ColumnHeadersDefaultCellStyle.BackColor =
                azulOscuro;

            dgvDetalle.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvDetalle.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            dgvDetalle.DefaultCellStyle.Font =
                new Font("Segoe UI", 9);

            dgvDetalle.DefaultCellStyle.ForeColor = texto;

            dgvDetalle.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(218, 232, 250);

            dgvDetalle.DefaultCellStyle.SelectionForeColor = texto;

            dgvDetalle.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(246, 248, 252);

            dgvDetalle.Columns.Add("ProductoId", "ID");
            dgvDetalle.Columns.Add("Producto", "PRODUCTO");
            dgvDetalle.Columns.Add("Precio", "PRECIO");
            dgvDetalle.Columns.Add("Cantidad", "CANT.");
            dgvDetalle.Columns.Add("Subtotal", "SUBTOTAL");

            dgvDetalle.Columns["ProductoId"].FillWeight = 35;
            dgvDetalle.Columns["Producto"].FillWeight = 155;
            dgvDetalle.Columns["Precio"].FillWeight = 75;
            dgvDetalle.Columns["Cantidad"].FillWeight = 55;
            dgvDetalle.Columns["Subtotal"].FillWeight = 85;

            panelDetalle.Controls.Add(dgvDetalle);

            // ======================================
            // PANEL INFERIOR
            // ======================================

            Panel panelInferior = new Panel();

            panelInferior.Location = new Point(25, 615);
            panelInferior.Size = new Size(1050, 135);
            panelInferior.BackColor = Color.White;
            panelInferior.BorderStyle = BorderStyle.FixedSingle;

            Controls.Add(panelInferior);

            btnEliminar = CrearBoton(
                "ELIMINAR PRODUCTO",
                Color.FromArgb(205, 70, 75));

            btnEliminar.Location = new Point(18, 18);
            btnEliminar.Size = new Size(190, 45);
            btnEliminar.Click += BtnEliminar_Click;

            panelInferior.Controls.Add(btnEliminar);

            lblTotal = new Label();

            lblTotal.Text = "TOTAL: S/ 0.00";
            lblTotal.Location = new Point(270, 15);
            lblTotal.Size = new Size(330, 50);
            lblTotal.Font = new Font("Segoe UI", 19, FontStyle.Bold);
            lblTotal.ForeColor = verde;
            lblTotal.TextAlign = ContentAlignment.MiddleRight;

            panelInferior.Controls.Add(lblTotal);

            btnRegistrar = CrearBoton(
                "REGISTRAR VENTA", verde);

            btnRegistrar.Location = new Point(640, 18);
            btnRegistrar.Size = new Size(190, 45);
            btnRegistrar.Click += BtnRegistrar_Click;

            panelInferior.Controls.Add(btnRegistrar);

            btnCerrar = CrearBoton(
                "CERRAR",
                Color.FromArgb(95, 105, 120));

            btnCerrar.Location = new Point(855, 18);
            btnCerrar.Size = new Size(155, 45);
            btnCerrar.Click += (s, e) => Close();

            panelInferior.Controls.Add(btnCerrar);

            // ======================================
            // BOTON VISTA PREVIA
            // ======================================

            btnVistaPreviaComprobante = CrearBoton(
                "VISTA PREVIA",
                Color.FromArgb(110, 85, 170));

            btnVistaPreviaComprobante.Location =
                new Point(250, 78);

            btnVistaPreviaComprobante.Size =
                new Size(220, 42);

            // HABILITADO DESDE EL INICIO
            btnVistaPreviaComprobante.Enabled = true;

            // EVENTO CLICK
            btnVistaPreviaComprobante.Click +=
                BtnVistaPreviaComprobante_Click;

            panelInferior.Controls.Add(
                btnVistaPreviaComprobante);

            // ======================================
            // BOTON EXPORTAR PDF
            // ======================================

            btnExportarPDF = CrearBoton(
                "EXPORTAR PDF",
                Color.FromArgb(35, 135, 120));

            btnExportarPDF.Location =
                new Point(510, 78);

            btnExportarPDF.Size =
                new Size(285, 42);

            // HABILITADO DESDE EL INICIO
            btnExportarPDF.Enabled = true;

            // EVENTO CLICK
            btnExportarPDF.Click +=
                BtnExportarPDF_Click;

            panelInferior.Controls.Add(btnExportarPDF);
        }

        // ==========================================
        // CREAR BOTONES
        // ==========================================

        private Button CrearBoton(
            string textoBoton,
            Color color)
        {
            Button boton = new Button();

            boton.Text = textoBoton;
            boton.BackColor = color;
            boton.ForeColor = Color.White;

            boton.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.Cursor = Cursors.Hand;

            return boton;
        }

        // ==========================================
        // CARGAR PRODUCTOS
        // ==========================================

        private void CargarProductos()
        {
            try
            {
                productos = productoServicio.ObtenerTodos();

                cmbProductos.DataSource = null;

                List<Producto> disponibles = productos
                    .Where(p => p.Stock > 0)
                    .ToList();

                cmbProductos.DataSource = disponibles;
                cmbProductos.DisplayMember = "Nombre";
                cmbProductos.ValueMember = "Id";

                if (cmbProductos.Items.Count == 0)
                {
                    lblStock.Text =
                        "No hay productos disponibles.";

                    btnAgregar.Enabled = false;
                }
                else
                {
                    btnAgregar.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar productos: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // MOSTRAR STOCK
        // ==========================================

        private void CmbProductos_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            Producto producto =
                cmbProductos.SelectedItem as Producto;

            if (producto != null)
            {
                lblStock.Text =
                    "Stock disponible: " + producto.Stock;

                nudCantidad.Value = 1;
            }
        }

        // ==========================================
        // AGREGAR PRODUCTO
        // ==========================================

        private void BtnAgregar_Click(
            object sender, EventArgs e)
        {
            try
            {
                Producto producto =
                    cmbProductos.SelectedItem as Producto;

                if (producto == null)
                {
                    MessageBox.Show(
                        "Seleccione un producto.",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                int cantidad = (int)nudCantidad.Value;

                List<Producto> inventarioActual =
                    productoServicio.ObtenerTodos();

                Producto productoActual =
                    inventarioActual.FirstOrDefault(
                        p => p.Id == producto.Id);

                if (productoActual == null)
                {
                    MessageBox.Show(
                        "El producto ya no existe.",
                        "Producto no encontrado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    CargarProductos();
                    return;
                }

                int cantidadExistente = 0;

                foreach (DataGridViewRow fila in dgvDetalle.Rows)
                {
                    if (Convert.ToInt32(
                        fila.Cells["ProductoId"].Value) == producto.Id)
                    {
                        cantidadExistente = Convert.ToInt32(
                            fila.Cells["Cantidad"].Value);

                        break;
                    }
                }

                int cantidadSolicitada =
                    cantidad + cantidadExistente;

                if (cantidadSolicitada > productoActual.Stock)
                {
                    MessageBox.Show(
                        "¡STOCK INSUFICIENTE!" +
                        "\n\nProducto: " + productoActual.Nombre +
                        "\nCantidad solicitada: " + cantidadSolicitada +
                        "\nStock disponible: " + productoActual.Stock +
                        "\n\nNo se puede agregar esta cantidad.",
                        "Control de inventario",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    lblStock.Text =
                        "Stock disponible: " + productoActual.Stock;

                    return;
                }

                bool encontrado = false;

                foreach (DataGridViewRow fila in dgvDetalle.Rows)
                {
                    if (Convert.ToInt32(
                        fila.Cells["ProductoId"].Value) == producto.Id)
                    {
                        int nuevaCantidad = Convert.ToInt32(
                            fila.Cells["Cantidad"].Value) + cantidad;

                        fila.Cells["Cantidad"].Value = nuevaCantidad;

                        fila.Cells["Subtotal"].Value =
                            (productoActual.Precio * nuevaCantidad)
                            .ToString("0.00");

                        encontrado = true;
                        break;
                    }
                }

                if (!encontrado)
                {
                    decimal subtotal =
                        productoActual.Precio * cantidad;

                    dgvDetalle.Rows.Add(
                        productoActual.Id,
                        productoActual.Nombre,
                        productoActual.Precio.ToString("0.00"),
                        cantidad,
                        subtotal.ToString("0.00"));
                }

                nudCantidad.Value = 1;
                CalcularTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al agregar el producto: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // CALCULAR TOTAL
        // ==========================================

        private void CalcularTotal()
        {
            decimal total = 0;

            foreach (DataGridViewRow fila in dgvDetalle.Rows)
            {
                total += Convert.ToDecimal(
                    fila.Cells["Subtotal"].Value);
            }

            lblTotal.Text =
                "TOTAL: S/ " + total.ToString("0.00");
        }

        // ==========================================
        // ELIMINAR PRODUCTO
        // ==========================================

        private void BtnEliminar_Click(
            object sender, EventArgs e)
        {
            if (dgvDetalle.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione un producto para eliminar.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            dgvDetalle.Rows.RemoveAt(
                dgvDetalle.SelectedRows[0].Index);

            CalcularTotal();
        }

        // ==========================================
        // REGISTRAR VENTA
        // ==========================================

        private void BtnRegistrar_Click(
            object sender, EventArgs e)
        {
            if (usuarioId <= 0)
            {
                MessageBox.Show(
                    "No se ha identificado al usuario que realiza la venta.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (dgvDetalle.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Agregue al menos un producto antes de registrar la venta.",
                    "Venta vacía",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                btnRegistrar.Enabled = false;

                List<Producto> inventarioActual =
                    productoServicio.ObtenerTodos();

                foreach (DataGridViewRow fila in dgvDetalle.Rows)
                {
                    int productoId = Convert.ToInt32(
                        fila.Cells["ProductoId"].Value);

                    int cantidad = Convert.ToInt32(
                        fila.Cells["Cantidad"].Value);

                    Producto productoActual =
                        inventarioActual.FirstOrDefault(
                            p => p.Id == productoId);

                    if (productoActual == null ||
                        productoActual.Stock < cantidad)
                    {
                        string nombre = productoActual == null
                            ? fila.Cells["Producto"].Value.ToString()
                            : productoActual.Nombre;

                        int stockDisponible = productoActual == null
                            ? 0
                            : productoActual.Stock;

                        MessageBox.Show(
                            "¡STOCK INSUFICIENTE!" +
                            "\n\nProducto: " + nombre +
                            "\nCantidad solicitada: " + cantidad +
                            "\nStock disponible: " + stockDisponible +
                            "\n\nLa venta no puede registrarse.",
                            "Control de inventario",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }
                }

                Venta venta = new Venta();

                venta.UsuarioId = usuarioId;
                venta.Fecha = DateTime.Now;
                venta.Detalles = new List<DetalleVenta>();

                decimal total = 0;

                foreach (DataGridViewRow fila in dgvDetalle.Rows)
                {
                    int productoId = Convert.ToInt32(
                        fila.Cells["ProductoId"].Value);

                    decimal precio = Convert.ToDecimal(
                        fila.Cells["Precio"].Value);

                    int cantidad = Convert.ToInt32(
                        fila.Cells["Cantidad"].Value);

                    decimal subtotal = precio * cantidad;

                    DetalleVenta detalle = new DetalleVenta();

                    detalle.ProductoId = productoId;
                    detalle.Cantidad = cantidad;
                    detalle.PrecioUnitario = precio;
                    detalle.Subtotal = subtotal;

                    venta.Detalles.Add(detalle);
                    total += subtotal;
                }

                venta.Total = total;

                // REGISTRAR VENTA EN SQL SERVER

                int ventaId =
                    ventaServicio.RegistrarVenta(venta);

                // GUARDAR DATOS DEL COMPROBANTE

                numeroVentaComprobante = ventaId;
                usuarioComprobante = usuarioId;
                fechaComprobante = venta.Fecha;
                totalComprobante = total;

                detallesComprobante = venta.Detalles
                    .Select(d => new DetalleVenta
                    {
                        ProductoId = d.ProductoId,

                        NombreProducto = dgvDetalle.Rows
                            .Cast<DataGridViewRow>()
                            .First(f =>
                                Convert.ToInt32(
                                    f.Cells["ProductoId"].Value)
                                == d.ProductoId)
                            .Cells["Producto"].Value.ToString(),

                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    })
                    .ToList();

                comprobanteDisponible = true;

                // MANTENER HABILITADOS LOS BOTONES

                btnVistaPreviaComprobante.Enabled = true;
                btnExportarPDF.Enabled = true;

                MessageBox.Show(
                    "¡VENTA REGISTRADA CORRECTAMENTE!" +
                    "\n\nNúmero de venta: " + ventaId +
                    "\nUsuario: " + usuarioId +
                    "\nTotal: S/ " + total.ToString("0.00") +
                    "\n\nEl stock fue actualizado correctamente." +
                    "\n\nAhora puede consultar la vista previa o exportar el PDF.",
                    "Venta completada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                dgvDetalle.Rows.Clear();
                CalcularTotal();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo registrar la venta." +
                    "\n\nDetalle del error: " + ex.Message,
                    "Error al registrar venta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnRegistrar.Enabled = true;
            }
        }

        // ==========================================
        // VALIDAR COMPROBANTE
        // ==========================================

        private bool ValidarComprobante()
        {
            if (!comprobanteDisponible ||
                detallesComprobante == null ||
                detallesComprobante.Count == 0)
            {
                MessageBox.Show(
                    "Primero debe registrar una venta para generar el comprobante.",
                    "Comprobante no disponible",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

        // ==========================================
        // CLICK VISTA PREVIA
        // ==========================================

        private void BtnVistaPreviaComprobante_Click(
            object sender, EventArgs e)
        {
            if (!ValidarComprobante())
                return;

            MostrarVistaPreviaComprobante();
        }

        // ==========================================
        // MOSTRAR VISTA PREVIA
        // ==========================================

        private void MostrarVistaPreviaComprobante()
        {
            try
            {
                using (PrintPreviewDialog vistaPrevia =
                    new PrintPreviewDialog())
                {
                    vistaPrevia.Document = documentoComprobante;

                    vistaPrevia.Width = 950;
                    vistaPrevia.Height = 750;

                    vistaPrevia.StartPosition =
                        FormStartPosition.CenterScreen;

                    vistaPrevia.Text =
                        "Vista previa del comprobante - TechSolutions";

                    vistaPrevia.ShowIcon = false;

                    vistaPrevia.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo mostrar el comprobante." +
                    "\n\nDetalle: " + ex.Message,
                    "Comprobante",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // ==========================================
        // CLICK EXPORTAR PDF
        // ==========================================

        private void BtnExportarPDF_Click(
            object sender, EventArgs e)
        {
            if (!ValidarComprobante())
                return;

            try
            {
                string impresoraPDF = "Microsoft Print to PDF";

                // COMPROBAR SI EXISTE LA IMPRESORA PDF

                bool impresoraDisponible =
                    PrinterSettings.InstalledPrinters
                    .Cast<string>()
                    .Any(p => string.Equals(
                        p,
                        impresoraPDF,
                        StringComparison.OrdinalIgnoreCase));

                if (!impresoraDisponible)
                {
                    MessageBox.Show(
                        "No se encontró Microsoft Print to PDF." +
                        "\n\nDebe habilitar esta impresora en Windows.",
                        "Impresora PDF no disponible",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // VENTANA PARA ELEGIR DONDE GUARDAR

                using (SaveFileDialog guardar = new SaveFileDialog())
                {
                    guardar.Title = "Guardar comprobante en PDF";

                    guardar.Filter =
                        "Archivo PDF (*.pdf)|*.pdf";

                    guardar.DefaultExt = "pdf";
                    guardar.AddExtension = true;

                    guardar.FileName =
                        "Comprobante_Venta_" +
                        numeroVentaComprobante + ".pdf";

                    guardar.OverwritePrompt = true;

                    if (guardar.ShowDialog(this) != DialogResult.OK)
                        return;

                    // GUARDAR CONFIGURACION ORIGINAL

                    PrinterSettings configuracionOriginal =
                        documentoComprobante.PrinterSettings;

                    PrintController controladorOriginal =
                        documentoComprobante.PrintController;

                    try
                    {
                        // CONFIGURAR IMPRESORA PDF

                        PrinterSettings configuracionPDF =
                            new PrinterSettings();

                        configuracionPDF.PrinterName =
                            impresoraPDF;

                        configuracionPDF.PrintToFile = true;

                        configuracionPDF.PrintFileName =
                            guardar.FileName;

                        if (!configuracionPDF.IsValid)
                        {
                            MessageBox.Show(
                                "No se pudo configurar Microsoft Print to PDF.",
                                "Error de impresora",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                            return;
                        }

                        documentoComprobante.PrinterSettings =
                            configuracionPDF;

                        documentoComprobante.PrintController =
                            new StandardPrintController();

                        // GENERAR PDF

                        documentoComprobante.Print();
                    }
                    finally
                    {
                        // RESTAURAR CONFIGURACION ORIGINAL

                        documentoComprobante.PrinterSettings =
                            configuracionOriginal;

                        documentoComprobante.PrintController =
                            controladorOriginal;
                    }

                    // COMPROBAR ARCHIVO GENERADO

                    if (File.Exists(guardar.FileName))
                    {
                        MessageBox.Show(
                            "¡COMPROBANTE PDF GENERADO CORRECTAMENTE!" +
                            "\n\nNúmero de venta: " +
                            numeroVentaComprobante +
                            "\n\nArchivo guardado en:\n" +
                            guardar.FileName,
                            "Exportación completada",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            "No se encontró el archivo PDF generado." +
                            "\n\nCompruebe la configuración de la impresora.",
                            "Error de exportación",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al exportar el comprobante PDF:" +
                    "\n\n" + ex.Message,
                    "TechSolutions",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // DIBUJAR COMPROBANTE
        // ==========================================

        private void DocumentoComprobante_PrintPage(
            object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;

            float izquierda = e.MarginBounds.Left;
            float derecha = e.MarginBounds.Right;
            float ancho = e.MarginBounds.Width;

            float y = e.MarginBounds.Top;

            using (Font fuenteEmpresa =
                new Font("Arial", 23, FontStyle.Bold))
            using (Font fuenteTitulo =
                new Font("Arial", 15, FontStyle.Bold))
            using (Font fuenteNormal =
                new Font("Arial", 10, FontStyle.Regular))
            using (Font fuenteNegrita =
                new Font("Arial", 10, FontStyle.Bold))
            using (Font fuentePequena =
                new Font("Arial", 9, FontStyle.Regular))
            using (Font fuenteTotal =
                new Font("Arial", 17, FontStyle.Bold))
            using (Pen lapiz =
                new Pen(Color.FromArgb(190, 200, 215), 1))
            using (SolidBrush pincelAzul =
                new SolidBrush(Color.FromArgb(23, 37, 61)))
            using (SolidBrush pincelVerde =
                new SolidBrush(Color.FromArgb(20, 145, 105)))
            using (SolidBrush pincelGris =
                new SolidBrush(Color.FromArgb(240, 243, 248)))
            using (SolidBrush pincelBlanco =
                new SolidBrush(Color.White))
            using (SolidBrush pincelTexto =
                new SolidBrush(Color.FromArgb(45, 55, 72)))
            using (StringFormat formatoCentro =
                new StringFormat())
            using (StringFormat formatoIzquierda =
                new StringFormat())
            {
                formatoCentro.Alignment =
                    StringAlignment.Center;

                formatoCentro.LineAlignment =
                    StringAlignment.Center;

                formatoCentro.Trimming =
                    StringTrimming.EllipsisCharacter;

                formatoIzquierda.Alignment =
                    StringAlignment.Near;

                formatoIzquierda.LineAlignment =
                    StringAlignment.Center;

                formatoIzquierda.Trimming =
                    StringTrimming.EllipsisCharacter;

                // ENCABEZADO

                g.DrawString(
                    "TECHSOLUTIONS",
                    fuenteEmpresa,
                    pincelAzul,
                    izquierda,
                    y);

                y += 42;

                g.DrawString(
                    "COMPROBANTE DE VENTA",
                    fuenteTitulo,
                    pincelTexto,
                    izquierda,
                    y);

                y += 35;

                g.DrawLine(
                    lapiz,
                    izquierda,
                    y,
                    derecha,
                    y);

                y += 20;

                // INFORMACION DE VENTA

                g.DrawString(
                    "Número de venta: " + numeroVentaComprobante,
                    fuenteNegrita,
                    pincelTexto,
                    izquierda,
                    y);

                y += 25;

                g.DrawString(
                    "Fecha: " +
                    fechaComprobante.ToString("dd/MM/yyyy HH:mm:ss"),
                    fuenteNormal,
                    pincelTexto,
                    izquierda,
                    y);

                y += 25;

                g.DrawString(
                    "Usuario responsable: " + usuarioComprobante,
                    fuenteNormal,
                    pincelTexto,
                    izquierda,
                    y);

                y += 30;

                g.DrawLine(
                    lapiz,
                    izquierda,
                    y,
                    derecha,
                    y);

                y += 20;

                // TABLA DE PRODUCTOS

                float[] anchos =
                {
                    ancho * 0.42f,
                    ancho * 0.16f,
                    ancho * 0.20f,
                    ancho * 0.22f
                };

                string[] encabezados =
                {
                    "PRODUCTO",
                    "CANTIDAD",
                    "PRECIO",
                    "SUBTOTAL"
                };

                float altoFila = 32;
                float x = izquierda;

                for (int i = 0; i < encabezados.Length; i++)
                {
                    RectangleF rectangulo =
                        new RectangleF(
                            x,
                            y,
                            anchos[i],
                            altoFila);

                    g.FillRectangle(
                        pincelAzul,
                        rectangulo);

                    g.DrawString(
                        encabezados[i],
                        fuenteNegrita,
                        pincelBlanco,
                        rectangulo,
                        formatoCentro);

                    x += anchos[i];
                }

                y += altoFila;

                // FILAS DE PRODUCTOS

                foreach (DetalleVenta detalle in detallesComprobante)
                {
                    RectangleF fondoFila =
                        new RectangleF(
                            izquierda,
                            y,
                            ancho,
                            altoFila);

                    g.FillRectangle(
                        pincelGris,
                        fondoFila);

                    string[] valores =
                    {
                        detalle.NombreProducto ?? "",
                        detalle.Cantidad.ToString(),
                        "S/ " + detalle.PrecioUnitario.ToString("0.00"),
                        "S/ " + detalle.Subtotal.ToString("0.00")
                    };

                    x = izquierda;

                    for (int i = 0; i < valores.Length; i++)
                    {
                        RectangleF rectangulo =
                            new RectangleF(
                                x + 5,
                                y,
                                anchos[i] - 10,
                                altoFila);

                        g.DrawString(
                            valores[i],
                            fuenteNormal,
                            pincelTexto,
                            rectangulo,
                            i == 0
                                ? formatoIzquierda
                                : formatoCentro);

                        x += anchos[i];
                    }

                    g.DrawLine(
                        lapiz,
                        izquierda,
                        y + altoFila,
                        derecha,
                        y + altoFila);

                    y += altoFila;
                }

                y += 25;

                // TOTAL

                g.DrawLine(
                    lapiz,
                    izquierda,
                    y,
                    derecha,
                    y);

                y += 20;

                g.DrawString(
                    "TOTAL DE LA VENTA:",
                    fuenteTotal,
                    pincelAzul,
                    izquierda,
                    y);

                string totalTexto =
                    "S/ " + totalComprobante.ToString("0.00");

                SizeF medidaTotal =
                    g.MeasureString(totalTexto, fuenteTotal);

                g.DrawString(
                    totalTexto,
                    fuenteTotal,
                    pincelVerde,
                    derecha - medidaTotal.Width,
                    y);

                y += 65;

                // PIE DEL COMPROBANTE

                g.DrawLine(
                    lapiz,
                    izquierda,
                    y,
                    derecha,
                    y);

                y += 20;

                string mensaje =
                    "¡Gracias por su compra!";

                SizeF medidaMensaje =
                    g.MeasureString(mensaje, fuenteNegrita);

                g.DrawString(
                    mensaje,
                    fuenteNegrita,
                    pincelAzul,
                    izquierda + (ancho - medidaMensaje.Width) / 2,
                    y);

                y += 30;

                string pie =
                    "TechSolutions | Sistema de gestión de ventas";

                SizeF medidaPie =
                    g.MeasureString(pie, fuentePequena);

                g.DrawString(
                    pie,
                    fuentePequena,
                    pincelTexto,
                    izquierda + (ancho - medidaPie.Width) / 2,
                    y);
            }

            e.HasMorePages = false;
        }
    }
}