Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreDTOStd.InData.Gis
Imports Newtonsoft.Json

Public Class Compliance_ISCC
    Inherits LogProvider

    Public Function IncludiEsludiAziendeControlloISCC(ByVal elencoAziende As IncludiEsludiImpresaISCC_In,
                                                      ByRef ObjParametri_Server As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.Compliance_ISCC.AggiungiAziendaAControllo()"
        Dim MessaggioErrore As String = ""
        Dim resp As Boolean = False
        Dim xISCCr As New AgronicaCoreGisDAL.CheckList_GIS_R
        Dim xISCCw As New AgronicaCoreGisDAL.CheckList_GIS_W

        Try
            For Each azienda In elencoAziende.elencoAziende
                Select Case azienda.flag_includi
                    Case enum_ComplianceISCC_Azienda_Controllo.Inclusa
                        Dim dt = xISCCr.LeggiElencoAbilitazioniAziende(elencoAziende.checkListType, azienda.piva, enum_ComplianceISCC_Azienda_Controllo.NotSet, "", "", ObjParametri_Server)
                        If dt.Rows.Count <= 0 Then
                            resp = xISCCw.ScriviAbilitazioneAziendaCheckListType(elencoAziende.checkListType, azienda.piva, enum_ComplianceISCC_Azienda_Controllo.Inclusa, ObjParametri_Server)
                        Else
                            resp = xISCCw.ModificaAbilitazioneAziendaCheckListType(elencoAziende.checkListType, azienda.piva, enum_ComplianceISCC_Azienda_Controllo.Inclusa, ObjParametri_Server)
                        End If
                    Case enum_ComplianceISCC_Azienda_Controllo.Esclusa
                        Dim dt = xISCCr.LeggiElencoAbilitazioniAziende(elencoAziende.checkListType, azienda.piva, enum_ComplianceISCC_Azienda_Controllo.NotSet, "", "", ObjParametri_Server)
                        If dt.Rows.Count > 0 Then
                            resp = xISCCw.ModificaAbilitazioneAziendaCheckListType(elencoAziende.checkListType, azienda.piva, enum_ComplianceISCC_Azienda_Controllo.Esclusa, ObjParametri_Server)
                        Else
                            resp = xISCCw.ScriviAbilitazioneAziendaCheckListType(elencoAziende.checkListType, azienda.piva, enum_ComplianceISCC_Azienda_Controllo.Esclusa, ObjParametri_Server)
                        End If
                End Select
            Next
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore, False)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return resp
    End Function

    Public Function AggiungiAziendaAControllo(ByVal checklist_type As Integer,
                                              ByVal piva As String,
                                              ByRef ObjParametri_Server As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.Compliance_ISCC.AggiungiAziendaAControllo()"

        Dim MessaggioErrore As String = ""
        Dim resp As Boolean = False
        Dim xISCCr As New AgronicaCoreGisDAL.CheckList_GIS_R
        Dim xISCCw As New AgronicaCoreGisDAL.CheckList_GIS_W
        Try
            Dim dt = xISCCr.LeggiElencoAbilitazioniAziende(checklist_type, piva, enum_ComplianceISCC_Azienda_Controllo.NotSet, "", "", ObjParametri_Server)
            If dt.Rows.Count <= 0 Then
                resp = xISCCw.ScriviAbilitazioneAziendaCheckListType(checklist_type, piva, enum_ComplianceISCC_Azienda_Controllo.Inclusa, ObjParametri_Server)
            Else
                resp = xISCCw.ModificaAbilitazioneAziendaCheckListType(checklist_type, piva, enum_ComplianceISCC_Azienda_Controllo.Inclusa, ObjParametri_Server)
            End If
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore, False)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return resp
    End Function

    Public Function EscludiAziendaDaControllo(ByVal checklist_type As Integer,
                                              ByVal piva As String,
                                              ByRef ObjParametri_Server As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.Compliance_ISCC.EscludiAziendaDaControllo()"

        Dim MessaggioErrore As String = ""
        Dim resp As Boolean = False
        Dim xISCCr As New AgronicaCoreGisDAL.CheckList_GIS_R
        Dim xISCCw As New AgronicaCoreGisDAL.CheckList_GIS_W
        Try
            Dim dt = xISCCr.LeggiElencoAbilitazioniAziende(checklist_type, piva, enum_ComplianceISCC_Azienda_Controllo.NotSet, "", "", ObjParametri_Server)
            If dt.Rows.Count > 0 Then
                resp = xISCCw.ModificaAbilitazioneAziendaCheckListType(checklist_type, piva, enum_ComplianceISCC_Azienda_Controllo.Esclusa, ObjParametri_Server)
            Else
                resp = xISCCw.ScriviAbilitazioneAziendaCheckListType(checklist_type, piva, enum_ComplianceISCC_Azienda_Controllo.Esclusa, ObjParametri_Server)
            End If
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore, False)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return resp
    End Function

    Public Function SottomettiElaborazioneMassiva(ByVal tipo_checklist As Integer,
                                                  ByVal data_riferimento As DateTime,
                                                  ByVal pivaList As List(Of String),
                                                  ByRef ObjParametri_Server As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.Compliance_ISCC.SottomettiElaborazioneMassiva()"

        Dim MessaggioErrore As String = ""
        Dim resp As Boolean = False



        Dim DataElaborazione As DateTime = DateTime.Now
        Dim layerList As New List(Of Integer)
        layerList.Add(enum_Gis_LayerElementiGrafici_std.IMPIANTI)

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim xChkListr As New AgronicaCoreGisDAL.CheckList_GIS_R
        Dim xISCCr As New AgronicaCoreGisDAL.Compliance_ISCC_R
        Dim xISCCw As New AgronicaCoreGisDAL.Compliance_ISCC_W
        Dim xPLayerR As New AgronicaCoreGisDAL.ProiezioniLayer_W
        Dim xPLayerW As New AgronicaCoreGisBIZ.ProiezioniLayer_W
        Dim xImpR As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim seq As New Agro_Sequenze

        If tipo_checklist = enum_GIS_CheckList_Type.ISCC Then
            Dim dtAziende = xChkListr.LeggiElencoAbilitazioniAziende(tipo_checklist, "", enum_ComplianceISCC_Azienda_Controllo.Inclusa, "", "", ObjParametri_Server)

            'se non ho specificato alcuna piva in pivaList, devo eseguire un controllo di tutte le aziende abilitate
            If pivaList Is Nothing OrElse pivaList.Count <= 0 Then
                If pivaList Is Nothing Then
                    pivaList = New List(Of String)
                End If

                For Each piva In dtAziende.Rows
                    pivaList.Add(piva("PIVA"))
                Next
            Else
                'ho passato un elenco di aziende alla chiamata devo prima controllare se tutte sono abilitate al controllo, altrimenti devo far scattare eccezione con l'elenco di quelle non abilitate
                Dim pivaNotAllowed As New List(Of String)
                For Each piva In pivaList
                    If dtAziende.Select(String.Format("PIVA = '{0}'", piva)).ToList().Count = 0 Then
                        pivaNotAllowed.Add(piva)
                    End If
                Next

                If pivaNotAllowed.Count > 0 Then
                    Throw New Exception("Alcune delle aziende indicate non sono abilitate al controllo ISCC: " & vbCrLf & String.Join(vbCrLf, pivaNotAllowed))
                End If
            End If
        End If


        'se eseguo una richista per la compliance ISCC 
        If pivaList Is Nothing OrElse pivaList.Count <= 0 Then
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, "Elenco azienda da elaborare non valorizzato. Impossibile proseguire", False)
            Throw New Exception("Elenco azienda da elaborare non valorizzato. Impossibile proseguire")
        End If

        Try
            '0. recupero l'elenco degli algoritmi da sottomettere recuperandoli dalla prima checklist del tipo indicato , valido alla data indicata
            Dim dtCheckList = xChkListr.LeggiChecklist(0, tipo_checklist, data_riferimento, "", "LayerAnalysisConfig_Cod ASC", ObjParametri_Server)
            If dtCheckList.Rows.Count <= 0 Then
                Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("Nessuna checklist di tipo {0} valida al {1}. Impossibile proseguire", [Enum].Parse(GetType(enum_GIS_CheckList_Type), tipo_checklist).ToString(), data_riferimento), False)
                Throw New Exception(String.Format("Nessuna checklist di tipo {0} valida al {1}. Impossibile proseguire", [Enum].Parse(GetType(enum_GIS_CheckList_Type), tipo_checklist).ToString(), data_riferimento))
            End If

            Dim dtAlgList = dtCheckList.DefaultView.ToTable(True, "LayerAnalysisConfig_Cod")
            If dtAlgList.Rows.Count <= 0 Then
                Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("Nessun algoritmo trovato nelle checklist di tipo {0} valida al {1}. Impossibile proseguire", [Enum].Parse(GetType(enum_GIS_CheckList_Type), tipo_checklist).ToString(), data_riferimento), False)
                Throw New Exception(String.Format("Nessun algoritmo trovato nelle checklist di tipo {0} valida al {1}. Impossibile proseguire", [Enum].Parse(GetType(enum_GIS_CheckList_Type), tipo_checklist).ToString(), data_riferimento))
            End If

            '1. per ogni azienda:
            '   a. check azienda non chiusa -> se non valida alla data ERRORE
            '   b. leggi elenco impianti validi alla data
            '   c. per ogni impianto
            '       i. check se ha il poligono -> se non ce l'ha scarta l'impianto
            '       ii. leggi entita_cod dell'impianto
            '       iii. per ogni algoritmo recuperato dalla checklist
            '           1) sottometti una richiesta di elaborazione algoritmo proiezione gis con i seguenti parametri aggiuntivi
            '               a) ID_Elaborazione
            '               b) Tipo_CheckList
            '               c) ID_CheckList

            Dim id_elaborazione = seq.NuovoId_Tabella("Imprese_ISCCXElaborazioni", 0, CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, ObjParametri_Server)



            For Each piva In pivaList

                Dim dtImp = xImpR.Leggi(piva, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", ObjParametri_Server)
                If dtImp.Rows.Count <= 0 Then
                    Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("Nesuna azienda trovata con la partita iva {0}. Impossibile proseguire.", piva))
                    Throw New Exception(String.Format("Nesuna azienda trovata con la partita iva {0}. Impossibile proseguire.", piva))
                Else
                    If dtImp.Rows(0)("Validita_Inizio") <= data_riferimento AndAlso dtImp.Rows(0)("Validita_Fine") >= data_riferimento Then
                        'lavez - 06/11/2024 - genero il group id da associare a tutte le richieste di tutti gli algoritmi della checklist per l'intera azienda
                        Dim groupId = seq.NuovoId_Tabella("GIS_GroupID", 0, CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, ObjParametri_Server)

                        Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("Recupero elenco impianti validi alla data {0} per la piva {1}", data_riferimento, piva), False)
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagTransazioneLocale, FlagTransazioneLocale, ObjParametri_Server)
                        Dim dtEnt = xISCCr.LeggiElencoEntitaDaPianoColturale(piva,
                                                                             0,
                                                                             data_riferimento,
                                                                             "",
                                                                             "",
                                                                             ObjParametri_Server)

                        Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("trovati {0} Impianti con cartografia ", dtEnt.Rows.Count), False)

                        For Each ent In dtEnt.Rows
                            For Each alg In dtCheckList.Rows
                                If alg("Applica_Risultato") = True Then
                                    Dim reqnum As Integer = -1
                                    Dim chk_ok As Integer = False
                                    chk_ok = xPLayerW.AttivaDisattivaConfigurazioneSuEntita(alg("LayerAnalysisConfig_Cod"),
                                                                                   ent("Entita_Cod"),
                                                                                   0,
                                                                                   0,
                                                                                   True,
                                                                                   reqnum,
                                                                                   ObjParametri_Server,
                                                                                   False,
                                                                                   False,
                                                                                   JsonConvert.SerializeObject(New ParametriAggiuntiviAlgoritmiCartografici_ISCC() With {
                                                                                    .CheckList_Type = tipo_checklist,
                                                                                    .CheckList_ID = alg("GIS_LayerAnalysisConfig_CheckList_Cod"),
                                                                                    .Elaborazione_ID = id_elaborazione
                                                                                   }),
                                                                                   groupId)
                                    If chk_ok = True Then
                                        chk_ok = xISCCw.ScriviRichiestaElaborazioneAlgoitmoPerCompliance_ISCC_Con_Chiave_Impianto(id_elaborazione,
                                                                                                                         piva,
                                                                                                                         ent("sa_cod"),
                                                                                                                         ent("appezza"),
                                                                                                                         ent("id_reg"),
                                                                                                                         reqnum,
                                                                                                                         ObjParametri_Server)

                                        If chk_ok = False Then
                                            Throw New Exception(String.Format("Errore in scrittura tabella Imprese_ISCCXElaborazioniXRichieste per la chiave ( {0} / {1} / {2} / {3} / {4} / {5} )", id_elaborazione, piva, ent("sa_Cod"), ent("appezza"), ent("id_reg"), reqnum))
                                        End If
                                        Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("Sottomessa richiesta per Entita_cod {0} su algorimo {1}", ent("Entita_Cod"), alg("LayerAnalysisConfig_Cod")), False)
                                    Else
                                        Throw New Exception(String.Format("Errore in sottomissione richiesta per Entita_cod {0} su algorimo {1}", ent("Entita_Cod"), alg("LayerAnalysisConfig_Cod")))
                                    End If
                                End If
                            Next
                        Next
                        If dtEnt.Rows.Count > 0 Then
                            If xISCCw.ScriviElaborazioneAziendaISCC(id_elaborazione,
                                         piva,
                                         data_riferimento,
                                         ObjParametri_Server) = False Then
                                Throw New Exception(String.Format("Errore in scrittura tabella Imprese_ISCCXElaborazioni per la chiave ( {0} / {1} / {2} )", id_elaborazione, piva, data_riferimento))

                            End If
                        Else
                            Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("Azienda {0} - {1} senza impianti validi alla data {2}. ", piva, dtImp.Rows(0)("rag_soc"), data_riferimento), False)
                        End If
                        If Not ObjParametri_Server.objTransazione Is Nothing Then
                            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, ObjParametri_Server)
                        End If

                    Else
                        Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("Azienda {0} - {1} non aperta alla data {2}. Non è possibile effettuare il controllo per aziende non valide.", piva, dtImp.Rows(0)("rag_soc"), data_riferimento), False)
                        Throw New Exception(String.Format("Azienda {0} - {1} non aperta alla data {2}. Non è possibile effettuare il controllo per aziende non valide.", piva, dtImp.Rows(0)("rag_soc"), data_riferimento))
                    End If
                End If

            Next

            resp = True
        Catch ex As Exception
            If Not ObjParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_Server)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore, False)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, ObjParametri_Server)
        End Try
        Return resp
    End Function

    Public Function SottomettiElaborazioneMassivaAziendeInVisbilita(ByVal tipo_checklist As Integer,
                                                                    ByVal data_riferimento As DateTime,
                                                                    ByVal userRequest As String,
                                                                    ByRef ObjParametri_Server As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.Compliance_ISCC.SottomettiElaborazioneMassivaAziendeInVisbilita()"

        Dim MessaggioErrore As String = ""
        Dim resp As Boolean = False

        Dim DataElaborazione As DateTime = DateTime.Now
        Dim layerList As New List(Of Integer)
        layerList.Add(enum_Gis_LayerElementiGrafici_std.IMPIANTI)
        Dim pivaList As New List(Of String)

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim xChkListr As New AgronicaCoreGisDAL.CheckList_GIS_R
        Dim xISCCr As New AgronicaCoreGisDAL.Compliance_ISCC_R
        Dim xISCCw As New AgronicaCoreGisDAL.Compliance_ISCC_W
        Dim xPLayerR As New AgronicaCoreGisDAL.ProiezioniLayer_W
        Dim xPLayerW As New AgronicaCoreGisBIZ.ProiezioniLayer_W
        Dim xImpR As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim seq As New Agro_Sequenze

        If tipo_checklist = enum_GIS_CheckList_Type.ISCC Then
            Dim dtAziende = xChkListr.LeggiElencoAbilitazioniAziendeInVisibilità(tipo_checklist, userRequest, data_riferimento, "", "", ObjParametri_Server)

            Dim dtAziendeNoAbilitate = dtAziende.Select("Piva_Abilitata=-1")
            If dtAziendeNoAbilitate.Count() > 0 Then
                For Each row In dtAziendeNoAbilitate
                    If AggiungiAziendaAControllo(tipo_checklist, row("Piva"), ObjParametri_Server) = False Then
                        Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("Errore in abilitatazione a compliance iscc - piva {0} ", row("Piva")), False)
                        Throw New Exception("Errore in abilitatazione a compliance iscc. Verificare log elaborazione")
                    Else
                        pivaList.Add(row("PIVA"))
                    End If
                Next
            End If
        End If

        ''se eseguo una richista per la compliance ISCC 
        If pivaList Is Nothing OrElse pivaList.Count <= 0 Then
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("Tutte le aziende in visibilità all'utente sono già abilitate al controllo ISCC oppure hanno il calcolo aggiornato ad una data posteriore al {0}. Impossibile proseguire", data_riferimento), False)
            Throw New Exception(String.Format("Tutte le aziende in visibilità all'utente sono già abilitate al controllo ISCC oppure hanno il calcolo aggiornato ad una data posteriore al {0}. Impossibile proseguire", data_riferimento))
        End If

        ''se eseguo una richista per la compliance ISCC 
        'If pivaList Is Nothing OrElse pivaList.Count <= 0 Then
        '    Scrivi_LOG(ObjParametri_Server, NomeRoutine, "Tutte le aziende in visibilità all'utente sono già abilitate al controllo ISCC. Eseguire la richiesta di calcolo puntuale", False)
        '    Throw New Exception("Tutte le aziende in visibilità all'utente sono già abilitate al controllo ISCC. Eseguire la richiesta di calcolo puntuale")
        'End If

        Try
            '0. recupero l'elenco degli algoritmi da sottomettere recuperandoli dalla prima checklist del tipo indicato , valido alla data indicata
            Dim dtCheckList = xChkListr.LeggiChecklist(0, tipo_checklist, data_riferimento, "", "LayerAnalysisConfig_Cod ASC", ObjParametri_Server)
            If dtCheckList.Rows.Count <= 0 Then
                Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("Nessuna checklist di tipo {0} valida al {1}. Impossibile proseguire", [Enum].Parse(GetType(enum_GIS_CheckList_Type), tipo_checklist).ToString(), data_riferimento), False)
                Throw New Exception(String.Format("Nessuna checklist di tipo {0} valida al {1}. Impossibile proseguire", [Enum].Parse(GetType(enum_GIS_CheckList_Type), tipo_checklist).ToString(), data_riferimento))
            End If

            Dim dtAlgList = dtCheckList.DefaultView.ToTable(True, "LayerAnalysisConfig_Cod")
            If dtAlgList.Rows.Count <= 0 Then
                Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("Nessun algoritmo trovato nelle checklist di tipo {0} valida al {1}. Impossibile proseguire", [Enum].Parse(GetType(enum_GIS_CheckList_Type), tipo_checklist).ToString(), data_riferimento), False)
                Throw New Exception(String.Format("Nessun algoritmo trovato nelle checklist di tipo {0} valida al {1}. Impossibile proseguire", [Enum].Parse(GetType(enum_GIS_CheckList_Type), tipo_checklist).ToString(), data_riferimento))
            End If

            '1. per ogni azienda:
            '   a. check azienda non chiusa -> se non valida alla data ERRORE
            '   b. leggi elenco impianti validi alla data
            '   c. per ogni impianto
            '       i. check se ha il poligono -> se non ce l'ha scarta l'impianto
            '       ii. leggi entita_cod dell'impianto
            '       iii. per ogni algoritmo recuperato dalla checklist
            '           1) sottometti una richiesta di elaborazione algoritmo proiezione gis con i seguenti parametri aggiuntivi
            '               a) ID_Elaborazione
            '               b) Tipo_CheckList
            '               c) ID_CheckList

            Dim id_elaborazione = seq.NuovoId_Tabella("Imprese_ISCCXElaborazioni", 0, CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, ObjParametri_Server)

            For Each piva In pivaList
                Dim dtImp = xImpR.Leggi(piva, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", ObjParametri_Server)
                If dtImp.Rows.Count <= 0 Then
                    Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("Nesuna azienda trovata con la partita iva {0}. Impossibile proseguire.", piva), False)
                    Throw New Exception(String.Format("Nesuna azienda trovata con la partita iva {0}. Impossibile proseguire.", piva))
                Else
                    If dtImp.Rows(0)("Validita_Inizio") <= data_riferimento AndAlso dtImp.Rows(0)("Validita_Fine") >= data_riferimento Then
                        'lavez - 06/11/2024 - genero il group id da associare a tutte le richieste di tutti gli algoritmi della checklist per l'intera azienda
                        Dim groupId = seq.NuovoId_Tabella("GIS_GroupID", 0, CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, ObjParametri_Server)

                        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagTransazioneLocale, FlagTransazioneLocale, ObjParametri_Server)
                        'Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("Recupero elenco impianti validi alla data {0} per la piva {1}", data_riferimento, piva))
                        Dim dtEnt = xISCCr.LeggiElencoEntitaDaPianoColturale(piva,
                                                                             0,
                                                                             data_riferimento,
                                                                             "",
                                                                             "",
                                                                             ObjParametri_Server)

                        'Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("trovati {0} Impianti con cartografia ", dtEnt.Rows.Count))

                        For Each ent In dtEnt.Rows
                            For Each alg In dtCheckList.Rows
                                If alg("Applica_Risultato") = True Then
                                    Dim reqnum As Integer = -1
                                    Dim chk_ok As Integer = False
                                    chk_ok = xPLayerW.AttivaDisattivaConfigurazioneSuEntita(alg("LayerAnalysisConfig_Cod"),
                                                                                   ent("Entita_Cod"),
                                                                                   0,
                                                                                   0,
                                                                                   True,
                                                                                   reqnum,
                                                                                   ObjParametri_Server,
                                                                                   False,
                                                                                   False,
                                                                                   JsonConvert.SerializeObject(New ParametriAggiuntiviAlgoritmiCartografici_ISCC() With {
                                                                                    .CheckList_Type = tipo_checklist,
                                                                                    .CheckList_ID = alg("GIS_LayerAnalysisConfig_CheckList_Cod"),
                                                                                    .Elaborazione_ID = id_elaborazione
                                                                                   }),
                                                                                   groupId)
                                    If chk_ok = True Then
                                        chk_ok = xISCCw.ScriviRichiestaElaborazioneAlgoitmoPerCompliance_ISCC_Con_Chiave_Impianto(id_elaborazione,
                                                                                                                         piva,
                                                                                                                         ent("sa_cod"),
                                                                                                                         ent("appezza"),
                                                                                                                         ent("id_reg"),
                                                                                                                         reqnum,
                                                                                                                         ObjParametri_Server)

                                        If chk_ok = False Then
                                            Throw New Exception(String.Format("Errore in scrittura tabella Imprese_ISCCXElaborazioniXRichieste per la chiave ( {0} / {1} / {2} / {3} / {4} / {5} )", id_elaborazione, piva, ent("sa_Cod"), ent("appezza"), ent("id_reg"), reqnum))
                                        End If
                                        'Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("Sottomessa richiesta per Entita_cod {0} su algorimo {1}", ent("Entita_Cod"), alg("LayerAnalysisConfig_Cod")))
                                    Else
                                        Throw New Exception(String.Format("Errore in sottomissione richiesta per Entita_cod {0} su algorimo {1}", ent("Entita_Cod"), alg("LayerAnalysisConfig_Cod")))
                                    End If
                                End If
                            Next
                        Next
                        If dtEnt.Rows.Count > 0 Then
                            If xISCCw.ScriviElaborazioneAziendaISCC(id_elaborazione,
                                         piva,
                                         data_riferimento,
                                         ObjParametri_Server) = False Then
                                Throw New Exception(String.Format("Errore in scrittura tabella Imprese_ISCCXElaborazioni per la chiave ( {0} / {1} / {2} )", id_elaborazione, piva, data_riferimento))

                            End If
                        Else
                            Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("Azienda {0} - {1} senza impianti validi alla data {2}. ", piva, dtImp.Rows(0)("rag_soc"), data_riferimento), False)
                        End If
                        If Not ObjParametri_Server.objTransazione Is Nothing Then
                            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, ObjParametri_Server)
                        End If
                    Else
                        Scrivi_LOG(ObjParametri_Server, NomeRoutine, String.Format("Azienda {0} - {1} non aperta alla data {2}. Non è possibile effettuare il controllo per aziende non valide.", piva, dtImp.Rows(0)("rag_soc"), data_riferimento), False)
                        Throw New Exception(String.Format("Azienda {0} - {1} non aperta alla data {2}. Non è possibile effettuare il controllo per aziende non valide.", piva, dtImp.Rows(0)("rag_soc"), data_riferimento))
                    End If
                End If

            Next

            resp = True
        Catch ex As Exception
            If Not ObjParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_Server)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore, False)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, ObjParametri_Server)
        End Try
        Return resp
    End Function

    Public Function AggiornaEsitoRichiestaElaborazione(ByVal Id_Elaborazione As Integer,
                                                        ByVal piva As String,
                                                        ByVal sa_cod As Integer,
                                                        ByVal appezza As Integer,
                                                        ByVal id_Reg As Integer,
                                                        ByVal ReqNum As Integer,
                                                       ByVal Esito As Integer,
                                                       ByRef ObjParametri_Server As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.Compliance_ISCC.AggiornaEsitoRichiestaElaborazione()"

        Dim MessaggioErrore As String = ""
        Dim resp As Boolean = False

        Dim xISCCr As New AgronicaCoreGisDAL.Compliance_ISCC_R
        Dim xISCCw As New AgronicaCoreGisDAL.Compliance_ISCC_W

        Try
            resp = xISCCw.ModificaRichiestaElaborazioneEntitaISCC(Id_Elaborazione, piva, sa_cod, appezza, id_Reg, ReqNum, Esito, ObjParametri_Server)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore, False)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return resp
    End Function

    Public Function VerificaAggiornaStatoElaborazioneRicheistaElaborazioneISCC(ByVal Id_Elaborazione As Integer,
                                                        ByVal piva As String,
                                                        ByVal sa_cod As Integer,
                                                        ByVal appezza As Integer,
                                                        ByVal id_Reg As Integer,
                                                        ByVal ReqNum As Integer,
                                                       ByVal Esito As Integer,
                                                       ByRef ObjParametri_Server As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.Compliance_ISCC.VerificaAggiornaStatoElaborazioneRicheistaElaborazioneISCC()"

        Dim MessaggioErrore As String = ""
        Dim resp As Boolean = False

        Dim xISCCr As New AgronicaCoreGisDAL.Compliance_ISCC_R
        Dim xISCCw As New AgronicaCoreGisDAL.Compliance_ISCC_W
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagTransazioneLocale, FlagTransazioneLocale, ObjParametri_Server)

            '0. modifico la richiesta di elaborazione puntuale
            If xISCCw.ModificaRichiestaElaborazioneEntitaISCC(Id_Elaborazione, piva, sa_cod, appezza, id_Reg, ReqNum, Esito, ObjParametri_Server) Then
                '1. recupero l'elenco di tutte richieste per la piva della richiesta
                Dim dtc = xISCCr.LeggiElencoRichiesteXElaborazione(Id_Elaborazione, piva, 0, "", "", ObjParametri_Server)
                If dtc.Rows.Count <= 0 Then
                    '1.a - se non ne ho nessuna -> errore
                    Throw New Exception(String.Format("Nessuna richiesta elaborazione trovata la chiave {0} / {1}", Id_Elaborazione, piva))
                Else
                    '1.b.1 - verifico se ci sono elaborazioni in pending (nel caso non faccio nulla)
                    Dim rowList = dtc.Select("Esito_Elaborazione = -1").ToList()
                    If rowList.Count <= 0 Then
                        '1.b.2 - verifico se ci sono record in stato KO -> in tal caso imposto l'azienda come NON COMPLIANT
                        Dim rowListKO = dtc.Select("Esito_Elaborazione = 0").ToList()
                        Dim dtc2 = xISCCr.LeggiElencoElaborazioni(Id_Elaborazione, piva, -1, AGRODATAINIZIO, "", "", ObjParametri_Server)
                        If dtc2.Rows.Count <= 0 Then
                            Throw New Exception(String.Format("Nessuna richiesta elaborazione trovata l'azienda {0} / {1}", Id_Elaborazione, piva))
                        Else
                            If dtc2.Rows(0)("Esito_Cod") = -1 Then
                                If xISCCw.ModificaElaborazioneAziendaISCC(Id_Elaborazione, piva, If(rowListKO.Count > 0, 0, 1), ObjParametri_Server) Then
                                    xISCCw.AggiornaDatiComplianceISCCSuImpresa(piva, If(rowListKO.Count > 0, 0, 1), DateTime.Now, ObjParametri_Server)
                                    resp = True
                                Else
                                    Throw New Exception(String.Format("Errore in aggiornamento di stato richiesta elaborazione {0} / {1}", Id_Elaborazione, piva))
                                End If
                            Else
                                resp = True
                                'non fare nulla
                            End If
                        End If

                    Else
                        resp = True
                        'non fare nulla. aggiorni lo stato della richiesta\piva SOLO quando tutte le richieste sono state elaborate
                    End If
                End If
            Else
                Throw New Exception(String.Format("Errore in modificato stato elaborazione per l'elaborazione {0} / {1}", Id_Elaborazione, ReqNum))
            End If
            If Not ObjParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, ObjParametri_Server)
            End If
        Catch ex As Exception
            If Not ObjParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_Server)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore, False)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, ObjParametri_Server)
        End Try

        Return resp
    End Function

    Public Function VerificaAziendaAbilitataISCC(ByVal checklist_type As Integer,
                                                 ByVal piva As String,
                                                 ByRef ObjParametri_Server As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.Compliance_ISCC.VerificaAziendaAbilitataISCC()"

        Dim MessaggioErrore As String = ""
        Dim resp As Boolean = False

        Dim xISCCr As New AgronicaCoreGisDAL.CheckList_GIS_R

        Try
            Dim dt = xISCCr.LeggiElencoAbilitazioniAziende(checklist_type, piva, -1, "", "", ObjParametri_Server)
            If dt.Rows.Count > 0 Then
                If dt.Rows(0)("Stato_Cod") = 1 Then
                    resp = True
                End If
            End If
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore, False)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp

    End Function

    Public Function LeggiElencoElaborazioniXTipoCheckList(ByVal tipo As Integer,
                                            ByVal dataRefStart As DateTime,
                                            ByVal dataRefEnd As DateTime,
                                            ByVal piva As String,
                                            ByRef ObjParametri_Server As AgronicaCoreParametri) As LeggiElencoElaborazioniMassive_Out
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.Compliance_ISCC.LeggiElencoElaborazioni()"

        Dim MessaggioErrore As String = ""
        Dim resp As LeggiElencoElaborazioniMassive_Out = Nothing

        Dim xISCCr As New AgronicaCoreGisDAL.CheckList_GIS_R

        Try
            resp = New LeggiElencoElaborazioniMassive_Out
            resp.elenco = New List(Of ElencoElaborazioniMassivePerAzienda)
            resp.elenco.Add(New ElencoElaborazioniMassivePerAzienda With {
                                .piva = piva,
                                .elencoElaborazioniPerTipo = New List(Of ElencoElaborazioniMassivePerTipoCheckList)
                            })

            Dim dt = xISCCr.LeggiElencoElaborazioniMassiveConDettaglio(tipo, dataRefStart, dataRefEnd, piva, "", "", ObjParametri_Server)

            If dt.Rows.Count > 0 Then


                Dim elabPerTipo As ElencoElaborazioniMassivePerTipoCheckList = Nothing
                Dim elaborazione As ElencoElaborazioniMassiveDettaglio = Nothing
                Dim dettaglio As ElencoElaborazioniMassiveDettaglioXAlgoritmo = Nothing

                Dim idAlg As Integer = -1
                Dim idElab As Integer = -1
                Dim CheckListId As Integer = -1

                For Each row In dt.Rows
                    If idAlg = -1 OrElse idAlg <> row("LayerAnalysisConfig_Cod") _
                        OrElse idElab = -1 OrElse idElab <> row("ID_Elaborazione") _
                        OrElse CheckListId = -1 OrElse CheckListId <> row("GIS_LayerAnalysisConfig_CheckList_Cod") Then

                        If idAlg = -1 OrElse idAlg <> row("LayerAnalysisConfig_Cod") Then
                            If dettaglio IsNot Nothing Then
                                elaborazione.dettaglioAlgoritmi.Add(dettaglio)
                            End If
                            dettaglio = New ElencoElaborazioniMassiveDettaglioXAlgoritmo
                            dettaglio.id_algoritmo = row("LayerAnalysisConfig_Cod")
                            dettaglio.descrizione = row("LayerAnalysisConfig_Des")
                            dettaglio.esito = row("Esito_Algoritmo")
                            dettaglio.dettaglio = New List(Of ElencoElaborazioniMassiveDettaglioXAlgoritmoXRichiesta)
                            'dettaglio.esito = row("Esito_Elaborazione")
                            idAlg = row("LayerAnalysisConfig_Cod")
                        End If

                        If idElab = -1 OrElse idElab <> row("ID_Elaborazione") Then
                            If elaborazione IsNot Nothing Then
                                elabPerTipo.elencoElaborazioni.Add(elaborazione)
                            End If
                            elaborazione = New ElencoElaborazioniMassiveDettaglio
                            elaborazione.dettaglioAlgoritmi = New List(Of ElencoElaborazioniMassiveDettaglioXAlgoritmo)
                            elaborazione.id_elaborazione = row("ID_Elaborazione")
                            elaborazione.dataElaborazione = row("Data_Controllo")
                            elaborazione.esito = row("Esito_Richiesta")
                            idElab = row("ID_Elaborazione")
                        End If

                        If CheckListId = -1 OrElse CheckListId <> row("GIS_LayerAnalysisConfig_CheckList_Cod") Then
                            If elabPerTipo IsNot Nothing Then
                                resp.elenco(0).elencoElaborazioniPerTipo.Add(elabPerTipo)
                            End If
                            elabPerTipo = New ElencoElaborazioniMassivePerTipoCheckList
                            elabPerTipo.elencoElaborazioni = New List(Of ElencoElaborazioniMassiveDettaglio)
                            elabPerTipo.checkList_id = row("GIS_LayerAnalysisConfig_CheckList_Cod")
                            elabPerTipo.checkList_des = row("Descrizione")
                            CheckListId = row("GIS_LayerAnalysisConfig_CheckList_Cod")
                        End If

                    End If
                    dettaglio.dettaglio.Add(New ElencoElaborazioniMassiveDettaglioXAlgoritmoXRichiesta() With {
                                                .descrizione = row("Descrizione_Elaborazione"),
                                                .esito = row("Esito_Elaborazione")
                                            })
                Next
                elaborazione.dettaglioAlgoritmi.Add(dettaglio)
                elabPerTipo.elencoElaborazioni.Add(elaborazione)
                resp.elenco(0).elencoElaborazioniPerTipo.Add(elabPerTipo)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore, False)
            resp = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp

    End Function

    Public Function SottomettiRichiestaDiElaborazioneAsincrona(ByVal Payload As SottomettiElaborazioneMassivaGISCheckList_In,
                                                               ByRef ObjParametri_Server As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.Compliance_ISCC.SottomettiRichiestaDiElaborazioneAsincrona()"

        Dim MessaggioErrore As String = ""
        Dim resp As Boolean = False

        Dim xChkListr As New AgronicaCoreGisDAL.CheckList_GIS_R
        Dim xChkListw As New AgronicaCoreGisDAL.CheckList_GIS_W
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        If xChkListr.VerificaSeEsisteteRichiestaElaborazioneMassivaAsincronaXUtente(Payload.checkListType, -1, ObjParametri_Server.UtenteUsername, "", "", ObjParametri_Server) = True Then
            Throw New Exception(String.Format("Esiste già una richiesta di elaborazione massiva per l'utente {0}. Operazione non permessa", ObjParametri_Server.UtenteUsername))
        End If

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagTransazioneLocale, FlagTransazioneLocale, ObjParametri_Server)
            Dim seq As New Agro_Sequenze

            Dim newIdRequest = seq.NuovoId_Tabella("GIS_LayerAnalysisConfig_Checklist_Elab_Async", 0, CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, ObjParametri_Server)

            xChkListw.InserisciNuovaRichiestaElaborazioneMassivaAsincrona(newIdRequest, Payload.checkListType, JsonConvert.SerializeObject(Payload), ObjParametri_Server)

            If ObjParametri_Server.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, ObjParametri_Server)
            End If
            resp = True
        Catch ex As Exception
            If ObjParametri_Server.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_Server)
            End If
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore, False)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagTransazioneLocale, ObjParametri_Server)
        End Try

        Return resp
    End Function

    Public Function LeggiElencoRichiesteDaElaborare(ByVal CheckList_Type As Integer,
                                                    ByRef ObjParametri_Server As AgronicaCoreParametri) As List(Of SottomettiElaborazioneMassivaGISCheckList_Async)

        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.Compliance_ISCC.SottomettiRichiestaDiElaborazioneAsincrona()"

        Dim MessaggioErrore As String = ""
        Dim resp As List(Of SottomettiElaborazioneMassivaGISCheckList_Async) = Nothing

        Dim xISCCr As New AgronicaCoreGisDAL.CheckList_GIS_R
        Try
            resp = New List(Of SottomettiElaborazioneMassivaGISCheckList_Async)
            Dim dt = xISCCr.LeggiElencoRichiesteElaborazioniMassiveAsincrone(CheckList_Type, -1, "", "", ObjParametri_Server)
            For Each row In dt.Rows
                Dim p = JsonConvert.DeserializeObject(Of SottomettiElaborazioneMassivaGISCheckList_In)(row("Request_Payload"))
                resp.Add(New SottomettiElaborazioneMassivaGISCheckList_Async() With {
                            .id = row("GIS_LayerAnalysisConfig_Checklist_Elab_Async_Cod"),
                            .checkListType = p.checkListType,
                            .dataRiferimento = p.dataRiferimento,
                            .elencoPiva = p.elencoPiva,
                            .elaboraAziendeInVisibilita = p.elaboraAziendeInVisibilita,
                            .UserRequest = row("UserRequest"),
                            .DateTimeRequest = row("Data_Creazione")
                         })
            Next
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore, False)
            resp = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return resp
    End Function

    Public Function AggiornaRichiestaDiElaborazioneAsincrona(ByVal idRequest As Integer,
                                                             ByVal Stato As Integer,
                                                               ByRef ObjParametri_Server As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.Compliance_ISCC.AggiornaRichiestaDiElaborazioneAsincrona()"

        Dim MessaggioErrore As String = ""
        Dim resp As Boolean = False

        Dim xChkListw As New AgronicaCoreGisDAL.CheckList_GIS_W
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            resp = xChkListw.AggiornaRichiestaElaborazioneMassivaAsincrona(idRequest, Stato, ObjParametri_Server)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore, False)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

End Class
