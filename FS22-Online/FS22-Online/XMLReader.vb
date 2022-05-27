' A class dedicated to reading in the XML files

Imports System.Xml
Imports System.IO

Public Class XMLReader
    Public Function ReadNode(ByRef xmlinput As FileStream, ByVal outernode As String, ByVal innernode As String) As String
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

    Public Function ReadNodesDetail(ByRef xmlinput As FileStream, ByVal outernode As String, ByVal innernode As String, ByVal attribname As String) As List(Of String)
        Dim xmldoc As New XmlDataDocument()
        Dim xmlnode As XmlNodeList
        Dim i As Integer
        Dim z As Integer
        Dim innerXML As New List(Of String)
        xmldoc.Load(xmlinput)
        xmlnode = xmldoc.GetElementsByTagName(outernode)
        For i = 0 To xmlnode(0).ChildNodes.Count - 1
            Dim innernodecur As String = xmlnode(0).ChildNodes.Item(i).Name
            If innernodecur = innernode Then
                For z = 0 To xmlnode(0).ChildNodes.Item(i).Attributes.Count - 1
                    If xmlnode(0).ChildNodes.Item(i).Attributes.Item(z).Name = attribname Then
                        innerXML.Add(xmlnode(0).ChildNodes.Item(i).Attributes.Item(z).InnerText.Trim())
                    End If
                Next
            End If
        Next
        xmlinput.Seek(0, SeekOrigin.Begin)
        Return innerXML
    End Function
End Class
