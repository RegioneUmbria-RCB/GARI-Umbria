Imports System.IO
Imports AgronicaCoreUtilityVersioni
Imports AgronicaCoreUtilityVersioni.Enumerativi

Namespace PianoConcimazioneVersione

    Partial Class Versione
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

        Private _objChangeSito As Changelog_Agronica_Obj


#Region "Costruttori"

        Public Sub New()
            _objChangeSito = Nothing
        End Sub

        Public Sub New(ByVal projectName As String)
            _objChangeSito = New Changelog_Agronica_Obj(projectName)
        End Sub

#End Region

#Region "Crea Changelog"

        Public Function Create_Changelog(ByVal dirOutput As String) As String
            Dim pathFileJson As String = ""

            Try

                If _objChangeSito Is Nothing Then
                    Throw New NullReferenceException("objChangeSito non è stato inizializzato")
                End If
                If String.IsNullOrEmpty(dirOutput) Then
                    Throw New ArgumentNullException(NameOf(dirOutput))
                End If

                'Chiamo il Versione.aspx
                Carica_Pannello()

                pathFileJson = Path.Combine(dirOutput, _objChangeSito.GetFileNameJson())
                Dim res As Boolean = _objChangeSito.ScriviJson(pathFileJson)

                If Not res Then
                    pathFileJson = ""
                End If

            Catch ex As Exception
                pathFileJson = ""
                Throw New Exception(ex.Message)
            End Try

            Return pathFileJson
        End Function

#End Region



        '#################################################################################################
        Private Sub Carica_Pannello()

            '==================================

            Riga_Versione("26 Giugno 2026", "151.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Piva reale",
                           "Fix per visualizzare in griglia la piva reale",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=220490, idTicketTesting:=0,
                           autore:="Cecalupo Marco", noteTest:="", noteTecniche:="")

            '==================================

            Riga_Versione("08 Giugno 2026", "151.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Tutte",
                           "Estrazione e visualizzazione della Partita Iva Reale",
                           cliente:="Tutti",
                           idTicketAssistenza:=0, idTicketSviluppo:=214395, idTicketTesting:=0,
                           autore:=DEV_Casa, noteTest:="", noteTecniche:="")

            '==================================

            Riga_Versione("22 Maggio 2026", "150.4.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Redirezione a Custom500 errata",
               "Corretta redirezione alla pagina custom 500 in caso di errore al passaggio tra siti",
               cliente:="TUTTI",
               idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
               noteTest:="Non testabile in autonomia",
               noteTecniche:="",
               autore:=DEV_Casa)

            '==================================

            Riga_Versione("27 Aprile 2026", "150.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Security,
                           "SQL_Dataprovider LanciaEccezioneSuInjection",
                           "Impostata di default la proprietà a true per tutti",
                           cliente:="TUTTI",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           noteTest:="Non testabile",
                           noteTecniche:="",
                           autore:="Giulia Bottan")

            Riga_Changelog(enum_Tipo_Changelog.Security,
                           area:="Librerie terze parti",
                           descrizione:="Aggiornamento di libreria HtmlSanitizer da 8.1.870 a 9.0.892 [fix CVE-2026-25543]",
                           cliente:="TUTTI",
                           idTicketAssistenza:=0, idTicketSviluppo:=163182, idTicketTesting:=0,
                           noteTecniche:="",
                           noteTest:="Non testabile da assistenza",
                           autore:="Giulia Bottan")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                "Tutte",
                "Estrazione e visualizzazione della Partita Iva Reale",
                cliente:="Coldiretti",
                idTicketAssistenza:=0, idTicketSviluppo:=214560, idTicketTesting:=0,
                autore:="Cecalupo Marco",
                noteTest:="",
                noteTecniche:="")

            '==================================

            Riga_Versione("20 Marzo 2026", "148.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                area:="Loghi dinamici stampe piano concimazione",
                descrizione:="Corretto logo mancante nelle stampe del piano concimazione per ambienti senza impostazione di personalizzazione",
                cliente:="Coldiretti",
                idTicketAssistenza:=208246,
                idTicketSviluppo:=0,
                idTicketTesting:=0,
                noteTest:="",
                autore:=DEV_Casa)

            '==================================

            Riga_Versione("10 Marzo 2026", "148.1.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                area:="Loghi dinamici stampe",
                descrizione:="Corretto errore al lancio su report PUA dovuto al logo dinamico",
                cliente:="Coldiretti",
                idTicketAssistenza:=0,
                idTicketSviluppo:=0,
                idTicketTesting:=0,
                noteTest:="",
                autore:=DEV_Casa)

            '==================================

            Riga_Versione("02 Marzo 2026", "148.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(
                enum_Tipo_Changelog.Feature,
                area:="Loghi dinamici stampe",
                descrizione:="Sono stati aggiunti i loghi dinamici per tutte le stampe del Piano di Concimazione",
                cliente:="Coldiretti",
                idTicketAssistenza:=0,
                idTicketSviluppo:=201847,
                idTicketTesting:=0,
                noteTest:="I report interessati sono: 'Piano di Concimazione' (scheda e bilancio), 'PUA', 'Piano Nutrizionale' (scheda e bilancio). 
                Questi ultimi sembrano essere stampabili solo su COPROB nell'azienda CO.PRO.B con filtro dal 01/01/2020 al 31/12/2026.",
                autore:=DEV_Casa)

            '==================================

            Riga_Versione("06 Febbraio 2026", "147.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                area:="Verifica presenza Personalizzazione Grafiche Cliente",
                descrizione:="Ora viene verificata la presenza di ogni proprietà che viene utilizzata",
                cliente:="Coldiretti",
                idTicketAssistenza:=205504,
                idTicketSviluppo:=0,
                idTicketTesting:=0,
                noteTest:="",
                autore:=DEV_Casa
            )

            '==================================

            Riga_Versione("07 Gennaio 2026", "146.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
               "Progetti",
               "Aggiunti i progetti AgronicaCoreEsitoVerificaConformitaBIZ e AgronicaCoreEsitoVerificaConformitaDAL alla soluzione, sono necessari come riferimenti per il progetto AgronicaCoreMapper",
               cliente:="",
               idTicketAssistenza:=0,
               idTicketSviluppo:=184774,
               idTicketTesting:=0,
               noteTest:="Non testabile",
               autore:="Paolo Netso")

            '==================================

            Riga_Versione("17 Dicembre 2025", "145.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "PUA - Modifica Piano di Distribuzione Associato",
               "Corretto salvataggio in modifica della riga del Piano di Distribuzione Associato",
               cliente:="Coldiretti",
               idTicketAssistenza:=201527,
               idTicketSviluppo:=0,
               idTicketTesting:=0,
               noteTest:="",
               autore:="Lorenzo Casanova")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "PUA - Piano Distribuzione - Apertura finestra Analisi Terreno",
               "Aggiornato l'indirizzamento verso la finestra delle Analisi del Terreno dalla pagina PUA Piano Distribuzione, ora la visibilità UMA non viene considerata",
               cliente:="Coldiretti",
               idTicketAssistenza:=201698,
               idTicketSviluppo:=0,
               idTicketTesting:=0,
               noteTest:="",
               autore:="Dario Cabras")

            '==================================

            Riga_Versione("12 Dicembre 2025", "145.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "QdCA - Apertura Ricette Organiche/Chimiche da pagina PUA",
               "Corretta valorizzazione della Superficie Trattata dell'impianto selezionato e sistemato Salvataggio Ricette Organiche/Chimiche",
               cliente:="Coldiretti",
               idTicketAssistenza:=201074,
               idTicketSviluppo:=0,
               idTicketTesting:=0,
               noteTest:="",
               autore:="Lorenzo Casanova")

            '==================================

            Riga_Versione("01 Dicembre 2025", "145.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
               "Personalizzazioni Loghi",
               "Gestito il nuovo parametro 'personalizzazioniGraficheCliente'. A questo parametro sarà associato un JSON con specificati i percorsi dei loghi da sostituire a quelli standard di Agronica. Se il valore non risulta essere un'istanza della classe PersonalizzazioniGraficheCliente, il valore verrà ignorato. Tale parametro andrà a sostituire 'personalizzazioniRegioneUmbria'",
               cliente:="",
               idTicketAssistenza:=0,
               idTicketSviluppo:=196565,
               idTicketTesting:=0,
               noteTest:="",
               autore:="Dario Cabras")

            '==================================

            Riga_Versione("07 Novembre 2025", "144.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Performance,
               "Gestione Cache Permessi e Impostazioni utente",
               "- Gestita la cache per Permessi e impostazioni utente;",
               cliente:="",
               idTicketAssistenza:=0, idTicketSviluppo:=195312, idTicketTesting:=0,
               autore:=DEV_Drudi)

            '==================================

            Riga_Versione("03 Novembre 2025", "144.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Performance,
                           "Aggiunta TryCatch e nuovi LivelliLog",
                           "- Aggiunti livelli di log: solo mail, file + mail, file + db;" &
                           "- Aggiunti blocchi TryCatch alla scrittura su file e db del log per evitare di lasciare thread zombie in giro in caso di errore",
                           cliente:="",
                           idTicketAssistenza:=196225, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Casa)

            '==================================

            Riga_Versione("17 Ottobre 2025", "143.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Security,
                       "Aggiornamento controllo filtro aggiuntivo",
                       "Ora i caratteri ';', '--' e '/* */' sono consentiti se presenti in un campo di testo (es. note, descrizione, ecc) e non più bloccanti",
                       cliente:="Coldiretti",
                       idTicketAssistenza:=188150, idTicketSviluppo:=0, idTicketTesting:=0,
                       autore:=DEV_Casa, noteTest:="Andare nel quaderno di campagna, inserire una nuova operazione, 
                       all'interno di essa selezionare un operazione che utilizza dei 
                       prodotti. Dopodiché, nella ricerca prodotti, selezionare un prodotto 
                       che ha nel nome uno dei seguenti caratteri ';', '--' o '/*'. 
                       Cliccare nella barra di ricerca contenente il nome del 
                       prodotto selezionato, cancellare l'ultimo carattere e 
                       lanciare la ricerca dei prodotti.", noteTecniche:="")

            '==================================

            Riga_Versione("19 Settembre 2025", "142.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Piano Concimazione Multiplo",
                           "Corretto caricamento particelle vulnerabili.
                           Corretto caricamento analisi per opzioni:
                           - Aggregazione Appezzamenti: Crea un Piano Concimazione per Azienda, Centro, Specie, Finalità, Vulnerabilità
                           - Aggregazione Appezzamenti: Crea un Piano Concimazione per Azienda, Centro, Specie, Finalità, Vulnerabilità, Analisi",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=190842, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Anna Funciello",
                           noteTest:="Correlati ticket #191160 e #191160
                           Le analisi vengono lette per ogni impianto in modo scalare: se trova le analisi in un livello (es catasto) non va a cercarle nei livelli successivi 
                           - 1) Catasto 
                           - 2) Impianto
                           - 3) Appezzamento
                           - 4) Campo
                           - 5) Centro
                           - 6) Impresa",
                           noteTecniche:="")

            '==================================

            Riga_Versione("08 Settembre 2025", "142.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Security,
                           "Controllo massivo SQL Injection",
                           "Verificati molti casi di possibili sql injection",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=0, idTicketSviluppo:=179500, idTicketTesting:=0,
                           autore:=DEV_Casa, noteTest:="", noteTecniche:="")

            '==================================

            Riga_Versione("25 Luglio 2025", "140.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Security,
                            "SQL_Dataprovider LanciaEccezioneSuInjection",
                            "Aggiunta lettura da DB del parametro LanciaEccezioneSuInjection",
                            cliente:="Coldiretti",
                            idTicketAssistenza:=184216, idTicketSviluppo:=0, idTicketTesting:=0,
                            autore:=DEV_Casa)

            '==================================

            Riga_Versione("09 Giugno 2025", "139.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(
                           enum_Tipo_Changelog.Performance,
                           "Lettura versione sql",
                           "Per motivi di performance, è stato cambiato il metodo che legge la versione di sql: adesso si legge il numero della build e non l'anno corrispondente",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=163175, idTicketTesting:=0,
                           autore:="Paolo Netso",
                           noteTest:="",
                           noteTecniche:=""
                           )

            Riga_Changelog(
                           enum_Tipo_Changelog.Performance,
                           "Sequenze - lettura versione sql",
                           "Per motivi di performance, è stato tolto il controllo sulla versione di sql quando si utilizzano le sequenze, commanda il valore del flag nel file appsettings",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=163175, idTicketTesting:=0,
                           autore:="Paolo Netso",
                           noteTest:="Non testabile",
                           noteTecniche:=""
                           )

            '==================================

            Riga_Versione("12 Maggio 2025", "138.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                "Invio Log Elastic Search - LOG APPLICATIVI (DataProvider)",
                "Rimosso possibile loop infinito in caso di errore di invio a ElasticSearch",
                cliente:="",
                idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Funcy
                )

            '==================================

            Riga_Versione("14 Aprile 2025", "137.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Header Gias",
                           "Aggiunto id ad anchor tag per cambio impresa",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=163356, idTicketTesting:=0,
                           autore:="Zammataro Salvatore",
                           noteTest:="Non testabile")

            '==================================

            Riga_Versione("19 Marzo 2025", "136.0.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Piano di Concimazione Singola Azienda",
                           "Fix query caricamento griglia appezzamenti.Il problema nasceva da un cast fatto male sulle particelle.",
                           cliente:="Zani",
                           idTicketAssistenza:=168133, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Funcy,
                           noteTecniche:="",
                           noteTest:="Prerequisiti: un appezzamento associato a particella con SEZIONE e SUBALTERNO valorizzati
                           Creare un piano di concimazione singola azienda e associarlo all'appezzamento")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Piano di Concimazione Singola Azienda",
                           "Fix query caricamento griglia appezzamenti.Il problema nasceva da un cast fatto male sulle particelle.",
                           cliente:="Asipo",
                           idTicketAssistenza:=168151, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Funcy,
                           noteTecniche:="",
                           noteTest:="Prerequisiti: un appezzamento associato a particella con SEZIONE e SUBALTERNO valorizzati
                           Creare un piano di concimazione singola azienda e associarlo all'appezzamento")

            '==================================

            Riga_Versione("17 Marzo 2025", "136.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Security, "Logout",
               "Migliorata gestione per la sicurezza del redirect di logout e di sessione scaduta",
               "Credit Agricole", 0, 162774, 0, "Gianluca Amoroso", "",
               "Il redirect deve continuare a funzionare come prima, riportando alla pagina di login. Stessa modifica dettagliata in sito dell'Agenda")

            '==================================

            Riga_Versione("04 Marzo 2025", "135.2.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Piano di Concimazione x Aggancio Entità Analisi",
                           "Quando si sceglie un'analisi, tutte le entità a cui è associata vengono automaticamente selezionate all'interno del piano di concimazione (se sono coerenti e presenti)
                           Il problema era nelle particelle",
                           "",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=163860,
                           DEV_Funcy,
                           noteTest:="-creare una particella con subalterno e sezione NON indicati (svuotare le caselle)
                           - associare la particella ad un appezzamento
                           - creare un'analisi associata alla particella dal nuovo modulo
                           - creare un piano di concimazione, selezionare specie dell'appezzamento e la relativa analisi associata alla particella
                           - l'appezzamento deve venire automaticamente selezionato nella griglia del piano di concimazione")

            '==================================

            Riga_Data("31 Gennaio 2025")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Bug("Security - AgronicaCoreParametri",
                     "Fix per recuperare nome db anche in caso di stringa connessione codificata")

            '==================================

            Riga_Data("20 Gennaio 2025")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("Autenticazione",
                      "Prima di ogni chiamata ai web method è stato introdotto un controllo di sicurezza (global asax)",
                            noteTest:="Verificare che una pagina qualsiasi di questo sito funzioni correttamente",
                            noteTecniche:="Il controllo di sicurezza viene effettuato solo quando il valore della chiave ControlloAutenticazioneConAuthCookie nei appsettings è uguale a true")

            '==================================

            Riga_Data("20 Dicembre 2024")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Bug("Analisi del Terreno (new!) - Security",
                      "Fix SonarQube per controllo origine",
                      noteTest:="riverificare 'Creazione Completa', 'Visualizza Completa' e 'Gestione Analisi' con salvataggio da Piano Concimazione, Piano Nutrionale e PUA (solo 'Gestione Analisi', ovvero griglia)")

            '==================================

            Riga_Data("16 Dicembre 2024")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("Security",
                      "Attivata modalità nel web.config per criptare il viewstate delle pagine",
                      noteTest:="Non testabile")

            Riga_Text("Security",
                      "Interventi vari per evitare il passaggio della stringa di connessione al db (8507)")

            Riga_Text("Analisi del Terreno (new!)",
                      "Aggancio Analisi Terreno (new!) alle pagine Piano Concimazione e Piano Nutrizionale, PUA Piano Distribuzione",
                      noteTest:="Mettersi in modifica/creazione di un nuovo piano, nella sezione 'CARATTERISTICHE SUOLO' testare i pulsanti 'Visualizza Completa' (analisi del terreno selezionata), Creazione Completa ('Nessuna Analisi') o Gestione Analisi (porta alla ricerca con possibilità di creazione)
                      Verificare la corretta apertura delle analisi terreno (new!), verificare anche il corretto salvataggio di una nuova Analisi
                      NB. Sul PUA c'è un solo pulsante che porta alla griglia delle analisi",
                      noteTecniche:="L'impostazione SUPERUSER_Mod_Analisi_Terreno (892) deve essere impostata con valore 2
                      Da interfaccia: Utenti e Permessi (new!) -> Impostazioni Superuser -> Impostazioni Superuser -> 'ALTRE IMPOSTAZIONI' -> 'Modalità richiamo Analisi del Terreno' -> 'Avanzata (Angular)'")

            '==================================

            Riga_Data("12 Dicembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           Ver:="776")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Bug("Sequence",
                     "Alla creazione delle Sequence startValue = 1",
                     noteTest:="")

            '==================================

            Riga_Data("06 Dicembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           Ver:="776")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Bug("Web Config",
                     "Corretta dipendenza libreria System.Memory",
                     noteTest:="Non testabile")

            '==================================

            Riga_Data("08 Novembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           Ver:="776")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("Security - Report Issue (punto 6)",
                      "Bonifica campi hidden della master mediante html sanitizer")

            Riga_Text("Codifica e decodifica password_smtp e PasswordArteaWS",
                      "-Introdotti metodi per la codifica e decodifica usando la classe AES
                            -Introdotta chiave cr2 in web.config o app.config")

            Riga_Text("SonarQube - Issues Bug",
                      "Risolte alcune issue di media e alta gravità, rilevati dal sistema di monitoraggio del codice")

            Riga_Text("Gestione sequence default",
  "Gestione sequence per progressivi chiavi tabelle di default attive per tutti. 
    Disattivabili esclusivamente impostando a False la chiave Allow_Sql_Sequence nell'appsettings")

            Riga_Text("Security - Query",
                      "Parametrizzate massivamente molte query in filtri aggiuntivi, order by e clausole IN per impedire sql injection")

            '==================================

            Riga_Data("11 Ottobre 2024")

            Riga_Requisiti("Migra",
                           "Permesso workflow new", "769")

            Riga_Requisiti("CoreWS",
                           "Per modifiche legate alla codifica e decodifica semplice delle stringhe", "17/09/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("Security - XSS Protection",
                      "- Aggiunto controllo per bloccare script injection nelle chiamate ai web method" &
                      "- Aggiunti hiddenfield per impedire injection script nelle variabili javascript",
                      noteTecniche:="Occorre impostare il parametro 'DetectScriptInjection' in appsettings.config")

            '==================================

            Riga_Data("17 Settembre 2024")

            Riga_Requisiti("Migra",
                           "Permesso workflow new", "769")

            Riga_Requisiti("CoreWS",
                           "Per modifiche legate alla codifica e decodifica semplice delle stringhe", "17/09/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("Codifica e decodifica stringhe",
                      "Introdotta la possibilità di effettuare una codifica semplice delle stringhe",
                      noteTecniche:="Per poter abilitare la codifica semplice la proprietà UsaCodificaSemplice in appsettings deve essere impostata a true")

            '==================================

            Riga_Data("13 Settembre 2024")

            Riga_Requisiti("Migra",
                           "Permesso workflow new", "769")

            Riga_Requisiti("CoreWS",
                           "Inserita nuova impostazione 'Modalità filtro di ricerca' (SUPERUSER_Mod_Filtro_Ricerca)", "23/08/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Net CORE API",
                           "Aggancio al Filtro Ricerca (new!)", "13/09/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("DataProvider - Log Agro_Sequenze",
                      "Modifica Scrivi_Log per loggare anche NomeTabella in caso di exception",
                      noteTest:="Non testabile da assistenza")

            Riga_Text("Security - Web.Config",
                      "- Aggiunta/sistemata parte del ""customErrors"" Per fare in modo che compilando in debug localmente sia visibile il messaggio di errore dettagliato, " &
                      "mentre compilando in Release per metterlo in produzione si verrà rimandati alla pagina di errore generico.",
                      "Coldiretti",
                      noteTest:="Non testabile",
                      noteTecniche:="In fase di compilazione/pubblicazione il valore del tag &lt;customErrors> del file Web.Config viene sovrascritto dal relativo blocco nel file Web.Debug.Config o Web.Release.config")

            Riga_Text("Security - SQL Dataprovider",
                      " - Implementato utilizzo dei parametri SQL nella maggior parte dei punti in cui non erano utilizzati",
                      noteTecniche:="Usate sempre Agro_SQL_Save_Clausola_IN e Agro_SQL_Save_xFiltroAggiuntivo nella composizione di query in cui compaiono parametri inseriti da interfaccia",
                      noteTest:="Effettuare giri generici sulla pagina cercando di utilizzare eventuali filtri disponibili nel modo più specifico possibile")

            Riga_Text("Utilizzo Filtro Ricerca (new!)",
                        "Agganciato Filtro Ricerca (new!) alla pagina PianoConcimazione_MenuBS.aspx",
                        noteTest:="Per poter testare bisogna andare in 'Piano di concimazione', Nuovo 'Bilancio Multi-Aziende Multi-Coltura' e Nuovo 'Schede Multi-Aziende Multi-Coltura'. 
                        L'impostazione superuser SUPERUSER_Mod_Filtro_Ricerca (891) deve essere uguale a 2.")


            '==================================

            Riga_Data("23 Agosto 2024")

            Riga_Requisiti("Net CORE API",
                           "Per sostituzione filtrino imprese.aspx con Filtro Ricerca (new!)", "15/07/2024")

            Riga_Requisiti("Migra",
                           "Permesso workflow new", "769")

            Riga_Requisiti("CoreWS",
                           "Inserita nuova impostazione 'Modalità filtro di ricerca' (SUPERUSER_Mod_Filtro_Ricerca)", "23/08/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("SQL Data Provider nuova impostazione e mail di log",
            " - Reinserita la possibilità di ricevere una mail di log per le query in errore" &
            " - Inserita la possibilità di impostare la non esecuzione della query originale in caso di errore di parametrizzazione")

            Riga_Text("WIP Gestione impostazione 'Modalità filtro di ricerca' (SUPERUSER_Mod_Filtro_Ricerca)",
                      "WIP: Nei punti dove il filtrone_nuovo.aspx veniva chiamato, è stato introdotto l'utilizzo dell'impostazione, per permettere all'utente di scegliere se andare nel filtrone_nuovo.aspx o nel Filtro Ricerca (new!) (non sono ancora stati gestiti tutti i chiamanti)",
                      noteTecniche:="NON TESTABILE")

            '==================================

            Riga_Data("02 Agosto 2024")

            Riga_Requisiti("Net CORE API",
                           "Per sostituzione filtrino imprese.aspx con Filtro Ricerca (new!)", "15/07/2024")

            Riga_Requisiti("Migra",
                           "Permesso workflow new", "769")

            Riga_Requisiti("CoreWS",
                           "Classi e metodi necessari per fare il redirect verso il Filtrino Imprese o verso il Filtro Ricerca (new!), in base al valore dell'impostazione", "15/07/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Bug("Piano Utilizzazione Agronomica (PUA) || Piano di concimazione",
                      "Il bottone ""Vai a gestione pratiche"" ora gestisce correttamente il redirect al Workflow new",
                      noteTest:="Non visibile con grafica nuova")

            '==================================

            Riga_Data("17 Luglio 2024")

            Riga_Requisiti("Net CORE API",
                           "Per sostituzione filtrino imprese.aspx con Filtro Ricerca (new!)", "15/07/2024")

            Riga_Requisiti("Migra",
                           "Permesso workflow new", "769")


            Riga_Requisiti("CoreWS",
                           "Classi e metodi necessari per fare il redirect verso il Filtrino Imprese o verso il Filtro Ricerca (new!), in base al valore dell'impostazione", "15/07/2024")
            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("SQL Sequence",
                      "Aggiunto parametro attivazione in appsettings")

            '==================================

            Riga_Data("15 Luglio 2024")

            Riga_Requisiti("Net CORE API",
                           "Per sostituzione filtrino imprese.aspx con Filtro Ricerca (new!)", "15/07/2024")

            Riga_Requisiti("Migra",
                           "Permesso workflow new", "769")


            Riga_Requisiti("CoreWS",
                           "Classi e metodi necessari per fare il redirect verso il Filtrino Imprese o verso il Filtro Ricerca (new!), in base al valore dell'impostazione", "15/07/2024")
            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Bug("Porting workflow",
                     " - Aggiunto controllo permesso Workflow new (agenda) a PianoConcimazione_MenuBS")

            Riga_Text("Contatori",
                      "Normalizzata funzione di richiamo stack counter", "", 0, "", "")

            Riga_Text("Sostituzione chiamate al Filtrino Imprese con chiamate al Filtro Ricerca (new!)",
                      "Quando l'impresa non è selezionata, la pagina che si aprirà per la selezione dell'impresa sarà il Filtro Ricerca (new!) e non il Filtrino Imprese",
                      noteTest:="Con l'aiuto di uno sviluppatore, nella tabella Utenti_Impostazioni impostare a 3 il valore dell'impostazione SUPERUSER_Mod_Ricerca_Impresa",
                      noteTecniche:="Per le aziende che hanno bisogno di non usare il Filtrino Imprese, impostare a 3 il valore dell'impostazione SUPERUSER_Mod_Ricerca_Impresa")

            '==================================

            Riga_Data("21 Giugno 2024")

            Riga_Requisiti("Migra",
                           "Aggiunta tabella Agronica_Log_Analisi per gestione log Analisi", "752")

            Riga_Requisiti("Core WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Bug("Elastic Search",
                     "Inserita gestione escaping per log applicativi inviati ad elastic search", "Coldiretti",
                     noteTecniche:="Aggiunto remove escaping pre e post serializzazione",
                     noteTest:="no test")

            '==================================
            Riga_Data("14 Giugno 2024")

            Riga_Requisiti("Migra",
                           "Aggiunta tabella Agronica_Log_Analisi per gestione log Analisi", "752")

            Riga_Requisiti("Core WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("Log Provider",
          " Modificato il valore di default da logga solo su file a logga solo su DB", noteTest:="Non testabile")

            Riga_Text("Piano di concimazione",
          "Aggiunta Impianti Bilancio/Scheda Multi-Azienda Multi-Coltura passano dal filtrone al posto del filtrotto")

            '==================================

            Riga_Data("17 Maggio 2024")

            Riga_Requisiti("Migra",
                           "Aggiunta tabella Agronica_Log_Analisi per gestione log Analisi", "752")

            Riga_Requisiti("Core WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Bug("SQL DataProvider ordinamento orderby",
                     "Fix ordinamento dei risultati di alcune query (come il widget delle colture)")

            Riga_Text("Miglioramento lettura imprese all'apertura di alcune pagine",
                      "Velocizzata la lettura delle imprese prendendo solo i campi necessari, introdotto metodo nuovo per la lettura")

            Riga_Text("Migliorata la parametrizzazione delle query",
                      "Velocizzata la procedura di sostituzione del testo necessaria per la parametrizzazione")

            '==================================

            Riga_Data("26 Aprile 2024")

            Riga_Requisiti("Migra",
                           "Aggiunta tabella Agronica_Log_Analisi per gestione log Analisi", "752")

            Riga_Requisiti("Core WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("machinekey.config",
                      "aggiunto il richiamo sul web.config")

            Riga_Text("Parametrizzatore DataProvider",
                      "- Aggiunto nuovo parametrizzatore come default per velocizzare l'esecuzione delle query" &
                      "- Corretto caso che mandava in errore la procedura quando veniva utilizzata una funzione aggregata come criterio di order by")

            Riga_Text("Servizio IsAlive",
                      "Aggiunto ws per verificare raggiungibilità sito")

            '==================================

            Riga_Data("26 Marzo 2024")

            Riga_Requisiti("Migra",
                           "Aggiunta tabella Agronica_Log_Analisi per gestione log Analisi", "752")

            Riga_Requisiti("Core WS",
                           "per Piano Nutrizionale IBF", "29/08/2023")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Bug("Aggiunto Invio Mail di Log per Segnalazioni Speciali",
                     "Aggiunto invio mail nel caso di Piva = stringa vuota per la funzione AgronicaCoreUtility.CaricaListControl.TutteSpecieColtivate_3_Data_Da_A()",
                     "Regione Umbria",
                     noteTest:="Non testare")

            '==================================

            Riga_Data("29 Febbraio 2024")

            Riga_Requisiti("Migra",
                           "Aggiunta tabella Agronica_Log_Analisi per gestione log Analisi", "752")

            Riga_Requisiti("Core WS",
                           "per Piano Nutrizionale IBF", "29/08/2023")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("Modifiche UI Regione Umbria",
                      "- Loghi header/footer, title, favicon " &
                      "- Modificato messaggio 'Leggi i dati delle operazioni registrate GIAS' In 'Leggi i dati delle operazioni registrate' " &
                      "- Modifica footer report")

            '==================================

            Riga_Data("20 Dicembre 2023")

            Riga_Requisiti("Migra",
                           "Aggiunta tabella Agronica_Log_Analisi per gestione log Analisi", "752")

            Riga_Requisiti("Core WS",
                           "per Piano Nutrizionale IBF", "29/08/2023")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("Analisi Piano Concimazione / Nutrizionale",
                     "Aggiunta gestione log Analisi")

            '==================================

            Riga_Data("06 Dicembre 2023")

            Riga_Requisiti("Migra",
                           "per Piano Nutrizionale IBF", "736")

            Riga_Requisiti("Core WS",
                           "per Piano Nutrizionale IBF", "29/08/2023")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Bug("Definizione Consistenza Testata PUA",
                      "Fix caricamento date validità testata PUA",
                      "Regione Umbria", 27468)

            '==================================

            Riga_Data("30 Ottobre 2023")

            Riga_Requisiti("Migra",
                           "per Piano Nutrizionale IBF", "736")

            Riga_Requisiti("Core WS",
                           "per Piano Nutrizionale IBF", "29/08/2023")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("WebConfig",
                      "Aggiunto file separato sessionState", "coldiretti", 0, "", "Nessun Test")

            '==================================

            Riga_Data("25 Ottobre 2023")

            Riga_Requisiti("Migra",
                           "per Piano Nutrizionale IBF", "736")

            Riga_Requisiti("Core WS",
                           "per Piano Nutrizionale IBF", "29/08/2023")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("WebConfig",
                      "Aggiunto file separato httpCookies", "coldiretti", 0, "", "Nessun Test")


            '==================================

            Riga_Data("19 Ottobre 2023")

            Riga_Requisiti("Migra",
                           "per Piano Nutrizionale IBF", "736")

            Riga_Requisiti("Core WS",
                           "per Piano Nutrizionale IBF", "29/08/2023")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("Web.config",
                      "Inserita nel web.config la chiave globalization per far renderizzare correttamente i caratteri accentati")

            '==================================

            Riga_Data("10 Ottobre 2023")

            Riga_Requisiti("Migra",
                           "per Piano Nutrizionale IBF", "736")

            Riga_Requisiti("Core WS",
                           "per Piano Nutrizionale IBF", "29/08/2023")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("Lettura Effluenti",
                      "Passato nuovo parametro objParametri_Super_Server alla funzione AgronicaCoreWebService.PianoConcimazione_WS.Effluenti per la lettura degli Effluenti")

            '==================================

            Riga_Data("29 Settembre 2023")

            Riga_Requisiti("Migra",
                           "per Piano Nutrizionale IBF", "736")

            Riga_Requisiti("Core WS",
                           "per Piano Nutrizionale IBF", "29/08/2023")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Bug("Piano Nutrizionale IBF",
                      "Fix calcolo Liscivazioni
                      Corretto l'utilizzo di N g/KG (analisi) nella formula")

            Riga_Text("Restyle Grafico",
                     "- Ri-organizzazione file css in multipli file separati <code>styleGiasComponents.css, styleGiasIcons.css, styleGiasPages.css, styleGiasUtils.css</code> anziché l'unico file <code>styleXonneTables.css</code>")

            '==================================

            Riga_Data("05 Settembre 2023")

            Riga_Requisiti("Migra",
                           "per Piano Nutrizionale IBF", "736")

            Riga_Requisiti("Core WS",
                           "per Piano Nutrizionale IBF", "29/08/2023")

            Riga_Requisiti("WebService",
                           "per fix chiamata 25380 (Bonifiche Ferraresi)", "05/09/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Bug("Caricamento Griglia PUA",
                      "Fix caricamento griglia PUA per errore di 'Conversione non Gestita' ",
                      "Bonifiche Ferraresi", 25380)

            '==================================

            Riga_Data("29 Agosto 2023")

            Riga_Requisiti("Migra",
                           "per Piano Nutrizionale IBF", "736")

            Riga_Requisiti("Core WS",
                           "per Piano Nutrizionale IBF", "29/08/2023")

            Riga_Requisiti("WebService",
                           "per Piano Nutrizionale IBF", "29/08/2023")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("Piano Nutrizionale IBF",
                      "Aggiunto nuovo Piano Nutrizionale IBF",
                      noteTecniche:="Sotto permesso Piano_Nutrizionale (469). Necessità presenza di un regolamento di tipo 5 (Piano Nutrizionale IBF) e tutti i dati correlati per la popolazione dei campi")

            '==================================

            Riga_Data("19 Luglio 2023")

            Riga_Requisiti("Migra",
                           "per Piano Nutrizionale", "668")

            Riga_Requisiti("Core Ws",
                           "x PUA", "19 Novembre 2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("Menu Piano Concimazione:",
                      "Gestita apertura pagina del Menu Piano Concimazione dal Nuovo Menu Agenda (Angular)")

            '==================================

            Riga_Data("23 Giugno 2023")

            Riga_Requisiti("Migra",
                           "per Piano Nutrizionale", "668")

            Riga_Requisiti("Core Ws",
                           "x PUA", "19 Novembre 2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("Restyle grafico:",
                      "- Impostata auto-chiusura dopo 3 secondi per notifiche di successo")

            '==================================

            Riga_Data("09 Giugno 2023")

            Riga_Requisiti("Migra",
                           "per Piano Nutrizionale", "668")

            Riga_Requisiti("Core Ws",
                           "x PUA", "19 Novembre 2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "09/06/2023")

            Riga_Text("Restyle Grafico",
                      "- Modifiche a MenuBS Piano Concimazione" &
                      "- Modifiche a PCB Inserimento per restyle ASP Checkbox in Switch")

            '==================================

            Riga_Data("23 Maggio 2023")

            Riga_Requisiti("Migra",
                           "per Piano Nutrizionale", "668")

            Riga_Requisiti("Core Ws",
                           "x PUA", "19 Novembre 2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "03/03/2023")

            Riga_Bug("Griglia Piano Colturale PUA",
                      "Fix grafico popup modifica Piano Colturale (In riga) : rimossi tag HTML 
                      Come conseguenza gradita sembra essersi sistemato il baco del Seleziona Tutto In griglia",
                      "GRANFRUTTAZANI", 23880)

            '==================================

            Riga_Data("12 Maggio 2023")

            Riga_Requisiti("Migra",
                           "per Piano Nutrizionale", "668")

            Riga_Requisiti("Core Ws",
                           "x PUA", "19 Novembre 2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "03/03/2023")

            Riga_Bug("Selezionare/Deselezionare Appezzamenti in Piano Concimazione",
                      "Fix funzionalità di selezione/deselezione di tutti gli appezzamenti nel piano concimazione.",
                      "GRANFRUTTAZANI", 23880)

            Riga_Text("Aggiornamento Stato Impianto PUA",
                      "Quando si carica la griglia degli impianti per il Piano Distribuzione, lo stato dell'impianto viene impostato a 'Impianto in Produzione' (=120), se salvato a 0 su db.")

            Riga_Text("GiasBase", "Eliminati tutti i riferimenti fissi al GiasBase (/GiasBase/... etc nel codice - lettura parametrizzata tramite chiave in configurazione siti")

            '==================================

            Riga_Data("28 Aprile 2023")

            Riga_Requisiti("Migra",
                           "per Piano Nutrizionale", "668")

            Riga_Requisiti("Core Ws",
                           "x PUA", "19 Novembre 2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "03/03/2023")

            Riga_Bug("Caricamento Origine Dati Piogge  Piano Concimazione",
                     "Sistemato il caricamento del campo Origine Dati nella sezione delle piogge",
                     "Consorzi Agrari d'Italia - CAI", 23773)

            Riga_Text("Origine Dati Piogge - Piano Concimazione",
                      "migliorata grafica menù a tendina")

            Riga_Text("Chiamate WS al Meteo",
                      "Aggiunta proprietà targetFramework='4.8' al WebConfig per fare le chiamate al Meteo")

            '==================================

            Riga_Data("21 Aprile 2023")

            Riga_Requisiti("Migra",
                           "per Piano Nutrizionale", "668")

            Riga_Requisiti("Core Ws",
                           "x PUA", "19 Novembre 2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "03/03/2023")

            Riga_Bug("Caricamento Piogge da Operazioni Registrate",
                     "Sistemato il caricamento piogge con check opzione 'Leggi i dati dalle operazioni registrate GIAS', avendo selezionato 'Tutti i Centri Aziendali' nel menu a tendina dei Centri.",
                     "CAB TERRA", 23366)

            '==================================

            Riga_Data("06 Marzo 2023")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "03/03/2023")

            Riga_Text(" Dashboard ",
                     "- Adeguamento master page per nuova grafica + header")

            Riga_Text("PUA piano distribuzione:",
                      "gestito default sullo stato impianto (in produzione) se non popolato sull'esercizio. Senza lo stato impianto non viene letto il MAS.")

            '==================================

            Riga_Data("10 Febbraio 2023")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Bug("Gestione Richieste",
                      "Implementazione per redirect da Angular ")

            '==================================

            Riga_Data("04 Novembre 2022")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Bug("Piano Concimazione",
                      "fix grafico: aumentata larghezza colonna 'Azioni'")

            '==================================

            Riga_Data("01 Settembre 2022")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("Sito completo",
                      "- Modifiche per gestire nuove versioni di kendo 2022.x")

            Riga_Text("PUA effluenti, letamazioni e piano distribuzione",
                      "- Modifiche specifiche sulla pagina per gestire correttamente editabilità o meno delle textbox numeriche")

            '==================================

            Riga_Data("07 Luglio 2022")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("GIAS BASE",
                       "Nuova versione Kendo", "19 Novembre 2021")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Bug("Stampa Piano di Fertilizzazione: ",
                     "Migliorato layout e mantenuta sezione analisi del suolo solo su prima pagina della stampa [rif. chiamata 18593 IDSC]")

            '==================================

            Riga_Data("01 Luglio 2022")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("GIAS BASE",
                       "Nuova versione Kendo", "19 Novembre 2021")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Bug("Stampa PC",
                     "Fixato errore stampa PC:il testo degli appezzamenti veniva sovrapposto alle note [rif. chiamata 18593 IDSC]")

            '==================================

            Riga_Data("27 Maggio 2022")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("GIAS BASE",
                       "Nuova versione Kendo", "19 Novembre 2021")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Text("RispostaStandard",
                     "- gestione compressione rispostastandard  lato server / decompressione lato client JS")

            '==================================

            Riga_Data("13 Maggio 2022")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("GIAS BASE",
                       "Nuova versione Kendo", "19 Novembre 2021")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Requisiti("WEB.CONFIG:",
                         "Modifiche NO")

            Riga_Text("Piano Nutrizionale:",
                      "modifica di alcune labels")

            '==================================

            Riga_Data("04 Aprile 2022")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("GIAS BASE",
                       "Nuova versione Kendo", "19 Novembre 2021")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Requisiti("WEB.CONFIG:",
                         "Modifiche NO")

            Riga_Bug("PianoConcimazione_MenuBS:",
                      "bugfix gestione modifica validita filtro ricerca .")

            '==================================

            Riga_Data("02 Marzo 2022")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("GIAS BASE",
                       "Nuova versione Kendo", "19 Novembre 2021")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Requisiti("WEB.CONFIG:",
                         "Modifiche NO")

            Riga_Text("Pua_Piano_Distribuzione, PC_PUA_FiltraEsporta :",
                      "gestione della validità della vulnerabilità delle particelle.")

            '==================================

            Riga_Data("01 Marzo 2022")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("GIAS BASE",
                       "Nuova versione Kendo", "19 Novembre 2021")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Requisiti("WEB.CONFIG:",
                         "Modifiche NO")

            Riga_Bug("Preimpostazione sorgenti meteo", "Bugfix errore 500 in chiamate Carica_Piogge_WS se le ddl 'Categoria sorgente dati' ed 'Origine dati meteo' sono preimpostate con i default")

            '==================================

            Riga_Data("01 Febbraio 2022")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("GIAS BASE",
                       "Nuova versione Kendo", "19 Novembre 2021")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Requisiti("WEB.CONFIG:",
                         "Modifiche NO")

            Riga_Text("Cache:", "Aggiunto asmx con metodo ClearCache chiamato da esterno")

            '==================================

            Riga_Data("15 Dicembre 2021")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("GIAS BASE",
                       "Nuova versione Kendo", "19 Novembre 2021")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Requisiti("WEB.CONFIG:",
                         "Modifiche NO")

            Riga_Bug("PianoNutrizionale, PianoConcimazione:",
                      "Bugfix chiamata stazioni meteo")

            Riga_Bug("PianoConcimazione:",
                      "Bugfix errore generazione pdf allegato se non già presente")

            '==================================

            Riga_Data("09 Dicembre 2021")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("GIAS BASE",
                       "Nuova versione Kendo", "19 Novembre 2021")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Requisiti("WEB.CONFIG:",
                         "Modifiche NO")

            Riga_Text("PianoNutrizionale:",
                      "Piano Nutrizionale - modifiche stampe")

            Riga_Text("PianoNutrizionale e Piano concimazione:",
                      "Aggiunta sorgente meteo per rilievo piogge (Stazioni preferite irrigazione)")

            '==================================

            Riga_Data("06 Dicembre 2021")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("GIAS BASE",
                       "Nuova versione Kendo", "19 Novembre 2021")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Requisiti("WEB.CONFIG:",
                         "Modifiche NO")

            Riga_Text("PianoNutrizionale:",
                      "Piano Nutrizionale - modifiche interfaccia metodo semplificato")

            '==================================

            Riga_Data("03 Dicembre 2021")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("GIAS BASE",
                       "Nuova versione Kendo", "19 Novembre 2021")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Requisiti("WEB.CONFIG:",
                         "Modifiche NO")

            Riga_Bug("PUA, PianoConcimazione:",
                      "Bugfix controllo chiusura sportello")

            Riga_Text("PianoNutrizionale:",
                      "Piano Nutrizionale - PRIMO RILASCIO")

            '==================================

            Riga_Data("29 Novembre 2021")

            Riga_Requisiti("Migra",
                       "per Piano Nutrizionale", "668")

            Riga_Requisiti("GIAS BASE",
                       "Nuova versione Kendo", "19 Novembre 2021")

            Riga_Requisiti("Core Ws",
                       "x PUA", "19 Novembre 2021")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Bug("PUA_Dichiarazione_Effluenti :",
                      "Bugfix caricamento regolamento in info e modifica pua")

            Riga_Text("PUA_Dichiarazione_Effluenti :",
                      "Tolta la possibilità di gestire il regolamento PAN 2018-2021 - Emilia Romagna")

            '==================================

            Riga_Data("19 Novembre 2021")

            Riga_Requisiti("Migra",
                           "per Piano Nutrizionale", "668")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione Kendo", "19 Novembre 2021")

            Riga_Requisiti("Core Ws",
                           "x PUA", "19 Novembre 2021")

            Riga_Requisiti("WEB.CONFIG :",
                             "Modifiche NO")

            Riga_Text("PianoNutrizionale:",
                      "piano nutrizionale wip")

            '==================================

            Riga_Data("10 Agosto 2021")

            Riga_Requisiti("Migra",
                       "per Piano Concimazione", "655")

            Riga_Requisiti("GIAS BASE",
                       "Nuova versione Kendo", "22 Febbraio 2021")

            Riga_Requisiti("Core Ws",
                       "x PUA", "10 Marzo 2021")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Bug("PCB_Inserimento:",
                  "bugfix piano concimazione semplificato in caso di mancanza del mas")

            '==================================

            Riga_Data("28 Luglio 2021")

            Riga_Requisiti("Migra '655' :",
                       "per Piano Concimazione")

            Riga_Requisiti("GIAS BASE - 22 Febbraio 2021:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 10 Marzo 2021",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Bug("PCB_Inserimento, PCB_InserimentoMultiplo :",
                  "bugfix errato uso Agro_SQL_SaveText nella funzione PianoConcimazione_EntitaxTestata_W.Scrivi")

            '==================================

            Riga_Data("26 Luglio 2021")

            Riga_Requisiti("Migra '655' :",
                       "per Piano Concimazione")

            Riga_Requisiti("GIAS BASE - 22 Febbraio 2021:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 10 Marzo 2021",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("PCB_Inserimento:",
                  "introdotta la possibilità di selezionare il mas della direttiva nitrati")

            '==================================

            Riga_Data("8 Luglio 2021")

            Riga_Requisiti("Migra '637' :",
                       "per PUA e Piano Concimazione")

            Riga_Requisiti("GIAS BASE - 22 Febbraio 2021:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 10 Marzo 2021",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Pua_Piano_Distribuzione:",
                  "bugfix su lettura default letamazioni precedenti")

            '==================================

            Riga_Data("18 Giugno 2021")

            Riga_Requisiti("Migra '637' :",
                       "per PUA e Piano Concimazione")

            Riga_Requisiti("GIAS BASE - 22 Febbraio 2021:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 10 Marzo 2021",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("PCB_Inserimento, PCB_InserimentoMultiplo :",
                  "- bugix errore visualizzazione rese n-p-k (alta e bassa) " &
                  "- modificata label da Mg a MgO (anche nelle stampe)")

            Riga_Text("PCB_Inserimento :",
                  "bugfix errore in caso di mancato caricamento dell'elenco delle finalità")

            Riga_Text("RPT_Scheda_NPK.rpt e RPT_Bilancio_NPK.rpt :",
                  "Aggiunta la Data e la Firma in fondo ai report visualizzabili solamente se è attiva l'Impostazione Utente 'Visualizza la riga Data/Firma' (Tipo enumerativo UTENTE_COD_FILTRO_STAMPA_PIANOCONCIMAZIONE_CAMPAGNA_DATA_FIRMA).")

            Riga_Text("MasterConcimazione.Master:",
                  "Allineamento ad AgronicaControlli_2010 per inclusione delle versioni di JQuery e Bootstrap scelte in Configurazione Siti")

            '==================================

            Riga_Data("1 Aprile 2021")

            Riga_Requisiti("Migra '637' :",
                       "per PUA e Piano Concimazione")

            Riga_Requisiti("GIAS BASE - 22 Febbraio 2021:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 10 Marzo 2021",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("PUA_Dichiarazione_Effluenti :",
                  "sistemata gestione intervalli per periodo divieto per gli effluenti caricati in automatico in base ai ddt caricati")

            '==================================

            Riga_Data("23 Marzo 2021")

            Riga_Requisiti("Migra '637' :",
                       "per PUA e Piano Concimazione")

            Riga_Requisiti("GIAS BASE - 22 Febbraio 2021:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 10 Marzo 2021",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Pua_Piano_Distribuzione :",
                  "modificato il controllo di assegnazione N Mas da PUA per conteggiare l'eventuale fattore correttivo")

            '==================================

            Riga_Data("22 Marzo 2021")

            Riga_Requisiti("Migra '637' :",
                       "per PUA e Piano Concimazione")

            Riga_Requisiti("GIAS BASE - 22 Febbraio 2021:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 10 Marzo 2021",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Pua_Piano_Distribuzione :",
                  "- nascosti solo i dati del bilancio (prima riga) nel caso di metodo semplificato dalla pagina di verifica" &
                  "- modificati filtri colonne")

            Riga_Text("PUA_Dichiarazione_Effluenti:",
                  "- sistemata conversione UdM in fase di caricamento dei conferimenti esterni" &
                  "- eliminato controllo effluenti doppi quando si caricano i conferimenti esterni" &
                  "- aggiunto Salva Personalizzazioni Griglia" &
                  "- sistemati filtri nelle colonne")

            Riga_Text("PianoConcimazione_MenuBS.aspx:",
                  "aggiunto Salva Personalizzazioni Griglia")

            '==================================

            Riga_Data("15 Marzo 2021")

            Riga_Requisiti("Migra '637' :",
                       "per PUA e Piano Concimazione")

            Riga_Requisiti("GIAS BASE - 22 Febbraio 2021:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 10 Marzo 2021",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("SqlClient:",
                  "Impostato uso SqlClient di default al posto dell'OleDb")

            '==================================

            Riga_Data("11 Marzo 2021")

            Riga_Requisiti("Migra '637' :",
                       "per PUA e Piano Concimazione")

            Riga_Requisiti("GIAS BASE - 22 Febbraio 2021:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 10 Marzo 2021",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("PUA_Dichiarazione_Effluenti:",
                  "sistemata gestione intervalli per periodo divieto")

            Riga_Text("Pua_Piano_Distribuzione :",
                  "- nel metodo semplificato gestita modifica di N mas, Resa riferimento e fattore correttivo al cambio della tipologia/finalità" &
                  "- nel metodo semplificato gestita la modifica multipla di alcuni parametri" &
                  "- corretto bug nella lettura dei dati dal precedente pua (dovuto al cambio codice direttiva)" &
                  "- nascosti dati delle medie nel caso di metodo semplificato dalla pagina di verifica")

            Riga_Text("Versione .Net 4.8",
                  "Passaggio alla versione .Net 4.8")

            '==================================

            Riga_Data("5 Marzo 2021")

            Riga_Requisiti("Migra '637' :",
                       "per PUA e Piano Concimazione")

            Riga_Requisiti("GIAS BASE - 22 Febbraio 2021:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 5 Marzo 2021",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Pua_Piano_Distribuzione :",
                  "- gestito il filtro PUA_Tipo nelle funzioni Precessione e PrecessionexSpecie" &
                  "- considerato parametro Ns (N da fertilizzazioni precedenti) ed eventuale Fattore Correttivo (per resa dichiarata maggiore) nella funzione FabbisognixTipoPUA x metodo MAS" &
                  "- salvata analisi nella tabella Pua_Elaborazione")

            Riga_Text("PUA_Dichiarazione_Effluenti:",
                  "- introdotto il flag dichiarazione di non utilizzo fertilizzanti" &
                  "- gestiti intervalli per periodo divieto")

            Riga_Text("PCB_Inserimento:",
                  "introdotti i campi note e flag dichiarazione di non utilizzo fertilizzanti")

            Riga_Text("PianoConcimazione_MenuBS.aspx:",
                  "visualizzato il campo note e flag dichiarazione di non utilizzo fertilizzanti")

            Riga_Text("Pua_Stampa:",
                  "- modificata lettura peridodo divieto per gestire intervalli" &
                  "- aggiunta la visualizzazione dichiarazione di non utilizzo fertilizzanti")

            Riga_Text("Stampa Piano Concimazione Schede/Bilancio:",
                  "aggiunte note e visualizzazione dichiarazione di non utilizzo fertilizzanti")

            Riga_Text("Versione .Net 4.8",
                  "Passaggio alla versione .Net 4.8")

            '==================================


            Riga_Data("2 Febbraio 2021")

            Riga_Requisiti("Migra '618' :",
                       "per nuove colonne movimenti dettagli")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Stampa Schede/Bilancio:",
                  "patch sul salvataggio del pdf del piano (nel caso il codice socio contiene il carattere '|' viene sostituito con '_' )")

            Riga_Text("PianoConcimazione_MenuBS.aspx:",
                  " Gestito Blocco e Sblocco di Piani Concimazione e PUA ")

            '==================================

            Riga_Data("29 Ottobre 2020")

            Riga_Requisiti("Migra '618' :",
                       "per nuove colonne movimenti dettagli")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("MasterConcimazione:",
                  "- impostato file corretto jquery.cookie da GiasBase ")

            Riga_Text("PUA_Dichiarazione_Effluenti:",
                  "corretto baco lettura disciplinari da ente")

            '==================================
            '==================================

            Riga_Data("26 Ottobre 2020")

            Riga_Requisiti("Migra '607' :",
                       "necessario per log pua e cambio chiave pua_effluenti")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("PUA_Dichiarazione_Effluenti:",
                  "- impostazione disciplinare-ente")

            '==================================
            '==================================

            Riga_Data("22 Ottobre 2020")

            Riga_Requisiti("Migra '607' :",
                       "necessario per log pua e cambio chiave pua_effluenti")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("tutto:",
                  "- Gestione SQLClient")

            '==================================

            Riga_Data("16 Ottobre 2020")

            Riga_Requisiti("Migra '607' :",
                       "necessario per log pua e cambio chiave pua_effluenti")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("PianoConcimazione_MenuBS:",
                  "- cookie per gestione date")

            '==================================
            '==================================

            Riga_Data("12 Ottobre 2020")

            Riga_Requisiti("Migra '607' :",
                       "necessario per log pua e cambio chiave pua_effluenti")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Pua_Stampa:",
                  "- modificato calcolo delle medie aziendali" &
                  "- aggiunta la visualizzazione dell'azoto distribuito totale")

            Riga_Text("Pua_Piano_Distribuzione:",
                  "aggiunta la visualizzazione dell'azoto distribuito totale")

            '==================================

            Riga_Data("8 Ottobre 2020")

            Riga_Requisiti("Migra '607' :",
                       "necessario per log pua e cambio chiave pua_effluenti")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Pua_Piano_Distribuzione:",
                  "- modificato calcolo delle medie aziendali" &
                  "- corretto calcolo N da letamazioni precedenti" &
                  "- aggiunto bottone 'aggiorna letamazioni precedenti'")

            Riga_Text("Pua_Letamazioni_Precedenti:",
                  "corretto calcolo N da letamazioni precedenti")

            Riga_Text("Stampa Piano Schede/Bilancio:",
                  "aggiunta indicazione MAS + eventuale fattore correttivo")

            Riga_Text("PCB_Inserimento:",
                  "aggiunta indicazione MAS + eventuale fattore correttivo anche nel metodo schede")

            Riga_Text("PCB_Inserimento:",
                  "bugfix gestione decimali col punto")

            '==================================

            Riga_Data("25 Settembre 2020")

            Riga_Requisiti("Migra '607' :",
                       "necessario per log pua e cambio chiave pua_effluenti")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("PCB_InserimentoMultiplo:",
                  "bugfix filtro per escludere le analisi scadute al cambio regolamento")

            '==================================

            Riga_Data("9 Settembre 2020")

            Riga_Requisiti("Migra '607' :",
                       "necessario per log pua e cambio chiave pua_effluenti")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("PCB_InserimentoMultiplo:",
                  "- permesso salvataggio piani anche se va in errore la creazione del report (loggata questa casistica)" &
                  "- inserito record allegato solo nel caso in cui è stato salvato il pdf (in caso contrario nel menu la graffetta era chiusa ma dava errore)" &
                  "- introdotta nuova lettura delle analisi al cambio del regolamento (cambio date)")

            Riga_Text("PCB_InserimentoMultiplo, PCB_Inserimento:",
                  "- bugfix caricamento analisi sul centro" &
                  "- bugfix lettura parametro K20 dell'analisi")

            '==================================

            Riga_Data("20 Agosto 2020")

            Riga_Requisiti("Migra '607' :",
                       "necessario per log pua e cambio chiave pua_effluenti")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("OPERAZIONE CRYSTAL REPORT:",
                  "impostata copia localmente = false sui riferimenti Crystal")

            Riga_Text("File_Temporanei:",
                  "creata cartella con file fittizio.")

            Riga_Text("report su PC_Stampe:",
                  "- introdotto salvataggio log_errori" &
                  "- commentato utilizzo CrystalReportViewer1 (inutile da quando si usa la apgina VisualizzatoreReport)")

            '==================================

            Riga_Data("19 Agosto 2020 versione B")

            Riga_Requisiti("Migra '607' :",
                       "necessario per log pua e cambio chiave pua_effluenti")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche SI: " &
                         "- introdotto file appsettings.config con link su web.config")

            Riga_Text("OPERAZIONE CRYSTAL REPORT:",
                  "- impostata versione specifica = false sui riferimenti cystal" &
                  "- Nella pagina VisualizzatoreReport.aspx modificata la versione crystal")

            '==================================

            Riga_Data("19 Agosto 2020")

            Riga_Requisiti("Migra '607' :",
                       "necessario per log pua e cambio chiave pua_effluenti")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Pua_Piano_Distribuzione:",
                  "inserito default letamazioni precedenti solo in fase di creazione pua (come gli atri default)")

            '==================================

            Riga_Data("18 Agosto 2020")

            Riga_Requisiti("Migra '607' :",
                       "necessario per log pua e cambio chiave pua_effluenti")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("PCB_InserimentoMultiplo:",
                  "introdotto filtro per escludere le analisi scadute")

            '==================================

            Riga_Data("4 Agosto 2020")

            Riga_Requisiti("Migra '607' :",
                       "necessario per log pua e cambio chiave pua_effluenti")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("PCB_InserimentoMultiplo:",
                  "bugfix routine aggregazione dei piani per analisi")

            '==================================

            Riga_Data("24 Luglio 2020")

            Riga_Requisiti("Migra '607' :",
                       "necessario per log pua e cambio chiave pua_effluenti")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Pua:",
                  "- introdotto salvataggio log (tabelle Pua_Testata, PUA_Effluente, Anagrafe_VincoliAgronomici, PUA_LetamazioniPrecedenti)" &
                  "- introdotta la possibilità di caricare più volte lo stesso effluente (con titolo o provenienza diversa) in fase di dichiarazione (cambiata chiave tabella PUA_Effluente)")

            '==================================

            Riga_Data("17 Luglio 2020")

            Riga_Requisiti("Migra '606' :",
                       "necessario per log ricette")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Pua_Piano_Distribuzione:",
                  "bugfix errore letamazioni precedenti con stessa unita di misura")

            '==================================

            Riga_Data("15 Luglio 2020")

            Riga_Requisiti("Migra '588' :",
                       "necessario per Allegati")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("PUA_LetamazioniPrecedenti:",
                  "aggiunta visualizzazione delle letamazioni presenti sul registro degli anni precedenti")

            Riga_Text("Pua_Piano_Distribuzione:",
                  "aggiunto calcolo e salvataggio del default N fertilizzazioni precedenti")

            Riga_Text("Pua_Stampa:",
                  "visualizzate anche le fertilizzazioni registrate scollegate dal pua (vengono ora lette tutte in base alle date del pua)")

            '==================================

            Riga_Data("13 Luglio 2020")

            Riga_Requisiti("Migra '588' :",
                       "necessario per Allegati")

            Riga_Requisiti("GIAS BASE - 3 Luglio 2020:",
                       "Nuova versione Kendo")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Pua_Stampa:",
                  "- corretta stampa fertilizzazioni (non le stampava con il nuovo stato 300)" &
                  "- sostituiti istat prov e com con le descrizioni")

            Riga_Text("PianoConcimazione_MenuBS:",
                  "- modificata grafica" &
                  "- aggiunto bottone 'Importa comunicazione' per PUA ER")

            Riga_Text("Pua_Piano_Distribuzione:",
                  "- visualizzati appezzamenti senza catasto" &
                  "- sostituiti istat prov e com con le descrizioni")

            '==================================

            Riga_Data("7 Luglio 2020")

            Riga_Requisiti("Migra '588' :",
                       "necessario per Allegati")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("PianoConcimazione_MenuBS:",
                  "eliminati dall'elenco i pua registrati con regolamento precedente al 78 (pua umbria) perché dati non compatibili")

            '==================================

            Riga_Data("6 Luglio 2020")

            Riga_Requisiti("Migra '588' :",
                       "necessario per Allegati")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")


            Riga_Text("PianoConcimazione_MenuBS:",
                  "- modificata colonna da tipo a metodo per pua" &
                  "- linkata stampa PUA semplificato")

            Riga_Text("Pua_Piano_Distribuzione:",
                  "- modificato filtro per caricamento appezzamenti (da date validita ricetta a date validita pua)" &
                  "- aggiunto dettaglio superfici nel bilancio (secondo ciclo, fertilizzata e fertilizzata con effluente zootecnico)" &
                  "- aggiunta griglia effluenti nel bilancio")

            Riga_Text("Pua_Stampa:",
                  "- aggiunto dettagli qta distribuita, efficienza riferimento e conseguita nella griglia effluenti" &
                  "- aggiunto dettaglio superfici nel bilancio (secondo ciclo, fertilizzata e fertilizzata con effluente zootecnico)")


            '==================================

            Riga_Data("18 Giugno 2020")

            Riga_Requisiti("Migra '588' :",
                       "necessario per Allegati")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("PUA_Dichiarazione_Effluenti:",
                  "eliminati fertilizzanti chimici con giacenza = 0 dalla visualizzazione")

            Riga_Text("PC_PUA_FiltraEsporta:",
                  "modifiche varie")

            Riga_Text("Pua_Piano_Distribuzione:",
                  "- ottimizzato caricamento (ottimizzata assegnazione default ed eliminata in modalita verifica bilancio)" &
                  "- aggiunto dettaglio superfici")

            '==================================

            Riga_Data("30 Aprile 2020")

            Riga_Requisiti("Migra '588' :",
                       "necessario per Allegati")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Migra :",
                  "Impostato giusto pre-requisito")

            '==================================

            Riga_Data("27 Aprile 2020")

            Riga_Requisiti("Migra '580' :",
                       "necessario per PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("PUA :",
                  "sistemati permessi")

            Riga_Text("Stampa Schede/Bilancio:",
                  "patch sul salvataggio del pdf del piano (nel caso il codice socio contiene il carattere '/' o '\' viene sostituito con '_' )")

            Riga_Text("PCB_Inserimento, PCB_InserimentoMultiplo :",
                  "ripristinata verifica dei parametri dell'analisi precedente se non presenti nella selezionata")

            '==================================

            Riga_Data("10 Marzo 2020")

            Riga_Requisiti("Migra '580' :",
                       "necessario per PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("PUA :",
                  "sistemati permessi")

            Riga_Text("PCB_InserimentoMultiplo:",
                  "- introdotta aggregazione per stato impianto/ciclo/fase" &
                  "- sistemato mas non definito")

            Riga_Text("PCB_Inserimento, PCB_InserimentoMultiplo :",
                  "- introdotta la possibilità di editare gli apporti massimi di N, P e K da assegnare agli appezzamenti (possono essere diversi da quelli calcolati nel piano)" &
                  "- introdotta gestione fattore correttivo per MAS in base alla resa (es. veneto)")

            Riga_Text("Stampa Schede/Bilancio:",
                 "sistemata visualizzazione finalità e fase/ciclo")

            '==================================

            Riga_Data("26 Febbraio 2020")

            Riga_Requisiti("Migra '580' :",
                       "necessario per PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("PianoConcimazione_MenuBS:",
                  "sistemata visibilita piani concimazione e pua (gestita visibilita centro)")

            '==================================

            Riga_Data("25 Febbraio 2020")

            Riga_Requisiti("Migra '580' :",
                       "necessario per PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("PianoConcimazione_MenuBS:",
                  "sistemata visibilita piani concimazione e pua (gestita visibilita centro)")

            Riga_Text("Pua_Piano_Distribuzione:",
                  "- messo sotto permesso bottone Importa Fertilizzazioni da Registro" &
                  "- visualizzato bottone Modifica Multipla Righe selezionate solo per metodo bilancio")

            Riga_Text("PCB_Inserimento, PCB_InserimentoMultiplo :",
                  "modificata data inizio inverno (utilizzata per caricare via WS i dati meteo)")

            Riga_Text("PCB_Inserimento:",
                  "settato il default sulle stazioni meteo se salvate sul centro")

            '==================================

            Riga_Data("21 Febbraio 2020")

            Riga_Requisiti("Migra '580' :",
                       "necessario per PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("PianoConcimazione_MenuBS:",
                  "- aggiunta indicazione del centro (se indicato nel piano concimazione o nel pua)" &
                  "- eliminata colonna specie per i pua")

            Riga_Text("PUA:",
                  "- introdotta gestione centro aziendale" &
                  "- introdotto menu metodo calcolo")

            Riga_Text("PCB_Inserimento:",
                  "gestione dati piogge (meteo) da stazioni in visibilità/proprietà")

            Riga_Fine()

            '==================================
            '==================================

            Riga_Data("17 Febbraio 2020")

            Riga_Requisiti("Migra '579' :",
                       "nuova tabela ServizixSportello")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("GestioneRichieste:",
                  "azzerata sessione in ingresso al sito per forzare il ricaricaricamento dei permessi")

            Riga_Text("PCB_Inserimento:",
                  "Controllo stato verifica con riserva per sblocco pua")

            Riga_Text("PCB_InserimentoMultiplo:",
                  "Controllo stato verifica con riserva per sblocco pua")

            Riga_Text("PCB_Dichiarazione_Effluenti:",
                  "Controllo stato verifica con riserva per sblocco pua")


            Riga_Fine()

            '==================================

            Riga_Data("12 Febbraio 2020")

            Riga_Requisiti("Migra '579' :",
                       "nuova tabela ServizixSportello")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 12 Febbraio 2020",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("PUA_Dichiarazione_Effluenti:",
                  "modificata funzione apriPianoDistr (aggiunti parametri passati al coreWS")


            Riga_Fine()

            '==================================

            Riga_Data("11 Febbraio 2020")

            Riga_Requisiti("Migra '579' :",
                       "nuova tabela ServizixSportello")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 22 Novembre 2019",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("PUA_Dichiarazione_Effluenti:",
                  "corretto baco che caricava gli effluenti ripetuti in fase di creazione del PUA")

            Riga_Text("Pua_Piano_Distribuzione:",
                  "- introdotti i default su analisi, ubicazione ed irrigazione presi dal pua precedente e sulla precessione ricavata dal piano colturale (passando dal catasto)" &
                  "- introdottti link Vai a Verifica Indici Bilancio e Vai a Piano Distribuzione")

            Riga_Text("PUA_Dichiarazione_Effluenti.aspx", "Controllo Sportello")

            Riga_Text("PCB_Inserimento.aspx", "Controllo Sportello")

            Riga_Text("PCB_InserimentoMultiplo.aspx", "Controllo Sportello")

            Riga_Fine()

            '==================================

            Riga_Data("3 Febbraio 2020")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 22 Novembre 2019",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("PC_PUA_FiltraEsporta:",
                  "introdotta pagina PC_PUA_FiltraEsporta (da finire bene)")

            Riga_Text("PCB_InserimentoMultiplo:",
                  "bug fix piani concimazione Singola-Azienda Multi-Coltura")

            Riga_Fine()

            '==================================

            Riga_Data("21 Gennaio 2020")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 22 Novembre 2019",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Pua_Stampa:",
                  "- rilasciata (visualizzato bottone stampa nel menu)" &
                  "- sistemato il layout (tagliava una colonna)" &
                  "- modificata visualizzazione qta di N nelle fertilizzazioni (ora sempre in Kg)")

            Riga_Text("PUA_Dichiarazione_Effluenti:",
                  "modificata visualizzazione qta di N (ora sempre in Kg anche per m3)")

            Riga_Text("PianoConcimazione_MenuBS:",
                  "modificato intervallo di visualizzazione elenco pua (anche anno precedente)")

            Riga_Fine()

            '==================================

            Riga_Data("20 Gennaio 2020")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 22 Novembre 2019",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Pua_Stampa:",
                  "- sistemato il layout (tagliava una colonna)")

            Riga_Fine()

            '==================================

            Riga_Data("16 Gennaio 2020")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 22 Novembre 2019",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Pua_Piano_Distribuzione:",
                  "fix terreni nudi che da quando non passano più da web service, mettevano -1 anziché 0 in N_Fabbisogno")

            Riga_Text("PCB_InserimentoMultiplo:",
                  "bug fix piani concimazione Multi-Aziende Multi-Coltura")

            Riga_Fine()

            '==================================

            Riga_Data("07 Gennaio 2020")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 22 Novembre 2019",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Pcb_inserimento e PCB_InserimentoMultiplo:",
                  "fix caricamento piogge quando anno attuale è bisestile, ma quello utilizzato no")

            Riga_Fine()

            '==================================

            Riga_Data("19 Dicembre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 22 Novembre 2019",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Pcb_inserimento:",
                  "fix su query che impiegava molto tempo...")

            Riga_Fine()

            '==================================

            Riga_Data("18 Dicembre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 22 Novembre 2019",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Pua_Dichiarazione_Effluenti:",
                  "fix ordinamento combo regolamenti")

            Riga_Fine()

            '==================================

            Riga_Data("17 Dicembre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 22 Novembre 2019",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("PUA_LetamazioniPrecedenti:",
                  "corretto salvataggio (in caso di eliminazione delle letamazioni precedenti non azzerava N_FertilizzazioniPrecedenti della tabella Anagrafe_VincoliAgronomici)")

            Riga_Fine()

            '==================================

            Riga_Data("16 Dicembre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 22 Novembre 2019",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("PUA_Piano_Distribuzione:",
                  "- modificata lettura impianti per leggere subito (N_TotaleSoddisfatto, N_Zootecnico_Letame, N_Zootecnico_Liquame)" &
                  "- modificata lettura dati analisi nella query per non mandare in errore con dati analisi salvati male (dettagli doppi)")

            Riga_Fine()

            '==================================

            Riga_Data("12 Dicembre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 22 Novembre 2019",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("PUA_Piano_Distribuzione:",
                  "- modificata lettura impianti per leggere subito (dati analisi, N_FabbisognoSoddisfatto, N_FabbisognoSoddisfattoOrganico, N_SoddisfattoDigestato)" &
                  "- esclusi i terreni nudi dalla chiamata al web service" &
                  "- creata nuova funzione di chiamata al web service che zippa la richiesta")

            Riga_Fine()

            '==================================


            Riga_Data("28 Novembre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 22 Novembre 2019",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PianoConcimazione_MenuBS:",
                  "corretto baco stampa piano concimazione (invertiva schede con bilancio)")

            Riga_Fine()

            '==================================

            Riga_Data("26 Novembre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 22 Novembre 2019",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PUA_Piano_Distribuzione:",
                  "modificato numero appezzamenti passati al web service")

            Riga_Text("PianoConcimazione_MenuBS:",
                  "eliminata icona stampa per il PUA")

            Riga_Fine()

            '==================================

            Riga_Data("25 Novembre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 22 Novembre 2019",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PUA_Dichiarazione_Effluenti:",
                  "- esclusi i concimi organici nell'elenco dei chimici" &
                  "- modificata la percentuale zootecnica (per i reflui)")

            Riga_Fine()

            '==================================

            Riga_Data("22 Novembre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 22 Novembre 2019",
                       "x PUA")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PUA_Piano_Distribuzione:",
                  "- aggiunti tipologia (e coefficiente B) alla modifica massiva" &
                  "- aggiunta resa alla modifica massiva" &
                  "- sostituita data registrazione con intervallo di date in cui vengono anche letti gli appezzamenti" &
                  "- esclusa la sup degli appezzamenti con ciclo secondario nel calcolo della superficie aziendale per il Bilancio" &
                  "- introdotto default su analisi piu recente")

            Riga_Text("PUA_Dichiarazione_Effluenti:",
                  "- sostituita data registrazione con intervallo date validita" &
                  "- introdotto campo note")

            Riga_Text("PianoConcimazione_MenuBS:",
                  "- sostituita data registrazione con intervallo date validita nella griglia elenco" &
                  "- corretto baco stampa in caso di permesso di sola lettura")

            Riga_Fine()

            '==================================

            Riga_Data("30 Ottobre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 7 Ottobre 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PianoConcimazione_MenuBS:",
                  "corretto filtro date nella lettura dell'elenco piani concimazione")

            Riga_Fine()

            '==================================

            Riga_Data("30 Ottobre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 7 Ottobre 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PianoConcimazione_MenuBS:",
                  " - Gestita eliminazione PianoConimazione_Elaborazione ")

            Riga_Fine()


            '==================================

            Riga_Data("30 Ottobre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 7 Ottobre 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PianoConcimazione_MenuBS:",
                  " - Gestita eliminazione PUA_LetamazioniPrecedenti e Anagrafe_VincoliAgronomici in eliminazione PUA " &
                  " - introdotta gestione blocco per piano concimazione e pua (blocco_flag)")

            Riga_Text("PUA_Piano_Distribuzione:",
                  " - visualizzati gli appezzamenti senza specie vegetale indicata (destinazione uso) " &
                  " - gestita nuova tabella Anagrafe_VincoliAgronomici (al posto di ParticelleCatastalixVincoliAgronomici)" &
                  " - introdotta nuova gestioone delle Letamazioni Precedenti" &
                  " - sistemato calcolo N_Zootecnico nel caso non sia stata fatta la dichiarazione dell'effluente ma si siano registrate le distribuzioni")

            Riga_Text("PUA_Dichiarazione_Effluenti:",
                  "- eliminato bottone salvataggio in caso il pua sia bloccato" &
                  "- corretto baco salvataggio date divieto")

            Riga_Text("PCB_Inserimento:",
                  "eliminato bottone salvataggio in caso il piano sia bloccato")

            Riga_Fine()


            '==================================

            Riga_Data("16 Ottobre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 7 Ottobre 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PUA_Piano_Distribuzione:",
                  "corretta lettura/valorizzazione del LimiteMas")

            Riga_Text("PCB_Inserimento, PCB_InserimentoMultiplo:",
                  " modificate funzioni SO_Elevata, SO_Bassa, CalcAtt_Elevato, ImpostaDoseStandard_P2O5_Default per filtrare il regolamento_cod ")

            Riga_Text("Master:",
                  "aggiunte proprieta flag_MostraHeader e flag_MostraFooter")

            Riga_Fine()

            '==================================

            Riga_Data("14 Ottobre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 7 Ottobre 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PianoConcimazione_MenuBS:",
                  "Controllo permesso scrittura pratica")

            Riga_Fine()

            '==================================

            Riga_Data("9 Ottobre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 7 Ottobre 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PUA_Piano_Distribuzione:",
                  "modificata chiamata al ws per passare 100 appezzamenti per volta")

            Riga_Fine()

            '==================================

            Riga_Data("8 Ottobre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 7 Ottobre 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PCB_Inserimento, PCB_InserimentoMultiplo:",
                  "introdotto controllo (alert non bloccante) nel caso l'analisi scelta sia scaduta")

            Riga_Fine()

            '==================================

            Riga_Data("7 Ottobre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 7 Ottobre 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PUA_Piano_Distribuzione:",
                  "aggiunta la gestione della scelta del coeff. B")

            Riga_Fine()

            '==================================

            Riga_Data("1 Ottobre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 21 Giugno 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PUA_Dichiarazione_Effluenti:",
                  "corretto baco errore salvataggio")

            Riga_Text("PUA_Piano_Distribuzione:",
                  "- modificate descrizioni ed alcuni valori del bilancio aziendale" &
                  "- eliminato salvataggio resa = -1")

            Riga_Fine()


            '==================================

            Riga_Data("30 Settembre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 21 Giugno 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PUA_Piano_Distribuzione:",
                  "- aggiunta visualizzazione N Massimo CBPA ed aggiunto bottone per associarlo agli appezzamenti selezionati." &
                  "- corretti valori N_Totale nel Bilancio")

            Riga_Fine()

            '==================================

            Riga_Data("23 Settembre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 21 Giugno 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PCB_Inserimento:",
                  "corretto baco salvataggio analisi (salvava male K e K2O).")

            Riga_Text("PUA_Piano_Distribuzione:",
                  "reso visibile bottone per importare le fertilizzazioni già registrate sul registro.")

            Riga_Fine()

            '==================================

            Riga_Data("19 Settembre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 21 Giugno 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PUA_Piano_Distribuzione:",
                  "- introdotto bottone per importare le fertilizzazioni già registrate sul registro (nascosto in questa versione)." &
                  "- modificata indicazione formula in testata griglia del bilancio" &
                  "- corretta indicazione dell N_Zootecnico (veniva conteggiato male se in dichiarazione non erano stati indicati effluenti)")

            Riga_Text("PUA_Dichiarazione_Effluenti:",
                  "modificato filtro metodo (bilancio o semplificato) sul caricamento elenco delle direttive.")

            Riga_Fine()

            '==================================

            Riga_Data("4 Settembre 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 21 Giugno 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO: ")

            Riga_Text("PUA_Dichiarazione_Effluenti:",
                  "introdotto filtro metodo (bilancio o semplificato) sul caricamento elenco delle direttive.")

            Riga_Text("Piano Concimazione:",
                  "- modificato controllo permesso stampe da modifica a lettura." &
                  "- corretto baco in modifica del piano che perdeva il legame con l'allegato (ora elimina l'allegato poiché dovrà essere rigenerato)")

            Riga_Fine()

            '==================================

            Riga_Data("29 agosto 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 21 Giugno 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche SI: MaxHttpCollectionKeys ")

            Riga_Text("Piano Concimazione massivo:",
                  "- bugfix errore quando calcola bilancio con molti dati. RISOLTO CON MODIFICA WEB CONFIG!" &
                  "- bugix errore visualizzazione rese (alta e bassa)")

            Riga_Text("Piano Concimazione menu BS:",
                  "- Creazione automatica pratiche se mancanti.")

            Riga_Fine()

            '==================================

            Riga_Data("4 Luglio 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 21 Giugno 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO. ")

            Riga_Text("Piano Concimazione massivo:",
                  "bugfix calcoli vari")

            Riga_Fine()

            '==================================

            Riga_Data("3 Luglio 2019 B")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 21 Giugno 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO. ")

            Riga_Text("Piano Concimazione massivo:",
                  "- bugfix dose calcolata K" &
                  "- bugfix su aggiorna bilancio (non considerava le modifiche dell'utente)")

            Riga_Fine()

            '==================================

            Riga_Data("3 Luglio 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 21 Giugno 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO. ")

            Riga_Text("Nuovo Pua:",
                  "primo rilascio")

            Riga_Fine()

            '==================================

            Riga_Data("28 Giugno 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 21 Giugno 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO. ")

            Riga_Text("Piano Concimazione massivo:",
                  "- bugfix raggruppamento per analisi" &
                  "- bugfix default dose P2O5" &
                  "- bugfix eliminata lettura analisi quando non presente")

            Riga_Text("Nuovo Pua:",
                  "spostamento pulsante associa N per uso su multiple righe")

            Riga_Fine()

            '==================================

            Riga_Data("27 Giugno 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 21 Giugno 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO. ")

            Riga_Text("Nuovo Pua:",
                  "WIP")

            Riga_Fine()

            '==================================

            Riga_Data("26 Giugno 2019")

            Riga_Requisiti("Migra '545' :",
                       "nuovi permessi PUA")

            Riga_Requisiti("GIAS BASE - 26 Giugno 2019 :",
                       "per modifiche a CreaKendoGrid")

            Riga_Requisiti("CoreWs 21 Giugno 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO. ")

            Riga_Text("Nuovo Pua:",
                  "WIP")

            Riga_Fine()

            '==================================

            Riga_Data("21 Giugno 2019")

            Riga_Requisiti("Migra '544' :",
                       "Richiesto per nuova colonna TipoAcqua")

            Riga_Requisiti("GIAS BASE - 07 Giugno 2019 :",
                       "per creaDropDownEditorId")

            Riga_Requisiti("CoreWs 21 Giugno 2019",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO. ")

            Riga_Text("Nuovo Pua:",
                  "WIP")

            Riga_Fine()

            '==================================

            Riga_Data("20 Giugno 2019")

            Riga_Requisiti("Migra '541' :",
                       "Richiesto per modifiche su classe tessitura")

            Riga_Requisiti("GIAS BASE - 07 Giugno 2019 :",
                       "per creaDropDownEditorId")

            Riga_Requisiti("CoreWs - 19 Giugno 2019 : ****** DA CHIUDERE!!!!! ********",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO. ")

            Riga_Text("Nuovo Pua:",
                  "WIP")

            Riga_Fine()

            '==================================

            Riga_Data("18 Giugno 2019")

            Riga_Requisiti("Migra '541' :",
                       "Richiesto per modifiche su classe tessitura")

            Riga_Requisiti("GIAS BASE - 07 Giugno 2019 :",
                       "per creaDropDownEditorId")

            Riga_Requisiti("CoreWs - 13 Giugno 2019 :",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO. ")

            Riga_Text("Nuovo Pua:",
                  "WIP")

            Riga_Fine()

            '==================================

            Riga_Data("17 Giugno 2019")

            Riga_Requisiti("Migra '541' :",
                       "Richiesto per modifiche su classe tessitura")

            Riga_Requisiti("GIAS BASE - 07 Giugno 2019 :",
                       "per creaDropDownEditorId")

            Riga_Requisiti("CoreWs - 13 Giugno 2019 :",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO. ")

            Riga_Text("Nuovo Pua:",
                  "WIP")

            Riga_Fine()

            '==================================

            Riga_Data("13 Giugno 2019")

            Riga_Requisiti("Migra '541' :",
                       "Richiesto per modifiche su classe tessitura")

            Riga_Requisiti("GIAS BASE - 07 Giugno 2019 :",
                       "per creaDropDownEditorId")

            Riga_Requisiti("CoreWs - 13 Giugno 2019 :",
                       "")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO. ")

            Riga_Text("Nuovo Pua:",
                  "WIP")

            Riga_Text("Piano Concimazione Massivo:",
                  "Sistemata aggregazione per analisi")

            Riga_Fine()

            '==================================

            Riga_Data("4 Giugno 2019")

            Riga_Requisiti("Migra '533' :",
                       "")

            Riga_Requisiti("GIAS BASE - 31 Maggio 2019 :",
                       "per gliglia Effluenti Pua2019")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO. ")

            Riga_Text("Nuovo Pua:",
                  "WIP")

            Riga_Text("Piano Concimazione Massivo:",
                 "- corretto baco salvataggio" &
                 "- introdotta nuova modalità piano concimazione massivo singola azienda multi specie con appositi filtri di aggregazione")

            Riga_Fine()

            '==================================


            Riga_Data("29 Maggio 2019")

            Riga_Requisiti("Migra '533' :",
                       "")

            Riga_Requisiti("GIAS BASE - 25 Gennaio 2019 :",
                       "per Versionamento file CSS - Obbligatorio per tutti")

            Riga_Requisiti("WEB.CONFIG:",
                       "Modifiche NO. ")

            Riga_Text("Nuovo Pua:",
                  "WIP")

            Riga_Fine()

            '==================================

            Riga_Data("17 Aprile 2019")

            Riga_Requisiti("Migra '533' :",
                  "")

            Riga_Requisiti("GIAS BASE - 25 Gennaio 2019 :",
                       "per Versionamento file CSS - Obbligatorio per tutti")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PCB_Inserimento:",
                  "introdotta opzione per scegliere se caricare le specie del piano concimazione e quelle del piano colturale")

            Riga_Fine()

            '==================================

            Riga_Data("16 Aprile 2019")

            Riga_Requisiti("Migra '533' :",
                  "")

            Riga_Requisiti("GIAS BASE - 25 Gennaio 2019 :",
                       "per Versionamento file CSS - Obbligatorio per tutti")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PCB_Inserimento:",
                  "introdotta scelta P o P2O5 e K o K2O")

            Riga_Text("Stampa Schede/Bilancio:",
                 "visualizzati parametri salvati P o P2O5 e K o K2O")

            Riga_Fine()

            '==================================

            Riga_Data("10 Aprile 2019")

            Riga_Requisiti("Migra '446' :",
                  "")

            Riga_Requisiti("GIAS BASE - 25 Gennaio 2019 :",
                       "per Versionamento file CSS - Obbligatorio per tutti")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("Stampa Schede/Bilancio:",
                 "- corretta indicazione della fase/ciclo" &
                 "- sistemati i dati del suolo su piu righe" &
                 "- aggiunti dettagli analisi")

            Riga_Fine()

            '==================================

            Riga_Data("11 Marzo 2019")

            Riga_Requisiti("Migra '446' :",
                  "")

            Riga_Requisiti("GIAS BASE - 25 Gennaio 2019 :",
                       "per Versionamento file CSS - Obbligatorio per tutti")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PCB_InserimentoMultiplo:",
                  "modificato ordinamento appezzamenti per sistemare l'aggregazione proposta")

            Riga_Text("Stampa Schede/Bilancio:",
                 "inserita indicazione del centro a cui è stato associato il piano (prima veniva stampato il centro solo nel caso in cui l'azienda aveva un solo centro)")

            Riga_Fine()

            '==================================

            Riga_Data("05 Marzo 2019")

            Riga_Requisiti("Migra '446' :",
                  "")

            Riga_Requisiti("GIAS BASE - 25 Gennaio 2019 :",
                       "per Versionamento file CSS - Obbligatorio per tutti")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PCB_Inserimento:",
                  "Nel Metodo Schede spostato il default del fosforo per giudizio elevato su dotazione normale (prima era su elevata)")

            Riga_Fine()

            '==================================

            Riga_Data("28 Febbraio 2019")

            Riga_Requisiti("Migra '446' :",
                  "")

            Riga_Requisiti("GIAS BASE - 25 Gennaio 2019 :",
                       "per Versionamento file CSS - Obbligatorio per tutti")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PCB_Inserimento:",
                 "- modificato caricamento elenco specie vegetali. vengono lette ora le specie della tabella PC_SpecieConcimazione (svincolato piano colturale)" &
                 "- aggiunta textbox descrizione analisi per poter indicare una descrizione quando si salva l'analisi direttamente in questa pagina")

            Riga_Text("Stampa Schede/Bilancio:",
                 "aggiunte unita di misura nel dettaglio del suolo dove non erano indicate")

            Riga_Fine()

            '==================================

            Riga_Data("22 Febbraio 2019")

            Riga_Requisiti("Migra '446' :",
                  "")

            Riga_Requisiti("GIAS BASE - 25 Gennaio 2019 :",
                       "per Versionamento file CSS - Obbligatorio per tutti")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PCB_Inserimento:",
                 "- introdotta indicazione catasto nella griglia impianti" &
                 "- preselezionati gli impianti in base all'analisi selezionata (se associata ad azienda,centro,campo,appezzamento,impianto,particella)" &
                 "- modificata indicazione del mas se non presente (sostituito il -1 con n.d)")

            Riga_Fine()

            '==================================

            Riga_Data("15 Ottobre 2018")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '446' :",
                  "")

            Riga_Requisiti("GIAS BASE '05 Marzo 2018 ' :",
                "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("Scheda_NPK.aspx:",
                 "Fix baco in caricamento P e K nel report a schede multiple")

            Riga_Fine()

            '==================================

            Riga_Data("25 Luglio 2018")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '446' :",
                  "")

            Riga_Requisiti("GIAS BASE '05 Marzo 2018 ' :",
                "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("Visualizzatore Report:",
                 "nel caso di visualizzazione di un allegato, bypassata lettura impostazioni per evitare sovrascrittura del pathfilepdf")

            Riga_Text("PianoConcimazione_MenuBS:",
                  "apriAllegatoPC: modificata gestione apertura allegati: non usa più la directory virtuale, ma apre il file pdf dal path del file su disco")

            Riga_Fine()

            '==================================

            Riga_Data("17 Aprile 2018")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '446' :",
                  "")

            Riga_Requisiti("GIAS BASE '05 Marzo 2018 ' :",
                "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("Visualizzatore Report:",
                 "Gestita opzione per mostrare direttamente il PDF sul browser")

            Riga_Text("Scheda_NPK:",
                  "corretta stampa metodo schede (stampava sempre dose standard)")

            Riga_Text("PCB_Inserimento:",
                 "corretto baco in modifica (visualizzava sempre dose standard poiché non salvava dose elevata, bassa etc.)")

            Riga_Fine()

            '==================================

            Riga_Data("09 Marzo 2018")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '446' :",
                  "")

            Riga_Requisiti("GIAS BASE '05 Marzo 2018 ' :",
                "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("Visualizzatore Report:",
                 "Gestita opzione per mostrare direttamente il PDF sul browser")

            Riga_Text("Master, PCB_Inserimento, PCB_InserimentoMultiplo, Filtro_StampaScadenza, PianoConcimazione_MenuBS:",
                 "generalizzata master con agronicacontrolli e modificate pagine di conseguenza")

            Riga_Text("PianoConcimazione_MenuBS:",
                 "corretto baco su redirect in accesso al filtrino")

            Riga_Text("PianoConcimazione_MenuBS:",
                 "corretto baco per cui se si faceva indietro sul filtrone poi erano visibili tutte le imprese nel menu")

            Riga_Fine()

            '==================================

            Riga_Data("08 Gennaio 2018")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PCB_Inserimento:",
                 "Corretto baco per cui veniva incluso il terreno nudo tra le specie")


            Riga_Fine()

            '==================================

            Riga_Data("06 Luglio 2017")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PianoConcimazione_MenuBS:",
                 "sistemato indietro per il menu' agenda")

            Riga_Text("Scheda_NPK:",
                  "sistemato baco su specie vegetale non inserita")

            Riga_Text("Scheda_NPK:",
                  "sistemato baco su decremento o incremento non inserito nelle schede")

            Riga_Fine()
            '==================================

            Riga_Data("22 Maggio 2017")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '446' :",
                  "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PCB_Inserimento.aspx:",
                 "tabelle bilancio allineate, associa solo dopo salva")

            Riga_Fine()

            '==================================
            Riga_Data("18 Maggio 2017")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '446' :",
                  "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PCB_Inserimento.aspx:",
                 "inserito bottone con iframe per le analisi + inserita gestione centro az. + centroaz piogge + camponome in griglia imp. + analisi + salvataggio realtime")

            Riga_Text("Master",
                  "Inserito scriptmanager e iframe per la modale generica")

            Riga_Fine()

            '==================================

            Riga_Data("08 Maggio 2017")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PianoConcimazione_MenuBS:",
                 "corretto baco su copertura cod non valorizzato nell'impianto nel passaggio al Piano Distrib.")

            Riga_Text("PianoConcimazione:",
                  "aggiunta in objAgroWebConfig chiave GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione (settabile ora anche su configurazione_siti del super_server)")

            Riga_Fine()

            '==================================

            Riga_Data("28 Aprile 2017")

            Riga_Requisiti("Migra '439' :",
                  "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PCB_Inserimento Metodo Schede:",
                  "Sistemata visualizzazione modifica piani allevamento e pre impianto.")

            Riga_Text("Stampe Piani:",
                  "modificato caricamento.")

            Riga_Fine()

            '==================================

            Riga_Data("27 Aprile 2017")

            Riga_Requisiti("Migra '439' :",
                  "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("file vari:",
                 "inserita versione corrente dei JS")

            Riga_Text("PCB_Inserimento e PCB_InserimentoMultiplo Metodo Schede:",
                  "- Corretto baco che andava in errore se alcuni fattori correttivi non erano presenti." &
                  "- Sistemato calcolo dosi pre impianto (= 0)." &
                  "- Evidenziato valore dose ricalcolata.")

            Riga_Text("PCB_Inserimento e PCB_InserimentoMultiplo:",
                  "modificata lettura resa. Nel singolo viene letta ora anche al cambio della finalità.")

            Riga_Fine()

            '==================================

            Riga_Data("12 Aprile 2017")

            Riga_Requisiti("Migra '439' :",
                  "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PianoConcimazione_MenuBS:",
                 "Aggiunte colonne e sistemato lo style (+ letture core)")

            Riga_Fine()

            '==================================

            Riga_Data("4 Aprile 2017")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '439' :",
                  "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("Scheda_NPK.vb:",
                 "sistemato baco nessuna specie")

            Riga_Text("PCB_Inserimento e PCB_InserimentoMultiplo:",
                  "- Corretto baco calcolo dose standard K nel metodo schede." &
                  "- Introdotta verifica eventuale correzione Potassio nei due metodi in base a Mg e CSC per Piano 2017.")

            Riga_Fine()

            '==================================

            Riga_Data("03 Aprile 2017")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("Scheda_NPK.vb:",
                 "sistemato baco nessuna specie")

            Riga_Text("PCB_InserimentoMultiplo.aspx:",
                 "sistemato baco salvataggio")

            Riga_Fine()

            '==================================

            Riga_Data("31 Marzo 2017")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '439' :",
                  "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PianoConcimazione_MenuBS:",
                 "allineata la pagina con le icone")

            Riga_Fine()

            '==================================
            Riga_Data("30 Marzo 2017")

            Riga_Requisiti("Siti 'AgronicaAgenda_2010' :",
                  "Aggiornata almeno al 30/03/2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PCB_Inserimento.aspx:",
                 "Sistemato in lettura e controlliWS")

            Riga_Text("PCB_InserimentoMultiplo.aspx:",
                 "controlliWS")

            Riga_Text("PianoConcimazione_MenuBS.aspx:",
                 "Intervallo temporale preimpostato sulle impostazioni utente, tasto cerca e altro")

            Riga_Text("PianoConcimazione.Master:",
                 "Aggiunto pulsante cerca")

            Riga_Fine()

            '==================================

            Riga_Data("22 Marzo 2017")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '436' :",
                  "Per gestione allegati")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PianoConcimazione_MenuBS.aspx:",
                 "Inserita la Gestione degli allegati")

            Riga_Text("Scheda_NPK.aspx:",
                 "Inserita la Gestione degli allegati singola")

            Riga_Text("Bilancio_NPK.aspx:",
                 "Inserita la Gestione degli allegati singola")

            Riga_Text("PCB_InserimentoMultiplo.aspx:",
                 "Inserita la Gestione degli allegati massivo")

            Riga_Fine()

            '==================================

            Riga_Data("17 Marzo 2017")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PianoConcimazione_MenuBS.aspx:",
                 "Sistemata la pagina di menu con filtro delle date ")

            Riga_Fine()

            '==================================

            Riga_Data("16 Marzo 2017")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '434' :",
                  "Per prima versione")

            Riga_Requisiti("WEB.CONFIG:",
                    "Modifiche NO. ")

            Riga_Text("PianoConcimazione_2017:",
                 "Prima versione")

            Riga_Fine()

            '==================================



            '==================================


            '##############################################
            '##############################################
            '##############################################

        End Sub


















        '#################################################################################################
        '#################################################################################################
        '#################################################################################################
        '#################################################################################################
        '#################################################################################################
        '#################################################################################################
        '#################################################################################################
        '#################################################################################################
        '#################################################################################################
        '#################################################################################################
        '#################################################################################################
        '#################################################################################################
        '#################################################################################################
        '#################################################################################################



        '#################################################################################################
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            '
            Me.TabellaVersione.Rows.Clear()
            '
        End Sub



        '#################################################################################################
        Private Sub ImgBtn_Codice_Ins_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Codice_Ins.Click

            Dim ChiaveUtente As String
            Dim ChiaveSistema As String

            If Not IsNothing(ConfigurationManager.AppSettings("Versione_Pwd")) Then
                ChiaveSistema = ConfigurationManager.AppSettings("Versione_Pwd").ToString
            Else
                ChiaveSistema = "ascolipiceno"
            End If

            ChiaveUtente = Me.Txt_ChiaveAccesso.Text

            If ChiaveUtente = "" Then
                'AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Non sono ammessi valori nulli per la PASSWORD !!!", Page)
                Exit Sub
            End If

            If ChiaveUtente <> ChiaveSistema Then
                Me.Txt_ChiaveAccesso.Text = ""
                'AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Password errata !!!", Page)
                Exit Sub
            End If

            Me.Pannello_Chiave.Visible = True
            Call Carica_Pannello()
            Me.Pannello_Versione.Visible = True

        End Sub


        '#################################################################################################
        Private Sub Riga_Versione(ByVal data As String, Optional ByVal versione As String = "")

            If _objChangeSito IsNot Nothing Then
                _objChangeSito.versioni.Add(New Versione_Obj(data, versione))
            End If

        End Sub

        '#################################################################################################
        <Obsolete("Usare la funzione Riga_Versione()")>
        Private Sub Riga_Data(ByVal DataAggiornamento As String)

            If _objChangeSito Is Nothing Then
                Dim Riga As New HtmlTableRow

                Riga.Cells.Add(New HtmlTableCell)

                Riga.Cells(0).BgColor = "#00bfff"
                Riga.Cells(0).Height = "25px"
                Riga.Cells(0).Attributes.Add("class", "Testo_08_Nero_Bold")
                Riga.Cells(0).InnerText = DataAggiornamento

                Me.TabellaVersione.Rows.Add(Riga)
            Else
                'Inizializzo tutto qui, visto che sono certa che sia il primo elemento del blocco
                _objChangeSito.versioni.Add(New Versione_Obj(DataAggiornamento))
            End If

        End Sub

        '#################################################################################################
        Private Sub Riga_Requisiti(ByVal Titolo As String, ByVal Testo As String, Optional ByVal Ver As String = "")

            If _objChangeSito Is Nothing Then
                Dim Riga As New HtmlTableRow

                Riga.Cells.Add(New HtmlTableCell)

                Riga.Cells(0).BgColor = "#FFFFC0"
                Riga.Cells(0).Height = "20px"
                Riga.Cells(0).Attributes.Add("class", "Testo_08_Rosso")

                Riga.Cells(0).InnerHtml = "<b>" & Titolo & If(Not String.IsNullOrEmpty(Ver), " [" & Ver & "]", "") & "</b><br>" & Testo

                Me.TabellaVersione.Rows.Add(Riga)
            Else
                _objChangeSito.versioni.Last().requisiti.Add(New Requisito_Obj(Titolo, Testo, Ver))
            End If

        End Sub



        '#################################################################################################
        Private Sub Riga_Changelog(ByVal tipoChangelog As enum_Tipo_Changelog,
                                   ByVal area As String,
                                   ByVal descrizione As String,
                                   ByVal cliente As String,
                                   ByVal idTicketAssistenza As Integer,
                                   ByVal idTicketSviluppo As Integer,
                                   ByVal idTicketTesting As Integer,
                                   ByVal autore As String,
                                   Optional ByVal noteTecniche As String = "",
                                   Optional ByVal noteTest As String = ""
                                   )

            If _objChangeSito IsNot Nothing Then
                _objChangeSito.Riga_Changelog(tipoChangelog,
                                              area, descrizione,
                                              cliente,
                                              idTicketAssistenza, idTicketSviluppo, idTicketTesting,
                                              autore,
                                              noteTecniche, noteTest)
            End If

        End Sub

        '#################################################################################################
        <Obsolete("Usare la Funzione Riga_Changelog() con primo parametro = enum_Tipo_Changelog.Feature")>
        Private Sub Riga_Text(ByVal Titolo As String, ByVal Testo As String,
                          Optional ByVal cliente As String = "",
                          Optional ByVal idPerforma As Integer = 0,
                          Optional ByVal noteTecniche As String = "",
                          Optional ByVal noteTest As String = ""
                          )

            If _objChangeSito Is Nothing Then
                Dim Riga As New HtmlTableRow

                Riga.Cells.Add(New HtmlTableCell)

                Riga.Cells(0).BgColor = "#c0ffc0"
                Riga.Cells(0).Height = "20px"
                Riga.Cells(0).Attributes.Add("class", "Testo_08_Nero")

                Riga.Cells(0).InnerHtml = "<b>" & Titolo & "</b><br>" & Testo

                Me.TabellaVersione.Rows.Add(Riga)
            Else
                _objChangeSito.versioni.Last().changelog.feature.Add(New Feature_Obj(True, Titolo, Testo, cliente, idPerforma, noteTecniche, noteTest))
            End If

        End Sub

        '#################################################################################################
        <Obsolete("Usare la Funzione Riga_Changelog() con primo parametro = enum_Tipo_Changelog.Bug")>
        Private Sub Riga_Bug(ByVal Titolo As String, ByVal Testo As String,
                         Optional ByVal cliente As String = "",
                         Optional ByVal idPerforma As Integer = 0,
                         Optional ByVal noteTecniche As String = "",
                         Optional ByVal noteTest As String = ""
                         )

            If _objChangeSito Is Nothing Then
                Dim Riga As New HtmlTableRow

                Riga.Cells.Add(New HtmlTableCell)

                Riga.Cells(0).BgColor = "#c0ffc0"
                Riga.Cells(0).Height = "20px"
                Riga.Cells(0).Attributes.Add("class", "Testo_08_Nero")

                Riga.Cells(0).InnerHtml = "<b>" & Titolo & "</b><br>" & Testo

                Me.TabellaVersione.Rows.Add(Riga)
            Else
                _objChangeSito.versioni.Last().changelog.bugfix.Add(New Bugfix_Obj(True, Titolo, Testo, cliente, idPerforma, noteTecniche, noteTest))
            End If

        End Sub



        '#################################################################################################
        Private Sub Riga_Fine()

            If _objChangeSito Is Nothing Then
                Dim Riga As New HtmlTableRow

                Riga.Cells.Add(New HtmlTableCell)

                Riga.Cells(0).BgColor = "whitesmoke"
                Riga.Cells(0).Height = "20px"
                Riga.Cells(0).Attributes.Add("class", "Testo_08_Nero")

                Riga.Cells(0).InnerHtml = "&nbsp;"

                Me.TabellaVersione.Rows.Add(Riga)
            End If

        End Sub

    End Class
End Namespace
