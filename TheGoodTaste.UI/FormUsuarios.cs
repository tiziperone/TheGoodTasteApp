using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using The_Good_Taste.Datos;

namespace TheGoodTaste.UI
{
    public partial class FormUsuarios : Form
    {
        private int? _idUsuarioSeleccionado = null;
        private DataRow _datosOriginales = null; // Para guardar los datos al hacer clic en modificar

        public FormUsuarios()
        {
            InitializeComponent();
        }

        private void FormUsuarios_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);
            CargarRoles();
            CargarLocalidades();
            ConfigurarEventos();
            ConfigurarAutocompletadoDireccion();
            LimpiarCampos();

            // Restringe el DateTimePicker para que no permita seleccionar fechas menores a 18 años atrás
            dateTimePickerFechNac.MaxDate = DateTime.Today.AddYears(-18);

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

        private void CargarLocalidades()
        {
            try
            {
                UsuarioDatos repo = new UsuarioDatos();
                comboBoxLocalidad.DataSource = repo.ObtenerLocalidades();
                comboBoxLocalidad.DisplayMember = "Descripcion";
                comboBoxLocalidad.ValueMember = "IdLocalidad";
                comboBoxLocalidad.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar localidades: " + ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ConfigurarAutocompletadoDireccion()
        {
            try
            {
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
            catch (Exception) { }
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
            comboBoxLocalidad.SelectedIndexChanged += Control_Modificado;

            radioButtonHom.CheckedChanged += Control_Modificado;
            radioButtonMuj.CheckedChanged += Control_Modificado;

            textBoxName.TextChanged += GenerarUsuarioSugerido;
            textBoxApellido.TextChanged += GenerarUsuarioSugerido;

            textBoxDNI.TextChanged += (s, e) => {
                if (!_idUsuarioSeleccionado.HasValue)
                    textBoxPass.Text = textBoxDNI.Text.Trim();
            };

            dataGridView1.CellContentClick += DataGridView1_CellContentClick;
            dataGridView1.CurrentCellDirtyStateChanged += DataGridView1_CurrentCellDirtyStateChanged;

            foreach (Control c in this.Controls)
            {
                if (c is TextBox)
                {
                    c.KeyDown += (s, e) =>
                    {
                        if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down)
                        {
                            e.SuppressKeyPress = true; 
                            SelectNextControl((Control)s, true, true, true, true);
                        }
                        else if (e.KeyCode == Keys.Up)
                        {
                            e.SuppressKeyPress = true;
                            SelectNextControl((Control)s, false, true, true, true);
                        }
                    };
                }
            }

            radioButtonAct.Click += (s, e) => CargarGrillaUsuarios(true);
            radioButtonInac.Click += (s, e) => CargarGrillaUsuarios(false);
        }

        // Este evento ahora maneja tanto el clic en el botón "Modificar" como en el checkbox de "Estado"
        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];

                // 1. Lógica para el botón "Modificar"
                if (dataGridView1.Columns[e.ColumnIndex].Name == "Modificar")
                {
                    if (fila.Cells["ID"].Value != DBNull.Value && fila.Cells["ID"].Value != null)
                    {
                        int dni = Convert.ToInt32(fila.Cells["ID"].Value);

                        UsuarioDatos repo = new UsuarioDatos();
                        _datosOriginales = repo.ObtenerUsuarioPorDNI(dni); // Traemos la fila entera desde SQL

                        if (_datosOriginales != null)
                        {
                            _idUsuarioSeleccionado = dni;

                            // Cargamos los campos en el formulario
                            textBoxDNI.Text = _datosOriginales["DNI"].ToString();
                            textBoxDNI.ReadOnly = true; // Bloqueamos el DNI para que no lo cambien por error

                            textBoxName.Text = _datosOriginales["Nombre"].ToString();
                            textBoxApellido.Text = _datosOriginales["Apellido"].ToString();
                            textBoxUser.Text = _datosOriginales["Username"].ToString();
                            textBoxPass.Text = "********";
                            textBoxEmail.Text = _datosOriginales["Email"].ToString();
                            textBoxDir.Text = _datosOriginales["Direccion"] != DBNull.Value ? _datosOriginales["Direccion"].ToString() : "";
                            textBoxNroTel.Text = _datosOriginales["Telefono"] != DBNull.Value ? _datosOriginales["Telefono"].ToString() : "";

                            if (_datosOriginales["FechaNacimiento"] != DBNull.Value)
                                dateTimePickerFechNac.Value = Convert.ToDateTime(_datosOriginales["FechaNacimiento"]);

                            comboBox1.SelectedValue = Convert.ToInt32(_datosOriginales["IdRol"]);

                            if (_datosOriginales["IdLocalidad"] != DBNull.Value)
                                comboBoxLocalidad.SelectedValue = Convert.ToInt32(_datosOriginales["IdLocalidad"]);

                            string sexo = _datosOriginales["Sexo"]?.ToString() ?? "";
                            radioButtonHom.Checked = (sexo == "M");
                            radioButtonMuj.Checked = (sexo == "F");

                            buttonSave.Text = "Modificar";
                            buttonDel.Text = "Cancelar";
                            ValidarReglaNegocioBotones();
                        }
                    }
                    return; // Salimos para no evaluar el checkbox
                }

                // 2. Lógica para el CheckBox "Estado" (Dar de baja / reactivar)
                if (dataGridView1.Columns[e.ColumnIndex].Name == "Estado")
                {
                    if (fila.Cells["ID"].Value != DBNull.Value && fila.Cells["ID"].Value != null)
                    {
                        int dni = Convert.ToInt32(fila.Cells["ID"].Value);
                        bool nuevoEstado = Convert.ToBoolean(fila.Cells["Estado"].Value);
                        string usuarioNombre = fila.Cells["Usuario"].Value?.ToString() ?? "este usuario";

                        string accion = nuevoEstado ? "reactivar" : "dar de baja";

                        DialogResult result = MessageBox.Show(
                            $"¿Está seguro de que desea {accion} al usuario '{usuarioNombre}'?",
                            "Confirmación de Estado",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button2
                        );

                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                UsuarioDatos repo = new UsuarioDatos();
                                if (repo.CambiarEstadoUsuario(dni, nuevoEstado))
                                {
                                    MessageBox.Show($"Usuario {(nuevoEstado ? "reactivado" : "dado de baja")} correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    CargarGrillaUsuarios(radioButtonAct.Checked);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error al cambiar el estado: " + ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                fila.Cells["Estado"].Value = !nuevoEstado;
                            }
                        }
                        else
                        {
                            dataGridView1.CancelEdit();
                            CargarGrillaUsuarios(radioButtonAct.Checked);
                        }
                    }
                }
            }
        }

        private void DataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty && dataGridView1.CurrentCell is DataGridViewCheckBoxCell)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
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
                                         comboBoxLocalidad.SelectedIndex != -1 &&
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

            // 3. VALIDACIÓN DE EDAD MÍNIMA (AQUÍ AGREGAS LA NUEVA)
            DateTime fechaNacimientoSeleccionada = dateTimePickerFechNac.Value.Date;
            DateTime fechaHoy = DateTime.Today;
            int edad = fechaHoy.Year - fechaNacimientoSeleccionada.Year;

            if (fechaNacimientoSeleccionada.Date > fechaHoy.AddYears(-edad))
            {
                edad--;
            }

            if (edad < 18)
            {
                MessageBox.Show("El usuario debe ser mayor de edad (mínimo 18 años).", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dateTimePickerFechNac.Focus();
                return;
            }

            try
            {
                int dni = Convert.ToInt32(textBoxDNI.Text.Trim());
                string username = textBoxUser.Text.Trim();
                string password = string.IsNullOrWhiteSpace(textBoxPass.Text) ? textBoxDNI.Text.Trim() : textBoxPass.Text.Trim();
                string nombre = textBoxName.Text.Trim();
                string apellido = textBoxApellido.Text.Trim();
                int idRol = Convert.ToInt32(comboBox1.SelectedValue);
                string direccion = textBoxDir.Text.Trim();
                int idLocalidad = comboBoxLocalidad.SelectedValue != null ? Convert.ToInt32(comboBoxLocalidad.SelectedValue) : 1;
                DateTime fechaNacimiento = dateTimePickerFechNac.Value;
                string telefono = textBoxNroTel.Text.Trim();
                string email = textBoxEmail.Text.Trim();
                string sexo = radioButtonHom.Checked ? "M" : (radioButtonMuj.Checked ? "F" : "Otro");

                UsuarioDatos repo = new UsuarioDatos();

                if (!_idUsuarioSeleccionado.HasValue) // ALTA
                {
                    if (repo.ExisteDNI(dni))
                    {
                        MessageBox.Show("El DNI ingresado ya está registrado.", "DNI Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBoxDNI.Focus();
                        return;
                    }

                    if (repo.RegistrarUsuario(dni, username, password, idRol, nombre, apellido, direccion, idLocalidad, fechaNacimiento, telefono, email, sexo))
                    {
                        MessageBox.Show("Usuario registrado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                        CargarGrillaUsuarios(true);
                    }
                }
                else // MODIFICACIÓN
                {
                    List<string> cambios = new List<string>();

                    if (_datosOriginales["Nombre"].ToString() != nombre) cambios.Add($"• Nombre: '{_datosOriginales["Nombre"]}' -> '{nombre}'");
                    if (_datosOriginales["Apellido"].ToString() != apellido) cambios.Add($"• Apellido: '{_datosOriginales["Apellido"]}' -> '{apellido}'");
                    if (_datosOriginales["Username"].ToString() != username) cambios.Add($"• Usuario: '{_datosOriginales["Username"]}' -> '{username}'");
                    if (password != "********") cambios.Add("• Contraseña: Se ingresó una nueva contraseña");
                    if (Convert.ToInt32(_datosOriginales["IdRol"]) != idRol) cambios.Add($"• Rol modificado");
                    if (Convert.ToInt32(_datosOriginales["IdLocalidad"] == DBNull.Value ? 0 : _datosOriginales["IdLocalidad"]) != idLocalidad) cambios.Add($"• Localidad modificada");
                    if (_datosOriginales["Email"].ToString() != email) cambios.Add($"• Email: '{_datosOriginales["Email"]}' -> '{email}'");

                    string dirOriginal = _datosOriginales["Direccion"] != DBNull.Value ? _datosOriginales["Direccion"].ToString() : "";
                    if (dirOriginal != direccion) cambios.Add($"• Dirección: '{dirOriginal}' -> '{direccion}'");

                    string telOriginal = _datosOriginales["Telefono"] != DBNull.Value ? _datosOriginales["Telefono"].ToString() : "";
                    if (telOriginal != telefono) cambios.Add($"• Teléfono: '{telOriginal}' -> '{telefono}'");

                    string sexoOriginal = _datosOriginales["Sexo"]?.ToString() ?? "";
                    if (sexoOriginal != sexo) cambios.Add($"• Sexo: '{sexoOriginal}' -> '{sexo}'");

                    if (cambios.Count == 0)
                    {
                        MessageBox.Show("No se detectaron modificaciones para guardar.", "Sin cambios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string mensaje = "¿Está seguro de aplicar los siguientes cambios?\n\n" + string.Join("\n", cambios);
                    DialogResult confirmacion = MessageBox.Show(mensaje, "Confirmar Modificación", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                    if (confirmacion == DialogResult.Yes)
                    {
                        if (repo.ActualizarUsuario(dni, username, password, idRol, nombre, apellido, direccion, idLocalidad, fechaNacimiento, telefono, email, sexo))
                        {
                            MessageBox.Show("Usuario actualizado con éxito en la base de datos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LimpiarCampos();
                            CargarGrillaUsuarios(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar/modificar: " + ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            _idUsuarioSeleccionado = null;
            _datosOriginales = null;

            textBoxName.Clear();
            textBoxApellido.Clear();
            textBoxUser.Clear();
            textBoxPass.Clear();
            textBoxEmail.Clear();
            textBoxDNI.Clear();
            textBoxDNI.ReadOnly = false;
            textBoxDir.Clear();
            textBoxNroTel.Clear();
            textBoxPass.UseSystemPasswordChar = true;
            

            comboBox1.SelectedIndex = -1;
            if (comboBoxLocalidad != null) comboBoxLocalidad.SelectedIndex = -1;

            radioButtonHom.Checked = false;
            radioButtonMuj.Checked = false;
            dateTimePickerFechNac.Value = DateTime.Today.AddYears(-18);

            buttonSave.Text = "Guardar";
            buttonDel.Text = "Limpiar";

            ValidarReglaNegocioBotones();
            textBoxName.Focus();
        }

        private void buttonDel_Click(object sender, EventArgs e) => LimpiarCampos();
        private void SoloLetras_KeyPress(object sender, KeyPressEventArgs e) { if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar)) e.Handled = true; }
        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e) { if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true; }

        private void CargarGrillaUsuarios(bool verActivos)
        {
            try
            {
                UsuarioDatos repo = new UsuarioDatos();
                dataGridView1.DataSource = repo.ObtenerUsuariosPorEstado(verActivos);

                // Agregamos la columna de botón si no existe
                if (!dataGridView1.Columns.Contains("Modificar"))
                {
                    DataGridViewButtonColumn btnModificar = new DataGridViewButtonColumn();
                    btnModificar.Name = "Modificar";
                    btnModificar.HeaderText = "Acción";
                    btnModificar.Text = "Modificar";
                    btnModificar.UseColumnTextForButtonValue = true;
                    // La agregamos al principio de todo
                    dataGridView1.Columns.Insert(0, btnModificar);
                }

                // Bloquear la edición de todas las columnas de texto
                foreach (DataGridViewColumn columna in dataGridView1.Columns)
                {
                    // Dejamos libre la columna "Estado" y el botón "Modificar"
                    if (columna.Name != "Estado" && columna.Name != "Modificar")
                    {
                        columna.ReadOnly = true;
                    }
                }

                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
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

      

        private void label8_Click(object sender, EventArgs e)
        {
        }
    }
}