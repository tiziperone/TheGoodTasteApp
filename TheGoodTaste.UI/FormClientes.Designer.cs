namespace TheGoodTaste.UI
{
    partial class FormClientes
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
            this.txtDni = new System.Windows.Forms.TextBox();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.txtApellido = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.txtCalle = new System.Windows.Forms.TextBox();
            this.txtProvincia = new System.Windows.Forms.TextBox();
            this.dtpFechaNacimiento = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.dgvClientes = new System.Windows.Forms.DataGridView();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textPais = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textLocalidad = new System.Windows.Forms.TextBox();
            this.textNroAltura = new System.Windows.Forms.TextBox();
            this.btnActivo = new System.Windows.Forms.Button();
            this.btnInactivo = new System.Windows.Forms.Button();
            this.textBoxBuscarCliente = new System.Windows.Forms.TextBox();
            this.labelBuscarClient = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).BeginInit();
            this.SuspendLayout();
            // 
            // txtDni
            // 
            this.txtDni.Location = new System.Drawing.Point(196, 82);
            this.txtDni.Name = "txtDni";
            this.txtDni.Size = new System.Drawing.Size(135, 22);
            this.txtDni.TabIndex = 0;
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(196, 131);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(135, 22);
            this.txtNombre.TabIndex = 1;
            // 
            // txtApellido
            // 
            this.txtApellido.Location = new System.Drawing.Point(196, 179);
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.Size = new System.Drawing.Size(135, 22);
            this.txtApellido.TabIndex = 2;
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(196, 261);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(135, 22);
            this.txtEmail.TabIndex = 3;
            // 
            // txtTelefono
            // 
            this.txtTelefono.Location = new System.Drawing.Point(196, 562);
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(135, 22);
            this.txtTelefono.TabIndex = 4;
            // 
            // txtCalle
            // 
            this.txtCalle.Location = new System.Drawing.Point(196, 461);
            this.txtCalle.Name = "txtCalle";
            this.txtCalle.Size = new System.Drawing.Size(135, 22);
            this.txtCalle.TabIndex = 5;
            // 
            // txtProvincia
            // 
            this.txtProvincia.Location = new System.Drawing.Point(196, 409);
            this.txtProvincia.Name = "txtProvincia";
            this.txtProvincia.Size = new System.Drawing.Size(135, 22);
            this.txtProvincia.TabIndex = 6;
            // 
            // dtpFechaNacimiento
            // 
            this.dtpFechaNacimiento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaNacimiento.Location = new System.Drawing.Point(196, 220);
            this.dtpFechaNacimiento.Name = "dtpFechaNacimiento";
            this.dtpFechaNacimiento.Size = new System.Drawing.Size(135, 22);
            this.dtpFechaNacimiento.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(67, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 16);
            this.label1.TabIndex = 9;
            this.label1.Text = "DNI: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(67, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Nombre:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(67, 179);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "Apellido:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(67, 225);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 16);
            this.label4.TabIndex = 12;
            this.label4.Text = "Fecha Nacimiento:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(67, 261);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 16);
            this.label5.TabIndex = 13;
            this.label5.Text = "Email:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(67, 568);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 16);
            this.label6.TabIndex = 14;
            this.label6.Text = "Teléfono:";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(67, 467);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 16);
            this.label7.TabIndex = 15;
            this.label7.Text = "Calle:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(67, 518);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(58, 16);
            this.label8.TabIndex = 16;
            this.label8.Text = "Número:";
            // 
            // dgvClientes
            // 
            this.dgvClientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClientes.Location = new System.Drawing.Point(458, 85);
            this.dgvClientes.Name = "dgvClientes";
            this.dgvClientes.ReadOnly = true;
            this.dgvClientes.RowHeadersWidth = 51;
            this.dgvClientes.RowTemplate.Height = 24;
            this.dgvClientes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvClientes.Size = new System.Drawing.Size(866, 246);
            this.dgvClientes.TabIndex = 18;
            this.dgvClientes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvClientes_CellContentClick);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(70, 615);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(109, 52);
            this.btnGuardar.TabIndex = 19;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(225, 615);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(106, 52);
            this.btnLimpiar.TabIndex = 22;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(67, 315);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(37, 16);
            this.label10.TabIndex = 23;
            this.label10.Text = "Pais:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(67, 415);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 16);
            this.label11.TabIndex = 24;
            this.label11.Text = "Provincia";
            // 
            // textPais
            // 
            this.textPais.Location = new System.Drawing.Point(196, 309);
            this.textPais.Name = "textPais";
            this.textPais.Size = new System.Drawing.Size(135, 22);
            this.textPais.TabIndex = 25;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(67, 362);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(67, 16);
            this.label12.TabIndex = 26;
            this.label12.Text = "Localidad";
            // 
            // textLocalidad
            // 
            this.textLocalidad.Location = new System.Drawing.Point(196, 359);
            this.textLocalidad.Name = "textLocalidad";
            this.textLocalidad.Size = new System.Drawing.Size(135, 22);
            this.textLocalidad.TabIndex = 27;
            // 
            // textNroAltura
            // 
            this.textNroAltura.Location = new System.Drawing.Point(196, 512);
            this.textNroAltura.Name = "textNroAltura";
            this.textNroAltura.Size = new System.Drawing.Size(135, 22);
            this.textNroAltura.TabIndex = 28;
            // 
            // btnActivo
            // 
            this.btnActivo.Location = new System.Drawing.Point(458, 46);
            this.btnActivo.Name = "btnActivo";
            this.btnActivo.Size = new System.Drawing.Size(91, 33);
            this.btnActivo.TabIndex = 29;
            this.btnActivo.Text = "Activo";
            this.btnActivo.UseVisualStyleBackColor = true;
            // 
            // btnInactivo
            // 
            this.btnInactivo.Location = new System.Drawing.Point(566, 46);
            this.btnInactivo.Name = "btnInactivo";
            this.btnInactivo.Size = new System.Drawing.Size(86, 33);
            this.btnInactivo.TabIndex = 30;
            this.btnInactivo.Text = "Inactivo";
            this.btnInactivo.UseVisualStyleBackColor = true;
            // 
            // textBoxBuscarCliente
            // 
            this.textBoxBuscarCliente.Location = new System.Drawing.Point(1044, 57);
            this.textBoxBuscarCliente.Name = "textBoxBuscarCliente";
            this.textBoxBuscarCliente.Size = new System.Drawing.Size(280, 22);
            this.textBoxBuscarCliente.TabIndex = 34;
            this.textBoxBuscarCliente.TextChanged += new System.EventHandler(this.textBoxBuscarCliente_TextChanged);
            // 
            // labelBuscarClient
            // 
            this.labelBuscarClient.AutoSize = true;
            this.labelBuscarClient.Location = new System.Drawing.Point(970, 60);
            this.labelBuscarClient.Name = "labelBuscarClient";
            this.labelBuscarClient.Size = new System.Drawing.Size(52, 16);
            this.labelBuscarClient.TabIndex = 35;
            this.labelBuscarClient.Text = "Buscar:";
            // 
            // FormClientes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1458, 825);
            this.Controls.Add(this.labelBuscarClient);
            this.Controls.Add(this.textBoxBuscarCliente);
            this.Controls.Add(this.btnInactivo);
            this.Controls.Add(this.btnActivo);
            this.Controls.Add(this.textNroAltura);
            this.Controls.Add(this.textLocalidad);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.textPais);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.dgvClientes);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpFechaNacimiento);
            this.Controls.Add(this.txtProvincia);
            this.Controls.Add(this.txtCalle);
            this.Controls.Add(this.txtTelefono);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.txtApellido);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.txtDni);
            this.Name = "FormClientes";
            this.Text = "FormClientes";
            this.Load += new System.EventHandler(this.FormClientes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDni;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.TextBox txtApellido;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.TextBox txtCalle;
        private System.Windows.Forms.TextBox txtProvincia;
        private System.Windows.Forms.DateTimePicker dtpFechaNacimiento;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridView dgvClientes;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textPais;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textLocalidad;
        private System.Windows.Forms.TextBox textNroAltura;
        private System.Windows.Forms.Button btnActivo;
        private System.Windows.Forms.Button btnInactivo;
        private System.Windows.Forms.TextBox textBoxBuscarCliente;
        private System.Windows.Forms.Label labelBuscarClient;
    }
}