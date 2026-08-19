<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormPuntoVentaVB
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        txtDniBusqueda = New TextBox()
        btnBuscarCliente = New Button()
        lblNombreCliente = New Label()
        cboProductos = New ComboBox()
        nudCantidad = New NumericUpDown()
        btnAgregar = New Button()
        dgvCarrito = New DataGridView()
        lblTotal = New Label()
        btnConfirmarVenta = New Button()
        CType(nudCantidad, ComponentModel.ISupportInitialize).BeginInit()
        CType(dgvCarrito, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' txtDniBusqueda
        ' 
        txtDniBusqueda.Location = New Point(616, 108)
        txtDniBusqueda.Name = "txtDniBusqueda"
        txtDniBusqueda.Size = New Size(125, 27)
        txtDniBusqueda.TabIndex = 0
        ' 
        ' btnBuscarCliente
        ' 
        btnBuscarCliente.Location = New Point(313, 247)
        btnBuscarCliente.Name = "btnBuscarCliente"
        btnBuscarCliente.Size = New Size(191, 63)
        btnBuscarCliente.TabIndex = 1
        btnBuscarCliente.Text = "Buscar Cliente"
        btnBuscarCliente.UseVisualStyleBackColor = True
        ' 
        ' lblNombreCliente
        ' 
        lblNombreCliente.AutoSize = True
        lblNombreCliente.Location = New Point(236, 367)
        lblNombreCliente.Name = "lblNombreCliente"
        lblNombreCliente.Size = New Size(53, 20)
        lblNombreCliente.TabIndex = 2
        lblNombreCliente.Text = "Label1"
        ' 
        ' cboProductos
        ' 
        cboProductos.FormattingEnabled = True
        cboProductos.Location = New Point(600, 40)
        cboProductos.Name = "cboProductos"
        cboProductos.Size = New Size(151, 28)
        cboProductos.TabIndex = 3
        ' 
        ' nudCantidad
        ' 
        nudCantidad.Location = New Point(117, 259)
        nudCantidad.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        nudCantidad.Name = "nudCantidad"
        nudCantidad.Size = New Size(83, 27)
        nudCantidad.TabIndex = 4
        nudCantidad.Value = New Decimal(New Integer() {1, 0, 0, 0})
        ' 
        ' btnAgregar
        ' 
        btnAgregar.Location = New Point(425, 349)
        btnAgregar.Name = "btnAgregar"
        btnAgregar.Size = New Size(94, 29)
        btnAgregar.TabIndex = 5
        btnAgregar.Text = "Agregar"
        btnAgregar.UseVisualStyleBackColor = True
        ' 
        ' dgvCarrito
        ' 
        dgvCarrito.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvCarrito.Location = New Point(12, 12)
        dgvCarrito.Name = "dgvCarrito"
        dgvCarrito.RowHeadersWidth = 51
        dgvCarrito.Size = New Size(300, 188)
        dgvCarrito.TabIndex = 6
        ' 
        ' lblTotal
        ' 
        lblTotal.AutoSize = True
        lblTotal.Location = New Point(677, 180)
        lblTotal.Name = "lblTotal"
        lblTotal.Size = New Size(53, 20)
        lblTotal.TabIndex = 7
        lblTotal.Text = "Label1"
        ' 
        ' btnConfirmarVenta
        ' 
        btnConfirmarVenta.Location = New Point(413, 40)
        btnConfirmarVenta.Name = "btnConfirmarVenta"
        btnConfirmarVenta.Size = New Size(115, 70)
        btnConfirmarVenta.TabIndex = 8
        btnConfirmarVenta.Text = "Confirmar"
        btnConfirmarVenta.UseVisualStyleBackColor = True
        ' 
        ' FormPuntoVentaVB
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(btnConfirmarVenta)
        Controls.Add(lblTotal)
        Controls.Add(dgvCarrito)
        Controls.Add(btnAgregar)
        Controls.Add(nudCantidad)
        Controls.Add(cboProductos)
        Controls.Add(lblNombreCliente)
        Controls.Add(btnBuscarCliente)
        Controls.Add(txtDniBusqueda)
        Name = "FormPuntoVentaVB"
        Text = "FormPuntoVentaVB"
        CType(nudCantidad, ComponentModel.ISupportInitialize).EndInit()
        CType(dgvCarrito, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents txtDniBusqueda As TextBox
    Friend WithEvents btnBuscarCliente As Button
    Friend WithEvents lblNombreCliente As Label
    Friend WithEvents cboProductos As ComboBox
    Friend WithEvents nudCantidad As NumericUpDown
    Friend WithEvents btnAgregar As Button
    Friend WithEvents dgvCarrito As DataGridView
    Friend WithEvents lblTotal As Label
    Friend WithEvents btnConfirmarVenta As Button
End Class
