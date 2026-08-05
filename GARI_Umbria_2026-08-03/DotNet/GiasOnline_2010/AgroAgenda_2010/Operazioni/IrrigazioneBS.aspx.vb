
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreGestioneRichieste

Public Class IrrigazioneBS

    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Shared objParametriAgenda As New ParametriAgenda

    Public objParametriAgenda_VisualizzaSoloBottoneSalvaEsci As String

    Public Shared DSSIrrigazione_Autorizzato As Boolean = False

    Private Sub verificoCredenzialiDiAccesso()
        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If
    End Sub

    Private Sub inizializzoObjParametri()

        objParametriAgenda_VisualizzaSoloBottoneSalvaEsci = "false"
        If objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Trattamenti Then
            objParametriAgenda_VisualizzaSoloBottoneSalvaEsci = "true"
        End If

        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'TODO: Importante svuoto le Note dell'objParametriAgenda,
        'quando avrò finito la gestione delle ricette di irrigazione e sarà
        'funzionante dobbiamo aggiungere questa parte anche alla Trattamenti_2!
        objParametriAgenda.Note = New List(Of Nota)

        verificoCredenzialiDiAccesso()
        inizializzoObjParametri()
        defaultSuperuser()
        inizializzoParametriPagina()

        Dim TipoOperazioneAgenda As enum_Tipo_Operazione_Agenda
        Dim Qs_Operazione_Ricetta As String
        Dim Qs_Ricetta_Cod As String

        If Not IsNothing(Request.QueryString("operazione_ricetta")) Then
            Qs_Operazione_Ricetta = Stringa_Decodifica(Request.QueryString("operazione_ricetta").ToString, AgroKey_EncoderDecoder, Nothing)
            hf_Operazione_Ricetta.Value = Stringa_Decodifica(Request.QueryString("operazione_ricetta").ToString, AgroKey_EncoderDecoder, Nothing)
        Else
            Qs_Operazione_Ricetta = ""
            hf_Operazione_Ricetta.Value = ""
        End If

        If Not IsNothing(Request.QueryString("r")) Then
            Qs_Ricetta_Cod = Stringa_Decodifica(Request.QueryString("r").ToString, AgroKey_EncoderDecoder, Nothing)
            hf_Ricetta_Cod.Value = Stringa_Decodifica(Request.QueryString("r").ToString, AgroKey_EncoderDecoder, Nothing)
        Else
            Qs_Ricetta_Cod = "0"
            hf_Ricetta_Cod.Value = "0"
        End If

        If Not IsPostBack Then

            Session("UtilizzataRicetta") = False
            Session("PrimaVolta") = True

            VerificaPermessi()


            'controlo il permesso sulla specie, se non ce l'ho metto operazione in lettura
            If objParametriAgenda.Tipo_Operazione = CStr(TipiEnumerativi.enum_TipoOperazioneDB.Modifica) AndAlso
                objParametriAgenda.Veg_Cod.Split("/")(0) <> "0" AndAlso objParametriAgenda.Veg_Cod.Split("/")(0) <> "-1" Then
                Try
                    Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                    Dim Dt As DataTable = objSpecVeg.SpecieVegetali_GestioneFiltroUtente_Leggi(objParametriAgenda.Veg_Cod.Split("/")(0),
                                                                     0,
                                                                     "",
                                                                     "",
                                                                     "",
                                                                     "",
                                                                     objParametri_Utenti)
                    If Dt.Rows.Count = 0 Then
                        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
                    End If
                Catch ex As Exception
                    objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
                End Try
            End If

            'Se lo sportello è aperto non posso salvare le modifiche alla Irrigazione
            Dim dataMin As Date = AGRODATAINIZIO
            Dim dataMax As Date = AGRODATAFINE
            Dim SportelloAperto As Boolean = True
            Dim MsgSportello As String = String.Empty

            If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura OrElse objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
                Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R
                objPratiche.Data_Sportello_Da_Servizio(objParametriAgenda.Piva, enum_Servizi.Quaderno_Campagna_Caa, DateTime.Now, SportelloAperto, dataMin, dataMax, objParametri_Server, objParametri_Utenti)

                objPratiche.Sportello_ChiamataSecondaria_SeNessunCambiamento(objParametriAgenda.Piva,
                                                                                        enum_Servizi.QuadernoCampagnaBio,
                                                                                        DateTime.Now,
                                                                                        True,
                                                                                        AGRODATAINIZIO,
                                                                                        AGRODATAFINE,
                                                                                        SportelloAperto,
                                                                                        dataMin,
                                                                                        dataMax,
                                                                                        objParametri_Server,
                                                                                        objParametri_Utenti)
                If Not SportelloAperto Then
                    If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
                        MsgSportello = " Non è possibile inserire nessuna operazione, Sportello chiuso. "
                    End If
                End If

                If objParametriAgenda.Data < dataMin Then
                    objParametriAgenda.Data = dataMin
                End If

                If objParametriAgenda.Data > dataMax Then
                    objParametriAgenda.Data = dataMax
                End If
            End If

            TipoOperazioneAgenda = objParametriAgenda.TipoOperazioneAgenda

            'In base al tipo di Operazione Agenda vado ad impostare la pagina
            Dim strtipoOperaz As String = ""
            Select Case CInt(objParametriAgenda.Tipo_Operazione)
                Case enum_TipoOperazioneDB.Lettura
                    strtipoOperaz = "Info Irrigazione"
                Case enum_TipoOperazioneDB.Modifica
                    strtipoOperaz = "Modifica Irrigazione"
                Case enum_TipoOperazioneDB.Scrittura
                    strtipoOperaz = "Nuova Irrigazione"
            End Select

            CType(Page.Master, AgendaBootstrap).flag_MostraBtnIndietro = True

            CType(Page.Master, AgendaBootstrap).Lbl_Titolo.Text = strtipoOperaz

            If TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta Then
                CType(Page.Master, AgendaBootstrap).Lbl_Titolo.Text = Resources.AgronicaAgenda_2010.RicettaOdL & " - " & CType(Page.Master, AgendaBootstrap).Lbl_Titolo.Text
            End If

            If TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio Then
                CType(Page.Master, AgendaBootstrap).Lbl_Titolo.Text = Resources.AgronicaAgenda_2010.Brogliaccio & " - " & CType(Page.Master, AgendaBootstrap).Lbl_Titolo.Text
            End If


            Dim supImp As Decimal = 0

            'Note Intervento sono le Note della Tab
            Dim NoteIntervento As String = ""

            Dim irrigazioni As New List(Of Object)

            'Carico i dati che dovranno essre impostati sui controlli
            Select Case objParametriAgenda.Tipo_Operazione

                Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                    objParametriAgenda.Veg_Cod = "-1"
                Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica,
                    TipiEnumerativi.enum_TipoOperazioneDB.Lettura

                    If TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna Then
                        irrigazioni = RipristinaControlliDaAgenda(objParametriAgenda, supImp, NoteIntervento, objParametri_Server)
                    End If

            End Select



            If Qs_Operazione_Ricetta <> "" AndAlso Qs_Ricetta_Cod <> "0" Then

                Session("ricetta_cod") = Qs_Ricetta_Cod
                Session("Ricetta_Operazione_Cod") = Qs_Operazione_Ricetta
                Session("Ricetta_Tipo") = enum_TipoRicetta.Standard_Destinazioni

                'Per le ricette prendi spunto dalla funzione Ripristina_Dati_nei_Controlli_xRicetta nella Trattamenti_2
                ' e fallo con la stessa procedura di RipristinaControlliDaAgenda
                irrigazioni = Ripristina_Dati_nei_ControlliDaRicetta(objParametriAgenda, supImp,
                                                                             NoteIntervento, objParametri_Server)
            End If


            hf_KendoGrid_Impianti_Irrigazione.Value = ""

            hf_ElencoColonneKendoGrid_Impianti_Irrigazione.Value = ""

            hf_KendoGrid_RilieviPioggie_Irrigazione.Value = ""

            hf_Piva.Value = objParametriAgenda.Piva

            hf_TipoOperazione.Value = objParametriAgenda.Tipo_Operazione

            'Salvo i dati ottenuti per caricate i controlli nell'HiddenField

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            Dim ParamAgenda As New JObject(New JProperty("objParametriAgenda", JsonConvert.SerializeObject(objParametriAgenda, Formatting.None, serializerSettings)),
                                           New JProperty("Sup_Selezionata", supImp),
                                           New JProperty("NoteIntervento", NoteIntervento),
                                           New JProperty("irrigazioni", JsonConvert.SerializeObject(irrigazioni, Formatting.None, serializerSettings)),
                                           New JProperty("MsgSportello", MsgSportello))

            hf_ParamAgenda.Value = JsonConvert.SerializeObject(ParamAgenda, Formatting.None, serializerSettings)
        End If
    End Sub

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim objParametriAgenda As New ParametriAgenda

        Dim TargetUrl As String = ""

        Try
            Dim sitoorigine As Enum_SiteRedirector = objParametriAgenda.SitoOrigine
            Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

            If sitoorigine = Enum_SiteRedirector.GiasNG Then
                MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                                      Enum_SiteRedirector.GiasNG,
                                                                                      objParametriAgenda.PaginaSitoOrigine,
                                                                                      TargetUrl,
                                                                                      objParametri_Server)
            Else

                TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
            End If

        Catch ex As Exception
            TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
        End Try

        Response.Redirect(TargetUrl)

    End Sub

    Private Sub IrrigazioneBS_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        CType(Me.Master, AgendaBootstrap).flag_MostraBtnIndietro = True

    End Sub

    Private Sub inizializzoParametriAgenda()
        objParametriAgenda = New ParametriAgenda
        objParametriAgenda.OperazioneMulticentro = True
    End Sub


    Private Sub inizializzoParametriPagina()

        inizializzoParametriAgenda()

        Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        Dim Lav_Des As String = objOperazioniLeggi.LavorazioneDes_from_LavorazioneCod(objParametriAgenda.Lav_Cod, objParametri_Server)
        objParametriAgenda.Lav_Des = Lav_Des

        Select Case CInt(objParametriAgenda.Lav_Cod)

            Case LAVCOD_IRRIGAZIONE
                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.LAVORAZIONE)


        End Select
    End Sub

    Private Sub VerificaPermessi()
        Dim UtenteAbilitato_Lettura As Boolean = False
        Dim UtenteAbilitato_Modifica As Boolean = False
        Dim objUtility As New AgronicaCoreModello.Utility_Operazioni

        objUtility.Verifica_Permessi_OperazioniAgenda_X_PagineAgronicaAgenda(objParametri_Server, objParametri_Utenti, UtenteAbilitato_Lettura, UtenteAbilitato_Modifica)

        If Not UtenteAbilitato_Lettura Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
            Exit Sub
        End If

        If objParametriAgenda.Tipo_Operazione <> enum_TipoOperazioneDB.Lettura AndAlso Not UtenteAbilitato_Modifica Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
            Exit Sub
        End If

        hf_UtenteAbilitatoLettura.Value = UtenteAbilitato_Lettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitato_Modifica

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        DSSIrrigazione_Autorizzato = ObjUtenti.Controlla_Permessi_Utente(
            objParametri_Utenti.UtenteUsername,
            HttpContext.Current.Session("ASG_IdServizio"),
            enum_Security_Attivita.DSS_Irrigazione,
            enum_Security_Operazione.Modifica,
            Date.Now, "", objParametri_Utenti)

    End Sub

    Private Sub defaultSuperuser()

        Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI") = False
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Dt_Impostazioni_Super As New DataTable
        Dt_Impostazioni_Super = ObjUtenti.Leggi(0,
                                      2,
                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "",
                                    "",
                                    objParametri_Utenti)

        For i = 0 To Dt_Impostazioni_Super.Rows.Count - 1
            Select Case CInt(Dt_Impostazioni_Super.Rows(i).Item("Impostazione_Cod"))
                Case enum_Impostazioni_Utenti.SUPERUSER_COD_PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI
                    Select Case Dt_Impostazioni_Super.Rows(i).Item("Impostazione_Valore_1")
                        Case "0"
                            Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI") = False
                        Case "1"
                            Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI") = True
                        Case Else
                            Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI") = False
                    End Select
            End Select
        Next
    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_CentriAziendali(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim ddl_CentroAziendale As New DropDownList

            AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(ddl_CentroAziendale,
                                                                     True,
                                                                     "Tutti i centri aziendali", "0",
                                                                    piva,
                                                                    True,
                                                                    2,
                                                                    "", "",
                                                                    objParametri_Server)


            Dim JArrayListaOp As New JArray()

            For Each i As ListItem In ddl_CentroAziendale.Items
                JArrayListaOp.Add(New JObject(New JProperty("Sa_Nome", i.Text), New JProperty("Sa_Cod", i.Value)))
            Next

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None, serializerSettings)

            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_TipoIrrigazione(ByVal tipo_ddl_irr As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim ddl_TipoIrrigazione As New DropDownList
            Dim JArrayListaOp As New JArray()

            'In base alla Dropdown che devo caricare aggiungo diverse righe
            If tipo_ddl_irr = "normale" Then

                AgronicaCoreUtility.CaricaListControl.CaricaCombo_ImpIrrigazione(ddl_TipoIrrigazione, True, "", "-2", -1,
                                                                                 "", "", "", objParametri_Server)

                For Each i As ListItem In ddl_TipoIrrigazione.Items
                    JArrayListaOp.Add(New JObject(New JProperty("Imp_DES", i.Text), New JProperty("Imp_COD", i.Value)))
                Next

                JArrayListaOp.Add(New JObject(New JProperty("Imp_DES", "NON IMPOSTATO"), New JProperty("Imp_COD", -1)))

            ElseIf tipo_ddl_irr = "grid_impianti" Then
                AgronicaCoreUtility.CaricaListControl.CaricaCombo_ImpIrrigazione(ddl_TipoIrrigazione,
                                                             True, "-Quella Dell'impianto-",
                                                             "-1", -1, "", "", "", objParametri_Server)

                For Each i As ListItem In ddl_TipoIrrigazione.Items
                    JArrayListaOp.Add(New JObject(New JProperty("ModifTipoIrriUtilizzata_Des", i.Text), New JProperty("ModifTipoIrriUtilizzata_Cod", i.Value)))
                Next

            ElseIf tipo_ddl_irr = "cambia_valore_grid_impianti" Then
                AgronicaCoreUtility.CaricaListControl.CaricaCombo_ImpIrrigazione(ddl_TipoIrrigazione, True, "", "-2", -1,
                                                                                 "", "", "", objParametri_Server)



                For Each i As ListItem In ddl_TipoIrrigazione.Items
                    JArrayListaOp.Add(New JObject(New JProperty("ModifTipoIrriUtilizzata_Des", i.Text), New JProperty("ModifTipoIrriUtilizzata_Cod", i.Value)))
                Next

                JArrayListaOp.Insert(1, New JObject(New JProperty("ModifTipoIrriUtilizzata_Des", "-Quella Dell'impianto-"), New JProperty("ModifTipoIrriUtilizzata_Cod", "-1")))

            End If

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Newtonsoft.Json.Formatting.None)

            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Specie(ByVal piva As String,
                                        ByVal sa_cod As Integer,
                                        ByVal data As String,
                                        ByVal tipo_operazione As Integer,
                                        ByVal veg_cod As Integer,
                                        ByVal id_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try

            If objParametri_Server Is Nothing AndAlso
                HttpContext.Current.Session IsNot Nothing AndAlso
                HttpContext.Current.Session("ASG_objParametri_Server") IsNot Nothing Then
                objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
            End If

            If objParametri_Utenti Is Nothing AndAlso
                HttpContext.Current.Session IsNot Nothing AndAlso
                HttpContext.Current.Session("ASG_objParametri_Utenti") IsNot Nothing Then
                objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
            End If

            Dim leggiAncheBloccati As Boolean = False
            If Not IsNothing(HttpContext.Current.Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI")) Then
                If HttpContext.Current.Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI") = True Then
                    leggiAncheBloccati = True
                End If
            End If

            Dim Data_Impostata = DateTime.ParseExact(data.ToString, "yyyyMMdd", Nothing)
            Dim ConsideraTerrenoNudo As Boolean = True
            Dim DettagliTerrenoNudo As Boolean = True
            Dim PrimaRiga_Flag As Boolean = True
            Dim PrimaRiga_Value As String = "-1"
            Dim PrimaRiga_Text As String = ""
            Dim Veg_Cod_ModificaLettura As Integer = 0
            Dim Usa_Filtro_Utente As Boolean = True
            Select Case tipo_operazione
                Case enum_TipoOperazioneDB.Lettura, enum_TipoOperazioneDB.Modifica
                    'se sono in modifica o lettura imposto la Veg_Cod_ModificaLettura in modo che la specie sia comunque inserita nella combo anche se filtrata
                    Veg_Cod_ModificaLettura = veg_cod
                Case enum_TipoOperazioneDB.Scrittura
                    Veg_Cod_ModificaLettura = 0
            End Select

            Dim ddl_Specie As New DropDownList

            Dim FiltroCampi As String = ""

            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.SpecieVegetale_Coltivate_Optimize(ddl_Specie,
                                                  piva,
                                                  sa_cod,
                                                  Data_Impostata,
                                                  ConsideraTerrenoNudo,
                                                  PrimaRiga_Flag,
                                                  PrimaRiga_Text,
                                                  PrimaRiga_Value,
                                                  0,
                                                  "",
                                                  Usa_Filtro_Utente,
                                                  0,
                                                  Veg_Cod_ModificaLettura, 0, DettagliTerrenoNudo,
                                                  "",
                                                  "",
                                                  objParametri_Server, objParametri_Utenti,
                                                  leggiAncheBloccati,
                                                  0, FiltroCampi)


            Dim JArrayListaOp As New JArray()

            For Each i As ListItem In ddl_Specie.Items
                JArrayListaOp.Add(New JObject(New JProperty("Veg_Des", i.Text), New JProperty("Veg_Cod", i.Value)))
            Next

            r.RispostaOK = True

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Newtonsoft.Json.Formatting.None)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Udm() As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim udmArea As String = AgronicaCoreMetaSchemaDAL.UDM_Helper.GetUdmSim_AREA(objParametri_Server, objParametri_Utenti)

            Dim JArrayListaOp As New JArray()

            JArrayListaOp.Add(New JObject(New JProperty("Udm_Des", "millimetri"), New JProperty("Udm_Cod", 18)))
            JArrayListaOp.Add(New JObject(New JProperty("Udm_Des", "m3/" & udmArea), New JProperty("Udm_Cod", 90)))

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Newtonsoft.Json.Formatting.None)

            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Controlla_Sportello(ByVal data As String,
                                               ByVal data_agenda As String,
                                               ByVal tipo_operazione As Integer,
                                               ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim Data_Impostata = DateTime.ParseExact(data.ToString, "yyyyMMdd", Nothing)
            Dim objParametriAgenda_Data = DateTime.ParseExact(data_agenda.ToString, "yyyyMMdd", Nothing)
            Dim modificateDate As Boolean = False
            Dim messsaggio_errore As String = String.Empty
            Dim dataMin As Date = AGRODATAINIZIO
            Dim dataMax As Date = AGRODATAFINE
            Dim SportelloAperto As Boolean = True

            If tipo_operazione = enum_TipoOperazioneDB.Scrittura Then
                Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R
                objPratiche.Data_Sportello_Da_Servizio(piva, enum_Servizi.Quaderno_Campagna_Caa, DateTime.Now, SportelloAperto, dataMin, dataMax, objParametri_Server, objParametri_Utenti)

                objPratiche.Sportello_ChiamataSecondaria_SeNessunCambiamento(piva,
                                                                                        enum_Servizi.QuadernoCampagnaBio,
                                                                                        DateTime.Now,
                                                                                        True,
                                                                                        AGRODATAINIZIO,
                                                                                        AGRODATAFINE,
                                                                                        SportelloAperto,
                                                                                        dataMin,
                                                                                        dataMax,
                                                                                        objParametri_Server,
                                                                                        objParametri_Utenti)
                If Data_Impostata < dataMin Then
                    Data_Impostata = dataMin.ToShortDateString
                    messsaggio_errore = "Non è possibile inserire un'operazione prima del " & dataMin.ToShortDateString
                    modificateDate = True
                End If

                If Data_Impostata > dataMax Then
                    Data_Impostata = dataMax.ToShortDateString
                    messsaggio_errore = "Non è possibile inserire un'operazione successiva a " & dataMax.ToShortDateString
                    modificateDate = True
                End If
            End If

            r.RispostaOK = True

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            Dim risp As New JObject(New JProperty("Data_Irrigazione", Data_Impostata.ToString),
                                           New JProperty("modificateDate", modificateDate),
                                           New JProperty("messsaggio_errore", messsaggio_errore))

            r.RispostaStringa = JsonConvert.SerializeObject(risp, Formatting.None, serializerSettings)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function GestioneTabDivNote(ByVal data As String,
                                              ByVal piva As String, ByVal veg_cod As Integer, ByVal id_cod As Integer,
                                              ByVal lav_cod As Integer, ByVal tipooperazioneagenda As Integer,
                                              ByVal objParametriAgendaNote As String, ByVal tipooperazione As Integer
                                              ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Dim Irrigazione_BiZ As New AgronicaCoreModello.Irrigazione_BIZ

        r = Irrigazione_BiZ.Carica_NoteIrrigazione(data, piva, veg_cod, lav_cod, tipooperazioneagenda,
                                                    objParametriAgendaNote, tipooperazione, objParametri_Server)


        Return r
    End Function

    Private Function Ripristina_Dati_nei_ControlliDaRicetta(ByRef objParametriAgenda As ParametriAgenda,
                                                             ByRef supImp As Decimal,
                                                             ByRef NoteIntervento As String,
                                                             ByVal objParametri_Server As AgronicaCoreParametri) As List(Of Object)

        Dim Irrigazioni As New List(Of Object)

        Session("UtilizzataRicetta") = False

        Dim ricetta_cod As String = Session("ricetta_cod")
        Dim Ricetta_Operazione_Cod As String = Session("Ricetta_Operazione_Cod")
        Dim Ricetta_Tipo As String = Session("Ricetta_Tipo")

        objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

        Dim objCOM As New AgronicaCoreContabBIZ.Ricette_Operazioni_R
        Dim strXMLOperazione As String = objCOM.Ricetta_Operazioni_Leggi(CInt(ricetta_cod),
                                                     CInt(Ricetta_Operazione_Cod),
                                                     0,
                                                     0,
                                                     AGRODATAINIZIO, AGRODATAFINE,
                                                     False,
                                                     objParametri_Server)

        If strXMLOperazione <> "" Then

            Dim XmlDoc As New System.Xml.XmlDocument
            XmlDoc.LoadXml(strXMLOperazione)

            Dim XML_DatiOperazioni As System.Xml.XmlElement = XmlDoc.SelectSingleNode("DatiRicetta_Operazioni")

            If XML_DatiOperazioni IsNot Nothing Then

                Dim XMLs_Operazione As System.Xml.XmlNodeList = XML_DatiOperazioni.GetElementsByTagName("Ricetta_Operazione")

                For i = 0 To XMLs_Operazione.Count - 1

                    Dim XML_Operazione As System.Xml.XmlElement = XMLs_Operazione.Item(i)
                    Ripristina_Dati_nei_Controlli_xRicetta(XML_Operazione.OuterXml,
                                                            Irrigazioni, objParametriAgenda,
                                                            supImp, NoteIntervento, objParametri_Server)

                    Session("UtilizzataRicetta") = True
                Next

            End If

        End If

        Return Irrigazioni

    End Function

    Private Sub Ripristina_Dati_nei_Controlli_xRicetta(ByVal Xml_Operazione As String,
                                                       ByRef irrigazioni As List(Of Object),
                                                       ByRef objParametriAgenda As ParametriAgenda,
                                                       ByRef supImp As Decimal,
                                                       ByRef NoteIntervento As String,
                                                       ByVal objParametri_Server As AgronicaCoreParametri)

        '------------------------------------------
        '----- Recupero le informazioni
        '------------------------------------------
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_DatiRicetta_Dettagli As System.Xml.XmlElement
        Dim XML_Ricetta_Operazione As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio As System.Xml.XmlNodeList
        Dim XML_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlNodeList
        Dim xDatiRicettaxNote As System.Xml.XmlElement
        Dim xListaRicettaxNote As System.Xml.XmlNodeList
        Dim xRicettaxNote As System.Xml.XmlElement
        Dim XML_RicettaDestinazione As System.Xml.XmlElement
        Dim XMLs_RicettaDestinazione As System.Xml.XmlNodeList

        Dim Cau_Mov As String
        Dim Elem_Cod As Integer
        Dim Pro_Cod As Integer
        Dim Mat_Cod As Integer
        Dim Lotto As String
        Dim Sup_Tot As Decimal = 0

        Dim Movimento As Movimento

        Dim Ricetta_Dettaglio_Cod As Integer

        XmlDoc.LoadXml(Xml_Operazione)

        XML_Ricetta_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")

        ' ----- NOTE

        NoteIntervento = CStr(XML_Ricetta_Operazione.GetAttribute("note"))

        Dim Nota As Nota

        xDatiRicettaxNote = XML_Ricetta_Operazione.SelectSingleNode("DatiRicettaxNote_2")
        If xDatiRicettaxNote IsNot Nothing Then
            xListaRicettaxNote = xDatiRicettaxNote.GetElementsByTagName("RicettaxNote_2")
            If xListaRicettaxNote IsNot Nothing Then
                For i = 0 To xListaRicettaxNote.Count - 1
                    xRicettaxNote = xListaRicettaxNote.Item(i)
                    Nota = New Nota
                    Nota.Nota_Cod = CInt(xRicettaxNote.GetAttribute("nota_cod"))
                    objParametriAgenda.Note.Add(Nota)
                Next
            End If
        End If
        objParametriAgenda.salva()

        '-MOVIMENTI
        Dim MovimentiCosti As List(Of Movimento) = New List(Of Movimento)

        XML_DatiRicetta_Dettagli = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli")

        If XML_DatiRicetta_Dettagli IsNot Nothing Then

            XMLs_Ricetta_Dettaglio = XML_DatiRicetta_Dettagli.GetElementsByTagName("Ricetta_Dettaglio")

            For i = 0 To XMLs_Ricetta_Dettaglio.Count - 1

                XML_Ricetta_Dettaglio = XMLs_Ricetta_Dettaglio.Item(i)

                Cau_Mov = XML_Ricetta_Dettaglio.GetAttribute("cau_mov")

                Ricetta_Dettaglio_Cod = XML_Ricetta_Dettaglio.GetAttribute("ricetta_dettaglio_cod")
                Elem_Cod = XML_Ricetta_Dettaglio.GetAttribute("elem_cod")
                Pro_Cod = XML_Ricetta_Dettaglio.GetAttribute("pro_cod")
                Mat_Cod = XML_Ricetta_Dettaglio.GetAttribute("mat_cod")

                Lotto = XML_Ricetta_Dettaglio.GetAttribute("lotto")

                Dim Mezzo_det As Integer = XML_Ricetta_Dettaglio.GetAttribute("mezzo_det")

                Movimento = New Movimento

                Movimento.Piva = objParametriAgenda.Piva
                Movimento.Sa_Cod = objParametriAgenda.Sa_Cod
                Movimento.Data = objParametriAgenda.Data
                Movimento.Lav_Cod = objParametriAgenda.Lav_Cod
                Movimento.Cau_Mov = Cau_Mov

                If Cau_Mov = CAU_LAVORAZIONE Then

                    XMLs_RicettaDestinazione = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Destinazione")

                    Dim idnudo As Integer = 0
                    Dim idnudoold As Integer = 0
                    Dim cambiatonudo As Boolean = False

                    'HO DESTINAZIONI (ricetta aziendale)
                    If XMLs_RicettaDestinazione IsNot Nothing AndAlso XMLs_RicettaDestinazione.Count > 0 Then

                        For j = 0 To XMLs_RicettaDestinazione.Count - 1

                            XML_RicettaDestinazione = XMLs_RicettaDestinazione.Item(j)

                            Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

                            objAppezzamento.Piva = CStr(XML_RicettaDestinazione.GetAttribute("piva"))
                            objAppezzamento.Sa_Cod = CInt(XML_RicettaDestinazione.GetAttribute("sa_cod"))
                            objAppezzamento.Appezza = CInt(XML_RicettaDestinazione.GetAttribute("appezza"))
                            objAppezzamento.ID_Reg = CInt(XML_RicettaDestinazione.GetAttribute("id_reg"))

                            Select Case CInt(objParametriAgenda.Lav_Cod)
                                Case LAVCOD_IRRIGAZIONE
                                    'QuaTot = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta
                                    If CDec(XML_RicettaDestinazione.GetAttribute("qta2")) > 0 Then
                                        objAppezzamento.Qta2 = CDec(XML_RicettaDestinazione.GetAttribute("qta2"))
                                    Else
                                        objAppezzamento.Qta2 = (New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read()).LeggiSuperficie(objAppezzamento.Piva, objAppezzamento.Sa_Cod, objAppezzamento.Appezza, objAppezzamento.ID_Reg, objParametri_Server)
                                    End If

                                Case Else
                                    Throw New NotImplementedException
                            End Select


                            Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                            Dim objPl As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

                            Dim Dt_Imp As New DataTable
                            If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
                                Dt_Imp = objImp.Leggi(CStr(XML_RicettaDestinazione.GetAttribute("piva")),
                                            CInt(XML_RicettaDestinazione.GetAttribute("sa_cod")),
                                            CInt(XML_RicettaDestinazione.GetAttribute("appezza")),
                                            CInt(XML_RicettaDestinazione.GetAttribute("id_reg")),
                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                            "", "", objParametri_Server)

                                If Dt_Imp.Rows.Count > 0 Then
                                    objParametriAgenda.Veg_Cod = Dt_Imp.Rows(0).Item("veg_cod").ToString()
                                End If

                            Else
                                Dt_Imp = objPl.Leggi_solo_Programmazione_entita(objParametriAgenda.Programmazione_Cod, objAppezzamento.Programmazione_Entita_cod, objParametri_Server)
                            End If

                            'If j = 0 Then
                            '    objParametriAgenda.Veg_Cod = CInt(Dt_Imp.Rows(0).Item("veg_cod"))
                            'End If

                            'per operazione su terreno nudo,
                            'se sempre la solita destinazione la setto nel veg_cod, altrimenti lascio 0
                            If CInt(objParametriAgenda.Veg_Cod) = 0 AndAlso CInt(Dt_Imp.Rows(0).Item("id_cod")) <> 0 Then
                                If j = 0 Then
                                    idnudo = CInt(Dt_Imp.Rows(0).Item("id_cod"))
                                    idnudoold = idnudo
                                Else
                                    idnudo = CInt(Dt_Imp.Rows(0).Item("id_cod"))
                                    If idnudoold <> idnudo Then
                                        cambiatonudo = True
                                    End If
                                End If
                                If j = XMLs_RicettaDestinazione.Count - 1 Then
                                    If cambiatonudo Then
                                        objParametriAgenda.Veg_Cod = "0"
                                    Else
                                        objParametriAgenda.Veg_Cod = "0/" & idnudo
                                    End If
                                End If
                            End If

                            Dim x_sup_imp As String
                            If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
                                x_sup_imp = "sup_imp"
                            Else
                                x_sup_imp = "superficie"
                            End If


                            If Dt_Imp.Rows.Count > 0 Then
                                Sup_Tot += Dt_Imp.Rows(0).Item(x_sup_imp)
                            End If

                            objParametriAgenda.Impianti.Add(objAppezzamento)


                            If Cau_Mov = CAU_LAVORAZIONE Then
                                objParametriAgenda.Sa_Cod = CInt(XML_RicettaDestinazione.GetAttribute("sa_cod"))
                            End If

                            objParametriAgenda.salva()

                            XMLs_Ricetta_Dettaglio_Tecnico_2 = XML_Ricetta_Operazione.GetElementsByTagName("Ricetta_Dettaglio_Tecnico_2")

                            If XMLs_Ricetta_Dettaglio_Tecnico_2 IsNot Nothing AndAlso XMLs_Ricetta_Dettaglio_Tecnico_2.Count > 0 Then

                                '------------------------------
                                '----- MOVIMENTO DET TECNICO --
                                '------------------------------
                                Dim dose As Decimal = 0
                                Dim ore As Decimal = 0
                                Dim portata As Integer = 0
                                Dim QuaTot As Decimal = 0
                                Dim Data_i As Date = AGRODATAINIZIO
                                Dim Data_f As Date = AGRODATAFINE
                                Dim frequenza As Integer = 0
                                Dim TipoIrrigaz As Integer = 0
                                Dim UDM As Integer = 0
                                Dim ID_DSS_Irrigazione As Integer = 0
                                Dim Qta_Acqua_Custom As Decimal = 0
                                Dim Data_Custom As Date = AGRODATAINIZIO

                                For x = 0 To XMLs_Ricetta_Dettaglio_Tecnico_2.Count - 1

                                    XML_Ricetta_Dettaglio_Tecnico_2 = XMLs_Ricetta_Dettaglio_Tecnico_2.Item(x)

                                    If Ricetta_Dettaglio_Cod = CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("ricetta_dettaglio_cod")) Then

                                        Select Case CInt(objParametriAgenda.Lav_Cod)
                                            Case LAVCOD_IRRIGAZIONE
                                                dose = CDec(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("qta_ril"))
                                                ore = CDec(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("dose"))
                                                portata = CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("parziale"))
                                                Data_i = CDate(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("inn1_data"))
                                                Data_f = CDate(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("inn2_data"))
                                                frequenza = CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("nitrati"))
                                                TipoIrrigaz = CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("freatimetro"))
                                                UDM = CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("dett_cod"))
                                                QuaTot = CDec(XML_RicettaDestinazione.GetAttribute("qta"))
                                            Case Else
                                                Throw New NotImplementedException
                                        End Select

                                        Dim qta_2 As Decimal = 0

                                        If objAppezzamento.Qta2 <> 0 Then
                                            qta_2 = objAppezzamento.Qta2
                                        Else
                                            qta_2 = (New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read()).LeggiSuperficie(objAppezzamento.Piva, objAppezzamento.Sa_Cod, objAppezzamento.Appezza, objAppezzamento.ID_Reg, objParametri_Server)
                                        End If

                                        irrigazioni.Add(New With
                                                                {
                                                                    .ImpiantoIrrigato = objAppezzamento.Piva.ToString & "-" & objAppezzamento.Sa_Cod.ToString & "-" & objAppezzamento.Appezza.ToString & "-" & objAppezzamento.ID_Reg.ToString,
                                                                    .Dose = dose,
                                                                    .Ore = ore,
                                                                    .Portata = portata,
                                                                    .Data_Inizio = Data_i,
                                                                    .Data_Fine = Data_f,
                                                                    .Frequenza = frequenza,
                                                                    .Qta_Totale = QuaTot,
                                                                    .UDM_Dose = UDM,
                                                                    .TipoIrrigazioneUtilizzata = TipoIrrigaz,
                                                                    .Qta2 = qta_2,
                                                                    .ID_DSS_Irrigazione = ID_DSS_Irrigazione,
                                                                    .Qta_Acqua_Custom = Qta_Acqua_Custom,
                                                                    .Data_Custom = Data_Custom
                                                                })
                                    End If

                                Next

                            End If

                            objParametriAgenda.salva()
                        Next


                        supImp = Sup_Tot

                    Else

                    End If

                End If

            Next


        End If

        objParametriAgenda.Movimenti = MovimentiCosti
    End Sub

    Private Function RipristinaControlliDaAgenda(ByRef objParametriAgenda As ParametriAgenda,
                                                 ByRef supImp As Decimal,
                                                 ByRef NoteIntervento As String,
                                                 ByVal objParametri_Server As AgronicaCoreParametri) As List(Of Object)

        Dim objAgenda As New Agenda_Operazione_Helper
        Dim Agenda As New Operazione_Agenda
        Dim Irrigazioni As New List(Of Object)

        Agenda = objAgenda.Leggi(objParametriAgenda.Piva,
                                     CInt(objParametriAgenda.Sa_Cod),
                                     CInt(objParametriAgenda.Id_Agenda),
                                     0,
                                     objParametri_Server)


        If Not IsNothing(Agenda) Then

            objParametriAgenda.Piva = Agenda.Piva
            objParametriAgenda.Sa_Cod = Agenda.Sa_Cod
            objParametriAgenda.Lav_Cod = Agenda.Lav_Cod

            'NOTE
            If Not IsNothing(Agenda.Note) Then
                For i = 0 To Agenda.Note.Count - 1
                    objParametriAgenda.Note.Add(Agenda.Note(i))
                Next
            End If
            objParametriAgenda.salva()

            'MOVIMENTI
            If Not IsNothing(Agenda.Movimenti) Then

                Dim MovimentiCosti As List(Of Movimento) = New List(Of Movimento)
                objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

                For i = 0 To Agenda.Movimenti.Count - 1

                    'controllo corrispondenza piva con agenda
                    If Agenda.Piva <> Agenda.Movimenti(i).Piva Then
                        Throw New ApplicationException
                    End If

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case enum_Agenda_Causali.LAVORAZIONE

                            If Agenda.Lav_Cod <> objParametriAgenda.Lav_Cod Then
                                Throw New NotImplementedException
                            End If

                            Select Case CInt(objParametriAgenda.Lav_Cod)
                                Case LAVCOD_IRRIGAZIONE
                                    LeggiMovimentoAgenda_Rilievi(Agenda, i, Irrigazioni,
                                                                 NoteIntervento, objParametriAgenda, objParametri_Server)
                                Case Else
                                    Throw New NotImplementedException
                            End Select



                            '----COSTO ACCESSORIO---------------------
                        Case enum_Agenda_Causali.SCARICO

                            'è un movimento dovuto ad un costo accessorio
                            MovimentiCosti.Add(Agenda.Movimenti(i))


                        Case CAU_IMPUTAZIONE_PARCOMACCHINE
                            MovimentiCosti.Add(Agenda.Movimenti(i))

                        Case CAU_IMPUTAZIONE_MANODOPERA
                            MovimentiCosti.Add(Agenda.Movimenti(i))

                        Case CAU_IMPUTAZIONE_TERZISTI
                            MovimentiCosti.Add(Agenda.Movimenti(i))

                        Case CAU_IMPUTAZIONE_UTILIZZO_PRODOTTI
                            MovimentiCosti.Add(Agenda.Movimenti(i))

                        Case CAU_IMPUTAZIONE_TECNICO_RESPONSABILE
                            MovimentiCosti.Add(Agenda.Movimenti(i))

                        Case Else
                            Throw New NotImplementedException

                    End Select
                Next

                objParametriAgenda.Movimenti = MovimentiCosti

            End If
        End If

        'Txt_SupSelezionata
        Dim ImpUtil As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        For Each imp As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In objParametriAgenda.Impianti
            supImp += ImpUtil.LeggiSuperficie(Agenda.Piva, Agenda.Sa_Cod, imp.Appezza, imp.ID_Reg, objParametri_Server)
        Next


        Return Irrigazioni
    End Function


    Private Sub LeggiMovimentoAgenda_Rilievi(ByRef agenda As Operazione_Agenda, ByRef i As Integer,
                                             ByRef irrigazioni As List(Of Object),
                                             ByRef NoteIntervento As String,
                                             ByRef objParametriAgenda As ParametriAgenda, ByVal objParametri_Server As AgronicaCoreParametri)

        objParametriAgenda.Data = agenda.Movimenti(i).Data
        NoteIntervento = agenda.Movimenti(i).Mov_Desc


        'MOVIMENTO_DETTAGLIO TECNICO (dovrebbe essere nothing)
        If Not IsNothing(agenda.Movimenti(i).Movimenti_Dettagli_Tecnici) Then

            For j = 0 To agenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1

                'controllo corrispondeza piva con agenda
                If agenda.Piva <> agenda.Movimenti(i).Piva Then
                    Throw New ApplicationException
                End If

                'non dovrebbe essercene nessuno
                Throw New NotImplementedException
            Next

        End If



        '---------------------
        '----MOVIMENTI_DETTAGLI
        '---------------------
        'uno per ciascun impianto influenzato dalle trappole installate
        If Not IsNothing(agenda.Movimenti(i).Movimenti_Dettagli) Then

            For j = 0 To agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                'controllo corrispondeza piva con agenda
                'sa cod è uguale ma potrebbe cambiare se...
                If agenda.Piva <> agenda.Movimenti(i).Movimenti_Dettagli(j).Piva Then
                    Throw New ApplicationException
                End If


                'ci deve essere un solo mov dettaglio tecnico per un mov dettaglio 
                If agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count <> 1 Then
                    Throw New ApplicationException
                End If


                'ci deve essere un solo mov destinaz tecnico per un mov dettaglio 
                If agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count <> 1 Then
                    Throw New ApplicationException
                End If

                '------------------------------
                '----- MOVIMENTO DET TECNICO --
                '------------------------------
                Dim Mov_Det_Tecnico As Movimento_Dettaglio_Tecnico = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0)
                Dim dose As Decimal = 0
                Dim ore As Decimal = 0
                Dim portata As Integer = 0
                Dim QuaTot As Decimal = 0
                Dim Data_i As Date = AGRODATAINIZIO
                Dim Data_f As Date = AGRODATAFINE
                Dim frequenza As Integer = 0
                Dim TipoIrrigaz As Integer = 0
                Dim UDM As Integer = 0
                Dim ID_DSS_Irrigazione As Integer = 0
                Dim Qta_Acqua_Custom As Decimal = 0
                Dim Data_Custom As Date = AGRODATAINIZIO

                Select Case CInt(objParametriAgenda.Lav_Cod)
                    Case LAVCOD_IRRIGAZIONE
                        dose = Mov_Det_Tecnico.Qta_Ril
                        ore = Mov_Det_Tecnico.Dose
                        portata = Mov_Det_Tecnico.Parziale
                        Data_i = Mov_Det_Tecnico.Inn1_data
                        Data_f = Mov_Det_Tecnico.Inn2_data
                        frequenza = Mov_Det_Tecnico.Nitrati
                        TipoIrrigaz = Mov_Det_Tecnico.Freatimetro
                        UDM = Mov_Det_Tecnico.dett_cod
                        ID_DSS_Irrigazione = If(IsDBNull(Mov_Det_Tecnico.Extra_Int), 0, Mov_Det_Tecnico.Extra_Int)
                        Qta_Acqua_Custom = If(IsDBNull(Mov_Det_Tecnico.ExtraStr) OrElse Not IsNumeric(Mov_Det_Tecnico.ExtraStr), 0, CDec(Mov_Det_Tecnico.ExtraStr))
                        Data_Custom = If(IsDBNull(Mov_Det_Tecnico.Extra_Date) OrElse Mov_Det_Tecnico.Extra_Date = New Date, AGRODATAINIZIO, Mov_Det_Tecnico.Extra_Date)
                    Case Else
                        Throw New NotImplementedException
                End Select

                '---------------------
                'MOVIMENTO_DESTINAZIONI
                '---------------------
                Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

                objAppezzamento.Piva = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Piva
                objAppezzamento.Sa_Cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Sa_Cod
                objAppezzamento.Appezza = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Appezza
                objAppezzamento.ID_Reg = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Id_Destinazione

                Select Case CInt(objParametriAgenda.Lav_Cod)
                    Case LAVCOD_IRRIGAZIONE
                        QuaTot = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta
                        If agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta2 > 0 Then
                            objAppezzamento.Qta2 = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta2
                        Else
                            objAppezzamento.Qta2 = (New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read()).LeggiSuperficie(objAppezzamento.Piva, objAppezzamento.Sa_Cod, objAppezzamento.Appezza, objAppezzamento.ID_Reg, objParametri_Server)
                        End If

                    Case Else
                        Throw New NotImplementedException
                End Select

                Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                Dim Dt_Imp As New DataTable
                Dt_Imp = objImp.Leggi(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Piva,
                             agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Sa_Cod,
                             agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Appezza,
                             agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Id_Destinazione,
                             enumSelezioneVariabile.Selezione_JoinDescrizioni,
                             "", "", objParametri_Server)

                If Dt_Imp.Rows.Count > 0 Then
                    objParametriAgenda.Veg_Cod = Dt_Imp.Rows(0).Item("veg_cod").ToString()
                End If

                If CInt(objParametriAgenda.Veg_Cod) = 0 AndAlso CInt(Dt_Imp.Rows(0).Item("id_cod")) <> 0 Then
                    objParametriAgenda.Veg_Cod = "0/" & Dt_Imp.Rows(0).Item("id_cod")
                End If

                'aggiungo impianto a parametri agenda, cosi pagina master può ricreare i check,
                'ma devo inserirlo sono se non è gia presente altrimenti mi sdoppia le colonne
                Dim presente As Boolean = False
                For Each ap As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In objParametriAgenda.Impianti
                    If ap.Piva = objAppezzamento.Piva AndAlso
                        ap.Sa_Cod = objAppezzamento.Sa_Cod AndAlso
                            ap.Appezza = objAppezzamento.Appezza AndAlso
                            ap.ID_Reg = objAppezzamento.ID_Reg Then

                        presente = True
                    End If
                Next
                If Not presente Then
                    objParametriAgenda.Impianti.Add(objAppezzamento)
                    objParametriAgenda.salva()
                End If


                '----------------------------------------------------------------------
                '----------------Creo un oggetto per irrigazione impianto------------------
                '----------------------------------------------------------------------
                Dim qta_2 As Decimal = 0

                If agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta2 <> 0 Then
                    qta_2 = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta2
                Else
                    qta_2 = (New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read()).LeggiSuperficie(objAppezzamento.Piva, objAppezzamento.Sa_Cod, objAppezzamento.Appezza, objAppezzamento.ID_Reg, objParametri_Server)
                End If


                irrigazioni.Add(New With
                                    {
                                        .ImpiantoIrrigato = objAppezzamento.Piva.ToString & "-" & objAppezzamento.Sa_Cod.ToString & "-" & objAppezzamento.Appezza.ToString & "-" & objAppezzamento.ID_Reg.ToString,
                                        .Dose = dose,
                                        .Ore = ore,
                                        .Portata = portata,
                                        .Data_Inizio = Data_i,
                                        .Data_Fine = Data_f,
                                        .Frequenza = frequenza,
                                        .Qta_Totale = QuaTot,
                                        .UDM_Dose = UDM,
                                        .TipoIrrigazioneUtilizzata = TipoIrrigaz,
                                        .Qta2 = qta_2,
                                        .ID_DSS_Irrigazione = ID_DSS_Irrigazione,
                                        .Qta_Acqua_Custom = Qta_Acqua_Custom,
                                        .Data_Custom = Data_Custom
                                    })



            Next


        Else
            'ci deve essere almeno un movimento dettaglio per l'installazione di una trappola in un appezzamento
            Throw New ApplicationException
        End If

        'verifico udm
        Dim UDM_Correntre As Integer = irrigazioni(0).UDM_Dose
        Dim TipoIrr_Corrente As Integer = irrigazioni(0).TipoIrrigazioneUtilizzata

    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Function caricaTestataRicetta_Irrigazione(ByVal ricetta_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objRicette_R As New AgronicaCoreContabDAL.Ricette_R
            Dim dt = objRicette_R.Leggi(ricetta_cod,
                               "", 0, 0, 0,
                               AGRODATAINIZIO,
                               AGRODATAFINE,
                               enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                               "", "",
                               objParametri_Server)

            Dim ObjRicetta As New JObject
            If dt.Rows.Count > 0 Then
                ObjRicetta("Ricetta_SuperUser") = CStr(dt.Rows(0)("Ricetta_SuperUser"))
                ObjRicetta("Ricetta_Cod") = CInt(dt.Rows(0)("Ricetta_Cod"))
                ObjRicetta("Ricetta_Des") = CStr(dt.Rows(0)("Ricetta_Des"))
                ObjRicetta("Veg_Cod") = CInt(dt.Rows(0)("Veg_Cod"))
                ObjRicetta("Note") = CStr(dt.Rows(0)("Note"))
                ObjRicetta("Validita_Inizio") = CDate(dt.Rows(0)("Validita_Inizio"))
                ObjRicetta("Valitida_Fine") = CDate(dt.Rows(0)("Validita_Fine"))
                ObjRicetta("Piva") = CStr(dt.Rows(0)("Piva"))
                ObjRicetta("Sa_Cod") = CInt(dt.Rows(0)("Sa_Cod"))
                ObjRicetta("Tipo_Ricetta") = CInt(dt.Rows(0)("Tipo_Ricetta"))
                ObjRicetta("Ricetta_Numero") = CStr(dt.Rows(0)("Ricetta_Numero"))
            End If

            r.RispostaStringa = ObjRicetta.ToString
            r.RispostaOK = True
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function caricaRicetteOperazioni_Irrigazione(ByVal ricetta_cod As Integer, ByVal ricetta_operazione_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objRicette_Operazioni As New AgronicaCoreContabDAL.Ricette_Operazioni_R
            Dim resArr As New JArray()

            If ricetta_cod <> 0 Then
                Dim dt = objRicette_Operazioni.Leggi(ricetta_cod,
                                        0,
                                        0, 0,
                                        AGRODATAINIZIO,
                                        AGRODATAFINE,
                                        enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                        " Ricette_Operazioni.Ricetta_Operazione_Cod <> " & CStr(ricetta_operazione_cod) & " ",
                                        " Validita_Inizio ",
                                        objParametri_Server)

                Dim ricettaxagenda As New AgronicaCoreContabDAL.RicettexAgenda_R

                For Each row In dt.Rows
                    Dim data = CDate(row("Validita_Inizio")).ToShortDateString
                    Dim dataLong = CStr(CDate(row("Validita_Inizio")))
                    Dim lav_des = row("Lav_Des")
                    Dim objOp As New JObject
                    objOp("tipo") = 3
                    objOp("des") = CStr("Modifica " & lav_des & " del " & data)
                    objOp("Ricetta_Cod") = CInt(row("Ricetta_Cod"))
                    objOp("Ricetta_Operazione_Cod") = CInt(row("Ricetta_Operazione_Cod"))
                    objOp("data") = dataLong
                    Dim dtRibaltata = ricettaxagenda.Leggi(row("Ricetta_Cod"),
                                         CInt(row("Ricetta_Operazione_Cod")),
                                         0,
                                         AGRODATAINIZIO,
                                         AGRODATAFINE,
                                         enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                         "",
                                         "",
                                         objParametri_Server)

                    Dim ribaltata As Boolean = False

                    If dtRibaltata.Rows.Count > 0 Then
                        ribaltata = True
                    End If

                    objOp("ribaltata") = CBool(ribaltata)

                    resArr.Add(objOp)
                Next

            End If

            Dim objOpN As New JObject
            objOpN("des") = "Aggiungi Nuovo Dettaglio"
            objOpN("Ricetta_Cod") = CInt(ricetta_cod)
            objOpN("tipo") = 3
            objOpN("Ricetta_Operazione_Cod") = CInt(0)
            objOpN("ribaltata") = False
            resArr.Add(objOpN)

            r.RispostaStringa = resArr.ToString
            r.RispostaOK = True
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ricetta_numero_default(ByVal piva As String, ByVal sa_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()


        Dim ricetta_numero As String = ""

        Dim dt_elenco As DataTable = New AgronicaCoreContabDAL.Ricette_R().Leggi(0, piva, sa_cod,
                                                                                 enum_TipoRicetta.Standard_Destinazioni, 0,
                                                                                 AGRODATAINIZIO, AGRODATAFINE,
                                                                                 enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                 "Validita_Inizio >= " & UtilityProvider.Agro_SQL_SaveDate(New Date(Date.Now.Year, 1, 1)) & " AND Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(New Date(Date.Now.Year, 12, 31)),
                                                                                 "", objParametri_Server)
        If Not IsNothing(dt_elenco) AndAlso dt_elenco.Rows.Count > 0 Then
            ricetta_numero = Date.Now.Year & "_" & CStr(dt_elenco.Rows.Count + 1)
        Else
            ricetta_numero = Date.Now.Year & "_1"
        End If

        r.RispostaOK = True
        r.RispostaStringa = ricetta_numero

        Return r

    End Function

#Region "Gestione Kendo Grid Impianti Irrigazione"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CreaKendoGrid_Impianti_Irrigazione(ByVal piva As String, ByVal veg_cod As Integer, ByVal id_cod As Integer,
                                                              ByVal cul_cod As String, ByVal sa_cod As Integer,
                                                              ByVal data As String, ByVal tipo_operazione As Integer,
                                                              ByVal tipo_operazione_agenda As Integer, ByVal lav_cod As Integer,
                                                              ByVal disciplinare As String, ByVal irrigazioni As String,
                                                              ByVal objParamAgendaImpianti As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim Irrigazione_BIZ = New AgronicaCoreModello.Irrigazione_BIZ

            Dim IrrigazioniArray As JArray = JArray.Parse(irrigazioni)

            Dim objParamAgendaImpiantiArray As JArray = JArray.Parse(objParamAgendaImpianti)

            r = Irrigazione_BIZ.CreaKendoGrid_Impianti_Irrigazione(piva, veg_cod, id_cod,
                                                                    cul_cod, sa_cod, data,
                                                                    tipo_operazione, tipo_operazione_agenda, lav_cod,
                                                                    disciplinare, IrrigazioniArray,
                                                                    objParamAgendaImpiantiArray, DSSIrrigazione_Autorizzato,
                                                                    objParametri_Server, objParametri_Utenti)


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_ImpostazioniKendoGrid_Impianti_Irrigazione(ByVal colonne_visibili As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim Irrigazione_BIZ = New AgronicaCoreModello.Irrigazione_BIZ

            Irrigazione_BIZ.Salva_ImpostazioniKendoGrid_Impianti_Irrigazione(colonne_visibili, objParametri_Utenti)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
#End Region


#Region "Gestione Kendo Grid Rilievi Pioggie Irrigazione"
    <WebMethod(EnableSession:=True)>
    Public Shared Function CreaKendoGrid_RilieviPioggie_Irrigazione(ByVal piva As String,
                                                                    ByVal data_da As String,
                                                                    ByVal data_a As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim Irrigazione_BIZ = New AgronicaCoreModello.Irrigazione_BIZ

            r = Irrigazione_BIZ.CercaPioggeIrrigazione(piva, data_da,
                                                       data_a, objParametri_Server)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function
#End Region

#Region "Gestione Salvataggio Irrigazione"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Irrigazione(ByVal model As String, ByVal TipoSalvataggio As Integer,
                                             ByVal Qs_Operazione_Ricetta As String, ByVal Qs_Ricetta_Cod As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim Irrigazione_BIZ = New AgronicaCoreModello.Irrigazione_BIZ

            Dim messaggio_errore As String = String.Empty

            Dim BaseCode As Integer
            Dim TopCode As Integer

            '------------------------------------------------
            '----- Calcolo i valori di BaseCode e TopCode
            '------------------------------------------------

            UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode,
                                                     HttpContext.Current.Session("ASG_ProgressivoGIAS"))

            Dim link As String = ""
            Dim Id_Agenda As Integer = 0

            Irrigazione_BIZ.SalvaTuttoIrrigazione(model, messaggio_errore, Id_Agenda,
                                                  BaseCode, TopCode, TipoSalvataggio, link,
                                                  Qs_Operazione_Ricetta, Qs_Ricetta_Cod, objParametri_Server, objParametriAgenda)

            If String.IsNullOrEmpty(messaggio_errore) Then
                r.RispostaOK = True

                Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                Dim risp As New JObject(New JProperty("MsgRegistrazioneEffetuata", "Registrazione Effettuata con Successo!"),
                                        New JProperty("Id_Agenda", Id_Agenda))

                r.RispostaStringa = JsonConvert.SerializeObject(risp, Formatting.None, serializerSettings)

                If Not String.IsNullOrEmpty(link) Then
                    r.ParametroDue = True
                    r.ParametroDue_stringa = link
                End If

            Else
                r.RispostaOK = False
                r.ParametroDue = False
                r.Errore = messaggio_errore
            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

#End Region

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_DSS_Irrigazione(ByVal data As Date, ByVal piva As String, ByVal sa_cod As Integer,
                                                 ByVal appezza As Integer, ByVal id_reg As Integer, ByVal progetto_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim DSS_Irrigazione As New AgronicaCoreMeteoDAL.DSS_Irrigazione_R

            Dim Data_Esecuzione_Inizio As Date = AGRODATAINIZIO

            Dim Data_Esecuzione_Fine As Date = data

            Dim Numero_Giorni_Precedenti As Integer = 15

            If Data_Esecuzione_Fine <> AGRODATAINIZIO AndAlso Data_Esecuzione_Fine <> AGRODATAFINE Then
                Data_Esecuzione_Inizio = Data_Esecuzione_Fine.AddDays(-15)
            End If

            Dim DT = DSS_Irrigazione.Leggi_Precedenti_Con_Turni(piva, sa_cod, appezza, id_reg, progetto_cod,
                                                                Data_Esecuzione_Inizio, Data_Esecuzione_Fine, "", "", objParametri_Server)

            r.RispostaOK = True

            r.RispostaStringa = JsonConvert.SerializeObject(DT)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Turni_Salvati(ByVal id_dss As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim DSS_Irrigazione As New AgronicaCoreMeteoDAL.DSS_Irrigazione_R

            Dim OrderBy As String = "Data_Turno ASC"

            Dim DT = DSS_Irrigazione.LeggiTurni(id_dss, 0, "", OrderBy, objParametri_Server)

            r.RispostaOK = True

            r.RispostaStringa = JsonConvert.SerializeObject(DT)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class

