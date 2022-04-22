' A class dedicated to reading in the XML files

Imports System.Xml
Imports System.IO

Public Class XMLReader
    Public Function ReadNode(ByRef xmlinput As FileStream, ByVal outernode As String, ByVal innernode As String)
        Dim xmldoc As New XmlDataDocument()
        Dim xmlnode As XmlNodeList
        Dim i As Integer
        xmldoc.Load(xmlinput)
        xmlnode = xmldoc.GetElementsByTagName(outernode)
        For i = 0 To xmlnode(0).ChildNodes.Count - 1
            Dim innernodecur As String = xmlnode(0).ChildNodes.Item(i).Name
            If innernodecur = innernode Then
                xmlinput.Seek(0, SeekOrigin.Begin)
                Return xmlnode(0).ChildNodes.Item(i).InnerText.Trim()
            End If
        Next
        Throw New XmlException
    End Function
End Class
