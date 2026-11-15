
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TechSolutions.Presentacion
{
    public class FrmGestionSistema : Form
    {
        private Color azulOscuro = Color.FromArgb(23, 37, 61);
        private Color azul = Color.FromArgb(37, 99, 180);
        private Color fondo = Color.FromArgb(242, 245, 250);

        public FrmGestionSistema()
        {
            CrearInterfaz();
        }

        private void CrearInterfaz()
        {
            Text = "Gestión del sistema - TechSolutions";
            Size = new Size(1000, 700);
            MinimumSize = new Size(1000, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = fondo;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ENCABEZADO
            Panel encabezado = new Panel();
            encabezado.Dock = DockStyle.Top;
            encabezado.Height = 160;
            encabezado.BackColor = azulOscuro;
            Controls.Add(encabezado);

            Label titulo = new Label();
            titulo.Text = "GESTIÓN DEL SISTEMA";
            titulo.Font = new Font("Segoe UI", 23, FontStyle.Bold);
            titulo.ForeColor = Color.White;
            titulo.Location = new Point(35, 35);
            titulo.AutoSize = true;
            encabezado.Controls.Add(titulo);

            Label subtitulo = new Label();
            subtitulo.Text = "Administra los usuarios, perfiles y configuraciones de TechSolutions";
            subtitulo.Font = new Font("Segoe UI", 10);
            subtitulo.ForeColor = Color.FromArgb(210, 220, 235);
            subtitulo.Location = new Point(40, 90);
            subtitulo.AutoSize = true;
            encabezado.Controls.Add(subtitulo);

            // TARJETAS
            CrearTarjeta(
                "01",
                "GESTIÓN DE USUARIOS",
                "Registrar, consultar y administrar los usuarios del sistema.",
                "ABRIR MÓDULO",
                35, 195,
                Color.FromArgb(37, 99, 180),
                (s, e) =>
                {
                    AbrirFormulario(new FrmAdministrarUsuarios(1));
                }
            );

            CrearTarjeta(
                "02",
                "GESTIÓN DE PERFILES",
                "Administrar los perfiles y permisos de los usuarios.",
                "ABRIR MÓDULO",
                510, 195,
                Color.FromArgb(112, 78, 180),
                (s, e) =>
                {
                    AbrirFormulario(new FrmGestionPerfiles());
                }
            );

            CrearTarjeta(
                "03",
                "SEGURIDAD DEL SISTEMA",
                "Consultar y administrar las opciones de seguridad.",
                "ABRIR MÓDULO",
                35, 390,
                Color.FromArgb(20, 145, 110),
                (s, e) =>
                {
                    AbrirFormulario(new FrmSeguridadSistema());
                }
            );

            CrearTarjeta(
                "04",
                "CONFIGURACIÓN",
                "Acceder a las opciones de configuración del sistema.",
                "ABRIR MÓDULO",
                510, 390,
                Color.FromArgb(210, 140, 35),
                (s, e) =>
                {
                    AbrirFormulario(new FrmConfiguracion());
                }
            );

            // PIE DE PÁGINA
            Panel pie = new Panel();
            pie.Location = new Point(0, 600);
            pie.Size = new Size(1000, 65);
            pie.BackColor = Color.White;
            Controls.Add(pie);

            Label marca = new Label();
            marca.Text = "TECHSOLUTIONS  |  PANEL ADMINISTRATIVO";
            marca.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            marca.ForeColor = Color.Gray;
            marca.Location = new Point(30, 24);
            marca.AutoSize = true;
            pie.Controls.Add(marca);

            Button btnVolver = new Button();
            btnVolver.Text = "VOLVER AL MENÚ";
            btnVolver.Size = new Size(190, 40);
            btnVolver.Location = new Point(765, 12);
            btnVolver.BackColor = azulOscuro;
            btnVolver.ForeColor = Color.White;
            btnVolver.FlatStyle = FlatStyle.Flat;
            btnVolver.FlatAppearance.BorderSize = 0;
            btnVolver.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnVolver.Cursor = Cursors.Hand;
            btnVolver.Click += (s, e) => Close();
            pie.Controls.Add(btnVolver);
        }

        // CREAR TARJETA CON SU PROPIO BOTÓN FUNCIONAL
        private void CrearTarjeta(
            string numero,
            string titulo,
            string descripcion,
            string textoBoton,
            int x,
            int y,
            Color color,
            EventHandler evento)
        {
            Panel tarjeta = new Panel();
            tarjeta.Location = new Point(x, y);
            tarjeta.Size = new Size(440, 175);
            tarjeta.BackColor = Color.White;
            tarjeta.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(tarjeta);

            Label lblNumero = new Label();
            lblNumero.Text = numero;
            lblNumero.Font = new Font("Segoe UI", 17, FontStyle.Bold);
            lblNumero.ForeColor = color;
            lblNumero.Location = new Point(20, 15);
            lblNumero.AutoSize = true;
            tarjeta.Controls.Add(lblNumero);

            Label lblTitulo = new Label();
            lblTitulo.Text = titulo;
            lblTitulo.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitulo.ForeColor = azulOscuro;
            lblTitulo.Location = new Point(80, 20);
            lblTitulo.AutoSize = true;
            tarjeta.Controls.Add(lblTitulo);

            Label lblDescripcion = new Label();
            lblDescripcion.Text = descripcion;
            lblDescripcion.Font = new Font("Segoe UI", 9);
            lblDescripcion.ForeColor = Color.DimGray;
            lblDescripcion.Location = new Point(25, 65);
            lblDescripcion.Size = new Size(390, 40);
            tarjeta.Controls.Add(lblDescripcion);

            Button btnAbrir = new Button();
            btnAbrir.Text = textoBoton;
            btnAbrir.Size = new Size(180, 38);
            btnAbrir.Location = new Point(25, 120);
            btnAbrir.BackColor = color;
            btnAbrir.ForeColor = Color.White;
            btnAbrir.FlatStyle = FlatStyle.Flat;
            btnAbrir.FlatAppearance.BorderSize = 0;
            btnAbrir.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnAbrir.Cursor = Cursors.Hand;

            // AQUÍ SE CONECTA EL EVENTO DEL BOTÓN
            btnAbrir.Click += evento;

            tarjeta.Controls.Add(btnAbrir);
        }

        // ABRIR FORMULARIO
        private void AbrirFormulario(Form formulario)
        {
            using (formulario)
            {
                formulario.StartPosition = FormStartPosition.CenterScreen;
                formulario.ShowDialog(this);
            }
        }
    }
}
