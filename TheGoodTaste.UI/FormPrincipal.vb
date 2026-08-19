Imports System.Drawing
Imports System.Windows.Forms
Imports The_Good_Taste.Entidades
Imports The_Good_Taste.Datos

Public Class FormPrincipal
    ' Variable para controlar qué formulario hijo está visible en el panel
    Private formularioActivo As Form = Nothing

    Private Sub FormPrincipal_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "The Good Taste - Sistema de Gestión"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.WindowState = FormWindowState.Maximized
    End Sub

    ''' <summary>
    ''' Incrusta cualquier formulario hijo dentro del panel central
    ''' </summary>
    Public Sub AbrirFormularioEnPanel(formularioHijo As Form)
        ' Si ya hay un formulario abierto, lo cerramos
        If formularioActivo IsNot Nothing Then
            formularioActivo.Close()
        End If

        formularioActivo = formularioHijo
        formularioHijo.TopLevel = False
        formularioHijo.FormBorderStyle = FormBorderStyle.None
        formularioHijo.Dock = DockStyle.Fill

        ' Limpia el contenedor y agrega la nueva ventana
        pnlContenedor.Controls.Clear()
        pnlContenedor.Controls.Add(formularioHijo)
        formularioHijo.Show()
    End Sub

    ' Botón Inicio
    Private Sub btnMenuInicio_Click(sender As Object, e As EventArgs) Handles btnMenuInicio.Click
        If formularioActivo IsNot Nothing Then
            formularioActivo.Close()
            formularioActivo = Nothing
        End If
        pnlContenedor.Controls.Clear()
    End Sub

    ' Botón Productos
    Private Sub btnMenuProductos_Click(sender As Object, e As EventArgs) Handles btnMenuProductos.Click
        AbrirFormularioEnPanel(New FormProductosVB())
    End Sub

    ' Botón Punto de Venta / Pedidos
    Private Sub btnMenuPedidos_Click(sender As Object, e As EventArgs) Handles btnMenuPedidos.Click
        AbrirFormularioEnPanel(New FormPuntoVentaVB())
    End Sub

    ' Botón Salir
    Private Sub btnCerrarSesion_Click(sender As Object, e As EventArgs) Handles btnCerrarSesion.Click
        Dim respuesta = MessageBox.Show("¿Desea cerrar el sistema?", "Salir", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If respuesta = DialogResult.Yes Then
            Application.Exit()
        End If
    End Sub

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint

    End Sub
End Class