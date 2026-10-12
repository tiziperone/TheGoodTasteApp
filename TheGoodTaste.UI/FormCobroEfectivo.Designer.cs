namespace TheGoodTaste.UI
{
    partial class FormCobroEfectivo
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
            this.labelMontoTotal = new System.Windows.Forms.Label();
            this.lblVuelto = new System.Windows.Forms.Label();
            this.txtEfectivoIngresado = new System.Windows.Forms.TextBox();
            this.buttonConf = new System.Windows.Forms.Button();
            this.buttonCanc = new System.Windows.Forms.Button();
            this.labelMonto = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelMontoTotal
            // 
            this.labelMontoTotal.AutoSize = true;
            this.labelMontoTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMontoTotal.Location = new System.Drawing.Point(180, 238);
            this.labelMontoTotal.Name = "labelMontoTotal";
            this.labelMontoTotal.Size = new System.Drawing.Size(205, 39);
            this.labelMontoTotal.TabIndex = 0;
            this.labelMontoTotal.Text = "Monto total ";
            this.labelMontoTotal.Click += new System.EventHandler(this.labelMontoTotal_Click);
            // 
            // lblVuelto
            // 
            this.lblVuelto.AutoSize = true;
            this.lblVuelto.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVuelto.Location = new System.Drawing.Point(180, 167);
            this.lblVuelto.Name = "lblVuelto";
            this.lblVuelto.Size = new System.Drawing.Size(120, 39);
            this.lblVuelto.TabIndex = 1;
            this.lblVuelto.Text = "Vuelto";
            this.lblVuelto.Click += new System.EventHandler(this.lblVuelto_Click);
            // 
            // txtEfectivoIngresado
            // 
            this.txtEfectivoIngresado.Location = new System.Drawing.Point(467, 98);
            this.txtEfectivoIngresado.Multiline = true;
            this.txtEfectivoIngresado.Name = "txtEfectivoIngresado";
            this.txtEfectivoIngresado.Size = new System.Drawing.Size(185, 41);
            this.txtEfectivoIngresado.TabIndex = 2;
            this.txtEfectivoIngresado.TextChanged += new System.EventHandler(this.txtEfectivoIngresado_TextChanged);
            // 
            // buttonConf
            // 
            this.buttonConf.Location = new System.Drawing.Point(187, 338);
            this.buttonConf.Name = "buttonConf";
            this.buttonConf.Size = new System.Drawing.Size(185, 83);
            this.buttonConf.TabIndex = 3;
            this.buttonConf.Text = "Confirmar";
            this.buttonConf.UseVisualStyleBackColor = true;
            this.buttonConf.Click += new System.EventHandler(this.buttonConf_Click);
            // 
            // buttonCanc
            // 
            this.buttonCanc.Location = new System.Drawing.Point(467, 338);
            this.buttonCanc.Name = "buttonCanc";
            this.buttonCanc.Size = new System.Drawing.Size(185, 83);
            this.buttonCanc.TabIndex = 4;
            this.buttonCanc.Text = "Cancelar";
            this.buttonCanc.UseVisualStyleBackColor = true;
            this.buttonCanc.Click += new System.EventHandler(this.buttonCanc_Click);
            // 
            // labelMonto
            // 
            this.labelMonto.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMonto.AutoSize = true;
            this.labelMonto.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMonto.Location = new System.Drawing.Point(180, 98);
            this.labelMonto.Name = "labelMonto";
            this.labelMonto.Size = new System.Drawing.Size(265, 39);
            this.labelMonto.TabIndex = 5;
            this.labelMonto.Text = "Monto del pago";
            // 
            // FormCobroEfectivo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 508);
            this.Controls.Add(this.labelMonto);
            this.Controls.Add(this.buttonCanc);
            this.Controls.Add(this.buttonConf);
            this.Controls.Add(this.txtEfectivoIngresado);
            this.Controls.Add(this.lblVuelto);
            this.Controls.Add(this.labelMontoTotal);
            this.Name = "FormCobroEfectivo";
            this.Text = "FormCobroEfectivo";
            this.Load += new System.EventHandler(this.FormCobroEfectivo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMontoTotal;
        private System.Windows.Forms.Label lblVuelto;
        private System.Windows.Forms.TextBox txtEfectivoIngresado;
        private System.Windows.Forms.Button buttonConf;
        private System.Windows.Forms.Button buttonCanc;
        private System.Windows.Forms.Label labelMonto;
    }
}