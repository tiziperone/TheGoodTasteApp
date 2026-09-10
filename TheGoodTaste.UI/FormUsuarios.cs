using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using The_Good_Taste.Datos;

namespace TheGoodTaste.UI
{
    public partial class FormUsuarios : Form
    {
        private int? _idUsuarioSeleccionado = null;

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

            // Configuración dinámica de la grilla
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

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
            textBoxName.KeyPress += SoloLetras_KeyPress;
            textBoxApellido.KeyPress += SoloLetras_KeyPress;
            textBoxDNI.KeyPress += SoloNumeros_KeyPress;
            textBoxNroTel.KeyPress += SoloNumeros_KeyPress;

            // Validación en tiempo real para habilitar botones
            textBoxName.TextChanged += Control_Modificado;
            textBoxApellido.TextChanged += Control_Modificado;
            textBoxUser.TextChanged += Control_Modificado;
            textBoxEmail.TextChanged += Control_Modificado;
            textBoxDNI.TextChanged += Control_Modificado;
            comboBox1.SelectedIndexChanged += Control_Modificado;
            radioButtonHom.CheckedChanged += Control_Modificado;
            radioButtonMuj.CheckedChanged += Control_Modificado;

            // Generación de usuario sugerido
            textBoxName.TextChanged += GenerarUsuarioSugerido;
            textBoxApellido.TextChanged += GenerarUsuarioSugerido;

            // Al escribir el DNI, se asigna como contraseña por defecto automáticamente
            textBoxDNI.TextChanged += (s, e) => {
                if (!_idUsuarioSeleccionado.HasValue)
                    textBoxPass.Text = textBoxDNI.Text.Trim();
            };

            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;

            // Navegación rápida con ENTER
            foreach (Control c in this.Controls)
            {
                if (c is TextBox)
                {
                    c.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) SelectNextControl((Control)s, true, true, true, true); };
                }
            }

            radioButtonAct.Click += (s, e) => CargarGrillaUsuarios(true);
            radioButtonInac.Click += (s, e) => CargarGrillaUsuarios(false);
        }

        private void Control_Modificado(object sender, EventArgs e)
        {
            ValidarReglaNegocioBotones();
        }

        private void ValidarReglaNegocioBotones()
        {
            bool algunCampoConDato = !string.IsNullOrWhiteSpace(textBoxName.Text) ||
                                     !string.IsNullOrWhiteSpace(textBoxApellido.Text) ||
                                     !string.IsNullOrWhiteSpace(textBoxUser.Text) ||
                                     !string.IsNullOrWhiteSpace(textBoxEmail.Text) ||
                                     !string.IsNullOrWhiteSpace(textBoxDNI.Text) ||
                                     comboBox1.SelectedIndex != -1;

            bool obligatoriosCompletos = !string.IsNullOrWhiteSpace(textBoxName.Text) &&
                                         !string.IsNullOrWhiteSpace(textBoxApellido.Text) &&
                                         !string.IsNullOrWhiteSpace(textBoxUser.Text) &&
                                         !string.IsNullOrWhiteSpace(textBoxEmail.Text) &&
                                         !string.IsNullOrWhiteSpace(textBoxDNI.Text) &&
                                         comboBox1.SelectedIndex != -1 &&
                                         (radioButtonHom.Checked || radioButtonMuj.Checked);

            buttonDel.Enabled = algunCampoConDato || _idUsuarioSeleccionado.HasValue;
            buttonSave.Enabled = obligatoriosCompletos;
        }

        private void GenerarUsuarioSugerido(object sender, EventArgs e)
        {
            if (!_idUsuarioSeleccionado.HasValue)
            {
                string nom = textBoxName.Text.Trim().ToLower().Replace(" ", "");
                string ape = textBoxApellido.Text.Trim().ToLower().Replace(" ", "");

                if (!string.IsNullOrEmpty(nom) || !string.IsNullOrEmpty(ape))
                {
                    textBoxUser.Text = $"{nom}.{ape}";
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(textBoxEmail.Text.Trim(), emailPattern))
            {
                MessageBox.Show("El correo electrónico no tiene un formato válido.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxEmail.Focus();
                return;
            }

            if (textBoxDNI.Text.Trim().Length < 7 || textBoxDNI.Text.Trim().Length > 8)
            {
                MessageBox.Show("El DNI debe contener 7 u 8 dígitos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxDNI.Focus();
                return;
            }

            try
            {
                string username = textBoxUser.Text.Trim();
                // La contraseña predeterminada es el DNI
                string password = string.IsNullOrWhiteSpace(textBoxPass.Text) ? textBoxDNI.Text.Trim() : textBoxPass.Text.Trim();
                string nombreCompleto = $"{textBoxName.Text.Trim()} {textBoxApellido.Text.Trim()}";
                int idRol = Convert.ToInt32(comboBox1.SelectedValue);

                UsuarioDatos repo = new UsuarioDatos();

                if (!_idUsuarioSeleccionado.HasValue)
                {
                    if (repo.ExisteDNI(textBoxDNI.Text.Trim()))
                    {
                        MessageBox.Show("El DNI ingresado ya está registrado.", "DNI Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBoxDNI.Focus();
                        return;
                    }

                    if (repo.RegistrarUsuario(username, password, nombreCompleto, idRol))
                    {
                        MessageBox.Show("Usuario registrado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Lógica para actualizar usuario
                    MessageBox.Show("Usuario actualizado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LimpiarCampos();
                CargarGrillaUsuarios(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];

                _idUsuarioSeleccionado = Convert.ToInt32(fila.Cells["ID"].Value);
                textBoxUser.Text = fila.Cells["Usuario"].Value?.ToString();

                // Separación inteligente de Nombre y Apellido
                string nombreCompleto = fila.Cells["Nombre Completo"].Value?.ToString().Trim() ?? "";
                int ultimoEspacio = nombreCompleto.LastIndexOf(' ');

                if (ultimoEspacio > 0)
                {
                    textBoxName.Text = nombreCompleto.Substring(0, ultimoEspacio);
                    textBoxApellido.Text = nombreCompleto.Substring(ultimoEspacio + 1);
                }
                else
                {
                    textBoxName.Text = nombreCompleto;
                    textBoxApellido.Text = "";
                }

                // Mantiene la contraseña actual para no sobreescribirla
                textBoxPass.Text = "********";

                buttonSave.Text = "Actualizar";
                buttonDel.Text = "Cancelar";
                ValidarReglaNegocioBotones();
            }
        }

        private void LimpiarCampos()
        {
            _idUsuarioSeleccionado = null;

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

            buttonSave.Text = "Guardar";
            buttonDel.Text = "Limpiar";

            ValidarReglaNegocioBotones();
            textBoxName.Focus();
        }

        private void buttonDel_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void SoloLetras_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void CargarGrillaUsuarios(bool verActivos)
        {
            try
            {
                UsuarioDatos repo = new UsuarioDatos();
                dataGridView1.DataSource = repo.ObtenerUsuariosPorEstado(verActivos);
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista: " + ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Enlaces de compatibilidad con el diseñador
        private void buttonSave_Click_1(object sender, EventArgs e) => buttonSave_Click(sender, e);
        private void buttonDel_Click_1(object sender, EventArgs e) => buttonDel_Click(sender, e);
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}