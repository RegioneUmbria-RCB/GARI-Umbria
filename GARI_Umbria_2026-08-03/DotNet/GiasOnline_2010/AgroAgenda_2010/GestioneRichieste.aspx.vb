Imports Agronica.Helpers.GiasBase
Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModello.Utility_Agenda
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreVarieBIZ
Imports AgronicaGIS2012.Commons
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Partial Class GestioneRichieste
    Inherits System.Web.UI.Page

    Dim objGestioneRichieste As GestioneRichiesteClasse
    '###############################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim Unid, Cn_Server As String
        Dim objGestioneRichieste As GestioneRichiesteClasse

        ' VAnni: 25/2/2020: recupero la lingua prima che venga ripulita la sessione 
        '   (il try catch vuoto è un orrore di codifica, ma visto l'importanza della pagina ho comunque una scappatoia)
        Dim ssLinguacod As Integer = -1
        Dim sslinguaIso As String = ""

        If Not IsNothing(System.Web.HttpContext.Current.Session("LinguaCorrente")) Then

            Try
                Dim linguaSession As Lingua = CType(System.Web.HttpContext.Current.Session("LinguaCorrente"), Lingua)
                ssLinguacod = linguaSession.Lingua_cod
                sslinguaIso = linguaSession.CodiceISO
            Catch ex As Exception

            End Try

        End If

        ' VAnni: 3/11/2017: da approfondire la necessità del clear..
        Session.Clear()


        If Not Request.QueryString("unid") Is Nothing Then

            Unid = Stringa_Decodifica(
                                    Request.QueryString("unid").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

            Cn_Server = Stringa_Decodifica(
                            Request.QueryString("cn").ToString,
                            AgroKey_EncoderDecoder,
                            Server)


            'devo inserire anche nella querystring 
            'la stringa connessione al superserver, 
            'altrimenti il sito chiamato (che non ha nel webconfig le chiavi per il superserver)
            'non è in grado di leggere l'xml di passaggio parametri
            'che è salvato sul db cliente.
            'quindi tramite la tringa superserver e l'id cn_server può
            'recuperare la stringa connessione per il db cliente e leggere xml
            'Senza superserver andava a leggere sempre da connessione.ini che ora non deve 
            'esser più utilizzato
            If Not Request.QueryString("StrConSup") Is Nothing AndAlso Request.QueryString("StrConSup") <> "" Then
                Dim StringaConnessioneSuperserver As String = ""
                StringaConnessioneSuperserver = Stringa_Decodifica(
                    Request.QueryString("StrConSup").ToString,
                    AgroKey_EncoderDecoder,
                    Server)

                objGestioneRichieste = New GestioneRichiesteClasse(Unid,
                                    Cn_Server,
                                    StringaConnessioneSuperserver,
                                    Server.MapPath("AB_Immagini/IconeVegetali").ToString)

            Else
                'modalità senza superserver
                objGestioneRichieste = New GestioneRichiesteClasse(Unid,
                                                    Cn_Server, "",
                                                    Server.MapPath("AB_Immagini/IconeVegetali").ToString)
            End If
        End If


        Dim objParametri_Super_Server As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


        If Not Request.QueryString("ln") Is Nothing Then
            Dim linguaCodiceISO As String() = Request.QueryString("ln").Split("|")
            ImpostaCultura(New Lingua With {.CodiceISO = linguaCodiceISO(0), .Lingua_cod = linguaCodiceISO(1)})
        Else
            If ssLinguacod > 1 And sslinguaIso <> "" Then
                ImpostaCultura(New Lingua With {.CodiceISO = sslinguaIso, .Lingua_cod = ssLinguacod})
            Else
                Dim DT As DataTable
                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read

                DT = objUtenti.LeggixLingue(objParametri_Utenti.UtenteUsername, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

                If DT.Rows.Count > 0 Then
                    Dim linguaCodice = DT.Rows(0).Item("Lingua_Cod")
                    'Dim leggiLingua As New Lingue_Read
                    'Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(linguaCodice, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
                    Dim linguaCodiceISO As String = DT.Rows(0)("CodiceISO")

                    ImpostaCultura(New Lingua With {.CodiceISO = linguaCodiceISO, .Lingua_cod = linguaCodice})
                Else
                    ImpostaCultura(New Lingua With {.CodiceISO = "it", .Lingua_cod = 1})
                End If
                'ImpostaCultura(New Lingua With {.CodiceISO = "it", .Lingua_cod = 1})
            End If

        End If

        GiasBaseHelper.WarmUp_GestioneRichieste()

        Dim apiController As CoreApiControllerFactory = New CoreApiControllerFactory
        apiController.Inizializza(objParametri_Super_Server, objParametri_Server)

        Dim idSezione = apiController.DammiIdSezioneDaQueryString()

        Dim sideBar As Boolean = True
        If Not IsNothing(Context.Request.QueryString("sidebar")) Then
            sideBar = False
        End If

        Dim cookieDaClientJS As String

        ' VAnni: 6/11/2017: Ripristina il cookie per il redirect su pagina login lato server, se non è sato impostato dalla pagina "index.aspx".
        If AgronicaBase.LinkHomePageGlobale <> "" Then

            cookieDaClientJS =
                AgronicaCoreUtility.Http.CookieLeggi("LinkHomePageGlobale")

            ' Read the cookie information and display it.
            If cookieDaClientJS = "" Then

                AgronicaCoreUtility.Http.CookieImposta("LinkHomePageGlobale", AgronicaBase.LinkHomePageGlobale)

            End If

        End If

        Dim xconf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        cookieDaClientJS = AgronicaCoreUtility.Http.CookieLeggi("LinkHomePageGlobale")
        Dim dtLinkAgenda = xconf.Leggi(0, "LinkAgronicaAgenda2010", "", "", objParametri_Server)


        Dim cookieModificato As String = ""

        'Casadei 15/01/2026 per gestire il caso in cui il cookie venga popolato con un protocollo diverso da quello che dovrebbe essere
        If dtLinkAgenda.Rows.Count > 0 Then
            Dim linkAgendaStr = CStr(dtLinkAgenda(0)("valore"))

            If linkAgendaStr.Contains("http") Then
                cookieModificato = linkAgendaStr
            Else
                Dim dtLinkIndex = xconf.Leggi(0, "LanToWebSiteBasePath", "", "", objParametri_Server)
                If dtLinkIndex.Rows.Count > 0 Then
                    Dim linkPathStr = CStr(dtLinkIndex(0)("valore"))

                    If (linkPathStr.EndsWith("/") AndAlso Not linkAgendaStr.StartsWith("/")) OrElse
                        (Not linkPathStr.EndsWith("/") AndAlso linkAgendaStr.StartsWith("/")) Then
                        cookieModificato = linkPathStr & linkAgendaStr
                    ElseIf linkPathStr.EndsWith("/") AndAlso linkAgendaStr.StartsWith("/") Then
                        cookieModificato = linkPathStr & linkAgendaStr.Substring(1)
                    Else
                        cookieModificato = linkPathStr & "/" & linkAgendaStr
                    End If
                End If
            End If

            cookieModificato = cookieModificato.Replace("GestioneRichieste", "index")

        End If

        If cookieModificato <> "" AndAlso cookieDaClientJS <> cookieModificato Then
            AgronicaCoreUtility.Http.CookieImposta("LinkHomePageGlobale", cookieModificato)
        End If

        ' VAnni: 6/11/2017: la piva super user serve sempre.
        AgronicaCoreUtility.Http.CookieImposta("LinkHomePageGlobalePivaSuperUser", "pivasuperuser=" & objParametri_Server.PivaSuperUser)

        Dim rFF As String = Request.QueryString("FF")
        If Not String.IsNullOrEmpty(rFF) Then
            Response.Redirect("./GestioneLavorazioni/LavorazioneMenu.aspx")
        End If

        '--------------------------------------------------------------
        '---------------VerificaDisciplinare AGENDA 2010---------------
        '--------------------------------------------------------------

        'se non è nothing allora è stato passato l'oggetto ParametriVerificaDisciplinare2010
        'quindi si vuole aprire la pagina VerificaDisciplinare
        If Not IsNothing(Session("ParametriVerificaDisciplinare2010")) Then

            Dim ParametriVerificaDisciplinare2010 As New ParametriVerificaDisciplinare2010
            ParametriVerificaDisciplinare2010.Leggi()
            Session("ParametriVerificaDisciplinare2010") = Nothing

            Dim Impianto_Da_Controllare As AgronicaCoreModello.Anagrafe.Impianto_Colturale
            Dim Disciplinare_Cod As Integer
            Dim Disciplinare_Des As String
            Dim Disciplinare_PubblicoPrivato As Integer
            Dim DataInizio As Date = AGRODATAINIZIO
            Dim DataFine As Date = AGRODATAFINE

            Impianto_Da_Controllare = New AgronicaCoreModello.Anagrafe.Impianto_Colturale(ParametriVerificaDisciplinare2010.Piva, ParametriVerificaDisciplinare2010.Sa_Cod, ParametriVerificaDisciplinare2010.Appezza, ParametriVerificaDisciplinare2010.Id_Reg, ParametriVerificaDisciplinare2010.Progetto_Cod, ParametriVerificaDisciplinare2010.Validita_Inizio, ParametriVerificaDisciplinare2010.Validita_Fine)
            Disciplinare_Cod = ParametriVerificaDisciplinare2010.Cod_Disciplinare
            Disciplinare_PubblicoPrivato = ParametriVerificaDisciplinare2010.Disciplinare_PubblicoPrivato
            Disciplinare_Des = ParametriVerificaDisciplinare2010.Des_Disciplinare

            Dim Elemento_Verifica_Disciplinare As AgronicaCoreModello.Agenda.Util.Elemento_Verifica_Disciplinare

            If ParametriVerificaDisciplinare2010.ListaImpianti.Length > 0 Then

                Dim Lista_Impianti_Da_Controllare As New List(Of AgronicaCoreModello.Anagrafe.Impianto_Colturale)
                Dim ImpiantoDaControllare As AgronicaCoreModello.Anagrafe.Impianto_Colturale
                For i = 0 To ParametriVerificaDisciplinare2010.ListaImpianti.Length - 1
                    ImpiantoDaControllare = New AgronicaCoreModello.Anagrafe.Impianto_Colturale(ParametriVerificaDisciplinare2010.ListaImpianti(i).Piva, ParametriVerificaDisciplinare2010.ListaImpianti(i).Sa_Cod, ParametriVerificaDisciplinare2010.ListaImpianti(i).Appezza, ParametriVerificaDisciplinare2010.ListaImpianti(i).Id_Reg, ParametriVerificaDisciplinare2010.ListaImpianti(i).Progetto_Cod, ParametriVerificaDisciplinare2010.ListaImpianti(i).Validita_Inizio, ParametriVerificaDisciplinare2010.ListaImpianti(i).Validita_Fine)
                    'ImpiantoDaControllare.Piva = ParametriVerificaDisciplinare2010.ListaImpianti(i).Piva
                    'ImpiantoDaControllare.Sa_Cod = ParametriVerificaDisciplinare2010.ListaImpianti(i).Sa_Cod
                    'ImpiantoDaControllare.Appezza = ParametriVerificaDisciplinare2010.ListaImpianti(i).Appezza
                    'ImpiantoDaControllare.ID_Reg = ParametriVerificaDisciplinare2010.ListaImpianti(i).Id_Reg
                    Lista_Impianti_Da_Controllare.Add(ImpiantoDaControllare)
                Next

                Elemento_Verifica_Disciplinare = New AgronicaCoreModello.Agenda.Util.Elemento_Verifica_Disciplinare(0, 0, "", Lista_Impianti_Da_Controllare)

                Elemento_Verifica_Disciplinare.Data_Inizio = ParametriVerificaDisciplinare2010.Validita_Inizio
                Elemento_Verifica_Disciplinare.Data_Fine = ParametriVerificaDisciplinare2010.Validita_Fine

            Else
                Elemento_Verifica_Disciplinare = New AgronicaCoreModello.Agenda.Util.Elemento_Verifica_Disciplinare(Disciplinare_Cod, Disciplinare_PubblicoPrivato, Disciplinare_Des, Impianto_Da_Controllare)
            End If

            Session("Elemento_Verifica_Disciplinare") = Elemento_Verifica_Disciplinare

            Dim TargetRedirect As String = "./GestioneDisciplinari/Verifica_Disciplinare.aspx"

            Dim objParametriAgenda As New ParametriAgenda
            objParametriAgenda.Piva = ParametriVerificaDisciplinare2010.Piva
            objParametriAgenda.Sa_Cod = ParametriVerificaDisciplinare2010.Sa_Cod
            ' objParametriAgenda.Veg_Cod = ParametriVerificaDisciplinare2010.Veg_Cod


            Response.Redirect(TargetRedirect)

            Exit Sub

        End If


        '--------------------------------------------------------------
        '---------------ParametriRicette_2010 -------------------------
        '--------------------------------------------------------------

        'se non è nothing allora è stato passato l'oggetto ParametriRicette_2010
        'quindi si vuole aprire la pagina Ricette_Edit
        If Not IsNothing(Session("ParametriRicette_2010")) Then

            Dim ParametriRicette_2010 As New ParametriRicette_2010
            ParametriRicette_2010.Leggi()
            Session("ParametriRicette_2010") = Nothing

            Dim objAgroWebConfig As New AgroWebConfig
            Dim objParametriAgenda As New ParametriAgenda

            Session("permessoDPIPrivati") = objAgroWebConfig.Flag_DisciplinarePrivato

            Dim Ricetta_Cod As Integer = ParametriRicette_2010.Ricetta_Cod
            Dim Piva As String = ParametriRicette_2010.Piva
            Dim Veg_Cod As Integer = ParametriRicette_2010.Veg_Cod
            Dim Tipo_Operazione As Integer = ParametriRicette_2010.Tipo_Operazione
            Dim Tipo_Ricetta As TipiEnumerativi.enum_TipoRicetta = ParametriRicette_2010.Tipo_Ricetta
            Dim Data_inizio As String = ParametriRicette_2010.data_inizio
            Dim Data_fine As String = ParametriRicette_2010.data_fine
            Dim Ricetta_Des As String = ParametriRicette_2010.Ricetta_Des
            Dim Ricetta_Des_Long As String = ParametriRicette_2010.Ricetta_Des_Long
            Dim Programmazione_Cod As Integer = ParametriRicette_2010.Programmazione_Cod
            Dim SitoOrigine As String = ""
            Dim SitoOrigineCod As Integer
            If Not IsNothing(ParametriRicette_2010.SitoOrigine) Then
                Select Case ParametriRicette_2010.SitoOrigine
                    Case Enum_SiteRedirector.Sito_PianoConcimazione_2017 ' virgola gli altri siti da cui parte dove devono tornare le ricette
                        'DO NOTHING
                        SitoOrigineCod = ParametriRicette_2010.SitoOrigine
                    Case Else
                        If Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi Then
                            Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                            Dim spli() As String = Split(objWebConfig.LinkPianoConcimazione, "/")
                            SitoOrigine = "/" & spli(1) & "/PianoConcimazione_Menu.aspx"  '  "\PianoConcimazione\PianoConcimazione_Menu.aspx"
                        End If

                End Select
            Else
                If Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi Then
                    Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                    Dim spli() As String = Split(objWebConfig.LinkPianoConcimazione, "/")
                    SitoOrigine = "/" & spli(1) & "/PianoConcimazione_Menu.aspx"  '  "\PianoConcimazione\PianoConcimazione_Menu.aspx"
                End If

            End If
            objParametriAgenda.Piva = Piva

            objParametriAgenda.WS_Disciplinari_AgroWS_Disciplinari = objAgroWebConfig.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
            objParametriAgenda.WS_Fitofarmaci_AgroWS_Fitofarmaci = objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
            'Pagina richiesta del sito destinazione
            Dim PaginaRichiesta As Integer = -1
            If Not IsNothing(ParametriRicette_2010.PaginaRichiesta) Then
                PaginaRichiesta = ParametriRicette_2010.PaginaRichiesta
            End If
            'Pagina provenienza del sito origine
            Dim PaginaProvenienza As Integer = -1
            If Not IsNothing(ParametriRicette_2010.PaginaProvenienza) Then
                PaginaProvenienza = ParametriRicette_2010.PaginaProvenienza
            End If


            Session("VariabiliFiltro") = ParametriRicette_2010.StrVariabiliAgenda

            Dim TargetRedirect As String
            Dim destinazionePagina As String

            Select Case SitoOrigine
                Case Enum_SiteRedirector.Sito_PianoConcimazione_2017

                    Select Case PaginaProvenienza
                        Case enum_PaginePianoConcimazione_2017.MenuBS
                            destinazionePagina = "PianoConcimazione_MenuBS.aspx"
                        Case Else
                            destinazionePagina = "Ricette_Manager.aspx"
                    End Select

            End Select



            Select Case PaginaRichiesta
                Case enum_PagineAgenda_2010.Pagina_RicetteStampa
                    TargetRedirect = "./Ricette/Stampa/Ricetta_Stampa.aspx?ricetta_cod=" & Stringa_Codifica(Ricetta_Cod, AgroKey_EncoderDecoder, Server) &
                            "&ricetta_stampa_tipo=" & Stringa_Codifica(CStr(1), AgroKey_EncoderDecoder, Server) &
                            "&destinazione=" & Stringa_Codifica(destinazionePagina, AgroKey_EncoderDecoder, Server) &
                            "&origine=" & Stringa_Codifica(SitoOrigine, AgroKey_EncoderDecoder, Server) &
                            "&sito_or=" & Stringa_Codifica(SitoOrigineCod, AgroKey_EncoderDecoder, Server) &
                            "&p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server)
                Case Else
                    TargetRedirect = "./Ricette/Ricette_Edit.aspx?r=" & Stringa_Codifica(Ricetta_Cod, AgroKey_EncoderDecoder, Server) &
                            "&o=" & Stringa_Codifica(Tipo_Operazione.ToString, AgroKey_EncoderDecoder, Server) &
                            "&destinazione=" & Stringa_Codifica(destinazionePagina, AgroKey_EncoderDecoder, Server) &
                            "&origine=" & Stringa_Codifica(SitoOrigine, AgroKey_EncoderDecoder, Server) &
                            "&sito_or=" & Stringa_Codifica(SitoOrigineCod, AgroKey_EncoderDecoder, Server) &
                            "&p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server) &
                            "&data_inizio=" & Stringa_Codifica(Data_inizio, AgroKey_EncoderDecoder, Server) &
                            "&data_fine=" & Stringa_Codifica(Data_fine, AgroKey_EncoderDecoder, Server) &
                            "&veg_cod=" & Stringa_Codifica(Veg_Cod, AgroKey_EncoderDecoder, Server) &
                            "&tipo_ricetta=" & Stringa_Codifica(Tipo_Ricetta, AgroKey_EncoderDecoder, Server) &
                            "&ricetta_des=" & Stringa_Codifica(Ricetta_Des, AgroKey_EncoderDecoder, Server) &
                            "&ricetta_des_long=" & Stringa_Codifica(Ricetta_Des_Long, AgroKey_EncoderDecoder, Server) &
                            "&programmazione_cod=" & Stringa_Codifica(Programmazione_Cod, AgroKey_EncoderDecoder, Server)

            End Select

            Response.Redirect(TargetRedirect)

            Exit Sub

        End If

        '--------------------------------------------------------------
        '---------------FINE ParametriRicette_2010 --------------------
        '--------------------------------------------------------------



        '--------------------------------------------------------------
        '---------------ParametriFILTRONE_2010 -------------------------
        '--------------------------------------------------------------

        'se non è nothing allora è stato passato l'oggetto ParametriRicette_2010
        'quindi si vuole aprire la pagina Ricette_Edit
        If Not IsNothing(Session("ParametriFILTRONE_2010")) Then

            Dim _ParametriFILTRONE_2010 As New ParametriFILTRONE_2010
            _ParametriFILTRONE_2010.Leggi()
            Session("ParametriFILTRONE_2010") = Nothing

            'Dim objAgroWebConfig As New AgroWebConfig
            'Dim objParametriAgenda As New ParametriAgenda

            'Session("permessoDPI") = objAgroWebConfig.Flag_DisciplinareAttivo
            'Session("permessoDPIPrivati") = objAgroWebConfig.Flag_DisciplinarePrivato

            'Dim Ricetta_Cod As Integer = ParametriRicette_2010.Ricetta_Cod
            'Dim Piva As String = ParametriRicette_2010.Piva
            'Dim Veg_Cod As Integer = ParametriRicette_2010.Veg_Cod
            'Dim Tipo_Operazione As Integer = ParametriRicette_2010.Tipo_Operazione

            'objParametriAgenda.Piva = Piva

            'objParametriAgenda.WS_Disciplinari_AgroWS_Disciplinari = objAgroWebConfig.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
            'objParametriAgenda.WS_Fitofarmaci_AgroWS_Fitofarmaci = objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci

            'Session("VariabiliFiltro") = ParametriRicette_2010.StrVariabiliAgenda

            Dim TargetRedirect As String

            TargetRedirect = "./Filtrone/Filtrone.aspx?p_o=" & _ParametriFILTRONE_2010.Pagina_Origine &
                            "&s_o=" & _ParametriFILTRONE_2010.Sito_Origine &
                            "&p_d=" & _ParametriFILTRONE_2010.Pagina_Destinazione &
                            "&s_d=" & _ParametriFILTRONE_2010.Sito_Destinazione &
                            "&t_f=" & _ParametriFILTRONE_2010.TipoFiltrone &
                            "&c_s=" & _ParametriFILTRONE_2010.CodificaStampe &
                            "&piva=" & _ParametriFILTRONE_2010.Piva &
                            "&v_c=" & _ParametriFILTRONE_2010.Veg_Cod &
                            "&c_c=" & _ParametriFILTRONE_2010.Cul_Cod &
                            "&d_i=" & _ParametriFILTRONE_2010.Data_Inizio &
                            "&d_f=" & _ParametriFILTRONE_2010.Data_Fine &
                            "&ddr=" & _ParametriFILTRONE_2010.DatiDiRitorno

            ' uso il filtrone nuovo se presente parametro in querystring
            If Not IsNothing(Request.QueryString("FiltroNew")) Then
                'Session("FiltroNew") = Request.QueryString("FiltroNew")
                TargetRedirect = TargetRedirect.Replace("Filtrone.aspx", "Filtrone_Nuovo.aspx")
            End If

            Response.Redirect(TargetRedirect)
            Exit Sub
        End If

        '--------------------------------------------------------------
        '---------------FINE ParametriFILTRONE_2010 --------------------
        '--------------------------------------------------------------


        '--------------------------------------------------------------
        '---------------ParametriNONCONFORMITA -------------------------
        '--------------------------------------------------------------

        'se non è nothing allora è stato passato l'oggetto ParametriRicette_2010
        'quindi si vuole aprire la pagina Ricette_Edit
        If Not IsNothing(Session("ParametriNonConformita")) Then

            Dim _ParametriNonConformita As New ParametriNonConformita
            _ParametriNonConformita.Leggi()
            Session("ParametriNonConformita") = Nothing

            Dim TargetRedirect As String

            Select Case _ParametriNonConformita.PaginaRichiesta
                Case enum_PagineAgenda_2010.Pagina_NonCOnformita_Crea
                    TargetRedirect = "./NonConformita/NC_CreaModificaItem.aspx?ncstr=" & _ParametriNonConformita.NC_Str & "&cat_a=" & _ParametriNonConformita.Categoria_Area_Str & "&cat_t=" & _ParametriNonConformita.Categoria_Tipologia_Str
                Case enum_PagineAgenda_2010.Pagina_NonConformita_Lista
                    TargetRedirect = "./NonConformita/NC_Lista.aspx?p=" & _ParametriNonConformita.Piva
            End Select

            Response.Redirect(TargetRedirect)
            Exit Sub
        End If

        '--------------------------------------------------------------
        '---------------FINE ParametriNONCONFORMITA --------------------
        '--------------------------------------------------------------







        '--------------------------------------------------------------
        '---------------OPERAZIONE AGENDA 2010-------------------------
        '--------------------------------------------------------------

        'se non è nothing allora è stato passato l'oggetto objParametriAgenda_2010
        'quindi si vuole aprire una pagina operazione agenda 2010
        If Not IsNothing(Session("ParametriAgenda_2010")) Then

            Dim objParametriAgenda_2010 As New ParametriAgenda_2010
            objParametriAgenda_2010.Leggi()
            'scommentare dopo test
            'Session("ParametriAgenda_2010") = Nothing
            System.Web.HttpContext.Current.Session("ParametriAgenda") = Nothing
            Dim objParametriAgenda As New ParametriAgenda

            'testare
            objParametriAgenda.SitoOrigine = HttpContext.Current.Session("Sito_Origine")
            objParametriAgenda.PaginaSitoOrigine = objParametriAgenda_2010.PaginaProvenienza

            Dim TargetRedirect As String


            ' VAnni: 5/9/2017: 'Loggo il login, in gestione richieste solo nel caso in cui non si fa un log completo: modalità "aggiornamento".
            'questo evita un sovraccarico sulla tabella.

            Dim dtConf As DataTable = xconf.Leggi(0, "LoginLogAccessoTutti", "", "", objParametri_Server)

            Dim loggaTutto As Boolean = False
            Dim richiediUpdate As Boolean = False

            If dtConf.Rows.Count > 0 AndAlso CStr(dtConf(0)("valore")).ToLower = "true" Then
                loggaTutto = True
            End If

            If Not loggaTutto Then
                Dim objAWS_Log As New AgronicaCoreUtentiBIZ.AWS_Log_W
                objAWS_Log.AccessoLog(
                    Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                    Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                    objParametriAgenda_2010.PaginaProvenienza,
                    objParametriAgenda_2010.PaginaRichiesta,
                    False,
                    objParametri_Server,
                    objParametri_Utenti)
            End If


            'VAnni: 12/9/2017: spostata la verifica...nella selezione da filtrino di un'impresa, rimane qui il solo redirect su profitosan e condizionalità.
            Dim xLeggiCfgVerifica As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dtLeggiCfgVerifica As DataTable =
                xLeggiCfgVerifica.Leggi(0, "VerificaAnagraficaImpreseProfilate", "", "", objParametri_Server)
            If dtLeggiCfgVerifica.Rows.Count > 0 AndAlso CStr(dtLeggiCfgVerifica.Rows(0)("valore")).ToLower = "true" Then

                'verifica il solo redirect per il profitosan
                Dim xLetturaServizi As New AgronicaCoreProfilazioneBIZ.Pratiche_R
                Dim r As RispostaStandard = xLetturaServizi.leggiServiziStati(enum_WWorflow.Servizi_Agronica_2017, "", 0, objParametri_Server.UtenteUsername, objParametri_Server, objParametri_Utenti)

                Dim autoRedirectAudit As Boolean = False

                If r.RispostaOK Then
                    Dim jArray1 As JArray = JArray.Parse(r.RispostaStringa)
                    Dim xSoloPonteRapido As List(Of Integer) = (
                        From jj In jArray1
                        Select CInt(jj("ServizioCod"))
                     ).Distinct.ToList

                    If xSoloPonteRapido.Count = 1 AndAlso xSoloPonteRapido.First = enum_Servizi.Profitosan Then

                        TargetRedirect = profitosan.getLinkSimple(True, HttpContext.Current.Request, New AgronicaCoreGestioneRichieste.AgroWebConfig, objParametri_Server, objParametri_Utenti)
                        Response.Redirect(TargetRedirect)
                    End If

                    If xSoloPonteRapido.Count = 1 AndAlso xSoloPonteRapido.First = enum_Servizi.Condizionalita2018 Then

                        autoRedirectAudit = True

                    End If

                End If

                'verifica la corretta impostazione delle aziende per le aziende generate dalla web API profilatore
                Dim verificaImpreseBasataSuProfioUtente As Boolean = False
                Dim letturaProfilo As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R
                verificaImpreseBasataSuProfioUtente = letturaProfilo.UTENTE_Attiva_Configurazione_Pratica_FlagAttivo(objParametri_Utenti.UtenteUsername, objParametri_Utenti)

                If verificaImpreseBasataSuProfioUtente Then
                    Dim xTestVerificaImpre As New AgronicaCoreAnagrafeBIZ.Impresa_R
                    Dim xFiltro As String = "1051,1052,1053,1054,1055"
                    Dim lImp As List(Of String) = xTestVerificaImpre.VerificaSituazioneImpreseDataVisibilitàUtente(objParametri_Server.UtenteUsername, xFiltro, objParametri_Server, objParametri_Utenti)

                    Dim xLeggiVisibilità As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
                    Dim filtroImpostato As String = xLeggiVisibilità.Leggi_FiltroUtenteSQL(objParametri_Utenti.UtenteUsername, 5, "", "", objParametri_Utenti)
                    Dim vFiltro As String() = filtroImpostato.Split("'")
                    If vFiltro.Length = 3 And autoRedirectAudit Then
                        objParametriAgenda.Piva = vFiltro(1)
                    End If

                    'redireziona sul sincro..
                    If lImp.Count > 0 Then
                        TargetRedirect = RedirectGestione.IndirizzoCompleto_Sitosincronizzatore_PassandoDirettamenteIParametri(
                            Enum_SiteRedirector.Sito_AgronicaAgenda_2010, enum_PagineAgronicaSincro.ImportazionePC_Anteprima,
                            0, objParametriAgenda.Piva, 0)

                        Response.Redirect(TargetRedirect)

                    Else

                        'tutto ok, 
                        If autoRedirectAudit Then

                            Dim LinkAgronicaAgenda2010 As String = ""
                            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                            Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "LinkAgronicaAgenda2010", "", "", objParametri_Server)

                            If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
                                LinkAgronicaAgenda2010 = DTConfigSiti.Rows(0).Item("Valore")
                            End If

                            Dim script As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_Sito_AgronicaAuditSicurezzaGlobalCoop_PassandoDirettamente_Parametri(
                                                TipiEnumerativi.Enum_SiteRedirector.Sito_GiasOnline, enum_AuditPuaTipo.Audit_Condizionalita,
                                                HttpContext.Current.Session("ASG_Utente_CodFiscale").ToString, objParametriAgenda.Piva, 0, LinkAgronicaAgenda2010)

                            TargetRedirect = script.Split("'")(3) & "&redir=1"
                            Response.Redirect(TargetRedirect)
                        End If

                    End If

                End If

            End If


            If ChiamatoDaAngularMenuAgendaQdC(objParametriAgenda) Then
                SetObjParametriAgenda(objParametriAgenda, objParametriAgenda_2010)
            End If

            If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG Then
                SetObjParametriAgenda(objParametriAgenda, objParametriAgenda_2010)
            End If

            Dim objAgroWebConfig As New AgroWebConfig
            objParametriAgenda.Piva = objParametriAgenda_2010.Piva
            objParametriAgenda.WS_Disciplinari_AgroWS_Disciplinari = objAgroWebConfig.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
            objParametriAgenda.WS_Fitofarmaci_AgroWS_Fitofarmaci = objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci

            Session("permessoDPIPrivati") = objAgroWebConfig.Flag_DisciplinarePrivato

            If Not String.IsNullOrEmpty(objParametriAgenda.RedirectUrl) Then
                ChangeRedirectForMenuAgenda(
                    Response, idSezione, TargetRedirect,
                    objParametriAgenda, objParametri_Server, objParametri_Utenti
                )


                Dim targetUrl As String = ""
                If objParametriAgenda.RedirectUrl.StartsWith("..") Then
                    targetUrl = objParametriAgenda.RedirectUrl.Replace("..", ".")
                ElseIf objParametriAgenda.RedirectUrl.Contains("http") Then
                    targetUrl = objParametriAgenda.RedirectUrl
                End If

                If targetUrl.ToLowerInvariant.Contains("[piva]") Then
                    targetUrl = targetUrl.Replace("[piva]", Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session))
                End If

                If idSezione <> 0 Then

                    MenuBS_2017.GestioneRedirectSezione(idSezione, objParametriAgenda.Piva, targetUrl)

                    If apiController.CanUseAPI AndAlso apiController.VersioneHeader = "2022" Then
                        targetUrl = AgronicaCoreUtility.Varie.aggiungiAQueryString(targetUrl, "idBC", Stringa_Codifica(idSezione, AgroKey_EncoderDecoder))
                    End If

                    If Not sideBar Then
                        targetUrl = AgronicaCoreUtility.Varie.aggiungiAQueryString(targetUrl, "sidebar", "off")
                    End If


                End If

                If idSezione <> 0 AndAlso idSezione <> -1 Then
                    ' se ho id sezione verifico casi puntuali
                    Select Case idSezione
                        Case enum_Sezioni_MenuBS_2017.MENU_NUOVA_VISITE
                            objParametriAgenda.RedirectUrl = ""
                            targetUrl = targetUrl.Replace("..", ".")
                    End Select
                End If

                Response.Redirect(targetUrl)

            End If

            Select Case objParametriAgenda_2010.PaginaRichiesta

                Case enum_PagineAgenda_2010.Pagina_RicetteStampa
                    TargetRedirect = "./Ricette/Stampa/Ricetta_Stampa.aspx"
                    TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino ' contiene ricetta_cod e ricetta_stampa_tipo

                Case enum_PagineAgenda_2010.Pagina_ZooAltreLavorazioni
                    TargetRedirect = "./Zoo/Zoo_Altre_Lavorazioni.aspx"

                Case enum_PagineAgenda_2010.Pagina_ZooAlimentazione
                    TargetRedirect = "./Zoo/Zoo_Alimentazione.aspx"

                Case enum_PagineAgenda_2010.Pagina_GestioneRifiuti
                    TargetRedirect = "./Operazioni/GestioneRifiuti.aspx"

                Case enum_PagineAgenda_2010.Pagina_TrattamentiPostRaccolta
                    TargetRedirect = "./Operazioni/Trattamenti_PostRaccolta.aspx"

                Case enum_PagineAgenda_2010.Pagina_IrrigazioneBS
                    TargetRedirect = "./Operazioni/IrrigazioneBS.aspx"
                    TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino

                Case enum_PagineAgenda_2010.Pagina_RilieviBS
                    TargetRedirect = "./Operazioni/RilieviBS.aspx"

                    Try
                        Dim attivita_str = objParametriAgenda_2010.GenericObj_string

                        Dim settings As New JsonSerializerSettings()
                        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

                        Dim attivita = JsonConvert.DeserializeObject(Of AgronicaCoreModelsSTD.attivita.Attivita)(attivita_str, settings)

                        If IsNothing(attivita) Then
                            Throw New Exception
                        End If

                        objParametriAgenda.Impianti = getImpianti_ParametriAgendaTemp(objParametriAgenda_2010.Impianti)
                        objParametriAgenda.ImpostaPerRientro()

                        objParametriAgenda.Rilievi = New List(Of rilievoAvv)

                        objParametriAgenda.Disciplinare = "0"

                        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Trattamenti

                        objParametriAgenda.Data = attivita.inizio

                        objParametriAgenda.Id_Agenda = "0"
                        objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                        If Not IsNothing(attivita.centroAziendale) Then
                            objParametriAgenda.Sa_Cod = attivita.centroAziendale.primaryKey.codice.ToString()
                        End If

                        If Not IsNothing(attivita.utilizzoTerreno) Then
                            If attivita.utilizzoTerreno.classType = ClassType.Varieta Then
                                objParametriAgenda.Veg_Cod = CType(attivita.utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta).specie.codice.ToString()
                            Else
                                objParametriAgenda.Veg_Cod = "0"
                            End If
                        End If

                        Dim lavCodFinale As String = objParametriAgenda.Lav_Cod ' 79 per FaseFenologica, 0 per RilievoAvversita

                        Select Case lavCodFinale

                            Case LAVCOD_FASI_FENOLOGICHE

                            Case Else

                                Dim udmFinale As Integer = 0
                                Dim valoreFinale As String = ""
                                Dim av_cod As Integer = 0

                                For Each r In attivita.risorse

                                    If r.classType = ClassType.DettaglioTrattamento Then
                                        Dim ris = CType(r, AgronicaCoreModelsSTD.attivita.dettagli.DettaglioTrattamento)


                                        If Not IsNothing(ris.soglia) AndAlso Not IsNothing(ris.soglia.lavorazione) AndAlso Not IsNothing(ris.soglia.udm) Then

                                            lavCodFinale = ris.soglia.lavorazione.primaryKey.codice
                                            udmFinale = ris.soglia.udm.codice

                                            If ris.soglia.quantita > 0 Then
                                                valoreFinale = ris.soglia.quantita
                                            Else
                                                valoreFinale = "1"
                                            End If
                                        End If

                                        'solo avversità singole per le soglie
                                        If Not IsNothing(ris.avversitaGruppo) Then
                                            av_cod = ris.avversitaGruppo.codice
                                        End If

                                        Exit For

                                    End If

                                Next

                                For Each impianto In objParametriAgenda.Impianti
                                    objParametriAgenda.Rilievi.Add(New rilievoAvv With {
                                                                    .Av_cod = av_cod,
                                                                    .Udm_cod = udmFinale,
                                                                    .Valore = valoreFinale,
                                                                    .Impianto = impianto
                                                                })
                                Next


                                If attivita.disciplinare IsNot Nothing Then
                                    objParametriAgenda.Disciplinare = "" &
                                                                        attivita.disciplinare.codice & "/" &
                                                                        If(attivita.disciplinare.raggruppamentiColturaliDPI IsNot Nothing, attivita.disciplinare.raggruppamentiColturaliDPI.codice, 0) & "/" &
                                                                        If(attivita.disciplinare.gruppoFinalita IsNot Nothing, attivita.disciplinare.gruppoFinalita.codice, 0) & "/" &
                                                                        attivita.disciplinare.flagProtetto & "/" &
                                                                        attivita.disciplinare.disciplinarePubblicoPrivato
                                Else
                                    objParametriAgenda.Disciplinare = "" &
                                                                        0 & "/" &
                                                                        0 & "/" &
                                                                        0 & "/" &
                                                                        0 & "/" &
                                                                        0
                                End If

                                '(13/03/2019 fede) forzo anche per le trappole il rilievo avv in campo per ora
                                lavCodFinale = LAVCOD_RILIEVO_AVVERSITA_CAMPO

                        End Select

                        'nuovo lav_cod
                        objParametriAgenda.Lav_Cod = lavCodFinale

                        'nuovi movimenti (da capire i movimenti di scarico magazzino e costi accessori)
                        objParametriAgenda.Movimenti = New List(Of Movimento)
                        objParametriAgenda.Movimenti.Add(New Movimento With {.Cau_Mov = CAU_RILIEVO_CAMPO, .Data_Registrazione = attivita.inizio})

                        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura

                        objParametriAgenda.salva()

                        TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino

                    Catch ex As Exception
                        ' Se un'eccezione viene generata, allora non ho un oggetto Attivita in GenericObj_string, allora non faccio nulla
                    End Try

                Case enum_PagineAgenda_2010.Pagina_Zoo_Scarico
                    TargetRedirect = "./Zoo/Zoo_Scarico.aspx"

                Case enum_PagineAgenda_2010.Pagina_Zoo_Pesatura
                    TargetRedirect = "./Zoo/Zoo_Pesatura.aspx"

                Case enum_PagineAgenda_2010.Pagina_Zoo_Spostamento
                    TargetRedirect = "./Zoo/Zoo_Spostamento.aspx"

                Case enum_PagineAgenda_2010.Pagina_Zoo_Carico
                    TargetRedirect = "./Zoo/Zoo_Carico.aspx"

                Case enum_PagineAgenda_2010.Pagina_Zoo_Trattamento
                    TargetRedirect = "./Zoo/Zoo_Trattamento.aspx"

                ' Operazione Deperecata
                'Case enum_PagineAgenda_2010.Pagina_Operazione_Di_Cura
                '    TargetRedirect = "./GestioneMagazzini/OperazioneDiCura.aspx"

                Case enum_PagineAgenda_2010.Pagina_Raccolta
                    TargetRedirect = "./Operazioni/Raccolta.aspx"
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
                    objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lavorazione
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_MonitoraggioCorpiEstranei
                    Dim PaginaLink As String
                    PaginaLink = "./GestioneMonitoraggioCorpiEstranei/GestioneMonitoraggioCE.aspx" &
                                "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)

                    TargetRedirect = PaginaLink

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                Case enum_PagineAgenda_2010.Pagina_DocumentoContabileGenerico

                    Select Case objParametriAgenda.SitoOrigine
                        Case Enum_SiteRedirector.Sito_PianoConcimazione_2017

                            Dim tipo As Integer = 0
                            Dim sa_cod As Integer = 0 '0 per tutti i doc contabili

                            objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                            objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lavorazione
                            objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                            objParametriAgenda.TornaASitoOrigine = True

                            TargetRedirect = "./GestioneContabilita/DocumentoContabileGenerico.aspx" &
                            "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                            "&o=" & Stringa_Codifica(objParametriAgenda.Tipo_Operazione, AgroKey_EncoderDecoder) &
                            "&s=" & Stringa_Codifica(sa_cod, AgroKey_EncoderDecoder) &
                            "&i=" & Stringa_Codifica(objParametriAgenda.Id_Agenda, AgroKey_EncoderDecoder) &
                            "&l=" & Stringa_Codifica(objParametriAgenda.Lav_Cod, AgroKey_EncoderDecoder) &
                            "&d=" & Stringa_Codifica(objParametriAgenda.Data, AgroKey_EncoderDecoder) &
                            "&tf=" & Stringa_Codifica(tipo, AgroKey_EncoderDecoder) &
                            "&rs=" & Stringa_Codifica(objParametriAgenda.RagSoc, AgroKey_EncoderDecoder) &
                            objParametriAgenda_2010.QueryStringFiltrino
                            '"&orig=" & Stringa_Codifica(PaginaSitoAgendaOrigine, AgroKey_EncoderDecoder)

                            Select Case objParametriAgenda.Lav_Cod
                                Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA

                                    Dim objconfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                                    Dim DocumentoRicevutoLight_str As String = objconfigSiti.Leggi_Valore(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, "DocumentoRicevutoLight", "", "", objParametri_Server)
                                    Dim DocumentoRicevutoLight As Boolean = If(DocumentoRicevutoLight_str = "", False, DocumentoRicevutoLight_str)

                                    If DocumentoRicevutoLight = True Then
                                        Dim mode As String = If(objParametriAgenda.Lav_Cod = LAVCOD_BOLLA_RICEVUTA, "bolla", "fattura")

                                        Dim OpUtil As New AgronicaCoreModello.Utility_Operazioni
                                        TargetRedirect = OpUtil.LinkPagina_from_LavCod_NEW(LAVCOD_CARICO, objParametriAgenda, DocumentoRicevutoLight:=DocumentoRicevutoLight).Replace("../", "./") &
                                                        "&light=" & Stringa_Codifica("true", AgroKey_EncoderDecoder) &
                                                        "&lcl=" & Stringa_Codifica(objParametriAgenda.Lav_Cod, AgroKey_EncoderDecoder) &
                                                        "&mol=" & Stringa_Codifica(mode, AgroKey_EncoderDecoder) &
                                                        objParametriAgenda_2010.QueryStringFiltrino

                                    End If
                            End Select


                        Case Else

                            TargetRedirect = "./GestioneContabilita/DocumentoContabileGenerico.aspx"

                            TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino
                            objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                            objParametriAgenda.TornaASitoOrigine = True

                    End Select



                Case enum_PagineAgenda_2010.Pagina_FormProdotto

                    Dim PaginaLink As String = "./GestioneMagazzini/FormProdotto.aspx"

                    Dim xChiave As String = objParametriAgenda_2010.Chiave
                    If xChiave = "" Then
                        Call Albero.ChiaveAlbero_Codifica(xChiave,
                                                        enum_TipoNodo.p_PortafoglioProdotti,
                                                        objParametriAgenda_2010.Piva,
                                                        objParametriAgenda_2010.Sa_Cod, , , , , , , , , , , ,
                                                        0)
                    End If

                    Dim Lav_Cod As Integer = LAVCOD_CARICO
                    If objParametriAgenda_2010.Lavorazione <> 0 Then
                        Lav_Cod = objParametriAgenda_2010.Lavorazione
                    End If

                    Dim mode As String = "magazzino"
                    If objParametriAgenda_2010.Mode <> "" Then
                        mode = objParametriAgenda_2010.Mode
                    End If

                    Dim strExit As String = "true"
                    Dim Origine As String = 0
                    'Select Case objParametriAgenda.PaginaSitoOrigine
                    '    Case 
                    'End Select
                    'If objParametriAgenda.PaginaSitoOrigine = enum_PagineGiasOnline.MenuMagazzini Then
                    'strExit = "false"
                    'Origine = enum_PagineGiasOnline.MenuMagazzini
                    'End If
                    strExit = "false"
                    Origine = objParametriAgenda.PaginaSitoOrigine

                    If ChiamatoDaAngularMenuAgendaQdC(objParametriAgenda) Then
                        TargetRedirect = PaginaLink & objParametriAgenda_2010.QueryStringFiltrino
                    Else
                        TargetRedirect = PaginaLink &
                                "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder) &
                                "&c=" & Stringa_Codifica(objParametriAgenda_2010.OperazioneMagazzino, AgroKey_EncoderDecoder) &
                                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder) &
                                "&orig=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder) &
                                "&mode=" & Stringa_Codifica(mode, AgroKey_EncoderDecoder) &
                                "&l=" & Stringa_Codifica(Lav_Cod, AgroKey_EncoderDecoder) &
                                "&d=" & Stringa_Codifica(CStr(objParametriAgenda_2010.DataSelezionata), AgroKey_EncoderDecoder) &
                                "&s=" & Stringa_Codifica(objParametriAgenda_2010.Sa_Cod, AgroKey_EncoderDecoder) &
                                "&a=" & Stringa_Codifica(objParametriAgenda_2010.Id_Agenda, AgroKey_EncoderDecoder) &
                                "&exit=" & strExit
                    End If


                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
                    objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
                    objParametriAgenda.TornaASitoOrigine = True




                Case enum_PagineAgenda_2010.Pagina_Fertilizzazione

                    Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim DTConfigSiti As DataTable

                    DTConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaBS", "", "", objParametri_Server)

                    If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
                        TargetRedirect = "./Operazioni/Trattamenti_2.aspx"
                    Else
                        TargetRedirect = "./Operazioni/Fertilizzazione.aspx"
                    End If

                    'If Not IsNothing(ConfigurationSettings.AppSettings("agenda_boot")) AndAlso ConfigurationSettings.AppSettings("agenda_boot") = True Then
                    '    TargetRedirect = "./Operazioni/Trattamenti_2.aspx"
                    'Else
                    '    TargetRedirect = "./Operazioni/Fertilizzazione.aspx"
                    'End If
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
                    objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lavorazione
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda

                    If objParametriAgenda_2010.Impianti IsNot Nothing AndAlso objParametriAgenda_2010.Impianti.Count > 0 Then
                        Dim impiantiList As New List(Of ParametriAgenda_Temp.Impianto)
                        For Each imp In objParametriAgenda_2010.Impianti
                            Dim impianto As New ParametriAgenda_Temp.Impianto
                            impianto.Piva = imp.Piva
                            impianto.Sa_Cod = imp.Sa_Cod
                            impianto.Appezza = imp.Appezza
                            impianto.ID_Reg = imp.Id_Reg
                            impiantiList.Add(impianto)
                        Next
                        objParametriAgenda.Impianti = impiantiList
                    End If

                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Trattamenti

                    TargetRedirect = "./Operazioni/Trattamenti_2.aspx"

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Appezza = objParametriAgenda_2010.Appezza
                    objParametriAgenda.Id_Imp = objParametriAgenda_2010.Id_Reg
                    objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
                    objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lavorazione
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
                    objParametriAgenda.TipoOperazioneAgenda = objParametriAgenda_2010.TipoOperazioneAgenda
                    objParametriAgenda.TargetOperazione = objParametriAgenda_2010.TargetOperazione
                    objParametriAgenda.Programmazione_Cod = objParametriAgenda_2010.Programmazione_Cod

                    If objParametriAgenda_2010.Impianti IsNot Nothing AndAlso objParametriAgenda_2010.Impianti.Count > 0 Then
                        Dim listImpianti As New List(Of ParametriAgenda_Temp.Impianto)
                        For Each imp As Impianti_2010 In objParametriAgenda_2010.Impianti
                            Dim importImp = New ParametriAgenda_Temp.Impianto
                            importImp.Piva = imp.Piva
                            importImp.Sa_Cod = imp.Sa_Cod
                            importImp.Appezza = imp.Appezza
                            importImp.ID_Reg = imp.Id_Reg
                            listImpianti.Add(importImp)
                        Next
                        objParametriAgenda.Impianti = listImpianti
                    End If

                    objParametriAgenda.TornaASitoOrigine = True
                    objParametriAgenda.QueryStringFiltrino = objParametriAgenda_2010.QueryStringFiltrino

                    TargetRedirect += objParametriAgenda_2010.QueryStringFiltrino

                    objParametriAgenda.salva()

                Case enum_PagineAgenda_2010.Pagina_Trattamenti_B
                    TargetRedirect = "./Operazioni/Trattamenti_2.aspx"

                    ' VAnni: 29/5/2019: se sto provenendo dal PUA 2019 allora ho impostato un Lav_Cod Negativo.
                    If objParametriAgenda_2010.Lavorazione < 0 Then
                        objParametriAgenda_2010.Lavorazione = Math.Abs(objParametriAgenda_2010.Lavorazione)
                        objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta
                        'objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio
                        objParametriAgenda.TipoRicetta = enum_TipoRicetta.PianoDistribuzionePua
                        objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale

                        'la testata della ricetta  - parcheggiato in id_Agenda
                        TargetRedirect &= "?r=" & Stringa_Codifica(objParametriAgenda_2010.Id_Agenda, AgroKey_EncoderDecoder, Server)
                        objParametriAgenda_2010.Id_Agenda = 0

                        'il Disciplinare?
                        objParametriAgenda.Cul_Cod = objParametriAgenda_2010.Cul_Cod

                        If objParametriAgenda_2010.Appezza <> 0 AndAlso objParametriAgenda_2010.Id_Reg <> 0 Then
                            Dim Imp As New ParametriAgenda_Temp.Impianto
                            Imp.Piva = objParametriAgenda_2010.Piva
                            Imp.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                            Imp.Appezza = objParametriAgenda_2010.Appezza
                            Imp.ID_Reg = objParametriAgenda_2010.Id_Reg
                            objParametriAgenda.Impianti.Add(Imp)
                        End If

                    End If
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
                    objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lavorazione
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Lavorazioni

                    Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim DTConfigSiti As DataTable

                    DTConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaBS", "", "", objParametri_Server)

                    If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
                        TargetRedirect = "./Operazioni/Trattamenti_2.aspx"
                    Else
                        TargetRedirect = "./Operazioni/Lavorazioni.aspx"
                    End If

                    'TargetRedirect = "./Operazioni/Lavorazioni.aspx"
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
                    objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lavorazione
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Installazione_Trapppole
                    TargetRedirect = "./Operazioni/Installazione_Trappole.aspx"
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
                    objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lavorazione
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
                    objParametriAgenda.TornaASitoOrigine = True


                Case enum_PagineAgenda_2010.Pagina_Reinnesco_Trapppole
                    TargetRedirect = "./Operazioni/Reinnesco_Rilievi_Trappole.aspx"
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
                    objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lavorazione
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Rilievi

                    'TargetRedirect = "./Operazioni/Rilievi.aspx"
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
                    objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lavorazione
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
                    objParametriAgenda.TornaASitoOrigine = True

                    TargetRedirect = "./Operazioni/Rilievi.aspx"

                'Select Case objParametriAgenda.Lav_Cod
                '    Case LAVCOD_FASI_FENOLOGICHE
                '        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                '        Dim DTConfigSiti As DataTable
                '        Dim objParametri_Server As AgronicaCoreParametri
                '        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

                '        DTConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaBS", "", "", objParametri_Server)

                '        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
                '            TargetRedirect = "./Operazioni/Rilievi_2.aspx"
                '        Else
                '            TargetRedirect = "./Operazioni/Rilievi.aspx"
                '        End If
                '    Case Else
                '        TargetRedirect = "./Operazioni/Rilievi.aspx"
                'End Select


                Case enum_PagineAgenda_2010.Pagina_Irrigazione
                    TargetRedirect = "./Operazioni/Irrigazione.aspx"
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
                    objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lavorazione
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_RilievoPiogge
                    TargetRedirect = "./Operazioni/RilievoPiogge.aspx"
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
                    objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lavorazione
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Semina_Trapianto
                    TargetRedirect = "./Operazioni/Semina_E_Trapianto_1.aspx"
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
                    objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lavorazione
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
                    Dim impianti As New List(Of ParametriAgenda_Temp.Impianto)
                    If objParametriAgenda_2010.Appezza <> 0 AndAlso objParametriAgenda_2010.Id_Reg <> 0 Then
                        Dim Imp As New ParametriAgenda_Temp.Impianto
                        Imp.Piva = objParametriAgenda.Piva
                        Imp.Sa_Cod = objParametriAgenda.Sa_Cod
                        Imp.Appezza = objParametriAgenda_2010.Appezza
                        Imp.ID_Reg = objParametriAgenda_2010.Id_Reg
                        impianti.Add(Imp)
                    End If
                    objParametriAgenda.Impianti = impianti

                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Distribuzione_Insetti
                    TargetRedirect = "./Operazioni/Distribuzione_Insetti.aspx"
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
                    objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lavorazione
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Ricette_Lista
                    TargetRedirect = "./Ricette/Ricette_Manager.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Ricette_Edit
                    TargetRedirect = "./Ricette/Ricette_Edit.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Gis_StrumentoDiRipartoCatasto
                    TargetRedirect = "./Gis/RipartoCatasto.aspx" & objParametriAgenda_2010.QueryStringFiltrino
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    Session("_piva") = objParametriAgenda_2010.Piva
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Gis_ImportazioneDati
                    TargetRedirect = "./Gis/CaricaShape.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    Session("_piva") = objParametriAgenda_2010.Piva
                    objParametriAgenda.TornaASitoOrigine = True


                Case enum_PagineAgenda_2010.Pagina_Gis_EsportazioneDati
                    TargetRedirect = $"./Gis/EsportaPF.aspx?piva={objParametriAgenda_2010.Piva}&sa_cod={objParametriAgenda_2010.Sa_Cod}"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    Session("_piva") = objParametriAgenda_2010.Piva
                    Session("_sa_cod") = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.TornaASitoOrigine = True

                    objParametriAgenda.salva()

                Case enum_PagineAgenda_2010.Menu

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.TornaASitoOrigine = True

                    Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim DTConfigSiti As DataTable

                    DTConfigSiti = objConfigSiti.Leggi(0, "MenuBS_2017", "", "", objParametri_Server)

                    If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then

                        TargetRedirect = "./menu/MenuBS_2017.aspx"

                        ' (se provengo dall'index) cancello i 100 web parametri più vecchi di un giorno
                        'If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 AndAlso objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_index Then
                        Dim objSatelliti As New AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti
                        objSatelliti.PulisciParametriGias(objParametri_Server)
                        'End If

                        ' salta alla pagina di menu selezionata + filtro aziende
                        If Not IsNothing(Request.QueryString("IDSezione")) AndAlso Request.QueryString("IDSezione") <> "0" Then
                            TargetRedirect &= "?IDSezione=" & CInt(Request.QueryString("IDSezione"))
                            If Not sideBar Then
                                TargetRedirect = AgronicaCoreUtility.Varie.aggiungiAQueryString(TargetRedirect, "sidebar", "off")
                            End If

                            If Not IsNothing(Request.QueryString("FiltroAziende")) Then
                                TargetRedirect &= "&FiltroAziende="
                            End If
                        End If

                        ' cambia azienda selezionata
                        If Not IsNothing(Request.QueryString("PivaSelezionata")) Then
                            Dim objImpre As New AgronicaCoreAnagrafeDAL.Imprese_Read
                            objParametriAgenda_2010.Piva = Request.QueryString("PivaSelezionata")
                            objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                            If objParametriAgenda.Piva <> "" Then
                                objParametriAgenda.RagSoc = objImpre.RagSoc_from_Piva(objParametriAgenda.Piva, objParametri_Server)
                            Else
                                objParametriAgenda.RagSoc = ""
                            End If
                            objParametriAgenda.salva()
                            HttpContext.Current.Session("_Piva") = Request.QueryString("PivaSelezionata")
                        End If

                    Else

                        DTConfigSiti = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)

                        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
                            TargetRedirect = "./menu/menubs_agenda_nuovo.aspx"
                        Else
                            TargetRedirect = "./menu/menu.aspx"
                        End If

                    End If

                'If Not IsNothing(ConfigurationSettings.AppSettings("MenuAgendaBS")) AndAlso ConfigurationSettings.AppSettings("MenuAgendaBS") = True Then
                '    TargetRedirect = "./menu/menubs_agenda_nuovo.aspx"
                'Else
                '    TargetRedirect = "./Menu/Menu.aspx"
                'End If

                'Case enum_PagineAgenda_2010.Pagina_DocumentoContabileGenerico
                '    TargetRedirect = "./GestioneContabilita/DocumentoContabileGenerico.aspx"
                '    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                '    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_FiltrinoImprese

                    objParametriAgenda.PaginaSitoOrigine = objParametriAgenda_2010.PaginaProvenienza
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.TornaASitoOrigine = True
                    objParametriAgenda.QueryStringFiltrino = objParametriAgenda_2010.QueryStringFiltrino
                    objParametriAgenda.SitoDestinazioneFiltrino = objParametriAgenda_2010.SitoDestinazioneFiltrino
                    objParametriAgenda.PaginaDestinazioneFiltrino = objParametriAgenda_2010.PaginaDestinazioneFiltrino
                    If objParametriAgenda_2010.PaginaDestinazioneFiltrino = enum_PagineProfilazione_2010.Pagina_Utenti_Visibilita Then

                        objParametriAgenda.Cod_Contatto = objParametriAgenda_2010.Chiave
                        TargetRedirect = "./Filtrino/FiltrinoImprese.aspx?d=" & Sicurezza.Stringa_Codifica("./Utenti/Utenti_Visibilita.aspx", AgroKey_EncoderDecoder, Server)
                    Else
                        Dim baseFiltrinoUrl = "./Filtrino/FiltrinoImprese.aspx?"

                        If objParametriAgenda.QueryStringFiltrino.StartsWith("?") Then
                            baseFiltrinoUrl = "./Filtrino/FiltrinoImprese.aspx"
                        End If

                        TargetRedirect = baseFiltrinoUrl & objParametriAgenda.QueryStringFiltrino
                    End If

                Case enum_PagineAgenda_2010.Pagina_GestioneMagazziniBS

                    TargetRedirect = "./GestioneMagazzini/GestioneMagazziniBS.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Fabbricato = objParametriAgenda_2010.Fabbricato
                    objParametriAgenda.TornaASitoOrigine = True

                    Try

                        Dim elem_cod = 0
                        Dim pro_cod = 0
                        Dim mat_cod = 0
                        Dim fabbricato_cod = ""
                        Dim piva = objParametriAgenda.Piva
                        Dim sa_cod = 0
                        Dim lav_cod = "0"
                        Dim data As New Date

                        Dim attivita_str = objParametriAgenda_2010.GenericObj_string

                        Dim settings As New JsonSerializerSettings()
                        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

                        Dim attivita = JsonConvert.DeserializeObject(Of AgronicaCoreModelsSTD.attivita.Attivita)(attivita_str, settings)

                        If IsNothing(attivita) Then
                            Throw New Exception
                        End If

                        sa_cod = attivita.centroAziendale.primaryKey.codice

                        lav_cod = attivita.job.primaryKey.codice

                        data = attivita.inizio

                        For Each r In attivita.risorse

                            If r.classType = AgronicaCoreModelsSTD.costanti.ClassType.DettaglioTrattamento Then
                                Dim ris = CType(r, AgronicaCoreModelsSTD.attivita.dettagli.DettaglioTrattamento)

                                pro_cod = ris.prodotto.codice
                                elem_cod = ris.prodotto.elemCod

                                If Not IsNothing(ris.MagazziniMovimentazioni) AndAlso ris.MagazziniMovimentazioni.Count = 1 Then
                                    fabbricato_cod = ris.MagazziniMovimentazioni(0).Magazzino.primaryKey.codice.ToString() + "|" + ris.MagazziniMovimentazioni(0).Magazzino.primaryKey.centroAziendalePK.codice.ToString() + "|" + ris.MagazziniMovimentazioni(0).Magazzino.primaryKey.centroAziendalePK.partitaIva.ToString()
                                End If

                            End If

                            If r.classType = AgronicaCoreModelsSTD.costanti.ClassType.DettaglioFertilizzazione Then
                                Dim ris = CType(r, AgronicaCoreModelsSTD.attivita.dettagli.DettaglioFertilizzazione)

                                pro_cod = ris.prodotto.codice
                                elem_cod = ris.prodotto.elemCod

                                If Not IsNothing(ris.MagazziniMovimentazioni) AndAlso ris.MagazziniMovimentazioni.Count = 1 Then
                                    fabbricato_cod = ris.MagazziniMovimentazioni(0).Magazzino.primaryKey.codice.ToString() + "|" + ris.MagazziniMovimentazioni(0).Magazzino.primaryKey.centroAziendalePK.codice.ToString() + "|" + ris.MagazziniMovimentazioni(0).Magazzino.primaryKey.centroAziendalePK.partitaIva.ToString()
                                End If

                            End If

                            If r.classType = AgronicaCoreModelsSTD.costanti.ClassType.DettaglioSemina Then
                                Dim ris = CType(r, AgronicaCoreModelsSTD.attivita.dettagli.DettaglioSemina)

                                mat_cod = ris.prodotto.codice
                                elem_cod = ris.prodotto.elemCod

                                If Not IsNothing(ris.MagazziniMovimentazioni) AndAlso ris.MagazziniMovimentazioni.Count = 1 Then
                                    fabbricato_cod = ris.MagazziniMovimentazioni(0).Magazzino.primaryKey.codice.ToString() + "|" + ris.MagazziniMovimentazioni(0).Magazzino.primaryKey.centroAziendalePK.codice.ToString() + "|" + ris.MagazziniMovimentazioni(0).Magazzino.primaryKey.centroAziendalePK.partitaIva.ToString()
                                End If
                            End If

                        Next

                        TargetRedirect &= "?piva=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, Server)
                        TargetRedirect &= "&sa_cod=" & Stringa_Codifica(sa_cod, AgroKey_EncoderDecoder, Server)
                        TargetRedirect &= "&pro_cod=" & Stringa_Codifica(pro_cod, AgroKey_EncoderDecoder, Server)
                        TargetRedirect &= "&elem_cod=" & Stringa_Codifica(elem_cod, AgroKey_EncoderDecoder, Server)
                        TargetRedirect &= "&lav_cod=" & Stringa_Codifica(lav_cod, AgroKey_EncoderDecoder, Server)
                        TargetRedirect &= "&data=" & Stringa_Codifica(data, AgroKey_EncoderDecoder, Server)
                        TargetRedirect &= "&visualizzazione_mode=" & Stringa_Codifica(1, AgroKey_EncoderDecoder, Server)
                        TargetRedirect &= "&tab_richiesto=" & Stringa_Codifica(1, AgroKey_EncoderDecoder, Server)
                        TargetRedirect &= "&fabbricato_cod=" & Stringa_Codifica(fabbricato_cod, AgroKey_EncoderDecoder, Server)
                        TargetRedirect &= "&mat_cod=" & Stringa_Codifica(mat_cod, AgroKey_EncoderDecoder, Server)

                        TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino

                    Catch ex As Exception
                        ' Se un'eccezione viene generata, allora non ho un oggetto Attivita in GenericObj_string, allora non faccio nulla
                    End Try

                Case enum_PagineAgenda_2010.Pagina_RilievoEffluenti

                    TargetRedirect = "./GestioneMagazzini/RilievoEffluenti.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Fabbricato = objParametriAgenda_2010.Fabbricato
                    objParametriAgenda.TornaASitoOrigine = True

                    Try
                        Dim attivita_str = objParametriAgenda_2010.GenericObj_string

                        Dim settings As New JsonSerializerSettings()
                        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

                        Dim attivita = JsonConvert.DeserializeObject(Of AgronicaCoreModelsSTD.attivita.Attivita)(attivita_str, settings)

                        If IsNothing(attivita) Then
                            Throw New Exception
                        End If

                        Dim piva As String = objParametriAgenda.Piva

                        Dim lav_cod As String = "0"

                        Dim data As String = CStr(attivita.inizio)

                        Dim ricetta_cod As Integer = 0

                        Dim piva_fabbricato As String = ""

                        Dim sa_cod_fabbricato As String = ""

                        Dim fabbricato_cod As String = "0"

                        Dim pua_cod As Integer = 0

                        Dim regolamento_cod_pua As Integer = 0

                        If Not IsNothing(attivita.job) Then
                            lav_cod = CStr(attivita.job.getCodice())
                        End If

                        If Not IsNothing(attivita.testataRicetta) Then
                            ricetta_cod = CInt(attivita.testataRicetta.Ricetta_Cod)

                            If Not IsNothing(attivita.testataRicetta.pua) Then

                                pua_cod = CInt(attivita.testataRicetta.pua.codice)

                                If Not IsNothing(attivita.testataRicetta.pua.disciplinare) Then

                                    If Not String.IsNullOrEmpty(attivita.testataRicetta.pua.disciplinare.codice) Then

                                        regolamento_cod_pua = CInt(attivita.testataRicetta.pua.disciplinare.codice)

                                    End If

                                    If regolamento_cod_pua = 0 AndAlso Not IsNothing(attivita.testataRicetta.pua.disciplinare.regolamentoConcimazione) AndAlso
                                        Not String.IsNullOrEmpty(attivita.testataRicetta.pua.disciplinare.regolamentoConcimazione.codice) Then

                                        regolamento_cod_pua = CInt(attivita.testataRicetta.pua.disciplinare.regolamentoConcimazione.codice)

                                    End If

                                End If
                            End If

                        End If

                        Dim dettaglioFertilizzazioneList As List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioFertilizzazione) = attivita.risorse.FindAll(Function(r) r.classType = ClassType.DettaglioFertilizzazione).ConvertAll(Function(obj1) CType(obj1, AgronicaCoreModelsSTD.attivita.dettagli.DettaglioFertilizzazione))

                        If Not IsNothing(dettaglioFertilizzazioneList) AndAlso dettaglioFertilizzazioneList.Count = 1 Then

                            Dim magazzinoMovimentazioni = dettaglioFertilizzazioneList(0).MagazziniMovimentazioni

                            If Not IsNothing(magazzinoMovimentazioni) AndAlso magazzinoMovimentazioni.Count = 1 Then
                                piva_fabbricato = magazzinoMovimentazioni(0).Magazzino.primaryKey.centroAziendalePK.partitaIva

                                sa_cod_fabbricato = CStr(magazzinoMovimentazioni(0).Magazzino.primaryKey.centroAziendalePK.codice)

                                fabbricato_cod = CStr(magazzinoMovimentazioni(0).Magazzino.primaryKey.codice)

                            End If

                        End If

                        TargetRedirect &= "?piva=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, Server)
                        TargetRedirect &= "&sa_cod=" & Stringa_Codifica(sa_cod_fabbricato, AgroKey_EncoderDecoder, Server)
                        TargetRedirect &= "&data=" & Stringa_Codifica(data, AgroKey_EncoderDecoder, Server)
                        TargetRedirect &= "&fabbricato_cod=" & Stringa_Codifica(fabbricato_cod, AgroKey_EncoderDecoder, Server)
                        TargetRedirect &= "&pua_cod=" & Stringa_Codifica(pua_cod, AgroKey_EncoderDecoder, Server)
                        TargetRedirect &= "&regolamento_cod=" & Stringa_Codifica(regolamento_cod_pua, AgroKey_EncoderDecoder, Server)
                        TargetRedirect &= objParametriAgenda.QueryStringFiltrino

                    Catch ex As Exception
                        ' Se un'eccezione viene generata, allora non ho un oggetto Attivita in GenericObj_string, allora non faccio nulla
                    End Try

                Case enum_PagineAgenda_2010.Pagina_GestioneContatti

                    'menu contatti
                    TargetRedirect = "./GestioneContatti/Contatti_Manager.aspx?piva=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder, Server)
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Liquidazione_soci


                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./GestioneContabilita/Liquidazione/Liquidazione.aspx" &
                                "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)


                Case enum_PagineAgenda_2010.Pagina_Menu_Lavorazioni


                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./GestioneLavorazioni/LavorazioneMenu.aspx" &
                                "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)

                Case enum_PagineAgenda_2010.Pagina_reportSostenibilita


                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                    TargetRedirect = "./GestioneDisciplinari/Verifica_Sostenibilita.aspx" &
                                        "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)

                    TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino

                    Try
                        Dim List_attivita_str = objParametriAgenda_2010.GenericObj_string

                        If List_attivita_str <> "" Then

                            Dim settings As New JsonSerializerSettings()
                            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

                            Dim List_attivita =
                                JsonConvert.DeserializeObject(Of List(Of AgronicaCoreModelsSTD.attivita.Attivita))(List_attivita_str, settings)

                            If Not IsNothing(List_attivita) AndAlso List_attivita.Count > 0 Then

                                'Dim codiciAttivita_x_CentriAziendali_List As List(Of AgronicaCoreModelsSTD.attivita.CodiciAttivita_x_CentriAziendali)
                                'estraiCodiciAttivita_x_CentriAziendali(objParametriAgenda.QueryStringFiltrino, codiciAttivita_x_CentriAziendali_List)

                                ''Split multicentro
                                'Dim mapper As New AgronicaCoreMapper.AttivitaToAgenda
                                'mapper.GestisciSplitAttivita(List_attivita, codiciAttivita_x_CentriAziendali_List, objParametri_Server, objParametri_Utenti)

                                Dim attivita = List_attivita(0)

                                Dim map As New AgronicaCoreMapper.AttivitaToAgenda
                                Dim agenda = map.MappaAttivitaToAgenda(attivita,
                                                                       objParametri_Server,
                                                                       objParametri_Server,
                                                                       objParametri_Utenti,
                                                                       False)

                                Dim Elemento_Verifica_Disciplinare As New AgronicaCoreModello.Agenda.Util.Elemento_Verifica_Disciplinare(
                                    attivita.disciplinare.codice,
                                    attivita.disciplinare.disciplinarePubblicoPrivato,
                                    attivita.disciplinare.descrizione,
                                    agenda)

                                If List_attivita.Count > 1 Then
                                    Elemento_Verifica_Disciplinare.Lista_Attivita = List_attivita
                                    Elemento_Verifica_Disciplinare.TipoElementoDaVerificare = AgronicaCoreModello.Agenda.Util.Enum_TipoElementoDaVerificare.AnalizzaOperazioni_DaListaAttivita
                                End If

                                Session("Elemento_Verifica_Disciplinare") = Elemento_Verifica_Disciplinare
                            End If
                        End If

                    Catch ex As Exception
                        ' Se un'eccezione viene generata, allora non ho un oggetto Attivita in GenericObj_string, allora non faccio nulla
                    End Try

                Case enum_PagineAgenda_2010.Pagina_Verifica_Conformita


                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                    TargetRedirect = "./GestioneDisciplinari/Verifica_DisciplinareBS.aspx" &
                                        "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)

                    TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino

                    Try
                        Dim List_attivita_str = objParametriAgenda_2010.GenericObj_string

                        If List_attivita_str <> "" Then

                            Dim settings As New JsonSerializerSettings()
                            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

                            Dim List_attivita =
                                JsonConvert.DeserializeObject(Of List(Of AgronicaCoreModelsSTD.attivita.Attivita))(List_attivita_str, settings)

                            If Not IsNothing(List_attivita) AndAlso List_attivita.Count > 0 Then

                                Dim Elemento_Verifica_Disciplinare As New AgronicaCoreModello.Agenda.Util.Elemento_Verifica_Disciplinare("", objParametriAgenda)

                                Dim codiciAttivita_x_CentriAziendali_List As List(Of AgronicaCoreModelsSTD.attivita.CodiciAttivita_x_CentriAziendali)
                                estraiCodiciAttivita_x_CentriAziendali(objParametriAgenda.QueryStringFiltrino, codiciAttivita_x_CentriAziendali_List)

                                'Split multicentro
                                Dim mapper As New AgronicaCoreMapper.AttivitaToAgenda
                                mapper.GestisciSplitAttivita(List_attivita, codiciAttivita_x_CentriAziendali_List, objParametri_Server, objParametri_Utenti)

                                'Singola Operazione
                                If List_attivita.Count = 1 Then

                                    Dim attivita = List_attivita(0)

                                    Dim map As New AgronicaCoreMapper.AttivitaToAgenda

                                    Dim agenda = map.MappaAttivitaToAgenda(attivita,
                                                                           objParametri_Server,
                                                                           objParametri_Server,
                                                                           objParametri_Utenti, False)

                                    'Dim info As New AgronicaCoreMapper.Utility
                                    Dim infoOperazione = GetInfoOperazione(agenda.Lav_Cod, attivita.tipo)

                                    Dim Dpi_Cod As Integer = 0
                                    Dim Dpi_PubblicoPrivato As Integer = 0
                                    Dim Bio As Boolean = False

                                    'una volta creato l'oggetto agenda posso invocare il suo metodo che mi genera l'xml
                                    Dim strXML = extraiXMLAgenda(agenda, infoOperazione, Dpi_Cod, Dpi_PubblicoPrivato)

                                    Elemento_Verifica_Disciplinare = New AgronicaCoreModello.Agenda.Util.Elemento_Verifica_Disciplinare(strXML, objParametriAgenda)

                                    If Not IsNothing(attivita.disciplinare) Then
                                        Elemento_Verifica_Disciplinare.Disciplinare_Cod = attivita.disciplinare.codice
                                        Elemento_Verifica_Disciplinare.Disciplinare_PubblicoPrivato = attivita.disciplinare.disciplinarePubblicoPrivato
                                        Elemento_Verifica_Disciplinare.Disciplinare_Des = attivita.disciplinare.descrizione
                                    End If

                                    Elemento_Verifica_Disciplinare.ObjParametriAgenda.Id_Agenda = agenda.Id_Agenda
                                Else
                                    'Multi Operazione
                                    Elemento_Verifica_Disciplinare.TipoElementoDaVerificare = AgronicaCoreModello.Agenda.Util.Enum_TipoElementoDaVerificare.AnalizzaOperazioni_DaListaAttivita
                                    Elemento_Verifica_Disciplinare.Lista_Attivita = List_attivita
                                End If

                                Session("Elemento_Verifica_Disciplinare") = Elemento_Verifica_Disciplinare
                            End If
                        Else
                            Dim Elemento_Verifica_Disciplinare As New AgronicaCoreModello.Agenda.Util.Elemento_Verifica_Disciplinare()
                            Session("Elemento_Verifica_Disciplinare") = Elemento_Verifica_Disciplinare
                        End If


                    Catch ex As Exception
                        ' Se un'eccezione viene generata, allora non ho un oggetto Attivita in GenericObj_string, allora non faccio nulla
                    End Try
                Case enum_PagineAgenda_2010.Pagina_DSS_Irrigazione

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                    TargetRedirect = "./DataAnalisiBI/DSS_Irrigazione/DSS_Irrigazione.aspx" & objParametriAgenda.QueryStringFiltrino

                Case enum_PagineAgenda_2010.Pagina_Verifica_DoseConsigliataBS

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                    TargetRedirect = "./GestioneDisciplinari/Verifica_DoseConsigliataBS.aspx" &
                                        "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)

                    TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino

                    Try
                        Dim attivita_str = objParametriAgenda_2010.GenericObj_string

                        Dim settings As New JsonSerializerSettings()
                        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

                        Dim attivita = JsonConvert.DeserializeObject(Of AgronicaCoreModelsSTD.attivita.Attivita)(attivita_str, settings)

                        Dim map As New AgronicaCoreMapper.AttivitaToAgenda
                        Dim agenda = map.MappaAttivitaToAgenda(attivita,
                                                               objParametri_Server,
                                                               objParametri_Server,
                                                               objParametri_Utenti, False)

                        'Dim info As New AgronicaCoreMapper.Utility
                        Dim infoOperazione = GetInfoOperazione(agenda.Lav_Cod, attivita.tipo)

                        Dim Dpi_Cod As Integer = 0
                        Dim Dpi_PubblicoPrivato As Integer = 0
                        Dim Bio As Boolean = False

                        'una volta creato l'oggetto agenda posso invocare il suo metodo che mi genera l'xml
                        Dim strXML = extraiXMLAgenda(agenda, infoOperazione, Dpi_Cod, Dpi_PubblicoPrivato)

                        Dim Elemento_Verifica_Disciplinare As New AgronicaCoreModello.Agenda.Util.Elemento_Verifica_Disciplinare(strXML, objParametriAgenda)

                        Elemento_Verifica_Disciplinare.Disciplinare_Cod = attivita.disciplinare.codice
                        Elemento_Verifica_Disciplinare.Disciplinare_PubblicoPrivato = attivita.disciplinare.disciplinarePubblicoPrivato
                        Elemento_Verifica_Disciplinare.Disciplinare_Des = attivita.disciplinare.descrizione

                        Elemento_Verifica_Disciplinare.ObjParametriAgenda.Id_Agenda = agenda.Id_Agenda

                        Session("Elemento_Verifica_Disciplinare") = Elemento_Verifica_Disciplinare


                    Catch ex As Exception
                        ' Se un'eccezione viene generata, allora non ho un oggetto Attivita in GenericObj_string, allora non faccio nulla
                    End Try

                Case enum_PagineAgenda_2010.Pagina_Gestione_Costi


                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                    TargetRedirect = "./AnalisiCostiProduzione/GestioneCosti.aspx" '&
                    '     "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)

                    TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino

                Case enum_PagineAgenda_2010.Pagina_AttivitaXCentri_Aziendali

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./AnalisiCostiProduzione/Anagrafiche/AttivitaXCentri_Aziendali.aspx" &
                                "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)


                Case enum_PagineAgenda_2010.Pagina_MenuStampe
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./Stampe/MenuStampe.aspx"


                Case enum_PagineAgenda_2010.Menu_BS

                    If PermessoRedirectMenuAgendaNG(objParametri_Server, objParametri_Utenti) Then
                        TargetRedirect = RedirectMenuAgendaNG(objParametriAgenda_2010.Piva)
                        Exit Select
                    End If

                    TargetRedirect = "./Menu/MenuBS_Agenda_Nuovo.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Appezza = objParametriAgenda_2010.Appezza
                    objParametriAgenda.Id_Imp = objParametriAgenda_2010.Id_Reg
                    objParametriAgenda.Campo_Cod = objParametriAgenda_2010.Campo_Cod
                    objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
                    objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lavorazione
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
                    Dim impianti As New List(Of ParametriAgenda_Temp.Impianto)
                    If objParametriAgenda_2010.Appezza <> 0 AndAlso objParametriAgenda_2010.Id_Reg <> 0 Then
                        Dim Imp As New ParametriAgenda_Temp.Impianto
                        Imp.Piva = objParametriAgenda.Piva
                        Imp.Sa_Cod = objParametriAgenda.Sa_Cod
                        Imp.Appezza = objParametriAgenda_2010.Appezza
                        Imp.ID_Reg = objParametriAgenda_2010.Id_Reg
                        impianti.Add(Imp)
                    End If

                    If objParametriAgenda_2010.Id_Cod = 0 Then
                        objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    Else
                        objParametriAgenda.Veg_Cod = CStr(objParametriAgenda_2010.Veg_Cod) & "/" & CStr(objParametriAgenda_2010.Id_Cod)
                    End If

                    objParametriAgenda.Impianti = impianti

                    objParametriAgenda.PaginaSitoOrigine = objParametriAgenda_2010.PaginaProvenienza

                    If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_PianoConcimazione_2017 Then
                        Select Case objParametriAgenda_2010.Mode
                            Case "AggiungiAlPua"
                                TargetRedirect &= "?Mode=" & Stringa_Codifica(objParametriAgenda_2010.Mode, AgroKey_EncoderDecoder, Server)
                                TargetRedirect &= "&DefaultTab=1"
                            Case Else
                                TargetRedirect &= "?DefaultTab=7"
                                'TargetRedirect &= "?DefaultTab=8"
                        End Select
                        TargetRedirect &= "&r=" & Stringa_Codifica(objParametriAgenda_2010.Id_Agenda, AgroKey_EncoderDecoder, Server)
                        objParametriAgenda_2010.Id_Agenda = 0
                    End If

                    TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino

                Case enum_PagineAgenda_2010.Menu_BS_TrackMode

                    Dim lotToTrack As String = Request.QueryString("lot")
                    If Not String.IsNullOrEmpty(lotToTrack) Then
                        lotToTrack = "&lot=" & lotToTrack
                    End If

                    Dim alToTrack As String = Request.QueryString("al")
                    If Not String.IsNullOrEmpty(alToTrack) Then
                        alToTrack = "&al=" & alToTrack
                    End If

                    TargetRedirect = "./menu/menuBS.aspx?QS_PageMode=track" & alToTrack & lotToTrack & "&QS_isLan=false"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Tipo_Operazione = "-1"
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.AnalisiDatiCura
                    TargetRedirect = "./GestioneMagazzini/TabaccoStatistiche.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Tipo_Operazione = "-1"
                    objParametriAgenda.TornaASitoOrigine = True


                Case enum_PagineAgenda_2010.BI_SchedeRilievi
                    TargetRedirect = "./DataAnalisiBI/SchedaRilievi/SchedaRilievi.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Tipo_Operazione = "-1"
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Report_Percorsi
                    TargetRedirect = "./DataAnalisiBI/ReportPercorsi/ReportPercorsi.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.Tipo_Operazione = "-1"
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Manutenzione_Macchine
                    TargetRedirect = "./Operazioni/ManutenzioneMacchine.aspx"
                    objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
                    objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lavorazione
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_NonConformita_Lista
                    TargetRedirect = "./NonConformita/NC_Lista.aspx"
                    '  Grilli, 23/01/2017 16:47:07: DA SISTEMARE: se non ho la piva, per evitere di fagliela scegliere
                    '  Prendo quella superuser...e se l'utente non avesse il permesso di vedere quella piva???

                    objParametriAgenda_2010.Piva = IIf(objParametriAgenda_2010.Piva = "", objParametri_Server.PivaSuperUser, objParametriAgenda_2010.Piva)

                Case enum_PagineAgenda_2010.Pagina_Scadenzario_Lista
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./Scadenzario/Scad_Lista.aspx"
                    TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino

                Case enum_PagineAgenda_2010.Pagina_Scadenzario_CreaModifica
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./Scadenzario/Scad_CreaModificaItem.aspx"
                    TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino

                Case enum_PagineAgenda_2010.Pagina_Menu_Liquidazione_soci


                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./GestioneContabilita/Liquidazione/LiquidazioneMenu.aspx" &
                                "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)

                Case enum_PagineAgenda_2010.Pagina_Analisi_Progetti


                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./AnalisiCostiProduzione/AnalisiProgetti.aspx" &
                                "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)


                Case enum_PagineAgenda_2010.Pagina_Analisi_Meteo


                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                    Dim oPunto As xyz = JsonConvert.DeserializeObject(Of xyz)(objParametriAgenda_2010.Chiave)

                    Dim latlng As String
                    If Not oPunto Is Nothing Then
                        latlng = "lat=" & oPunto.Y.ToString.Replace(",", ".") & "&" &
                            "lng=" & oPunto.X.ToString.Replace(",", ".") & "&"
                    Else
                        latlng = "lat=0&lng=0&"
                    End If

                    TargetRedirect = "./DataAnalisiBI/Meteo/Meteo.aspx" &
                        objParametriAgenda_2010.QueryStringFiltrino & "&" &
                        latlng &
                        "p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder) &
                        "&veg_Cod=" & objParametriAgenda_2010.Veg_Cod

                Case enum_PagineAgenda_2010.Pagina_DSS_Difesa

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                    Dim qsFiltrino = objParametriAgenda_2010.QueryStringFiltrino.ToLowerInvariant
                    If Not String.IsNullOrEmpty(qsFiltrino) AndAlso (qsFiltrino.Contains("externalload") OrElse qsFiltrino.Contains("params")) Then
                        TargetRedirect = "./DataAnalisiBI/Meteo/DSS_Difesa.aspx" &
                           objParametriAgenda_2010.QueryStringFiltrino & "&" &
                           "p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)
                    Else
                        Dim oPunto As xyz = JsonConvert.DeserializeObject(Of xyz)(objParametriAgenda_2010.Chiave.Split("&")(0))

                        Dim latlng As String
                        If Not oPunto Is Nothing Then
                            latlng = "lat=" & oPunto.Y.ToString.Replace(",", ".") & "&" &
                                "lng=" & oPunto.X.ToString.Replace(",", ".") & "&"
                        Else
                            latlng = "lat=0&lng=0&"
                        End If

                        TargetRedirect = "./DataAnalisiBI/Meteo/DSS_Difesa.aspx" &
                            objParametriAgenda_2010.QueryStringFiltrino & "&" &
                            latlng &
                            "p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder) &
                            "&veg_Cod=" & objParametriAgenda_2010.Veg_Cod
                    End If

                Case enum_PagineAgenda_2010.Pagina_DSS_Irrigazione_App
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./DataAnalisiBI/DSS_Irrigazione_APP/DSS_Irrigazione_APP.aspx"
                Case enum_PagineAgenda_2010.Pagina_ConsiglioFertirriguo_App
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./DataAnalisiBI/DSS_Fertirrigazione/DSS_Fertirrigazione.aspx"

                Case enum_PagineAgenda_2010.Pagina_Analisi_Rilievi

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                    Dim oPunto As xyz = JsonConvert.DeserializeObject(Of xyz)(objParametriAgenda_2010.Chiave.Split("&")(0))

                    Dim latlng As String
                    If Not oPunto Is Nothing Then
                        latlng = "lat=" & oPunto.Y.ToString.Replace(",", ".") & "&" &
                            "lng=" & oPunto.X.ToString.Replace(",", ".") & "&"
                    Else
                        latlng = "lat=0&lng=0&"
                    End If

                    TargetRedirect = "./DataAnalisiBI/AnalisiRilievi/AnalisiRilievi.aspx" &
                        objParametriAgenda_2010.QueryStringFiltrino & "&" &
                        latlng &
                        "p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder) &
                        "&veg_Cod=" & objParametriAgenda_2010.Veg_Cod

                Case enum_PagineAgenda_2010.Pagina_DatiReteAcqua

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                    Dim oPunto As xyz = JsonConvert.DeserializeObject(Of xyz)(objParametriAgenda_2010.Chiave.Split("&")(0))

                    Dim latlng As String
                    If Not oPunto Is Nothing Then
                        latlng = "lat=" & oPunto.Y.ToString.Replace(",", ".") & "&" &
                            "lng=" & oPunto.X.ToString.Replace(",", ".") & "&"
                    Else
                        latlng = "lat=0&lng=0&"
                    End If

                    TargetRedirect = "./DataAnalisiBI/DatiReteAcqua/DatiReteAcqua.aspx" &
                        objParametriAgenda_2010.QueryStringFiltrino & "&" &
                        latlng &
                        "p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder) &
                        "&veg_Cod=" & objParametriAgenda_2010.Veg_Cod

                Case enum_PagineAgenda_2010.visite


                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    Dim latlng As String = ""
                    If objParametriAgenda_2010.Chiave <> "" Then
                        Dim oPunto As xyz = JsonConvert.DeserializeObject(Of xyz)(objParametriAgenda_2010.Chiave)


                        If Not oPunto Is Nothing Then
                            latlng = "lat=" & oPunto.Y.ToString.Replace(",", ".") & "&" &
                                "lng=" & oPunto.X.ToString.Replace(",", ".") & "&"
                        Else
                            latlng = "lat=0&lng=0&"
                        End If
                    End If

                    TargetRedirect = "./Visite/Visite_CreaModificaItem.aspx?" &
                            latlng &
                                "p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder) &
                                "&veg_Cod=" & objParametriAgenda_2010.Veg_Cod

                Case enum_PagineAgenda_2010.Pagina_index



                    'logout
                    Session.Abandon()
                    Session.Clear()

                    Response.Redirect("./index.aspx?pivasuperuser=" & objParametri_Server.PivaSuperUser)


                Case enum_PagineAgenda_2010.Pagina_Anagrafica_Appezzamento
                    TargetRedirect = "./Anagrafica/Appezzamento_Edit.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Campo_Cod = objParametriAgenda_2010.Campo_Cod
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.Appezza = objParametriAgenda_2010.Appezza
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Anagrafica_Campo
                    TargetRedirect = "./Anagrafica/Campo_Edit.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Campo_Cod = objParametriAgenda_2010.Campo_Cod
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Anagrafica_Impianto
                    TargetRedirect = "./Anagrafica/Impianto_Edit.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.Appezza = objParametriAgenda_2010.Appezza
                    'TODO: inserire parametro per l'impianto
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Anagrafica_Centro
                    TargetRedirect = "./Anagrafica/Centro_Edit.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Anagrafica_Fabbricato
                    TargetRedirect = "./Anagrafica/Fabbricato_Edit.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Fabbricato = objParametriAgenda_2010.Fabbricato
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Anagrafica_Impresa
                    TargetRedirect = "./Anagrafica/Impresa_Edit.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Anagrafica_Particella
                    TargetRedirect = "./Anagrafica/Impresa_Edit.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    'TODO: inserire il parametro della particella usata
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Anagrafica_Contatto
                    TargetRedirect = "./Anagrafica/Contatto_Edit.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    'TODO: inserire il parametro del contatto usato
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Anagrafica_Contatto_New
                    TargetRedirect = "./Anagrafica/New_Contatto_Edit.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    'TODO: inserire il parametro del contatto usato
                    objParametriAgenda.Cod_Contatto = objParametriAgenda_2010.Cod_Contatto
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.TornaASitoOrigine = True

                    'Aggiunto per passare i parametri in querystring
                    TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino

                Case enum_PagineAgenda_2010.Pagina_PlugIn_Riepilogo_Meteo
                    TargetRedirect = "./DataAnalisiBI/Meteo/Widget/RiepilogoMeteo.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    'TODO: inserire il parametro del contatto usato
                    objParametriAgenda.Cod_Contatto = objParametriAgenda_2010.Cod_Contatto
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.AppWeatherSummary
                    TargetRedirect = "./DataAnalisiBI/Meteo/AppWidget/WeatherSummary/WeatherSummary.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    'TODO: inserire il parametro del contatto usato
                    objParametriAgenda.Cod_Contatto = objParametriAgenda_2010.Cod_Contatto
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_PlugIn_Monitoraggio
                    TargetRedirect = "./DataAnalisiBI/Meteo/Widget/MonitorSuolo.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    'TODO: inserire il parametro del contatto usato
                    objParametriAgenda.Cod_Contatto = objParametriAgenda_2010.Cod_Contatto
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_PlugIn_IndicatoriDSS
                    TargetRedirect = "./DataAnalisiBI/Meteo/Widget/DSS_Gauges.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    'TODO: inserire il parametro del contatto usato
                    objParametriAgenda.Cod_Contatto = objParametriAgenda_2010.Cod_Contatto
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Anagrafica_Macchina
                    TargetRedirect = "./Anagrafica/Macchina_Edit.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    'TODO: inserire il parametro della macchina usata
                    objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Operazione
                    objParametriAgenda.TornaASitoOrigine = True


                Case enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
                    If PermessoRedirectAnagrafiche(objParametri_Server, objParametri_Utenti) Then
                        MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                      Enum_SiteRedirector.GiasNG,
                                                                      enum_PagineGiasNG.Pagina_Menu_Anagrafica_Imprese,
                                                                      TargetRedirect,
                                                                      objParametri_Server)
                        Exit Select
                    End If
                    TargetRedirect = "./MenuAnagrafica/Menubs_anagrafica.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Fabbricato = objParametriAgenda_2010.Fabbricato
                    objParametriAgenda.Campo_Cod = objParametriAgenda_2010.Campo_Cod
                    objParametriAgenda.Tipo_Operazione = Nothing
                    objParametriAgenda.Appezza = objParametriAgenda_2010.Appezza
                    objParametriAgenda.TornaASitoOrigine = True

                Case enum_PagineAgenda_2010.Pagina_Anagrafica_Menu_Zoo
                    TargetRedirect = "./MenuAnagrafica/Menubs_anagrafica.aspx?visibilita=2"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                Case enum_PagineAgenda_2010.Pagina_FatturaElettronica

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./GestioneContabilita/Fatturazione/FattElettronica.aspx" &
                                     "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)

                Case enum_PagineAgenda_2010.Pagina_ReportVendite

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                    Dim bReportAcquistiLan As Boolean = False

                    'Marco Controllo Lan Report Acquisti
                    If Not IsNothing(objParametriAgenda_2010.Chiave) Then

                        If objParametriAgenda_2010.Chiave = "ALan" Then
                            bReportAcquistiLan = True
                        End If
                    End If

                    Select Case bReportAcquistiLan

                        Case False

                            TargetRedirect = "./Statistiche/Vendite/ReportVendite.aspx" &
                                     "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)


                        Case True

                            TargetRedirect = "./Statistiche/Vendite/ReportVendite.aspx" &
                                     "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder) &
                                     "&type=" & objParametriAgenda_2010.Chiave


                    End Select



                Case enum_PagineAgenda_2010.Pagina_KendoEditor

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./Editor/Editor.aspx" &
                                "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder) &
                                "&Cod_RisUm=" & Stringa_Codifica(objParametriAgenda_2010.Cod_risum, AgroKey_EncoderDecoder)


                Case enum_PagineAgenda_2010.Pagina_SementiInterferenzeSmall
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./GST/GST_Menu/GST_Menu.aspx?sview=1"


                Case enum_PagineAgenda_2010.DSSWidget

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./DataAnalisiBI/Meteo/DSSWidget.aspx"

                Case enum_PagineAgenda_2010.DSSWidget_ConRdir

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./DataAnalisiBI/Meteo/DSSWidget.aspx?enablerdir=1"

                Case enum_PagineAgenda_2010.Pagina_MVVElettronico

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./GestioneContabilita/MVV/MVVElettronico.aspx" &
                                     "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)

                Case enum_PagineAgenda_2010.Pagina_DAA_Elettronico

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./GestioneContabilita/DAA/DAAElettronico.aspx" &
                                     "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)

                    '03/12/21: ANNA: Modifica Multipla Piano Culturale
                Case enum_PagineAgenda_2010.Pagina_Modifica_Multipla_PianoColturale

                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    TargetRedirect = "./Anagrafica/Modifica_Multipla_PianoColturale.aspx" &
                                     "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)

                    If objParametriAgenda.QueryStringFiltrino <> "" Then
                        TargetRedirect &= "&" & objParametriAgenda.QueryStringFiltrino.Replace("?", "")
                    End If


                Case enum_PagineAgenda_2010.Pagina_Gestione_Esercizi
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                    TargetRedirect = "./Anagrafica/GestioneEsercizi.aspx" &
                                     "?p=" & Stringa_Codifica(objParametriAgenda_2010.Piva, AgroKey_EncoderDecoder)

                Case enum_PagineAgenda_2010.Pagina_DocContabile
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lav_Cod

                    Dim qsAggiuntiva As String = objParametriAgenda_2010.QueryStringFiltrino

                    TargetRedirect = "./GestioneContabilita/DocContabile.aspx"

                    If Not ChiamatoDaAngularMenuAgendaQdC(objParametriAgenda) Then

                        Dim qs As String = ""

                        'Per ora gestito solamente Carico di magazzino e Nuovo DDT Ricevuto
                        Select Case objParametriAgenda.Lav_Cod
                            Case LAVCOD_BOLLA_RICEVUTA
                                'Documenti contabili: Bolla ricevuta
                                qs &= "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                                                    "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder) &
                                                    "&s=" & Stringa_Codifica(objParametriAgenda.Sa_Cod, AgroKey_EncoderDecoder) &
                                                    "&i=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                                                    "&l=" & Stringa_Codifica(LAVCOD_BOLLA_RICEVUTA, AgroKey_EncoderDecoder) &
                                                    "&md=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                                                    "&ricercatype=" & Stringa_Codifica("A", AgroKey_EncoderDecoder) &
                                                    "&ricercadoc=" & Stringa_Codifica("C", AgroKey_EncoderDecoder)
                            ' "&orig=" & Stringa_Codifica("", AgroKey_EncoderDecoder) &
                            Case LAVCOD_CARICO
                                'Documenti contabili: Carico magazzino
                                qs &= "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                                                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder) &
                                                "&s=" & Stringa_Codifica(objParametriAgenda.Sa_Cod, AgroKey_EncoderDecoder) &
                                                "&i=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                                                "&l=" & Stringa_Codifica(LAVCOD_CARICO, AgroKey_EncoderDecoder) &
                                                "&md=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
                                                "&ricercatype=" & Stringa_Codifica("", AgroKey_EncoderDecoder) &
                                                "&ricercadoc=" & Stringa_Codifica("", AgroKey_EncoderDecoder)
                                ' "&orig=" & Stringa_Codifica("", AgroKey_EncoderDecoder) &
                        End Select

                        TargetRedirect &= qs

                        If Not String.IsNullOrEmpty(qsAggiuntiva) Then


                            If qs.StartsWith("?") Then
                                'Se ci sono già altri parametri in query string
                                If qsAggiuntiva.StartsWith("?") Then
                                    qsAggiuntiva = "&" & qsAggiuntiva.Remove(0, 1)
                                ElseIf Not qsAggiuntiva.StartsWith("&") Then
                                    qsAggiuntiva = "&" & qsAggiuntiva
                                End If
                            Else
                                'Se non ci sono già altri parametri in query string
                                If qsAggiuntiva.StartsWith("&") Then
                                    qsAggiuntiva = "?" & qsAggiuntiva.Remove(0, 1)
                                ElseIf Not qsAggiuntiva.StartsWith("?") Then
                                    qsAggiuntiva = "?" & qsAggiuntiva
                                End If

                            End If

                        End If

                    End If


                    TargetRedirect &= qsAggiuntiva

                Case enum_PagineAgenda_2010.Pagina_DuplicaOperazione
                    TargetRedirect = "./Operazioni/DuplicaOperazione.aspx"
                    objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
                    objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
                    objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lav_Cod
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    ' objParametriAgenda.RagSoc = objParametriAgenda_2010.Sa_Cod
                    ' objParametriAgenda.SaNome = objParametriAgenda_2010.Sa_Cod

                    TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino

                Case enum_PagineAgenda_2010.Pagina_Filtrone
                    TargetRedirect = "./Filtrone/Filtrone_nuovo.aspx"
                    TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino

                Case enum_PagineAgenda_2010.Pagina_Analisi_Correzione_Testate
                    TargetRedirect = "./Zoo/Analisi_Correzione_Testate.aspx"
                    TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino

                Case enum_PagineAgenda_2010.Pagina_Passaggio_Stato
                    TargetRedirect = "./Servizi/PassaggioDiStato.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                    TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino

                Case enum_PagineAgenda_2010.Pagina_Gestione_Servizi
                    TargetRedirect = "./Servizi/Servizi_Lista_BS.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                Case enum_PagineAgenda_2010.Pagina_Zoo_Trattamenti_Capo
                    TargetRedirect = "./Zoo/Zoo_Trattamenti_Capo.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                    TargetRedirect &= objParametriAgenda_2010.QueryStringFiltrino

                Case enum_PagineAgenda_2010.Pagina_BilancioDiMassa
                    TargetRedirect = "./GestioneMagazzini/BilancioDiMassa.aspx"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                Case enum_PagineAgenda_2010.Pagina_Operazioni_Zootecniche
                    TargetRedirect = "./Menu/MenuBS_Agenda_Nuovo.aspx?DefaultTab=5"
                    objParametriAgenda.Piva = objParametriAgenda_2010.Piva

                Case Else

                    Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim DTConfigSiti As DataTable
                    Dim i As Integer

                    DTConfigSiti = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)

                    If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
                        TargetRedirect = "./menu/menubs_agenda_nuovo.aspx"
                    Else
                        TargetRedirect = "./menu/menu.aspx"
                    End If

                    'If Not IsNothing(ConfigurationSettings.AppSettings("MenuAgendaBS")) AndAlso ConfigurationSettings.AppSettings("MenuAgendaBS") = True Then
                    '    TargetRedirect = "./menu/menubs_agenda_nuovo.aspx"
                    'Else
                    '    TargetRedirect = "./Menu/Menu.aspx"
                    'End If

                    'TargetRedirect = "./menu/menu.aspx"
                    objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
                    objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
                    objParametriAgenda.TornaASitoOrigine = True

            End Select

            'Dim objAgroWebConfig As New AgroWebConfig


            'objParametriAgenda.Piva = objParametriAgenda_2010.Piva

            'objParametriAgenda.WS_Disciplinari_AgroWS_Disciplinari = objAgroWebConfig.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
            'objParametriAgenda.WS_Fitofarmaci_AgroWS_Fitofarmaci = objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
            ''objParametriAgenda.Salva()

            'Session("permessoDPIPrivati") = objAgroWebConfig.Flag_DisciplinarePrivato

            ' se attiva la nuova dashboard salta direttamente alla pagina senza passare dal vecchio menu
            If objParametriAgenda_2010.PaginaRichiesta = enum_PagineAgenda_2010.Menu Then
                If apiController.CanUseAPI AndAlso apiController.VersioneHeader = "2022" Then
                    GestioneRedirectPagina(idSezione, TargetRedirect, objParametriAgenda, objParametri_Server)
                End If
            End If

            If idSezione <> 0 Then
                If apiController.CanUseAPI AndAlso apiController.VersioneHeader = "2022" Then
                    TargetRedirect = AgronicaCoreUtility.Varie.aggiungiAQueryString(TargetRedirect, "idBC", Stringa_Codifica(idSezione, AgroKey_EncoderDecoder))
                End If
                If Not sideBar Then
                    TargetRedirect = AgronicaCoreUtility.Varie.aggiungiAQueryString(TargetRedirect, "sidebar", "off")
                End If
            End If

            Response.Redirect(TargetRedirect)

            Exit Sub

        End If

        '--------------------------------------------------------------
        '---------------FINE OPERAZIONE AGENDA 2010 -------------------
        '--------------------------------------------------------------


        '--------------------------------------------------------------
        '---------------ParametriAnalisiCosti_2010 --------------------
        '--------------------------------------------------------------

        'se non è nothing allora è stato passato l'oggetto ParametriRicette_2010
        'quindi si vuole aprire la pagina Ricette_Edit
        If Not IsNothing(Session("ParametriAnalisiCosti_2010")) Then

            Dim ParametriAnalisi_2010 As New ParametriAnalisiCosti_2010
            ParametriAnalisi_2010.Leggi()
            Session("ParametriAnalisiCosti_2010") = Nothing

            Dim objAgroWebConfig As New AgroWebConfig
            Dim objParametriAgenda As New ParametriAgenda

            '  Session("permessoDPI") = objAgroWebConfig.Flag_DisciplinareAttivo
            ' Session("permessoDPIPrivati") = objAgroWebConfig.Flag_DisciplinarePrivato

            'Dim Piva As String = ParametriAnalisi_2010.
            'Dim Sa_Cod As Integer = ParametriAnalisi_2010.sa_cod
            'Dim Tipo_Operazione As Integer = ParametriRicette_2010.Tipo_Operazione

            Dim impianti As New List(Of ParametriAgenda_Temp.Impianto)
            ' If objParametriAgenda_2010.Appezza <> 0 AndAlso objParametriAgenda_2010.Id_Reg <> 0 Then

            Dim Imp As ParametriAgenda_Temp.Impianto
            For i = 0 To ParametriAnalisi_2010.ListaImpianti.Length - 1

                Imp = New ParametriAgenda_Temp.Impianto

                Imp.Piva = ParametriAnalisi_2010.ListaImpianti(i).Piva
                Imp.Sa_Cod = ParametriAnalisi_2010.ListaImpianti(i).Sa_Cod
                Imp.App_Nome = ParametriAnalisi_2010.ListaImpianti(i).App_Nome
                Imp.Appezza = ParametriAnalisi_2010.ListaImpianti(i).Appezza
                Imp.ID_Reg = ParametriAnalisi_2010.ListaImpianti(i).Id_Reg
                Imp.Sup_Imp = ParametriAnalisi_2010.ListaImpianti(i).Sup_Imp
                Imp.Veg_Cod = ParametriAnalisi_2010.ListaImpianti(i).Veg_Cod
                Imp.Cul_Cod = ParametriAnalisi_2010.ListaImpianti(i).Cul_Cod
                Imp.Rag_Soc = ParametriAnalisi_2010.ListaImpianti(i).Rag_Soc
                Imp.Validita_Inizio = ParametriAnalisi_2010.ListaImpianti(i).Validita_Inizio
                Imp.Validita_Fine = ParametriAnalisi_2010.ListaImpianti(i).Validita_Fine
                impianti.Add(Imp)
                Imp = Nothing
            Next

            ' End If

            objParametriAgenda.Impianti = impianti


            ' objParametriAgenda.WS_Disciplinari_AgroWS_Disciplinari = objAgroWebConfig.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
            ' objParametriAgenda.WS_Fitofarmaci_AgroWS_Fitofarmaci = objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci

            '  Session("VariabiliFiltro") = ParametriRicette_2010.StrVariabiliAgenda

            Dim TargetRedirect As String

            TargetRedirect = "./AnalisiCostiProduzione/AnalisiCostiProduzione.aspx"

            Response.Redirect(TargetRedirect)

            Exit Sub

        End If

        '--------------------------------------------------------------
        '---------------FINE ParametriAnalisiCosti_2010 ---------------
        '--------------------------------------------------------------

        If Not IsNothing(Session("ParametriScadenziario")) Then

            Dim TargetRedirect As String
            Dim ParametriScadenziario As New ParametriScadenziario

            ParametriScadenziario.Leggi()
            System.Web.HttpContext.Current.Session("ParametriScadenziario") = Nothing

            Dim objParametriAgenda As New ParametriAgenda
            objParametriAgenda.Piva = ParametriScadenziario.Piva

            Select Case ParametriScadenziario.Pagina_Richiesta

                Case enum_PagineAgenda_2010.Pagina_GestioneAllegati

                    Dim Piva As String = ParametriScadenziario.Piva
                    Dim Cod_Contatto As String = ParametriScadenziario.Cod_Contatto
                    Dim Area As Integer = ParametriScadenziario.Id_Area
                    Dim Tipologia As Integer = ParametriScadenziario.Id_Tipologia
                    Dim AnalisiTestataCod As Integer = ParametriScadenziario.Analisi_Testata_Cod
                    Dim DataScadenza As Date = ParametriScadenziario.Data_Scadenza
                    'Dim PCTestataCod As Integer = ParametriScadenziario.PC_Testata_Cod
                    'Dim PathFile As String = ParametriScadenziario.PathFile
                    'Dim PUA_Cod As Integer = ParametriScadenziario.PUA_Cod

                    TargetRedirect = RedirectGestione.GetLinkGestioneAllegati(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, Piva, Cod_Contatto, Tipologia, AnalisiTestataCod, DataScadenza)

                    Response.Redirect(Replace(TargetRedirect, "..", "."))

                Case enum_PagineAgenda_2010.Pagina_Documentale_Lista, enum_PagineAgenda_2010.Pagina_Scadenzario_Lista

                    Dim Piva As String = ParametriScadenziario.Piva
                    Dim Area As Integer = ParametriScadenziario.Id_Area
                    Dim Tipologia As Integer = ParametriScadenziario.Id_Tipologia
                    Dim AnalisiTestataCod As Integer = ParametriScadenziario.Analisi_Testata_Cod
                    Dim QueryStringFiltrino As String = ParametriScadenziario.QueryStringFiltrino
                    Dim Modalita As String = If(ParametriScadenziario.Pagina_Richiesta = enum_PagineAgenda_2010.Pagina_Documentale_Lista, "doc", "")

                    TargetRedirect = "../Scadenzario/Scad_lista.aspx?area_provenienza=" & Area & "&tipologia_provenienza=" & Tipologia & "&p=" & Piva & "&type=" & Modalita
                    If AnalisiTestataCod <> 0 Then
                        TargetRedirect &= "&analisi_testata_cod=" & AnalisiTestataCod & "&origine_nc=.../AgronicaAnalisi_2010/GestioneAnalisiTerreno/Analisi.aspx"
                    End If

                    If QueryStringFiltrino <> "" Then
                        TargetRedirect += QueryStringFiltrino
                    End If

                    Response.Redirect(Replace(TargetRedirect, "..", "."))

                Case enum_PagineAgenda_2010.Pagina_Documentale_CreaModifica, enum_PagineAgenda_2010.Pagina_Scadenzario_CreaModifica

                    Dim JObj = New JObject()
                    JObj("ID_Alert_Entita") = -1
                    JObj("ID_Elenco") = -1
                    JObj("Analisi_Testata_Cod") = ParametriScadenziario.Analisi_Testata_Cod
                    JObj("Modalita") = If(ParametriScadenziario.Pagina_Richiesta = enum_PagineAgenda_2010.Pagina_Documentale_CreaModifica, "doc", "")
                    JObj("Tipologia") = ParametriScadenziario.Id_Tipologia
                    JObj("Piva") = ParametriScadenziario.Piva
                    JObj("Data_Scadenza_Analisi") = ParametriScadenziario.Data_Scadenza.ToShortDateString
                    JObj("area_provenienza") = ParametriScadenziario.Id_Area

                    If Not String.IsNullOrEmpty(ParametriScadenziario.QueryStringFiltrino) Then
                        'rimuovo il primo &, che serve solo per il case Case enum_PagineAgenda_2010.Pagina_Documentale_Lista
                        Dim QueryStringFiltrino As String = ParametriScadenziario.QueryStringFiltrino.Substring(1)
                        Dim parametriFiltrino() As String = QueryStringFiltrino.Split("&")

                        For Each parametro In parametriFiltrino
                            Dim nome = parametro.Split("=")(0)
                            Dim value = parametro.Split("=")(1)

                            JObj(nome) = value
                        Next
                    End If

                    Dim serializerSettings As New JsonSerializerSettings()
                    serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

                    Dim param As String = JsonConvert.SerializeObject(JObj, Formatting.None, serializerSettings)

                    Dim Area As Integer = ParametriScadenziario.Id_Area

                    TargetRedirect = "../Scadenzario/Scad_CreaModificaItem.aspx?scadstr=" & param & "&type=" & JObj("Modalita").ToString() & "&origine_nc=.../AgronicaAnalisi_2010/GestioneAnalisiTerreno/Analisi.aspx" & "&ac=" & JObj("Analisi_Testata_Cod").ToString()



                    Response.Redirect(Replace(TargetRedirect, "..", "."))
            End Select

        End If

        '--------------------------------------------------------------
        '--------------- ParametriLiquidazione ------------------------
        '--------------------------------------------------------------


        '--------------------------------------------------------------
        '--------------- FINE ParametriLiquidazione ------------------------
        '--------------------------------------------------------------

    End Sub

    ''' <summary>
    ''' Esegue un check quando si cerca di effettuare il redirect verso il vecchio menù agenda.
    ''' Se sussistono le condizioni, modifica l'indirizzo di redirect per puntare alle nuove pagine in Angular.
    ''' </summary>
    Private Sub ChangeRedirectForMenuAgenda(
        ByRef Response As HttpResponse,
        ByVal idSezione As Integer,
        ByRef targetRedirect As String,
        ByVal objParametriAgenda As ParametriAgenda,
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri
    )
        Dim redirectUrl = objParametriAgenda.RedirectUrl.ToLowerInvariant()
        Dim isRedirectingToMenuAgenda = redirectUrl.Contains("/menu/menubs_agenda_nuovo.aspx")
        Dim isZooTab = redirectUrl.Contains("defaulttab=5")
        Dim shouldRedirectToNewZoo = idSezione = 73 AndAlso PermessoRedirectMenuZooNG(objParametri_Server, objParametri_Utenti)
        Dim shouldRedirectToNewAgenda = PermessoRedirectMenuAgendaNG(objParametri_Server, objParametri_Utenti)

        If isRedirectingToMenuAgenda Then

            If isZooTab AndAlso shouldRedirectToNewZoo Then
                MenuBS_2017_RedirectGestione.RedirectGenerico(
                    objParametriAgenda.Piva,
                    Enum_SiteRedirector.GiasNG, enum_PagineGiasNG.Pagina_Menu_Zoo,
                    targetRedirect, objParametri_Server
                )
                Response.Redirect(targetRedirect)
            ElseIf Not isZooTab AndAlso shouldRedirectToNewAgenda Then
                MenuBS_2017_RedirectGestione.RedirectGenerico(
                    objParametriAgenda.Piva,
                    Enum_SiteRedirector.GiasNG, enum_PagineGiasNG.Pagina_Menu_Agenda,
                    targetRedirect, objParametri_Server
                )
                Response.Redirect(targetRedirect)
            End If

        End If
    End Sub

    Private Function PermessoRedirectMenuAgendaNG(
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri
    ) As Boolean
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim hasReadingPermission = ObjUtenti.Controlla_Permessi_Utente(
            HttpContext.Current.Session("ASG_Utente_Username"),
            HttpContext.Current.Session("ASG_IdServizio"),
            enum_Security_Attivita.Agenda_AccessoMenu_NG,
            enum_Security_Operazione.Lettura,
            Date.Now, "", objParametri_Utenti
        )
        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim shouldRedirectToNew = objConfSiti.Leggi_Valore(0, "MenuAgendaNG", "", "", objParametri_Server) = "true"

        Return shouldRedirectToNew AndAlso hasReadingPermission
    End Function

    Private Function PermessoRedirectMenuZooNG(
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri
    ) As Boolean
        '' Uso lo stesso permesso del menù zoo vecchio
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim hasReadingPermission = ObjUtenti.Controlla_Permessi_Utente(
            HttpContext.Current.Session("ASG_Utente_Username"),
            HttpContext.Current.Session("ASG_IdServizio"),
            enum_Security_Attivita.Gest_Stalle,
            enum_Security_Operazione.Lettura,
            Date.Now, "", objParametri_Utenti
        )

        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim isNewMenuActive = objConfSiti.Leggi_Valore(0, "useNewZooMenu", "", "", objParametri_Server) = "true"

        Return hasReadingPermission AndAlso isNewMenuActive
    End Function

    Private Function PermessoRedirectAnagrafiche(ByRef objParametri_Server As AgronicaCoreParametri,
                                                  ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        'Dim Permesso_Lettura = ObjUtenti.Controlla_Permessi_Utente(
        '        HttpContext.Current.Session("ASG_Utente_Username"),
        '        HttpContext.Current.Session("ASG_IdServizio"),
        '        enum_Security_Attivita.Gest_AnagraficaAzienda_NG,
        '        enum_Security_Operazione.Lettura,
        '        Date.Now, "", objParametri_Utenti)

        Dim Permesso_Lettura = True

        If objConfSiti.Leggi_Valore(0, "MenuAnagrafeNG", "", "", objParametri_Server) = "true" AndAlso Permesso_Lettura Then
            Return True
        End If
        Return False

    End Function

    Private Function RedirectMenuAgendaNG(Piva As String) As String
        Dim objAgendaNG As New Parametri_ObjParametriAgenda_NG
        objAgendaNG.Piva = Piva
        objAgendaNG.Pagina_Richiesta = enum_PagineGiasNG.Pagina_Menu_Agenda
        objAgendaNG.Salva()
        Return RedirectGestione.IndirizzoCompleto_SitoGiasNG_PassandoDirettamente_ParametriAgenda_NG(Enum_SiteRedirector.Sito_GiasOnline_2010, objAgendaNG)
    End Function


    Private Function ChiamatoDaAngularMenuAgendaQdC(objParametriAgenda As ParametriAgenda) As Boolean
        Return objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG AndAlso
            (objParametriAgenda.PaginaSitoOrigine = enum_PagineGiasNG.Pagina_Menu_Agenda OrElse objParametriAgenda.PaginaSitoOrigine = enum_PagineGiasNG.Pagina_Edit_Attivita)
    End Function

    Private Sub SetObjParametriAgenda(ByRef objParametriAgenda As ParametriAgenda, ByRef objParametriAgenda_2010 As ParametriAgenda_2010)
        objParametriAgenda.Piva = objParametriAgenda_2010.Piva
        objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
        objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
        objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
        objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
        objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lav_Cod
        objParametriAgenda.TipoOperazioneAgenda = objParametriAgenda_2010.TipoOperazioneAgenda
        objParametriAgenda.TargetOperazione = objParametriAgenda_2010.TargetOperazione
        objParametriAgenda.Programmazione_Cod = objParametriAgenda_2010.Programmazione_Cod
        objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Tipo_Operazione
        objParametriAgenda.TipoRicetta = objParametriAgenda_2010.TipoRicetta
        objParametriAgenda.RedirectUrl = objParametriAgenda_2010.RedirectUrl

        objParametriAgenda.QueryStringFiltrino = objParametriAgenda_2010.QueryStringFiltrino

        If objParametriAgenda_2010.Impianti IsNot Nothing AndAlso objParametriAgenda_2010.Impianti.Count > 0 Then
            objParametriAgenda.Impianti = New List(Of ParametriAgenda_Temp.Impianto)
            For Each imp In objParametriAgenda_2010.Impianti
                Dim impianto_temp As New ParametriAgenda_Temp.Impianto

                impianto_temp.Piva = imp.Piva
                impianto_temp.Sa_Cod = imp.Sa_Cod
                impianto_temp.Appezza = imp.Appezza
                impianto_temp.ID_Reg = imp.Id_Reg
                impianto_temp.Progetto_Cod = imp.Progetto_Cod
                impianto_temp.Veg_Cod = imp.veg_cod

                objParametriAgenda.Impianti.Add(impianto_temp)
            Next
        End If

    End Sub

    Friend Sub ImpostaCultura(ByRef lingua As Lingua)
        If Not lingua Is Nothing Then
            System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(lingua.CodiceISO)
            '' questa istruzione da errore
            'System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("en")
            '_LinguaCorrente = lingua
            Session("LinguaCorrente") = lingua
        End If
    End Sub

    Private Function getImpianti_ParametriAgendaTemp(ByVal impianti As List(Of Impianti_2010)
        ) As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

        Dim objImpianti As New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

        For Each i In impianti
            Dim imp = New AgronicaCoreModello.ParametriAgenda_Temp.Impianto()

            imp.Appezza = i.Appezza
            imp.Piva = i.Piva
            imp.Sa_Cod = i.Sa_Cod
            imp.ID_Reg = i.Id_Reg

            objImpianti.Add(imp)
        Next

        Return objImpianti
    End Function

    Private Sub GestioneRedirectPagina(ByVal idSezione As Integer, ByRef TargetRedirect As String, ByRef objParametriAgenda As ParametriAgenda, ByRef objParametri_Server As AgronicaCoreParametri)

        Dim RedirectURL As String = ""

        If idSezione <> 0 Then

            If Not IsNothing(Request.QueryString("FiltroAziende")) Then
                RedirectURL = MenuBS_2017.GetFiltroAziende(idSezione)
            Else
                Dim risposta
                If idSezione = enum_PagineGiasNG.Pagina_Dashboard OrElse idSezione = enum_PagineGiasNG.Pagina_Gestione_Preferiti Then
                    risposta = MenuBS_2017.redirectGiasNG(idSezione)
                Else
                    risposta = MenuBS_2017.salvaTitoloSezioneConGestioneRedirect(idSezione)
                End If
                If risposta.RispostaOK Then
                    RedirectURL = risposta.RispostaStringa
                End If
            End If

            If Not String.IsNullOrEmpty(RedirectURL) AndAlso Not RedirectURL.Contains("<script") Then
                TargetRedirect = If(RedirectURL.StartsWith(".."), RedirectURL.Replace("..", "."), RedirectURL)
            End If

        Else

            Dim Parametri_Aggiuntivi As New JObject
            Parametri_Aggiuntivi.Item("Pagina_Provenienza") = enum_PagineGiasNG.Pagina_Dashboard

            MenuBS_2017_RedirectGestione.RedirectGenerico(
                            objParametriAgenda.Piva, Enum_SiteRedirector.GiasNG, enum_PagineGiasNG.Pagina_Dashboard, RedirectURL, objParametri_Server,
                            Parametri_Aggiuntivi:=Parametri_Aggiuntivi, SitoOrigine:=Enum_SiteRedirector.GiasNG)

            TargetRedirect = RedirectURL

        End If

    End Sub

End Class
