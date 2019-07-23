Imports System.Configuration

''' <summary>
''' 2019.7.21
''' ��ȡ����Ӧ�ó���������Ϣ
''' </summary>
Public Class AppConfigInfo
    ''' <summary>
    ''' ��Ӧ����Ӧ�õ�AppKey
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function CorpId() As String
        Return GetConfigValue("CorpID")
    End Function
    ''' <summary>
    ''' ��Ӧ����Ӧ�õ�AppSecret
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function CorpSecret() As String
        Return GetConfigValue("CorpSecret")
    End Function
    ''' <summary>
    ''' ��ȡ���ñ�ָ��������ֵ
    ''' </summary>
    ''' <param name="key"></param>
    ''' <returns></returns>
    Private Shared Function GetConfigValue(ByVal key As String) As String
        Dim value As String = ConfigurationManager.AppSettings(key)
        
        If value Is Nothing Then
            Throw New Exception("{key} is null.��ȷ�������ļ����Ƿ�������.")
        End If

        Return value
    End Function
End Class