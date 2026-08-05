

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreXML.XML_Stampe
Imports AgronicaCoreVarieBIZ

Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreAnagrafeDAL


Public Class ConfigurazioneImballiProdotto
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

#Region "script services Carica Griglia imballi prodotto"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaImballiProdotto(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim leggi As New Configurazione_Imballaggi_R
            r.RispostaStringa =
                leggi.Leggi_Configurazione_Imballaggi(piva, objParametri_Server)

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

#Region "script services aggiorna proprietà imballaggi per prodotto"
    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaImballiProdotto(ByVal piva As String, Modulo_Generazione As Integer, Tipo_Config As Integer, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim scrivi As New Configurazione_Imballaggi_BIZ_W
            r.RispostaStringa =
                scrivi.AggiornaImballiProdottoBIZ(piva, Modulo_Generazione, Tipo_Config, righeInserite, righeModificate, righeCancellate, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=False)
        End Try

        Return r
    End Function

#End Region

#Region "Caricamento"

    Private Sub caricaControlli()

        'Select Case objParametriAgenda.Tipo_Operazione

        '    Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura

        '    Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica, TipiEnumerativi.enum_TipoOperazioneDB.Lettura
        '        RipristinaControlliDaAgenda()

        'End Select
    End Sub

#End Region




    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master().Lbl_Titolo.Text = "Configurazione Beni di confezionamento / specie / varietà / peso contenuto"

        inizializzoObjParametri()
        inizializzoParametriPagina()

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Gestione_Imballaggi,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Gestione_Imballaggi,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        If UtenteAbilitatoLettura = False Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                            AgroKey_EncoderDecoder, _
                                            Server)

        If Not Page.IsPostBack Then
            caricaControlli()
        End If

    End Sub


    Private Sub inizializzoParametriPagina()


    End Sub

    ''Public Shared Function DT_to_Json_Testata_Griglia_Campionamento(ByVal dt As DataTable, ByVal PermessiScrittura As Boolean) As RispostaStandard

    ''    Dim r As New RispostaStandard

    ''    'creo la lista delle colonne da visualizzare
    ''    Dim l As New List(Of ColonneNome)

    ''    Dim c As New ColonneNome("", "", "")


    ''    'aggiungo i pulsanti per modifica ed eliminazione
    ''    c = New ColonneNome("Id_TestataGriglia", "Azioni", "string")

    ''    Dim listaBtn = New List(Of btnAzioni)
    ''    listaBtn.Add(New btnAzioni("fa-share calibri", "", "Calibri"))
    ''    listaBtn.Add(New btnAzioni("fa-lemon-o prodotti", "", "Prodotti"))

    ''    If PermessiScrittura = True Then
    ''        listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "", "Modifica"))
    ''        listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "", "Cancella"))
    ''    End If

    ''    Dim tool As New ToolStandard(listaBtn)
    ''        c._FormatoParticolare = tool.toString()
    ''        c._Filtrabile = False
    ''        l.Add(c)


    ''        c = New ColonneNome("Id_TestataGriglia", "Sel.", "string")
    ''    c._Filtrabile = True
    ''    c._ColonnaDiSelezione = True
    ''    c._hidden = True

    ''    l.Add(c)

    ''    c = New ColonneNome("des_TestataGriglia", "Descrizione", "string")
    ''    'c._placeHolder = "..."
    ''    l.Add(c)

    ''    c = New ColonneNome("Validita_Inizio", "Data inizio", "date")
    ''    c._Filtrabile = False
    ''    l.Add(c)

    ''    c = New ColonneNome("Validita_Fine", "Data fine", "date")
    ''    c._Filtrabile = False
    ''    l.Add(c)

    ''    'Aggiungo la tabella in sessione
    ''    Dim nomeVarDtInSession As String = "WAExport_TestateGrigliaCampionamento"
    ''    HttpContext.Current.Session.Add(nomeVarDtInSession, dt)

    ''    'ritorno la tabella trasformata in json (aggiustando anche le colonne)
    ''    Dim js As New AgronicaCoreDataProvider.JSON_DataTable
    ''    r.RispostaStringa = js.JSON_DataTable(dt, l)
    ''    r.opzioniWatable.nomeVarDtInSession = nomeVarDtInSession
    ''    r.opzioniWatable.PrefissoNomeFileExport = "ExportTestateGrigliaCampionamento"
    ''    r.RispostaOK = True

    ''    Return r

    ''End Function

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

End Class