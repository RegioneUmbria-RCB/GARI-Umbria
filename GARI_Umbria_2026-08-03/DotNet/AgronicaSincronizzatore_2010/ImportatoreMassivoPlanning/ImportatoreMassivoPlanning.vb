Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class ImportatoreMassivoPlanning

    Private objParametri_SuperServer As AgronicaCoreParametri
    Private objParametri_Server As AgronicaCoreParametri
    Private objParametri_Utenti As AgronicaCoreParametri

    Private _hostSmtp As String = ""
    Private _mittente As String = ""
    Private _destinatari As String = ""

    ''Oggetti utilizzati
    Private objProgrammazione As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
    Private objImprese_Codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
    Private Utenti_Read As AgronicaCoreUtentiDAL.Utenti_Read
    Private Utenti_CodiciGiasPRO_R As AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
    Private Configurazione_Siti_R As AgronicaCoreVarieDAL.Configurazione_Siti_R
    Private objLog As AgronicaCoreDataProvider.LogProvider
    Private Configurazione_Servizi_R As AgronicaCoreVarieDAL.Configurazione_Servizi_R

    ''Da ricavare da Configurazione_Servizio
    Private SuperUserUsername As String
    Private SuperUserPassword As String
    Private SuperUserPiva As String
    Private Codice_Chiave_Cliente As Integer
    Private Piva_Padre As String
    Private LogFileName As String
    Private LogDirectory As String
    Private LogDescrizioneUtente As String
    Private DirectoryFileImportazioni As String
    Private DirectoryFileEsportazioni As String
    Private Parametri_Extra As String

    ''Parametri fissi impostati dalla classe
    Private ProgressivoGIAS As String
    Private LinkWSImportaGIAS As String

    Private ASG_Utente_Username As String
    Private ASG_Utente_Password As String

    Private TipoImportFascicolo As enum_TipoImportFascicolo

    Private RicercaUltimaSchedaDB As enum_RicercaUltimaSchedaDB

    Private MaxImpreseDaImportare As Integer

    Public Enum enum_TipoImportFascicolo
        ImportFascicoloAgea = 0
        ImportFascicoloAgeaRealTime = 1
    End Enum

    Public Enum enum_RicercaUltimaSchedaDB
        Tutte = 0
        SoloImportateAutomaticamente = 1
    End Enum

    Public Sub New(ByVal _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio,
            ByVal _ObjParametri_SuperServer As AgronicaCoreParametri,
            ByVal _ObjParametri_Server As AgronicaCoreParametri,
            ByVal _ObjParametri_Utenti As AgronicaCoreParametri)

        Parametri_Extra = _Configurazione_Servizio.Parametri_Extra
        LogDirectory = _Configurazione_Servizio.DirectoryLOG
        'Configurazione_Servizio = _Configurazione_Servizio leggo solo i parametri che mi servono 
        objParametri_SuperServer = _ObjParametri_SuperServer
        objParametri_Server = _ObjParametri_Server
        objParametri_Utenti = _ObjParametri_Utenti

        _mittente = _Configurazione_Servizio.Mittente
        _destinatari = _Configurazione_Servizio.Destinatari

        InizializzoOggettiCore()

        ObjParametriExtraInizializza()

        objParametri_Server.FinestraTemporaleInizio = AGRODATAINIZIO
        objParametri_Server.FinestraTemporaleFine = AGRODATAFINE

    End Sub

    Private Sub ObjParametriExtraInizializza()

        Dim obV As JObject = JsonConvert.DeserializeObject(Parametri_Extra)

        ' Utente / Password

        ASG_Utente_Username = obV("username")
        ASG_Utente_Password = obV("password")

        ' tipoImportFascicolo

        Dim paramTipoImportFascicolo As String = ""
        Try
            paramTipoImportFascicolo = obV("tipoImportFascicolo").ToString()
        Catch
            paramTipoImportFascicolo = enum_TipoImportFascicolo.ImportFascicoloAgea.ToString("D")
        End Try

        Select Case paramTipoImportFascicolo
            Case enum_TipoImportFascicolo.ImportFascicoloAgeaRealTime
                TipoImportFascicolo = enum_TipoImportFascicolo.ImportFascicoloAgeaRealTime
            Case Else
                TipoImportFascicolo = enum_TipoImportFascicolo.ImportFascicoloAgea
        End Select

        ' ricercaUltimaSchedaDB

        Dim paramRicercaUltimaSchedaDB As String = ""
        Try
            paramRicercaUltimaSchedaDB = obV("ricercaUltimaSchedaDB").ToString()
        Catch
            paramRicercaUltimaSchedaDB = enum_RicercaUltimaSchedaDB.Tutte.ToString("D")
        End Try

        Select Case paramRicercaUltimaSchedaDB
            Case enum_RicercaUltimaSchedaDB.SoloImportateAutomaticamente
                RicercaUltimaSchedaDB = enum_RicercaUltimaSchedaDB.SoloImportateAutomaticamente
            Case Else
                RicercaUltimaSchedaDB = enum_RicercaUltimaSchedaDB.Tutte
        End Select

        ' maxImpreseDaImportare

        Dim paramMaxImpreseDaImportare = 0
        Try
            paramMaxImpreseDaImportare = CInt(obV("maxImpreseDaImportare"))
        Catch
            paramMaxImpreseDaImportare = 0
        End Try

        MaxImpreseDaImportare = paramMaxImpreseDaImportare

        ' Log File Name

        LogFileName = Date.Now.Year & "-" & Date.Now.Month.ToString("D2") & "-" & Date.Now.Day.ToString("D2") & " Importazione Massiva Planning.txt"

        ' Parametri Server

        objParametri_Server.UtenteUsername = ASG_Utente_Username

    End Sub

    Private Sub InizializzoOggettiCore()

        objLog = New AgronicaCoreDataProvider.LogProvider

    End Sub

    Public Sub AvviaImportazione()
        Dim objImprese_R_Dal As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim objImprese_W_Dal As New AgronicaCoreAnagrafeDAL.Imprese_Write
        Dim DTImprese As DataTable = objImprese_R_Dal.Leggi_CUAA_ImpreseFoglia_TOPN("", "", Nothing, 1, Nothing, 1000, "", " i.dataVariazioneAzienda ASC ", objParametri_Server)
        Dim obj As New Importatore_UMBRIA.AGEA_UMBRIA_Utility
        Dim NoFascicoloSuAGEA As New List(Of ImpreseResult)

        Dim listImpreseImportate As New List(Of ImpreseResult)
        Dim listImpreseUltimoFascicoloGiaPresente As New List(Of ImpreseResult)
        Dim listImpreseErroreImportazione As New List(Of ImpreseResult)
        Dim listImpreseNoConsistenza As New List(Of ImpreseResult)

        Try

            Dim DTImpreseMod = obj.CaricaCuaaModificata(0, "", -2, objParametri_Server)

            Dim deserializedObject As List(Of CuaaModificata) = JsonConvert.DeserializeObject(Of List(Of CuaaModificata))(DTImpreseMod)

            If Not IsNothing(deserializedObject) Then

                deserializedObject = deserializedObject.Where(Function(z) z.Istat_provincia = "054" OrElse z.Istat_provincia = "055").ToList()

                For Each CuaaUmbria In deserializedObject
                    Dim R As DataRow = DTImprese.NewRow
                    R("PIVA") = ""
                    R("rag_soc") = ""
                    R("CUAA") = CuaaUmbria.CUAA
                    DTImprese.Rows.InsertAt(R, 0)
                Next

            End If

            logga("Avvio importazione, totale imprese da importare:" & CStr(DTImprese.Rows.Count))

            If MaxImpreseDaImportare > 0 Then
                logga("Max imprese da importare:" & MaxImpreseDaImportare.ToString())
            End If

        Catch ex As Exception

            logga("Errore AvviaImportazione: " & ex.Message)

        End Try

        logga(StrDup(80, "-"))

        For index As Integer = 0 To DTImprese.Rows.Count

            Dim startTime As Long = DateTime.Now.Ticks
            Dim impresaResult As New ImpreseResult

            If MaxImpreseDaImportare > 0 AndAlso listImpreseImportate.Count >= MaxImpreseDaImportare Then
                logga("Raggiunto numero massimo di imprese da importare")
                logga(StrDup(80, "="))
                Exit For
            End If

            Try

                Dim importazioneCompletata = False
                Dim dr = DTImprese.Rows(index)
                impresaResult.Piva = dr("Piva")
                impresaResult.RagioneSociale = dr("Rag_Soc")
                impresaResult.Cuaa = dr("CUAA")

                If impresaResult.Piva = "" AndAlso impresaResult.Cuaa <> "" Then
                    impresaResult.Piva = objImprese_Codici.Piva_from_CUAA(impresaResult.Cuaa, objParametri_Server)
                End If

                Dim strRis As String = "1"
                Dim strErr As String = ""

                impresaResult.Ris = strRis
                impresaResult.Err = strErr

                ' Lettura fascicolo da AGEA

                Dim ultimaSchedaAGEA As SchedaFascicolo = Nothing
                Dim msgOrigineUltimaSchedaAGEA = ""
                If TipoImportFascicolo = enum_TipoImportFascicolo.ImportFascicoloAgeaRealTime Then
                    ultimaSchedaAGEA = leggiUltimaSchedaAGEA_RealTime(impresaResult)
                    msgOrigineUltimaSchedaAGEA = " Ultima scheda Agea Real-Time "
                Else
                    ultimaSchedaAGEA = leggiUltimaSchedaAGEA(impresaResult)
                    msgOrigineUltimaSchedaAGEA = " Ultima scheda Agea "
                End If

                If ultimaSchedaAGEA IsNot Nothing Then
                    impresaResult.UltimoFascicoloAGEA = ultimaSchedaAGEA
                    logga(dr("CUAA") & msgOrigineUltimaSchedaAGEA & ultimaSchedaAGEA.NumeroValidazione & " " & ultimaSchedaAGEA.DataValidazione.ToShortDateString())
                End If

                ' Lettura fascicolo sul DB locale (planning + allegato)

                Dim soloImportatiAutomaticamente = (RicercaUltimaSchedaDB = enum_RicercaUltimaSchedaDB.SoloImportateAutomaticamente)
                Dim ultimaSchedaDB = LeggiUltimaSchedaDB(impresaResult, soloImportatiAutomaticamente)
                If ultimaSchedaDB IsNot Nothing Then
                    Dim msgRicercaUltimaSchedaDB = ""
                    If soloImportatiAutomaticamente Then
                        msgRicercaUltimaSchedaDB = " Ultima scheda DB (solo import. autom.) "
                    Else
                        msgRicercaUltimaSchedaDB = " Ultima scheda DB "
                    End If
                    impresaResult.UltimoFascicoloImportato = ultimaSchedaDB
                    logga(dr("CUAA") & msgRicercaUltimaSchedaDB & ultimaSchedaDB.NumeroValidazione & " " & ultimaSchedaDB.DataValidazione.ToShortDateString())
                End If

                '--------------------------------------------------------------------------------
                ' Caso 1 - Nuova impresa
                '--------------------------------------------------------------------------------

                If impresaResult.Piva = "" AndAlso ultimaSchedaAGEA IsNot Nothing Then

                    impresaResult.importata = importaDaFascicoloAGEA(impresaResult, ultimaSchedaAGEA, strRis, strErr)
                    impresaResult.Ris = strRis
                    impresaResult.Err = strErr

                    Dim endTime As Long = DateTime.Now.Ticks
                    impresaResult.tempoTotale = (endTime - startTime) / TimeSpan.TicksPerSecond

                    If impresaResult.importata Then
                        listImpreseImportate.Add(impresaResult)
                    Else
                        If impresaResult.Ris = "No Consistenza" Then
                            listImpreseNoConsistenza.Add(impresaResult)
                        Else
                            listImpreseErroreImportazione.Add(impresaResult)
                        End If
                    End If

                    importazioneCompletata = True
                    impresaResult.Ris = "NUOVA IMPRESA " & impresaResult.Ris

                End If

                '--------------------------------------------------------------------------------
                ' Caso 2 - Non aggiornato: ultima scheda AGEA non valorizzata
                '--------------------------------------------------------------------------------

                If Not importazioneCompletata AndAlso ultimaSchedaAGEA Is Nothing Then

                    Dim endTime As Long = DateTime.Now.Ticks
                    impresaResult.tempoTotale = (endTime - startTime) / TimeSpan.TicksPerSecond
                    impresaResult.Ris = "Not importazioneCompletata AndAlso ultimaSchedaAGEA Is Nothing"
                    NoFascicoloSuAGEA.Add(impresaResult)
                    importazioneCompletata = True
                    impresaResult.Ris = "2 " & impresaResult.Ris

                End If

                '--------------------------------------------------------------------------------
                ' Caso 3 - Aggiornamento: ultima scheda DB non valorizzata
                '--------------------------------------------------------------------------------

                If Not importazioneCompletata AndAlso ultimaSchedaDB Is Nothing AndAlso ultimaSchedaAGEA IsNot Nothing Then

                    impresaResult.importata = importaDaFascicoloAGEA(impresaResult, ultimaSchedaAGEA, strRis, strErr)
                    impresaResult.Ris = strRis
                    impresaResult.Err = strErr

                    Dim endTime As Long = DateTime.Now.Ticks
                    impresaResult.tempoTotale = (endTime - startTime) / TimeSpan.TicksPerSecond

                    If impresaResult.importata Then
                        listImpreseImportate.Add(impresaResult)
                    Else
                        If impresaResult.Ris = "No Consistenza" Then
                            listImpreseNoConsistenza.Add(impresaResult)
                        Else
                            listImpreseErroreImportazione.Add(impresaResult)
                        End If
                    End If

                    importazioneCompletata = True
                    impresaResult.Ris = "3 " & impresaResult.Ris

                End If

                '--------------------------------------------------------------------------------
                ' Caso 4 - Non aggiornato: ultima scheda DB = ultima scheda AGEA
                '--------------------------------------------------------------------------------

                If Not importazioneCompletata AndAlso ultimaSchedaDB IsNot Nothing AndAlso ultimaSchedaDB.NumeroValidazione = ultimaSchedaAGEA.NumeroValidazione Then
                    Dim endTime As Long = DateTime.Now.Ticks
                    impresaResult.tempoTotale = (endTime - startTime) / TimeSpan.TicksPerSecond
                    impresaResult.Ris = "Ultimo Fascicolo Già presente"
                    listImpreseUltimoFascicoloGiaPresente.Add(impresaResult)

                    importazioneCompletata = True
                    impresaResult.Ris = "4 " & impresaResult.Ris
                End If

                '--------------------------------------------------------------------------------
                ' Caso 5 - Aggiornamento: ultima scheda DB <> ultima scheda AGEA
                '--------------------------------------------------------------------------------

                If Not importazioneCompletata AndAlso ultimaSchedaDB.NumeroValidazione <> ultimaSchedaAGEA.NumeroValidazione AndAlso ultimaSchedaAGEA.DataValidazione > ultimaSchedaDB.DataValidazione Then

                    impresaResult.importata = importaDaFascicoloAGEA(impresaResult, ultimaSchedaAGEA, strRis, strErr)
                    impresaResult.Ris = strRis
                    impresaResult.Err = strErr

                    Dim endTime As Long = DateTime.Now.Ticks
                    impresaResult.tempoTotale = (endTime - startTime) / TimeSpan.TicksPerSecond

                    If impresaResult.importata Then
                        listImpreseImportate.Add(impresaResult)
                    Else
                        If impresaResult.Ris = "No Consistenza" Then
                            listImpreseNoConsistenza.Add(impresaResult)
                        Else
                            listImpreseErroreImportazione.Add(impresaResult)
                        End If
                    End If

                    importazioneCompletata = True
                    impresaResult.Ris = "5 " & impresaResult.Ris

                End If

                '--------------------------------------------------------------------------------

                objImprese_W_Dal.Modifica_dataVariazioneAzienda(impresaResult.Piva, DateTime.Now, objParametri_Server)

                logga("Esito Importazione")
                logga(impresaResult.ToString)
                logConteggiFineImportazioneImpresa(DTImprese, listImpreseImportate, index)

                logConteggiFineImportazioneImpresa(DTImprese, listImpreseImportate, index)
            Catch ex As Exception

                logga("ERRORE: Esito Importazione -> " & ex.Message)

                If impresaResult.Piva IsNot Nothing AndAlso impresaResult.Piva <> "" Then
                    Dim endTime As Long = DateTime.Now.Ticks
                    impresaResult.tempoTotale = (endTime - startTime) / TimeSpan.TicksPerSecond
                    impresaResult.importata = False
                    impresaResult.Err = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)
                    listImpreseErroreImportazione.Add(impresaResult)
                    logga(impresaResult.ToString)
                End If

                logConteggiFineImportazioneImpresa(DTImprese, listImpreseImportate, index)

            End Try

        Next

    End Sub

    Private Sub logConteggiFineImportazioneImpresa(DTImprese As DataTable,
                                                   listImpreseImportate As List(Of ImpreseResult),
                                                   index As Integer)
        Dim impreseTrattate = index + 1

        logga(String.Format("Avanzamento Totale:{0}% ({1}/{2})",
                            Math.Round(CDec(impreseTrattate * 100 / DTImprese.Rows.Count), 2),
                            impreseTrattate,
                            DTImprese.Rows.Count))

        If MaxImpreseDaImportare > 0 Then
            logga(String.Format("Avanzamento Imprese Importate:{0}% ({1}/{2})",
                                Math.Round(CDec(listImpreseImportate.Count * 100 / MaxImpreseDaImportare), 2),
                                listImpreseImportate.Count,
                                MaxImpreseDaImportare))
        End If

        logga(StrDup(80, "-"))

    End Sub


    Private Function importaDaFascicoloAGEA(impresa As ImpreseResult,
                                            fascicolo As SchedaFascicolo,
                                            ByRef strRes As String,
                                            ByRef strErr As String) As Boolean

        If TipoImportFascicolo = enum_TipoImportFascicolo.ImportFascicoloAgeaRealTime Then

            Dim fascicolo_agea_rt As New Importazione_Agea_BIZ.Import_Agea

            Dim esito_agea_rt = False

            Try

                fascicolo_agea_rt.importa_CUAA(impresa.Cuaa,
                                               strRes,
                                               True,
                                               True,
                                               True,
                                               True,
                                               True,
                                               ProgressivoGIAS,
                                               ASG_Utente_Password,
                                               objParametri_Server,
                                               objParametri_Utenti,
                                               "",
                                               importatoAutomaticamente:=True)

                esito_agea_rt = True

            Catch ex As Exception

                esito_agea_rt = False

            End Try

            Return esito_agea_rt

        Else

            Return Importa_Impresa(impresa.Cuaa,
                                   fascicolo.NumeroValidazione,
                                   fascicolo.DataValidazione,
                                   True,
                                   True,
                                   True,
                                   True,
                                   ASG_Utente_Username,
                                   ASG_Utente_Password,
                                   ProgressivoGIAS,
                                   strRes,
                                   objParametri_Server,
                                   objParametri_Utenti,
                                   "")

        End If

    End Function

    Private Function leggiUltimaSchedaAGEA_RealTime(impresa As ImpreseResult) As SchedaFascicolo

        Dim fascicolo_agea_rt As New Importazione_Agea_BIZ.Import_Agea

        Dim numeroValidazione = ""
        Dim dataValidazione = AGRODATAINIZIO

        fascicolo_agea_rt.leggiSchedaFascicolo_CUAA(impresa.Cuaa,
                                                    objParametri_Server,
                                                    numeroValidazione,
                                                    dataValidazione)
        If numeroValidazione <> "" Then
            Return New SchedaFascicolo() With {
                .NumeroValidazione = numeroValidazione,
                .DataValidazione = dataValidazione
            }
        End If

        Return Nothing

    End Function

    Private Function leggiUltimaSchedaAGEA(impresaResult As ImpreseResult) As SchedaFascicolo

        Dim DtSchede = CreaDTSchede()
        Forza_Scarico_Fascicolo_UMBRIA(impresaResult.Cuaa)
        Popola_Dt_Schede_UMBRIA(impresaResult.Cuaa, AGRODATAINIZIO, AGRODATAFINE, DtSchede)

        If DtSchede.Rows.Count > 0 Then
            DtSchede.DefaultView.Sort = "Validita_Inizio DESC"
            DtSchede = DtSchede.DefaultView.ToTable

            Return New SchedaFascicolo() With {
                .NumeroValidazione = DtSchede.Rows(0)("Scheda"),
                .DataValidazione = DtSchede.Rows(0)("Validita_Inizio")
            }

        End If

        Return Nothing

    End Function

    Private Function LeggiUltimaSchedaDB(impresa As ImpreseResult, soloImportatiAutomaticamente As Boolean) As SchedaFascicolo

        Dim filtroAggiuntivo = String.Format(" {0} {1} ",
                                             "Allegati_Documenti_Numero <> ''",
                                             IIf(soloImportatiAutomaticamente, "AND tt.importato_automaticamente = 1", ""))

        Dim DTPlanning = objProgrammazione.Leggi("",
                                                 0,
                                                 impresa.Piva,
                                                 "",
                                                 0,
                                                 AGRODATAINIZIO,
                                                 AGRODATAFINE,
                                                 enum_TipoPianificazione.Pianificazione_Annuale,
                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                 filtroAggiuntivo,
                                                 " Validazione_Data DESC ",
                                                 objParametri_Server)

        If DTPlanning.Rows.Count > 0 Then

            Return New SchedaFascicolo() With {
                .DataValidazione = DTPlanning.Rows(0)("Validazione_Data"),
                .NumeroValidazione = DTPlanning.Rows(0)("Allegati_Documenti_Numero")
            }

        End If

        Return Nothing

    End Function

    Private Function Importa_Impresa(ByVal Cuaa As String,
                                    ByVal Num_Scheda As String,
                                     ByVal DataValidazione As Date,
                                           ByVal Flag_ImportaAnagrafica As Boolean,
                                           ByVal Flag_ImportaMacchine As Boolean,
                                           ByVal Flag_ImportaCatasto As Boolean,
                                           ByVal Flag_ImportaPianoColturale As Boolean,
                                           ByVal Utente_Username As String,
                                           ByVal Utente_Password As String,
                                           ByVal ProgressivoGIAS As Integer,
                                           ByRef Messaggio As String,
                                           ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef Piva_Padre As String) As Boolean

        Dim import_umbria As New Importatore_UMBRIA.AGEA_UMBRIA_Utility

        Dim Fascicolo_Umbria As String
        Dim strErr As String = ""
        Dim statoWS As Boolean
        Dim messaggioWS As String = ""
        Dim piva As String = ""

        Dim OrigineOpr As String
        Dim allegatoDocumentoCod As Integer
        Dim xml_Response As String

        Dim esisteFascicolo As Boolean = import_umbria.FascicoloLeggiWSMemorizza_UMBRIA(
                            False,
                            0,
                            objParametri_Server,
                            Fascicolo_Umbria,
                            messaggioWS,
                            statoWS,
                            strErr,
                            piva,
                            Cuaa,
                            Num_Scheda,
                            DataValidazione,
                            OrigineOpr,
                            allegatoDocumentoCod,
                            xml_Response
                        )

        Dim importatore As New Importatore_UMBRIA.Importatore

        If Flag_ImportaAnagrafica Then

            If Fascicolo_Umbria <> "" Then

                Dim ragSoc As String = ""
                Dim Indirizzo As String = ""
                Dim Cap As String = ""
                Dim Ista_Prov As String = ""
                Dim Istat_Com As String = ""
                Dim AziendaVisibile As Boolean



                Dim AziendaCreata = importatore.CreaAzienda(ProgressivoGIAS,
                                                            Utente_Password,
                                                            piva,
                                                            Cuaa,
                                                            Fascicolo_Umbria,
                                                            strErr,
                                                            Messaggio,
                                                            ragSoc, Indirizzo, Cap, Ista_Prov, Istat_Com, OrigineOpr,
                                                            objParametri_Server,
                                                            objParametri_Utenti,
                                                            AziendaVisibile,
                                                            Piva_Padre, Flag_ImportaMacchine, Flag_ImportaCatasto,
                                                            False, False, True)



                If AziendaCreata AndAlso Flag_ImportaPianoColturale Then

                    Dim DtParticelle As New DataTable
                    Dim dtPartielleAggregate As New DataTable

                    Importatore_UMBRIA.Importatore.dtParticelleCreaStruttura(DtParticelle)
                    Importatore_UMBRIA.Importatore.dtParticelleCreaStruttura(dtPartielleAggregate)

                    Dim DataInizio As Date = AGRODATAINIZIO
                    Dim DataFine As Date = AGRODATAFINE

                    Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                    Try
                        objImpost.AnnataAgraria(DataValidazione, DataInizio, DataFine, objParametri_Utenti)
                    Catch ex As Exception
                        objImpost.AnnataAgraria(Date.Now, DataInizio, DataFine, objParametri_Utenti)
                    End Try

                    importatore.PopolaDT_Appezzamenti(piva,
                                                      Fascicolo_Umbria,
                                                      False,
                                                      False,
                                                      DataInizio,
                                                      DataFine,
                                                      DtParticelle,
                                                      objParametri_Server,
                                                      objParametri_Utenti,
                                                      Num_Scheda,
                                                      Utente_Username,
                                                      Utente_Password,
                                                      ProgressivoGIAS,
                                                      Flag_ImportaAnagrafica)

                    importatore.dtParticelleAggrega(False, False, False, DtParticelle, dtPartielleAggregate)


                    Dim strAppezzamenti = JSON_DataTableAppezzamenti_Tabella(dtPartielleAggregate, DataInizio, DataFine)

                    Dim scriviConRibaltamento As New AgronicaCoreAnagrafeBIZ.ImportazionePcFast

                    Dim programmazione_cod As Integer = 0

                    scriviConRibaltamento.Aggiorna_PcFast_PianoColtuale(piva,
                                                                        Cuaa,
                                                                        programmazione_cod,
                                                                        DataInizio,
                                                                        DataFine,
                                                                        Num_Scheda,
                                                                        DataValidazione,
                                                                        enum_Planning_Fonte.AGEA_Coordinamento,
                                                                        ProgressivoGIAS,
                                                                        allegatoDocumentoCod,
                                                                        False,
                                                                        False,
                                                                        objParametri_Server,
                                                                        objParametri_Utenti,
                                                                        0,
                                                                        strAppezzamenti,
                                                                        importatoAutomaticamente:=True)

                    Return True

                End If

            Else

                Messaggio = "Fascicolo non presente"

            End If
        End If

        Messaggio = strErr & Messaggio

        If Trim(Messaggio) = "" Then
            Messaggio = "importata?"
        End If

        Return False
    End Function

    'Private Function importaFascicoloAGEA(impresa As ImpreseResult,
    '                                      fascicolo As SchedaFascicolo,
    '                                      ByRef strRes As String,
    '                                      ByRef strErr As String) As Boolean
    '    Dim RagSoc As String = ""
    '    Dim Indirizzo As String = ""
    '    Dim Cap As String = ""
    '    Dim Ista_Prov As String = ""
    '    Dim Istat_Com As String = ""

    '    Dim Provincia As String = ""
    '    Dim Comune As String = ""

    '    strErr = ""
    '    strRes = ""

    '    Dim Legale_Rappresentante_Nome As String = ""
    '    Dim Legale_Rappresentante_Cognome As String = ""
    '    Dim Legale_Rappresentante_CF As String = ""
    '    Dim Legale_Rappresentante_Sesso As String = ""
    '    Dim Legale_Rappresentante_Data_Nascita As String = ""

    '    Dim Piva_Padre As String = ""
    '    Dim Rag_Soc_Padre As String = ""

    '    Dim Indirizzo_Stato As String = ""
    '    Dim Indirizzo_Stato_Des As String = ""

    '    Dim Frazione As String = ""

    '    Dim AziendaCreata As Boolean
    '    Dim AziendaLetta As Boolean
    '    Dim AziendaVisibile As Boolean = True

    '    Dim esisteFascicolo = False

    '    Dim Fascicolo_Umbria As String = Nothing
    '    Dim xml_Response As String = Nothing

    '    Dim Programmazione_Cod As Integer = 0

    '    Dim messaggioWS As String = ""
    '    Dim statoWS As Boolean = True

    '    Dim Num_Scheda As String = ""
    '    Dim DataValidazione As Date = AGRODATAINIZIO
    '    Dim allegatoDocumentocod As Integer = 0
    '    Dim OrigineOpr As String = ""

    '    Dim import_umbriaUtility As New Importatore_UMBRIA.AGEA_UMBRIA_Utility
    '    esisteFascicolo = import_umbriaUtility.FascicoloLeggiWSMemorizza_UMBRIA(
    '                                                    False,
    '                                                    4,
    '                                                    objParametri_Server,
    '                                                    Fascicolo_Umbria,
    '                                                    messaggioWS,
    '                                                    statoWS,
    '                                                    strErr,
    '                                                    impresa.Piva,
    '                                                    impresa.Cuaa,
    '                                                    impresa.UltimoFascicoloAGEA.NumeroValidazione,
    '                                                    impresa.UltimoFascicoloAGEA.DataValidazione,
    '                                                    OrigineOpr,
    '                                                    allegatoDocumentocod,
    '                                                    xml_Response
    '                                                )

    '    Dim import_umbria As New Importatore_UMBRIA.Importatore

    '    AziendaCreata = import_umbria.CreaAzienda(ProgressivoGIAS,
    '                                                  ASG_Utente_Password,
    '                                                  impresa.Piva, impresa.Cuaa,
    '                                                  Fascicolo_Umbria,
    '                                                  strErr,
    '                                                  strRes,
    '                                                  RagSoc,
    '                                                  Indirizzo,
    '                                                  Cap,
    '                                                  Ista_Prov,
    '                                                  Istat_Com,
    '                                                  OrigineOpr,
    '                                                  objParametri_Server,
    '                                                  objParametri_Utenti,
    '                                                  AziendaVisibile,
    '                                                  "", True, True,
    '                                                  True, True)

    '    If AziendaCreata Then
    '        Dim strAppezzamentiFascicolo As String = ""

    '        Dim validita_inizio = New Date(impresa.UltimoFascicoloAGEA.DataValidazione.Year, 1, 1)
    '        Dim validita_fine = New Date(impresa.UltimoFascicoloAGEA.DataValidazione.Year, 1, 1)

    '        strAppezzamentiFascicolo =
    '            LeggiAppezzamentiFascicolo_UMBRIA(
    '                impresa.Piva, impresa.Cuaa, 4,
    '                impresa.UltimoFascicoloAGEA.NumeroValidazione,
    '                False, False, False,
    '                objParametri_Server, objParametri_Utenti,
    '                validita_inizio.ToShortDateString,
    '                validita_fine.ToShortDateString,
    '                OrigineOpr,
    '                ASG_Utente_Username:=ASG_Utente_Username,
    '                ASG_Utente_Password:=ASG_Utente_Password,
    '                ASG_ProgressivoGIAS:=ProgressivoGIAS
    '            )

    '        If strAppezzamentiFascicolo <> " [  ]" Then

    '            SalvaKendo(impresa.Piva,
    '                       strAppezzamentiFascicolo,
    '                       impresa.UltimoFascicoloAGEA.NumeroValidazione,
    '                       validita_inizio, validita_fine, objParametri_Server)

    '            SalvaPiano(impresa.Piva, impresa.Cuaa, 0,
    '                       validita_inizio, validita_fine, 0,
    '                       impresa.UltimoFascicoloAGEA.NumeroValidazione, impresa.UltimoFascicoloAGEA.DataValidazione,
    '                       4, False, False, 0, ProgressivoGIAS, objParametri_Server, objParametri_Utenti)

    '        End If
    '    End If

    '    Return AziendaCreata
    'End Function

    'Private Function LeggiAppezzamentiFascicolo_UMBRIA(
    '    ByVal piva As String,
    '    ByVal cuaa As String,
    '    ByVal Fonte_Cod As enum_Planning_Fonte,
    '    ByVal Allegati_Documenti_Numero As String,
    '    ByVal Dividi_Centri As String,
    '    ByVal Aggrega_Specie As String,
    '    ByVal Aggrega_Tare As String,
    '    ByVal objParametri_Server As AgronicaCoreParametri,
    '    ByVal objParametri_Utenti As AgronicaCoreParametri,
    '    ByVal DataInizio As String,
    '    ByVal DataFine As String,
    '    ByVal OrigineOPR As String,
    '    ByRef ASG_Utente_Username As String,
    '    ByRef ASG_Utente_Password As String,
    '    ByRef ASG_ProgressivoGIAS As String
    ') As String


    '    Dim strAppezzamenti As String = ""

    '    'Dim bAggregaSpecie As Boolean = CBool(Aggrega_Specie)
    '    Dim iAggregaSpecie As Integer = CInt(Aggrega_Specie)
    '    Dim bDividiCentri As Boolean = CBool(Dividi_Centri)
    '    Dim bAggregaTare As Boolean = CBool(Aggrega_Tare)


    '    Try

    '        Dim objLog As New AgronicaCoreDataProvider.LogProvider

    '        Dim NomeRoutine As String = "LeggiAppezzamentiFascicolo"
    '        Dim MsgOK As String = ""


    '        Dim DT_Appezzamenti As New DataTable
    '        Dim strErr As String = ""
    '        Dim Validazione_Data As Date = AGRODATAINIZIO

    '        Dim import_umbria_utils As New Importatore_UMBRIA.AGEA_UMBRIA_Utility
    '        Dim import_umbria As New Importatore_UMBRIA.Importatore

    '        Dim fascicolo As String =
    '            import_umbria_utils.Leggi_Dati_Fascicolo_UMBRIA(cuaa, piva, Allegati_Documenti_Numero, "", "", objParametri_Server)

    '        Dim obj_fascicolo As New AGEA_Coordinamento.ISWSToOprResponse

    '        Dim dtParticelleAggregate As New DataTable
    '        DtParticelleCreaStruttura(dtParticelleAggregate)

    '        Dim x As New Xml.Serialization.XmlSerializer(GetType(AGEA_Coordinamento.ISWSToOprResponse))
    '        Dim string_reader As New StringReader(fascicolo)
    '        obj_fascicolo = DirectCast(x.Deserialize(string_reader), AGEA_Coordinamento.ISWSToOprResponse)

    '        If obj_fascicolo.Items(0).GetType.Name = "ISWSRespAnagFascicolo15" Then
    '            Dim Fascicolo_Umbria = DirectCast(obj_fascicolo.Items(0), AGEA_Coordinamento.ISWSRespAnagFascicolo15)

    '            Dim Codice_Detentore = Fascicolo_Umbria.detentore

    '            'salvo il fascicolo originale
    '            If Not fascicolo Is Nothing AndAlso fascicolo <> "" Then
    '                Dim DataValidazione As Date = AGRODATAINIZIO
    '                Dim schedaValidazione As String = ""
    '                Try
    '                    DataValidazione = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(Fascicolo_Umbria.dataValidazFascicolo))
    '                Catch ex As Exception

    '                End Try

    '                schedaValidazione = Fascicolo_Umbria.schedaValidazione

    '                import_umbria_utils.FascicoloMemorizza_UMBRIA(Fonte_Cod, piva, DataValidazione, schedaValidazione, fascicolo, objParametri_Server, 0, Codice_Detentore)

    '            End If

    '            Dim DtParticelle As New DataTable
    '            DtParticelleCreaStruttura(DtParticelle)



    '            Dim importCatasto = False

    '            import_umbria.PopolaDT_Appezzamenti(piva, fascicolo, iAggregaSpecie, bAggregaTare, DataInizio, DataFine, DtParticelle, objParametri_Server, objParametri_Utenti, Allegati_Documenti_Numero, ASG_Utente_Username, ASG_Utente_Password, ASG_ProgressivoGIAS)

    '            If importCatasto Then
    '                import_umbria.PopolaDT_Appezzamenti(piva, fascicolo, iAggregaSpecie, bAggregaTare, DataInizio, DataFine, DtParticelle, objParametri_Server, objParametri_Utenti, Allegati_Documenti_Numero, ASG_Utente_Username, ASG_Utente_Password, ASG_ProgressivoGIAS)
    '            End If

    '            DtParticelleAggrega(iAggregaSpecie, bDividiCentri, bAggregaTare, DtParticelle, dtParticelleAggregate)

    '            If dtParticelleAggregate IsNot Nothing Then

    '            End If

    '        ElseIf obj_fascicolo.Items(0).GetType.Name = "ISWSRespAnagFascicolo2" Then

    '            Dim Fascicolo_Umbria = DirectCast(obj_fascicolo.Items(0), AGEA_Coordinamento.ISWSRespAnagFascicolo2)

    '            Dim Codice_Detentore = Fascicolo_Umbria.detentore

    '            'salvo il fascicolo originale
    '            If Not fascicolo Is Nothing AndAlso fascicolo <> "" Then
    '                Dim DataValidazione As Date = AGRODATAINIZIO
    '                Dim schedaValidazione As String = ""
    '                Try
    '                    DataValidazione = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(Fascicolo_Umbria.dataValidazFascicolo))
    '                Catch ex As Exception

    '                End Try

    '                schedaValidazione = Fascicolo_Umbria.schedaValidazione

    '                import_umbria_utils.FascicoloMemorizza_UMBRIA(Fonte_Cod, piva, DataValidazione, schedaValidazione, fascicolo, objParametri_Server, 0, Codice_Detentore)

    '            End If

    '            Dim DtParticelle As New DataTable
    '            DtParticelleCreaStruttura(DtParticelle)

    '            Dim importCatasto = False

    '            import_umbria.PopolaDT_Appezzamenti_ISWSRespAnagFascicolo2(piva, fascicolo, iAggregaSpecie, bAggregaTare, DataInizio, DataFine, DtParticelle, objParametri_Server, objParametri_Utenti, Allegati_Documenti_Numero, ASG_Utente_Username, ASG_Utente_Password, ASG_ProgressivoGIAS)

    '            If importCatasto Then
    '                import_umbria.PopolaDT_Appezzamenti_ISWSRespAnagFascicolo2(piva, fascicolo, iAggregaSpecie, bAggregaTare, DataInizio, DataFine, DtParticelle, objParametri_Server, objParametri_Utenti, Allegati_Documenti_Numero, ASG_Utente_Username, ASG_Utente_Password, ASG_ProgressivoGIAS)
    '            End If


    '            DtParticelleAggrega(iAggregaSpecie, bDividiCentri, bAggregaTare, DtParticelle, dtParticelleAggregate)

    '            If dtParticelleAggregate IsNot Nothing Then

    '            End If

    '        End If


    '        strAppezzamenti = JSON_DataTableAppezzamenti_Tabella(dtParticelleAggregate, DataInizio, DataFine)

    '    Catch ex As Exception

    '        If ex.Message = "Catasto non presente, non è possibile compilare il fascicolo" Then

    '        End If
    '        Return ""

    '    End Try

    '    Return strAppezzamenti

    'End Function

    Private Sub Forza_Scarico_Fascicolo_UMBRIA(cuaa)
        Dim import_umbria_utils As New Importatore_UMBRIA.AGEA_UMBRIA_Utility
        import_umbria_utils.CaricaDati_WS_AgroFascicolo_UMBRIA(0, "", cuaa, "-2", objParametri_Server)
    End Sub

    Private Function Popola_Dt_Schede_UMBRIA(cuaa As String, data_da As Date, data_a As Date, Dt_Schede As DataTable) As Integer

        Dim DataDa As Integer = CInt(AgronicaCoreDataProvider.Conversioni.DateTime_To_DataintYYYYMMGG(data_da))
        Dim DataA As Integer = CInt(AgronicaCoreDataProvider.Conversioni.DateTime_To_DataintYYYYMMGG(data_a))
        Dim import_umbria_utils As New Importatore_UMBRIA.AGEA_UMBRIA_Utility
        'ws_SchedeResponse = ws_FascicoloColdi.getSchedeFascicolo(gS)

        Dim key = cuaa & "-" & DataDa & "-" & DataA
        Dim schede As String = ""
        Dim Num_Scheda As String
        Dim DataValidazione As Date
        Dim N_Schede As Integer = 0
        Dim OrigineOpr = ""

        schede = import_umbria_utils.CaricaSchede_WS_AgroFascicolo_UMBRIA(0, "", cuaa, objParametri_Server)

        If schede IsNot Nothing AndAlso schede <> "" Then

            Dim obj_Schede = JArray.Parse(schede)

            N_Schede = obj_Schede.Count

            For Each obj_scheda In obj_Schede

                Dim DrScheda = Dt_Schede.NewRow

                Num_Scheda = obj_scheda("Numero_Validazione")

                DataValidazione = CDate(obj_scheda("Data_Validazione")).ToShortDateString

                OrigineOpr = obj_scheda("Ente_Des")

                DrScheda.Item("Scheda") = Num_Scheda
                DrScheda.Item("Validita_Inizio") = CDate(DataValidazione).ToShortDateString
                DrScheda.Item("Validita_Fine") = CDate(DataValidazione).ToShortDateString
                DrScheda.Item("OrigineOpr") = OrigineOpr
                DrScheda.Item("Data") = CDate(DataValidazione).ToShortDateString
                DrScheda.Item("fonte_cod") = 4
                DrScheda.Item("desOPR") = "AGEA-COORDINAMENTO"
                Dt_Schede.Rows.Add(DrScheda)
            Next

            Dt_Schede.DefaultView.Sort = " Validita_Inizio DESC "

            DataValidazione = Dt_Schede.DefaultView.Item(0).Row.Item("Validita_Inizio")
            Num_Scheda = Dt_Schede.DefaultView.Item(0).Row.Item("Scheda")
            OrigineOpr = Dt_Schede.DefaultView.Item(0).Row.Item("OrigineOpr")

        End If

        Return N_Schede

    End Function

    Private Function CreaDTSchede() As DataTable
        Dim Dt_Schede As New DataTable

        Dt_Schede.Columns.Add(New DataColumn("Scheda", GetType(String)))
        Dt_Schede.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
        Dt_Schede.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))
        Dt_Schede.Columns.Add(New DataColumn("Data", GetType(String)))
        Dt_Schede.Columns.Add(New DataColumn("desOPR", GetType(String)))
        Dt_Schede.Columns.Add(New DataColumn("OrigineOpr", GetType(String)))
        Dt_Schede.Columns.Add(New DataColumn("fonte_cod", GetType(Integer)))

        Return Dt_Schede
    End Function

    Private Sub logga(ByVal msg As String)
        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        objLog.Scrivi_LOG(objParametri_Server, "", msg, CustomLOGParams:=customLOGParams)
    End Sub

    Private Shared Function JSON_DataTableAppezzamenti_Tabella(ByRef DT_Appezzamenti As DataTable,
                                                               ByVal DataInizio As String, ByVal DataFine As String,
                                                               Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("chiave", "chiave", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("piva", "piva", "string")
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("sa_cod", "sa_cod", "number")
        c._hidden = True
        c._Editabile = True
        l.Add(c)
        c = New ColonneNome("appezza", "appezza", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("programmazione_entita_cod", "Programmazione_Entita_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("datoGis", "Gis", "String")
        c._FormatoParticolare = "<span class='fa fa-globe fa-2x'></span>"
        'c._RemoveHtmlEncode = True
        c._Filtrabile = False
        c._width = "60px"
        l.Add(c)

        c = New ColonneNome("app_nome", "App.", "string")
        c._Editabile = True
        c._obbligatorio = True
        c._Filtrabile = False
        c._width = "78px"
        l.Add(c)

        c = New ColonneNome("utilizzo_sup", "Sup. Utilizzo [ha] (SAU)", "number")
        c._Editabile = True
        c._obbligatorio = True
        c._Filtrabile = False
        c._width = "120px"
        c._formatNr = "n4"
        c._sum = True
        l.Add(c)

        c = New ColonneNome("macrouso_cod", "Macrouso_Cod", "string")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("macrouso_sup", "Macrouso_Sup", "string")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("utilizzo", "Utilizzo (Specie Vegetale)", "string")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("varieta", "Varietà", "string")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("veg_des", "Specie Vegetale", "string")
        c._Editabile = True
        'c._hidden = True
        c._width = "200px"
        l.Add(c)

        c = New ColonneNome("cul_des", "Varietà", "string")
        c._Editabile = True
        'c._hidden = True
        c._width = "200px"
        l.Add(c)

        c = New ColonneNome("grfi_des", "Finalità Produttiva", "string")
        c._Editabile = True
        'c._hidden = True
        c._width = "200px"
        l.Add(c)

        c = New ColonneNome("macrouso", "Macrouso", "string")
        c._Editabile = True
        'c._hidden = True
        c._width = "200px"
        l.Add(c)

        c = New ColonneNome("grva_des", "Tipologia Varietale", "string")
        c._hidden = True
        l.Add(c)

        ''GESTIONE CATASTO IN LINEA
        c = New ColonneNome("PROV", "PROV", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("prov_des", "PR.", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("COM", "COM", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("com_des", "Comune", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "150px"
        l.Add(c)

        c = New ColonneNome("sezione", "Sez.", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("foglio", "Fgl.", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("numero", "Numero", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "100px"
        l.Add(c)

        c = New ColonneNome("subalterno", "Sub.", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("sa_nome", "Centro", "string")
        c._Editabile = True
        c._Filtrabile = False
        c._width = "100px"
        c._Filtrabile = True
        l.Add(c)

        c = New ColonneNome("validita_inizio", "Inizio Gestione Appezzamento", "date")
        c._Editabile = True
        c._obbligatorio = True
        c._width = "133px"
        c._valueDefault = DataInizio
        c._Filtrabile = True
        l.Add(c)

        c = New ColonneNome("validita_fine", "Fine Gestione Appezzamento", "date")
        c._Editabile = True
        c._obbligatorio = True
        c._width = "123px"
        c._valueDefault = DataFine
        c._Filtrabile = True
        l.Add(c)

        c = New ColonneNome("prov", "PROV", "string")
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("com", "COM", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("catasto", "Catasto (Provincia-Comune-Sezione-Foglio-Numero-Subalterno)", "string")
        c._FormatoParticolare = "#= GetValoreParticella(chiave, catasto)#"
        c._RemoveHtmlEncode = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("catasto_key", "Catasto_Key", "String")
        c._RemoveHtmlEncode = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("id_cod", "Id_Cod", "String")
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("veg_cod", "Veg_Cod", "String")
        c._hidden = True
        c._Editabile = True
        l.Add(c)
        c = New ColonneNome("cul_cod", "Cul_Cod", "String")
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("grfi_cod", "Grfi_Cod", "String")
        c._hidden = True
        c._Editabile = True
        l.Add(c)
        c = New ColonneNome("grva_cod", "Grva_Cod", "String")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("veg_cod_agea", "Veg_Cod_Agea", "String")
        c._hidden = True
        c._Editabile = True
        l.Add(c)
        c = New ColonneNome("cul_cod_agea", "Cul_Cod_Agea", "String")
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("ribaltato", "ribaltato", "String")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("movimentato", "movimentato", "String")
        c._hidden = True
        l.Add(c)


        c = New ColonneNome("Cop_Cod", "Cop_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Cop_Des", "Cop_Des", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Lotto", "Lotto", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Resa", "Resa", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Num_Piante", "Num_Piante", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("TRA_Fila", "TRA_Fila", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("SU_Fila", "SU_Fila", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Validita_Inizio_Impianto", "Validita_Inizio_Impianto", "Date")
        c._Editabile = True
        'c._valueDefault = DataInizio
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("TipoZona", "TipoZona", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("TipoZona_Des", "Tipo Zona", "String")
        c._Editabile = True
        'c._hidden = True
        l.Add(c)
        c = New ColonneNome("MetodoProduzione_Cod", "MetodoProduzione_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("MetodoProduzione_Des", "Metodo Produzione", "String")
        c._Editabile = True
        'c._hidden = True
        l.Add(c)
        c = New ColonneNome("Unita_Vitata", "Unita_Vitata", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Veg_Cod_Agea", "Veg_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Veg_Des_Agea", "Veg_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Cul_Cod_Agea", "Cul_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Cul_Des_Agea", "Cul_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Uso_Cod_Agea", "Uso_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Uso_Des_Agea", "Uso_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Occupazione_Cod_Agea", "Occupazione_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Occupazione_Des_Agea", "Occupazione_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Destinazione_Cod_Agea", "Destinazione_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Destinazione_Des_Agea", "Destinazione_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Qualita_Cod_Agea", "Qualita_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Qualita_Des_Agea", "Qualita_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Gru_Cod", "Gru_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Dpi_Cod", "Dpi_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Reg_Cod", "Reg_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Regolamento_Concimazione_Cod", "Regolamento_Concimazione_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Disciplinare", "Disciplinare", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Flag_PubblicoPrivato", "Flag_PubblicoPrivato", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("id_tr", "id_tr", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("StatoImpianto_Cod", "StatoImpianto_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("N", "N", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("P", "P", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("K", "K", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Data_Semina", "Data_Semina", "Date")
        c._Editabile = True
        c._hidden = True
        'c._valueDefault = DataInizio
        l.Add(c)

        c = New ColonneNome("Data_Raccolta", "Data_Raccolta", "Date")
        c._Editabile = True
        c._hidden = True
        'c._valueDefault = DataInizio
        l.Add(c)

        c = New ColonneNome("Data_Fioritura", "Data_Fioritura", "Date")
        c._Editabile = True
        c._hidden = True
        'c._valueDefault = DataInizio
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente", "Coltura_Precedente", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente2", "Coltura_Precedente2", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente3", "Coltura_Precedente3", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente4", "Coltura_Precedente4", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Piano_Semina", "Piano_Semina", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Codice_Contratto", "Codice_Contratto", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("unito", "unito", "number")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("frazionato", "frazionato", "number")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Codice_Fiscale_Tecnico", "Codice_Fiscale_Tecnico", "String")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("stato_ribaltamento", "stato_ribaltamento", "number")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("provenienza_fascicolo", "provenienza_fascicolo", "String")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("IAF", "IAF", "String")
        'c._Editabile = True
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("DistBZ_CorpiIdrici", "DistBZ_CorpiIdrici", "number")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("DistBZ_AreeResPub", "DistBZ_AreeResPub", "number")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("DistBZ_Allevamenti", "DistBZ_Allevamenti", "number")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("DistBZ_VegNatNonColt", "DistBZ_VegNatNonColt", "number")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = False

        Dim risp As String

        If stringaKendoRow = "" Then
            Dim strKendoRow As New StringBuilder
            AgronicaCoreDataProvider.JSON_DataTable.kendo_Rows(DT_Appezzamenti, l, strKendoRow)
            risp = strKendoRow.ToString
        Else
            risp = js.JSON_DataTable_Kendo(DT_Appezzamenti, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu,
                                           stringaKendoRow:=stringaKendoRow) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '
        End If

        Return risp

    End Function

End Class

Public Class CuaaModificata

    Public Property CUAA As String

    Public Property Istat_provincia As String

    Public Property Istat_comune As String

End Class