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
        SuspendLayout()
        ' 
        ' txtDniBusqueda
        ' 
        txtDniBusqueda.Location = New Point(241, 160)
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
        ' FormPuntoVentaVB
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(lblNombreCliente)
        Controls.Add(btnBuscarCliente)
        Controls.Add(txtDniBusqueda)
        Name = "FormPuntoVentaVB"
        Text = "FormPuntoVentaVB"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents txtDniBusqueda As TextBox
    Friend WithEvents btnBuscarCliente As Button
    Friend WithEvents lblNombreCliente As Label
End Class
