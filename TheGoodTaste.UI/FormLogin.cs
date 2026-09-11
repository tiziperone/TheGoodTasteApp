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

            // Habilita la captura previa de teclas en el formulario
            this.KeyPreview = true;
            this.KeyDown += FormLogin_KeyDown;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);

            // Asigna el botón por defecto para ejecutar la acción con ENTER
            this.AcceptButton = btnIngresar;

            // Asegura que la contraseña inicie oculta con los caracteres nativos del sistema
            txtPassword.UseSystemPasswordChar = true;

            // Foco inicial en el usuario
            txtUsuario.Focus();
        }

        // Manejo global de teclas (Atajo para cerrar con ESC)
        private void FormLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        // Evento para el botón/icono de "Ver / Ocultar Contraseña"
        private void btnVerPassword_Click(object sender, EventArgs e)
        {
            // Alterna la visibilidad del texto
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;

            // Cambia el texto/emoji del botón según el estado
            if (sender is Button btn)
            {
                btn.Text = txtPassword.UseSystemPasswordChar ? "👁️" : "🙈";
            }
        }

        // Evento del botón "Ingresar" / "Iniciar Sesión"
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

        // Método oficial para CheckBox de Ver Contraseña
        private void chkVerPassLogin_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkVerPassLogin.Checked;
            chkVerPassLogin.Text = chkVerPassLogin.Checked ? "🙈" : "👁️";
        }

        // Parche para eliminar el error del Diseñador si quedó el evento viejo enlazado
        private void btnVerPassword_CheckedChanged(object sender, EventArgs e) => chkVerPassLogin_CheckedChanged(sender, e);
    }
}