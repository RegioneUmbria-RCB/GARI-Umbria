Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports CrystalDecisions.CrystalReports
Imports AgronicaCoreUtility.Gestione_Eccezioni

Public Class CrystalHelper

    Private Function GetStreamAsByteArray(ByVal stream As System.IO.Stream) As Byte()

        Dim streamLength As Integer = Convert.ToInt32(stream.Length)

        Dim fileData As Byte() = New Byte(streamLength) {}

        ' Read the file into a byte array
        stream.Read(fileData, 0, streamLength)
        stream.Close()

        Return fileData

    End Function

    Public Function getReportDaFile(ByVal Percorso As String, ByVal NomeFileReport As String) As Engine.ReportDocument
        '@@MS TODO: Pericoloso, verificare come viene distrutto

        Dim rpt As New Engine.ReportDocument
        Percorso = AgronicaCoreUtility.FileSystemHelper.AggiungiSlashSeNonEsiste(Percorso)
        rpt.Load(Percorso & NomeFileReport)

        Return rpt

    End Function

    Public Function getReportDaRisorseByTempFile(ByVal NomeFileReport As String, Optional ByVal PercorsoAlternativo As String = "") As Engine.ReportDocument
        '@@MS TODO: Pericoloso, verificare come viene distrutto

        Dim _imageStream As Stream
        Dim _assembly As [Assembly]

        _assembly = [Assembly].GetExecutingAssembly()
        Dim rptFileName As String = NomeFileReport
        _imageStream = _assembly.GetManifestResourceStream("AgronicaStampe_2010." & rptFileName)


        Dim fileContents() As Byte =
            GetStreamAsByteArray(_imageStream)


        Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig

        Dim PathFileTemporanei As String
        If objAgroWeb.PathFileTemporanei = "" Then
            If PercorsoAlternativo <> "" Then
                PathFileTemporanei = PercorsoAlternativo
            Else
                Throw New Exception("Non è stato configurato il parametro PathFileTemporanei in Configurazione_Siti, nè è stata chiamata la funzione getReportDaRisorseByTempFile con il parametro PercorsoAlternativo")
            End If

        Else
            PathFileTemporanei = objAgroWeb.PathFileTemporanei
        End If

        PathFileTemporanei = AgronicaCoreUtility.FileSystemHelper.AggiungiSlashSeNonEsiste(PathFileTemporanei)

        Dim nF As String
        nF = AgronicaCoreUtility.FileSystemHelper.NomeFileUnivoco(".rpt")

        My.Computer.FileSystem.WriteAllBytes(PathFileTemporanei & nF, fileContents, False)

        Dim rpt As New Engine.ReportDocument
        rpt.Load(PathFileTemporanei & nF)

        My.Computer.FileSystem.DeleteFile(PathFileTemporanei & nF, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)

        Return rpt

    End Function

    ''' <summary>
    ''' Torna la cartella per la scrittura dei report temporanei che verranno passati a VisualizzatoreReport
    ''' I files vengono salvati all'interno della cartella File_Temporanei del sito AgronicaStampe_2010
    ''' Se la cartella non esiste viene creata.
    ''' In questa cartella andrà fatta la pulizia periodica in entrata da GestioneRichieste
    ''' </summary>
    Public Shared Function getCartellaReportTemporanei() As String
        Dim pathReportTemporanei As String = HttpContext.Current.Server.MapPath("~") & "\File_Temporanei"
        If Not Directory.Exists(pathReportTemporanei) Then
            Try
                Directory.CreateDirectory(pathReportTemporanei)
            Catch ex As Exception
                Throw New Exception("Errore nella creazione cartella report temporanei: " &
                                    MessaggioCompletoDataEccezione(ex, True), ex)
            End Try
        End If
        Return HttpContext.Current.Server.MapPath("~") & "\File_Temporanei"
    End Function

    ''' <summary>
    ''' Torna path e nome file univoco dove salvare il report temporaneo da passare a VisualizzatoreReport.
    ''' </summary>
    Public Shared Function getFileReportTemporaneo() As String
        '24/02/2021: per il momento usiamo GetRandomFileName, che è più corto... se ci dovessero essere ancora sovrapposizioni si dovrà usare Guid
        'Dim random As String = Guid.NewGuid.ToString
        Dim random As String = Path.GetRandomFileName().Replace(".", "")
        Dim nomeTemp As String = "tmpReport_" & random & "_" & AgronicaCoreUtility.FileSystemHelper.NomeFileUnivoco("rpt")

        Return getCartellaReportTemporanei() & "\" & nomeTemp
    End Function

    ''' <summary>
    ''' Elimina e report temporanei più vecchi di un giorno
    ''' </summary>
    Public Shared Sub eliminaReportTemporanei()
        AgronicaCoreUtility.FileSystemHelper.PuliziaCartella(getCartellaReportTemporanei, "tmpReport*.rpt")
    End Sub

End Class
