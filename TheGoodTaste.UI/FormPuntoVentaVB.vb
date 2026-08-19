Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports The_Good_Taste.Datos
Imports The_Good_Taste.Entidades

Public Class FormPuntoVentaVB
    Private clienteSeleccionado As Cliente = Nothing
    Private carrito As New List(Of VentaDetalle)()

    Public Sub New()
        InitializeComponent()
        Me.BackColor = Color.FromArgb(33, 37, 41)
        Me.ForeColor = Color.White
    End Sub

    Private Sub FormPuntoVentaVB_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CargarComboProductos()
    End Sub

    Private Sub CargarComboProductos()
        Try
            Dim listaProductos = ProductoDatos.ObtenerActivos()
            cboProductos.DataSource = listaProductos
            cboProductos.DisplayMember = "Nombre"
            cboProductos.ValueMember = "IdProducto"
        Catch ex As Exception
            MessageBox.Show("Error al cargar lista de productos: " & ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    Private Sub btnBuscarCliente_Click(sender As Object, e As EventArgs) Handles btnBuscarCliente.Click
        Dim dni As String = txtDniBusqueda.Text.Trim()
        If String.IsNullOrEmpty(dni) Then Return

        Try
            clienteSeleccionado = ClienteDatos.ObtenerPorDni(dni)
            If clienteSeleccionado IsNot Nothing Then
                lblNombreCliente.Text = $"{clienteSeleccionado.Apellido}, {clienteSeleccionado.Nombre}"
            Else
                MessageBox.Show("Cliente no encontrado.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information)
                lblNombreCliente.Text = "No seleccionado"
            End If
        Catch ex As Exception
            MessageBox.Show("Error en la búsqueda: " & ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
        If cboProductos.SelectedItem Is Nothing Then Return

        Dim prod As Producto = DirectCast(cboProductos.SelectedItem, Producto)
        Dim cant As Integer = CInt(nudCantidad.Value)

        Dim detalle As New VentaDetalle() With {
            .IdProducto = prod.IdProducto,
            .Producto = prod,
            .Cantidad = cant,
            .PrecioUnitario = prod.Precio
        }

        carrito.Add(detalle)
        ActualizarCarrito()
    End Sub

    Private Sub ActualizarCarrito()
        dgvCarrito.DataSource = Nothing
        dgvCarrito.DataSource = carrito

        Dim total As Decimal = 0
        For Each item In carrito
            total += item.Subtotal
        Next
        lblTotal.Text = $"$ {total:N2}"
    End Sub

    Private Sub btnConfirmarVenta_Click(sender As Object, e As EventArgs) Handles btnConfirmarVenta.Click
        If clienteSeleccionado Is Nothing Then
            MessageBox.Show("Debe seleccionar un cliente antes de registrar la venta.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If carrito.Count = 0 Then
            MessageBox.Show("El carrito está vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Dim venta As New Venta() With {
                .Fecha = DateTime.Now,
                .IdCliente = clienteSeleccionado.IdCliente,
                .Total = Convert.ToDecimal(lblTotal.Text.Replace("$ ", "")),
                .Detalles = carrito
            }

            If VentaDatos.RegistrarVenta(venta) Then
                MessageBox.Show("Venta registrada y stock actualizado con éxito.", "Venta Completada", MessageBoxButtons.OK, MessageBoxIcon.Information)
                carrito.Clear()
                ActualizarCarrito()
            End If
        Catch ex As Exception
            MessageBox.Show("Error al guardar la venta: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class