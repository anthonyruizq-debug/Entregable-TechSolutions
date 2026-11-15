
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TechSolutions.Negocio.Servicios;
using TechSolutions.Entidades.Seguridad;

namespace TechSolutions.Presentacion
{
    public partial class Form1 : Form
    {
        private UsuarioServicio usuarioServicio;

        private TextBox txtUsuario;
        private TextBox txtPassword;
        private CheckBox chkMostrarPassword;

        private Button btnIngresar;
        private Button btnSalir;
        private Button btnAbrirRegistro;

        private Panel panelLateral;
        private Panel panelLogin;

        private readonly Color azulOscuro =
            Color.FromArgb(18, 36, 58);

        private readonly Color azul =
            Color.FromArgb(35, 90, 155);

        private readonly Color verde =
            Color.FromArgb(35, 145, 105);

        public Form1()
        {
            usuarioServicio = new UsuarioServicio();
            CrearInterfaz();
        }

        private void CrearInterfaz()
        {
            Text = "TechSolutions - Iniciar Sesión";

            ClientSize = new Size(950, 600);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(241, 245, 250);
            Font = new Font("Segoe UI", 10);

            // PANEL IZQUIERDO
            panelLateral = new Panel();
            panelLateral.Dock = DockStyle.Left;
            panelLateral.Width = 340;
            panelLateral.BackColor = azulOscuro;
            Controls.Add(panelLateral);

            Label lblLogo = new Label();
            lblLogo.Text = "TS";
            lblLogo.Font = new Font(
                "Segoe UI", 30, FontStyle.Bold);
            lblLogo.ForeColor = Color.White;
            lblLogo.BackColor = azul;
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            lblLogo.Size = new Size(95, 95);
            lblLogo.Location = new Point(120, 105);
            panelLateral.Controls.Add(lblLogo);

            Label lblEmpresa = new Label();
            lblEmpresa.Text = "TECHSOLUTIONS";
            lblEmpresa.Font = new Font(
                "Segoe UI", 22, FontStyle.Bold);
            lblEmpresa.ForeColor = Color.White;
            lblEmpresa.AutoSize = false;
            lblEmpresa.TextAlign = ContentAlignment.MiddleCenter;
            lblEmpresa.Size = new Size(320, 45);
            lblEmpresa.Location = new Point(10, 225);
            panelLateral.Controls.Add(lblEmpresa);

            Label lblDescripcion = new Label();
            lblDescripcion.Text =
                "Sistema de gestión empresarial\n\n" +
                "Administra productos, ventas y usuarios\n" +
                "desde una sola plataforma.";
            lblDescripcion.Font = new Font(
                "Segoe UI", 10);
            lblDescripcion.ForeColor =
                Color.FromArgb(195, 210, 230);
            lblDescripcion.TextAlign =
                ContentAlignment.MiddleCenter;
            lblDescripcion.Size = new Size(300, 110);
            lblDescripcion.Location = new Point(20, 285);
            panelLateral.Controls.Add(lblDescripcion);

            Label lblVersion = new Label();
            lblVersion.Text = "SISTEMA DE GESTIÓN  |  v1.0";
            lblVersion.Font = new Font(
                "Segoe UI", 8, FontStyle.Bold);
            lblVersion.ForeColor =
                Color.FromArgb(140, 165, 195);
            lblVersion.AutoSize = false;
            lblVersion.TextAlign =
                ContentAlignment.MiddleCenter;
            lblVersion.Size = new Size(320, 30);
            lblVersion.Location = new Point(10, 535);
            panelLateral.Controls.Add(lblVersion);

            // PANEL DERECHO
            panelLogin = new Panel();
            panelLogin.Size = new Size(490, 490);
            panelLogin.Location = new Point(405, 55);
            panelLogin.BackColor = Color.White;
            panelLogin.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(panelLogin);

            Label lblBienvenida = new Label();
            lblBienvenida.Text = "¡Bienvenido!";
            lblBienvenida.Font = new Font(
                "Segoe UI", 25, FontStyle.Bold);
            lblBienvenida.ForeColor = azulOscuro;
            lblBienvenida.AutoSize = true;
            lblBienvenida.Location = new Point(55, 35);
            panelLogin.Controls.Add(lblBienvenida);

            Label lblSubtitulo = new Label();
            lblSubtitulo.Text =
                "Ingresa tus credenciales para continuar";
            lblSubtitulo.Font = new Font(
                "Segoe UI", 10);
            lblSubtitulo.ForeColor = Color.Gray;
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Location = new Point(58, 85);
            panelLogin.Controls.Add(lblSubtitulo);

            // USUARIO
            Label lblUsuario = new Label();
            lblUsuario.Text = "USUARIO";
            lblUsuario.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);
            lblUsuario.ForeColor = azulOscuro;
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(60, 145);
            panelLogin.Controls.Add(lblUsuario);

            txtUsuario = new TextBox();
            txtUsuario.Font = new Font(
                "Segoe UI", 12);
            txtUsuario.Location = new Point(60, 175);
            txtUsuario.Size = new Size(365, 35);
            txtUsuario.BorderStyle = BorderStyle.FixedSingle;
            panelLogin.Controls.Add(txtUsuario);

            // CONTRASEÑA
            Label lblPassword = new Label();
            lblPassword.Text = "CONTRASEÑA";
            lblPassword.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);
            lblPassword.ForeColor = azulOscuro;
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(60, 230);
            panelLogin.Controls.Add(lblPassword);

            txtPassword = new TextBox();
            txtPassword.Font = new Font(
                "Segoe UI", 12);
            txtPassword.Location = new Point(60, 260);
            txtPassword.Size = new Size(365, 35);
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            panelLogin.Controls.Add(txtPassword);

            // MOSTRAR CONTRASEÑA
            chkMostrarPassword = new CheckBox();
            chkMostrarPassword.Text = "Mostrar contraseña";
            chkMostrarPassword.Font = new Font(
                "Segoe UI", 9);
            chkMostrarPassword.ForeColor = Color.Gray;
            chkMostrarPassword.AutoSize = true;
            chkMostrarPassword.Location = new Point(60, 305);
            chkMostrarPassword.CheckedChanged +=
                chkMostrarPassword_CheckedChanged;
            panelLogin.Controls.Add(chkMostrarPassword);

            // BOTÓN INGRESAR
            btnIngresar = CrearBoton(
                "INICIAR SESIÓN",
                azul,
                Color.White,
                new Point(60, 345),
                new Size(365, 45));

            btnIngresar.Click += btnIngresar_Click;
            panelLogin.Controls.Add(btnIngresar);

            // BOTONES INFERIORES
            btnAbrirRegistro = CrearBoton(
                "REGISTRAR USUARIO",
                verde,
                Color.White,
                new Point(60, 405),
                new Size(175, 40));

            btnAbrirRegistro.Click += btnAbrirRegistro_Click;
            panelLogin.Controls.Add(btnAbrirRegistro);

            btnSalir = CrearBoton(
                "SALIR",
                Color.FromArgb(230, 235, 241),
                azulOscuro,
                new Point(250, 405),
                new Size(175, 40));

            btnSalir.Click += btnSalir_Click;
            panelLogin.Controls.Add(btnSalir);

            AcceptButton = btnIngresar;
        }

        private Button CrearBoton(
            string texto,
            Color fondo,
            Color textoColor,
            Point posicion,
            Size tamaño)
        {
            Button boton = new Button();

            boton.Text = texto;
            boton.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);
            boton.ForeColor = textoColor;
            boton.BackColor = fondo;
            boton.Location = posicion;
            boton.Size = tamaño;
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.Cursor = Cursors.Hand;

            return boton;
        }

        private void btnAbrirRegistro_Click(
            object sender, EventArgs e)
        {
            using (FrmRegistro registro = new FrmRegistro())
            {
                registro.ShowDialog(this);
            }
        }

        private void chkMostrarPassword_CheckedChanged(
            object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar =
                !chkMostrarPassword.Checked;
        }

        private void btnIngresar_Click(
            object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contraseña = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(contraseña))
            {
                MessageBox.Show(
                    "Ingrese su usuario y contraseña.",
                    "Advertencia",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                btnIngresar.Enabled = false;

                ResultadoLogin resultado =
                    usuarioServicio.IniciarSesion(
                        usuario, contraseña);

                if (resultado.Exitoso)
                {
                    txtPassword.Clear();

                    Hide();

                    using (FrmMenuPrincipal menu =
                        new FrmMenuPrincipal(
                            resultado.UsuarioId,
                            resultado.RolId,
                            resultado.Nombre))
                    {
                        menu.ShowDialog();
                    }

                    Show();

                    txtPassword.Clear();
                    txtUsuario.Clear();
                    txtUsuario.Focus();
                }
                else
                {
                    MessageBox.Show(
                        resultado.Mensaje,
                        "Acceso denegado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    if (resultado.UsuarioBloqueado)
                    {
                        txtUsuario.Clear();
                        txtPassword.Clear();
                    }
                    else
                    {
                        txtPassword.Clear();
                        txtPassword.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error: " + ex.Message,
                    "Error del sistema",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnIngresar.Enabled = true;
            }
        }

        private void btnSalir_Click(
            object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "¿Desea salir del sistema?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }
        }
    }
}
