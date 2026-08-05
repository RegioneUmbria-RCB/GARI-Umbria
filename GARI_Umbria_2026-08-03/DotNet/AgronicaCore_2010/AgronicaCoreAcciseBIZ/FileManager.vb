Imports System.IO

Public Class FileManager : Implements IFileManager

    Private ReadOnly _root As String
    Private ReadOnly _log As String
    Public Sub New(ByVal root As String, ByVal log As String)
        _root = root
        _log = log
    End Sub

    Public Sub Initialize() Implements IFileManager.Initialize

        Dim percorsi = [Enum].GetValues(GetType(AccisePath))

        For Each p As AccisePath In percorsi
            Dim percorsoFisico = OttieniPercorso(p)
            If Not String.IsNullOrEmpty(percorsoFisico) AndAlso Not Directory.Exists(percorsoFisico) Then
                Directory.CreateDirectory(percorsoFisico)
            End If
        Next

    End Sub
    Public Function OttieniPercorso(ByVal tipoPercorso As AccisePath) As String Implements IFileManager.OttieniPercorso

        Select Case tipoPercorso
            Case AccisePath.Root
                Return _root
            Case AccisePath.Temp, AccisePath.Signed
                Return Path.Combine(_root, tipoPercorso.ToString())
            Case AccisePath.Log
                Return Path.Combine(_root, _log)
            Case Else
                Return String.Empty
        End Select

    End Function

    Public Sub Delete(ByVal percorsoFileCompleto As String) Implements IFileManager.Delete

        If File.Exists(percorsoFileCompleto) Then
            File.Delete(percorsoFileCompleto)
        End If

    End Sub
    Public Function OttieniNomeFileLog() As String Implements IFileManager.OttieniNomeFileLog
        Return DateTime.Now.ToString("yyyyMMdd") + "_Log.txt"
    End Function

End Class
