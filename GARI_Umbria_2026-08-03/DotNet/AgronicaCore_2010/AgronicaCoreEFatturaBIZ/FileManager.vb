Imports System.IO

Public Class FileManager : Implements IFIleManager

    Private ReadOnly _root As String
    Private ReadOnly _log As String

    Public Sub New(ByVal root As String, ByVal log As String)
        _root = root
        _log = log

    End Sub

    Public Sub Initialize() Implements IFIleManager.Initialize

        Dim percorsi = [Enum].GetValues(GetType(FatturaElettronicaPath))

        For Each p As FatturaElettronicaPath In percorsi
            Dim percorsoFisico = OttieniPercorso(p)
            If Not String.IsNullOrEmpty(percorsoFisico) AndAlso Not Directory.Exists(percorsoFisico) Then
                Directory.CreateDirectory(percorsoFisico)
            End If
        Next

    End Sub

    Public Sub SpostaXMLInSpediti(fileName As String) Implements IFIleManager.SpostaXMLInSpediti

        Try
            Dim percorsoDestinazione = OttieniPercorso(FatturaElettronicaPath.XmlSpediti)

            Dim sorgente = Path.Combine(OttieniPercorso(FatturaElettronicaPath.XmlGenerati), fileName)
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
            Dim percorso = OttieniPercorso(FatturaElettronicaPath.XmlGenerati)
            Dim percorsoCompleto = Path.Combine(percorso, fileName)

            If String.IsNullOrEmpty(fileName) Then Return
            If Not File.Exists(percorsoCompleto) Then Return

            File.Delete(percorsoCompleto)
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Public Sub EliminaZip(fileName As String) Implements IFIleManager.EliminaZip

        Try
            Dim percorso = OttieniPercorso(FatturaElettronicaPath.ZipCicloAttivo)
            Dim percorsoCompleto = Path.Combine(percorso, fileName)

            If String.IsNullOrEmpty(fileName) Then Return
            If Not File.Exists(percorsoCompleto) Then Return

            File.Delete(percorsoCompleto)
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Public Function CreaFileZip(zipFileName As String, xmlFilePath As String) As String Implements IFIleManager.CreaFileZip
        Dim zipFullPathName = Path.Combine(OttieniPercorso(FatturaElettronicaPath.ZipCicloAttivo), zipFileName)
        AgronicaCoreUtility.AgroZip.AggiungiFileAZip(zipFullPathName, xmlFilePath, "", True) ' sovrascrivi sempre
        Return zipFullPathName
    End Function

    Public Function CreaFileZip(zipFileName As String, xmlFiles As List(Of String)) As String Implements IFIleManager.CreaFileZip
        AgronicaCoreUtility.AgroZip.AggiungiPiuFileAZip(zipFileName, xmlFiles, "", True) ' sovrascrivi sempre
        Return zipFileName
    End Function

    Public Function OttieniPercorso(ByVal tipoPercorso As FatturaElettronicaPath) As String Implements IFIleManager.OttieniPercorso

        Select Case tipoPercorso
            Case FatturaElettronicaPath.Root
                Return _root
            Case FatturaElettronicaPath.CicloAttivo,
                 FatturaElettronicaPath.CicloPassivo
                Return Path.Combine(_root, tipoPercorso.ToString())
            Case FatturaElettronicaPath.Log
                Return Path.Combine(_root, _log)
            Case FatturaElettronicaPath.ZipCicloAttivo,
                 FatturaElettronicaPath.XmlCicloAttivo
                Return Path.Combine(_root, FatturaElettronicaPath.CicloAttivo.ToString(), tipoPercorso.ToString())
            Case FatturaElettronicaPath.XmlSpediti,
                 FatturaElettronicaPath.XmlGenerati
                Return Path.Combine(_root, FatturaElettronicaPath.CicloAttivo.ToString(), FatturaElettronicaPath.XmlCicloAttivo.ToString(), tipoPercorso.ToString())
            Case FatturaElettronicaPath.ZipCicloPassivo,
                 FatturaElettronicaPath.XmlCicloPassivo
                Return Path.Combine(_root, FatturaElettronicaPath.CicloPassivo.ToString(), tipoPercorso.ToString())
            Case Else
                Return String.Empty
        End Select

    End Function

    Public Function LeggiFilesDaSpedire() As List(Of FileInfo) Implements IFIleManager.LeggiFilesDaSpedire

        Dim cartlleInput = Me.OttieniPercorso(FatturaElettronicaPath.XmlGenerati)
        If Not Directory.Exists(cartlleInput) Then
            Return Enumerable.Empty(Of String)
        End If

        Dim dirInfo = New DirectoryInfo(cartlleInput)
        Dim files = dirInfo.GetFiles("*.xml").OrderBy(Function(f) f.Name)
        Return files.ToList()

    End Function

    Public Function OttieniNomeFileLog() As String Implements IFIleManager.OttieniNomeFileLog
        Return DateTime.Now.ToString("yyyyMMdd") & "_Log.txt"
    End Function
End Class

Public Enum FatturaElettronicaPath

    Root = 0

    CicloAttivo = 10
    ZipCicloAttivo = 11
    XmlCicloAttivo = 12
    XmlGenerati = 13
    XmlSpediti = 14

    CicloPassivo = 20
    ZipCicloPassivo = 21
    XmlCicloPassivo = 22

    Log = 30

End Enum
