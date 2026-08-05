Imports System.IO
Imports AgronicaCoreUtility.Gestione_Eccezioni

Public Class FileSystemHelper

    Public Shared Sub CopiaFile(ByVal sourceFilePath As String, ByVal destFolder As String)

        Try
            If Not File.Exists(sourceFilePath) Then
                Return
            End If

            If Not Directory.Exists(destFolder) Then
                Directory.CreateDirectory(destFolder)
            End If

            Dim fileName As String = Path.GetFileName(sourceFilePath)
            Dim destFilePath As String = Path.Combine(destFolder, fileName)

            File.Copy(sourceFilePath, destFilePath, True)

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Public Shared Sub EliminaFile(percorsoFile As String)
        Try
            File.Delete(percorsoFile)
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Shared Sub EliminaFiles(listaFiles As List(Of String))
        Try
            For Each percorsoFile In listaFiles
                File.Delete(percorsoFile)
            Next
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Shared Function AggiungiSlashSeNonEsiste(ByVal Percorso As String) As String
        If Not Percorso.EndsWith("\") Then
            Return Percorso & "\"
        Else
            Return Percorso
        End If
    End Function

    Public Shared Function NomeFileUnivoco(ByVal ext As String, Optional ByVal bSuffisso As Boolean = False, Optional ByVal Add_Undescore As Boolean = False) As String
        Dim New_Name As String

        If ext <> "" And Not ext.StartsWith(".") Then
            ext = "." & ext
        End If

        Dim n As DateTime = Now

        Select Case bSuffisso

            Case False

                New_Name =
                   n.Year &
                   n.Month.ToString.PadLeft(2, "0") &
                   n.Day.ToString.PadLeft(2, "0") &
                   "_" &
                   n.Hour.ToString.PadLeft(2, "0") &
                   n.Minute.ToString.PadLeft(2, "0") &
                   n.Second.ToString.PadLeft(2, "0") &
                   n.Millisecond.ToString.PadLeft(3, "0") &
                   ext

            Case True

                'Verifica Nome File
                Dim Arrayp = Split(ext, ".")

                Select Case UBound(Arrayp)

                    Case 1

                        'Nessuna Estensione
                        New_Name = ext &
                   n.Year &
                   n.Month.ToString.PadLeft(2, "0") &
                   n.Day.ToString.PadLeft(2, "0") &
                   "_" &
                   n.Hour.ToString.PadLeft(2, "0") &
                   n.Minute.ToString.PadLeft(2, "0") &
                   n.Second.ToString.PadLeft(2, "0") &
                   n.Millisecond.ToString.PadLeft(3, "0")

                    Case 2

                        'Nome File
                        New_Name = Arrayp(1) & IIf(Add_Undescore, "_", "") &
                   n.Year &
                   n.Month.ToString.PadLeft(2, "0") &
                   n.Day.ToString.PadLeft(2, "0") &
                   "_" &
                   n.Hour.ToString.PadLeft(2, "0") &
                   n.Minute.ToString.PadLeft(2, "0") &
                   n.Second.ToString.PadLeft(2, "0") &
                   n.Millisecond.ToString.PadLeft(3, "0") &
                   "." & Arrayp(2)


                    Case Else

                        'Nome Invariato
                        New_Name = ext


                End Select



        End Select

        Return New_Name

    End Function

    ''' <summary>
    ''' Cancella i files contenuti nella cartella specificata più vecchi di un periodo definito.
    ''' Default: Cancella tutti i files più vecchi di 1 giorno
    ''' </summary>
    ''' <param name="tmpPath">Path della cartella da pulire</param>
    ''' <param name="filtro">Filtro per selezionare i files da pulire es. *.rpt</param>
    ''' <param name="tipoPeriodo">Tipo periodo es. Day</param>
    ''' <remarks></remarks>
    ''' 
    'MS 29/20/2016 rivista utilizzando System.IO e aggiungendo i parametri
    Public Shared Sub PuliziaCartella(ByVal tmpPath As String,
                               Optional ByVal filtro As String = "*",
                               Optional ByVal tipoPeriodo As DateInterval = DateInterval.Day,
                               Optional ByVal periodo As Integer = 1)
        Try
            For Each file In System.IO.Directory.EnumerateFiles(tmpPath, filtro, IO.SearchOption.TopDirectoryOnly)
                If DateDiff(tipoPeriodo, System.IO.File.GetCreationTime(file), Date.Now) >= periodo Then
                    System.IO.File.Delete(file)
                End If
            Next
        Catch ex As Exception
            Throw (New Exception("Errore nella pulizia della cartella report temporanie: " +
                                 MessaggioCompletoDataEccezione(ex, True), ex))
        End Try
    End Sub

    ''' <summary>
    ''' Sposta il file specificato in un nuovo file nella cartella elaborazione e ne restituisce il FullPath
    ''' </summary>
    ''' <param name="NomeCompletoDiPathDelFileDaTrasferire"></param>
    ''' <param name="CartellaDiDestinazione"></param>
    ''' <param name="NomeCompletoDiPathDelFileTrasferito"></param>
    Public Sub FileSpostaRinominaPerElaborazione(ByVal NomeCompletoDiPathDelFileDaTrasferire As String, ByVal CartellaDiDestinazione As String, ByRef NomeCompletoDiPathDelFileTrasferito As String)

        Dim sNomeFile As String
        Dim sPercorsoLog As String
        Dim spostato As Boolean = False

        'Dim e1 As New EventoServizio("Data " & Now() & ": Inizio importazione dal file " & FullPath)
        'RaiseEvent _EventoImportazione(Me, e1)

        Try

            'MC 160206: Spostato il percorso del log dal file di configurazione al DB
            sPercorsoLog = CartellaDiDestinazione
            '/MC 160206
            If Right(sPercorsoLog, 1) <> "\" Then sPercorsoLog &= "\"

            sNomeFile = System.IO.Path.GetFileName(NomeCompletoDiPathDelFileDaTrasferire)

            sPercorsoLog &= sNomeFile   ' sNomeFile.Substring(1, sNomeFile.IndexOf("."))
            sPercorsoLog &= "_" & Year(Now()) & Right("0" & Month(Now()), 2) & Right("0" & Day(Now()), 2) &
              Right("0" & Hour(Now()), 2) & Right("0" & Minute(Now()), 2) & Right("0" & Second(Now()), 2)

            If Not Directory.Exists(sPercorsoLog) Then
                Directory.CreateDirectory(sPercorsoLog)
            End If

            If Right(sPercorsoLog, 1) <> "\" Then sPercorsoLog &= "\"

            'File.Move(e.FullPath, sPercorsoLog & sNomeFile)
            If Not File.Exists(NomeCompletoDiPathDelFileDaTrasferire) Then Throw New Exception("Il file " & NomeCompletoDiPathDelFileDaTrasferire & " non esiste.")
            NomeCompletoDiPathDelFileTrasferito = sPercorsoLog & sNomeFile
            While Not spostato
                Try
                    File.Move(NomeCompletoDiPathDelFileDaTrasferire, NomeCompletoDiPathDelFileTrasferito)
                    spostato = True
                Catch ex As Exception
                    spostato = False
                End Try
            End While


        Catch ex As Exception

        End Try



    End Sub
End Class
