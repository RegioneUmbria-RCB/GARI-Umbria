Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider
Imports AgronicaCoreGestioneRichieste
Imports AgronicaControlli_2010
Imports Agronica.Helpers.GiasBase

''MIO DIO IL VB, NO, NON L'AVEVO CONSIDERATO...
'Imports AgronicaCoreDataProvider

'Imports AgronicaCoreDataProvider.CostantiPersonalizzate
'Imports AgronicaCoreDataProvider.Sicurezza
''Imports AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti
'Imports AgronicaCoreUtentiDAL
'Imports AgronicaCoreGestioneRichieste
'Imports AgronicaCoreDataProvider.TipiEnumerativi

Partial Class GestioneRichieste
    Inherits System.Web.UI.Page

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region "Dichiarazione Variabili"

    Dim TargetURL As String
    Const PaginaLink_PianoConcimazione_Menu = "PianoConcimazione_MenuBS.aspx"
    'Const PaginaLink_Filtro_PAP_Vegetali = "Stampa_PAP_Vegetale/Filtro_SchedaPAPVegetaleBiologico.aspx"
    Dim Qs_PagSelezionata As String

#End Region


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Session.Clear()

        If (Not IsPostBack) Then
            'controllo la query string....
            If (Not IsNothing(Request.QueryString("unid")) And Not IsNothing(Request.QueryString("cn"))) Then

                ''Dim Unid As String = Stringa_Decodifica( _
                ''                        Request.QueryString("unid").ToString, _
                ''                        AgroKey_EncoderDecoder, _
                ''                        Server)

                ''Dim Cn_Server As String = Stringa_Decodifica( _
                ''                Request.QueryString("cn").ToString, _
                ''                AgroKey_EncoderDecoder, _
                ''                Server)

                ''Dim objGestioneRichieste As GestioneRichiesteClasse
                ''objGestioneRichieste = New GestioneRichiesteClasse(Unid,
                ''                                                    Cn_Server,
                ''                                                    "", _
                ''                                                    Server.MapPath("AB_Immagini/IconeVegetali").ToString)

                '25/02/2026 Casadei - Aggiunta pulizia dei report temporanei
                CrystalHelper.eliminaReportTemporanei()
                '--------------------------------------------------------------
                '---------------SUPERSERVER-----------------------------------------
                '--------------------------------------------------------------
                Dim objGestioneRichieste As GestioneRichiesteClasse

                Dim Unid As String = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(
                          Request.QueryString("unid").ToString,
                          AgroKey_EncoderDecoder,
                          Server)

                Dim Cn_Server As String = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(
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
                    StringaConnessioneSuperserver = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(
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

                Dim objParametri_Super_Server As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
                Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
                Dim objParametri_Utenti As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

                GiasBaseHelper.WarmUp_GestioneRichieste()

                '--------------------------------------------------------------
                '---------------SUPERSERVER END----------------------------------
                '--------------------------------------------------------------

                Dim ling As New Lingua
                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
                ling = objUtenti.Leggi_Lingua(CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri).UtenteUsername, "", "", CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri))
                CType(Session("ASG_objParametri_Server"), AgronicaCoreParametri).Lingua_Cod = ling.Lingua_cod
                CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri).Lingua_Cod = ling.Lingua_cod
                ImpostaCultura(ling)

                Session("IDSezione") = Nothing

                Dim objParametriConcimazione_2017 As New ParametriConcimazione_2017
                objParametriConcimazione_2017.Leggi()

                Dim modalitaMenuBsPcPua As enum_PUARegolamenti_Tipo = enum_PUARegolamenti_Tipo.PianoComcimazione

                Select Case objParametriConcimazione_2017.Pagina_Richiesta
                    Case enum_PaginePianoConcimazione_2017.MenuBS_PUA,
                         enum_PaginePianoConcimazione_2017.PUA_Dichiarazione_Effluenti
                        modalitaMenuBsPcPua = enum_PUARegolamenti_Tipo.PUA
                        Session("IDSezione") = 107

                        Dim objParametriPua As New ParametriPUA
                        objParametriPua.Piva = objParametriConcimazione_2017.Piva
                        objParametriPua.Salva()

                    Case enum_PaginePianoConcimazione_2017.MenuBs_Piano_Nutrizionale
                        modalitaMenuBsPcPua = enum_PUARegolamenti_Tipo.PianoNutrizionale
                        Session("IDSezione") = 244

                    Case Else
                        Session("IDSezione") = 20
                End Select

                Select Case objParametriConcimazione_2017.Pagina_Richiesta

                    Case enum_PaginePianoConcimazione_2017.MenuBS,
                         enum_PaginePianoConcimazione_2017.MenuBS_PUA,
                         enum_PaginePianoConcimazione_2017.MenuBs_Piano_Nutrizionale

                        'richiamo la pagina di gestione
                        TargetURL = "~/PianoConcimazione_MenuBS.aspx" &
                                    "?p=" & Stringa_Codifica(CStr(objParametriConcimazione_2017.Piva), AgroKey_EncoderDecoder, Server) &
                                    "&o=" & Stringa_Codifica(CStr("0"), AgroKey_EncoderDecoder, Server) &
                                    "&n=" &
                                    "&s=" & Stringa_Codifica(CStr(objParametriConcimazione_2017.Sa_Cod), AgroKey_EncoderDecoder, Server) &
                                    "&t=" &
                                    "&m=" & Stringa_Codifica(CStr(modalitaMenuBsPcPua), AgroKey_EncoderDecoder, Server)

                        'Case enum_CodificaPagPianoConcimazione.Menu

                        '    'richiamo la pagina di gestione
                        '    TargetURL = "~/PianoConcimazione_Menu.aspx" & _
                        '                "?p=" & _
                        '                Stringa_Codifica(CStr(objParametriConcimazione_2017.Piva), AgroKey_EncoderDecoder, Server) & _
                        '                "&o=" & _
                        '                Stringa_Codifica(CStr("0"), AgroKey_EncoderDecoder, Server) & _
                        '                "&n=" & _
                        '                "&s=" & Stringa_Codifica(CStr(objParametriConcimazione_2017.Sa_Cod), AgroKey_EncoderDecoder, Server) & _
                        '                "&t="

                    Case enum_PaginePianoConcimazione_2017.PUA_Dichiarazione_Effluenti

                        Dim xRileggiEf As New AgronicaCoreContabDAL.Ricette_R
                        Dim dtRic As DataTable = xRileggiEf.Leggi(
                            objParametriConcimazione_2017.PianoConcimazione_Testata_Cod,
                            objParametriConcimazione_2017.Piva, 0, 0, 0,
                            AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "", "", objParametri_Server
                        )

                        If dtRic.Rows.Count > 0 Then

                            Dim tipoOp As Integer = enum_TipoOperazioneDB.Modifica
                            Dim Regolamento_Cod As Integer
                            Dim puaCod As Integer = dtRic(0)("Programmazione_Cod")

                            Dim xLeggiPua As New AgronicaCorePUA_DAL.PUA_Testata_R
                            Dim dtPuaLetto As DataTable = xLeggiPua.Leggi(0, puaCod, objParametriConcimazione_2017.Piva, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

                            Regolamento_Cod = dtPuaLetto(0)("Regolamento_Cod")

                            TargetURL = "~/PUA/PUA_Piano_Distribuzione.aspx?" &
                                                                "tipo=" & Stringa_Codifica(CStr(dtRic(0)("Tipo_Ricetta")), AgroKey_EncoderDecoder, Server) &
                                                                "&o=" & Stringa_Codifica(CStr(tipoOp), AgroKey_EncoderDecoder, Server) &
                                                                "&p=" & Stringa_Codifica(CStr(objParametriConcimazione_2017.Piva), AgroKey_EncoderDecoder, Server) &
                                                                "&q=" & Stringa_Codifica(CStr(puaCod), AgroKey_EncoderDecoder, Server) &
                                                                "&r=" & Stringa_Codifica(CStr(Regolamento_Cod), AgroKey_EncoderDecoder, Server) &
                                                                "&data_da=" & Stringa_Codifica(CStr(dtRic(0)("validita_inizio")), AgroKey_EncoderDecoder, Server) &
                                                                "&data_a=" & Stringa_Codifica(CStr(dtRic(0)("validita_fine")), AgroKey_EncoderDecoder, Server) &
                                                                "&ric=" & Stringa_Codifica(CStr(objParametriConcimazione_2017.PianoConcimazione_Testata_Cod), AgroKey_EncoderDecoder, Server)
                        Else

                            'richiamo la pagina di gestione
                            TargetURL = "~/PianoConcimazione_MenuBS.aspx" &
                                        "?p=" &
                                        Stringa_Codifica(CStr(objParametriConcimazione_2017.Piva), AgroKey_EncoderDecoder, Server) &
                                        "&o=" &
                                        Stringa_Codifica(CStr(objParametriConcimazione_2017.Tipo_Operazione), AgroKey_EncoderDecoder, Server) &
                                        "&n=" &
                                        "&s=" & Stringa_Codifica(CStr(objParametriConcimazione_2017.Sa_Cod), AgroKey_EncoderDecoder, Server) &
                                        "&t="

                        End If




                    Case enum_PaginePianoConcimazione_2017.PCB_Inserimento

                        TargetURL = "~/PC_Bilancio/PCB_Inserimento.aspx" &
                                    "?p=" &
                                    Stringa_Codifica(CStr(objParametriConcimazione_2017.Piva), AgroKey_EncoderDecoder, Server) &
                                    "&o=" &
                                    Stringa_Codifica(CStr(objParametriConcimazione_2017.Tipo_Operazione), AgroKey_EncoderDecoder, Server) &
                                    "&n=" &
                                    "&s=" & Stringa_Codifica(CStr(objParametriConcimazione_2017.Sa_Cod), AgroKey_EncoderDecoder, Server) &
                                    "&tipo=" & Stringa_Codifica(CStr(objParametriConcimazione_2017.Tipo_Concimazione), AgroKey_EncoderDecoder, Server)

                    Case enum_PaginePianoConcimazione_2017.PCB_InserimentoMultiplo

                        TargetURL = "~/PC_Bilancio/PCB_InserimentoMultiplo.aspx" &
                                    "?p=" &
                                    Stringa_Codifica(CStr(objParametriConcimazione_2017.Piva), AgroKey_EncoderDecoder, Server) &
                                    "&o=" &
                                    Stringa_Codifica(CStr(objParametriConcimazione_2017.Tipo_Operazione), AgroKey_EncoderDecoder, Server) &
                                    "&q=" &
                                    Stringa_Codifica(CStr(objParametriConcimazione_2017.PianoConcimazione_Testata_Cod), AgroKey_EncoderDecoder, Server) &
                                    "&s=" &
                                    Stringa_Codifica(CStr(objParametriConcimazione_2017.Sa_Cod), AgroKey_EncoderDecoder, Server) &
                                    "&tipo=" &
                                    Stringa_Codifica(CStr(objParametriConcimazione_2017.Tipo_Concimazione), AgroKey_EncoderDecoder, Server)

                        'Case enum_PaginePianoConcimazione_2017.PCS_Inserimento

                        '    TargetURL = "~/PC_Semplificato/PCS_Inserimento.aspx" & _
                        '                "?p=" & _
                        '                Stringa_Codifica(CStr(objParametriConcimazione_2017.Piva), AgroKey_EncoderDecoder, Server) & _
                        '                "&o=" & _
                        '                Stringa_Codifica(CStr(objParametriConcimazione_2017.Tipo_Operazione), AgroKey_EncoderDecoder, Server) & _
                        '                "&n=" & _
                        '                "&s=" & Stringa_Codifica(CStr(objParametriConcimazione_2017.Sa_Cod), AgroKey_EncoderDecoder, Server) & _
                        '                "&t="

                        'Case enum_PaginePianoConcimazione_2017.PCS_InserimentoMultiplo

                        '    TargetURL = "~/PC_Semplificato/PCS_InserimentoMultiplo.aspx" & _
                        '                "?p=" & _
                        '                Stringa_Codifica(CStr(objParametriConcimazione_2017.Piva), AgroKey_EncoderDecoder, Server) & _
                        '                "&o=" & _
                        '                Stringa_Codifica(CStr(objParametriConcimazione_2017.Tipo_Operazione), AgroKey_EncoderDecoder, Server) & _
                        '                "&n=" & _
                        '                "&s=" & Stringa_Codifica(CStr(objParametriConcimazione_2017.Sa_Cod), AgroKey_EncoderDecoder, Server) & _
                        '                "&t="

                    Case enum_PaginePianoConcimazione_2017.FiltraEdEsporta
                        TargetURL = "~/PC_Stampe/PC_PUA_FiltraEsporta.aspx"


                    Case Else

                        'richiamo la pagina di gestione
                        TargetURL = "~/PianoConcimazione_MenuBS.aspx" &
                                    "?p=" &
                                    Stringa_Codifica(CStr(objParametriConcimazione_2017.Piva), AgroKey_EncoderDecoder, Server) &
                                    "&o=" &
                                    Stringa_Codifica(CStr(objParametriConcimazione_2017.Tipo_Operazione), AgroKey_EncoderDecoder, Server) &
                                    "&n=" &
                                    "&s=" & Stringa_Codifica(CStr(objParametriConcimazione_2017.Sa_Cod), AgroKey_EncoderDecoder, Server) &
                                    "&t="



                End Select

                Redirect(TargetURL)

            End If

        End If
    End Sub

    Private Sub Redirect(ByVal targetUrl As String)

        Dim objParametri_Super_Server As AgronicaCoreParametri = Nothing
        Dim objParametri_Server As AgronicaCoreParametri = Nothing

        If Not IsNothing(Session("ASG_objParametri_Super_Server")) Then
            objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        End If
        If Not IsNothing(Session("ASG_objParametri_Server")) Then
            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        End If
        Dim idSezione = 0

        Dim apiController As CoreApiControllerFactory = New CoreApiControllerFactory
        If Not IsNothing(objParametri_Super_Server) AndAlso Not IsNothing(objParametri_Server) Then
            apiController.Inizializza(objParametri_Super_Server, objParametri_Server)
            idSezione = apiController.DammiIdSezioneDaQueryString()
        End If

        If apiController.CanUseAPI AndAlso apiController.VersioneHeader = "2022" Then

            If idSezione <> 0 Then
                targetUrl = AgronicaCoreUtility.Varie.aggiungiAQueryString(targetUrl, "idBC", Stringa_Codifica(idSezione, AgroKey_EncoderDecoder))
            End If

            If Not IsNothing(Request.QueryString("sidebar")) Then
                targetUrl = AgronicaCoreUtility.Varie.aggiungiAQueryString(targetUrl, "sidebar", "off")
            End If

        End If

        Response.Redirect(targetUrl)

    End Sub

#Region "Funzione Commentata"

    ''###############################################################################################
    'Private Sub Page_Load_old(ByVal sender As System.Object, ByVal e As System.EventArgs) ' Handles MyBase.Load

    '    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    '    'IMPOSTA IL NUMERO DI MINUTI DOPO I QUALI
    '    'LA PAGINA MEMORIZZATA NELLA CACHE SCADE

    '    Response.Expires = 0


    '    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

    '    Dim UtenteAbilitato As Boolean
    '    Dim strXmlVariabilistampe As String
    '    Dim StrParametri As String
    '    Dim XmlDoc As New System.Xml.XmlDocument
    '    Dim XmlParametri As System.Xml.XmlElement
    '    Dim XML_VariabiliPianoConcimazione As System.Xml.XmlElement
    '    Dim XML_Parametri As System.Xml.XmlElement
    '    Dim XML_VariabiliSessione As System.Xml.XmlElement


    '    Dim xOperazione As String
    '    Dim xChiave As String
    '    Dim xPianoConcimazione_Testata As String

    '    Dim Unid As String
    '    Dim Cn_Server As String
    '    Dim Piva As String
    '    Dim Sa_Cod As Integer

    '    Dim AgronicaCore_Flag_CancellazioneLogica As String = ""
    '    Dim AgronicaCore_Flag_Visibilita As String = ""
    '    Dim AgronicaCore_DirectoryLOG As String = ""
    '    Dim AgronicaCore_FileNameLOG As String = ""


    '    '--------------------------------------------------
    '    '--------------------------------------------------
    '    '--------------------------------------------------
    '    '--------------------------------------------------
    '    '--------------------------------------------------
    '    '-----  NUOVO PROTOCOLLO COMUNICAZIONE TRA SITI
    '    '--------------------------------------------------
    '    '-----  Dal sito CHIAMANTE arriva 
    '    '-----  la chiave di lettura (Unid) del record, 
    '    '-----  della tabella Web_Parametri, 
    '    '-----  che contiene i parametri passati.
    '    '-----  Tale record deve poi essere cancellato.
    '    '--------------------------------------------------
    '    '-----  N.B. DEVE cmq rimanere in piedi 
    '    '-----  la vecchia versione x compatibilità col LAN
    '    '--------------------------------------------------
    '    '--------------------------------------------------
    '    '--------------------------------------------------
    '    '--------------------------------------------------
    '    '--------------------------------------------------
    '    '----------------------------------------------------------------

    '    '----------------------------------------------------------------
    '    'VECCHIA VERSIONE 
    '    'parametri inviati dal sito chiamante tramite il POST
    '    If Not Request.Form("TxtPARAMETRI") Is Nothing Then

    '        TxtRisultato.Text = Request.Form("TxtPARAMETRI").ToString()

    '        If TxtRisultato.Text <> "" Then

    '            strXmlVariabilistampe = Stringa_Decodifica_Nuova( _
    '                                        TxtRisultato.Text, _
    '                                        AgroKey_EncoderDecoder, _
    '                                        Server)

    '            StrParametri = strXmlVariabilistampe

    '        Else

    '            Response.Redirect("Messaggi/AccessoNegato.htm")

    '        End If

    '        '----------------------------------------------------------------
    '        'NUOVA VERSIONE 
    '        'parametri salvati nella tabella Web_Parametri
    '    Else

    '        If Not Request.QueryString("unid") Is Nothing Then

    '            Unid = Stringa_Decodifica( _
    '                            Request.QueryString("unid").ToString, _
    '                            AgroKey_EncoderDecoder, _
    '                            Server)

    '            Cn_Server = Stringa_Decodifica( _
    '                            Request.QueryString("cn").ToString, _
    '                            AgroKey_EncoderDecoder, _
    '                            Server)

    '            If Not Unid Is Nothing And Not Cn_Server Is Nothing Then

    '                StrParametri = NewCom_Web_Parametri_Leggi(Server, Session, Page, Cn_Server, Unid)

    '                NewCom_Web_Parametri_Cancella(Server, Session, Page, Cn_Server, Unid)

    '            Else

    '                Response.Redirect("Messaggi/AccessoNegato.htm")

    '            End If

    '        Else

    '            Response.Redirect("Messaggi/AccessoNegato.htm")

    '        End If

    '    End If

    '    'Carico la stringa xml in un nuovo documento
    '    XmlDoc = New System.Xml.XmlDocument

    '    If StrParametri <> "" Then

    '        XmlDoc.LoadXml(StrParametri)

    '        If XmlDoc.HasChildNodes Then

    '            '------------------------------------------------------------------
    '            'NUOVA VERSIONE
    '            '------------------------------------------------------------------
    '            'se ho NODO Parametri
    '            'imposto tutte le variabili di sessione che mi servono x il sito
    '            '------------------------------------------------------------------
    '            XML_Parametri = XmlDoc.SelectSingleNode("Parametri")

    '            If Not XML_Parametri Is Nothing Then

    '                '------------------------------------------------------------------
    '                'NODO ---> VariabiliSessione
    '                '------------------------------------------------------------------
    '                XML_VariabiliSessione = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("VariabiliSessione")

    '                If Not XML_VariabiliSessione Is Nothing Then

    '                    Session("ASG_Utente_Username") = XML_VariabiliSessione.GetAttribute("utente_usr")
    '                    Session("ASG_Utente_Password") = XML_VariabiliSessione.GetAttribute("utente_pwd")
    '                    Session("ASG_Utente_CodFiscale") = XML_VariabiliSessione.GetAttribute("utente_codfiscale")
    '                    Session("ASG_Utente_Username_Crypt") = XML_VariabiliSessione.GetAttribute(LCase("Utente_Usr_Crypt"))
    '                    Session("ASG_Utente_Password_Crypt") = XML_VariabiliSessione.GetAttribute(LCase("Utente_Pwd_Crypt"))

    '                    Session("ASG_SuperUser_Username") = XML_VariabiliSessione.GetAttribute("superuser_usr")
    '                    Session("ASG_SuperUser_Password") = XML_VariabiliSessione.GetAttribute("superuser_pwd")
    '                    Session("ASG_SuperUser_CodFiscale") = XML_VariabiliSessione.GetAttribute("superuser_piva")
    '                    Session("ASG_SuperUser_Username_Crypt") = XML_VariabiliSessione.GetAttribute(LCase("SuperUser_Usr_Crypt"))
    '                    Session("ASG_SuperUser_Password_Crypt") = XML_VariabiliSessione.GetAttribute(LCase("SuperUser_Pwd_Crypt"))

    '                    Session("ASG_ProgressivoGIAS") = XML_VariabiliSessione.GetAttribute("progressivo_gias")

    '                    Session("ASG_Connessione_Server") = XML_VariabiliSessione.GetAttribute("cn_server")
    '                    Session("ASG_Connessione_Utenti") = XML_VariabiliSessione.GetAttribute("cn_utenti")

    '                    Session("ASG_StringaConnessione_Server") = XML_VariabiliSessione.GetAttribute("stringa_cn_server")
    '                    Session("ASG_StringaConnessione_Utenti") = XML_VariabiliSessione.GetAttribute("stringa_cn_utenti")


    '                    If Not IsDate(XML_VariabiliSessione.GetAttribute("finestratemporale_inizio")) Then
    '                        Session("ASG_FinestraTemporale_Inizio") = CDate("01/01/1900")
    '                    Else
    '                        Session("ASG_FinestraTemporale_Inizio") = CDate(XML_VariabiliSessione.GetAttribute("finestratemporale_inizio"))
    '                    End If

    '                    If Not IsDate(XML_VariabiliSessione.GetAttribute("finestratemporale_fine")) Then
    '                        Session("ASG_FinestraTemporale_Fine") = CDate("31/12/2100")
    '                    Else
    '                        Session("ASG_FinestraTemporale_Fine") = CDate(XML_VariabiliSessione.GetAttribute("finestratemporale_fine"))
    '                    End If

    '                    AgronicaCore_Flag_CancellazioneLogica = XML_VariabiliSessione.GetAttribute("agronicacore_flag_cancellazionelogica")
    '                    AgronicaCore_Flag_Visibilita = XML_VariabiliSessione.GetAttribute("agronicacore_flag_visibilita")
    '                    AgronicaCore_DirectoryLOG = XML_VariabiliSessione.GetAttribute("pathdirectorylog")
    '                    AgronicaCore_FileNameLOG = XML_VariabiliSessione.GetAttribute("agronicacore_filenamelog")

    '                    '<VariabiliSessione  progressivo_gias=”” cn_server=”” cn_utenti=”” 
    '                    'utente_usr=”” utente_pwd=”” superuser_usr=”” superuser_pwd=”” superuser_piva=”” />

    '                    ''|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||

    '                End If

    '                '------------------------------------------------------------------
    '                'NODO ---> VariabiliPianoConcimazione
    '                '------------------------------------------------------------------
    '                XML_VariabiliPianoConcimazione = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("VariabiliPianoConcimazione")

    '                If Not XML_VariabiliPianoConcimazione Is Nothing Then

    '                    Piva = XML_VariabiliPianoConcimazione.GetAttribute("piva")

    '                    If XML_VariabiliPianoConcimazione.GetAttribute("sa_cod") <> Nothing Then
    '                        If XML_VariabiliPianoConcimazione.GetAttribute("sa_cod") <> "" Then
    '                            Sa_Cod = CInt(XML_VariabiliPianoConcimazione.GetAttribute("sa_cod"))
    '                        Else
    '                            Sa_Cod = 0
    '                        End If
    '                    Else
    '                        Sa_Cod = 0
    '                    End If

    '                    Qs_PagSelezionata = XML_VariabiliPianoConcimazione.GetAttribute("paginapianoconcimazionirichiesta")


    '                End If

    '            End If


    '            '--------------------------------------------------------
    '            '----- Carico l'elenco delle Icone Specie Vegetali ------
    '            '--------------------------------------------------------

    '            Dim Elenco_Icone_SpecieVegetali As String

    '            Call Recupera_Elenco_Icone_SpecieVegetali(Elenco_Icone_SpecieVegetali)

    '            Session("Elenco_Icone_SpecieVegetali") = Elenco_Icone_SpecieVegetali


    '            '--------------------------------------------------------
    '            '--------------------------------------------------------
    '            '--------------------------------------------------------
    '            'a seconda della pagina ricavo i parametri specifici che mi interessano
    '            Select Case Qs_PagSelezionata

    '                Case enum_CodificaPagPianoConcimazione.Menu

    '                    'Non faccio una cippa

    '                Case enum_CodificaPagPianoConcimazione.InserimentoTestata

    '                    xChiave = XML_VariabiliPianoConcimazione.GetAttribute("nodochiave")
    '                    xOperazione = XML_VariabiliPianoConcimazione.GetAttribute("operazione")
    '                    xPianoConcimazione_Testata = XML_VariabiliPianoConcimazione.GetAttribute("pianoconcimazione_testata")

    '            End Select

    '        Else

    '            Response.Redirect("Messaggi/AccessoNegato.htm")


    '        End If

    '    Else

    '        Response.Redirect("Messaggi/AccessoNegato.htm")

    '    End If

    '    ''##############################################################
    '    ''#####  Verifico Credenziali di Accesso  ######################
    '    ''##############################################################

    '    ''----- Verifico che l'utente sia autenticato

    '    'If Session("ASG_Utente_Username") = "" Then
    '    '    Response.Redirect("Messaggi/AccessoNegato.htm")
    '    'End If


    '    ''----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

    '    'Dim strDummy As String      'controllo accesso negato.....

    '    'UtenteAbilitato = Controlla_Permessi_Utente_2( _
    '    '                        Server, Session, Page, _
    '    '                        Session("ASG_Utente_Username"), _
    '    '                        Session("ASG_IdServizio"), _
    '    '                        TipiEnumerativi.enum_Security_Attivita.SupportoDecisioni_PianoConcimazione, _
    '    '                        TipiEnumerativi.enum_Security_Operazione.Lettura, _
    '    '                        strDummy)

    '    ''----- Se l'utente non ha il permesso per visualizzare la pagina ... lo butto fuori

    '    'If UtenteAbilitato = False Then
    '    '    Response.Redirect("Messaggi/AccessoNegato.htm")
    '    'End If


    '    '---------------------------------------------------------
    '    '---------------------------------------------------------
    '    'verifico quale pagina devo accedere --> imposto il TargetURL
    '    '---------------------------------------------------------
    '    '---------------------------------------------------------
    '    Select Case Qs_PagSelezionata

    '        Case enum_CodificaPagPianoConcimazione.Menu

    '            'p = PIVA azienda
    '            'o = operazione
    '            'n = kiave dell'appezzamento

    '            'richiamo la pagina di gestione
    '            TargetURL = PaginaLink_PianoConcimazione_Menu & _
    '                        "?p=" & _
    '                        Stringa_Codifica(CStr(Piva), AgroKey_EncoderDecoder, Server) & _
    '                        "&o=" & _
    '                        Stringa_Codifica(CStr("0"), AgroKey_EncoderDecoder, Server) & _
    '                        "&n=" & _
    '                        "&s=" & Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) & _
    '                        "&t="


    '        Case enum_CodificaPagPianoConcimazione.InserimentoTestata


    '            TargetURL = "inserimento.aspx" & _
    '                                "?n=" + Stringa_Codifica(CStr(xChiave), AgroKey_EncoderDecoder, Server) + _
    '                                "&o=" + Stringa_Codifica(CStr(xOperazione), AgroKey_EncoderDecoder, Server) + _
    '                                "&p=" + Stringa_Codifica(CStr(Piva), AgroKey_EncoderDecoder, Server) + _
    '                                "&q=" + Stringa_Codifica(CStr(xPianoConcimazione_Testata), AgroKey_EncoderDecoder, Server) + _
    '                                "&t="

    '    End Select

    '    '##############################################################################################
    '    '####################    CREAZIONE OBJPARAMETRI E VALORIZZAZIONE NELLA SESSIONE  ##############
    '    '##############################################################################################

    '    Crea_ObjParametri(AgronicaCore_Flag_CancellazioneLogica, _
    '                      AgronicaCore_Flag_Visibilita, _
    '                      AgronicaCore_DirectoryLOG, _
    '                      AgronicaCore_FileNameLOG)

    '    'Salto alla pagina
    '    Response.Redirect(TargetURL)


    'End Sub

#End Region

    '############################################################################
    Private Sub Recupera_Elenco_Icone_SpecieVegetali(ByRef Elenco_Icone_SpecieVegetali As String)

        Dim Percorso As String
        Dim NomeFile As String
        Dim Str_Veg_Cod As String

        'Inizializzo
        Elenco_Icone_SpecieVegetali = ""

        'Recupero il percorso reale della directory
        Percorso = Server.MapPath("AB_Immagini/IconeVegetali").ToString

        'Recupero 
        NomeFile = FileSystem.Dir(Percorso & "\*.ico")

        Do While NomeFile <> ""

            'Elimino l'estensione
            Str_Veg_Cod = Microsoft.VisualBasic.Strings.Left(NomeFile, 7)

            'Tolgo gli zeri di troppo
            Str_Veg_Cod = CInt(Str_Veg_Cod).ToString

            'Aggiungo il nuovo Veg_Cod all'elenco
            Elenco_Icone_SpecieVegetali += ":" & Str_Veg_Cod

            'Avanti un altro ...
            NomeFile = FileSystem.Dir()

        Loop

        'Tolgo il separatore iniziale
        Elenco_Icone_SpecieVegetali = Mid(Elenco_Icone_SpecieVegetali, 2)

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


End Class
