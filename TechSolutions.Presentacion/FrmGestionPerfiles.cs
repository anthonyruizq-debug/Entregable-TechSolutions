using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TechSolutions.Presentacion
{
    public class FrmGestionPerfiles : Form
    {
        private DataGridView dgvPerfiles;

        public FrmGestionPerfiles()
        {
            CrearInterfaz();
            CargarPerfiles();
        }

        private void CrearInterfaz()
        {
            this.Text = "TechSolutions - Gestión de Perfiles";
            this.Size = new Size(700, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            Label titulo = new Label();
            titulo.Text = "GESTIÓN DE PERFILES";
            titulo.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            titulo.ForeColor = Color.FromArgb(35, 75, 130);
            titulo.AutoSize = true;
            titulo.Location = new Point(170, 30);
            this.Controls.Add(titulo);

            Label descripcion = new Label();
            descripcion.Text = "Perfiles y niveles de acceso del sistema";
            descripcion.Font = new Font("Segoe UI", 10);
            descripcion.ForeColor = Color.Gray;
            descripcion.AutoSize = true;
            descripcion.Location = new Point(175, 75);
            this.Controls.Add(descripcion);

            dgvPerfiles = new DataGridView();
            dgvPerfiles.Location = new Point(35, 125);
            dgvPerfiles.Size = new Size(615, 190);
            dgvPerfiles.ReadOnly = true;
            dgvPerfiles.AllowUserToAddRows = false;
            dgvPerfiles.AllowUserToDeleteRows = false;
            dgvPerfiles.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvPerfiles.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvPerfiles.MultiSelect = false;
            dgvPerfiles.RowHeadersVisible = false;
            dgvPerfiles.BackgroundColor = Color.White;
            dgvPerfiles.BorderStyle = BorderStyle.FixedSingle;

            dgvPerfiles.Columns.Add("RolId", "ID");
            dgvPerfiles.Columns.Add("Nombre", "Perfil");
            dgvPerfiles.Columns.Add("Descripcion", "Descripción");

            dgvPerfiles.Columns["RolId"].FillWeight = 15;
            dgvPerfiles.Columns["Nombre"].FillWeight = 30;
            dgvPerfiles.Columns["Descripcion"].FillWeight = 55;

            this.Controls.Add(dgvPerfiles);

            Button salir = new Button();
            salir.Text = "VOLVER";
            salir.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            salir.ForeColor = Color.White;
            salir.BackColor = Color.FromArgb(190, 55, 55);
            salir.FlatStyle = FlatStyle.Flat;
            salir.FlatAppearance.BorderSize = 0;
            salir.Location = new Point(225, 340);
            salir.Size = new Size(230, 45);
            salir.Click += (s, e) => this.Close();

            this.Controls.Add(salir);
        }

        private void CargarPerfiles()
        {
            dgvPerfiles.Rows.Clear();

            dgvPerfiles.Rows.Add(
                1, "Administrador", "Control total del sistema");

            dgvPerfiles.Rows.Add(
                2, "Personal", "Control limitado");

            dgvPerfiles.Rows.Add(
                3, "Encargado", "Control parcial");
        }
    }
}