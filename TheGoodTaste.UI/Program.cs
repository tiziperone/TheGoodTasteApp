using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheGoodTaste.UI
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var login = new FormLogin())
            {
                // Abre el login de forma modal
                if (login.ShowDialog() == DialogResult.OK)
                {
                    // Pasa el usuario autenticado a la ventana principal
                    Application.Run(new FormPrincipal(login.UsuarioAutenticado));
                }
                else
                {
                    // Si cierra o cancela la ventana de login, termina la app
                    Application.Exit();
                }
            }
        }
    }
}
