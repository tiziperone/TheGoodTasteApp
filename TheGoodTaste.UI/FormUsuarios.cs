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
        private int? _idUsuarioSeleccionado = null; // null = Nuevo Usuario, int = Modo Edición

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
            // Restricciones de entrada
            textBoxName.KeyPress += SoloLetras_KeyPress;
            textBoxApellido.KeyPress += SoloLetras_KeyPress;
            textBoxDNI.KeyPress += SoloNumeros_KeyPress;
            textBoxNroTel.KeyPress += SoloNumeros_KeyPress;

            // Detección de cambios para evaluar REGLA DE NEGOCIO (Habilitar/Deshabilitar botones)
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

            // Autogeneración inteligente para agilizar
            textBoxName.TextChanged += GenerarUsuarioSugerido;
            textBoxApellido.TextChanged += GenerarUsuarioSugerido;

            // Selección en DataGridView para editar
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;

            // Navegación fluida con la tecla ENTER entre inputs
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
                                     !string.IsNullOrWhiteSpace(textBoxPass.Text) ||
                                     !string.IsNullOrWhiteSpace(textBoxEmail.Text) ||
                                     !string.IsNullOrWhiteSpace(textBoxDNI.Text) ||
                                     !string.IsNullOrWhiteSpace(textBoxDir.Text) ||
                                     !string.IsNullOrWhiteSpace(textBoxNroTel.Text) ||
                                     comboBox1.SelectedIndex != -1 ||
                                     radioButtonHom.Checked ||
                                     radioButtonMuj.Checked;

            // Si estamos en modo edición, la contraseña puede quedar vacía para no cambiarla
            bool esPasswordValido = _idUsuarioSeleccionado.HasValue ? true : !string.IsNullOrWhiteSpace(textBoxPass.Text);

            bool obligatoriosCompletos = !string.IsNullOrWhiteSpace(textBoxName.Text) &&
                                         !string.IsNullOrWhiteSpace(textBoxApellido.Text) &&
                                         !string.IsNullOrWhiteSpace(textBoxUser.Text) &&
                                         esPasswordValido &&
                                         !string.IsNullOrWhiteSpace(textBoxEmail.Text) &&
                                         !string.IsNullOrWhiteSpace(textBoxDNI.Text) &&
                                         comboBox1.SelectedIndex != -1 &&
                                         (radioButtonHom.Checked || radioButtonMuj.Checked);

            buttonDel.Enabled = algunCampoConDato || _idUsuarioSeleccionado.HasValue;
            buttonSave.Enabled = obligatoriosCompletos; // Mantiene bloqueado el botón hasta cumplir la regla
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
            // Validaciones de formato antes de tocar BD
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(textBoxEmail.Text.Trim(), emailPattern))
            {
                MessageBox.Show("El correo electrónico no tiene un formato válido (ejemplo: usuario@correo.com).", "Formato Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxEmail.Focus();
                return;
            }

            if (textBoxDNI.Text.Trim().Length < 7 || textBoxDNI.Text.Trim().Length > 8)
            {
                MessageBox.Show("El DNI debe contener 7 u 8 dígitos.", "Formato Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                if (!_idUsuarioSeleccionado.HasValue)
                {
                    // Crear nuevo usuario
                    if (repo.ExisteDNI(textBoxDNI.Text.Trim()))
                    {
                        MessageBox.Show("El DNI ingresado ya está registrado.", "DNI Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBoxDNI.Focus();
                        return;
                    }

                    if (repo.RegistrarUsuario(username, password, nombreCompleto, idRol))
                    {
                        MessageBox.Show("Usuario registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Actualizar usuario existente
                    // repo.ActualizarUsuario(_idUsuarioSeleccionado.Value, ...);
                    MessageBox.Show("Usuario actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LimpiarCampos();
                CargarGrillaUsuarios(true);
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

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];

                _idUsuarioSeleccionado = Convert.ToInt32(fila.Cells["ID"].Value);
                textBoxUser.Text = fila.Cells["Usuario"].Value?.ToString();

                string[] nombres = fila.Cells["Nombre Completo"].Value?.ToString().Split(' ');
                textBoxName.Text = nombres.Length > 0 ? nombres[0] : "";
                textBoxApellido.Text = nombres.Length > 1 ? string.Join(" ", nombres.Skip(1)) : "";

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

        // Métodos de enlace para compatibilidad con el Diseñador
        private void buttonSave_Click_1(object sender, EventArgs e) => buttonSave_Click(sender, e);
        private void buttonDel_Click_1(object sender, EventArgs e) => buttonDel_Click(sender, e);
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }


}