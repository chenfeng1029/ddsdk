Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Web
Imports LINK.ddsdk
Imports LINK.ddsdk.Base
Imports Newtonsoft.Json

Namespace Tools

    ''' <summary>
    ''' 2019.7.27
    ''' kevin zhu
    ''' 网络类相关操作
    ''' </summary>
    Public Class WebTooslib
        Implements IErrInfo

#Region "申明变量"
        ''' <summary>
        ''' 错误信息
        ''' </summary>
        Private _nErrmessage1 As String

#End Region

#Region "类自定义属性"

        ''' <summary>
        ''' 2019.7.24
        ''' 等待请求开始返回的超时时间
        ''' </summary>
        ''' <returns></returns>
        Public Property Timeout As Integer = 20000

        ''' <summary>
        ''' 等待读取数据完成的超时时间
        ''' </summary>
        ''' <returns></returns>
        Public Property ReadWriteTimeout As Integer = 60000

        ''' <summary>
        ''' 2019.7.24
        ''' 禁止本地代理
        ''' </summary>
        ''' <returns></returns>
        Public Property DisableWebProxy As Boolean = False

        ''' <summary>
        ''' 2019.7.26
        ''' 是否忽略ssl检查
        ''' </summary>
        ''' <returns></returns>
        Public Property IgnoreSslCheck As Boolean = True

        ''' <summary>
        ''' 2019.7.24
        ''' 返回错误信息
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ErrMessage As String Implements IErrInfo.ErrMessage
            Get
                Return _nErrmessage1
            End Get
        End Property

#End Region
        ''' <summary>
        ''' 2019.7.29
        ''' 执行http,发起Post,获取返回值
        ''' </summary>
        ''' <param name="vurl">请求web地址</param>
        ''' <param name="vtextParams">请求的文本参数</param>
        ''' <param name="vheaderParams">请求的头部参数</param>
        ''' <returns></returns>
        Public Function ExecutePost(ByVal vurl As String, ByVal vtextParams As IDictionary(Of String, String), ByVal vheaderParams As IDictionary(Of String, String)) As String
            Try
                '发起请求
                Dim req As HttpWebRequest = GetWebRequest(vurl, "POST", vheaderParams)
                '请求内容形式
                '            req的内容类型有多种
                '            application/xhtml+xml ：XHTML格式
                '              application/xml     ： XML数据格式
                '              application/atom+xml  ：Atom XML聚合格式    
                '              application/json    ： JSON数据格式
                '               application/pdf       ：pdf格式  
                '               application/msword  ： Word文档格式
                '               application/octet-stream ： 二进制流数据（如常见的文件下载）
                '            application/x-www-form-urlencoded ： <form encType=””>中默认的encType，form表单数据被编码为key/value格式发送到服务器（表单默认的提交数据的格式）
                '
                req.ContentType = "application/x-www-form-urlencoded;charset=utf-8"
                '文本参数分解成数组
                Dim bs As Byte() = Encoding.UTF8.GetBytes(BuildQuery(vtextParams))
                '获取写入请求数据的流对象
                Dim reqStream As Stream = req.GetRequestStream()
                '从0位置开始，读到末尾
                reqStream.Write(bs, 0, bs.Length)
                reqStream.Close()
                Dim rsp As HttpWebResponse = CType(req.GetResponse(), HttpWebResponse)
                Dim encoding1 As Encoding = GetResponseEncoding(rsp)
                Return GetResponseAsString(rsp, encoding1)
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

        End Function
        ''' <summary>
        ''' 2019.7.24
        ''' 发送url指令信息，返回接收消息
        ''' </summary>
        ''' <param name="vurl">请求页面</param>
        ''' <param name="vdata">请求的字符串数据</param>
        ''' <param name="vTimeout">默认超时2000</param>
        ''' <returns></returns>
        Public Function ExecutePost(ByVal vurl As String, ByVal vdata As String, Optional ByVal vTimeout As Integer = 20000) As String
            Dim req As HttpWebRequest = CType(Net.HttpWebRequest.Create(vurl), HttpWebRequest) '
            Dim bs As Byte() = Encoding.UTF8.GetBytes(vdata) '将字符串转换为UTF8编码的字节数组
            req.Method = "POST" '设置请求类型
            'req的内容类型有多种
            ' application/xhtml+xml ：XHTML格式
            '   application/xml     ： XML数据格式
            '   application/atom+xml  ：Atom XML聚合格式    
            '  application/json    ： JSON数据格式
            '   application/pdf       ：pdf格式  
            '   application/msword  ： Word文档格式
            '   application/octet-stream ： 二进制流数据（如常见的文件下载）
            'application/x-www-form-urlencoded ： <form encType=””>中默认的encType，form表单数据被编码为key/value格式发送到服务器（表单默认的提交数据的格式）
            req.ContentType = "application/json"
            req.ContentLength = bs.Length
            req.Timeout = vTimeout '超时时间
            '将参数写入请求地址中
            '表达的意思是bs从req的"0"位置中开始写入，长度为"bs.length",说白了就是把参数数据都写入到请求数据中
            req.GetRequestStream().Write(bs, 0, bs.Length)
            '发送请求
            Dim res As HttpWebResponse = CType(req.GetResponse(), HttpWebResponse)
            '读取返回数据
            Dim streamReader As StreamReader = New StreamReader(res.GetResponseStream(), Encoding.UTF8) '
            Dim strResponseContent As String = streamReader.ReadToEnd()
            streamReader.Close()
            res.Close()
            req.Abort() '取消http请求
            Return strResponseContent
        End Function

        ''' <summary>
        ''' 2019.7.29
        ''' 发送Http请求
        ''' </summary>
        ''' <param name="vurl">请求web地址</param>
        ''' <param name="vmethod">请求方法</param>
        ''' <param name="vheaderParams">请求的头部文本参数</param>
        ''' <returns></returns>
        Public Function GetWebRequest(ByVal vurl As String, ByVal vmethod As String, ByVal vheaderParams As IDictionary(Of String, String)) As HttpWebRequest
            Dim req As HttpWebRequest = Nothing
            '字符串读取从指定位置开始读取，字符串不区别大小写
            If vurl.StartsWith("https", StringComparison.OrdinalIgnoreCase) Then
                '是否忽略ssl检查
                If IgnoreSslCheck Then
                    '设置验证证书回调
                    ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf TrustAllValidationCallback)
                End If
                '创建新的http请求
                req = CType(WebRequest.CreateDefault(New Uri(vurl)), HttpWebRequest)
            Else
                req = CType(WebRequest.Create(vurl), HttpWebRequest)
            End If
            '是否禁止本地代理
            If DisableWebProxy Then
                req.Proxy = Nothing
            End If

            If vheaderParams IsNot Nothing AndAlso vheaderParams.Count > 0 Then

                For Each key As String In vheaderParams.Keys
                    req.Headers.Add(key, vheaderParams(key))
                Next
            End If

            req.ServicePoint.Expect100Continue = False
            req.Method = vmethod '请求方法
            req.KeepAlive = True
            req.UserAgent = "top-sdk-net" '获取或设置 User-agent HTTP 标头的值
            req.Accept = "text/xml,text/javascript" '
            req.Timeout = Timeout '等待请求开始返回的超时时间
            req.ReadWriteTimeout = ReadWriteTimeout '文本读取等待时间
            Return req
        End Function
        Private Shared Function TrustAllValidationCallback(ByVal sender As Object, ByVal certificate As X509Certificate, ByVal chain As X509Chain, ByVal errors As SslPolicyErrors) As Boolean
            Return True
        End Function
        ''' <summary>
        ''' 2019.7.27
        ''' 组装普通文本请求参数
        ''' </summary>
        ''' <param name="vTxtPara">key-value形式请求参数字典</param>
        ''' <returns>URL编码后的请求数码</returns>
        Private Shared Function BuildQuery(ByVal vTxtPara As IDictionary(Of String, String)) As String

            If vTxtPara Is Nothing OrElse vTxtPara.Count = 0 Then
                '未存在参数
                Return Nothing
            End If
            Dim query As StringBuilder = New StringBuilder() '一个可变的字符序列
            Dim blnParam As Boolean = False

            'KeyValuePair 和 Dictionary 的关系
            'KeyValuePair 是一个结构体（struct）；KeyValuePair 只包含一个Key、Value的键值对。
            'Dictionary 可以简单的看作是KeyValuePair 的集合；Dictionary 可以包含多个Key、Value的键值对。
            For Each kv As KeyValuePair(Of String, String) In vTxtPara
                Dim strname As String = kv.Key '键
                Dim strvalue As String = kv.Value '值
                If Not String.IsNullOrEmpty(strname) AndAlso Not String.IsNullOrEmpty(strvalue) Then
                    '
                    If blnParam Then
                        query.Append("&")
                    End If
                    query.Append(strname)
                    query.Append("=")
                    'HttpUtility
                    query.Append(System.Web.HttpUtility.UrlEncode(strvalue, Encoding.UTF8))
                    blnParam = True
                End If
            Next
            '  MsgBox(query.ToString())
            Return query.ToString()


        End Function
        ''' <summary>
        ''' 2019.7.26
        ''' 返回指定的代码页名称的关联编码
        ''' </summary>
        ''' <param name="rsp">响应</param>
        ''' <returns></returns>
        Private Function GetResponseEncoding(ByVal rsp As HttpWebResponse) As Encoding
            Dim charset As String = rsp.CharacterSet '获取响应的字符集

            If String.IsNullOrEmpty(charset) Then
                charset = Constants.CHARSET_UTF8 '常量 utf-8,方便调用
            End If

            Return Encoding.GetEncoding(charset)
        End Function
        ''' <summary>
        ''' 2019.7.27
        ''' 把响应流转换为文本。
        ''' </summary>
        ''' <param name="vrsp">响应流对象</param>
        ''' <param name="encode">编码方式，默认可以Encoding.UTF8</param>
        ''' <returns></returns>
        Public Function GetResponseAsString(ByVal vrsp As HttpWebResponse, ByVal encode As Encoding) As String
            Dim strValue As String = ""
            Dim stream As Stream = Nothing
            Dim reader As StreamReader = Nothing
            Try


                stream = vrsp.GetResponseStream()
                '是否压缩方式传输
                If Constants.CONTENT_ENCODING_GZIP.Equals(vrsp.ContentEncoding) Then
                    stream = New GZipStream(stream, CompressionMode.Decompress) '如果流传输是压缩方式，则进行解压
                End If
                reader = New StreamReader(stream, encode)
                strValue = reader.ReadToEnd() '从0位置到尾末读取


            Finally
                If reader IsNot Nothing Then reader.Close()
                If stream IsNot Nothing Then stream.Close()
                If vrsp IsNot Nothing Then vrsp.Close()
            End Try
            Return strValue
        End Function
        ''' <summary>
        ''' 2019.7.29
        ''' 组装含参数的请求URL。
        ''' </summary>
        ''' <param name="url">请求的web地址</param>
        ''' <param name="parameters">请求参数映射</param>
        ''' <returns>带参数的请求URL</returns>
        Public Shared Function BuildRequestUrl(ByVal url As String, ByVal parameters As IDictionary(Of String, String)) As String
            If parameters IsNot Nothing AndAlso parameters.Count > 0 Then
                Return BuildRequestUrl(url, BuildQuery(parameters))
            End If

            Return url
        End Function
        ''' <summary>
        ''' 2019.7.29
        ''' 组装含参数的请求URL。
        ''' </summary>
        ''' <param name="url">请求web地址</param>
        ''' <param name="queries">一个或多个经过URL编码后的请求参数串</param>
        ''' <returns>带参数的请求URL</returns>
        Public Shared Function BuildRequestUrl(ByVal url As String, ParamArray queries As String()) As String
            If queries Is Nothing OrElse queries.Length = 0 Then
                Return url
            End If

            Dim newUrl As StringBuilder = New StringBuilder(url)
            Dim hasQuery As Boolean = url.Contains("?")
            Dim hasPrepend As Boolean = url.EndsWith("?") OrElse url.EndsWith("&")

            For Each query As String In queries

                If Not String.IsNullOrEmpty(query) Then

                    If Not hasPrepend Then

                        If hasQuery Then
                            newUrl.Append("&")
                        Else
                            newUrl.Append("?")
                            hasQuery = True
                        End If
                    End If

                    newUrl.Append(query)
                    hasPrepend = False
                End If
            Next

            Return newUrl.ToString()
        End Function

    End Class
End Namespace