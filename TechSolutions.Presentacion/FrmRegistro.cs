
using System;
using System.Drawing;
using System.Windows.Forms;
using TechSolutions.Negocio.Servicios;

namespace TechSolutions.Presentacion
{
    public partial class FrmRegistro : Form
    {
        private UsuarioServicio servicio;

        private TextBox txtNombre;
        private TextBox txtUsuario;
        private TextBox txtPassword;
        private TextBox txtConfirmar;

        private ComboBox cmbRol;

        private Button btnRegistrar;
        private Button btnCancelar;

        public FrmRegistro()
        {
            InitializeComponent();

            servicio = new UsuarioServicio();

            CrearFormulario();
        }

        private void CrearFormulario()
        {
            this.Controls.Clear();

            this.Text = "Registro de usuarios - TechSolutions";
            this.Size = new Size(450, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            Label titulo = new Label();
            titulo.Text = "REGISTRO DE USUARIO";
            titulo.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            titulo.ForeColor = Color.FromArgb(35, 75, 130);
            titulo.Location = new Point(65, 25);
            titulo.AutoSize = true;
            this.Controls.Add(titulo);

            CrearCampo("Nombre completo", ref txtNombre, 75);
            CrearCampo("Nombre de usuario", ref txtUsuario, 145);
            CrearCampo("Contraseña", ref txtPassword, 215);
            CrearCampo("Confirmar contraseña", ref txtConfirmar, 285);

            txtPassword.UseSystemPasswordChar = true;
            txtConfirmar.UseSystemPasswordChar = true;

            // Selección del rol
            Label lblRol = new Label();
            lblRol.Text = "Seleccionar rol:";
            lblRol.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblRol.Location = new Point(65, 355);
            lblRol.AutoSize = true;
            this.Controls.Add(lblRol);

            cmbRol = new ComboBox();
            cmbRol.Location = new Point(65, 380);
            cmbRol.Size = new Size(280, 30);
            cmbRol.Font = new Font("Segoe UI", 10);
            cmbRol.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbRol.Items.Add("Administrador");
            cmbRol.Items.Add("Personal");
            cmbRol.Items.Add("Encargado");

            cmbRol.SelectedIndex = 1;

            this.Controls.Add(cmbRol);

            // Botón registrar
            btnRegistrar = new Button();
            btnRegistrar.Text = "REGISTRAR";
            btnRegistrar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnRegistrar.Location = new Point(65, 435);
            btnRegistrar.Size = new Size(130, 40);
            btnRegistrar.BackColor = Color.FromArgb(35, 100, 180);
            btnRegistrar.ForeColor = Color.White;
            btnRegistrar.FlatStyle = FlatStyle.Flat;
            btnRegistrar.Click += btnRegistrar_Click;
            this.Controls.Add(btnRegistrar);

            // Botón cancelar
            btnCancelar = new Button();
            btnCancelar.Text = "CANCELAR";
            btnCancelar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnCancelar.Location = new Point(215, 435);
            btnCancelar.Size = new Size(130, 40);
            btnCancelar.Click += btnCancelar_Click;
            this.Controls.Add(btnCancelar);
        }

        private void CrearCampo(string titulo, ref TextBox caja, int posicion)
        {
            Label etiqueta = new Label();
            etiqueta.Text = titulo;
            etiqueta.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            etiqueta.Location = new Point(65, posicion);
            etiqueta.AutoSize = true;
            this.Controls.Add(etiqueta);

            caja = new TextBox();
            caja.Font = new Font("Segoe UI", 10);
            caja.Location = new Point(65, posicion + 25);
            caja.Size = new Size(280, 25);
            this.Controls.Add(caja);
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Text;
            string confirmar = txtConfirmar.Text;

            if (nombre == "" || usuario == "" ||
                password == "" || confirmar == "")
            {
                MessageBox.Show("Complete todos los campos.");
                return;
            }

            if (password != confirmar)
            {
                MessageBox.Show("Las contraseñas no coinciden.");
                return;
            }

            if (cmbRol.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un rol.");
                return;
            }

            try
            {
                // Obtener el ID del rol seleccionado
                int rolId = cmbRol.SelectedIndex + 1;

                bool resultado = servicio.RegistrarUsuario(
                    nombre,
                    usuario,
                    password,
                    rolId
                );

                if (resultado)
                {
                    MessageBox.Show(
                        "Usuario registrado correctamente.",
                        "Registro exitoso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    txtNombre.Clear();
                    txtUsuario.Clear();
                    txtPassword.Clear();
                    txtConfirmar.Clear();

                    cmbRol.SelectedIndex = 1;
                    txtNombre.Focus();
                }
                else
                {
                    MessageBox.Show("No se pudo registrar el usuario.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error: " + ex.Message,
                    "Error de registro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
