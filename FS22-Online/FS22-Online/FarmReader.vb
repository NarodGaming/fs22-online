' Checks various Farm (savegame) related information

Imports System.IO
Imports Microsoft.VisualBasic.FileIO
Public Class FarmReader

    Public Function GetFarms(ByVal savegamenum As Integer)
        Dim FS22SGFolder As String = SpecialDirectories.MyDocuments + "\My Games\FarmingSimulator2022\savegame" + savegamenum.ToString
        Dim sgfs As New FileStream(FS22SGFolder + "\farms.xml", FileMode.Open, FileAccess.Read)
        Dim XMLObj As New XMLReader
        Dim FarmID As List(Of String) = XMLObj.ReadNodesDetail(sgfs, "farms", "farm", "farmId")
        Dim FarmNames As List(Of String) = XMLObj.ReadNodesDetail(sgfs, "farms", "farm", "name")
        Dim FarmMoney As List(Of String) = XMLObj.ReadNodesDetail(sgfs, "farms", "farm", "money")
        Dim FarmLoan As List(Of String) = XMLObj.ReadNodesDetail(sgfs, "farms", "farm", "loan")
        Dim FarmFullList As New List(Of List(Of String))
        Dim i As Integer
        For i = 0 To FarmNames.Count - 1
            Dim newlist As New List(Of String)
            newlist.Add(FarmID(i))
            newlist.Add(FarmNames(i))
            newlist.Add(FarmMoney(i))
            newlist.Add(FarmLoan(i))
            FarmFullList.Add(newlist)
        Next
        Return FarmFullList
    End Function

End Class
