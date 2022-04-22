' Checks to see whether My Games/FarmingSimulator22 folder exists, and which save games also do

Imports System.IO
Imports Microsoft.VisualBasic.FileIO

Public Class SavegameChecker
    Public Function CheckFolder()
        ' Will check that FS22 folder exists
        Dim FS22ExpectedFolder As String = SpecialDirectories.MyDocuments + "\My Games\FarmingSimulator2022"
        If Directory.Exists(FS22ExpectedFolder) Then
            Return True ' FS22 folder located
        Else
            Return False ' FS22 folder not located
        End If
    End Function

    Public Function ReturnSavegames()
        ' Will return which savegames exist (1-20, list returned as array of true and false)
        Dim FS22ExpectedFolder As String = SpecialDirectories.MyDocuments + "\My Games\FarmingSimulator2022"
        Dim returnArray As New List(Of Boolean)
        For i As Integer = 1 To 20
            If File.Exists(FS22ExpectedFolder + "\savegame" + i.ToString + "\careerSavegame.xml") Then
                returnArray.Add(True)
            Else
                returnArray.Add(False)
            End If
        Next
        Return returnArray
    End Function

    Public Function CheckSaveGame(ByVal sgnum As Integer)
        ' Will return True or False depending on if savegame exists
        Dim FS22ExpectedFolder As String = SpecialDirectories.MyDocuments + "\My Games\FarmingSimulator2022"
        If File.Exists(FS22ExpectedFolder + "\savegame" + sgnum.ToString + "\careerSavegame.xml") Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function FetchSavegameInfo(ByVal savegamenum As Integer)
        ' Gets basic save game info like name
        Dim FS22ExpectedFolder As String = SpecialDirectories.MyDocuments + "\My Games\FarmingSimulator2022"
        Dim sgfs As New FileStream(FS22ExpectedFolder + "\savegame" + savegamenum.ToString + "\careerSavegame.xml", FileMode.Open, FileAccess.Read)
        Dim xmlreader As New XMLReader
        Dim SavegameName As String = xmlreader.ReadNode(sgfs, "settings", "savegameName")
        Dim saveDate As String = xmlreader.ReadNode(sgfs, "settings", "saveDate")
        Dim saveMap As String = xmlreader.ReadNode(sgfs, "settings", "mapTitle")
        Dim saveData As New List(Of String)({SavegameName, saveDate, saveMap})
        Return saveData
    End Function
End Class
