using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheGoodTaste.UI
{
    public partial class FormPuntoVenta : Form
    {
        public FormPuntoVenta()
        {
            InitializeComponent();
        }

        private void FormPuntoVenta_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
