using System;
using System.Data;
using System.Windows.Forms;
using TheGoodTaste.Negocio;

namespace TheGoodTaste.UI
{
    public partial class FormClientes : Form
    {
        private readonly ClienteNegocio _negocio = new ClienteNegocio();
        private string _dniSeleccionado = "";

        // Variables para recordar los datos originales y comparar qué cambió
        private string _dniOrig, _nombreOrig, _apellidoOrig, _emailOrig, _telefonoOrig, _paisOrig, _localidadOrig, _provinciaOrig, _calleOrig, _alturaOrig;
        private DateTime _fechaNacOrig;

        public FormClientes()
        {
            InitializeComponent();
        }

        private void FormClientes_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);
            ConfigurarEventos();
            ConfigurarGrilla();
            LimpiarCampos();
            CargarGrilla(true);
        }

        private void ConfigurarEventos()
        {
            txtNombre.KeyPress += SoloLetras_KeyPress;
            txtApellido.KeyPress += SoloLetras_KeyPress;
            textPais.KeyPress += SoloLetras_KeyPress;
            textLocalidad.KeyPress += SoloLetras_KeyPress;
            txtProvincia.KeyPress += SoloLetras_KeyPress;

            txtDni.KeyPress += SoloNumeros_KeyPress;
            txtTelefono.KeyPress += SoloNumeros_KeyPress;
            textNroAltura.KeyPress += SoloNumeros_KeyPress;

            txtDni.TextChanged += Control_Modificado;
            txtNombre.TextChanged += Control_Modificado;
            txtApellido.TextChanged += Control_Modificado;
            txtEmail.TextChanged += Control_Modificado;
            txtTelefono.TextChanged += Control_Modificado;
            textPais.TextChanged += Control_Modificado;
            textLocalidad.TextChanged += Control_Modificado;
            txtProvincia.TextChanged += Control_Modificado;
            txtCalle.TextChanged += Control_Modificado;
            textNroAltura.TextChanged += Control_Modificado;

            btnActivo.Click += btnActivo_Click;
            btnInactivo.Click += btnInactivo_Click;

            dgvClientes.CellContentClick += dgvClientes_CellContentClick;
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

        private void Control_Modificado(object sender, EventArgs e) => ActualizarEstadoBotones();

        private void ActualizarEstadoBotones()
        {
            btnLimpiar.Enabled = !string.IsNullOrWhiteSpace(txtDni.Text) ||
                                 !string.IsNullOrWhiteSpace(txtNombre.Text) ||
                                 !string.IsNullOrWhiteSpace(txtApellido.Text) ||
                                 !string.IsNullOrWhiteSpace(txtEmail.Text) ||
                                 !string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                                 !string.IsNullOrWhiteSpace(textPais.Text) ||
                                 !string.IsNullOrWhiteSpace(textLocalidad.Text) ||
                                 !string.IsNullOrWhiteSpace(txtProvincia.Text) ||
                                 !string.IsNullOrWhiteSpace(txtCalle.Text) ||
                                 !string.IsNullOrWhiteSpace(textNroAltura.Text);

            btnGuardar.Enabled = !string.IsNullOrWhiteSpace(txtDni.Text) &&
                                 !string.IsNullOrWhiteSpace(txtNombre.Text) &&
                                 !string.IsNullOrWhiteSpace(txtApellido.Text) &&
                                 !string.IsNullOrWhiteSpace(txtEmail.Text) &&
                                 !string.IsNullOrWhiteSpace(txtTelefono.Text);
        }

        // --- CONFIGURACIÓN Y MÉTODOS DE LA GRILLA ---

        private void ConfigurarGrilla()
        {
            if (!dgvClientes.Columns.Contains("btnModificarGrilla"))
            {
                DataGridViewButtonColumn btnColumn = new DataGridViewButtonColumn();
                btnColumn.Name = "btnModificarGrilla";
                btnColumn.HeaderText = "Acción";
                btnColumn.Text = "Modificar";
                btnColumn.UseColumnTextForButtonValue = true;
                dgvClientes.Columns.Insert(0, btnColumn);
            }
        }

        private void CargarGrilla(bool estadoActivo)
        {
            try
            {
                dgvClientes.DataSource = _negocio.ObtenerClientes(estadoActivo);

                if (dgvClientes.Columns.Count > 0)
                {
                    dgvClientes.Columns["fechaNaciminetoCliente"].Visible = false;
                    dgvClientes.Columns["telefonoCliente"].Visible = false;
                    dgvClientes.Columns["paisCliente"].Visible = false;
                    dgvClientes.Columns["provinciaCliente"].Visible = false;

                    dgvClientes.Columns["dniCliente"].HeaderText = "DNI";
                    dgvClientes.Columns["nombreCliente"].HeaderText = "Nombre";
                    dgvClientes.Columns["apellidoCliente"].HeaderText = "Apellido";
                    dgvClientes.Columns["correoCliente"].HeaderText = "Correo";
                    dgvClientes.Columns["localidadCliente"].HeaderText = "Localidad";
                    dgvClientes.Columns["calleCliente"].HeaderText = "Calle";
                    dgvClientes.Columns["altura"].HeaderText = "Nro";

                    dgvClientes.Columns["btnModificarGrilla"].Visible = estadoActivo;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los clientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActivo_Click(object sender, EventArgs e)
        {
            CargarGrilla(true);
        }

        private void btnInactivo_Click(object sender, EventArgs e)
        {
            CargarGrilla(false);
            LimpiarCampos();
        }

        private void dgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvClientes.Columns[e.ColumnIndex].Name == "btnModificarGrilla")
            {
                DataGridViewRow fila = dgvClientes.Rows[e.RowIndex];

                // Guardamos los datos originales en las variables para compararlos luego
                _dniOrig = fila.Cells["dniCliente"].Value.ToString();
                _nombreOrig = fila.Cells["nombreCliente"].Value.ToString();
                _apellidoOrig = fila.Cells["apellidoCliente"].Value.ToString();
                if (DateTime.TryParse(fila.Cells["fechaNaciminetoCliente"].Value.ToString(), out DateTime fechaNac))
                    _fechaNacOrig = fechaNac;
                _emailOrig = fila.Cells["correoCliente"].Value.ToString();
                _telefonoOrig = fila.Cells["telefonoCliente"].Value.ToString();
                _paisOrig = fila.Cells["paisCliente"].Value.ToString();
                _localidadOrig = fila.Cells["localidadCliente"].Value.ToString();
                _provinciaOrig = fila.Cells["provinciaCliente"].Value.ToString();
                _calleOrig = fila.Cells["calleCliente"].Value.ToString();
                _alturaOrig = fila.Cells["altura"].Value.ToString();

                // Pasamos los datos a los TextBox
                _dniSeleccionado = _dniOrig;
                txtDni.Text = _dniOrig;
                txtNombre.Text = _nombreOrig;
                txtApellido.Text = _apellidoOrig;
                dtpFechaNacimiento.Value = _fechaNacOrig;
                txtEmail.Text = _emailOrig;
                txtTelefono.Text = _telefonoOrig;
                textPais.Text = _paisOrig;
                textLocalidad.Text = _localidadOrig;
                txtProvincia.Text = _provinciaOrig;
                txtCalle.Text = _calleOrig;
                textNroAltura.Text = _alturaOrig;

                btnGuardar.Text = "Guardar Modificación";
            }
        }

        private void dgvClientes_CellDoubleClick(object sender, DataGridViewCellEventArgs e) { }

        // --- GUARDAR Y LIMPIAR ---

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_dniSeleccionado))
                {
                    _negocio.GuardarCliente(txtDni.Text.Trim(), txtNombre.Text.Trim(), txtApellido.Text.Trim(), dtpFechaNacimiento.Value, txtEmail.Text.Trim(), txtTelefono.Text.Trim(), textPais.Text.Trim(), textLocalidad.Text.Trim(), txtProvincia.Text.Trim(), txtCalle.Text.Trim(), textNroAltura.Text.Trim());
                    MessageBox.Show("Cliente guardado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Comparamos los valores originales con los nuevos y armamos el mensaje
                    string cambios = "";

                    if (_dniOrig != txtDni.Text.Trim())
                        cambios += $"- DNI: {_dniOrig} ➔ {txtDni.Text.Trim()}\n";

                    if (_nombreOrig != txtNombre.Text.Trim() || _apellidoOrig != txtApellido.Text.Trim())
                        cambios += $"- Nombre: {_nombreOrig} {_apellidoOrig} ➔ {txtNombre.Text.Trim()} {txtApellido.Text.Trim()}\n";

                    if (_fechaNacOrig.Date != dtpFechaNacimiento.Value.Date)
                        cambios += $"- Fecha Nac.: {_fechaNacOrig.ToShortDateString()} ➔ {dtpFechaNacimiento.Value.ToShortDateString()}\n";

                    if (_emailOrig != txtEmail.Text.Trim())
                        cambios += $"- Email: {_emailOrig} ➔ {txtEmail.Text.Trim()}\n";

                    if (_telefonoOrig != txtTelefono.Text.Trim())
                        cambios += $"- Teléfono: {_telefonoOrig} ➔ {txtTelefono.Text.Trim()}\n";

                    if (_paisOrig != textPais.Text.Trim())
                        cambios += $"- País: {_paisOrig} ➔ {textPais.Text.Trim()}\n";

                    if (_localidadOrig != textLocalidad.Text.Trim())
                        cambios += $"- Localidad: {_localidadOrig} ➔ {textLocalidad.Text.Trim()}\n";

                    if (_provinciaOrig != txtProvincia.Text.Trim())
                        cambios += $"- Provincia: {_provinciaOrig} ➔ {txtProvincia.Text.Trim()}\n";

                    if (_calleOrig != txtCalle.Text.Trim() || _alturaOrig != textNroAltura.Text.Trim())
                        cambios += $"- Dirección: {_calleOrig} {_alturaOrig} ➔ {txtCalle.Text.Trim()} {textNroAltura.Text.Trim()}\n";

                    // Si la variable "cambios" sigue vacía, significa que el usuario no tocó nada
                    if (string.IsNullOrEmpty(cambios))
                    {
                        MessageBox.Show("No se detectó ningún cambio en los datos.", "Sin cambios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string mensajeConfirmacion = $"Se detectaron los siguientes cambios:\n\n{cambios}\n¿Desea aceptar y guardar los cambios?";

                    if (MessageBox.Show(mensajeConfirmacion, "Confirmar Modificación", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        _negocio.ModificarCliente(_dniSeleccionado, txtDni.Text.Trim(), txtNombre.Text.Trim(), txtApellido.Text.Trim(), dtpFechaNacimiento.Value, txtEmail.Text.Trim(), txtTelefono.Text.Trim(), textPais.Text.Trim(), textLocalidad.Text.Trim(), txtProvincia.Text.Trim(), txtCalle.Text.Trim(), textNroAltura.Text.Trim());
                        MessageBox.Show("Cliente modificado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        return;
                    }
                }

                LimpiarCampos();
                CargarGrilla(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnLimpiar_Click_1(object sender, EventArgs e) => LimpiarCampos();

        private void LimpiarCampos()
        {
            _dniSeleccionado = "";
            btnGuardar.Text = "Guardar";

            txtDni.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            dtpFechaNacimiento.Value = DateTime.Now.AddYears(-18);
            txtEmail.Clear();
            txtTelefono.Clear();
            textPais.Clear();
            textLocalidad.Clear();
            txtProvincia.Clear();
            txtCalle.Clear();
            textNroAltura.Clear();

            ActualizarEstadoBotones();
        }

        private void buttonMod_Click(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { }
        private void button2_Click(object sender, EventArgs e) { }
    }
}