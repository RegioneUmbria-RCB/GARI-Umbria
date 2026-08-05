Imports System.Configuration
Imports System.IO
Imports System.Security.Cryptography
Imports System.Web.UI.HtmlControls
Imports AgronicaCoreUtilityVersioni
Imports AgronicaCoreUtilityVersioni.Enumerativi

Namespace DomandaIrriguaVersione
    Public Class Versione
        Inherits System.Web.UI.Page
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

                Dim fileName As String = Replace(_objChangeSito.progetto, " ", "") & ".json"

                'Chiamo il Versione.aspx
                Carica_Pannello()

                pathFileJson = Path.Combine(dirOutput, fileName)
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

        Private Sub Carica_Pannello()

            Riga_Versione("08 Giugno 2026", "151.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Tutte",
                           "Estrazione e visualizzazione della Partita Iva Reale",
                           cliente:="Tutti",
                           idTicketAssistenza:=0, idTicketSviluppo:=214395, idTicketTesting:=0,
                           autore:=DEV_Casa, noteTest:="", noteTecniche:="")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("26 Maggio 2026", "150.4.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Porting pagine vecchie qdc",
                           "L'installazione trappole non si apriva per sovrapposizione di enum pagina con 'Calcolo Tariffazione'",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=217557,
                           autore:="Giulia Bottan",
                           noteTecniche:="L'enum della pagina del calcolo tariffazione è stato spostato da '4' a '400' e cambiata la config del relativo pulsante di menù",
                           noteTest:="")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                "Tutte",
                "Estrazione e visualizzazione della Partita Iva Reale",
                cliente:="Coldiretti",
                idTicketAssistenza:=0, idTicketSviluppo:=214560, idTicketTesting:=0,
                autore:="Cecalupo Marco",
                noteTest:="",
                noteTecniche:="")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("22 Maggio 2026", "150.4.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Fix domanda irrigua",
                           " - Corretto redirect alla pagine domanda irrigua originali
                                       - Corretto caricamento waitform",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=217448,
                           autore:=DEV_Uhalid,
                           noteTest:="")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Redirezione a Custom500 errata",
                           "Corretta redirezione alla pagina custom 500 in caso di errore al passaggio tra siti",
                           cliente:="TUTTI",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           noteTest:="Non testabile in autonomia",
                           noteTecniche:="",
                           autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("08 Maggio 2026", "150.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Fix Porting pagine Quaderno di Campagna",
                           " - immagine non caricate nelle operazione che hanno avuto il porting
                           - Sistemato redirect a op. trattamenti antiparassitari non funzionante.",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=214602,
                           autore:=DEV_Uhalid,
                           noteTest:="Nello stesso ciclo di sviluppo e' stato rilasciato anche il rifacimento delle operazione installazione trappole e reinnesco in angular, per testare le vecchie operazione, bisogna passare dal quaderno di campagna vecchio (agenda, per accederci bisogna avere ""MenuAgendaNG"" a false). Per il resto delle operazione rimane da settare il flag ""OperazioniAgendaNG"" a false.")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("27 Aprile 2026", "150.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Porting pagine Quaderno di Campagna",
                           "- Portate le seguenti pagine da AgroAgenda a DomandaIrrigua:" &
                           "  Installazione_Trappole, Trattamenti_2, Reinnesco_Rilievi_Trappole," &
                           "  RilieviBS, Raccolta, Trattamenti_PostRaccolta;",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=206553, idTicketTesting:=0,
                           autore:=DEV_Uhalid, noteTest:="Per testare le pagine, tranne la installazione trappola e reinnesco trappole, bisogna settare a false la chiave ""OperazioniAgendaNG"" su Conf siti.
                           Per testare le varie pagine ho usato le seguenti operazioni:
                           - Installazione_Trappole.aspx - Installazione Trappole
                           - Reinnesco_Rilievi_Trappole.aspx - Reinnesco Trappole
                           - RilieviBS.aspx - Rilievo avversita' in campo
                           - Raccolta.aspx - Raccolta
                           - Trattamenti_PostRaccolta.aspx - Trattamento Post Raccolta
                           - Trattamenti_2.aspx - Trattamento Antiparassitario
                           ")

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

            '------------------------------------------------------------------------------------------------

            Riga_Versione("01 Dicembre 2025", "145.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Changelog(enum_Tipo_Changelog.Security,
                           "Librerie terze parti",
                           "- Eliminate librerie html5shiv, respond e ie-emulation-modes-warning (non usate)",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Giulia Bottan")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("07 Novembre 2025", "144.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Changelog(enum_Tipo_Changelog.Performance,
               "Gestione Cache Permessi e Impostazioni utente",
               "- Gestita la cache per Permessi e impostazioni utente;",
               cliente:="",
               idTicketAssistenza:=0, idTicketSviluppo:=195312, idTicketTesting:=0,
               autore:=DEV_Drudi)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("03 Novembre 2025", "144.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Changelog(enum_Tipo_Changelog.Performance,
                           "Aggiunta TryCatch e nuovi LivelliLog",
                           "- Aggiunti livelli di log: solo mail, file + mail, file + db;" &
                           "- Aggiunti blocchi TryCatch alla scrittura su file e db del log per evitare di lasciare thread zombie in giro in caso di errore",
                           cliente:="",
                           idTicketAssistenza:=196225, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("17 Ottobre 2025", "143.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

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

            '------------------------------------------------------------------------------------------------

            Riga_Versione("08 Settembre 2025", "142.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Changelog(enum_Tipo_Changelog.Security,
                           "Controllo massivo SQL Injection",
                           "Verificati molti casi di possibili sql injection",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=0, idTicketSviluppo:=179500, idTicketTesting:=0,
                           autore:=DEV_Casa, noteTest:="", noteTecniche:="")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("25 Luglio 2025", "140.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Changelog(enum_Tipo_Changelog.Security,
                            "SQL_Dataprovider LanciaEccezioneSuInjection",
                            "Aggiunta lettura da DB del parametro LanciaEccezioneSuInjection",
                            cliente:="Coldiretti",
                            idTicketAssistenza:=184216, idTicketSviluppo:=0, idTicketTesting:=0,
                            autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("17 Giugno 2025", "139.1.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Changelog(enum_Tipo_Changelog.Security,
                           "Librerie di terze parti",
                           "Rimozione di file di librerie di terze parti che erano state incluse ma mai usate",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Giulia Bottan",
                           noteTest:="",
                           noteTecniche:=""
                           )

            '------------------------------------------------------------------------------------------------

            Riga_Versione("09 Giugno 2025", "139.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

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
                           "Per motivi di performance, è stato tolto il controllo sulla versione di sql quando si utilizzano le sequenze, comanda il valore del flag nel file appsettings",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=163175, idTicketTesting:=0,
                           autore:="Paolo Netso",
                           noteTest:="Non testabile",
                           noteTecniche:=""
                           )

            '------------------------------------------------------------------------------------------------

            Riga_Versione("12 Maggio 2025", "138.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                "Invio Log Elastic Search - LOG APPLICATIVI (DataProvider)",
                "Rimosso possibile loop infinito in caso di errore di invio a ElasticSearch",
                cliente:="",
                idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Funcy
                )

            '------------------------------------------------------------------------------------------------

            Riga_Versione("17 Marzo 2025", "136.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Changelog(enum_Tipo_Changelog.Security, "Logout",
               "Migliorata gestione per la sicurezza del redirect di logout e di sessione scaduta",
               "Credit Agricole", 0, 162774, 0, "Gianluca Amoroso", "",
               "Il redirect deve continuare a funzionare come prima, riportando alla pagina di login. Stessa modifica dettagliata in sito dell'Agenda")

            '------------------------------------------------------------------------------------------------

            Riga_Data("31 Gennaio 2025")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("Security - AgronicaCoreParametri",
                     "Fix per recuperare nome db anche in caso di stringa connessione codificata")

            '------------------------------------------------------------------------------------------------

            Riga_Data("20 Gennaio 2025")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Autenticazione",
                      "Prima di ogni chiamata ai web method è stato introdotto un controllo di sicurezza (global asax)",
                            noteTest:="Verificare che una pagina qualsiasi di questo sito funzioni correttamente",
                            noteTecniche:="Il controllo di sicurezza viene effettuato solo quando il valore della chiave ControlloAutenticazioneConAuthCookie nei appsettings è uguale a true")

            '------------------------------------------------------------------------------------------------

            Riga_Data("16 Dicembre 2024")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Security",
                      "Interventi vari per evitare il passaggio della stringa di connessione al db (8507)")

            '------------------------------------------------------------------------------------------------

            Riga_Data("12 Dicembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           Ver:="776")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("Sequence",
                     "Alla creazione delle Sequence startValue = 1",
                     noteTest:="")

            '------------------------------------------------------------------------------------------------

            Riga_Data("06 Dicembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           Ver:="776")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("Web Config",
                     "Corretta dipendenza libreria System.Memory",
                     noteTest:="Non testabile")

            '------------------------------------------------------------------------------------------------

            Riga_Data("08 Novembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           Ver:="776")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Codifica e decodifica password_smtp e PasswordArteaWS",
                      "-Introdotti metodi per la codifica e decodifica usando la classe AES
                            -Introdotta chiave cr2 in web.config o app.config")

            Riga_Text("Gestione sequence default",
  "Gestione sequence per progressivi chiavi tabelle di default attive per tutti. 
    Disattivabili esclusivamente impostando a False la chiave Allow_Sql_Sequence nell'appsettings")

            Riga_Text("Security - Query",
                      "Parametrizzate massivamente molte query in filtri aggiuntivi, order by e clausole IN per impedire sql injection")

            '------------------------------------------------------------------------------------------------

            Riga_Data("11 Ottobre 2024")

            Riga_Requisiti("Migra",
                           "Creazione tabelle iniziali", "735")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaDomandaIrrigua", "124")

            Riga_Requisiti("CoreWS",
                           "Per modifiche legate alla codifica e decodifica semplice delle stringhe", "17/09/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Security - XSS Detection",
                      "Aggiunto controllo per bloccare script injection nelle chiamate ai web method")

            '------------------------------------------------------------------------------------------------

            Riga_Data("17 Settembre 2024")

            Riga_Requisiti("Migra",
                           "Creazione tabelle iniziali", "735")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaDomandaIrrigua", "124")

            Riga_Requisiti("CoreWS",
                           "Per modifiche legate alla codifica e decodifica semplice delle stringhe", "17/09/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Codifica e decodifica stringhe",
                      "Introdotta la possibilità di effettuare una codifica semplice delle stringhe",
                      noteTecniche:="Per poter abilitare la codifica semplice la proprietà UsaCodificaSemplice in appsettings deve essere impostata a true")

            '------------------------------------------------------------------------------------------------

            Riga_Data("13 Settembre 2024")

            Riga_Requisiti("Migra",
                           "Creazione tabelle iniziali", "735")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaDomandaIrrigua", "124")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("DataProvider - Log Agro_Sequenze",
                      "Modifica Scrivi_Log per loggare anche NomeTabella in caso di exception",
                      noteTest:="Non testabile da assistenza")

            Riga_Text("Security - SQL Dataprovider",
                      " - Implementato utilizzo dei parametri SQL nella maggior parte dei punti in cui non erano utilizzati",
                      noteTecniche:="Usate sempre Agro_SQL_Save_Clausola_IN e Agro_SQL_Save_xFiltroAggiuntivo nella composizione di query in cui compaiono parametri inseriti da interfaccia",
                      noteTest:="Effettuare giri generici sulla pagina cercando di utilizzare eventuali filtri disponibili nel modo più specifico possibile")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("17 Luglio 2024")

            Riga_Requisiti("Migra",
                           "Creazione tabelle iniziali", "735")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaDomandaIrrigua", "124")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("SQL Sequence",
                      "Aggiunto parametro attivazione in appsettings")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("15 Luglio 2024")

            Riga_Requisiti("Migra",
                           "Creazione tabelle iniziali", "735")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaDomandaIrrigua", "124")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("SQL Sequence",
                      "nuova gestione id tabella tramite sequence per alcune tabelle", "", 0, "
                      Queste le tabelle coinvolte:
                      	Agenda
	                    Movimenti
	                    Movimenti_dettagli
	                    Movimenti_dettagli_tecnici
	                    Movimenti_dettagli_tecnici_extra
	                    Idtestatatemp
	                    Ricette
	                    Ricette_operazioni
	                    Ricette_dettaglio_tecnico
	                    Ricette_dettagli
	                    Ricette_destinazioni
                        Raccoglitore,
                        gis_entita,
                        gis_elementigrafici,
                        impresa_progetto,
                        Agronica_Log_Invio_Chiamate
                      ", "")

            Riga_Text("SQL Sequence",
                      "esclusa gestione sequence dai database 2008", "", 0, "", "")

            Riga_Text("ISOLATION LEVEL READ UNCOMMITTED",
                      "ISOLATION LEVEL READ UNCOMMITTED in tutte le query che non sono dentro una transazione", "", 0,
                      "Disabilitabile tramite la chiave nell'appsettings: : &lt;add key=""ReadUncommittedDefault"" value=""False""/>.",
                      "")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("29 Settembre 2023")

            Riga_Requisiti("Migra",
                           "Creazione tabelle iniziali", "735")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaDomandaIrrigua", "124")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Restyle Grafico",
                     "- Ri-organizzazione file css in multipli file separati <code>styleGiasComponents.css, styleGiasIcons.css, styleGiasPages.css, styleGiasUtils.css</code> anzichè l'unico file <code>styleXonneTables.css</code>")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("29 Agosto 2023")

            Riga_Requisiti("Migra",
                           "Creazione tabelle iniziali", "735")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaDomandaIrrigua", "124")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("DomandaIrrigua",
                      "Fix query lettura pianocolturaleconcatasto in caso di pc senza catasto", "MIDAR", 0,
                     "se ho un pc valido nell'anno ma senza catasto associato deve escludere le occorrenze di pc senza catasto",
                     "entrare nella funzione di domanda irrigua, per un'azienda che non ha domanda preesistente non deve dare errore")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("24 Agosto 2023")

            Riga_Requisiti("Migra",
                           "Creazione tabelle iniziali", "735")

            Riga_Requisiti("Aggancio",
                           "per compatibilità", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaDomandaIrrigua", "124")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Baseline",
                      "Primo rilascio")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.TabellaVersione.Rows.Clear()
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


        Protected Sub Btn_Codice_Ins_Click(sender As Object, e As EventArgs) Handles Btn_Codice_Ins.Click
            Dim ChiaveUtente As String
            Dim ChiaveSistema As String

            If Not IsNothing(ConfigurationManager.AppSettings("Versione_Pwd")) Then
                ChiaveSistema = ConfigurationManager.AppSettings("Versione_Pwd").ToString
                ChiaveSistema = Stringa_Decodifica(ChiaveSistema, False)
            Else
                ChiaveSistema = "ascolipiceno"
            End If

            ChiaveUtente = Me.Txt_ChiaveAccesso.Text

            If ChiaveUtente = "" Then
                'AgroMsgBox("Non sono ammessi valori nulli per la PASSWORD !!!", Page)
                Exit Sub
            End If

            If ChiaveUtente <> ChiaveSistema Then
                Me.Txt_ChiaveAccesso.Text = ""
                'AgroMsgBox("Password errata !!!", Page)
                Exit Sub
            End If

            Me.Pannello_Chiave.Visible = True
            Call Carica_Pannello()
            Me.Pannello_Versione.Visible = True
        End Sub



        '#####################################################################
        Private Key() As Byte = {12, 52, 74, 32, 33, 36, 23, 48, 14, 50,
                                 52, 112, 60, 14, 135, 116, 84, 149,
                                 81, 200, 211, 29, 65, 35}

        Private Iv() As Byte = {12, 36, 37, 97, 106, 56, 76, 18, 99, 107,
                                21, 123, 65, 114, 159, 196, 179,
                                198, 192, 241, 212, 123, 0, 54}

        '#####################################################################
        Public Function Stringa_Decodifica(ByVal StringaCriptata As String,
                                           ByVal FormattaURL As Boolean
                                           ) As String

            Dim StringaCriptataBis As String
            Dim cryptoProvider As New TripleDESCryptoServiceProvider

            '----- Decrypt di una stringa
            If FormattaURL = True Then
                StringaCriptataBis = Replace(StringaCriptata, "^", "+")
            Else
                StringaCriptataBis = StringaCriptata
            End If

            'Converto la stringa con formato XML in un array di byte
            Dim buffer As Byte() = Convert.FromBase64String(StringaCriptataBis)
            Dim ms As New MemoryStream(buffer)
            Dim cs As New CryptoStream(ms,
                                       cryptoProvider.CreateDecryptor(Key, Iv),
                                       CryptoStreamMode.Read)

            Dim sr As New StreamReader(cs)
            Return sr.ReadToEnd()

        End Function

    End Class

End Namespace