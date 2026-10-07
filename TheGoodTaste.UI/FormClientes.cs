using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TheGoodTaste.Negocio;

namespace TheGoodTaste.UI
{
    public partial class FormClientes : Form
    {
        private readonly ClienteNegocio _negocio = new ClienteNegocio();
        private string _dniSeleccionado = null;
        private DataRow _datosOriginales = null;
        private bool _verActivos = true; // Control de estado actual

        public FormClientes()
        {
            InitializeComponent();
        }

        private void FormClientes_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);
            ConfigurarLimitesCaracteres();
            ConfigurarGrilla();
            ConfigurarEventos();
            LimpiarCampos();
            CargarGrillaClientes(true); // Carga los activos por defecto
        }

        private void ConfigurarLimitesCaracteres()
        {
            if (txtNombre != null) txtNombre.MaxLength = 50;
            if (txtApellido != null) txtApellido.MaxLength = 50;
            if (txtDni != null) txtDni.MaxLength = 8;
            if (txtEmail != null) txtEmail.MaxLength = 100;
            if (txtTelefono != null) txtTelefono.MaxLength = 15;
            if (textPais != null) textPais.MaxLength = 50;
            if (txtProvincia != null) txtProvincia.MaxLength = 50;
            if (textLocalidad != null) textLocalidad.MaxLength = 50;
            if (txtCalle != null) txtCalle.MaxLength = 100;
            if (textNroAltura != null) textNroAltura.MaxLength = 10;
        }

        private void ConfigurarGrilla()
        {
            if (dgvClientes == null) return;
            dgvClientes.AllowUserToAddRows = false;
            dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClientes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void ConfigurarEventos()
        {
            if (txtNombre != null) txtNombre.KeyPress += SoloLetras_KeyPress;
            if (txtApellido != null) txtApellido.KeyPress += SoloLetras_KeyPress;
            if (textPais != null) textPais.KeyPress += SoloLetras_KeyPress;
            if (txtProvincia != null) txtProvincia.KeyPress += SoloLetras_KeyPress;
            if (textLocalidad != null) textLocalidad.KeyPress += SoloLetras_KeyPress;

            if (txtDni != null) txtDni.KeyPress += SoloNumeros_KeyPress;
            if (txtTelefono != null) txtTelefono.KeyPress += SoloNumeros_KeyPress;
            if (textNroAltura != null) textNroAltura.KeyPress += SoloNumeros_KeyPress;

            if (btnGuardar != null)
            {
                btnGuardar.Click -= btnGuardar_Click;
                btnGuardar.Click += btnGuardar_Click;
            }
            if (btnLimpiar != null)
            {
                btnLimpiar.Click -= btnLimpiar_Click;
                btnLimpiar.Click += btnLimpiar_Click;
            }

            Control[] controlesTexto = { txtDni, txtNombre, txtApellido, txtEmail, txtTelefono, textPais, txtProvincia, textLocalidad, txtCalle, textNroAltura };
            foreach (var control in controlesTexto)
            {
                if (control != null) control.TextChanged += Control_Modificado;
            }

            var dtp = ObtenerDateTimePicker();
            if (dtp != null) dtp.ValueChanged += Control_Modificado;

            // BOTONES DE FILTRO ACTIVO/INACTIVO
            if (btnActivo != null) btnActivo.Click += (s, e) => { CargarGrillaClientes(true); };
            if (btnInactivo != null) btnInactivo.Click += (s, e) => { CargarGrillaClientes(false); };

            if (dgvClientes != null)
            {
                dgvClientes.CellContentClick += dgvClientes_CellContentClick;
                dgvClientes.CurrentCellDirtyStateChanged += dgvClientes_CurrentCellDirtyStateChanged;
            }

            ConfigurarNavegacionTeclado();
        }

        private void ConfigurarNavegacionTeclado()
        {
            foreach (Control c in this.Controls)
            {
                if (c is TextBox && !c.Name.ToLower().Contains("buscar"))
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
        }

        private void Control_Modificado(object sender, EventArgs e) => ValidarReglaNegocioBotones();

        private void ValidarReglaNegocioBotones()
        {
            bool algunCampoConDato = !string.IsNullOrWhiteSpace(txtDni?.Text) || !string.IsNullOrWhiteSpace(txtNombre?.Text) ||
                                     !string.IsNullOrWhiteSpace(txtApellido?.Text) || !string.IsNullOrWhiteSpace(txtEmail?.Text) ||
                                     !string.IsNullOrWhiteSpace(txtTelefono?.Text) || !string.IsNullOrWhiteSpace(textPais?.Text) ||
                                     !string.IsNullOrWhiteSpace(txtProvincia?.Text) || !string.IsNullOrWhiteSpace(textLocalidad?.Text) ||
                                     !string.IsNullOrWhiteSpace(txtCalle?.Text) || !string.IsNullOrWhiteSpace(textNroAltura?.Text);

            bool obligatoriosCompletos = !string.IsNullOrWhiteSpace(txtDni?.Text) && !string.IsNullOrWhiteSpace(txtNombre?.Text) &&
                                         !string.IsNullOrWhiteSpace(txtApellido?.Text) && !string.IsNullOrWhiteSpace(txtEmail?.Text) &&
                                         !string.IsNullOrWhiteSpace(txtTelefono?.Text);

            if (btnLimpiar != null) btnLimpiar.Enabled = algunCampoConDato || !string.IsNullOrEmpty(_dniSeleccionado);
            if (btnGuardar != null) btnGuardar.Enabled = obligatoriosCompletos;
        }

        private DateTimePicker ObtenerDateTimePicker() => Controls.OfType<DateTimePicker>().FirstOrDefault();

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string dni = txtDni.Text.Trim();
                string nombre = txtNombre.Text.Trim();
                string apellido = txtApellido.Text.Trim();
                string email = txtEmail.Text.Trim();
                string telefono = txtTelefono.Text.Trim();
                string pais = textPais?.Text.Trim() ?? "";
                string provincia = txtProvincia?.Text.Trim() ?? "";
                string localidad = textLocalidad?.Text.Trim() ?? "";
                string calle = txtCalle?.Text.Trim() ?? "";
                string altura = textNroAltura?.Text.Trim() ?? "";
                var dtp = ObtenerDateTimePicker();
                DateTime fechaNacimiento = dtp != null ? dtp.Value.Date : DateTime.Today.AddYears(-18);

                if (string.IsNullOrEmpty(_dniSeleccionado))
                {
                    _negocio.GuardarCliente(dni, nombre, apellido, fechaNacimiento, email, telefono, pais, localidad, provincia, calle, altura);
                    MessageBox.Show("Cliente registrado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                    CargarGrillaClientes(_verActivos);
                }
                else
                {
                    List<string> cambios = ConstruirListaCambios(dni, nombre, apellido, email, telefono, pais, provincia, localidad, calle, altura, fechaNacimiento);
                    if (cambios.Count == 0)
                    {
                        MessageBox.Show("No se detectaron modificaciones para guardar.", "Sin cambios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    string mensaje = "¿Está seguro de aplicar los siguientes cambios?\n\n" + string.Join("\n", cambios);
                    if (MessageBox.Show(mensaje, "Confirmar Modificación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        _negocio.ModificarCliente(_dniSeleccionado, dni, nombre, apellido, fechaNacimiento, email, telefono, pais, localidad, provincia, calle, altura);
                        MessageBox.Show("Cliente actualizado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                        CargarGrillaClientes(_verActivos);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atención / Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private List<string> ConstruirListaCambios(string dni, string nombre, string apellido, string email, string telefono, string pais, string provincia, string localidad, string calle, string altura, DateTime fechaNacimiento)
        {
            var cambios = new List<string>();
            if (_datosOriginales == null) return cambios;

            string ObtenerOrig(string col) => _datosOriginales.Table.Columns.Contains(col) && _datosOriginales[col] != DBNull.Value ? _datosOriginales[col].ToString() : "";

            if (ObtenerOrig("dniCliente") != dni) cambios.Add($"• DNI: '{ObtenerOrig("dniCliente")}' -> '{dni}'");
            if (ObtenerOrig("nombreCliente") != nombre) cambios.Add($"• Nombre: '{ObtenerOrig("nombreCliente")}' -> '{nombre}'");
            if (ObtenerOrig("apellidoCliente") != apellido) cambios.Add($"• Apellido: '{ObtenerOrig("apellidoCliente")}' -> '{apellido}'");
            if (ObtenerOrig("correoCliente") != email) cambios.Add($"• Email: '{ObtenerOrig("correoCliente")}' -> '{email}'");
            if (ObtenerOrig("telefonoCliente") != telefono) cambios.Add($"• Teléfono: '{ObtenerOrig("telefonoCliente")}' -> '{telefono}'");
            if (ObtenerOrig("paisCliente") != pais) cambios.Add($"• País: '{ObtenerOrig("paisCliente")}' -> '{pais}'");
            if (ObtenerOrig("provinciaCliente") != provincia) cambios.Add($"• Provincia: '{ObtenerOrig("provinciaCliente")}' -> '{provincia}'");
            if (ObtenerOrig("localidadCliente") != localidad) cambios.Add($"• Localidad: '{ObtenerOrig("localidadCliente")}' -> '{localidad}'");
            if (ObtenerOrig("calleCliente") != calle) cambios.Add($"• Calle: '{ObtenerOrig("calleCliente")}' -> '{calle}'");
            if (ObtenerOrig("altura") != altura) cambios.Add($"• Altura: '{ObtenerOrig("altura")}' -> '{altura}'");

            if (_datosOriginales.Table.Columns.Contains("fechaNaciminetoCliente") && _datosOriginales["fechaNaciminetoCliente"] != DBNull.Value)
            {
                DateTime fechaOrig = Convert.ToDateTime(_datosOriginales["fechaNaciminetoCliente"]);
                if (fechaOrig.Date != fechaNacimiento.Date)
                    cambios.Add($"• Fecha Nacimiento: '{fechaOrig.ToShortDateString()}' -> '{fechaNacimiento.ToShortDateString()}'");
            }
            return cambios;
        }

        private void CargarGrillaClientes(bool verActivos)
        {
            if (dgvClientes == null) return;
            try
            {
                _verActivos = verActivos;
                DataTable dt = _negocio.ObtenerClientes(verActivos);
                dgvClientes.DataSource = dt;

                if (dgvClientes.Columns.Contains("Modificar")) dgvClientes.Columns.Remove("Modificar");
                if (dgvClientes.Columns.Contains("AccionEstado")) dgvClientes.Columns.Remove("AccionEstado");

                if (verActivos)
                {
                    var btnModificar = new DataGridViewButtonColumn
                    {
                        Name = "Modificar",
                        HeaderText = "Editar",
                        Text = "Modificar",
                        UseColumnTextForButtonValue = true
                    };
                    dgvClientes.Columns.Insert(0, btnModificar);
                }

                var btnEstado = new DataGridViewButtonColumn
                {
                    Name = "AccionEstado",
                    HeaderText = verActivos ? "Baja" : "Alta",
                    Text = verActivos ? "Eliminar" : "Activar",
                    UseColumnTextForButtonValue = true
                };
                dgvClientes.Columns.Insert(verActivos ? 1 : 0, btnEstado);

                if (dgvClientes.Columns.Contains("Activo")) dgvClientes.Columns["Activo"].Visible = false;
                if (dgvClientes.Columns.Contains("Estado")) dgvClientes.Columns["Estado"].Visible = false;

                foreach (DataGridViewColumn col in dgvClientes.Columns)
                {
                    if (col.Name != "Modificar" && col.Name != "AccionEstado")
                        col.ReadOnly = true;
                }

                dgvClientes.ClearSelection();

                // Limpiar el buscador visualmente al cambiar de estado
                if (textBoxBuscarCliente != null) textBoxBuscarCliente.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista de clientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvClientes.Rows.Count) return;

            string nombreColumna = dgvClientes.Columns[e.ColumnIndex].Name;
            DataGridViewRow fila = dgvClientes.Rows[e.RowIndex];

            if (nombreColumna == "Modificar")
            {
                CargarClienteParaEdicion(fila);
            }
            else if (nombreColumna == "AccionEstado")
            {
                ProcesarCambioEstado(fila);
            }
        }

        private void ProcesarCambioEstado(DataGridViewRow fila)
        {
            if (fila.Cells["dniCliente"]?.Value == null || fila.Cells["dniCliente"].Value == DBNull.Value) return;

            string dni = fila.Cells["dniCliente"].Value.ToString();
            string clienteNombre = fila.Cells["nombreCliente"]?.Value?.ToString() ?? "este cliente";

            bool nuevoEstado = !_verActivos;
            string accion = nuevoEstado ? "reactivar a" : "dar de baja a";

            if (MessageBox.Show($"¿Está seguro de que desea {accion} '{clienteNombre}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (_negocio.CambiarEstadoCliente(dni, nuevoEstado))
                    {
                        MessageBox.Show($"Cliente {(nuevoEstado ? "reactivado" : "dado de baja")} correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                        CargarGrillaClientes(_verActivos); // Recargamos el filtro actual
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CargarClienteParaEdicion(DataGridViewRow fila)
        {
            if (!_verActivos)
            {
                MessageBox.Show("No se pueden modificar datos de un cliente dado de baja.", "Acción no permitida", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dgvClientes.DataSource is DataTable dt && fila.DataBoundItem is DataRowView drv)
            {
                _datosOriginales = drv.Row;

                string ObtenerValor(string nombreColumna)
                {
                    if (_datosOriginales.Table.Columns.Contains(nombreColumna) && _datosOriginales[nombreColumna] != DBNull.Value)
                        return _datosOriginales[nombreColumna].ToString();
                    return "";
                }

                _dniSeleccionado = ObtenerValor("dniCliente");

                if (string.IsNullOrEmpty(_dniSeleccionado))
                {
                    MessageBox.Show("Error al cargar los datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                txtDni.Text = _dniSeleccionado;
                txtNombre.Text = ObtenerValor("nombreCliente");
                txtApellido.Text = ObtenerValor("apellidoCliente");
                txtEmail.Text = ObtenerValor("correoCliente");
                txtTelefono.Text = ObtenerValor("telefonoCliente");

                if (textPais != null) textPais.Text = ObtenerValor("paisCliente");
                if (txtProvincia != null) txtProvincia.Text = ObtenerValor("provinciaCliente");
                if (textLocalidad != null) textLocalidad.Text = ObtenerValor("localidadCliente");
                if (txtCalle != null) txtCalle.Text = ObtenerValor("calleCliente");
                if (textNroAltura != null) textNroAltura.Text = ObtenerValor("altura");

                var dtp = ObtenerDateTimePicker();
                if (dtp != null)
                {
                    string fechaStr = ObtenerValor("fechaNaciminetoCliente");
                    if (DateTime.TryParse(fechaStr, out DateTime fecha))
                        dtp.Value = fecha;
                }

                if (btnGuardar != null) btnGuardar.Text = "Modificar";
                if (btnLimpiar != null) btnLimpiar.Text = "Cancelar";

                ValidarReglaNegocioBotones();
            }
        }

        private void dgvClientes_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvClientes.IsCurrentCellDirty && dgvClientes.CurrentCell is DataGridViewCheckBoxCell)
                dgvClientes.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        // FUNCIÓN DE FILTRADO EN MEMORIA
        private void FiltrarGrilla(string filtro)
        {
            if (dgvClientes.DataSource is DataTable dt)
            {
                string f = filtro.Replace("'", "''");
                dt.DefaultView.RowFilter = string.IsNullOrEmpty(f) ? "" : $"(Convert(dniCliente, 'System.String') LIKE '%{f}%') OR (nombreCliente LIKE '%{f}%') OR (apellidoCliente LIKE '%{f}%') OR (correoCliente LIKE '%{f}%')";
            }
        }

        private void LimpiarCampos()
        {
            _dniSeleccionado = null;
            _datosOriginales = null;

            if (txtDni != null) txtDni.Clear();
            if (txtNombre != null) txtNombre.Clear();
            if (txtApellido != null) txtApellido.Clear();
            if (txtEmail != null) txtEmail.Clear();
            if (txtTelefono != null) txtTelefono.Clear();
            if (textPais != null) textPais.Clear();
            if (txtProvincia != null) txtProvincia.Clear();
            if (textLocalidad != null) textLocalidad.Clear();
            if (txtCalle != null) txtCalle.Clear();
            if (textNroAltura != null) textNroAltura.Clear();

            var dtp = ObtenerDateTimePicker();
            if (dtp != null) dtp.Value = DateTime.Today.AddYears(-18);

            if (btnGuardar != null) btnGuardar.Text = "Guardar";
            if (btnLimpiar != null) btnLimpiar.Text = "Limpiar";

            ValidarReglaNegocioBotones();
            if (txtNombre != null) txtNombre.Focus();
        }

        private void btnLimpiar_Click(object sender, EventArgs e) => LimpiarCampos();
        private void SoloLetras_KeyPress(object sender, KeyPressEventArgs e) { if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar)) e.Handled = true; }
        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e) { if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true; }

        private void textBoxBuscarCliente_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox txt)
            {
                FiltrarGrilla(txt.Text.Trim());
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}