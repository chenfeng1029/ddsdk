Imports System.Xml.Serialization

Namespace Response
    ''' <summary>
    ''' 2019.8.1
    ''' 钉钉回应类，需要子类集成重写
    ''' </summary>
    Public MustInherit Class TopResponse
        <XmlElement("code")>
        Public Property ErrCode As String
        <XmlElement("msg")>
        Public Property ErrMsg As String
        <XmlElement("sub_code")>
        Public Property SubErrCode As String
        <XmlElement("sub_msg")>
        Public Property SubErrMsg As String
        <XmlElement("request_id")>
        Public Property RequestId As String
        Public Property Body As String

        Public ReadOnly Property IsError As Boolean
            Get
                Return Not String.IsNullOrEmpty(Me.ErrCode) OrElse Not String.IsNullOrEmpty(Me.SubErrCode)
            End Get
        End Property
    End Class
End NameSpace