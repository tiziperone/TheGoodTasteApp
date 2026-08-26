using System.Drawing;
using System.Windows.Forms;

namespace TheGoodTaste.UI
{
    public static class TemaVisual
    {
        // Paleta de colores moderna
        public static readonly Color FondoForm = Color.FromArgb(245, 246, 250);
        public static readonly Color Primario = Color.FromArgb(41, 128, 185);     // Azul moderno
        public static readonly Color Exito = Color.FromArgb(39, 174, 96);        // Verde confirmación
        public static readonly Color Peligro = Color.FromArgb(192, 57, 43);       // Rojo cancelar/limpiar
        public static readonly Color TextoOscuro = Color.FromArgb(44, 62, 80);
        public static readonly Font FuenteGeneral = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        public static readonly Font FuenteTitulos = new Font("Segoe UI Semibold", 10.5f);

        public static void AplicarEstilo(Form form)
        {
            form.BackColor = FondoForm;
            form.Font = FuenteGeneral;
            form.StartPosition = FormStartPosition.CenterScreen;

            AplicarAControles(form.Controls);
        }

        private static void AplicarAControles(Control.ControlCollection controles)
        {
            foreach (Control c in controles)
            {
                if (c is Label lbl)
                {
                    lbl.ForeColor = TextoOscuro;
                    lbl.Font = lbl.Font.Bold ? FuenteTitulos : FuenteGeneral;
                }
                else if (c is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Font = FuenteTitulos;
                    btn.ForeColor = Color.White;
                    btn.Cursor = Cursors.Hand;

                    // Asigna color según el texto o nombre del botón
                    string txt = btn.Text.ToLower();
                    if (txt.Contains("guardar") || txt.Contains("confirmar") || txt.Contains("agregar"))
                        btn.BackColor = Exito;
                    else if (txt.Contains("eliminar") || txt.Contains("cancelar") || txt.Contains("limpiar") || txt.Contains("salir"))
                        btn.BackColor = Peligro;
                    else
                        btn.BackColor = Primario;
                }
                else if (c is TextBox txt)
                {
                    txt.BorderStyle = BorderStyle.FixedSingle;
                    txt.Font = FuenteGeneral;
                }
                else if (c is ComboBox cbo)
                {
                    cbo.FlatStyle = FlatStyle.Flat;
                    cbo.Font = FuenteGeneral;
                }
                else if (c is DataGridView dgv)
                {
                    dgv.BackgroundColor = Color.White;
                    dgv.BorderStyle = BorderStyle.None;
                    dgv.EnableHeadersVisualStyles = false;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = Primario;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dgv.ColumnHeadersDefaultCellStyle.Font = FuenteTitulos;
                    dgv.RowHeadersVisible = false;
                    dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 243, 244);
                }

                // Si el control contiene otros controles (como un Panel o GroupBox), aplica recursivamente
                if (c.HasChildren)
                {
                    AplicarAControles(c.Controls);
                }
            }
        }
    }
}