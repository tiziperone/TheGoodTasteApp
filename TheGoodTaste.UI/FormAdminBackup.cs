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
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de Respaldo (*.bak)|*.bak";
                ofd.InitialDirectory = @"C:\Users\Public\";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    DialogResult confirmacion = MessageBox.Show(
                        "¿Está seguro de que desea restaurar la base de datos?\n\nSe perderán los cambios o registros posteriores a la fecha del backup seleccionado.",
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
    }
}