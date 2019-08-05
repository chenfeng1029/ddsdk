''' <summary>
''' 2019.7.27
''' kevin zhu
''' 符合TOP习惯的纯字符串字典结构。
''' </summary>
Public Class TopDictionary
    Inherits Dictionary(Of String, String) '键值集合
    Public Sub New()

    End Sub
    Public Sub New(ByVal dictionary As IDictionary(Of String, String))
        MyBase.New(dictionary)
    End Sub
    ''' <summary>
    ''' 添加键值操作
    ''' </summary>
    ''' <param name="vkey">键</param>
    ''' <param name="vValue">值</param>
    Public Sub AddDictionary(ByVal vkey As String, ByVal vValue As Object)
        Dim strValue As String
        If vValue Is Nothing Then
            strValue = Nothing
        End If
        If TypeOf vValue Is String Then
            strValue = CStr(vValue)
        ElseIf TypeOf vValue Is DateTime? Then 'datetime?与datetime区别是前者可以为空值
            strValue = vValue.ToString(Constants.DATE_TIME_FORMAT) '时间格式转化
        ElseIf TypeOf vValue Is Double? Then
            strValue = vValue.ToString()
        Else
            strValue = vValue.ToString()
        End If

        Me.Add(vkey, vValue)
    End Sub
    ''' <summary>
    ''' 全部加入键值
    ''' </summary>
    ''' <param name="vDict">键值集合</param>
    Public Sub AddAllDictionary(ByVal vDict As IDictionary(Of String, String))
        If vDict IsNot Nothing AndAlso vDict.Count > 0 Then
            '不为空的情况下
            For Each kv As KeyValuePair(Of String, String) In vDict
                Dim kvps As IEnumerator(Of KeyValuePair(Of String, String)) = vDict.GetEnumerator()
                While kvps.MoveNext()
                    Dim kvp As KeyValuePair(Of String, String) = kvps.Current
                    Add(kvp.Key, kvp.Value)
                End While
            Next
        End If
    End Sub
End Class