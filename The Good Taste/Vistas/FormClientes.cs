using System;
using System.Drawing;
using System.Windows.Forms;
using The_Good_Taste.Entidades;

namespace The_Good_Taste.Vistas
{
    public partial class FormClientes : Form
    {
        public FormClientes()
        {
            InitializeComponente();
            ConfigurarEstilos();
        }

        private void ConfigurarEstilos()
        {
            this.BackColor = Color.FromArgb(33, 37, 41);
            this.ForeColor = Color.White;
        }

        private void btnGuardarCliente_Click(object sender, EventArgs e)
        {
            try
            {
                // Instanciamos el cliente según nuestra Entidad
                Cliente nuevoCliente = new Cliente
                {
                    Dni = txtDni.Text.Trim(),
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    FechaNacimiento = dtpFechaNacimiento.Value,
                    Email = txtEmail.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    Calle = txtCalle.Text.Trim(),
                    Numero = txtNumero.Text.Trim(),
                    Barrio = txtBarrio.Text.Trim(),
                    FechaAlta = DateTime.Now,
                    Activo = true
                };

                // Lógica para guardar en MariaDB
                // ClienteLogica.RegistrarCliente(nuevoCliente);

                MessageBox.Show("Cliente registrado con éxito en el sistema.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            txtDni.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            txtCalle.Clear();
            txtNumero.Clear();
            txtBarrio.Clear();
        }
    }
}