Imports System.Runtime.Serialization
Imports System 
Public Class TopException
    Inherits Exception

    Private errorCode1 As String
    Private errorMsg1 As String
    Private subErrorCode1 As String
    Private subErrorMsg1 As String

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
        MyBase.New(info, context)
    End Sub

    Public Sub New(ByVal message As String, ByVal innerException As Exception)
        MyBase.New(message, innerException)
    End Sub

    Public Sub New(ByVal errorCode As String, ByVal errorMsg As String)
        MyBase.New(errorCode & ":" & errorMsg)
        Me.errorCode1 = errorCode
        Me.errorMsg1 = errorMsg
    End Sub

    Public Sub New(ByVal errorCode As String, ByVal errorMsg As String, ByVal subErrorCode As String, ByVal subErrorMsg As String)
        MyBase.New(errorCode & ":" & errorMsg & ":" & subErrorCode & ":" & subErrorMsg)
        Me.errorCode1 = errorCode
        Me.errorMsg1 = errorMsg
        Me.subErrorCode1 = subErrorCode
        Me.subErrorMsg1 = subErrorMsg
    End Sub

    Public ReadOnly Property ErrorCode As String
        Get
            Return Me.errorCode
        End Get
    End Property

    Public ReadOnly Property ErrorMsg As String
        Get
            Return Me.errorMsg
        End Get
    End Property

    Public ReadOnly Property SubErrorCode As String
        Get
            Return Me.subErrorCode
        End Get
    End Property

    Public ReadOnly Property SubErrorMsg As String
        Get
            Return Me.subErrorMsg
        End Get
    End Property
End Class

