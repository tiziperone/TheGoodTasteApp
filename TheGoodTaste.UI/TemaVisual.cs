using System.Drawing;
using System.Windows.Forms;

namespace TheGoodTaste.UI
{
    public static class TemaVisual
    {
        public static Color ColorFondoGeneral = Color.FromArgb(52, 38, 30);
        public static Color ColorFondoGrilla = Color.FromArgb(40, 28, 22);
        public static Color ColorFilasTabla = Color.FromArgb(68, 50, 40);
        public static Color ColorFilaAltTabla = Color.FromArgb(60, 44, 35);
        public static Color ColorTextoClaro = Color.FromArgb(245, 240, 230);
        public static Color ColorDoradoMarca = Color.FromArgb(195, 140, 45);

        public static Color ColorVerdeVivo = Color.FromArgb(46, 160, 80);
        public static Color ColorRojoVivo = Color.FromArgb(190, 50, 50);
        public static Color ColorBotonComun = Color.FromArgb(85, 62, 48);

        public static void AplicarEstilo(Control control)
        {
            if (control is Form form)
            {
                form.BackColor = ColorFondoGeneral;
                form.ForeColor = ColorTextoClaro;
            }

            foreach (Control subControl in control.Controls)
            {
                if (subControl is Panel || subControl is GroupBox)
                {
                    subControl.BackColor = ColorFondoGeneral;
                    subControl.ForeColor = ColorTextoClaro;
                }
                else if (subControl is Label lbl)
                {
                    lbl.ForeColor = ColorTextoClaro;
                    lbl.BackColor = Color.Transparent;
                }
                // Manejo especial para RadioButtons y CheckBoxes
                else if (subControl is RadioButton rb)
                {
                    rb.ForeColor = ColorTextoClaro;
                    rb.BackColor = Color.Transparent;

                    // Si el RadioButton tiene apariencia de Botón (como los de Activos/Inactivos)
                    if (rb.Appearance == Appearance.Button)
                    {
                        rb.FlatStyle = FlatStyle.Flat;
                        rb.FlatAppearance.BorderSize = 0;
                        rb.BackColor = ColorDoradoMarca;
                        rb.ForeColor = Color.Black;
                        rb.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                    }
                }
                else if (subControl is CheckBox chk)
                {
                    chk.ForeColor = ColorTextoClaro;
                    chk.BackColor = Color.Transparent;
                }
                else if (subControl is TextBox || subControl is ComboBox || subControl is DateTimePicker || subControl is NumericUpDown)
                {
                    subControl.BackColor = Color.White;
                    subControl.ForeColor = Color.Black;
                }
                // Botones estándar
                else if (subControl is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                    btn.Cursor = Cursors.Hand;

                    string nombre = btn.Name.ToLower();
                    string texto = btn.Text.ToLower();

                    if (nombre.Contains("save") || nombre.Contains("guardar") || nombre.Contains("confirmar") || texto.Contains("guardar") || texto.Contains("confirmar") || texto.Contains("ingresar"))
                    {
                        btn.BackColor = ColorVerdeVivo;
                        btn.ForeColor = Color.White;
                    }
                    else if (nombre.Contains("del") || nombre.Contains("limpiar") || nombre.Contains("eliminar") || texto.Contains("limpiar") || texto.Contains("eliminar") || texto.Contains("baja"))
                    {
                        btn.BackColor = ColorRojoVivo;
                        btn.ForeColor = Color.White;
                    }
                    else if (nombre.Contains("activo") || texto.Contains("activo") || texto.Contains("inactivo"))
                    {
                        btn.BackColor = ColorDoradoMarca;
                        btn.ForeColor = Color.Black;
                    }
                    else
                    {
                        btn.BackColor = ColorBotonComun;
                        btn.ForeColor = Color.White;
                    }
                }
                else if (subControl is DataGridView dgv)
                {
                    dgv.BackgroundColor = ColorFondoGrilla;
                    dgv.BorderStyle = BorderStyle.FixedSingle;
                    dgv.EnableHeadersVisualStyles = false;
                    dgv.RowHeadersVisible = false;

                    dgv.ColumnHeadersDefaultCellStyle.BackColor = ColorDoradoMarca;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                    dgv.ColumnHeadersDefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);

                    dgv.DefaultCellStyle.BackColor = ColorFilasTabla;
                    dgv.DefaultCellStyle.ForeColor = ColorTextoClaro;
                    dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(120, 90, 65);
                    dgv.DefaultCellStyle.SelectionForeColor = ColorTextoClaro;

                    dgv.AlternatingRowsDefaultCellStyle.BackColor = ColorFilaAltTabla;
                }

                if (subControl.HasChildren)
                {
                    AplicarEstilo(subControl);
                }
            }
        }
    }
}