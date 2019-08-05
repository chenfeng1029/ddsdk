''' <summary>
''' TOP API响应解释器接口。响应格式可以是XML, JSON等等。
''' </summary>
''' <typeparam name="T"></typeparam>
Public Interface ITopParser(of T)
    Function Parse(ByVal body As String) As T
    Function Parse(ByVal body As String, ByVal type As Type) As T
End Interface