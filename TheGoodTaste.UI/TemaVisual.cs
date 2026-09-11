using System.Drawing;
using System.Windows.Forms;

namespace TheGoodTaste.UI
{
    public static class TemaVisual
    {
        // Paleta base "The Good Taste"
        public static Color ColorFondoGeneral = Color.FromArgb(52, 38, 30);     // Marrón café cálido
        public static Color ColorFondoGrilla = Color.FromArgb(40, 28, 22);     // Fondo oscuro para delimitar la tabla vacía
        public static Color ColorFilasTabla = Color.FromArgb(68, 50, 40);     // Marrón medio para filas cargadas
        public static Color ColorFilaAltTabla = Color.FromArgb(60, 44, 35);     // Tono alternativo
        public static Color ColorTextoClaro = Color.FromArgb(245, 240, 230);  // Crema suave
        public static Color ColorDoradoMarca = Color.FromArgb(195, 140, 45);   // Dorado destacado

        // Botones de acción vivos y brillantes
        public static Color ColorVerdeVivo = Color.FromArgb(46, 160, 80);    // Verde brillante
        public static Color ColorRojoVivo = Color.FromArgb(190, 50, 50);    // Rojo brillante
        public static Color ColorBotonComun = Color.FromArgb(85, 62, 48);     // Marrón claro para acciones generales

        public static void AplicarEstilo(Control control)
        {
            if (control is Form form)
            {
                form.BackColor = ColorFondoGeneral;
                form.ForeColor = ColorTextoClaro;
            }

            foreach (Control subControl in control.Controls)
            {
                // Contenedores
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
                // Controles de selección
                else if (subControl is RadioButton || subControl is CheckBox)
                {
                    subControl.ForeColor = ColorTextoClaro;
                    subControl.BackColor = Color.Transparent;
                }
                // Entradas de texto
                else if (subControl is TextBox || subControl is ComboBox || subControl is DateTimePicker || subControl is NumericUpDown)
                {
                    subControl.BackColor = Color.White;
                    subControl.ForeColor = Color.Black;
                }
                // Botones
                else if (subControl is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                    btn.Cursor = Cursors.Hand;

                    string nombre = btn.Name.ToLower();
                    string texto = btn.Text.ToLower();

                    // Guardar / Confirmar / Ingresar
                    if (nombre.Contains("save") || nombre.Contains("guardar") || nombre.Contains("confirmar") || texto.Contains("guardar") || texto.Contains("confirmar") || texto.Contains("ingresar"))
                    {
                        btn.BackColor = ColorVerdeVivo;
                        btn.ForeColor = Color.White;
                    }
                    // Eliminar / Limpiar / Cancelar
                    else if (nombre.Contains("del") || nombre.Contains("limpiar") || nombre.Contains("eliminar") || texto.Contains("limpiar") || texto.Contains("eliminar") || texto.Contains("baja"))
                    {
                        btn.BackColor = ColorRojoVivo;
                        btn.ForeColor = Color.White;
                    }
                    // Filtros Activos / Inactivos
                    else if (nombre.Contains("activo") || texto.Contains("activo") || texto.Contains("inactivo"))
                    {
                        btn.BackColor = ColorDoradoMarca;
                        btn.ForeColor = Color.Black; // Texto oscuro para que resalte sobre el dorado
                    }
                    // Botones genéricos (ej. Agregar Producto)
                    else
                    {
                        btn.BackColor = ColorBotonComun;
                        btn.ForeColor = Color.White;
                    }
                }
                // Tablas
                else if (subControl is DataGridView dgv)
                {
                    dgv.BackgroundColor = ColorFondoGrilla;
                    dgv.BorderStyle = BorderStyle.FixedSingle;
                    dgv.EnableHeadersVisualStyles = false;
                    dgv.RowHeadersVisible = false;

                    // Encabezado
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = ColorDoradoMarca;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                    dgv.ColumnHeadersDefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);

                    // Filas
                    dgv.DefaultCellStyle.BackColor = ColorFilasTabla;
                    dgv.DefaultCellStyle.ForeColor = ColorTextoClaro;
                    dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(120, 90, 65);
                    dgv.DefaultCellStyle.SelectionForeColor = ColorTextoClaro;

                    dgv.AlternatingRowsDefaultCellStyle.BackColor = ColorFilaAltTabla;
                }

                // Aplicar recursivamente a paneles anidados
                if (subControl.HasChildren)
                {
                    AplicarEstilo(subControl);
                }
            }
        }
    }
}