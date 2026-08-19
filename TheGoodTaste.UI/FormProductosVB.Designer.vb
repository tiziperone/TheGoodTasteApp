<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormProductosVB
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
        dgvProductos = New DataGridView()
        btnNuevoProducto = New Button()
        CType(dgvProductos, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' dgvProductos
        ' 
        dgvProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvProductos.Location = New Point(235, 138)
        dgvProductos.Name = "dgvProductos"
        dgvProductos.RowHeadersWidth = 51
        dgvProductos.Size = New Size(300, 188)
        dgvProductos.TabIndex = 0
        ' 
        ' btnNuevoProducto
        ' 
        btnNuevoProducto.Location = New Point(309, 209)
        btnNuevoProducto.Name = "btnNuevoProducto"
        btnNuevoProducto.Size = New Size(152, 50)
        btnNuevoProducto.TabIndex = 1
        btnNuevoProducto.Text = "Nuevo Producto"
        btnNuevoProducto.UseVisualStyleBackColor = True
        ' 
        ' FormProductosVB
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(btnNuevoProducto)
        Controls.Add(dgvProductos)
        Name = "FormProductosVB"
        Text = "FormProductosVB"
        CType(dgvProductos, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents dgvProductos As DataGridView
    Friend WithEvents btnNuevoProducto As Button
End Class
