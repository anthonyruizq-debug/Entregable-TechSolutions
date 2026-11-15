
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using TechSolutions.Entidades.Productos;
using TechSolutions.Entidades.Ventas;
using TechSolutions.Negocio.Servicios;

namespace TechSolutions.Presentacion
{
    public class FrmVentas : Form
    {
        private readonly int usuarioId;
        private readonly int rolId;

        private readonly VentaServicio ventaServicio;
        private readonly ProductoServicio productoServicio;

        private List<Producto> productos;
        private List<DetalleVenta> carrito;

        private Label lblTitulo;
        private Label lblSesion;
        private Label lblProducto;
        private Label lblCantidad;
        private Label lblStock;
        private Label lblTotal;

        private ComboBox cmbProductos;
        private NumericUpDown numCantidad;

        private DataGridView dgvCarrito;

        private Button btnAgregar;
        private Button btnQuitar;
        private Button btnRegistrar;
        private Button btnActualizar;
        private Button btnCerrar;

        private readonly Color azulOscuro =
            Color.FromArgb(23, 37, 61);

        private readonly Color azul =
            Color.FromArgb(37, 99, 180);

        private readonly Color fondo =
            Color.FromArgb(242, 245, 250);

        private readonly Color texto =
            Color.FromArgb(45, 55, 72);

        private readonly Color verde =
            Color.FromArgb(20, 145, 105);

        public FrmVentas(int usuarioId, int rolId)
        {
            if (usuarioId <= 0)
                throw new ArgumentException("Usuario no válido.");

            this.usuarioId = usuarioId;
            this.rolId = rolId;

            ventaServicio = new VentaServicio();
            productoServicio = new ProductoServicio();

            productos = new List<Producto>();
            carrito = new List<DetalleVenta>();

            ConfigurarFormulario();
            CrearControles();
            CargarProductos();
        }

        // CONFIGURAR FORMULARIO
        private void ConfigurarFormulario()
        {
            Text = "TechSolutions | Punto de venta";
            Size = new Size(1100, 760);
            MinimumSize = new Size(1100, 760);

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            BackColor = fondo;
            Font = new Font("Segoe UI", 10);
        }

        // CREAR INTERFAZ
        private void CrearControles()
        {
            // ENCABEZADO
            Panel encabezado = new Panel();
            encabezado.Dock = DockStyle.Top;
            encabezado.Height = 110;
            encabezado.BackColor = azulOscuro;
            Controls.Add(encabezado);

            lblTitulo = new Label();
            lblTitulo.Text = "PUNTO DE VENTA";
            lblTitulo.Font = new Font(
                "Segoe UI", 23, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(30, 18);
            lblTitulo.AutoSize = true;
            encabezado.Controls.Add(lblTitulo);

            lblSesion = new Label();
            lblSesion.Text =
                "Sesión activa  |  Usuario: " + usuarioId +
                "  |  Perfil: " + ObtenerNombreRol();
            lblSesion.Font = new Font("Segoe UI", 9);
            lblSesion.ForeColor =
                Color.FromArgb(195, 210, 230);
            lblSesion.Location = new Point(35, 70);
            lblSesion.AutoSize = true;
            encabezado.Controls.Add(lblSesion);

            // PANEL DE PRODUCTOS
            Panel panelProductos = new Panel();
            panelProductos.Location = new Point(25, 135);
            panelProductos.Size = new Size(340, 440);
            panelProductos.BackColor = Color.White;
            panelProductos.BorderStyle =
                BorderStyle.FixedSingle;
            Controls.Add(panelProductos);

            Label lblTituloProductos = new Label();
            lblTituloProductos.Text = "PRODUCTOS";
            lblTituloProductos.Font =
                new Font("Segoe UI", 13, FontStyle.Bold);
            lblTituloProductos.ForeColor = azulOscuro;
            lblTituloProductos.Location = new Point(20, 20);
            lblTituloProductos.AutoSize = true;
            panelProductos.Controls.Add(lblTituloProductos);

            Label lblDescripcion = new Label();
            lblDescripcion.Text =
                "Selecciona un artículo para agregarlo a la venta.";
            lblDescripcion.Font = new Font("Segoe UI", 9);
            lblDescripcion.ForeColor = Color.Gray;
            lblDescripcion.Location = new Point(22, 55);
            lblDescripcion.Size = new Size(295, 40);
            panelProductos.Controls.Add(lblDescripcion);

            lblProducto = new Label();
            lblProducto.Text = "Producto disponible";
            lblProducto.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);
            lblProducto.ForeColor = texto;
            lblProducto.Location = new Point(22, 115);
            lblProducto.AutoSize = true;
            panelProductos.Controls.Add(lblProducto);

            cmbProductos = new ComboBox();
            cmbProductos.Font = new Font("Segoe UI", 10);
            cmbProductos.DropDownStyle =
                ComboBoxStyle.DropDownList;
            cmbProductos.Location = new Point(22, 145);
            cmbProductos.Size = new Size(290, 35);
            cmbProductos.SelectedIndexChanged +=
                CmbProductos_SelectedIndexChanged;
            panelProductos.Controls.Add(cmbProductos);

            lblStock = new Label();
            lblStock.Text = "Stock disponible: 0";
            lblStock.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);
            lblStock.ForeColor = verde;
            lblStock.Location = new Point(22, 200);
            lblStock.Size = new Size(290, 25);
            panelProductos.Controls.Add(lblStock);

            lblCantidad = new Label();
            lblCantidad.Text = "Cantidad";
            lblCantidad.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);
            lblCantidad.ForeColor = texto;
            lblCantidad.Location = new Point(22, 250);
            lblCantidad.AutoSize = true;
            panelProductos.Controls.Add(lblCantidad);

            numCantidad = new NumericUpDown();
            numCantidad.Minimum = 1;
            numCantidad.Maximum = 99999;
            numCantidad.Value = 1;
            numCantidad.Font = new Font("Segoe UI", 11);
            numCantidad.Location = new Point(22, 280);
            numCantidad.Size = new Size(290, 35);
            panelProductos.Controls.Add(numCantidad);

            btnAgregar = CrearBoton(
                "+  AGREGAR AL CARRITO",
                verde);

            btnAgregar.Location = new Point(22, 345);
            btnAgregar.Size = new Size(290, 45);
            btnAgregar.Click += BtnAgregar_Click;
            panelProductos.Controls.Add(btnAgregar);

            btnActualizar = CrearBoton(
                "ACTUALIZAR PRODUCTOS",
                azul);

            btnActualizar.Location = new Point(22, 395);
            btnActualizar.Size = new Size(290, 32);
            btnActualizar.Font = new Font(
                "Segoe UI", 8, FontStyle.Bold);
            btnActualizar.Click += BtnActualizar_Click;
            panelProductos.Controls.Add(btnActualizar);

            // PANEL DEL CARRITO
            Panel panelCarrito = new Panel();
            panelCarrito.Location = new Point(385, 135);
            panelCarrito.Size = new Size(690, 440);
            panelCarrito.BackColor = Color.White;
            panelCarrito.BorderStyle =
                BorderStyle.FixedSingle;
            Controls.Add(panelCarrito);

            Label lblCarrito = new Label();
            lblCarrito.Text = "DETALLE DE LA VENTA";
            lblCarrito.Font =
                new Font("Segoe UI", 13, FontStyle.Bold);
            lblCarrito.ForeColor = azulOscuro;
            lblCarrito.Location = new Point(20, 20);
            lblCarrito.AutoSize = true;
            panelCarrito.Controls.Add(lblCarrito);

            Label lblSubtituloCarrito = new Label();
            lblSubtituloCarrito.Text =
                "Resumen de los productos seleccionados";
            lblSubtituloCarrito.Font =
                new Font("Segoe UI", 9);
            lblSubtituloCarrito.ForeColor = Color.Gray;
            lblSubtituloCarrito.Location = new Point(22, 55);
            lblSubtituloCarrito.AutoSize = true;
            panelCarrito.Controls.Add(lblSubtituloCarrito);

            dgvCarrito = new DataGridView();
            dgvCarrito.Location = new Point(20, 95);
            dgvCarrito.Size = new Size(650, 320);

            dgvCarrito.AllowUserToAddRows = false;
            dgvCarrito.AllowUserToDeleteRows = false;
            dgvCarrito.ReadOnly = true;
            dgvCarrito.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvCarrito.MultiSelect = false;
            dgvCarrito.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvCarrito.BackgroundColor = Color.White;
            dgvCarrito.BorderStyle = BorderStyle.None;
            dgvCarrito.RowHeadersVisible = false;
            dgvCarrito.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCarrito.EnableHeadersVisualStyles = false;
            dgvCarrito.ColumnHeadersHeight = 42;
            dgvCarrito.RowTemplate.Height = 38;
            dgvCarrito.GridColor =
                Color.FromArgb(230, 234, 240);

            dgvCarrito.ColumnHeadersDefaultCellStyle.BackColor =
                azulOscuro;
            dgvCarrito.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;
            dgvCarrito.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            dgvCarrito.DefaultCellStyle.Font =
                new Font("Segoe UI", 9);
            dgvCarrito.DefaultCellStyle.ForeColor = texto;
            dgvCarrito.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(218, 232, 250);
            dgvCarrito.DefaultCellStyle.SelectionForeColor =
                texto;

            dgvCarrito.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(246, 248, 252);

            dgvCarrito.Columns.Add("ProductoId", "ID");
            dgvCarrito.Columns.Add("Producto", "PRODUCTO");
            dgvCarrito.Columns.Add("Cantidad", "CANTIDAD");
            dgvCarrito.Columns.Add("Precio", "PRECIO");
            dgvCarrito.Columns.Add("Subtotal", "SUBTOTAL");

            dgvCarrito.Columns["ProductoId"].FillWeight = 40;
            dgvCarrito.Columns["Producto"].FillWeight = 150;
            dgvCarrito.Columns["Cantidad"].FillWeight = 65;
            dgvCarrito.Columns["Precio"].FillWeight = 75;
            dgvCarrito.Columns["Subtotal"].FillWeight = 85;

            panelCarrito.Controls.Add(dgvCarrito);

            // PANEL INFERIOR
            Panel panelInferior = new Panel();
            panelInferior.Location = new Point(25, 595);
            panelInferior.Size = new Size(1050, 95);
            panelInferior.BackColor = Color.White;
            panelInferior.BorderStyle =
                BorderStyle.FixedSingle;
            Controls.Add(panelInferior);

            btnQuitar = CrearBoton(
                "QUITAR PRODUCTO",
                Color.FromArgb(205, 70, 75));

            btnQuitar.Location = new Point(18, 25);
            btnQuitar.Size = new Size(180, 45);
            btnQuitar.Click += BtnQuitar_Click;
            panelInferior.Controls.Add(btnQuitar);

            Label lblEtiquetaTotal = new Label();
            lblEtiquetaTotal.Text = "TOTAL A PAGAR";
            lblEtiquetaTotal.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            lblEtiquetaTotal.ForeColor = Color.Gray;
            lblEtiquetaTotal.Location = new Point(340, 15);
            lblEtiquetaTotal.AutoSize = true;
            panelInferior.Controls.Add(lblEtiquetaTotal);

            lblTotal = new Label();
            lblTotal.Text = "S/ 0.00";
            lblTotal.Font =
                new Font("Segoe UI", 21, FontStyle.Bold);
            lblTotal.ForeColor = verde;
            lblTotal.Location = new Point(335, 37);
            lblTotal.Size = new Size(280, 42);
            panelInferior.Controls.Add(lblTotal);

            btnRegistrar = CrearBoton(
                "CONFIRMAR VENTA",
                verde);

            btnRegistrar.Location = new Point(665, 25);
            btnRegistrar.Size = new Size(195, 45);
            btnRegistrar.Click += BtnRegistrar_Click;
            panelInferior.Controls.Add(btnRegistrar);

            btnCerrar = CrearBoton(
                "CERRAR",
                Color.FromArgb(95, 105, 120));

            btnCerrar.Location = new Point(875, 25);
            btnCerrar.Size = new Size(150, 45);
            btnCerrar.Click += (s, e) => Close();
            panelInferior.Controls.Add(btnCerrar);
        }

        // CREAR BOTONES
        private Button CrearBoton(string nombre, Color color)
        {
            Button boton = new Button();

            boton.Text = nombre;
            boton.BackColor = color;
            boton.ForeColor = Color.White;
            boton.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);

            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.Cursor = Cursors.Hand;

            return boton;
        }

        // NOMBRE DEL PERFIL
        private string ObtenerNombreRol()
        {
            switch (rolId)
            {
                case 1:
                    return "Administrador";

                case 2:
                    return "Personal";

                case 3:
                    return "Encargado";

                default:
                    return "Usuario";
            }
        }

        // CARGAR PRODUCTOS
        private void CargarProductos()
        {
            try
            {
                productos = productoServicio.ObtenerTodos()
                    .Where(p => p.Stock > 0)
                    .ToList();

                cmbProductos.DataSource = null;
                cmbProductos.DataSource = productos;
                cmbProductos.DisplayMember = "Nombre";
                cmbProductos.ValueMember = "Id";

                btnAgregar.Enabled = productos.Count > 0;

                if (productos.Count == 0)
                {
                    lblStock.Text = "No hay productos disponibles.";
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

        // MOSTRAR STOCK
        private void CmbProductos_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            Producto producto =
                cmbProductos.SelectedItem as Producto;

            if (producto != null)
            {
                int cantidadEnCarrito = carrito
                    .Where(d => d.ProductoId == producto.Id)
                    .Sum(d => d.Cantidad);

                int disponible =
                    producto.Stock - cantidadEnCarrito;

                lblStock.Text =
                    "Stock disponible: " + disponible;

                lblStock.ForeColor =
                    disponible > 0 ? verde : Color.Red;
            }
        }

        // AGREGAR AL CARRITO
        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            Producto producto =
                cmbProductos.SelectedItem as Producto;

            if (producto == null)
            {
                MessageBox.Show("Seleccione un producto.");
                return;
            }

            int cantidad = Convert.ToInt32(numCantidad.Value);

            DetalleVenta existente = carrito
                .FirstOrDefault(d => d.ProductoId == producto.Id);

            int cantidadActual =
                existente == null ? 0 : existente.Cantidad;

            if (cantidad + cantidadActual > producto.Stock)
            {
                MessageBox.Show(
                    "Stock insuficiente. Disponible: " +
                    (producto.Stock - cantidadActual),
                    "Inventario",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (existente != null)
            {
                existente.Cantidad += cantidad;

                existente.Subtotal =
                    existente.Cantidad *
                    existente.PrecioUnitario;
            }
            else
            {
                DetalleVenta detalle = new DetalleVenta
                {
                    ProductoId = producto.Id,
                    NombreProducto = producto.Nombre,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio,
                    Subtotal = producto.Precio * cantidad
                };

                carrito.Add(detalle);
            }

            ActualizarCarrito();
            numCantidad.Value = 1;
        }

        // ACTUALIZAR TABLA Y TOTAL
        private void ActualizarCarrito()
        {
            dgvCarrito.Rows.Clear();

            decimal total = 0;

            foreach (DetalleVenta detalle in carrito)
            {
                detalle.Subtotal =
                    detalle.Cantidad *
                    detalle.PrecioUnitario;

                dgvCarrito.Rows.Add(
                    detalle.ProductoId,
                    detalle.NombreProducto,
                    detalle.Cantidad,
                    detalle.PrecioUnitario.ToString("N2"),
                    detalle.Subtotal.ToString("N2"));

                total += detalle.Subtotal;
            }

            lblTotal.Text = "S/ " + total.ToString("N2");

            CmbProductos_SelectedIndexChanged(null, null);
        }

        // QUITAR PRODUCTO
        private void BtnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione un producto del carrito.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            int indice = dgvCarrito.SelectedRows[0].Index;

            if (indice >= 0 && indice < carrito.Count)
            {
                carrito.RemoveAt(indice);
                ActualizarCarrito();
            }
        }

        // ACTUALIZAR PRODUCTOS
        private void BtnActualizar_Click(object sender, EventArgs e)
        {
            CargarProductos();
        }

        // REGISTRAR VENTA
        private void BtnRegistrar_Click(object sender, EventArgs e)
        {
            if (carrito.Count == 0)
            {
                MessageBox.Show(
                    "Agregue productos antes de registrar la venta.");

                return;
            }

            DialogResult confirmar = MessageBox.Show(
                "¿Desea confirmar esta venta?",
                "Confirmar venta",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
                return;

            try
            {
                decimal total = carrito.Sum(
                    d => d.Cantidad * d.PrecioUnitario);

                Venta venta = new Venta
                {
                    UsuarioId = usuarioId,
                    Fecha = DateTime.Now,
                    Total = total,
                    Detalles = new List<DetalleVenta>(carrito)
                };

                int ventaId =
                    ventaServicio.RegistrarVenta(venta);

                MessageBox.Show(
                    "Venta registrada correctamente.\n\n" +
                    "Número de venta: " + ventaId +
                    "\nTotal: S/ " + total.ToString("N2") +
                    "\nEl inventario ha sido actualizado.",
                    "Venta exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                carrito.Clear();
                ActualizarCarrito();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo registrar la venta:\n" +
                    ex.Message,
                    "Error de venta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                CargarProductos();
            }
        }
    }
}
