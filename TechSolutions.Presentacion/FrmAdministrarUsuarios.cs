
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TechSolutions.Entidades.Usuarios;
using TechSolutions.Negocio.Servicios;

namespace TechSolutions.Presentacion
{
    public partial class FrmAdministrarUsuarios : Form
    {
        private readonly UsuarioServicio usuarioServicio;
        private readonly int rolId;

        private DataGridView dgvUsuarios;
        private Label lblTotal;
        private Label lblActivos;
        private Label lblBloqueados;
        private TextBox txtBuscar;

        private Button btnRegistrar;
        private Button btnEditar;
        private Button btnActualizar;
        private Button btnDesbloquear;
        private Button btnCerrar;

        private List<Usuario> usuarios;

        private readonly Color azulOscuro =
            Color.FromArgb(18, 36, 58);

        private readonly Color azul =
            Color.FromArgb(35, 90, 155);

        private readonly Color fondo =
            Color.FromArgb(241, 245, 250);

        public FrmAdministrarUsuarios(int rolId)
        {
            this.rolId = rolId;

            if (rolId != 1)
            {
                MessageBox.Show(
                    "Acceso denegado. Solo el Administrador puede ingresar.",
                    "Seguridad del sistema",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                this.Load += (s, e) => this.Close();
                return;
            }

            usuarioServicio = new UsuarioServicio();
            usuarios = new List<Usuario>();

            InicializarFormulario();
            CargarUsuarios();
        }

        // ==========================================
        // DISEÑO PRINCIPAL
        // ==========================================

        private void InicializarFormulario()
        {
            Text = "TechSolutions | Administración de usuarios";

            ClientSize = new Size(1150, 760);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = fondo;
            Font = new Font("Segoe UI", 10);

            // ENCABEZADO
            Panel panelSuperior = new Panel();
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Height = 105;
            panelSuperior.BackColor = azulOscuro;

            Label lblTitulo = new Label();
            lblTitulo.Text = "ADMINISTRACIÓN DE USUARIOS";
            lblTitulo.Font = new Font(
                "Segoe UI", 21, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(35, 17);

            Label lblSubtitulo = new Label();
            lblSubtitulo.Text =
                "Gestiona las cuentas, perfiles y seguridad del sistema";
            lblSubtitulo.Font = new Font("Segoe UI", 10);
            lblSubtitulo.ForeColor =
                Color.FromArgb(190, 205, 225);
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Location = new Point(39, 65);

            Label lblIcono = new Label();
            lblIcono.Text = "USUARIOS";
            lblIcono.Font = new Font(
                "Segoe UI", 11, FontStyle.Bold);
            lblIcono.ForeColor = Color.White;
            lblIcono.BackColor = Color.FromArgb(35, 90, 155);
            lblIcono.TextAlign = ContentAlignment.MiddleCenter;
            lblIcono.Size = new Size(135, 42);
            lblIcono.Location = new Point(965, 30);

            panelSuperior.Controls.Add(lblTitulo);
            panelSuperior.Controls.Add(lblSubtitulo);
            panelSuperior.Controls.Add(lblIcono);

            Controls.Add(panelSuperior);

            // TARJETAS DE RESUMEN
            Panel tarjetaTotal = CrearTarjetaResumen(
                "TOTAL DE USUARIOS",
                "0",
                Color.FromArgb(35, 90, 155),
                30,
                out lblTotal);

            Panel tarjetaActivos = CrearTarjetaResumen(
                "USUARIOS ACTIVOS",
                "0",
                Color.FromArgb(35, 150, 105),
                390,
                out lblActivos);

            Panel tarjetaBloqueados = CrearTarjetaResumen(
                "USUARIOS BLOQUEADOS",
                "0",
                Color.FromArgb(210, 75, 75),
                750,
                out lblBloqueados);

            Controls.Add(tarjetaTotal);
            Controls.Add(tarjetaActivos);
            Controls.Add(tarjetaBloqueados);

            // PANEL DE BÚSQUEDA
            Panel panelBusqueda = new Panel();
            panelBusqueda.Location = new Point(30, 230);
            panelBusqueda.Size = new Size(1090, 75);
            panelBusqueda.BackColor = Color.White;

            Label lblBuscar = new Label();
            lblBuscar.Text = "BUSCAR USUARIO";
            lblBuscar.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);
            lblBuscar.ForeColor = azulOscuro;
            lblBuscar.Location = new Point(20, 12);
            lblBuscar.AutoSize = true;

            txtBuscar = new TextBox();
            txtBuscar.Location = new Point(20, 37);
            txtBuscar.Size = new Size(400, 27);
            txtBuscar.Font = new Font("Segoe UI", 11);
            txtBuscar.BorderStyle = BorderStyle.FixedSingle;
            txtBuscar.TextChanged += TxtBuscar_TextChanged;

            Label lblAyuda = new Label();
            lblAyuda.Text =
                "Puedes buscar por nombre o nombre de usuario.";
            lblAyuda.Location = new Point(445, 40);
            lblAyuda.AutoSize = true;
            lblAyuda.ForeColor = Color.Gray;
            lblAyuda.Font = new Font("Segoe UI", 9);

            panelBusqueda.Controls.Add(lblBuscar);
            panelBusqueda.Controls.Add(txtBuscar);
            panelBusqueda.Controls.Add(lblAyuda);

            Controls.Add(panelBusqueda);

            // TABLA
            Panel panelTabla = new Panel();
            panelTabla.Location = new Point(30, 325);
            panelTabla.Size = new Size(1090, 290);
            panelTabla.BackColor = Color.White;

            dgvUsuarios = new DataGridView();
            dgvUsuarios.Dock = DockStyle.Fill;
            dgvUsuarios.BackgroundColor = Color.White;
            dgvUsuarios.BorderStyle = BorderStyle.None;
            dgvUsuarios.AllowUserToAddRows = false;
            dgvUsuarios.AllowUserToDeleteRows = false;
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.RowHeadersVisible = false;
            dgvUsuarios.EnableHeadersVisualStyles = false;
            dgvUsuarios.AllowUserToResizeRows = false;

            dgvUsuarios.ColumnHeadersDefaultCellStyle.BackColor =
                azulOscuro;

            dgvUsuarios.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvUsuarios.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            dgvUsuarios.ColumnHeadersHeight = 42;

            dgvUsuarios.DefaultCellStyle.Font =
                new Font("Segoe UI", 9);

            dgvUsuarios.DefaultCellStyle.ForeColor =
                Color.FromArgb(45, 55, 70);

            dgvUsuarios.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(210, 230, 250);

            dgvUsuarios.DefaultCellStyle.SelectionForeColor =
                Color.Black;

            dgvUsuarios.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(245, 248, 252);

            dgvUsuarios.RowTemplate.Height = 38;

            dgvUsuarios.GridColor =
                Color.FromArgb(225, 230, 238);

            panelTabla.Controls.Add(dgvUsuarios);
            Controls.Add(panelTabla);

            // PANEL DE ACCIONES
            Label lblAcciones = new Label();
            lblAcciones.Text = "ACCIONES DE USUARIO";
            lblAcciones.Font = new Font(
                "Segoe UI", 10, FontStyle.Bold);
            lblAcciones.ForeColor = azulOscuro;
            lblAcciones.AutoSize = true;
            lblAcciones.Location = new Point(35, 635);
            Controls.Add(lblAcciones);

            btnRegistrar = CrearBoton(
                "＋  REGISTRAR",
                Color.FromArgb(35, 150, 105),
                30);

            btnEditar = CrearBoton(
                "✎  EDITAR",
                Color.FromArgb(35, 115, 190),
                250);

            btnActualizar = CrearBoton(
                "↻  ACTUALIZAR",
                Color.FromArgb(90, 100, 115),
                470);

            btnDesbloquear = CrearBoton(
                "🔓  DESBLOQUEAR",
                Color.FromArgb(220, 145, 35),
                690);

            btnCerrar = CrearBoton(
                "✕  CERRAR",
                Color.FromArgb(190, 65, 65),
                910);

            btnRegistrar.Click += BtnRegistrar_Click;
            btnEditar.Click += BtnEditar_Click;
            btnActualizar.Click += BtnActualizar_Click;
            btnDesbloquear.Click += BtnDesbloquear_Click;
            btnCerrar.Click += (s, e) => Close();

            Controls.Add(btnRegistrar);
            Controls.Add(btnEditar);
            Controls.Add(btnActualizar);
            Controls.Add(btnDesbloquear);
            Controls.Add(btnCerrar);

            CrearColumnas();
        }

        // ==========================================
        // TARJETAS DE RESUMEN
        // ==========================================

        private Panel CrearTarjetaResumen(
            string titulo,
            string valor,
            Color color,
            int x,
            out Label lblValor)
        {
            Panel tarjeta = new Panel();

            tarjeta.Location = new Point(x, 125);
            tarjeta.Size = new Size(340, 85);
            tarjeta.BackColor = Color.White;

            Panel indicador = new Panel();
            indicador.Location = new Point(0, 0);
            indicador.Size = new Size(7, 85);
            indicador.BackColor = color;

            Label lblTitulo = new Label();
            lblTitulo.Text = titulo;
            lblTitulo.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);
            lblTitulo.ForeColor = Color.Gray;
            lblTitulo.Location = new Point(22, 15);
            lblTitulo.AutoSize = true;

            lblValor = new Label();
            lblValor.Text = valor;
            lblValor.Font = new Font(
                "Segoe UI", 23, FontStyle.Bold);
            lblValor.ForeColor = color;
            lblValor.Location = new Point(20, 35);
            lblValor.AutoSize = true;

            tarjeta.Controls.Add(indicador);
            tarjeta.Controls.Add(lblTitulo);
            tarjeta.Controls.Add(lblValor);

            return tarjeta;
        }

        // ==========================================
        // BOTONES
        // ==========================================

        private Button CrearBoton(
            string texto,
            Color color,
            int x)
        {
            Button boton = new Button();

            boton.Text = texto;
            boton.Location = new Point(x, 670);
            boton.Size = new Size(195, 48);
            boton.BackColor = color;
            boton.ForeColor = Color.White;
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);
            boton.Cursor = Cursors.Hand;

            boton.MouseEnter += (s, e) =>
            {
                boton.BackColor = ControlPaint.Dark(color, 0.12f);
            };

            boton.MouseLeave += (s, e) =>
            {
                boton.BackColor = color;
            };

            return boton;
        }

        // ==========================================
        // COLUMNAS DE LA TABLA
        // ==========================================

        private void CrearColumnas()
        {
            dgvUsuarios.Columns.Clear();

            dgvUsuarios.Columns.Add("Id", "ID");
            dgvUsuarios.Columns.Add("Nombre", "NOMBRE");
            dgvUsuarios.Columns.Add("Usuario", "USUARIO");
            dgvUsuarios.Columns.Add("Perfil", "PERFIL");
            dgvUsuarios.Columns.Add("Intentos", "INTENTOS");
            dgvUsuarios.Columns.Add("Estado", "ESTADO");
            dgvUsuarios.Columns.Add("Bloqueo", "TIPO DE BLOQUEO");
        }

        // ==========================================
        // CARGAR USUARIOS
        // ==========================================

        private void CargarUsuarios()
        {
            try
            {
                usuarios = usuarioServicio.ObtenerTodosLosUsuarios();

                MostrarUsuarios(usuarios);
                ActualizarResumen();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar usuarios: " + ex.Message,
                    "TechSolutions",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void MostrarUsuarios(List<Usuario> lista)
        {
            dgvUsuarios.Rows.Clear();

            foreach (Usuario usuario in lista)
            {
                string perfil = ObtenerNombreRol(usuario.RolId);

                string estado = usuario.Bloqueado
                    ? "BLOQUEADO"
                    : "ACTIVO";

                string tipoBloqueo = usuario.BloqueoPermanente
                    ? "PERMANENTE"
                    : usuario.Bloqueado ? "TEMPORAL" : "NINGUNO";

                int fila = dgvUsuarios.Rows.Add(
                    usuario.Id,
                    usuario.Nombre,
                    usuario.UsuarioNombre,
                    perfil,
                    usuario.IntentosFallidos,
                    estado,
                    tipoBloqueo);

                if (usuario.Bloqueado)
                {
                    dgvUsuarios.Rows[fila].DefaultCellStyle.ForeColor =
                        Color.DarkRed;
                }
            }
        }

        private string ObtenerNombreRol(int rolId)
        {
            switch (rolId)
            {
                case 1: return "Administrador";
                case 2: return "Personal";
                case 3: return "Encargado";
                default: return "Desconocido";
            }
        }

        // ==========================================
        // RESUMEN DE USUARIOS
        // ==========================================

        private void ActualizarResumen()
        {
            int total = usuarios.Count;
            int bloqueados = usuarios.Count(u => u.Bloqueado);
            int activos = total - bloqueados;

            lblTotal.Text = total.ToString();
            lblActivos.Text = activos.ToString();
            lblBloqueados.Text = bloqueados.ToString();
        }

        // ==========================================
        // BUSCAR USUARIOS CON LINQ
        // ==========================================

        private void TxtBuscar_TextChanged(
            object sender, EventArgs e)
        {
            string busqueda = txtBuscar.Text.Trim();

            List<Usuario> filtrados = usuarios
                .Where(u =>
                    (!string.IsNullOrEmpty(u.Nombre) &&
                     u.Nombre.IndexOf(
                         busqueda,
                         StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(u.UsuarioNombre) &&
                     u.UsuarioNombre.IndexOf(
                         busqueda,
                         StringComparison.OrdinalIgnoreCase) >= 0))
                .ToList();

            MostrarUsuarios(filtrados);
        }

        // ==========================================
        // REGISTRAR USUARIO
        // ==========================================

        private void BtnRegistrar_Click(
            object sender, EventArgs e)
        {
            using (FrmRegistro formulario = new FrmRegistro())
            {
                formulario.ShowDialog(this);
            }

            txtBuscar.Clear();
            CargarUsuarios();
        }

        // ==========================================
        // EDITAR USUARIO
        // ==========================================

        private void BtnEditar_Click(
            object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null)
            {
                MessageBox.Show(
                    "Seleccione un usuario de la tabla.");
                return;
            }

            int idUsuario = Convert.ToInt32(
                dgvUsuarios.CurrentRow.Cells["Id"].Value);

            Usuario usuario = usuarios.Find(
                u => u.Id == idUsuario);

            if (usuario == null)
            {
                MessageBox.Show(
                    "No se encontró el usuario seleccionado.");
                return;
            }

            AbrirVentanaEdicion(usuario);
        }

        private void AbrirVentanaEdicion(Usuario usuario)
        {
            Form formulario = new Form();

            formulario.Text = "TechSolutions | Editar usuario";
            formulario.Size = new Size(500, 490);
            formulario.StartPosition = FormStartPosition.CenterParent;
            formulario.FormBorderStyle = FormBorderStyle.FixedDialog;
            formulario.MaximizeBox = false;
            formulario.MinimizeBox = false;
            formulario.BackColor = fondo;
            formulario.Font = new Font("Segoe UI", 10);

            Panel panelSuperior = new Panel();
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Height = 75;
            panelSuperior.BackColor = azulOscuro;

            Label lblTitulo = new Label();
            lblTitulo.Text = "EDITAR USUARIO";
            lblTitulo.Font = new Font(
                "Segoe UI", 17, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(25, 20);
            lblTitulo.AutoSize = true;

            panelSuperior.Controls.Add(lblTitulo);

            Label lblNombre = CrearEtiquetaEdicion(
                "Nombre completo:", 40, 100);

            TextBox txtNombre = new TextBox();
            txtNombre.Location = new Point(40, 128);
            txtNombre.Size = new Size(400, 30);
            txtNombre.Text = usuario.Nombre;
            txtNombre.Font = new Font("Segoe UI", 11);

            Label lblUsuario = CrearEtiquetaEdicion(
                "Nombre de usuario:", 40, 185);

            TextBox txtUsuario = new TextBox();
            txtUsuario.Location = new Point(40, 213);
            txtUsuario.Size = new Size(400, 30);
            txtUsuario.Text = usuario.UsuarioNombre;
            txtUsuario.Font = new Font("Segoe UI", 11);

            Label lblRol = CrearEtiquetaEdicion(
                "Perfil del usuario:", 40, 270);

            ComboBox cmbRol = new ComboBox();
            cmbRol.Location = new Point(40, 298);
            cmbRol.Size = new Size(400, 30);
            cmbRol.DropDownStyle =
                ComboBoxStyle.DropDownList;
            cmbRol.Font = new Font("Segoe UI", 10);

            cmbRol.Items.Add(
                new KeyValuePair<int, string>(1, "Administrador"));

            cmbRol.Items.Add(
                new KeyValuePair<int, string>(2, "Personal"));

            cmbRol.Items.Add(
                new KeyValuePair<int, string>(3, "Encargado"));

            cmbRol.DisplayMember = "Value";
            cmbRol.ValueMember = "Key";

            for (int i = 0; i < cmbRol.Items.Count; i++)
            {
                KeyValuePair<int, string> item =
                    (KeyValuePair<int, string>)cmbRol.Items[i];

                if (item.Key == usuario.RolId)
                {
                    cmbRol.SelectedIndex = i;
                    break;
                }
            }

            Button btnGuardar = CrearBotonEdicion(
                "GUARDAR CAMBIOS",
                Color.FromArgb(35, 150, 105),
                new Point(40, 370),
                new Size(195, 45));

            Button btnCancelar = CrearBotonEdicion(
                "CANCELAR",
                Color.FromArgb(90, 100, 115),
                new Point(255, 370),
                new Size(185, 45));

            btnGuardar.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtUsuario.Text))
                {
                    MessageBox.Show(
                        "Complete todos los campos.");
                    return;
                }

                if (cmbRol.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Seleccione un perfil.");
                    return;
                }

                KeyValuePair<int, string> rolSeleccionado =
                    (KeyValuePair<int, string>)cmbRol.SelectedItem;

                try
                {
                    bool resultado =
                        usuarioServicio.ActualizarUsuario(
                            usuario.Id,
                            txtNombre.Text.Trim(),
                            txtUsuario.Text.Trim(),
                            rolSeleccionado.Key);

                    if (resultado)
                    {
                        MessageBox.Show(
                            "Usuario actualizado correctamente.");

                        formulario.DialogResult = DialogResult.OK;
                        formulario.Close();
                    }
                    else
                    {
                        MessageBox.Show(
                            "No se realizaron cambios.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message,
                        "Error al actualizar");
                }
            };

            btnCancelar.Click += (s, e) =>
                formulario.Close();

            formulario.Controls.Add(panelSuperior);
            formulario.Controls.Add(lblNombre);
            formulario.Controls.Add(txtNombre);
            formulario.Controls.Add(lblUsuario);
            formulario.Controls.Add(txtUsuario);
            formulario.Controls.Add(lblRol);
            formulario.Controls.Add(cmbRol);
            formulario.Controls.Add(btnGuardar);
            formulario.Controls.Add(btnCancelar);

            if (formulario.ShowDialog(this) == DialogResult.OK)
            {
                txtBuscar.Clear();
                CargarUsuarios();
            }
        }

        private Label CrearEtiquetaEdicion(
            string texto, int x, int y)
        {
            Label etiqueta = new Label();

            etiqueta.Text = texto;
            etiqueta.Location = new Point(x, y);
            etiqueta.AutoSize = true;
            etiqueta.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);
            etiqueta.ForeColor = azulOscuro;

            return etiqueta;
        }

        private Button CrearBotonEdicion(
            string texto,
            Color color,
            Point posicion,
            Size tamaño)
        {
            Button boton = new Button();

            boton.Text = texto;
            boton.Location = posicion;
            boton.Size = tamaño;
            boton.BackColor = color;
            boton.ForeColor = Color.White;
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);
            boton.Cursor = Cursors.Hand;

            return boton;
        }

        // ==========================================
        // ACTUALIZAR LISTA
        // ==========================================

        private void BtnActualizar_Click(
            object sender, EventArgs e)
        {
            txtBuscar.Clear();
            CargarUsuarios();

            MessageBox.Show(
                "Lista de usuarios actualizada.");
        }

        // ==========================================
        // DESBLOQUEAR USUARIO
        // ==========================================

        private void BtnDesbloquear_Click(
            object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null)
            {
                MessageBox.Show(
                    "Seleccione un usuario de la tabla.");
                return;
            }

            int idUsuario = Convert.ToInt32(
                dgvUsuarios.CurrentRow.Cells["Id"].Value);

            Usuario usuario = usuarios.Find(
                u => u.Id == idUsuario);

            if (usuario == null)
                return;

            if (!usuario.Bloqueado)
            {
                MessageBox.Show(
                    "El usuario seleccionado no está bloqueado.");
                return;
            }

            if (usuario.BloqueoPermanente)
            {
                MessageBox.Show(
                    "Esta cuenta tiene un bloqueo permanente y no puede desbloquearse.");
                return;
            }

            DialogResult confirmacion = MessageBox.Show(
                "¿Está seguro de desbloquear al usuario " +
                usuario.UsuarioNombre + "?",
                "Confirmar desbloqueo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                bool resultado =
                    usuarioServicio.DesbloquearUsuario(idUsuario);

                if (resultado)
                {
                    MessageBox.Show(
                        "Usuario desbloqueado correctamente.");
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo desbloquear la cuenta.");
                }

                CargarUsuarios();
            }
        }
    }
}
