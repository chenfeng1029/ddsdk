Namespace Base

    ''' <summary>
    ''' 2019.7.22
    ''' access_token验证返回信息
    ''' </summary>
    Public Class TokenResult
        Public Property ErrCode As ErrCodeEnum = ErrCodeEnum.Unknown
        Public Property ErrMsg As String
        Public Property Access_token As String
    End Class
End NameSpace