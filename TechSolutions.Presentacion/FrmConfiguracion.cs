using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TechSolutions.Presentacion
{
    public class FrmConfiguracion : Form
    {
        public FrmConfiguracion()
        {
            CrearInterfaz();
        }

        private void CrearInterfaz()
        {
            this.Text = "TechSolutions - Configuración";
            this.Size = new Size(700, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            Label titulo = new Label();
            titulo.Text = "CONFIGURACIÓN DEL SISTEMA";
            titulo.Font = new Font("Segoe UI", 19, FontStyle.Bold);
            titulo.ForeColor = Color.FromArgb(35, 75, 130);
            titulo.AutoSize = true;
            titulo.Location = new Point(100, 35);
            this.Controls.Add(titulo);

            Label descripcion = new Label();
            descripcion.Text = "Información general de TechSolutions";
            descripcion.Font = new Font("Segoe UI", 10);
            descripcion.ForeColor = Color.Gray;
            descripcion.AutoSize = true;
            descripcion.Location = new Point(175, 80);
            this.Controls.Add(descripcion);

            CrearOpcion("Nombre del sistema",
                "TechSolutions", 135);

            CrearOpcion("Control de usuarios",
                "Administración de cuentas y perfiles", 195);

            CrearOpcion("Seguridad",
                "Protección y control de acceso", 255);

            Button salir = new Button();
            salir.Text = "VOLVER";
            salir.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            salir.ForeColor = Color.White;
            salir.BackColor = Color.FromArgb(190, 55, 55);
            salir.FlatStyle = FlatStyle.Flat;
            salir.FlatAppearance.BorderSize = 0;
            salir.Location = new Point(225, 335);
            salir.Size = new Size(230, 45);
            salir.Click += (s, e) => this.Close();

            this.Controls.Add(salir);
        }

        private void CrearOpcion(
            string nombre, string valor, int posicionY)
        {
            Label lblNombre = new Label();
            lblNombre.Text = nombre + ":";
            lblNombre.Font = new Font(
                "Segoe UI", 10, FontStyle.Bold);
            lblNombre.ForeColor = Color.FromArgb(35, 75, 130);
            lblNombre.Location = new Point(65, posicionY);
            lblNombre.Size = new Size(180, 30);

            Label lblValor = new Label();
            lblValor.Text = valor;
            lblValor.Font = new Font("Segoe UI", 10);
            lblValor.Location = new Point(260, posicionY);
            lblValor.Size = new Size(370, 30);

            this.Controls.Add(lblNombre);
            this.Controls.Add(lblValor);
        }
    }
}
