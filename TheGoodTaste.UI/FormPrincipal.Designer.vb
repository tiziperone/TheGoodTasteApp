<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormPrincipal
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
        pnlContenedor = New Panel()
        pnlInicioDashboard = New Panel()
        Panel1 = New Panel()
        btnCerrarSesion = New Button()
        pnlMenuLateral = New Panel()
        btnMenuProductos = New Button()
        btnMenuClientes = New Button()
        btnMenuInicio = New Button()
        btnMenuPedidos = New Button()
        pnlContenedor.SuspendLayout()
        pnlInicioDashboard.SuspendLayout()
        pnlMenuLateral.SuspendLayout()
        SuspendLayout()
        ' 
        ' pnlContenedor
        ' 
        pnlContenedor.Controls.Add(pnlInicioDashboard)
        pnlContenedor.Dock = DockStyle.Fill
        pnlContenedor.Location = New Point(0, 0)
        pnlContenedor.Name = "pnlContenedor"
        pnlContenedor.Size = New Size(800, 450)
        pnlContenedor.TabIndex = 0
        ' 
        ' pnlInicioDashboard
        ' 
        pnlInicioDashboard.Controls.Add(Panel1)
        pnlInicioDashboard.Controls.Add(btnCerrarSesion)
        pnlInicioDashboard.Controls.Add(pnlMenuLateral)
        pnlInicioDashboard.Dock = DockStyle.Fill
        pnlInicioDashboard.Location = New Point(0, 0)
        pnlInicioDashboard.Name = "pnlInicioDashboard"
        pnlInicioDashboard.Size = New Size(800, 450)
        pnlInicioDashboard.TabIndex = 0
        ' 
        ' Panel1
        ' 
        Panel1.BackColor = Color.Silver
        Panel1.Dock = DockStyle.Right
        Panel1.Location = New Point(250, 0)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(550, 421)
        Panel1.TabIndex = 6
        ' 
        ' btnCerrarSesion
        ' 
        btnCerrarSesion.Dock = DockStyle.Bottom
        btnCerrarSesion.Location = New Point(250, 421)
        btnCerrarSesion.Name = "btnCerrarSesion"
        btnCerrarSesion.Size = New Size(550, 29)
        btnCerrarSesion.TabIndex = 5
        btnCerrarSesion.Text = "Cerra Sesion"
        btnCerrarSesion.UseVisualStyleBackColor = True
        ' 
        ' pnlMenuLateral
        ' 
        pnlMenuLateral.BackColor = SystemColors.ControlDarkDark
        pnlMenuLateral.Controls.Add(btnMenuProductos)
        pnlMenuLateral.Controls.Add(btnMenuClientes)
        pnlMenuLateral.Controls.Add(btnMenuInicio)
        pnlMenuLateral.Controls.Add(btnMenuPedidos)
        pnlMenuLateral.Dock = DockStyle.Left
        pnlMenuLateral.Location = New Point(0, 0)
        pnlMenuLateral.Name = "pnlMenuLateral"
        pnlMenuLateral.Size = New Size(250, 450)
        pnlMenuLateral.TabIndex = 0
        ' 
        ' btnMenuProductos
        ' 
        btnMenuProductos.FlatStyle = FlatStyle.Flat
        btnMenuProductos.ForeColor = SystemColors.Control
        btnMenuProductos.Location = New Point(60, 388)
        btnMenuProductos.Name = "btnMenuProductos"
        btnMenuProductos.Size = New Size(94, 29)
        btnMenuProductos.TabIndex = 2
        btnMenuProductos.Text = "Producto"
        btnMenuProductos.UseVisualStyleBackColor = True
        ' 
        ' btnMenuClientes
        ' 
        btnMenuClientes.FlatStyle = FlatStyle.Flat
        btnMenuClientes.ForeColor = SystemColors.Control
        btnMenuClientes.Location = New Point(91, 129)
        btnMenuClientes.Name = "btnMenuClientes"
        btnMenuClientes.Size = New Size(94, 29)
        btnMenuClientes.TabIndex = 4
        btnMenuClientes.Text = "Clientes"
        btnMenuClientes.UseVisualStyleBackColor = True
        ' 
        ' btnMenuInicio
        ' 
        btnMenuInicio.BackColor = Color.Yellow
        btnMenuInicio.Location = New Point(3, 198)
        btnMenuInicio.Name = "btnMenuInicio"
        btnMenuInicio.Size = New Size(94, 29)
        btnMenuInicio.TabIndex = 1
        btnMenuInicio.Text = "Inicio"
        btnMenuInicio.UseVisualStyleBackColor = False
        ' 
        ' btnMenuPedidos
        ' 
        btnMenuPedidos.FlatStyle = FlatStyle.Flat
        btnMenuPedidos.ForeColor = SystemColors.Control
        btnMenuPedidos.Location = New Point(30, 299)
        btnMenuPedidos.Name = "btnMenuPedidos"
        btnMenuPedidos.Size = New Size(135, 39)
        btnMenuPedidos.TabIndex = 3
        btnMenuPedidos.Text = "Punto de Venta"
        btnMenuPedidos.UseVisualStyleBackColor = True
        ' 
        ' FormPrincipal
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(pnlContenedor)
        Name = "FormPrincipal"
        Text = "FormPrincipal"
        pnlContenedor.ResumeLayout(False)
        pnlInicioDashboard.ResumeLayout(False)
        pnlMenuLateral.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlContenedor As Panel
    Friend WithEvents pnlInicioDashboard As Panel
    Friend WithEvents pnlMenuLateral As Panel
    Friend WithEvents btnCerrarSesion As Button
    Friend WithEvents btnMenuClientes As Button
    Friend WithEvents btnMenuPedidos As Button
    Friend WithEvents btnMenuProductos As Button
    Friend WithEvents btnMenuInicio As Button
    Friend WithEvents Panel1 As Panel
End Class
