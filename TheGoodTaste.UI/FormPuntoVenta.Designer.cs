namespace TheGoodTaste.UI
{
    partial class FormPuntoVenta
    {
        private System.ComponentModel.IContainer components = null; //Variable del diseñador necesaria.

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblInfoCliente = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpFechaVenta = new System.Windows.Forms.DateTimePicker();
            this.lblProductos = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPrecio = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.dgvDetalles = new System.Windows.Forms.DataGridView();
            this.colIdProducto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrecio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSubtotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label7 = new System.Windows.Forms.Label();
            this.lblTotalMonto = new System.Windows.Forms.Label();
            this.btnGuardarVenta = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.txtBuscarCliente = new System.Windows.Forms.TextBox();
            this.lblVendedor = new System.Windows.Forms.Label();
            this.txtBuscarProducto = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalles)).BeginInit();
            this.SuspendLayout();
            // 
            // lblInfoCliente
            // 
            this.lblInfoCliente.AutoSize = true;
            this.lblInfoCliente.Location = new System.Drawing.Point(37, 106);
            this.lblInfoCliente.Name = "lblInfoCliente";
            this.lblInfoCliente.Size = new System.Drawing.Size(48, 16);
            this.lblInfoCliente.TabIndex = 0;
            this.lblInfoCliente.Text = "Cliente";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Fecha";
            // 
            // dtpFechaVenta
            // 
            this.dtpFechaVenta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaVenta.Location = new System.Drawing.Point(147, 147);
            this.dtpFechaVenta.Name = "dtpFechaVenta";
            this.dtpFechaVenta.Size = new System.Drawing.Size(171, 22);
            this.dtpFechaVenta.TabIndex = 6;
            // 
            // lblProductos
            // 
            this.lblProductos.AutoSize = true;
            this.lblProductos.Location = new System.Drawing.Point(34, 198);
            this.lblProductos.Name = "lblProductos";
            this.lblProductos.Size = new System.Drawing.Size(61, 16);
            this.lblProductos.TabIndex = 7;
            this.lblProductos.Text = "Producto";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(37, 288);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 16);
            this.label5.TabIndex = 9;
            this.label5.Text = "Precio";
            // 
            // txtPrecio
            // 
            this.txtPrecio.Location = new System.Drawing.Point(149, 282);
            this.txtPrecio.Name = "txtPrecio";
            this.txtPrecio.ReadOnly = true;
            this.txtPrecio.Size = new System.Drawing.Size(169, 22);
            this.txtPrecio.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(37, 246);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 16);
            this.label6.TabIndex = 11;
            this.label6.Text = "Cantidad";
            // 
            // nudCantidad
            // 
            this.nudCantidad.Location = new System.Drawing.Point(148, 240);
            this.nudCantidad.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCantidad.Name = "nudCantidad";
            this.nudCantidad.Size = new System.Drawing.Size(170, 22);
            this.nudCantidad.TabIndex = 12;
            this.nudCantidad.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnAgregar
            // 
            this.btnAgregar.Location = new System.Drawing.Point(95, 342);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(132, 44);
            this.btnAgregar.TabIndex = 13;
            this.btnAgregar.Text = "Agregar Producto";
            this.btnAgregar.UseVisualStyleBackColor = true;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // dgvDetalles
            // 
            this.dgvDetalles.AllowUserToAddRows = false;
            this.dgvDetalles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDetalles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDetalles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIdProducto,
            this.colNombre,
            this.colPrecio,
            this.colCantidad,
            this.colSubtotal});
            this.dgvDetalles.Location = new System.Drawing.Point(394, 34);
            this.dgvDetalles.Name = "dgvDetalles";
            this.dgvDetalles.RowHeadersWidth = 51;
            this.dgvDetalles.RowTemplate.Height = 24;
            this.dgvDetalles.Size = new System.Drawing.Size(561, 254);
            this.dgvDetalles.TabIndex = 14;
            // 
            // colIdProducto
            // 
            this.colIdProducto.HeaderText = "ID";
            this.colIdProducto.MinimumWidth = 6;
            this.colIdProducto.Name = "colIdProducto";
            // 
            // colNombre
            // 
            this.colNombre.HeaderText = "Producto";
            this.colNombre.MinimumWidth = 6;
            this.colNombre.Name = "colNombre";
            // 
            // colPrecio
            // 
            this.colPrecio.HeaderText = "Precio Unitario";
            this.colPrecio.MinimumWidth = 6;
            this.colPrecio.Name = "colPrecio";
            // 
            // colCantidad
            // 
            this.colCantidad.HeaderText = "Cantidad";
            this.colCantidad.MinimumWidth = 6;
            this.colCantidad.Name = "colCantidad";
            // 
            // colSubtotal
            // 
            this.colSubtotal.HeaderText = "Subtotal";
            this.colSubtotal.MinimumWidth = 6;
            this.colSubtotal.Name = "colSubtotal";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(494, 307);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 16);
            this.label7.TabIndex = 15;
            this.label7.Text = "TOTAL: $";
            // 
            // lblTotalMonto
            // 
            this.lblTotalMonto.AutoSize = true;
            this.lblTotalMonto.Location = new System.Drawing.Point(605, 307);
            this.lblTotalMonto.Name = "lblTotalMonto";
            this.lblTotalMonto.Size = new System.Drawing.Size(31, 16);
            this.lblTotalMonto.TabIndex = 16;
            this.lblTotalMonto.Text = "0.00";
            this.lblTotalMonto.Click += new System.EventHandler(this.lblTotalMonto_Click);
            // 
            // btnGuardarVenta
            // 
            this.btnGuardarVenta.Location = new System.Drawing.Point(429, 342);
            this.btnGuardarVenta.Name = "btnGuardarVenta";
            this.btnGuardarVenta.Size = new System.Drawing.Size(114, 46);
            this.btnGuardarVenta.TabIndex = 17;
            this.btnGuardarVenta.Text = "Confirmar Venta";
            this.btnGuardarVenta.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.btnGuardarVenta.UseVisualStyleBackColor = true;
            this.btnGuardarVenta.Click += new System.EventHandler(this.btnGuardarVenta_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(583, 342);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(102, 46);
            this.btnLimpiar.TabIndex = 18;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // txtBuscarCliente
            // 
            this.txtBuscarCliente.Location = new System.Drawing.Point(40, 64);
            this.txtBuscarCliente.Name = "txtBuscarCliente";
            this.txtBuscarCliente.Size = new System.Drawing.Size(278, 22);
            this.txtBuscarCliente.TabIndex = 19;
            // 
            // lblVendedor
            // 
            this.lblVendedor.AutoSize = true;
            this.lblVendedor.Location = new System.Drawing.Point(37, 9);
            this.lblVendedor.Name = "lblVendedor";
            this.lblVendedor.Size = new System.Drawing.Size(70, 16);
            this.lblVendedor.TabIndex = 0;
            this.lblVendedor.Text = "Vendedor:";
            // 
            // txtBuscarProducto
            // 
            this.txtBuscarProducto.Location = new System.Drawing.Point(149, 192);
            this.txtBuscarProducto.Name = "txtBuscarProducto";
            this.txtBuscarProducto.Size = new System.Drawing.Size(169, 22);
            this.txtBuscarProducto.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ingresar DNI del Cliente";
            // 
            // FormPuntoVenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 441);
            this.Controls.Add(this.txtBuscarProducto);
            this.Controls.Add(this.txtBuscarCliente);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnGuardarVenta);
            this.Controls.Add(this.lblTotalMonto);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.dgvDetalles);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.nudCantidad);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtPrecio);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblProductos);
            this.Controls.Add(this.dtpFechaVenta);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblVendedor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblInfoCliente);
            this.Name = "FormPuntoVenta";
            this.Text = "PuntoVenta";
            this.Load += new System.EventHandler(this.FormPuntoVenta_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalles)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInfoCliente;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpFechaVenta;
        private System.Windows.Forms.Label lblProductos;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPrecio;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.DataGridView dgvDetalles;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblTotalMonto;
        private System.Windows.Forms.Button btnGuardarVenta;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIdProducto;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrecio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCantidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSubtotal;
        private System.Windows.Forms.TextBox txtBuscarCliente;
        private System.Windows.Forms.Label lblVendedor;
        private System.Windows.Forms.TextBox txtBuscarProducto;
        private System.Windows.Forms.Label label1;
    }
}