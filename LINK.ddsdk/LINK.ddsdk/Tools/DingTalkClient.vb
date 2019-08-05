Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Xml
Imports LINK.ddsdk
Imports LINK.ddsdk.Base
Imports LINK.ddsdk.Request
Imports LINK.ddsdk.Response
Imports Newtonsoft.Json

Namespace Tools

    Public Class DingTalkClient
        Implements IErrInfo

#Region "类申明变量"

        ''' <summary>
        ''' 错误信息
        ''' </summary>
        Private _nErrmessage1 As String

#End Region

#Region "类自定义属性"

        Friend Serverurl As String

        ''' <summary>
        ''' 应用key
        ''' </summary>
        Friend Appkey As String

        ''' <summary>
        ''' 2019.7.27
        ''' 应用serect
        ''' </summary>
        Friend Appsecret As String

        Friend format As String = Constants.FORMAT_XML

        ''' <summary>
        ''' 禁止响应结果解释
        ''' </summary>
        Friend DisableParser As Boolean = False

        ''' <summary>
        ''' 是否启用响应GZIP压缩
        ''' </summary>
        Friend UseGzipEncoding As Boolean = True

        ''' <summary>
        ''' 网络类
        ''' </summary>
        Friend WebTools As WebTooslib

        ''' <summary>
        ''' ' 设置所有请求共享的系统级参数
        ''' </summary>
        Friend SystemParameters As IDictionary(Of String, String)

        ''' <summary>
        ''' 是否采用精简化的JSON返回
        ''' </summary>
        Friend UseSimplifyJson As Boolean = False
        ''' <summary>
        ''' 净值日志调试
        ''' </summary>
        Friend Disablelogdugging As Boolean = False
        ''' <summary>
        ''' 2019.7.29
        ''' AccessToken值
        ''' </summary>
        ''' <returns></returns>
        Public Property AccessToken As String
        Public ReadOnly Property ErrMessage As String Implements IErrInfo.ErrMessage
            Get
                Return _nErrmessage1
            End Get
        End Property

#End Region

        Public Sub New(ByVal serverurl As String)
            Me.Serverurl = serverurl
            Me.WebTools = New WebTooslib()
        End Sub
        ''' <summary>
        ''' 2019.8.1
        ''' 重构
        ''' </summary>
        ''' <param name="serverurl">url</param>
        ''' <param name="appkey">钉钉应用程序appkey</param>
        ''' <param name="appsecret">钉钉应用程序appsecret</param>
        Public Sub New(ByVal serverurl As String, ByVal appkey As String, ByVal appsecret As String)
            Me.Appkey = appkey
            Me.Appsecret = appsecret
            Me.Appsecret = appsecret
            Me.Serverurl = serverurl
            Me.WebTools = New WebTooslib()
            GetSession(appkey, appsecret) '2019.7.30 获取accesstoken
        End Sub
        ''' <summary>
        ''' 2019.8.1
        ''' 执行http，发送请求
        ''' </summary>
        ''' <param name="vRequest">DingTalk消息内容类</param>
        ''' <returns></returns>
        Public Overridable Function Execute(ByVal vRequest As DingTalkMessageSendRequest) As DingTalkResponse
            Return DoExecute(vRequest, AccessToken, DateTime.Now)
        End Function
        ''' <summary>
        ''' 2019.7.31
        ''' 执行http，发送请求
        ''' </summary>
        ''' <param name="vRequest">DingTalk消息内容类</param>
        ''' <param name="vsession">accessToken</param>
        ''' <returns></returns>
        Public Overridable Function Execute(ByVal vRequest As DingTalkMessageSendRequest, ByVal vsession As String) As DingTalkResponse
            Return DoExecute(vRequest, vsession, DateTime.Now)
        End Function

        Private Function DoExecute(ByVal vrequest As DingTalkMessageSendRequest, ByVal vsession As String, ByVal vtimestamp As DateTime) As DingTalkResponse
            If vrequest.GetApiCallType() Is Nothing OrElse vrequest.GetApiCallType().Equals(DingTalkConstants.CALL_TYPE_TOP) Then
                Return DoExecuteToP(vrequest, vsession, vtimestamp)
            End If
        End Function
        ''' <summary>
        ''' 2019.8.1
        ''' 发送消息到指定钉钉
        ''' </summary>
        ''' <param name="vrequest">需要被发送的消息请求</param>
        ''' <param name="vsession">access_Token</param>
        ''' <param name="vtimestamp"></param>
        ''' <returns></returns>
        Private Function DoExecuteToP(ByVal vrequest As DingTalkMessageSendRequest, ByVal vsession As String, ByVal vtimestamp As DateTime) As DingTalkResponse
            Dim start As Long = DateTime.Now.Ticks

            Try
                vrequest.Validate()
            Catch ex As TopException
                Return CreateErrorResponse(ex.ErrorCode, ex.ErrorMsg)
            End Try

            Dim txtParams As TopDictionary = New TopDictionary(vrequest.GetParameters())
            txtParams.Add(Constants.METHOD, vrequest.GetApiName())
            txtParams.Add(Constants.VERSION, "2.0")
            txtParams.Add(Constants.FORMAT, format)
            txtParams.Add(Constants.PARTNER_ID, GetSdkVersion())
            txtParams.Add(Constants.TIMESTAMP, vtimestamp)
            txtParams.Add(Constants.SESSION, vsession)
            txtParams.AddAllDictionary(Me.SystemParameters)

            If Me.UseSimplifyJson Then
                txtParams.Add(Constants.SIMPLIFY, "true")
            End If

            If Me.UseGzipEncoding Then
                vrequest.GetHeaderParameters()(Constants.ACCEPT_ENCODING) = Constants.CONTENT_ENCODING_GZIP
            End If

            Dim realServerUrl As String = GetServerUrl(Me.Serverurl, vrequest.GetApiName(), vsession)
            Dim reqUrl As String = WebTools.BuildRequestUrl(realServerUrl, txtParams)

            Try
                Dim body As String

                body = WebTools.ExecutePost(realServerUrl, txtParams, vrequest.GetHeaderParameters())

                Dim rsp As DingTalkResponse
                If Constants.FORMAT_XML.Equals(format) Then
                    Dim tp As TopXmlParser(Of DingTalkResponse) = New TopXmlParser(Of DingTalkResponse)()
                    rsp = tp.Parse(body)
                End If

                Return rsp
            Catch ex As Exception
                _nErrmessage1=ex.Message 
            End Try
        End Function

        ''' <summary>
        ''' 2019.7.23
        ''' 获取access_token信息，获取成功返回1，失败返回-2，并输出错误信息
        ''' </summary>
        ''' <param name="vAppkey">应用appkey</param>
        ''' <param name="vAppSerect">应用appSerect</param>
        ''' <returns></returns>
        Public Function GetSession(ByVal vAppkey As String, ByVal vAppSerect As String) As Integer
            Dim intsuc As Integer = -2
            Try
                Dim strCorpid As String = Appkey
                Dim strcorpSecret As String = Appsecret
                Dim apiurl As String = String.Format("https://oapi.dingtalk.com/gettoken?appkey={0}&appsecret={1}", strCorpid, strcorpSecret)
                Dim request As WebRequest = WebRequest.Create(apiurl) '发出url指令
                Dim response As WebResponse = request.GetResponse() '发送指令后，返回反馈信息
                Dim stream As Stream = response.GetResponseStream() '反馈回来信息，中获取数据流
                Dim encode As Encoding = Encoding.UTF8 '字符格式
                Dim reader As StreamReader = New StreamReader(stream, encode) '数据流以某种字符方式读取
                Dim resultJson As String = reader.ReadToEnd() '从头到尾数据流读取
                Dim tokenResult As TokenResult = JsonConvert.DeserializeObject(Of TokenResult)(resultJson)
                If tokenResult.ErrCode = ErrCodeEnum.OK Then
                    AccessToken = tokenResult.Access_token
                    Base.AccessToken.Value = tokenResult.Access_token
                    Base.AccessToken.StartTime = Now
                    intsuc = 1
                Else
                    _nErrmessage1 = String.Format("错误代码:{0},错误信息:{1}", tokenResult.ErrCode, tokenResult.ErrMsg)
                End If
            Catch ex As Exception
                _nErrmessage1 = ex.Message
            End Try
            Return intsuc
        End Function
        Friend Overridable Function GetServerUrl(ByVal serverUrl As String, ByVal apiName As String, ByVal session As String) As String
            Return serverUrl
        End Function

        Friend Overridable Function GetSdkVersion() As String
            Return Constants.SDK_VERSION
        End Function
        ''' <summary>
        ''' 2019.8.1
        ''' 创建响应错误信息返回
        ''' </summary>
        ''' <param name="errCode">错误代码</param>
        ''' <param name="errMsg">错误信息</param>
        ''' <returns></returns>
        Friend Function CreateErrorResponse(ByVal errCode As String, ByVal errMsg As String) As DingTalkResponse
            Dim rsp As DingTalkResponse = Activator.CreateInstance(Of DingTalkResponse)()
            rsp.ErrCode = errCode
            rsp.ErrMsg = errMsg

            If Constants.FORMAT_XML.Equals(format) Then
                Dim root As XmlDocument = New XmlDocument()
                Dim bodyE As XmlElement = root.CreateElement(Constants.ERROR_RESPONSE)
                Dim codeE As XmlElement = root.CreateElement(Constants.ERROR_CODE)
                codeE.InnerText = errCode
                bodyE.AppendChild(codeE)
                Dim msgE As XmlElement = root.CreateElement(Constants.ERROR_MSG)
                msgE.InnerText = errMsg
                bodyE.AppendChild(msgE)
                root.AppendChild(bodyE)
                rsp.Body = root.OuterXml
            Else
                Dim errObj As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
                errObj.Add(Constants.ERROR_CODE, errCode)
                errObj.Add(Constants.ERROR_MSG, errMsg)
                Dim root As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
                root.Add(Constants.ERROR_RESPONSE, errObj)
                ' Dim body As String = JSON.ToJSON(root)
                'rsp.Body = body
            End If

            Return rsp
        End Function

    End Class
End NameSpace