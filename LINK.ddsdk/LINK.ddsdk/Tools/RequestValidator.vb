Namespace Tools
    ''' <summary>
    ''' 2019.7.29
    ''' 请求验证器类
    ''' </summary>
    Public NotInheritable Class RequestValidator
        Private Const ErrCodeParamMissing As String = "40"
        Private Const ErrCodeParamInvalid As String = "41"
        Private Const ErrMsgParamMissing As String = "client-error:Missing required arguments:{0}"
        Private Const ErrMsgParamInvalid As String = "client-error:Invalid arguments:{0}"
        ''' <summary>
        ''' 2019.7.30
        ''' 验证value字段是否是字符串，如果是空值，则抛出错误
        ''' </summary>
        ''' <param name="name">键</param>
        ''' <param name="value">值</param>
        Public Shared Sub ValidateRequired(ByVal name As String, ByVal value As Object)
            If value Is Nothing Then
                Throw New TopException(ErrCodeParamMissing, String.Format(ErrMsgParamMissing, name))
            Else
                
                If value.[GetType]() = GetType(String) Then
                    Dim strValue As String = TryCast(value, String)

                    If String.IsNullOrEmpty(strValue) Then
                        Throw New TopException(ErrCodeParamMissing, String.Format(ErrMsgParamMissing, name))
                    End If
                End If
            End If
        End Sub

        Public Shared Sub ValidateMaxLength(ByVal name As String, ByVal value As String, ByVal maxLength As Integer)
            If value IsNot Nothing AndAlso value.Length > maxLength Then
                Throw New TopException(ErrCodeParamInvalid, String.Format(ErrMsgParamInvalid, name))
            End If
        End Sub
'
'    Public Shared Sub ValidateMaxLength(ByVal name As String, ByVal value As FileItem, ByVal maxLength As Integer)
'        If value IsNot Nothing AndAlso value.GetFileLength() > maxLength Then
'            Throw New TopException(ERR_CODE_PARAM_INVALID, String.Format(ERR_MSG_PARAM_INVALID, name))
'        End If
'    End Sub

        Public Shared Sub ValidateMaxListSize(ByVal name As String, ByVal value As String, ByVal maxSize As Integer)
            If value IsNot Nothing Then
                Dim data As String() = value.Split(","c)

                If data IsNot Nothing AndAlso data.Length > maxSize Then
                    Throw New TopException(ErrCodeParamInvalid, String.Format(ErrMsgParamInvalid, name))
                End If
            End If
        End Sub

        Public Shared Sub ValidateMaxListSize(ByVal name As String, ByVal value As List(Of String), ByVal maxSize As Integer)
            If value IsNot Nothing Then

                If value IsNot Nothing AndAlso value.Count > maxSize Then
                    Throw New TopException(ErrCodeParamInvalid, ErrMsgParamInvalid)
                End If
            End If
        End Sub

        Public Shared Sub ValidateMaxListSize(ByVal name As String, ByVal value As List(Of Long), ByVal maxSize As Integer)
            If value IsNot Nothing Then

                If value IsNot Nothing AndAlso value.Count > maxSize Then
                    Throw New TopException(ErrCodeParamInvalid, ErrMsgParamInvalid)
                End If
            End If
        End Sub

        Public Shared Sub ValidateMaxListSize(ByVal name As String, ByVal value As List(Of Boolean), ByVal maxSize As Integer)
            If value IsNot Nothing Then

                If value IsNot Nothing AndAlso value.Count > maxSize Then
                    Throw New TopException(ErrCodeParamInvalid, ErrMsgParamInvalid)
                End If
            End If
        End Sub

'    Public Shared Sub ValidateObjectMaxListSize(ByVal name As String, ByVal value As String, ByVal maxSize As Integer)
'        If value IsNot Nothing Then
'            Dim list As IList = TryCast(JSON.Parse(value), IList)
'
'            If list IsNot Nothing AndAlso list.Count > maxSize Then
'                Throw New TopException(ERR_CODE_PARAM_INVALID, String.Format(ERR_MSG_PARAM_INVALID, name))
'            End If
'        End If
'    End Sub

        Public Shared Sub ValidateMinLength(ByVal name As String, ByVal value As String, ByVal minLength As Integer)
            If value IsNot Nothing AndAlso value.Length < minLength Then
                Throw New TopException(ErrCodeParamInvalid, String.Format(ErrMsgParamInvalid, name))
            End If
        End Sub

        Public Shared Sub ValidateMaxValue(ByVal name As String, ByVal value As Nullable(Of Long), ByVal maxValue As Long)
            If value IsNot Nothing AndAlso value > maxValue Then
                Throw New TopException(ErrCodeParamInvalid, String.Format(ErrMsgParamInvalid, name))
            End If
        End Sub

        Public Shared Sub ValidateMinValue(ByVal name As String, ByVal value As Nullable(Of Long), ByVal minValue As Long)
            If value IsNot Nothing AndAlso value < minValue Then
                Throw New TopException(ErrCodeParamInvalid, String.Format(ErrMsgParamInvalid, name))
            End If
        End Sub
    End Class
End NameSpace