Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello
Imports System.IO
Imports System.IO.Compression
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreUtility

Public Class Allegati
    Inherits AgronicaCoreDataProvider.DataProvider

    ' converte allegati da base 64 a binario
    Public Shared Sub ConvertiAllegati(ByVal Allegati As List(Of DocumentoAllegato), ByRef FileName As String, ByRef FileAllegato As Byte())
        If Allegati.Count = 1 Then
            Dim allegato = Allegati.FirstOrDefault
            FileName = allegato.FileName
            FileAllegato = Convert.FromBase64String(allegato.FileByte)
        ElseIf Allegati.Count > 1 Then
            Dim FileZip = New MemoryStream()
            Using archive = New ZipArchive(FileZip, ZipArchiveMode.Create, leaveOpen:=True)
                For Each allegato In Allegati
                    Dim entry = archive.CreateEntry(allegato.FileName, CompressionLevel.Fastest)
                    Dim originalFileStream = New MemoryStream(Convert.FromBase64String(allegato.FileByte))
                    Using stream = entry.Open()
                        originalFileStream.CopyTo(stream)
                    End Using
                Next
            End Using
            FileZip.Position = 0
            FileName = "Allegati_" & Date.Now.ToString("yyyyMMdd_HHmmss") & ".zip"
            FileAllegato = FileZip.ToArray()
        End If
    End Sub


    Public Function DocumentiScrivi_APP(
            ByVal Documento As DocumentoPerScarico,
            ByRef unidParam As String,
            objParametri_server As AgronicaCoreParametri,
            objParametri_utenti As AgronicaCoreParametri
            ) As RispostaStandard

        Dim unid As String
        If unidParam <> "" Then
            unid = unidParam
        Else
            unid = Documento.guid
            unidParam = unid
        End If

        Dim cancellazione As Boolean = False


        If String.IsNullOrEmpty(unid) Then
            unid = Guid.NewGuid().ToString()
        Else
            cancellazione = True
        End If

        Dim result As New RispostaStandard

        result.RispostaStringa = unid
        result.RispostaOK = True

        Dim allegatiScrivi As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W

        ' cancellazione documento app
        If cancellazione Then
            allegatiScrivi.Cancella_DocumentoAPP(unid, objParametri_server)
        End If

        ' inserimento documento app
        If Not Documento.cancellato Then

            Dim FileName As String = ""
            Dim FileAllegato As Byte() = Nothing
            Allegati.ConvertiAllegati(Documento.Allegati, FileName, FileAllegato)

            Dim DocumentoID As String = unid & "|" & Documento.Documento_Cod
            Dim RicettaOperazioneID As String = ""
            Dim RicettaDestinazioneID As String = ""

            If Documento.Ricetta_Operazione_Cod <> 0 Then
                RicettaOperazioneID = unid & "|" & Documento.Ricetta_Operazione_Cod
            End If

            If Documento.Ricetta_Destinazione_Cod <> 0 Then
                RicettaDestinazioneID = unid & "|" & Documento.Ricetta_Destinazione_Cod
            End If

            result.RispostaOK = allegatiScrivi.Scrivi_DocumentoAPP(
                DocumentoID,
                Documento.Piva,
                Documento.Documento_Cod,
                Documento.ID_Tipologia,
                Documento.Descrizione,
                Documento.Data_Scadenza,
                Documento.Note,
                FileName,
                RicettaOperazioneID,
                RicettaDestinazioneID,
                FileAllegato,
                objParametri_server
            )

        End If

        Return result

    End Function


    ' restituisce il codice della tabella Allegati_Documenti
    Public Function SalvaAllegato(ByVal Piva As String, _
                                   ByVal CatCod As enum_CategorieDocumenti, _
                                   ByVal DesAllegato As String, _
                                   ByVal NomeFile As String, _
                                   ByVal Sottocartella As String, _
                                       ByVal Chiave1 As String, _
                                       ByVal Chiave2 As String, _
                                       ByVal Chiave3 As String, _
                                       ByVal Chiave4 As String, _
                                          ByVal Validita_inizio As String, _
                                          ByVal Validita_fine As String, _
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                             ) As Integer


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_BIZ.Allegati.SalvaAllegato()"

        Dim MessaggioErrore As String = ""

        Dim objEntita As AgronicaCoreScadenziario.Alert_Entita
        Dim objScriviEntita As AgronicaCoreScadenziario.Alert_Entita_W
        Dim objLeggiEntita As AgronicaCoreScadenziario.Alert_Entita_R
        Dim objAllegatiDocumenti As AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
        Dim objSeq As AgronicaCoreDataProvider.Agro_Sequenze

        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean

        Dim Allegati_Documenti_Cod As Integer

        Try

            objEntita = New AgronicaCoreScadenziario.Alert_Entita
            objScriviEntita = New AgronicaCoreScadenziario.Alert_Entita_W
            objAllegatiDocumenti = New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W

            objLeggiEntita = New AgronicaCoreScadenziario.Alert_Entita_R

            NomeFile = UtilityProvider.Agro_SQL_SaveText(NomeFile)
            Sottocartella = UtilityProvider.Agro_SQL_SaveText(Sottocartella)

            Dim strFiltro As String = "Allegati_Documenti_NomeFile='" & NomeFile & "' AND SottoCartella = '" & Sottocartella & "'"
            Dim dtDoc As DataTable
            dtDoc = objLeggiEntita.Leggi_con_documenti(0, strFiltro, "", objParametri)

            If dtDoc.Rows.Count = 0 Then

                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                 FlagTransazioneLocale, _
                                                                                 objParametri)
                ' forzati
                Dim Base, Top As Integer
                Base = 0
                Top = 2000000000

                Dim DataInizio As Date = AGRODATAINIZIO
                Dim DataFine As Date = AGRODATAFINE

                If IsDate(Validita_inizio) Then
                    DataInizio = CDate(Validita_inizio)
                End If

                If IsDate(Validita_fine) Then
                    DataFine = CDate(Validita_fine)
                End If

                'creo l'entita e il documento associato
                Dim objDoc As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
                objDoc.Scrivi(Piva, _
                              DesAllegato, _
                              CatCod, _
                              NomeFile, _
                              Nothing, Nothing, _
                              Sottocartella, _
                              DataInizio, DataFine, _
                              Allegati_Documenti_Cod, _
                              objParametri)


                objSeq = New AgronicaCoreDataProvider.Agro_Sequenze
                objEntita.ID_Alert_Entita = objSeq.NuovoId_Tabella("Alert_Entita", Base, Top, objParametri)
                objEntita.Allegati_Documenti_Cod = Allegati_Documenti_Cod

                Select Case CatCod

                    Case enum_CategorieDocumenti.Fattura_Emessa, _
                        enum_CategorieDocumenti.Conferimento, _
                        enum_CategorieDocumenti.DDT_Contabilizzato_Emesso, _
                        enum_CategorieDocumenti.DDT_Emesso, _
                        enum_CategorieDocumenti.NotaAccredito_Emessa, _
                        enum_CategorieDocumenti.Ordine_Vendita, _
                        enum_CategorieDocumenti.Ordine_Acquisto, _
                        enum_CategorieDocumenti.Preventivo_Emesso

                        objEntita.ID_Agenda = Chiave1
                        objEntita.TipoEntita_Cod = enum_TipoEntita.Impresa

                    Case enum_CategorieDocumenti.Bilancio, _
                  enum_CategorieDocumenti.ScadenziarioPagamentiClienti, _
                  enum_CategorieDocumenti.ScadenziarioPagamentiFornitori, _
                  enum_CategorieDocumenti.LiquidazioneIVA, _
                  enum_CategorieDocumenti.RegistriIVA_Vendite, _
                  enum_CategorieDocumenti.RegistriIVA_Acquisti, _
                  enum_CategorieDocumenti.RegistroCorrispettivi_Vendita, _
                  enum_CategorieDocumenti.PresentazioneRiba, _
                        enum_CategorieDocumenti.LibroGiornale

                        objEntita.TipoEntita_Cod = enum_TipoEntita.Impresa

                    Case enum_CategorieDocumenti.Magazzino_Giacenze, _
                        enum_CategorieDocumenti.Magazzino_Movimenti, _
                        enum_CategorieDocumenti.Magazzino_Fertilizzanti, _
                        enum_CategorieDocumenti.Magazzino_Fitosanitari

                        objEntita.Sa_Cod = Chiave1
                        'objEntita.Fabbricato_cod = Chiave2
                        objEntita.TipoEntita_Cod = enum_TipoEntita.Fabbricato


                    Case enum_CategorieDocumenti.Biologico_MateriePrime, _
                       enum_CategorieDocumenti.Biologico_Vendite, _
                       enum_CategorieDocumenti.Biologico_Preparazioni

                        objEntita.TipoEntita_Cod = enum_TipoEntita.Impresa

                    Case enum_CategorieDocumenti.AccettazioneDaDiversi_Bolla, _
                        enum_CategorieDocumenti.AccettazioneDaDiversi_CertificatoPomodoro, _
                        enum_CategorieDocumenti.AccettazioneDaDiversi_ReportImballi, _
                        enum_CategorieDocumenti.AccettazioneDaDiversi_Report
                        objEntita.TipoEntita_Cod = enum_TipoEntita.Impresa

                    Case enum_CategorieDocumenti.Cantina_ConsistenzeEnologiche, _
                        enum_CategorieDocumenti.Cantina_RegistroVinificazione, _
                        enum_CategorieDocumenti.Cantina_RegistroCommercializzazione, _
                        enum_CategorieDocumenti.Cantina_RegistroImbottigliamento, _
                        enum_CategorieDocumenti.Cantina_RegistroSpumanti, _
                        enum_CategorieDocumenti.Cantina_RegistroFrizzanti

                        objEntita.TipoEntita_Cod = enum_TipoEntita.Impresa

                    Case enum_CategorieDocumenti.PUA
                        objEntita.PUA_Cod = Chiave1
                        objEntita.TipoEntita_Cod = enum_TipoEntita.PUA

                    Case enum_CategorieDocumenti.PianoConcimazione

                        objEntita.PC_Testata_Cod = Chiave1
                        objEntita.Piva = Chiave2
                        objEntita.TipoEntita_Cod = enum_TipoEntita.PianoConcimazione

                    Case enum_CategorieDocumenti.PrecisionFarming_FileBordoMacchina,
                         enum_CategorieDocumenti.PrecisionFarming_MappaPrescrizione,
                         enum_CategorieDocumenti.PrecisionFarming_MappaProduzione

                        objEntita.Ricetta_Operazione_cod = Chiave1
                        objEntita.TipoEntita_Cod = enum_TipoEntita.Ricetta_Destinazione

                    Case Else
                        objEntita.TipoEntita_Cod = enum_TipoEntita.Impresa

                End Select


                objEntita.Piva = Piva
                objEntita.PivaSuperUser = objParametri.PivaSuperUser

                'scrivo l alert entita
                objScriviEntita.Scrivi(objEntita, Validita_inizio, Validita_fine, objParametri)

                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            End If

        Catch ex As Exception
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return Allegati_Documenti_Cod

    End Function


    ' restituisce il codice della tabella Allegati_Documenti
    Public Function SalvaAllegatoSchedaCampagna(ByVal Piva As String, _
                                   ByVal CatCod As enum_CategorieDocumenti, _
                                   ByVal DesAllegato As String, _
                                   ByVal NomeFile As String, _
                                   ByVal Sottocartella As String, _
                                   ByVal Matrice_Variabili(,) As String, _
                                          ByVal Validita_inizio As String, _
                                          ByVal Validita_fine As String, _
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                             ) As Integer


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_BIZ.Allegati.Scrivi()"

        Dim MessaggioErrore As String = ""

        Dim objEntita As AgronicaCoreScadenziario.Alert_Entita
        Dim objScriviEntita As AgronicaCoreScadenziario.Alert_Entita_W
        Dim objLeggiEntita As AgronicaCoreScadenziario.Alert_Entita_R
        Dim objAllegatiDocumenti As AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
        Dim objSeq As AgronicaCoreDataProvider.Agro_Sequenze

        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean

        Dim Allegati_Documenti_Cod As Integer

        Piva = Matrice_Variabili(0, 0)

        Try

            objEntita = New AgronicaCoreScadenziario.Alert_Entita
            objScriviEntita = New AgronicaCoreScadenziario.Alert_Entita_W
            objAllegatiDocumenti = New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W

            objLeggiEntita = New AgronicaCoreScadenziario.Alert_Entita_R

            Dim strFiltro As String = "Allegati_Documenti_NomeFile='" & UtilityProvider.Agro_SQL_SaveText(NomeFile) & "' AND SottoCartella = '" & UtilityProvider.Agro_SQL_SaveText(Sottocartella) & "'"
            Dim dtDoc As DataTable
            dtDoc = objLeggiEntita.Leggi_con_documenti(0, strFiltro, "", objParametri)

            If dtDoc.Rows.Count = 0 Then

                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                 FlagTransazioneLocale, _
                                                                                 objParametri)
                ' forzati
                Dim Base, Top As Integer
                Base = 0
                Top = 2000000000

                Dim DataInizio As Date = AGRODATAINIZIO
                Dim DataFine As Date = AGRODATAFINE

                If IsDate(Validita_inizio) Then
                    DataInizio = CDate(Validita_inizio)
                End If

                If IsDate(Validita_fine) Then
                    DataFine = CDate(Validita_fine)
                End If

                'creo l'entita e il documento associato
                Dim objDoc As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
                objDoc.Scrivi(Piva,
                              DesAllegato,
                              CatCod,
                              NomeFile,
                              Nothing, Nothing,
                              Sottocartella,
                              DataInizio, DataFine,
                              Allegati_Documenti_Cod,
                              objParametri)


                objSeq = New AgronicaCoreDataProvider.Agro_Sequenze

                objEntita.Allegati_Documenti_Cod = Allegati_Documenti_Cod

                objEntita.TipoEntita_Cod = enum_TipoEntita.Impianto
                objEntita.Piva = Piva
                objEntita.PivaSuperUser = objParametri.PivaSuperUser

                Dim i As Integer
                If Not IsNothing(Matrice_Variabili) Then
                    For i = 0 To UBound(Matrice_Variabili, 2)

                        objEntita.ID_Alert_Entita = objSeq.NuovoId_Tabella("Alert_Entita", Base, Top, objParametri)

                        objEntita.Piva = Matrice_Variabili(0, i)
                        objEntita.Sa_Cod = Matrice_Variabili(1, i)
                        objEntita.Appezza = Matrice_Variabili(2, i)
                        objEntita.Id_Imp = Matrice_Variabili(3, i)

                        'scrivo l alert entita
                        objScriviEntita.Scrivi(objEntita, Validita_inizio, Validita_fine, objParametri)

                    Next
                End If

                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            End If
        Catch ex As Exception
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return Allegati_Documenti_Cod

    End Function


    '' restituisce il codice della tabella Allegati_Documenti
    'Public Function SalvaAllegatoChecklistGlobalGap(ByVal Piva As String,
    '                                                ByVal CatCod As enum_CategorieDocumenti,
    '                                                ByVal DesAllegato As String,
    '                                                ByVal NomeFile As String,
    '                                                ByVal Sottocartella As String,
    '                                                ByVal File_Allegato As Byte(),
    '                                                ByVal Validita_inizio As String,
    '                                                ByVal Validita_fine As String,
    '                                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
    '                                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    '                                                ) As Integer


    '    Dim NomeRoutine As String = "AgronicaCoreScadenziario_BIZ.Allegati.SalvaAllegatoChecklistGlobalGap()"

    '    Dim MessaggioErrore As String = ""

    '    Dim objEntita As AgronicaCoreScadenziario.Alert_Entita
    '    Dim objScriviEntita As AgronicaCoreScadenziario.Alert_Entita_W
    '    Dim objLeggiEntita As AgronicaCoreScadenziario.Alert_Entita_R
    '    Dim objAllegatiDocumenti As AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
    '    Dim objSeq As AgronicaCoreDataProvider.Agro_Sequenze

    '    Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean

    '    Dim Allegati_Documenti_Cod As Integer

    '    'Piva = Matrice_Variabili(0, 0)

    '    Try

    '        objEntita = New AgronicaCoreScadenziario.Alert_Entita
    '        objScriviEntita = New AgronicaCoreScadenziario.Alert_Entita_W
    '        objAllegatiDocumenti = New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W

    '        objLeggiEntita = New AgronicaCoreScadenziario.Alert_Entita_R

    '        Dim strFiltro As String = "Allegati_Documenti_NomeFile='" & UtilityProvider.Agro_SQL_SaveText(NomeFile) & "' AND SottoCartella = '" & UtilityProvider.Agro_SQL_SaveText(Sottocartella) & "'"
    '        Dim dtDoc As DataTable
    '        dtDoc = objLeggiEntita.Leggi_con_documenti(0, strFiltro, "", objParametri_Server)

    '        If dtDoc.Rows.Count = 0 Then

    '            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
    '                                                                                    FlagTransazioneLocale,
    '                                                                                    objParametri_Server)
    '            ' forzati
    '            Dim Base, Top As Integer
    '            Base = 0
    '            Top = 2000000000

    '            Dim DataInizio As Date = AGRODATAINIZIO
    '            Dim DataFine As Date = AGRODATAFINE

    '            If IsDate(Validita_inizio) Then
    '                DataInizio = CDate(Validita_inizio)
    '            End If

    '            If IsDate(Validita_fine) Then
    '                DataFine = CDate(Validita_fine)
    '            End If

    '            Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
    '            Dim DTSalvaAllegato As New DataTable
    '            Dim SalvaAllegato As Integer = 0 'File System Default

    '            DTSalvaAllegato = objImpostazioni.Leggi2(2, objParametri_Server.SuperUserUsername, enum_Impostazioni_Utenti.SUPERUSER_DOCUMENTALE_SALVA_ALLEGATO_SU_DB, "", "", objParametri_Utenti)

    '            If Not IsNothing(DTSalvaAllegato) AndAlso DTSalvaAllegato.Rows.Count > 0 Then
    '                SalvaAllegato = CInt(DTSalvaAllegato.Rows(0).Item("Impostazione_Valore_1"))
    '            End If

    '            If SalvaAllegato = 1 Then
    '                ' il file esportato viene eliminato dal disco
    '                Dim outputFilePath As String = Path.Combine(Sottocartella, NomeFile)
    '                System.IO.File.Delete(outputFilePath)
    '            End If


    '            'creo l'entita e il documento associato
    '            Dim objDoc As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
    '            objDoc.Scrivi(Piva,
    '                          DesAllegato,
    '                          CatCod,
    '                          NomeFile,
    '                          Nothing, Nothing,
    '                          Sottocartella,
    '                          DataInizio, DataFine,
    '                          Allegati_Documenti_Cod,
    '                          objParametri_Server,,,,,,,,,,,, File_Allegato,
    '                          SalvaAllegato:=SalvaAllegato)


    '            objSeq = New AgronicaCoreDataProvider.Agro_Sequenze

    '            objEntita.Allegati_Documenti_Cod = Allegati_Documenti_Cod

    '            objEntita.TipoEntita_Cod = enum_TipoEntita.Impianto
    '            objEntita.Piva = Piva
    '            objEntita.PivaSuperUser = objParametri_Server.PivaSuperUser

    '            'Dim i As Integer
    '            'If Not IsNothing(Matrice_Variabili) Then
    '            '    For i = 0 To UBound(Matrice_Variabili, 2)

    '            objEntita.ID_Alert_Entita = objSeq.NuovoId_Tabella("Alert_Entita", Base, Top, objParametri_Server)

    '            objEntita.Piva = Piva
    '            objEntita.Sa_Cod = 0
    '            objEntita.Appezza = 0
    '            objEntita.Id_Imp = 0

    '            'scrivo l alert entita
    '            objScriviEntita.Scrivi(objEntita, Validita_inizio, Validita_fine, objParametri_Server)

    '            '    Next
    '            'End If

    '            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

    '        End If
    '    Catch ex As Exception
    '        If Not objParametri_Server.objTransazione Is Nothing Then
    '            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
    '        End If

    '        Scrivi_LOG(objParametri_Server, NomeRoutine, ex.Message)

    '    Finally
    '        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

    '    End Try

    '    Return Allegati_Documenti_Cod

    'End Function


    Public Function Leggi_File_Da_AllegatoCod(ByVal piva As String, ByVal allegati_documenti_cod As Integer, ByRef objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim dt As New DataTable

        'Lettura Elenco
        Dim objElenco As New AgronicaCoreScadenziario.Alert_Elenco_R
        dt = objElenco.Leggi_Allegato(piva, allegati_documenti_cod, objParametri_Server)

        If dt.Rows.Count > 0 Then

            Dim bFS As Boolean = True

            'Controllo Tipo Salvataggio
            If Not IsDBNull(dt(0).Item("File_Allegato_DB")) Then
                If dt(0).Item("File_Allegato_DB").ToString = "System.Byte[]" Then
                    'Salvataggio su DB
                    bFS = False
                End If
            End If

            If bFS Then

                Dim LeggiConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
                Dim Percorso As String = dt_Conf.Rows(0).Item("Valore")

                Percorso = FileSystemHelper.AggiungiSlashSeNonEsiste(Percorso)

                If dt(0).Item("Sottocartella") <> "" Then
                    Percorso = FileSystemHelper.AggiungiSlashSeNonEsiste(Percorso) & FileSystemHelper.AggiungiSlashSeNonEsiste(dt(0).Item("Sottocartella"))
                End If

                Dim File_Name As String
                File_Name = FileSystemHelper.AggiungiSlashSeNonEsiste(Percorso) & dt(0).Item("Allegati_Documenti_NomeFile")

                Dim fileByteArray As Byte()
                'Salvataggio su FS
                fileByteArray = My.Computer.FileSystem.ReadAllBytes(File_Name)

                'Aggiornamento DT
                dt(0).Item("File_Allegato_DB") = fileByteArray

            End If

        End If

        Return dt

    End Function

End Class
