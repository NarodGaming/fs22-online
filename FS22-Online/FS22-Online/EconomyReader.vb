Imports System.IO
Imports Microsoft.VisualBasic.FileIO

Public Structure EconomyItem
    Private ItemName As String
    Private PriceHistory As List(Of Decimal)
End Structure

Public Class EconomyReader
    Public Function GetEconomy(ByVal savegamenum As Integer) As List(Of EconomyItem)
        Dim EconomyItemList As New List(Of EconomyItem)
        Dim FS22SGFolder As String = SpecialDirectories.MyDocuments + "\My Games\FarmingSimulator2022\savegame" + savegamenum.ToString
        Dim sgfs As New FileStream(FS22SGFolder + "\economy.xml", FileMode.Open, FileAccess.Read)
        Dim XMLObj As New XMLReader
        Return EconomyItemList
    End Function

End Class
