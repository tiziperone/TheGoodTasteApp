using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using The_Good_Taste.Datos;

namespace TheGoodTaste.UI
{
    public partial class FormUsuarios : Form
    {
        public FormUsuarios()
        {
            InitializeComponent();
        }

        private void FormUsuarios_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);
            CargarRoles();
            ConfigurarEventos();
            LimpiarCampos();

            // Carga inicial de la grilla con usuarios activos
            CargarGrillaUsuarios(true);
        }

        private void CargarRoles()
        {
            var roles = new Dictionary<int, string>
            {
                { 1, "Admin" },
                { 2, "Gerente" },
                { 3, "Vendedor" }
            };

            comboBox1.DataSource = new BindingSource(roles, null);
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.SelectedIndex = -1;
        }

        private void ConfigurarEventos()
        {
            // Restricción de caracteres
            textBoxName.KeyPress += SoloLetras_KeyPress;
            textBoxApellido.KeyPress += SoloLetras_KeyPress;
            textBoxDNI.KeyPress += SoloNumeros_KeyPress;
            textBoxNroTel.KeyPress += SoloNumeros_KeyPress;

            // Detección de cambios de texto
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

            // Acciones principales
            buttonSave.Click += buttonSave_Click;
            buttonDel.Click += buttonDel_Click;

            // Eventos de los botones de filtro de la grilla
            // Si tus botones se llaman distinto en el Diseñador, cambia buttonActivos/buttonInactivos
            radioButtonAct.Click += (s, e) => CargarGrillaUsuarios(true);
            radioButtonInac.Click += (s, e) => CargarGrillaUsuarios(false);
        }

        private void CargarGrillaUsuarios(bool verActivos)
        {
            try
            {
                UsuarioDatos repo = new UsuarioDatos();
                // Si tu DataGridView tiene otro nombre (ej: dgvUsuarios), reemplázalo aquí
                dataGridView1.DataSource = repo.ObtenerUsuariosPorEstado(verActivos);
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista de usuarios: " + ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SoloLetras_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Control_Modificado(object sender, EventArgs e)
        {
            ActualizarEstadoBotones();
        }

        private void ActualizarEstadoBotones()
        {
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

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(textBoxEmail.Text.Trim(), emailPattern))
            {
                MessageBox.Show("El correo electrónico no tiene un formato válido (ejemplo: usuario@correo.com).",
                                "Formato Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxEmail.Focus();
                return;
            }

            if (textBoxDNI.Text.Trim().Length < 7 || textBoxDNI.Text.Trim().Length > 8)
            {
                MessageBox.Show("El DNI debe contener 7 u 8 dígitos.",
                                "Formato Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxDNI.Focus();
                return;
            }

            try
            {
                string username = textBoxUser.Text.Trim();
                string password = textBoxPass.Text.Trim();
                string nombreCompleto = $"{textBoxName.Text.Trim()} {textBoxApellido.Text.Trim()}";
                int idRol = Convert.ToInt32(comboBox1.SelectedValue);

                UsuarioDatos repo = new UsuarioDatos();
                bool registrado = repo.RegistrarUsuario(username, password, nombreCompleto, idRol);

                if (registrado)
                {
                    MessageBox.Show("Usuario registrado correctamente en la base de datos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                    CargarGrillaUsuarios(true); // Refresca automáticamente los activos
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Ya existe un usuario con ese nombre de usuario. Elija otro.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBoxUser.Focus();
                }
                else
                {
                    MessageBox.Show("Error al guardar en la base de datos: " + ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
    
    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Método requerido por FormUsuarios.Designer.cs
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    } 
}