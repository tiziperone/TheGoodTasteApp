using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization; // Requerido para el formateo de texto
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
            ConfigurarAutocompletadoDireccion();
            LimpiarCampos();

            // Deshabilitar la creación de nuevas filas manuales en el DataGridView
            dataGridView1.AllowUserToAddRows = false;

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

        private void ConfigurarAutocompletadoDireccion()
        {
            try
            {
                // Lista de calles para sugerir mientras se escribe en textBoxDir
                AutoCompleteStringCollection callesSugeridas = new AutoCompleteStringCollection();

                string[] listaCalles = new string[]
                {
                    "Av. 3 de Abril", "Av. Pedro Ferré", "Av. Gobernador Ruiz", "Av. Armenia",
                    "Av. Independencia", "Av. Maipú", "Av. Centenario", "Junín",
                    "Pellegrini", "9 de Julio", "San Martín", "Córdoba", "Mendoza",
                    "Salta", "Tucumán", "Buenos Aires", "Belgrano", "Bolívar", "Sarmiento"
                };

                callesSugeridas.AddRange(listaCalles);

                textBoxDir.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                textBoxDir.AutoCompleteSource = AutoCompleteSource.CustomSource;
                textBoxDir.AutoCompleteCustomSource = callesSugeridas;
            }
            catch (Exception)
            {
                // Control silencioso si falla la carga
            }
        }

        // Método para formatear la dirección (Primera letra Mayúscula, resto minúscula)
        private string FormatearDireccion(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(texto.Trim().ToLower());
        }

        private void ConfigurarEventos()
        {
            textBoxName.KeyPress += SoloLetras_KeyPress;
            textBoxApellido.KeyPress += SoloLetras_KeyPress;
            textBoxDNI.KeyPress += SoloNumeros_KeyPress;
            textBoxNroTel.KeyPress += SoloNumeros_KeyPress;

            textBoxName.TextChanged += Control_Modificado;
            textBoxApellido.TextChanged += Control_Modificado;
            textBoxUser.TextChanged += Control_Modificado;
            textBoxEmail.TextChanged += Control_Modificado;
            textBoxDNI.TextChanged += Control_Modificado;
            comboBox1.SelectedIndexChanged += Control_Modificado;
            radioButtonHom.CheckedChanged += Control_Modificado;
            radioButtonMuj.CheckedChanged += Control_Modificado;

            textBoxName.TextChanged += GenerarUsuarioSugerido;
            textBoxApellido.TextChanged += GenerarUsuarioSugerido;

            textBoxDNI.TextChanged += (s, e) => {
                if (!_idUsuarioSeleccionado.HasValue)
                    textBoxPass.Text = textBoxDNI.Text.Trim();
            };

            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;

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
                string password = string.IsNullOrWhiteSpace(textBoxPass.Text) ? textBoxDNI.Text.Trim() : textBoxPass.Text.Trim();
                string nombreCompleto = $"{textBoxName.Text.Trim()} {textBoxApellido.Text.Trim()}";
                int idRol = Convert.ToInt32(comboBox1.SelectedValue);

                // Aplicación del formato Title Case a la dirección ingresada
                string direccionFormateada = FormatearDireccion(textBoxDir.Text);

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
            // 1. Validar que la fila sea válida y que no sea la fila de agregación nueva (isNewRow)
            if (e.RowIndex >= 0 && !dataGridView1.Rows[e.RowIndex].IsNewRow)
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];

                // 2. Validar que la celda ID contenga un valor válido antes de convertir
                if (fila.Cells["ID"].Value != DBNull.Value && fila.Cells["ID"].Value != null)
                {
                    _idUsuarioSeleccionado = Convert.ToInt32(fila.Cells["ID"].Value);
                    textBoxUser.Text = fila.Cells["Usuario"].Value?.ToString();

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

                    textBoxPass.Text = "********";

                    buttonSave.Text = "Actualizar";
                    buttonDel.Text = "Cancelar";
                    ValidarReglaNegocioBotones();
                }
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
            textBoxPass.UseSystemPasswordChar = true;
            chkVerPassUsuario.Checked = false;
            chkVerPassUsuario.Text = "👁️";

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

        private void buttonSave_Click_1(object sender, EventArgs e) => buttonSave_Click(sender, e);
        private void buttonDel_Click_1(object sender, EventArgs e) => buttonDel_Click(sender, e);
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void btnMostrarPassword_Click(object sender, EventArgs e)
        {
            textBoxPass.UseSystemPasswordChar = !textBoxPass.UseSystemPasswordChar;
            chkVerPassUsuario.Text = textBoxPass.UseSystemPasswordChar ? "👁️" : "🙈";
        }

        private void chkVerPassUsuario_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPass.UseSystemPasswordChar = !chkVerPassUsuario.Checked;
            chkVerPassUsuario.Text = chkVerPassUsuario.Checked ? "🙈" : "👁️";
        }

        private void btnMostrarPassword_CheckedChanged(object sender, EventArgs e) => chkVerPassUsuario_CheckedChanged(sender, e);
    }
}