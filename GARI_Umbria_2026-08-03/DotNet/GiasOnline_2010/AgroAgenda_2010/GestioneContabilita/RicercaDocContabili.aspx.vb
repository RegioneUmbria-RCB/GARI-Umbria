Imports System.Web.Services
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class RicercaDocContabili
    Inherits System.Web.UI.Page

    Dim objParametriAgenda As ParametriAgenda
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String

    'Parametri Query String
    Dim QS_Piva As String
    Dim QS_Type As String = String.Empty
    Dim QS_DocType As String = String.Empty
    Dim QS_ModalitaFatturazione As Boolean = False
    Dim UtenteAbilitatoLettura As Boolean = False
    Dim UtenteAbilitatoScrittura As Boolean = False

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Private Sub InizializzoObjParametri()
        objParametriAgenda = New ParametriAgenda
        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

    Private Sub InizializzoParametriPagina()

        ' Lettura parametri Query string
        QS_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, AgroKey_EncoderDecoder)

        If Not String.IsNullOrWhiteSpace(Request.QueryString("type")) Then
            QS_Type = Request.QueryString("type").ToString
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("doc")) Then
            QS_DocType = Request.QueryString("doc").ToString
        End If

        If LCase(Request.QueryString.ToString).Contains("fatturazione") AndAlso Not String.IsNullOrWhiteSpace(Request.QueryString("fatturazione")) Then
            Dim boolTester As Boolean = False
            If Boolean.TryParse(Request.QueryString("fatturazione"), boolTester) Then
                QS_ModalitaFatturazione = boolTester
            End If
        End If

        hdPiva.Value = QS_Piva
        hdDocType.Value = QS_DocType
        hdType.Value = QS_Type
        hf_ModalitaFatturazione.Value = QS_ModalitaFatturazione

        ImpostaTitoloMenu()

        HttpContext.Current.Session("ricerca_type") = QS_Type
        HttpContext.Current.Session("ricerca_doc") = QS_DocType

        If QS_Type = DocContab_TipoRicerca_Conferimenti Then
            Dim objImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
            hf_filtroMateriePrimeConferimento.Value = objImp.RecuperaFiltroSQLMatPrime_from_AreaGIAS(enum_AreaGIAS.Conferimento, objParametri_Utenti)
        End If


        'Controllo se F&F / Cantine / Tabacco / Zoo
        hf_Modulo_Anagrafe_Log.Value = ""
        Dim elencoModuli = ""
        Dim leggiAnagrafeLog As New OGenerazioni_Anagrafe_Moduli_Log_R
        Dim dtAnagrafeLog = leggiAnagrafeLog.Leggi(hdPiva.Value, 0, 0, 0, 0, -1, -1, -1, AGRODATAINIZIO, AGRODATAFINE,
                                                   "", "",
                                                   objParametri_Server)
        If dtAnagrafeLog IsNot Nothing AndAlso dtAnagrafeLog.Rows.Count > 0 Then
            For Each rAnagrafeLog In dtAnagrafeLog.Rows
                If elencoModuli <> "" Then
                    elencoModuli &= "|"
                End If
                elencoModuli &= CStr(rAnagrafeLog.Item("Modulo_Generazione"))
            Next
        End If
        hf_Modulo_Anagrafe_Log.Value = elencoModuli
        dtAnagrafeLog = Nothing

    End Sub

    Private Sub Leggi_Impostazione_ContattiAcc4ConGerarchia()

        Dim objImpR As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim dtSuperUser As DataTable
        Dim valoreRitorno As Boolean = False

        Try
            dtSuperUser = objImpR.Leggi(enum_Impostazioni_Utenti.SUPERUSER_ACCETTAZIONE_CON_GERARCHIA, 2,
                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "", "", objParametri_Utenti)
            If dtSuperUser IsNot Nothing AndAlso dtSuperUser.Rows.Count > 0 Then
                valoreRitorno = CBool(dtSuperUser.Rows(0).Item("Impostazione_Valore_1"))
            End If

            hdContattiAcc4ConGerarchia.Value = valoreRitorno

        Catch ex As Exception
            Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

    End Sub

    Private Sub Leggi_Impostazione_Workflow()

        Dim objImpR As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim valoreRitorno As String = ""

        Try

            valoreRitorno = objImpR.LeggiStringa_WorkflowDocContabili(hdPiva.Value, objParametri_Server)

            hf_GestioneWorkflow.Value = valoreRitorno

        Catch ex As Exception
            Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

    End Sub

    Private Sub Leggi_Impostazione_GruppiMerce()

        Dim objImpR As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim defaultCategorie As String

        Try
            defaultCategorie = objImpR.LeggiScalareMulticentroAziendaSuperUser(hdPiva.Value, Nothing, enum_Impostazioni_Utenti.Default_GruppoMerce_CategoriaProdotto, "", objParametri_Utenti, objParametri_Server)

            hf_GestioneGruppiMerce.Value = defaultCategorie

        Catch ex As Exception
            Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

    End Sub

    Private Sub Leggi_Impostazione_IB_Controlli()

        Dim objImpR As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim valImp As String

        Try
            valImp = objImpR.LeggiScalareMulticentroAziendaSuperUser(hdPiva.Value, Nothing,
                                                                    enum_Impostazioni_Utenti.IB_Funzione_Controlli_PreInvio,
                                                                    "0",
                                                                    objParametri_Utenti,
                                                                    objParametri_Server)

            hf_IB_Controlli.Value = valImp

        Catch ex As Exception
            Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

    End Sub

    Private Sub ImpostaTitoloMenu()

        Select Case QS_Type

            Case DocContab_TipoRicerca_Acquisti

                Select Case QS_DocType

                    Case DocContab_TipoDoc_Ordine
                        Master.SetTitoloPagina(enum_Sezioni_MenuBS_2017.DocContabile_Ricerca_Ordine_Acquisto)

                    Case DocContab_TipoDoc_Consegna
                        Master.SetTitoloPagina(enum_Sezioni_MenuBS_2017.DocContabile_Ricerca_DDT_Ricevuto)

                    Case DocContab_TipoDoc_Fattura
                        Master.SetTitoloPagina(enum_Sezioni_MenuBS_2017.DocContabile_Ricerca_fattura_Ricevuta)

                End Select

            Case DocContab_TipoRicerca_Vendite

                Select Case QS_DocType

                    Case DocContab_TipoDoc_Ordine
                        Master.SetTitoloPagina(enum_Sezioni_MenuBS_2017.DocContabile_Ricerca_Ordine_Vendita)

                    Case DocContab_TipoDoc_Consegna
                        Master.SetTitoloPagina(enum_Sezioni_MenuBS_2017.DocContabile_Ricerca_DDT_Emesso)

                    Case DocContab_TipoDoc_Fattura
                        Master.SetTitoloPagina(enum_Sezioni_MenuBS_2017.DocContabile_Ricerca_Fattura_Emessa)

                End Select

            Case DocContab_TipoRicerca_Conferimenti

                Select Case QS_DocType

                    Case DocContab_TipoDoc_Consegna, DocContab_TipoDoc_Pomodoro
                        Master.SetTitoloPagina(enum_Sezioni_MenuBS_2017.DocContabile_Ricerca_Conferimento)

                End Select

            Case DocContab_TipoRicerca_Contratti

                Select Case QS_DocType

                    Case DocContab_TipoDoc_ContrattoAffitto
                        Master.SetTitoloPagina(enum_Sezioni_MenuBS_2017.DocContabile_Ricerca_Contratto_Affitto)

                End Select


        End Select

        'TODO
        If QS_ModalitaFatturazione Then
            Master.SetTitoloPaginaCustom("Ricerca per Fatturazione")
        End If

    End Sub

    Private Sub ControlloPermessiUtente()

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        If String.IsNullOrEmpty(QS_Type) OrElse String.IsNullOrEmpty(QS_DocType) Then
            UtenteAbilitatoLettura = False
            UtenteAbilitatoScrittura = False
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
            Exit Sub
        End If

        ' Lettura
        UtenteAbilitatoLettura = UtenteAbilitato_LetturaScrittura(objPermessi, enum_Security_Operazione.Lettura)
        If Not UtenteAbilitatoLettura Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
            Exit Sub
        End If
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura

        ' Scrittura
        UtenteAbilitatoScrittura = UtenteAbilitato_LetturaScrittura(objPermessi, enum_Security_Operazione.Modifica)
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        ' Scrittura Conferimento Pomodoro
        hf_UtenteAbilitatoPomodoro.Value = objPermessi.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                                              Session("ASG_IdServizio"),
                                                                              enum_Security_Attivita.Nuovo_Conferimento_Pomodoro,
                                                                              enum_Security_Operazione.Modifica,
                                                                              Now, "", objParametri_Utenti)

        'Blocco Documento
        hdUtenteAbilitatoBlocco.Value = objPermessi.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                                              Session("ASG_IdServizio"),
                                                                              enum_Security_Attivita.Contabilita_Operazioni_Blocco,
                                                                              enum_Security_Operazione.Modifica,
                                                                              Now, "", objParametri_Utenti)

        'Sblocco Documento
        hdUtenteAbilitatoSblocco.Value = objPermessi.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                                               Session("ASG_IdServizio"),
                                                                               enum_Security_Attivita.Contabilita_Operazioni_Sblocco,
                                                                               enum_Security_Operazione.Modifica,
                                                                               Now, "", objParametri_Utenti)

        'Gestione Prezzi Lettura
        hdUtenteAbilitatoGestionePrezziLettura.Value = objPermessi.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                                                             Session("ASG_IdServizio"),
                                                                                             enum_Security_Attivita.Gestione_Prezzi,
                                                                                             enum_Security_Operazione.Lettura,
                                                                                             Now, "", objParametri_Utenti)

        hf_UtenteAbilitatoGestioneNuovoAllegato.Value = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                              Session("ASG_IdServizio"),
                                                                              enum_Security_Attivita.Documentale_Inser,
                                                                              enum_Security_Operazione.Modifica,
                                                                              Now, "",
                                                                              objParametri_Utenti)

        hf_UtenteAbilitatoGestioneVisualizaAllegato.Value = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                              Session("ASG_IdServizio"),
                                                                              enum_Security_Attivita.Documentale_Lista,
                                                                              enum_Security_Operazione.Lettura,
                                                                              Now, "",
                                                                              objParametri_Utenti)

    End Sub

    Private Function UtenteAbilitato_LetturaScrittura(ByVal objPermessi As AgronicaCoreUtentiDAL.Utenti_Permessi_R,
                                                      ByVal tipoOperazione As enum_Security_Operazione) As Boolean

        Dim attivitaDaControllare As enum_Security_Attivita = 0
        Dim risultato As Boolean = False

        Select Case QS_Type.ToUpper()

            Case DocContab_TipoRicerca_Acquisti

                Select Case QS_DocType

                    Case DocContab_TipoDoc_Ordine
                        attivitaDaControllare = enum_Security_Attivita.Ordini_Acquisto

                    Case DocContab_TipoDoc_Consegna
                        attivitaDaControllare = enum_Security_Attivita.Consegne_Acquisto

                    Case DocContab_TipoDoc_Fattura
                        attivitaDaControllare = enum_Security_Attivita.Fatture_Acquisto

                End Select

            Case DocContab_TipoRicerca_Vendite

                Select Case QS_DocType

                    Case DocContab_TipoDoc_Ordine
                        attivitaDaControllare = enum_Security_Attivita.Ordini_Vendita

                    Case DocContab_TipoDoc_Consegna
                        attivitaDaControllare = enum_Security_Attivita.Consegne_Vendita

                    Case DocContab_TipoDoc_Fattura
                        attivitaDaControllare = enum_Security_Attivita.Fatture_Vendita

                End Select

            Case DocContab_TipoRicerca_Conferimenti

                Select Case QS_DocType

                    Case DocContab_TipoDoc_Consegna, DocContab_TipoDoc_Pomodoro
                        attivitaDaControllare = enum_Security_Attivita.Consegne_Conferimento

                End Select

            Case DocContab_TipoRicerca_Contratti

                Select Case QS_DocType

                    Case DocContab_TipoDoc_ContrattoAffitto
                        attivitaDaControllare = enum_Security_Attivita.Contratti_Affitto

                End Select

        End Select

        risultato = objPermessi.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                          Session("ASG_IdServizio"),
                                                          attivitaDaControllare,
                                                          tipoOperazione,
                                                          Date.Now,
                                                          "",
                                                          objParametri_Utenti)

        Return risultato

    End Function

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master().Lbl_Titolo.Text = AgronicaAgenda_2010.RicercaDocumenti

        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        InizializzoObjParametri()

        If Not IsPostBack Then
            InizializzoParametriPagina()
            ControlloPermessiUtente()
            Leggi_Impostazione_ContattiAcc4ConGerarchia()
            Leggi_Impostazione_Workflow()
            Leggi_Impostazione_GruppiMerce()
            Leggi_Impostazione_IB_Controlli()
        End If

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Parametri_Qualitativi(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim leggi As New Statistiche_R
            Dim dtParamQual = leggi.Leggi_Parametri_Qualitativi(piva, objParametriServer)

            Dim parametro As String = ""
            Dim campo As String = ""
            Dim tabellaDes As String = ""
            Dim jArrayParamQual As New JArray()
            For Each dr As DataRow In dtParamQual.Rows
                If dr.Item("Tipo") = 1 OrElse
                    dr.Item("Tipo") = 3 OrElse
                    dr.Item("Tipo") = 4 OrElse
                    dr.Item("Tipo") = 5 Then

                    parametro = CStr(dr.Item("Tabella_Key"))

                    If parametro <> "" Then
                        tabellaDes = CStr(dr.Item("Tabella_Des"))

                        If dr.Item("Tipo") = 1 Then
                            campo = parametro & "_Descrizione"
                        End If
                        If dr.Item("Tipo") = 3 OrElse
                            dr.Item("Tipo") = 4 OrElse
                            dr.Item("Tipo") = 5 Then
                            campo = parametro & "_Val_Cod"
                        End If

                        'Dim nome As String = parametro.Substring(0, 1).ToUpper() + parametro.Substring(1)
                        jArrayParamQual.Add(New JObject(New JProperty("campo", campo), New JProperty("nome", tabellaDes), New JProperty("tipo", dr.Item("Tipo"))))
                    End If

                End If
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayParamQual, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Categorie_Magazzino(ByVal type As String, ByVal isFFZooTabacco As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim xFiltroAggiuntivo = " Elem_Cod NOT IN (1,4,300) "
            If isFFZooTabacco = 1 Then
                If type = "C" Then
                    xFiltroAggiuntivo = " Elem_Cod  IN (210,310) "
                Else
                    If type = "A" Then
                        xFiltroAggiuntivo = " Elem_Cod NOT IN (1,4,300,210,310) "
                    End If
                End If
            End If
            Dim objCM As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
            Dim dtCategMag = objCM.Leggi(0, "", False, xFiltroAggiuntivo, "", objParametriServer)

            Dim jArrayCategorie As New JArray()
            For Each dr As DataRow In dtCategMag.Rows
                jArrayCategorie.Add(New JObject(New JProperty("Elem_Cod", CStr(dr.Item("Elem_Cod"))), New JProperty("Elem_Des", dr.Item("NomeComune"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayCategorie, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Categorie_Commerciali(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objCC As New AgronicaCoreContabDAL.Linee_Classi_Produzioni_R
            Dim dtCategCommle = objCC.Leggi(piva, 0, "", "", objParametriServer)

            Dim jArrayCategorieComm As New JArray()
            For Each dr As DataRow In dtCategCommle.Rows
                jArrayCategorieComm.Add(New JObject(New JProperty("Linea_Classe_Cod", CStr(dr.Item("Linea_Classe_Cod"))), New JProperty("Linea_Classe_Des", dr.Item("Linea_Classe_Des"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayCategorieComm, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Specie() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim SpecieVegetali_R As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R()
            Dim dtSpecie = SpecieVegetali_R.Leggi(0, 0, "", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametriServer)

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dtSpecie, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Varieta(filtro_specie As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim filtro_aggiuntivo As String = ""
            If Trim(filtro_specie) <> "" Then
                filtro_aggiuntivo = " Cultivar.Veg_Cod IN (" & filtro_specie & ") "
            End If

            Dim Cultivar_R As New AgronicaCoreMetaSchemaDAL.Cultivar_R
            Dim dtVarieta = Cultivar_R.Leggi(0, 0, "", enumSelezioneVariabile.Selezione_JoinDescrizioni, filtro_aggiuntivo, "", objParametri_Server)

            Dim jArrayParamQual As New JArray()
            For Each dr As DataRow In dtVarieta.Rows
                jArrayParamQual.Add(New JObject(New JProperty("Cul_Cod", dr.Item("Cul_Cod")), New JProperty("Cul_Des", dr.Item("Veg_Des") & " - " & dr.Item("Cul_Des"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayParamQual, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Contatti_Clienti_Agenti(ByVal piva As String,
                                                         ByVal testoRicerca As String,
                                                         ByVal cliente As Integer,
                                                         ByVal fornitore As Integer,
                                                         ByVal agente As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim valoreFiltro As String = ""

            Dim objFilters As JArray = Nothing
            If Not String.IsNullOrEmpty(testoRicerca) Then
                objFilters = JArray.Parse(testoRicerca)
                If objFilters IsNot Nothing Then
                    For Each obj As JObject In objFilters
                        valoreFiltro = obj("value").ToString()
                    Next
                End If
            End If

            Dim filtro As String = If(String.IsNullOrEmpty(valoreFiltro), "",
                String.Format(" Rag_Soc Like '%{0}%' OR Nome Like '%{0}%' or Cognome Like '%{0}%' ", valoreFiltro))

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dtContatti As DataTable = objContatti.Contatti_Contatto_Leggi(piva,
                "", 0, 0, False, False, 0, 0, False, 0, ID_CF_NOFILTRO, 0, "", False, cliente, fornitore, 0, 0, 0,
                enumSelezioneVariabile.Selezione_JoinDescrizioni, filtro, "Rag_Soc, Cognome, Nome", objParametriServer, Agente:=agente)

            Dim jArrayListaClientiAgenti As New JArray()
            For Each dr As DataRow In dtContatti.Rows
                Dim ragSoc As String = CStr(dr.Item("Rag_Soc")) & CStr(dr.Item("Cognome")) & " " & CStr(dr.Item("Nome"))
                jArrayListaClientiAgenti.Add(New JObject(New JProperty("cod_contatto", dr.Item("cod_contatto")), New JProperty("nome", ragSoc.Replace("""", "'"))))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaClientiAgenti.Take(10), Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ApriModificaDocumento(ByVal piva As String,
                                                 ByVal Id_Agenda As Integer,
                                                 ByVal Lav_Cod As Integer,
                                                 ByVal Tipo_Operazione As Integer,
                                                 ByVal Modalita_Protetta As Integer
                                                 ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim paginaLink As String = ""
        Dim tipo As Integer = 0

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objParametriAgenda = New ParametriAgenda With {
                .Tipo_Operazione = Tipo_Operazione,
                .Lav_Cod = Lav_Cod,
                .Id_Agenda = Id_Agenda
            }

            paginaLink = "../GestioneContabilita/DocContabile.aspx"
            paginaLink &= "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                          "&o=" & Stringa_Codifica(objParametriAgenda.Tipo_Operazione, AgroKey_EncoderDecoder) &
                          "&s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                          "&i=" & Stringa_Codifica(objParametriAgenda.Id_Agenda, AgroKey_EncoderDecoder) &
                          "&l=" & Stringa_Codifica(objParametriAgenda.Lav_Cod, AgroKey_EncoderDecoder) &
                          "&d=" & Stringa_Codifica(CStr(objParametriAgenda.Data), AgroKey_EncoderDecoder) &
                          "&mp=" & Stringa_Codifica(Modalita_Protetta, AgroKey_EncoderDecoder) &
                          "&tf=" & Stringa_Codifica(tipo, AgroKey_EncoderDecoder) &
                          "&rs=" & Stringa_Codifica(objParametriAgenda.RagSoc, AgroKey_EncoderDecoder) &
                          "&orig=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_RicercaDocContabili, AgroKey_EncoderDecoder) &
                          "&ricercatype=" & HttpContext.Current.Session("ricerca_type") &
                          "&ricercadoc=" & HttpContext.Current.Session("ricerca_doc")

            r.RispostaOK = True
            r.RispostaStringa = paginaLink

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function NuovoDocumento(ByVal piva As String, ByVal Id_Agenda As Integer, ByVal Lav_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim paginaLink As String = ""
        Dim tipo As Integer = 0

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objParametriAgenda = New ParametriAgenda With {
                .Tipo_Operazione = enum_TipoOperazioneDB.Scrittura,
                .Lav_Cod = Lav_Cod,
                .Id_Agenda = Id_Agenda
            }

            paginaLink = "../GestioneContabilita/DocContabile.aspx"

            paginaLink &= "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                          "&o=" & Stringa_Codifica(objParametriAgenda.Tipo_Operazione, AgroKey_EncoderDecoder) &
                          "&s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                          "&i=" & Stringa_Codifica(objParametriAgenda.Id_Agenda, AgroKey_EncoderDecoder) &
                          "&l=" & Stringa_Codifica(objParametriAgenda.Lav_Cod, AgroKey_EncoderDecoder) &
                          "&d=" & Stringa_Codifica(CStr(objParametriAgenda.Data), AgroKey_EncoderDecoder) &
                          "&tf=" & Stringa_Codifica(tipo, AgroKey_EncoderDecoder) &
                          "&rs=" & Stringa_Codifica(objParametriAgenda.RagSoc, AgroKey_EncoderDecoder) &
                          "&orig=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_RicercaDocContabili, AgroKey_EncoderDecoder)

            r.RispostaOK = True
            r.RispostaStringa = paginaLink

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    'salvataggio parametri report
    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaFiltri(ByVal parametri As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim modificati As New JArray()
            Dim nuovi As New JArray()
            Dim tutti As JArray = Nothing
            Dim jsonDaSalvare = JsonConvert.DeserializeObject(parametri)
            Dim scriviImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W

            Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtImpostazioni = leggiImpostazioni.Leggi(enum_Impostazioni_Utenti.UTENTE_FILTRI_RICERCA_DOC_CONTABILI, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            If dtImpostazioni.Rows.Count > 0 Then
                Dim salvato = JsonConvert.DeserializeObject(dtImpostazioni.Rows(0).Item("Impostazione_Valore_1"))
                Dim versioneSalvata = salvato("_versione")
                Dim versioneCorrente = jsonDaSalvare("_versione")

                If versioneSalvata Is Nothing OrElse versioneSalvata.ToString() <> versioneCorrente.ToString Then
                    scriviImpostazioni.Cancella(enum_Impostazioni_Utenti.UTENTE_FILTRI_RICERCA_DOC_CONTABILI, "", objParametri_Utenti)
                End If

                Dim griglieSalvate As JArray = salvato("_griglie")
                Dim griglieDaSalvare As JArray = jsonDaSalvare("_griglie")

                For Each daSalvare In griglieDaSalvare
                    If griglieSalvate.Any(Function(s) s("IdControllo").ToString() = daSalvare("IdControllo").ToString()) Then
                        modificati.Add(daSalvare)
                    Else
                        nuovi.Add(daSalvare)
                    End If
                Next
                tutti = New JArray(modificati.Union(nuovi))

                For Each salvato In griglieSalvate
                    If Not tutti.Any(Function(s) s("IdControllo").ToString() = salvato("IdControllo").ToString()) Then
                        tutti.Add(salvato)
                    End If
                Next
            Else
                tutti = jsonDaSalvare("_griglie")
            End If

            jsonDaSalvare("_griglie") = tutti
            Dim impostazioniReport = JsonConvert.SerializeObject(jsonDaSalvare, Formatting.None)
            r.RispostaOK = scriviImpostazioni.Cancella(enum_Impostazioni_Utenti.UTENTE_FILTRI_RICERCA_DOC_CONTABILI, "", objParametri_Utenti)
            r.RispostaOK = scriviImpostazioni.Scrivi(enum_Impostazioni_Utenti.UTENTE_FILTRI_RICERCA_DOC_CONTABILI, impostazioniReport, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)

            If r.RispostaOK Then
                r.RispostaStringa = "true"
            Else
                r.RispostaStringa = "Problema nel salvataggio del report"
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function ValorizzazioneConferimenti(ByVal piva As String,
                                                      ByVal elenco As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim ObjReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim ObjCDG_BIZ As New AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ
        Dim utilityHelper As New AgronicaCoreModello.UtilityHelper
        Dim ObjMovimento As New AgronicaCoreContabDAL.Movimenti_W
        Dim dt As DataTable

        Dim Arrayp As String()
        Dim i As Integer
        Dim Filtro_Aggiuntivo As String = ""
        Dim Costo As Decimal = 0
        Dim Prezzo_Unitario As Decimal = 0
        Dim Costo_Delta As Decimal = 0
        Dim Importo As Decimal = 0
        Dim Lav_cod As Integer = 0
        Dim Id_Agenda As Integer = 0
        Dim Id_Mov_Det As Integer = 0
        Dim Qta_Totale As Decimal = 0
        Dim Variazioni As Decimal = 0
        Dim Imposta As Decimal = 0
        Dim Imponibile_Netto As Decimal = 0
        Dim Imponibile_Lordo As Decimal = 0
        Dim listCastelletto As List(Of Contabilita_Castelletto_Iva) = Nothing
        Dim objMovDets As List(Of Movimento_Dettaglio)
        Dim objMovimenti_DettagliHelper As New Agenda_Movimenti_Dettagli_Helper
        Dim ht As New Hashtable
        Dim myKey As String = ""
        Dim numero As Integer = 0
        Dim count As Integer = 0
        Dim bArresto As Boolean = False

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Arrayp = Split(elenco & ",", ",")

            For i = 0 To UBound(Arrayp) - 1

                If IsNumeric(Arrayp(i)) Then

                    'Lettura degli impianti e prodotti associati ai conferimenti selezionati
                    dt = ObjReg_Impianti.LeggixConferimento(piva, Arrayp(i), objParametriServer)

                    If dt.Rows.Count > 0 Then

                        For Each dr As DataRow In dt.Rows

                            Lav_cod = dr.Item("Lav_Cod")
                            count += 1

                            myKey = dr.Item("Mat_Cod") & "|" & dr.Item("Progetto_Cod")

                            If Not ht.ContainsKey(myKey) Then

                                Costo = ObjCDG_BIZ.Valorizzazione_Conferimento(dr.Item("Piva"),
                                                                               dr.Item("Progetto_Cod"),
                                                                               dr.Item("Elem_Cod"),
                                                                               dr.Item("Mat_Cod"),
                                                                               dr.Item("Priorita"),
                                                                               dr.Item("Validita_Inizio"),
                                                                               dr.Item("Validita_Fine"),
                                                                               objParametriServer)

                                'Salvo il costo della coppia mat_cod-impianto
                                ht.Add(myKey, Costo)

                            Else

                                'Costo già calcolato
                                Costo = ht(myKey)

                            End If

                            'Aggiornamento Parziali
                            Costo_Delta = Costo_Delta + (Costo * dr.Item("Qta"))
                            Qta_Totale = Qta_Totale + dr.Item("Qta")


                            'Controllo criterio di arresto sul dettaglio e valorizzazione qta totale
                            bArresto = False
                            If count = dt.Rows.Count Then
                                bArresto = True
                            ElseIf dt(count).Item("Id_Mov_Det") <> dr.Item("Id_Mov_Det") Then
                                bArresto = True
                            End If

                            If bArresto AndAlso Qta_Totale <> 0 Then

                                Id_Mov_Det = dr.Item("Id_Mov_Det")

                                'Variazione di Id_Mov Det --> aggiornamento Riga 
                                Prezzo_Unitario = Math.Round(Costo_Delta / Qta_Totale, 6)

                                'Reset Valori
                                Costo_Delta = 0
                                Qta_Totale = 0

                                'Calcolo ed aggiornamento dei dati economici
                                ContabilitaHelper_Dettaglio.AggiornaDettagliEconomiciDaPrezzoKg(piva, Id_Mov_Det, Prezzo_Unitario, False, 0, objParametriServer,,,,, True)

                                numero += 1


                                'Aggiornamento Num_Protocollo
                                If count = dt.Rows.Count Then

                                    count = 0
                                    Id_Agenda = dr.Item("Id_Agenda")


                                    'Lettura Dettaglio
                                    objMovDets = objMovimenti_DettagliHelper.Leggi(piva, 0, Id_Agenda, 0, 0, objParametriServer)

                                    'Calcolo Importo Totale documento
                                    Importo = utilityHelper.Calcola_Importo_Documento(Imponibile_Netto,
                                                                                      Imponibile_Lordo,
                                                                                      Variazioni,
                                                                                      Imposta,
                                                                                      listCastelletto,
                                                                                      Lav_cod,
                                                                                      0,
                                                                                      objMovDets,
                                                                                      objParametriServer)

                                    'Aggiornamento Totale Documento
                                    ObjMovimento.ModificaPuntuale(piva, 0, Id_Agenda, 0, objParametriServer, Num_Protocollo:=Importo)

                                End If

                            End If


                        Next

                    End If

                End If

            Next

            r.RispostaStringa = "Processo Terminato Correttamente. Aggiornate " & numero & " Righe"
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function



    'legge parametri report
    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiFiltri(ByVal versione As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim scriviImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
            Dim dtImpostazioni = leggiImpostazioni.Leggi(enum_Impostazioni_Utenti.UTENTE_FILTRI_RICERCA_DOC_CONTABILI, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            If dtImpostazioni.Rows.Count > 0 Then
                Dim impostazioni = JsonConvert.DeserializeObject(dtImpostazioni.Rows(0).Item("Impostazione_Valore_1"))

                Dim versioneSalvata = impostazioni("_versione")
                If versioneSalvata Is Nothing OrElse versioneSalvata.ToString <> versione Then
                    scriviImpostazioni.Cancella(enum_Impostazioni_Utenti.UTENTE_FILTRI_RICERCA_DOC_CONTABILI, "", objParametri_Utenti)
                    r.RispostaStringa = ""
                Else
                    r.RispostaStringa = JsonConvert.SerializeObject(impostazioni, Formatting.None)
                End If

            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    'salvataggio parametri report
    <WebMethod(EnableSession:=True)>
    Public Shared Function PulisciFiltri() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim scriviImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
            r.RispostaOK = scriviImpostazioni.Cancella(enum_Impostazioni_Utenti.UTENTE_FILTRI_RICERCA_DOC_CONTABILI, "", objParametriUtenti)

            If r.RispostaOK Then
                r.RispostaStringa = ""
            Else
                r.RispostaStringa = "Problema nel salvataggio del report"
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    'salvataggio parametri report
    <WebMethod(EnableSession:=True)>
    Public Shared Function LinkPassaggioDiStato(praticheCod As String) As RispostaStandard

        Dim r As New RispostaStandard
        r.RispostaOK = True

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            r.RispostaStringa = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkPassaggioDiStato(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, praticheCod, 0)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#Region "Scripts Gestione Campionamneto"

    <WebMethod(EnableSession:=True)>
    Public Shared Function UrlGestioneCampionamento(
                                    ByVal piva As String,
                                    ByVal id_mov_det As Integer,
                                    ByVal chiamatodaRisLiqu As String
                                     ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim permessi = New PermessiUtente
            Dim utenteAbilitato_Modifica = permessi.getPermesso(enum_Security_Attivita.FF_CampionamentoLiquidazioni_Movimenti).Scrittura

            If Not utenteAbilitato_Modifica Then
                r.RispostaOK = False
                r.Errore = AgronicaAgenda_2010.MancanzaPermessiModificaContatti
            Else
                r.RispostaStringa = Redirect_Gestione_Campionamneto(piva, id_mov_det, CBool(chiamatodaRisLiqu))
                r.RispostaOK = True
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    Private Shared Function Redirect_Gestione_Campionamneto(
                            ByVal piva As String,
                            ByVal id_mov_det As Integer,
                            ByVal chiamatodaRisLiqu As Boolean
                            ) As String

        Dim queryString As String = "?p=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, Nothing)
        queryString += "&pr=" & Stringa_Codifica(True, AgroKey_EncoderDecoder, Nothing)
        queryString += "&id_mov_det=" & Stringa_Codifica(id_mov_det, AgroKey_EncoderDecoder, Nothing)

        'Se viene chiamato dalla pagina RisultatoLiquidazione compongo un URL diverso e ci aggiungo anche un parametro in query
        'string per l'apertura dall' Iframe di RisultatoLiquidazione
        Dim url As String = String.Empty
        If chiamatodaRisLiqu = False Then
            url = "../GestioneContabilita/Liquidazione/CampionamentoConferito/CampionamentoMovimenti.aspx"
            queryString += "&ifrisliq=" & Stringa_Codifica(False, AgroKey_EncoderDecoder, Nothing)
        Else
            url = "../Liquidazione/CampionamentoConferito/CampionamentoMovimenti.aspx"
            queryString += "&ifrisliq=" & Stringa_Codifica(True, AgroKey_EncoderDecoder, Nothing)
        End If

        Return url & queryString

    End Function

#End Region

#Region "script services Cerca Id Testata Griglia da Movimento di conferimento"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Id_Testata_Griglia_Da_Movim_Conferimento(ByVal piva As String,
            ByVal Id_Mov_Det As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim Id_Testata_Griglia_Trovata As Integer = 0
            Dim Id_Testata_Griglia_Prod_Trovata As Integer = 0
            Dim leggi As New FF_CampionamentoConferimento_R
            r.RispostaStringa = leggi.Leggi_Id_Testata_Griglia_Da_Movim_Conferimento(piva,
                    Id_Mov_Det, Id_Testata_Griglia_Trovata, Id_Testata_Griglia_Prod_Trovata, True, True, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=False)
        End Try

        Return r
    End Function

#End Region

#Region "Schedulazione Fatture"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Schedulazione_Fatture(ByVal lav_cod_destinazione As Integer,
                                                       ByVal ids_agende As String,
                                                       ByVal ids_mov_det As String,
                                                       ByVal parametriRottura As String,
                                                       ByVal data As String
                                                       ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim dataTester As DateTime
            If String.IsNullOrEmpty(data) Then
                dataTester = DateTime.Now
            Else
                DateTime.TryParse(data, dataTester)
            End If

            Dim dp As New Scheduling_Documenti_Contabili_W
            dp.Scrivi(lav_cod_destinazione, 0, dataTester, ids_agende, ids_mov_det, parametriRottura,
                      Nothing, Nothing, Nothing,
                      AGRODATAINIZIO, AGRODATAFINE, objParametriServer)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=False)
        End Try

        Return r

    End Function

#End Region

End Class
