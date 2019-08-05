Imports System.IO

Friend Class StreamContract
    Private fileName As String
    Private stream As Stream
    Private mimeType As String

    Public Sub New(ByVal fileName As String, ByVal stream As Stream, ByVal mimeType As String)
        Me.fileName = fileName
        Me.stream = stream
        Me.mimeType = mimeType
    End Sub

    Public Function GetFileLength() As Long
        Return 0L
    End Function

    Public Function GetFileName() As String
        Return Me.fileName
    End Function

    Public Function GetMimeType() As String
        If String.IsNullOrEmpty(mimeType) Then
            Return Constants.CTYPE_DEFAULT
        Else
            Return Me.mimeType
        End If
    End Function

    Public Function IsValid() As Boolean
        Return Me.stream IsNot Nothing AndAlso Me.fileName IsNot Nothing
    End Function

    Public Sub Write(ByVal output As Stream)
        Using Me.stream
            Dim n As Integer = 0
            Dim buffer As Byte() = New Byte(Constants.READ_BUFFER_SIZE - 1) {}

            While (CSharpImpl.__Assign(n, Me.stream.Read(buffer, 0, buffer.Length))) > 0
                output.Write(buffer, 0, n)
            End While
        End Using
    End Sub

    Private Class CSharpImpl
        <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
        Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class
End Class