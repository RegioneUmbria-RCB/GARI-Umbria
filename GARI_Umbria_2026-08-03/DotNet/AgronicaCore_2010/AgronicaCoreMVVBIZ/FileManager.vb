Imports System.IO
Public Class FileManager : Implements IFIleManager

    Private ReadOnly _root As String
    Private ReadOnly _log As String
    Public Sub New(ByVal root As String, ByVal log As String)
        _root = root
        _log = log
    End Sub

    Public Sub Initialize() Implements IFIleManager.Initialize

        Dim percorsi = [Enum].GetValues(GetType(MVV_E_Path))

        For Each p As MVV_E_Path In percorsi
            Dim percorsoFisico = OttieniPercorso(p)
            If Not String.IsNullOrEmpty(percorsoFisico) AndAlso Not Directory.Exists(percorsoFisico) Then
                Directory.CreateDirectory(percorsoFisico)
            End If
        Next

    End Sub

    Public Sub SpostaXMLInSpediti(fileName As String) Implements IFIleManager.SpostaXMLInSpediti

        Try
            Dim percorsoDestinazione = OttieniPercorso(MVV_E_Path.XmlSpediti)

            Dim sorgente = Path.Combine(OttieniPercorso(MVV_E_Path.XmlGenerati), fileName)
            Dim destinazione = Path.Combine(percorsoDestinazione, fileName)

            If Not Directory.Exists(percorsoDestinazione) Then
                Directory.CreateDirectory(percorsoDestinazione)
            End If

            If File.Exists(sorgente) Then
                If File.Exists(destinazione) Then
                    File.Delete(destinazione)
                End If
                File.Move(sorgente, destinazione)
            End If
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Public Sub EliminaXML(fileName As String) Implements IFIleManager.EliminaXML

        Try
            Dim percorso = OttieniPercorso(MVV_E_Path.XmlGenerati)
            Dim percorsoCompleto = Path.Combine(percorso, fileName)

            If String.IsNullOrEmpty(fileName) Then Return
            If Not File.Exists(percorsoCompleto) Then Return

            File.Delete(percorsoCompleto)
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Public Sub CopiaFile(ByVal sourceFilePath As String, ByVal destFolder As String)

        Try
            If Not File.Exists(sourceFilePath) Then
                Return
            End If

            Dim fileName As String = Path.GetFileName(sourceFilePath)
            If Not Directory.Exists(destFolder) Then
                Directory.CreateDirectory(destFolder)
            End If
            Dim destFilePath As String = Path.Combine(destFolder, fileName)

            File.Copy(sourceFilePath, destFilePath, True)

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Public Function OttieniPercorso(ByVal tipoPercorso As MVV_E_Path) As String Implements IFIleManager.OttieniPercorso

        Select Case tipoPercorso
            Case MVV_E_Path.Root
                Return _root
            Case MVV_E_Path.Log
                Return Path.Combine(_root, _log)
            Case MVV_E_Path.XmlGenerati,
                 MVV_E_Path.XmlSpediti,
                 MVV_E_Path.PdfRicevuti
                Return Path.Combine(_root, tipoPercorso.ToString())
            Case Else
                Return String.Empty
        End Select

    End Function

    Public Function LeggiFilesDaSpedire() As List(Of FileInfo) Implements IFIleManager.LeggiFilesDaSpedire

        Dim cartlleInput = Me.OttieniPercorso(MVV_E_Path.XmlGenerati)
        If Not Directory.Exists(cartlleInput) Then
            Return Enumerable.Empty(Of String)
        End If

        Dim dirInfo = New DirectoryInfo(cartlleInput)
        Dim files = dirInfo.GetFiles("*.xml").OrderBy(Function(f) f.Name)
        Return files.ToList()

    End Function

    Public Function OttieniNomeFileLog() As String Implements IFIleManager.OttieniNomeFileLog
        Return DateTime.Now.ToString("yyyyMMdd") + "_Log.txt"
    End Function
End Class

Public Enum MVV_E_Path

    Root = 0

    XmlGenerati = 10
    XmlSpediti = 11
    PdfRicevuti = 12

    Log = 30

End Enum
