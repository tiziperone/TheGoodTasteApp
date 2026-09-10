using System;
using System.Drawing;
using System.Windows.Forms;
using The_Good_Taste.Datos;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.UI
{
    public partial class FormLogin : Form
    {
        // Propiedad pública para que Program.cs pueda leer quién inició sesión
        public UsuarioSistema UsuarioAutenticado { get; private set; }

        public FormLogin()
        {
            InitializeComponent();

            // Centra la ventana de login en la pantalla
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);

            // Asegura que la contraseña inicie oculta con los caracteres nativos del sistema
            txtPassword.UseSystemPasswordChar = true;
        }

        // Evento para el botón/icono de "Ver / Ocultar Contraseña"
        private void btnVerPassword_Click(object sender, EventArgs e)
        {
            // Alterna la visibilidad del texto
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;

            // Opcional: cambia el texto/emoji del botón según el estado
            if (sender is Button btn)
            {
                btn.Text = txtPassword.UseSystemPasswordChar ? "👁️" : "🙈";
            }
        }

        // Asocia este evento al botón "Ingresar" / "Iniciar Sesión"
        private void btnIngresar_Click_1(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string clave = txtPassword.Text.Trim();

            // 1. Validaciones básicas de campos vacíos
            if (string.IsNullOrEmpty(usuario))
            {
                MessageBox.Show("Por favor, ingrese su nombre de usuario.", "Campo requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsuario.Focus();
                return;
            }

            if (string.IsNullOrEmpty(clave))
            {
                MessageBox.Show("Por favor, ingrese su contraseña.", "Campo requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            // 2. Consulta a la capa de datos
            UsuarioDatos repoDatos = new UsuarioDatos();
            UsuarioSistema userEncontrado = repoDatos.Autenticar(usuario, clave);

            if (userEncontrado != null)
            {
                // Guardamos la sesión y cerramos devolviendo OK
                this.UsuarioAutenticado = userEncontrado;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Error de autenticación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        // Método oficial con el último nombre
        private void chkVerPassLogin_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkVerPassLogin.Checked;
            chkVerPassLogin.Text = chkVerPassLogin.Checked ? "🙈" : "👁️";
        }

        // Parche para eliminar el error del Diseñador si quedó el evento viejo enlazado
        private void btnVerPassword_CheckedChanged(object sender, EventArgs e) => chkVerPassLogin_CheckedChanged(sender, e);
    }
}