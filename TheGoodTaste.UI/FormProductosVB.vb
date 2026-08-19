Imports System.Drawing
Imports System.Windows.Forms
Imports The_Good_Taste.Datos
Imports The_Good_Taste.Entidades

Public Class FormProductosVB
    Public Sub New()
        InitializeComponent()
        ConfigurarEstiloGrilla()
    End Sub

    Private Sub FormProductosVB_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CargarListaProductos()
    End Sub

    Private Sub ConfigurarEstiloGrilla()
        dgvProductos.BackgroundColor = Color.FromArgb(33, 37, 41)
        dgvProductos.ForeColor = Color.Black
        dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvProductos.MultiSelect = False
        dgvProductos.ReadOnly = True
    End Sub

    Public Sub CargarListaProductos()
        Try
            ' Consume directamente el método estático desarrollado en C#
            Dim lista = ProductoDatos.ObtenerActivos()
            dgvProductos.DataSource = Nothing
            dgvProductos.DataSource = lista

            ' Ocultar columnas internas si no se desean ver
            If dgvProductos.Columns.Contains("DeletedAt") Then
                dgvProductos.Columns("DeletedAt").Visible = False
            End If
        Catch ex As Exception
            MessageBox.Show("Error al conectar con la base de datos: " & ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Formato condicional para alertar Stock Bajo
    Private Sub dgvProductos_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvProductos.CellFormatting
        If dgvProductos.Columns(e.ColumnIndex).Name = "Stock" AndAlso e.Value IsNot Nothing Then
            Dim stockActual As Integer = Convert.ToInt32(e.Value)
            If stockActual <= 0 Then
                e.CellStyle.BackColor = Color.IndianRed
                e.CellStyle.ForeColor = Color.White
            ElseIf stockActual <= 5 Then
                e.CellStyle.BackColor = Color.Orange
                e.CellStyle.ForeColor = Color.Black
            End If
        End If
    End Sub
End Class