
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TechSolutions.Presentacion
{
    public class FrmSeguridadSistema : Form
    {
        private DataGridView dgvSeguridad;

        private Color azulOscuro = Color.FromArgb(23, 37, 61);
        private Color fondo = Color.FromArgb(242, 245, 250);
        private Color texto = Color.FromArgb(45, 55, 72);

        public FrmSeguridadSistema()
        {
            CrearInterfaz();
            CargarSeguridad();
        }

        private void CrearInterfaz()
        {
            Text = "TechSolutions | Seguridad del sistema";
            Size = new Size(950, 680);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = fondo;
            Font = new Font("Segoe UI", 10);

            // ENCABEZADO
            Panel encabezado = new Panel();
            encabezado.Dock = DockStyle.Top;
            encabezado.Height = 115;
            encabezado.BackColor = azulOscuro;

            Label titulo = new Label();
            titulo.Text = "CENTRO DE SEGURIDAD";
            titulo.Font = new Font("Segoe UI", 23, FontStyle.Bold);
            titulo.ForeColor = Color.White;
            titulo.Location = new Point(30, 20);
            titulo.AutoSize = true;

            Label descripcion = new Label();
            descripcion.Text =
                "Supervisión de accesos y protección de las cuentas de usuario";
            descripcion.Font = new Font("Segoe UI", 10);
            descripcion.ForeColor = Color.FromArgb(190, 205, 225);
            descripcion.Location = new Point(34, 68);
            descripcion.AutoSize = true;

            encabezado.Controls.Add(titulo);
            encabezado.Controls.Add(descripcion);

            // TARJETAS DE SEGURIDAD
            Panel tarjeta1 = CrearTarjeta(
                "CONTROL DE ACCESO",
                "PERFILES Y PERMISOS",
                Color.FromArgb(37, 99, 180),
                new Point(30, 140));

            Panel tarjeta2 = CrearTarjeta(
                "PROTECCIÓN",
                "CONTRASEÑAS SEGURAS",
                Color.FromArgb(22, 145, 95),
                new Point(330, 140));

            Panel tarjeta3 = CrearTarjeta(
                "BLOQUEOS",
                "CONTROL DE INTENTOS",
                Color.FromArgb(205, 135, 35),
                new Point(630, 140));

            // PANEL DE INFORMACIÓN
            Panel panelTabla = new Panel();
            panelTabla.Location = new Point(30, 280);
            panelTabla.Size = new Size(870, 290);
            panelTabla.BackColor = Color.White;

            Label lblTabla = new Label();
            lblTabla.Text = "FUNCIONES DE SEGURIDAD";
            lblTabla.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTabla.ForeColor = azulOscuro;
            lblTabla.Location = new Point(20, 15);
            lblTabla.AutoSize = true;

            Label lblInfo = new Label();
            lblInfo.Text = "Módulos de protección implementados en TechSolutions";
            lblInfo.Font = new Font("Segoe UI", 9);
            lblInfo.ForeColor = Color.Gray;
            lblInfo.Location = new Point(22, 43);
            lblInfo.AutoSize = true;

            dgvSeguridad = new DataGridView();
            dgvSeguridad.Location = new Point(20, 80);
            dgvSeguridad.Size = new Size(830, 190);

            dgvSeguridad.ReadOnly = true;
            dgvSeguridad.AllowUserToAddRows = false;
            dgvSeguridad.AllowUserToDeleteRows = false;
            dgvSeguridad.AllowUserToResizeRows = false;
            dgvSeguridad.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvSeguridad.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvSeguridad.MultiSelect = false;
            dgvSeguridad.RowHeadersVisible = false;
            dgvSeguridad.BackgroundColor = Color.White;
            dgvSeguridad.BorderStyle = BorderStyle.None;
            dgvSeguridad.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;
            dgvSeguridad.EnableHeadersVisualStyles = false;
            dgvSeguridad.ColumnHeadersHeight = 38;
            dgvSeguridad.RowTemplate.Height = 30;
            dgvSeguridad.GridColor = Color.FromArgb(230, 234, 240);

            dgvSeguridad.ColumnHeadersDefaultCellStyle.BackColor =
                azulOscuro;
            dgvSeguridad.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;
            dgvSeguridad.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            dgvSeguridad.DefaultCellStyle.Font =
                new Font("Segoe UI", 9);
            dgvSeguridad.DefaultCellStyle.ForeColor = texto;
            dgvSeguridad.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(218, 232, 250);
            dgvSeguridad.DefaultCellStyle.SelectionForeColor = texto;
            dgvSeguridad.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(246, 248, 252);

            dgvSeguridad.Columns.Add("Funcion", "FUNCIÓN");
            dgvSeguridad.Columns.Add("Descripcion", "DESCRIPCIÓN");

            dgvSeguridad.Columns["Funcion"].FillWeight = 35;
            dgvSeguridad.Columns["Descripcion"].FillWeight = 65;

            panelTabla.Controls.Add(lblTabla);
            panelTabla.Controls.Add(lblInfo);
            panelTabla.Controls.Add(dgvSeguridad);

            // INDICADOR INFERIOR
            Label lblEstado = new Label();
            lblEstado.Text = "●  SISTEMA DE PROTECCIÓN CONFIGURADO";
            lblEstado.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblEstado.ForeColor = Color.FromArgb(22, 128, 90);
            lblEstado.Location = new Point(35, 600);
            lblEstado.AutoSize = true;

            Button salir = new Button();
            salir.Text = "VOLVER AL MENÚ";
            salir.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            salir.ForeColor = Color.White;
            salir.BackColor = Color.FromArgb(190, 55, 55);
            salir.FlatStyle = FlatStyle.Flat;
            salir.FlatAppearance.BorderSize = 0;
            salir.Cursor = Cursors.Hand;
            salir.Location = new Point(675, 590);
            salir.Size = new Size(225, 45);
            salir.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                encabezado,
                tarjeta1,
                tarjeta2,
                tarjeta3,
                panelTabla,
                lblEstado,
                salir
            });
        }

        // CREAR TARJETAS
        private Panel CrearTarjeta(
            string titulo,
            string descripcion,
            Color color,
            Point ubicacion)
        {
            Panel tarjeta = new Panel();
            tarjeta.Location = ubicacion;
            tarjeta.Size = new Size(270, 115);
            tarjeta.BackColor = Color.White;

            Panel barra = new Panel();
            barra.Location = new Point(0, 0);
            barra.Size = new Size(7, 115);
            barra.BackColor = color;

            Label lblTitulo = new Label();
            lblTitulo.Text = titulo;
            lblTitulo.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(45, 55, 72);
            lblTitulo.Location = new Point(22, 28);
            lblTitulo.AutoSize = true;

            Label lblDescripcion = new Label();
            lblDescripcion.Text = descripcion;
            lblDescripcion.Font = new Font("Segoe UI", 9);
            lblDescripcion.ForeColor = color;
            lblDescripcion.Location = new Point(22, 60);
            lblDescripcion.AutoSize = true;

            tarjeta.Controls.Add(barra);
            tarjeta.Controls.Add(lblTitulo);
            tarjeta.Controls.Add(lblDescripcion);

            return tarjeta;
        }

        // CARGAR FUNCIONES DE SEGURIDAD
        private void CargarSeguridad()
        {
            dgvSeguridad.Rows.Clear();

            dgvSeguridad.Rows.Add(
                "Control de acceso",
                "Validación de permisos según el perfil del usuario.");

            dgvSeguridad.Rows.Add(
                "Intentos fallidos",
                "Registro de intentos incorrectos de inicio de sesión.");

            dgvSeguridad.Rows.Add(
                "Bloqueo de cuentas",
                "Control de bloqueos temporales y permanentes.");

            dgvSeguridad.Rows.Add(
                "Desbloqueo",
                "Permite desbloquear cuentas con bloqueo temporal.");

            dgvSeguridad.Rows.Add(
                "Protección de usuarios",
                "Administración de usuarios restringida al Administrador.");
        }
    }
}
