Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Text
Imports RestSharp

Public Class Form1
    Private apiUrl As String = "https://inventarioapi-f3c6fvgsh0f2aueq.eastus2-01.azurewebsites.net/api/Proovedores"

    ' Declaración de controles
    Private WithEvents btnGuardar As Button
    Private WithEvents btnActualizar As Button
    Private WithEvents btnEliminar As Button
    Private WithEvents btnLimpiar As Button
    Private WithEvents dgvProveedores As DataGridView

    Private txtId As TextBox
    Private txtNombre As TextBox
    Private txtContacto As TextBox
    Private txtDireccion As TextBox

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CargarProveedores()
    End Sub

    Private Sub CargarProveedores()
        Try
            Dim api = New Provedoresapi()
            Dim headers As New List(Of Parametro) From {
                New Parametro("Content-Type", "application/json")
            }

            Dim response = api.Obtener(apiUrl, headers, New List(Of Parametro))
            Dim proveedores = JsonConvert.DeserializeObject(Of List(Of Proveedor))(response)

            dgvProveedores.DataSource = proveedores
            dgvProveedores.ClearSelection()
        Catch ex As Exception
            MessageBox.Show($"Error al cargar proveedores: {ex.Message}")
        End Try
    End Sub

    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If ValidarCampos() Then
            Try
                Dim api = New Provedoresapi()
                Dim headers As New List(Of Parametro) From {
                    New Parametro("Content-Type", "application/json")
                }

                Dim nuevoProveedor As New Proveedor With {
                    .nombre = txtNombre.Text,
                    .contacto = txtContacto.Text,
                    .direccion = txtDireccion.Text
                }

                Dim response = api.Post(apiUrl, headers, New List(Of Parametro), nuevoProveedor)
                MessageBox.Show("Proveedor creado exitosamente")
                CargarProveedores()
                LimpiarCampos()
            Catch ex As Exception
                MessageBox.Show($"Error al crear proveedor: {ex.Message}")
            End Try
        End If
    End Sub

    Private Sub btnActualizar_Click(sender As Object, e As EventArgs) Handles btnActualizar.Click
        If ValidarCampos() Then
            Try
                If String.IsNullOrEmpty(txtId.Text) Then
                    MessageBox.Show("Seleccione un proveedor de la lista primero")
                    Return
                End If

                Dim api = New Provedoresapi()
                Dim headers As New List(Of Parametro) From {
                    New Parametro("Content-Type", "application/json")
                }

                Dim proveedorActualizado As New Proveedor With {
                    .proovedorId = Integer.Parse(txtId.Text),
                    .nombre = txtNombre.Text,
                    .contacto = txtContacto.Text,
                    .direccion = txtDireccion.Text
                }

                Dim response = api.Put($"{apiUrl}/{txtId.Text}", headers, New List(Of Parametro), proveedorActualizado)
                MessageBox.Show("Proveedor actualizado exitosamente")
                CargarProveedores()
                LimpiarCampos()
            Catch ex As Exception
                MessageBox.Show($"Error al actualizar proveedor: {ex.Message}")
            End Try
        End If
    End Sub

    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        If Not String.IsNullOrEmpty(txtId.Text) Then
            Try
                Dim confirmar = MessageBox.Show("¿Está seguro de eliminar este proveedor?", "Confirmar", MessageBoxButtons.YesNo)
                If confirmar = DialogResult.Yes Then
                    Dim api = New Provedoresapi()
                    Dim headers As New List(Of Parametro) From {
                        New Parametro("Content-Type", "application/json")
                    }

                    Dim response = api.Delete($"{apiUrl}/{txtId.Text}", headers, New List(Of Parametro))
                    MessageBox.Show("Proveedor eliminado exitosamente")
                    CargarProveedores()
                    LimpiarCampos()
                End If
            Catch ex As Exception
                MessageBox.Show($"Error al eliminar proveedor: {ex.Message}")
            End Try
        Else
            MessageBox.Show("Seleccione un proveedor de la lista primero")
        End If
    End Sub

    Private Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        LimpiarCampos()
    End Sub

    Private Sub dgvProveedores_SelectionChanged(sender As Object, e As EventArgs) Handles dgvProveedores.SelectionChanged
        Try
            If dgvProveedores.SelectedRows.Count > 0 Then
                Dim selectedRow = dgvProveedores.SelectedRows(0)
                If selectedRow IsNot Nothing AndAlso Not selectedRow.IsNewRow Then
                    txtId.Text = If(selectedRow.Cells("proovedorId").Value?.ToString(), "")
                    txtNombre.Text = If(selectedRow.Cells("nombre").Value?.ToString(), "")
                    txtContacto.Text = If(selectedRow.Cells("contacto").Value?.ToString(), "")
                    txtDireccion.Text = If(selectedRow.Cells("direccion").Value?.ToString(), "")
                End If
            End If
        Catch ex As Exception
            MessageBox.Show($"Error al cargar datos: {ex.Message}")
        End Try
    End Sub

    Private Function ValidarCampos() As Boolean
        If String.IsNullOrWhiteSpace(txtNombre.Text) OrElse
           String.IsNullOrWhiteSpace(txtContacto.Text) OrElse
           String.IsNullOrWhiteSpace(txtDireccion.Text) Then
            MessageBox.Show("Todos los campos son requeridos")
            Return False
        End If
        Return True
    End Function

    Private Sub LimpiarCampos()
        txtId.Clear()
        txtNombre.Clear()
        txtContacto.Clear()
        txtDireccion.Clear()
        dgvProveedores.ClearSelection()
    End Sub

    Private Sub InitializeComponent()
        ' Configuración de controles
        Me.txtId = New TextBox With {
            .Location = New Point(20, 20),
            .Size = New Size(100, 20),
            .ReadOnly = True
        }
        Me.txtNombre = New TextBox With {
            .Location = New Point(20, 60),
            .Size = New Size(200, 20)
        }
        Me.txtContacto = New TextBox With {
            .Location = New Point(20, 100),
            .Size = New Size(200, 20)
        }
        Me.txtDireccion = New TextBox With {
            .Location = New Point(20, 140),
            .Size = New Size(200, 20)
        }

        Me.dgvProveedores = New DataGridView With {
            .Location = New Point(250, 20),
            .Size = New Size(450, 200),
            .AllowUserToAddRows = False,
            .ReadOnly = True,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        }

        Me.btnGuardar = New Button With {
            .Text = "Guardar",
            .Location = New Point(20, 180),
            .Size = New Size(80, 30)
        }
        Me.btnActualizar = New Button With {
            .Text = "Actualizar",
            .Location = New Point(110, 180),
            .Size = New Size(80, 30)
        }
        Me.btnEliminar = New Button With {
            .Text = "Eliminar",
            .Location = New Point(200, 180),
            .Size = New Size(80, 30)
        }
        Me.btnLimpiar = New Button With {
            .Text = "Limpiar",
            .Location = New Point(20, 220),
            .Size = New Size(80, 30)
        }

        Me.Controls.AddRange({txtId, txtNombre, txtContacto, txtDireccion, dgvProveedores,
                            btnGuardar, btnActualizar, btnEliminar, btnLimpiar})

        Me.ClientSize = New Size(720, 300)
        Me.Text = "Gestión de Proveedores"
    End Sub
End Class

Public Class Proveedor
    Public Property proovedorId As Integer
    Public Property nombre As String
    Public Property contacto As String
    Public Property direccion As String
End Class

Public Class Parametro
    Public Property Clave As String
    Public Property Valor As String

    Public Sub New()
    End Sub

    Public Sub New(_clave As String, _valor As String)
        Clave = _clave
        Valor = _valor
    End Sub
End Class

Public Class Provedoresapi
    Public Function Obtener(url As String, headers As List(Of Parametro), parametros As List(Of Parametro)) As String
        Return EjecutarSolicitud(url, Method.GET, headers, parametros, Nothing)
    End Function

    Public Function Post(url As String, headers As List(Of Parametro), parametros As List(Of Parametro), objeto As Object) As String
        Return EjecutarSolicitud(url, Method.POST, headers, parametros, objeto)
    End Function

    Public Function Put(url As String, headers As List(Of Parametro), parametros As List(Of Parametro), objeto As Object) As String
        Return EjecutarSolicitud(url, Method.PUT, headers, parametros, objeto)
    End Function

    Public Function Delete(url As String, headers As List(Of Parametro), parametros As List(Of Parametro)) As String
        Return EjecutarSolicitud(url, Method.DELETE, headers, parametros, Nothing)
    End Function

    Private Function EjecutarSolicitud(url As String, metodo As Method, headers As List(Of Parametro),
                                      parametros As List(Of Parametro), objeto As Object) As String
        Try
            Dim client = New RestClient(url)
            Dim request = New RestRequest() With {.Method = metodo}

            ' Agregar headers
            For Each header In headers
                request.AddHeader(header.Clave, header.Valor)
            Next

            ' Agregar parámetros
            For Each param In parametros
                request.AddParameter(param.Clave, param.Valor)
            Next

            ' Agregar cuerpo si existe
            If objeto IsNot Nothing Then
                request.AddJsonBody(objeto)
            End If

            Dim response = client.Execute(request)
            If response.IsSuccessful Then
                Return response.Content
            Else
                Throw New Exception($"Error: {response.StatusCode} - {response.ErrorMessage}")
            End If
        Catch ex As Exception
            Throw New Exception($"Error en la solicitud: {ex.Message}")
        End Try
    End Function
End Class