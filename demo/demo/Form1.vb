Imports System.IO
Imports System.Net
Imports System.Text
Imports Newtonsoft.Json

Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim ddsdk As New ddSdk
        Dim intsuc As Integer = ddsdk.GetAccessToken()
        If intsuc = 1 Then
            Dim accessToken As String = ddsdk.AccessToken.Value
            Dim strMessageUrl As String = String.Format("https://oapi.dingtalk.com/message/send?access_token={0}", accessToken)
            Dim json_req = New With {
            Key .touser = "userid",'//用户id
            Key .toparty = "",
            Key .agentid = "276144477",'//钉钉应用上的AgentId
            Key .msgtype = "text",
            Key .text = New With
            {Key .content = txt_content.text
            }
        }
            Dim jsonRequest As String = JsonConvert.SerializeObject(json_req)
            HttpPost(strMessageUrl, jsonRequest)

        End If
    End Sub
    Private Shared Function HttpPost(ByVal url As String, ByVal data As String) As String
        Dim httpWebRequest As HttpWebRequest = CType(HttpWebRequest.Create(url), HttpWebRequest)
        Dim bs As Byte() = Encoding.UTF8.GetBytes(data)
        httpWebRequest.ContentType = "application/json"
        httpWebRequest.ContentLength = bs.Length
        httpWebRequest.Method = "POST"
        httpWebRequest.Timeout = 20000
        httpWebRequest.GetRequestStream().Write(bs, 0, bs.Length)
        Dim httpWebResponse As HttpWebResponse = CType(httpWebRequest.GetResponse(), HttpWebResponse)
        Dim streamReader As StreamReader = New StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8)
        Dim responseContent As String = streamReader.ReadToEnd()
        streamReader.Close()
        httpWebResponse.Close()
        httpWebRequest.Abort()
        Return responseContent
    End Function
End Class
