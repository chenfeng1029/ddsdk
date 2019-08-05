Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Xml.Serialization
Imports LINK.ddsdk.Response

Public Class TopXmlParser(Of T As TopResponse)
    'Inherits ITopParser (Of T)

    Private Shared ReadOnly regex As Regex = New Regex("<(\w+?)[ >]", RegexOptions.Compiled)
    Private Shared ReadOnly rwLock As ReaderWriterLock = New ReaderWriterLock()
    Private Shared ReadOnly parsers As Dictionary(Of String, XmlSerializer) = New Dictionary(Of String, XmlSerializer)()

    Public Function Parse(ByVal body As String, ByVal type As Type) As T
        Return DoParse(body, type)
    End Function

    Public Function Parse(ByVal body As String) As T
        Dim type As Type = GetType(T)
        Return DoParse(body, type)
    End Function

    Private Function DoParse(ByVal body As String, ByVal type As Type) As T
        Dim rootTagName As String = GetRootElement(body)
        Dim key As String = type.FullName

        If Constants.ERROR_RESPONSE.Equals(rootTagName) Then
            key += ("_" & Constants.ERROR_RESPONSE)
        End If

        Dim serializer As XmlSerializer = Nothing
        Dim incl As Boolean = False
        rwLock.AcquireReaderLock(50)

        Try

            If rwLock.IsReaderLockHeld Then
                incl = parsers.TryGetValue(key, serializer)
            End If

        Finally

            If rwLock.IsReaderLockHeld Then
                rwLock.ReleaseReaderLock()
            End If
        End Try

        If Not incl OrElse serializer Is Nothing Then
            Dim rootAttrs As XmlAttributes = New XmlAttributes()
            rootAttrs.XmlRoot = New XmlRootAttribute(rootTagName)
            Dim attrOvrs As XmlAttributeOverrides = New XmlAttributeOverrides()
            attrOvrs.Add(type, rootAttrs)
            serializer = New XmlSerializer(type, attrOvrs)
            rwLock.AcquireWriterLock(50)

            Try

                If rwLock.IsWriterLockHeld Then
                    parsers(key) = serializer
                End If

            Finally

                If rwLock.IsWriterLockHeld Then
                    rwLock.ReleaseWriterLock()
                End If
            End Try
        End If

        Dim obj As Object = Nothing

        Using stream As System.IO.Stream = New MemoryStream(Encoding.UTF8.GetBytes(body))
            obj = serializer.Deserialize(stream)
        End Using

        Dim rsp As T = CType(obj, T)

        If rsp IsNot Nothing Then
            rsp.Body = body
        End If

        Return rsp
    End Function

    Private Function GetRootElement(ByVal body As String) As String
        Dim match As Match = regex.Match(body)

        If match.Success Then
            Return match.Groups(1).ToString()
        Else
            Throw New TopException("Invalid XML response format!")
        End If
    End Function
End Class