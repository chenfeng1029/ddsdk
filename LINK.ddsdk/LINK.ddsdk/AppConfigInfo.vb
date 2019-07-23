Imports System.Configuration

''' <summary>
''' 2019.7.21
''' 获取本地应用程序配置信息
''' </summary>
Public Class AppConfigInfo
    ''' <summary>
    ''' 对应钉钉应用的AppKey
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function CorpId() As String
        Return GetConfigValue("CorpID")
    End Function
    ''' <summary>
    ''' 对应钉钉应用的AppSecret
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function CorpSecret() As String
        Return GetConfigValue("CorpSecret")
    End Function
    ''' <summary>
    ''' 获取配置表指定参数的值
    ''' </summary>
    ''' <param name="key"></param>
    ''' <returns></returns>
    Private Shared Function GetConfigValue(ByVal key As String) As String
        Dim value As String = ConfigurationManager.AppSettings(key)
        
        If value Is Nothing Then
            Throw New Exception("{key} is null.请确认配置文件中是否已配置.")
        End If

        Return value
    End Function
End Class