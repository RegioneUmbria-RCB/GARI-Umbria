
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreGestioneRichieste
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Security.Policy
Imports AgronicaCoreModello

Public Class AnalisiRilievi
    Inherits System.Web.UI.Page

    Private objParametriAgenda As ParametriAgenda
    Public Analisi_Rilievi_Autorizzato As Boolean
    Public FiltroneImpostato As Boolean
    Public DescrizioneFiltroSemplice As String

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Private Sub AnalisiRilievi_Init(sender As Object, e As EventArgs) Handles Me.Init
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        objParametriAgenda = New ParametriAgenda
        Session("objParametriAgenda") = objParametriAgenda

        Analisi_Rilievi_Autorizzato = False

        Master.SetTitoloPagina(88)

        'If Request.QueryString("gis") <> "" Then
        '    Me.Master.flag_MostraHeader = False
        '    Me.Master.flag_MostraFooter = False
        'Else
        '    Me.Master.flag_pag_Operazione = True
        'End If

        Master.flag_pag_Operazione = True

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If Not (HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server)) Then

            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Analisi_Rilievi_Autorizzato = ObjUtenti.Controlla_Permessi_Utente(
                                       HttpContext.Current.Session("ASG_Utente_Username"),
                                       HttpContext.Current.Session("ASG_IdServizio"),
                                       enum_Security_Attivita.Analisi_Curve_Maturazione,
                                       enum_Security_Operazione.Modifica,
                                       Date.Now, "", objParametri_Utenti)

        End If

        Dim numeroAziende As Integer = (From ii In objParametriAgenda.Impianti Distinct Select ii.Piva).Distinct.Count
        Dim numeroImpianti As Integer = objParametriAgenda.Impianti.Count


        Dim usaFiltroRicercaNG As Boolean

        If Not Page.IsPostBack Then
            Dim objFiltroRicerca As New AgronicaCoreFiltroneBIZ.FiltroRicerca
            usaFiltroRicercaNG = objFiltroRicerca.usaFiltroRicercaNG(objParametri_Utenti)
            hd_usaFiltroRicercaNG.Value = usaFiltroRicercaNG
            hd_CurrentPiva.Value = objParametriAgenda.Piva
        End If

        FiltroneImpostato = (Not String.IsNullOrEmpty(Request.QueryString("f"))) Or (usaFiltroRicercaNG And objParametriAgenda.Impianti.Count > 0)

        Dim rag_soc As String = objParametriAgenda.RagSoc
        If String.IsNullOrEmpty(rag_soc) Then
            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
            rag_soc = objImprese.RagSoc_from_Piva(objParametriAgenda.Piva, objParametri_Server)
        End If

        DescrizioneFiltroSemplice = DirectCast(GetLocalResourceObject("FiltroImpostatoSuImpresa_"), String) & rag_soc.Replace("""", "\""")

        Dim IDTestataTemp As Integer = 0

        If FiltroneImpostato Then

            If numeroImpianti > 0 Then

                Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze

                'IDTestataTemp = xAgrosequenze.Agronica_SequenzaTabelle_NuovoID("__tmp_FiltroImpianti", objParametri_Server)
                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                IDTestataTemp = xAgrosequenze.NuovoId_Tabella("__tmp_FiltroImpianti", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)

                Dim OperazioneCorrente_FiltroImpianti As String = ""

                FiltroneImpiantiDbTemp.OperazioneCorrente_EstraiFiltrone(IDTestataTemp, objParametriAgenda.Impianti, OperazioneCorrente_FiltroImpianti)

                FiltroneImpiantiDbTemp.PopolaTabellaFiltroImpianti(OperazioneCorrente_FiltroImpianti, objParametri_Server)

            End If

            lblFiltroImpostato.Text = String.Format(DirectCast(GetLocalResourceObject("FiltroAvanzatoNumeroAziendeENumeroImpianti"), String), numeroAziende, numeroImpianti)

        Else

            lblFiltroImpostato.Text = DescrizioneFiltroSemplice

        End If

        Dim AperturaDaPopup = Request.QueryString.AllKeys.Contains("Ifr")

        If AperturaDaPopup Then
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        End If

        id_TestataTemp.Value = IDTestataTemp

        hdPiva.Value = objParametriAgenda.Piva

    End Sub

    Private Sub AnnullaTutto()

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim configurazioneSitiLettura As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim flagMenu = configurazioneSitiLettura.Leggi_Valore(0, "MenuBS_2017", "", "", objParametri_Server)

        If flagMenu.ToLower = "true" Then

            Response.Redirect("../../menu/menubs_2017.aspx")
        Else

            Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineAgenda_2010.Menu, objParametriAgenda))
        End If


        objParametriAgenda = Session("objParametriAgenda")


        Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline And
            paginaOnLineRitorno = enum_PagineGiasOnline.MenuMagazzini Then

            Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(True, paginaOnLineRitorno, objParametriAgenda))

        End If

        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline_2010 And
                paginaOnLineRitorno = enum_PagineGiasOnline_2010.Menu Then

            Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(False, paginaOnLineRitorno, objParametriAgenda))

        End If

        If paginaOnLineRitorno = 0 Then
            If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline_2010 Then
                paginaOnLineRitorno = enum_PagineGiasOnline_2010.Menu
            Else
                paginaOnLineRitorno = enum_PagineAgenda_2010.Menu
            End If
            Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(False, paginaOnLineRitorno, objParametriAgenda))
        End If

        Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineAgenda_2010.Menu, objParametriAgenda))

    End Sub

    Private Sub Btn_FiltraImpianti_Click(sender As Object, e As EventArgs) Handles Btn_FiltraImpianti.Click

        objParametriAgenda.Impianti.Clear()

        Dim TargetRedirect As String

        Dim url As String = ""

        For Each key As String In Request.QueryString.AllKeys
            url &= key & "=" & Request.QueryString(key) & "&"
        Next

        Dim headerFooter As Boolean = True
        Dim qs_hdr_ftr As String = ""

        If Not Me.Master.flag_MostraHeader Then
            qs_hdr_ftr = "&hdr_ftr=false"
            headerFooter = False
        End If

        TargetRedirect = "../../Filtrone/Filtrone_nuovo.aspx?p_o=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_DuplicaOperazione, AgroKey_EncoderDecoder, Server) &
            "&s_o=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Server) &
            "&p_d=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_Analisi_Rilievi, AgroKey_EncoderDecoder, Server) &
            "&s_d=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Server) &
            "&t_f=" & Stringa_Codifica(enum_TipoFiltrone.OperazioniMultiAziendali, AgroKey_EncoderDecoder, Server) &
            "&c_s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
            "&piva=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) & qs_hdr_ftr &
            "&redir=" & Server.UrlEncode(url)

        Response.Redirect(TargetRedirect)

    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Function Link_Pagina_FiltroRicercaNG(piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objParametriAgenda As New ParametriAgenda
            Dim objFiltroRicerca As New AgronicaCoreFiltroneBIZ.FiltroRicerca

            Dim parametriFiltroRicercaNG As New ParametriFiltroRicercaNG With {
                .TipoComportamentoFiltroRicercaNG = Enum_TipoComportamento_FiltroRicerca.SelezionamentoEntita,
                .PaginaProvenienza = enum_PagineAgenda_2010.Pagina_Analisi_Rilievi,
                .SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                .Piva = objParametriAgenda.Piva,
                .TipoMostraGestitiChiamante = New List(Of Enum_TipoMostra_FiltroRicerca) From {Enum_TipoMostra_FiltroRicerca.Impianti, Enum_TipoMostra_FiltroRicerca.Esercizi},
                .FiltriTemporali = objFiltroRicerca.Imposta_FiltroEntitaAttivaAllaData(Date.Now, Enum_Entita_FiltroRicerca.Esercizio)
            }

            r.RispostaStringa = objFiltroRicerca.Link_Pagina_FiltroRicercaNG(piva, parametriFiltroRicercaNG)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CreaEntitaDaChiavi(chiavi As List(Of String)) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametriAgenda As New ParametriAgenda
            objParametriAgenda.Impianti.Clear()

            For Each chiave As String In chiavi
                Dim objImpianto As New ParametriAgenda_Temp.Impianto
                Dim piva As String = chiave.Split("_")(0)
                Dim sa_cod As String = chiave.Split("_")(1)
                Dim appezza As String = chiave.Split("_")(2)
                Dim id_reg As String = chiave.Split("_")(3)
                Dim veg_cod As String = chiave.Split("_")(4)
                Dim progetto_cod As String = chiave.Split("_")(5)

                objImpianto.Piva = piva
                objImpianto.Sa_Cod = sa_cod
                objImpianto.Appezza = appezza
                objImpianto.ID_Reg = id_reg
                objImpianto.Veg_Cod = veg_cod
                objImpianto.Progetto_Cod = progetto_cod

                objParametriAgenda.Impianti.Add(objImpianto)
            Next

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    'Private Class FiltroGrigliaElem
    '    Public ChiaveImpianto As String
    'End Class

    Private Sub Btn_BackToGIS_Click(sender As Object, e As EventArgs) Handles Btn_BackToGIS.Click

        'Dim TargetRedirect = "../../Gis/Gis.aspx?"

        'Dim cliIDTestataTemp As Integer = id_TestataTemp.Value
        'Dim cliFiltroGriglia As String = id_FiltroGriglia.Value
        'If Not String.IsNullOrEmpty(cliFiltroGriglia) Then

        '    Dim js As New System.Web.Script.Serialization.JavaScriptSerializer
        '    Dim arrFiltro = js.Deserialize(Of List(Of FiltroGrigliaElem))(cliFiltroGriglia)
        '    If arrFiltro.Count > 0 Then

        '        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        '        Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        '        cliIDTestataTemp = xAgrosequenze.Agronica_SequenzaTabelle_NuovoID("__tmp_FiltroImpianti", objParametri_Server)

        '        Dim OperazioneCorrente_FiltroImpianti As String = "insert into __tmp_FiltroImpianti (piva, sa_cod, appezza, id_reg, IDTestataTemp) values "

        '        Dim listaFiltroImpianti As New List(Of String)
        '        For idx = 0 To arrFiltro.Count - 1
        '            Dim ci = arrFiltro(idx).ChiaveImpianto.Split("_")



        '            Dim daAggiungere As String = "('" &
        '            ci(0) & "'," &
        '            ci(1) & "," &
        '            ci(2) & "," &
        '            ci(3) & "," &
        '            cliIDTestataTemp &
        '            ")"

        '            If Not listaFiltroImpianti.Contains(daAggiungere) Then
        '                listaFiltroImpianti.Add(daAggiungere)
        '            End If

        '        Next

        '        OperazioneCorrente_FiltroImpianti &= String.Join(",", listaFiltroImpianti)

        '        FiltroneImpiantiDbTemp.PopolaTabellaFiltroImpianti(OperazioneCorrente_FiltroImpianti, objParametri_Server)

        '    End If
        'End If

        'If cliIDTestataTemp <> 0 Then
        '    TargetRedirect = TargetRedirect & "f=true&IDTestataTemp=" & cliIDTestataTemp
        'End If

        ''Response.Redirect(TargetRedirect)
        ''Se la pagina è in un frame il Response.Redirect(...) carica la pagina GIS nel frame a meno che...
        'Response.Write("<script>top.location='" + TargetRedirect + "';parent.location='" + TargetRedirect + "';</script>")

    End Sub


End Class

