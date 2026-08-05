Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports System.Web.Services
Imports AgronicaCoreContabDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreXML.XML_Stampe
Imports AgronicaCoreUtentiDAL
Imports AgroAgenda_2010.Resources

Public Class RicercaGiacenzeDocContabili
    Inherits System.Web.UI.Page

    Dim objParametriAgenda As ParametriAgenda
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String

    'Parametri Query String
    Dim QS_Piva As String
    Dim QS_Type As String = String.Empty
    Dim QS_DocType As String = String.Empty
    Dim QS_Modalita As String = String.Empty
    Dim UtenteAbilitatoLettura As Boolean = False
    Dim UtenteAbilitatoScrittura As Boolean = False

    Private Sub InizializzoObjParametri()
        objParametriAgenda = New ParametriAgenda
        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

    ''' <summary>
    ''' Lettura parametri Query string
    ''' </summary>
    Private Sub InizializzoParametriPagina()

        QS_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, AgroKey_EncoderDecoder)

        If String.IsNullOrWhiteSpace(Request.QueryString("mod")) Or (Request.QueryString("mod") <> "P" And Request.QueryString("mod") <> "F") Then
            QS_Modalita = "P"
        Else
            QS_Modalita = Request.QueryString("mod").ToString
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("type")) Then
            QS_Type = Request.QueryString("type").ToString
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("doc")) Then
            QS_DocType = Request.QueryString("doc").ToString
        End If

        'Utilizzo
        If Not String.IsNullOrWhiteSpace(Request.QueryString("ut")) Then
            hdUtilizzo.Value = Request.QueryString("ut")
        End If

        If QS_Modalita = "F" Then
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        Else
            Master().Lbl_Titolo.Text = "Ricerca Documenti"
            ImpostaTitoloMenu()
        End If

        hdPiva.Value = QS_Piva
        hdDocType.Value = QS_DocType
        hdType.Value = QS_Type
        hdMod.Value = QS_Modalita

        HttpContext.Current.Session("ricerca_type") = QS_Type
        HttpContext.Current.Session("ricerca_doc") = QS_DocType


        'SA_COD
        Dim strJsonCentriAziendali As String = Stringa_Decodifica(Request.QueryString("sa_cod_m"), AgroKey_EncoderDecoder, Nothing)
        If Not String.IsNullOrWhiteSpace(strJsonCentriAziendali) Then
            hdSaCod.Value = strJsonCentriAziendali
        End If

        Dim qsDataGiacenza As String = Request.QueryString("data_giacenza")
        If Not qsDataGiacenza Is Nothing Then
            hdDataGiacenza.Value = Stringa_Decodifica(qsDataGiacenza, AgroKey_EncoderDecoder, Nothing)
        End If

        Dim strJsonCodContatti As String = Stringa_Decodifica(Request.QueryString("cod_contatto_m"), AgroKey_EncoderDecoder, Nothing)
        If Not String.IsNullOrWhiteSpace(strJsonCodContatti) Then
            hdContatto.Value = strJsonCodContatti
        End If
        
        Dim strJsonCategorieProdotti As String = Stringa_Decodifica(Request.QueryString("elem_cod_m"), AgroKey_EncoderDecoder, Nothing)
        If Not String.IsNullOrWhiteSpace(strJsonCategorieProdotti) Then
            hdCategProdotto.Value = strJsonCategorieProdotti
        End If

        Dim strJsonSpecieVegetale As String = Stringa_Decodifica(Request.QueryString("veg_cod_m"), AgroKey_EncoderDecoder, Nothing)
        If Not String.IsNullOrWhiteSpace(strJsonSpecieVegetale) Then
            hdSpecieVeg.Value = strJsonSpecieVegetale
        End If

        Dim strJsonVarieta As String = Stringa_Decodifica(Request.QueryString("cul_cod_m"), AgroKey_EncoderDecoder, Nothing)
        If Not String.IsNullOrWhiteSpace(strJsonVarieta) Then
            hdVarieta.Value = strJsonVarieta
        End If

        Dim qsCodiceFabbricato As String = Stringa_Decodifica(Request.QueryString("fabbricato_cod"), AgroKey_EncoderDecoder, Nothing)
        if Not String.IsNullOrWhiteSpace(qsCodiceFabbricato) Then
            hdFabbricatoCod.Value = qsCodiceFabbricato
        End If
        
        Dim lotto As String = Stringa_Decodifica(Request.QueryString("lotto"), AgroKey_EncoderDecoder, Nothing)
        If Not String.IsNullOrWhiteSpace(lotto) Then
            hdLotto.Value = lotto
        End If

        Dim qsDataInizioRangeDDT As String = Request.QueryString("di_ddt")
        If Not qsDataInizioRangeDDT Is Nothing Then
            hdDataInizioRangeDDT.Value = Stringa_Decodifica(qsDataInizioRangeDDT, AgroKey_EncoderDecoder, Nothing)
        End If

        Dim qsDataFineRangeDDT As String = Request.QueryString("df_ddt")
        If Not qsDataFineRangeDDT Is Nothing Then
            hdDataFineRangeDDT.Value = Stringa_Decodifica(qsDataFineRangeDDT, AgroKey_EncoderDecoder, Nothing)
        End If

        Dim qsInGiacenza As String = Request.QueryString("in_giacenza")
        If Not qsInGiacenza Is Nothing Then
            hdInGiacenza.Value = Stringa_Decodifica(qsInGiacenza, AgroKey_EncoderDecoder, Nothing)
        End If

    End Sub

    Private Sub Leggi_Impostazione_ContattiAcc4ConGerarchia()

        Dim objImpR As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim dtSuperUser As DataTable
        Dim valoreRitorno As Boolean = False

        Try
            dtSuperUser = objImpR.Leggi(enum_Impostazioni_Utenti.SUPERUSER_ACCETTAZIONE_CON_GERARCHIA, 2,
                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "", "", objParametri_Utenti)
            If Not dtSuperUser Is Nothing AndAlso dtSuperUser.Rows.Count > 0 Then
                valoreRitorno = CBool(dtSuperUser.Rows(0).Item("Impostazione_Valore_1"))
            End If

            hdContattiAcc4ConGerarchia.Value = valoreRitorno

        Catch ex As Exception
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

    End Sub


    Private Sub ImpostaTitoloMenu()

        Select Case QS_Type
            Case "A"
                Select Case QS_DocType
                    Case "O"
                        Master.SetTitoloPagina(121)
                    Case "C"
                        Master.SetTitoloPagina(123)
                    Case "F"
                        Master.SetTitoloPagina(127)
                End Select

            Case "V"
                Select Case QS_DocType
                    Case "O"
                        Master.SetTitoloPagina(130)
                    Case "C"
                        Master.SetTitoloPagina(132)
                    Case "F"
                        Master.SetTitoloPagina(134)
                End Select

            Case "C"
                Select Case QS_DocType
                    Case "C", "P"
                        Master.SetTitoloPagina(139)
                End Select
        End Select
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

    End Sub

    Private Function UtenteAbilitato_LetturaScrittura(ByVal objPermessi As AgronicaCoreUtentiDAL.Utenti_Permessi_R,
                                                      ByVal tipoOperazione As enum_Security_Operazione) As Boolean

        Dim attivitaDaControllare As enum_Security_Attivita = 0
        Dim risultato As Boolean = False

        Select Case QS_Type.ToUpper()

            ' ACQUISTI
            Case "A"
                Select Case QS_DocType
                    ' Ordini Acquisto
                    Case "O"
                        attivitaDaControllare = enum_Security_Attivita.Ordini_Acquisto
                    ' Consegne Acquisto
                    Case "C"
                        attivitaDaControllare = enum_Security_Attivita.Consegne_Acquisto
                    ' Fatture Acquisto
                    Case "F"
                        attivitaDaControllare = enum_Security_Attivita.Fatture_Acquisto
                End Select

            ' VENDITE
            Case "V"
                Select Case QS_DocType
                    'Ordini Vendita 
                    Case "O"
                        attivitaDaControllare = enum_Security_Attivita.Ordini_Vendita
                    'Documenti Vendita 
                    Case "C"
                        attivitaDaControllare = enum_Security_Attivita.Consegne_Vendita
                    Case "F"
                        attivitaDaControllare = enum_Security_Attivita.Fatture_Vendita
                End Select

            ' CONFERIMENTI
            Case "C"
                Select Case QS_DocType
                    Case "C", "P"
                        attivitaDaControllare = enum_Security_Attivita.Consegne_Conferimento
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
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        InizializzoObjParametri()

        If Not IsPostBack Then
            InizializzoParametriPagina()
            ControlloPermessiUtente()
            Leggi_Impostazione_ContattiAcc4ConGerarchia()
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

            Dim jArrayParamQual As New JArray()
            For Each dr As DataRow In dtParamQual.Rows
                If dr.Item("Tipo") <> 3 Then
                    Dim parametro As String = CStr(dr.Item("Tabella_Key"))
                    Dim campo As String = parametro & "_Descrizione"
                    Dim tabellaDes As String = CStr(dr.Item("Tabella_Des"))

                    'Dim nome As String = parametro.Substring(0, 1).ToUpper() + parametro.Substring(1)
                    jArrayParamQual.Add(New JObject(New JProperty("campo", campo), New JProperty("nome", tabellaDes)))
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
    Public Shared Function Leggi_Categorie_Magazzino() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objCM As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
            Dim dtCategMag = objCM.Leggi(0, "", False, " Elem_Cod NOT IN (1,4,300) ", "", objParametriServer)

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
                If Not objFilters Is Nothing Then
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
    Public Shared Function StampaBarCode(ByVal piva As String,
                                         ByVal Id_Agenda As Integer,
                                         ByVal Lav_Cod As Integer,
                                         ByVal Mat_Cod As String,
                                         ByVal Cal_Cod As String,
                                         ByVal Lotto As String,
                                         ByVal Modulo As Integer,
                                         ByVal Tipo_Accettazione As Integer,
                                         ByVal Dettaglio As Boolean
                                         ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Dim vVarStampe = New List(Of ElementoStampe)
        vVarStampe.Add(New ElementoStampe With {.Nome = "piva", .Valore = piva})
        vVarStampe.Add(New ElementoStampe With {.Nome = "id_agenda", .Valore = Id_Agenda})
        vVarStampe.Add(New ElementoStampe With {.Nome = "printcode", .Valore = Lav_Cod})
        vVarStampe.Add(New ElementoStampe With {.Nome = "printtoprinter", .Valore = 0})
        vVarStampe.Add(New ElementoStampe With {.Nome = "printname", .Valore = ""})
        If Dettaglio Then
            Dim filtroStampa = String.Format("{0},{1},{2}", Mat_Cod, Lotto, Cal_Cod)
            vVarStampe.Add(New ElementoStampe With {.Nome = "attributo_jolly", .Valore = filtroStampa})
        End If

        Dim objVS As New AgronicaCoreXML.XML_Stampe
        Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe.ToArray)
        Dim StrNodiVariabili As String = StrNodo
        Dim Report As Integer = enum_CodificaStampe.FF_Etichette

        If Report <> 0 Then

            Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe With {
                .report = Report,
                .username = CStr(HttpContext.Current.Session("ASG_Utente_Username")),
                .user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
            }
            objAgronicaStampe.Xml_Generico.Length = 0
            objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)

            r.RispostaStringa = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(Enum_SiteRedirector.Sito_AgronicaStampe_2010, objAgronicaStampe)
            r.RispostaOK = True

        Else
            r.RispostaOK = False
            r.Errore = "Tipo di stampa non disponibile"
        End If

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

            Dim trovato As Boolean = False
            Dim modificati As JArray = New JArray()
            Dim nuovi As JArray = New JArray()
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

            If r.RispostaOK = True Then
                r.RispostaStringa = "true"
            Else
                r.RispostaStringa = "Problema nel salvataggio del report"
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

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

            Dim trovato As Boolean = False
            Dim jArrayListaReport As New JArray()
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

            If r.RispostaOK = True Then
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

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiDocumenti(ByVal filtriRicerca As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim userName As String = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
            Dim prgGias As String = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))

            Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim objFiltriRicerca As FiltriRicercaDocumenti = JsonConvert.DeserializeObject(Of FiltriRicercaDocumenti)(filtriRicerca, settingLoc)

            If objFiltriRicerca._dataGiacenza Is Nothing OrElse Not objFiltriRicerca._dataGiacenza.HasValue Then
                objFiltriRicerca._dataGiacenza = Date.Now
            End If

            If objFiltriRicerca._dataRegDal Is Nothing OrElse Not objFiltriRicerca._dataRegDal.HasValue Then
                objFiltriRicerca._dataRegDal = AGRODATAINIZIO
            End If
            If objFiltriRicerca._dataRegAl Is Nothing OrElse Not objFiltriRicerca._dataRegAl.HasValue Then
                objFiltriRicerca._dataRegAl = AGRODATAFINE
            End If
            
            Dim _causali_trasp As String = ""
            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "Report_Vendite_Causali", "", "", objParametriServer)
            If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
                _causali_trasp = DTConfigSiti.Rows(0)("valore").ToString
            End If

            Dim isFreshAndFoodZooTabacco = false
            Dim leggiAnagrafeLog As New OGenerazioni_Anagrafe_Moduli_Log_R
            Dim dtAnagrafeLog = leggiAnagrafeLog.Leggi(objFiltriRicerca.piva, 0, 0, 0, 0, -1, -1, -1, AGRODATAINIZIO, AGRODATAFINE,
                                                           "", "",
                                                           objParametriServer)
                If Not dtAnagrafeLog Is Nothing AndAlso dtAnagrafeLog.Rows.Count > 0 Then
                    For Each rAnagrafeLog In dtAnagrafeLog.Rows
                        If CInt(rAnagrafeLog.Item("Modulo_Generazione")) = enum_Omni_Modulo_Generazione.FreshFood OrElse
                            CInt(rAnagrafeLog.Item("Modulo_Generazione")) = enum_Omni_Modulo_Generazione.Zoo OrElse
                            Cint(rAnagrafeLog.Item("Modulo_Generazione")) = enum_Omni_Modulo_Generazione.Tabacco Then
                                isFreshAndFoodZooTabacco = True
                        End If
                    Next
                End If
            dtAnagrafeLog = Nothing

            Dim ddl_Centri As New DropDownList
            CaricaListControl.Centri_Aziendali(ddl_Centri,
                                               False, "", "",
                                               objFiltriRicerca.piva, False, 2,
                                               "", "", objParametriServer)


            Dim centriAziendaliAmmessi = New List(Of Object)

            For Each i As ListItem In ddl_Centri.Items
                centriAziendaliAmmessi.Add(New With
                                    {
                                        .sa_nome = i.Text,
                                        .sa_cod = i.Value
                                    })
            Next

            Dim causali As String = String.Join(", ", New Integer() {
                LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_DISTINTA_CARICO, LAVCOD_ACCETTAZIONE_DIVERSI, 
                LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE 
            } )

            Dim ricercaDoc As New RicercaDocumenti_R
            Dim dtRigheDDT = ricercaDoc.Cerca_Documenti(
                objFiltriRicerca.type,
                objFiltriRicerca.doc_type,
                objFiltriRicerca.tutteLeCausali,
                centriAziendaliAmmessi,
                objFiltriRicerca.dettaglio,
                objFiltriRicerca.piva,
                objFiltriRicerca.report,
                objFiltriRicerca.cubo,
                objFiltriRicerca._descrizione,
                objFiltriRicerca._docNumeroSin,
                objFiltriRicerca._docNumero,
                objFiltriRicerca._docNumeroDes,
                objFiltriRicerca._nrRiga,
                objFiltriRicerca._dataRegDal.Value.ToString("u"),
                objFiltriRicerca._dataRegAl.Value.ToString("u"),
                objFiltriRicerca._centriAziendali,
                objFiltriRicerca._clienti,
                objFiltriRicerca._agenti,
                causali,
                objFiltriRicerca._specie,
                objFiltriRicerca._varieta,
                objFiltriRicerca._prodotti,
                objFiltriRicerca._categorie,
                objFiltriRicerca._categcommerciali,
                _causali_trasp,
                objFiltriRicerca._soloDDTNonFatturati,
                objFiltriRicerca._soloOrdiniNonSpediti,
                False,
                "",
                objParametriServer,
                objParametriUtenti)


            'NOTA: Il codice seguente serve a calcolare la Qta_Residua in caso di incoerenza dei dati forniti dalla Cerca_Documenti
            'in caso cambiare i nomi di colonna o comunque assicurarsi di crearne un corrispettivo nel javascript
            'dtRigheDDT.Columns.Add(New DataColumn("Qta_Utilizzata_2", GetType(Double)))
            'dtRigheDDT.Columns.Add(New DataColumn("Qta_Residua_2", GetType(Double)))
            'For Each prodottoDDT As DataRow In dtRigheDDT.Rows
            '    'Devo calcolare la giacenza residua del mio movimento
            '    Dim objMovDettRiferimenti As New Mov_Dettagli_Riferimenti_R()
            '    Dim dtMovDettRif = objMovDettRiferimenti.Leggi_Specifica(
            '        Piva:="",
            '        Sa_Cod:=0,
            '        Id_Agenda:=0,
            '        Id_Mov:=0,
            '        Id_Mov_Det:=0,
            '        Lav_Cod:=0,
            '        Cau_Mov:="",
            '        Piva_Rif:=objFiltriRicerca.piva,
            '        Sa_Cod_Rif:=0,
            '        Id_Agenda_Rif:=prodottoDDT("Id_Agenda"),
            '        Id_Mov_Rif:=prodottoDDT("id_mov"),
            '        Id_Mov_Det_Rif:=prodottoDDT("Id_Mov_Det"),
            '        Lav_Cod_Rif:=0,
            '        Cau_Mov_Rif:="",
            '        xFiltroAggiuntivo:="",
            '        xOrderBy:="",
            '        objParametri:=objParametriServer)

            '    Dim qtaUtilizzataDDT As Double = 0
            '    For Each movDettRif As DataRow In dtMovDettRif.Rows
            '        qtaUtilizzataDDT += movDettRif.Field(Of Double)("Qta")
            '    Next

            '    prodottoDDT("Qta_Utilizzata_2") = qtaUtilizzataDDT
            '    Dim qtaOriginariaDDT As Double = prodottoDDT("Qta")
            '    prodottoDDT("Qta_Residua_2") = qtaOriginariaDDT - qtaUtilizzataDDT

            '    'If (qtaOriginariaDDT - qtaUtilizzataDDT) = 0 Then
            '    '    prodottoDDT("inGiacenza") = False
            '    'Else
            '    '    prodottoDDT("inGiacenza") = True
            '    'End If

            'Next

            If objFiltriRicerca.ProdottiInGiacenza Then
                dtRigheDDT.Columns.Add(New DataColumn("chiave_giacenze", GetType(String)))
                dtRigheDDT.Columns.Add(New DataColumn("inGiacenza", GetType(Boolean)))

                Dim fabbricatoKeyDest As String() = objFiltriRicerca._fabbricato.Split("_")
                Dim tipoFabbricato As Integer = 0
                Dim saCod As Integer = 0
                Dim fabbricatoCod As Integer = 0
                If fabbricatoKeyDest.Length = 3 Then
                    tipoFabbricato = fabbricatoKeyDest(0)
                    saCod = fabbricatoKeyDest(1)
                    fabbricatoCod = fabbricatoKeyDest(2)
                End If

                For Each prodottoDDT As DataRow In dtRigheDDT.Rows
                    Dim xFiltroTutti As String = ""
                    Dim strQueryOutput As String = ""
                    Dim Flag_QtaMaggioreZero As Boolean = True

                    Dim elemCod As Integer = prodottoDDT("Elem_Cod")
                    Dim calCodBaseElemCod = 0
                    If elemCod = TRASFORMATI_VEGETALI OrElse elemCod = TRASFORMATI_ANIMALI Then
                        calCodBaseElemCod = prodottoDDT("Cal_Cod")
                    End If

                    Dim lotto As String = prodottoDDT.Field(Of String)("Lotto")
                    'If lotto = "" Then
                    '    xFiltroTutti = " AND (Movimenti_Dettagli.lotto  <> '' and Movimenti_Dettagli.lotto <> 'indefinito')"
                    'End If
                    
                    Dim dtGiacenze As DataTable
                    Dim objGiacenze As New Giacenze_R
                    dtGiacenze = objGiacenze.SchedaGiacenzeMagazzino(
                        Data_Giacenza:= objFiltriRicerca._dataGiacenza.Value,
                        Piva:= objFiltriRicerca.piva,
                        Sa_Cod:= saCod,
                        Id_Destinazione:= fabbricatoCod,
                        Elem_Cod:= elemCod,
                        Pro_Cod:= prodottoDDT.Field(Of Integer)("Pro_Cod"),
                        Mat_Cod:= prodottoDDT.Field(Of Integer)("Mat_Cod"),
                        Cal_Cod:= calCodBaseElemCod,
                        Cod_Progetto:= 0,
                        Fase_Cod:= 0,
                        Udm_Cod:= 0,
                        Lotto:= lotto,
                        Flag_QtaNoZero:= True,
                        xFiltroAggiuntivo:= xFiltroTutti,
                        xFiltroAggiuntivo_Coadiuvanti:= "",
                        xFiltroAggiuntivo_Carburanti:= "",
                        xFiltroAggiuntivo_Fertilizzanti:= "",
                        xFiltroAggiuntivo_Formulati:= "",
                        xFiltroAggiuntivo_InneschiTrappole:= "",
                        xFiltroAggiuntivo_InsettiUtili:= "",
                        xFiltroAggiuntivo_MateriePrime:= "",
                        xFiltroAggiuntivo_Trappole:= "",
                        xFiltroAggiuntivo_Semilavorati:= "",
                        xFiltroAggiuntivo_TrasformatiVeg:= "",
                        xOrderBy:= "",
                        objParametriServer:= objParametriServer,
                        objParametriUtenti:= objParametriUtenti,
                        xFiltroAggiuntivo_13:= "",
                        StrQuery_Output:= strQueryOutput,
                        isFreshAndFood:= isFreshAndFoodZooTabacco AndAlso (elemCod = TRASFORMATI_VEGETALI OrElse elemCod = TRASFORMATI_ANIMALI),
                        cercaLottoPerLike:= False,
                        xFiltroAggiuntivo_14:= "",
                        flagRecuperaCodArticolo:= False,
                        codArticolo:= "",
                        cercaCodArticoloPerLike:= False,
                        Flag_QtaMaggioreZero:= True)

                    If dtGiacenze.Rows.Count = 0 Then
                        prodottoDDT.Item("chiave_giacenze") = ""
                        prodottoDDT.Item("inGiacenza") = False
                    Else
                        Dim rigaGiacenza As DataRow = dtGiacenze.Rows()(0)
                        prodottoDDT.Item("chiave_giacenze") = rigaGiacenza("piva") & "_" &
                            rigaGiacenza("sa_cod") & "_" &
                            rigaGiacenza("Tipo_Destinazione") & "_" &
                            rigaGiacenza("Id_Destinazione") & "_" &
                            rigaGiacenza("cat_cod") & "_" &
                            rigaGiacenza("pro_cod") & "_" &
                            rigaGiacenza("mat_cod") & "_" &
                            rigaGiacenza("Lotto") & "_" &
                            rigaGiacenza("cal_cod") & "_" &
                            rigaGiacenza("cod_progetto") & "_" &
                            rigaGiacenza("udm_cod")
                        
                        prodottoDDT.Item("inGiacenza") = True

                        'Dim qtaGiacenzaProdotto As Double = rigaGiacenza("Giacenza")

                        'Dim listNomiColonne2 As New List(Of String)
                        'For Each columnsDtDDT As DataColumn In dtGiacenze.Columns
                        '    listNomiColonne2.Add(columnsDtDDT.ColumnName & " - " & columnsDtDDT.DataType.ToString)
                        'Next
                        'Dim nomiColonne2 = String.Join(vbCrLf, listNomiColonne2)

                    End If

                Next

                Dim enumerableRigheGiacenzaDDT = (From dt In dtRigheDDT.AsEnumerable
                          Where dt("inGiacenza") = True And dt("Qta_Residua") > 0)

                If enumerableRigheGiacenzaDDT.Count = 0 Then
                    dtRigheDDT.Rows.Clear()
                Else
                    dtRigheDDT = enumerableRigheGiacenzaDDT.CopyToDataTable()
                End If

            End If

            'Dim listNomiColonne As New List(Of String)
            'For Each columnsDtDDT As DataColumn In dtRigheDDT.Columns
            '    listNomiColonne.Add(columnsDtDDT.ColumnName & " - " & columnsDtDDT.DataType.ToString)
            'Next
            'Dim nomiColonne = String.Join(vbCrLf, listNomiColonne)
            
            r.RispostaOK = True
            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dtRigheDDT, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r
    End Function


End Class


Friend Class FiltriRicercaDocumenti
    Public Property type() As String
    Public Property doc_type() As String
    Public Property tutteLeCausali() As String
    Public Property dettaglio() As Boolean
    Public Property piva() As String
    Public Property ProdottiInGiacenza As Boolean
    Public Property report() As String
    Public Property cubo() As Boolean
    Public Property _descrizione() As String
    Public Property _docNumeroSin() As String
    Public Property _docNumero() As Integer
    Public Property _docNumeroDes() As String
    Public Property _nrRiga() As String
    Public Property _dataRegDal() As Date?
    Public Property _dataRegAl() As Date?
    Public Property _centriAziendali() As String
    Public Property _fabbricato() As String
    Public Property _clienti() As String
    Public Property _agenti() As String
    Public Property _causali() As String
    Public Property _specie() As String
    Public Property _varieta() As String
    Public Property _prodotti() As String
    Public Property _lotto() As String
    Public Property _dataGiacenza() As Date?
    Public Property _categorie() As String
    Public Property _categcommerciali() As String
    Public Property _soloDDTNonFatturati() As Boolean
    Public Property _soloOrdiniNonSpediti() As Boolean
    'Public Property objP_server() As String
End Class