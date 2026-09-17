using System;
using System.Windows.Forms;
using TheGoodTaste.Negocio;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.UI
{
    public partial class FormLogin : Form
    {
        public UsuarioSistema UsuarioAutenticado { get; private set; }
        private readonly UsuarioNegocio _negocio = new UsuarioNegocio();

        public FormLogin()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.KeyPreview = true;
            this.KeyDown += FormLogin_KeyDown;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);
            this.AcceptButton = btnIngresar;
            txtPassword.UseSystemPasswordChar = true;
            txtUsuario.KeyDown += Campos_KeyDown;
            txtPassword.KeyDown += Campos_KeyDown;
            txtUsuario.Focus();
        }

        private void FormLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { this.DialogResult = DialogResult.Cancel; this.Close(); }
        }

        private void Campos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down) { e.SuppressKeyPress = true; SelectNextControl((Control)sender, true, true, true, true); }
            else if (e.KeyCode == Keys.Up) { e.SuppressKeyPress = true; SelectNextControl((Control)sender, false, true, true, true); }
        }

        private void chkVerPassLogin_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkVerPassLogin.Checked;
            chkVerPassLogin.Text = chkVerPassLogin.Checked ? "🙈" : "👁️";
        }

        private void btnIngresar_Click_1(object sender, EventArgs e)
        {
            try
            {
                UsuarioAutenticado = _negocio.Autenticar(txtUsuario.Text.Trim(), txtPassword.Text.Trim());
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
    }
}