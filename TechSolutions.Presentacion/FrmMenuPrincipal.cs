
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TechSolutions.Presentacion
{
    public class FrmMenuPrincipal : Form
    {
        private int usuarioId;
        private int rolId;
        private string nombre;

        private Timer timerInactividad;

        private const int TiempoInactividad = 180000;
        private int segundosInactivo = 0;

        // COLORES GENERALES
        private readonly Color fondo =
            Color.FromArgb(241, 245, 250);

        private readonly Color azulOscuro =
            Color.FromArgb(18, 36, 58);

        public FrmMenuPrincipal(int usuarioId, int rolId, string nombre)
        {
            this.usuarioId = usuarioId;
            this.rolId = rolId;
            this.nombre = nombre;

            KeyPreview = true;

            CrearInterfaz();
            ConfigurarTimerInactividad();
        }

        // ==========================================
        // INTERFAZ PRINCIPAL
        // ==========================================

        private void CrearInterfaz()
        {
            Text = "TechSolutions - Panel Principal";

            Size = new Size(920, 820);
            StartPosition = FormStartPosition.CenterScreen;

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            BackColor = fondo;

            // ENCABEZADO
            Panel encabezado = new Panel();

            encabezado.Location = new Point(0, 0);
            encabezado.Size = new Size(920, 145);
            encabezado.BackColor = azulOscuro;

            Controls.Add(encabezado);

            Label titulo = new Label();

            titulo.Text = "TECHSOLUTIONS";
            titulo.Font = new Font("Segoe UI", 25, FontStyle.Bold);
            titulo.ForeColor = Color.White;
            titulo.AutoSize = true;
            titulo.Location = new Point(40, 25);

            encabezado.Controls.Add(titulo);

            Label subtitulo = new Label();

            subtitulo.Text = "SISTEMA DE GESTIÓN EMPRESARIAL";
            subtitulo.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            subtitulo.ForeColor = Color.LightBlue;
            subtitulo.AutoSize = true;
            subtitulo.Location = new Point(44, 78);

            encabezado.Controls.Add(subtitulo);

            Label bienvenida = new Label();

            bienvenida.Text = "¡Bienvenido, " + nombre + "!";
            bienvenida.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            bienvenida.ForeColor = Color.White;
            bienvenida.AutoSize = true;
            bienvenida.Location = new Point(590, 35);

            encabezado.Controls.Add(bienvenida);

            Label perfil = new Label();

            perfil.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            perfil.ForeColor = Color.FromArgb(110, 230, 180);
            perfil.AutoSize = true;
            perfil.Location = new Point(592, 72);

            if (rolId == 1)
                perfil.Text = "ADMINISTRADOR";
            else if (rolId == 2)
                perfil.Text = "PERSONAL";
            else if (rolId == 3)
                perfil.Text = "ENCARGADO";
            else
                perfil.Text = "NO IDENTIFICADO";

            encabezado.Controls.Add(perfil);

            // TITULO DEL PANEL
            Label tituloPanel = new Label();

            tituloPanel.Text = "Panel principal";
            tituloPanel.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            tituloPanel.ForeColor = Color.FromArgb(35, 55, 80);
            tituloPanel.Location = new Point(45, 165);
            tituloPanel.AutoSize = true;

            Controls.Add(tituloPanel);

            Label descripcion = new Label();

            descripcion.Text = "Seleccione el módulo que desea utilizar.";
            descripcion.Font = new Font("Segoe UI", 10);
            descripcion.ForeColor = Color.Gray;
            descripcion.Location = new Point(48, 205);
            descripcion.AutoSize = true;

            Controls.Add(descripcion);

            // ======================================
            // ADMINISTRADOR
            // ======================================

            if (rolId == 1)
            {
                CrearTarjeta(
                    "ADMINISTRAR USUARIOS",
                    "Registrar y administrar usuarios",
                    "usuarios",
                    45, 250,
                    Color.FromArgb(235, 243, 255),
                    Color.FromArgb(37, 99, 235),
                    AdministrarUsuarios_Click);

                CrearTarjeta(
                    "GESTIÓN DEL SISTEMA",
                    "Configuración del sistema",
                    "sistema",
                    465, 250,
                    Color.FromArgb(230, 250, 242),
                    Color.FromArgb(5, 150, 105),
                    GestionSistema_Click);

                CrearTarjeta(
                    "GESTIÓN DE PRODUCTOS",
                    "Controlar el inventario",
                    "productos",
                    45, 390,
                    Color.FromArgb(243, 235, 255),
                    Color.FromArgb(124, 58, 237),
                    GestionProductos_Click);

                CrearTarjeta(
                    "VENTAS",
                    "Consultar las ventas realizadas",
                    "ventas",
                    465, 390,
                    Color.FromArgb(255, 241, 229),
                    Color.FromArgb(234, 88, 12),
                    Ventas_Click);

                CrearTarjeta(
                    "REPORTES DE VENTAS",
                    "Consultar informes y resultados",
                    "reportes",
                    45, 530,
                    Color.FromArgb(225, 247, 251),
                    Color.FromArgb(8, 145, 178),
                    ReportesVentas_Click);

                CrearTarjeta(
                    "REGISTRAR VENTA",
                    "Registrar una nueva operación",
                    "carrito",
                    465, 530,
                    Color.FromArgb(231, 250, 235),
                    Color.FromArgb(22, 163, 74),
                    RegistrarVenta_Click);
            }

            // ======================================
            // PERSONAL
            // ======================================

            else if (rolId == 2)
            {
                CrearTarjeta(
                    "MÓDULO DE PERSONAL",
                    "Opciones del área de personal",
                    "usuarios",
                    45, 250,
                    Color.FromArgb(235, 243, 255),
                    Color.FromArgb(37, 99, 235),
                    Personal_Click);

                CrearTarjeta(
                    "VENTAS",
                    "Consultar las ventas realizadas",
                    "ventas",
                    465, 250,
                    Color.FromArgb(255, 241, 229),
                    Color.FromArgb(234, 88, 12),
                    Ventas_Click);

                CrearTarjeta(
                    "REGISTRAR VENTA",
                    "Registrar una nueva operación",
                    "carrito",
                    45, 390,
                    Color.FromArgb(231, 250, 235),
                    Color.FromArgb(22, 163, 74),
                    RegistrarVenta_Click);
            }

            // ======================================
            // ENCARGADO
            // ======================================

            else if (rolId == 3)
            {
                CrearTarjeta(
                    "MÓDULO DE ENCARGADO",
                    "Opciones del área de encargado",
                    "usuarios",
                    45, 250,
                    Color.FromArgb(235, 243, 255),
                    Color.FromArgb(37, 99, 235),
                    Encargado_Click);

                CrearTarjeta(
                    "VENTAS",
                    "Consultar las ventas realizadas",
                    "ventas",
                    465, 250,
                    Color.FromArgb(255, 241, 229),
                    Color.FromArgb(234, 88, 12),
                    Ventas_Click);

                CrearTarjeta(
                    "REPORTES DE VENTAS",
                    "Consultar informes y resultados",
                    "reportes",
                    45, 390,
                    Color.FromArgb(225, 247, 251),
                    Color.FromArgb(8, 145, 178),
                    ReportesVentas_Click);

                CrearTarjeta(
                    "REGISTRAR VENTA",
                    "Registrar una nueva operación",
                    "carrito",
                    465, 390,
                    Color.FromArgb(231, 250, 235),
                    Color.FromArgb(22, 163, 74),
                    RegistrarVenta_Click);
            }

            // ======================================
            // CERRAR SESIÓN
            // ======================================

            Button salir = new Button();

            salir.Text = "↪     CERRAR SESIÓN";

            salir.Font = new Font(
                "Segoe UI", 11, FontStyle.Bold);

            salir.ForeColor = Color.White;

            salir.BackColor = Color.FromArgb(194, 55, 55);

            salir.FlatStyle = FlatStyle.Flat;
            salir.FlatAppearance.BorderSize = 0;

            salir.Location = new Point(315, 690);
            salir.Size = new Size(290, 48);

            salir.Cursor = Cursors.Hand;

            salir.Click += (s, e) =>
            {
                Close();
            };

            Controls.Add(salir);

            Label pie = new Label();

            pie.Text = "TechSolutions | Sistema de gestión empresarial";

            pie.Font = new Font("Segoe UI", 9);

            pie.ForeColor = Color.Gray;

            pie.AutoSize = true;

            pie.Location = new Point(305, 755);

            Controls.Add(pie);
        }

        // ==========================================
        // CREAR TARJETAS INDIVIDUALES
        // ==========================================

        private void CrearTarjeta(
            string titulo,
            string descripcion,
            string tipoImagen,
            int x,
            int y,
            Color colorFondo,
            Color colorAcento,
            EventHandler evento)
        {
            Panel tarjeta = new Panel();

            tarjeta.Location = new Point(x, y);
            tarjeta.Size = new Size(390, 120);

            tarjeta.BackColor = colorFondo;

            tarjeta.Cursor = Cursors.Hand;

            // BORDE DE LA TARJETA
            tarjeta.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode =
                    SmoothingMode.AntiAlias;

                using (Pen lapiz = new Pen(
                    Color.FromArgb(220, 225, 235), 1))
                {
                    e.Graphics.DrawRectangle(
                        lapiz,
                        0, 0,
                        tarjeta.Width - 1,
                        tarjeta.Height - 1);
                }
            };

            // BARRA LATERAL DE COLOR
            Panel barra = new Panel();

            barra.Location = new Point(0, 0);
            barra.Size = new Size(6, 120);

            barra.BackColor = colorAcento;

            tarjeta.Controls.Add(barra);

            // IMAGEN
            PictureBox imagen = new PictureBox();

            imagen.Location = new Point(20, 12);
            imagen.Size = new Size(100, 95);

            imagen.BackColor = Color.Transparent;

            imagen.SizeMode = PictureBoxSizeMode.Zoom;

            imagen.Image = DibujarImagen(tipoImagen, colorAcento);

            imagen.Cursor = Cursors.Hand;

            tarjeta.Controls.Add(imagen);

            // TITULO
            Label lblTitulo = new Label();

            lblTitulo.Text = titulo;

            lblTitulo.Font = new Font(
                "Segoe UI", 11, FontStyle.Bold);

            lblTitulo.ForeColor = colorAcento;

            lblTitulo.AutoSize = false;

            lblTitulo.Location = new Point(135, 30);

            lblTitulo.Size = new Size(240, 30);

            lblTitulo.TextAlign =
                ContentAlignment.MiddleLeft;

            lblTitulo.Cursor = Cursors.Hand;

            tarjeta.Controls.Add(lblTitulo);

            // DESCRIPCIÓN
            Label lblDescripcion = new Label();

            lblDescripcion.Text = descripcion;

            lblDescripcion.Font = new Font(
                "Segoe UI", 9);

            lblDescripcion.ForeColor =
                Color.FromArgb(90, 100, 115);

            lblDescripcion.AutoSize = false;

            lblDescripcion.Location = new Point(135, 65);

            lblDescripcion.Size = new Size(240, 30);

            lblDescripcion.TextAlign =
                ContentAlignment.MiddleLeft;

            lblDescripcion.Cursor = Cursors.Hand;

            tarjeta.Controls.Add(lblDescripcion);

            // EVENTOS DE CLIC
            tarjeta.Click += evento;
            imagen.Click += evento;
            lblTitulo.Click += evento;
            lblDescripcion.Click += evento;

            Controls.Add(tarjeta);

            tarjeta.BringToFront();
        }

        // ==========================================
        // DIBUJAR ILUSTRACIONES PARA LAS TARJETAS
        // ==========================================

        private Bitmap DibujarImagen(
            string tipo,
            Color color)
        {
            Bitmap imagen = new Bitmap(110, 100);

            using (Graphics g = Graphics.FromImage(imagen))
            {
                g.SmoothingMode =
                    SmoothingMode.AntiAlias;

                using (SolidBrush pincel =
                    new SolidBrush(color))
                using (Pen lapiz =
                    new Pen(color, 4))
                using (Pen lapizOscuro =
                    new Pen(Color.FromArgb(55, 65, 80), 3))
                using (SolidBrush blanco =
                    new SolidBrush(Color.White))
                {
                    if (tipo == "productos")
                    {
                        g.FillRectangle(
                            pincel, 15, 30, 35, 35);

                        g.FillRectangle(
                            new SolidBrush(
                                Color.FromArgb(180, color)),
                            53, 42, 38, 32);

                        g.DrawRectangle(
                            lapizOscuro, 15, 30, 35, 35);

                        g.DrawRectangle(
                            lapizOscuro, 53, 42, 38, 32);

                        g.DrawLine(
                            lapizOscuro, 32, 30, 32, 65);

                        g.DrawLine(
                            lapizOscuro, 15, 45, 50, 45);

                        g.DrawLine(
                            lapizOscuro, 72, 42, 72, 74);

                        g.DrawLine(
                            lapizOscuro, 53, 55, 91, 55);
                    }

                    else if (tipo == "carrito")
                    {
                        g.DrawLine(lapiz, 12, 23, 25, 23);
                        g.DrawLine(lapiz, 25, 23, 35, 65);
                        g.DrawLine(lapiz, 35, 65, 82, 65);

                        g.DrawLine(lapiz, 30, 38, 88, 38);
                        g.DrawLine(lapiz, 88, 38, 78, 59);
                        g.DrawLine(lapiz, 78, 59, 35, 59);

                        g.FillEllipse(
                            pincel, 37, 73, 12, 12);

                        g.FillEllipse(
                            pincel, 72, 73, 12, 12);

                        g.DrawLine(
                            lapizOscuro, 40, 40, 45, 55);

                        g.DrawLine(
                            lapizOscuro, 60, 40, 60, 55);
                    }

                    else if (tipo == "ventas")
                    {
                        g.DrawLine(
                            lapizOscuro, 15, 80, 95, 80);

                        g.DrawLine(
                            lapizOscuro, 15, 80, 15, 15);

                        g.FillRectangle(
                            pincel, 25, 53, 14, 27);

                        g.FillRectangle(
                            pincel, 47, 38, 14, 42);

                        g.FillRectangle(
                            pincel, 69, 20, 14, 60);

                        g.DrawLine(
                            new Pen(Color.FromArgb(245, 160, 40), 3),
                            30, 44, 54, 28);

                        g.DrawLine(
                            new Pen(Color.FromArgb(245, 160, 40), 3),
                            54, 28, 76, 13);
                    }

                    else if (tipo == "reportes")
                    {
                        g.FillRectangle(
                            blanco, 27, 10, 58, 78);

                        g.DrawRectangle(
                            lapiz, 27, 10, 58, 78);

                        g.DrawLine(
                            lapizOscuro, 38, 28, 72, 28);

                        g.DrawLine(
                            lapizOscuro, 38, 37, 72, 37);

                        g.DrawLine(
                            lapizOscuro, 38, 46, 72, 46);

                        g.FillRectangle(
                            pincel, 38, 60, 8, 17);

                        g.FillRectangle(
                            pincel, 51, 54, 8, 23);

                        g.FillRectangle(
                            pincel, 64, 49, 8, 28);
                    }

                    else if (tipo == "usuarios")
                    {
                        g.FillEllipse(
                            pincel, 37, 10, 28, 28);

                        g.FillEllipse(
                            pincel, 12, 25, 21, 21);

                        g.FillEllipse(
                            pincel, 69, 25, 21, 21);

                        g.FillEllipse(
                            pincel, 22, 47, 57, 39);

                        g.FillEllipse(
                            blanco, 3, 51, 35, 32);

                        g.FillEllipse(
                            blanco, 69, 51, 35, 32);
                    }

                    else if (tipo == "sistema")
                    {
                        g.FillEllipse(
                            pincel, 20, 15, 65, 65);

                        g.FillEllipse(
                            blanco, 38, 33, 29, 29);

                        g.DrawEllipse(
                            lapizOscuro, 38, 33, 29, 29);

                        g.FillRectangle(
                            pincel, 45, 5, 15, 15);

                        g.FillRectangle(
                            pincel, 45, 75, 15, 15);

                        g.FillRectangle(
                            pincel, 10, 40, 15, 15);

                        g.FillRectangle(
                            pincel, 80, 40, 15, 15);

                        g.FillRectangle(
                            pincel, 18, 17, 13, 13);

                        g.FillRectangle(
                            pincel, 74, 17, 13, 13);

                        g.FillRectangle(
                            pincel, 18, 68, 13, 13);

                        g.FillRectangle(
                            pincel, 74, 68, 13, 13);
                    }
                }
            }

            return imagen;
        }

        // ==========================================
        // TEMPORIZADOR DE INACTIVIDAD
        // ==========================================

        private void ConfigurarTimerInactividad()
        {
            timerInactividad = new Timer();

            timerInactividad.Interval = 1000;

            timerInactividad.Tick += TimerInactividad_Tick;

            MouseMove += RegistrarActividad;
            MouseDown += RegistrarActividad;
            KeyDown += RegistrarActividad;

            RegistrarEventosControles(this);

            timerInactividad.Start();
        }

        private void RegistrarEventosControles(
            Control contenedor)
        {
            foreach (Control control in contenedor.Controls)
            {
                control.MouseMove += RegistrarActividad;
                control.MouseDown += RegistrarActividad;
                control.KeyDown += RegistrarActividad;

                if (control.HasChildren)
                    RegistrarEventosControles(control);
            }
        }

        private void RegistrarActividad(
            object sender, EventArgs e)
        {
            if (timerInactividad != null)
                segundosInactivo = 0;
        }

        private void TimerInactividad_Tick(
            object sender, EventArgs e)
        {
            segundosInactivo++;

            if (segundosInactivo >= TiempoInactividad / 1000)
            {
                timerInactividad.Stop();

                MessageBox.Show(
                    "Su sesión se cerrará por inactividad.",
                    "Seguridad del sistema",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Close();
            }
        }

        // ==========================================
        // ADMINISTRAR USUARIOS
        // ==========================================

        private void AdministrarUsuarios_Click(
            object sender, EventArgs e)
        {
            if (rolId != 1)
            {
                MessageBox.Show("Acceso denegado.");
                return;
            }

            RegistrarActividad(sender, e);

            using (FrmAdministrarUsuarios formulario =
                new FrmAdministrarUsuarios(rolId))
            {
                formulario.ShowDialog(this);
            }

            RegistrarActividad(sender, e);
        }

        // ==========================================
        // GESTIÓN DEL SISTEMA
        // ==========================================

        private void GestionSistema_Click(
            object sender, EventArgs e)
        {
            RegistrarActividad(sender, e);

            using (FrmGestionSistema formulario =
                new FrmGestionSistema())
            {
                formulario.ShowDialog(this);
            }

            RegistrarActividad(sender, e);
        }

        // ==========================================
        // GESTIÓN DE PRODUCTOS
        // ==========================================

        private void GestionProductos_Click(
            object sender, EventArgs e)
        {
            if (rolId != 1)
            {
                MessageBox.Show("Acceso denegado.");
                return;
            }

            RegistrarActividad(sender, e);

            using (FrmGestionProductos formulario =
                new FrmGestionProductos())
            {
                formulario.ShowDialog(this);
            }

            RegistrarActividad(sender, e);
        }

        // ==========================================
        // VENTAS
        // ==========================================

        private void Ventas_Click(
            object sender, EventArgs e)
        {
            RegistrarActividad(sender, e);

            using (FrmVentas formulario =
                new FrmVentas(usuarioId, rolId))
            {
                formulario.ShowDialog(this);
            }

            RegistrarActividad(sender, e);
        }

        // ==========================================
        // REGISTRAR VENTA
        // ==========================================

        private void RegistrarVenta_Click(
            object sender, EventArgs e)
        {
            RegistrarActividad(sender, e);

            using (FrmRegistrarVenta formulario =
                new FrmRegistrarVenta(usuarioId))
            {
                formulario.ShowDialog(this);
            }

            RegistrarActividad(sender, e);
        }

        // ==========================================
        // REPORTES DE VENTAS
        // ==========================================

        private void ReportesVentas_Click(
            object sender, EventArgs e)
        {
            if (rolId != 1 && rolId != 3)
            {
                MessageBox.Show(
                    "No tiene permiso para consultar reportes.");

                return;
            }

            RegistrarActividad(sender, e);

            using (FrmReporteVentas formulario =
                new FrmReporteVentas())
            {
                formulario.ShowDialog(this);
            }

            RegistrarActividad(sender, e);
        }

        // ==========================================
        // PERSONAL
        // ==========================================

        private void Personal_Click(
            object sender, EventArgs e)
        {
            RegistrarActividad(sender, e);

            MessageBox.Show(
                "Módulo de personal.",
                "TechSolutions",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // ==========================================
        // ENCARGADO
        // ==========================================

        private void Encargado_Click(
            object sender, EventArgs e)
        {
            RegistrarActividad(sender, e);

            MessageBox.Show(
                "Módulo de encargado.",
                "TechSolutions",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // ==========================================
        // LIBERAR RECURSOS
        // ==========================================

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            if (timerInactividad != null)
            {
                timerInactividad.Stop();
                timerInactividad.Dispose();
                timerInactividad = null;
            }

            base.OnFormClosed(e);
        }
    }
}
