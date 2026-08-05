Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgroAgenda_2010

Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports System.IO

Public Class Rilievi_2
    Inherits System.Web.UI.Page

    Public Master_Operazione As OperazioneBootstrap
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda

    Dim Id_Agenda_Old As Integer = 0
    Dim TipoOperazioneAgenda As enum_Tipo_Operazione_Agenda

    Dim Movimento_Dettaglio_Pendente As Integer
    Dim Movimento_Dettaglio_Extra_Date As Date
    Dim Movimento_Dettaglio_Anno As Integer

    Dim Movimento_Dettaglio_Contabilizzato As Integer = 1

    Dim FasiOLD As Boolean = False

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Private Sub Trattamenti_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Master_Operazione = CType(Page.Master, OperazioneBootstrap)

        'AddHandler CType(Page.Master.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto

        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Salva"), ImageButton).Click, AddressOf Me.SalvaTutto

        'AddHandler CType(Ricerca.FindControlIterative(Page.Master, "BTN_ComboCentroAziendale"), Button).Click, AddressOf Me.Aggiorna_Centro_Specie

        'AddHandler CType(Ricerca.FindControlIterative(Page.Master, "BTN_ComboSpecie"), Button).Click, AddressOf Me.Aggiorna_Centro_Specie


    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---

        objParametriAgenda = New ParametriAgenda
        TipoOperazioneAgenda = objParametriAgenda.TipoOperazioneAgenda
        Id_Agenda_Old = objParametriAgenda.Id_Agenda

        Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        Dim Lav_Des As String = objOperazioniLeggi.LavorazioneDes_from_LavorazioneCod(objParametriAgenda.Lav_Cod, objParametri_Server)
        objParametriAgenda.Lav_Des = Lav_Des

        Dim strtipoOperaz As String = ""
        Select Case objParametriAgenda.Tipo_Operazione
            Case enum_TipoOperazioneDB.Lettura
                strtipoOperaz = "Info <br>"
            Case enum_TipoOperazioneDB.Modifica
                strtipoOperaz = "Modifica <br>"
            Case enum_TipoOperazioneDB.Scrittura
                strtipoOperaz = "Nuovo <br>"
        End Select
        CType(Page.Master.Master.FindControl("LblTitolo"), Label).Text = strtipoOperaz & Lav_Des

        Master_Operazione.Property_Div_ProvenienzaRisorse.Visible = False
        CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_CheckDPI_div"), HtmlControl).Visible = False

        'valori copiati da agenda vecchia
        Movimento_Dettaglio_Pendente = 3
        Movimento_Dettaglio_Extra_Date = AGRODATAINIZIO
        Movimento_Dettaglio_Anno = 1900

        Select Case CInt(objParametriAgenda.Lav_Cod)

            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_CAMPO)

            Case LAVCOD_FASI_FENOLOGICHE

                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_CAMPO)

            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO

                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_CAMPO)

            Case LAVCOD_RILIEVO_INDICI_MATURITA

                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_RACCOLTA)

        End Select

        'LeggiImpostazioniSempre()

        'If objParametriAgenda.Lav_Cod <> LAVCOD_RILIEVO_AVVERSITA_CAMPO Then
        '    ComboMisuraXAvversitaInputData1.Visible = False
        'End If

        If Not IsPostBack Then

            Session("Tabella") = Nothing
            VerificaPermessi()
            LeggiImpostazioni()
            caricaControlli()

            'disabilitaControlli()

        Else


            'Select Case CInt(objParametriAgenda.Lav_Cod)
            '    Case LAVCOD_FASI_FENOLOGICHE, LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_ERBE_INFESTANTI
            '        If Not IsNothing(Session("Tabella")) Then
            '            If IsNothing(TabellaTrappole) Then
            '                'premutosalva = False
            '                RigeneraTabellaPerSalvataggio()
            '            End If

            '        End If
            '    Case Else
            '        Throw New NotImplementedException
            'End Select

        End If
    End Sub

    Private Sub caricaControlli()
        Select Case objParametriAgenda.Tipo_Operazione

            Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                'CaricaFasiFenologiche()
            Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica
                'CaricaFasiFenologiche()
                RipristinaControlliDaAgenda()
                Master_Operazione.CaricaCostiAccessori()

            Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura
                RipristinaControlliDaAgenda()
                Master_Operazione.CaricaCostiAccessori()

        End Select
    End Sub

    Private Sub VerificaPermessi()
        Dim UtenteAbilitato_Lettura As Boolean = False
        Dim UtenteAbilitato_Modifica As Boolean = False
        Dim UtenteAbilitato_Lettura_Temp As Boolean = False

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        UtenteAbilitato_Lettura_Temp = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"),
                                    Session("ASG_IdServizio"),
                                    enum_Security_Attivita.Agenda_AccessoMenu,
                                    enum_Security_Operazione.Lettura,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)

        Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura_Temp
        UtenteAbilitato_Lettura = UtenteAbilitato_Lettura_Temp

        If Not UtenteAbilitato_Lettura Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
            Exit Sub
        End If

        Dim UtenteAbilitato_Modifica_Temp As Boolean = False
        UtenteAbilitato_Modifica_Temp = objPermessi.Controlla_Permessi_Utente(
                                   Session("ASG_Utente_Username"),
                                   Session("ASG_IdServizio"),
                                   enum_Security_Attivita.Agenda_AccessoMenu,
                                   enum_Security_Operazione.Modifica,
                                   Date.Now,
                                   "",
                                   objParametri_Utenti)

        Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica_Temp
        UtenteAbilitato_Modifica = UtenteAbilitato_Modifica_Temp

        If objParametriAgenda.Tipo_Operazione <> enum_TipoOperazioneDB.Lettura AndAlso Not UtenteAbilitato_Modifica Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
            Exit Sub
        End If


    End Sub

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Rilievi_Disposed()
        objParametriAgenda.Svuota_DatiOperazione()

        'If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasLan Then
        '    Dim strJS As New StringBuilder
        '    strJS.AppendLine("$(document).ready(function () { ")
        '    strJS.AppendLine("      window.close(); ")
        '    strJS.AppendLine(" });")
        '    ScriptManager.RegisterStartupScript(
        '       UpdatePanel_Prodotti,
        '       UpdatePanel_Prodotti.GetType(),
        '           String.Format("jQuery_{0}", UpdatePanel_Prodotti.ClientID), strJS.ToString, True)
        '    Exit Sub
        'End If


        Dim link As String = ""
        Try
            Dim sitoorigine As Enum_SiteRedirector = HttpContext.Current.Session("Sito_Origine")
            Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

            If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                       enum_PagineGiasOnline_2010.RegistazioneSmart,
                                       enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

            Else
                link = CType(Master.Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
            End If

        Catch ex As Exception
            link = CType(Master.Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
        End Try

        Response.Redirect(link)


    End Sub

    Private Sub Rilievi_Disposed()
        'Master
        CType(Page.Master, OperazioneBootstrap).Operazioni_Dispose()

        'Page
        Session.Remove("UtenteAbilitato_Lettura")
        Session.Remove("UtenteAbilitato_Modifica")

        Session.Remove("comportamentoComboFF")
        Session.Remove("Tabella")

    End Sub

    Private Sub SalvaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Master_Operazione.SalvaCostiAccessori_SuAgendaMovimenti(False, Nothing)

        Dim messaggio_errore As String = ""
        Dim messaggio_alert As String = ""
        Dim Unid_Operazione As String = ""

        Dim TipoSalvataggio As Integer = CType(Ricerca.FindControlIterative(Page.Master, "tipo_salva"), HiddenField).Value

        Dim ListaOpAgenda As New List(Of Operazione_Agenda)

        If SalvaOperazioneAgenda(TipoSalvataggio, messaggio_errore, Unid_Operazione, ListaOpAgenda, messaggio_alert) Then

            'ScriptManager.RegisterStartupScript(UpdatePanel_Tabella, UpdatePanel_Tabella.GetType, "azzera",
            '                                        "$(document).ready(function () {$('#" & HiddenVarie.ClientID & "').val(''); });", True)

            fine_salvataggio(TipoSalvataggio, Unid_Operazione)

        Else
            If messaggio_errore <> "" Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & messaggio_errore, Page, ,
                                    CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Else
                If messaggio_alert <> "" Then
                    ' VAnni: 28/2/2020: Non usare il resx sulla stringa "Salva"
                    Messaggi.AgroSiNo(messaggio_alert & vbCr & Resources.AgronicaAgenda_2010.BrBIProcedereUgualmenteIB, "Salva", Page, ,
                                      CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                End If
            End If
        End If


    End Sub

    Private Function SalvaOperazioneAgenda(
                           ByVal TipoSalvataggio As Integer,
                           ByRef messaggio_errore As String,
                           ByRef Unid_Operazione As String,
                           ByRef ListaOpAgenda As List(Of Operazione_Agenda),
                           ByRef messaggio_alert As String) As Boolean


        Dim res As Boolean = False

        '---------------------------------------
        ' recupero il CENTRO
        Dim Sa_Cod As Integer = 0
        If objParametriAgenda.Sa_Cod = "" Then
            messaggio_errore = Resources.AgronicaAgenda_2010.SelezionareUnCentroAziendale
            Return False
        Else
            Sa_Cod = CInt(objParametriAgenda.Sa_Cod)
        End If


        '////////////////////////////////////////////////////////////
        '//////////////////DATI PER L OPERAZIONE///////////////////////
        '////////////////////////////////////////////////////////////

        '---------------------------------------
        ' recupero gli IMPIANTI
        Dim ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
        ListaImpianti = CType(Master, OperazioneBootstrap).GetImpianti()
        If ListaImpianti.Count = 0 Then
            messaggio_errore = Resources.AgronicaAgenda_2010.SelezionareAlmenoUnImpiantoColturale
            Return False
        End If


        Try


            '-----------------------------------------------------
            '----------- CONNESSIONE E TRANSAZIONE ---------------
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
            '-----------------------------------------------------

            Select Case objParametriAgenda.Sa_Cod

                '--------------------------------------------------
                '--------------------------------------------------
                '--------- OPERAZIONE MULTI-CENTRO    -------------
                '--------------------------------------------------
                '--------------------------------------------------
                Case "0"

                    Dim ListaSacod As New List(Of Integer)
                    Dim Trovato As Boolean
                    For i = 0 To ListaImpianti.Count - 1
                        Trovato = False
                        For j = 0 To ListaSacod.Count - 1
                            If ListaImpianti(i).Sa_Cod = ListaSacod(j) Then
                                Trovato = True
                                Exit For
                            End If
                        Next
                        If Not Trovato Then
                            ListaSacod.Add(ListaImpianti(i).Sa_Cod)
                        End If
                    Next


                    'Ciclo per ogni centro aziendale
                    For i = 0 To ListaSacod.Count - 1

                        Dim ListaImpiantixQuestoSaCod As New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

                        'Seleziono solamente gli impianti relativi a questo centro aziendale
                        For j = 0 To ListaImpianti.Count - 1
                            If ListaImpianti(j).Sa_Cod = ListaSacod(i) Then
                                ListaImpiantixQuestoSaCod.Add(ListaImpianti(j))
                            End If
                        Next

                        '--------------------------------------------------
                        '--------------------------------------------------
                        '--------- CREAZIONE OGGETTO DA SALVARE   ---------
                        '--------------------------------------------------
                        '--------------------------------------------------

                        Dim Agenda As Operazione_Agenda
                        Dim Id_Agenda As Integer = 0

                        Agenda = CreaOggettoAgenda(ListaImpiantixQuestoSaCod, ListaSacod(i))

                        If IsNothing(Agenda) Then
                            Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                        End If

                        Dim objAgendaScrivi As New Agenda_Operazione_Helper

                        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                            Dim CancellataOperazione As Boolean = False
                            CancellataOperazione = objAgendaScrivi.Cancella(objParametriAgenda.Piva,
                                                                                     objParametriAgenda.Sa_Cod,
                                                                                     objParametriAgenda.Id_Agenda, False,
                                                                                     objParametri_Server, logCancellazione:=False)
                        End If

                        Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)


                    Next



                    '--------------------------------------------------
                    '--------------------------------------------------
                    '--------- OPERAZIONE SINGOLO CENTRO    -----------
                    '--------------------------------------------------
                    '--------------------------------------------------
                Case Else


                    '--------------------------------------------------
                    '--------------------------------------------------
                    '--------- CREAZIONE OGGETTO DA SALVARE   ---------
                    '--------------------------------------------------
                    '--------------------------------------------------


                    Dim Agenda As Operazione_Agenda
                    Dim Id_Agenda As Integer = 0

                    Agenda = CreaOggettoAgenda(ListaImpianti, objParametriAgenda.Sa_Cod)

                    If IsNothing(Agenda) Then
                        Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                    End If


                    Dim objAgendaScrivi As New Agenda_Operazione_Helper

                    If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                        Dim CancellataOperazione As Boolean = False
                        CancellataOperazione = objAgendaScrivi.Cancella(objParametriAgenda.Piva,
                                                                                 objParametriAgenda.Sa_Cod,
                                                                                 objParametriAgenda.Id_Agenda, False,
                                                                                 objParametri_Server, logCancellazione:=False)



                    End If

                    Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)


                    If TipoSalvataggio = 1 Then
                        objParametriAgenda.Id_Agenda = Id_Agenda
                    End If



            End Select

            res = True


            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

        Catch ex As Exception

            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            If ex.Message <> "" Then
                messaggio_errore = Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & ex.Message
            Else
                'alert
            End If

            res = False

        Finally

            'chiudi connessione
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        End Try


        Return res


    End Function

    Private Function CreaOggettoAgenda(ByVal ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto),
                                               ByVal Sa_Cod As Integer) As Operazione_Agenda


        '---------------------------------------
        ' recupero i Consigli
        Dim ListaConsigli As List(Of Nota)
        ListaConsigli = CType(Master, OperazioneBootstrap).GetConsigli()


        '---------------------------------------
        ' recupero le NOTE
        Dim strNota As String = CType(Master, OperazioneBootstrap).GetNota()


        '---------------------------------------
        ' recupero la DATA
        Dim Data As Date
        If objParametriAgenda.Data = AGRODATAINIZIO Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IndicareUnaData, Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        Else
            Data = objParametriAgenda.Data
        End If

        '---------------------------------------
        ' recupero la SPECIE
        Dim Veg_Cod As String = ""
        Dim Veg_Des As String = ""
        If objParametriAgenda.Veg_Cod.Split("/")(0) = "-1" Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareUnaSpecieVegetale, Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        Else
            Veg_Cod = objParametriAgenda.Veg_Cod.Split("/")(0)
            Veg_Des = CType(Ricerca.FindControlIterative(Page.Master, "ComboSpecie"), AgronicaControlli_2010.ComboSpecie).Testo_Combo
        End If

        '---------------------------------------
        ' recupero la OPERAZIONE
        Dim Lav_Cod As String = ""
        Dim Lav_Des As String = ""
        If objParametriAgenda.Lav_Cod = "" Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareUnOperazione, Page, , UpdatePanel_Tabella)
            Return Nothing
        Else
            Lav_Cod = objParametriAgenda.Lav_Cod
            Lav_Des = CType(Ricerca.FindControlIterative(Page.Master, "ComboOperazione"), AgronicaControlli_2010.ComboOperazioni).Testo_Combo
        End If


        '---------------------------------------

        Dim righeModificate As String = hdRilievi.Value

        If String.IsNullOrEmpty(righeModificate) OrElse righeModificate = "[]" Then
            Messaggi.AgroMsgBox("", Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        End If

        'Dim righeModificateArray As JArray = JArray.Parse(righeModificate)

        Dim reader As JsonReader = New JsonTextReader(New StringReader(righeModificate))
        reader.DateParseHandling = DateParseHandling.None
        Dim righeModificateArray As JArray = JArray.Load(reader)


        Dim impiantiSelezionati As String = hdImpianti.Value

        If String.IsNullOrEmpty(impiantiSelezionati) OrElse impiantiSelezionati = "[]" Then
            Messaggi.AgroMsgBox("", Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        End If

        Dim impiantiSelezionatiArray As JArray = JArray.Parse(impiantiSelezionati)



        Dim BaseCode As Integer = 0
        Dim TopCode As Integer = 0

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------
        UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode,
                                                 Session("ASG_ProgressivoGIAS"))


        ''  Vanni, 03/06/2014 17:25:31: recupera la info sull'ora
        Dim xH As String = ""
        If objParametriAgenda.Des_lib.StartsWith("H") AndAlso objParametriAgenda.Des_lib.Contains("-") Then
            Dim app As String() = objParametriAgenda.Des_lib.Split("-")
            xH = app(0).TrimEnd(" ") & " - "
        End If
        'fine Vanni, 03/06/2014 17:25:31: recupera la info sull'ora  

        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Agenda As Operazione_Agenda
        Dim Nota As Nota
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Destinazione As Movimento_Destinazione

        Dim trovato As Boolean
        Dim ListaVarieta As New List(Of String)
        Dim StrVarieta As String = ""
        For i = 0 To ListaImpianti.Count - 1
            trovato = False
            For j = 0 To ListaVarieta.Count - 1
                If ListaVarieta(j) = ListaImpianti(i).Cul_Des Then
                    trovato = True
                    Exit For
                End If
            Next
            If Not trovato Then
                ListaVarieta.Add(ListaImpianti(i).Cul_Des)
                StrVarieta &= ", " & ListaImpianti(i).Cul_Des
            End If
        Next

        StrVarieta = StrVarieta.Substring(2, (StrVarieta.Length - 2))

        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = objParametriAgenda.Tipo_Operazione
        'Agenda.Id_Agenda = 0
        Agenda.Id_Agenda = objParametriAgenda.Id_Agenda
        Agenda.Data = Data
        Agenda.Piva = objParametriAgenda.Piva
        Agenda.Sa_Cod = Sa_Cod
        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = Lav_Des & " (" & Veg_Des & "  [" & StrVarieta & "])"

        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode


        '------------------------------------------------
        '----- NOTE
        '------------------------------------------------

        If ListaConsigli.Count > 0 Then
            Agenda.Note = New List(Of Nota)
            For i = 0 To ListaConsigli.Count - 1
                Nota = New Nota
                Nota.Id_Agenda = objParametriAgenda.Id_Agenda
                Nota.Nota_Cod = ListaConsigli(i).Nota_Cod
                Agenda.Note.Add(Nota)
            Next
        End If


        '------------------------------------------------
        '----- MOVIMENTI
        '------------------------------------------------
        Agenda.Movimenti = New List(Of Movimento)


        'COSTI ACCESSORI
        For i = 0 To objParametriAgenda.Movimenti.Count - 1
            objParametriAgenda.Movimenti(i).Id_Agenda = objParametriAgenda.Id_Agenda
            objParametriAgenda.Movimenti(i).Sa_Cod = Agenda.Sa_Cod
            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli) Then
                Dim j As Integer = 0
                For j = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda = objParametriAgenda.Id_Agenda
                    If objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = 0 Then
                        objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = Agenda.Sa_Cod
                    End If
                Next
            End If

            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici) Then
                Dim j As Integer = 0
                For j = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Id_Agenda = objParametriAgenda.Id_Agenda
                    If objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Sa_Cod = 0 Then
                        objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Sa_Cod = Agenda.Sa_Cod
                    End If
                Next
            End If

            objParametriAgenda.Movimenti(i).Data = Data
            Agenda.Movimenti.Add(objParametriAgenda.Movimenti(i))
        Next



        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO LAVORAZIONE
        '------------------------------------------------
        '------------------------------------------------
        Movimento = New Movimento

        Movimento.Id_Agenda = objParametriAgenda.Id_Agenda
        Movimento.Piva = Agenda.Piva
        Movimento.Sa_Cod = Agenda.Sa_Cod
        Movimento.Data = Data
        Movimento.Lav_Cod = Lav_Cod
        Movimento.Cau_Mov = objParametriAgenda.Cau_Mov

        Movimento.Mov_Desc = strNota

        Movimento.BaseCode = BaseCode
        Movimento.TopCode = TopCode

        Agenda.Movimenti.Add(Movimento)


        '------------------------------------------------
        '----- MOVIMENTI DETTAGLI
        '------------------------------------------------

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        Dim dataRilievo As Date
        Dim valoreRilevato As String = ""

        Dim FaseSelezionata As Boolean

        Dim N_App As Integer = impiantiSelezionatiArray.Count

        Dim Codice As Integer = 0
        Dim Descrizione As String = ""

        For Each cRow In righeModificateArray

            Codice = cRow("Codice")
            Descrizione = cRow("Descrizione")

            'per ciascuna riga creo un movimento dettaglio (se è inserito almeno un dato)

            '------------------------------
            '----- MOVIMENTO DETTAGLIO-----
            '------------------------------
            Movimento_Dettaglio = New Movimento_Dettaglio
            Movimento_Dettaglio.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento_Dettaglio.Piva = Agenda.Piva
            Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio.Elem_Cod = 0
            Movimento_Dettaglio.Pro_Cod = 0
            'impostazioni in base alla lavorazione
            Select Case CInt(objParametriAgenda.Lav_Cod)
                Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                    Movimento_Dettaglio.Udm_Cod = 10 'sempre piante per metro quadro
                Case LAVCOD_FASI_FENOLOGICHE
                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                    'CType(TabellaTrappole.Rows(iRiga).Cells(iColonna).Controls(0), TextBox).Text
                    'Movimento_Dettaglio.Udm_Cod = TabellaTrappole.Rows(iRiga).Cells(1).Attributes.Item("Udm_Cod")
                Case LAVCOD_RILIEVO_INDICI_MATURITA
                Case Else
                    Throw New NotImplementedException
            End Select
            Movimento_Dettaglio.Qta = 0
            Movimento_Dettaglio.Contabilizzato = Movimento_Dettaglio_Contabilizzato
            Movimento_Dettaglio.Pendente = Movimento_Dettaglio_Pendente
            Movimento_Dettaglio.Extra_Date = Movimento_Dettaglio_Extra_Date
            Movimento_Dettaglio.Anno = Movimento_Dettaglio_Anno
            Movimento_Dettaglio.Data = Agenda.Data
            Movimento_Dettaglio.BaseCode = Agenda.BaseCode
            Movimento_Dettaglio.TopCode = Agenda.TopCode


            '------------------------------
            '----- MOVIMENTO DET TECNICO --
            '------------------------------
            Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico
            Movimento_Dettaglio_Tecnico.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
            Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio_Tecnico.Data = Agenda.Data
            Movimento_Dettaglio_Tecnico.Ditta_cod = 0
            Movimento_Dettaglio_Tecnico.Dose = 0
            Movimento_Dettaglio_Tecnico.Freatimetro = 0
            Movimento_Dettaglio_Tecnico.BaseCode = Agenda.BaseCode
            Movimento_Dettaglio_Tecnico.TopCode = Agenda.TopCode
            Movimento_Dettaglio_Tecnico.Inn1_data = New Date
            Select Case CInt(objParametriAgenda.Lav_Cod)
                Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                    ' sempre piante per metro quadro
                    Movimento_Dettaglio_Tecnico.dett_cod = 10
                    Movimento_Dettaglio_Tecnico.Sigla_av = "0"
                    Movimento_Dettaglio_Tecnico.Av_Gru = Codice 'TabellaTrappole.Rows(iRiga).Cells(0).Attributes.Item("Av_Cod")
                Case LAVCOD_FASI_FENOLOGICHE
                    Movimento_Dettaglio_Tecnico.ff_classe = Codice
                    Movimento_Dettaglio_Tecnico.ExtraStr = Descrizione
                    Movimento_Dettaglio_Tecnico.Sigla_av = "0"
                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                    Movimento_Dettaglio_Tecnico.dett_cod = 0 'TabellaTrappole.Rows(iRiga).Cells(1).Attributes.Item("Udm_Cod")
                    Movimento_Dettaglio_Tecnico.Av_Cod = Codice 'TabellaTrappole.Rows(iRiga).Cells(0).Attributes.Item("Av_Cod")
                    Movimento_Dettaglio_Tecnico.Sigla_av = "0"
                    Movimento_Dettaglio_Tecnico.ff_classe = 0 'ddlFF1.selectedValue
                    Movimento_Dettaglio_Tecnico.Piezo2 = 0'ddlFF2.selectedValue
                Case LAVCOD_RILIEVO_INDICI_MATURITA
                    Movimento_Dettaglio_Tecnico.dett_cod = 0 'TabellaTrappole.Rows(iRiga).Cells(1).Attributes.Item("Udm_Cod")
                    Movimento_Dettaglio_Tecnico.ff_classe = Codice 'TabellaTrappole.Rows(iRiga).Cells(0).Attributes.Item("Av_Cod")
                    Movimento_Dettaglio_Tecnico.Sigla_av = "0"
                Case Else
                    Throw New NotImplementedException
            End Select

            'aggiungo il movimento dettaglio tecnico al movimento dettaglio
            Movimento_Dettaglio.Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

            FaseSelezionata = False

            For a = 1 To N_App

                Select Case CInt(objParametriAgenda.Lav_Cod)

                    Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                        'udmdes = ""

                    Case LAVCOD_FASI_FENOLOGICHE
                        'udmdes = CType(TabellaTrappole.Rows(iRiga).Cells(1).Controls(0), LiteralControl).Text

                        valoreRilevato = cRow("App_" & a).ToString

                        If IsDate(valoreRilevato) Then

                            dataRilievo = CDate(cRow("App_" & a))

                            FaseSelezionata = True

                            Dim o = (From aa In impiantiSelezionatiArray Where aa("IdColonna") = "App_" & a).FirstOrDefault

                            Dim strChiave As String = o("Chiave")

                            Dim ImpArray() As String = strChiave.Split("_")

                            If ImpArray IsNot Nothing AndAlso ImpArray.Length > 0 Then


                                '-----------------------------------
                                '----- MOVIMENTI DESTINAZIONE ------
                                '-----------------------------------

                                'per ciascun movimento dettaglio creo un figlio mov. destinazione
                                Movimento_Destinazione = New Movimento_Destinazione

                                Movimento_Destinazione.Id_Agenda = objParametriAgenda.Id_Agenda
                                Movimento_Destinazione.Piva = ImpArray(0)
                                Movimento_Destinazione.Sa_Cod = ImpArray(1)

                                If Movimento_Destinazione.Piva <> Agenda.Piva OrElse Movimento_Destinazione.Sa_Cod <> Agenda.Sa_Cod Then
                                    'passa ad altra colonna, perché la colonna riguarda un altro centro
                                    Dim cftygv = 0
                                Else

                                    Movimento_Destinazione.Appezza = ImpArray(2)
                                    Movimento_Destinazione.Id_Destinazione = ImpArray(3)
                                    '  Vanni, 27/05/2014 17:45:48: gestione della chiave da tabella grafica
                                    Movimento_Destinazione.mov_destinazioni_graphickey = "" 'TabellaTrappole.Rows(1).Cells(iColonna).Attributes("mov_destinazioni_graphickey")

                                    For Each riliev As rilievoAvv In objParametriAgenda.Rilievi
                                        If Movimento_Destinazione.Piva = riliev.Impianto.Piva AndAlso
                                            Movimento_Destinazione.Sa_Cod = riliev.Impianto.Sa_Cod AndAlso
                                            Movimento_Destinazione.Appezza = riliev.Impianto.Appezza AndAlso
                                            Movimento_Destinazione.Id_Destinazione = riliev.Impianto.ID_Reg Then
                                            Movimento_Destinazione.mov_destinazioni_graphickey = riliev.mov_destinazioni_graphickey
                                            Exit For
                                        End If
                                    Next

                                    Movimento_Destinazione.BaseCode = Agenda.BaseCode
                                            Movimento_Destinazione.TopCode = Agenda.TopCode

                                            Movimento_Destinazione.Qta = 0 'aggiorno dopo mentre leggo righje per fare i Movimento_Dettaglio_Tecnico

                                            Select Case CInt(objParametriAgenda.Lav_Cod)
                                                Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                                            '        Movimento_Destinazione.Qta = CDbl(valoreRilevato)
                                            '        Movimento_Destinazione.Data = Agenda.Data
                                                Case LAVCOD_FASI_FENOLOGICHE
                                                    '        If Not IsDate(valoreRilevato) Then
                                                    '            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.ANonÈAmmissibile
                                                    '            Return False
                                                    '        End If
                                                    Movimento_Destinazione.Data = CDate(valoreRilevato)
                                                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                                            '        Movimento_Destinazione.Qta = CDbl(valoreRilevato)
                                            '        Movimento_Destinazione.Data = Agenda.Data
                                                Case LAVCOD_RILIEVO_INDICI_MATURITA
                                                    '        Movimento_Destinazione.Qta = CDbl(valoreRilevato)
                                                    '        Movimento_Destinazione.Data = Agenda.Data
                                                Case Else
                                                    Throw New NotImplementedException
                                            End Select


                                            'aggiungo il movimento destinazione al movimento detaglio
                                            Movimento_Dettaglio.Movimenti_Destinazioni.Add(Movimento_Destinazione)

                                            'righeConDati = righeConDati + 1
                                        End If

                            End If


                        End If


                    Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                        'udmdes = CType(TabellaTrappole.Rows(iRiga).Cells(1).Controls(0), LiteralControl).Text
                    Case LAVCOD_RILIEVO_INDICI_MATURITA
                        'udmdes = ""
                    Case Else
                        Throw New NotImplementedException
                End Select

            Next

            ' se il movimento dettaglio corrente contiene almeno uuna destinazione allora
            'vuol dire che ho almeno un valore rilevato sull'appezzamento, quindi aggiungo
            'il movimento dettaglio con iul suo mov dettaglio tecnico e l'insieme delle destinazioni al movimento
            If Movimento_Dettaglio.Movimenti_Destinazioni.Count > 0 Then

                'aggiungo il movimento dettaglio al movimento
                Movimento.Movimenti_Dettagli.Add(Movimento_Dettaglio)
            End If

        Next

        'se ho almeno un movimento dettaglio nell'operazione la aggiungo all'agenda
        If Movimento.Movimenti_Dettagli.Count > 0 Then
            'aggiungo il movimento rilievo in campo-installazione trappola all'operazione agenda
            'Agenda.Movimenti.Add(Movimento)
        Else
            'messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.NessunDatoSalvato
            Return Nothing
        End If

        Return Agenda

    End Function

    Private Sub fine_salvataggio(ByVal TipoSalvataggio As Integer, ByVal Unid_Operazione As String)

        Session("UtilizzataRicetta") = False

        Select Case TipoSalvataggio

            Case 1

                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")

                If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasLan Then
                    strJS.AppendLine("      window.close(); ")
                End If

                Dim link As String = ""
                Try
                    Dim sitoorigine As Enum_SiteRedirector = HttpContext.Current.Session("Sito_Origine")
                    Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

                    If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                        link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                                   enum_PagineGiasOnline_2010.RegistazioneSmart,
                                                   enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

                    Else
                        link = CType(Master.Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                    End If

                Catch ex As Exception
                    link = CType(Master.Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                End Try

                strJS.AppendLine("      ChiamataParent_Id_Ageda(" & objParametriAgenda.Id_Agenda & "); ")
                strJS.AppendLine("      window.location = '" & link & "'; ")

                strJS.AppendLine(" });")

                ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

                Rilievi_Disposed()
                objParametriAgenda.Svuota_DatiOperazione()

            Case 2
                Rilievi_Disposed()
                objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                objParametriAgenda.Note = New List(Of Nota)
                objParametriAgenda.Movimenti = New List(Of Movimento)

                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")
                strJS.AppendLine("      window.location = '../Operazioni/rilievi_2.aspx'; ")
                strJS.AppendLine(" });")
                ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

            Case 3
                'Dim strJS As New StringBuilder
                'strJS.AppendLine("$(document).ready(function () { ")
                'strJS.AppendLine("      Abilita_Disabilita_Resto(); ")
                'strJS.AppendLine(" });")
                'ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                '                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))


        End Select
    End Sub

    Private Sub blocca()

        Dim Veg_Cod As String = ""
        Dim Veg_Des As String = ""
        If objParametriAgenda.Veg_Cod.Split("/")(0) = "-1" Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareUnaSpecieVegetale, Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Exit Sub
        Else
            Veg_Cod = objParametriAgenda.Veg_Cod.Split("/")(0)
            Veg_Des = CType(Ricerca.FindControlIterative(Page.Master, "ComboSpecie"), AgronicaControlli_2010.ComboSpecie).Testo_Combo
        End If

        GeneraTabella(True)

    End Sub

    Private Sub GeneraTabella(ByVal genera_DaMaster As Boolean)

        '-------------------------Genero le tabelle avversità per la lavorazione-------------------------------------
        Dim DT_Infestanti As DataTable
        Dim DT_Avversita As DataTable
        Dim DT_Indici As DataTable

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("Codice", GetType(String)))
        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Default", GetType(String)))

        Dt.Columns.Add(New DataColumn("Fioritura", GetType(String)))

        Dim strImpianti As String = ""
        hdImpianti.Value = ""

        Dim N_Righe As Integer = 0
        Dim N_Colonne As Integer = 0

        Dim ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
        If genera_DaMaster Then
            'genero tabella da impianti master
            ListaImpianti = CType(Master, OperazioneBootstrap).GetImpianti()
        Else
            'genero tabella da oggetto parametri agenda, quando sono in modifica e devo generare
            'la tabella prima che venga chiamata la load della master
            ListaImpianti = objParametriAgenda.Impianti
        End If


        N_Colonne = ListaImpianti.Count
        If N_Colonne = 0 Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareAlmenoUnImpiantoColturale, Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return

        Else
            For i = 0 To ListaImpianti.Count - 1
                Dt.Columns.Add(New DataColumn("App_" & i + 1, GetType(String)))
                'Dt.Columns("App_" & i + 1).ColumnName =ListaImpianti(i).Sa_Nome & " </br> " & ListaImpianti(i).Campo_Des & " </br> " & ListaImpianti(i).App_Nome & " </br>(" & ListaImpianti(i).Cul_Des & ")"
                Dt.Columns("App_" & i + 1).Caption = ListaImpianti(i).Sa_Nome & " </br> " & ListaImpianti(i).Campo_Des & " </br> " & ListaImpianti(i).App_Nome & " </br>(" & ListaImpianti(i).Cul_Des & ")"
                strImpianti &= "{" & """IdColonna"":""App_" & i + 1 & """, ""Chiave"":""" & ListaImpianti(i).Piva & "_" & ListaImpianti(i).Sa_Cod & "_" & ListaImpianti(i).Appezza & "_" & ListaImpianti(i).ID_Reg & """" & "},"
            Next
            If strImpianti <> "" Then
                strImpianti = Left(strImpianti, strImpianti.Length - 1)
                hdImpianti.Value = "[" & strImpianti & "]"
            End If
        End If


        Select Case CInt(objParametriAgenda.Lav_Cod)

            Case LAVCOD_RILIEVO_ERBE_INFESTANTI

                Dt.Columns(1).Caption = "Infestante"
                Dt.Columns(2).Caption = "Piante per metro quadro Default"

                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_CAMPO)
                DT_Infestanti = New AgronicaCoreMetaSchemaDAL.GruppoAvversitaAttive_R().Leggi(0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Case LAVCOD_FASI_FENOLOGICHE

                Dt.Columns(1).Caption = "Fase Fenologica"
                Dt.Columns(2).Caption = "Data Rilievi Default"

                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_CAMPO)

                'lettura fasi fenologiche (da web service sia nuove fasi bbch sia vecchie fasi)
                Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
                objParametriIngresso.Veg_Cod = CInt(objParametriAgenda.Veg_Cod.Split("/")(0))
                Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
                Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS

                If Not String.IsNullOrEmpty(Session("Personalizzate_Fasi")) AndAlso Session("Personalizzate_Fasi") = True Then
                    objParametriIngresso.Personalizzate = True
                    objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser
                End If

                If Not FasiOLD Then
                    objParametriUscita = objFasi_WS.FasiFenologiche(objParametriIngresso)
                Else
                    objParametriIngresso.strOrdinamento = "Progressivo ASC"
                    objParametriUscita = objFasi_WS.FasiFenologiche_OLD(objParametriIngresso)
                End If

                N_Righe = objParametriUscita.ListaFasiFenologiche.Count

                Dim CodiceFase As Integer = 0

                For f = 0 To objParametriUscita.ListaFasiFenologiche.Count - 1

                    Dr = Dt.NewRow

                    If Not FasiOLD Then
                        CodiceFase = objParametriUscita.ListaFasiFenologiche(f).Cod_SS
                    Else
                        CodiceFase = objParametriUscita.ListaFasiFenologiche(f).FF_Cod
                    End If

                    Dr.Item("Codice") = CodiceFase
                    Dr.Item("Descrizione") = objParametriUscita.ListaFasiFenologiche(f).Descrizione

                    Dr.Item("Fioritura") = objParametriUscita.ListaFasiFenologiche(f).Fioritura

                    For i = 0 To ListaImpianti.Count - 1

                        If genera_DaMaster Then

                        Else

                            '-------------------------------------------------------------------
                            '-------------------------INSERIMENTO DATI--------------------------
                            '-------------quando tabella generata dai parametri agenda----------
                            '-------------------------------------------------------------------
                            For Each riliev As rilievoAvv In objParametriAgenda.Rilievi



                                If ListaImpianti(i).Piva = riliev.Impianto.Piva AndAlso
                                    ListaImpianti(i).Sa_Cod = riliev.Impianto.Sa_Cod AndAlso
                                    ListaImpianti(i).Appezza = riliev.Impianto.Appezza AndAlso
                                    ListaImpianti(i).ID_Reg = riliev.Impianto.ID_Reg AndAlso
                                    CodiceFase = riliev.Av_cod Then

                                    Select Case CInt(objParametriAgenda.Lav_Cod)
                                        Case LAVCOD_RILIEVO_ERBE_INFESTANTI

                                            'txt.Text = riliev.Valore

                                        Case LAVCOD_FASI_FENOLOGICHE

                                            Dr.Item("App_" & i + 1) = riliev.Valore
                                            'txt.Text = riliev.Valore

                                        Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                                            'If Udm_Cod = riliev.Udm_cod Then
                                            '    txt.Text = riliev.Valore

                                            'End If
                                        Case LAVCOD_RILIEVO_INDICI_MATURITA
                                            'If Udm_Cod = riliev.Udm_cod Then
                                            '    txt.Text = riliev.Valore
                                            'End If
                                        Case Else
                                            Throw New NotImplementedException
                                    End Select

                                End If

                            Next

                        End If

                    Next

                    Dt.Rows.Add(Dr)
                Next

                'DT_Fasi = New AgronicaCoreMetaSchemaDAL.FasiFenologichexSpecie_R().Leggi(0, objParametriAgenda.Veg_Cod.Split("/")(0), enumSelezioneVariabile.Selezione_JoinCompleta, "", "Progressivo ASC", objParametri_Server)
            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO

                Dt.Columns(1).Caption = "Avversità"
                Dt.Columns(2).Caption = "Quantità Default"

                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_CAMPO)
                'nel caso si voglia usare la combo avversità (nascosta) per scremarle mettere av_cod invece di 0
                DT_Avversita = New AgronicaCoreMetaSchemaDAL.Avversita_R().Leggi_Con_Misura(objParametriAgenda.Veg_Cod.Split("/")(0), 0, "", "", "", objParametri_Server)

            Case LAVCOD_RILIEVO_INDICI_MATURITA

                Dt.Columns(1).Caption = "Indice di Maturità"
                Dt.Columns(2).Caption = "Valore Default"

                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_RACCOLTA)
                DT_Indici = New AgronicaCoreMetaSchemaDAL.IndiciMaturitaxSpecie_R().Leggi_X_UDM(0,
                                 objParametriAgenda.Veg_Cod.Split("/")(0),
                                "", "IND_MAT_DES ASC", objParametri_Server)
        End Select
        '-----------------------------------------------------------------------------------------


        ''  Vanni, 27/05/2014 18:00:11: al momento memorizziamo un punto per ogni operazione, che è uguale per tutti.
        'If objParametriAgenda.Rilievi.Count > 0 Then
        '    tcell.Attributes.Add("mov_destinazioni_graphickey", objParametriAgenda.Rilievi.FirstOrDefault.mov_destinazioni_graphickey)
        'End If

        Session("Tabella") = Dt

    End Sub

    Private Sub btn_MostraTrappole_Click(sender As Object, e As EventArgs) Handles btn_MostraTrappole.Click
        blocca()
    End Sub

    Private Sub Btn_NascondiTrappole_Click(sender As Object, e As EventArgs) Handles Btn_NascondiTrappole.Click

    End Sub

    Private Sub RipristinaControlliDaAgenda()


        Dim objAgenda As New Agenda_Operazione_Helper
        Dim Agenda As New Operazione_Agenda
        Agenda = objAgenda.Leggi(objParametriAgenda.Piva,
                                     CInt(objParametriAgenda.Sa_Cod),
                                     CInt(objParametriAgenda.Id_Agenda),
                                     0,
                                     objParametri_Server)


        If Not IsNothing(Agenda) Then

            objParametriAgenda.Piva = Agenda.Piva
            objParametriAgenda.Sa_Cod = Agenda.Sa_Cod
            objParametriAgenda.Lav_Cod = Agenda.Lav_Cod
            objParametriAgenda.Des_lib = Agenda.Des_Lib

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

                    'controllo corrispondeza piva con agenda
                    If Agenda.Piva <> Agenda.Movimenti(i).Piva Then
                        Throw New ApplicationException
                    End If

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case enum_Agenda_Causali.RILIEVO_CAMPO, enum_Agenda_Causali.RILIEVO_RACCOLTA

                            Select Case CInt(objParametriAgenda.Lav_Cod)
                                Case LAVCOD_FASI_FENOLOGICHE, LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_ERBE_INFESTANTI
                                    LeggiMovimentoAgenda_Rilievi(Agenda, i)
                                Case Else
                                    Throw New NotImplementedException
                            End Select

                                                        '----COSTO ACCESSORIO---------------------
                        Case enum_Agenda_Causali.SCARICO
                            'se c'è è costo accessorio
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


        '................................
        'se tutto è andato bene ora ho i dati e devo settare le combo, text e ricostruire la tabella... 



        'data, impianti, centro già impostati dalla master

        'Txt_SupSelezionata
        Dim ImpUtil As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim supImp As Decimal = 0
        For Each imp As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In objParametriAgenda.Impianti
            supImp += ImpUtil.LeggiSuperficie(Agenda.Piva, Agenda.Sa_Cod, imp.Appezza, imp.ID_Reg, objParametri_Server)
        Next
        Txt_SupSelezionata.Value = CStr(supImp)

        GeneraTabella(False)

        Dim strJS1 As New StringBuilder
        strJS1.AppendLine("$(document).ready(function () { ")
        strJS1.AppendLine("      letturaTabella_Kendo(); ")
        strJS1.AppendLine(" });")
        ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                      String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS1.ToString, True)


    End Sub

    Private Sub LeggiMovimentoAgenda_Rilievi(ByRef agenda As Operazione_Agenda, ByRef i As Integer)


        ''------------------------------------------
        ''----- Dichiarazione delle Variabili
        ''------------------------------------------

        ''lista degli appezzamenti coinvolti nell'operazione agenda
        'Dim AppezzamentiCoinvolti_NumTrappole As New Hashtable()
        ''elenco trappele (numero-nome personalizzato)
        'Dim TrappoleLista As New Hashtable()
        ''Coppia trappola - appezzamento che la contiene
        'Dim Trappole_Appezzamenti As New Hashtable()
        ''Coppia innesco - trappola che la contiene che la contiene
        'Dim Inneschi_Trappole As New Hashtable()

        ''variabili di controllo
        'Dim conteggioTrappolePerVerifica As Integer = 0

        Dim rilievoAvvList As New List(Of rilievoAvv)

        ''------------------------------------------
        ''----- Recupero le informazioni
        ''------------------------------------------



        objParametriAgenda.Data = agenda.Movimenti(i).Data

        Master_Operazione.SetNota(agenda.Movimenti(i).Mov_Desc)

        If Not IsNothing(agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici) AndAlso
            agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici.Count > 0 Then


            Dim lFF_cod1 As Integer = agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).ff_classe
            Dim lFF_cod2 As Integer = agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).Piezo2

            'If lFF_cod1 <> 0 Then
            '    ddlFF1.selectedValue = lFF_cod1
            'End If

            'If lFF_cod2 <> 0 Then
            '    ddlFF2.selectedValue = lFF_cod2
            'End If
        End If



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

                For k = 0 To agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count - 1
                    'ci deve essere un solo mov dettaglio tecnico per un mov dettaglio (piu destinazioni invece)
                    If agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count <> 1 Then
                        Throw New ApplicationException
                    End If

                    For r = 0 To agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                        Dim rilievoAvv As New rilievoAvv

                        '------------------------------
                        '----- MOVIMENTO DETTAGLIO-----
                        '------------------------------
                        'impostazioni in base alla lavorazione
                        Select Case CInt(objParametriAgenda.Lav_Cod)
                            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                                'rilievoAvv.Udm_cod = sempre numero piante per metro quadro
                                rilievoAvv.Udm_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod
                            Case LAVCOD_FASI_FENOLOGICHE

                            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                                'CType(TabellaTrappole.Rows(iRiga).Cells(iColonna).Controls(0), TextBox).Text
                                rilievoAvv.Udm_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod
                            Case LAVCOD_RILIEVO_INDICI_MATURITA

                            Case Else
                                Throw New NotImplementedException
                        End Select

                        '------------------------------
                        '----- MOVIMENTO DET TECNICO --
                        '------------------------------
                        Select Case CInt(objParametriAgenda.Lav_Cod)
                            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                                'rilievoAvv.Udm_cod = sempre numero piante per metro quadro
                                rilievoAvv.Udm_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(k).dett_cod
                                rilievoAvv.Av_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(k).Av_Gru
                            Case LAVCOD_FASI_FENOLOGICHE
                                rilievoAvv.Av_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(k).ff_classe
                                If agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(k).ff_classe < 1000 Then
                                    FasiOLD = True
                                End If
                            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                                rilievoAvv.Udm_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(k).dett_cod
                                rilievoAvv.Av_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(k).Av_Cod
                            Case LAVCOD_RILIEVO_INDICI_MATURITA
                                rilievoAvv.Udm_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(k).dett_cod
                                rilievoAvv.Av_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(k).ff_classe
                            Case Else
                                Throw New NotImplementedException
                        End Select

                        '---------------------
                        'MOVIMENTO_DESTINAZIONI
                        '---------------------
                        'ci deve essere almeno un movimento destinazione, uno per ciascun appezzamento 
                        If IsNothing(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
                            Throw New ApplicationException
                        End If

                        Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

                        objAppezzamento.Piva = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Piva
                        objAppezzamento.Sa_Cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Sa_Cod
                        objAppezzamento.Appezza = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Appezza
                        objAppezzamento.ID_Reg = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Id_Destinazione




                        Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                        Dim Dt_Imp As New DataTable
                        Dt_Imp = objImp.Leggi_DescrizioniImpianti2(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Piva,
                                     agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Sa_Cod,
                                     agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Appezza,
                                     agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Id_Destinazione,
                                     AGRODATAINIZIO, AGRODATAFINE,
                                     "", "", objParametri_Server)

                        If Dt_Imp.Rows.Count > 0 Then

                            objParametriAgenda.Veg_Cod = CInt(Dt_Imp.Rows(0).Item("veg_cod"))

                            objAppezzamento.Sa_Nome = Dt_Imp.Rows(0).Item("sa_nome")
                            objAppezzamento.Campo_Des = Dt_Imp.Rows(0).Item("campo_des")
                            objAppezzamento.App_Nome = Dt_Imp.Rows(0).Item("app_nome")
                            objAppezzamento.Cul_Des = Dt_Imp.Rows(0).Item("cul_des")

                        End If

                        'aggiungo impioanto a parameteri agenda, cosi pagina master pouò ricrreare i check,
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





                        'parametri operazione
                        rilievoAvv.Impianto = objAppezzamento
                        '  Vanni, 27/05/2014 18:03:00: 
                        rilievoAvv.mov_destinazioni_graphickey = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).mov_destinazioni_graphickey
                        Select Case CInt(objParametriAgenda.Lav_Cod)
                            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                                rilievoAvv.Valore = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Qta
                            Case LAVCOD_FASI_FENOLOGICHE
                                If CDate(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Data) <> AGRODATAINIZIO Then
                                    rilievoAvv.Valore = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Data
                                Else
                                    rilievoAvv.Valore = ""
                                End If
                            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                                rilievoAvv.Valore = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Qta
                            Case LAVCOD_RILIEVO_INDICI_MATURITA
                                rilievoAvv.Valore = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Qta
                            Case Else
                                Throw New NotImplementedException
                        End Select

                        rilievoAvvList.Add(rilievoAvv)

                    Next
                Next
            Next


        Else
            'ci seve essere almeno un movimento dettaglio per l'ìinstallazione di una trappola in un appezzamento
            Throw New ApplicationException
        End If

        ''---------Inserisco i dati in objParametriAgenda ------------
        objParametriAgenda.Rilievi = rilievoAvvList

    End Sub



    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaTabella_Kendo(ByVal param As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            r.RispostaOK = False
            Return r
        End If

        Dim Dt_Tabella As New DataTable

        Try
            r.RispostaOK = True
            Dt_Tabella = HttpContext.Current.Session("Tabella")
            r.RispostaStringa = JSON_Datatable_Tabella(Dt_Tabella)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Private Shared Function JSON_Datatable_Tabella(ByVal dt As DataTable) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("Codice", "Codice", "string")
        c._Display = False
        l.Add(c)
        c = New ColonneNome("Fioritura", "Fioritura", "string")
        c._Display = False
        l.Add(c)
        c = New ColonneNome("Descrizione", dt.Columns(1).Caption, "string")
        c._Editabile = False
        l.Add(c)
        c = New ColonneNome("Default", dt.Columns(2).Caption, "date")
        c._Editabile = True
        c._css = "default"
        l.Add(c)

        For i = 4 To dt.Columns.Count - 1
            c = New ColonneNome(dt.Columns(i).ColumnName, dt.Columns(i).Caption, "date")
            c._Editabile = True
            l.Add(c)
        Next

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable

        Return js.JSON_DataTable_Kendo(dt, l)

    End Function

    Private Sub LeggiImpostazioni()

        Session("Personalizzate_Fasi") = False

        Select Case CInt(objParametriAgenda.Lav_Cod)

            Case LAVCOD_RILIEVO_ERBE_INFESTANTI

            Case LAVCOD_FASI_FENOLOGICHE

                If Not String.IsNullOrEmpty(Session("Personalizzate_Fasi")) Then

                    Dim Leggi_impostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                    Dim dt As DataTable =
                        Leggi_impostazioni.Leggi(
                            TipiEnumerativi.enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE,
                            2,
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "",
                            "",
                            objParametri_Utenti
                        )

                    If dt.Rows.Count > 0 AndAlso dt.Rows(0).Item("Impostazione_Valore_1") = "1" Then
                        Session("Personalizzate_Fasi") = True
                    End If

                End If

            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO


            Case LAVCOD_RILIEVO_INDICI_MATURITA

        End Select



    End Sub

End Class