Imports System.Drawing
Imports System.Windows.Forms
Imports The_Good_Taste.Datos
Imports The_Good_Taste.Entidades

Public Class FormProductosVB
    Public Sub New()
        InitializeComponent()
        ConfigurarDiseno()
    End Sub

    Private Sub FormProductosVB_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CargarProductos()
    End Sub

    Private Sub ConfigurarDiseno()
        Me.BackColor = Color.FromArgb(33, 37, 41)
        Me.ForeColor = Color.White

        dgvProductos.BackgroundColor = Color.FromArgb(45, 50, 56)
        dgvProductos.ForeColor = Color.Black
        dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvProductos.MultiSelect = False
        dgvProductos.ReadOnly = True
    End Sub

    Public Sub CargarProductos()
        Try
            ' Invoca el método estático en C# dentro de ProductoDatos
            Dim productos = ProductoDatos.ObtenerActivos()
            dgvProductos.DataSource = Nothing
            dgvProductos.DataSource = productos
        Catch ex As Exception
            MessageBox.Show("Error al cargar productos: " & ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnRecargar_Click(sender As Object, e As EventArgs) Handles btnRecargar.Click
        CargarProductos()
    End Sub

    ' Alerta visual de stock (igual a los badges de la web)
    Private Sub dgvProductos_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvProductos.CellFormatting
        If dgvProductos.Columns(e.ColumnIndex).Name = "Stock" AndAlso e.Value IsNot Nothing Then
            Dim stock As Integer = Convert.ToInt32(e.Value)
            If stock <= 0 Then
                e.CellStyle.BackColor = Color.IndianRed
                e.CellStyle.ForeColor = Color.White
            ElseIf stock <= 5 Then
                e.CellStyle.BackColor = Color.Orange
                e.CellStyle.ForeColor = Color.Black
            End If
        End If
    End Sub
End Class