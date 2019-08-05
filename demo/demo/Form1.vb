Imports System.IO
Imports System.Net
Imports System.Text
Imports Newtonsoft.Json

Public Class Form1


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim req As LINK.ddsdk.Request.DingTalkMessageSendRequest = New DingTalkMessageSendRequest()
        req.Msgtype = "oa"
        req.AgentId = "AgentId" '更改钉钉应用id
        req.UseridList = "userid"'用户id
        req.ToAllUser = False
        Dim json_req = New With {
            Key .head = New With {
             Key .text = "推送消息"},
            Key .body = New With {
                      Key .content = txt_content.Text
                       }
        }

        Dim strMsg As String = JsonConvert.SerializeObject(json_req) ' 
        req.Msgcontent = strMsg
        Dim strappkey As String = AppConfigInfo.Appkey()
        Dim strappsecret As String = AppConfigInfo.AppSecret()
        Dim client As New DingTalkClient("https://eco.taobao.com/router/rest", strappkey, strappsecret)
        client.Execute(req)
    End Sub
End Class
