Imports System
Imports System.IO
Imports System.Collections
Imports AgronicaCoreWinsortDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreContabBIZ

Public Class ImportaCalibratureManager

    Private objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private reader As DBCalibrature_R
    Private writer As DBCalibrature_W
    Private codMacchinaLav As String
    Private tipoImportatore As TipoImportatore
    Private folder As String
    Private piva As String

    Private Function importaFile(ByVal filename As String) As Integer

        Dim importatore As AbsImportatoreFile = Nothing

        Select Case tipoImportatore

            Case TipoImportatore.Importatore_Calibratrice
                'Importazione dati da calibratrice MINIFRUTTA
                importatore = New ImportatoreCalibratrice(filename)

            Case TipoImportatore.Importatore_Campionatrice
                'Importazione dati da campionatrice CoFruTa
                importatore = New ImportatoreCampionatrice(filename)

                'Case TipoImportatore.Importatore_CalibCompac
                '    'Importazione dati da calibratrice Compac (CoFruTa)
                '    importatore = New ImportatoreCalibCompac(filename)

        End Select

        If importatore Is Nothing Then
            Return 0
        End If

        Dim id As Integer

        If (Not importatore.importa()) Then
            Return 0
        End If

        id = writer.InserisciCalibratura(importatore.conferitoreCodice,
                                         importatore.conferitoreNome,
                                         importatore.dataInizio,
                                         importatore.dataFine,
                                         importatore.lotto,
                                         importatore.varieta,
                                         importatore.programma,
                                         importatore.nrBolla,
                                         importatore.rifBolla,
                                         importatore.pesoTot,
                                         importatore.numeroTot,
                                         importatore.durata,
                                         importatore.scarti,
                                         importatore.note,
                                         codMacchinaLav,
                                         statoImportazione.fileImportato)

        For Each cal As AbsImportatoreFile.Calibro In importatore.Calibri

            Try

                writer.InserisciCalibriXCalibrature(id, cal.qualita, cal.nome, cal.peso, cal.perc, cal.num)

            Catch ex As Exception

            End Try

        Next

        'Controllo che il file non sia già stato importato
        Dim id2 As Integer = reader.leggi_ID_da_filename(Path.GetFileName(filename))
        If (id2 > 0) Then
            Dim lotto As String = reader.leggi_Lotto(id2)
            If (Not String.IsNullOrEmpty(lotto)) AndAlso (String.Compare(importatore.lotto, lotto) = 0) Then
                writer.elimina(id2)
            End If
        End If

        Return id

    End Function

    Private Function spostafile(subFolder As String, srcFileName As String) As Boolean

        Dim MovePath As String = Path.GetDirectoryName(srcFileName)
        MovePath += subFolder

        Dim MoveFile = Path.GetFileName(srcFileName)
        Dim MoveSuffix = ""

        Dim Done = False
        Dim iSfx As Integer = 1
        While Not Done

            Dim destMovePath = MovePath + MoveSuffix

            If Not Directory.Exists(destMovePath) Then
                Directory.CreateDirectory(destMovePath)
            End If

            Dim destFileName = destMovePath + "\" + MoveFile

            Try

                My.Computer.FileSystem.MoveFile(srcFileName, destFileName, False)
                Done = True

            Catch ioex As IOException
                'errore file esistente
                MoveSuffix = "_" + iSfx.ToString("00")

            Catch ex As Exception

            End Try

            iSfx += 1

        End While

        Return True

    End Function


    Public Sub New(ByVal codMacchinaLav As String, ByRef folder As String, ByVal piva As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Me.objParametri = objParametri
        reader = New DBCalibrature_R(objParametri)
        writer = New DBCalibrature_W(objParametri)
        Dim iImp = reader.leggiTipoImportatore(codMacchinaLav)
        If ([Enum].IsDefined(GetType(TipoImportatore), iImp)) Then
            Me.tipoImportatore = iImp
        Else
            Me.tipoImportatore = TipoImportatore.Importatore_Sconosciuto
        End If
        Me.codMacchinaLav = codMacchinaLav
        Me.folder = folder
        Me.piva = piva

    End Sub

    Public Function importa() As Boolean

        If Not Directory.Exists(folder) Then
            Return False
        End If

        Dim arrayFile As String()
        arrayFile = Directory.GetFiles(folder)
        Dim listaFileDaAggiornare As New List(Of String)(arrayFile)

        For Each filename As String In listaFileDaAggiornare

            Dim FlagTransazioneLocale As Boolean = False
            Dim FlagConnessioneLocale As Boolean = False
            Dim id As Integer

            Try
                'Apro la connessione al DB e la transazione
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                    FlagTransazioneLocale,
                                                                    objParametri)

                id = importaFile(filename)

                If (id > 0) Then

                    Try

                        writer.inserisciLogImportazione(id, DateTime.Now, filename)

                    Catch ex As Exception
                        'anche se non riesco ad inserire il log devo comunque spostare il file
                    End Try

                    spostafile("\Importati\" & DateTime.Now.ToString("yyyy_MM_dd"), filename)

                ElseIf (id = 0) Then

                    'sposto il file nella sottocartella NonImportati...
                    'spostafile("\NonImportati", filename) modificato il 23 Novembre 2017
                    spostafile("\Importati\Errori\" & DateTime.Now.ToString("yyyy_MM_dd"), filename)

                End If

                'Chiudo la connessione al DB
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            Catch ex As Exception

                'Faccio il rollback della transazione
                If Not objParametri.objTransazione Is Nothing Then
                    'objParametri.objTransazione.Rollback()
                    'objParametri.objTransazione = Nothing
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
                End If

            End Try

            If (id > 0) Then

                Try

                    Dim campConf = New FF_CampionamentoConferimento
                    campConf.TrovaErroriInLottoDaImportare(piva, id, tipoImportatore, objParametri)

                Catch ex As Exception

                End Try
            End If

        Next

        Return True

    End Function

End Class


