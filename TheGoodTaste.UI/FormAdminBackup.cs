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

        public FormAdminBackup()
        {
            InitializeComponent();
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

        private void btnCrearBackup_Click(object sender, EventArgs e)
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
                            MessageBox.Show($"Copia de seguridad creada correctamente en:\n{sfd.FileName}",
                                            "Backup Exitoso",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al generar el backup: {ex.Message}",
                                        "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnRestaurarBackup_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de Respaldo (*.bak)|*.bak";
                ofd.InitialDirectory = @"C:\Users\Public\";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    DialogResult confirmacion = MessageBox.Show(
                        "¿Está seguro de que desea restaurar la base de datos?\nSe perderán los cambios que no estén guardados en el archivo seleccionado.",
                        "Confirmar Restauración",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (confirmacion == DialogResult.Yes)
                    {
                        try
                        {
                            if (_backupNegocio.RestaurarBackup(ofd.FileName))
                            {
                                MessageBox.Show("Base de datos restaurada con éxito.",
                                                "Restauración Completada",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Information);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error al restaurar la base de datos: {ex.Message}",
                                            "Error",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
    }
}