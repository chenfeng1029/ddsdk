
Imports System.IO
Imports System.Net
Imports System.Text
Imports Newtonsoft.Json
''' <summary>
''' 2019.7.23
''' 钉钉相关操作类库
''' </summary>
Public Class ddSdk
    ''' <summary>
    ''' 对应钉钉应用的AppKey
    ''' </summary>
    ''' <returns></returns>
    Public Property CorpId As String
    ''' <summary>
    ''' 对应钉钉应用的AppSecret
    ''' </summary>
    ''' <returns></returns>
    Public Property CorpSecret As String
    ''' <summary>
    ''' 2019.7.22
    ''' 错误信息
    ''' </summary>
    ''' <returns></returns>
    Public Property ErrorInfo As String
    Public  Property AccessToken As AccessToken
    Public Sub New ()

    End Sub
    ''' <summary>
    ''' 2019.7.23
    ''' 获取access_token信息，获取成功返回1，失败返回-2，并输出错误信息
    ''' </summary>
    ''' <returns></returns>
    Public Function GetAccessToken() As Integer
        Dim intsuc As Integer = -2
        Try
            Dim strCorpid As String = CorpId
            Dim strcorpSecret As String = CorpSecret
            If Len(strCorpid) = 0 Then '如果系统未提供，则从配置表中读取
                strCorpid=AppConfigInfo.CorpId()
                strcorpSecret=AppConfigInfo.CorpSecret()
            End If
            dim apiurl As String=String.Format("https://oapi.dingtalk.com/gettoken?appkey={0}&appsecret={1}",strCorpid,strcorpSecret)
            Dim request As WebRequest=WebRequest.Create(apiurl)'发出url指令
            dim response As WebResponse =request.GetResponse()'发送指令后，返回反馈信息
            dim stream as Stream=response.GetResponseStream()'反馈回来信息，中获取数据流
            Dim encode as encoding=encoding.UTF8'字符格式
            dim reader as StreamReader=New StreamReader(stream,encode)'数据流以某种字符方式读取
            Dim resultJson as String=reader.ReadToEnd()’从头到尾数据流读取
            Dim tokenResult As TokenResult=JsonConvert.DeserializeObject(Of TokenResult)(resultJson)
            If tokenResult.ErrCode = ErrCodeEnum.OK Then
                AccessToken.Value=tokenResult.Access_token
                AccessToken.StartTime=Now
                intsuc=1
            Else 
                ErrorInfo=String.Format("错误代码:{0},错误信息:{1}",tokenResult.ErrCode,tokenResult.ErrMsg)
            End If
        Catch ex As Exception
            ErrorInfo=ex.Message 
        End Try
        Return intsuc
    End Function
End Class

''' <summary>
''' 2019.7.22
''' 错误代码
''' </summary>
Public Enum ErrCodeEnum
    OK = 0
    VoildAccessToken = 40014
    Unknown = Integer.MaxValue
End Enum