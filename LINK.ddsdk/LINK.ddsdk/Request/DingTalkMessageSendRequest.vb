Imports System.Web.Util
Imports RequestValidator = LINK.ddsdk.Tools.RequestValidator

Namespace Request
    ''' <summary>
    ''' 2019.7.30
    ''' dingtalk发送消息类，主要是为了键值组合
    ''' </summary>
    Public Class DingTalkMessageSendRequest

        Public Property AgentId As Nullable(Of Long)
        Public Property DeptIdList As String
        Public Property Msgcontent As String
        Public Property Msgtype As String
        Public Property ToAllUser As Nullable(Of Boolean)
        Public Property UseridList As String
        Friend otherParams As TopDictionary
        Friend headerParams As TopDictionary 
        Public Function GetApiName() As String
            Return "dingtalk.corp.message.corpconversation.asyncsend"
        End Function

        Public Function GetApiCallType() As String
            Return DingTalkConstants.CALL_TYPE_TOP
        End Function
        Public Function GetParameters() As IDictionary(Of String, String)
            Dim parameters As TopDictionary = New TopDictionary()
            parameters.Add("agent_id", AgentId)
            If DeptIdList IsNot  Nothing Then
                parameters.Add("dept_id_list", DeptIdList)
            End If
        
            parameters.Add("msgcontent", Msgcontent)
            parameters.Add("msgtype", Msgtype)
         ‘   If ToAllUser IsNot Nothing Then
         ’       parameters.Add("to_all_user", ToAllUser)
   ‘         End If
       
            parameters.Add("userid_list", UseridList)

            If otherParams IsNot Nothing Then
                parameters.AddAllDictionary(otherParams) '键值全部加入
            End If

            Return parameters
        End Function
        ''' <summary>
        ''' 2019.7.30
        ''' 键值信息验证
        ''' </summary>
        Public  Sub Validate()
            RequestValidator.ValidateRequired("agent_id", Me.AgentId)
            RequestValidator.ValidateMaxListSize("dept_id_list", Me.DeptIdList, 20)
            RequestValidator.ValidateRequired("msgcontent", Me.Msgcontent)
            RequestValidator.ValidateRequired("msgtype", Me.Msgtype)
            RequestValidator.ValidateMaxListSize("userid_list", Me.UseridList, 100)
        End Sub
        Public Function GetHeaderParameters() As IDictionary(Of String, String)
            If headerParams Is Nothing Then
                headerParams = New TopDictionary()
            End If

            Return headerParams
        End Function
    End Class
End NameSpace