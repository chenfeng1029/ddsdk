Imports System.IO
Imports System.Net
Imports System.Net.Mime
Imports System.Text
''' <summary>
''' 2019.7.24
''' HttpWebRequest封装的类，用于发起Get,Post请求，返回获取信息值
''' </summary>
Public Class Httplib
    ''' <summary>
    ''' 2019.7.24
    ''' 发送url指令信息，返回接收消息
    ''' </summary>
    ''' <param name="vurl"></param>
    ''' <param name="vdata"></param>
    ''' <param name="vTimeout">默认超时2000</param>
    ''' <returns></returns>
    Public Shared Function PostData(ByVal vurl As String,ByVal vdata As string,Optional ByVal vTimeout As Integer=20000) As String
        Dim req As HttpWebRequest=CType(Net.HttpWebRequest.Create(vurl), HttpWebRequest) '
        Dim bs As Byte()=Encoding.UTF8.GetBytes(vdata) '将字符串转换为UTF8编码的字节数组
        req.Method="POST"'设置请求类型
        'req的内容类型有多种
       ‘ application/xhtml+xml ：XHTML格式
	’   application/xml     ： XML数据格式
	‘   application/atom+xml  ：Atom XML聚合格式    
	’  application/json    ： JSON数据格式
	‘   application/pdf       ：pdf格式  
	’   application/msword  ： Word文档格式
	'   application/octet-stream ： 二进制流数据（如常见的文件下载）
        ‘application/x-www-form-urlencoded ： <form encType=””>中默认的encType，form表单数据被编码为key/value格式发送到服务器（表单默认的提交数据的格式）
        req.ContentType="application/json"
        req.ContentLength=bs.Length
        req.Timeout=vTimeout '超时时间
        ’将参数写入请求地址中
        ’表达的意思是bs从req的"0"位置中开始写入，长度为"bs.length",说白了就是把参数数据都写入到请求数据中
        req.GetRequestStream().Write(bs,0,bs.Length) 
        '发送请求
        Dim res As HttpWebResponse=CType(req.GetResponse(),HttpWebResponse)
        ‘读取返回数据
        Dim streamReader As StreamReader=New StreamReader(res.GetResponseStream(),Encoding.UTF8)'
        Dim strResponseContent As String=streamReader.ReadToEnd()
        streamReader.Close()
        res.Close()
        req.Abort()'取消http请求
        Return strResponseContent
    End Function
    ''' <summary>
    ''' 2019.7.24
    ''' 请求url,如果需要传参数，可以在url末尾加上?+参数名=参数值即可
    ''' </summary>
    ''' <param name="vurl"></param>
    ''' <param name="vTimeout"></param>
    ''' <returns></returns>
    Public Shared Function GetData(ByVal vurl As string,Optional ByVal vTimeout As Integer=20000) As String
        Dim req As HttpWebRequest=CType(HttpWebRequest.Create(vurl),HttpWebRequest)
        req.ContentType="GET"'请求方式
        req.Timeout =0
        Dim res As HttpWebResponse=CType(req.GetResponse(),HttpWebResponse)
        '通过streamreader流读取返回数据
        Dim streamreader As StreamReader=New StreamReader(res.GetResponseStream(),encoding.UTF8)
        '获取json数据
        Dim strResponseContent As String=streamreader.ReadToEnd()
        streamreader.Close()
        res.Close()
        req.Abort()
        Return strResponseContent
    End Function
End Class
