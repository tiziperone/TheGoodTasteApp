using System;
using System.Globalization;
using System.Windows.Forms;

namespace TheGoodTaste.UI
{
    public partial class FormCobroEfectivo : Form
    {
        public decimal TotalEfectivoAPagar { get; private set; }
        public decimal DineroIngresado { get; private set; }
        public decimal VueltoCalculado { get; private set; }
        public bool Confirmado { get; private set; } = false;

        public FormCobroEfectivo(decimal totalEfectivo)
        {
            InitializeComponent();
            TotalEfectivoAPagar = totalEfectivo;
        }

        private void FormCobroEfectivo_Load(object sender, EventArgs e)
        {

            TemaVisual.AplicarEstilo(this);
            labelMontoTotal.Text = $"Total a Pagar: ${TotalEfectivoAPagar:N2}";
            lblVuelto.Text = "Vuelto: $0,00";

            txtEfectivoIngresado.Focus();
            this.AcceptButton = buttonConf;
            this.CancelButton = buttonCanc;
        }

        private void txtEfectivoIngresado_TextChanged(object sender, EventArgs e)
        {
            DineroIngresado = ConvertirADecimal(txtEfectivoIngresado.Text);
            VueltoCalculado = DineroIngresado - TotalEfectivoAPagar;

            if (VueltoCalculado >= 0)
            {
                lblVuelto.Text = $"Vuelto: ${VueltoCalculado:N2}";
                lblVuelto.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                lblVuelto.Text = $"Falta: ${Math.Abs(VueltoCalculado):N2}";
                lblVuelto.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void txtEfectivoIngresado_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '.')
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.') e.KeyChar = ',';
            if (e.KeyChar == ',' && txtEfectivoIngresado.Text.Contains(",")) e.Handled = true;
        }

        private void buttonConf_Click(object sender, EventArgs e)
        {
            if (DineroIngresado < TotalEfectivoAPagar)
            {
                MessageBox.Show("El dinero ingresado es menor al total a pagar en efectivo.", "Monto Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEfectivoIngresado.Focus();
                return;
            }

            Confirmado = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCanc_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private decimal ConvertirADecimal(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return 0;
            string textoLimpio = texto.Replace("$", "").Trim();
            textoLimpio = textoLimpio.Replace(".", "").Replace(",", ".");
            decimal.TryParse(textoLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal resultado);
            return resultado;
        }

        private void labelMontoTotal_Click(object sender, EventArgs e) { }
        private void lblVuelto_Click(object sender, EventArgs e) { }
    }
}