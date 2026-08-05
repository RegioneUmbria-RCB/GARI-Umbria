Imports System.Configuration
Imports System.Web
Imports Agronica.Helpers.SAML
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreModelsSTD.profilazione
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.exceptions

Public Class Inizializzatore
    Inherits AgronicaCoreDataProvider.DataProvider


    '--------------------------------------------------------------------------------------------------------------
    '------------------CHIAMATA QUANDO SI ENTRA NELLA PAGINA DEFAULT DELL'ONLINE-------------------------------
    '-----------------E QUANDO SI FA IL LOGOUT E SI VA NELLA DEFAULT-----------------------------------------------
    '----------------------------------------------------------------------------------------------
    'Al momento utilizza i parametri letti dal webconfig e dalla sessione, dovrà invece
    'utilizzare e creare l'oggetto agrowebconfig e l'objparametri per connessione al superserver
    'da usare solo per online che deve avere nel webconfig le 
    'uniche chiavi necessarie, quelle per connettersi al superserver (metaschema che sia)
    'per gli altri siti occorre che nei passaggi vengano passati i parametri per creare l'objparametrisuperserver
    'anche l'online al ritorno sarebbe meglio che utilizzasse questo modo invece che il webconfig
    'che sarebbe meglio che venisse utilizzato solo quando la pagina default chiama questa funzione
    '--------------------------------------------------------------------------------------------------
    '--------------------------------------------------------------------------------------------------
    Public Sub InizializzaSito_GiasOnLine(ByRef objSession As System.Web.SessionState.HttpSessionState)

        '--------------------------------------------------------------------------------------------------
        '--------------------------------------------------------------------------------------------------
        'da revisionare e vedere cosa e dove tenere per memorizzare parametri dopo pogout

        'memorizzo le variabili utili di ingresso al gias (come la querystring)
        Dim filtriSito As AgronicaCoreDataProvider.AgronicaCoreParametriFiltroIngressoSitoOnline
        If Not IsNothing(objSession("Filtri_Siti_Per_PaginaDefault")) Then
            filtriSito = objSession("Filtri_Siti_Per_PaginaDefault")
        Else
            filtriSito.ID_DB = 0 'usato in querystring/viewstate
            filtriSito.TipoDB = enum_Tipo_DB.GIAS_SERVER
            filtriSito.Server = "" 'usato in querystring/viewstate
            filtriSito.DB = "" 'usato in querystring/viewstate
            filtriSito.Provider = ""
            filtriSito.UserId = ""
            filtriSito.Password = ""
            filtriSito.PivaSuperUser = "" 'usato in querystring/viewstate
            filtriSito.Note = ""
            filtriSito.Progressivo = 0 'usato in querystring/viewstate
            filtriSito.Descrizione = ""
            filtriSito.Filtro_Temp_Inizio = Date.Today
            filtriSito.Filtro_Temp_Fine = Date.Today
        End If
        '--------------------------------------------------------------------------------------------------
        '--------------------------------------------------------------------------------------------------

        'pulisco la sessione
        objSession.Clear()

        objSession("Filtri_Siti_Per_PaginaDefault") = filtriSito



        'Gestione superserver o gias_server in base alla chiave del webconfig Super_Server
        If ConfigurationManager.AppSettings("Super_Server") = "true" Then


            'La prima volta che entro nel sito o aggioeno la sessione devo 
            'creare stringa connessione superserver
            'creare agrowebconfig superserver
            'creare / inizializzare altri parametri
            'creare objparametrisuperserver 
            'Chiamo questa funzione che fa tutto
            Inizializza_Super_Sito_Da_Config_GiasOnLine(objSession)

        Else

            'Inizializzo la sessione (copiata da global.asax quando si inizializza sito, 
            'da modificare e leggere tutto dalla tabella configurazione sito del superserver)            
            'non dal config
            'comunque la chiamo solo senza superserver cosa vedo cosa serve e cosa no
            'la versione nuova dovrà usare solo AgroWebConfig
            InizializzoSessione_Old(objSession)
            Inizializza_Sito_Specifico_Versione_Standard(objSession)

        End If

    End Sub

    Private Sub InizializzoSessione_Old(ByRef Session As System.Web.SessionState.HttpSessionState)

        'Gestione superserver o gias_server in base alla chiave del webconfig Super_Server
        'per testare modifiche e pulire dai vari utilizzi sessione
        'non inizializzo niente se ho la chiave che fa usare superserver
        If ConfigurationManager.AppSettings("Super_Server") = "true" Then
            Exit Sub
        End If

        'Variabili di comodo per gestire l'architettura disconnessa
        Session("DataTable") = New DataTable
        Session("myOleDap") = New OleDb.OleDbDataAdapter

        'Disciplinari Privati
        If (Not IsNothing(ConfigurationManager.AppSettings("Flag_DisciplinarePrivato"))) AndAlso
            (ConfigurationManager.AppSettings("Flag_DisciplinarePrivato") <> "") Then
            Session("permessoDPIPrivati") = CBool(ConfigurationManager.AppSettings("Flag_DisciplinarePrivato"))
        Else
            Session("permessoDPIPrivati") = False
        End If

        'Soglie
        If (Not IsNothing(ConfigurationManager.AppSettings("Flag_SoglieAttive"))) AndAlso
                   (ConfigurationManager.AppSettings("Flag_SoglieAttive") <> "") Then
            Session("SoglieAttive") = CBool(ConfigurationManager.AppSettings("Flag_SoglieAttive"))
        Else
            Session("SoglieAttive") = False
        End If

        'Variabili di appoggio
        Session("FlagAggiornaControlli") = "0"
        Session("VersioneAlbero") = 0
        Session("PartitaIVA") = ""
        Session("AlberoImprese_SaCod") = ""
        Session("Pagina_Messaggio") = ""
        Session("Elenco_Icone_SpecieVegetali") = ""

        '|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        '     AGRONICA STARGATE: NUOVE VARIABILI DI SESSIONE
        Session("ASG_Utente_Username") = ""
        Session("ASG_Utente_Password") = ""
        Session("ASG_Utente_Username_Crypt") = ""
        Session("ASG_Utente_Password_Crypt") = ""
        Session("ASG_Utente_CodFiscale") = ""

        Session("ASG_SuperUser_Username") = ""
        Session("ASG_SuperUser_Password") = ""
        Session("ASG_SuperUser_Username_Crypt") = ""
        Session("ASG_SuperUser_Password_Crypt") = ""
        Session("ASG_SuperUser_CodFiscale") = ""

        Session("ASG_ProgressivoGIAS") = ""

        Session("ASG_PathFileINI") = ""
        Session("ASG_IdServizio") = ""

        Session("ASG_StringaConnessione_Server") = ""
        Session("ASG_StringaConnessione_Utenti") = ""

        Session("ASG_Connessione_Server") = ""
        Session("ASG_Connessione_Tabelle") = ""
        Session("ASG_Connessione_Utenti") = ""
        Session("ASG_Connessione_DPI") = ""
        Session("ASG_Connessione_LOG") = ""

        Session("ASG_FinestraTemporale_Inizio") = ""
        Session("ASG_FinestraTemporale_Fine") = ""
        '|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||

        'Variabili di Sessione di appoggio usate nella Contabilita:

        'Vettore di stringhe
        'Utilizzata per passare i dati dalla FormProdotto alla Bolla
        'Viene passato l'xml del carico o scarico di magazzino,
        'dal quale si preleva il movimento_dettaglio
        Session("matrix_XML_To_Documento") = Nothing

        'Datatable dei prodotti  presenti nella griglia della Bolla
        '(deve essere memorizzato perché poi si chiama la FormProdotto
        'e non deve andare perso)
        Session("DT_Prodotti_nel_Doc") = Nothing

        'Stringa xml (costituita dal tag Movimento_Dettaglio) del prodotto,
        'selezionato nella griglia della bolla, 
        'che si deve modificare nella FormProdotto
        Session("vet_XML_To_FormProdotto") = Nothing

    End Sub


    '#################################################################################################
    Private Sub Inizializza_Super_Sito_Da_Config_GiasOnLine(
                                ByRef Session As System.Web.SessionState.HttpSessionState
                                )

        'Gli oggetti sotto devono essere creati nell'ordine

        '--------------------------------------------------------------------
        'creazione stringa connessione al sito
        Dim Super_Server_PivaSuperUser As String = ""
        Dim StringaConnessione_Super_Server As String = ""
        If Sicurezza.ExistStringaConnessione(Sicurezza.ID_DB_Super_Server) Then
            StringaConnessione_Super_Server = Sicurezza.ID_DB_Super_Server
        Else
            StringaConnessione_Super_Server = Crea_Stringa_Connessione_Super_Server_Da_Config_GiasOnLine(Super_Server_PivaSuperUser)
        End If

        Inizializza_Super_Sito_Da_Config_GiasOnLine(Super_Server_PivaSuperUser, StringaConnessione_Super_Server, Session)

    End Sub


    '#################################################################################################
    Private Sub Inizializza_Super_Sito_Da_Config_GiasOnLine(
                                ByVal Super_Server_PivaSuperUser As String,
                                ByVal StringaConnessione_Super_Server As String,
                                ByRef Session As System.Web.SessionState.HttpSessionState
                                )

        'Gli oggetti sotto devono essere creati nell'ordine

        '--------------------------------------------------------------------
        'Passata la stringa connessione al sito

        '--------------------------------------------------------------------
        'Creo AgroWebConfig dalla tabella configurazione_siti del superserver
        'Una volta autenticati e impostato il server occorrerà ricrearlo
        'passandogli i due objparametri (metaschema e server)
        'in modo che sia lui a leggere entrambi e decidere la priorità delle chiavi
        'creo un objparam_temp per avere la stringa connessione con cui creare agrowebconfig leggendo dalla tabella
        'i parametri dato che non  sono è ancora stato creato il weconfig saranno letti dalla sessione o impostati dei
        'valori di default, non ha importanza dato che è un oggetto temporaneo
        Dim objparam_temp As AgronicaCoreParametri = Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(StringaConnessione_Super_Server, Super_Server_PivaSuperUser, Nothing, Session)
        Dim AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objparam_temp, True)

        '--------------------------------------------------------------------
        'Inizializzo le variabili della sessione non presenti in AgroWebConfig/configurazione_siti
        InizializzoVariabiliSessione_GiasOnline_NonPresentiNel_Webconfig(AgroWebConfig, Session)

        Dim objParametri_Super_Server = Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(StringaConnessione_Super_Server, Super_Server_PivaSuperUser, AgroWebConfig, Session)

        Dim linguaBrowserUtente = "it"
        Dim listLingue As String() = Web.HttpContext.Current.Request.UserLanguages
        If Not IsNothing(listLingue) Then
            linguaBrowserUtente = listLingue(0).Split("-")(0)
        End If
        Dim objLingua As New AgronicaCoreDataProvider.Lingua With {.CodiceISO = linguaBrowserUtente}
        objLingua.Calcola_LinguaCod_da_CodiceISO()

        Session("LinguaCorrente") = objLingua
        objParametri_Super_Server.Lingua_Cod = objLingua.Lingua_cod

        Session("ASG_Super_Server_Link_Gias_Base") = AgroWebConfig.LinkGiasBase
        objParametri_Super_Server.LinkGiasBase = AgroWebConfig.LinkGiasBase

        '--------------------------------------------------------------------
        'creazione objparametri_super_server (quello finale)
        Session("ASG_objParametri_Super_Server") = Nothing
        Session("ASG_objParametri_Super_Server") = objParametri_Super_Server
        Session("ASG_StringaConnessione_Super_Server") = StringaConnessione_Super_Server
        Session("ASG_Super_Server_PivaSuperUser") = Super_Server_PivaSuperUser


    End Sub

    Public Function InizializzaObjParametriSuperServer() As AgronicaCoreParametri
        'Gestione superserver o gias_server in base alla chiave del webconfig Super_Server
        If ConfigurationManager.AppSettings("Super_Server") = "true" Then

            'creazione stringa connessione al sito
            Dim Super_Server_PivaSuperUser As String = ""
            Dim StringaConnessione_Super_Server As String = Crea_Stringa_Connessione_Super_Server_Da_Config_GiasOnLine(Super_Server_PivaSuperUser)

            Dim objParametriSuperServer As AgronicaCoreParametri = Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(StringaConnessione_Super_Server, Super_Server_PivaSuperUser, Nothing, Nothing)
            Return objParametriSuperServer
        Else
            Return Nothing
        End If
    End Function


    '#################################################################################################
    Public Sub InizializzoVariabiliSessione_GiasOnline_NonPresentiNel_Webconfig(ByRef AgroWebConfig As AgronicaCoreGestioneRichieste.AgroWebConfig,
                                                                                ByRef Session As System.Web.SessionState.HttpSessionState)



        'Variabili di appoggio
        'Session("FlagAggiornaControlli") = "0" 'non presente nella soluzione
        Session("VersioneAlbero") = 0 'presente ma non nel webconfig
        Session("PartitaIVA") = "" 'presente ma non nel webconfig
        Session("AlberoImprese_SaCod") = "" 'presente ma non nel webconfig
        Session("Pagina_Messaggio") = "" 'presente ma non nel webconfig
        Session("Elenco_Icone_SpecieVegetali") = "" 'presente ma non nel webconfig

        '|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        '     AGRONICA STARGATE: NUOVE VARIABILI DI SESSIONE
        Session("ASG_Utente_Username") = ""
        Session("ASG_Utente_Password") = ""
        Session("ASG_Utente_Username_Crypt") = ""
        Session("ASG_Utente_Password_Crypt") = ""
        Session("ASG_Utente_CodFiscale") = ""

        Session("ASG_SuperUser_Username") = ""
        Session("ASG_SuperUser_Password") = ""
        Session("ASG_SuperUser_Username_Crypt") = ""
        Session("ASG_SuperUser_Password_Crypt") = ""
        Session("ASG_SuperUser_CodFiscale") = ""

        Session("ASG_ProgressivoGIAS") = ""

        Session("ASG_PathFileINI") = ""
        Session("ASG_IdServizio") = ""

        Session("ASG_StringaConnessione_Server") = ""
        Session("ASG_StringaConnessione_Utenti") = ""

        Session("ASG_Connessione_Server") = ""
        Session("ASG_Connessione_Tabelle") = ""
        Session("ASG_Connessione_Utenti") = ""
        Session("ASG_Connessione_DPI") = ""
        Session("ASG_Connessione_LOG") = ""

        Session("ASG_FinestraTemporale_Inizio") = ""
        Session("ASG_FinestraTemporale_Fine") = ""
        '|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||

        'Variabili di Sessione di appoggio usate nella Contabilita:

        'Vettore di stringhe
        'Utilizzata per passare i dati dalla FormProdotto alla Bolla
        'Viene passato l'xml del carico o scarico di magazzino,
        'dal quale si preleva il movimento_dettaglio
        Session("matrix_XML_To_Documento") = Nothing

        'Datatable dei prodotti  presenti nella griglia della Bolla
        '(deve essere memorizzato perché poi si chiama la FormProdotto
        'e non deve andare perso)
        Session("DT_Prodotti_nel_Doc") = Nothing

        'Stringa xml (costituita dal tag Movimento_Dettaglio) del prodotto,
        'selezionato nella griglia della bolla, 
        'che si deve modificare nella FormProdotto
        Session("vet_XML_To_FormProdotto") = Nothing



    End Sub





    '----------------------------------------------------------------------------------------------------------------------
    '--------------------------------------------------VERSIONE CON SUPERSERVER------------------------------------------------
    '------------------CREA OBJPARAMETRI_SERVER E UTENTI USANDO OBJ_PARAMETRI_SUPER_SERVER QUINDI IL SUPER_SERVER--------------------------
    '-------------------------------- E L'ID SERVER NELLA TABELLA CONNESSIONI------------------------------------------------
    '------------------------------------------------------------------------------------------------------------------------
    '------------------CHIAMATA QUANDO SI DALLA PAGINA DEFAULT DELL'ONLINE SI SELEZIONA UN SERVER GIAS-------------------------------
    '-----------------O QUANDO SI PASSA DA UN SITO ALL'ALTRO -------------------------------------------------------
    '--------------------------------------------------------------------------------------------------------------
    'Al momento utilizza i parametri letti dal webconfig e dalla sessione, dovrà invece
    'utilizzare e creare l'oggetto agrowebconfig e l'objparametri per connessione al superserver
    '--------------------------------------------------------------------------------------------------
    'Dopo l'autenticazione verranno creati objparametri server e utenti---------------------------------
    '--------------------------------------------------------------------------------------------------
    '--------------------------------------------------------------------------------------------------
    '#################################################################################################
    Public Sub Inizializza_Sito_Specifico_Con_Superserver_GiasOnLine(
                            ByVal Username As String,
                            ByVal Password As String,
                            ByVal ID_DB_Server As Integer,
                            ByRef Ritorno_objParametri_Server As AgronicaCoreParametri,
                            ByRef Ritorno_objParametri_Utenti As AgronicaCoreParametri,
                            ByRef Ritorno_Descrizione As String,
                            ByRef Ritorno_Note As String,
                            ByRef Ritorno_Descrizione_Utenti As String,
                            ByRef Ritorno_Note_Utenti As String,
                            ByRef objParametri_Super_Server As AgronicaCoreParametri,
                            ByRef objSession As System.Web.SessionState.HttpSessionState,
                            Optional ByVal xFiltroAggiuntivoAutenticazione As String = "")

        'Gli oggetti sotto devono essere creati nell'ordine

        '--------------------------------------------------------------------
        'creazione stringa connessione ai siti
        Dim StringaConnessione_Server As String = ""
        Dim StringaConnessione_Utenti As String = ""
        'Dim ID_DB_Server ce l'ho
        Dim ID_DB_Utenti As String = "" '= CStr(CInt(dt.Rows(0).Item("ID_DB"))) 'lo leggo quando leggo il record del db utenti in connessioni
        Dim PivaSuperUser As String = ""
        Dim NomeServer As String = ""

        Dim AgroWebConfig As AgronicaCoreGestioneRichieste.AgroWebConfig = Nothing

        Try

            'Input ID_DB_Server e objParametri_Super_Server per leggere
            'output tutti gli altri
            Dim progressivo_Gias_Server As Integer
            Dim progressivo_Gias_Utenti As Integer
            Ricavo_Parametri_Server_Utenti_Da_Id_Db_Server(objParametri_Super_Server,
                                                           ID_DB_Server,
                                                           Ritorno_Descrizione,
                                                           Ritorno_Note,
                                                           Ritorno_Descrizione_Utenti,
                                                           Ritorno_Note_Utenti,
                                                           StringaConnessione_Server,
                                                           StringaConnessione_Utenti,
                                                           ID_DB_Utenti,
                                                           PivaSuperUser,
                                                           NomeServer,
                                                           progressivo_Gias_Server,
                                                           progressivo_Gias_Utenti)

            If StringaConnessione_Server = "" Then
                Throw New Exception("la stringa per la connessione al gia server è vuota")
            End If
            If StringaConnessione_Utenti = "" Then
                Throw New Exception("la stringa per la connessione al gias utenti è vuota")
            End If
            If PivaSuperUser = "" Then
                Throw New Exception("la PivaSuperUser è vuota")
            End If
            If NomeServer = "" Then
                Throw New Exception("il NomeServer è vuoto")
            End If

            Inizializza_Sito_Specifico_Con_Superserver_GiasOnLine(
                Username,
                Password,
                Ritorno_objParametri_Server,
                Ritorno_objParametri_Utenti,
                AgroWebConfig,
                PivaSuperUser,
                StringaConnessione_Server,
                StringaConnessione_Utenti,
                objParametri_Super_Server,
                objSession,
                xFiltroAggiuntivoAutenticazione
             )

            'salvasessione oggettti i objparametriutentieserver

            'parametri obbligatori:
            objSession("ASG_objParametri_Server") = Ritorno_objParametri_Server
            objSession("ASG_objParametri_Utenti") = Ritorno_objParametri_Utenti
            'AgroWebConfig salvato in automatico, devo rinizializzarlo però se va male con superserver

            'altri parametri, occorrono finchè non si tolgono e usano solo gli obbligatori
            'li ho nella tabella connessioni del superserver, sono l'id univoco intero >1 delle tabelle

            'quelli in sessione saranno da eliminare
            SalvaParametriInSessione_SarannoDaEliminareDefinitamente(objSession,
                                                                     CStr(ID_DB_Server), CStr(ID_DB_Utenti),
                                                                     StringaConnessione_Server, StringaConnessione_Utenti)

            'Disciplinari Privati
            objSession("permessoDPIPrivati") = CBool(AgroWebConfig.Flag_DisciplinarePrivato)

            'Soglie
            objSession("SoglieAttive") = CBool(AgroWebConfig.Flag_SoglieAttive)


            objSession("ASG_PathFileINI") = AgroWebConfig.StarGate_PathFileINI
            objSession("ASG_IdServizio") = "5"

            objSession("ASG_Connessione_DPI") = AgroWebConfig.Connessione_ONLINE_DPI
            objSession("ASG_Connessione_LOG") = CStr(ID_DB_Utenti)

            objSession("ASG_FinestraTemporale_Inizio") = Ritorno_objParametri_Utenti.FinestraTemporaleInizio
            objSession("ASG_FinestraTemporale_Fine") = Ritorno_objParametri_Utenti.FinestraTemporaleFine

            objSession("ASG_AgronicaCore_Flag_CancellazioneLogica") = Ritorno_objParametri_Utenti.FlagCancellazioneLogica
            objSession("ASG_AgronicaCore_Flag_Visibilita") = Ritorno_objParametri_Utenti.FlagVisibilita
            objSession("ASG_AgronicaCore_DirectoryLOG") = AgroWebConfig.AgronicaCore_DirectoryLOG

            ' Inizializzo configurazione x compressione chiamate Ajax
            ConfigurazioneAjaxFactory.Instance(Ritorno_objParametri_Server)


        Catch ex1 As AgroEccezioni_LoginFallito_Exception
            Throw ex1

        Catch ex As Exception


            'puliscisessione

            'parametri obbligatori:
            objSession("ASG_objParametri_Server") = Nothing
            objSession("ASG_objParametri_Utenti") = Nothing
            'AgroWebConfig non devo cancellarlo, ma rimettere quello del superserver
            AgroWebConfig = New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, True)

            'altri parametri, occorrono finchè non si tolgono e usano solo gli obbligatori
            objSession("ASG_Connessione_Server") = Nothing
            objSession("ASG_Connessione_Tabelle") = Nothing
            objSession("ASG_Connessione_Utenti") = Nothing
            objSession("ASG_StringaConnessione_Server") = Nothing
            objSession("ASG_StringaConnessione_Utenti") = Nothing

            'Disciplinari Privati
            objSession("permessoDPIPrivati") = Nothing

            'Soglie
            objSession("SoglieAttive") = Nothing

            objSession("ASG_PathFileINI") = Nothing
            objSession("ASG_IdServizio") = Nothing

            objSession("ASG_Connessione_DPI") = Nothing
            objSession("ASG_Connessione_LOG") = Nothing

            objSession("ASG_FinestraTemporale_Inizio") = Nothing
            objSession("ASG_FinestraTemporale_Fine") = Nothing

            objSession("ASG_AgronicaCore_Flag_CancellazioneLogica") = Nothing
            objSession("ASG_AgronicaCore_Flag_Visibilita") = Nothing
            objSession("ASG_AgronicaCore_DirectoryLOG") = Nothing

            If Debugger.IsAttached Then
                Throw New Exception("Non è stato possibile creare le connessioni ai database: " & ex.Message, ex)
            Else
                Throw New Exception("Non è stato possibile creare le connessioni ai database ", ex)
            End If

        End Try



    End Sub

    ''' <summary>Checks the visibility of the user who's trying to log in.</summary>
    ''' <remarks><list type="table">
    ''' <item> (05/08/2024) Add check to forbid log in to users with empty visibility. </item>
    ''' </list></remarks>
    ''' <exception cref="AgroEccezioni_LoginFallito_Exception">If visibility is empty.</exception>
    Private Sub verifyVisibility(ByVal Ritorno_objParametri_Utenti As AgronicaCoreParametri, ByVal username As String)
        Dim userProfiles As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim p2 = Ritorno_objParametri_Utenti.CreateDeepCopy(Ritorno_objParametri_Utenti)
        p2.SuperUserUsername = ""
        Dim sqlStr = userProfiles.Leggi_FiltroUtenteSQL(username, 0, String.Empty, String.Empty, p2)
        If sqlStr.Contains("########") Then
            Throw New AgroEccezioni_LoginFallito_Exception("nessuna azienda presente in visibilità.")
        End If
    End Sub

    Public Sub Inizializza_Sito_Specifico_Con_Superserver_GiasOnLine_SAML(
                            cfgRiconoscimentoUtente As SMLClaimsCfg,
                            ByRef Username As String,
                            ByRef Password As String,
                            ByVal ID_DB_Server As Integer,
                            ByRef Ritorno_objParametri_Server As AgronicaCoreParametri,
                            ByRef Ritorno_objParametri_Utenti As AgronicaCoreParametri,
                            ByRef Ritorno_Descrizione As String,
                            ByRef Ritorno_Note As String,
                            ByRef Ritorno_Descrizione_Utenti As String,
                            ByRef Ritorno_Note_Utenti As String,
                            ByRef objParametri_Super_Server As AgronicaCoreParametri,
                            ByRef objSession As System.Web.SessionState.HttpSessionState,
                            Optional ByVal xFiltroAggiuntivoAutenticazione As String = "",
                            Optional accountEStatoSelezionato As String = Nothing,
                            Optional usernameScelto As String = Nothing)

        'Gli oggetti sotto devono essere creati nell'ordine

        '--------------------------------------------------------------------
        'creazione stringa connessione ai siti
        Dim StringaConnessione_Server As String = ""
        Dim StringaConnessione_Utenti As String = ""
        'Dim ID_DB_Server ce l'ho
        Dim ID_DB_Utenti As String = "" '= CStr(CInt(dt.Rows(0).Item("ID_DB"))) 'lo leggo quando leggo il record del db utenti in connessioni
        Dim PivaSuperUser As String = ""
        Dim NomeServer As String = ""

        Dim AgroWebConfig As AgronicaCoreGestioneRichieste.AgroWebConfig = Nothing

        Try

            'Input ID_DB_Server e objParametri_Super_Server per leggere
            'output tutti gli altri
            Dim progressivo_Gias_Server As Integer
            Dim progressivo_Gias_Utenti As Integer
            Ricavo_Parametri_Server_Utenti_Da_Id_Db_Server(objParametri_Super_Server,
                                                           ID_DB_Server,
                                                           Ritorno_Descrizione,
                                                           Ritorno_Note,
                                                           Ritorno_Descrizione_Utenti,
                                                           Ritorno_Note_Utenti,
                                                           StringaConnessione_Server,
                                                           StringaConnessione_Utenti,
                                                           ID_DB_Utenti,
                                                           PivaSuperUser,
                                                           NomeServer,
                                                           progressivo_Gias_Server,
                                                           progressivo_Gias_Utenti)

            If StringaConnessione_Server = "" Then
                Throw New Exception("la stringa per la connessione al gia server è vuota")
            End If
            If StringaConnessione_Utenti = "" Then
                Throw New Exception("la stringa per la connessione al gias utenti è vuota")
            End If
            If PivaSuperUser = "" Then
                Throw New Exception("la PivaSuperUser è vuota")
            End If
            If NomeServer = "" Then
                Throw New Exception("il NomeServer è vuoto")
            End If

            Inizializza_Sito_Specifico_Con_Superserver_GiasOnLine_SAML_Attiva(
                cfgRiconoscimentoUtente,
                Username,
                Password,
                Ritorno_objParametri_Server,
                Ritorno_objParametri_Utenti,
                AgroWebConfig,
                PivaSuperUser,
                StringaConnessione_Server,
                StringaConnessione_Utenti,
                objParametri_Super_Server,
                objSession,
                xFiltroAggiuntivoAutenticazione,
                accountEStatoSelezionato,
                usernameScelto
             )

            'salvasessione oggettti i objparametriutentieserver

            'parametri obbligatori:
            objSession("ASG_objParametri_Server") = Ritorno_objParametri_Server
            objSession("ASG_objParametri_Utenti") = Ritorno_objParametri_Utenti
            'AgroWebConfig salvato in automatico, devo rinizializzarlo però se va male con superserver

            'altri parametri, occorrono finchè non si tolgono e usano solo gli obbligatori
            'li ho nella tabella connessioni del superserver, sono l'id univoco intero >1 delle tabelle

            'quelli in sessione saranno da eliminare
            SalvaParametriInSessione_SarannoDaEliminareDefinitamente(objSession,
                                                                     CStr(ID_DB_Server), CStr(ID_DB_Utenti),
                                                                     StringaConnessione_Server, StringaConnessione_Utenti)

            'Disciplinari Privati
            objSession("permessoDPIPrivati") = CBool(AgroWebConfig.Flag_DisciplinarePrivato)

            'Soglie
            objSession("SoglieAttive") = CBool(AgroWebConfig.Flag_SoglieAttive)


            objSession("ASG_PathFileINI") = AgroWebConfig.StarGate_PathFileINI
            objSession("ASG_IdServizio") = "5"

            objSession("ASG_Connessione_DPI") = AgroWebConfig.Connessione_ONLINE_DPI
            objSession("ASG_Connessione_LOG") = CStr(ID_DB_Utenti)

            objSession("ASG_FinestraTemporale_Inizio") = Ritorno_objParametri_Utenti.FinestraTemporaleInizio
            objSession("ASG_FinestraTemporale_Fine") = Ritorno_objParametri_Utenti.FinestraTemporaleFine

            objSession("ASG_AgronicaCore_Flag_CancellazioneLogica") = Ritorno_objParametri_Utenti.FlagCancellazioneLogica
            objSession("ASG_AgronicaCore_Flag_Visibilita") = Ritorno_objParametri_Utenti.FlagVisibilita
            objSession("ASG_AgronicaCore_DirectoryLOG") = AgroWebConfig.AgronicaCore_DirectoryLOG

        Catch ex1 As AgroEccezioni_LoginFallito_Exception
            Throw ex1
        Catch ex2 As RecoverableSPIDMultipleAccountLoginException
            Throw ex2

        Catch ex As Exception


            'puliscisessione

            'parametri obbligatori:
            objSession("ASG_objParametri_Server") = Nothing
            objSession("ASG_objParametri_Utenti") = Nothing
            'AgroWebConfig non devo cancellarlo, ma rimettere quello del superserver
            AgroWebConfig = New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, True)

            'altri parametri, occorrono finchè non si tolgono e usano solo gli obbligatori
            objSession("ASG_Connessione_Server") = Nothing
            objSession("ASG_Connessione_Tabelle") = Nothing
            objSession("ASG_Connessione_Utenti") = Nothing
            objSession("ASG_StringaConnessione_Server") = Nothing
            objSession("ASG_StringaConnessione_Utenti") = Nothing

            'Disciplinari Privati
            objSession("permessoDPIPrivati") = Nothing

            'Soglie
            objSession("SoglieAttive") = Nothing

            objSession("ASG_PathFileINI") = Nothing
            objSession("ASG_IdServizio") = Nothing

            objSession("ASG_Connessione_DPI") = Nothing
            objSession("ASG_Connessione_LOG") = Nothing

            objSession("ASG_FinestraTemporale_Inizio") = Nothing
            objSession("ASG_FinestraTemporale_Fine") = Nothing

            objSession("ASG_AgronicaCore_Flag_CancellazioneLogica") = Nothing
            objSession("ASG_AgronicaCore_Flag_Visibilita") = Nothing
            objSession("ASG_AgronicaCore_DirectoryLOG") = Nothing

            If Debugger.IsAttached Then
                Throw New Exception("Non è stato possibile creare le connessioni ai database: " & ex.Message, ex)
            Else
                Throw New Exception("Non è stato possibile creare le connessioni ai database ", ex)
            End If

        End Try



    End Sub

    '#################################################################################################
    Public Sub Inizializza_Sito_Specifico_Con_Superserver_GiasOnLine(
                                ByVal Username As String,
                                ByVal Password As String,
                                ByRef Ritorno_objParametri_Server As AgronicaCoreParametri,
                                ByRef Ritorno_objParametri_Utenti As AgronicaCoreParametri,
                                ByRef Ritorno_AgroWebConfig As AgronicaCoreGestioneRichieste.AgroWebConfig,
                                ByVal Server_PivaSuperUser As String,
                                ByVal StringaConnessione_Server As String,
                                ByVal StringaConnessione_Utenti As String,
                                ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                ByRef Session As System.Web.SessionState.HttpSessionState,
                                Optional ByVal xFiltroAggiuntivoAutenticazione As String = ""
                                )

        'Gli oggetti sotto devono essere creati nell'ordine

        '--------------------------------------------------------------------
        'Passata la stringa connessione al sito

        '--------------------------------------------------------------------
        'Avevo prima creato  AgroWebConfig dalla tabella configurazione_siti del superserver
        'Ora occorrerà ricrearlo
        'passandogli i due objparametri (metaschema e server)
        'in modo che sia lui a leggere entrambi e decidere la priorità delle chiavi
        'creo un objparam_temp per avere la stringa connessione con cui creare agrowebconfig leggendo dalla tabella
        'i parametri dato che non  sono è ancora stato creato il weconfig saranno letti dalla sessione o impostati dei
        'valori di default, non ha importanza dato che è un oggetto temporaneo
        Dim objparam_temp As AgronicaCoreParametri = Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(StringaConnessione_Server, Server_PivaSuperUser, Nothing, Session)
        Ritorno_AgroWebConfig = New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objparam_temp, True)



        '--------------------------------------------------------------------
        'creazione objparametri_server  (quello finale) e utenti

        'prima creo objparametri da inizializzare, temporanei, solo con la stringa connessione
        'e passando Ritorno_AgroWebConfig per gli altri parametri della configurazione db utente
        'poi l'autenticazione inserirà i valori finali
        Ritorno_objParametri_Server = Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(StringaConnessione_Server, Server_PivaSuperUser, Ritorno_AgroWebConfig, Session)
        Ritorno_objParametri_Utenti = Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(StringaConnessione_Utenti, Server_PivaSuperUser, Ritorno_AgroWebConfig, Session)
        Dim handleConfigSiti As New Configurazione_Siti_R
        Dim dtConfigSitiGiasBase As DataTable = handleConfigSiti.Leggi(0, "LinkGiasBase", "", "", Ritorno_objParametri_Server)
        If dtConfigSitiGiasBase.Rows.Count > 0 Then
            Ritorno_objParametri_Server.LinkGiasBase = CStr(dtConfigSitiGiasBase.Rows(0)("Valore"))
            Ritorno_objParametri_Utenti.LinkGiasBase = CStr(dtConfigSitiGiasBase.Rows(0)("Valore"))
        End If

        verifyVisibility(Ritorno_objParametri_Utenti, Username)

        Dim autenticadal As New AgronicaCoreUtentiDAL.AutenticaUtente
        'AUTENTICO E CREO OBJPARAMETRI SERVER E UTENTI FINALI
        'attenzione, qua dentro vengono inserite in sessione molti parametri
        'deo pian piano toglierli e infilarli nell'objparametri se non ci sono
        'o nell'agrowebconfig, poi toglierli da tutti i progetti e fare riferimento a solo quei 4 oggetti
        'objparametri server, utenti, superserver, agrowebconfig
        Dim dtConfigSiti As DataTable = handleConfigSiti.Leggi(0, "AbilitaHashPassword", "", "", Ritorno_objParametri_Server)
        Dim hashPasswordAbilitato As Boolean = False
        If dtConfigSiti.Rows.Count > 0 Then
            hashPasswordAbilitato = dtConfigSiti.Rows(0)("Valore")
        End If
        autenticadal.ASG_Autenticazione_Utente_verificaPPT(Username, Password, hashPasswordAbilitato, Ritorno_objParametri_Server, Ritorno_objParametri_Utenti)
        Dim risp As String = autenticadal.ASG_Autenticazione_Utente(Username, Password, 1, 0, False, hashPasswordAbilitato, (Ritorno_objParametri_Server), (Ritorno_objParametri_Utenti), xFiltroAggiuntivoAutenticazione)
        ConfigurazioneEstesaSqlProviderFactory.Instance(Ritorno_objParametri_Server)
        If risp <> "" Then
            Ritorno_objParametri_Server = Nothing
            'Ritorno_objParametri_Utenti = Nothing

            Throw New AgroEccezioni_LoginFallito_Exception(Gias.AutenticazioneFallita & ". " & risp)
        Else



            'ritorno Ritorno_objParametri_Server e Ritorno_objParametri_Utenti


        End If



    End Sub

    Private Sub FixSuperuserUsername(ByRef objP_Utenti As AgronicaCoreParametri)
        Dim userReader As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        If objP_Utenti.SuperUserUsername = objP_Utenti.PivaSuperUser Then
            objP_Utenti.SuperUserUsername = userReader.Username_From_CodFisc(objP_Utenti.PivaSuperUser, objP_Utenti)
        End If
    End Sub

    Public Sub Inizializza_Sito_Specifico_Con_Superserver_GiasOnLine_SAML_Attiva(
        cfgRiconoscimentoUtente As SMLClaimsCfg,
        ByRef Username As String,
        ByRef Password As String,
        ByRef Ritorno_objParametri_Server As AgronicaCoreParametri,
        ByRef Ritorno_objParametri_Utenti As AgronicaCoreParametri,
        ByRef Ritorno_AgroWebConfig As AgronicaCoreGestioneRichieste.AgroWebConfig,
        ByVal Server_PivaSuperUser As String,
        ByVal StringaConnessione_Server As String,
        ByVal StringaConnessione_Utenti As String,
        ByRef objParametri_Super_Server As AgronicaCoreParametri,
        ByRef Session As System.Web.SessionState.HttpSessionState,
        Optional ByVal xFiltroAggiuntivoAutenticazione As String = "",
        Optional accountEStatoSelezionato As String = Nothing,
        Optional usernameScelto As String = Nothing
    )
        'Gli oggetti sotto devono essere creati nell'ordine

        '--------------------------------------------------------------------
        'Passata la stringa connessione al sito

        '--------------------------------------------------------------------
        'Avevo prima creato  AgroWebConfig dalla tabella configurazione_siti del superserver
        'Ora occorrerà ricrearlo
        'passandogli i due objparametri (metaschema e server)
        'in modo che sia lui a leggere entrambi e decidere la priorità delle chiavi
        'creo un objparam_temp per avere la stringa connessione con cui creare agrowebconfig leggendo dalla tabella
        'i parametri dato che non  sono è ancora stato creato il weconfig saranno letti dalla sessione o impostati dei
        'valori di default, non ha importanza dato che è un oggetto temporaneo
        Dim objparam_temp As AgronicaCoreParametri = Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(StringaConnessione_Server, Server_PivaSuperUser, Nothing, Session)
        Ritorno_AgroWebConfig = New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objparam_temp, True)

        '--------------------------------------------------------------------
        'creazione objparametri_server  (quello finale) e utenti

        'prima creo objparametri da inizializzare, temporanei, solo con la stringa connessione
        'e passando Ritorno_AgroWebConfig per gli altri parametri della configurazione db utente
        'poi l'autenticazione inserirà i valori finali
        Ritorno_objParametri_Server = Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(StringaConnessione_Server, Server_PivaSuperUser, Ritorno_AgroWebConfig, Session)
        Ritorno_objParametri_Utenti = Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(StringaConnessione_Utenti, Server_PivaSuperUser, Ritorno_AgroWebConfig, Session)
        FixSuperuserUsername(Ritorno_objParametri_Utenti)
        Ritorno_objParametri_Server.SuperUserUsername = Ritorno_objParametri_Utenti.SuperUserUsername
        ConfigurazioneEstesaSqlProviderFactory.Instance(Ritorno_objParametri_Server)
        Dim autenticadal As New AgronicaCoreUtentiDAL.AutenticaUtente
        'AUTENTICO E CREO OBJPARAMETRI SERVER E UTENTI FINALI
        'attenzione, qua dentro vengono inserite in sessione molti parametri
        'deo pian piano toglierli e infilarli nell'objparametri se non ci sono
        'o nell'agrowebconfig, poi toglierli da tutti i progetti e fare riferimento a solo quei 4 oggetti
        'objparametri server, utenti, superserver, agrowebconfig

        'autenticadal.ASG_Autenticazione_Utente_verficaPPT(Username, Password, Ritorno_objParametri_Server, Ritorno_objParametri_Utenti)
        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim objUtentiDettagliDAL As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R

        Dim qrySAMLClaimPerRiconoscereUtente = GetUserRecognitionSAMLClaimQuery(cfgRiconoscimentoUtente)
        Dim trovatoSingoloUtenteSPID = False
        Dim authWithSSO = False

        If accountEStatoSelezionato Is Nothing Then
            ' Pulsante SPID Login è stato selezionato
            If cfgRiconoscimentoUtente.TipoDiSAMLClaimPerRiconoscereUtente = enum_SAML_RiconoscimentoUtente.CodiceFiscale Then
                ReadUserWithSAMLClaimQuery(
                    qrySAMLClaimPerRiconoscereUtente, Ritorno_objParametri_Utenti,
                    Username, trovatoSingoloUtenteSPID
                )
            End If
        End If

        If usernameScelto IsNot Nothing AndAlso usernameScelto <> "" Then
            ' Fai login usando il campo del dropdown list
            Username = usernameScelto
            Dim dtPwd = objUtentiDAL.Leggi2(Username, "", "", Ritorno_objParametri_Utenti)
            If dtPwd.Rows.Count > 0 Then
                Password = dtPwd.Rows(0)("Password")
            End If
        Else
            Dim trovatoUsername = False

            If trovatoSingoloUtenteSPID Then
                ' Esiste un solo utente collegato al codice fiscale in tabella SPID
                trovatoUsername = True
            Else
                Dim params As New ObjParams With {
                    .ObjParametri_Server = Ritorno_objParametri_Server,
                    .ObjParametri_Utenti = Ritorno_objParametri_Utenti
                }

                Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim Profile As Integer = 0
                Dim dtConfigSiti = objConfigSiti.Leggi(0, "SSOLogin_UserCreationConfig", "", "", params.ObjParametri_Server)
                If dtConfigSiti.Rows.Count > 0 Then
                    Dim creationConfig As UserCreationConfig = JsonConvert.DeserializeObject(Of UserCreationConfig)(dtConfigSiti.Rows(0)("Valore"))
                    Profile = creationConfig.profile
                End If

                If Profile <> 0 Then

                    ' Nessun utente collegato al codice fiscale in tabella SPID : lo leggo da anagrafica
                    Dim dt = objUtentiDettagliDAL.Leggi("", 0, enumSelezioneVariabile.Selezione_TabellaCompleta, qrySAMLClaimPerRiconoscereUtente, "", Ritorno_objParametri_Utenti)
                    If dt.Rows.Count > 0 Then
                        If Username = "" Then
                            Username = dt.Rows(0)("Username")
                            trovatoUsername = True

                            UpdateUserFromSpid(cfgRiconoscimentoUtente, params, dt.Select.Single)
                        End If
                    Else
                        ' Non ho trovato l'utente corrispondente ai criteri stabiliti, ne creo uno nuovo
                        Dim _user = CreateNewUserFromSPID(cfgRiconoscimentoUtente, params)
                        If Username = "" AndAlso _user <> "" Then
                            Username = _user
                            trovatoUsername = True
                        End If
                    End If
                Else

                    Dim dt = objUtentiDettagliDAL.Leggi("", 0, enumSelezioneVariabile.Selezione_TabellaCompleta, qrySAMLClaimPerRiconoscereUtente, "", Ritorno_objParametri_Utenti)
                    If dt.Rows.Count > 0 Then
                        Username = dt.Rows(0)("Username")
                        trovatoUsername = True
                    Else
                        Throw New AgroEccezioni_LoginFallito_Exception(Gias.AutenticazioneFallita & ". ")
                    End If
                End If
                'fine if profile
            End If

            If trovatoUsername Then
                ' L'utente è stato scelto
                Dim dtPwd = objUtentiDAL.Leggi2(Username, "", "", Ritorno_objParametri_Utenti)
                If dtPwd.Rows.Count > 0 Then
                    Password = dtPwd.Rows(0)("Password")
                End If
                authWithSSO = True
            End If
        End If

        Dim risp As String = ""
        If Username <> "" AndAlso Password <> "" Then
            Dim handleConfigSiti As New Configurazione_Siti_R
            Dim dtConfigSiti As DataTable = handleConfigSiti.Leggi(0, "AbilitaHashPassword", "", "", Ritorno_objParametri_Server)
            Dim hashPasswordAbilitato As Boolean = False
            If dtConfigSiti.Rows.Count > 0 Then
                hashPasswordAbilitato = dtConfigSiti.Rows(0)("Valore")
            End If
            risp = autenticadal.ASG_Autenticazione_Utente(
                Username, Password, 1, 0, False, hashPasswordAbilitato,
                Ritorno_objParametri_Server, Ritorno_objParametri_Utenti,
                xFiltroAggiuntivoAutenticazione, authWithSSO
            )
        Else
            risp = "Utente SPID non trovato"
        End If


        If risp <> "" Then
            Ritorno_objParametri_Server = Nothing
            'Ritorno_objParametri_Utenti = Nothing
            Throw New AgroEccezioni_LoginFallito_Exception(Gias.AutenticazioneFallita & ". " & risp)
        Else
            'ritorno Ritorno_objParametri_Server e Ritorno_objParametri_Utenti
        End If
    End Sub

    Private Function GetUserRecognitionSAMLClaimQuery(cfgRiconoscimentoUtente As SMLClaimsCfg) As String
        Dim qrySAMLClaimPerRiconoscereUtente As String
        Select Case cfgRiconoscimentoUtente.TipoDiSAMLClaimPerRiconoscereUtente
            Case enum_SAML_RiconoscimentoUtente.CodiceFiscale
                qrySAMLClaimPerRiconoscereUtente =
                " CodFisc = '" & SAMLClaimsControllers.LeggiValoreDataChiave("CodiceFiscale", cfgRiconoscimentoUtente.ListaClaimsPerRiconoscimento) & "' "

            Case enum_SAML_RiconoscimentoUtente.email
                qrySAMLClaimPerRiconoscereUtente =
                " Email = '" & SAMLClaimsControllers.LeggiValoreDataChiave("email", cfgRiconoscimentoUtente.ListaClaimsPerRiconoscimento) & "' "
            Case enum_SAML_RiconoscimentoUtente.email_nome_cognome
                qrySAMLClaimPerRiconoscereUtente =
                "     Email   = '" & SAMLClaimsControllers.LeggiValoreDataChiave("email", cfgRiconoscimentoUtente.ListaClaimsPerRiconoscimento) & "' " &
                " and nome    = '" & SAMLClaimsControllers.LeggiValoreDataChiave("nome", cfgRiconoscimentoUtente.ListaClaimsPerRiconoscimento) & "' " &
                " and cognome = '" & SAMLClaimsControllers.LeggiValoreDataChiave("cognome", cfgRiconoscimentoUtente.ListaClaimsPerRiconoscimento) & "' "
            Case enum_SAML_RiconoscimentoUtente.userID
                qrySAMLClaimPerRiconoscereUtente =
                "     username   = '" & SAMLClaimsControllers.LeggiValoreDataChiave("userID", cfgRiconoscimentoUtente.ListaClaimsPerRiconoscimento) & "' "

            Case Else
                Throw New Exception("Parametro di riconoscimento utente non valido.")
        End Select
        Return qrySAMLClaimPerRiconoscereUtente
    End Function

    Private Sub ReadUserWithSAMLClaimQuery(
        qrySAMLClaimPerRiconoscereUtente As String,
        objParametri_Utenti As AgronicaCoreParametri,
        ByRef Username As String,
        ByRef trovatoSingoloUtenteSPID As Boolean
    )
        Dim objUtentiDettagliDAL As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim utentixCodFiscSVC As New UtentixCodFisc_R
        Dim utentiDT = utentixCodFiscSVC.Leggi(
            "", "",
            qrySAMLClaimPerRiconoscereUtente, "",
            objParametri_Utenti
        )
        If utentiDT.Rows.Count > 0 Then
            Try
                ' Leggo eventuale utente valido con stesso codice fiscale in anagrafica
                Dim dtUtentiAnagrafica = objUtentiDettagliDAL.Leggi(
                    "", 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                    qrySAMLClaimPerRiconoscereUtente, "",
                    objParametri_Utenti, SoloUtentiValidi:=True
                )
                If dtUtentiAnagrafica.Rows.Count > 0 Then
                    Dim usernameAnagrafica = dtUtentiAnagrafica.Rows(0)("Username")
                    ' Controllo che l'utente non sia già presente in tabella SPID
                    If utentiDT.Select("Username = '" & usernameAnagrafica & "'").Count = 0 Then
                        ' Se non presente, lo aggiungo all'elenco utenti
                        Dim drUtentiDT As DataRow = utentiDT.NewRow
                        drUtentiDT("Username") = dtUtentiAnagrafica.Rows(0)("Username")
                        utentiDT.Rows.InsertAt(drUtentiDT, 0)
                    End If
                End If
            Catch ex As Exception
                Dim objLog As New LogProvider
                objLog.Scrivi_LOG(
                    objParametri_Utenti,
                    "AgronicaCoreGestioneRichieste.Inizializzatore.Inizializza_Sito_Specifico_Con_Superserver_GiasOnLine_SAML_Attiva()",
                    ex.Message
                )
            End Try
        End If
        If utentiDT.Rows.Count > 1 Then
            ' Popola il dropdown list
            Dim multAccountsLogin = New UserLinkedAccountRisposta(utentiDT, "Seleziona un account per accedere")
            Throw New RecoverableSPIDMultipleAccountLoginException(multAccountsLogin)
        ElseIf utentiDT.Rows.Count = 1 Then
            ' Esiste un solo utente associato allo SPID
            Username = utentiDT.Rows(0)("Username")
            trovatoSingoloUtenteSPID = True
        End If

    End Sub

    Private Function CreateNewUserFromSPID(clams As SMLClaimsCfg, params As ObjParams) As String
        Dim userWriter As New AgronicaCoreUtentiDAL.UserWriter
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim claims As IEnumerable(Of SAMLClaimsRiconoscimento) = clams.ListaClaimsPerRiconoscimento
        Dim name = claims.FirstOrDefault(Function(x) x.GiasKey = "nome")?.Valore
        Dim surname = claims.FirstOrDefault(Function(x) x.GiasKey = "cognome")?.Valore
        Dim userID = claims.FirstOrDefault(Function(x) x.GiasKey = "userID")?.Valore
        Dim email = claims.FirstOrDefault(Function(x) x.GiasKey = "email")?.Valore
        Dim profile As Integer = 0
        Dim usernameCreated = String.Empty

        Dim dtConfigSiti = objConfigSiti.Leggi(0, "AbilitaHashPassword", "", "", params.ObjParametri_Server)
        Dim hashPwdEnabled As Boolean = False
        If dtConfigSiti.Rows.Count > 0 Then
            hashPwdEnabled = dtConfigSiti.Rows(0)("Valore")
        End If
        dtConfigSiti = objConfigSiti.Leggi(0, "SSOLogin_UserCreationConfig", "", "", params.ObjParametri_Server)
        If dtConfigSiti.Rows.Count > 0 Then
            Dim creationConfig As UserCreationConfig = JsonConvert.DeserializeObject(Of UserCreationConfig)(dtConfigSiti.Rows(0)("Valore"))
            profile = creationConfig.profile
        End If

        If profile = 0 Then
            Throw New InvalidOperationException("Must specify a profile")
        End If

        Select Case clams.TipoDiSAMLClaimPerRiconoscereUtente
            'Case enum_SAML_RiconoscimentoUtente.CodiceFiscale
            'Case enum_SAML_RiconoscimentoUtente.email
            Case enum_SAML_RiconoscimentoUtente.email_nome_cognome
                Dim userCreated = userWriter.CreateFromEmailNameSurname(email, name, surname, profile, params, hashPwdEnabled)
                usernameCreated = If(userCreated, email, String.Empty)

            Case enum_SAML_RiconoscimentoUtente.userID
                Dim userCreated = userWriter.CreateFromSSOData(userID, name, surname, email, profile, params, hashPwdEnabled)
                usernameCreated = If(userCreated, userID, String.Empty)

            Case Else
                Throw New NotImplementedException
        End Select
        If Not String.IsNullOrEmpty(usernameCreated) Then
            SkipGDPR(usernameCreated, params)
        End If
        Return usernameCreated
    End Function

    Private Sub SkipGDPR(username As String, params As ObjParams)
        Dim gdprWriter As New AgronicaCoreUtentiDAL.Utenti_GDPR_Accettazione_W
        Dim codeGDPR = 1
        Dim statusGDPR = 1
        gdprWriter.ScriviXutente(
            username, codeGDPR, statusGDPR, Now, params.ObjParametri_Utenti,
            Now, Now, params.ObjParametri_Utenti.UsernameOperazione
        )
    End Sub

    Private Sub UpdateUserFromSpid(clams As SMLClaimsCfg, params As ObjParams, user As DataRow)
        Dim spidName = clams.ListaClaimsPerRiconoscimento.FirstOrDefault(Function(x) x.GiasKey = "nome")?.Valore
        Dim spidSurname = clams.ListaClaimsPerRiconoscimento.FirstOrDefault(Function(x) x.GiasKey = "cognome")?.Valore
        Dim spidEmail = clams.ListaClaimsPerRiconoscimento.FirstOrDefault(Function(x) x.GiasKey = "email")?.Valore
        Dim name = If(Not String.IsNullOrEmpty(spidName), spidName, CStr(user.Item("nome")))
        Dim surname = If(Not String.IsNullOrEmpty(spidSurname), spidSurname, CStr(user.Item("cognome")))
        Dim email = If(Not String.IsNullOrEmpty(spidEmail), spidEmail, CStr(user.Item("email")))
        Dim userWriter As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W
        userWriter.ModificaDettagliBasePersona(
            user.Item("username"), surname, name, user.Item("tel"), email,
            user.Item("CodFisc"), user.Item("usernameCommerciale"),
            params.ObjParametri_Utenti
        )
    End Sub


    '#################################################################################################
    Public Sub Inizializza_Sito_Specifico_Versione_Standard(ByRef objSession As System.Web.SessionState.HttpSessionState)


        Dim chiave_webconfig As String

        Dim Connessione_Server As String
        Dim Connessione_Tabelle As String
        Dim Connessione_Utenti As String
        Dim Connessione_Disciplinari As String
        Dim Connessione_LogAccessi As String

        Dim Id_Servizio As Integer
        Dim PathFileINI As String
        Dim StringaConnessione_Server As String
        Dim StringaConnessione_Utenti As String

        Dim Finestra_Temp_Inizio As Date
        Dim Finestra_Temp_Fine As Date

        Dim AgronicaCore_Flag_CancellazioneLogica As Integer
        Dim AgronicaCore_Flag_Visibilita As Integer
        Dim AgronicaCore_DirectoryLOG As String

        '============================================================


        '----------------------------------------------------------
        '---------- Codice del SERVIZIO ---------------------------
        '----------------------------------------------------------

        'Definizione del servizio corrente : Gias Online = 5
        Id_Servizio = 5

        PathFileINI = "c:\Agroconnessioni\Connessioni.ini"


        '----------------------------------------------------------
        '---------- CONNESSIONI ai database -----------------------
        '----------------------------------------------------------

        '--- Connessione al database dei dati

        chiave_webconfig = ConfigurationManager.AppSettings("Connessione_ONLINE_Server")

        If chiave_webconfig = "" Then
            Connessione_Server = "cnONLINE_Server"          '"cnONLINE_Server"
        Else
            Connessione_Server = chiave_webconfig
        End If

        '--- Connessione al database degli utenti

        chiave_webconfig = ConfigurationManager.AppSettings("Connessione_ONLINE_Utenti")

        If chiave_webconfig = "" Then
            Connessione_Utenti = "cnONLINE_Utenti"          '"cnONLINE_Utenti"
        Else
            Connessione_Utenti = chiave_webconfig
        End If

        '--- Connessione al database delle tabelle informative

        chiave_webconfig = ConfigurationManager.AppSettings("Connessione_ONLINE_Server")

        If chiave_webconfig = "" Then
            Connessione_Tabelle = "cnONLINE_Server"         '"cnONLINE_Tabelle"
        Else
            Connessione_Tabelle = chiave_webconfig
        End If

        '--- Connessione al database dei Disciplinari

        chiave_webconfig = ConfigurationManager.AppSettings("Connessione_ONLINE_DPI")

        If chiave_webconfig = "" Then
            Connessione_Disciplinari = "cnGIAS_DPI"         '"cnGIAS_DPI"
        Else
            Connessione_Disciplinari = chiave_webconfig
        End If

        '--- Connessione al database dei LOG degli accessi

        chiave_webconfig = ConfigurationManager.AppSettings("Connessione_ONLINE_Utenti")

        If chiave_webconfig = "" Then
            Connessione_LogAccessi = "cnONLINE_Utenti"      '"cnONLINE_Utenti"
        Else
            Connessione_LogAccessi = chiave_webconfig
        End If


        '----------------------------------------------------------
        '---------- Gestione della FINESTRA TEMPORALE -------------
        '----------------------------------------------------------

        Finestra_Temp_Inizio = AGRODATAINIZIO
        Finestra_Temp_Fine = AGRODATAFINE


        '----------------------------------------------------------
        '------------- AGRONICA CORE: IMPOSTAZIONI ----------------
        '----------------------------------------------------------

        chiave_webconfig = CStr(ConfigurationManager.AppSettings("AgronicaCore_Flag_CancellazioneLogica"))

        If chiave_webconfig = "" Then
            AgronicaCore_Flag_CancellazioneLogica = CInt("0") 'default su false
        Else
            AgronicaCore_Flag_CancellazioneLogica = CInt(chiave_webconfig)
        End If

        chiave_webconfig = CStr(ConfigurationManager.AppSettings("AgronicaCore_Flag_Visibilita"))

        If chiave_webconfig = "" Then
            AgronicaCore_Flag_Visibilita = CInt("1") ' default su Visibilita_Solo_NON_Cancellati
        Else
            AgronicaCore_Flag_Visibilita = CInt(chiave_webconfig)
        End If

        chiave_webconfig = CStr(ConfigurationManager.AppSettings("AgronicaCore_DirectoryLOG"))

        If chiave_webconfig = "" Then
            AgronicaCore_DirectoryLOG = "C:\GIASLAN\LOG"
        Else
            AgronicaCore_DirectoryLOG = chiave_webconfig
        End If



        '----------------------------------------------------------
        '----------------------------------------------------------
        '----------------------------------------------------------


        '============================================================
        '============================================================


        objSession("ASG_PathFileINI") = PathFileINI
        objSession("ASG_IdServizio") = Id_Servizio

        objSession("ASG_Connessione_Server") = Connessione_Server
        objSession("ASG_Connessione_Tabelle") = Connessione_Server
        objSession("ASG_Connessione_Utenti") = Connessione_Utenti
        objSession("ASG_Connessione_DPI") = Connessione_Disciplinari
        objSession("ASG_Connessione_LOG") = Connessione_LogAccessi

        objSession("ASG_FinestraTemporale_Inizio") = Finestra_Temp_Inizio
        objSession("ASG_FinestraTemporale_Fine") = Finestra_Temp_Fine

        objSession("ASG_AgronicaCore_Flag_CancellazioneLogica") = AgronicaCore_Flag_CancellazioneLogica
        objSession("ASG_AgronicaCore_Flag_Visibilita") = AgronicaCore_Flag_Visibilita
        objSession("ASG_AgronicaCore_DirectoryLOG") = AgronicaCore_DirectoryLOG


        '----------------------------------------------------------
        '--------------- STRINGA DI CONNESSIONE -------------------
        '----------------------------------------------------------

        'Creo la stringa di connessione ai database dal file connessioni.ini
        Dim objAgronicaCore As New AgronicaCoreDataProvider.DataProvider

        Try

            'Creo la connessione a GIAS_Server
            StringaConnessione_Server = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(PathFileINI, Connessione_Server)

            'Creo la connessione a Utenti
            StringaConnessione_Utenti = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(PathFileINI, Connessione_Utenti)

        Catch ex As Exception


        End Try

        objSession("ASG_StringaConnessione_Server") = StringaConnessione_Server
        objSession("ASG_StringaConnessione_Utenti") = StringaConnessione_Utenti

        'creo agrowebconfig, altrimenti non funziona apripopup per stampe etc,
        'al passaggio tra i siti agrowebconfig viene comunque riletto da db e ricreato, 
        'viene creato con superserver all'ingresso ma non era creato all'ingresso del giasonline
        Dim objparam_temp As AgronicaCoreParametri = Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(StringaConnessione_Server, "", Nothing, objSession)
        Dim AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objparam_temp, True)

        Crea_ObjParametri_Server_E_ObjParametri_Utenti_E_Salva_In_Sessione(objSession,
                                                                            StringaConnessione_Server,
                                                                            StringaConnessione_Utenti)
    End Sub





    '###############################################################################################
    Public Sub Inizializza_Sito_Specifico_Da_Stringa_Passaggio(ByVal StringaConnessione_Super_Server As String,
                                                                ByVal ID_DB_Server As String,
                                                                ByVal Sito_Origine As String,
                                                                ByVal Sito_Destinazione As String,
                                                                ByRef strParametri As String,
                                                                ByRef Session As System.Web.SessionState.HttpSessionState
                                                                )


        Session("Sito_Origine") = Sito_Origine

        '--------------------------------------------------------------------
        'Inizializzo le variabili della sessione passate
        'lo faccio subito perché ho bisogno della stringa connessione server per webconfig
        Dim VariabiliSessione As New VariabiliSessione(strParametri)


        'versione senza StringaConnessione_Super_Server e quindi senza superserver
        If StringaConnessione_Super_Server = "" Then


            '--------------------------------------------------------------------
            'Creo AgroWebConfig 

            'Session("ASG_StringaConnessione_Server") è in sessione, letto dai parametrixml
            Dim objparam_Server_temp2 As AgronicaCoreParametri =
                            Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(
                                        Session("ASG_StringaConnessione_Server"),
                                          "",
                                          Nothing,
                                          Session)

            Dim AgroWebConfig2 As New AgronicaCoreGestioneRichieste.AgroWebConfig(objparam_Server_temp2, True)

            '--------------------------------------------------------------------
            'creazione objparametri server e utenti
            Dim StringaConnessione_Server As String = Session("ASG_StringaConnessione_Server")
            Dim StringaConnessione_Utenti As String = Session("ASG_StringaConnessione_Utenti")
            Crea_ObjParametri_Server_E_ObjParametri_Utenti_E_Salva_In_Sessione(Session, StringaConnessione_Server, StringaConnessione_Utenti)

            Exit Sub



        Else




            '--------------------------------------------------------------------
            'Creo AgroWebConfig 
            Dim objParametri_Super_Server_Temp As AgronicaCoreParametri =
                                    Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(
                                                StringaConnessione_Super_Server,
                                                  "",
                                                  Nothing,
                                                  Session)




            Dim StringaConnessione_Server As String = ""
            Dim StringaConnessione_Utenti As String = ""
            'Dim ID_DB_Server ce l'ho
            Dim ID_DB_Utenti As String = "" 'lo leggo quando leggo il record del db utenti in connessioni
            Dim PivaSuperUser As String = ""
            Dim NomeServer As String = ""
            Dim Ritorno_Descrizione As String = ""
            Dim Ritorno_Note As String = ""
            Dim Ritorno_Descrizione_Utenti As String = ""
            Dim Ritorno_Note_Utenti As String = ""
            'Input ID_DB_Server e objParametri_Super_Server per leggere
            'output tutti gli altri
            Dim progressivo_Gias_Server As Integer
            Dim progressivo_Gias_Utenti As Integer
            Ricavo_Parametri_Server_Utenti_Da_Id_Db_Server(objParametri_Super_Server_Temp,
                                                           CInt(ID_DB_Server),
                                                           Ritorno_Descrizione,
                                                           Ritorno_Note,
                                                           Ritorno_Descrizione_Utenti,
                                                           Ritorno_Note_Utenti,
                                                           StringaConnessione_Server,
                                                           StringaConnessione_Utenti,
                                                           ID_DB_Utenti,
                                                           PivaSuperUser,
                                                           NomeServer,
                                                           progressivo_Gias_Server,
                                                           progressivo_Gias_Utenti)

            If StringaConnessione_Server = "" Then
                Throw New Exception("la stringa per la connessione al gia server è vuota")
            End If
            If StringaConnessione_Utenti = "" Then
                Throw New Exception("la stringa per la connessione al gias utenti è vuota")
            End If
            If PivaSuperUser = "" Then
                Throw New Exception("la PivaSuperUser è vuota")
            End If
            If NomeServer = "" Then
                Throw New Exception("il NomeServer è vuoto")
            End If

            If IsNothing(HttpContext.Current.Session("ASG_SuperUser_CodFiscale")) Or HttpContext.Current.Session("ASG_SuperUser_CodFiscale") = "" Then
                HttpContext.Current.Session("ASG_SuperUser_CodFiscale") = PivaSuperUser
            End If


            'Session("ASG_StringaConnessione_Server") è in sessione, letto dai parametrixml
            Dim objparam_Server_temp As AgronicaCoreParametri =
                            Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(
                                          StringaConnessione_Server,
                                          "",
                                          Nothing,
                                          Session)

            Dim AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server_Temp, objparam_Server_temp, True)

            '--------------------------------------------------------------------
            'Creo in sessione StringaConnessione_Super_Server e objparametrisuperserver

            Dim Super_Server_PivaSuperUser = ""

            If Not IsNothing(Session("ASG_Super_Server_PivaSuperUser")) AndAlso
                    Session("ASG_Super_Server_PivaSuperUser") <> "" Then

                Super_Server_PivaSuperUser = Session("ASG_Super_Server_PivaSuperUser")

            End If

            'creazione objparametri_super_server 
            Session("ASG_StringaConnessione_Super_Server") = Nothing
            Session("ASG_Super_Server_PivaSuperUser") = StringaConnessione_Super_Server
            Session("ASG_objParametri_Super_Server") = Nothing
            Session("ASG_objParametri_Super_Server") = Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(StringaConnessione_Super_Server, Super_Server_PivaSuperUser, AgroWebConfig, Session)


            '--------------------------------------------------------------------
            'creazione objparametri server e utenti
            Crea_ObjParametri_Server_E_ObjParametri_Utenti_E_Salva_In_Sessione(Session, StringaConnessione_Server, StringaConnessione_Utenti)


            'quelli in sessione saranno da eliminare
            SalvaParametriInSessione_SarannoDaEliminareDefinitamente(Session,
                                                                     CStr(ID_DB_Server), CStr(ID_DB_Utenti),
                                                                     StringaConnessione_Server, StringaConnessione_Utenti)

        End If


    End Sub


    'Input ID_DB_Server e objParametri_Super_Server per leggere
    'output tutti gli altri
    Public Shared Sub Ricavo_Parametri_Server_Utenti_Da_Id_Db_Server(ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                                                      ByVal ID_DB_Server As Integer,
                                                                      ByRef Ritorno_Descrizione As String,
                                                                      ByRef Ritorno_Note As String,
                                                                      ByRef Ritorno_Descrizione_Utenti As String,
                                                                      ByRef Ritorno_Note_Utenti As String,
                                                                      ByRef StringaConnessione_Server As String,
                                                                      ByRef StringaConnessione_Utenti As String,
                                                                      ByRef ID_DB_Utenti As String,
                                                                      ByRef PivaSuperUser As String,
                                                                      ByRef NomeServer As String,
                                                                      ByRef progressivo_Gias_Server As Integer,
                                                                      ByRef progressivo_Gias_Utenti As Integer)


        'Input ID_DB_Server e objParametri_Super_Server per leggere
        'output tutti gli altri


        'Creo la connessione a GIAS_Server leggendo dalla tabella
        Dim obj As New AgronicaCoreDataProvider.Connessioni

        'Genero la stringa connessione al db
        StringaConnessione_Server = Utility_Sicurezza.Leggi_Stringa_Connessione(ID_DB_Server, objParametri_Super_Server)
        'StringaConnessione_Server = obj.Leggi_Stringa_Connessione(ID_DB_Server, objParametri_Super_Server)

        'recupero la descrizione del db
        Dim dt As DataTable
        dt = obj.Leggi(ID_DB_Server,
               enum_Tipo_DB.TUTTI,
               "",
               "",
               "",
               "",
               "",
               "",
               "",
               0,
               "",
               AGRODATAINIZIO,
               AGRODATAFINE,
                       "",
                       "",
                       objParametri_Super_Server)

        If dt.Rows.Count = 0 Then
            Throw New Exception("Non è stato trovato il record del server gias selezionato")
        End If
        If dt.Rows.Count > 1 Then
            Throw New Exception("Sono stati caricati più server gias, non è consentito")
        End If
        Ritorno_Descrizione = dt.Rows(0).Item("Descrizione")
        Ritorno_Note = dt.Rows(0).Item("Note")
        PivaSuperUser = dt.Rows(0).Item("PivaSuperUser")
        NomeServer = dt.Rows(0).Item("Server")
        progressivo_Gias_Server = dt.Rows(0).Item("Progressivo")

        If progressivo_Gias_Server < 1 Then
            Throw New Exception("Il progressivo del db in superserver.connessioni deve essere > 0")
        End If

        RicavoParametriDBUtentiDalServer(objParametri_Super_Server,
                                         Ritorno_Descrizione_Utenti,
                                         Ritorno_Note_Utenti,
                                         StringaConnessione_Utenti,
                                         ID_DB_Utenti,
                                         PivaSuperUser,
                                         NomeServer,
                                         progressivo_Gias_Server,
                                         progressivo_Gias_Utenti)
    End Sub



    Private Shared Sub RicavoParametriDBUtentiDalServer(ByRef objParametri_Super_Server_IN As AgronicaCoreParametri,
                                                        ByRef Ritorno_Descrizione_Utenti As String,
                                                        ByRef Ritorno_Note_Utenti As String,
                                                        ByRef StringaConnessione_Utenti As String,
                                                        ByRef ID_DB_Utenti As String,
                                                        ByRef PivaSuperUser_IN As String,
                                                        ByRef NomeServer_IN As String,
                                                        ByRef progressivo_Gias_Server_IN As Integer,
                                                        ByRef progressivo_Gias_Utenti As Integer)
        'Per la ricerca del db utenti in base al server procedo in questo modo,
        'Cerco in connessione il record del db che:
        '   è di tipo enum_Tipo_DB.GIAS_UTENTI =2
        '   risiede sullo stesso server del db gias_server (NomeServer)
        '   ha lo stesso progressivo del db server
        '
        'Mi aspetto di trovarne solo uno, in tal caso lo utilizzo come db utenti
        'Se ne trovo più di uno con quel progressivo lancio eccezione,
        '
        'Se non ne trovo nessuno allora faccio una ricerca senza utilizzare il progressivo
        'quindi cercando in connessioni il record che:
        '   è di tipo enum_Tipo_DB.GIAS_UTENTI =2
        '   risiede sullo stesso server del db gias_server (NomeServer)
        '
        'Mi aspetto di trovarne solo uno, in tal caso lo utilizzo come db utenti
        'Se ne trovo più di uno lancio eccezione
        'Se non ne trovo nessuno lancio eccezione

        Dim obj As New AgronicaCoreDataProvider.Connessioni

        'Recupero il db utenti e la descrizione
        Dim dt As DataTable
        dt = obj.Leggi(0,
               enum_Tipo_DB.GIAS_UTENTI,
                NomeServer_IN,
               "",
               "",
               "",
               "",
               PivaSuperUser_IN,
               "",
               progressivo_Gias_Server_IN,
               "",
               AGRODATAINIZIO,
               AGRODATAFINE,
                       "",
                       "",
                       objParametri_Super_Server_IN)


        If dt.Rows.Count > 1 Then
            Throw New Exception("Sono stati caricati più db utenti del server gias(stessa piva su stesso server e con il progressivo del server), non è consentito, occorre avere o un solo db con lo stesso progressivo o un unico db utenti")
        End If
        If dt.Rows.Count = 1 Then
            'ok ho trovato il db utenti con quel progressivo
            'proseguo

        ElseIf dt.Rows.Count = 0 Then
            'NON ho trovato il db utenti con quel progressivo,
            'quindi lo cerco senza indicare il progressivo e mi aspetto di trovarne uno solo
            'Recupero il db utenti e la descrizione
            dt = obj.Leggi(0,
                   enum_Tipo_DB.GIAS_UTENTI,
                    NomeServer_IN,
                   "",
                   "",
                   "",
                   "",
                   PivaSuperUser_IN,
                   "",
                   0,
                   "",
                   AGRODATAINIZIO,
               AGRODATAFINE,
                           "",
                           "",
                           objParametri_Super_Server_IN)

            If dt.Rows.Count = 0 Then
                Throw New Exception("Non è stato trovato il record del db utenti del server gias selezionato")
            End If
            If dt.Rows.Count > 1 Then
                Throw New Exception("Sono stati caricati più db utenti del server gias(stessa piva su stesso server), non è consentito, o c'è un solo db utenti per ciascun superuser in ciascun server o deve corrispondere al progressivo del db gias server")
            End If

        End If


        'Imposto i valori del db utenti
        ID_DB_Utenti = CInt(dt.Rows(0).Item("ID_DB")) 'lo leggo quando leggo il record del db utenti in connessioni

        Ritorno_Descrizione_Utenti = dt.Rows(0).Item("Descrizione")
        Ritorno_Note_Utenti = dt.Rows(0).Item("Note")
        progressivo_Gias_Utenti = dt.Rows(0).Item("Progressivo")


        'Genero la stringa connessione al db utenti
        StringaConnessione_Utenti = Utility_Sicurezza.Leggi_Stringa_Connessione(ID_DB_Utenti, objParametri_Super_Server_IN)
        'StringaConnessione_Utenti = obj.Leggi_Stringa_Connessione(dt.Rows(0).Item("ID_DB"), objParametri_Super_Server_IN)
    End Sub



    Private Shared Sub SalvaParametriInSessione_SarannoDaEliminareDefinitamente(ByRef Session As System.Web.SessionState.HttpSessionState,
                                                                                   ByVal ID_DB_Server As String,
                                                                                   ByVal ID_DB_Utenti As String,
                                                                                   ByRef StringaConnessione_Server As String,
                                                                                   ByRef StringaConnessione_Utenti As String)
        Session("ASG_Connessione_Server") = ID_DB_Server
        Session("ASG_Connessione_Tabelle") = ID_DB_Server
        Session("ASG_Connessione_Utenti") = ID_DB_Utenti
        Session("ASG_StringaConnessione_Server") = StringaConnessione_Server
        Session("ASG_StringaConnessione_Utenti") = StringaConnessione_Utenti

    End Sub


    '###############################################################################################
    Public Function Verifica_Mirroring()

        Dim Flag_Mirror As Integer

        '-------------------------------------------------
        '---- verifica l'attivazione del MIRRORING -------
        '-------------------------------------------------
        If Not IsNothing(ConfigurationManager.AppSettings("Flag_Mirror")) Then


            If ConfigurationManager.AppSettings("Flag_Mirror") = "SYSTEM_FRAMEWORK" Then

                Flag_Mirror = 0

            Else

                Flag_Mirror = 1

            End If

        End If

        Return Flag_Mirror

    End Function

    '#################################################################################################
    Public Function Crea_Stringa_Connessione_Super_Server_Da_Config_GiasOnLine(
                        ByRef Super_Server_PivaSuperUser As String) As String


        '---------------------------------------------------------------------------------
        '---------- CREO LA STRINGACONNESSIONE ai database SUPER_SERVER ------------------
        '---------------------------------------------------------------------------------

        '--- Connessione al database dei dati
        '<!-- ##### cnONLINE_Super_Server ########
        '<add key="Super_Server" value="true"/>
        '<add key="Super_Server_Provider" value="SQLOLEDB"/>
        '<add key="Super_Server_Server" value="*****"/>
        '<add key="Super_Server_DB" value="GIAS_Super_Server"/>
        '<add key="Super_Server_UserId" value="*****"/>
        '<add key="Super_Server_Password" value="*****"/>
        '<add key="Super_Server_PivaSuperUser" value=""/>

        'tolgo, altrimenti non inizializza superserver negli altri siti quando chiama FindConnessione_Su_Ini_O_Superserver
        If ConfigurationManager.AppSettings("Super_Server") = "true" Then
            'ok
        Else
            Return ""
        End If

        Dim Super_Server_Provider As String = ConfigurationManager.AppSettings("Super_Server_Provider")
        If Super_Server_Provider = "" Then
            Super_Server_Provider = "SQLOLEDB"
        End If

        Dim Super_Server_Server As String = ConfigurationManager.AppSettings("Super_Server_Server")
        If Super_Server_Server = "" Then
            Super_Server_Server = "*****"
        End If

        Dim Super_Server_DB As String = ConfigurationManager.AppSettings("Super_Server_DB")
        If Super_Server_DB = "" Then
            Super_Server_DB = "GIAS_Super_Server"
        End If

        Dim Super_Server_UserId As String = ConfigurationManager.AppSettings("Super_Server_UserId")
        If Super_Server_UserId = "" Then
            Super_Server_UserId = "*****"
        End If

        Dim Super_Server_Password As String = ConfigurationManager.AppSettings("Super_Server_Password")
        If Super_Server_Password = "" Then
            Super_Server_Password = ""
        End If

        Super_Server_PivaSuperUser = ConfigurationManager.AppSettings("Super_Server_PivaSuperUser")
        If Super_Server_PivaSuperUser = "" Then
            Super_Server_PivaSuperUser = ""
        End If

        Dim StringaConnessione_Super_Server As String
        StringaConnessione_Super_Server = "Provider=" & Super_Server_Provider & ";Server=" & Super_Server_Server & ";Initial Catalog=" & Super_Server_DB & ";User Id=" & Super_Server_UserId & ";Password=" & Super_Server_Password & ";"

        Return StringaConnessione_Super_Server

    End Function



    'crea un objparametri semplificato con valori de default tranne
    'stringa connessione (necessaria)
    'Default_CodFiscale_Usernale_Superuser_Utente impostata come nome e piva utente e superuser
    'Gli altri valori vengono letti dall'agrowebconfig se presente,
    'altrimenti dalla sessione
    'infine dal webconfig
    'Se non trovato viene impostato un valore di default
    '#################################################################################################
    Public Function Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(ByVal StringaConnessione As String,
                                                                              ByVal Default_CodFiscale_Usernale_Superuser_Utente As String,
                                                                              ByRef AgroWebConfig As AgronicaCoreGestioneRichieste.AgroWebConfig,
                                                                              ByRef objSession As System.Web.SessionState.HttpSessionState
                                                                              ) As AgronicaCoreParametri




        '---------------------------------------------------------------------------------
        '---------- CREO GLI ALTRI PARAMETRI PER CREARE L'OBJPARAMETRI ------------------
        '---------------------------------------------------------------------------------


        Dim NomeParametro As String
        Dim Valore_Default As String


        '--FinestraTemporale_Inizio
        Dim FinestraTemporale_Inizio As String
        NomeParametro = "ASG_FinestraTemporale_Inizio"
        Valore_Default = CDate(AGRODATAINIZIO)
        FinestraTemporale_Inizio = CDate(LeggiParametroSessione(objSession, NomeParametro, Valore_Default))


        '--FinestraTemporale_Fine
        Dim FinestraTemporale_Fine As String
        NomeParametro = "ASG_FinestraTemporale_Fine"
        Valore_Default = CDate(AGRODATAFINE)
        FinestraTemporale_Fine = CDate(LeggiParametroSessione(objSession, NomeParametro, Valore_Default))


        '--AgronicaCore_Flag_CancellazioneLogica
        Dim FlagCancellazioneLogica As enumCancellazioneLogica
        NomeParametro = "AgronicaCore_Flag_CancellazioneLogica"
        Valore_Default = CStr(enumCancellazioneLogica.CancellazioneFisica)
        If Not IsNothing(AgroWebConfig) Then
            FlagCancellazioneLogica = AgroWebConfig.AgronicaCore_Flag_CancellazioneLogica
            If Not IsNothing(objSession) AndAlso CStr(FlagCancellazioneLogica) <> "" Then
                objSession(NomeParametro) = FlagCancellazioneLogica
            End If
        End If
        FlagCancellazioneLogica = CInt(LeggiParametroSessione(objSession, NomeParametro, Valore_Default))


        '--AgronicaCore_Flag_Visibilita
        Dim FlagVisibilita As enumVisibilita
        NomeParametro = "AgronicaCore_Flag_Visibilita"
        Valore_Default = CStr(enumVisibilita.Visibilita_SoloNonCancellati)
        If Not IsNothing(AgroWebConfig) Then
            FlagVisibilita = AgroWebConfig.AgronicaCore_Flag_Visibilita
            If Not IsNothing(objSession) AndAlso CStr(FlagVisibilita) <> "" Then
                objSession(NomeParametro) = FlagVisibilita
            End If
        End If
        FlagVisibilita = CInt(LeggiParametroSessione(objSession, NomeParametro, Valore_Default))


        '--AgronicaCore_DirectoryLOG
        Dim LogDirectory As String
        NomeParametro = "AgronicaCore_DirectoryLOG"
        Valore_Default = "C:\GIASLAN\LOG"
        If Not IsNothing(AgroWebConfig) Then
            LogDirectory = AgroWebConfig.AgronicaCore_DirectoryLOG
            If Not IsNothing(objSession) AndAlso CStr(LogDirectory) <> "" Then
                objSession(NomeParametro) = LogDirectory
            End If
        End If
        LogDirectory = LeggiParametroSessione(objSession, NomeParametro, Valore_Default)


        '--AgronicaCore_FileNameLOG
        Dim LogFileName As String
        NomeParametro = "AgronicaCore_FileNameLOG"
        Valore_Default = "GiasOnlineLog.txt"
        If Not IsNothing(AgroWebConfig) Then
            LogFileName = AgroWebConfig.AgronicaCore_FileNameLOG
            If Not IsNothing(objSession) AndAlso CStr(LogFileName) <> "" Then
                objSession(NomeParametro) = LogFileName
            End If
        End If
        LogFileName = LeggiParametroSessione(objSession, NomeParametro, Valore_Default)


        '--SuperUser_Username
        Dim SuperUser_Username As String
        NomeParametro = "ASG_SuperUser_Username"
        Valore_Default = Default_CodFiscale_Usernale_Superuser_Utente
        SuperUser_Username = LeggiParametroSessione(objSession, NomeParametro, Valore_Default)


        '--SuperUser_Username
        Dim SuperUser_CodFiscale As String
        NomeParametro = "ASG_SuperUser_CodFiscale"
        Valore_Default = Default_CodFiscale_Usernale_Superuser_Utente
        SuperUser_CodFiscale = LeggiParametroSessione(objSession, NomeParametro, Valore_Default)


        '--SuperUser_Username
        Dim Utente_Username As String
        NomeParametro = "ASG_Utente_Username"
        Valore_Default = Default_CodFiscale_Usernale_Superuser_Utente
        Utente_Username = LeggiParametroSessione(objSession, NomeParametro, Valore_Default)


        '--SuperUser_Username
        Dim Utente_CodFiscale As String
        NomeParametro = "ASG_Utente_CodFiscale"
        Valore_Default = Default_CodFiscale_Usernale_Superuser_Utente
        Utente_CodFiscale = LeggiParametroSessione(objSession, NomeParametro, Valore_Default)


        '--SuperUser_Username
        Dim Utente_Lingua_Cod As Integer = 1
        NomeParametro = "LinguaCorrente"
        Valore_Default = Default_CodFiscale_Usernale_Superuser_Utente
        If (Not IsNothing(HttpContext.Current.Session)) AndAlso (Not IsNothing(HttpContext.Current.Session("LinguaCorrente"))) Then
            Dim lingua_corrente As AgronicaCoreDataProvider.Lingua = HttpContext.Current.Session("LinguaCorrente")
            Utente_Lingua_Cod = lingua_corrente.Lingua_cod
        End If

        '---------------------------------------------------------------------------------
        '---------- CREO objParametri_Super_Server e salvo in sessione -------------------
        '---------------------------------------------------------------------------------
        Dim objParametriHLP As New AgronicaCoreDataProvider.AgronicaCoreParametri_Helper

        Dim objParametri_Super_Server As AgronicaCoreParametri
        objParametri_Super_Server = objParametriHLP.Crea_ObjParametri(FinestraTemporale_Inizio,
                                                                     FinestraTemporale_Fine,
                                                                     FlagCancellazioneLogica,
                                                                     FlagVisibilita,
                                                                     LogDirectory,
                                                                     LogFileName,
                                                                     SuperUser_Username,
                                                                     SuperUser_CodFiscale,
                                                                     Utente_Username,
                                                                     Utente_CodFiscale,
                                                                     StringaConnessione)


        objParametri_Super_Server.Lingua_Cod = Utente_Lingua_Cod
        Return objParametri_Super_Server

    End Function


    'Legge il parametro da sessione,
    'se non lo trova lo legge con stringa ASG iniziale
    'se non lo trova lo legge dal webconfig
    'se non lo trova da il valore di default
    'salva il valore trovato in sessione
    'nei due formati, con e senza ASG
    'USATO SOLO IN Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig
    'Public Function LeggiImpostaParametriSessione_SENZA_ASG_(ByRef objSession As System.Web.SessionState.HttpSessionState, _
    '                                              ByVal NomeParametro As String, _
    '                                              ByVal Valore_Default As String) As String

    '    Dim Valore As String
    '    Dim NomeParametroSession = "ASG_" & NomeParametro


    '    If Not IsNothing(objSession(NomeParametro)) AndAlso _
    '            CStr(objSession(NomeParametro)) <> "" Then

    '        'se c'è in sessione assegno quello
    '        Valore = CStr(objSession(NomeParametro))

    '        'ElseIf Not leggidaconfigurazionesiti(NomeParametro)) AndAlso _
    '        '                leggidaconfigurazionesiti(NomeParametro) <> "" Then

    '        '    'se è nella tabella configurazione siti assegno quello
    '        '    Valore = leggidaconfigurazionesiti(NomeParametro)


    '    ElseIf Not IsNothing(objSession(NomeParametroSession)) AndAlso _
    '            CStr(objSession(NomeParametroSession)) <> "" Then

    '        'se c'è in sessione assegno quello
    '        Valore = CStr(objSession(NomeParametroSession))

    '        'ElseIf Not leggidaconfigurazionesiti(NomeParametro)) AndAlso _
    '        '                leggidaconfigurazionesiti(NomeParametro) <> "" Then

    '        '    'se è nella tabella configurazione siti assegno quello
    '        '    Valore = leggidaconfigurazionesiti(NomeParametro)


    '    ElseIf Not IsNothing(ConfigurationManager.AppSettings(NomeParametro)) AndAlso _
    '                    CStr(ConfigurationManager.AppSettings(NomeParametro)) <> "" Then

    '        'se è nel config assegno quello
    '        Valore = CStr(ConfigurationManager.AppSettings(NomeParametro))

    '    Else
    '        'valore di default
    '        Valore = Valore_Default
    '    End If

    '    objSession(NomeParametroSession) = Valore
    '    objSession(NomeParametro) = Valore
    '    'configurazionesiticlasse.parametro=valore
    '    Return Valore

    'End Function

    Private Function LeggiParametroSessione(ByRef objSession As System.Web.SessionState.HttpSessionState,
                                                 ByVal NomeParametro As String,
                                                 ByVal Valore_Default As String) As String

        Dim Valore As String


        If (Not IsNothing(objSession)) AndAlso (Not IsNothing(objSession(NomeParametro))) AndAlso
                CStr(objSession(NomeParametro)) <> "" Then

            'se c'è in sessione assegno quello
            Valore = CStr(objSession(NomeParametro))

            'ElseIf Not leggidaconfigurazionesiti(NomeParametro)) AndAlso _
            '                leggidaconfigurazionesiti(NomeParametro) <> "" Then

            '    'se è nella tabella configurazione siti assegno quello
            '    Valore = leggidaconfigurazionesiti(NomeParametro)

        Else
            'valore di default
            Valore = Valore_Default
        End If

        Return Valore

    End Function



    'Crea Objparametri server e utenti leggendo i parametri dal dalla sessione dand
    '#################################################################################################
    Public Sub Crea_ObjParametri_Server_E_ObjParametri_Utenti_E_Salva_In_Sessione(
                                               ByRef objSession As System.Web.SessionState.HttpSessionState,
                                               ByVal StringaConnessione_Server As String,
                                               ByVal StringaConnessione_Utenti As String)


        objSession("ASG_objParametri_Server") = Nothing
        objSession("ASG_objParametri_Utenti") = Nothing


        Dim ASG_objParametri_Server As AgronicaCoreParametri = Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(StringaConnessione_Server, "", Nothing, objSession)
        Dim ASG_objParametri_Utente As AgronicaCoreParametri = Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(StringaConnessione_Utenti, "", Nothing, objSession)


        objSession("ASG_objParametri_Server") = ASG_objParametri_Server
        objSession("ASG_objParametri_Utenti") = ASG_objParametri_Utente

        ConfigurazioneEstesaSqlProviderFactory.Instance(ASG_objParametri_Server)
    End Sub


    '####################################################################
    Public Sub AvviamentoConSuperServerFast(ByVal IN_Super_Server_Provider As String,
                                        ByVal IN_Super_Server_Server As String,
                                        ByVal IN_Super_Server_DB As String,
                                        ByVal IN_Super_Server_UserId As String,
                                        ByVal IN_Super_Server_Password As String,
                                        ByVal PathDirFileLog As String,
                                        ByVal NomeFileLog As String,
                                        ByVal PivaSuperUser As String,
                                        ByVal Utente_Username As String,
                                        ByVal ID_DB As Integer,
                                        ByRef objParametriSuperServer As AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreGestioneRichieste.Inizializzatore.AvviamentoConSuperServer()"
        Dim Flag_0LeggiConfig_1PassaParametri As Integer = 1
        Dim IN_Super_Server As Boolean = True
        Dim FiltroAggiuntivo_QueryIdDbGiasServer As String
        If ID_DB <> 0 Then
            FiltroAggiuntivo_QueryIdDbGiasServer = " ID_DB = " & ID_DB & " "
        Else
            FiltroAggiuntivo_QueryIdDbGiasServer = ""
        End If
        Dim OrderBy_QueryIdDbGiasServer As String = " Progressivo DESC "

        'mancano 
        'SuperUser_Username
        'Utente_CodFiscale
        'cerco dopo e li inserisco
        AvviamentoConSuperServer(Flag_0LeggiConfig_1PassaParametri,
                                 IN_Super_Server,
                                 IN_Super_Server_Provider,
                                 IN_Super_Server_Server,
                                 IN_Super_Server_DB,
                                 IN_Super_Server_UserId,
                                 IN_Super_Server_Password,
                                 PathDirFileLog,
                                 NomeFileLog,
                                 PivaSuperUser,
                                 "",
                                 PivaSuperUser,
                                 Utente_Username,
                                 "",
                                 FiltroAggiuntivo_QueryIdDbGiasServer,
                                 OrderBy_QueryIdDbGiasServer,
                                 objParametriSuperServer,
                                 objParametri_Server,
                                 objParametri_Utenti)


        'devo ricavarli
        Dim Utenti As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R()

        'vanni, capire il senso di passare il super user per poi recuperarlo di nuovo... aggiungo la riga di codice sotto, così sovrascrivo lo username passato.
        Utente_Username = Utenti.Username_From_CodFisc(PivaSuperUser, objParametri_Utenti)

        Dim SuperUser_Username As String = Utenti.Username_From_CodFisc(PivaSuperUser, objParametri_Utenti)
        Dim Utente_CodFiscale As String = Utenti.CodFisc_From_Username(Utente_Username, objParametri_Utenti)
        If Utente_CodFiscale = "" Then
            Throw New Exception("L'utente " & Utente_Username & " non esiste, non è stato possibile trovare il codice fiscale")
        End If

        'imposto i valori negli objparametri
        objParametri_Server.SuperUserUsername = SuperUser_Username
        objParametri_Utenti.SuperUserUsername = SuperUser_Username
        objParametri_Utenti.UtenteCodFiscale = Utente_CodFiscale
        objParametri_Server.UtenteCodFiscale = Utente_CodFiscale
        objParametri_Server.UsernameOperazione = Utente_CodFiscale
        objParametri_Utenti.UsernameOperazione = Utente_CodFiscale
        objParametri_Server.UtenteUsername = SuperUser_Username
        objParametri_Utenti.UtenteUsername = SuperUser_Username

    End Sub


    '####################################################################
    Public Sub AvviamentoConSuperServer(ByVal Flag_0LeggiConfig_1PassaParametri As Integer,
                                        ByVal IN_Super_Server As Boolean,
                                        ByVal IN_Super_Server_Provider As String,
                                        ByVal IN_Super_Server_Server As String,
                                        ByVal IN_Super_Server_DB As String,
                                        ByVal IN_Super_Server_UserId As String,
                                        ByVal IN_Super_Server_Password As String,
                                        ByVal PathDirFileLog As String,
                                        ByVal NomeFileLog As String,
                                        ByVal PivaSuperUser_xLetturaSuperServer As String,
                                        ByVal SuperUser_Username As String,
                                        ByVal SuperUser_CodFiscale As String,
                                        ByVal Import_Username As String,
                                        ByVal Import_CodFiscale As String,
                                        ByVal FiltroAggiuntivo_QueryIdDbGiasServer As String,
                                        ByVal OrderBy_QueryIdDbGiasServer As String,
                                        ByRef objParametriSuperServer As AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreGestioneRichieste.Inizializzatore.AvviamentoConSuperServer()"
        Dim MessaggioErrore As String
        Dim Super_Server As Boolean
        Dim Super_Server_Provider As String = ""
        Dim Super_Server_Server As String = ""
        Dim Super_Server_DB As String = ""
        Dim Super_Server_UserId As String = ""
        Dim Super_Server_Password As String = ""

        Try

            If Flag_0LeggiConfig_1PassaParametri = 0 Then

                RecuperaConfig_Super_Server(Super_Server,
                                            Super_Server_Provider,
                                            Super_Server_Server,
                                            Super_Server_DB,
                                            Super_Server_UserId,
                                            Super_Server_Password
                                            )

            ElseIf Flag_0LeggiConfig_1PassaParametri = 1 Then
                Super_Server = IN_Super_Server
                Super_Server_Provider = IN_Super_Server_Provider
                Super_Server_Server = IN_Super_Server_Server
                Super_Server_DB = IN_Super_Server_DB
                Super_Server_UserId = IN_Super_Server_UserId
                Super_Server_Password = IN_Super_Server_Password
            End If

            If Not Super_Server Then
                Throw New Exception("Flag Super_Server disattivato.")
            End If

            Dim StringaConnessione_Super_Server As String = ""
            If Sicurezza.ExistStringaConnessione(Sicurezza.ID_DB_Super_Server) Then
                StringaConnessione_Super_Server = Sicurezza.ID_DB_Super_Server
            Else
                StringaConnessione_Super_Server =
                    Crea_Stringa_Connessione_Super_Server(Super_Server_Provider,
                                                        Super_Server_Server,
                                                        Super_Server_DB,
                                                        Super_Server_UserId,
                                                        Super_Server_Password
                                                        )
            End If


            '####################     OBJPARAMETRI SUPER SERVER ###########################################
            objParametriSuperServer = New AgronicaCoreParametri(AGRODATAINIZIO,
                                                                AGRODATAFINE,
                                                                enumCancellazioneLogica.CancellazioneFisica,
                                                                enumVisibilita.Visibilita_SoloNonCancellati,
                                                                PathDirFileLog,
                                                                NomeFileLog,
                                                                "",
                                                                "",
                                                                "",
                                                                "",
                                                                StringaConnessione_Super_Server)

            Dim obj As New AgronicaCoreDataProvider.Connessioni
            Dim ID_DB_Gias_Server As Integer = 0
            'ByVal TipoDB As TipiEnumerativi.enum_Tipo_DB, _
            '                               ByVal Server As String, _
            '                               ByVal DB As String, _
            '                               ByVal Provider As String, _
            '                               ByVal UserId As String, _
            '                               ByVal Password As String, _
            '                                ByVal Note As String, _
            '                                 ByVal Progressivo As Integer, _
            '                                 ByVal Descrizione As String, _

            ID_DB_Gias_Server = obj.Recupera_IdDb(enum_Tipo_DB.GIAS_SERVER,
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  PivaSuperUser_xLetturaSuperServer,
                                                  "",
                                                  0,
                                                  "",
                                                  FiltroAggiuntivo_QueryIdDbGiasServer,
                                                  OrderBy_QueryIdDbGiasServer,
                                                  objParametriSuperServer)

            Dim Descrizione_Server As String = ""
            Dim Note_Server As String = ""
            Dim Descrizione_Utenti As String = ""
            Dim Note_Utenti As String = ""
            Dim StringaConnessione_Server As String = ""
            Dim StringaConnessione_Utenti As String = ""
            Dim ID_DB_Utenti As Integer = 0
            Dim NomeServer As String = ""
            Dim progressivo_Gias_Server As Integer
            Dim progressivo_Gias_Utenti As Integer
            'Input ID_DB_Server e objParametri_Super_Server per leggere
            'output tutti gli altri
            Ricavo_Parametri_Server_Utenti_Da_Id_Db_Server(objParametriSuperServer,
                                                           ID_DB_Gias_Server,
                                                           Descrizione_Server,
                                                           Note_Server,
                                                           Descrizione_Utenti,
                                                           Note_Utenti,
                                                           StringaConnessione_Server,
                                                           StringaConnessione_Utenti,
                                                           ID_DB_Utenti,
                                                           SuperUser_CodFiscale,
                                                           NomeServer,
                                                           progressivo_Gias_Server,
                                                           progressivo_Gias_Utenti)



            '####################     OBJPARAMETRI SERVER ###########################################
            objParametri_Server = New AgronicaCoreParametri(AGRODATAINIZIO,
                                                            AGRODATAFINE,
                                                            enumCancellazioneLogica.CancellazioneFisica,
                                                            enumVisibilita.Visibilita_SoloNonCancellati,
                                                            PathDirFileLog,
                                                            NomeFileLog,
                                                            SuperUser_Username,
                                                            SuperUser_CodFiscale,
                                                            Import_Username,
                                                            Import_CodFiscale,
                                                            StringaConnessione_Server)

            '####################     OBJPARAMETRI UTENTI ###########################################
            objParametri_Utenti = New AgronicaCoreParametri(AGRODATAINIZIO,
                                                            AGRODATAFINE,
                                                            enumCancellazioneLogica.CancellazioneFisica,
                                                            enumVisibilita.Visibilita_SoloNonCancellati,
                                                            PathDirFileLog,
                                                            NomeFileLog,
                                                            SuperUser_Username,
                                                            SuperUser_CodFiscale,
                                                            Import_Username,
                                                            Import_CodFiscale,
                                                            StringaConnessione_Utenti)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub

    '#################################################################################################
    Private Sub RecuperaConfig_Super_Server(ByRef Super_Server As Boolean,
                                            ByRef Super_Server_Provider As String,
                                            ByRef Super_Server_Server As String,
                                            ByRef Super_Server_DB As String,
                                            ByRef Super_Server_UserId As String,
                                            ByRef Super_Server_Password As String)

        Dim ErroreFlag As Boolean = False
        Dim ErroreMessaggio As String = ""

        '---------------------------------------------------------------------------------
        '----- Recupera chiavi web.config del SUPER_SERVER ------------------

        If Not IsNothing(ConfigurationManager.AppSettings("Super_Server")) AndAlso
               ConfigurationManager.AppSettings("Super_Server") <> "" Then

            Super_Server = ConfigurationManager.AppSettings("Super_Server").ToString

        Else
            ErroreFlag = True
            ErroreMessaggio = "Super_Server non valorizzato!" & vbCrLf
        End If

        '--------------------

        If Not IsNothing(ConfigurationManager.AppSettings("Super_Server_Provider")) AndAlso
                ConfigurationManager.AppSettings("Super_Server_Provider") <> "" Then

            Super_Server_Provider = ConfigurationManager.AppSettings("Super_Server_Provider").ToString

        Else
            ErroreFlag = True
            ErroreMessaggio = "Super_Server_Provider non valorizzato!" & vbCrLf
        End If

        '--------------------

        If Not IsNothing(ConfigurationManager.AppSettings("Super_Server_Server")) AndAlso
                ConfigurationManager.AppSettings("Super_Server_Server") <> "" Then

            Super_Server_Server = ConfigurationManager.AppSettings("Super_Server_Server").ToString

        Else
            ErroreFlag = True
            ErroreMessaggio = "Super_Server_Server non valorizzato!" & vbCrLf
        End If

        '--------------------

        If Not IsNothing(ConfigurationManager.AppSettings("Super_Server_DB")) AndAlso
                ConfigurationManager.AppSettings("Super_Server_DB") <> "" Then

            Super_Server_DB = ConfigurationManager.AppSettings("Super_Server_DB").ToString

        Else
            ErroreFlag = True
            ErroreMessaggio = "Super_Server_DB non valorizzato!" & vbCrLf
        End If


        '--------------------

        If Not IsNothing(ConfigurationManager.AppSettings("Super_Server_UserId")) AndAlso
                ConfigurationManager.AppSettings("Super_Server_UserId") <> "" Then

            Super_Server_UserId = ConfigurationManager.AppSettings("Super_Server_UserId").ToString

        Else
            ErroreFlag = True
            ErroreMessaggio = "Super_Server_UserId non valorizzato!" & vbCrLf
        End If
        '--------------------

        If Not IsNothing(ConfigurationManager.AppSettings("Super_Server_Password")) AndAlso
                ConfigurationManager.AppSettings("Super_Server_Password") <> "" Then

            Super_Server_Password = ConfigurationManager.AppSettings("Super_Server_Password").ToString

        Else
            ErroreFlag = True
            ErroreMessaggio = "Super_Server_Password non valorizzato!" & vbCrLf
        End If

        If ErroreFlag Then
            Throw New Exception(ErroreMessaggio)
        End If

    End Sub

    '#################################################################################################
    Private Function Crea_Stringa_Connessione_Super_Server(ByVal Super_Server_Provider As String, _
                                                               ByVal Super_Server_Server As String, _
                                                               ByVal Super_Server_DB As String, _
                                                               ByVal Super_Server_UserId As String, _
                                                               ByVal Super_Server_Password As String _
                                                              ) As String

        '---------------------------------------------------------------------------------
        '---------- CREO LA STRINGACONNESSIONE al database SUPER_SERVER ------------------
        '---------------------------------------------------------------------------------

        Dim StringaConnessione_Super_Server As String = ""

        StringaConnessione_Super_Server = "Provider=" & Super_Server_Provider & ";Server=" & Super_Server_Server & ";Initial Catalog=" & Super_Server_DB & ";User Id=" & Super_Server_UserId & ";Password=" & Super_Server_Password & ";"

        Return StringaConnessione_Super_Server

    End Function


End Class
