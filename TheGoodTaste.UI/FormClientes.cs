using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheGoodTaste.UI
{
    public partial class FormClientes : Form
    {
        public FormClientes()
        {
            InitializeComponent();
            ConfigurarEventos();
        }

        private void FormClientes_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);
            LimpiarCampos();
        }

        private void ConfigurarEventos()
        {
            // Restricción de caracteres: solo letras
            txtNombre.KeyPress += SoloLetras_KeyPress;
            txtApellido.KeyPress += SoloLetras_KeyPress;
            textPais.KeyPress += SoloLetras_KeyPress;
            textLocalidad.KeyPress += SoloLetras_KeyPress;

            // Restricción de caracteres: solo números
            txtDni.KeyPress += SoloNumeros_KeyPress;
            txtTelefono.KeyPress += SoloNumeros_KeyPress;
            textNroAltura.KeyPress += SoloNumeros_KeyPress;

            // Detección de cambios de texto para habilitar/deshabilitar botones
            txtDni.TextChanged += Control_Modificado;
            txtNombre.TextChanged += Control_Modificado;
            txtApellido.TextChanged += Control_Modificado;
            txtEmail.TextChanged += Control_Modificado;
            txtTelefono.TextChanged += Control_Modificado;
            textPais.TextChanged += Control_Modificado;
            textLocalidad.TextChanged += Control_Modificado;
            txtCalle.TextChanged += Control_Modificado;
            textNroAltura.TextChanged += Control_Modificado;

            // Eventos de botones
            btnGuardar.Click += btnGuardar_Click;
            btnLimpiar.Click += btnLimpiar_Click;
        }

        // =======================
        // FILTRADO DE ENTRADA
        // =======================
        private void SoloLetras_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permite letras, espacios y teclas de control (retroceso/borrar)
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permite solo dígitos numéricos y teclas de control
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // =======================
        // ESTADO DE BOTONES
        // =======================
        private void Control_Modificado(object sender, EventArgs e)
        {
            ActualizarEstadoBotones();
        }

        private void ActualizarEstadoBotones()
        {
            // Limpiar se habilita si hay al menos un dato escrito
            bool algunCampoLleno = !string.IsNullOrWhiteSpace(txtDni.Text) ||
                                  !string.IsNullOrWhiteSpace(txtNombre.Text) ||
                                  !string.IsNullOrWhiteSpace(txtApellido.Text) ||
                                  !string.IsNullOrWhiteSpace(txtEmail.Text) ||
                                  !string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                                  !string.IsNullOrWhiteSpace(textPais.Text) ||
                                  !string.IsNullOrWhiteSpace(textLocalidad.Text) ||
                                  !string.IsNullOrWhiteSpace(txtCalle.Text) ||
                                  !string.IsNullOrWhiteSpace(textNroAltura.Text);

            // Guardar se habilita cuando los datos obligatorios están completos
            bool obligatoriosLlenos = !string.IsNullOrWhiteSpace(txtDni.Text) &&
                                      !string.IsNullOrWhiteSpace(txtNombre.Text) &&
                                      !string.IsNullOrWhiteSpace(txtApellido.Text) &&
                                      !string.IsNullOrWhiteSpace(txtEmail.Text) &&
                                      !string.IsNullOrWhiteSpace(txtTelefono.Text);

            btnLimpiar.Enabled = algunCampoLleno;
            btnGuardar.Enabled = obligatoriosLlenos;

         
        }

        // =======================
        // ACCIONES DE BOTONES
        // =======================
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Validación de formato de Email
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(txtEmail.Text.Trim(), emailPattern))
            {
                MessageBox.Show("El correo electrónico no tiene un formato válido (ejemplo: usuario@correo.com).",
                                "Formato Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            // Validación de longitud de DNI (7 u 8 dígitos)
            if (txtDni.Text.Trim().Length < 7 || txtDni.Text.Trim().Length > 8)
            {
                MessageBox.Show("El DNI debe tener 7 u 8 dígitos.",
                                "Formato Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return;
            }

            // Lógica de inserción en base de datos aquí...
            MessageBox.Show("Cliente guardado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimpiarCampos();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            // Lógica de actualización en base de datos aquí...
            MessageBox.Show("Cliente modificado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimpiarCampos();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtDni.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            textPais.Clear();
            textLocalidad.Clear();
            txtCalle.Clear();
            textNroAltura.Clear();

            ActualizarEstadoBotones();
        }
    }
}