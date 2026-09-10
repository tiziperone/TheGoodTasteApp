using System.Drawing;
using System.Windows.Forms;

namespace TheGoodTaste.UI
{
    public static class TemaVisual
    {
        // Paleta "The Good Taste" - Ajustada para contraste y botones vivos
        public static Color ColorFondoGeneral = Color.FromArgb(52, 38, 30);     // Marrón café cálido
        public static Color ColorFondoGrilla = Color.FromArgb(40, 28, 22);     // Fondo oscuro para delimitar la tabla vacía
        public static Color ColorFilasTabla = Color.FromArgb(68, 50, 40);     // Marrón medio para filas cargadas
        public static Color ColorFilaAltTabla = Color.FromArgb(60, 44, 35);     // Tono alternativo
        public static Color ColorTextoClaro = Color.FromArgb(245, 240, 230);  // Crema suave
        public static Color ColorDoradoMarca = Color.FromArgb(195, 140, 45);   // Dorado encabezados

        // Botones con tonos más vivos y brillantes
        public static Color ColorVerdeVivo = Color.FromArgb(46, 160, 80);    // Verde brillante
        public static Color ColorRojoVivo = Color.FromArgb(190, 50, 50);    // Rojo brillante
        public static Color ColorBotonComun = Color.FromArgb(85, 62, 48);     // Marrón claro para botones comunes (Agregar)

        public static void AplicarEstilo(Control control)
        {
            if (control is Form form)
            {
                form.BackColor = ColorFondoGeneral;
                form.ForeColor = ColorTextoClaro;
            }

            foreach (Control subControl in control.Controls)
            {
                // Paneles / GroupBox
                if (subControl is Panel || subControl is GroupBox)
                {
                    subControl.BackColor = ColorFondoGeneral;
                    subControl.ForeColor = ColorTextoClaro;
                }
                // Etiquetas
                else if (subControl is Label lbl)
                {
                    lbl.ForeColor = ColorTextoClaro;
                    lbl.BackColor = Color.Transparent;
                }
                // Radios / Checkbox
                else if (subControl is RadioButton || subControl is CheckBox)
                {
                    subControl.ForeColor = ColorTextoClaro;
                    subControl.BackColor = Color.Transparent;
                }
                // Cajas de texto y combos
                else if (subControl is TextBox || subControl is ComboBox || subControl is DateTimePicker || subControl is NumericUpDown)
                {
                    subControl.BackColor = Color.White;
                    subControl.ForeColor = Color.Black;
                }
                // Todos los Botones
                else if (subControl is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.ForeColor = Color.White;

                    string nombre = btn.Name.ToLower();
                    string texto = btn.Text.ToLower();

                    // Identificación por nombre de variable o texto del botón
                    if (nombre.Contains("save") || nombre.Contains("guardar") || nombre.Contains("confirmar") || texto.Contains("confirmar"))
                    {
                        btn.BackColor = ColorVerdeVivo;
                    }
                    else if (nombre.Contains("del") || nombre.Contains("limpiar") || nombre.Contains("eliminar") || texto.Contains("limpiar"))
                    {
                        btn.BackColor = ColorRojoVivo;
                    }
                    else
                    {
                        // Para "Agregar Producto" y otros botones de acción
                        btn.BackColor = ColorBotonComun;
                    }
                }
                // Tabla DataGridView
                else if (subControl is DataGridView dgv)
                {
                    // Un fondo más oscuro para la tabla delimita el área visible aunque no haya filas
                    dgv.BackgroundColor = ColorFondoGrilla;
                    dgv.BorderStyle = BorderStyle.FixedSingle;
                    dgv.EnableHeadersVisualStyles = false;
                    dgv.RowHeadersVisible = false;

                    // Encabezados Dorados
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = ColorDoradoMarca;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                    dgv.ColumnHeadersDefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);

                    // Filas de la grilla
                    dgv.DefaultCellStyle.BackColor = ColorFilasTabla;
                    dgv.DefaultCellStyle.ForeColor = ColorTextoClaro;
                    dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(120, 90, 65);
                    dgv.DefaultCellStyle.SelectionForeColor = ColorTextoClaro;

                    dgv.AlternatingRowsDefaultCellStyle.BackColor = ColorFilaAltTabla;
                }

                // Recursión
                if (subControl.HasChildren)
                {
                    AplicarEstilo(subControl);
                }
            }
        }
    }
}