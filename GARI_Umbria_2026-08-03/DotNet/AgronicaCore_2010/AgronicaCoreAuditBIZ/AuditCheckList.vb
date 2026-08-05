Imports System.Text
Imports System.Web
Imports System.Xml
Imports AgronicaCoreAnagrafeBIZ.AnagrafeNG
Imports AgronicaCoreAuditDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.Audit.AuditCompletamentoModel
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
'Imports AgronicaCoreXML


Public Class AuditCheckList

    Public Function LeggiWorkflowAudit(ByVal audit_Tipo As Integer, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim workFlow As String = ""
        Dim workflow_Cod = 0
        Dim MessaggioErrore As String = ""

        Dim ObjAudit_Impostazione As New AgronicaCoreAuditDAL.Audit_Impostazioni_R
        Dim dtImpostazioni As DataTable = ObjAudit_Impostazione.LeggiImpostazione(Enum_Audit_impostazione.Documentale_GestioneChecklist, "", "", MessaggioErrore, objParametri_Server)

        If dtImpostazioni.Rows.Count > 0 Then
            workFlow = dtImpostazioni.Rows(0).Item("Valore1")
        End If

        If workFlow.Length > 0 Then
            Dim workFlows = workFlow.Split("|")
            Dim area_Servizio = workFlows.Where(Function(x) x.Split("_").First.Equals(CStr(audit_Tipo)))
            If area_Servizio.Count > 0 Then
                workflow_Cod = CInt(area_Servizio.First.Split("_").Last)
            End If
        End If

        Return workflow_Cod
    End Function

    Public Function LeggiAuditDocumenti(ByVal Tipo As Integer,
                                        ByVal Piva As String,
                                        ByVal Riferimento_Cod As String,
                                        ByVal TipoDocumento_Cod As String,
                                        ByVal Data As Date,
                                        ByRef Valutazione As String,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        Optional ByRef objParametri_Utenti As AgronicaCoreParametri = Nothing,
                                        Optional Workflow_Cod As Integer = -1
                                        ) As String

        Dim Valore As String = ""
        Dim strStoricizzato As String = ""
        Dim strNonPresente As String = "NON PRESENTE"
        Dim MessaggioErrrore As String = ""
        Dim workflowAbilitato = False

        Select Case Workflow_Cod
            Case -1
                If IsNothing(objParametri_Utenti) Then
                    objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
                End If

                'Lettura Vincoli Upload
                Dim ObjAudit_Impostazione As New AgronicaCoreAuditDAL.Audit_Impostazioni_R
                Dim DtImpostazioni As DataTable = ObjAudit_Impostazione.LeggiImpostazione(Enum_Audit_impostazione.Documentale_GestioneWorkFlow, "", "", MessaggioErrrore, objParametri)

                workflowAbilitato = DtImpostazioni.Rows.Count > 0
            Case 0
                workflowAbilitato = False
            Case Else
                workflowAbilitato = True
        End Select

        If Piva <> "" AndAlso (Riferimento_Cod <> "" OrElse Tipo = 15 OrElse Tipo = 16 OrElse Tipo = 17 OrElse Tipo = 22) Then
            Dim auditLeggi As New Audit_Risposte_R
            ' fix per considerare anche i documenti scaduti nell'anno di compilazione della checklist (20012)
            Dim DataRiferimento = If(Tipo = 15 OrElse Tipo = 16, AGRODATAINIZIO, New Date(Data.Year, 1, 1))
            Dim DataUpload As Boolean = Tipo = 12 OrElse Tipo = 13 OrElse Tipo = 17
            Dim tipologie = auditLeggi.LeggiTipologieRiferimento(If(Tipo = 15 OrElse Tipo = 16 OrElse Tipo = 21, "", objParametri.PivaSuperUser), Riferimento_Cod, TipoDocumento_Cod, "", "", objParametri)
            Dim documenti = auditLeggi.LeggiAuditDocumenti(Piva, If(tipologie.Rows.Count > 0 OrElse Tipo = 17, Riferimento_Cod, ""), 0, TipoDocumento_Cod, 0, DataRiferimento, If(Tipo = 15 OrElse Tipo = 16, 11, 0), "", "", objParametri, DataUpload, WorkFlow_Documentale:=workflowAbilitato, Audit_Tipo:=Tipo)
            If documenti.Rows.Count > 0 Then
                Dim Categoria As String = ""
                Dim Descrizione As String = ""
                'Dim Valutazione As String = ""
                For Each doc In documenti.Rows
                    Dim stato As Integer = 0
                    Dim valido As Boolean = False
                    Dim non_valido As Boolean = False
                    Dim da_validare As Boolean = False

                    If workflowAbilitato Then
                        stato = doc.Item("Stato_Attuale")
                    Else
                        stato = CInt(doc.Item("Validazione_Flag"))
                    End If

                    If workflowAbilitato Then
                        valido = stato = 401 OrElse stato = 403 OrElse stato = 405
                        non_valido = stato = 402 OrElse stato = 404 OrElse stato = 406
                        da_validare = stato = 400
                    Else
                        valido = (stato = 1 OrElse stato = 2)
                        non_valido = (stato = -1 OrElse stato = -2)
                        da_validare = stato = 0
                    End If

                    '===============================================================================================
                    'Controllo Documento Storicizzato
                    '-----------------------------------------------------------------------------------------------
                    Select Case doc.Item("ChkStorico")

                        Case 1

                            If Valutazione = "" Then
                                Valutazione = strNonPresente
                            End If

                            strStoricizzato = " [Storicizzato]"

                        Case Else

                            If non_valido Then
                                Valutazione = "NON VALIDATO"
                            ElseIf valido AndAlso (Valutazione = "" Or Valutazione = strNonPresente) Then
                                Valutazione = "VALIDATO"
                            ElseIf da_validare AndAlso Valutazione <> "NON VALIDATO" Then
                                Valutazione = "DA VALIDARE"
                            End If

                            strStoricizzato = ""

                    End Select

                    If HttpContext.Current.Session("Lingua_Audit") = 1 Then
                        doc.Item("Area") = TraduciArea(doc.Item("Area"), 1)
                        doc.Item("Tipologia") = TraduciTipologia(doc.Item("Tipologia"), 1)
                        doc.Item("Descrizione_Scadenza") = TraduciDescrizioneScadenza(doc.Item("Descrizione_Scadenza"), 1)
                    End If

                    Categoria = doc.Item("Area") & "|" & doc.Item("Tipologia")
                    Descrizione &= If(Descrizione <> "", vbCrLf, "") & stato & "|" & doc.Item("Descrizione_Scadenza") & strStoricizzato & "|" & doc.Item("Allegati_Documenti_Numero") & "|" & Format(CDate(doc.Item("Data_Scadenza")), "dd/MM/yyyy" & "|" & doc.Item("Allegati_Documenti_Cod")) & "|" & doc.Item("ChkStorico")
                    If Tipo = 15 OrElse Tipo = 16 Then
                        Descrizione &= "|" & doc.Item("Valore_Indice")
                    End If
                Next
                Valore = Valutazione & "|" & Categoria & vbCrLf & Descrizione
            Else
                Valore = strNonPresente
            End If
        End If

        Return Valore

    End Function

    Private Function TraduciArea(Testo As String, Lingua As Integer)
        Dim testoTradotto = ""

        If Lingua = 1 Then
            Select Case Testo
                Case "Questionario Fornitori EUDR"
                    testoTradotto = "EUDR Provider Checklist"
            End Select
        End If

        Return testoTradotto

    End Function

    Private Function TraduciTipologia(Testo As String, Lingua As Integer)
        Dim testoTradotto = ""

        If Lingua = 1 Then
            Select Case Testo
                Case "Licenza FLEGT"
                    testoTradotto = "FLEGT certification"
                Case "Rischio non conformità al Regolamento (UE) 2023/1115"
                    testoTradotto = "Risk of non-compliance with Regulation (UE) 2023/1115"
                Case "Presenza di Foreste  nell'area di produzione"
                    testoTradotto = "Presence of forests in the production area"
                Case "Diffusione della Deforestazione o degrado forestale"
                    testoTradotto = "Forest Degradation"
                Case "Rischio di commistione"
                    testoTradotto = "Commingling risk"
            End Select
        End If

        Return testoTradotto

    End Function

    Private Function TraduciDescrizioneScadenza(Testo As String, Lingua As Integer)
        Dim testoTradotto = ""

        If Lingua = 1 Then
            Select Case Testo
                Case "Licenza FLEGT, "
                    testoTradotto = "FLEGT certification, "
                Case "Rischio non conformità al Regolamento (UE) 2023/1115, "
                    testoTradotto = "Risk of non-compliance with Regulation (UE) 2023/1115, "
                Case "Presenza di Foreste  nell'area di produzione, "
                    testoTradotto = "Presence of forests in the production area, "
                Case "Diffusione della Deforestazione o degrado forestale, "
                    testoTradotto = "Forest Degradation, "
                Case "Rischio di commistione, "
                    testoTradotto = "Commingling risk, "
            End Select
        End If

        Return testoTradotto

    End Function

    Public Function LeggiAreeDocumentale(params As ObjParams) as IEnumerable(of BaseCodeDescr)
        Dim auditDal As New AgronicaCoreAuditDAL.Audit_Stati_R
        Dim areas = auditDal.LeggiAreeWorkflow(params.ObjParametri_Server).AsEnumerable().
            Select(Function(row) New BaseCodeDescr(row.Field(Of Integer)("ID_Area"), row.Field(Of String)("Nome")))
        Return areas
    End Function

    Public Function LeggiTipiAudit(params As ObjParams) as IEnumerable(of BaseCodeDescr)
        Dim auditDal As New AgronicaCoreAuditDAL.Audit_R
        Dim types = auditDal.LeggiAuditTipi(params.ObjParametri_Server).AsEnumerable().
            Select(Function(row) New BaseCodeDescr(row.Field(Of integer)("Audit_Tipo"), row.Field(Of string)("Audit_Des")))
        Return types
    End Function

     Public Function LeggiServiziChecklist(params As ObjParams) as IEnumerable(of BaseCodeDescr)
        Dim auditDal As New AgronicaCoreAuditDAL.Audit_R
        Dim services = auditDal.LeggiServiziAuditChecklist(params.ObjParametri_Server).AsEnumerable().
            Select(Function(row) New BaseCodeDescr(row.Field(Of integer)("Servizio_Cod"), row.Field(Of string)("Servizio_Des")))
        Return services
    End Function

     Public Function LeggiServiziWorkflow(params As ObjParams) as IEnumerable(of BaseCodeDescr)
        Dim auditDal As New AgronicaCoreAuditDAL.Audit_R
        Dim services = auditDal.LeggiServiziAuditWorkflow(params.ObjParametri_Server).AsEnumerable().
            Select(Function(row) New BaseCodeDescr(row.Field(Of integer)("Servizio_Cod"), row.Field(Of string)("Servizio_Des")))
        Return services
    End Function

    Public Function LeggiAuditImpostazioni(impostazioneCod As Enum_Audit_impostazione, params As ObjParams) As String
        Dim auditDal As New AgronicaCoreAuditDAL.Audit_Impostazioni_R
        Dim dt = auditDal.LeggiImpostazione(impostazioneCod, String.Empty, String.Empty, "", params.ObjParametri_Server)
        Dim settingValue = ""
        If dt IsNot Nothing AndAlso dt.Rows.Count = 1 Then
            settingValue = dt.AsEnumerable().First().Field(Of String)("Valore1")
        End If
        Return settingValue
    End Function

    Public Sub ScriviAuditImpostazioni(impostazioneCod As Enum_Audit_impostazione, valore As String, params As ObjParams)
        Dim auditDal As New AgronicaCoreAuditDAL.Audit_Impostazioni_W
        auditDal.ScriviImpostazione(impostazioneCod, valore, params.ObjParametri_Server)
    End Sub

    Public Sub VerificaPermessoAudit(ByVal Audit_Tipo As Integer, ByRef Permesso As Integer, ByRef PermessoAdmin As Integer)
        Select Case Audit_Tipo
            Case enum_AuditPuaTipo.Audit_SchedaControlliALP
                Permesso = TipiEnumerativi.enum_Security_Attivita.Gestione_MenuControlliALP
                PermessoAdmin = TipiEnumerativi.enum_Security_Attivita.Gestione_MenuControlliALP_Admin
            Case enum_AuditPuaTipo.Audit_BIO_COPROB
                Permesso = TipiEnumerativi.enum_Security_Attivita.Audit_BIO_COPROB
                PermessoAdmin = TipiEnumerativi.enum_Security_Attivita.Audit_BIO_COPROB
            Case enum_AuditPuaTipo.Audit_SQNPI_COPROB
                Permesso = TipiEnumerativi.enum_Security_Attivita.Audit_SQNPI_COPROB
                PermessoAdmin = TipiEnumerativi.enum_Security_Attivita.Audit_SQNPI_COPROB
            Case enum_AuditPuaTipo.Audit_Filiera_Trasporti_COPROB
                Permesso = TipiEnumerativi.enum_Security_Attivita.Filiera_Trasporti_COPROB
                PermessoAdmin = TipiEnumerativi.enum_Security_Attivita.Filiera_Trasporti_COPROB
            Case enum_AuditPuaTipo.Audit_Enquete_certification_Hevea_brasiliensis
                Permesso = TipiEnumerativi.enum_Security_Attivita.Enquete_certification_Hevea_brasiliensis
                PermessoAdmin = TipiEnumerativi.enum_Security_Attivita.Enquete_certification_Hevea_brasiliensis
            Case enum_AuditPuaTipo.Audit_Azienda_Banca_Cambiano
                Permesso = TipiEnumerativi.enum_Security_Attivita.Banca_Cambiano_Azienda
                PermessoAdmin = TipiEnumerativi.enum_Security_Attivita.Banca_Cambiano_Azienda
            Case enum_AuditPuaTipo.Audit_Budwood_Projects
                Permesso = TipiEnumerativi.enum_Security_Attivita.Audit_Budwood_Projects
                PermessoAdmin = TipiEnumerativi.enum_Security_Attivita.Audit_Budwood_Projects
            Case enum_AuditPuaTipo.Audit_GlobalGap
                Permesso = TipiEnumerativi.enum_Security_Attivita.Gest_CartellaAziendale_GlobalGap
                PermessoAdmin = TipiEnumerativi.enum_Security_Attivita.Gest_CartellaAziendale_GlobalGap
            Case enum_AuditPuaTipo.Audit_Convenzionale_Greenyard
                Permesso = TipiEnumerativi.enum_Security_Attivita.Audit_Convenzionale_Greenyard
                PermessoAdmin = TipiEnumerativi.enum_Security_Attivita.Audit_Convenzionale_Greenyard
            Case enum_AuditPuaTipo.Audit_Biologico_Greenyard
                Permesso = TipiEnumerativi.enum_Security_Attivita.Audit_Biologico_Greenyard
                PermessoAdmin = TipiEnumerativi.enum_Security_Attivita.Audit_Biologico_Greenyard
            Case enum_AuditPuaTipo.Audit_BIO_FILENI
                Permesso = TipiEnumerativi.enum_Security_Attivita.Audit_BIO_FILENI
                PermessoAdmin = TipiEnumerativi.enum_Security_Attivita.Audit_BIO_FILENI
            Case enum_AuditPuaTipo.Audit_Fornitori_EUDR_INALCA
                Permesso = TipiEnumerativi.enum_Security_Attivita.Audit_Fornitori_EUDR
                PermessoAdmin = TipiEnumerativi.enum_Security_Attivita.Audit_Fornitori_EUDR
            Case enum_AuditPuaTipo.Audit_Controllo_DPI_DeMatteis
                Permesso = TipiEnumerativi.enum_Security_Attivita.Controllo_DPI_DeMatteis
                PermessoAdmin = TipiEnumerativi.enum_Security_Attivita.Controllo_DPI_DeMatteis
        End Select
    End Sub

    Public Function VerificaPermessoUploadDocumento(ByVal Piva As String,
                                                    ByVal Workflow_Cod As Integer,
                                                    ByVal Audit_Tipo As Integer,
                                                    ByVal Id_Tipologia As Integer,
                                                    ByVal Stato_Cod As Integer,
                                                    ByVal Data As String,
                                                    ByRef objParametri_Server As AgronicaCoreParametri,
                                                    ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim Errore As String = "" 'Inizializzazione
        Dim Gruppo_Utente_Cod As Integer = 0
        Dim Array_Stati() As String
        Dim Array_Gruppi() As String
        Dim _objImpostazioniRead As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim isSuperUser As Boolean = (objParametri_Utenti.SuperUserUsername = objParametri_Utenti.UtenteUsername)
        Dim Stato_Des As String = ""
        Dim jsonUpload As String = String.Empty
        Dim MessaggioErrrore As String = String.Empty
        Try


            'Controllo Workflow (in caso sia sconosciuto --> -1)
            If Workflow_Cod <> 0 Then

                'Lettura Vincoli Upload
                Dim ObjAudit_Impostazione As New AgronicaCoreAuditDAL.Audit_Impostazioni_R
                Dim dt As DataTable = ObjAudit_Impostazione.LeggiImpostazione(Enum_Audit_impostazione.Audit_Json_Upload_Doc, "", "", MessaggioErrrore, objParametri_Server)

                If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                    jsonUpload = CStr(dt.Rows(0).Item("Valore1"))
                End If

                If jsonUpload <> "" AndAlso jsonUpload <> "[]" Then

                    If Audit_Tipo = 0 And Id_Tipologia <> 0 Then

                        'Ricavo il tipo dall'id_tipologia (situazione inserimento documento direttamente da gestione documenti e non da checklist)
                        Dim auditLeggi As New Audit_Disposizioni_R
                        Dim auditRisposteLeggi As New Audit_Risposte_R
                        Dim auditAgronica As New AuditAgronicaWS(objParametri_Server)
                        Dim ddCodici As New Dictionary(Of String, String)

                        Dim codici As List(Of AuditCodiciModel) = auditAgronica.LeggiCodici(0, 0, 0, 0, Data, "Audit_Codici.Audit_Tipo In (12,13)")
                        For Each codice In codici
                            If Left(codice.Tipo, 3) = "doc" Then
                                If Id_Tipologia = Replace(codice.Tipo, "doc", "") Then
                                    Audit_Tipo = codice.Audit_Tipo
                                    Exit For
                                End If
                            End If
                        Next

                    End If

                    'Determino il workflow
                    If Workflow_Cod = -1 And Audit_Tipo <> 0 Then
                        Workflow_Cod = LeggiWorkflowAudit(Audit_Tipo, objParametri_Server)
                    End If

                    If Not isSuperUser And Workflow_Cod <> 0 And (Audit_Tipo = 12 Or Audit_Tipo = 13 Or Audit_Tipo = 20) Then

                        If Stato_Cod = 0 Then

                            'Determinazione dello stato della checklist nel periodo di competenza
                            'recupero pratica
                            Dim objAudit As New AgronicaCoreAuditDAL.Audit_R
                            Dim objPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R
                            Dim dtAudit = objAudit.Leggi(0, Audit_Tipo, 0, Piva, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, True)
                            If dtAudit.Rows.Count > 0 Then

                                dtAudit.DefaultView.Sort = "Validita_Inizio DESC"
                                Dim dtStato = objPratiche.Leggi_conStatoAttuale(dtAudit.DefaultView(0).Item("Pratica_Cod"), "", Piva, "", 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, 0, 0)
                                If dtStato.Rows.Count > 0 Then
                                    Stato_Des = CStr(dtStato.Rows(0).Item("Stato_Des"))
                                    Stato_Cod = CInt(dtStato.Rows(0).Item("Stato_Cod"))
                                End If
                            End If

                        End If

                        If Stato_Cod <> 0 Then

                            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                            Dim jArrayAudit As New JArray

                            jArrayAudit = JArray.Parse(jsonUpload)

                            For Each objRiga As JObject In jArrayAudit

                                If CInt(objRiga("tipo")) = Audit_Tipo Then

                                    Array_Stati = Split(objRiga("stati_disabilitati"), ",")
                                    For i = 0 To UBound(Array_Stati)
                                        If CInt(Array_Stati(i)) = Stato_Cod Then

                                            Errore = "La categoria non è valida poichè associata ad un audit in stato " & IIf(Trim(Stato_Des) = "", "attuale", Stato_Des) & "."

                                            'Lettura Gruppo Utente                                
                                            Dim Utente_xGruppi_R As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
                                            Dim Gruppi_Utente_R As New AgronicaCoreUtentiDAL.Gruppi_Utente_R
                                            'Lettura del gruppo
                                            Dim dt_Gruppo = Utente_xGruppi_R.Leggi(objParametri_Utenti.UtenteUsername, 0, "", "", objParametri_Utenti)
                                            If dt_Gruppo IsNot Nothing AndAlso dt_Gruppo.Rows.Count > 0 Then
                                                Gruppo_Utente_Cod = dt_Gruppo.Rows(0)("Gruppi_Utente_Cod")
                                            End If

                                            Array_Gruppi = Split(objRiga("gruppi_utente_eccezione"), ",")
                                            For j = 0 To UBound(Array_Gruppi)
                                                If CInt(Array_Gruppi(j)) = Gruppo_Utente_Cod Then
                                                    Errore = ""
                                                    Exit For
                                                End If
                                            Next
                                        End If

                                    Next
                                End If
                            Next

                        End If

                    End If

                End If

            End If

        Catch ex As Exception

            Errore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

        Finally

        End Try

        Return Errore

    End Function

    Public Function AggiornaAuditDocumenti(Audit_Tipo As Integer,
                                           Regolamento_Cod As Integer,
                                           Audit_Cod As Integer,
                                           Piva As String,
                                           Riferimento_Cod As String,
                                           Data As Date,
                                           ByRef codici As List(Of AuditCodiciModel),
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional Rintracciabilita As Integer = 0,
                                           Optional ByRef objParametri_Utenti As AgronicaCoreParametri = Nothing,
                                           Optional Workflow_Cod As Integer = -1
                                           ) As Integer

        Dim auditRisposteScrivi As New Audit_Risposte_W
        Dim aggiornamenti As Integer = 0
        Dim bBypass As Boolean = False

        For Each codice In codici

            Dim Valutazione As String = ""
            Dim TipoDocumento_Cod As String = ""

            bBypass = False

            '===========================================================================================================
            'Controllo Bypass
            '-----------------------------------------------------------------------------------------------------------
            '1. Controllo Bypass Codici SQNPI Coprob con Rintraccibilita = 0
            If Audit_Tipo = 13 AndAlso Regolamento_Cod = 3 AndAlso codice.Sezione_Cod = 1 AndAlso Rintracciabilita = 0 Then
                bBypass = True
            Else
                If Left(codice.Tipo, 3) = "doc" Then
                    TipoDocumento_Cod = Replace(codice.Tipo, "doc", "")
                ElseIf Left(codice.Tipo, 2) = "ue" Then
                    TipoDocumento_Cod = Replace(codice.Tipo, "ue", "")
                End If
                bBypass = String.IsNullOrEmpty(TipoDocumento_Cod)
            End If
            '===========================================================================================================

            If Not bBypass Then

                Dim Valore_2 = LeggiAuditDocumenti(Audit_Tipo,
                                                       Piva,
                                                       Riferimento_Cod,
                                                       TipoDocumento_Cod,
                                                       Data,
                                                       Valutazione,
                                                       objParametri,
                                                       objParametri_Utenti:=objParametri_Utenti,
                                                       Workflow_Cod:=Workflow_Cod)

                auditRisposteScrivi.AggiornaRisposte(Audit_Tipo, Regolamento_Cod, Audit_Cod, objParametri.PivaSuperUser, codice.Disp_Cod, codice.Punto_Numero, "Valore_2", Valore_2, objParametri)

                aggiornamenti += 1

            End If

        Next

        Return aggiornamenti

    End Function

    Public Function AggiornaAudit(Audit_Tipo As Integer, Regolamento_Cod As Integer, audit As String, ByRef objParametri As AgronicaCoreParametri) As String

        Dim rval As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            Dim auditAgronica As New AuditAgronicaWS(objParametri)
            Dim codici As List(Of AuditCodiciModel) = auditAgronica.LeggiCodici(Audit_Tipo, Regolamento_Cod, 0, 0, "")
            Dim auditArray As JArray = JArray.Parse(audit)

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            For Each obj As JObject In auditArray

                Dim Piva As String = CStr(obj("Piva"))
                Dim Audit_Cod As Integer = CInt(obj("Audit_Cod"))
                Dim Audit_Data As Date = CDate(CStr(obj("Validita_Inizio")))
                Dim Riferimento_Cod As String = CStr(obj("Riferimento_Cod"))
                Dim Rintracciabilita As Integer = CInt(obj("Rintracciabilita"))

                AggiornaAuditDocumenti(Audit_Tipo,
                                       Regolamento_Cod,
                                       Audit_Cod,
                                       Piva,
                                       Riferimento_Cod,
                                       Audit_Data,
                                       codici,
                                       objParametri,
                                       Rintracciabilita:=Rintracciabilita)

            Next

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            rval = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

        Finally

            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return rval

    End Function

    Public Function AggiornaWorkflow(Audit_Tipo As Integer, Regolamento_Cod As Integer, Audit_Cod As Integer, Piva As String, ByRef objParametri As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard

        Dim risp As New RispostaStandard

        Try

            Dim auditLeggi As New Audit_R
            Dim dtAudit As DataTable = auditLeggi.LeggiAudit(Audit_Cod, Audit_Tipo, Regolamento_Cod, Piva, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri)

            Dim Audit_Stato As Integer = dtAudit.Rows(0).Item("Audit_Stato")
            Dim Riferimento_Cod As String = dtAudit.Rows(0).Item("Riferimento_Cod")
            Dim riferimenti = LeggiAuditProgrammazioneEntita(Piva, Riferimento_Cod, objParametri)
            Dim riferimento As JObject = If(riferimenti.Count > 0, riferimenti.First, Nothing)

            If Not IsNothing(riferimento) AndAlso Not String.IsNullOrEmpty(riferimento.GetValue("Pratica_Cod")) Then

                Dim Pratica_Cod As Integer = CInt(riferimento.GetValue("Pratica_Cod"))
                Dim Servizio_Cod As Integer = CInt(riferimento.GetValue("Servizio_Cod"))
                Dim Stato_Iniziale As Integer = CInt(riferimento.GetValue("Stato_Cod"))
                Dim Stato_Pratica As String = CStr(riferimento.GetValue("Stato_Des"))

                Dim auditLeggiStati As New Audit_Stati_R
                Dim dtStati As DataTable = auditLeggiStati.LeggiStatiWorkflow(
                    Audit_Tipo, Audit_Stato, Servizio_Cod, Stato_Iniziale, 0,
                    enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

                Dim Stati_Avanzamento As New List(Of Integer)
                For Each stato In dtStati.Rows
                    Stati_Avanzamento.Add(stato.Item("Stato_Destinazione_cod"))
                Next

                Dim messaggioErrore = AuditLabel.LeggiEtichetta("StatoNonValido", "Stato non valido")

                If Stati_Avanzamento.Count = 0 Then
                    risp.Errore = messaggioErrore & ": <b>" & Stato_Pratica & "</b>"
                Else
                    Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_W
                    For Each Stato In Stati_Avanzamento
                        risp = objPratiche.impostaPratica(0, Piva, "", "",
                                                          Servizio_Cod,
                                                          Stato,
                                                          objParametri,
                                                          objParametri_Utenti,
                                                          0, "",
                                                          Pratica_Cod,
                                                          False, "",
                                                          AGRODATAINIZIO,
                                                          AGRODATAFINE, 0, 0)
                        If Not risp.RispostaOK Then
                            risp.Errore = messaggioErrore & ": <b>" & Stato_Pratica & "</b>"
                        Else
                            Exit For
                        End If
                    Next
                End If

            End If

        Catch ex As Exception

            risp.Errore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

        End Try

        If Not risp.RispostaOK Then
            Dim infoErrore = AuditLabel.LeggiEtichetta("ErroreAggiornamentoWorkflow", "Aggiornamento workflow non consentito")
            risp.Errore = infoErrore & "<br>" & risp.Errore
        End If

        Return risp

    End Function

    Public Function LeggiAuditRiferimenti(ByVal riferimento_cod As String,
                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                          Optional ByVal label As String = Nothing,
                                          Optional ByRef dtCampiCompleta As DataTable = Nothing) As JArray

        Dim jArrayRiferimenti As New JArray

        If Not String.IsNullOrEmpty(riferimento_cod) Then

            Dim chiave() As String = Split(riferimento_cod, "_")
            Dim tipo As String = chiave(0)
            Dim piva As String = chiave(1)

            Select Case tipo

                Case enum_TipoEntita.Centro

                    Dim sa_cod As Integer = If(chiave.Length > 2, chiave(2), 0)

                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    Dim dtCentri = objCentri.Leggi(piva, sa_cod, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)

                    For Each centro In dtCentri.Rows
                        Dim codice As String = tipo & "_" & centro.Item("Piva") & "_" & centro.Item("Sa_Cod")
                        Dim descrizione As String = If(label, "") & centro.Item("Sa_Nome")
                        jArrayRiferimenti.Add(New JObject(New JProperty("Riferimento_Des", descrizione), New JProperty("Riferimento_Cod", codice)))
                    Next

                Case enum_TipoEntita.Campo

                    Dim sa_cod As Integer = If(chiave.Length > 2, chiave(2), 0)
                    Dim campo_cod As Integer = If(chiave.Length > 3, chiave(3), 0)

                    If Not IsNothing(dtCampiCompleta) Then

                        Dim filtroDt = String.Format("piva = '{0}' and sa_cod = {1} and campo_cod = {2}", piva, sa_cod, campo_cod)
                        Dim dtCampiCompletaFiltrati = dtCampiCompleta.Select(filtroDt)

                        For Each campo In dtCampiCompletaFiltrati
                            AggiungiRiferimento(campo, tipo, label, jArrayRiferimenti)
                        Next

                    Else

                        Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R
                        Dim dtCampi = objCampi.Leggi(piva, sa_cod, campo_cod, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)

                        For Each campo As DataRow In dtCampi.Rows
                            AggiungiRiferimento(campo, tipo, label, jArrayRiferimenti)
                        Next

                    End If

                Case enum_TipoEntita.Contatto

                    Dim cod_contatto As String = If(chiave.Length > 2, chiave(2), "")
                    Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
                    Dim dtContatti = objContatti.Leggi_Contatti_ByCod_Rapporto(False, piva, cod_contatto, enum_Rapporti_Contabili_Standard.Fornitore, "", "", objParametri_Server)

                    For Each contatto In dtContatti.Rows
                        Dim codice As String = tipo & "_" & contatto("Piva") & "_" & contatto("Cod_Contatto")
                        Dim descrizione As String = If(label, "") & contatto.Item("Rag_Soc")
                        jArrayRiferimenti.Add(New JObject(New JProperty("Riferimento_Des", descrizione), New JProperty("Riferimento_Cod", codice)))
                    Next

                Case enum_TipoEntita.Impianto
                    Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                    Dim filtroAggiuntivo = If(label, "")
                    Dim specie = If(chiave.Length > 2, chiave(2), 0)
                    Dim dtImpianti = objImpianti.Leggi_Specie(piva, "", 0, 0, specie, 0, filtroAggiuntivo, "", objParametri_Server)
                    For Each impianto In dtImpianti.Rows
                        Dim codice As String = tipo & "_" & piva & "_" & impianto.Item("Veg_Cod")
                        Dim descrizione As String = impianto.Item("Veg_Des")
                        jArrayRiferimenti.Add(New JObject(New JProperty("Riferimento_Des", descrizione), New JProperty("Riferimento_Cod", codice)))
                    Next
            End Select

        End If

        Return jArrayRiferimenti

    End Function

    Private Sub AggiungiRiferimento(ByVal campo As DataRow,
                                    ByVal tipo As String,
                                    ByVal label As String,
                                    ByRef jArrayRiferimenti As JArray)

        Dim codice As String = tipo & "_" & campo.Item("Piva") & "_" & campo.Item("Sa_Cod") & "_" & campo.Item("Campo_Cod")
        Dim descrizione As String = If(label, "") & campo.Item("Sa_Nome") & " - " & campo.Item("Campo_Des")
        jArrayRiferimenti.Add(New JObject(New JProperty("Riferimento_Des", descrizione), New JProperty("Riferimento_Cod", codice)))

    End Sub

    Public Function LeggiFornitoriEUDR(ByVal riferimento_cod As String, ByRef objParametri_Server As AgronicaCoreParametri) As JArray

        Dim jArrayRiferimenti As New JArray

        If Not String.IsNullOrEmpty(riferimento_cod) Then

            Dim chiave() As String = Split(riferimento_cod, "_")
            Dim tipo As String = chiave(0)
            Dim piva As String = chiave(1)
            Dim cod_contatto As String = If(chiave.Length > 2, chiave(2), "")

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dtContatti = objContatti.LeggiFornitoriEUDR(piva, cod_contatto, objParametri_Server)

            For Each contatto In dtContatti.Rows

                Dim codice As String = tipo & "_" & contatto("Piva") & "_" & contatto("Cod_Contatto")
                Dim cod_fornitore As String = contatto.Item("Cod_Fornitore")
                Dim descrizione As String = contatto.Item("Fornitore")
                Dim indirizzo As String = ""
                Dim fornitoreEU As String = "0"
                Dim rischioPaese As String = ""

                'If Not String.IsNullOrEmpty(cod_fornitore) Then
                'descrizione &= " (" & cod_fornitore & ")"
                'End If

                If Not IsDBNull(contatto.Item("Indirizzo")) Then
                    indirizzo = contatto.Item("Indirizzo")
                    indirizzo &= " - " & contatto.Item("CAP")
                    indirizzo &= " " & contatto.Item("Frazione")
                    indirizzo &= " " & contatto.Item("Comune")
                    indirizzo &= " " & contatto.Item("Provincia")
                    indirizzo &= " " & contatto.Item("Stato")
                End If

                If Not IsDBNull(contatto.Item("PaeseEU")) Then
                    fornitoreEU = contatto.Item("PaeseEU")
                End If

                If Not IsDBNull(contatto.Item("Paese")) Then
                    rischioPaese = contatto.Item("Paese") & " = " & contatto.Item("Rischio")
                End If

                jArrayRiferimenti.Add(
                    New JObject(
                        New JProperty("Riferimento_Cod", codice),
                        New JProperty("Riferimento_Des", descrizione),
                        New JProperty("Cod_Fornitore", cod_fornitore),
                        New JProperty("Indirizzo", indirizzo),
                        New JProperty("FornitoreEU", fornitoreEU),
                        New JProperty("RischioPaese", rischioPaese)
                    ))

            Next

        End If

        Return jArrayRiferimenti

    End Function

    Public Function LeggiAuditProgrammazioneEntita(ByVal piva As String, ByVal riferimento_cod As String, ByRef objParametri_Server As AgronicaCoreParametri) As JArray

        Dim jArrayRiferimenti As New JArray

        If Not String.IsNullOrEmpty(riferimento_cod) Then

            Dim chiave() As String = Split(riferimento_cod, "_")
            Dim Programmazione_Cod As Integer = CInt(chiave(0))
            Dim Programmazione_Entita_Cod As Integer = CInt(chiave(1))

            Dim Programmazione_Des As String = ""
            Dim Programmazione_Des_Long As String = ""
            Dim Tipo_Pianificazione As Integer
            Dim TuttiCentri As Boolean
            Dim DT_Appezzamenti As New DataTable
            Dim DT_Intersezioni As New DataTable

            Dim objProgrammazione As New AgronicaCoreAnagrafeBIZ.Programmazione_R

            objProgrammazione.DT_Appezzamenti_Crea(DT_Appezzamenti)
            objProgrammazione.DT_Intersezioni_Crea(DT_Intersezioni)

            objProgrammazione.Pianificazione_Leggi(
                Programmazione_Cod, Programmazione_Des, Programmazione_Des_Long, "", 0, "", AGRODATAINIZIO, AGRODATAFINE,
                Tipo_Pianificazione, DT_Appezzamenti, DT_Intersezioni, TuttiCentri, objParametri_Server, True, True, True, True)

            For Each entita In DT_Appezzamenti.Rows

                If Programmazione_Entita_Cod = 0 OrElse entita.Item("Programmazione_Entita_Cod") = Programmazione_Entita_Cod Then

                    Dim codice As String = Programmazione_Cod & "_" & entita.Item("Programmazione_Entita_Cod")
                    Dim descrizione As String = entita.Item("App_Nome")
                    descrizione &= " - " & entita.Item("Veg_Des") & " " & entita.Item("Cul_Des")
                    descrizione &= " KPIN:" & entita.Item("KPIN") & " - Block Name:" & entita.Item("Block_Name")
                    Dim superficie As Decimal = entita.Item("Sup_App")

                    Dim Pratica_Cod As String = ""
                    Dim Pratica_Des As String = ""
                    Dim Stato_Cod As String = ""
                    Dim Stato_Des As String = ""
                    Dim Servizio_Cod As String = ""

                    Dim objPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R
                    Dim dtPratiche = objPratiche.Leggi_conStatoAttuale(0, "", piva, "", 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, 0, entita.Item("Programmazione_Entita_Cod"))

                    If dtPratiche IsNot Nothing AndAlso dtPratiche.Rows.Count > 0 Then
                        Pratica_Cod = dtPratiche.Rows(0).Item("Pratica_Cod")
                        Pratica_Des = dtPratiche.Rows(0).Item("Servizio_Des") & " - " & dtPratiche.Rows(0).Item("Stato_Des")
                        Stato_Cod = dtPratiche.Rows(0).Item("Stato_Cod")
                        Stato_Des = dtPratiche.Rows(0).Item("Stato_Des")
                        Servizio_Cod = dtPratiche.Rows(0).Item("Servizio_Cod")
                        If Programmazione_Entita_Cod = 0 Then
                            descrizione &= " - Stato Pratica: " & Stato_Des
                        End If
                    End If

                    jArrayRiferimenti.Add(
                        New JObject(
                            New JProperty("Riferimento_Cod", codice),
                            New JProperty("Riferimento_Des", descrizione),
                            New JProperty("Superficie", superficie),
                            New JProperty("Pratica_Cod", Pratica_Cod),
                            New JProperty("Pratica_Des", Pratica_Des),
                            New JProperty("Stato_Cod", Stato_Cod),
                            New JProperty("Stato_Des", Stato_Des),
                            New JProperty("Servizio_Cod", Servizio_Cod)
                        ))

                    If Programmazione_Entita_Cod <> 0 Then
                        Exit For
                    End If

                End If
            Next

        End If

        Return jArrayRiferimenti

    End Function


    Public Function LeggiAuditJson(Audit_Cod As Integer,
                                   Audit_Tipo As Integer,
                                   Regolamento_Cod As Integer,
                                   Piva As String,
                                   ByVal Data_Inizio As Date,
                                   ByVal Data_Fine As Date,
                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                   ByVal objParametri_Utenti As AgronicaCoreParametri) As String

        'Dim dp As New DataProvider
        'Dim dp As DataProvider = Nothing
        'SeScriviLogLeggiAuditJson(dp, objParametri_Server, "TipoAudit : " + Audit_Tipo.ToString())

        Dim praticheR As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim Workflow_Cod = LeggiWorkflowAudit(Audit_Tipo, objParametri_Server)
        Dim dr_search() As DataRow
        Dim auditAgronica As New AuditAgronicaWS(objParametri_Server)
        Dim ddStati As New Dictionary(Of String, String)

        If Workflow_Cod = 0 Then
            Dim stati As List(Of AuditStatiModel) = auditAgronica.LeggiStati(Audit_Tipo)
            ddStati = stati.ToDictionary(Function(x) x.Stato_Cod, Function(x) x.Stato_Des)
        Else
            Dim statiWorkFlow = praticheR.Leggi_StatiServizio(Workflow_Cod, objParametri_Server)
            For Each stato In statiWorkFlow.Rows
                ddStati.Add(CStr(stato.item("WAnagraficaStati_Cod")), CStr(stato.Item("WAnagraficaStati_Des")))
            Next
        End If

        Dim disposizioni As List(Of AuditDisposizioniModel) = auditAgronica.LeggiDisposizioni(Audit_Tipo, Regolamento_Cod, 0, 0)
        Dim ddDisposizioni = disposizioni.ToDictionary(Function(x) x.Disp_Cod, Function(x) x.Disp_Nome)

        Dim idArea As Integer = 0
        Dim tipoDoc As String = ""
        Dim ddCodici As New Dictionary(Of String, AuditCodiciModel)

        If Audit_Tipo = 12 OrElse Audit_Tipo = 13 OrElse Audit_Tipo = 14 OrElse Audit_Tipo = 15 OrElse Audit_Tipo = 16 OrElse Audit_Tipo = 17 OrElse Audit_Tipo = 20 OrElse Audit_Tipo = 21 OrElse Audit_Tipo = 22 Then
            Dim codici As List(Of AuditCodiciModel) = auditAgronica.LeggiCodici(Audit_Tipo, Regolamento_Cod, 0, 0, "")
            ddCodici = codici.ToDictionary(Function(x) x.Punto_Numero, Function(x) x)
            For Each codice In codici
                If Left(codice.Tipo, 3) = "doc" Then
                    tipoDoc = Replace(codice.Tipo, "doc", "")
                    Exit For
                End If
            Next
        End If

        Dim ddColonne As New Dictionary(Of String, String)
        Dim colonne = AuditLabel.LeggiEtichetta("ColonneAudit" + CStr(HttpContext.Current.Session("Lingua_Audit")), "")
        If Audit_Tipo = 21 AndAlso Not String.IsNullOrEmpty(colonne) Then
            Dim listaColonne = JsonConvert.DeserializeObject(Of List(Of String))(colonne)
            ddColonne = listaColonne.ToDictionary(Function(x) x.Split("=")(0), Function(x) x.Split("=")(1))
        End If

        Dim utentiLeggi As New Utenti_Dettagli_R
        Dim dtUtenti As DataTable = utentiLeggi.Leggi("", 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Utenti)
        Dim ddUtenti As New Dictionary(Of String, String)
        For Each r As DataRow In dtUtenti.Rows
            Dim utente = r.Item("Cognome") & " " & r.Item("Nome")
            If Not ddUtenti.ContainsKey(r.Item("CodFisc")) Then
                ddUtenti.Add(r.Item("CodFisc"), If(Trim(utente) = "", r.Item("UserName"), utente))
            End If
        Next

        'SeScriviLogLeggiAuditJson(dp, objParametri_Server, "LeggiAudit_B")

        Dim auditLeggi As New Audit_R
        Dim dt As DataTable = auditLeggi.LeggiAudit(Audit_Cod, Audit_Tipo, Regolamento_Cod, Piva, Data_Inizio, Data_Fine, "", "", objParametri_Server, True, Workflow_Cod > 0)

        'SeScriviLogLeggiAuditJson(dp, objParametri_Server, "LeggiAudit_E : " + dt.Rows.Count().ToString())

        Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
        Dim jArrayAudit As New JArray
        Dim auditRiferimento As Boolean = dt.Columns.Contains("Riferimento_Cod")

        Dim auditRisposteLeggi As New Audit_Risposte_R
        Dim dtRisposte As DataTable = auditRisposteLeggi.Leggi(Audit_Cod, Audit_Tipo, Regolamento_Cod, 0, Data_Inizio, Data_Fine, "", "", objParametri_Server)

        ' ricavo ID Area per aprire pagina documentale
        If Not String.IsNullOrEmpty(tipoDoc) Then
            idArea = auditRisposteLeggi.LeggiAreaDocumenti(tipoDoc, objParametri_Server)
        End If

        Dim listaAudit As New List(Of Integer)

        Dim dtCampiCompleta As DataTable = Nothing

        If Audit_Tipo = 12 OrElse Audit_Tipo = 13 Or Audit_Tipo = 17 Or Audit_Tipo = 20 Then
            'SeScriviLogLeggiAuditJson(dp, objParametri_Server, "LeggiCampi_B")
            Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R
            dtCampiCompleta = objCampi.Leggi("", 0, 0, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)
            'SeScriviLogLeggiAuditJson(dp, objParametri_Server, "LeggiCampi_E : " + dtCampiCompleta.Rows.Count().ToString())
        End If

        'SeScriviLogLeggiAuditJson(dp, objParametri_Server, "ForEach_B")

        For Each audit In dt.Rows

            ' fix per righe ripetute (19599)
            If Not listaAudit.Contains(audit.Item("Audit_Cod")) Then
                listaAudit.Add(audit.Item("Audit_Cod"))
            Else
                Continue For
            End If

            Dim objAudit As New JObject(
                New JProperty("Audit_Tipo", audit.Item("Audit_Tipo")),
                New JProperty("Regolamento_Cod", audit.Item("Regolamento_Cod")),
                New JProperty("Audit_Cod", audit.Item("Audit_Cod")),
                New JProperty("Profilo_Cod", audit.Item("Profilo_Cod")),
                New JProperty("Audit_SuperUser", audit.Item("Audit_SuperUser")),
                New JProperty("Audit_Stato", audit.Item("Audit_Stato")),
                New JProperty("Stato_Des", If(Not audit.Item("Stato_Cod").Equals(DBNull.Value) AndAlso ddStati.ContainsKey(audit.Item("Stato_Cod")), ddStati(audit.Item("Stato_Cod")), If(ddStati.ContainsKey(audit.Item("Audit_Stato")), ddStati(audit.Item("Audit_Stato")), ""))),
                New JProperty("Piva", audit.Item("Piva")),
                New JProperty("Piva_Url", Stringa_Codifica(audit.Item("Piva"), AgroKey_EncoderDecoder, objParametri_Server)),
                New JProperty("Rag_Soc", audit.Item("Rag_Soc")),
                New JProperty("Note", audit.Item("Note")),
                New JProperty("Campionato", audit.Item("Campionato")),
                New JProperty("Rintracciabilita", audit.Item("Rintracciabilita")),
                New JProperty("ID_NC", audit.Item("ID_NC")),
                New JProperty("Username_Modifica", audit.Item("Username_Modifica")),
                New JProperty("Utente", If(ddUtenti.ContainsKey(audit.Item("Username_Modifica")), ddUtenti(audit.Item("Username_Modifica")), audit.Item("Username_Modifica"))),
                New JProperty("Validita_Inizio", AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(audit.Item("Validita_Inizio"), DateTimeKind.Local)),
                New JProperty("Validita_Fine", AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(audit.Item("Validita_Fine"), DateTimeKind.Local)),
                New JProperty("ID_Area", idArea),
                New JProperty("Pratica_Cod", audit.Item("Pratica_Cod")),
                New JProperty("ExtraInfo", audit.Item("ExtraInfo"))
            )

            If Workflow_Cod > 0 Then
                objAudit.Add(New JProperty("Stato_Cod", audit.Item("Stato_Cod")))
                objAudit.Add(New JProperty("DataChiusuraPratica", audit.Item("Data_Chiusura_Pratica")))
            End If

            If Audit_Tipo <> 4 Then

                objAudit.Add(New JProperty("CUAA", audit.Item("CUAA")))
                objAudit.Add(New JProperty("Piva_OP", audit.Item("Piva_OP")))
                objAudit.Add(New JProperty("Rag_Soc_OP", audit.Item("Rag_Soc_OP")))
                objAudit.Add(New JProperty("Codice_Socio", audit.Item("Codice_Socio")))
                objAudit.Add(New JProperty("Contratto_Produzione", audit.Item("Contratto_Produzione")))
                objAudit.Add(New JProperty("Codice_Campo", audit.Item("Codice_Campo")))
                objAudit.Add(New JProperty("Filiera", audit.Item("Filiera")))

                ' riferimento elemento anagrafico
                If auditRiferimento AndAlso Not IsDBNull(audit.Item("Riferimento_Cod")) Then
                    Dim riferimento_cod As String = audit.Item("Riferimento_Cod")

                    If Audit_Tipo = 17 Then
                        objAudit.Add("Riferimento_Cod", riferimento_cod)
                    ElseIf Audit_Tipo = 21 Then
                        objAudit.Add("Riferimento_Cod", riferimento_cod)
                        If Audit_Cod <> 0 OrElse String.IsNullOrEmpty(audit.Item("ExtraInfo")) Then
                            Dim riferimenti = LeggiFornitoriEUDR(riferimento_cod, objParametri_Server)
                            Dim riferimento As JObject = If(riferimenti.Count > 0, riferimenti.First, Nothing)
                            If riferimento IsNot Nothing Then
                                objAudit.Add("Riferimento_Des", riferimento.GetValue("Riferimento_Des"))
                                objAudit.Add("CodFornitore", riferimento.GetValue("Cod_Fornitore"))
                                objAudit.Add("IndirizzoFornitore", riferimento.GetValue("Indirizzo"))
                                objAudit.Add("RischioPaese", riferimento.GetValue("RischioPaese"))
                                objAudit.Add("FornitoreEU", riferimento.GetValue("FornitoreEU"))
                                Dim info = JsonConvert.DeserializeObject(Of AuditExtraInfoFornitoreEUDR)(audit.Item("ExtraInfo"))
                                If (info IsNot Nothing) Then
                                    If (IsDate(info.DataValidazione)) Then
                                        objAudit.Add("DataValidazione", info.DataValidazione)
                                    Else
                                        objAudit.Add("DataValidazione", Format(audit.Item("Validita_Inizio"), "dd/MM/yyyy"))
                                    End If
                                Else
                                    objAudit.Add("DataValidazione", Format(audit.Item("Validita_Inizio"), "dd/MM/yyyy"))
                                End If
                            Else
                                objAudit.Add("Riferimento_Des", "")
                            End If
                        Else
                            Dim info = JsonConvert.DeserializeObject(Of AuditExtraInfoFornitoreEUDR)(audit.Item("ExtraInfo"))
                            objAudit.Add("Riferimento_Des", info.Fornitore)
                            objAudit.Add("CodFornitore", info.CodFornitore)
                            objAudit.Add("IndirizzoFornitore", info.Indirizzo)
                            objAudit.Add("RischioPaese", info.RischioPaese)
                            objAudit.Add("FornitoreEU", info.FornitoreEU)
                            If IsDate(info.DataValidazione) Then
                                objAudit.Add("DataValidazione", info.DataValidazione)
                            Else
                                objAudit.Add("DataValidazione", Format(audit.Item("Validita_Inizio"), "dd/MM/yyyy"))
                            End If
                        End If
                    Else
                        Dim riferimenti As New JArray
                        If Audit_Tipo = 14 Then
                            riferimenti = LeggiAuditProgrammazioneEntita(Piva, riferimento_cod, objParametri_Server)
                        Else
                            riferimenti = LeggiAuditRiferimenti(riferimento_cod, objParametri_Server, dtCampiCompleta:=dtCampiCompleta)
                        End If
                        Dim riferimento As JObject = If(riferimenti.Count > 0, riferimenti.First, Nothing)
                        objAudit.Add("Riferimento_Cod", riferimento_cod)
                        objAudit.Add("Riferimento_Des", If(riferimento Is Nothing, "", riferimento.GetValue("Riferimento_Des")))
                        If riferimento IsNot Nothing Then
                            If Audit_Tipo = 14 Then
                                objAudit.Add("Stato_Pratica_Cod", riferimento.GetValue("Stato_Cod"))
                                objAudit.Add("Stato_Pratica", riferimento.GetValue("Stato_Des"))
                                objAudit.Add("Superficie", riferimento.GetValue("Superficie"))
                            End If
                        End If
                    End If

                End If

                If Audit_Tipo = 12 OrElse Audit_Tipo = 13 OrElse Audit_Tipo = 15 OrElse Audit_Tipo = 16 OrElse Audit_Tipo = 17 OrElse Audit_Tipo = 20 OrElse Audit_Tipo = 21 Then
                    Dim stato = IIf(audit.Item("Stato_Cod").Equals(DBNull.Value), 0, audit.Item("Stato_Cod"))
                    If Workflow_Cod > 0 Then
                        Select Case stato
                            Case "500"
                                objAudit.Add("Stato", "BOX")
                            Case "501"
                                objAudit.Add("Stato", "!")
                            Case "502"
                                objAudit.Add("Stato", "...")
                            Case "503"
                                objAudit.Add("Stato", "OK")
                            Case "504"
                                objAudit.Add("Stato", "NO")
                            Case "505"
                                objAudit.Add("Stato", "S")
                            Case "506"
                                objAudit.Add("Stato", "R")
                            Case "507"
                                objAudit.Add("Stato", "NC")
                            Case "508"
                                objAudit.Add("Stato", "PSO")
                            Case "509"
                                objAudit.Add("Stato", "ALT")
                            Case "510"
                                objAudit.Add("Stato", "CI")
                            Case "511"
                                objAudit.Add("Stato", "VU")
                            Case "512"
                                objAudit.Add("Stato", "ALT")
                            Case "513"
                                objAudit.Add("Stato", "CA")
                            Case "514"
                                objAudit.Add("Stato", "AOK")
                            Case "515"
                                objAudit.Add("Stato", "AKO")
                            Case Else
                                objAudit.Add("Stato", "")
                        End Select
                    Else
                        If Audit_Tipo = 12 OrElse Audit_Tipo = 13 OrElse Audit_Tipo = 20 Then
                            If stato = "0" Then
                                objAudit.Add("Stato", "UF")
                            ElseIf stato = "1" Then
                                objAudit.Add("Stato", "OK")
                            ElseIf stato = "2" Then
                                objAudit.Add("Stato", "NO")
                            ElseIf stato = "3" Then
                                objAudit.Add("Stato", "!")
                            ElseIf stato = "4" Then
                                objAudit.Add("Stato", "...")
                            ElseIf stato = "5" Then
                                objAudit.Add("Stato", "R")
                            ElseIf stato = "6" Then
                                objAudit.Add("Stato", If(Audit_Tipo = 12 Or Audit_Tipo = 20, "S", "NC"))
                            Else
                                objAudit.Add("Stato", "")
                            End If
                        ElseIf Audit_Tipo = 21 Then
                            Dim s = audit.Item("Audit_Stato")
                            objAudit.Add("Stato", If(s = "1", "503", If(s = "2", "501", If(s = "3", "504", "500"))))
                        Else
                            objAudit.Add("Stato", If(audit.Item("Audit_Stato") = "1", "OK", If(audit.Item("Audit_Stato") = "2", "NO", "")))
                        End If
                    End If
                End If

                If Audit_Tipo = 12 Or Audit_Tipo = 20 Then
                    Dim info As New AuditExtraInfoBIO
                    If Not String.IsNullOrEmpty(audit.Item("ExtraInfo")) Then
                        info = JsonConvert.DeserializeObject(Of AuditExtraInfoBIO)(audit.Item("ExtraInfo"))
                    End If
                    objAudit.Add("EnteCertificazione", info.EnteCertificazione)
                    objAudit.Add("Valutatore", info.Valutatore)
                    objAudit.Add("MotivoSospesaEsclusa", info.MotivoSospesaEsclusa)
                    objAudit.Add("StabilimentoConferimento", info.StabilimentoConferimento)
                    objAudit.Add("Tecnico", info.Tecnico)
                    objAudit.Add("ConferimentoChiuso", If(info.ConferimentoChiuso, "SI", "NO"))
                    objAudit.Add("ConfiniRischio", If(info.ConfiniRischio, "SI", "NO"))
                    objAudit.Add("DescrizioneConfiniRischio", info.DescrizioneConfiniRischio)
                    objAudit.Add("SuperficiConversione", If(info.SuperficiConversione, "SI", "NO"))
                    objAudit.Add("DettaglioSuperficiConversione", info.DettaglioSuperficiConversione)
                    objAudit.Add("DettaglioSuperficiBio", info.DettaglioSuperficiBio)
                    objAudit.Add("Geolocalizzazione", LeggiPosizione(info.Geolocalizzazione))
                    objAudit.Add("SeminaAutunnale", info.SeminaAutunnale)
                    objAudit.Add("SbloccoCCPB", If(info.SbloccoCCPB, "SI", "NO"))
                    objAudit.Add("AnalisiMultiresidualeConforme", If(info.AnalisiMultiresidualeConforme, "SI", "NO"))
                    objAudit.Add("NumeroRapportoProva", info.NumeroRapportoProva)
                    objAudit.Add("SAValore", info.SAValore)
                    objAudit.Add("ComunicazioneAziendePositivita", If(info.ComunicazioneAziendePositivita, "SI", "NO"))
                    objAudit.Add("RiscontroAzienda", If(info.RiscontroAzienda, "SI", "NO"))
                    objAudit.Add("DataInvioControCampione", info.DataInvioControCampione)
                    objAudit.Add("ControCampioneConforme", If(info.ControCampioneConforme, "SI", "NO"))
                    objAudit.Add("NumeroRapportoProvaControCampione", info.NumeroRapportoProvaControCampione)
                    objAudit.Add("ControCampioneSAValore", info.ControCampioneSAValore)
                    objAudit.Add("ComunicazioneEntePositivita", If(info.ComunicazioneEntePositivita, "SI", "NO"))
                    objAudit.Add("SbloccoEntePositivita", If(info.SbloccoEntePositivita, "SI", "NO"))
                    objAudit.Add("MisuraPrecauzionalePrevista", info.MisuraPrecauzionalePrevista)
                    objAudit.Add("ModuloPrescrizioneRelazioneTecnica", If(info.ModuloPrescrizioneRelazioneTecnica, "SI", "NO"))
                    objAudit.Add("Foto", If(info.Foto, "SI", "NO"))
                    objAudit.Add("Bio", If(info.Bio, "SI", "NO"))
                    objAudit.Add("Bio_Text", info.Bio_Text)
                    objAudit.Add("FuoriBio", If(info.FuoriBio, "SI", "NO"))
                    objAudit.Add("FuoriBio_Text", info.FuoriBio_Text)
                    objAudit.Add("Declassato", If(info.Declassato, "SI", "NO"))
                    objAudit.Add("Declassato_Text", info.Declassato_Text)

                    If Audit_Tipo = 20 Then

                        'Lettura Cooperativa Conferimento
                        Dim ObjImpreseGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
                        Dim Piva_Padre As String = ""
                        Dim Rag_Soc_Padre As String = ""
                        Piva_Padre = ObjImpreseGerarchia.LeggiPadre(audit.Item("Piva"), objParametri_Server, Rag_Soc_Padre)
                        objAudit.Add("CooperativaConferente", Rag_Soc_Padre)

                    End If







                ElseIf Audit_Tipo = 13 Then
                    Dim info As New AuditExtraInfoSQNPI
                    If Not String.IsNullOrEmpty(audit.Item("ExtraInfo")) Then
                        info = JsonConvert.DeserializeObject(Of AuditExtraInfoSQNPI)(audit.Item("ExtraInfo"))
                    End If
                    objAudit.Add("PrimoConferimento", info.PrimoConferimento)
                    objAudit.Add("Valutatore", info.Valutatore)
                    objAudit.Add("NoteContatti", info.NoteContatti)
                    objAudit.Add("ConferimentoChiuso", If(info.ConferimentoChiuso, "SI", "NO"))
                    objAudit.Add("AnalisiMultiresiduale", If(info.AnalisiMultiresiduale, "SI", "NO"))
                    objAudit.Add("Tecnico", info.Tecnico)
                    objAudit.Add("Regione", info.Regione)
                    objAudit.Add("Telefono", info.Telefono)
                    objAudit.Add("Mail", info.Mail)
                    objAudit.Add("DelegatoDoc", info.DelegatoDoc)
                    objAudit.Add("OperatoreUff", info.OperatoreUff)
                    objAudit.Add("ValutatoreUff", info.ValutatoreUff)
                    objAudit.Add("AdesioneMisure", If(info.AdesioneMisure, "SI", "NO"))
                    objAudit.Add("EnteCertificazione", info.EnteCertificazione)
                    objAudit.Add("EttariVerificati", info.EttariVerificati)
                    objAudit.Add("EttariDomanda", info.EttariDomanda)
                    objAudit.Add("Geolocalizzazione", LeggiPosizione(info.Geolocalizzazione))
                    'objAudit.Add("DataChiusuraPratica", info.DataChiusuraPratica)
                    objAudit.Add("DataFatturazione", info.DataFatturazione)


                ElseIf Audit_Tipo = 17 Then
                    Dim info As New AuditExtraInfoTrasporti
                    If Not String.IsNullOrEmpty(audit.Item("ExtraInfo")) Then
                        info = JsonConvert.DeserializeObject(Of AuditExtraInfoTrasporti)(audit.Item("ExtraInfo"))
                    End If
                    objAudit.Add("Categoria", info.Categoria)
                    objAudit.Add("Categoria_Des", info.Categoria_Des)
                    objAudit.Add("CodiceContrattoTrasporto", info.CodiceContrattoTrasporto)
                    objAudit.Add("DittaTrasporto_Des", info.DittaTrasporto_Des)
                    objAudit.Add("Targa", info.Targa)
                    objAudit.Add("Conducente_Des", info.Conducente_Des)

                ElseIf Audit_Tipo = 18 Then
                    Dim info As New AuditExtraInfoEnqueteHevea
                    If Not String.IsNullOrEmpty(audit.Item("ExtraInfo")) Then
                        info = JsonConvert.DeserializeObject(Of AuditExtraInfoEnqueteHevea)(audit.Item("ExtraInfo"))
                    End If
                    objAudit.Add("Secteur_FPH_CI", info.Secteur)
                    objAudit.Add("Superficie_Hevea", info.Superficie_totale_hevea)
                    objAudit.Add("Nom_de_Enqueteur", info.Nom_de_Enqueteur)

                    dr_search = dtRisposte.Select("Audit_Cod = " & audit("Audit_Cod"))

                    If dr_search.Length <> 0 Then

                        For Each drRisposte In dr_search

                            If drRisposte("Disp_Cod") = 1 And drRisposte("Punto_Numero") = "110" Then
                                objAudit.Add(New JProperty("a1_3", drRisposte("Valore")))
                            End If
                            If drRisposte("Disp_Cod") = 1 And drRisposte("Punto_Numero") = "240" Then
                                If InStr(drRisposte("Valore"), "*") > 0 Then
                                    objAudit.Add(New JProperty("a1_8", drRisposte("PropostaCorrettiva")))
                                Else
                                    objAudit.Add(New JProperty("a1_8", drRisposte("Valore")))
                                End If
                            End If
                            If drRisposte("Disp_Cod") = 3 And drRisposte("Punto_Numero") = "030" Then
                                objAudit.Add(New JProperty("a3_1_De_0_12_ans", drRisposte("Valore")))
                            End If
                            If drRisposte("Disp_Cod") = 3 And drRisposte("Punto_Numero") = "040" Then
                                objAudit.Add(New JProperty("a3_1_De_13_16_ans", drRisposte("Valore")))
                            End If
                            If drRisposte("Disp_Cod") = 3 And drRisposte("Punto_Numero") = "050" Then
                                objAudit.Add(New JProperty("a3_1_De_16_18_ans", drRisposte("Valore")))
                            End If
                            If drRisposte("Disp_Cod") = 3 And drRisposte("Punto_Numero") = "070" Then
                                objAudit.Add(New JProperty("a3_2", drRisposte("Valore")))
                            End If
                            If drRisposte("Disp_Cod") = 3 And drRisposte("Punto_Numero") = "090" Then
                                objAudit.Add(New JProperty("a3_3", drRisposte("Valore")))
                            End If

                        Next

                    End If

                ElseIf Audit_Tipo = 22 Then

                    Dim stato = IIf(audit.Item("Stato_Cod").Equals(DBNull.Value), 0, audit.Item("Stato_Cod"))
                    If Workflow_Cod > 0 Then
                        Select Case stato
                            Case "1"
                                objAudit.Add("Stato", "OK")
                            Case "2"
                                objAudit.Add("Stato", "NO")
                        End Select


                    End If

                End If

                Dim completa As String = "NO"
                Dim cod_documenti As String = ""
                Dim num_registrazioni As Integer = 0
                Dim registrazioni = LeggiRegistrazioni(Audit_Tipo, audit.Item("Audit_Cod"), objAudit, dtRisposte, ddCodici, num_registrazioni, completa, cod_documenti, ddColonne, objParametri_Server)

                objAudit.Add("Completa", completa)
                objAudit.Add("Registrazioni", registrazioni)
                objAudit.Add("Num_Registrazioni", num_registrazioni)
                objAudit.Add("Cod_Documenti", cod_documenti)
            Else
                Dim riferimenti As New JArray
                If auditRiferimento AndAlso Not IsDBNull(audit.Item("Riferimento_Cod")) Then
                    Dim riferimento_cod As String = audit.Item("Riferimento_Cod")
                    riferimenti = LeggiAuditRiferimenti(riferimento_cod, objParametri_Server, dtCampiCompleta:=dtCampiCompleta)
                    Dim riferimento As JObject = If(riferimenti.Count > 0, riferimenti.First, Nothing)
                    objAudit.Add("Riferimento_Cod", riferimento_cod)
                    objAudit.Add("Riferimento_Des", If(riferimento Is Nothing, "", riferimento.GetValue("Riferimento_Des")))
                End If
            End If

            jArrayAudit.Add(objAudit)

        Next

        'SeScriviLogLeggiAuditJson(dp, objParametri_Server, "ForEach_E")

        Return JsonConvert.SerializeObject(jArrayAudit, Newtonsoft.Json.Formatting.None)

    End Function

    Private Function LeggiPosizione(ByVal Geolocalizzazione As String) As String
        If Not String.IsNullOrEmpty(Geolocalizzazione) Then
            Dim posizione = Replace(Geolocalizzazione, ",", ".").Split("|")
            If posizione.Length > 1 Then
                Return posizione(0) & ", " & posizione(1)
            End If
        End If

        Return ""
    End Function

    Private Sub SeScriviLogLeggiAuditJson(ByRef dp As DataProvider,
                                               ByRef objParametri_Server As AgronicaCoreParametri,
                                               ByVal messaggio As String)
        If Not IsNothing(dp) Then
            dp.Scrivi_LOG(objParametri_Server, "LeggiAuditJson", messaggio)
        End If

    End Sub

    Public Function LeggiRegistrazioni(Audit_Tipo As Integer, Audit_Cod As Integer, ByRef objAudit As JObject,
                                       ByRef dtRisposte As DataTable, ByRef ddCodici As Dictionary(Of String, AuditCodiciModel),
                                       ByRef num_registrazioni As Integer, ByRef completa As String, ByRef cod_documenti As String,
                                       ByRef ddColonne As Dictionary(Of String, String), ByRef ObjParametri_Server As AgronicaCoreParametri) As String

        Dim listaDocumenti As New List(Of String)
        Dim registrazioni As New StringBuilder
        Dim puntiKO As New List(Of String)
        Dim objPaesi As New AgronicaCoreMetaSchemaDAL.EUDR_Classificazione_Paesi_R
        Dim dtPaesi = objPaesi.Leggi("", "", "", ObjParametri_Server)
        Dim dtPaesiNonUE = dtPaesi.Select("PaeseEU = 0").
            Where(Function(r) Not r.IsNull("Codice")).
            Select(Function(r) r.Field(Of String)("Codice")).
            Distinct().
            ToList()

        For Each risposta In dtRisposte.Select("Audit_Cod=" & Audit_Cod)

            Dim codice = CStr(risposta.Item("Punto_Numero"))
            Dim valore = CStr(risposta.Item("Valore"))
            Dim valore2 = CStr(risposta.Item("Valore_2"))
            Dim rispostaAudit = "Risposta_" & codice

            If ddCodici.ContainsKey(codice) Then

                Dim codiceAudit = ddCodici(codice)
                Dim tipo = codiceAudit.Tipo

                If Audit_Tipo = 14 Then
                    If tipo = "z" Then
                        valore = If(valore = "1", "OK", If(valore = "2", "Not ideal", If(valore = "3", "Avoid", "")))
                    End If
                    objAudit.Add(rispostaAudit, valore)
                End If

                If Audit_Tipo = 12 OrElse Audit_Tipo = 13 OrElse Audit_Tipo = 15 OrElse Audit_Tipo = 16 OrElse Audit_Tipo = 17 OrElse Audit_Tipo = 20 OrElse Audit_Tipo = 22 Then
                    If tipo <> "num" Then
                        valore = If(valore = "1", "SI", If(valore = "0", "NO", valore))
                    End If
                    Dim valoreDocumenti = valore2.Split(vbCrLf)
                    Dim valutazione = valoreDocumenti(0)
                    If Not String.IsNullOrEmpty(valutazione) Then
                        valore &= " (" & valutazione.Split("|")(0) & ")"
                    End If
                    Dim numDoc As Integer = 0
                    For Each valDoc In valoreDocumenti
                        Dim dati = valDoc.Split("|")
                        If numDoc > 0 AndAlso dati.Length > 4 Then
                            If Not listaDocumenti.Contains(dati(4)) Then
                                listaDocumenti.Add(dati(4))
                            End If
                        End If
                        numDoc += 1
                    Next
                    objAudit.Add(rispostaAudit, valore)
                End If

                If Audit_Tipo = 21 Then
                    If codice = "092" Then
                        Dim listaPaesi = risposta.Item("Valore").Split("|")
                        Dim paesiNonUEPresenti As Boolean = False
                        For Each paese In listaPaesi
                            If dtPaesiNonUE.Contains(paese) Then
                                paesiNonUEPresenti = True
                                Exit For
                            End If
                        Next
                        If paesiNonUEPresenti Then
                            num_registrazioni += 4
                        End If
                    End If
                    If Left(tipo, 2) = "ue" AndAlso codiceAudit.Punteggio > 0 Then
                        For Each item In codiceAudit.Criterio.Split("|")
                            Dim opzione = item.Split("=")
                            Dim v = If(opzione.Length > 1, opzione(1), opzione(0))
                            If valore = v Then
                                If valore = "0" Then
                                    If ddColonne.ContainsKey(codice) AndAlso codiceAudit.Punteggio > 1 Then
                                        puntiKO.Add(ddColonne(codice))
                                    End If
                                ElseIf valore = "1" Then
                                    num_registrazioni += 1
                                ElseIf valore = "2" OrElse valore = "3" Then
                                    num_registrazioni += 2
                                End If
                                valore = opzione(0)
                                Exit For
                            End If
                        Next
                    ElseIf Left(tipo, 2) = "ms" Then
                        valore = Replace(valore, "|", ", ")
                    End If
                    objAudit.Add(rispostaAudit, valore)
                End If

            End If

            If Audit_Tipo = 11 Then
                If codice = "999999" Then
                    completa = If(valore = "1", "SI", "NO")
                Else
                    objAudit.Add(rispostaAudit, If(valore = "1", 1, 0))
                    registrazioni.Append(If(registrazioni.Length > 0, ", ", "") & codice)
                    num_registrazioni += 1
                End If
            End If
        Next

        ' lista codici documenti checklist
        If listaDocumenti.Count > 0 Then
            cod_documenti = String.Join(",", listaDocumenti)
        End If

        If puntiKO.Count > 0 Then
            registrazioni.Append(String.Join(", ", puntiKO))
        End If

        Return registrazioni.ToString

    End Function
    Public Function LeggiAudit(
        Audit_Cod As Integer,
        Audit_Tipo As Integer,
        Regolamento_Cod As Integer,
        Piva As String,
        ByVal Data_Inizio As Date,
        ByVal Data_Fine As Date,
        ByVal objParametri As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri
        ) As List(Of AuditModel)

        Dim praticheR As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim Workflow_Cod = LeggiWorkflowAudit(Audit_Tipo, objParametri)

        Dim auditAgronica As New AuditAgronicaWS(objParametri)
        Dim ddStati As New Dictionary(Of String, String)

        If Workflow_Cod = 0 Then
            Dim stati As List(Of AuditStatiModel) = auditAgronica.LeggiStati(Audit_Tipo)
            ddStati = stati.ToDictionary(Function(x) x.Stato_Cod, Function(x) x.Stato_Des)
        Else
            Dim statiWorkFlow = praticheR.Leggi_StatiServizio(Workflow_Cod, objParametri)
            For Each stato In statiWorkFlow.Rows
                ddStati.Add(CStr(stato.item("WAnagraficaStati_Cod")), CStr(stato.Item("WAnagraficaStati_Des")))
            Next
        End If
        ddStati.Add(-1, "")

        Dim auditLeggi As New Audit_R
        Dim auditRisposteLeggi As New Audit_Risposte_R
        Dim dt As DataTable = auditLeggi.LeggiAudit(Audit_Cod, Audit_Tipo, Regolamento_Cod, Piva, Data_Inizio, Data_Fine, "", "", objParametri, workFlow:=If(Workflow_Cod = 0, False, True))

        Dim rval As New List(Of AuditModel)

        For i = 0 To dt.Rows.Count - 1
            Dim stato As Integer = 0
            Dim dtRisposte As DataTable = auditRisposteLeggi.Controlla(dt.Rows(i).Item("Audit_Cod"), Audit_Tipo, Regolamento_Cod, 0, Data_Inizio, Data_Fine, "Valore=''", "", objParametri)
            If Workflow_Cod = 0 Then
                stato = dt.Rows(i).Item("Audit_Stato")
            Else
                If Not dt.Rows(i).Item("Stato_Cod").Equals(DBNull.Value) Then
                    stato = dt.Rows(i).Item("Stato_Cod")
                Else
                    Continue For
                End If
            End If
            Dim riferimento = dt.Rows(i).Item("Riferimento_Cod")
            Dim item As New AuditModel With {
                .Audit_Tipo = dt.Rows(i).Item("Audit_tipo"),
                .Regolamento_Cod = dt.Rows(i).Item("Regolamento_Cod"),
                .Audit_Cod = dt.Rows(i).Item("Audit_Cod"),
                .Audit_SuperUser = dt.Rows(i).Item("Audit_SuperUser"),
                .Audit_Stato = stato,
                .Stato_Des = If(ddStati.ContainsKey(stato), ddStati(stato), ""),
                .Piva = dt.Rows(i).Item("Piva"),
                .PivaReale = dt.Rows(i).Item("PivaReale"),
                .Piva_Url = Stringa_Codifica(dt.Rows(i).Item("Piva"), AgroKey_EncoderDecoder, objParametri),
                .Rag_Soc = dt.Rows(i).Item("Rag_Soc"),
                .Note = dt.Rows(i).Item("Note"),
                .Completa = (dtRisposte.Rows.Count = 0),
                .Username_Modifica = dt.Rows(i).Item("Username_Modifica"),
                .Riferimento = If(IsDBNull(riferimento), "", riferimento),
                .Validita_Inizio = AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(dt.Rows(i).Item("Validita_Inizio"), DateTimeKind.Local),
                .Validita_Fine = AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(dt.Rows(i).Item("Validita_Fine"), DateTimeKind.Local),
                .Rintracciabilita = dt.Rows(i).Item("Rintracciabilita")
            }

            If Audit_Tipo = 13 Then
                Dim Campionato As String = "Campionato da Valore Italia: " & If(dt.Rows(i).Item("Campionato") = "1", "SI", "NO")
                item.Note &= If(String.IsNullOrEmpty(item.Note), "", vbCrLf) & Campionato
            End If

            rval.Add(item)

        Next

        Return rval

    End Function

    Public Function ScriviAuditRisposte(ByVal Audit_Tipo As Integer,
                                        ByVal Regolamento_Cod As Integer,
                                        ByVal Audit_Cod As Integer,
                                        ByVal Piva As String,
                                        ByVal Riferimento_Cod As String,
                                        ByVal Data As Date,
                                        ByVal Stato As Integer,
                                        ByVal Note As String,
                                        ByVal Campionato As Integer,
                                        ByVal Risposte As String,
                                        ByRef Errore As String,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        Optional ByVal Rintracciabilita As Integer = 0,
                                        Optional ByVal ExtraInfo As String = "",
                                        Optional ByVal Pratica_Cod As Integer = 0,
                                        Optional ByRef objParametri_Utenti As AgronicaCoreParametri = Nothing)

        Const azioneUpdate As String = "UPD"
        Const azioneInsert As String = "INS"

        Dim azione As String = azioneUpdate
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim objPraticheW As New AgronicaCoreProfilazioneDAL.Pratiche_W

        Dim auditLeggi As New Audit_R
        Dim auditScrivi As New Audit_W
        Dim auditRisposteLeggi As New Audit_Risposte_R
        Dim auditRisposteScrivi As New Audit_Risposte_W
        Dim AgroSequenze As New Agro_Sequenze
        Dim Profilo_Cod As Integer = 0
        Dim codiciBypass As List(Of AuditCodiciModel)
        Dim bBypass As Boolean = False
        Dim Workflow_Cod = LeggiWorkflowAudit(Audit_Tipo, objParametri)

        If Audit_Tipo = 4 Then
            Dim auditIntervisteLeggi As New Audit_Interviste_R
            Dim dt As DataTable = auditIntervisteLeggi.LeggiInterviste(Audit_Tipo, Regolamento_Cod, 0, Piva, Data, Data, "", "", objParametri)
            If dt.Rows.Count > 0 Then
                Profilo_Cod = dt.Rows(0).Item("Intervista_Cod")
            End If
        End If

        If Audit_Tipo = 13 And Regolamento_Cod = 3 And Rintracciabilita = 0 Then
            'Leggi codici di tipo coprob sqnpi con regolamento = 3 e sezionale = 1 (vanno bypassati poicheè nascosti all'utente)
            Dim auditAgronica As New AuditAgronicaWS(objParametri)
            Dim auditCodiciLeggi As New Audit_Codici_R
            codiciBypass = auditAgronica.LeggiCodici(Audit_Tipo, Regolamento_Cod, 0, 1, Data)
        End If

        If Audit_Cod = 0 Then
            'Audit_Cod = AgroSequenze.Agronica_SequenzaTabelle_NuovoID("Audit", objParametri)
            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            Audit_Cod = AgroSequenze.NuovoId_Tabella("Audit", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)
            azione = azioneInsert
            Pratica_Cod = 0
        End If

        If (Audit_Tipo = 12 OrElse Audit_Tipo = 13 OrElse Audit_Tipo = 20 OrElse Audit_Tipo = 22) AndAlso Workflow_Cod > 0 Then
            Stato = 0
        End If

        Try

            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            If azione = azioneUpdate Then
                auditRisposteScrivi.Cancellazione(Audit_Tipo, Regolamento_Cod, Audit_Cod, objParametri.PivaSuperUser, 0, "", objParametri)
                auditScrivi.Cancellazione(Audit_Cod, objParametri.PivaSuperUser, Piva, objParametri)
            End If

            If azione = azioneInsert AndAlso Workflow_Cod > 0 Then
                Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_W
                Dim r = objPratiche.GeneraPratica(Piva,
                                                  Workflow_Cod,
                                                  "",
                                                  "",
                                                  Date.Now.ToString,
                                                  "",
                                                  objParametri,
                                                  New AgronicaCoreParametri,
                                                  ControllaEsistenzaPratica:=False,
                                                  Pratica_Cod,
                                                  -1)
            End If

            Dim Data_Fine As Date = Data 'Inizializzazione
            If Audit_Tipo = 21 Then
                'Checklist Fornitori EUDR
                Dim info = JsonConvert.DeserializeObject(Of AuditExtraInfoFornitoreEUDR)(ExtraInfo)
                If IsDate(info.DataValidazione) Then
                    Data_Fine = info.DataValidazione
                    Data_Fine = Data_Fine.AddYears(1)
                End If
            End If


            If auditScrivi.scrivi(Audit_Cod, Audit_Tipo, 0, Stato, Regolamento_Cod, Piva, Note, Campionato, Data, Data_Fine, Date.Now, Date.Now, objParametri.UsernameOperazione, objParametri.UsernameOperazione, objParametri, Riferimento_Cod, Profilo_Cod, Rintracciabilita, ExtraInfo, Pratica_Cod) Then

                If azione = azioneInsert AndAlso Workflow_Cod > 0 Then
                    objPraticheW.AggiornaNumero(Pratica_Cod, "Audit " & Audit_Cod, "", objParametri)
                End If

                Dim disposizioni As New List(Of Integer)
                Dim campi As List(Of AuditFormModel) = JsonConvert.DeserializeObject(Of List(Of AuditFormModel))(Risposte)
                Dim proposte As New Dictionary(Of String, String)
                Dim risposte2 As New Dictionary(Of String, String)

                For Each campo In campi
                    Dim Codice As String = campo.name
                    Dim Valore As String = campo.value
                    Dim Codici() As String = Split(Codice, "_")
                    If Codici(0) = "proposta" AndAlso Valore <> "" Then
                        proposte.Add(Codici(1) & "_" & Codici(2), Valore)
                    End If
                    If Codici(0) = "risposta2" AndAlso Valore <> "" Then
                        risposte2.Add(Codici(1) & "_" & Codici(2), Valore)
                    End If
                Next

                campi = multiSelct(campi)

                For Each campo In campi
                    Dim Codice As String = campo.name
                    Dim Valore As String = campo.value
                    Dim Codici() As String = Split(Codice, "_")

                    If Codici(0) = "risposta" AndAlso (Audit_Tipo = 4 OrElse Valore <> "-1") Then
                        Dim Disp_Cod As Integer = CInt(Codici(1))
                        Dim Punto_Numero As String = CStr(Codici(2))
                        Dim PropostaCorrettiva As String = ""
                        Dim Valore2 As String = ""
                        Dim Chiave = Codici(1) & "_" & Codici(2)

                        bBypass = False

                        If Audit_Tipo = 13 And Regolamento_Cod = 3 And Rintracciabilita = 0 Then
                            If Codici.Length > 0 Then
                                For Each CodiceBypass In codiciBypass
                                    If Trim(UCase(CodiceBypass.Punto_Numero)) = Trim(UCase(Punto_Numero)) Then
                                        'Audit sqnpi con regolamento 3 e sezionale_cod = 1 --> bypass salvataggio
                                        bBypass = True
                                        Exit For
                                    End If
                                Next
                            End If

                        End If

                        If Not bBypass Then
                            If proposte.ContainsKey(Chiave) Then
                                PropostaCorrettiva = proposte(Chiave)
                            End If
                            If risposte2.ContainsKey(Chiave) Then
                                Valore2 = risposte2(Chiave)
                            End If
                            auditRisposteScrivi.ScriviRisposte(Audit_Cod, Audit_Tipo, Punto_Numero, Regolamento_Cod, Disp_Cod, PropostaCorrettiva, Valore, Valore2, Data, Data, DateTime.Now, DateTime.Now, objParametri.UsernameOperazione, objParametri.UsernameOperazione, objParametri)
                            If Not disposizioni.Contains(Disp_Cod) Then
                                disposizioni.Add(Disp_Cod)
                            End If
                        End If

                    End If

                Next

                'Checklist COPROB: aggiornamento automatico info documenti collegati
                If Audit_Tipo = 12 OrElse Audit_Tipo = 13 OrElse Audit_Tipo = 15 OrElse Audit_Tipo = 16 OrElse Audit_Tipo = 17 OrElse Audit_Tipo = 20 OrElse Audit_Tipo = 21 OrElse Audit_Tipo = 22 Then
                    Dim workflow As Integer = If(Audit_Tipo = 21, 0, -1)
                    Dim auditAgronica As New AuditAgronicaWS(objParametri)
                    Dim codici As List(Of AuditCodiciModel) = auditAgronica.LeggiCodici(Audit_Tipo, Regolamento_Cod, 0, 0, Data)
                    AggiornaAuditDocumenti(Audit_Tipo, Regolamento_Cod, Audit_Cod, Piva, Riferimento_Cod, Data, codici, objParametri, Rintracciabilita, Workflow_Cod:=workflow)
                End If

            End If

            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            Errore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

        Finally

            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return Audit_Cod

    End Function


    Public Function multiSelct(campi As List(Of AuditFormModel)) As List(Of AuditFormModel)
        Dim listCampi As List(Of AuditFormModel) = New List(Of AuditFormModel)
        Dim value As String = ""

        'Dim listaCampiDist = (From c In campi Select New With {c.name}).Distinct().ToList()
        'Dim listaCampiDist = campi.[Select](Function(o) o.name).Distinct().ToList()


        Dim listaCampiDist = (From item In campi Select New With {Key .name = item.name}).ToList().Distinct()

        For Each campo In listaCampiDist
            value = ""
            For Each campoBis In campi
                If campo.name = campoBis.name Then
                    If value <> "" Then
                        value += "|"
                    End If

                    value += campoBis.value
                End If
            Next

            listCampi.Add(New AuditFormModel With {.name = campo.name, .value = value})

        Next
        Return listCampi
    End Function

    Public Function ScriviAudit(Audit_Tipo As Integer, Regolamento_Cod As Integer, Audit_Cod As Integer, Piva As String,
                                ByVal Data As Date, ByVal Stato As Integer, ByVal Note As String, ByVal Campionato As Integer,
                                ByVal Risposte As String, ByVal Calcola As String,
                                ByVal objParametri As AgronicaCoreParametri, Optional ByVal Rintracciabilita As Integer = 0) As Boolean

        Dim azione As String = "UPD"
        Dim errore As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim auditLeggi As New Audit_R
        Dim auditScrivi As New Audit_W
        Dim auditRisposteLeggi As New Audit_Risposte_R
        Dim auditRisposteScrivi As New Audit_Risposte_W

        Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        If Audit_Cod = 0 Then
            'Audit_Cod = AgroSequenze.Agronica_SequenzaTabelle_NuovoID("Audit", objParametri)
            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            Audit_Cod = AgroSequenze.NuovoId_Tabella("Audit", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)
            azione = "INS"
        End If

        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri)

            If azione = "UPD" Then
                auditRisposteScrivi.Cancellazione(Audit_Tipo, Regolamento_Cod, Audit_Cod, objParametri.PivaSuperUser, 0, "", objParametri)
                auditScrivi.Cancellazione(Audit_Cod, objParametri.PivaSuperUser, Piva, objParametri)
            End If

            If auditScrivi.scrivi(Audit_Cod, Audit_Tipo, 0, Stato, Regolamento_Cod, Piva, Note, Campionato, Data, Data, Date.Now, Date.Now, objParametri.UsernameOperazione, objParametri.UsernameOperazione, objParametri, Rintracciabilita) Then

                Dim disposizioni As New List(Of Integer)
                Dim campi As List(Of AuditFormModel) = JsonConvert.DeserializeObject(Of List(Of AuditFormModel))(Risposte)
                Dim proposte As New Dictionary(Of String, String)
                Dim risposte2 As New Dictionary(Of String, String)

                For Each campo In campi
                    Dim Codice As String = campo.name
                    Dim Valore As String = campo.value
                    Dim Codici() As String = Split(Codice, "_")
                    If Codici(0) = "proposta" AndAlso Valore <> "" Then
                        proposte.Add(Codici(1) & "_" & Codici(2), Valore)
                    End If
                    If Codici(0) = "risposta2" AndAlso Valore <> "" Then
                        risposte2.Add(Codici(1) & "_" & Codici(2), Valore)
                    End If
                Next

                For Each campo In campi
                    Dim Codice As String = campo.name
                    Dim Valore As String = campo.value
                    Dim Codici() As String = Split(Codice, "_")
                    If Codici(0) = "risposta" AndAlso Valore <> "-1" Then
                        Dim Disp_Cod As Integer = CInt(Codici(1))
                        Dim Punto_Numero As String = CStr(Codici(2))
                        Dim PropostaCorrettiva As String = ""
                        Dim Valore2 As String = ""
                        Dim Chiave = Codici(1) & "_" & Codici(2)
                        If proposte.ContainsKey(Chiave) Then
                            PropostaCorrettiva = proposte(Chiave)
                        End If
                        If risposte2.ContainsKey(Chiave) Then
                            Valore2 = risposte2(Chiave)
                        End If
                        auditRisposteScrivi.ScriviRisposte(Audit_Cod, Audit_Tipo, Punto_Numero, Regolamento_Cod, Disp_Cod, PropostaCorrettiva, Valore, Valore2, Data, Data, DateTime.Now, DateTime.Now, objParametri.UsernameOperazione, objParametri.UsernameOperazione, objParametri)
                        If Not disposizioni.Contains(Disp_Cod) Then
                            disposizioni.Add(Disp_Cod)
                        End If
                    End If
                Next

                'Condizionalità: calcolo automatico livelli di portata, gravità e durata
                If Calcola = "1" AndAlso Audit_Tipo = 1 AndAlso Regolamento_Cod = 10 Then
                    Dim auditAgronica As New AuditAgronicaWS(objParametri)
                    Dim auditCalcolo As New AuditCalcoloLivelli(0, Risposte, objParametri)
                    Dim codici As List(Of AuditCodiciModel) = auditAgronica.LeggiCodici(Audit_Tipo, Regolamento_Cod, 0, 0, Data)
                    For Each disposizione In disposizioni
                        Dim punteggi As List(Of AuditFormModel) = auditCalcolo.CalcolaPunteggi(Audit_Tipo, Regolamento_Cod, disposizione, Piva, Data, codici, objParametri)
                        For Each punteggio In punteggi
                            Dim Punto_Numero As String = punteggio.name
                            Dim Valore As String = punteggio.value
                            auditRisposteScrivi.Cancellazione(Audit_Tipo, Regolamento_Cod, Audit_Cod, objParametri.PivaSuperUser, disposizione, Punto_Numero, objParametri)
                            If Valore = "1" Then
                                auditRisposteScrivi.ScriviRisposte(Audit_Cod, Audit_Tipo, Punto_Numero, Regolamento_Cod, disposizione, "", Valore, "", Data, Data, DateTime.Now, DateTime.Now, objParametri.UsernameOperazione, objParametri.UsernameOperazione, objParametri)
                            End If
                        Next
                    Next
                End If

            End If

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            Dim messaggioErrore As String = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then
                Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio += "" & vbCrLf
                Messaggio += messaggioErrore
                Messaggio += "" & vbCrLf
                Messaggio += "Ritentare il salvataggio dopo la correzione ..."
            End If

            'Dim dp As New AgronicaCoreDataProvider.DataProvider
            'dp.Scrivi_LOG(objParametri, "AgronicaCoreAuditBiz.AuditController.Scrivi", messaggioErrore)

            errore = True

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return Not errore

    End Function

    Public Function CancellaAudit(Audit_Tipo As Integer, Regolamento_Cod As Integer, Audit_Cod As Integer, Piva As String, ByVal objParametri As AgronicaCoreParametri) As Boolean

        Dim errore As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim auditLeggi As New Audit_R
        Dim auditScrivi As New Audit_W
        Dim auditRisposteScrivi As New Audit_Risposte_W

        Dim dt As New DataTable
        Dim messaggioErrore As String = ""

        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            dt = auditLeggi.LeggiAudit(Audit_Cod, Audit_Tipo, Regolamento_Cod, Piva, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri)

            auditRisposteScrivi.Cancellazione(Audit_Tipo, Regolamento_Cod, Audit_Cod, objParametri.PivaSuperUser, 0, "", objParametri)
            auditScrivi.Cancellazione(Audit_Cod, objParametri.PivaSuperUser, Piva, objParametri)

            If dt.Rows.Count > 0 Then
                If dt.Rows(0).Item("Pratica_Cod") > 0 Then
                    Dim objpratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_W
                    objpratiche.Elimina_Pratica(dt.Rows(0).Item("Pratica_Cod"), objParametri, messaggioErrore)
                End If
            End If

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            errore = True

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return Not errore

    End Function

    Public Function LeggiCodiciDefault(Audit_Tipo As Integer, Regolamento_Cod As Integer, Disp_Cod As Integer, ByVal Sezione_Cod As Integer, ByVal Data As String, ByVal Risposte As String, ByVal objParametri As AgronicaCoreParametri) As List(Of AuditFormModel)

        Dim inizio As Date = AGRODATAINIZIO
        Dim fine As Date = AGRODATAFINE
        If Data <> "" Then
            inizio = CDate(Data)
            fine = inizio
        End If

        ' codici check list
        Dim auditAgronica As New AuditAgronicaWS(objParametri)
        Dim codici As List(Of AuditCodiciModel) = auditAgronica.LeggiCodiciDefault(Audit_Tipo, Regolamento_Cod, Disp_Cod, Sezione_Cod, Data)
        Dim campi As List(Of AuditFormModel) = JsonConvert.DeserializeObject(Of List(Of AuditFormModel))(Risposte)
        Dim rval As New List(Of AuditFormModel)

        For Each codice In codici

            'imposta default sul campo collegato
            For Each campo In campi
                Dim name As String = campo.name
                Dim value As String = campo.value
                Dim names() As String = Split(name, "_")
                If names(0) = "risposta" Then
                    Dim disposizione As Integer = CInt(names(1))
                    Dim punto As String = CStr(names(2))
                    If disposizione = codice.Disp_Cod AndAlso punto = codice.Punto_Numero_Default AndAlso value = "0" Then
                        Dim item As New AuditFormModel With {
                            .name = codice.Punto_Numero,
                            .value = "1"
                        }
                        rval.Add(item)
                    End If
                End If
            Next

        Next

        Return rval

    End Function

    Public Function LeggiCodiciDeroga(Audit_Tipo As Integer, Regolamento_Cod As Integer, Disp_Cod As Integer, ByVal Sezione_Cod As Integer, ByVal Data As String, ByVal Risposte As String, ByVal objParametri As AgronicaCoreParametri) As List(Of AuditFormModel)

        Dim inizio As Date = AGRODATAINIZIO
        Dim fine As Date = AGRODATAFINE
        If Data <> "" Then
            inizio = CDate(Data)
            fine = inizio
        End If

        ' codici check list
        Dim auditAgronica As New AuditAgronicaWS(objParametri)
        Dim codici As List(Of AuditCodiciModel) = auditAgronica.LeggiCodiciDeroga(Audit_Tipo, Regolamento_Cod, Disp_Cod, Sezione_Cod, Data)
        Dim campi As List(Of AuditFormModel) = JsonConvert.DeserializeObject(Of List(Of AuditFormModel))(Risposte)
        Dim deroghe As New List(Of AuditFormModel)

        For Each codice In codici

            'controllo sui campi deroga: se campo deroga si allora non va su parte 2 anche se campo derogato è no
            For Each campo In campi
                Dim name As String = campo.name
                Dim value As String = campo.value
                Dim names() As String = Split(name, "_")
                If names(0) = "risposta" Then
                    Dim disposizione As Integer = CInt(names(1))
                    Dim punto As String = CStr(names(2))
                    If disposizione = codice.Disp_Cod AndAlso punto = codice.Punto_Numero_Deroga AndAlso value = "0" Then
                        Dim item As New AuditFormModel With {
                            .name = codice.Punto_Numero,
                            .value = codice.Punto_Numero_Deroga
                        }
                        deroghe.Add(item)
                    End If
                End If
            Next

        Next

        Return deroghe

    End Function

    Public Function LeggiCodici(Audit_Tipo As Integer, Regolamento_Cod As Integer, Audit_Cod As Integer, Disp_Cod As Integer, ByVal Sezione_Cod As Integer, ByVal Data As String, ByVal objParametri As AgronicaCoreParametri) As List(Of AuditRisposteModel)

        Dim inizio As Date = AGRODATAINIZIO
        Dim fine As Date = AGRODATAFINE
        If Data <> "" Then
            inizio = CDate(Data)
            fine = inizio
        End If

        ' risposte check list
        Dim rval As New List(Of AuditRisposteModel)
        Dim auditLeggi As New Audit_Risposte_R
        Dim dt As DataTable = Nothing
        If Audit_Cod <> 0 Then
            auditLeggi.Leggi(Audit_Cod, Audit_Tipo, Regolamento_Cod, Disp_Cod, inizio, fine, "", "", objParametri)
        End If

        ' codici check list
        Dim auditAgronica As New AuditAgronicaWS(objParametri)
        Dim codici As List(Of AuditCodiciModel) = auditAgronica.LeggiCodici(Audit_Tipo, Regolamento_Cod, Disp_Cod, Sezione_Cod, Data)

        For Each codice In codici

            Dim Valore As String = ""
            Dim Valore_2 As String = ""
            Dim ValorePropostaCorrettiva As String = ""

            If dt IsNot Nothing Then
                For i = 0 To dt.Rows.Count - 1
                    If dt.Rows(i).Item("Disp_Cod") = codice.Disp_Cod AndAlso dt.Rows(i).Item("Punto_Numero") = codice.Punto_Numero Then
                        Valore = IIf(IsDBNull(dt.Rows(i).Item("Valore")), "", dt.Rows(i).Item("Valore"))
                        Valore_2 = IIf(IsDBNull(dt.Rows(i).Item("Valore_2")), "", dt.Rows(i).Item("Valore_2"))
                        ValorePropostaCorrettiva = IIf(IsDBNull(dt.Rows(i).Item("PropostaCorrettiva")), "", dt.Rows(i).Item("PropostaCorrettiva"))
                        Exit For
                    End If
                Next
            End If

            Dim item As New AuditRisposteModel With {
                    .Audit_Tipo = codice.Audit_Tipo,
                    .Regolamento_Cod = codice.Regolamento_Cod,
                    .Disp_Cod = codice.Disp_Cod,
                    .Sezione_Cod = codice.Sezione_Cod,
                    .Sezione_Des = codice.Sezione_Des,
                    .Parte = codice.Parte,
                    .Punto_Numero = codice.Punto_Numero,
                    .Descrizione = codice.Descrizione,
                    .Allegato = codice.Allegato,
                    .Nota = codice.Nota,
                    .Tipo = codice.Tipo,
                    .Punteggio = codice.Punteggio,
                    .Valore = Valore,
                    .Valore_2 = Valore_2,
                    .PropostaCorrettiva = codice.PropostaCorrettiva,
                    .ValorePropostaCorrettiva = ValorePropostaCorrettiva
                }

            rval.Add(item)

        Next

        Return rval

    End Function

    Public Function LeggiRisposteAudit(Audit_Tipo As Integer, Regolamento_Cod As Integer, Audit_Cod As Integer, Disp_Cod As Integer, ByVal Parte As Integer, ByVal Data As String, ByVal objParametri As AgronicaCoreParametri) As List(Of AuditRisposteModel)

        ' ricavo data inizio e fine
        Dim Data_Inizio As Date = AGRODATAINIZIO
        Dim Data_Fine As Date = AGRODATAFINE
        If Data <> "" Then
            Data_Inizio = CDate(Data)
            Data_Fine = Data_Inizio
        End If

        ' risposte check list
        Dim rval As New List(Of AuditRisposteModel)
        Dim auditLeggi As New Audit_Risposte_R
        Dim dt As DataTable = Nothing
        If Audit_Cod <> 0 Then
            dt = auditLeggi.Leggi(Audit_Cod, Audit_Tipo, Regolamento_Cod, Disp_Cod, Data_Inizio, Data_Fine, "", "", objParametri)
        End If

        Dim auditAgronica As New AuditAgronicaWS(objParametri)

        ' sezioni disposizione
        Dim sezioni As List(Of AuditSezioniModel) = auditAgronica.LeggiSezioni(Audit_Tipo, Regolamento_Cod, Disp_Cod, Parte, Data)

        For Each sezione In sezioni

            ' codici sezione
            Dim codici As List(Of AuditCodiciModel) = auditAgronica.LeggiCodici(Audit_Tipo, Regolamento_Cod, Disp_Cod, sezione.Sezione_Cod, Data)

            For Each codice In codici

                Dim Valore As String = ""
                Dim Valore_2 As String = ""
                Dim ValorePropostaCorrettiva As String = ""

                ' loop su risposte
                If dt IsNot Nothing Then
                    For i = 0 To dt.Rows.Count - 1
                        If dt.Rows(i).Item("Disp_Cod") = codice.Disp_Cod AndAlso dt.Rows(i).Item("Punto_Numero") = codice.Punto_Numero Then
                            Valore = IIf(IsDBNull(dt.Rows(i).Item("Valore")), "", dt.Rows(i).Item("Valore"))
                            Valore_2 = IIf(IsDBNull(dt.Rows(i).Item("Valore_2")), "", dt.Rows(i).Item("Valore_2"))
                            ValorePropostaCorrettiva = IIf(IsDBNull(dt.Rows(i).Item("PropostaCorrettiva")), "", dt.Rows(i).Item("PropostaCorrettiva"))
                            Exit For
                        End If
                    Next
                End If

                Dim item As New AuditRisposteModel With {
                        .Audit_Tipo = codice.Audit_Tipo,
                        .Regolamento_Cod = codice.Regolamento_Cod,
                        .Disp_Cod = codice.Disp_Cod,
                        .Sezione_Cod = codice.Sezione_Cod,
                        .Sezione_Des = codice.Sezione_Des,
                        .FunCalcoloLivello = sezione.FunCalcoloLivello,
                        .Parte = codice.Parte,
                        .Punto_Numero = codice.Punto_Numero,
                        .Descrizione = codice.Descrizione,
                        .Allegato = codice.Allegato,
                        .Nota = codice.Nota,
                        .Tipo = codice.Tipo,
                        .Punteggio = codice.Punteggio,
                        .Valore = Valore,
                        .Valore_2 = Valore_2,
                        .PropostaCorrettiva = codice.PropostaCorrettiva,
                        .ValorePropostaCorrettiva = ValorePropostaCorrettiva
                    }

                rval.Add(item)

            Next

        Next

        Return rval

    End Function

    Public Function LeggiAuditRisposte(Audit_Tipo As Integer, Regolamento_Cod As Integer, Audit_Cod As Integer, Disp_Cod As Integer, ByVal Parte As Integer, ByVal Data As String, ByRef objParametri As AgronicaCoreParametri, Optional ByVal Piva As String = "", Optional ByVal Riferimento_Cod As String = "") As List(Of AuditRisposteModel)

        ' ricavo data inizio e fine
        Dim Data_Inizio As Date = AGRODATAINIZIO
        Dim Data_Fine As Date = AGRODATAFINE
        If Data <> "" Then
            Data_Inizio = CDate(Data)
            Data_Fine = Data_Inizio
        End If

        ' risposte check list
        Dim rval As New List(Of AuditRisposteModel)
        Dim auditLeggi As New Audit_Risposte_R
        Dim dt As DataTable = Nothing
        If Audit_Cod <> 0 Then
            dt = auditLeggi.Leggi(Audit_Cod, Audit_Tipo, Regolamento_Cod, Disp_Cod, Data_Inizio, Data_Fine, "", "", objParametri)
        End If

        Dim auditAgronica As New AuditAgronicaWS(objParametri)
        Dim codici As List(Of AuditCodiciModel) = auditAgronica.LeggiCodiciDisposizioni(Audit_Tipo, Regolamento_Cod, Disp_Cod, Parte, Data)

        For Each codice In codici

            Dim Valore As String = ""
            Dim Valore_2 As String = ""
            Dim ValorePropostaCorrettiva As String = ""

            ' loop su risposte
            If dt IsNot Nothing Then
                For i = 0 To dt.Rows.Count - 1
                    If dt.Rows(i).Item("Disp_Cod") = codice.Disp_Cod AndAlso dt.Rows(i).Item("Punto_Numero") = codice.Punto_Numero Then
                        Valore = If(IsDBNull(dt.Rows(i).Item("Valore")), "", dt.Rows(i).Item("Valore"))
                        Valore_2 = If(IsDBNull(dt.Rows(i).Item("Valore_2")), "", dt.Rows(i).Item("Valore_2"))
                        ValorePropostaCorrettiva = If(IsDBNull(dt.Rows(i).Item("PropostaCorrettiva")), "", dt.Rows(i).Item("PropostaCorrettiva"))
                        Exit For
                    End If
                Next
            End If

            ' requisito di tipo documento
            If Audit_Tipo = 12 OrElse Audit_Tipo = 13 OrElse Audit_Tipo = 15 OrElse Audit_Tipo = 16 OrElse Audit_Tipo = 17 OrElse Audit_Tipo = 20 OrElse Audit_Tipo = 22 Then
                If Piva <> "" AndAlso Left(codice.Tipo, 3) = "doc" AndAlso Len(codice.Tipo) > 3 Then

                    Dim Valutazione As String = ""
                    Dim TipoDocumento_Cod As String = Replace(codice.Tipo, "doc", "")
                    Valore_2 = LeggiAuditDocumenti(Audit_Tipo, Piva, Riferimento_Cod, TipoDocumento_Cod, Data_Inizio, Valutazione, objParametri)

                    ' forzatura iniziale stato documenti
                    If Valore = "" Then
                        If Valutazione = "VALIDATO" Then
                            Valore = "1"
                        ElseIf Valutazione = "" AndAlso Audit_Tipo = 13 Then
                            ' forzo stato a SI se non ci sono documenti
                            If codice.Punto_Numero = "090" OrElse codice.Punto_Numero = "110" Then
                                Valore = "1"
                            End If
                        End If
                    End If

                End If
            ElseIf Audit_Tipo = 21 Then
                If Piva <> "" AndAlso Left(codice.Tipo, 2) = "ue" AndAlso Len(codice.Tipo) > 2 Then
                    Dim Valutazione As String = ""
                    Dim TipoDocumento_Cod As String = Replace(codice.Tipo, "ue", "")
                    Valore_2 = LeggiAuditDocumenti(Audit_Tipo, Piva, Riferimento_Cod, TipoDocumento_Cod, Data_Inizio, Valutazione, objParametri)
                End If
            End If

            Dim item As New AuditRisposteModel With {
                    .Audit_Tipo = codice.Audit_Tipo,
                    .Regolamento_Cod = codice.Regolamento_Cod,
                    .Disp_Cod = codice.Disp_Cod,
                    .Sezione_Cod = codice.Sezione_Cod,
                    .Sezione_Des = codice.Sezione_Des,
                    .FunCalcoloLivello = codice.FunCalcoloLivello,
                    .Parte = codice.Parte,
                    .Punto_Numero = codice.Punto_Numero,
                    .Descrizione = codice.Descrizione,
                    .Allegato = codice.Allegato,
                    .Nota = codice.Nota,
                    .Tipo = codice.Tipo,
                    .Punteggio = codice.Punteggio,
                    .Valore = Valore,
                    .Valore_2 = Valore_2,
                    .Criterio = TraduciCriterio(codice.Criterio),
                    .PropostaCorrettiva = codice.PropostaCorrettiva,
                    .ValorePropostaCorrettiva = ValorePropostaCorrettiva
                }

            rval.Add(item)

        Next

        Return rval

    End Function

    Private Function TraduciCriterio(Criterio As String) As String
        Dim rval As String

        If HttpContext.Current.Session("Lingua_Audit") = 1 AndAlso Criterio <> "" Then
            Select Case Criterio
                Case "No=0|Sì=2|Non Applicabile=3"
                    rval = "No=0|Yes=2|Not Applicable=3"
                Case "No=0|Sì=2"
                    rval = "No=0|Yes=2"
                Case "No=2|Sì=0"
                    rval = "No=2|Yes=0"
                Case Else
                    rval = Criterio
            End Select
        Else
            rval = Criterio
        End If
        Return rval
    End Function

    Public Function CompletamentoChecklist(ByVal objParametri_Utenti As AgronicaCoreParametri, ByVal objParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.Audit.AuditCompletamentoModel)

        Try
            Dim utenteUsername As String = If(objParametri_Utenti.UtenteUsername, "")

            Dim checklistVisibili As New List(Of Integer)
            Dim permessiUtente As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim permessiChecklists = permessiUtente.Controlla_Permessi_Utente_Tutti(utenteUsername,
                                                                                    5, 0,
                                                                                    AgronicaCoreDataProvider.TipiEnumerativi.enum_Security_Operazione.Lettura,
                                                                                    Date.Now, "", objParametri_Utenti)

            Dim listaIdAttivita As New List(Of Integer)()

            Dim IntList As New List(Of Int32)
            For Each DtRow As DataRow In permessiChecklists.Rows
                If DtRow("Id_Attivita") IsNot Nothing Then
                    listaIdAttivita.Add(CInt(DtRow("Id_Attivita")))
                End If
            Next

            If listaIdAttivita.Contains(CInt(enum_Security_Attivita.Audit_BIO_FILENI)) Then
                checklistVisibili.Add(20)
            End If

            If listaIdAttivita.Contains(CInt(enum_Security_Attivita.Filiera_Trasporti_COPROB)) Then
                checklistVisibili.Add(17)
            End If

            If listaIdAttivita.Contains(CInt(enum_Security_Attivita.Audit_SQNPI_COPROB)) Then
                checklistVisibili.Add(13)
            End If

            If listaIdAttivita.Contains(CInt(enum_Security_Attivita.Audit_BIO_COPROB)) Then
                checklistVisibili.Add(12)
            End If

            If listaIdAttivita.Contains(CInt(enum_Security_Attivita.Audit_Fornitori_EUDR)) Then
                checklistVisibili.Add(21)
            End If

            If listaIdAttivita.Contains(CInt(enum_Security_Attivita.Banca_Cambiano_Azienda)) Then
                checklistVisibili.Add(19)
            End If

            If listaIdAttivita.Contains(CInt(enum_Security_Attivita.Enquete_certification_Hevea_brasiliensis)) Then
                checklistVisibili.Add(18)
            End If

            If listaIdAttivita.Contains(CInt(enum_Security_Attivita.Controllo_DPI_DeMatteis)) Then
                checklistVisibili.Add(22)
            End If

            If listaIdAttivita.Contains(CInt(enum_Security_Attivita.Audit_Biologico_Greenyard)) Then
                checklistVisibili.Add(16)
            End If

            If listaIdAttivita.Contains(CInt(enum_Security_Attivita.Audit_Convenzionale_Greenyard)) Then
                checklistVisibili.Add(15)
            End If

            If listaIdAttivita.Contains(CInt(enum_Security_Attivita.Audit_Budwood_Projects)) Then
                checklistVisibili.Add(14)
            End If

            If listaIdAttivita.Contains(CInt(enum_Security_Attivita.CheckList_Pratiche_Ecologiche_APOT)) Then
                checklistVisibili.Add(8)
            End If

            If listaIdAttivita.Contains(CInt(enum_Security_Attivita.CheckList_Sicurezza_Lavoro)) Then
                checklistVisibili.Add(3)
            End If

            If listaIdAttivita.Contains(CInt(enum_Security_Attivita.CheckList_Formazione)) Then
                checklistVisibili.Add(9)
            End If

            Dim FiltroImpreseVisibili As String = ""
            Dim UtentiVisibilitaLeggi As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim UtentiProfiliVisibilita As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
            Dim ImpreseVisibili As DataTable = UtentiVisibilitaLeggi.Leggi(1, "", "", objParametri_Server)
            Dim ProfiliVisibilita As DataTable = UtentiProfiliVisibilita.Leggi_2(utenteUsername, 5, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Utenti)
            Dim FiltroImprese As String = ""
            If ImpreseVisibili IsNot Nothing AndAlso ImpreseVisibili.Rows.Count > 0 Then
                For i = 0 To ImpreseVisibili.Rows.Count - 1
                    FiltroImpreseVisibili &= "'" & ImpreseVisibili.Rows(i).Item("Piva") & "',"
                Next
                If FiltroImpreseVisibili <> "" Then
                    FiltroImpreseVisibili = Left(FiltroImpreseVisibili, FiltroImpreseVisibili.Length - 1)
                End If
            End If

            If ProfiliVisibilita.Rows.Count > 0 Then
                If ProfiliVisibilita.Rows(0).Item("Descrizione_2").ToString = "" Then
                    FiltroImpreseVisibili = ""
                ElseIf ProfiliVisibilita.Rows(0).Item("Descrizione_2").ToString.Contains("#") Then
                    FiltroImpreseVisibili = "'nessunaVisibilita'"
                End If
            End If

            Dim listaCompletamento As New List(Of AgronicaCoreModelsSTD.Audit.AuditCompletamentoModel)
            Dim objAudit_R As New AgronicaCoreAuditDAL.Audit_R
            Dim dt = objAudit_R.Leggi_Completamento_Checklists(FiltroImpreseVisibili, checklistVisibili, "", "", objParametri_Server)

            For Each row As DataRow In dt.Rows
                Dim item As New AgronicaCoreModelsSTD.Audit.AuditCompletamentoModel With {
                                .Audit_Tipo = row.Item("Audit_Tipo"),
                                .Audit_Nome = AssociaNomeChecklist(row.Item("Audit_Tipo")), 'row.Item("Audit_Nome"),
                                .Non_Completate = row.Item("Non_Completate"),
                                .Coltura = If(IsDBNull(row.Item("Coltura")), "", row.Item("Coltura")),
                                .Completate = row.Item("Completate")
                            }
                listaCompletamento.Add(item)
            Next

            Return listaCompletamento

        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function AssociaNomeChecklist(ByVal Audit_Tipo As Integer) As String
        Select Case Audit_Tipo
            Case 3
                Return "Checklist Sicurezza Lavoro"
            Case 8
                Return "Checklist Pratiche ecologiche APOT"
            Case 9
                Return "Checklist Formazione"
            Case 12
                Return "Checklist BIO COPROB"
            Case 13
                Return "Checklist SQNPI COPROB"
            Case 14
                Return "Budwood Projects Audit"
            Case 15
                Return "Controllo Fornitori Convenzionale"
            Case 16
                Return "Controllo Fornitori Biologico"
            Case 17
                Return "Checklist Trasporti"
            Case 18
                Return "Enquête de certification ISCC-EU"
            Case 19
                Return "Checklist Aziende Agricole"
            Case 20
                Return "Checklist BIO Fileni"
            Case 21
                Return "Questionario Fornitori EUDR"
            Case 22
                Return "Checklist Controllo del Disciplinare"
            Case Else
                Return "Checklist Sconosciuta"
        End Select
    End Function

    ' restituisce le disposizioni attive
    Public Function LeggiDisposizioniAttive(ByVal Audit_Tipo As Integer, ByVal Regolamento_Cod As Integer, ByVal Piva As String, ByVal Data As Date, ByVal objParametri As AgronicaCoreParametri, Optional ByVal Disposizione_Cod As Integer = 0) As List(Of AuditDisposizioniModel)
        Try

            Dim auditDisposizioni As New Audit_Disposizioni_R
            Dim auditAgronica As New AuditAgronicaWS(objParametri)
            Dim campi As List(Of AuditCampiModel) = auditAgronica.LeggiCampi(Audit_Tipo, Regolamento_Cod)
            Dim disposizioni As List(Of AuditDisposizioniModel) = auditAgronica.LeggiDisposizioni(Audit_Tipo, Regolamento_Cod, 0, Disposizione_Cod)
            Dim domande As List(Of AuditDomandeDisposizioniModel) = auditAgronica.LeggiDomandeDisposizioni(Audit_Tipo, Regolamento_Cod, Disposizione_Cod)
            Dim rval As New List(Of AuditDisposizioniModel)

            For Each campo In campi

                For Each disposizione In disposizioni

                    If disposizione.Campo = campo.Campo_Cod Then

                        ' se non ci sono domande collegate alla disposizione la considero sempre attiva
                        Dim disposizione_attiva As Boolean = True
                        Dim dtRisposte As DataTable = auditDisposizioni.LeggiRisposte(Audit_Tipo, Regolamento_Cod, Piva, disposizione.Disp_Cod, Data, objParametri)

                        For Each domanda In domande
                            If domanda.Disp_Cod = disposizione.Disp_Cod Then
                                disposizione_attiva = False

                                If Audit_Tipo = 17 Then
                                    disposizione_attiva = True
                                End If

                                For i = 0 To dtRisposte.Rows.Count - 1
                                    If (domanda.Domanda_Cod = dtRisposte.Rows(i).Item("Domanda_Cod")) Then
                                        If Not IsDBNull(dtRisposte.Rows(i).Item("Valore")) Then
                                            If dtRisposte.Rows(i).Item("Valore").ToString = "1" Then
                                                disposizione_attiva = True
                                                Exit For
                                            End If
                                        End If
                                    End If
                                Next
                                If disposizione_attiva Then
                                    Exit For
                                End If
                            End If
                        Next

                        If disposizione_attiva Then

                            Dim item As New AuditDisposizioniModel With {
                                .Audit_Tipo = disposizione.Audit_Tipo,
                                .Regolamento_Cod = disposizione.Regolamento_Cod,
                                .Disp_Cod = disposizione.Disp_Cod,
                                .Disp_Nome = disposizione.Disp_Nome,
                                .Descrizione = disposizione.Descrizione,
                                .Attivazione = disposizione.Attivazione,
                                .Campo = disposizione.Campo,
                                .Campo_Des = campo.Campo_Des,
                                .Ordine = disposizione.Ordine,
                                .Note = disposizione.Note
                            }

                            rval.Add(item)

                        End If

                    End If

                Next

            Next

            Return rval

        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Public Function LeggiPunteggi(ByVal Audit_Tipo As Integer,
                                ByVal Regolamento_Cod As Integer,
                                ByVal Audit_Cod As Integer,
                                ByVal Piva As String,
                                ByVal Data As Date,
                                ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of AuditPunteggiModel)

        Dim auditDisposizioni As New Audit_Disposizioni_R
        Dim disposizioni As List(Of AuditDisposizioniModel) = LeggiDisposizioniAttive(Audit_Tipo, Regolamento_Cod, Piva, Data, objParametri)
        Dim punteggi As New List(Of AuditPunteggiModel)

        Dim Portata As Integer
        Dim Gravita As Integer
        Dim Durata As Integer
        Dim Esito As Boolean
        Dim PunteggioPonderato As Double
        Dim PunteggioTotale As Double
        Dim RiduzioneComplessiva As Integer = 0

        Dim Intenzionalita As Integer = 0
        Dim Reiterazione As Integer = 0
        Dim Inadempienza As Integer = 0

        Dim k As Integer

        For Each disposizione In disposizioni

            Dim dtPunteggi As DataTable = auditDisposizioni.LeggiPunteggi(Audit_Tipo, Regolamento_Cod, Audit_Cod, disposizione.Disp_Cod, 0, objParametri)

            Portata = 0
            Gravita = 0
            Durata = 0
            PunteggioPonderato = 0

            For k = 0 To dtPunteggi.Rows.Count - 1

                If Not IsDBNull(dtPunteggi.Rows(k).Item("Valore")) AndAlso dtPunteggi.Rows(k).Item("Valore").ToString = "1" Then

                    Select Case CInt(dtPunteggi.Rows(k).Item("Sezione_Cod"))
                        Case TipiEnumerativi.enum_AuditSezioni.AuditSezioni_Portata
                            Portata = CInt(dtPunteggi.Rows(k).Item("Punteggio"))
                        Case TipiEnumerativi.enum_AuditSezioni.AuditSezioni_Gravita
                            Gravita = CInt(dtPunteggi.Rows(k).Item("Punteggio"))
                        Case TipiEnumerativi.enum_AuditSezioni.AuditSezioni_Durata
                            Durata = CInt(dtPunteggi.Rows(k).Item("Punteggio"))
                        Case TipiEnumerativi.enum_AuditSezioni.AuditSezioni_PortataGravitaDurata
                            Portata = CInt(dtPunteggi.Rows(k).Item("Punteggio"))
                            Gravita = CInt(dtPunteggi.Rows(k).Item("Punteggio"))
                            Durata = CInt(dtPunteggi.Rows(k).Item("Punteggio"))
                    End Select

                End If

            Next

            PunteggioPonderato = (Portata + Gravita + Durata) / 3
            PunteggioPonderato = Math.Round(PunteggioPonderato, 2)

            Inadempienza = auditDisposizioni.LeggiPunteggi(Audit_Tipo, Regolamento_Cod, Audit_Cod, disposizione.Disp_Cod, TipiEnumerativi.enum_AuditSezioni.AuditSezioni_InadempienzePortataMinore, objParametri).Rows.Count
            Intenzionalita = auditDisposizioni.LeggiPunteggi(Audit_Tipo, Regolamento_Cod, Audit_Cod, disposizione.Disp_Cod, TipiEnumerativi.enum_AuditSezioni.AuditSezioni_Intenzionalita, objParametri).Rows.Count
            Reiterazione = auditDisposizioni.LeggiPunteggi(Audit_Tipo, Regolamento_Cod, Audit_Cod, disposizione.Disp_Cod, TipiEnumerativi.enum_AuditSezioni.AuditSezioni_Reiterazione, objParametri).Rows.Count

            ' se portata, gravità e durata sono a 0 e se è un'inademnpienza di portata minore l'esito sarà positivo
            Esito = False
            If (Portata = 0 And Gravita = 0 And Durata = 0) Or (Inadempienza > 0) Then
                Esito = True
                PunteggioPonderato = 0
            End If

            PunteggioTotale += PunteggioPonderato

            Dim item As New AuditPunteggiModel With {
                    .Audit_Tipo = disposizione.Audit_Tipo,
                    .Regolamento_Cod = disposizione.Regolamento_Cod,
                    .Disp_Cod = disposizione.Disp_Cod,
                    .Disp_Nome = disposizione.Disp_Nome,
                    .Descrizione = disposizione.Descrizione,
                    .Campo = disposizione.Campo,
                    .Campo_Des = disposizione.Campo_Des,
                    .Portata = Portata,
                    .Gravita = Gravita,
                    .Durata = Durata,
                    .Esito = Esito,
                    .Inadempienza = Inadempienza > 0,
                    .Intenzionalita = Intenzionalita > 0,
                    .Reiterazione = Reiterazione > 0,
                    .PunteggioPonderato = PunteggioPonderato
                }

            punteggi.Add(item)

        Next

        Return punteggi

    End Function

    Public Function LeggiAuditPunteggi(ByVal Audit_Tipo As Integer,
                                ByVal Regolamento_Cod As Integer,
                                ByVal Audit_Cod As Integer,
                                ByVal Piva As String,
                                ByVal Data As Date,
                                ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of AuditPunteggiModel)

        Dim auditDisposizioni As New Audit_Disposizioni_R
        Dim disposizioni As List(Of AuditDisposizioniModel) = LeggiDisposizioniAttive(Audit_Tipo, Regolamento_Cod, Piva, Data, objParametri)
        Dim punteggi As New List(Of AuditPunteggiModel)

        Dim Portata As Integer
        Dim Gravita As Integer
        Dim Durata As Integer
        Dim Esito As Boolean
        Dim Verifica As Boolean
        Dim PunteggioPonderato As Double
        Dim PunteggioTotale As Double
        Dim RiduzioneComplessiva As Integer = 0

        Dim Intenzionalita As Integer = 0
        Dim Reiterazione As Integer = 0
        Dim Inadempienza As Integer = 0

        For Each disposizione In disposizioni

            Dim risposte As List(Of AuditRisposteModel) = LeggiAuditRisposte(Audit_Tipo, Regolamento_Cod, Audit_Cod, disposizione.Disp_Cod, 0, Data, objParametri)

            Portata = 0
            Gravita = 0
            Durata = 0
            PunteggioPonderato = 0
            Verifica = True
            Inadempienza = 0
            Intenzionalita = 0
            Reiterazione = 0

            For Each risposta In risposte

                If risposta.Valore = "1" Then

                    If risposta.Punteggio <> 0 Then
                        Select Case risposta.Sezione_Cod
                            Case TipiEnumerativi.enum_AuditSezioni.AuditSezioni_Portata
                                Portata = risposta.Punteggio
                            Case TipiEnumerativi.enum_AuditSezioni.AuditSezioni_Gravita
                                Gravita = risposta.Punteggio
                            Case TipiEnumerativi.enum_AuditSezioni.AuditSezioni_Durata
                                Durata = risposta.Punteggio
                            Case TipiEnumerativi.enum_AuditSezioni.AuditSezioni_PortataGravitaDurata
                                Portata = risposta.Punteggio
                                Gravita = risposta.Punteggio
                                Durata = risposta.Punteggio
                        End Select
                    Else
                        Select Case risposta.Sezione_Cod
                            Case TipiEnumerativi.enum_AuditSezioni.AuditSezioni_InadempienzePortataMinore
                                Inadempienza += 1
                            Case TipiEnumerativi.enum_AuditSezioni.AuditSezioni_Intenzionalita
                                Intenzionalita += 1
                            Case TipiEnumerativi.enum_AuditSezioni.AuditSezioni_Reiterazione
                                Reiterazione += 1
                        End Select
                    End If

                ElseIf risposta.Valore = "0" AndAlso risposta.Parte = "1" Then

                    Verifica = False

                End If

            Next

            PunteggioPonderato = (Portata + Gravita + Durata) / 3
            PunteggioPonderato = Math.Round(PunteggioPonderato, 2)

            ' se portata, gravità e durata sono a 0 e se è un'inademnpienza di portata minore l'esito sarà positivo
            Esito = False
            If (Portata = 0 And Gravita = 0 And Durata = 0) Or (Inadempienza > 0) Then
                Esito = True
                PunteggioPonderato = 0
            End If

            PunteggioTotale += PunteggioPonderato

            Dim item As New AuditPunteggiModel With {
                    .Audit_Tipo = disposizione.Audit_Tipo,
                    .Regolamento_Cod = disposizione.Regolamento_Cod,
                    .Disp_Cod = disposizione.Disp_Cod,
                    .Disp_Nome = disposizione.Disp_Nome,
                    .Descrizione = disposizione.Descrizione,
                    .Campo = disposizione.Campo,
                    .Campo_Des = disposizione.Campo_Des,
                    .Portata = Portata,
                    .Gravita = Gravita,
                    .Durata = Durata,
                    .Esito = Esito,
                    .Verifica = Verifica,
                    .Inadempienza = Inadempienza > 0,
                    .Intenzionalita = Intenzionalita > 0,
                    .Reiterazione = Reiterazione > 0,
                    .PunteggioPonderato = PunteggioPonderato
                }

            punteggi.Add(item)

        Next

        Return punteggi

    End Function

    Public Function CalcolaLivello(ByVal Audit_Tipo As Integer,
                                    ByVal Regolamento_Cod As Integer,
                                    ByVal Piva As String,
                                    ByVal Disposizione_Cod As Integer,
                                    ByVal Sezione_Cod As Integer,
                                    ByVal Livello As String,
                                    ByVal Risposte As String,
                                    ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim vetParametri(2) As String
        vetParametri(0) = objParametri.PivaSuperUser
        vetParametri(1) = Piva
        vetParametri(2) = Regolamento_Cod

        Dim bRet As String = ""
        Dim intPunteggio As Integer = 0

        Try

            If Regolamento_Cod = 10 Then
                Dim auditCalcolo As New AuditCalcoloLivelli(Disposizione_Cod, Risposte, objParametri)
                intPunteggio = CallByName(auditCalcolo, Livello, CallType.Method, vetParametri)
            Else
                Dim auditAutomazioni As New AuditAutomazioni(Disposizione_Cod, Risposte, objParametri)
                intPunteggio = CallByName(auditAutomazioni, Livello, CallType.Method, vetParametri)
            End If

        Catch ex As Exception

        End Try

        ' restituisce la lista campi punteggi da aggiornare
        Dim auditAgronica As New AuditAgronicaWS(objParametri)
        Dim codici As List(Of AuditCodiciModel) = auditAgronica.LeggiCodici(Audit_Tipo, Regolamento_Cod, Disposizione_Cod, Sezione_Cod, "")
        For Each codice In codici
            If codice.Punteggio > 0 Then
                If bRet <> "" Then
                    bRet &= ","
                End If
                If codice.Punteggio = intPunteggio Then
                    bRet &= codice.Punto_Numero & "=1"
                Else
                    bRet &= codice.Punto_Numero & "=0"
                End If
            End If
        Next

        Return bRet

    End Function

    Public Function CalcolaRiduzione(ByVal Audit_Tipo As Integer,
                                ByVal Regolamento_Cod As Integer,
                                ByVal Audit_Cod As Integer,
                                ByVal Piva As String,
                                ByVal Data As Date,
                                ByVal objParametri As AgronicaCoreParametri) As String

        Dim auditDisposizioni As New Audit_Disposizioni_R
        Dim disposizioni As List(Of AuditDisposizioniModel) = LeggiDisposizioniAttive(Audit_Tipo, Regolamento_Cod, Piva, Data, objParametri)
        Dim punteggi As New List(Of AuditPunteggiModel)

        Dim Maggiori_TOT As Integer = 0
        Dim Maggiori_SI As Integer = 0
        Dim Minori_TOT As Integer = 0
        Dim Minori_SI As Integer = 0
        Dim Raccom_TOT As Integer = 0
        Dim Raccom_SI As Integer = 0

        Dim TOT_Maggiori_TOT As Integer = 0
        Dim TOT_Maggiori_SI As Integer = 0
        Dim TOT_Minori_TOT As Integer = 0
        Dim TOT_Minori_SI As Integer = 0
        Dim TOT_Raccom_TOT As Integer = 0
        Dim TOT_Raccom_SI As Integer = 0

        Dim table As New StringBuilder
        table.Append("<table class='table table-hover table-bordered'>")
        table.Append("<thead><tr class='warning'>")
        table.Append("<th>Modulo</th><th>Maggiori</th><th>Minori</th><th>Raccomandazioni</th>")
        table.Append("</tr></thead><tbody>")

        For Each disposizione In disposizioni

            Dim risposte As List(Of AuditRisposteModel) = LeggiAuditRisposte(Audit_Tipo, Regolamento_Cod, Audit_Cod, disposizione.Disp_Cod, 0, Data, objParametri)

            Maggiori_TOT = 0
            Maggiori_SI = 0
            Minori_TOT = 0
            Minori_SI = 0
            Raccom_TOT = 0
            Raccom_SI = 0

            For Each risposta In risposte

                If risposta.Punteggio = 1 Then
                    If risposta.Valore = "1" Then
                        Raccom_TOT += 1
                        Raccom_SI += 1
                    ElseIf risposta.Valore = "0" Then
                        Raccom_TOT += 1
                    End If
                ElseIf risposta.Punteggio = 2 Then
                    If risposta.Valore = "1" Then
                        Minori_TOT += 1
                        Minori_SI += 1
                    ElseIf risposta.Valore = "0" Then
                        Minori_TOT += 1
                    End If
                ElseIf risposta.Punteggio = 3 Then
                    If risposta.Valore = "1" Then
                        Maggiori_TOT += 1
                        Maggiori_SI += 1
                    ElseIf risposta.Valore = "0" Then
                        Maggiori_TOT += 1
                    End If
                End If

            Next

            table.Append("<tr><td>" & disposizione.Disp_Nome & "</td>")
            table.Append("<td>" & Maggiori_SI & "/" & Maggiori_TOT & "</td>")
            table.Append("<td>" & Minori_SI & "/" & Minori_TOT & "</td>")
            table.Append("<td>" & Raccom_SI & "/" & Raccom_TOT & "</td></tr>")

            TOT_Maggiori_TOT += Maggiori_TOT
            TOT_Maggiori_SI += Maggiori_SI
            TOT_Minori_TOT += Minori_TOT
            TOT_Minori_SI += Minori_SI
            TOT_Raccom_TOT += Raccom_TOT
            TOT_Raccom_SI += Raccom_SI

        Next

        table.Append("<tr><td rowspan='2'>TOTALE</td>")
        table.Append("<td>" & TOT_Maggiori_SI & "/" & TOT_Maggiori_TOT & "</td>")
        table.Append("<td>" & TOT_Minori_SI & "/" & TOT_Minori_TOT & "</td>")
        table.Append("<td>" & TOT_Raccom_SI & "/" & TOT_Raccom_TOT & "</td></tr>")

        Dim percMaggiori As Double = 0
        Dim percMinori As Double = 0
        Dim percRaccom As Double = 0

        If TOT_Maggiori_TOT <> 0 Then
            percMaggiori = TOT_Maggiori_SI * 100 / TOT_Maggiori_TOT
        End If

        If TOT_Minori_TOT <> 0 Then
            percMinori = TOT_Minori_SI * 100 / TOT_Minori_TOT
        End If

        If TOT_Raccom_TOT <> 0 Then
            percRaccom = TOT_Raccom_SI * 100 / TOT_Raccom_TOT
        End If

        table.Append("<tr>")
        table.Append("<td>" & Format(percMaggiori, "0.00") & "%</td>")
        table.Append("<td>" & Format(percMinori, "0.00") & "%</td>")
        table.Append("<td>" & Format(percRaccom, "0.00") & "%</td></tr>")

        Dim strSiNo As String = String.Empty

        If percMaggiori < 100 Or percMinori < 95 Then
            strSiNo = "NON"
        End If

        table.Append("<tr><td colspan='4' class='" & If(strSiNo = String.Empty, "success", "danger") & "'>")
        table.Append("L'azienda " & strSiNo & " risulta conforme e quindi " & strSiNo & " certificabile in quanto adempie al " &
                          Format(percMaggiori, "0.00") & "% dei requisiti Maggiori e al " &
                          Format(percMinori, "0.00") & "% dei requisiti Minori")
        table.Append("</td></tr>")
        table.Append("</tbody></table>")

        Return table.ToString

    End Function

    Public Function NCS_AuditCheckList(ByVal Dati As String,
                                       ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreVarieBIZ.RispostaStandard


        ''Test
        'Dim oxmlDocTest = New XmlDocument
        'Dim XmlRequestTest = oxmlDocTest.CreateElement("CheckListRequest")
        'Dim XmlCheckListAziendaTest = oxmlDocTest.CreateElement("CheckListAzienda")

        'With XmlCheckListAziendaTest
        '    .SetAttribute("CodiceAzienda", "1006104")
        '    '      .SetAttribute("CodContratto", "5200028187")
        '    .SetAttribute("Anno", 2020)
        '    .SetAttribute("Tipo", "BIO")

        'End With
        'XmlRequestTest.AppendChild(XmlCheckListAziendaTest)

        'Dim XmlCheckListAziendaTest2 = oxmlDocTest.CreateElement("CheckListAzienda")

        'With XmlCheckListAziendaTest2
        '    .SetAttribute("CodiceAzienda", "")
        '    '.SetAttribute("CodContratto", "5200028187")
        '    .SetAttribute("Anno", 2020)
        '    .SetAttribute("Tipo", "BIO")

        'End With
        'XmlRequestTest.AppendChild(XmlCheckListAziendaTest2)

        'oxmlDocTest.AppendChild(XmlRequestTest)

        'Dim strXml As String = AgronicaCoreUtility.AgroZip.DeCompressioneBase64(1, oxmlDocTest.OuterXml)
        ''----- < / Documento XML > -----




        'Lettura Xml

        '<?xml version="1.0" encoding="UTF-8"?>
        ' <CheckListRequest> 
        '   <CheckListAzienda>
        '         <CodiceAzienda>1000031</CodiceAzienda>
        '       <CodContratto>5200034666</CodContratto>
        '       <Anno>2020</Anno>
        '      <Tipo>BIO</Tipo>
        '   </CheckListAzienda>



        Dim rval As New AgronicaCoreVarieBIZ.RispostaStandard

        Dim ixmlDoc = New XmlDocument
        Dim ixCheckListRequests As XmlNodeList
        Dim ixCheckListRequest As XmlElement
        Dim ixCheckListAziendas As XmlNodeList
        Dim ixCheckListAzienda As XmlElement

        Dim CodiceAzienda As String
        Dim CodiceContratto As String
        Dim Anno As String
        Dim Tipo As String

        Dim Piva As String

        Dim i_DatiCheckListAzienda As Integer


        Dim xFiltroAggiuntivo As String = ""
        Dim RisultatoFunzione As String = ""

        Dim ObjCodice As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim ObjAudit As New AgronicaCoreAuditDAL.Audit_R

        Dim DT_Audit As DataTable
        Dim DT_Codice As DataTable
        Dim Dr_Search() As DataRow

        Dim Num_Conformi As Integer
        Dim Num_Non_Conformi As Integer

        Dim ReturnCode As String
        Dim Dettaglio As String
        Dim iErrore As Integer

        Dim Verifica As XmlNode


        Dim strXml As String = AgronicaCoreUtility.AgroZip.DeCompressioneBase64(1, Dati)


        '----- < Documento Output XML > -----
        Dim oxmlDoc = New XmlDocument
        Dim XmlResponse = oxmlDoc.CreateElement("CheckListResponse")

        '----- < Documento Input XML > -----
        'ixmlDoc.async = False
        ixmlDoc.LoadXml(strXml)

        ixCheckListRequests = ixmlDoc.GetElementsByTagName("CheckListRequest")

        If ixCheckListRequests.Count > 0 Then

            ixCheckListRequest = ixCheckListRequests.Item(0)

            ixCheckListAziendas = ixCheckListRequest.GetElementsByTagName("CheckListAzienda")

            i_DatiCheckListAzienda = 0

            Do While i_DatiCheckListAzienda < ixCheckListAziendas.Count

                'Prelevo l'i-esimo blocco di ixCheckListAzienda 
                ixCheckListAzienda = ixCheckListAziendas.Item(i_DatiCheckListAzienda)

                'Inizializzazione Response
                ReturnCode = ""
                Dettaglio = ""
                iErrore = 0
                Anno = Year(Now)
                CodiceAzienda = ""
                CodiceContratto = ""
                Tipo = ""

                'Anno
                Verifica = ixCheckListAzienda.SelectSingleNode("Anno")
                If Verifica IsNot Nothing Then
                    Anno = Verifica.InnerText
                End If

                Verifica = ixCheckListAzienda.SelectSingleNode("CodiceAzienda")
                If Verifica IsNot Nothing Then
                    'If CStr(ixCheckListAzienda.GetElementsByTagName("CodiceAzienda").Item(0).FirstChild.Value) <> "" Then

                    CodiceAzienda = Verifica.InnerText 'CStr(ixCheckListAzienda.GetElementsByTagName("CodiceAzienda").Item(0).FirstChild.Value)

                    'Ricavo la Piva dal codice
                    Piva = ObjCodice.Piva_from_IdCodValCod(1033, CodiceAzienda, objParametri)

                    If Trim(Piva) = "" Then

                        iErrore = 1
                        ReturnCode = "NPRES"
                        Dettaglio = "Azienda non presente in GIAS"

                    Else

                        Verifica = ixCheckListAzienda.SelectSingleNode("CodContratto")
                        If Verifica IsNot Nothing Then
                            'If CStr(ixCheckListAzienda.GetElementsByTagName("CodContratto").Item(0).FirstChild.Value) <> "" Then

                            CodiceContratto = Verifica.InnerText 'CStr(ixCheckListAzienda.GetElementsByTagName("CodContratto").Item(0).FirstChild.Value)
                            xFiltroAggiuntivo = "Upper(Val_Cod) = '" & UCase(CodiceContratto) & "'"

                            'Controllo che il contratto esista
                            DT_Codice = ObjCodice.LeggixCodice(Piva, 1324, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, "", objParametri)

                            If DT_Codice.Rows.Count <> 0 Then

                                'Ok

                            Else

                                iErrore = 1
                                ReturnCode = "NPRES"
                                Dettaglio = "Codice Contratto non valido in GIAS"

                            End If



                        End If




                        If iErrore = 0 Then

                            Verifica = ixCheckListAzienda.SelectSingleNode("Tipo")
                            If Verifica IsNot Nothing Then
                                'If CStr(ixCheckListAzienda.GetElementsByTagName("Tipo").Item(0).FirstChild.Value) <> "" Then
                                Tipo = Verifica.InnerText

                                If UCase(Tipo) = "BIO" Then

                                    'Ok

                                    'Lettura Tabella Audit
                                    DT_Audit = ObjAudit.Leggi(0, 12, 0, Piva, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri)

                                    If DT_Audit.Rows.Count > 0 Then

                                        Dr_Search = DT_Audit.Select("Audit_Stato <> 1")
                                        Num_Non_Conformi = Dr_Search.Count

                                        'Dr_Search = DT_Audit.Select("Audit_Stato = 1")
                                        'Num_Conformi = Dr_Search.Count

                                        'Controllo Risultato
                                        If Num_Non_Conformi <> 0 Then

                                            iErrore = 5
                                            ReturnCode = "PAR"
                                            Dettaglio = "Checklist incompleta o non conforme"

                                        Else

                                            iErrore = 0
                                            ReturnCode = "OK"
                                            Dettaglio = "Checklist conforme"

                                        End If


                                    Else

                                        iErrore = 4
                                        ReturnCode = "NGEST"
                                        Dettaglio = "Nessuna checklist presente"

                                    End If

                                Else

                                    iErrore = 3
                                    ReturnCode = "NPRES"
                                    Dettaglio = "Tipo non presente in GIAS"

                                End If

                            Else

                                iErrore = 3
                                ReturnCode = "NPRES"
                                Dettaglio = "Tipo non valido in GIAS"

                            End If

                        End If

                    End If




                Else

                    iErrore = 1
                    ReturnCode = "NPRES"
                    Dettaglio = "Azienda non valida in GIAS"

                End If

                Verifica = ixCheckListAzienda.SelectSingleNode("CodContratto")
                If Verifica IsNot Nothing Then
                    CodiceContratto = Verifica.InnerText
                End If

                Verifica = ixCheckListAzienda.SelectSingleNode("Tipo")
                If Verifica IsNot Nothing Then
                    Tipo = Verifica.InnerText
                End If

                Verifica = ixCheckListAzienda.SelectSingleNode("Anno")
                If Verifica IsNot Nothing Then
                    Anno = Verifica.InnerText
                Else
                    Anno = ""
                End If


                'Impacchettamento Xml Risulato

                '----- < CheckListAzienda > -----

                Dim oxCheckListAzienda = oxmlDoc.CreateElement("CheckListAzienda")

                Dim oxCodiceAzienda As XmlElement
                oxCodiceAzienda = oxmlDoc.CreateElement("CodiceAzienda")
                oxCodiceAzienda.InnerText = CodiceAzienda
                oxCheckListAzienda.AppendChild(oxCodiceAzienda)

                Dim oxCodContratto As XmlElement
                oxCodContratto = oxmlDoc.CreateElement("CodContratto")
                oxCodContratto.InnerText = CodiceContratto
                oxCheckListAzienda.AppendChild(oxCodContratto)

                Dim oxAnno As XmlElement
                oxAnno = oxmlDoc.CreateElement("Anno")
                oxAnno.InnerText = Anno
                oxCheckListAzienda.AppendChild(oxAnno)

                Dim oxTipo As XmlElement
                oxTipo = oxmlDoc.CreateElement("Tipo")
                oxTipo.InnerText = Tipo
                oxCheckListAzienda.AppendChild(oxTipo)

                Dim oxReturnCode As XmlElement
                oxReturnCode = oxmlDoc.CreateElement("ReturnCode")
                oxReturnCode.InnerText = ReturnCode
                oxCheckListAzienda.AppendChild(oxReturnCode)

                Dim oxDettaglio As XmlElement
                oxDettaglio = oxmlDoc.CreateElement("Dettaglio")
                oxDettaglio.InnerText = Dettaglio
                oxCheckListAzienda.AppendChild(oxDettaglio)

                XmlResponse.AppendChild(oxCheckListAzienda)
                '----- < / CheckListAzienda > -----

                i_DatiCheckListAzienda = i_DatiCheckListAzienda + 1

            Loop

            oxmlDoc.AppendChild(XmlResponse)

            rval.RispostaStringa = oxmlDoc.OuterXml
            rval.RispostaOK = True
            rval.Errore = ""

            '----- < / Documento XML > -----

        End If

        Return rval

    End Function


    'Private Shared Function DeCompressioneBase64(ByVal ZipMode As Byte, ByVal StringIn As String) As String
    '    'converto la stringa base64 in un array di byte
    '    Dim byteArray As Byte() = StrBASE64ToByteArray(StringIn)
    '    'Decomprimo il risultato ottenuto in un nuovo byteArray
    '    Dim byteOut As Byte() = DeCompressione(ZipMode, byteArray)
    '    Return ByteArrayToStr(byteOut)
    'End Function

    'Private Shared Function ByteArrayToStr(ByVal byteArray As Byte()) As String
    '    Dim encoding As New System.Text.UnicodeEncoding()
    '    Return encoding.GetString(byteArray)
    'End Function

    'Private Shared Function StrBASE64ToByteArray(ByVal str As String) As Byte()
    '    Return Convert.FromBase64String(str)
    'End Function


    'Private Shared Function DeCompressione(ByVal ZipMode As Byte, ByVal VettoreByteIn() As Byte) As Byte()
    '    Dim MemStream As New MemoryStream(VettoreByteIn)
    '    Dim ZipStream As Stream = Nothing
    '    ZipStream = New GZipStream(MemStream, CompressionMode.Decompress, True)
    '    Dim VettoreByteOut() As Byte
    '    VettoreByteOut = RetrieveBytesFromStream(ZipStream, VettoreByteIn.Length)
    '    Return VettoreByteOut
    'End Function

    Public Function GeneraLinkAudit(ByVal tipo As Integer, ByVal cod_reg As Integer,
                                    ByVal cod_aud As Integer, ByVal piva As String,
                                    ByRef objParametri_Server As AgronicaCoreParametri,
                                    ByRef objParametri_Super_Server As AgronicaCoreParametri) As String

        Dim objAudit As New AgronicaCoreAuditDAL.Audit_R
        Dim datiAudit As String = cod_aud & "," & cod_reg & "," & piva
        Dim linkAudit = objAudit.LeggiAuditLabel(tipo, "LinkAudit", objParametri_Server)
        Dim tokenAudit = objAudit.LeggiAuditLabel(tipo, "TokenAudit", objParametri_Server)

        If Not String.IsNullOrEmpty(linkAudit) AndAlso Not String.IsNullOrEmpty(tokenAudit) Then
            Dim configurazioneSiti As New Configurazione_Siti_R
            Dim cryptKey As String = configurazioneSiti.Leggi_Valore(0, "cr", "", "", objParametri_Super_Server)
            Dim sicurezza As New AgronicaCoreDataProvider.Sicurezza
            linkAudit &= If(linkAudit.Contains("?"), "&", "?")
            linkAudit &= "token=" & HttpUtility.UrlEncode(sicurezza.EncryptString(tokenAudit & "|" & datiAudit, cryptKey)) & "&tt=audit"
        End If

        Return linkAudit

    End Function

    Public Function InviaMailAudit(ByVal destinatario As String, ByVal oggetto As String, ByVal testo As String, ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim email = destinatario.Split("|")
        Dim mittente As String = If(email.Length > 1, email(0), "")
        Dim mailA As String = If(email.Length > 1, email(1), email(0))
        Dim mailCC As String = If(email.Length > 2, email(2), "")
        Dim mailCCN As String = If(email.Length > 3, email(3), "")

        If String.IsNullOrWhiteSpace(mittente) Then
            Dim configurazioneSiti As New Configurazione_Siti_R
            mittente = configurazioneSiti.Leggi_Valore(0, "MailFrom_smtp", "", "", objParametri_Server)
        End If

        Dim mail As New AgronicaCoreUtility.Mail
        Return mail.invia(objParametri_Server, mittente, mailA, mailCC, mailCCN, oggetto, testo, True, Nothing)

    End Function

End Class
