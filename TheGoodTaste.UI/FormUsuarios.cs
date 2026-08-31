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
    public partial class FormUsuarios : Form
    {
        public FormUsuarios()
        {
            InitializeComponent();
            ConfigurarEventos();
        }

        private void FormUsuarios_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);
            LimpiarCampos();
        }

        private void ConfigurarEventos()
        {
            // Restricción de caracteres: solo letras
            textBoxName.KeyPress += SoloLetras_KeyPress;
            textBoxApellido.KeyPress += SoloLetras_KeyPress;

            // Restricción de caracteres: solo números
            textBoxDNI.KeyPress += SoloNumeros_KeyPress;
            textBoxNroTel.KeyPress += SoloNumeros_KeyPress;

            // Detección de cambios para actualizar el estado de los botones
            textBoxName.TextChanged += Control_Modificado;
            textBoxApellido.TextChanged += Control_Modificado;
            textBoxUser.TextChanged += Control_Modificado;
            textBoxPass.TextChanged += Control_Modificado;
            textBoxEmail.TextChanged += Control_Modificado;
            textBoxDNI.TextChanged += Control_Modificado;
            textBoxDir.TextChanged += Control_Modificado;
            textBoxNroTel.TextChanged += Control_Modificado;

            comboBox1.SelectedIndexChanged += Control_Modificado;
            radioButtonHom.CheckedChanged += Control_Modificado;
            radioButtonMuj.CheckedChanged += Control_Modificado;

            // Eventos de clic
            buttonSave.Click += buttonSave_Click;
            buttonDel.Click += buttonDel_Click;
        }

        // =======================
        // FILTRADO DE ENTRADA
        // =======================
        private void SoloLetras_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permite solo letras, espacio y teclas de control (retroceso/borrar)
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
            // Habilita Cancelar (buttonDel) si hay al menos un dato cargado
            bool algunCampoConDato = !string.IsNullOrWhiteSpace(textBoxName.Text) ||
                                     !string.IsNullOrWhiteSpace(textBoxApellido.Text) ||
                                     !string.IsNullOrWhiteSpace(textBoxUser.Text) ||
                                     !string.IsNullOrWhiteSpace(textBoxPass.Text) ||
                                     !string.IsNullOrWhiteSpace(textBoxEmail.Text) ||
                                     !string.IsNullOrWhiteSpace(textBoxDNI.Text) ||
                                     !string.IsNullOrWhiteSpace(textBoxDir.Text) ||
                                     !string.IsNullOrWhiteSpace(textBoxNroTel.Text) ||
                                     comboBox1.SelectedIndex != -1 ||
                                     radioButtonHom.Checked ||
                                     radioButtonMuj.Checked;

            // Habilita Guardar (buttonSave) si todos los obligatorios están llenos
            bool obligatoriosCompletos = !string.IsNullOrWhiteSpace(textBoxName.Text) &&
                                         !string.IsNullOrWhiteSpace(textBoxApellido.Text) &&
                                         !string.IsNullOrWhiteSpace(textBoxUser.Text) &&
                                         !string.IsNullOrWhiteSpace(textBoxPass.Text) &&
                                         !string.IsNullOrWhiteSpace(textBoxEmail.Text) &&
                                         !string.IsNullOrWhiteSpace(textBoxDNI.Text) &&
                                         comboBox1.SelectedIndex != -1 &&
                                         (radioButtonHom.Checked || radioButtonMuj.Checked);

            buttonDel.Enabled = algunCampoConDato;
            buttonSave.Enabled = obligatoriosCompletos;
        }

        // =======================
        // ACCIONES DE BOTONES
        // =======================
        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Validación de formato de email
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(textBoxEmail.Text.Trim(), emailPattern))
            {
                MessageBox.Show("El correo electrónico no tiene un formato válido (ejemplo: usuario@correo.com).",
                                "Formato Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxEmail.Focus();
                return;
            }

            // Validación de longitud de DNI (7 u 8 dígitos)
            if (textBoxDNI.Text.Trim().Length < 7 || textBoxDNI.Text.Trim().Length > 8)
            {
                MessageBox.Show("El DNI debe contener 7 u 8 dígitos.",
                                "Formato Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxDNI.Focus();
                return;
            }

            // Lógica de inserción/actualización en base de datos aquí...
            MessageBox.Show("Usuario registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LimpiarCampos();
        }

        private void buttonDel_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            textBoxName.Clear();
            textBoxApellido.Clear();
            textBoxUser.Clear();
            textBoxPass.Clear();
            textBoxEmail.Clear();
            textBoxDNI.Clear();
            textBoxDir.Clear();
            textBoxNroTel.Clear();

            comboBox1.SelectedIndex = -1;
            radioButtonHom.Checked = false;
            radioButtonMuj.Checked = false;
            dateTimePickerFechNac.Value = DateTime.Today;

            ActualizarEstadoBotones();
        }
    }
}