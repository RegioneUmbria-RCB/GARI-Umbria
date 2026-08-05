Imports System.Web.Services
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

Public Class Trasferimento
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    Private _qsPagRitorno As String

    Private Sub InizializzoObjParametri()
        objParametriAgenda = New ParametriAgenda
        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'pagina chiamante --> per gestire il tipo di uscita dalla pagina
        'usa enum_PagineAgenda_2010
        If Not IsNothing(Request.QueryString("orig")) Then
            _qsPagRitorno = Stringa_Decodifica(Request.QueryString("orig").ToString, AgroKey_EncoderDecoder)
        Else
            _qsPagRitorno = CStr(0)
        End If
        'hf_Qs_PagRitorno.Value = _qsPagRitorno

        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder)
        hdId_Agenda.Value = Request.QueryString("Id_Agenda")



        InizializzoObjParametri()

        If Not IsPostBack Then
            InizializzoParametriPagina()

            'Controllo se F&F / Cantine / Tabacco
            hf_Modulo_Anagrafe_Log.Value = ""
            Dim elencoModuli = ""
            Dim leggiAnagrafeLog As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
            Dim listModuli As List(Of Integer) = leggiAnagrafeLog.Recupera_Moduli_Cliente(hdPiva.Value, objParametri_Server)
            If listModuli IsNot Nothing AndAlso listModuli.Count > 0 Then
                elencoModuli = String.Join("|", listModuli.ToArray())
            End If
            hf_Modulo_Anagrafe_Log.Value = elencoModuli

        End If

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim utenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                                                      Session("ASG_IdServizio"),
                                                                                      enum_Security_Attivita.Trasferimenti_Magazzino,
                                                                                      enum_Security_Operazione.Lettura,
                                                                                      Date.Now,
                                                                                      "",
                                                                                      objParametri_Utenti)

        Dim utenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                                                        Session("ASG_IdServizio"),
                                                                                        enum_Security_Attivita.Trasferimenti_Magazzino,
                                                                                        enum_Security_Operazione.Modifica,
                                                                                        Date.Now,
                                                                                        "",
                                                                                        objParametri_Utenti)


        'Controllo Abilitazione GHG
        Dim objLetturaImpreseImpostazioni = New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

        Dim listaCentriAziendali As New List(Of Integer)
        listaCentriAziendali.Add(0)

        Dim utenteAbilitatoFGestioneGHG As Boolean =
            CBool(objLetturaImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser(hdPiva.Value,
                                                                                        listaCentriAziendali,
                                                                                        enum_Impostazioni_Utenti.SUPERUSER_Gestione_GHG,
                                                                                        0,
                                                                                        objParametri_Utenti,
                                                                                        objParametri_Server))

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = utenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = utenteAbilitatoScrittura
        hf_UtenteAbilitatoGestioneGHG.Value = utenteAbilitatoFGestioneGHG

        If utenteAbilitatoLettura = False Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        If utenteAbilitatoScrittura = False Then

        End If

        If Not Page.IsPostBack Then
            'caricaControlli()
        End If

        If Not String.IsNullOrEmpty(hdId_Agenda.Value) Then

            Dim objAgenda = New AgronicaCoreContabDAL.Agenda_R
            Dim dtAgenda = objAgenda.Leggi(hdPiva.Value,
                                           0,
                                           hdId_Agenda.Value,
                                           0,
                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           "",
                                           "",
                                           objParametri_Server)

            If dtAgenda.Rows.Count > 0 Then
                Master().Lbl_Titolo.Text = AgronicaAgenda_2010.Modifica & " " & dtAgenda.Rows(0).Item("Des_Lib")
                hdData_Op.Value = JsonConvert.SerializeObject(CDate(dtAgenda.Rows(0).Item("Validita_inizio")))
            Else
                hdData_Op.Value = ""
            End If

            'Dim objP_server As String = Utility.convertOBJparametritoString(objParametri_Server)
            'Dim objP_utenti As String = Utility.convertOBJparametritoString(objParametri_Utenti)

            Dim objMagazzinoBIZ As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ

            hdKendo_Scarichi.Value = objMagazzinoBIZ.Leggi_Movimenti_Carico_Scarico(hdPiva.Value, hdId_Agenda.Value, CAU_SCARICO, 0,
                                                                                    LAVCOD_TRASFERIMENTO,
                                                                                    True, Nothing,
                                                                                    objParametri_Server, objParametri_Utenti,
                                                                                    xFiltroAggiuntivo:="Ordine_Det <> 1000", contestoDocContabile:=True)

            hdKendo_Carichi.Value = objMagazzinoBIZ.Leggi_Movimenti_Carico_Scarico(hdPiva.Value, hdId_Agenda.Value, CAU_CARICO, 0,
                                                                                   LAVCOD_TRASFERIMENTO,
                                                                                   False, Nothing,
                                                                                   objParametri_Server, objParametri_Utenti,
                                                                                   xFiltroAggiuntivo:="Ordine_Det <> 1000", contestoDocContabile:=True)

            Dim objMov As New AgronicaCoreContabDAL.Movimenti_R
            Dim dtMov As DataTable = objMov.Leggi2(hdPiva.Value, 0, hdId_Agenda.Value,
                                                   0, 0, "", "", "",
                                                   objParametri_Server)

            If dtMov IsNot Nothing AndAlso dtMov.Rows.Count > 0 Then

                Dim idMovS As String = (From x In dtMov.AsEnumerable() Where x.Item("Cau_Mov") = CAU_SCARICO Select x.Item("Id_Mov")).FirstOrDefault()
                Dim idMovC As String = (From x In dtMov.AsEnumerable() Where x.Item("Cau_Mov") = CAU_CARICO Select x.Item("Id_Mov")).FirstOrDefault()

                hdId_Mov_Scarico.Value = If(String.IsNullOrEmpty(idMovS), 0, idMovS)
                hdId_Mov_Carico.Value = If(String.IsNullOrEmpty(idMovC), 0, idMovC)

                'Lettura dei parametri distanza per il movimento di carico
                Dim objMov_Extra As New AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_R
                Dim dtMov_EXtra As DataTable = objMov_Extra.Leggi2(hdPiva.Value, 0, hdId_Agenda.Value, hdId_Mov_Carico.Value,
                                                                   0, 0, 1, "", "",
                                                                  objParametri_Server)

                If dtMov_EXtra IsNot Nothing AndAlso dtMov_EXtra.Rows.Count > 0 Then

                    hdDistanza_Trasporto_UDM.Value = If(String.IsNullOrEmpty(dtMov_EXtra(0)("Distanza_Trasporto_UDM")), 0, dtMov_EXtra(0)("Distanza_Trasporto_UDM"))
                    hdDistanza_Trasporto.Value = If(String.IsNullOrEmpty(dtMov_EXtra(0)("Distanza_Trasporto")), 0, dtMov_EXtra(0)("Distanza_Trasporto"))

                End If

            End If

        Else

            Master().Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("NuovoTrasferimento"), String)

            hdId_Mov_Scarico.Value = 0
            hdId_Mov_Carico.Value = 0

            'TODO: DEBUG! simulo la lettura per fare in modo che venga creata cmq la griglia

            Dim objMagazzinoBIZ As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ

            hdKendo_Scarichi.Value = objMagazzinoBIZ.Leggi_Movimenti_Carico_Scarico(hdPiva.Value, -999999, CAU_SCARICO, 0,
                                                                                    LAVCOD_TRASFERIMENTO,
                                                                                    True, Nothing,
                                                                                    objParametri_Server, objParametri_Utenti,
                                                                                    xFiltroAggiuntivo:="Ordine_Det <> 1000", contestoDocContabile:=True)

            hdKendo_Carichi.Value = objMagazzinoBIZ.Leggi_Movimenti_Carico_Scarico(hdPiva.Value, -999999, CAU_CARICO, 0,
                                                                                   LAVCOD_TRASFERIMENTO,
                                                                                   False, Nothing,
                                                                                   objParametri_Server, objParametri_Utenti,
                                                                                   xFiltroAggiuntivo:="Ordine_Det <> 1000", contestoDocContabile:=True)

        End If

        Page.Title = Master.Lbl_Titolo.Text

        Master.flag_MostraBtnEsci = True

        Master.flag_MostraBtnIndietro = True
        AddHandler Master.ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto


    End Sub

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)
        'PremutoAnnulla = True
        'AAA_GestioneUscitaPagina()

        Select Case CInt(_qsPagRitorno)
            Case enum_PagineAgenda_2010.Pagina_RicercaTrasferimenti
                Dim url As String = "~/GestioneMagazzini/RicercaTrasferimenti.aspx" &
                                    "?p=" & Stringa_Codifica(hdPiva.Value, AgroKey_EncoderDecoder)
                Response.Redirect(url)
            Case enum_PagineAgenda_2010.Menu
                If Master.flag_MenuBS_2017 Then
                    Response.Redirect("~/Menu/MenuBS_2017.aspx")
                Else
                    Response.Redirect("~/Menu/Menu.aspx")
                End If
            Case enum_PagineAgenda_2010.Menu_BS
                Response.Redirect("~/Menu/MenuBS_Agenda_Nuovo.aspx")
        End Select

    End Sub


    Private Sub InizializzoParametriPagina()
        Master.SetTitoloPagina(193)
        hdPivaSuperuser.Value = objParametri_Server.PivaSuperUser
    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

#Region "script services Carica Griglia Movimenti"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaGiacenze(ByVal piva As String, ByVal _prodotto As Integer,
                                                 ByVal _specie As Integer, ByVal _varieta As String, ByVal _sa_cod As Integer,
                                                 ByVal _dataRif As String, ByVal _lotto As String,
                                                 ByVal _calibro As Integer, ByVal _qualita As Integer,
                                                 ByVal _certificazione As Integer, ByVal _rugginosita As Integer,
                                                 ByVal _imballaggio As Integer, ByVal _contenitore As Integer,
                                                 ByVal _confezione As Integer, ByVal _chkGiacenzePositive As Boolean, _mostraCampiInput As Boolean
                                                 ) As RispostaStandard

        Return Giacenze_MagazzinoUC.CaricaGrigliaGiacenze(piva, _prodotto, _specie, _varieta,
                                                          _sa_cod, _dataRif, _lotto, _calibro, _qualita, _certificazione, _rugginosita,
                                                          _imballaggio, _contenitore, _confezione, _chkGiacenzePositive, _mostraCampiInput)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RicercaScarichi(ByVal piva As String, ByVal id_agenda As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim objMagazzinoBIZ As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ
            r.RispostaStringa = objMagazzinoBIZ.Leggi_Movimenti_Carico_Scarico(piva, id_agenda, CAU_SCARICO, 0,
                                                                               LAVCOD_TRASFERIMENTO,
                                                                               True, Nothing,
                                                                               objParametriServer, objParametriUtenti,
                                                                               xFiltroAggiuntivo:="Ordine_Det <> 1000", contestoDocContabile:=True)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RicercaCarichi(ByVal piva As String, ByVal id_agenda As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            Dim objMagazzinoBIZ As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ
            r.RispostaStringa = objMagazzinoBIZ.Leggi_Movimenti_Carico_Scarico(piva, id_agenda, CAU_CARICO, 0,
                                                                               LAVCOD_TRASFERIMENTO,
                                                                               False, Nothing,
                                                                               objParametriServer, objParametriUtenti,
                                                                               xFiltroAggiuntivo:="Ordine_Det <> 1000", contestoDocContabile:=True)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r

    End Function

#End Region

    <WebMethod(EnableSession:=True)>
    Public Shared Function InserisciScaricoTrasferimento(ByVal piva As String,
                                                         ByVal idAgenda As Integer,
                                                         ByVal idMovScarico As Integer,
                                                         ByVal idMovCarico As Integer,
                                                         ByVal trasferimento As String,
                                                         ByVal messaggioDettagliato As Boolean
                                                         ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objOutput As New AgronicaCoreModello.Trasferimento_Output
        Dim msgError As String = ""

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim agroTrasf = JsonConvert.DeserializeObject(Of AgronicaCoreModello.Trasferimento)(trasferimento, settingLoc)

            agroTrasf.Piva = piva
            agroTrasf.TipoDestinazione = CELLA_FRIGORIFERA
            agroTrasf.JollyInt = MagazzinoMovimentato

            Dim objTrasfHelper As New AgronicaCoreModello.TrasferimentoHelper(objParametriServer, objParametriUtenti)
            objOutput = objTrasfHelper.ScriviDettaglioTrasferimentoScarico(piva, idAgenda, idMovScarico, idMovCarico,
                                                                           agroTrasf, messaggioDettagliato, msgError)

            r.RispostaOK = objOutput.Risultato
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)

        Catch ex As Exception

            If objOutput.MsgError = "" Then
                objOutput.MsgError = AgronicaAgenda_2010.ErroreInserimento
            End If

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ModificaScaricoTrasferimento(ByVal piva As String,
                                                        ByVal trasferimento As String,
                                                        ByVal messaggioDettagliato As Boolean
                                                        ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objOutput As New AgronicaCoreModello.Trasferimento_Output
        Dim msgError As String = ""

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            
            Dim agroTrasf As AgronicaCoreModello.Trasferimento = JsonConvert.DeserializeObject(trasferimento, (New AgronicaCoreModello.Trasferimento).GetType(), settingLoc)

            Dim objTrasfHelper As New AgronicaCoreModello.TrasferimentoHelper(objParametriServer, objParametriUtenti)
            objOutput = objTrasfHelper.AggiornaTrasferimentoScarico(agroTrasf, messaggioDettagliato, msgError)

            r.RispostaOK = objOutput.Risultato
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)

        Catch ex As Exception

            If objOutput.MsgError = "" Then
                objOutput.MsgError = AgronicaAgenda_2010.ErroreModifica
            End If

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ModificaCaricoTrasferimento(ByVal piva As String,
                                                       ByVal trasferimento As String,
                                                       ByVal messaggioDettagliato As Boolean
                                                       ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objOutput As New AgronicaCoreModello.Trasferimento_Output
        Dim msgError As String = ""

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            
            Dim agroTrasf As AgronicaCoreModello.Trasferimento = JsonConvert.DeserializeObject(trasferimento, (New AgronicaCoreModello.Trasferimento).GetType(), settingLoc)

            Dim objTrasfHelper As New AgronicaCoreModello.TrasferimentoHelper(objParametriServer, objParametriUtenti)
            objOutput = objTrasfHelper.AggiornaTrasferimentoCarico(agroTrasf, messaggioDettagliato, msgError)

            r.RispostaOK = objOutput.Risultato
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)

        Catch ex As Exception
            If objOutput.MsgError = "" Then
                objOutput.MsgError = AgronicaAgenda_2010.ErroreModifica
            End If

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CancellaScaricoTrasferimento(ByVal piva As String,
                                                        ByVal trasferimento As String,
                                                        ByVal messaggioDettagliato As Boolean
                                                        ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objOutput As New AgronicaCoreModello.Trasferimento_Output
        Dim msgError As String = ""

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            
            Dim agroTrasf As AgronicaCoreModello.Trasferimento = JsonConvert.DeserializeObject(trasferimento, (New AgronicaCoreModello.Trasferimento).GetType(), settingLoc)

            Dim objTrasfHelper As New AgronicaCoreModello.TrasferimentoHelper(objParametriServer, objParametriUtenti)
            objOutput = objTrasfHelper.CancellaRigaTrasferimento(agroTrasf, messaggioDettagliato, msgError)

            r.RispostaOK = objOutput.Risultato
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)

        Catch ex As Exception

            If objOutput.MsgError = "" Then
                objOutput.MsgError = AgronicaAgenda_2010.ErroreCancellazione
            End If

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CancellaTotaleTrasferimento(ByVal piva As String,
                                                       ByVal idAgenda As Integer,
                                                       ByVal messaggioDettagliato As Boolean
                                                       ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objOutput As New AgronicaCoreModello.Trasferimento_Output
        Dim msgError As String = ""

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim objTrasfHelper As New AgronicaCoreModello.TrasferimentoHelper(objParametriServer, objParametriUtenti)
            objOutput = objTrasfHelper.CancellaDocumentoTrasferimento(piva, idAgenda, messaggioDettagliato, msgError)

            r.RispostaOK = objOutput.Risultato

            If objOutput.Risultato = True Then
                Dim paginaLink = "Trasferimento.aspx" &
                                 "?p=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder) &
                                 "&orig=" & Stringa_Codifica(enum_PagineAgenda_2010.Menu, AgroKey_EncoderDecoder)
                                 '"&orig=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_Ricercatrasferimenti, AgroKey_EncoderDecoder)

                objOutput.Redirect = paginaLink
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)

        Catch ex As Exception

            If objOutput.MsgError = "" Then
                objOutput.MsgError = AgronicaAgenda_2010.ErroreCancellazione
            End If

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class