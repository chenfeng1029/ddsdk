''' <summary>
''' TOP API��Ӧ�������ӿڡ���Ӧ��ʽ������XML, JSON�ȵȡ�
''' </summary>
''' <typeparam name="T"></typeparam>
Public Interface ITopParser(of T)
    Function Parse(ByVal body As String) As T
    Function Parse(ByVal body As String, ByVal type As Type) As T
End Interface