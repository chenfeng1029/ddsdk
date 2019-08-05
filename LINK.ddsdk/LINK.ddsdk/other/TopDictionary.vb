''' <summary>
''' 2019.7.27
''' kevin zhu
''' ����TOPϰ�ߵĴ��ַ����ֵ�ṹ��
''' </summary>
Public Class TopDictionary
    Inherits Dictionary(Of String, String) '��ֵ����
    Public Sub New()

    End Sub
    Public Sub New(ByVal dictionary As IDictionary(Of String, String))
        MyBase.New(dictionary)
    End Sub
    ''' <summary>
    ''' ��Ӽ�ֵ����
    ''' </summary>
    ''' <param name="vkey">��</param>
    ''' <param name="vValue">ֵ</param>
    Public Sub AddDictionary(ByVal vkey As String, ByVal vValue As Object)
        Dim strValue As String
        If vValue Is Nothing Then
            strValue = Nothing
        End If
        If TypeOf vValue Is String Then
            strValue = CStr(vValue)
        ElseIf TypeOf vValue Is DateTime? Then 'datetime?��datetime������ǰ�߿���Ϊ��ֵ
            strValue = vValue.ToString(Constants.DATE_TIME_FORMAT) 'ʱ���ʽת��
        ElseIf TypeOf vValue Is Double? Then
            strValue = vValue.ToString()
        Else
            strValue = vValue.ToString()
        End If

        Me.Add(vkey, vValue)
    End Sub
    ''' <summary>
    ''' ȫ�������ֵ
    ''' </summary>
    ''' <param name="vDict">��ֵ����</param>
    Public Sub AddAllDictionary(ByVal vDict As IDictionary(Of String, String))
        If vDict IsNot Nothing AndAlso vDict.Count > 0 Then
            '��Ϊ�յ������
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