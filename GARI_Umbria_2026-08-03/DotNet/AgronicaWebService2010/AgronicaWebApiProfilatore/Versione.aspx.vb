Imports System.IO
Imports AgronicaCoreUtilityVersioni
Imports AgronicaCoreUtilityVersioni.Enumerativi


Namespace WebApiProfilatoreVersione

Public Class Versione
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Carica_Pannello()
    End Sub

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
            pathFileJson= ""
            Throw New Exception(ex.Message)
        End Try

        Return pathFileJson
    End Function

#End Region


    '#################################################################################################
    Private Sub Carica_Pannello()

        '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
        '                     CHI COMPILA
        '           SI PREOCCUPI DI VERIFICARE COSA SERVE:
        '                       NEI COM,
        '                       NEL MIGRA,
        '                       NEL WEB.CONFIG
        '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

        '==================================

        Riga_Versione("08 Giugno 2026", "151.0.0")

        Riga_Requisiti("Migra",
           "Nuova colonna Priorita in SistemiEsterni_RicezioneNotifiche",
           Ver:="788")

        Riga_Requisiti("Configurazione_Siti",
           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

        Riga_Requisiti("CoreWS",
           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

        Riga_Requisiti("GIAS BASE",
           "Nuova versione 2022", "22/12/2022")

        Riga_Changelog(enum_Tipo_Changelog.Feature,
                       "Tutte",
                       "Estrazione e visualizzazione della Partita Iva Reale",
                       cliente:="Tutti",
                       idTicketAssistenza:=0, idTicketSviluppo:=214395, idTicketTesting:=0,
                       autore:=DEV_Casa, noteTest:="", noteTecniche:="")

        '==================================

        Riga_Versione("22 Maggio 2026", "150.4.0")

        Riga_Requisiti("Migra",
               "Nuova colonna Priorita in SistemiEsterni_RicezioneNotifiche",
               Ver:="788")

        Riga_Requisiti("Configurazione_Siti",
               "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

        Riga_Requisiti("CoreWS",
               "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

        Riga_Requisiti("GIAS BASE",
               "Nuova versione 2022", "22/12/2022")

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
                   "Nuova colonna Priorita in SistemiEsterni_RicezioneNotifiche",
                   Ver:="788")

        Riga_Requisiti("Configurazione_Siti",
                   "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

        Riga_Requisiti("CoreWS",
                   "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

        Riga_Requisiti("GIAS BASE",
                   "Nuova versione 2022", "22/12/2022")

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

        '==================================

        Riga_Versione("01 Dicembre 2025", "145.0.0")

        Riga_Requisiti("Migra",
                   "Nuova colonna Priorita in SistemiEsterni_RicezioneNotifiche",
                   Ver:="788")

        Riga_Requisiti("Configurazione_Siti",
                   "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

        Riga_Requisiti("CoreWS",
                   "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

        Riga_Requisiti("GIAS BASE",
                   "Nuova versione 2022", "22/12/2022")

        Riga_Changelog(enum_Tipo_Changelog.Security,
                       "Reimposta password Profitosan",
                       "Hotspot: Hard-coded secrets are security-sensitive [vbnet:S6418]",
                       cliente:="",
                       idTicketAssistenza:=0, idTicketSviluppo:=197181, idTicketTesting:=0,
                       autore:="Giulia Bottan",
                       noteTecniche:="Spostato il token per il profitosan su appsettings.config")

        '==================================

        Riga_Versione("03 Novembre 2025", "144.0.0")

        Riga_Requisiti("Migra",
                   "Nuova colonna Priorita in SistemiEsterni_RicezioneNotifiche",
                   Ver:="788")

        Riga_Requisiti("Configurazione_Siti",
                   "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

        Riga_Requisiti("CoreWS",
                   "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

        Riga_Requisiti("GIAS BASE",
                   "Nuova versione 2022", "22/12/2022")

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
                   "Nuova colonna Priorita in SistemiEsterni_RicezioneNotifiche",
                   Ver:="788")

        Riga_Requisiti("Configurazione_Siti",
                   "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

        Riga_Requisiti("CoreWS",
                   "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

        Riga_Requisiti("GIAS BASE",
                   "Nuova versione 2022", "22/12/2022")

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

        Riga_Versione("08 Settembre 2025", "142.0.0")

        Riga_Requisiti("Migra",
                       "Nuova colonna Priorita in SistemiEsterni_RicezioneNotifiche",
                       Ver:="788")

        Riga_Requisiti("Configurazione_Siti",
                       "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

        Riga_Requisiti("CoreWS",
                       "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

        Riga_Requisiti("GIAS BASE",
                       "Nuova versione 2022", "22/12/2022")

        Riga_Changelog(enum_Tipo_Changelog.Security,
                       "Controllo massivo SQL Injection",
                       "Verificati molti casi di possibili sql injection",
                       cliente:="Coldiretti",
                       idTicketAssistenza:=0, idTicketSviluppo:=179500, idTicketTesting:=0,
                       autore:=DEV_Casa, noteTest:="", noteTecniche:="")

        '==================================

            Riga_Versione("25 Luglio 2025", "140.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Priorita in SistemiEsterni_RicezioneNotifiche",
                           Ver:="788")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Changelog(enum_Tipo_Changelog.Security,
                            "SQL_Dataprovider LanciaEccezioneSuInjection",
                            "Aggiunta lettura da DB del parametro LanciaEccezioneSuInjection",
                            cliente:="Coldiretti",
                            idTicketAssistenza:=184216, idTicketSviluppo:=0, idTicketTesting:=0,
                            autore:=DEV_Casa)

            '==================================

            Riga_Versione("09 Giugno 2025", "139.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Priorita in SistemiEsterni_RicezioneNotifiche",
                           Ver:="788")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

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

            Riga_Versione("27 Maggio 2025", "138.2.2")

            Riga_Requisiti("Migra",
                           "Nuova colonna Priorita in SistemiEsterni_RicezioneNotifiche",
                           Ver:="788")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Changelog(
                enum_Tipo_Changelog.Feature,
                "Autenticazione Demetra",
                "Aggiunta gestione web service persone fisiche in creazione utente azienda agricola",
                cliente:="Coldiretti",
                idTicketAssistenza:=0, idTicketSviluppo:=176169, idTicketTesting:=0,
                autore:="Lorenzo Lavezzo"
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

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                "Invio Log Elastic Search - LOG APPLICATIVI (DataProvider)",
                "Rimosso possibile loop infinito in caso di errore di invio a ElasticSearch",
                cliente:="",
                idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Funcy
                )

            '==================================

            Riga_Versione("18 Aprile 2025", "137.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Changelog(
                enum_Tipo_Changelog.Feature,
                "ProfilatoreUtenze",
                "Aggiunto metodo di autenticazione per NewAgri",
                cliente:="Regione Umbria",
                idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:="Lorenzo Lavezzo",
                noteTest:="non testabile",
                noteTecniche:=""
                )

            '==================================

            Riga_Data("20 Gennaio 2025")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("SQL DataProvider parametrizzatore order by",
                            "Ora la presenza di una clausola OFFSET all'interno di un Order By è contemplato",
                            noteTest:="Non testabile")

            '==================================

            Riga_Data("07 Gennaio 2025")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Sicurezza",
                     "Fix decodifica utente e password connessione nei siti con autenticazione token bearer")

            '==================================

            Riga_Data("16 Dicembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           Ver:="776")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Web Config",
                     "Corretta dipendenza libreria System.Memory",
                     noteTest:="Non testabile")

            Riga_Text("Web Config",
                     "Portata sezione bindings su file esterno bindings.config",
                     noteTest:="Non testabile")

            '==================================

            Riga_Data("12 Dicembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           Ver:="776")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Sequence",
                     "Alla creazione delle Sequence startValue = 1",
                     noteTest:="")

            '==================================

            Riga_Data("08 Novembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           Ver:="776")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("Codifica e decodifica password_smtp e PasswordArteaWS",
                      "- Introdotti metodi per la codifica e decodifica usando la classe AES
                      - Introdotta chiave cr2 in web.config o app.config")

            Riga_Text("Agronica WebApiProfilatore --> Agronica Portal",
                      "- Rimozione da Agronica WebApiProfilatore delle pagine ad uso interno 
                      (schedulazione aggiornamenti e gestione licenze / superuser) 
                      che vengono spostate su nuovo sito Agronica Portal, 
                      per lasciare su WebApiProfilatore solo le funzionalità di produzione ad uso cliente / Profitosan")

            Riga_Text("Gestione sequence default",
          "Gestione sequence per progressivi chiavi tabelle di default attive per tutti. 
           Disattivabili esclusivamente impostando a False la chiave Allow_Sql_Sequence nell'appsettings")

            Riga_Text("Security - Query",
                      "Parametrizzate massivamente molte query in filtri aggiuntivi, order by e clausole IN per impedire sql injection")

            '==================================

            Riga_Data("11 Ottobre 2024")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Requisiti("CoreWS",
                           "Per modifiche legate alla codifica e decodifica semplice delle stringhe", "17/09/2024")

            Riga_Text("Security - XSS Detection",
                      "Aggiunto controllo per bloccare script injection nelle chiamate ai web method")

            '==================================

            Riga_Data("17 Settembre 2024")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Requisiti("CoreWS",
                           "Per modifiche legate alla codifica e decodifica semplice delle stringhe", "17/09/2024")

            Riga_Text("Codifica e decodifica stringhe",
                      "Introdotta la possibilità di effettuare una codifica semplice delle stringhe",
                      noteTecniche:="Per poter abilitare la codifica semplice la proprietà UsaCodificaSemplice in appsettings deve essere impostata a true")

            '==================================

            Riga_Data("13 Settembre 2024")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

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

            '==================================

            Riga_Data("06 Settembre 2024")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Autenticazione Demetra",
                      "Fix scrittura GDPR_Accettazione per considerare l'utente appena creato al posto del superuser", "Coldiretti", 0,
                      "", "nessun test")

            '==================================

            Riga_Data("28 Agosto 2024")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Autenticazione Demetra",
                      "Corretto riferimento variabile utente in fase di controllo GDPR che fa fallire l'autenticazione", "Coldiretti", 0,
                      "", "nessun test")

            '==================================

            Riga_Data("27 Agosto 2024")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Autenticazione Demetra",
                      "Corretto controllo GDPR_Accettazione in fase di creazione utente azienda agricola in modo che venga gestito l'utente appena creato", "Coldiretti", 0,
                      "", "nessun test")

            '==================================

            Riga_Data("23 Agosto 2024")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("SQL Data Provider nuova impostazione e mail di log",
           " - Reinserita la possibilità di ricevere una mail di log per le query in errore" &
           " - Inserita la possibilità di impostare la non esecuzione della query originale in caso di errore di parametrizzazione")

            '==================================

            Riga_Data("17 Luglio 2024")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("SQL Sequence",
                      "Aggiunto parametro attivazione in appsettings")

            '==================================

            Riga_Data("15 Luglio 2024")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("Contatori",
                      "Normalizzata funzione di richiamo stack counter", "", 0, "", "")

            '==================================

            Riga_Data("21 Giugno 2024")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Elastic Search",
                     "Inserita gestione escaping per log applicativi inviati ad elastic search", "Coldiretti",
                     noteTecniche:="Aggiunto remove escaping pre e post serializzazione",
                     noteTest:="no test")

            '==================================

            Riga_Data("14 Giugno 2024")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("Log Provider",
                      " Modificato il valore di default da logga solo su file a logga solo su DB", noteTest:="Non testabile")

            '==================================

            Riga_Data("17 Maggio 2024")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("SQL DataProvider ordinamento orderby",
         "Fix ordinamento dei risultati di alcune query (come il widget delle colture)")

            Riga_Text("Miglioramento lettura imprese all'apertura di alcune pagine",
                      "Velocizzata la lettura delle imprese prendendo solo i campi necessari, introdotto metodo nuovo per la lettura")

            Riga_Text("Migliorata la parametrizzazione delle query",
                      "Velocizzata la procedura di sostituzione del testo necessaria per la parametrizzazione")

            '==================================

            Riga_Data("26 Aprile 2024")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("Servizio IsAlive",
                      "Aggiunto ws per verificare raggiungibilità sito")

            '==================================

            Riga_Data("05 Marzo 2024")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("Gestione Aggiornamenti",
                      "- DDL Repository ora mostra solo quelli abilitati, così vecchi repo (ad es Demetra) non sono più sceglibili")

            '==================================

            Riga_Data("14 Febbraio 2024")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("Autenticazione Demetra",
                      "Autenticazione token mono uso", "Coldiretti", 0,
                      "Ad ogni chiamata viene sostituito il token precedente con uno nuovo", "nessun test")

            '==================================

            Riga_Data("20 Dicembre 2023")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("Autenticazione Demetra",
                      "Modificata valorizzazione Piva\CodFis su Utenti_Dettagli quando viene creato l'utente collegato alla scheda socio coldiretti", "Coldiretti", 0, "E' stato riscontrato un problema in fase di login tale per cui deve essere sempre valorizzato il campo CodFis della Utenti_Dettagli altrimenti non si accede", "nessun test")

            '==================================

            Riga_Data("21 Novembre 2023")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("Gestione Aggiornamenti",
                      "Lettura dinamica DDL Repository anziché valori fissi a codice")

            Riga_Bug("Login",
                      "fix problema per cui premendo invio ricaricava la pagina anziché eseguire la login")

            '==================================

            Riga_Data("23 Ottobre 2023")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("Demetra", "aggiunta forzatura GDPR accepted in creazione utente gias", "coldiretti", 0, "", "")

            '==================================

            Riga_Data("16 Ottobre 2023")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("Demetra", "Creato nuovo metodo per l'autenticazione bypassando la password (basta specificare CUAA e Username). Quest'ultimo prevede anche la creazione dell'utente qualora non esistesse e restituisce un token di accesso valido per la nuova utenza appena creata.", "coldiretti", 0, "metodo aggiuntivo per l'autenticazione. attenzione che prevede l'aggiunta di due nuove voci in appsettings.config mandatoria per il funzionamento di questa nuova funzione",
                       "configurare webapiprofilatore su ambiente collaudo coldiretti (impostare le chiavi BaseUrlColdirettiWebServicePortaleSocio e ProfiloPermessiDefault), recuperare un token di accesso valido(super server), tramite postman provare la nuova chiamata impostando nel payload: un CUAA presente su GIAS ed il codice scheda socio associato al CUAA")

            '==================================

            Riga_Data("10 ottobre 2023")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("Demetra", "Testato nuovo servizio QDemetraQdCBluarancio = 1014 in aggiunta al precedente QDemetra 1017")

            '==================================

            Riga_Data("12 Maggio 2023")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("GiasBase", "Eliminati tutti i riferimenti fissi al GiasBase (/GiasBase/... etc nel codice - lettura parametrizzata tramite chiave in configurazione siti")

            '==================================

            Riga_Data("13 Febbraio 2023")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("Versione Prodotti",
                      "- Introdotta doppia griglia per versioni raggruppate per installazione e con dettaglio imprese")

            '==================================

            Riga_Data("22 Dicembre 2022")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("Gestione Aggiornamenti",
                      "- Abilitato export Excel tabella")

            Riga_Text("WebApiProfilatore",
                      "- Nuova pagina reimpostazione password Profitosan")

            '==================================

            Riga_Data("29 Settembre 2022")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("Nuove pagine",
                      "- RichiesteIscrizioni: Modificata procedura")

            Riga_Bug("MasterPage", "Fix inizializzazione date finestra temporale, bypass culture")

            '==================================

            Riga_Data("26 Settembre 2022")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("Nuove pagine",
                      "- RichiesteCancellazioni: inserita bozza cancellazione utente per app -> Apple requirement")

            Riga_Bug("MasterPage", "Fix inizializzazione date finestra temporale, bypass culture")

            '==================================

            Riga_Data("23 Settembre 2022")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("Nuove pagine",
                      "- RichiesteIscrizioni: inserita bozza registrazione per app -> Apple requirement")

            '==================================

            Riga_Data("21 Settembre 2022")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("Nuove pagine",
                      "- RichiesteCancellazioni, RichiesteIscrizioni")

            '==================================

            Riga_Data("01 Settembre 2022")

            Riga_Requisiti("Migra",
                           "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                           "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("Sito completo",
                      "- Modifiche per gestire nuove versioni di kendo 2022.x")

            '==================================

            Riga_Data("27 Maggio 2022")

            Riga_Requisiti("Migra",
                       "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                       "Gias_WEBUtenti_Migra", "1")

            Riga_Text("RispostaStandard",
                     "- gestione compressione rispostastandard  lato server / decompressione lato client JS")

            '==================================

            Riga_Data("12 Maggio 2022")

            Riga_Requisiti("Migra",
                       "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                       "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO ")

            Riga_Text("Gestione Aggiornamenti Gias:",
                      "Aggiunta colonna Versione Client nella tabella di gestione aggiornamenti")

            '==================================

            Riga_Data("15 Marzo 2022")

            Riga_Requisiti("Migra",
                       "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                       "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO ")

            Riga_Text("Gestione Licenze Gias:",
                      "Aggiunto messaggio di errore su generazione\rigenerazione GiasAppKey se non esiste record configurazione endpoint corews")

            Riga_Bug("Gestione Licenze Gias:",
                      "ripristinato timeout a valore strd 100 secondi per cloud")

            '==================================

            Riga_Data("15 Marzo 2022")

            Riga_Requisiti("Migra",
                       "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Migra",
                       "Gias_WEBUtenti_Migra", "1")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO ")

            Riga_Text("Gestione Licenze Gias:",
                      "Modificata tabella dati licenze - aggiunta colonna GiasAppKey
                       Modificato web method Ricerca_Licenze per aggiungere la colonna GiasAppKey
                       Aggiunto nuovo web method GeneraGiasAppKey per generare\rigenerare la GiasAppKey
                       Aggiunto pulsante 'chiave' per richiamre la funzione di generazione\rigenerazione della GiasAppKey")


            '==================================

            Riga_Data("11 Febbraio 2022 B")

            Riga_Requisiti("Migra",
                       "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Configurazione_Siti",
                       "Aggiunta chiave AbilitaHashPassword", "74")

            Riga_Requisiti("GiasBase",
                       "per KendoDateTimeEditor in Grid", "25/06/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO ")

            Riga_Text("Gestione Licenze Gias:",
                  "Correzione Bug Date Moduli GiasOnline")
            '==================================

            Riga_Data("11 Febbraio 2022")

            Riga_Requisiti("Migra",
                       "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Configurazione_Siti",
                       "Aggiunta chiave AbilitaHashPassword", "74")

            Riga_Requisiti("GiasBase",
                       "per KendoDateTimeEditor in Grid", "25/06/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO ")

            Riga_Text("Gestione Licenze Gias:",
                  "Correzione Bug Modifica Superuser")

            '==================================

            Riga_Data("10 Febbraio 2022")

            Riga_Requisiti("Migra",
                       "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Configurazione_Siti",
                       "Aggiunta chiave AbilitaHashPassword", "74")

            Riga_Requisiti("GiasBase",
                       "per KendoDateTimeEditor in Grid", "25/06/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO ")

            Riga_Text("Gestione Licenze Gias:",
                  "Correzione Bug numero aziende Non riportato in GiasLan")

            '==================================


            '==================================

            Riga_Data("01 Febbraio 2022")

            Riga_Requisiti("Migra",
                       "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Configurazione_Siti",
                       "Aggiunta chiave AbilitaHashPassword", "74")

            Riga_Requisiti("GiasBase",
                       "per KendoDateTimeEditor in Grid", "25/06/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO ")

            Riga_Text("Gestione Licenze Gias:",
                  "Chiamata WS Completo per Debug")

            '==================================

            Riga_Data("26 Gennaio 2022")

            Riga_Requisiti("Migra",
                       "Aumento dimensione campo Password tabella Utenti", "624")

            Riga_Requisiti("Configurazione_Siti",
                       "Aggiunta chiave AbilitaHashPassword", "74")

            Riga_Requisiti("GiasBase",
                       "per KendoDateTimeEditor in Grid", "25/06/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO ")

            Riga_Text("Gestione Licenze Gias:",
                  "Chiamata WS")

            '==================================

            Riga_Data("13 Dicembre 2021")

        Riga_Requisiti("Migra",
                       "Aumento dimensione campo Password tabella Utenti", "624")

        Riga_Requisiti("Configurazione_Siti",
                       "Aggiunta chiave AbilitaHashPassword", "74")

        Riga_Requisiti("GiasBase",
                       "per KendoDateTimeEditor in Grid", "25/06/2021")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO ")

        Riga_Bug("Gestione Licenze Gias:",
                  "Bug fix per errori js non bloccanti")

        '==================================

        Riga_Data("03 Novembre 2021")

        Riga_Requisiti("Migra",
                       "Aumento dimensione campo Password tabella Utenti", "624")

        Riga_Requisiti("Configurazione_Siti",
                       "Aggiunta chiave AbilitaHashPassword", "74")

        Riga_Requisiti("GiasBase",
                       "per KendoDateTimeEditor in Grid", "25/06/2021")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO ")

        Riga_Text("Gestione Aggiornamenti e Versione Prodotti:",
                  "Migliorie a griglie kendo")

        '==================================

        Riga_Data("26 Agosto 2021")

        Riga_Requisiti("Migra",
                       "Aumento dimensione campo Password tabella Utenti", "624")

        Riga_Requisiti("Configurazione_Siti",
                       "Aggiunta chiave AbilitaHashPassword", "74")

        Riga_Requisiti("GiasBase",
                       "per KendoDateTimeEditor in Grid", "25/06/2021")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO ")

        Riga_Bug("CoreDAL/GerarchiaImprese:",
                  "Bug fix per mantenere allineata Utenti_Visibilita_Appoggio al cambiamento di gerarchia imprese")

        Riga_Text("Gestione Aggiornamenti:",
                  "Aggiunta colonna Id Client")

        Riga_Text("Tutte le pagine:",
                  "Aggiunto pulsante Indietro per tornare al menù")

        '==================================

        Riga_Data("12 Luglio 2021")

        Riga_Requisiti("Migra",
                       "Aumento dimensione campo Password tabella Utenti", "624")

        Riga_Requisiti("Configurazione_Siti",
                       "Aggiunta chiave AbilitaHashPassword", "74")

        Riga_Requisiti("GiasBase",
                       "per KendoDateTimeEditor in Grid", "25/06/2021")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")
        
        Riga_Bug("Login:",
                  "Bugfix aggiunta di spazio al termine del gruppo")
        
        Riga_Text("Login:",
                  "Verifica appartenenza gruppo anche con gruppi innestati")
        
        '==================================
        
        Riga_Data("25 Giugno 2021")

        Riga_Requisiti("Migra",
                       "Aumento dimensione campo Password tabella Utenti", "624")

        Riga_Requisiti("Configurazione_Siti",
                       "Aggiunta chiave AbilitaHashPassword", "74")

        Riga_Requisiti("GiasBase",
                       "per KendoDateTimeEditor in Grid", "25/06/2021")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")

        Riga_Text("Profilatore_Bootstrap.Master:",
                  "- Allineamento ad AgronicaControlli_2010 per inclusione delle versioni di JQuery e Bootstrap scelte in Configurazione Siti" & vbCrLf &
                  "- fix posizione footer che finisce sotto al contenuto in alcuni casi")
        
        Riga_Text("Login:",
                  "Nuova pagina login per gestire puntualmente i permessi con i gruppi di dominio")
        
        Riga_Text("Menu:",
                  "Nuova pagina menu per andare nelle varie pagine post Login")
        
        Riga_Text("Menu/Error:",
                  "pagina errore per mancanza autorizzazioni post-menu")
        
        Riga_Text("VersioneProdotti:",
                  "nuova pagina per mostrare elenco clienti e versione dei loro siti")
        
        Riga_Text("GestioneAggiornamenti:",
                  "nuova pagina per autorizzare gli aggiornamenti per l'AutoAggiornatore")
        
        '==================================
        
        Riga_Data("15/03/2021")

        Riga_Requisiti("Migra '624' :",
                  "Aumento dimensione campo Password tabella Utenti")

        Riga_Requisiti("Configurazione_Siti '74' :",
                  "Aggiunta chiave AbilitaHashPassword")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")

        Riga_Text("SqlClient:",
                  "Impostato uso SqlClient di default al posto dell'OleDb")

        '==================================
        
        Riga_Data("05/03/2021")

        Riga_Requisiti("Migra '624' :",
                  "Aumento dimensione campo Password tabella Utenti")

        Riga_Requisiti("Configurazione_Siti '74' :",
                  "Aggiunta chiave AbilitaHashPassword")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")

        Riga_Text("Versione .Net 4.8",
                  "Passaggio alla versione .Net 4.8")

        '==================================
        
        Riga_Data("04 Dicembre 2020")

        Riga_Requisiti("Migra '624' :",
                  "Aumento dimensione campo Password tabella Utenti")

        Riga_Requisiti("Configurazione_Siti '74' :",
                  "Aggiunta chiave AbilitaHashPassword")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")

        Riga_Text("ProfilatoreUtenze.svc.vb:",
                  "Demetra: Nel caso di richiesta Prima Attivazione, Annulla Conclusione Procedura per consentire un secondo invio al provisioning ")

        Riga_Text("ProfilatoreUtenze.svc.vb:",
                  "Demetra: Modificata verifica su esistenza impianto ""Non Demetra"", ora viene verificato l'esistenza di un impianto valido alla data della chiamata e non intersecato con l'annata agraria di riferimento della chiamata. ")

        '==================================

        Riga_Data("26 Novembre 2020")

        Riga_Requisiti("Migra '624' :",
                  "Aumento dimensione campo Password tabella Utenti")

        Riga_Requisiti("Configurazione_Siti '74' :",
                  "Aggiunta chiave AbilitaHashPassword")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")

        Riga_Text("ProfilatoreUtenze.svc.vb:",
                  "Aggiornata per nuova gestione hash password")

        '==================================

        Riga_Data("06 Agosto 2020")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '607' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("Aggancio '84' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche Sì - autorizzato servizio 1017 (chave ""serviziAutorizzati"") ")


        Riga_Text("ProfilatoreUtenze:",
                  " Test su Piano colturale assente in fase attivazione Servizio Demetra ")

        '==================================

        Riga_Data("05 Agosto 2020")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '607' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("Aggancio '84' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")

        Riga_Text("ProfilatoreUtenze:",
                  " Nuovo Servizio Demetra")

        '==================================


        Riga_Data("02 Dicembre 2019 B")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '520' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("Aggancio '77' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")

        Riga_Text("ProfilatoreUtenze:",
                  " bug fix su procedura di autenticazione in modo che non venga re-inizializzato il token se si fa login da più dispositivi")

        '==================================


        Riga_Data("02 Dicembre 2019")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '520' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("Aggancio '77' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")

        Riga_Text("ProfilatoreUtenze:",
                  " idWidget presente nella risposta del webservice, come parametro di widget ed implementazione in pagina DSSWidget")

        '==================================


        Riga_Data("27 Novembre 2019")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '520' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("Aggancio '77' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")

        Riga_Text("ProfilatoreUtenze:",
                  " Testi aggiornati per widget + logo")

        '==================================

        Riga_Data("14 Novembre 2019")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '520' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("Aggancio '77' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")


        Riga_Text("ProfilatoreUtenze:",
                  " Widget per indicatori di modelli difesa configurati ")

        '==================================

        Riga_Data("11 Novembre 2019")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '520' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("Aggancio '77' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")


        Riga_Text("ProfilatoreUtenze:",
                  " Aggiunto endpoint 'widgetManager' con primo widget per costruire pulsante di accesso a Gias ")

        '==================================

        Riga_Data("08 Novembre 2019")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '520' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("Aggancio '77' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")


        Riga_Text("ProfilatoreUtenze:",
                  " Aggiunto endpoint 'autenticazione' con diversa gestione della validazione del token ")
        '==================================


        Riga_Data("10 Maggio 2019")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '520' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("Aggancio '77' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")

        Riga_Text("ProfilatoreUtenze:",
                  " Rimosso endpoint ""passwordReset"" e mantenenuto solamente endpoint ""accountEdit"" per le operazioni di update ")

        '==================================


        Riga_Data("08 Maggio 2019 (B)")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '520' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("Aggancio '77' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")

        Riga_Text("ProfilatoreUtenze:",
                  " profileManager, ora in fase di scrittura delle impostazioni utente si tiene conto dell'aggiornamento di valori pre-esistenti ")
        '==================================


        Riga_Data("08 Maggio 2019")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '520' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("Aggancio '77' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")

        Riga_Text("ProfilatoreUtenze:",
                  " accountEdit, corretta anomalia in fase di chiamata ")
        '==================================


        Riga_Data("18 Aprile 2019")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '520' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("Aggancio '77' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")

        Riga_Text("ProfilatoreUtenze:",
                  " Nuove api passwordReset, accountEdit ")
        '==================================


        Riga_Data("20 Febbraio 2019")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '520' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("Aggancio '77' :",
                  "OBBLIGATORIA: Per gestione DSS, utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO ")

        Riga_Text("ProfilatoreUtenze:",
                  " Nuovo metodo rest per gestione provisioning DSS ")

        '==================================


        Riga_Data("05 Dicembre 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '514' :",
                  "OBBLIGATORIA: Per gestione utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI, per stabilire servizi ed autorizzazioni su chiamata ""estesa"". ")


        Riga_Text("ProfilatoreUtenze:",
                  " Affinamenti ")

        '==================================


        Riga_Data("19 Novembre 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '514' :",
                  "OBBLIGATORIA: Per gestione utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI, per stabilire servizi ed autorizzazioni su chiamata ""estesa"". ")

        Riga_Text("ProfilatoreUtenze:",
                  " Gestiti CORS ")

        '==================================


        Riga_Data("16 Novembre 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '514' :",
                  "OBBLIGATORIA: Per gestione utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI, per stabilire servizi ed autorizzazioni su chiamata ""estesa"". ")

        Riga_Text("ProfilatoreUtenze:",
                  " vari bug fix dopo primo rilascio ")
        '==================================


        Riga_Data("15 Novembre 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '514' :",
                  "OBBLIGATORIA: Per gestione utenti_dettagli ed altro")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI, per stabilire servizi ed autorizzazioni su chiamata ""estesa"". ")

        Riga_Text("ProfilatoreUtenze:",
                  " Prima versione in TEST per chiamata API Estesa (eCommerce Profitosan) ")

        '==================================


        Riga_Data("26 Luglio 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI, per stabilire servizi ed autorizzazioni. ")

        Riga_Text("ProfilatoreUtenze:",
                  " impostato anche lo stato attivo gratuito sugli stati iniziali disponibili ")

        '==================================


        Riga_Data("26 Giugno 2018")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "")

        Riga_Text("ProfilatoreUtenze:",
                  " Modifiche su funzione 'profileManager': se viene passata la p.iva allora verifico se è già stato memorizzato in precedenza il dato del CUAA associato alla p.iva ed uso comunque questo dato. ")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI, per stabilire servizi ed autorizzazioni. ")

        '==================================


        Riga_Data("30 Maggio 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI, per stabilire servizi ed autorizzazioni. ")


        Riga_Text("ProfilatoreUtenze:",
                  " modifiche su funzione 'accountingUser' ")

        '==================================


        Riga_Data("24 Maggio 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI, per stabilire servizi ed autorizzazioni. ")


        Riga_Text("ProfilatoreUtenze:",
                  " nuova funzione 'accountingUser' ")
        '==================================


        Riga_Data("22 Maggio 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI, per stabilire servizi ed autorizzazioni. ")

        Riga_Text("ProfilatoreUtenze:",
                  "Le generazione di permessi e profili su utente avviene solo a seguito di scrittura dei record di pratica attiva su QBASE,ecc.")

        Riga_Text("ProfilatoreUtenze:",
                  "Small bug fix")

        '==================================
        Riga_Data("16 Maggio 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI, per stabilire servizi ed autorizzazioni. ")

        Riga_Text("ProfilatoreUtenze:",
                  "Chiusura di account.")

        Riga_Text("ProfilatoreUtenze:",
                  "Gestione di profilazione per utenti 'esterni'.")

        Riga_Text("ProfilatoreUtenze:",
                  " Gestione del CUAA come unico identificativo in fase di profilazione di un'azienda.")

        Riga_Text(" :",
                  "" &
                  "<br> ")

        Riga_Fine()


        '==================================


        Riga_Data("24 Novembre 2017 ")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI/NO. ")

        Riga_Text("ProfilatoreUtenze:",
                  "Il tipo di utente memorizzato è ora ""impresa"", anche in modifica + ripristinata verifica su esistenza utente.")

        Riga_Text(" :",
                  "" &
                  "<br> ")

        Riga_Fine()

        '==================================


        Riga_Data("23 Novembre 2017 ")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI/NO. ")

        Riga_Text("ProfilatoreUtenze:",
                  "Il tipo di utente memorizzato è ora ""impresa"".")

        Riga_Text(" :",
                  "" &
                  "<br> ")

        Riga_Fine()
        '==================================


        Riga_Data(" 08 Novembre 2017")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI/NO. ")

        Riga_Text("ProfilatoreUtenze:",
                  "Verifica e decide se impostare l'impostazione UTENTE_Attiva_Configurazione_Pratica.")

        Riga_Text(" :",
                  "" &
                  "<br> ")

        Riga_Fine()

        '==================================


        Riga_Data(" 17 Ottobre 2017")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI/NO. ")

        Riga_Text("ProfilatoreUtenze:",
                  "Verifica e decide se impostare visibilità utente (Utenti_Dettagli, campo userNameCommerciale).")

        Riga_Text(" :",
                  "" &
                  "<br> ")

        Riga_Fine()

        '==================================

        Riga_Data("27 Settembre 2017")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI/NO. ")

        Riga_Text("ProfilatoreUtenze:",
                  "Web Api profilatore: integrazione per effettuare una chiamata passando una p.iva: a quel punto si ottiene l’Accounting per la sola azienda (e non più il totale)")

        Riga_Text("ProfilatoreUtenze:",
                  "Web Api Prifilatore - Impedire la generazione di un utente laddove esiste un altro utente con lo stesso codice fiscale")

        Riga_Text(" ProfilatoreUtenze:",
                  " Chiusura versione " &
                  "<br> ")

        Riga_Fine()
        '==================================

        Riga_Data("20 Settembre 2017")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI/NO. ")

        Riga_Text("ProfilatoreUtenze:",
                  "Nella API profilatore eseguire la verifica che la p.iva passata non coincida con i primi 11 caratteri del CUAA, laddove questo sia un CF di 14 caratteri.")

        Riga_Text("ProfilatoreUtenze:",
                  "API Profilatore: scrivere il default in tabella Utenti_xGruppi_Utente per potere attivare la cartografia")

        Riga_Text(" ProfilatoreUtenze:",
                  " Chiusura versione " &
                  "<br> ")

        Riga_Fine()

        '==================================

        Riga_Data("19 Settembre 2017")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI/NO. ")

        Riga_Text("ProfilatoreUtenze:",
                  "Work in progress.")

        Riga_Text(" ProfilatoreUtenze:",
                  " Chiusura versione " &
                  "<br> ")

        Riga_Fine()

        '==================================

        Riga_Data("16 Settembre 2017 (2) ")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI/NO. ")

        Riga_Text("ProfilatoreUtenze:",
                  "Work in progress.")

        Riga_Text(" ProfilatoreUtenze:",
                  " Gestite utenti_Impostazioni e utenti_impostazioni_FiltroMono " &
                  "<br> ")

        Riga_Fine()
        '==================================


        Riga_Data("31 Agosto 2017 ")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI/NO. ")

        Riga_Text("ProfilatoreUtenze:",
                  "Work in progress.")

        Riga_Text(" ProfilatoreUtenze:",
                  "Gestione date fine valità utenti, pratiche stati; gestiti id negativi in Tipologie utenti" &
                  "<br> ")

        Riga_Fine()
        '==================================


        Riga_Data("30 Agosto 2017 ")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI/NO. ")

        Riga_Text("ProfilatoreUtenze:",
                  "Work in progress.")

        Riga_Text(" ProfilatoreUtenze:",
                  "Gestiti stati 'in prova'" &
                  "<br> ")

        Riga_Fine()

        '==================================


        Riga_Data("25 Agosto 2017")

        Riga_Requisiti("ATTENZIONE :",
                       "in web_utenti necessaria tabella AWS_log.")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "Tabelle Utenti_Token, Utenti_Validita_Token")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI.")

        Riga_Text("ProfilatoreUtenze:",
                  "Work in progress.")

        Riga_Fine()


        '==================================

        Riga_Data("4 Agosto 2017")

        Riga_Requisiti("ATTENZIONE :",
                       "in web_utenti necessaria tabella AWS_log.")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("Migra '456' :",
                  "Tabelle Utenti_Token, Utenti_Validita_Token")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI.")

        Riga_Text("ProfilatoreUtenze:",
                  "Work in progress.")

        Riga_Fine()


        '==================================


    End Sub




    '#################################################################################################
    Private Sub ImgBtn_Codice_Ins_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Codice_Ins.Click

        Dim ChiaveUtente As String
        Dim ChiaveSistema As String

        If Not IsNothing(ConfigurationManager.AppSettings("Versione_Pwd")) Then
            ChiaveSistema = ConfigurationManager.AppSettings("Versione_Pwd").ToString
        Else
            ChiaveSistema = "frigoriferoconcubetti"
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
