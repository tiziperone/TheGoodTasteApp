namespace TheGoodTaste.UI
{
    partial class FormReporteGerente
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelReportes = new System.Windows.Forms.Panel();
            this.panelVentasVendedor = new System.Windows.Forms.Panel();
            this.textBoxBuscarVendedor = new System.Windows.Forms.TextBox();
            this.botonVentasVendedor = new System.Windows.Forms.Button();
            this.tituloVendedor = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tituloDesde = new System.Windows.Forms.Label();
            this.fechaHasta = new System.Windows.Forms.DateTimePicker();
            this.fechaDesde = new System.Windows.Forms.DateTimePicker();
            this.botonProductoVendido = new System.Windows.Forms.Button();
            this.botonVentas = new System.Windows.Forms.Button();
            this.botonRecaudacion = new System.Windows.Forms.Button();
            this.panelReportes.SuspendLayout();
            this.panelVentasVendedor.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelReportes
            // 
            this.panelReportes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelReportes.AutoSize = true;
            this.panelReportes.Controls.Add(this.panelVentasVendedor);
            this.panelReportes.Controls.Add(this.label1);
            this.panelReportes.Controls.Add(this.tituloDesde);
            this.panelReportes.Controls.Add(this.fechaHasta);
            this.panelReportes.Controls.Add(this.fechaDesde);
            this.panelReportes.Controls.Add(this.botonProductoVendido);
            this.panelReportes.Controls.Add(this.botonVentas);
            this.panelReportes.Controls.Add(this.botonRecaudacion);
            this.panelReportes.Location = new System.Drawing.Point(23, 67);
            this.panelReportes.Name = "panelReportes";
            this.panelReportes.Size = new System.Drawing.Size(666, 369);
            this.panelReportes.TabIndex = 0;
            this.panelReportes.Paint += new System.Windows.Forms.PaintEventHandler(this.panelReportes_Paint);
            // 
            // panelVentasVendedor
            // 
            this.panelVentasVendedor.Controls.Add(this.textBoxBuscarVendedor);
            this.panelVentasVendedor.Controls.Add(this.botonVentasVendedor);
            this.panelVentasVendedor.Controls.Add(this.tituloVendedor);
            this.panelVentasVendedor.Location = new System.Drawing.Point(39, 229);
            this.panelVentasVendedor.Name = "panelVentasVendedor";
            this.panelVentasVendedor.Size = new System.Drawing.Size(563, 91);
            this.panelVentasVendedor.TabIndex = 3;
            this.panelVentasVendedor.Paint += new System.Windows.Forms.PaintEventHandler(this.panelVentasVendedor_Paint);
            // 
            // textBoxBuscarVendedor
            // 
            this.textBoxBuscarVendedor.Location = new System.Drawing.Point(62, 41);
            this.textBoxBuscarVendedor.Name = "textBoxBuscarVendedor";
            this.textBoxBuscarVendedor.Size = new System.Drawing.Size(239, 22);
            this.textBoxBuscarVendedor.TabIndex = 3;
            this.textBoxBuscarVendedor.TextChanged += new System.EventHandler(this.textBoxBuscarVendedor_TextChanged);
            // 
            // botonVentasVendedor
            // 
            this.botonVentasVendedor.Location = new System.Drawing.Point(344, 24);
            this.botonVentasVendedor.Name = "botonVentasVendedor";
            this.botonVentasVendedor.Size = new System.Drawing.Size(181, 56);
            this.botonVentasVendedor.TabIndex = 0;
            this.botonVentasVendedor.Text = "Ventas por vendedor";
            this.botonVentasVendedor.UseVisualStyleBackColor = true;
            this.botonVentasVendedor.Click += new System.EventHandler(this.botonVentasVendedor_Click);
            // 
            // tituloVendedor
            // 
            this.tituloVendedor.AutoSize = true;
            this.tituloVendedor.Location = new System.Drawing.Point(59, 13);
            this.tituloVendedor.Name = "tituloVendedor";
            this.tituloVendedor.Size = new System.Drawing.Size(67, 16);
            this.tituloVendedor.TabIndex = 2;
            this.tituloVendedor.Text = "Vendedor";
            this.tituloVendedor.Click += new System.EventHandler(this.tituloVendedor_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(413, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Hasta";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // tituloDesde
            // 
            this.tituloDesde.AutoSize = true;
            this.tituloDesde.Location = new System.Drawing.Point(179, 50);
            this.tituloDesde.Name = "tituloDesde";
            this.tituloDesde.Size = new System.Drawing.Size(48, 16);
            this.tituloDesde.TabIndex = 2;
            this.tituloDesde.Text = "Desde";
            this.tituloDesde.Click += new System.EventHandler(this.tituloDesde_Click);
            // 
            // fechaHasta
            // 
            this.fechaHasta.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.fechaHasta.Location = new System.Drawing.Point(360, 78);
            this.fechaHasta.Name = "fechaHasta";
            this.fechaHasta.Size = new System.Drawing.Size(153, 22);
            this.fechaHasta.TabIndex = 1;
            this.fechaHasta.ValueChanged += new System.EventHandler(this.fechaHasta_ValueChanged);
            // 
            // fechaDesde
            // 
            this.fechaDesde.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.fechaDesde.Location = new System.Drawing.Point(123, 78);
            this.fechaDesde.Name = "fechaDesde";
            this.fechaDesde.Size = new System.Drawing.Size(153, 22);
            this.fechaDesde.TabIndex = 1;
            this.fechaDesde.ValueChanged += new System.EventHandler(this.fechaDesde_ValueChanged);
            // 
            // botonProductoVendido
            // 
            this.botonProductoVendido.Location = new System.Drawing.Point(431, 137);
            this.botonProductoVendido.Name = "botonProductoVendido";
            this.botonProductoVendido.Size = new System.Drawing.Size(152, 56);
            this.botonProductoVendido.TabIndex = 0;
            this.botonProductoVendido.Text = "Producto mas vendido";
            this.botonProductoVendido.UseVisualStyleBackColor = true;
            this.botonProductoVendido.Click += new System.EventHandler(this.botonProductoVendido_Click);
            // 
            // botonVentas
            // 
            this.botonVentas.Location = new System.Drawing.Point(251, 137);
            this.botonVentas.Name = "botonVentas";
            this.botonVentas.Size = new System.Drawing.Size(139, 56);
            this.botonVentas.TabIndex = 0;
            this.botonVentas.Text = "Ventas";
            this.botonVentas.UseVisualStyleBackColor = true;
            this.botonVentas.Click += new System.EventHandler(this.botonVentas_Click);
            // 
            // botonRecaudacion
            // 
            this.botonRecaudacion.Location = new System.Drawing.Point(74, 137);
            this.botonRecaudacion.Name = "botonRecaudacion";
            this.botonRecaudacion.Size = new System.Drawing.Size(139, 56);
            this.botonRecaudacion.TabIndex = 0;
            this.botonRecaudacion.Text = "Recaudacion";
            this.botonRecaudacion.UseVisualStyleBackColor = true;
            this.botonRecaudacion.Click += new System.EventHandler(this.botonRecaudacion_Click);
            // 
            // FormReporteGerente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1164, 524);
            this.Controls.Add(this.panelReportes);
            this.Name = "FormReporteGerente";
            this.Text = "FormReporteGerente";
            this.Load += new System.EventHandler(this.FormReporteGerente_Load);
            this.panelReportes.ResumeLayout(false);
            this.panelReportes.PerformLayout();
            this.panelVentasVendedor.ResumeLayout(false);
            this.panelVentasVendedor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelReportes;
        private System.Windows.Forms.Button botonVentas;
        private System.Windows.Forms.Button botonRecaudacion;
        private System.Windows.Forms.Button botonProductoVendido;
        private System.Windows.Forms.Panel panelVentasVendedor;
        private System.Windows.Forms.TextBox textBoxBuscarVendedor;
        private System.Windows.Forms.Button botonVentasVendedor;
        private System.Windows.Forms.Label tituloVendedor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label tituloDesde;
        private System.Windows.Forms.DateTimePicker fechaHasta;
        private System.Windows.Forms.DateTimePicker fechaDesde;
    }
}