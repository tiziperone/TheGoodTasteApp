using System;
using System.Drawing;
using System.Windows.Forms;
using The_Good_Taste.Entidades;
using TheGoodTaste.Negocio;

namespace TheGoodTaste.UI
{
    public partial class FormAdminBackup : Form
    {
        private readonly UsuarioSistema _usuarioActual;
        private readonly BackupNegocio _backupNegocio = new BackupNegocio();
        private readonly UsuarioNegocio _usuarioNegocio = new UsuarioNegocio();

        // Controles de interfaz
        private Panel panelTarjeta;
        private Label lblTitulo;
        private Label lblDescripcion;
        private Button btnCrearBackup;
        private Button btnRestaurarBackup;

        public FormAdminBackup()
        {
            InitializeComponent();
            ConstruirInterfazGrafica();
        }

        public FormAdminBackup(UsuarioSistema usuario) : this()
        {
            _usuarioActual = usuario;
        }

        private void FormAdminBackup_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);
            this.Text = "Mantenimiento del Sistema - Copias de Seguridad";
        }

        private void ConstruirInterfazGrafica()
        {
            this.BackColor = Color.FromArgb(35, 25, 20); // Fondo marrón oscuro del sistema

            // Contenedor principal centrado
            panelTarjeta = new Panel
            {
                Size = new Size(520, 310),
                BackColor = Color.FromArgb(45, 33, 27),
                BorderStyle = BorderStyle.FixedSingle
            };
            panelTarjeta.Location = new Point((this.ClientSize.Width - panelTarjeta.Width) / 2,
                                             (this.ClientSize.Height - panelTarjeta.Height) / 2);
            panelTarjeta.Anchor = AnchorStyles.None;

            // Título de la tarjeta
            lblTitulo = new Label
            {
                Text = "Gestión de Copias de Seguridad",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 160, 50), // Dorado
                AutoSize = true,
                Location = new Point(30, 25)
            };

            // Descripción
            lblDescripcion = new Label
            {
                Text = "Módulo exclusivo de administración para la generación de respaldos (.bak) e integridad de datos de TheGoodTasteDB.",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.White,
                Size = new Size(460, 45),
                Location = new Point(30, 65)
            };

            // Botón 1: Crear Backup
            btnCrearBackup = new Button
            {
                Text = "💾  Generar Copia de Seguridad",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = Color.FromArgb(180, 130, 40), // Dorado activo
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(460, 50),
                Location = new Point(30, 130),
                Cursor = Cursors.Hand
            };
            btnCrearBackup.FlatAppearance.BorderSize = 0;
            btnCrearBackup.Click += BtnCrearBackup_Click;

            // Botón 2: Restaurar Backup
            btnRestaurarBackup = new Button
            {
                Text = "📂  Restaurar Base de Datos",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = Color.FromArgb(60, 45, 38),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(460, 50),
                Location = new Point(30, 200),
                Cursor = Cursors.Hand
            };
            btnRestaurarBackup.FlatAppearance.BorderSize = 0;
            btnRestaurarBackup.Click += BtnRestaurarBackup_Click;

            // Agregar controles al panel
            panelTarjeta.Controls.Add(lblTitulo);
            panelTarjeta.Controls.Add(lblDescripcion);
            panelTarjeta.Controls.Add(btnCrearBackup);
            panelTarjeta.Controls.Add(btnRestaurarBackup);

            // Agregar panel al formulario
            this.Controls.Add(panelTarjeta);

            // Mantener centrado si se redimensiona la ventana
            this.Resize += (s, e) =>
            {
                panelTarjeta.Location = new Point((this.ClientSize.Width - panelTarjeta.Width) / 2,
                                                 (this.ClientSize.Height - panelTarjeta.Height) / 2);
            };
        }

        private void BtnCrearBackup_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Archivos de Respaldo (*.bak)|*.bak";
                sfd.FileName = $"TheGoodTasteDB_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                sfd.InitialDirectory = @"C:\Users\Public\";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (_backupNegocio.RealizarBackup(sfd.FileName))
                        {
                            MessageBox.Show($"Copia de seguridad realizada con éxito en:\n{sfd.FileName}",
                                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al generar el backup: {ex.Message}",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnRestaurarBackup_Click(object sender, EventArgs e)
        {
            // 1. Solicitar usuario y contraseña del gerente para la doble autorización
            var credencialesGerente = SolicitarCredencialesGerente();
            if (string.IsNullOrEmpty(credencialesGerente.username) || string.IsNullOrEmpty(credencialesGerente.password))
            {
                // Solo llegará aquí si el usuario canceló o cerró la ventana
                return;
            }

            try
            {
                // 2. Validar las credenciales del gerente en la base de datos
                var usuarioGerente = _usuarioNegocio.Autenticar(credencialesGerente.username, credencialesGerente.password);

                if (usuarioGerente == null)
                {
                    MessageBox.Show("El usuario o la contraseña del gerente son incorrectos.",
                                    "Validación Fallida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Opcional: Si deseas validar estrictamente que el rol sea Gerente (descomenta si corresponde):
                /*
                if (usuarioGerente.IdRol != 1) // Asumiendo que 1 es Gerente
                {
                    MessageBox.Show("El usuario ingresado no cuenta con privilegios de Gerente.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                */
            }
            catch (Exception)
            {
                MessageBox.Show("Credenciales de gerente inválidas. No se puede proceder con la restauración.",
                                "Validación Fallida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. Si la autenticación es exitosa, procedemos a seleccionar el archivo de backup
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de Respaldo (*.bak)|*.bak";
                ofd.InitialDirectory = @"C:\Users\Public\";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    DialogResult confirmacion = MessageBox.Show(
                        $"Restauración autorizada por el gerente: [{credencialesGerente.username}]\n\n¿Está seguro de que desea restaurar la base de datos?\nSe perderán los cambios o registros posteriores a la fecha del backup seleccionado.",
                        "Confirmar Restauración", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirmacion == DialogResult.Yes)
                    {
                        try
                        {
                            if (_backupNegocio.RestaurarBackup(ofd.FileName))
                            {
                                MessageBox.Show("Base de datos restaurada correctamente.",
                                                "Restauración Completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error al restaurar la base de datos: {ex.Message}",
                                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Muestra un cuadro de diálogo para solicitar tanto el usuario como la contraseña del gerente con opción de visibilidad.
        /// </summary>
        private (string username, string password) SolicitarCredencialesGerente()
        {
            using (Form formPassword = new Form())
            {
                formPassword.Width = 380;
                formPassword.Height = 270;
                formPassword.FormBorderStyle = FormBorderStyle.FixedDialog;
                formPassword.Text = "Autorización de Gerencia";
                formPassword.StartPosition = FormStartPosition.CenterParent;
                formPassword.MaximizeBox = false;
                formPassword.MinimizeBox = false;
                formPassword.BackColor = Color.FromArgb(35, 25, 20);

                Label lblUser = new Label()
                {
                    Left = 20,
                    Top = 15,
                    Width = 320,
                    Text = "Usuario del Gerente:",
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9.5f)
                };

                TextBox txtUser = new TextBox()
                {
                    Left = 20,
                    Top = 40,
                    Width = 320,
                    Font = new Font("Segoe UI", 10f)
                };

                Label lblPassword = new Label()
                {
                    Left = 20,
                    Top = 75,
                    Width = 320,
                    Text = "Contraseña:",
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9.5f)
                };

                TextBox txtPassword = new TextBox()
                {
                    Left = 20,
                    Top = 100,
                    Width = 320,
                    UseSystemPasswordChar = true,
                    Font = new Font("Segoe UI", 10f)
                };

                // CheckBox para mostrar/ocultar contraseña
                CheckBox chkMostrarOcultar = new CheckBox()
                {
                    Left = 20,
                    Top = 135,
                    Width = 200,
                    Text = "Mostrar contraseña",
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9f),
                    Cursor = Cursors.Hand
                };

                chkMostrarOcultar.CheckedChanged += (s, e) =>
                {
                    txtPassword.UseSystemPasswordChar = !chkMostrarOcultar.Checked;
                };

                Button btnAceptar = new Button()
                {
                    Text = "Aceptar",
                    Left = 140,
                    Width = 90,
                    Top = 175,
                    BackColor = Color.FromArgb(180, 130, 40),
                    ForeColor = Color.Black,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnAceptar.FlatAppearance.BorderSize = 0;

                Button btnCancelar = new Button()
                {
                    Text = "Cancelar",
                    Left = 250,
                    Width = 90,
                    Top = 175,
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.FromArgb(60, 45, 38),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnCancelar.FlatAppearance.BorderSize = 0;

                // Validación antes de cerrar: solo se cierra con OK si los campos están completos
                btnAceptar.Click += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(txtUser.Text))
                    {
                        MessageBox.Show("Debe ingresar el usuario del gerente.",
                                        "Campo requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtUser.Focus();
                        return;
                    }

                    if (string.IsNullOrEmpty(txtPassword.Text))
                    {
                        MessageBox.Show("Debe ingresar la contraseña del gerente.",
                                        "Campo requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPassword.Focus();
                        return;
                    }

                    formPassword.DialogResult = DialogResult.OK;
                };

                formPassword.Controls.Add(lblUser);
                formPassword.Controls.Add(txtUser);
                formPassword.Controls.Add(lblPassword);
                formPassword.Controls.Add(txtPassword);
                formPassword.Controls.Add(chkMostrarOcultar);
                formPassword.Controls.Add(btnAceptar);
                formPassword.Controls.Add(btnCancelar);

                formPassword.AcceptButton = btnAceptar;
                formPassword.CancelButton = btnCancelar;

                if (formPassword.ShowDialog(this) == DialogResult.OK)
                {
                    return (txtUser.Text.Trim(), txtPassword.Text);
                }

                return (string.Empty, string.Empty);
            }
        }
    }
}