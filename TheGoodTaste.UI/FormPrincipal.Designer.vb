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
        pnlMenuLateral = New Panel()
        pnlContenedor.SuspendLayout()
        pnlInicioDashboard.SuspendLayout()
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
        pnlInicioDashboard.Controls.Add(pnlMenuLateral)
        pnlInicioDashboard.Dock = DockStyle.Fill
        pnlInicioDashboard.Location = New Point(0, 0)
        pnlInicioDashboard.Name = "pnlInicioDashboard"
        pnlInicioDashboard.Size = New Size(800, 450)
        pnlInicioDashboard.TabIndex = 0
        ' 
        ' pnlMenuLateral
        ' 
        pnlMenuLateral.BackColor = SystemColors.ControlDarkDark
        pnlMenuLateral.Dock = DockStyle.Left
        pnlMenuLateral.Location = New Point(0, 0)
        pnlMenuLateral.Name = "pnlMenuLateral"
        pnlMenuLateral.Size = New Size(250, 450)
        pnlMenuLateral.TabIndex = 0
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
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlContenedor As Panel
    Friend WithEvents pnlInicioDashboard As Panel
    Friend WithEvents pnlMenuLateral As Panel
End Class
