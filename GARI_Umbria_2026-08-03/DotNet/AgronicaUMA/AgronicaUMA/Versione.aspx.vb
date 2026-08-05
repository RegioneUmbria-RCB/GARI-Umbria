Imports System.IO
Imports System.Security.Cryptography
Imports AgronicaCoreUtilityVersioni
Imports AgronicaCoreUtilityVersioni.Enumerativi

Namespace UmaVersione

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

            Riga_Versione("01 Luglio 2026", "151.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuovi loghi", "01/12/2025")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                "Elenco Vendite UMA",
                "Fix per visualizzare ed utilizzare la piva reale per i proessi UMA",
                cliente:="Regione Umbria",
                idTicketAssistenza:=219395, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:="Marco Cecalupo",
                noteTest:="",
                noteTecniche:="")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("12 Giugno 2026", "151.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuovi loghi", "01/12/2025")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                "Elenco Vendite UMA",
                "Correzione bug che non evidenziava in rosso tutte le righe interessate (data documento < data creazione)",
                cliente:="Regione Umbria",
                idTicketAssistenza:=219395, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Casa,
                noteTest:="",
                noteTecniche:="")

            '------------------------------------------------------------------------------------------------


            Riga_Versione("08 Giugno 2026", "151.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuovi loghi", "01/12/2025")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                "Tutte",
                "Estrazione e visualizzazione della Partita Iva Reale",
                cliente:="Tutti",
                idTicketAssistenza:=0, idTicketSviluppo:=214395, idTicketTesting:=0,
                autore:="Cecalupo Marco",
                noteTest:="",
                noteTecniche:="")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("22 Maggio 2026", "150.4.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuovi loghi", "01/12/2025")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                            "Redirezione a Custom500 errata",
                            "Corretta redirezione alla pagina custom 500 in caso di errore al passaggio tra siti",
                            cliente:="TUTTI",
                            idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                            noteTest:="Non testabile in autonomia",
                            noteTecniche:="",
                            autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------


            Riga_Versione("04 Maggio 2026", "150.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuovi loghi", "01/12/2025")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Visualizzazione rendicontazioni terzisti",
               "Corretto errore che non mostrava le rendicontazioni dei terzisti",
               cliente:="Regione Umbria",
               idTicketAssistenza:=214972,
               idTicketSviluppo:=0,
               idTicketTesting:=0,
               noteTest:="",
               autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("27 Aprile 2026", "150.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuovi loghi", "01/12/2025")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Anticipazioni colturali da terzisti anno precedente",
               "Corretta visualizzazione non arrotondata del carburante delle anticipazioni colturali terzisti nelle richieste/rendicontazioni",
               cliente:="Regione Umbria",
               idTicketAssistenza:=211926,
               idTicketSviluppo:=0,
               idTicketTesting:=0,
               noteTest:="",
               autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Errore calcolo carburante passaggio di stato",
               "Corretto un errore lato griglia colture, all'inserimento multiplo di una lavorazione 'MAX X VOLTE' a volte veniva registrata come lavorazione singola, portando ad un calcolo errato al passaggio di stato",
               cliente:="Regione Umbria",
               idTicketAssistenza:=213053,
               idTicketSviluppo:=0,
               idTicketTesting:=0,
               noteTest:="Per testare bene, provare prima ad inserire le lavorazioni 'MAX X VOLTE' una ad una, poi provare il passaggio di stato
               (se si ferma al 'mancano documenti obbligatori' significa che tutti gli altri controlli sono andati a buon fine. 
               Poi ripetere provando ad inserire le lavorazioni con l'inserimento multiplo a partire da una riga già presente 
               e poi da una vuota.",
               autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Errore macrousi Richieste 2026",
               "Corretto errore nelle richieste 2026 che impediva di aggiungere nuovi macrousi mostrando l'errore 'Nessuna coltura situata in Umbria trovata'",
               cliente:="Regione Umbria",
               idTicketAssistenza:=213385,
               idTicketSviluppo:=0,
               idTicketTesting:=0,
               noteTest:="",
               autore:=DEV_Casa)

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

            Riga_Versione("01 Aprile 2026", "149.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuovi loghi", "01/12/2025")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Elenco Rendicontazioni UMA",
               "Corretto errore di arrotondamento nella parte che calcola se l'azienda ha dei litri di carburante in esubero",
               cliente:="Regione Umbria",
               idTicketAssistenza:=211377,
               idTicketSviluppo:=0,
               idTicketTesting:=0,
               noteTest:="Testato ricreando il caso presente in produzione dell'azienda BONOMI MARCO",
               autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("13 Marzo 2026", "148.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuovi loghi", "01/12/2025")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Errore classificazione colture",
               "Corretto errore di merge",
               cliente:="Regione Umbria",
               idTicketAssistenza:=207850,
               idTicketSviluppo:=0,
               idTicketTesting:=0,
               noteTest:="Testato su rendicontazione Morami Federica MRMFRC98B50G478O, in rendicontazione 17 ha di lupini venivano classificati erroneamente come erbai anzichè leguminose primaverili ",
               autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Inserimento Colture UF",
               "Corretto errore che impediva l'inserimento delle colture per il calcolo delle UF per le pratiche del 2026",
               cliente:="Regione Umbria",
               idTicketAssistenza:=209576,
               idTicketSviluppo:=0,
               idTicketTesting:=0,
               noteTest:="Testato su richieste 2026",
               autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("10 Marzo 2026", "148.1.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuovi loghi", "01/12/2025")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Errore classificazione colture",
               "Corretto errore sul recupero del codice coltura agea dell'appezzamento",
               cliente:="Regione Umbria",
               idTicketAssistenza:=207850,
               idTicketSviluppo:=0,
               idTicketTesting:=0,
               noteTest:="Testato su rendicontazione Morami Federica MRMFRC98B50G478O, in rendicontazione 17 ha di lupini venivano classificati erroneamente come erbai anzichè leguminose primaverili ",
               autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("02 Marzo 2026", "148.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuovi loghi", "01/12/2025")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Errore azzeramento anticipo",
               "Corretto errore per il quale non era possibile azzerare una quota di carburante nell'anticipo",
               cliente:="Regione Umbria",
               idTicketAssistenza:=207426,
               idTicketSviluppo:=0,
               idTicketTesting:=0,
               noteTest:="Testato su Bennati Maurizio 01470830546 anticipo 2026 ",
               autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Errore classificazione colture",
               "Corretto errore che in rari casi classificava male le colture su nuova richiesta/rendicontazione",
               cliente:="Regione Umbria",
               idTicketAssistenza:=207850,
               idTicketSviluppo:=0,
               idTicketTesting:=0,
               noteTest:="Testato su rendicontazione Morami Federica MRMFRC98B50G478O, in rendicontazione 17 ha di lupini venivano classificati erroneamente come erbai anzichè leguminose primaverili ",
               autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("13 Febbraio 2026", "147.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuovi loghi", "01/12/2025")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Errore data inizio rendicontazione",
               "Corretto errore nella valorizzazione della data Inizio Rendicontazione che aggiungeva un mese alla data reale",
               cliente:="Regione Umbria",
               idTicketAssistenza:=205953,
               idTicketSviluppo:=0,
               idTicketTesting:=0,
               noteTest:="",
               autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("06 Febbraio 2026", "147.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuovi loghi", "01/12/2025")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Oscurare username in UMA",
               "Sono stati rimossi gli username degli utenti dall'Elenco Richieste/Rendicontazioni e dalla Ricerca per Macrousi/Lavorazioni lasciando visibile solo nome e cognome nelle colonne 'Richiedente' e 'Approvatore'",
               cliente:="Regione Umbria",
               idTicketAssistenza:=0,
               idTicketSviluppo:=204802,
               idTicketTesting:=0,
               noteTest:="",
               autore:=DEV_Casa)

            Riga_Changelog(
               enum_Tipo_Changelog.Bug,
               area:="Verifica presenza Personalizzazione Grafiche Cliente",
               descrizione:="Ora viene verificata la presenza di ogni proprietà che viene utilizzata",
               cliente:="Coldiretti",
               idTicketAssistenza:=205504,
               idTicketSviluppo:=0,
               idTicketTesting:=0,
               noteTest:="",
               autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("27 Gennaio 2026", "146.2.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuovi loghi", "01/12/2025")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")


            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Fix caricamento infinito anticipi",
               "Corretto problema che impediva ad alcuni anticipi di caricarsi correttamente, risultando in un loop infinito di richieste ajax",
               cliente:="Regione Umbria",
               idTicketAssistenza:=204866,
               idTicketSviluppo:=0,
               idTicketTesting:=0,
               noteTest:="",
               autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("17 Dicembre 2025", "145.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuovi loghi", "01/12/2025")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Verifica appezzamenti decimali",
                           "Arrotondate tutte le superfici (da UMA e da piano colturale) a 4 cifre decimali per evitare errori di approssimazione",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=201415,
                           idTicketSviluppo:=0,
                           idTicketTesting:=0,
                           noteTest:="",
                           autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "UMA Configurazione date rendicontazione",
                           "Corretto errore sul controllo della data di blocco rendicontazione all'inserimento di un nuovo record",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=201437, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("01 Dicembre 2025", "145.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuovi loghi", "01/12/2025")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Personalizzazioni Loghi",
                           "Gestito il nuovo parametro 'personalizzazioniLoghi'. A questo parametro sarà associato un JSON con specificati i percorsi dei loghi da sostituire a quelli standard di Agronica. Se il valore non risulta essere un'istanza della classe PersonalizzazioniLoghi, il valore verrà ignorato. Tale parametro andrà a sostituire 'personalizzazioniRegioneUmbria'",
                           cliente:="",
                           idTicketAssistenza:=0,
                           idTicketSviluppo:=196565,
                           idTicketTesting:=0,
                           noteTest:="",
                           autore:="Dario Cabras")

            Riga_Changelog(enum_Tipo_Changelog.Security,
                           "Librerie terze parti",
                           "- Eliminate librerie html5shiv, respond e ie-emulation-modes-warning (non usate)",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Giulia Bottan")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("07 Novembre 2025", "144.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Performance,
                           "Gestione Cache Permessi e Impostazioni utente",
                           "- Gestita la cache per Permessi e impostazioni utente;",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=195312, idTicketTesting:=0,
                           autore:=DEV_Drudi)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("03 Novembre 2025", "144.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

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
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

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

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                area:="UMA report controllo",
                descrizione:="Fix ricerca in caso di assenza dei filtri provincia e comune",
                cliente:="Umbria",
                idTicketAssistenza:=0,
                idTicketSviluppo:=0,
                idTicketTesting:=0,
                noteTecniche:="",
                autore:=DEV_Casa
                )

            '------------------------------------------------------------------------------------------------

            Riga_Versione("10 Ottobre 2025", "143.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                area:="Documentale",
                descrizione:="Rollback aggiunta AgroSQL_Save_Text su piva in AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti.Controllo_Permessi_JOIN " &
                             "perché pezzo di query costruito esternamente alla sua esecuzione e rompe le query",
                cliente:="Umbria",
                idTicketAssistenza:=0,
                idTicketSviluppo:=0,
                idTicketTesting:=0,
                noteTecniche:="",
                autore:="Giulia Bottan"
                )

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                area:="UMA inserimento legami richieste regImpianti",
                descrizione:="Diminuito il numero di chunk della query per evitare sovraccarico di parametri",
                cliente:="Umbria",
                idTicketAssistenza:=0,
                idTicketSviluppo:=0,
                idTicketTesting:=0,
                noteTecniche:="",
                autore:=DEV_Casa
                )

            '------------------------------------------------------------------------------------------------

            Riga_Versione("26 Settembre 2025", "142.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "UMA errore passaggio di stato",
                           "Corretto errore su passaggio di stato UMA per aziende che usano colture bio ",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Casa,
                           noteTest:="Portare indietro la rendicontazione 2024 di BIAGETTI NICO (BGTNCI74R25D653Q) 
                           fino a 'In Compilazione' e provare a seguire il flusso normale dei passaggi di stato",
                           noteTecniche:="")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("19 Settembre 2025", "142.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "UMA richieste: verifica ed allineamento superfici",
                           "Corretta la verifica e l'allineamento delle superfici per rendicontazioni di tipo Conto Terzi, in caso di più aziende per stesso gruppo colturale",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=191325, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Novaga,
                           noteTest:="Considerare il caso di una rendicontazione di tipo Conto Terzi in compilazione, con più aziende per uno stesso gruppo colturale UMA, 
                           verificare che in caso di modifiche anagrafiche (nuovo impianto, cancellazione impianto o modifica della superficie di un impianto coinvolto) 
                           per una di queste aziende e gruppo colturale, venga rilevata la variazione corretta e che questa venga effettuata cliccando Allinea.",
                           noteTecniche:="")


            '------------------------------------------------------------------------------------------------

            Riga_Versione("08 Settembre 2025", "142.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "UMA allineamento inconguenze",
                           "Corretta situazione a seguito dell'allineamento dove i dati lato DB non venivano aggiornati fino al ricaricamento della pagina",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Security,
                           "Controllo massivo SQL Injection",
                           "Verificati molti casi di possibili sql injection",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=0, idTicketSviluppo:=179500, idTicketTesting:=0,
                           autore:=DEV_Casa, noteTest:="", noteTecniche:="")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("22 Agosto 2025", "141.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                            "UMA caricamento infinito analisi 2024",
                            "Corretto bug per le richieste UMA 2024 che, all'uscita dalle analisi del terreno, mostrava un caricamento infinito",
                            cliente:="Regione Umbria",
                            idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                            autore:=DEV_Casa,
                            noteTest:="Provare su qualunque richiesta 2024")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("07 Agosto 2025", "141.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                            "UMA pulsante dettaglio appezzamenti",
                            "Corretto errore di query per cui non mostrava l'elenco degli appezzamenti alla pressione del pulsante",
                            cliente:="Regione Umbria",
                            idTicketAssistenza:=186809, idTicketSviluppo:=0, idTicketTesting:=0,
                            autore:=DEV_Casa,
                            noteTest:="Provare su qualunque richiesta 2025")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("29 Luglio 2025", "140.3.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                            "UMA associazione impianti richiesta",
                            "Ridotta la dimensione dei chunk per l'inserimento massivo",
                            cliente:="Regione Umbria",
                            idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                            autore:=DEV_Casa,
                            noteTest:="Provare a creare una richiesta UMA per un'azienda che possiede più di 500 appezzamenti validi per il 2025 (che abbiano le particelle associate o l'indirizzo in Umbria)")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("25 Luglio 2025", "140.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Security,
                            "SQL_Dataprovider LanciaEccezioneSuInjection",
                            "Aggiunta lettura da DB del parametro LanciaEccezioneSuInjection",
                            cliente:="Coldiretti",
                            idTicketAssistenza:=184216, idTicketSviluppo:=0, idTicketTesting:=0,
                            autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("16 Luglio 2025", "140.1.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Rendicontazione terzista 2025",
                           "Ora la presenza di un conto proprio che ha piva uguale al CUAA in una rendicontazione terzista 2025 non blocca più il controllo sugli appezzamenti",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Casa
                           )

            '------------------------------------------------------------------------------------------------


            Riga_Versione("10 Luglio 2025", "139.5.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Esclusione impianti cessati",
                           "Ora gli impianti con 'FlagCessata' ipostato non verranno considerati nelle pratiche UMA",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=182987, idTicketTesting:=0,
                           autore:=DEV_Casa,
                           noteTest:="Test da fare insieme a qualcuno che possa modificare il flag lato DB"
                           )

            '------------------------------------------------------------------------------------------------


            Riga_Versione("02 Luglio 2025", "139.4.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Performance,
                           "Ottimizzata query Dettaglio Colture",
                           "Ulteriormente ottimizzato il caricamento del Dettaglio Colture",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=177979, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Casa,
                           noteTest:="test effettuato su richiesta 2025 di Lancioni Massimo (Approvata con successo) nei macrousi dei PRATI AVVICENDATI e SILVICOLTURA E MANUTENZIONE BOSCHI"
                           )

            '------------------------------------------------------------------------------------------------

            Riga_Versione("27 Giugno 2025", "139.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Corretta visualizzazione di colonne nascoste Dettaglio Appezzamenti",
                           "Corretta visualizzazione delle colonne Superficie tenace, Centro, Validità inizio appezzamento e 
                           validità fine appezzamento nella tabella del Dettaglio Appezzamenti che precedentemente erano erroneamente nascoste di default",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Casa,
                           noteTest:=""
                           )

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "UMA richieste 2025 senza catasto",
                           "Verifica degli eventuali nuovi impianti da associare ad una richiesta e da utilizzare per il controllo sulle superfici, e degli impianti non più esistenti per l'azienda e quindi da disassociare e non considerare più per il controllo stesso.",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=163342, idTicketTesting:=0,
                           autore:="Andrea Novaga",
                           noteTest:=""
                           )

            '------------------------------------------------------------------------------------------------

            Riga_Versione("20 Giugno 2025", "139.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Corretta query colture senza fascicolo",
                           "Corretta query per trovare le colture dal 2025 in poi per includere appezzamenti in Umbria senza particelle.",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Casa,
                           noteTest:=""
                           )

            '------------------------------------------------------------------------------------------------

            Riga_Versione("17 Giugno 2025 BIS", "139.1.2")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Consultazione dati azienda e situazione da richiesta/rendicontazione",
                           "Corretta lettura dati anagrafici Approvatore per rendicontazioni.",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=180587, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Andrea Novaga",
                           noteTest:=""
                           )

            '------------------------------------------------------------------------------------------------

            Riga_Versione("17 Giugno 2025", "139.1.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Security,
                           "Librerie di terze parti",
                           "Rimozione di file di librerie di terze parti che erano state incluse ma mai usate",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Giulia Bottan",
                           noteTest:=""
                           )

            '------------------------------------------------------------------------------------------------

            Riga_Versione("13 Giugno 2025", "139.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(
                           enum_Tipo_Changelog.Bug,
                           "Errore sovrapposizione al passaggio di stato",
                           "Corretto errore al passaggio di stato che controllava erroneamente le sovrapposizioni anche quando non dovrebbe",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=179855, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Casa,
                           noteTest:=""
                           )

            '------------------------------------------------------------------------------------------------

            Riga_Versione("10 Giugno 2025", "139.0.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(
                           enum_Tipo_Changelog.Performance,
                           "Ottimizzazione query dettaglio colture",
                           "Ottimizzata esecuzione della query del dettaglio colture",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=177979, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Casa,
                           noteTest:="Da testare bene anche la creazione di una richiesta (perché utilizza in parte la query che è stata ottimizzata)"
                           )

            Riga_Changelog(
                           enum_Tipo_Changelog.Bug,
                           "Ripristino codice",
                           "Ripristinato codice a seguito di errato merge (Hash 576bea80cfbb9569b9ad078aea9a0caa96ddeee7 WIP Trattamenti)",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_LC,
                           noteTest:="Non testabile"
                           )

            '------------------------------------------------------------------------------------------------

            Riga_Versione("09 Giugno 2025", "139.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(
                           enum_Tipo_Changelog.Bug,
                           "Configurazione UMA - associazione macrousi Set Aside",
                           "Corretto problema che impediva l'associazione di una qualunque riga a 'Set Aside e Condizionalità'",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=178578, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Casa,
                           noteTest:=""
                           )

            Riga_Changelog(
                           enum_Tipo_Changelog.Bug,
                           "Bugfix allineamento con documentazione inserita",
                           "Corretto bug che consentiva l'allineamento anche se presente la documentazione che impedisce la modifica",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=179183, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Casa,
                           noteTest:=""
                           )

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

            '------------------------------------------------------------------------------------------------

            Riga_Versione("05 Giugno 2025 BIS", "138.3.3")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Fix pulsante dettaglio appezzamento",
                "Rimossi ulteriori casi di duplicati",
                cliente:="Regione Umbria",
                idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=176384,
                autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("05 Giugno 2025", "138.3.2")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Consultazione dati azienda e situazione da richiesta/rendicontazione",
                "Corretta la lettura per i valori di recupero accisa dichiarati e confermati: dati ricavati sia da richieste che da rendicontazione per anno selezionato.",
                cliente:="Regione Umbria",
                idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=175326,
                autore:="Andrea Novaga")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("03 Giugno 2025", "138.3.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Consultazione dati azienda e situazione da richiesta/rendicontazione",
                "Correzione bug su query.",
                cliente:="Regione Umbria",
                idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=175326,
                autore:="Andrea Novaga")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Fix pulsante dettaglio appezzamento",
                "- Rimossi i fascicoli di anni superiori al 2024.
                - Corrette le funzionalità sulla griglia del 2024 (pagina 0 di 0, nessun elemento, ecc.).
                - Aggiunto nome appezzamento griglia 2025 ed eliminato i duplicati (ora, a parità di macrouso, i risultati coincidono con il dettaglio macrouso)",
                cliente:="Regione Umbria",
                idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=176384,
                autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("30 Maggio 2025", "138.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "UMA Miglioramento gestione sessioni",
                "Al momento della caduta della sessione, il comportamento di tutte le pagine UMA è stato reso coerente col resto dei siti (ossia redirect alla login in 5 secondi)",
                cliente:="Regione Umbria",
                idTicketAssistenza:=168301, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Casa,
                noteTest:="Da testare insieme ad uno sviluppatore che può riavviare il pool UMA")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Consultazione dati azienda e situazione da richiesta/rendicontazione",
                "Corretta la lettura dei dati, sia per aziende in conto proprio che per terzisti.",
                cliente:="Regione Umbria",
                idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=175326,
                autore:="Andrea Novaga")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("26 Maggio 2025", "138.2.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Velocizzazione pulsante 'Consultazione lavorazioni terzisti'",
                "La funzione chiamata da questo pulsante è stata notevolmente velocizzata",
                cliente:="Regione Umbria",
                idTicketAssistenza:=176696, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Casa,
                noteTest:="Da testare molto bene anche tutte le sovrapposizioni in quanto una parte della funzione ottimizzata era condivisa con quella sezione")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                "Aggiunto criterio per localizzazione appezzamento UMA",
                "Ora viene determinato se un appezzamento si trova in Umbria o regioni limitrofe anche se non possiede particelle associate",
                cliente:="Regione Umbria",
                idTicketAssistenza:=176605, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Casa,
                noteTest:="")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("23 Maggio 2025", "138.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Duplicazione righe colture UF fuori regione",
                "Corretto bug che duplicava le colture in Umbria nella griglia delle Colture UF con la dicitura 'Esterno Umbria'",
                cliente:="Regione Umbria",
                idTicketAssistenza:=176605, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Visualizzazione pulsanti colture UF",
                "Corretto bug che non solo mostrava cliccabili i pulsanti 'Aggiungi colture' ed 'Elimina tutto' anche in verifica in corso, ma li rendeva cliccabili anche da disabilitati",
                cliente:="Regione Umbria",
                idTicketAssistenza:=176867, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Casa,
                noteTest:="Testare tutte le pratiche conto proprio in ognuna delle fasi possibili e con/senza i documenti obbligatori che bloccano la pratica")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Richieste UMA 2025 per aziende con oltre 1000 appezzamenti",
                "Corretto bug che non consentiva la corretta apertura di una richiesta nel caso l'azienda abbia più di 1000 appezzamenti",
                cliente:="Regione Umbria",
                idTicketAssistenza:=176668, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Casa,
                noteTest:="Testato su Bondi Claudio, stessa azienda del ticket")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Consultazione dati azienda e situazione da richiesta/rendicontazione",
                "Ridotta la spaziatura fra etichette e campi di testo nella pagina modale Sintesi UMA. Invertita la posizione testi fra benzina e gasolio serra.",
                cliente:="Regione Umbria",
                idTicketAssistenza:=0, idTicketSviluppo:=163352, idTicketTesting:=0,
                autore:="Andrea Novaga")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("21 Maggio 2025", "138.1.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(
                enum_Tipo_Changelog.Feature,
                "UMA Ricarica griglia all'uscita delle Analisi del Terreno",
                "- Cliccando il pulsante Analisi del Terreno ora viene visualizzato un messaggio che impedisce il proseguimento in caso di modifiche non salvate sulla griglia delle colture.
                - All'uscita della sezione Analisi del Terreno, ora viene sempre ricaricata la griglia delle colture (e si controllano eventuali disallineamenti tra le superfici UMA e quelle in anagrafica)",
                cliente:="Regione Umbria",
                idTicketAssistenza:=0, idTicketSviluppo:=174449, idTicketTesting:=0,
                autore:=DEV_Casa
                )

            '------------------------------------------------------------------------------------------------

            Riga_Versione("16 Maggio 2025", "138.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                "Pulsante Dettaglio Appezzamento per pratiche 2025",
                "Corretto funzionamento del pulsante Dettaglio Appezzamento per le pratiche dal 2025 in poi",
                cliente:="Regione Umbria",
                idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Casa
                )

            '------------------------------------------------------------------------------------------------

            Riga_Versione("12 Maggio 2025", "138.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                "Invio Log Elastic Search - LOG APPLICATIVI (DataProvider)",
                "Rimosso possibile loop infinito in caso di errore di invio a ElasticSearch",
                cliente:="",
                idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Funcy
                )

            Riga_Changelog(
                enum_Tipo_Changelog.Feature,
                "Consultazione dati azienda e situazione da richiesta/rendicontazione",
                "Resa possibile la visione di insieme dell'attività aziendale, direttamente dalla pagina di una richiesta/rendicontazione, tramite nuovo pulsante 'SINTESI UMA'.
                Il nuovo pulsante prende il posto del pulsante 'SINTESI RICHIESTE E RENDICONTAZIONI', che viene nascosto e la relativa pagina diviene raggiungibile solo da menù.
                Informazioni visualizzate:
                    - Dati anagrafici e contatti dell'azienda
                    - Litri di carburante assegnati l'anno precedente e rendicontati (con richiedente ed approvatore)
                    - Rimanenza dichiarata per l'anno precedente 
                    - Recuperi di accisa dichiarati e confermati 
                    - Litri assegnati nell'anno corrente, con le varie richieste (anticipo, prima richiesta ed eventuali integrazioni, con richiedenti ed approvatori per ciascuna) 
                    - Litri acquistati e acquistabili (al netto delle rimanenze) nell'anno corrente, al momento dell'interrogazione.
                L'anno di riferimento è selezionabile fra gli anni per i quali risultano richieste, e le informazioni visualizzate dipendono da questo.
                Le informazioni dipendono anche se si sta visualizzando l'azienda come conto proprio o come terzista (a seconda del tipo di richiesta della pagina).
                Per ogni dato inerente i litri di carburanti, vengono riportati separatamente (se presenti) i litri di gasolio, benzina e gasolio serra.",
                cliente:="Regione Umbria",
                idTicketAssistenza:=0, idTicketSviluppo:=163352, idTicketTesting:=0,
                autore:="Andrea Novaga"
                )

            '------------------------------------------------------------------------------------------------

            Riga_Versione("09 Maggio 2025", "137.3.5")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Integrazioni di richieste logica terzisti",
                "In rendicontazione controlla solo i casi in cui o il terzista o il conto proprio sono in stato approvato, per rispecchiare la logica delle normali sovrapposizioni",
                cliente:="Regione Umbria",
                idTicketAssistenza:=0, idTicketSviluppo:=163351, idTicketTesting:=0,
                autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("08 Maggio 2025 BIS", "137.3.4")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Integrazioni di richieste errore tipo sovrapposizione",
                "- Corretto bug che segnava una sovrapposizione di tipo superficie (le vecchie sovrapposizioni) nel caso 
                in cui i litri richiesti precedentemente fossero maggiori del carburante calcolato nella pratica attuale (a seguito di una diminuzione della superficie);
                - Corretto bug in cui in rendicontazione conto proprio non leggeva il carburante richiesto in rendicontazione conto terzi",
                cliente:="Regione Umbria",
                idTicketAssistenza:=0, idTicketSviluppo:=163351, idTicketTesting:=0,
                autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("08 Maggio 2025", "137.3.3")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Integrazioni di richieste passaggio di stato",
                "Corretto bug che impediva erroneamente il passaggio di stato nella prima richiesta verso la compilazione completata",
                cliente:="Regione Umbria",
                idTicketAssistenza:=0, idTicketSviluppo:=163351, idTicketTesting:=0,
                autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("06 Maggio 2025", "137.3.2")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Correzione JOIN Gis_Entita",
                "Le query UMA non andranno più in JOIN con la tabella GIS_Entita",
                cliente:="Regione Umbria",
                idTicketAssistenza:=174386, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Casa,
                noteTest:="Testare che il funzionamento per le richieste/rendicontazioni UMA 2024 rimanga invariato."
                )

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                          "Associazione Analisi Su Particelle - Pratiche Pre 2025",
                          "Corretta lettura tessitura terreno da Analisi associata a particelle, per le sole Pratiche pre 2025",
                          cliente:="Regione Umbria",
                          idTicketAssistenza:=174366, idTicketSviluppo:=0, idTicketTesting:=0,
                          autore:=DEV_Funcy,
                          noteTest:="- Creare un analisi, valida dal 2024 in poi, con tessitura TENACE sulle particelle con sezione e/o subalterno non indicati  
                           - aprire una pratica valida nel 2024
                           --> L'analisi deve essere considerata e le tessiture devono essere tenaci (dove sensato).
                           - Modificare l'analisi in modo che la tessitura sia media
                           - Rimuovere e reinserire ogni riga dei macrousi
                           --> Le tessiture devono essere aggiornate
                           NB. la superficie totale per ogni macrouso deve corrispondere a quella indicata nel riepilogo (pulsante DETTAGLI in riga)
                                                 
                           Riepilogo valori tessiture:
                           tessitura normale => sabbia = 72	 argilla = 2
                           tessitura media   => sabbia = 31	 argilla = 27
                           tessitura tenace  => sabbia = 7   argilla = 70")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("05 Maggio 2025", "137.3.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Aggiunti controlli UMA richieste/rendicontazioni",
                "Aggiunti controlli UMA per evitare che una riduzione della superficie 
                di un terreno, data dalla modifica del piano colturale, porti a richiedere più carburante di quanto se ne avrebbe diritto",
                cliente:="Regione Umbria",
                idTicketAssistenza:=0, idTicketSviluppo:=163351, idTicketTesting:=0,
                autore:=DEV_Casa,
                noteTest:="Per testare questa parte è necessario creare una richiesta UMA 2025, inserire delle lavorazioni e portarla in approvazione e 
                poi in verifica completata con successo ma approvando solo una parte (o nessuno) dei litri richiesti per una o più lavorazioni.
                Dopodiché aprire una richiesta integrativa per la stessa azienda e inserire per le lavorazioni precedenti i litri mancanti. Allora procedere 
                con una riduzione notevole della superficie dell'impianto corrispondente e tornare nella schermata di richiesta UMA per 'Allineare' i dati.
                Aprire il macrouso interessato e verificare che sia segnato in rosso. Lo stesso test dovrà essere fatto in rendicontazione conto proprio utilizzando una 
                rendicontazione conto terzi come 'integrativa' (i controlli vengono eseguiti sia che la rendicontazione terzista sia in stato di compilazione sia che sia stata completata)")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Corretto fascicolo non utilizzato in questa pratica",
                "Corretto errore che mostrava il messaggio 'Fascicolo non utilizzato in questa pratica' quando si prova ad inserire le colture nella tab degli allevamenti per il calcolo delle UF",
                cliente:="Regione Umbria",
                idTicketAssistenza:=173971, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("30 Aprile 2025", "137.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Nascosto pulsante Dettaglio Appezzamenti",
                      "Nascosto pulsante 'Dettaglio Appezzamenti' per richieste carburanti UMA dal 2025 in poi",
                      cliente:="Regione Umbria",
                      idTicketAssistenza:=173239, idTicketSviluppo:=0, idTicketTesting:=0,
                      autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "UMA calcolo tessitura terreno",
                           "Corretta lettura tessitura terreno da analisi: ora vengono solo considerate le analisi valide nell'anno della richiesta.",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=173872, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Funcy,
                           noteTest:="- Creare un analisi valida dal 2026 in poi con tessitura TENACE sugli appezzamenti    
                           - aprire una pratica valida nel 2025
                           --> L'analisi NON deve essere considerata.
                           - Modificare la validità dell'analisi in modo che copra l'anno della richiesta
                           - Ricaricare la pagina della richiesta
                           --> Se le tessiture sono differenti, deve venir proposta la correzione delle superfici
                           - Provare anche a modificare nuovamente la data inizio analisi al 2026
                           -- Ricaricando la richieste deve essere riproposta la correzione delle superfici
                                                 
                           Riepilogo valori tessiture:
                           tessitura normale => sabbia = 72	 argilla = 2
                           tessitura media   => sabbia = 31	 argilla = 27
                           tessitura tenace  => sabbia = 7  	 argilla = 70")

            '------------------------------------------------------------------------------------------------


            Riga_Versione("24 Aprile 2025", "137.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "UMA fix ricerca macrousi prima del 2025",
                      "Corretta classificazione dei macrousi all'apertura di una nuova richiesta/rendicontazione di un anno precedente al 2025",
                      cliente:="Regione Umbria",
                      idTicketAssistenza:=172990, idTicketSviluppo:=0, idTicketTesting:=0,
                      autore:=DEV_Casa,
                      noteTest:="Testare sul caso descritto nel ticket.")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("14 Aprile 2025", "137.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "UMA integrazione di richieste",
                      "Ora il CAA può richiedere, all'interno di richieste integrative, il quantitativo di carburante che AFOR non ha concesso nelle precedenti richieste (dello stesso anno). Questo meccanismo è applicato anche
                      alle rendicontazioni con il carburante che AFOR non ha assegnato ai terzisti che dichiarano la lavorazione su una specifica azienda e la rendicontazione di quella azienda",
                      cliente:="Regione Umbria",
                      idTicketAssistenza:=0, idTicketSviluppo:=163351, idTicketTesting:=0,
                      autore:=DEV_Casa,
                      noteTest:="Affinché questo nuovo tipo di sovrapposizione venga utilizzata, è necessario che le superfici specificate nelle lavorazioni interessate coincidano con la superficie totale
                      di quel Macrouso. Esempio: su 10 Ha di FRUTTA POLPOSA richiedo un'aratura da 10 Lt, di cui AFOR ne approva solo 5. Posso aprire una richiesta integrativa e inserire un'aratura
                      su 10 Ha di FRUTTA POLPOSA e il sistema dovrebbe concedere la richiesta di al massimo 5 Lt.
                      Nel caso le superfici siano diverse, viene utilizzato il vecchio sistema di sovrapposizioni. Testare bene l'utilizzo di lavorazioni 'Max N volte'")

            '------------------------------------------------------------------------------------------------


            Riga_Versione("04 Aprile 2025", "136.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Inserimento lavorazioni multiple UMA",
                           "Corretto bug che impediva la chiusura della finestra di inserimento delle lavorazioni multiple, corretto bug che mostrava tutte le lavorazioni possibili invece che solo quelle relative al macrouso selezionato",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=169948,
                           idTicketSviluppo:=0,
                           idTicketTesting:=0,
                           autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Esportazione Excel UMA Vendite",
                           "Aggiunta opzione per esportare la griglia UMA Vendite in Excel",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0,
                           idTicketSviluppo:=0,
                           idTicketTesting:=0,
                           autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("27 Marzo 2025", "136.1.3")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Controllo validità appezzamenti",
                           "Modificato il controllo: ora si guarda il 'Codice Campagna' di ogni impianto (anno di validità dell'impianto) e confrontato con l'anno della pratica UMA",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0,
                           idTicketSviluppo:=0,
                           idTicketTesting:=168128,
                           autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("26 Marzo 2025", "136.1.2")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Irrigazioni straordinarie",
                           "Bugfix: sia con lavorazioni di irrigazione appena create che già presenti, è possibile salvare la richiesta senza specificare MC o Note Permesso Acqua, 
                           se si tratta di richiesta integrativa e con MC già richiesti in precedenza.",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0,
                           idTicketSviluppo:=0,
                           idTicketTesting:=168129,
                           autore:="Andrea Novaga")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("25 Marzo 2025", "136.1.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "UMA proposta Fascicolo anno precedente",
                           "Alla creazione di una richiesta/rendicontazione di un anno precedente al 2025 ora propone correttamente solo i fascicoli dell'anno selezionato",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0,
                           idTicketSviluppo:=0,
                           idTicketTesting:=164157,
                           autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Irrigazioni straordinarie",
                           "Correzioni:
                           - non è più obbligatorio specificare un valore di MC d'acqua maggiore di 0 o delle note permesso d'acqua, quando si tratta di MC aggiuntivi in richiesta integrativa 
                           - il controllo in inserimento di nuova lavorazione di tipo irrigazione, tiene ora conto di tutte le altre irrigazioni per la richiesta, per il valore di Richiesto (lt.).",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0,
                           idTicketSviluppo:=0,
                           idTicketTesting:=168129,
                           autore:="Andrea Novaga")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("21 Marzo 2025", "136.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "UMA Lettura scalare tessitura",
                           "Fix lettura scalare tessiture da n particelle, quando la loro somma = superficie impianto",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0,
                           idTicketSviluppo:=0,
                           idTicketTesting:=164616,
                           autore:=DEV_Funcy,
                           noteTest:="Avere un impianto associato a due particelle
                           LA somma delle aree delle particelle deve essere identica alla superficie impianto
                           Registrare analisi del terreno sulle singole particelle, in modo che abbiano tessiture diverse.
                           Verificare sull'UMA che la lettura scalare sia eseguita correttamente.
                           esempi tessitura:
                           tessitura normale => sabbia = 72	 argilla = 2
                           tessitura media   => sabbia = 31	 argilla = 27
                           tessitura tenace  => sabbia = 7   argilla = 70")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Verifica tessiture e pendenze",
                           "Quando la richiesta viene aperta in modifica vengono fatti i seguenti controlli per verificare possibili variazioni di pendenze e tessiture, rispetto alla data di ultima modifica:
                           - Ultima modifica/cancellazione appezzamento, impianto, esercizio, catasto
                           - Ultima modifica/cancellazione analisi del terreno",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0,
                           idTicketSviluppo:=0,
                           idTicketTesting:=164636,
                           autore:=$"{DEV_Funcy} e Andrea Novaga",
                           noteTest:="")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("17 Marzo 2025", "136.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Modifica al controllo per validita' appezzamenti pratiche UMA 2025",
                      "Modificato il controllo di validità per includere tutti gli appezzamenti il cui periodo di validità includeva una parte dell'anno della pratica UMA.",
                      cliente:="Regione Umbria",
                      idTicketAssistenza:=0, idTicketSviluppo:=163348, idTicketTesting:=0,
                      autore:=DEV_Casa,
                      noteTest:="Casi resi validi: 
                        -inizio validità < anno pratica e fine validità > anno pratica,
                        -inizio validità < anno pratica e fine validita all'interno dell'anno pratica,
                        -inizio validità all'interno dell'anno pratica e fine validità > anno pratica.")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Irrigazioni straordinarie",
                           "Resi modificabili i mc di Permesso Acqua richiedibili anche per le richieste integrative, in modo tale da poter richiedere una quantità aggiuntiva d'acqua, rispetto quanto già richiesto.",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=163349, idTicketTesting:=0,
                           autore:="Andrea Novaga",
                           noteTest:="Per richieste integrative, il campo 'Totale permesso acqua (mc)' deve ora essere editabile, ma con nuova etichetta 'Mc aggiuntivi permesso acqua (mc)', e non riportare quindi più il valore della prima richiesta. 
                          Il label soprastante ('Permesso attingimento acqua...') invece, deve ora riportare il totale di quanto inserito in prima richiesta ed in eventuali altre richieste integrative, precedenti alla presente, già approvate. 
                          Il controllo in inserimento di nuove lavorazioni di irrigazione, deve tenere conto dei mc di acqua totali ancora disponibili, data la quantità totale di mc richiesti (sia con la presente richiesta che con quelle precedenti) 
                          e di quelli già utilizzati per altre lavorazioni di irrigazioni (sia nella presente richiesta che in quelle precedenti). In caso di sforamento, deve comunque dare errore in fase di salvataggio. ")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                          "Visibilità Aziende UMA",
                          "Pagina 'Visibilità Aziende UMA' (Utenti_Visibilita_Area.aspx) portata dal sito di profilazione a quello di Agronica UMA.",
                          cliente:="Regione Umbria",
                          idTicketAssistenza:=0, idTicketSviluppo:=163349, idTicketTesting:=0,
                          autore:="Andrea Novaga",
                          noteTest:="Il pulsante di menù 'Visibilità Aziende UMA' deve portare alla pagina nel sito dell'UMA (/AgronicaUMA/CarburantiUMA/Utenti_Visibilita_Area.aspx) invece che su quello di profilazione.")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "UMA passaggio indesiderato da elenco rendicontazioni a elenco richieste",
                      "Corretto il raro caso in cui, trovandosi nell'elenco rendicontazioni ed entrando in una rendicontazione, tornando indietro ci si ritrova nell'elenco richieste",
                      cliente:="Regione Umbria",
                      idTicketAssistenza:=167582, idTicketSviluppo:=0, idTicketTesting:=0,
                      autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "UMA problema arrotondamento lavorazioni",
                      "Corretto problema di arrotondamento all'inserimento di lavorazioni UMA che produceva un messaggio di superfici non coincidenti di 0.0000 Ha",
                      cliente:="Regione Umbria",
                      idTicketAssistenza:=167687, idTicketSviluppo:=0, idTicketTesting:=0,
                      autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("12 Marzo 2025", "135.3.3")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "UMA Lettura scalare pendenze",
                           "Corretta lettura delle pendenze nel caso di superficie impianto diversa da superficie appezzamento, presenza di una o più particelle catastali senza specificazione di pendenza 
                           e pendenza dell'appezzamento superiore al 10%",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=164641,
                           autore:=DEV_Casa)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("11 Marzo 2025", "135.3.2")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "UMA superfici impianti e pendenze",
                           "Ora, all'interno delle richieste/rendicontazioni, vengono sempre considerate le superfici dell'impianto piuttosto che quelle dell'appezzamento in caso di discordanza tra le due.
                           In caso di superfici discordanti e di molteplici particelle, la superficie dell'impianto viene considerata della pendenza a costo minore tra quelle presenti nelle particelle.",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=164636,
                           autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "UMA tessiture impianti",
                           "Durante il controllo sulle tessiture vengono sempre considerate le superfici degli impianti al posto degli appezzamenti.
                           In caso di analisi su particelle: 
                           se la somma delle superfici delle particelle è maggiore della superficie impianto, viene presa la tessitura con costo minore (costo normale < media < tenace) e la si applica a tutta la superficie dell'impianto",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=164636,
                           autore:=DEV_Funcy)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "UMA richieste - lavorazioni",
                      "Corretto l'aggiornamento dei fabbisogni di carburante per le lavorazioni, quando ne è necessario l'aggiornamento in seguito a quello delle superfici dichiarate, tramite pulsante Correggi.
                      Corretto anche l'aggiornamento del carburante calcolato e richiesto delle righe di richiesta per macrouso, in base al fabbisogno totale delle relative lavorazioni, e di quello totale in testata.",
                      cliente:="Regione Umbria",
                      idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=164636,
                      autore:="Andrea Novaga")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("07 Marzo 2025", "135.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "UMA richieste 2025 senza catasto",
                      "Corretto l'aggiornamento, tramite pulsante, delle superfici dichiarate, in modo tale che vengano ricalcolati correttamente anche i fabbisogni carburante per le lavorazioni",
                      cliente:="Regione Umbria",
                      idTicketAssistenza:=164636, idTicketSviluppo:=0, idTicketTesting:=0,
                      autore:="Andrea Novaga")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("05 Marzo 2025", "135.2.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "UMA richieste 2025 senza catasto",
                      "Corretta la lettura dei macorusi rendendo il catasto completamente opzionale (in un caso residuo ancora non lo era)",
                      cliente:="Regione Umbria",
                      idTicketAssistenza:=164636, idTicketSviluppo:=0, idTicketTesting:=0,
                      autore:="Giacomo Casadei")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("28 Febbraio 2025", "135.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Controllo per data limite inserimento nuovi CUAA in rendicontazione terzisti",
                      "Corretto il suddetto controllo per farlo funzionare anche per i terzisti (prima funzionaza solo per le cooperative, ora non piu' gestite)",
                      cliente:="Regione Umbria",
                      idTicketAssistenza:=165700, idTicketSviluppo:=0, idTicketTesting:=0,
                      autore:="Giacomo Casadei")

            '------------------------------------------------------------------------------------------------

            Riga_Versione("21 Febbraio 2025", "135.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Porting su piano colturale effettivo invece che sul planning",
                      "Corretto allineamento superfici dichiarate in richiesta, tramite pulsante Correggi Tutti: verificato che i dettagli di pendenze e tessiture, per le lavorazioni, non superino mai quelli della relativa richiesta per gruppo colturale.
                      Per lavorazioni che non coinvolgono tutta la superficie dichiarata per gruppo colturale, i valori di pendenza e tessitura vengono eventualmente ridistribuiti (fra zona A e zona B per i primi, fra Normale, Medio e Tanace per gli altri) per non superare il limite.
                      Correzione effettuata per la query dei dettagli degli appezzamenti per gruppo colturale, sui valori di pendenza ritornati per appezzamento.",
                      cliente:="Regione Umbria",
                      idTicketAssistenza:=0, idTicketSviluppo:=163342, idTicketTesting:=0,
                      autore:="Andrea Novaga")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Modifica al controllo per rilascio rendicontazione",
                      "Aggiunti controlli sull'inserimento della data 'Data da cui permettere la chiusura rendicontazione conto proprio anche in mancanza di chiusura del terzista' in Configurazione UMA - Date Rendicontazione.",
                      cliente:="Regione Umbria",
                      idTicketAssistenza:=0, idTicketSviluppo:=163343, idTicketTesting:=0,
                      autore:="Giacomo Casadei",
                      noteTest:="Sono stati aggiunti i seguenti controlli: 
                      - Non è possibile inserire questa data in una riga con tipo azienda diverso da 'Azienda Agricola Privata',
                      - Non è possibile inserire questa data se inferiore alla data di inizio rendicontazione delle stessa riga,
                      - Non è possibile inserire questa data se superiore al termine ultimo rendicontazione delle stessa riga.")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Agro_Sequenze",
                           "Ripristino porzione di codice rimossa erroneamente",
                           "",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Funcy)

            '------------------------------------------------------------------------------------------------

            Riga_Versione("17 Febbraio 2025", "135.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Data_Fine_Blocco_Rendic_Conto_Proprio in UMA_Setup_Date_Rendicontazione",
                           Ver:="782")

            Riga_Requisiti("Aggancio",
                           "per UMA", "131")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Requisiti("WebService",
                           "per estrazione tessitura da parametri analisi UMA", "17/02/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Modifica al controllo per rilascio rendicontazione",
                           "Aggiunto nuovo campo in Configurazione UMA, per permettere alle aziende di rilasciare rendicontazioni in conto proprio, a partire da una certa data ed a prescindere dal fatto che siano inserite in rendicontazioni ancora aperte di terzisti",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=163343, idTicketTesting:=0,
                           autore:="Andrea Novaga",
                           noteTest:="In 'Configurazione UMA' -> 'Date Rendicontazione', impostare un valore per 'Data da cui permettere la chiusura rendicontazione conto proprio anche in mancanza di chiusura del terzista', per tipo azienda 'Azienda Agricola Privata' 
                      ed anno richiesta a scelta, verificare che per quell'anno il valore di Data Rendicontazione Fine per Azienda Terzista non sia già passata, e provare a creare nuova rendicontazione in conto proprio per un'azienda citata in una rendicontazione 
                      terzista ancora aperta (cioè non in stato 'Verifica intermedia completata con Successo / non Superata' o 'Rinuncia') e dello stesso anno: la creazione deve essere consentita solo se la data impostata nel nuovo campo risulta <= quella corrente. 
                      Effettuare lo stesso tipo di controllo anche per il passaggio di stato di una rendicontazione in conto proprio già esistente: considerare una rendicontazione in conto proprio di un'azienda citata in rendicontazione terzista ancora aperta 
                      e verificare che il passaggio di stato sia consentito solo alla stessa condizione. ")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Modifica al controllo Data Documento e Tipo Documento in Vendita Carburanti",
                           "- Il campo Note Rivenditore diventa obbligatorio se Data Documento è antecedente a Data Creazione, inoltre le righe in cui tale condizione è verificata vengono evidenziate di rosso
                      - I valori per il campo Tipo Documento sono stati cambiati da 'DAS' e 'Fattura accompagnatoria' a 'E-DAS' e 'Doc. cartaceo (causa di forza maggiore)', rispettivamente.",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=163344, idTicketTesting:=0,
                           autore:="Andrea Novaga",
                           noteTest:="Verificare sia per inserimento che per modifica record esistenti in Vendita Carburanti che, nel caso in cui Data Documento sia minore di Data Creazione, venga visualizzato un messaggio di errore nel caso in cui si provi a salvare senza impostare il campo Note Rivenditore 
                      (ed il campo non può essere di soli spazi). Anche in caso di inserimento/modifica, con campo Note settato, il record deve essere evidenziato in rosso con un messaggio sopra la griglia (se sono già presenti record con questa condizione, all'apertura pagina devono già essere evidenziati).")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Porting su piano colturale effettivo invece che sul planning",
                      "Per richieste carburante e rendicontazioni (queste solo per terzisti) a partire dall'anno 2025, la gestione avviene tramite il PCG effettivo, il raggruppamento in griglia avviene solo per Macrouso e non più anche per fascicolo.
                      E' stato implementato un controllo per verificare che il totale delle superfici dichiarate in richiesta o rendicontazione, e per poterle eventualmente riallineare.
                      Non deve essere possibile procedere con l'avanzamento di stato, se ci sono disallineamenti e la richiesta è ancora in compilazione,.",
                      cliente:="Regione Umbria",
                      idTicketAssistenza:=0, idTicketSviluppo:=163342, idTicketTesting:=0,
                      autore:="Giacomo Casadei",
                    noteTest:="- Verificare il corretto funzionamento delle pagine di richiesta carburanti che rendicontazione (creazione o modifica), sia per anno 2025 che precedenti
                      - Verificare, per la pagina di richiesta carburanti, la corretta apertura e funzionamento della finestra dei dettagli richiesta
                      - Provare a modificare la superficie di un impianto collegato una richiesta e macrouso/gruppo (o più macrousi) colturale UMA specifico: deve essere visualizzato il messaggio di errore che riporta i macrousi con le superfici non più allineate, oltre al pulsante per la correzione
                      - Se è visualizzato il messaggio di errore non deve essere possibile procedere con l'avanzamento della pratica, se lo stato è ancora In Compilazione
                      - Alla pressione del pulsante Correggi Tutti, le superfici dichiarate devono essere allinate come indicato, ed il messaggio non deve essere più presente
                      - Se sono presenti lavorazioni per richieste coinvolte nel riallineamento, anche queste devono essere riallineate correttamente (se si tratta di lavorazioni sull'intera superficie della richeista, devono essere allineate allo stesso modo)")


            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Estrazione tessitura terreno",
                      "La tessitura del terreno viene estratta a scalare in questo:
                      1) Analisi su Appezzamento
                      2) Analisi su Campo associato ad appezzamento
                      3) Analisi sul catasto dell'appezzamento
                      4) Valori di sabbia-limo-argilla-tessitura salvati su Anagrafica Appezzamento
                      5) Analisi sul centro aziendale
                      
                      Se a parità di livello esistono più analisi con sabbia-limo-argilla, viene fatta una media aritmetica dei valori e il ricalcolo della tessitura in base ai nuovi valori.
                      Esempio: 
                      Analisi 1: Sabbia 50, Limo 20, Argilla 30 
                      Analisi 2: Sabbia 60, Limo 25, Argilla 15
                      Valori Calcolo Tessitura: Sabbia 55, Limo 23 (arrotondato), Argilla 23 (arrotondato)
                      (Il punto (3) fa una media delle analisi per singola particella)

                      Poniamo che per un appezzamento esistano solo analisi associate alle particelle (3) e valori registrati sull'anagrafica (4):
                      Se l'appezzamento è associato a due particelle, di cui solo una ha un'analisi registrata, la tessitura della superficie rimanente sarà estratta a scalare partendo dal punto 4) in poi.",
                      cliente:="Regione Umbria",
                      idTicketAssistenza:=0, idTicketSviluppo:=163347, idTicketTesting:=0,
                      autore:=DEV_Funcy,
                      noteTecniche:="Per poter testare è necessario puntare i WebService del PianoConcimazione al link di TestInterni, allinearsi con lo sviluppatore al momento del test.
                      Quando il test è approvato bisogna aggiornare i WebService di Produzione (Giulia)")

            '------------------------------------------------------------------------------------------------

            Riga_Data("12 Febbraio 2025")

            Riga_Requisiti("Migra",
                           "-Aggiunta la colonna Validita_Inizio alla chiave primaria di UMA_UF_Colture
                           -Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Fix controllo per cancellazione UMA Rendicontazioni",
                "Modificato il controllo sui permessi per determinare se è possibile cancellare le rendicontazioni dall'elenco UMA",
                noteTest:="ID redmine 164154")

            '------------------------------------------------------------------------------------------------

            Riga_Data("10 Febbraio 2025")

            Riga_Requisiti("Migra",
                           "-Aggiunta la colonna Validita_Inizio alla chiave primaria di UMA_UF_Colture
                           -Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Cambio fine validita UMA Configurazione",
                "Modificato intervallo ammissibile di fine validità per Macrousi-Lavorazioni per includere tutte le date a partire dalla data di Inizio validità",
                noteTest:="ID redmine 163704")

            '------------------------------------------------------------------------------------------------

            Riga_Data("04 Febbraio 2025")

            Riga_Requisiti("Migra",
                           "Nuova tabella UMA_RichiestexReg_Impianti",
                           Ver:="781")

            Riga_Requisiti("Aggancio",
                           "per UMA", "130")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Rimozione visualizzazione alcuni stati pratiche UMA",
                        "Rimozione dalla visualizzazione degli stati UMA non più utilizzati")

            Riga_Bug("Bug in creazione e modifica anticipi",
                     "- Corretta la possibilità di modifica per anticipi creati in modalità forfettaria (valori dei campi carburante corretti ed editabli)
                     - Corretto il controllo, in creazione nuovo anticipo, che verifica l'esistenza di almeno una richiesta approvata dall'ultimo anticipo richiesto",
                     noteTest:="ID Redmine: 161920. ")

            '------------------------------------------------------------------------------------------------

            Riga_Data("31 Gennaio 2025")

            Riga_Requisiti("Migra",
                           "Nuova tabella UMA_RichiestexReg_Impianti",
                           Ver:="781")

            Riga_Requisiti("Aggancio",
                           "per UMA", "130")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("Bug pulsante salva in rendicontazione",
                     "Corretta verifica dei permessi per mostrare il pulsante salva nelle rendicontazioni",
                     idPerforma:=34644)

            Riga_Bug("Security - AgronicaCoreParametri",
                     "Fix per recuperare nome db anche in caso di stringa connessione codificata")

            '------------------------------------------------------------------------------------------------

            Riga_Data("24 Gennaio 2025")

            Riga_Requisiti("Migra",
                           "Nuova tabella UMA_RichiestexReg_Impianti",
                           Ver:="781")

            Riga_Requisiti("Aggancio",
                           "per UMA", "130")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Modifiche testi anticipo per aziende senza acquisti",
                        "Modificato il titolo della finestra degli anticipi e del messaggio in rosso in caso di azienda senza acquisti nel 2024")

            Riga_Bug("Bug inserimento lavorazione Configurazione UMA",
                     "Corretto bug che impediva l'inserimento di una nuova lavorazione in Elenco Lavorazioni all'interno della pagina 'Configurazione UMA'",
                     idPerforma:=34581)

            '------------------------------------------------------------------------------------------------

            Riga_Data("20 Gennaio 2025")

            Riga_Requisiti("Migra",
                           "Nuova tabella UMA_RichiestexReg_Impianti",
                           Ver:="781")

            Riga_Requisiti("Aggancio",
                           "per UMA", "130")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Autenticazione",
                      "Prima di ogni chiamata ai web method è stato introdotto un controllo di sicurezza (global asax)",
                            noteTest:="Verificare che una pagina qualsiasi di questo sito funzioni correttamente",
                            noteTecniche:="Il controllo di sicurezza viene effettuato solo quando il valore della chiave ControlloAutenticazioneConAuthCookie nei appsettings è uguale a true")

            Riga_Bug("Analisi del Terreno (new!) - Security",
                      "Fix SonarQube per controllo origine",
                      noteTest:="Ri-verificare 'Gestione Analisi' con salvataggio da UMA (gestione richieste, rendicontazioni)")

            Riga_Text("UMA Anticipi",
                      "- Consentita possibilità di effettuare richieste di anticipo per nuove aziende in modalità forfetario, per il solo anno 2025. 
                      - Consentita modifica delle richieste di anticipo, successivamente la loro creazione (finché non sono presenti richieste di carburanti successive). 
                      - Consentita modifica diretta sia della percentuale (entro valore consentito) che delle quantità di carburanti di anticipo. 
                      - La richiesta di un nuovo anticipo viene ora bloccata se non è presente almeno una richiesta approvata posteriore all'ultimo anticipo concesso.",
                      noteTest:="Effettuando una nuova richiesta di anticipo per l'anno 2025 e per un'azienda che non risulta avere mai effettuato precedenti richieste di carburante, la creazione della richiesta deve avvenire in modalità forfetaria, " &
                      "cioè con l'inserimento diretto dei valori di quantità carburante, senza campo percentuale. " &
                      "Nella pagina di elenco richieste, deve essere presente il pulsante Modifica anche per le righe degli anticipi, ma solo per gli anticipi per i quali l'azienda non ha fatto richieste di carburante successive (ad esempio anticipi appena creati), " &
                      "ed alla pressione del pulsante si deve aprire la pagina di modifica richiesta, come per le normali richieste. " &
                      "Sia per l'inserimento che per la modifica di un anticipo, deve essere possibile editare sia il campo Percentuale che quello della relativa quantità di carburante (gasolio, benzina o gasolio serra), e la modifica di uno si deve riflettere sull'altro; " &
                      "questo a meno che l'azienda non risulti nuova e richieda un inserimento forfetario, oppure abbia acquistato più tipi di carburanti lo scorso anno, ed in entrambi i casi tutti i campi quantità sono editabili ed il campo percentuale non è visibile. ")

            '------------------------------------------------------------------------------------------------

            Riga_Data("15 Gennaio 2025")

            Riga_Requisiti("Migra",
                           "-Aggiunta la colonna Validita_Inizio alla chiave primaria di UMA_UF_Colture
                           -Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("Bug assegnazione automatica rendicontazione terzisti",
                     "Risolto bug che impediva l'assegnazione automatica del carburante in una rendicontazione terzista",
                     idPerforma:=34348,
                     noteTest:="Per testare la situazione di errore, la percentuale di riduzione per l'anno della rendicontazione deve essere 0 e deve essere una rendicontazione appena arrivata in stato di verifica in corso (non deve avere avuto modifiche ai carburanti assegnati)")

            '------------------------------------------------------------------------------------------------

            Riga_Data("10 Gennaio 2025")

            Riga_Requisiti("Migra",
                           "-Aggiunta la colonna Validita_Inizio alla chiave primaria di UMA_UF_Colture
                           -Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("UMA Vendite con trasferimento senza assegnato",
                     "Corretto il calcolo dei litri acquistabili ",
                     idPerforma:=33957)

            Riga_Bug("Configurazione UMA Duplicazione riga setup",
                    "Corretto bug sulla duplicazione di una riga nella tabella di UMA configurazione setup che non duplicava correttmanete le ultime 5 colonne",
                        idPerforma:=34245)

            Riga_Text("Configurazione UMA colonna Limite Max in Configurazione Macrousi x Lavorazioni",
                    "Aggiunto un limite minimo (0) e un limite massimo (1) per la colonna Limite Max nella griglia Configurazione Macrousi x Lavorazioni in UMA Configurazioni")

            Riga_Bug("UMA Calcolo capi allevabili",
                        "Corretto raro caso in cui al salvataggio non veniva utilizzato il valore più recente dei capi allevati a seguito di una modifica")

            '------------------------------------------------------------------------------------------------

            Riga_Data("16 Dicembre 2024")

            Riga_Requisiti("Migra",
                           "-Aggiunta la colonna Validita_Inizio alla chiave primaria di UMA_UF_Colture
                           -Nuova colonna Flag_Encrypted in tabella Connessioni",
                           Ver:="778")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "-Aggiunta riferimento al web service di aggiornamento della tabella Codifica_Specie_Vegetali_Agea_2025_2020 in Configurazione UMA
                           -Aggiunta chiave 'UserPwdConnectionString_toCrypt", "150")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Riepilogo sviluppi giugno-novembre",
                      "- @1 Speed Up caricamento anno Segnalazione recupero accise " &
                      "- @2 Inserimento nuovo passaggio di stato e workflow relativo + aggiunta colonne dettaglio " &
                      "- @3 Gestione sovrapposizione segnalazione recupero accise " &
                      "- @4 Velocizzazione inserimento massivo di segnalazioni recupero accise " &
                      "- @5 Aggiunta colonne codici Agea e 'Produce UF' in griglia allevamenti UF " &
                      "- @7 UMA Ricerca Macrousi/Lavorazioni " &
                      "- @11 Aggancio Analisi Terreno x UMA " &
                      "- @18 Aggiornamento creazione righe Allevamenti UF in richiesta " &
                      "- @23 Modificata selezione data minima per validità fine UMA Macrousi Lavorazioni ",
                      noteTecniche:="per info dettagliate cercare @X nel file versione Branch UMA del sito UMA",
                      noteTest:="PROVENIENTE DAL RAMO UMBRIA - GIA' TESTATO")

            Riga_Bug("Riepilogo bugfix giugno-novembre",
                      "- @6 Fix colonna Approvatore in Elenco e disabilitazione pulsanti " &
                      "- @8 UMA fix dettaglio colture " &
                      "- @9  UMA fix permesso acqua integrative " &
                      "- @10 UMA fix azienda cessata ignora limiti temporali per rendicontazione " &
                      "- @13 Fix nomi tabelle " &
                      "- @14 Primi fix post test " &
                      "- @15 Fix sulle query " &
                      "- @16 Correzione elenco stati per coordinatore afor + bugfix vendite " &
                      "- @17 Correzione ddl associazione macrousi " &
                      "- @19 Fix inserimento righe configurazione MacrousixLavorazioni " &
                      "- @20 Correzione elenco stati per coordinatore afor " &
                      "- @21 Correzione visibilità elenco " &
                      "- @22 Ulteriore Correzione visibilità elenco " &
                      "- @24 Fix pre collaudo " &
                      "- @25 Fix post collaudo " &
                      "- @26 Fix pre rilascio in produzione " &
                      "- @27 Bugfix recupero accise minore di 0 " &
                      "- @28 Bugfix creazione di una lavorazione con lavorazioni previste maggiore di 1 " &
                      "- @29 Bugfix ingresso richiesta da ricerca macrousi/lavorazioni + bugfix rimessa in compilazione ",
                      noteTecniche:="per info dettagliate cercare @X nel file versione Branch UMA del sito UMA",
                      noteTest:="PROVENIENTE DAL RAMO UMBRIA - GIA' TESTATO")

            Riga_Text("Security",
                      "Attivata modalità nel web.config per criptare il viewstate delle pagine",
                      noteTest:="Non testabile")

            Riga_Text("Security",
                      "Interventi vari per evitare il passaggio della stringa di connessione al db (8507)")

            Riga_Bug("UMA Eliminazione cifre decimali da export Excel Sintesi",
                     "Sono stati convertiti tutti i valori decimali relativi ai quantitativi di carburante in valori interi nell'export su excel della griglia UMA Sintesi Richieste e Rendicontazioni",
                     cliente:="01212820540 - Regione Umbria", idPerforma:=33857)

            Riga_Text("Analisi del Terreno (new!)",
                     "Aggancio Analisi Terreno (new!) alla pagina UMA Richiesta Carburanti",
                     noteTest:="Richiesta/Rendicontazione, click sul pulsante 'Analisi Terreni', verificare la corretta apertura delle analisi terreno (new!)",
                     noteTecniche:="L'impostazione SUPERUSER_Mod_Analisi_Terreno (892) deve essere impostata con valore 2
                      Da interfaccia: Utenti e Permessi (new!) -> Impostazioni Superuser -> Impostazioni Superuser -> 'ALTRE IMPOSTAZIONI' -> 'Modalità richiamo Analisi del Terreno' -> 'Avanzata (Angular)'")


            '------------------------------------------------------------------------------------------------

            Riga_Data("12 Dicembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           Ver:="776")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Text("Codifica e decodifica password_smtp e PasswordArteaWS",
                      "-Introdotti metodi per la codifica e decodifica usando la classe AES
                            -Introdotta chiave cr2 in web.config o app.config")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("Sequence",
                     "Alla creazione delle Sequence startValue = 1",
                     noteTest:="")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("08 Novembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           Ver:="776")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Text("Codifica e decodifica password_smtp e PasswordArteaWS",
                      "-Introdotti metodi per la codifica e decodifica usando la classe AES
                            -Introdotta chiave cr2 in web.config o app.config")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Gestione sequence default",
          "Gestione sequence per progressivi chiavi tabelle di default attive per tutti. 
           Disattivabili esclusivamente impostando a False la chiave Allow_Sql_Sequence nell'appsettings")

            Riga_Text("Security - Query",
                      "Parametrizzate massivamente molte query in filtri aggiuntivi, order by e clausole IN per impedire sql injection")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("11 Ottobre 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "768")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CoreWS",
                           "Per modifiche legate alla codifica e decodifica semplice delle stringhe", "17/09/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Security - XSS Detection",
                      "Aggiunto controllo per bloccare script injection nelle chiamate ai web method")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("17 Settembre 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "768")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CoreWS",
                           "Per modifiche legate alla codifica e decodifica semplice delle stringhe", "17/09/2024")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Codifica e decodifica stringhe",
                      "Introdotta la possibilità di effettuare una codifica semplice delle stringhe",
                      noteTecniche:="Per poter abilitare la codifica semplice la proprietà UsaCodificaSemplice in appsettings deve essere impostata a true")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("13 Settembre 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "768")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

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
                           "Nuove colonne tabelle UMA", "768")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

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
                           "Nuove colonne tabelle UMA", "768")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Rilascio modifiche UMA schedulate per il 07/06/2024",
                     "Riepilogo interventi: " &
                     "- Elenco inadempienti " &
                     "- Elenco trasferimenti senza pratiche " &
                     "- Segnalazione recupero accise " &
                     "- Data rilascio pratica " &
                     "- Mail automatica al passaggio di stato ",
                     "Regione Umbria",
                     noteTest:="Non testare")

            Riga_Bug("Fix richieste/rendicontazioni",
                     "Corretta composizione della query di lettura di UMA_UF_Colture per parametri in formato stringa.",
                     idPerforma:=30285,
                     noteTest:="Non testare")

            Riga_Bug("Porting workflow",
                     "Corretto problema su redirect")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("14 Giugno 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "766")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Rilascio modifiche UMA schedulate per il 19/04/2024",
                     "Riepilogo interventi: " &
                     "- Gestione calcolo dei capi allevabili " &
                     "- Gestione note obbligatorie " &
                     "- Gestione macchine " &
                     "- Report ELAS ",
                     "Regione Umbria",
                     noteTest:="Non testare")

            Riga_Bug("Fix richieste/rendicontazioni",
                     "Sistemato caricamento descrizione lavorazione per considerare correttamente il regolamento.",
                     idPerforma:=29232,
                     noteTest:="Non testare")

            Riga_Bug("Fix messaggio errore in caso di pendenze o tessiture mancanti",
                     "Aggiornato il messaggio errore all'apertura di una rendicontazione nel caso in cui" &
                     "manchino le informazioni relative alla pendenza e/o tessitura delle varie colture" &
                     "presenti in quel fascicolo rendendolo più comprensibile.",
                     idPerforma:=29238,
                     noteTest:="Non testare")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("26 Marzo 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Rilascio modifiche UMA schedulate per il 23/02/2024",
                     "Riepilogo interventi: " &
                     "- Gestione anticipazione colturali " &
                     "- Gestione permesso attingimento acqua " &
                     "- Gestione rimanenze " &
                     "- Rendicontazione anticipata per cessazione attività",
                     "Regione Umbria",
                     noteTest:="Non testare")

            Riga_Bug("Fix sintesi richieste/rendicontazioni",
                     "Modificata lettura LeggiRiepilogo per CTE terzisti carburante richiesto/approvato raggruppata per richiesta per errata join UMA_Richieste_Lavorazioni.",
                     idPerforma:=24816,
                     noteTest:="Non testare")

            Riga_Bug("Fix diminuzione lt richiesti rendicontazioni terzista",
                     "Modificato per consentire la diminuzione dei lt richiesti in fase di rendicontazione terzista.",
                     idPerforma:=28689,
                     noteTest:="Non testare")

            Riga_Bug("Fix controllo sovrapposizioni su anticipi",
                     "Inibito controllo sovrapposizioni in rendicontazioni terzisti in caso di fascicolo fittizio: anticipi, colture non imputabili al fascicolo, trasferimenti.",
                     idPerforma:=29107,
                     noteTest:="Non testare")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("29 Febbraio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "758")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Modifiche UI Regione Umbria",
                      "Loghi header/footer, title, favicon")

            Riga_Text("Gestione lavori straordinari",
                      "Gestiti lavori straordinari 2024 sotto specifico macrouso LAVORI STRAORDINARI all'interno del fascicolo 'Colture non imputabili al Fascicolo'")

            Riga_Bug("Gestione validità lavorazioni alternative",
                     "Gestione validità per le lavorazioni alternative",
                     idPerforma:=28197,
                     noteTest:="Non testare")

            Riga_Bug("Fix lavorazioni inserite più volte",
                     "Prima il controllo sul numero di lavorazioni veniva evitato in caso di azienda cooperativa o terzista. Modificato per mmettere lavorazioni ripetute solo in caso di macrouso relativo ad anticipazioni colturali.",
                     noteTest:="Non testare")

            Riga_Bug("Fix sintesi richieste/rendicontazioni",
                     "Modificata lettura LeggiRiepilogo per CTE terzisti carburante richiesto/approvato raggruppata per richiesta per errata join UMA_Richieste_Lavorazioni. " &
                     "Modificata lettura LeggiRiepilogo per testare correttamente alcuni valori nulli, che in alcuni casi facevano risultare le rimanenze finali a zero.",
                     idPerforma:=24816,
                     noteTest:="Non testare")

            Riga_Bug("Fix diminuzione lt richiesti rendicontazioni terzista",
                     "Modificato per consentire la diminuzione dei lt richiesti in fase di rendicontazione terzista. ",
                     idPerforma:=28689,
                     noteTest:="Non testare")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("11 Gennaio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "754")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Rilascio modifiche UMA schedulate per il 15/12/2023",
                      "Riepilogo interventi: " &
                      "- Gestione validità configurazione macrousi/lavorazioni" &
                      "- Gestione validità configurazione allevamenti" &
                      "- Gestione litri già decurtati" &
                      "- Nomenclatura fascicolo fittizio anticipazioni colturali" &
                      "- Gestione eccedenze anticipo e trasferimenti rendicontazioni 2023" &
                      "- Gestione biologico" &
                      "- Ritorno allo stato di verifica in corso",
                      "Regione Umbria")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("27 Novembre 2023")

            Riga_Requisiti("Migra",
                           "Aggiunta colonna Regolamento_Cod IN CHIAVE alle tabelle UMA_Richieste, UMA_Richieste_Lavorazioni, UMA_Lavorazioni_Parziali", "746")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("Consulta lavorazioni terzisti / coop",
                     "Corretto elenco lavorazioni terzisti / cooperative per escludere lavorazioni derivanti da richieste integrative",
                     idPerforma:=27534,
                     noteTest:="Prendere un'impresa che ha una richiesta integrativa c/proprio: premendo sul pulsante lavorazioni terzisti" &
                     " NON si devono vedere le lavorazioni della prima richiesta. Viceversa, premendo il pulsante lavorazioni terzisti sulla" &
                     " prima richiesta, NON si devono vedere le lavorazioni della richiesta integrativa")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("22 Novembre 2023")

            Riga_Requisiti("Migra",
                           "Aggiunta colonna Regolamento_Cod IN CHIAVE alle tabelle UMA_Richieste, UMA_Richieste_Lavorazioni, UMA_Lavorazioni_Parziali", "746")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Aggiunta colonne alle tabelle",
                      "E' stata aggiunta a varie tabelle la colonna per la gestione del biologico",
                            noteTest:="Fare un giro generale per assicurarsi che tutto funzioni come prima")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("29 Settembre 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Nuova Richiesta",
                      "Consentito inserimento di una richiesta, previa conferma, se è presente una rendicontazione nello stato in compilazione")

            Riga_Text("Restyle Grafico",
                     "- Ri-organizzazione file css in multipli file separati <code>styleGiasComponents.css, styleGiasIcons.css, styleGiasPages.css, styleGiasUtils.css</code> anzichè l'unico file <code>styleXonneTables.css</code>")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("20 Settembre 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("Sintesi Richieste e Rendicontazioni",
                      "Corretto calcolo carburante richiesto e carburante assegnato per conto terzi",
                      idPerforma:=26011,
                      noteTecniche:="Rivista query in funzione LeggiRiepilogo per conto terzi, aggiungendo nuove tabelle cte Terzisti_Rich_Appr/Terzisti_Rich_Appr_Finale per ottenere, a fronte di ogni richiesta, il valore maggiore fra richiesta iniziale e somma lavorazioni",
                      noteTest:="Fare qualche test su aziende conto terzi con richieste integrative. " &
                      "A seguire alcuni esempi con i dati errati prima della modifica. " &
                      "1) AGRESTINI GIUSEPPE - Anno: 2023, Gasolio Richiesto: 4000, Gasolio Approvato: 3080. " &
                      "2) CONIGLIO LUIGI - Anno: 2023, Gasolio Richiesto: 2900, Gasolio Approvato: 2232.")

            Riga_Bug("Elenco Richieste",
                      "In modifica di una richiesta fatta da una cooperativa, corretto errore che si verificava in caso di CUAA azienda avente come ultimo carattere uno spazio",
                      idPerforma:=26074)

            Riga_Bug("Elenco Rendicontazioni",
                      "In modifica di una rendicontazione, passando l'indicativo AZIENDA CESSATA da SI a NO, se erano presenti richieste successive non veniva aggiornato l'indicativo di AZIENDA CESSATA",
                      idPerforma:=26101)

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("14 Settembre 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("UMA Vendite",
                      "Corretto calcolo dettaglio acquistabile per considerare il valore maggiore fra la richiesta iniziale approvata e il totale approvato da righe lavorazioni",
                      idPerforma:=25881,
                      noteTecniche:="Rivista query in funzione OttieneLtAssegnati per conto terzi, aggiungendo livello di raggruppamento aggiuntivo richiesta_cod, per poter prendere il max (e non la somma) della richiesta iniziale a fronte di ogni richiesta",
                      noteTest:="Inserire richiesta terzista, con almeno una lavorazione, con richiesta iniziale maggiore del totale carburante da lavorazioni: verificare quindi che venga visualizzata la quota maggiore nel 'Dettaglio Lt. Acquistabili'")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("06 Settembre 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("UMA Sintesi richieste",
                      "Corretto calcolo dei litri acquistabili a seguito dell'approvazione di una richiesta (ora fa fede solo il valore approvato nella richiesta piuttosto che l'anticipo)",
                     idPerforma:=25861)

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("05 Settembre 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("UMA Sintesi rendicontazioni",
                      "Corretto bug che moltiplicava la rimanenza finale per il numero di capi allevabili inseriti",
                     idPerforma:=25835)

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("24 Agosto 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("Vendite dettaglio acquistabile",
                      "Corretto calcolo dettaglio acquistabile nel caso di richiesta terzista contenente molteplici lavorazioni parziali", idPerforma:=25185,
                     noteTest:="Risolve anche la chiamata 25502. Il caso che dava errore era composto da una richiesta terzista con 5 lavorazioni parziali e il dettaglio acquistabile considerava come approvato l'approvazione inziale x il numero di lavorazioni")

            Riga_Bug("Cancellazione vendita con litri non interi",
                      "Corretto errore che impediva la cancellazione di una riga avente i litri non interi. Al momento di inserimento e modifica di una vendita ora i litri vengono arrotondati all'intero più vicino",
                          idPerforma:=25508)

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("27 Luglio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("Sintesi",
                      "Corretto calcolo della rimanenza finale in rendicontazione nel caso di una rendicontazione contenente una rimanenza e solo lavorazioni di allevamento", idPerforma:=25136)

            Riga_Bug("Dettagli Colture",
                    "Allineato calcolo delle tessiture con i nuovi filtri sulle date", idPerforma:=25077)

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("25 Luglio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("Analisi del terreno per la determinazione delle tessiture",
                      "Aggiunto controllo sulla data di validità delle analisi al momento del reperimento delle tessiture dei terreni")

            Riga_Bug("Vendite Terzisti con richieste integrative",
                    "Correzione anomalia per la quale un terzista che ha eseguito una o più richieste integrative non era in grado di acquistare tutto il carburante assegnatogli", idPerforma:=25077)

            Riga_Text("Eccedenze Anticipo",
                       "Aggiunto messaggio sulla cella 'Qta manuale' che indica che i litri verranno maggiorati per fare tornare i conti dopo l'assegnazione")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("19 Luglio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("Richieste Anticipo",
                      "Fix controllo date per richiesta anticipo")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("29 Giugno 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("UMA Allevamenti",
                      "Eliminata necessità di entrare nuovamente in modifica per vedere l'azzeramento dell'assegnato in seguito alla rimessa in compilazione da 'verifica intermedia completata con riserva'")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("28 Giugno 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("UMA Allevamenti",
                      "Ora viene correttamente eliminata la quantità di carburante approvato in caso la richiesta/rendicontazione venga messa in riserva")

            Riga_Bug("UMA Elenco Rendicontazioni",
                        "Corretto bug che in alcune rendicontazioni duplicava le rimanenze")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("20 Giugno 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("UMA Vendite",
                      "Corretto bug che impediva l'inserimento di una vendita nei casi in cui non veniva salvato il cuaa all'interno della pratica")

            Riga_Bug("UMA scelta azienda in qualunque pagina",
                        "Corretto bug visivo che mostrava un messaggio di errore alla scelta di un'azienda")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("15 Giugno 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("UMA Richieste",
                      "Corretto bug che impostava la rimanenza dell'anno precedente al valore del carburante residuo preso dalla erndicontazione dello stesso anno della richiesta")

            Riga_Bug("UMA Flag azienda cessata",
                        "fix flag reimpostato automaticamente su 'NO'")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("09 Giugno 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("UMA Setup",
                      "Aggiunta colonna per impostare il vincolo tra richieste e rendicontazioni")

            Riga_Bug("UMA Flag azienda cessata",
                        "Adesso, nel caso in cui si provi a impostare un'azienda come cessata mentre sono presenti degli anticipi o richieste per l'anno successivo, l'interruttore verrà reimpostato automaticamente su 'NO'")

            Riga_Bug("UMA Flag azienda cessata per conto terzi",
                        "Risolto problema che impediva l'impostazione del flag 'Cessata Attivita' per le aziende conto terzi anche quando era tutto in regola per farlo")

            Riga_Bug("UMA Anticipi",
                      "Corretto bug che permetteva l'inserimento di un anticipo nonostante la comparsa del messaggio di errore dovuto alla presenza del flag cessata attività nella rendicontazione dell'anno precedente")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("31 Maggio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("UMA Rimanenza richieste integrative",
                      "Corretto bug che inseriva la rimanenza nelle richieste integrative, portando alla generazione di un verbale errato")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("23 Maggio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("UMA Cessazione Azienda",
                      "Ora viene impedita la cessazione dell'azienda nel caso siano presenti degli anticipi o delle richieste per l'anno successivo")

            Riga_Bug("UMA Sintesi richieste integrative",
                      "Corretto totale dei litri richiesti in caso di richieste integrative", idPerforma:=24069)

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("18 Maggio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("UMA Setup",
                      "Alla creazione di una nuova riga nella tabella, viene impostato automaticamente la creazione contemporanea di richiesta e rendicontazione.")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("12 Maggio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("UMA Richieste",
                            "- Corretto aggiornamento della rimanenza all'apertura di una richiesta in seguito alla modifica delle rimanenze nella rendicontazione dell'anno precedente")

            Riga_Text("Ottimizzazione caricamento Ricerca documenti",
                      "Ottimizzazione caricamento Ricerca documenti, post inserimento di un documento",
                      noteTecniche:="Modificato il caricamento della DDL delle imprese. Sull'UMA venivano caricati più di 19.000 elementi, che rallentavano il caricamento della pagina. Ora viene caricata solo la PIVA passata in QS.")

            Riga_Text("GiasBase", "Eliminati tutti i riferimenti fissi al GiasBase (/GiasBase/... etc nel codice - lettura parametrizzata tramite chiave in configurazione siti")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("27 Aprile 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("UMA Anticipi",
                            "- Il sistema ora blocca i tentativi di richiedere un anticipo anche nel caso siano presenti solo richieste bocciate.")

            Riga_Text("UMA Vendite",
                            "- Il sistema ora impedisce l'acquisto di carburante o la modifica di una vendita nel caso sia presente una rendicontazione per l'azienda cliente " &
                              "che si trovi in una stato successivo al 'In Compilazione' (Se sono presenti più rendicontazioni bocciate e una in compilazione, il sistema permette di proseguire)")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("21 Aprile 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("UMA Conto Proprio",
                            "- Agli utenti è ora concesso modificare le tessiture del terreno sulla riga della coltura. Nel caso siano già presenti delle lavorazioni, sarà necessario eliminarle per proseguire.")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("11 Aprile 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("UMA Rendicontazioni",
                            "- Disabilitato switch per la cessata attività per gli utenti AFOR e in tutte le fasi successive alla compilazione")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("06 Aprile 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Bug("UMA Flag Cessata Terzisti", "Corretta visualizzazione dello switch al ricaricamento della pagina")

            Riga_Bug("UMA Fascicolo",
                      "- Corretto l'aggiornamento del fascicolo al cambio dell'anno")

            Riga_Bug("UMA Richieste",
                      "- Corretto il ripescamento della rimanenza in una nuova richiesta in caso siano presenti una o più rendicontazioni bocciate", idPerforma:=23465)

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("05 Aprile 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("UMA Sovrapposizioni", "Migliorato controllo delle sovrapposizioni nel caso il conto proprio abbia apportato delle riduzioni alle superfici di fascicolo", idPerforma:=23388)

            Riga_Text("UMA Richieste e Rendicontazioni Conto Proprio",
                      "- Ora si può solo ridurre la superficie totale da fascicolo, le endenze e le tessiture verranno ridotte automaticamente." &
                      "- Nelle lavorazioni è possibile specificare manualmente le superfici")

            Riga_Bug("UMA Richieste Terzista", "Corretto bug per il quale il gasolio approvato non veniva arrotondato all'intero più vicino")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("31 Marzo 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("UMA Nuova Richiesta", "Ora viene impedita la creazione di una nuova richiesta se l'azienda è stata dichiarata cessata nella rendicontazione dell'anno precedente")

            Riga_Text("UMA Richieste e Rendicontazioni Terzisti", "Migliorata la ripartizione automatica delle superfici nel caso di terreni con tutte e tre le tessiture valorizzate", noteTest:="Test eseguiti utilizzando CEREALI AUTUNNO VERNINI nel fascicolo del 17/03/2022 di 00644410557")

            Riga_Bug("UMA Rendicontazione Terzista", "Corretto bug per il quale veniva effettuato il controllo esclusivo delle cooperative secondo il quale un CUAA inserito deve essere stato stato inserito anche nella richiesta dello stesso anno")

            Riga_Bug("UMA Richiesta Terzista", "Corretto calcolo dei litri all'inserimento di una lavorazione per fare in modo che consideri sempre il quantitativo maggiore di litri di carburante tra tutte le colture su cui può essere eseguita quella lavorazione.")

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("17 Marzo 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "17/03/2023")

            Riga_Text("Dashboard", "Adeguamento Master + Gestione richieste x Dashboard (Header + nuova grafica)")

            Riga_Text("UMA Conto Proprio", "Aggiornata ripartizione automatica delle superfici per tener conto delle sovrapposizioni")

            Riga_Bug("UMA Conto proprio Assegnato", "Corretto limite superiore dell'assegnato nelle lavorazioni del conto proprio in fase di verifica")

            Riga_Bug("UMA Rendicontazione Conto Proprio Consulta Lavorazione Terzisti", "Corretto bug nella finestra 'Consulta Lavorazione Terzisti' che recuperava anche numerose lavorazioni non eseguite verso l'azienda selezionata da parte di terzisti", idPerforma:=22980)

            Riga_Fine()

            '------------------------------------------------------------------------------------------------

            Riga_Data("09 Marzo 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("UMA Richieste: Controllo litri richiesti minori di quelli richiesti nell'anticipo",
                     "Controllo spostato al momento del passaggio di stato", idPerforma:=22930, noteTest:="Testato su un passaggio di stato simultaneo di richiesta e rendicontazione per un conto proprio e un terzista. Testato su una richiesta singola cooperativa.")

            Riga_Bug("UMA Riduzione automatica delle superfici",
                     "Corretta ripartizione automatica delle tessiture per i conti propri", idPerforma:=22905, noteTest:="Testato in richiesta e rendicontazione di AZ. AGR. CALDERINI E MANGANELLI S.S. (02637050549), fascicolo del 10/05/2022 su coltura PROTEOLEAGINOSE")

            Riga_Bug("UMA Rendicontazioni: Pulsante cessata attivita",
                     "Corretto aggiornamento visivo dello switch nel caso si entri in modifica di una rendicontazione che ha indicato la cessata attivita", idPerforma:=22976)

            Riga_Bug("UMA Rendicontazioni: Inserimento di colture provenienti da altri fascicoli",
                     "Corretto recupero delle colture inserite indipendentemente dal loro fascicolo. Nascosto il campo del fascicolo nella parte superiore dell'interfaccia in ogni caso che non sia l'inserimento di una nuova richiesta/rendicontazione.", idPerforma:=22864, noteTest:="Testato su azienda 3M PARADISO S.S. DI MARINELLI GIUSEPPE ALESSANDRO E PAOLO SOCIETA' AGRICOLA (01975850510) con un giro completo in richiesta e rendicontazione inserendo una coltura di ogni fascicolo disponibile")

            Riga_Bug("UMA Sintesi: Richieste terzisti con lavorazioni parziali",
                     "Corretto errore nella query di popolamento della griglia delle sintesi nel caso di richieste di aziende terzisti che hanno dichiarato delle lavorazioni parziali", idPerforma:=23018, noteTest:="Testato su aziende SARTI FRANCO (SRTFNC54S26F024D) e MASSIMI SERGIO (MSSSRG68A24A475O) sul database in produzione")

            '------------------------------------------------------------------------------------------------

            Riga_Data("07 Marzo 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("UMA Vendite",
                     "Corretto calcolo del carburante acquistabile nel caso di un'azienda terzista che non ha specificato lavorazioni")

            '------------------------------------------------------------------------------------------------

            Riga_Data("06 Marzo 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Configurazione Date Rendicontazione",
                     "fix filtro colonna Data_Inizio_Rendicontazione")

            Riga_Bug("Blocco modifica Rendicontazione post Data fine da configurazione",
                     "fix controllo")

            Riga_Bug("Uma Terzisti",
                     "eliminato messaggio errore 'I litri richiesti superano il totale richiesto inzialmente' perché risulta obsoleto")

            '------------------------------------------------------------------------------------------------

            Riga_Data("01 Marzo 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("UMA controllo congruenza tra litri richiesti prima richiesta e anticipo",
                     "Aggiunta indicazione su quale tipo di carburante manca per raggiungere la soglia minima")

            Riga_Bug("UMA Vendite",
                     "Corretta query che restituiva sempre 0 nel caso non fosse presente una richiesta")

            Riga_Bug("UMA Sintesi richieste",
                     "Corretta query che calcolava male l'acquistabile")

            Riga_Bug("UMA controllo aziende conto proprio citate in rendicontazioni conto terzi",
                     "Aggiornata definizione di rendicontazione aperta per includere solo lo stato 'In Compilazione'")

            Riga_Bug("UMA Controllo date rendicontazioni per AFOR",
                     "Rimosso controllo sulle date rendicontazione per gli utenti AFOR")

            '------------------------------------------------------------------------------------------------

            Riga_Data("28 Febbraio 2023 quadris")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("UMA Controllo CUAA inseriti in richiesta per le rendicontazioni cooperative",
                     "Rimosso il controllo per l'anno 2022")

            Riga_Bug("UMA Vendite",
                     "Corretta query che non recuperava le rimanenze in presenza di una richiesta di anticipo")

            '------------------------------------------------------------------------------------------------

            Riga_Data("27 Febbraio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("UMA Avanzamento di Stato Afor",
                     "Aggiunto messaggio per afor quando non Approvano con successo una rendicontazione, per ricordare di inserire lo stesso stato nella richiesta: 'Passaggio di Stato Completato. ATTENZIONE! Per poter completare l'iter è necessario riportare lo stesso stato nella richiesta dell'anno successivo.'")

            '------------------------------------------------------------------------------------------------

            Riga_Data("23 Febbraio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("UMA Controlli sulle date di fine inserimento rendicontazioni",
                     "I controlli vengono ignorati nel caso la rendicontazione in compilazione provenga da una approvazione con riserva o come conseguenza di una precedente rendicontazione respinta")

            '------------------------------------------------------------------------------------------------

            Riga_Data("22 Febbraio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("UMA Avanzamenti di Stato",
                     "Bug fix vari sugli avanzamenti di stato")

            '----------------------------------------------------------------------------------------------

            Riga_Data("21 Febbraio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Richiesta Cooperative ",
                     "- Le cooperative possono fare l'avanzamento di pratica SOLO se hanno inserito almeno un CUAA nelle lavorazioni ")

            Riga_Bug("Fix grafico caricamento Richieste/Rendicontazioni UMA",
                     "- migliorato graficamente il caricamento di richieste e rendicontazioni (non appaiono più gli elementi delle richieste anticipo in fondo allo schermo)")

            Riga_Bug("Nuova Rendicontazione UMA",
                     "- Il controllo sulla presenza della piva in rendicontazioni conto terzi viene effettuata solo in nuova rendicontazione conto proprio (non anche in conto terzi nel caso l'azienda avesse anche un conto proprio citato in altre rendicontazioni)")

            Riga_Bug("Fix approvazione richiesta iniziale",
                     "- Fix controllo sull'approvazione della richiesta iniziale che non possa superare i litri richiesti decurtati")

            Riga_Text("Inserimento DOCUMENTI",
                     "Vengono ora ricordati i filtri impostati nella griglia dei documenti quando si apre/chiude il pop-up del documentale")

            '----------------------------------------------------------------------------------------------

            Riga_Data("16 Febbraio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Rimanenza in Rendicontazioni",
                      "Fix visualizzazione della rimanenza finale dell'anno precedente in rendicontazione (la impostava uguale a quella finale dell'anno corrente)")

            Riga_Bug("Carburante assegnato -- Approvazione richieste / rendicontazioni ",
                     "- Fix allineamento tra i lt Assegnati in griglia e quelli dichiarati nella richiesta iniziale approvata
                     - Aggiunti controlli su valore massimo e minimo inseribile rispetto al richiesto decurtato ")

            '----------------------------------------------------------------------------------------------

            Riga_Data("10 Febbraio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("UMA Macrousi",
                       "Aggiunto fascicolo fittizio 'Anticipazioni Colturali' e il relativo gruppo colturale fittizio 'Anticipazioni'",
                        noteTest:="Necessario inserire manualmente delle lavorazioni tramite le tabelle di setup associate a questo gruppo colturale")

            Riga_Text("Richieste Carburante",
                       "Aggiunto controllo al salvataggio della griglia per verificare che si richiedano più litri di carburante rispetto alla richiesta di anticipo e alle rimanenze")

            Riga_Bug("Carburanti acquistabili",
                     "In caso sia presente una richiesta verranno considerati solo il litri presenti in essa, altrimenti si considerano quelli relativi all'anticipo")

            Riga_Bug("Rendicontazioni conto proprio",
                      "Fix inserimento di una lavorazione dichiarate come eseguita da un terzista")

            '----------------------------------------------------------------------------------------------

            Riga_Data("9 Febbraio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Controllo inserimento CUAA rendicontazione Coop",
                     "fix controllo")

            '----------------------------------------------------------------------------------------------

            Riga_Data("8 Febbraio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("UMA Controlli Date",
                     "Fix controlli date usando gli operatori <= e >=")

            Riga_Bug("UMA Calcolo pendenza",
                     "Corretto calcolo pendenza superfici se zona non indicata in zonexparticelle")

            '----------------------------------------------------------------------------------------------

            Riga_Data("7 Febbraio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("UMA Anticipo",
                     "- Fix inserimento anticipo")

            Riga_Bug("UMA Richieste conto Proprio",
                     "- Fascicolo selezionato nuovamente visibile in modifica di una rendicontazione conto proprio")

            Riga_Bug("Messaggio 'Richiesta/Rend non modificabile causa presenza del documento...",
                     "Ripristinata la visiblità del messaggio dopo aver allegato i documenti UMA Rendicontazione / Uma Richieste Carburante")

            Riga_Bug("Stampe UMA",
                     "Ripristinato il funzionamento delle stampe UMA")

            Riga_Bug("Assegnazione Carburante Richieste",
                     "- Fix allineamento con richiesta iniziale assegnata nei terzisti
                     - Fix controllo sul massimo assegnabile in conto proprio ")

            Riga_Text("Blocco inserimento CUAA dopo data da setup",
                      "Ora il blocco sui CUAA è effettivo subito dopo aver cliccato il pulsante SALVA")

            Riga_Bug("UMA Richieste - Rimanenze Iniziali",
                      "Ora le tabelline delle rimanenze vengono popolate correttamente al cambio dell'impresa")

            Riga_Bug("Edit Griglia Terzisti/Coop con CUAA",
                     "Se si clicca erroneamente sul CUAA, l'intera riga NON verrà più resettata")

            Riga_Bug("Baco griglia Macchine",
                     "Risolto visualizzazione macchine nella sezione delle lavorazioni terzisti")

            '----------------------------------------------------------------------------------------------

            Riga_Data("2 Febbraio 2023 bis")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("UMA Approvazione Richieste Terzisti",
                     "- L'ispettore AFOR è di nuovo in grado di modificare manualmente il carburante assegnato ")

            '----------------------------------------------------------------------------------------------

            Riga_Data("2 Febbraio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("UMA Richieste Terzisti",
                     "- Corretto assegnamento automatico dei litri tramite pulsante, " &
                     "- Corretto allineamento dei litri nella richiesta iniziale al cambiamento delle lavorazioni dichiarate e al salvataggio")

            '----------------------------------------------------------------------------------------------

            Riga_Data("31 Gennaio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("UMA Richieste",
                     "- Corretto controllo per le date limite delle rendicontazioni che veniva eseguito erroneamente anche per le richieste")

            '----------------------------------------------------------------------------------------------

            Riga_Data("27 Gennaio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("UMA Richieste conto proprio e conto terzi",
                            "I controlli sulle sovrapposizioni sono abilitati e includono le richieste integrative")

            Riga_Text("UMA Vendite",
                            "Aggiornato il dettaglio del carburante acquistabile per includere anche le richieste terzisti con lavorazioni parziali")

            Riga_Text("UMA Carburanti",
                            "- Aggiunti 'Elettricità' e 'Carburante non agricolo' all'elenco dei carburanti da scegliere al momento della lavorazione" &
                            "- Aggiunte colonne 'Numero di Operazioni eseguite con' per i carburanti nuovi, sia in richieste che in rendicontazioni, in Sintesi Richieste e Rendicontazioni")

            Riga_Text("UMA Rendicontazioni Aziende Private",
                            "Aggiunto controllo sulla congruenza delle lavorazioni dichiarate come eseguite dai terzisti; deve essere presente la stessa lavorazione per quell'azienda nella rendicontazione del terzista in questione.")

            Riga_Bug("UMA Blocco Particelle",
                            "Fix salvataggio dei blocchi particelle con utenti diversi dal superuser")

            Riga_Bug("UMA Richieste Azienda Agromeccanica",
                            "Fix aggiornamento dei litri iniziali se il totale dei litri richiesti nelle lavorazioni risulta maggiore")

            Riga_Text("UMA",
                      "Aggiunti tipi enumarativi per i carburanti: enum_TipoCarburante_UMA")

            Riga_Text("UMA Configurazioni - Date Rendicontazioni",
                      "miglioramento grafico: se la data è '01/01/1900' o '31/12/2100', non viene visualizzata")

            Riga_Text("UMA Richieste Anticipo - Migliorie grafiche",
                      "Migliorato l'aspetto grafico del pop-up richiesta di anticipo: 
                      - Aggiunto il pulsante per chiudere il pop-up
                      - Aggiunte tabelline con 'preview' lt di anticipo per tipo carburante, ricalcolate in base alla percentuale inserita")

            Riga_Text("Rendicontazioni Conto Proprio",
                      "Aggiunto controllo sulle rendicontazioni conto proprio: 
                      - se l'azienda selezionata risulta presente in rendicontazioni conto terzi/coop per l'anno, non sarà possibile inserire rendicontazioni fino alla data_fine_rendicontazione della coop o terzista (in base alle informazioni trovate)
                      Comparirà un messaggio del tipo: 'Attenzione: l'azienda selezionata risulta presente in...Sarà possibile procedere con la rend. in conto proprio dopo il ...', con in elenco le coop/terzisti che hanno dichiarato il CUAA nelle loro rendicontazioni.")

            Riga_Text("Rendicontazioni Conto Terzi / Coop",
                      "Se si è oltre la Data Limite Inserimento CUAA e si prova a cancellare un CUAA con una sola occorrenza in griglia, apparirà un messaggio di conferma dell'azione, poichè una volta salvata la rendicontazione non sarà più possibile reinserire l'elemento eliminato. 
                      Sarà comunque possibile reinserire il CUAA PRIMA del salvataggio.")

            Riga_Bug("Rendicontazioni Coop / Terzisti",
                     "- Reinserito controllo sulla coerenza delle superfici: la somma di SupA e SubB deve essere uguale alla superficie totale dichiarata.
                     - bug fix incoerenza fra lt. richiesti e fabbisogno calcolato")

            '----------------------------------------------------------------------------------------------

            Riga_Data("17 Gennaio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("UMA Anticipi",
                            "- Aggiornata finestra del messaggio di errore 'Oltre la data di inserimento' per essere conforme a tutti gli altri errori ")

            Riga_Text("UMA configurazioni - Date Rendicontazioni",
                            "aggiunti controlli sulla tabella:  
                                    - è possibile aggiungere solo una riga per coppia Anno+Tipo Azienda
                                    - migliorati controlli su campi obbligatori (aggiunto messaggio di errore con lista campi mancanti) ")

            '----------------------------------------------------------------------------------------------

            Riga_Data("16 Gennaio 2023")

            Riga_Requisiti("Migra",
                           "Creazione permesso per Blocco Particelle", "715")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("Richiesta anticipo - % Aliquota",
                        "- Aggiunta possibilità di inserire una % aliquota minore rispetto alla default, nelle richieste di anticipo")

            Riga_Text("Richiesta anticipo - Data Limite Inserimento",
                        "- Aggiunta gestione Data Limite Inserimento Richiesta Anticipo: se si è fuori dal periodo consentito compare il messaggio 'Non è possibile inserire una richiesta di anticipo oltre il gg/mm'.  
                        Il messaggio e il blocco saranno mostrati anche se la richiesta era già stata inserita in precedenza, ma rimasta in stato di compilazione")

            Riga_Bug("Pulsante 'Consulta rendicontazione terzisti'",
                     "Ora il pulsante 'Consulta rendicontazione terzisti' viene mostrato solamente nelle rendicontazioni in conto proprio. ")

            Riga_Bug("Pulsante 'Consulta lavorazioni terzisti'",
                     "Il pulsante diventa 'Consulta rendicontazione terzisti / coop' e riporta anche le lavorazioni delle cooperative ")

            Riga_Text("UMA Configurazione",
                        "- Aggiunta gestione della Data Limite Inserimento Richiesta Anticipo nel tab UMA SETUP
                        - Aggiunta gestione tabella Periodi di Rendicontazione")

            Riga_Bug("UMA Elenco richieste",
                        "Correzione del calcolo dei litri mostrati per le richieste terzista contenenti delle lavorazioni parziali (senza aver specificato CUAA e coltura)")

            Riga_Bug("UMA Richieste Terzista",
                        "Correzione aggiornamento dei litri totali di una richiesta al salvataggio di una nuova lavorazione parziale")

            Riga_Bug("UMA - Iscrizione al CCIAA Aziende Pubbliche",
                      "- Corretta la lettura del Numero Iscrizione Camera Di Commercio")

            Riga_Text("Controlli date rendicontazioni",
                      "- non è possibile creare rendicontazione prima della data_inizio a tabella
                      - non è possibile creare / modificare rendicontazione dopo la data_fine a tabella 
                      - non è possibile avanzare lo stato della rendicontazione dopo la data_fine a tabella
                      - è possibile modificare gli allegati fino al termine_ultimo in tabella (SOLO stato <> approvato da afor --> completato e verificato con successo)")

            Riga_Text("Tipi Enumerativi Stati",
                      "Aggiunti tipi enumerativi enum_WAnagraficaStati e sostituite tutte le occorrenze nel codice")

            Riga_Bug("Controllo lt Richiesti rispetto alla Rendicontazione dell'anno precedente",
                     "Il controllo sui litri richiesti rispetto a quelli rendicontati nell'anno precedente, ora è impostato solo per le imprese agromeccaniche")

            Riga_Bug("Inserimento Lavorazioni Uguali",
                     "Il controllo sull'inserimento di lavorazioni uguali è stato circoscritto al Conto Proprio. 
                     Terzisti e Coop potranno inserire N volte la stessa lavorazione (tranne x FURTO CARBURANTE - max 1).")

            '-----------------------------------------------------------------------------

            Riga_Data("12 Gennaio 2023")

            Riga_Requisiti("Migra",
                           "Creazione tabelle UMA_Lavorazioni_Parziali e UMA_Blocco_Particelle", "711")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("UMA",
                      "Soluzione vari bug")

            '-----------------------------------------------------------------------------

            Riga_Data("11 Gennaio 2023")

            Riga_Requisiti("Migra",
                           "Creazione tabelle UMA_Lavorazioni_Parziali e UMA_Blocco_Particelle", "711")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("UMA",
                      "- Le cooperative non possono fare richieste, rendicontazioni, richieste anticipo come terzisti (non appaiono le coop nella lista imprese e se si entra con una coop questa non sarà utilizzabile)
                      - Aggiunti controlli date inizio e fine rendicontazione
                      - I terzisti e le coop possono dichiarare CUAA di imprese pubbliche, senza la necessità che queste siano iscritte al CCIAA
                      - Inseriti controlli inserimento nuovi CUAA dopo il 15/01 (o data tabellata) per terzisti e cooperative:
                        Non sarà possibile inserire CUAA che non sia stato precedentemente dichiarato in rendicontazione entro il giorno dato.
                        Inoltre le coop sono vincolate ad usare solo i CUAA precedentente dichiarati nelle richieste")

            '-----------------------------------------------------------------------------

            Riga_Data("10 Gennaio 2023")

            Riga_Requisiti("Migra",
                           "Creazione tabelle UMA_Lavorazioni_Parziali e UMA_Blocco_Particelle", "711")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("UMA",
                      "Soluzione vari bug")

            '-----------------------------------------------------------------------------

            Riga_Data("04 Gennaio 2023")

            Riga_Requisiti("Migra",
                           "Creazione tabelle UMA_Lavorazioni_Parziali e UMA_Blocco_Particelle", "711")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Lentezza caricamento richieste",
                      "Migliorate performance", "Regione Umbria", 22122, "Tolta parametrizzazione query, replicare il caso con Belmonte")

            Riga_Bug("Rendicontazione Terzisti",
                      "corretto bug nell'apertura delle lavorazioni inserite", "Regione Umbria", 22126)

            '-----------------------------------------------------------------------------

            Riga_Data("03 Gennaio 2023")

            Riga_Requisiti("Migra",
                           "Creazione tabelle UMA_Lavorazioni_Parziali e UMA_Blocco_Particelle", "711")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Titolo anticipo",
                      "Corretto titolo richiesta di anticipo contro proprio")

            Riga_Bug("Richiesta Terzisti",
                      "Corretto inserimento lavorazioni richiesta terzisti")

            '-----------------------------------------------------------------------------

            Riga_Data("29 Dicembre 2022 bis")

            Riga_Requisiti("Migra",
                           "Creazione tabelle UMA_Lavorazioni_Parziali e UMA_Blocco_Particelle", "711")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("UMA Bug fix 2",
                      "Bug fix vari in seguito agli ultimi rilasci")

            '-----------------------------------------------------------------------------

            Riga_Data("29 Dicembre 2022")

            Riga_Requisiti("Migra",
                           "Creazione tabelle UMA_Lavorazioni_Parziali e UMA_Blocco_Particelle", "711")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("UMA Bug",
                      "Bug fix vari in seguito agli ultimi rilasci")

            '-----------------------------------------------------------------------------

            Riga_Data("28 Dicembre 2022 bis")

            Riga_Requisiti("Migra",
                           "Creazione tabelle UMA_Lavorazioni_Parziali e UMA_Blocco_Particelle", "711")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("UMA Bug fix 2",
                      "Bug fix vari in seguito agli ultimi rilasci, in particolare per le richieste di anticipo ")

            '-----------------------------------------------------------------------------

            Riga_Data("28 Dicembre 2022")

            Riga_Requisiti("Migra",
                           "Creazione tabelle UMA_Lavorazioni_Parziali e UMA_Blocco_Particelle", "711")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("UMA",
                      "Bug fix vari in seguito agli ultimi rilasci, in particolare per le richieste di anticipo ")

            '-----------------------------------------------------------------------------

            Riga_Data("23 Dicembre 2022")

            Riga_Requisiti("Migra",
                           "Creazione tabelle UMA_Lavorazioni_Parziali e UMA_Blocco_Particelle", "711")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("UMA Sintesi",
                      "- Rinominate le colonne anteponendo il nome del carburante al resto per una migliore visualizzazione" &
                      "- Aggiunte colonne di sintesi per i tre carburanti principali e aggiornato il calcolo dell'acquistabile")

            Riga_Text("UMA Blocco Particelle",
                        "- Aggiunti permessi")

            Riga_Text("UMA Presentazione Richieste",
                        "Aggiunto switch nelle rendicontazioni per indicare che non si intende creare una nuova richiesta per l'anno successivo")

            Riga_Text("UMA Anticipo",
                        "fix e controlli vari")

            Riga_Text("UMA Cooperative",
                                "- In rendicontazione viene controllato che il CUAA per cui è stato effettuato il lavoro rientri fra quelli per cui è stata fatta una richiesta")

            Riga_Text("UMA Terzisti",
                        "- In rendicontazione aggiunta una limitazione: non è possibile aggiungere nuovi CUAA alle richieste se si è superata la data limite inserimento (UMA_Setup_Date_Rendicontazione.Data_Limite_INS_Azienda_Terzista)")

            Riga_Text("UMA Aziende Pubbliche",
                        "- In richiesta non viene controllata l'iscrizione al CCIAA per le aziende pubbliche")

            '-----------------------------------------------------------------------------

            Riga_Data("22 Dicembre 2022")

            Riga_Requisiti("Migra",
                           "Creazione tabelle UMA_Lavorazioni_Parziali e UMA_Blocco_Particelle", "711")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("UMA Richieste",
                      "- Inserito nuovo pulsante Dettaglio Appezzamenti " &
                      "- Inserito nuovo pulsante Sintesi richieste e rendicontazioni " &
                      "- Inserito nuovo pulsante Consulta Lavorazioni Terzisti " &
                      "- Inserita la funzionalità nelle richieste conto proprio  che è possibile modificare soltanto il campo totale delle lavorazioni e in automatico le pendenze e le tessiture si modificano automaticamente togliendo prioritariamente dalla pendenza B e dal terreno tenace ")

            Riga_Text("UMA Richieste Terzisti",
                      "- Aggiunta nuova tab per l'inserimento delle lavorazioni inserendo il tipo di lavorazione e non inserendo il gruppo colturale")

            Riga_Text("UMA Rendicontazioni",
                      "- Aggiunto vincolo per il quale non è possibile chiudere una rendicontazione senza aver prima compilato la richiesta per l'anno successivo, viceversa non è possibile avanzare una richiesta senza aver prima chiuso la rendicontazione dell'anno precedente (escluse aziende nuove e cessate)")

            Riga_Text("UMA Rendicontazione Terzisti",
                      "- Aggiunta la possibilita' di poter salvare nelle lavorazioni soltanto l'azienda a cui vien e effettuato il lavoro.")

            Riga_Text("UMA Richieste di Anticipo Carburante",
                      "- Nuova pagina per effettuare le richieste di anticipo carburante.")

            Riga_Text("UMA Blocco Particelle",
                      "- Aggiunta nuova pagina per permettere agli ispettori AFOR di bloccare specfiche particelle per un anno specifico (i litri inseriti in tali particelle in richieste/rendicontazioni di quell'anno non verranno considerati)")

            Riga_Text("UMA Richieste / Rendicontazioni",
                      "- Aggiunto pulsante 'AGGIUNGI MACCHINA' che apre un pop-up per aggiungere una macchina dall'anagrafica
                      - Aggiungo controllo sul tipo di azienda: se di tipo 'CONSORZIO DI BONIFICA', verranno caricate solo le lavorazioni con flag 'Utilizzata_da_consorzio_bonifica' = True")

            Riga_Text("Menu Carburanti UMA",
                      "- rinominati pulsanti conto proprio in 'Az. Priv., Pubb., Cons. Bonif.'
                      - rinominati pulsanti terzista in 'Impresa Agromeccanica'
                      - aggiunti pulsanti al menu per Cooperative")

            Riga_Text("UMA Richiesta Integrativa  Terzista ",
                      "Aggiunto controllo sulla richiesta integrativa per le imprese terziste: non sarà possibile inserire una nuova richiesta senza aver prima acquistato una data percentuale di carburante. La percentuale viene impostata in Configurazioni UMA > UMA Setup > Colonna Percentuale_Integrazione_Terzista")

            Riga_Text("Documenti allegati richiesta POST approvazione AFOR",
                      "è ora possibile allegare documenti, anche dopo il rilascio di AFOR, entro un termine ultimo salvato su tabella UMA_Setup_Date_Rendicontazione")

            '-----------------------------------------------------------------------------
            Riga_Data("20 Dicembre 2022")

            Riga_Requisiti("Migra",
                           "Creazione tabelle UMA_Lavorazioni_Parziali e UMA_Blocco_Particelle", "711")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Bug("UMA richieste",
                      "- Rimossi decimali sui campi relativi ai litri dai totali colonna",
                      "Regione Umbria / umbriadigitale", 19987)

            '-----------------------------------------------------------------------------

            Riga_Data("13 Dicembre 2022")

            Riga_Requisiti("Migra",
                           "Creazione tabelle UMA_Lavorazioni_Parziali e UMA_Blocco_Particelle", "711")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Bug("UMA Elenco Rendicontazioni",
                      "- Corretto controllo litri in esubero",
                      "Regione Umbria / umbriadigitale", 19987)

            Riga_Bug("UMA Sintesi ",
                     "- Gestita la presenza di richieste integrative senza lavorazioni dei terzisti",
                     "", 19224)

            '-----------------------------------------------------------------------------

            Riga_Data("01 Dicembre 2022")

            Riga_Requisiti("Migra",
                           "Aggiunta Colonna Rintracciabilita in Tabella Audit", "696")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Bug("UMA richieste",
                      "- Corretto un bug per il quale alcuni controlli sulle sovrapposizioni venivano eseguiti anche nelle richieste (invece che solo nelle rendicontazioni)",
                      "Regione Umbria / umbriadigitale", 21667)

            '-----------------------------------------------------------------------------

            Riga_Data("28 Novembre 2022")

            Riga_Requisiti("Migra",
                           "Aggiunta Colonna Rintracciabilita in Tabella Audit", "696")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("UMA approvazione rendicontazioni",
                      "- ora se un'assegnazione viene impostata a 0 lt, quella viene ignorata nei controlli",
                      "Regione Umbria / umbriadigitale", 21542)

            Riga_Text("UMA Terzisti",
                      "- La richiesta iniziale viene aggiornata ogniqualvolta si richiedono dei litri che eccedono quelli specificati nella suddetta richiesta iniziale " &
                      "- Aggiunto messaggio di errore in caso si tenti di procedere al salvataggio di una nuova riga dopo aver specificato il CUAA ma non gli altri dati",
                      "Regione Umbria / umbriadigitale")

            '-----------------------------------------------------------------------------

            '-----------------------------------------------------------------------------

            Riga_Data("15 NOvembre 2022")

            Riga_Requisiti("Migra",
                           "Aggiunta Colonna Rintracciabilita in Tabella Audit", "696")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("UMA Rendicontazioni",
                      "- Aggiunto messaggio di errore specifico quando non viene inserita una data per le lavorazioni",
                      "Regione Umbria")

            Riga_Bug("UMA Richieste/Rendicontazioni Terzisti",
                      "- Corretto messaggio di errore per l'inserimento di più litri di quelli specificati nella richiesta iniziale ",
                      "Regione Umbria")

            Riga_Bug("UMA Vendite e Sintesi",
                      "- Corretta incongruenza sul calcolo dei litri acquistabili quando presente una richiesta/rendicontazione terzista con lavorazioni il cui totale dei litri non supera quello specificato nella relativa richiesta iniziale ",
                        "Regione Umbria")

            '-----------------------------------------------------------------------------

            Riga_Data("04 Novembre 2022")

            Riga_Requisiti("Migra",
                           "Aggiunta Colonna Rintracciabilita in Tabella Audit", "696")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Bug("Elenco Richieste/Rendicontazioni",
                      "- Correzione controllo sui litri richiesti minori di quelli iniziali. " &
                      "- Modificata visualizzazione nell'elenco richieste per mostrare il massimo tra il carburante richiesto inizialmente e quello effettivamente richiesto.",
                      "Regione Umbria", 21212)

            Riga_Bug("UMA Vendite",
                      "- Correzione dell'errore lato server dovuto alla mancanza di un controllo sui valori mancanti.",
                        "Regione Umbria", 21192)

            '-----------------------------------------------------------------------------

            Riga_Data("28 Settembre 2022")

            Riga_Requisiti("Migra",
                           "Aggiunta Colonna Rintracciabilita in Tabella Audit", "696")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Bug("UMA Rendicontazioni",
                      "- Corretto caso particolare in cui una rendicontazione senza lavorazioni non veniva rilevata nell'estrazione")

            Riga_Text("Versione jQuery",
                      "Impostata di default per tutti la versione di jQuery 3.5.1")

            '-----------------------------------------------------------------------------

            Riga_Data("14 Settembre 2022")

            Riga_Requisiti("Migra",
                           "Aggiunta Colonna Rintracciabilita in Tabella Audit", "696")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Bug("UMA Sintesi Richieste e Rendicontazioni",
                      "- Fix distribuzione dei litri rendicontati per gli allevamenti (chiamata 19987)")

            Riga_Bug("UMA Vendite",
                     "- Fix calcolo dei litri acquistabili per le aziende che hanno una richiesta di carburante iniziale e almeno una lavorazione")

            '-----------------------------------------------------------------------------

            Riga_Data("12 Settembre 2022")

            Riga_Requisiti("Migra",
                           "Aggiunta Colonna Rintracciabilita in Tabella Audit", "696")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("UMA Sintesi richieste e rendicontazioni",
                      "- Aggiunta colonna 'Data Presentazione Rendicontazione'")

            Riga_Bug("UMA Approvazioni",
                     "- Fix pulsante assegnamento automatico disabilitato anche se viene cancellato il documento 'verbale istruttoria richiesta'")

            '-----------------------------------------------------------------------------

            Riga_Data("07 Settembre 2022")

            Riga_Requisiti("Migra",
                           "Aggiunta Colonna Rintracciabilita in Tabella Audit", "696")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("UMA Approvazione lavorazioni terzisti",
                      "- Ora le modifiche effettuate manualmente ai valori dei litri assegnati successive all'assegnamento automatico verranno trasposte anche nel campo 'Richiesta Iniziale Approvata'")

            Riga_Bug("UMA Approvazioni",
                     "- Ora il pulsante dell'assegnamento automatico di carburante viene correttamente disabilitato in caso non sia possibile la modifica della richiesta")

            '-----------------------------------------------------------------------------

            Riga_Data("06 Settembre 2022")

            Riga_Requisiti("Migra",
                           "Aggiunta Colonna Rintracciabilita in Tabella Audit", "696")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("Pulizia Cache",
                      "- Aggiunta pagina per pulizia della cache DataProvider")

            Riga_Fine()

            '-----------------------------------------------------------------------------

            Riga_Data("01 Settembre 2022")

            Riga_Requisiti("Migra",
                           "Aggiunta Colonna Rintracciabilita in Tabella Audit", "696")

            Riga_Requisiti("Aggancio",
                           "per UMA", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkAgronicaUMA", "107")

            Riga_Requisiti("CORE WS",
                           "Per documentale", "11/08/2022")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("Sito completo",
                      "- Modifiche per gestire nuove versioni di kendo 2022.x")

            Riga_Text("Allineamento",
                      "Allineato il sito con la versione presente su GIAS ONLINE")

            Riga_Text("Porting Documentale",
                      "Sistemate chiamate al documentale: ricerca e nuovi documenti")

            Riga_Text("UMA validità lavorazioni",
                      "- La validità delle lavorazioni nelle richieste viene stabilita sulla base della data dell'ultimo passaggio di stato")

            Riga_Bug("UMA - Richieste Terzisti",
                     "fix caricamento griglia documenti, un dato fondamentale per il funzionamento veniva sovrascritto alla selezione di un gruppo colturale nella tabella dei terzisti [rif. chiamata 19655, mail 'fix sull'UMA e nuovo rilascio su TEST INTERNI'] ")

            Riga_Fine()

            '-----------------------------------------------------------------------------

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
                Dim Riga As New Web.UI.HtmlControls.HtmlTableRow

                Riga.Cells.Add(New Web.UI.HtmlControls.HtmlTableCell)

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
                Dim Riga As New Web.UI.HtmlControls.HtmlTableRow

                Riga.Cells.Add(New Web.UI.HtmlControls.HtmlTableCell)

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
                Dim Riga As New Web.UI.HtmlControls.HtmlTableRow

                Riga.Cells.Add(New Web.UI.HtmlControls.HtmlTableCell)

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
                Dim Riga As New Web.UI.HtmlControls.HtmlTableRow

                Riga.Cells.Add(New Web.UI.HtmlControls.HtmlTableCell)

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
                Dim Riga As New Web.UI.HtmlControls.HtmlTableRow

                Riga.Cells.Add(New Web.UI.HtmlControls.HtmlTableCell)

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

            If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("Versione_Pwd")) Then
                ChiaveSistema = System.Configuration.ConfigurationManager.AppSettings("Versione_Pwd").ToString
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