Imports System.IO
Imports AgronicaCoreUtilityVersioni
Imports AgronicaCoreUtilityVersioni.Enumerativi

Namespace StampeVersione

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

            Riga_Versione("23 Luglio 2026", "152.2.1")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Stampa Dettaglio Partite",
               "Fix per visualizzare anche le partite che non hanno valori nei pesi e nei costi",
               cliente:="Inalca",
               idTicketAssistenza:=207907, idTicketSviluppo:=0, idTicketTesting:=0,
               autore:="Marco Rossi",
               noteTecniche:="",
               noteTest:="Provare la stampa con diverse partite con e senza pesi/costi/ricavi")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Stampa Sintesi Partite",
               "Fix numero capi duplicati",
               cliente:="Inalca",
               idTicketAssistenza:=209108, idTicketSviluppo:=0, idTicketTesting:=0,
               autore:="Marco Rossi",
               noteTecniche:="",
               noteTest:="Ricontrollare che i risultati delle due stampe Dettaglio e Sintesi ora siano coerenti per la stessa partita")

            '---------------------------------------------------------------------------

            Riga_Versione("10 Luglio 2026", "152.1.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Certificato Pomodoro",
                           "Adeguamento in base al contratto 2026 del pomodoro dei calcoli del grado brix e della variazione di prezzo sui difetti maggiori",
                           cliente:="Fruttagel",
                           idTicketAssistenza:=0, idTicketSviluppo:=221972, idTicketTesting:=0,
                           autore:="Gianluca Amoroso",
                           noteTecniche:="",
                           noteTest:="Indicazioni sul ticket di sviluppo")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Piano Colturale per Catasto Griglia",
               "Fix per visualizzare al piva reale",
               cliente:="Tutti",
               idTicketAssistenza:=0, idTicketSviluppo:=222172, idTicketTesting:=0,
               autore:="Marco Cecalupo",
               noteTecniche:="",
               noteTest:="")

            '---------------------------------------------------------------------------

            Riga_Versione("06 Luglio 2026", "152.0.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Stampe irrigazioni",
                "Corretta la stampa delle quantità totale dell'acqua nel periodo",
                cliente:="",
                idTicketAssistenza:=221045, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:="Paolo Netso",
                noteTecniche:="",
                noteTest:="Provare la stampa di tutti i report che stampano l'acqua dell'irrigazione, i report dovrebbero essere questi (ma se ci sono anche altri report fare il test anche per gli altri report): global gap, global gap multicentro, registro aziendale unico, scheda di campagna, scheda di campagna multicentro, scheda campagna prov autonoma trento ")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Stampa 'Scheda Colturale Bio'",
                "Corretta la stampa delle fertirrigazioni, appariva una riga per l'acqua, rimossa",
                cliente:="Randi",
                idTicketAssistenza:=216918, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:="Paolo Netso",
                noteTecniche:="",
                noteTest:="Riprodurre il caso che ha dato problemi al cliente ")

            '---------------------------------------------------------------------------

            Riga_Versione("26 Giugno 2026", "151.3.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Verbale Istruttoria Richiesta Carburante UMA Anno in Corso",
                           "Modificato il testo del primo capoverso dopo l'oggetto come da richiesta cliente",
                           cliente:="Regione Umbria GARI",
                           idTicketAssistenza:=0, idTicketSviluppo:=219194, idTicketTesting:=0,
                           noteTest:="",
                           noteTecniche:="",
                           autore:="Gianluca Amoroso")
                           
            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Piva reale",
                "Corrette le stampe ddt acquisto e vendita e l'estrazione delle statistiche di utilizzo",
                cliente:="",
                idTicketAssistenza:=0, idTicketSviluppo:=220467, idTicketTesting:=0,
                autore:="Cecalupo Marco",
                noteTecniche:="",
                noteTest:="")

            '---------------------------------------------------------------------------

            Riga_Versione("19 Giugno 2026", "151.2.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")


            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Rilievi avversità trappole nelle stampe",
                "Fix per visualizzare correttamente i rilievi avversità trappole nelle stampe della scheda di campagna",
                cliente:="CRA",
                idTicketAssistenza:=220275, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Casa,
                noteTecniche:="",
                noteTest:="")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Irrigazioni nelle stampe",
                "Corretti fix per visualizzare correttamente tutti i campi della sezione irrigazioni nelle stampe",
                cliente:="CRA",
                idTicketAssistenza:=220180, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Casa,
                noteTecniche:="",
                noteTest:="Schede di campagna (normale e multi), GlobalGAP, Scheda Campagna Trento, Registri ACA")

            '---------------------------------------------------------------------------

            Riga_Versione("12 Giugno 2026", "151.1.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")


            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Import zootecnia",
                "Fix per gestire la PivaReale durante l'import del flusso dei capi di zootecnia e sull'emissione delle fatture elettroniche",
                cliente:="Coldiretti",
                idTicketAssistenza:=0, idTicketSviluppo:=219043, idTicketTesting:=0,
                autore:="Cecalupo Marco",
                noteTecniche:="",
                noteTest:="")

            '---------------------------------------------------------------------------

            Riga_Versione("08 Giugno 2026", "151.0.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                "Tutte",
                "Estrazione e visualizzazione della Partita Iva Reale",
                cliente:="Tutti",
                idTicketAssistenza:=0, idTicketSviluppo:=214395, idTicketTesting:=0,
                autore:="Cecalupo Marco",
                noteTest:="",
                noteTecniche:="")

            '---------------------------------------------------------------------------

            Riga_Versione("22 Maggio 2026", "150.4.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                            "Redirezione a Custom500 errata",
                            "Corretta redirezione alla pagina custom 500 in caso di errore al passaggio tra siti",
                            cliente:="TUTTI",
                            idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                            noteTest:="Non testabile in autonomia",
                            noteTecniche:="",
                            autore:=DEV_Casa)

            '---------------------------------------------------------------------------

            Riga_Versione("08 Maggio 2026", "150.2.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Stampe GLOBAL GAP",
                           "- Rimossi tutti i riferimenti alle viste locali e sostituite con chiamate ai web service" &
                           "- Corretto totale superficie coltivata stampa verifica ispettiva",
                           cliente:="Zani",
                           idTicketAssistenza:=213519, idTicketSviluppo:=0, idTicketTesting:=0,
                           noteTest:="Verificare che le stampe vengano eseguite senza errori bloccanti",
                           autore:=DEV_Casa)

            '---------------------------------------------------------------------------

            Riga_Versione("04 Maggio 2026", "150.1.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Stampa dettaglio partite",
                           "Corretta query che non permetteva di scegliere delle partite che non avessero dei pesi associati",
                           cliente:="Inalca",
                           idTicketAssistenza:=207907, idTicketSviluppo:=0, idTicketTesting:=0,
                           noteTest:="Testare tutta la stampa perché la query è stata modificata, ora le partite senza pesi associati vengono correttamente incluse tra quelle selezionabili",
                           autore:="Marco Rossi")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                   "Stampa report verifica conformita",
                   "Corregge il passaggio del nome specie e centro aziendale",
                   cliente:="tutti",
                   idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=211084,
                   autore:="Anny Bevilacqua")

            '---------------------------------------------------------------------------

            Riga_Versione("28 Aprile 2026", "150.0.1")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Statistiche Utilizzo",
                           "Ridefinizione del permesso per il tasto 'StatistometroReportDettagliServiziAzienda'",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=0, idTicketSviluppo:=203826, idTicketTesting:=0,
                           noteTest:="Logica applicata al permesso 'Statistometro - Report Dettagli Servizi Azienda':
                           Per rendere visibile il tasto 'Report Dettagli Servizi Azienda' impostare il permesso di scrittura a 'Si'
                           Per nascondere il tasto 'Report Dettagli Servizi Azienda' impostare il permesso di scrittura a 'No'",
                           autore:="Cecalupo Marco")

            '---------------------------------------------------------------------------

            Riga_Versione("27 Aprile 2026", "150.0.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(
                enum_Tipo_Changelog.Feature,
                area:="QdCA",
                descrizione:="Sviluppata nuova Operazione Installazione Trappole/Catture di Massa che sostituisce le precedenti Installazione Trappole e Cattura di Massa",
                cliente:="",
                idTicketAssistenza:=0,
                idTicketSviluppo:=179308,
                idTicketTesting:=0,
                autore:=DEV_LC,
                noteTest:="Testare inserimento, modifica, cancellazione, Ricette/Brogliaccio e Stampe",
                noteTecniche:="Ora le Trappole non vengono più gestite con la Categoria di Magazzino 'Trappole', ma con la nuova categoria Formulati e classificazione cattura Massale e Attraticida")

            Riga_Changelog(
                enum_Tipo_Changelog.Feature,
                area:="QdCA",
                descrizione:="Porting Operazione Reinnesco Trappole e sviluppato nuova tab 'Gestione Trappole' dentro la pagina 'Operazioni Colturali (new!)' che permette di effettuare il Reinnesco solo delle Installazioni Trappole/Catture di Massa registrate",
                cliente:="",
                idTicketAssistenza:=0,
                idTicketSviluppo:=179320,
                idTicketTesting:=0,
                autore:=DEV_LC,
                noteTest:="Testare inserimento, cancellazione e Stampe",
                noteTecniche:="Ora le Trappole non vengono più gestite con la Categoria di Magazzino 'Trappole', ma con la nuova categoria Formulati e classificazione cattura Massale e Attraticida")

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

            '---------------------------------------------------------------------------

            Riga_Versione("17 Aprile 2026", "149.3.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                   "Checklist GlobalGAP 6 verifica ispettiva",
                   "Corretto campo raccomandazioni quando ci sono 0 raccomandazioni (prima veniva fuori NaN, ora 0)",
                   cliente:="Zani",
                   idTicketAssistenza:=0, idTicketSviluppo:=207980, idTicketTesting:=0,
                   autore:=DEV_Casa,
                   noteTest:=""
                   )

            '---------------------------------------------------------------------------

            Riga_Versione("01 Aprile 2026", "149.0.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Statistiche Utilizzo",
                           "Appliczione dei permessi per il tasto 'StatistometroReportDettagliServiziAzienda'",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=0, idTicketSviluppo:=203826, idTicketTesting:=0,
                           noteTest:="",
                           autore:="Cecalupo Marco")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Stampa Liquidazione IVA",
                           "Corretto calcolo dell'IVA per i documenti emessi con sezionale in esigibilità di tipo split payment. " &
                           "IVA derivante da questo caso aggiunta nelle sezioni dell'IVA a Credito (acquisti) con indicazione di imponibile a zero, sia nella pagina di lancio che nella stampa stessa. " &
                           "Inoltre aggiunta indicazione sul tipo di esigibilità del sezionale scelto nella pagina di filtro della stampa",
                           cliente:="Garuti",
                           idTicketAssistenza:=191732, idTicketSviluppo:=0, idTicketTesting:=0,
                           noteTest:="",
                           noteTecniche:="",
                           autore:="Gianluca Amoroso")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Report Sintesi Zootecnia",
                           "Aggiunto filtro per partite chiuse, aperte o tutte al report SINTESI PARTITE",
                           cliente:="Inalca",
                           idTicketAssistenza:=0, idTicketSviluppo:=208631, idTicketTesting:=0,
                           noteTest:="",
                           autore:=DEV_Casa)

            '---------------------------------------------------------------------------

            Riga_Versione("13 Marzo 2026", "148.2.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Allineato logo",
                           "Allineato il logo al centro nella stampa Scheda Materie Prime Bio",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=193468, idTicketSviluppo:=0, idTicketTesting:=0,
                           noteTest:="",
                           autore:=DEV_Casa)

            '---------------------------------------------------------------------------

            Riga_Versione("06 Marzo 2026", "148.1.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Aggiunta campi report Zootecnia",
                           "Aggiunti campi 'Costi Alimentari' e 'Costi Sanitari' alle tabelle di sintesi e di dettaglio partite (nei dettagli sono in basso insieme ai campi dei totali)",
                           cliente:="INALCA",
                           idTicketAssistenza:=0, idTicketSviluppo:=205755, idTicketTesting:=0,
                           noteTest:="",
                           autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Report Dettaglio partite",
                           "Corretto errore in report Dettaglio Partite, il report non visualizzava nessun dato se alcuni campi erano vuoti",
                           cliente:="INALCA",
                           idTicketAssistenza:=207907, idTicketSviluppo:=0, idTicketTesting:=0,
                           noteTest:="Provare a stampare il report sapendo che su alcune partite i campi tipo peso sono vuoti, vedere se il report visualizza correttamente i dati",
                           autore:="Marco Rossi")

            '---------------------------------------------------------------------------

            Riga_Versione("02 Marzo 2026", "148.0.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Report Bolle Conferimento",
                           "Aggiunto logo dinamico ai report Bolle Conferimento",
                           cliente:="COLDIRETTI",
                           idTicketAssistenza:=0, idTicketSviluppo:=201847, idTicketTesting:=0,
                           noteTest:="Il report interessato è stampabile dalla sezione 'Acquisti' e poi su ricerca DDT ricevuti",
                           autore:=DEV_Casa)

            '---------------------------------------------------------------------------

            Riga_Versione("24 Febbraio 2026", "147.3.1")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Stampa sintesi partite",
                           "Corretto errore di divisione per zero",
                           cliente:="INALCA",
                           idTicketAssistenza:=207522, idTicketSviluppo:=0, idTicketTesting:=0,
                           noteTest:="Provare a stampare il report Sintesi Partite e verificare che vengano mostrate delle righe a zero se non tutti i valori sono inseriti, piuttosto che un report vuoto",
                           autore:="Marco Rossi")

            '---------------------------------------------------------------------------

            Riga_Versione("20 Febbraio 2026", "147.3.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Nuove colonne stampe zootecnia",
                           "Aggiunte colonne 'Costi Sanitari' e 'Costi Alimentari' ai report Sintesi Partite e Dettaglio Partite",
                           cliente:="INALCA",
                           idTicketAssistenza:=0, idTicketSviluppo:=206726, idTicketTesting:=0,
                           noteTest:="",
                           autore:=DEV_Casa)

            '---------------------------------------------------------------------------

            Riga_Versione("13 Febbraio 2026", "147.2.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Menu Stampe (new!)",
                           "Sulla stampa 'Scheda Autocertificazione', aggiunta la superficie catastale sulle particelle; sulla stampa 'Scheda Aziendale' visibili i dati catastali anche per le aziende che non hanno impianti caricati",
                           cliente:="OROGEL",
                           idTicketAssistenza:=0, idTicketSviluppo:=204065, idTicketTesting:=0,
                           noteTest:="",
                           autore:="Dario Cabras")

            '---------------------------------------------------------------------------

            Riga_Versione("09 Febbraio 2026", "147.1.1")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Personalizzazione Grafiche Cliente",
                           "Corretta verifica della presenza del percorso per il logo personalizzato",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=205504, idTicketSviluppo:=0, idTicketTesting:=0,
                           noteTest:="",
                           autore:=DEV_Casa)

            '---------------------------------------------------------------------------

            Riga_Versione("06 Febbraio 2026", "147.1.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Stampa Dettaglio Partite",
                           "Corretto report che non mostrava la partite che coincidevano con la data fine",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=203007, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Marco Rossi",
                           noteTest:="")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Verifica presenza Personalizzazione Grafiche Cliente",
                           "Ora viene verificata la presenza di ogni proprietà che viene utilizzata",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=205504, idTicketSviluppo:=0, idTicketTesting:=0,
                           noteTest:="",
                           autore:=DEV_Casa)

            '---------------------------------------------------------------------------

            Riga_Versione("02 Febbraio 2026", "147.0.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Loghi footer dinamici stampe",
                           "Per i report BIO, Magazzino e Scheda di Campagna, ora il logo del footer è gestito da una chiave in Configurazione Siti (in assenza della quale mostrerà il logo di agronica di default)",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=0, idTicketSviluppo:=201847, idTicketTesting:=0,
                           autore:=DEV_Casa,
                           noteTest:="")

            '---------------------------------------------------------------------------

            Riga_Versione("23 Gennaio 2026", "146.2.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Dettaglio Partite",
                           "Corretti calcoli di indici riepilogativi relativi ai KG totali per usare l'indicazione del Peso Vendita piuttosto che il Peso Netto Vendita",
                           cliente:="Inalca",
                           idTicketAssistenza:=203125, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Casa,
                           noteTest:="")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Dettaglio Partite",
                           "Corretta gestione data fine che non includeva l'ultimo giorno del range",
                           cliente:="Inalca",
                           idTicketAssistenza:=203007, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Marco Rossi",
                           noteTest:="")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                area:="Scadenzario Clienti/Fornitori",
                descrizione:="Fix query di lettura lanciata con filtro su contatto",
                cliente:="Trombin Ortofrutta",
                idTicketAssistenza:=204011,
                idTicketSviluppo:=0,
                idTicketTesting:=0,
                noteTecniche:="",
                autore:="Gianluca Amoroso"
            )

            '---------------------------------------------------------------------------

            Riga_Versione("16 Gennaio 2026", "146.1.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Dettaglio Partite",
                           "Corretti calcoli di indici riepilogativi in fondo e aggiornati alcuni nomi di colonne",
                           cliente:="Inalca",
                           idTicketAssistenza:=203125, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Casa,
                           noteTest:="")

            '---------------------------------------------------------------------------

            Riga_Versione("07 Gennaio 2026", "146.0.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Sintesi Partite",
                           "Quando si selezionavano le date tramite il calendario queste non venivano considerate",
                           cliente:="Inalca",
                           idTicketAssistenza:=202011, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Marco Rossi",
                           noteTest:="Provare a selezionare le date tramite calendario e verificare che non imposti come date 1900 e 2100")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Report partite",
                           "Corretta gestione del calcolo calo peso",
                           cliente:="Genagricola",
                           idTicketAssistenza:=199952, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Marco Rossi",
                           noteTest:="Il calo peso ora viene calcolato come media del valore inserito nelle operazioni zootecniche in fase di carico e di scarico")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Salvataggio Report Coldiretti su disco",
                           "Ora i report usati da Coldiretti (sezioni Scheda di campagna, Magazzini e Biologico) memorizzano i report generati su disco e non in sessione",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=0, idTicketSviluppo:=200114, idTicketTesting:=0,
                           autore:=DEV_Casa,
                           noteTest:="")

            '---------------------------------------------------------------------------

            Riga_Versione("17 Dicembre 2025", "145.3.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                          "Sessione Stampe",
                           "Aggiunti controlli sulla sessione scaduta in quasi tutti i punti nelle stampe",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=0, idTicketSviluppo:=200114, idTicketTesting:=0,
                           autore:=DEV_Casa,
                           noteTest:="")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                          "Stampa Scheda Movimenti Magazzino",
                           "Correzione errore che impediva il corretto caricamento dei movimenti di trasformati vegetali, fornendo quindi una stampa con solo un sottoinsieme dei movimenti da estrarre",
                           cliente:="Genagricola",
                           idTicketAssistenza:=199350, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Gianluca Amoroso",
                           noteTest:="")

            '---------------------------------------------------------------------------

            Riga_Versione("12 Dicembre 2025", "145.2.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                          "Statistiche utilizzo",
                           "risolto errore in estrazione per data di registrazione, attualmente l'estrazione può essere usata correttamente usato la data competenza",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=0, idTicketSviluppo:=201279, idTicketTesting:=0,
                           autore:="Marco Cecalupo",
                           noteTest:="Lanciare l'estrazione (con 'Data Competenza', con 'Data Registrazione', con e senza dettagli) e verificare che vengano generati correttamente gli excel")

            '---------------------------------------------------------------------------

            Riga_Versione("09 Dicembre 2025", "145.1.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
               "Stampa Fitoregolatori Kiwi",
               "- Sostituito 'Reg. CE 834/2007' con 'Reg. CE 2018/84' " &
               "- Rimosso l'anno e inserito '........:' in 'Che, dall'allegagione alla raccolta, sulla produzione di kiwi da conferire per l'anno 2022' " &
               "- Sostituiti tutti i 'JINGOLD' con 'JINTAO'",
               cliente:="Orogel",
               idTicketAssistenza:=0, idTicketSviluppo:=199343, idTicketTesting:=0,
               noteTest:="Nel caso vedere anche il ticket 199342 perchè c'è stata un po' di confusione coi ticket",
               autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Correzione campi totali report dettaglio Zootecnia",
               "Modificati totali dei campi 'calo%, presenze,euro al kg' per trasformarli in medie",
               cliente:="Inalca",
               idTicketAssistenza:=199735, idTicketSviluppo:=0, idTicketTesting:=0,
               autore:=DEV_Casa,
               noteTest:=""
               )

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Stampe Sintesi Partite Zootecnia",
               "Modificato il calcolo della resa euro al kg e resa euro per presenza",
               cliente:="Inalca",
               idTicketAssistenza:=199652, idTicketSviluppo:=0, idTicketTesting:=0,
               noteTest:="Verificare se la resa euro al kg e la resa euro presenza vengono calcolate correttamente",
               autore:="Rossi Marco")

            '---------------------------------------------------------------------------

            Riga_Versione("01 Dicembre 2025", "145.0.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default", "165")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Personalizzazioni Grafiche Cliente",
               "Aggiunto il nuovo parametro PersonalizzazioniGraficheCliente, in cui è possibile configurare i loghi da visualizzare al posto di quelli di default",
               cliente:="Food MetaVerse",
               idTicketAssistenza:=0, idTicketSviluppo:=196565, idTicketTesting:=0,
               noteTest:="",
               autore:="Dario Cabras")

            '---------------------------------------------------------------------------

            Riga_Versione("26 Novembre 2025", "144.3.1")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Stampe Sintesi Partite Zootecnia",
               "Inserite anche le righe dove non c'è stata pesatura",
               cliente:="Inalca",
               idTicketAssistenza:=199222, idTicketSviluppo:=0, idTicketTesting:=0,
               noteTest:="Verificare se escono anche le macellazioni con pesoo non valorizzato",
               autore:="Rossi Marco")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Fix decimali Report Sintesi zootecnia",
               "Aggiunte cifre decimali in 'Incremento KG' e 'Incremento giornaliero' nel report Sintesi",
               cliente:="Inalca",
               idTicketAssistenza:=197709, idTicketSviluppo:=0, idTicketTesting:=0,
               autore:=DEV_Casa,
               noteTest:=""
               )

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Fix label Report Sintesi zootecnia",
               "Aggiustate celle per evitare parole tagliate in 'Codice' e 'Razza'",
               cliente:="Inalca",
               idTicketAssistenza:=199243, idTicketSviluppo:=0, idTicketTesting:=0,
               autore:=DEV_Casa,
               noteTest:=""
               )

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Fix label Report Dettagli zootecnia",
               "Aggiustate celle per evitare numeri tagliati nelle sezioni relative al peso nel report dettagli sezione Carico",
               cliente:="Inalca",
               idTicketAssistenza:=198033, idTicketSviluppo:=0, idTicketTesting:=0,
               autore:=DEV_Casa,
               noteTest:=""
               )

            '---------------------------------------------------------------------------

            Riga_Versione("21 Novembre 2025", "144.3.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Stampe - Filtro Zootecnia",
               "Ora è obbligatorio inserire l'intervallo di partite per la stampa Dettagli Partite e aggiunta pulsante 'Seleziona tutto'",
               cliente:="",
               idTicketAssistenza:=198029, idTicketSviluppo:=0, idTicketTesting:=0,
               noteTest:="Il non poter selezionare il 'vuoto' è voluto, in quanto se si vuole selezionare tutto si seleziona la prima partita dell'elenco 'Da' e l'ultima dall'elenco 'A'. Per quanto riguarda il commento di Marco, non sono riuscito a riprodurre l'errore.",
               autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Stampe partite Zootecnia",
               "Allargate le caselle di testo nel report per visualizzare correttamente valori lunghi",
               cliente:="",
               idTicketAssistenza:=198033, idTicketSviluppo:=0, idTicketTesting:=0,
               noteTest:="Venivano tagliati: data, centro, stalla, razza",
               autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Stampe partite Zootecnia",
               "Corretta la visualizzazione dei decimali in tutti i report",
               cliente:="",
               idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=197710,
               noteTest:="",
               autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Stampe Sintesi partite Zootecnia",
               "Corretto bug che mostrava anche capi non macellati in partite contenenti anche altri capi macellati",
               cliente:="",
               idTicketAssistenza:=198302, idTicketSviluppo:=0, idTicketTesting:=0,
               noteTest:="",
               autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Stampe Zootecnia",
               "Corretto bug che, a volte, mostrava una data di default rispetto a quella inserita",
               cliente:="",
               idTicketAssistenza:=198300, idTicketSviluppo:=0, idTicketTesting:=0,
               noteTest:="",
               autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Stampe Dettagli Partite Zootecnia",
               "Corretti calcoli riepilogativi nella sezione di scarico e corretta anomalia che impediva il caricamento di alcune righe del report",
               cliente:="",
               idTicketAssistenza:=197359, idTicketSviluppo:=0, idTicketTesting:=0,
               noteTest:="",
               autore:=DEV_Casa)

            '---------------------------------------------------------------------------

            Riga_Versione("14 Novembre 2025", "144.2.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
               "Stampe relative all'acqua di irrigazione e fertirrigazione",
               "Cambiata l'unità di misura da 'ha' a 'Ha'",
               cliente:="",
               idTicketAssistenza:=197118, idTicketSviluppo:=0, idTicketTesting:=0,
               noteTest:="",
               autore:="Paolo Netso")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Stampe relative all'acqua di irrigazione e fertirrigazione",
               "Sistemate stampe, veniva tagliata l'unità di misura in cella",
               cliente:="",
               idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=197821,
               noteTest:="",
               autore:="Paolo Netso")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Stampe relative all'acqua di irrigazione e fertirrigazione",
               "Nella colonna 'Impianto Irriguo' adesso appare la macchina di irrigazione (qualora fosse usata per l'irrigazione dell'impianto)",
               cliente:="",
               idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=197815,
               noteTest:="",
               autore:="Paolo Netso")

            '---------------------------------------------------------------------------

            Riga_Versione("11 Novembre 2025", "144.1.1")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Stampe Orogel su singolo impianto",
               "Corretto errore che non mostrava la cooperativa referente in vari report nel caso venisse selezionato solo un singolo impianto",
               cliente:="Orogel",
               idTicketAssistenza:=197276, idTicketSviluppo:=0, idTicketTesting:=0,
               noteTest:="Testato su scheda aziendale e autocertificazione",
               autore:=DEV_Casa)

            '---------------------------------------------------------------------------

            Riga_Versione("07 Novembre 2025", "144.1.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Performance,
               "Gestione Cache Permessi e Impostazioni utente",
               "- Gestita la cache per Permessi e impostazioni utente;",
               cliente:="",
               idTicketAssistenza:=0, idTicketSviluppo:=195312, idTicketTesting:=0,
               autore:=DEV_Drudi)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Stampe Zootecnia",
               "- Corretti filtri per le date delle operazioni e altre segnalazioni mail di Miriam",
               cliente:="",
               idTicketAssistenza:=196781, idTicketSviluppo:=0, idTicketTesting:=0,
               autore:=DEV_Drudi)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Stampa Dettagli partita Zootecnia",
               "Aggiunte cifre decimali nelle colonne 'Euro/Kg' nei sottoreport carichi e scarichi del report Dettagli Partita",
               cliente:="Inalca",
               idTicketAssistenza:=196781, idTicketSviluppo:=0, idTicketTesting:=0,
               noteTest:="Andare nel filtrone stampe, selezionare il report 'Registro Trattamenti ACA', compilare i filtri e lanciare la stampa. Verificare che il report venga generato correttamente.",
               autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
               "Stampa irrigazioni",
               "Nella stampa delle irrigazioni veniva stampata la superficie totale dell'impianto, adesso viene stampata solo la superficie trattata. Cambiato anche il nome della colonna in 'Sup. Tratt. (ha)'",
               cliente:="",
               idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=197118,
               autore:="Paolo Netso")

            '---------------------------------------------------------------------------

            Riga_Versione("03 Novembre 2025", "144.0.0")

            Riga_Requisiti("Migra",
                           " - Aggiornamento campo 'Parziale' di 'Mov_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) 
                                  - Aggiornamento campo 'Parziale' di 'Ricette_Dettaglio_Tecnico' da smallint a int, aggiornamento valori parziale per l'irrigazione (moltiplicazione per 1000 da m3/h a l/h) ",
                           ver:="802")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("CORE WS",
                           "Modifiche relative all'irrigazione new", "03/11/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Operazioni Colturali (new!)",
                           "Aggiunta la gestione in Angular dell'operazione colturale 'Irrigazione'. Ci sono diversi cambiamenti rispetto alla gestione attuale delle irrigazioni (pagina IrrigazioneBS.aspx). E' stata introdotta la possibilità di scegliere una macchina di irrigazione (alla scelta della macchina di irrigazione i campi 'Portata' ed 'Efficienza' vengono popolati con i dati della macchina). E' stato introdotto il campo 'Efficienza' che viene usato per il calcolo delle quantità assorbite (nuovi campi 'Qta Totale Acqua Assorbita nel periodo [M3]' e 'Qta Totale Acqua Assorbita Giornaliera [M3]'). La 'Portata' è adesso espressa in l/h e non più m3/h, questa modifica è stata fatta anche nella pagina IrrigazioneBS.aspx. L'utente inserisce la 'Dose Acqua Giornaliera', in 'm3/ha' o 'mm', la quantità totale dell'acqua viene calcolata usando la 'Data Inizio Irrigazione', la 'Data Fine Irrigazione' e la 'Frequenza di Irrigazione' (stesso funzionamento della irrigazione attuale). In alternativa, l'utente può cambiare la 'Portata' e le 'Ore Di Irrigazione Giornaliere' per cambiare la dose di acqua giornaliera. E' stato introdotto un popup per permettere la modifica di più impianti contemporaneamente. Il funzionamento dei consigli è rimasto come nella pagina IrrigazioneBS.aspx.",
                          cliente:="",
                          idTicketAssistenza:=0,
                          idTicketSviluppo:=179266,
                          idTicketTesting:=0,
                          autore:="Paolo Netso",
                          noteTest:="Testare l'inserimento/modifica/cancellazione di un'irrigazione. Testare il funzionamento della griglia degli impianti e l'inserimento massivo tramite il popup. Testare il ribaltamento dell'operazione da ricette/brogliaccio. Testare l'inserimento di un'operazione multicentro. Testare le stampe, i dati relativi alla portata dell'acqua dovrebbero apparire in l/h.")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Scheda di Campagna, Registro Trattamenti , Registro Fertilizzazioni, Scheda Prodotti Fitosanitari in Magazzino e Scheda Fertilizzanti in Magazzino",
                           "Aggiornati i riferimenti ai Regolamenti se viene scelta una data di fine nel filtro pre-stampa >= 01/01/2024, altrimenti vengono proposti i riferimenti ai Regolamenti attuali.",
                          cliente:="Coldiretti",
                          idTicketAssistenza:=195112,
                          idTicketSviluppo:=0,
                          idTicketTesting:=0,
                          autore:=DEV_LC,
                          noteTest:="Testare le stampe indicate selezionando come data di fine >= 01/01/2024 poi una data di fine antecedente, selezionando una specie arborea poi erbacea e scegliendo come regione emilia-romagna poi un'altra diversa, verificare che i riferimenti ai regolamenti cambino correttamente come indicato nel ticket. ")

            Riga_Changelog(enum_Tipo_Changelog.Performance,
                           "Aggiunta TryCatch e nuovi LivelliLog",
                           "- Aggiunti livelli di log: solo mail, file + mail, file + db;" &
                           "- Aggiunti blocchi TryCatch alla scrittura su file e db del log per evitare di lasciare thread zombie in giro in caso di errore",
                           cliente:="",
                           idTicketAssistenza:=196225, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Casa)

            '---------------------------------------------------------------------------

            Riga_Versione("24 Ottobre 2025", "143.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                area:="SQL Data Provider",
                descrizione:="Aggiornata parametrizzazione SQL command in AccessoMultiQuery",
                cliente:="Aboca",
                idTicketAssistenza:=0,
                idTicketSviluppo:=0,
                idTicketTesting:=0,
                noteTecniche:="",
                autore:="Salvatore Zammataro"
                )

            Riga_Changelog(
                enum_Tipo_Changelog.Feature,
                area:="Monitoraggio Corpi Estranei",
                descrizione:="Porting dei due report in excel (semplice e aggregato) da vecchio sito Stampe",
                cliente:="Fruttagel",
                idTicketAssistenza:=0,
                idTicketSviluppo:=194651,
                idTicketTesting:=0,
                noteTecniche:="",
                autore:="Giulia Bottan"
                )

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                "Report Zootecnia",
                "Aggiunta pulsante pulisci filtri",
                cliente:="Inalca",
                idTicketAssistenza:=0, idTicketSviluppo:=171568, idTicketTesting:=0,
                noteTest:="Anche ticket sviluppo 171567",
                noteTecniche:="",
                autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Report Zootecnia",
                "Ora i filtri si mantengono ad ogni stampa + fix grafici report",
                cliente:="Inalca",
                idTicketAssistenza:=0, idTicketSviluppo:=171568, idTicketTesting:=0,
                noteTest:="Anche ticket sviluppo 171567",
                noteTecniche:="",
                autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                "Report Fitoregolatori Kiwi",
                "Modifica testo dichiarazione fitoregolatori",
                cliente:="Orogel",
                idTicketAssistenza:=0, idTicketSviluppo:=191511, idTicketTesting:=0,
                noteTest:="",
                noteTecniche:="",
                autore:=DEV_Casa)

            '---------------------------------------------------------------------------

            Riga_Versione("17 Ottobre 2025", "143.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "GSB Import DQC",
                "Valorizzazione campo descrizione = specie vegetale",
                cliente:="GranFruttaZani",
                idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                noteTest:="",
                noteTecniche:="",
                autore:="Marco Lucchi")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                "Nuovi report Zootecnia sintesi e dettaglio per partita",
                "Aggiunti due report zootecnia al menu stampe e pagina di filtro",
                cliente:="Inalca",
                idTicketAssistenza:=0, idTicketSviluppo:=171568, idTicketTesting:=0,
                noteTest:="Anche ticket sviluppo 171567",
                noteTecniche:="",
                autore:=DEV_Casa)

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

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Stampa Registro Trattamenti Lombardia - Veneto",
                "Evitata la moltiplicazione dei Trattamenti e operazioni suddivise per impianti",
                cliente:="Coldiretti",
                idTicketAssistenza:=191997, idTicketSviluppo:=0, idTicketTesting:=0,
                noteTest:="",
                noteTecniche:="",
                autore:=DEV_LC)

            '---------------------------------------------------------------------------

            Riga_Versione("10 Ottobre 2025", "143.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Esportazione Excel Conferimenti",
                "Correzione regressione che causava errore nel report",
                cliente:="Fruttagel",
                idTicketAssistenza:=193456, idTicketSviluppo:=0, idTicketTesting:=0,
                noteTest:="",
                noteTecniche:="",
                autore:="Gianluca Amoroso")

            '---------------------------------------------------------------------------

            Riga_Versione("30 Settembre 2025", "142.3.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Anomalia visualizzazione fertirrigazioni in stampe",
                "Rimossa la riga 'ACQUA PER FERTIRRIGAZIONE - NON USARE' dalla stampa global gap multicentro e dalle altre stampe interessate",
                cliente:="",
                idTicketAssistenza:=192959, idTicketSviluppo:=0, idTicketTesting:=0,
                noteTest:="",
                noteTecniche:="",
                autore:="Paolo Netso")

            '---------------------------------------------------------------------------

            Riga_Versione("26 Settembre 2025", "142.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Stampa Registro Trattamenti Veneto (std. condizionalità)",
                "Evitata la moltiplicazione dei Trattamenti e operazioni suddivise per impianti",
                cliente:="Coldiretti",
                idTicketAssistenza:=191997, idTicketSviluppo:=0, idTicketTesting:=0,
                noteTest:="",
                noteTecniche:="",
                autore:=DEV_LC)

            '---------------------------------------------------------------------------

            Riga_Versione("08 Settembre 2025", "142.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan versione 12/03/25 in OP",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Integrazione Matomo",
                "Dal GIAS, fatto in modo di comunicare a Matomo lo userName, la lista dei Widget Visibili, il routing, l'azienda su cui si lavora e l'eventuale cambio.",
                cliente:="Coldiretti",
                idTicketAssistenza:=188548, idTicketSviluppo:=0, idTicketTesting:=0,
                noteTest:="",
                noteTecniche:="",
                autore:="Dario Cabras")
            Riga_Changelog(enum_Tipo_Changelog.Security,
                           "Controllo massivo SQL Injection",
                           "Verificati molti casi di possibili sql injection",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=0, idTicketSviluppo:=179500, idTicketTesting:=0,
                           autore:=DEV_Casa, noteTest:="", noteTecniche:="")

            '---------------------------------------------------------------------------

            Riga_Versione("29 Agosto 2025", "141.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                "Stampa Registro Aziendale Unico",
                "Modificata Intestazione",
                cliente:="Agri Consulting DCA",
                idTicketAssistenza:=185911, idTicketSviluppo:=0, idTicketTesting:=0,
                noteTest:="",
                noteTecniche:="",
                autore:=DEV_LC)

            '---------------------------------------------------------------------------

            Riga_Versione("07 Agosto 2025", "141.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Report Biologico S/Carichi",
                "Corretto errore per il quale non venivano estratti i dati dei costi relativi allo zoo",
                cliente:="Aboca",
                idTicketAssistenza:=183530, idTicketSviluppo:=0, idTicketTesting:=0,
                noteTest:="Nella pagina 'Report Biologico' lanciare l'estrazione in tabella 'carichi/scarichi', assicurandosi di usare varie categorie prodotto " &
                "fra cui anche 'materie prime animali', il filtro dovrebbe già essere impostato così; " &
                "impostare il filtro 'tipi movimenti' a 'tutti' per recuperare carichi e scarichi",
                noteTecniche:="Ho anche effettuato queste correzioni, emerse sviluppando il fix:" &
                "- Il filtro Data fine è stato corretto per comprendere anche le movimentazioni la cui Data principale è salvata con un orario diverso dal default di mezzanotte nel giorno indicato" &
                "- Migliorata la griglia mantenendo l'intestazione in alto nella pagina, scorrendo le righe",
                autore:="Gianluca Amoroso")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Master Boostrap",
                "Nelle pagine con la grafica nuova (come le statistiche utilizzo) non si vedevano i messaggi di errore o successo " &
                "perché mancavano dei pezzi sulla master per l'allineamento con la grafica nuova e l'header nuovo",
                cliente:="TUTTI",
                idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                noteTest:="",
                noteTecniche:="",
                autore:="Giulia Bottan")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Statistiche utilizzo",
                           "- Ripristinate le chiamate verso il metodo d'estrazione delle statistiche chiamando la funzione locale anzichè passare da NetCoreAPI",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=0, idTicketSviluppo:=185925, idTicketTesting:=0,
                           autore:="Giulia Bottan",
                           noteTest:="")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Scheda Colturale Biologico",
                "Corretta quantità totale per le irrigazioni",
                cliente:="Sintea",
                idTicketAssistenza:=183858, idTicketSviluppo:=0, idTicketTesting:=0,
                noteTest:="",
                noteTecniche:="",
                autore:="Gianluca Amoroso")

            '---------------------------------------------------------------------------

            Riga_Versione("04 Agosto 2025", "141.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                          "Statistiche utilizzo",
                           "- Ripristinate le chiamate verso il metodo d'estrazione delle statistiche",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=0, idTicketSviluppo:=185925, idTicketTesting:=0,
                           autore:="Marco Cecalupo",
                           noteTest:="Lanciare l'estrazione (con e senza dettagli) e verificare che vengano generati correttamente gli excel e che siano uno solo per tipologia")

            '---------------------------------------------------------------------------

            Riga_Versione("25 Luglio 2025", "140.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Security,
                            "SQL_Dataprovider LanciaEccezioneSuInjection",
                            "Aggiunta lettura da DB del parametro LanciaEccezioneSuInjection",
                            cliente:="Coldiretti",
                            idTicketAssistenza:=184216, idTicketSviluppo:=0, idTicketTesting:=0,
                            autore:=DEV_Casa)

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                          "Scheda Autocertificazione",
                           "- Risolto problema della sezione firma che veniva spostata in una pagina nuova anche quando c'era spazio, 
                           se il flag ""Stampa Fronte-Retro"" veniva attivato",
                           cliente:="Orogel",
                           idTicketAssistenza:=175332, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Uhalid,
                           noteTest:="")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                          "Stampe di Campagna",
                           "Sistemata visualizzazione Data e Ora",
                           cliente:="Zani",
                           idTicketAssistenza:=180164, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_LC,
                           noteTest:="")

            '---------------------------------------------------------------------------

            Riga_Versione("21 Luglio 2025", "140.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                          "Integrazione Matomo",
                           "Integrazione Matomo sul sito delle Stampe",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=0, idTicketSviluppo:=171983, idTicketTesting:=0,
                           autore:="Dario Cabras",
                           noteTest:="")

            '---------------------------------------------------------------------------

            Riga_Versione("11 Luglio 2025", "140.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                          "Statistometro (Statistiche Utilizzo)",
                           "Ottimizzazione estrazione Excel",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=0, idTicketSviluppo:=170817, idTicketTesting:=0,
                           autore:="Marco Cecalupo",
                           noteTest:="Lanciare elenco sintetico ed elenco sintetico con dettagli e verificare che siano estratti correttamente.")

            '---------------------------------------------------------------------------

            Riga_Versione("07 Luglio 2025", "140.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_BS' (server)", "159")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(
                           enum_Tipo_Changelog.Feature,
                          "Pagina di lancio stampe",
                           "E' stata rifatta la pagina di lancio per le seguenti stampe: 
                                - Scheda di Campagna (variante multicentro e non)
                                - Registro dei Trattamenti
                                - Registro dei Trattamenti Emilia-Romagna
                                - GlobalGap (variante multicentro e non)
                                - Registro Fertilizzazioni
                                - Registro Trattamenti Lombardia - Veneto
                                - Registro Trattamenti Veneto (std. condizionalità)
                                - Scheda Prov. Aut. Trento
                                - Quaderno di Campagna (Lombardia - Veneto)
                                - Scheda Interventi Agronomici
                                - Registro Aziendale Unico
                                - Registri ACA (Disponibili solo su ambienti umbria)",
                           cliente:="Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=9003, idTicketTesting:=0,
                           autore:="Uhalid Abou El Kheir",
                           noteTest:="La stessa pagina gestisce anche la stampa ""Scheda Colturale BIO"", che era gia' stata testata ma visto le modifiche massive direi di ritestarla.
                           La pagina di lancio a cui si fa riferimento e' quella a cui si arriva passando dal Menu stampe new!, selezionando una delle stampe nel elenco, selezionare gli impianti e fare stampa a quel punto si e' nella pagina di lancio che ha diverse opzioni di configurazione per la stampa.",
                           noteTecniche:="La modifica e' legata dietro la chiave di conf siti Filtro_SchedaCampagna_BS che va messa a 'true'")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                          "Stampe Scheda di Campagna",
                           "Gestita stampa nuove operazioni di pascolamento animali (174,175) come altre operazioni colturali",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=0, idTicketSviluppo:=181629, idTicketTesting:=0,
                           autore:="Carlo Giovanardi",
                           noteTest:="Verificare che nella stampa della scheda di campagna compaiano anche le operazioni di pascolamento animali.")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                          "Stampa Global Gap",
                           "Nascosto l'orario nella colonna 'Carenza'",
                           cliente:="Granfrutta Zani",
                           idTicketAssistenza:=180164, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_LC)

            '---------------------------------------------------------------------------

            Riga_Versione("09 Giugno 2025", "139.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_ColturaleBiologico_BS' (server)", "154")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(
                enum_Tipo_Changelog.Feature,
                "Stampa Checklist GlobalGap",
                "Spostate ne sito stampe le seguenti stampe checklist: global gap, rapporto nc, visita ispettiva",
                cliente:="Zani",
                idTicketAssistenza:=0, idTicketSviluppo:=163030, idTicketTesting:=0,
                autore:=DEV_LC
                )

            Riga_Changelog(
                enum_Tipo_Changelog.Feature,
                "Report Abilitazione PdC.",
                "Implementata la pagina stampa Report Abilitazione PdC.",
                cliente:="Zani",
                idTicketAssistenza:=0, idTicketSviluppo:=163026, idTicketTesting:=0,
                autore:="Turci Tommaso"
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

            Riga_Changelog(
                           enum_Tipo_Changelog.Bug,
                           "Scheda Colturale BIO",
                           "Sistemato arrotondamento nella stampa delle Raccolte",
                           cliente:="Studio Pentalab",
                           idTicketAssistenza:=175834, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_LC,
                           noteTest:="",
                           noteTecniche:=""
                           )

            '---------------------------------------------------------------------------

            Riga_Versione("14 Maggio 2025", "138.0.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_ColturaleBiologico_BS' (server)", "154")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                "Stampa Global Gap per l'Irrigazione",
                "Ripristinata la visualizzazione dei dettagli dell'Irrigazione nella stampa",
                cliente:="Apo Scaligera",
                idTicketAssistenza:=175442, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_LC
                )

            '---------------------------------------------------------------------------

            Riga_Versione("12 Maggio 2025", "138.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_ColturaleBiologico_BS' (server)", "154")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                "Invio Log Elastic Search - LOG APPLICATIVI (DataProvider)",
                "Rimosso possibile loop infinito in caso di errore di invio a ElasticSearch",
                cliente:="",
                idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:=DEV_Funcy
                )

            '---------------------------------------------------------------------------

            Riga_Versione("09 Maggio 2025", "137.3.3")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_ColturaleBiologico_BS' (server)", "154")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                "Report Bio",
                "Corretto errore per cui se nei costi era stato addebitato un costo a più distinte dello stesso impianto (nel futuro) questo moltiplicava le righe del report",
                cliente:="Aboca",
                idTicketAssistenza:=174776, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:="Giulia Bottan")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                "Report Orogel - Scheda Aziendale e Scheda Autocertificazione",
                "Corretta estrazione e visualizzazione degli organismi referenti associati agli impianti.",
                cliente:="Orogel",
                idTicketAssistenza:=172010, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:="Tommaso Turci")

            '---------------------------------------------------------------------------

            Riga_Versione("06 Maggio 2025", "137.3.2")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_ColturaleBiologico_BS' (server)", "154")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                "Stampe Orogel - Scheda Aziendale",
                "Corretta visualizzazione report Scheda Aziendale.",
                cliente:="Orogel",
                idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:="Tommaso Turci"
                )

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                "Report - GLOBALGAP e GLOBALGAP Multicentro",
                "Corretto caricamento di data + ora movimento causa cast incorretto",
                cliente:="",
                idTicketAssistenza:=0, idTicketSviluppo:=164638, idTicketTesting:=0,
                autore:="Tommaso Turci"
                )

            '---------------------------------------------------------------------------

            Riga_Versione("05 Maggio 2025", "137.3.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_ColturaleBiologico_BS' (server)", "154")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                "Stampe Orogel",
                "Corretto caricamento e concatenamento padri.",
                cliente:="Orogel",
                idTicketAssistenza:=172010, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:="Tommaso Turci"
                )

            '---------------------------------------------------------------------------

            Riga_Versione("30 Aprile 2025", "137.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_ColturaleBiologico_BS' (server)", "154")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                "Stampa Fattura",
                "Corretto bug che riportava il valore di sconto relativo alla riga di prodotto, nella successiva di tipo 'campione gratuito'",
                cliente:="Cantina Randi",
                idTicketAssistenza:=173185, idTicketSviluppo:=0, idTicketTesting:=0,
                autore:="Gianluca Amoroso"
                )

            '---------------------------------------------------------------------------

            Riga_Versione("24 Aprile 2025", "137.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_ColturaleBiologico_BS' (server)", "154")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(
                enum_Tipo_Changelog.Bug,
                "Report - GLOBALGAP e GLOBALGAP Multicentro",
                "Implementata la visualizzazione dell'ora nelle movimentazioni / rilievi nella stampa GLOBALGAP e GLOBALGAP Multicentro.",
                cliente:="",
                idTicketAssistenza:=0, idTicketSviluppo:=164638, idTicketTesting:=0,
                autore:="Tommaso Turci"
                )

            '---------------------------------------------------------------------------

            Riga_Versione("14 Aprile 2025", "137.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_ColturaleBiologico_BS' (server)", "154")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(
                enum_Tipo_Changelog.Performance,
                "Report Orogel",
                "Ottimizzati i tempi di stampa di tutti i report Orogel. Inoltre, corrretta e re-implementata la visualizzazione delle cooperative padre nei campi appositi.",
                cliente:="Orogel",
                idTicketAssistenza:=0, idTicketSviluppo:=166946, idTicketTesting:=0,
                autore:="Tommaso Turci"
                )

            Riga_Changelog(
                           enum_Tipo_Changelog.Feature,
                          "Pagina Di Lancio Scheda Colturale BIO",
                           " - E' stato fatto un redesign UI della pagina di lancio della stampa Scheda Colturale BIO, in futuro verra' usata anche per le altre stampe (Global Gap etc).
                           Sviluppo fatto da Fabio Benetazzo",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=163377, idTicketTesting:=0,
                           autore:="Uhalid Abou El Kheir",
                           noteTest:="La modifica e' sotto chiave di configurazione_siti ""Filtro_SchedaCampagna_ColturaleBiologico_BS"", si arriva alla pagina passando dal Menu Stampe new e selezionado la Scheda Colturale BIO",
                           noteTecniche:="")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Esportazione Agenda",
                           "Cambiato il modo in cui l'excel viene generato, evitando l'apertura del pop up",
                           "Regione Umbria",
                           170074, 0, 0, "Paolo Netso", noteTest:="Provare la generazione dell'excel (scegliere i movimenti tramite il filtro di ricerca NG)")

            Riga_Changelog(enum_Tipo_Changelog.Security,
                           "Web.Config",
                           "Modificato requestValidationMode da 2.0 a 4.5 Security Hotspots SonarQube",
                           cliente:="Coldiretti",
                           idTicketAssistenza:=0, idTicketSviluppo:=163182, idTicketTesting:=0,
                           autore:="Gianluca Amoroso", noteTest:="Nessun Test")

            '---------------------------------------------------------------------------

            Riga_Versione("04 Aprile 2025", "136.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_ColturaleBiologico_BS' (server)", "154")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Esportazione Agenda",
                           "Aggiunto attributo maxRequestLength (fissato a 20 MB) per permettere di effettuare correttamente l'esportazione nei casi in cui con il filtro di ricerca vengono scelte circa 100 K movimenti",
                           "Regione Umbria",
                           noteTest:="Provare l'esportazione di circa 100 K movimenti, verificare che funzioni",
                           idTicketAssistenza:=170074, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Paolo Netso")

            Riga_Changelog(enum_Tipo_Changelog.Performance,
                           "Esportazione Agenda",
                           "Migliorato l'inserimento dei record nella tabella tmp_agenda, usando un inserimento in chunks da 1000 record",
                           "Regione Umbria",
                           noteTest:="Non testabile",
                           idTicketAssistenza:=170074, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Paolo Netso")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Esportazione Agenda",
                           "Quando l'utente sceglie di includere nell'esportazione anche i codici ACA e/o la ZVN, nei dati aggiuntivi del filtro di ricerca vengono abilitati gli switch corrispondenti (Contributi ACA e/o Dati Catastali Appezzamento della sezione Piano Colturale). 
                           Gestito lato backend l'invio dell'informazione, inserendo una nuova property nella classe ParametriFiltroRicercaNG",
                           "Regione Umbria",
                           noteTest:="Verificare che l'attivazione degli switch in base alla configurazione scelta nella stampa",
                           idTicketAssistenza:=170074, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Paolo Netso")

            '==================================

            Riga_Versione("19 Marzo 2025", "136.0.1")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_ColturaleBiologico_BS' (server)", "154")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Analisi Fitofarmaci",
                           "Rimosso da sito stampe e ripristinato su sito analisi il VisualizzatoreFile in quanto non correlato alle stampe.",
                           "Apofruit",
                           idTicketAssistenza:=168074, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Tommaso Turci")

            '==================================

            Riga_Versione("17 Marzo 2025", "136.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_ColturaleBiologico_BS' (server)", "154")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Requisiti("GiasLan",
                           "Per passaggio a stampe2010 per excel automatico da griglia", "12/03/2025")


            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Gias Lan -- Excel Automatico da Griglia",
                           "Migrazione excel da griglia",
                           "Cantine",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Marco Lucchi")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Scheda di Campagna",
                           "Sistemata stampa se attivo il flag 'Visualizza Campo solo su frontespizio'",
                           "",
                           idTicketAssistenza:=167020, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Lorenzo Casanova")

            '==================================


            Riga_Versione("07 Marzo 2025", "135.3.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'Filtro_SchedaCampagna_ColturaleBiologico_BS' (server)", "154")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "PdC - Stampa Rapporto di Prova",
                           "Corretto caricamento del valore WS_Timeout per evitare errori di caricamento del report Stampa Rapporto di Prova.",
                           "Granfruttazani",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=164620,
                           autore:="Tommaso Turci")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Report Orogel",
                           "Effettuata minore correzione grafica sui report nuovi per orogel.",
                           "Orogel",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=166415,
                           autore:="Tommaso Turci")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Report GLOBALGAP",
                           "Rimossa visualizzazione ora movimento per temporaneamente uniformare report in vista di sviluppo futuro nel quale verrà reintegrata.",
                           "",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=164638,
                           autore:="Tommaso Turci")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "PdC - Stampa Rapporto di Prova",
                           "Implementata visualizzazione di LDM a tre decimali.",
                           "Fruttagel",
                           idTicketAssistenza:=0, idTicketSviluppo:=166323, idTicketTesting:=0,
                           autore:="Tommaso Turci")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Scheda colturale BIO - pagina di lancio",
                           " - Cambiata la pagina di lancio per la stampa ""Scheda Colturale BIO""",
                           "Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=163377, idTicketTesting:=0,
                           autore:=DEV_Uhalid,
                           noteTecniche:="La modifica e' sotto chiave di configurazione_siti ""Filtro_SchedaCampagna_ColturaleBiologico_BS""")

            '==================================


            Riga_Versione("28 Febbraio 2025", "135.2.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Report Orogel",
                           "Effettuato miglioramento estetico minore (spazio mancante fra comune e sigla provincia)",
                           "Orogel",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Tommaso Turci")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                "Stampa UMA Rendicontazione",
                "Ripristinato funzionamento stampa, in errore dall'ultima versione",
                "Regione Umbria GARI", 0, 163342, 0, "Gianluca Amoroso", "TFS 8925")

            '==================================

            Riga_Versione("21 Febbraio 2025", "135.1.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                            "Esportatore Universale: cambiato il modo in cui vengono estratti i contributi ACA",
                            "Ora vengono presi gli ACA dei soli esercizi attivi alla data dell'operazione, mentre prima venivano presi quelli di tutti gli esercizi associati agli impianti coinvolti nell'operazione",
                            cliente:="",
                            idTicketAssistenza:=0,
                            idTicketSviluppo:=163377,
                            idTicketTesting:=0,
                            autore:="Paolo Netso")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Menu stampe (new!) -> Esportazione Agenda",
                           "Quando l'utente clicca su 'Esportazione Agenda' viene adesso indirizzato verso la pagina Esportatore_Universale.aspx e non verso la pagina del filtro di ricerca NG. Nella pagina dell'esportatore è stato introdotto un pulsante 'Filtra ed esporta dati', che apre il filtro di ricerca NG per permettere la scelta dei movimenti, dopodichè viene creato il file excel. È stato quindi invertito l'ordine delle pagine (prima l'ordine era 'filtro di ricerca NG' -> 'esportatore', adesso l'ordine è 'esportatore' -> 'filtro di ricerca NG'). Le modifiche sono valide solo per l'esportazione agenda e solo quando il filtro di ricerca NG è abilitato, tutti gli altri casi sono rimasti invariati ",
                           "",
                           0,
                           163377,
                           0,
                           "Paolo Netso",
                           "Necessario impostare a 2 (= filtro di ricerca NG) il valore dell'impostazione 891",
                           "Verificare che l'esportazione agenda funzioni correttamente. Verificare il funzionamento delle altre esportazioni, che deve rimanere invariato.")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Correzione report Scheda Aziendale e Scheda Autocertificazione per Orogel",
                           "Corretti i report secondo precisazioni Orogel e implementati controlli per valori irregolari in corrispondenza di indirizzo residenza e impresa + cap impresa.",
                           cliente:="Orogel",
                           idTicketAssistenza:=0,
                           idTicketSviluppo:=165092,
                           idTicketTesting:=0,
                           autore:="Tommaso Turci")

            Riga_Changelog(enum_Tipo_Changelog.Bug,
                           "Agro_Sequenze",
                           "Ripristino porzione di codice rimossa erroneamente",
                           "",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:=DEV_Funcy)

            '==================================

            Riga_Versione("17 Febbraio 2025", "135.0.0")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "17/02/2025")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Mostra Firma ODC e Default Mostra Data",
                           "Aggiunte 3 nuove impostazioni utente:
                        - Mostra Firma ODC (Stampe BIO): rende disponibile il checkbox 'Mostra Firma ODC' nelle stampe, pre-selezionato di default. Quando questo checkbox viene selezionato, aggiunge una sezione per la firma con nome e cognome dell'utente loggato
                        - Default Mostra Data Stampa (Stampe BIO): determina se il relativo checkbox deve essere pre-selezionato nelle stampe BIO
                        - Default Mostra Data Stampa (Registri ACA): analoga alla precedente ma per i Registri ACA
                        
                        Inoltre le stampe Materie Prime Bio e Vendite Bio, avviate dal menu stampe (new!), passeranno ora dalla pagina Report Biologico. E' stato aggiungo il check mostra data stampa e Mostra Firma ODC alla pagina.",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=163377, idTicketTesting:=0,
                           autore:="Uhalid Abou El Kheir",
                           noteTest:="La stampa Registri ACA è disponibile solo negli ambienti Umbria. Le impostazioni utente delle stampe BIO sono accessibili in 'STAMPE' -> 'Scheda BIO', mentre quella per i Registri ACA si trova nell'omonima sezione sotto 'STAMPE'. Le impostazioni BIO influiscono su: scheda colturale, materie prime e vendite")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Ora Movimento in GLOBALGAP e GLOBALGAP Multicentro",
                           "Implementata la visualizzazione dell'ora movimento nella stampa GLOBALGAP e GLOBALGAP Multicentro.",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=163254, idTicketTesting:=0,
                           autore:="Tommaso Turci")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Trasferite di soluzione da Analisi/PianiCampionamento a Stampe più stampe",
                           "Spostate le stampe 'Analisi Fitofarmaci' da soluzione Analisi a soluzione Stampe, 'Etichetta di Prova' e 'Rapporto di Prova' da soluzione PianiCampionamento a Stampe.",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=163026, idTicketTesting:=0,
                           autore:="Tommaso Turci",
                           noteTest:="Le stampe sono accessibili su Piani di Campionamento, ma bisogna abilitare i bottoni di stampa rapida per accederci. 
                                 La stampa 'Rapporto di Prova', invece, è attualmente inaccessibile da interfaccia, stampabile da un bottone disattivato a forza.")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Esportatore Universale: nuove checkbox nel caso in cui si esportano delle operazioni di agenda",
                            "Aggiunte nuove opzioni 'ZVN', 'ACA' e 'Destinazioni d'uso'. Nascoste parti della pagina non più necessarie quando si effettua l'export delle operazioni di agenda. ",
                           cliente:="",
                           idTicketAssistenza:=0, idTicketSviluppo:=0, idTicketTesting:=0,
                           autore:="Paolo Netso",
                           noteTest:="Verificare che con le opzioni 'ZVN', 'ACA' e 'Destinazioni d'uso' abilitate le colonne appaiano correttamente nell'excel esportato. I codici ACA non saranno più inseribili come codici aggiuntivi esercizio, ma da un apposito menu a tendina che è in sviluppo posizionato nell'anagrafica esercizio, quindi al momento non è possibile inserire valori; quando vedrete nelle note di versione GiasNG questo nuovo sviluppo (presumibilmente la settimana del 24 febbraio) potrete procedere ai test completi. Attualmente l'unico test che si può fare con questa estrazione è vedere i tre nuovi flag ('ZVN', 'ACA' e 'Destinazioni d'uso') e le corrispondenti colonne su excel (se selezionati), ma ancora senza valori per gli ACA")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                           "Stampe ACA",
                           "Implementate nuove logiche per l'estrazione dei Codici ACA associati agli impianti",
                           cliente:="Regione Umbria",
                           idTicketAssistenza:=0, idTicketSviluppo:=163377, idTicketTesting:=0,
                           autore:="Marco Cecalupo",
                           noteTest:="I codici ACA non saranno più inseribili come codici aggiuntivi esercizio, " &
                      "ma da un apposito menu a tendina che è in sviluppo posizionato nell'anagrafica esercizio, " &
                      "quindi al momento non è possibile inserire valori; " &
                      "quando vedrete nelle note di versione GiasNG questo nuovo sviluppo (presumibilmente la settimana del 24 febbraio) " &
                      "potrete procedere ai test completi. " &
                      "Attualmente l'unico test che si può fare con queste stampe è vedere che non si vedano più i codici precedentemente inseriti, " &
                      "ma conviene aspettare che lo sviluppo sia completo)")

            Riga_Changelog(enum_Tipo_Changelog.Feature,
                "Stampe UMA Richiesta/Rendicontazione",
                "Modificata nome colonna 'Fascicolo' in 'Origine' e gestiti i relativi valori nuovi per porting dal planning a piano colturale effettivo",
                "Regione Umbria GARI", 0, 163342, 0, "Gianluca Amoroso", "TFS 8925",
                "Non essendo più usato il fascicolo, nelle griglie delle stampe che ne mostravano il valore relativo al gruppo colturale UMA, " &
                "ora possono essere mostrate a seconda dei dati inseriti nella richiesta, le seguenti scritte fisse: " &
                "'Piano Colturale', 'Trasferimenti', 'Anticipi', 'Coltura non legata a fascicolo'")

            '==================================

            Riga_Data("12 Febbraio 2025")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "16/12/2024")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Text("Stampa Rendicontazione UMA",
                     "Modificate le seguenti diciture dell'ultima pagina della stampa: " &
                     "- 'Il CAA/Soggetto compilatore, avendo provveduto al rilascio nel SIGPA della rendicontazione n.' sostituita da " &
                     "'Il CAA/Soggetto compilatore, avendo provveduto al rilascio nel GARI – UMA della rendicontazione n.' e" &
                     "- 'che la rendicontazione e i relativi allegati sono stati compilati sulla base del fascicolo e secondo le indicazioni fornite dall'Azienda' sostituita da " &
                     "'che la rendicontazione e i relativi allegati sono stati compilati sulla base del fascicolo e secondo le indicazioni fornite dall'Azienda anche riguardo alla dichiarazione della rimanenza dei litri di gasolio acquistati e non utilizzati'", "Regione Umbria")

            '==================================

            Riga_Data("31 Gennaio 2025")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "16/12/2024")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Bug("Security - AgronicaCoreParametri",
                     "Fix per recuperare nome db anche in caso di stringa connessione codificata")

            '==================================

            Riga_Data("24 Gennaio 2025")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "16/12/2024")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Bug("Stampa Bilancio Fertilizzazioni",
                    "Correzione errore di sintassi della query della stampa che impediva la sua generazione",
                     "Tecnoterr",
                     idPerforma:=34538)

            Riga_Bug("Stampa Autocertificazione",
                     "Aggiunti controlli in caso di valori nulli/vuoti + resi riferimenti a coop. referente in Autocertificazione dinamici invece che fissi a Orogel Fresco.",
                     "Orogel",
                     idPerforma:=34530)

            '==================================

            Riga_Data("20 Gennaio 2025")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "16/12/2024")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Text("Autenticazione",
                      "Prima di ogni chiamata ai web method è stato introdotto un controllo di sicurezza (global asax)",
                            noteTest:="Verificare che una pagina qualsiasi di questo sito funzioni correttamente",
                            noteTecniche:="Il controllo di sicurezza viene effettuato solo quando il valore della chiave ControlloAutenticazioneConAuthCookie nei appsettings è uguale a true")

            Riga_Text("Report Biologico Tabellare (versione Aboca)",
                      "Aggiunta colonna apporti Fosforo al report biologico tabellare",
                            noteTest:="Lanciare l'estrazione in un periodo con carichi / scarichi di fertilizzanti con fosforo")

            Riga_Text("ACA - riorganizzazione interna tipi enumerativi per stampe",
                      "Regione Umbria",
                      noteTest:="rilanciare le stampe aca per controllare che vengano prodotte correttamente")

            Riga_Bug("Stampa Scheda Giacenze Magazzino",
                    "Correzione errore che genera la stampa vuota in caso di lancio con la opzione 'stampa raggruppata'",
                     idPerforma:=34515)

            '==================================

            Riga_Data("10 Gennaio 2025")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "16/12/2024")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Bug("UMA bug assegnazione in caso di rimanenze, trasferimenti e recupero accise",
                    "Corretto bug che mostrava erroneamente un'assegnazione uguale ai trasferimenti o recuperi accise confermati all'interno del verbale Afor",
                     idPerforma:=33957)

            '==================================

            Riga_Data("20 Dicembre 2024")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "16/12/2024")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Bug("Report Orogel",
                    "Corretto il caricamento delle cooperative. Corretta la visualizzazione delle diciture a fondo pagina nelle stampe nelle quali mancavano.")

            '==================================

            Riga_Data("18 Dicembre 2024")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "16/12/2024")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Text("Security - Nomi Report",
                      "Ora è possibile salvare caratteri accentati nei nomi dei report
                      Aggiunti controlli centralizzati anche lato front end",
                      noteTecniche:="1. Caratteri pericolosi in HTML e JavaScript Injection: 
                         Tag HTML: <, >.
                         Caratteri speciali HTML: & (entità HTML come &), ' e "" (delimitatori di attributi).
                         Caratteri JavaScript: ()(funzioni), {}, [](strutture di codice), ;, = (assegnazioni).
                      2. Caratteri pericolosi in SQL Injection:
                         Separatori e commenti: ;, /, *.
                         Delimitatori stringhe: ', "".
                      3. Caratteri di escape:
                         Backslash: \ (utilizzato per fare escaping).",
                      noteTest:="Nella pagina Piano Colturale Per Catasto Griglia (Menu Stampe (new!)>Piano Colturale per Catasto Griglia), provare a salvare un report contentente uno o più caratteri indicati nelle noteTecniche: i caratteri devono essere rimossi. 
                      NB: un nome report che contiene solo caratteri non ammessi (ex ;;;) darà errore.",
                      cliente:="Aboca", idPerforma:=34022)

            Riga_Text("Stampe 'Scheda di Campagna GLOBALGAP' e 'Scheda di Campagna GLOBALGAP Multicentro'",
                        "Rimossi riferimenti ad Agronica nel pié di pagina della stampa per la regione Umbria")

            Riga_Bug("Report Orogel - Cooperative multiple",
                    "Aggiunto un check per casi nulli nel caricamento delle cooperative.")

            '==================================

            Riga_Data("16 Dicembre 2024")

            Riga_Requisiti("Migra",
                           "Nuova colonna Flag_Encrypted in tabella Connessioni",
                           ver:="778")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunta chiave 'UserPwdConnectionString_toCrypt' (super server)", "150")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "per nuovo codice anagrafe esercizio", "16/12/2024")

            Riga_Requisiti("Agenda",
                           "Per passaggio su filtrone nuova stampa Registri ACA", "16/12/2024")

            Riga_Text("Stampe 'DDT Emesso', 'Scheda di Campagna' e 'Scheda di Campagna Multicentro'",
                        "Rimossi riferimenti ad Agronica nel pié di pagina della stampa per la regione Umbria",
                              noteTest:="PROVENIENTE DAL RAMO UMBRIA - GIA' TESTATO")

            Riga_Text("Nuova Stampa Registri ACA",
                      "Aggiunto nuovo report rpt: codice anagrafe 'Nr Domanda ACA' in appezzamenti/note in sezioni Concimazioni, Trattamenti ed Irrigazioni;" &
                      "aggiunto modalità di applicazione in note Concimazioni; aggiunta data odierna in footer",
                      "Regione Umbria",
                      0,
                      "",
                      "Popolare 'Nr Domanda ACA' su esercizi e modalità di applicazione in Concimazioni QDC e verificare che la stampa risulti corretta")

            Riga_Text("Security",
                      "Attivata modalità nel web.config per criptare il viewstate delle pagine",
                      noteTest:="Non testabile")

            Riga_Text("Security",
                      "Interventi vari per evitare il passaggio della stringa di connessione al db (8507)")

            Riga_Bug("Stampe Orogel",
                     "Corretto il funzionamento della stampa vuota in Impegnativa Coltivazione Conferimento; Correzione di typo e nome azienda tagliato in Scheda Autocertificazione.")

            Riga_Text("Modifica a stampe per ACA",
                      "aggiunta numero domanda ACA, modalità applicazione fertilizzazioni, data stampa, nelle seguenti stampe (non voci di menu dedicati, ma dati che vengono mostrati se presenti su database):" &
                        "- Scheda Colturale Bio: nr domanda ACA (se esistente) vicino all'appezzamento, modalità di applicazione (se esistente) nelle note delle fertilizzazioni, data di stampa visibile in basso a sx se selezionato il flag 'mostra data stampa' nella maschera di lancio" &
                        "- Scheda Materie Prime e Scheda Vendite: nr domanda ACA (se esistenti) di tutti gli esercizi coinvolti nella stampa (fa fede la data di validità del codice 'nr domanda ACA' rispetto all'intervallo di stampa) nella voce 'impegni aggiuntivi' inserita nella sezione di testata (prima di 'via'), data di stampa visibile in basso a sx se selezionato il flag 'mostra data stampa' nella maschera di lancio",
                      "Regione Umbria",
                      noteTest:="Test:" &
                        "- aggiungere codici aggiunti esercizio relativi al 'nr domanda ACA', aggiungere modalità di applicazione nelle fertilizzazioni e vedere che vengano stampate correttamente" &
                        "- selezionare/deselezionare il flag 'mostra data stampa' nella maschera di lancio e vedere che le tre stampe + 'Registri ACA' la mostrino/nascondano di conseguenza")

            Riga_Text("Stampe Orogel - Scheda Aziendale e Impegnativa Coltivazione Conferimento",
                      "Implementata la visualizzazione di molteplici cooperative nell'intestazione di Impegnativa Coltivazione Conferimenti e Scheda Aziendale.
                      (tali cooperative vengono prelevate dagli esercizi selezionati nel filtro o appartenenti alle aziende/impianti selezionate/i nel filtro.)")

            '==================================

            Riga_Data("12 Dicembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           ver:="777")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Bug("Sequence",
                     "Alla creazione delle Sequence startValue = 1",
                     noteTest:="")

            '==================================

            Riga_Data("06 Dicembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           ver:="777")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Bug("Report Orogel",
                    "Effettuate seguenti correzioni su report per Orogel: Caricamento particelle e impianti sia in Scheda Aziendale che Scheda Autocertificazione.")

            Riga_Bug("Stampa colturale Bio",
                      "Corretta gestione arrotondamenti per la quantità totale e parziale",
                     "Agrites",
                     33572,
                     noteTest:="Selezionare un sottoinsieme di impianti di una semina ed un trattamento e verificare che stampi il valore parziale per quegl'impianti; Selezionare tutti gli impianti di una semina ed un trattamento e verificare che stampi il valore totale originale. LO STESSO PROBLEMA POTREBBE ESSERCI IN ALTRE STAMPE, ATTENDIAMO RISCONTRI")

            '==================================

            Riga_Data("29 Novembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           ver:="777")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Bug("Report Orogel",
                    "Effettuate seguenti correzioni su report per Orogel: Corretta visualizzazione indirizzo impresa,
                    espansi campi per foglio/numero/subalterno/sezione perché non taglino numeri a più di 3 cifre
                    e corretta visualizzazione n. libro soci su Despar e Conad.")

            '==================================

            Riga_Data("22 Novembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           ver:="777")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Bug("Web Config",
                     "Corretta dipendenza libreria System.Memory",
                     noteTest:="Non testabile")

            Riga_Bug("Report Orogel",
                    "Scheda Aziendale: Corretto il caricamento delle particelle relative al centro aziendale. Ora vengono anche caricate particelle con nessuna specie assegnata.
                    Inoltre, è stata corretta la visualizzazione di comune e provincia residenza/aziendale/nascita in tutti i report, in modo da non stampare nulla nel caso sul database sia riportato uno dei dati come '00' o 'Non Definita', che precedentemente venivano stampati.")

            '==================================

            Riga_Data("15 Novembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           ver:="777")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Text("Stampe report Orogel",
                      "Adeguati i report perché possano supportare la modalità stampa fronte-retro. Note test: testare stampe sia su filtrone vecchio che nuovo.")

            '==================================

            Riga_Data("08 Novembre 2024")

            Riga_Requisiti("Migra",
                           "-Nuova colonna smtp_password_isEncrypted in tabella Configurazione_Servizi",
                           ver:="777")

            Riga_Requisiti("Configurazione_Siti",
                           "Inserite le chiavi 'password_smtp_isEncrypted', 'PasswordArteaWS_isEncrypted' e 'cr'", "149")

            Riga_Requisiti("CoreWS",
                           "Per modifiche relative alla codifica e decodifica delle password usando AES", "08/11/2024")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Text("Codifica e decodifica password_smtp e PasswordArteaWS",
                      "-Introdotti metodi per la codifica e decodifica usando la classe AES
                            -Introdotta chiave cr2 in web.config o app.config")

            Riga_Text("Gestione sequence default",
                      "Gestione sequence per progressivi chiavi tabelle di default attive per tutti. 
                      Disattivabili esclusivamente impostando a False la chiave Allow_Sql_Sequence nell'appsettings")

            Riga_Text("Security - Query",
                      "Parametrizzate massivamente molte query in filtri aggiuntivi, order by e clausole IN per impedire sql injection")

            Riga_Text("Lotto impianto su stampe",
                      "Ovunque c'era il riferimento all'appezzamento e' stato aggiunto il lotto, sulle seguenti stampe:
                            - Scheda colturale biologico
                            - Globalgap (sia nella variante multicentro che non)
                            - Scheda di campagna (sia nella variante multicentro che non)
                            - Registro dei trattamenti")

            Riga_Bug("Stampa Riepilogo Utilizzo Prodotti",
                     "- Ripristinata la stampa (precedentemente veniva sempre aperta senza l'elenco dei prodotti)",
                     "Coldiretti",
                     noteTest:="è la stampa che si lancia dalla pagina dei magazzini; ci siamo accorti del non funzionamento grazie alle mail degli errori query che ci arrivano da Coldiretti")

            '==================================

            Riga_Data("30 Ottobre 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto a enumStampe2010 lo Statistometro", "144")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CoreWS",
                           "Per modifiche legate alla codifica e decodifica semplice delle stringhe", "17/09/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Text("Report Orogel",
                      "Eliminati il logo e testo Agronica dai footer; Corretto numero di iscrizione al libro soci su stampa Autocertificazione (nel testo in basso); 
                      Corretto il caricamento del codice fiscale del rappresentante legale in alcune stampe; Corretto il numero di decimali visualizzati nei dati in Scheda Aziendale, 
                      rimossa la colonna 'Titolo di possesso'; Verificata la stampa dell'anno corrente su tutte le stampe.")

            '==================================

            Riga_Data("24 Ottobre 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto a enumStampe2010 lo Statistometro", "144")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CoreWS",
                           "Per modifiche legate alla codifica e decodifica semplice delle stringhe", "17/09/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Text("Security",
                      "Rimossi tutti i file di librerie di terze parti non utilizzate")

            '==================================

            Riga_Data("22 Ottobre 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto a enumStampe2010 lo Statistometro", "144")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CoreWS",
                           "Per modifiche legate alla codifica e decodifica semplice delle stringhe", "17/09/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Bug("Stampe contabili",
                      "Corretta gestione del carattere tedesco ""ß"" che trovandosi nel nome file impediva la visualizzazione della stampa",
                     "La Spinosa", 33103)

            '==================================

            Riga_Data("21 Ottobre 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto a enumStampe2010 lo Statistometro", "144")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CoreWS",
                           "Per modifiche legate alla codifica e decodifica semplice delle stringhe", "17/09/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Text("Correzione anno Autocertificazione",
                      "Corretta la visualizzazione dell'anno corrente nella stampa Autocertificazione.")

            Riga_Text("Statistometro",
                      "- velocizzata la pagina rimuovendo l'autofit delle colonne
                       - modifiche per renderla asincrona e far comparire il loader")

            '==================================

            Riga_Data("11 Ottobre 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto a enumStampe2010 lo Statistometro", "144")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CoreWS",
                           "Per modifiche legate alla codifica e decodifica semplice delle stringhe", "17/09/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Text("Introdotto un quarto punto nella sezione 'Dichiara' di una rendicontazione",
                            "Introdotta una quarta spunta nella sezione 'Dichiara' della stampa rendicontazione che dichiara che l'azienda dispone delle attrezzature necessarie per esequire le lavorazioni elencate")

            Riga_Text("Security - XSS Protection",
                      "- Aggiunto controllo per bloccare script injection nelle chiamate ai web method" &
                      "- Aggiunti hiddenfield per impedire injection script nelle variabili javascript",
                      noteTecniche:="Occorre impostare il parametro 'DetectScriptInjection' in appsettings.config")

            '==================================

            Riga_Data("10 Ottobre 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto a enumStampe2010 lo Statistometro", "144")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CoreWS",
                           "Per modifiche legate alla codifica e decodifica semplice delle stringhe", "17/09/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Bug("Stampa Scheda Giacenze Magazzino",
                     "Fix per archiviazione precedente: non utilizzava la data stampa inserita dall'utente",
                     noteTest:="Stampa di Giacenze con varie date, oggi, data passata e nessuna data (non fa stampare). Deve funzionare correttamente ")

            '==================================

            Riga_Data("19 Settembre 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto a enumStampe2010 lo Statistometro", "144")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CoreWS",
                           "Per modifiche legate alla codifica e decodifica semplice delle stringhe", "17/09/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Bug("Correzioni report Orogel",
                     "Corretto caricamento e visualizzazione dati in report Autocertificazione e Scheda Aziendale. (Introdotte anche minori migliorie estetiche)")

            '==================================

            Riga_Data("17 Settembre 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto a enumStampe2010 lo Statistometro", "144")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CoreWS",
                           "Per modifiche legate alla codifica e decodifica semplice delle stringhe", "17/09/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Text("Stampa DAA Registro Accise",
                      "Modifica intestazione per stampare l'anno relativo alle date impostate nella pagina di filtro e non quello relativo alle date di emissione dei documenti",
                      cliente:="Borgoluce", idPerforma:=30932)

            Riga_Text("Codifica e decodifica stringhe",
                      "Introdotta la possibilità di effettuare una codifica semplice delle stringhe",
                      noteTecniche:="Per poter abilitare la codifica semplice la proprietà UsaCodificaSemplice in appsettings deve essere impostata a true")

            '==================================

            Riga_Data("13 Settembre 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto a enumStampe2010 lo Statistometro", "144")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per redirect a statistometro da NG", "17/07/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Text("DataProvider - Log Agro_Sequenze",
                      "Modifica Scrivi_Log per loggare anche NomeTabella in caso di exception",
                      noteTest:="Non testabile da assistenza")

            Riga_Text("Download Allegati - Path visibile lato client",
                      "Sostituzione metodologia dowload allegato per le seguenti pagine:
                      - Statistiche Utilizzo (new!) ",
                      noteTest:="Verificare che la stampa funzioni correttamente")

            Riga_Text("Download Allegati - Path visibile lato client",
                      "Rimossi Vecchi Riferimenti (commentati) ",
                      noteTest:="Non testabile")

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

            Riga_Text("Stampe Orogel",
                      "Aggiunta funzionalità stampa massiva per le stampe per Orogel - è possibile stampare la singola stampa per più aziende al tempo stesso selezionando più imprese/impianti/esercizi al tempo stesso.")

            '==================================

            Riga_Data("03 Settembre 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto a enumStampe2010 lo Statistometro", "144")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per redirect a statistometro da NG", "17/07/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Bug("Stampa Giacenze magazzino",
                      "La stampa raggruppata non funzionava più a causa di un SET ISOLATION LEVEL che finiva in mezzo alla query stessa",
                     "Al Molejn", 32181)

            Riga_Text("Stampa DDT Emesso",
                      "Aggiunta valorizzazione corretta mail del centro se Impostazione Superuser SUPERUSER_INTESTAZIONE_STAMPA_CENTROAZIENDALEPARTENZA = 877 Attiva (1) utilizzata per stampare l'indirizzo del centro di partenza ",
                      "INALCA",
                      0,
                      "",
                      "Fare prove su azienda con più centri e un indirizzo mail diverso per ogni centro")

            '==================================

            Riga_Data("23 Agosto 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto a enumStampe2010 lo Statistometro", "144")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per redirect a statistometro da NG", "17/07/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Text("Security",
                      " - Aggiornamento libreria RestSharp da 106.11.7 a 106.15.0 [fix CVE-2021-27293]")

            Riga_Text("SQL Data Provider nuova impostazione e mail di log",
                      " - Reinserita la possibilità di ricevere una mail di log per le query in errore" &
                      " - Inserita la possibilità di impostare la non esecuzione della query originale in caso di errore di parametrizzazione")

            Riga_Bug("Stampa Orogel",
                      "Corretta query SQL caricamento intestazione.")

            '==================================

            Riga_Data("20 Agosto 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto a enumStampe2010 lo Statistometro", "144")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per redirect a statistometro da NG", "17/07/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Bug("Report Piano Colturale per Catasto Griglia",
                      "Correzione di un apparente caricamento infinito che impediva l'utilizzo della pagina se si ricercava un report salvato nell'elenco a discesa, prima di averne salvato almeno uno
                      Corretto errore che impediva la sostituzione dei caratteri speciali",
                      noteTest:="Segnalazione emersa dai test interni a seguito dei test per 'Hardening Sicurezza Viste/Report'")

            '==================================

            Riga_Data("02 Agosto 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto a enumStampe2010 lo Statistometro", "144")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per redirect a statistometro da NG", "17/07/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Text("Hardening Sicurezza Viste/Report",
                      "Aggiunto controllo per evitare HTML/Js injections. I caratteri speciali inseriti vengono salvati con le relative codifiche.",
                      noteTest:="ex. sulla pagina Report Colturale Catasto creare e salvare un report che contiene il seguente testo '&lt;script>'.
                      Ricaricando la pagina i caratteri speciali < e > dovranno essere stati sostituiti con &amp;lt; e &amp;gt;")

            '==================================

            Riga_Data("18 Luglio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto a enumStampe2010 lo Statistometro", "144")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per redirect a statistometro da NG", "17/07/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Bug("Stampa Orogel - GLOBALGAP",
                      "Corrette varie inconsistenze nel report.")

            Riga_Bug("Stampa Lettera con Elenco Conferenti",
                      "Aumentato numero decimali del prezzo al Kg da due a tre e migliorato ordinamento bolle a parità di conferente, articolo e data", "Fruttagel", 31455)

            '==================================

            Riga_Data("17 Luglio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto a enumStampe2010 lo Statistometro", "144")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per redirect a statistometro da NG", "17/07/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "17/07/2024")

            Riga_Text("SQL Sequence",
                      "Aggiunto parametro attivazione in appsettings")

            '==================================

            Riga_Data("15 Luglio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunto a enumStampe2010 lo Statistometro", "144")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per redirect a statistometro da NG", "15/07/2024")

            Riga_Requisiti("Agenda",
                           "Per redirect a statistometro da .net", "15/07/2024")

            Riga_Text("Statistometro",
                      " - Spostato statistometro da Profilazione a stampe con grafica nuova")

            Riga_Text("Contatori",
                      "Normalizzata funzione di richiamo stack counter", "", 0, "", "")

            '==================================

            Riga_Data("12 Luglio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti nuovi enum stampe per piano colturale catasto griglia", "143")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Bug("Report per Orogel - GlobalGAP-GRASP", "Corretta una textbox che veniva tagliata in stampa.")

            '==================================
            Riga_Data("08 Luglio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti nuovi enum stampe per piano colturale catasto griglia", "143")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Bug("Piano colturale per catasto griglia",
                     " - Modifica Tipologia Varietale iniziale, viene data la priorità al gruppo variatale del anagrafica",
                        noteTecniche:="Cambiata priorità di come viene letta la Tipologia Variatale iniziale, prima usando cul_cod trovava il gruppo_variatale (grva_cod)
                        passando tramite la tabella [Codifica_Varieta_OIPomodorodaIndustriaNordItalia], se non riusciva a trovarla (cul_cod non mappato nella tabella) usava quello che c'è in anagrafica,
                        ora è stato invertito quindi viene data priorità a quello delle anagrafiche ")

            '==================================

            Riga_Data("28 Giugno 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti nuovi enum stampe per piano colturale catasto griglia", "143")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Bug("Piano colturale per catasto",
                     " - Fix colonna subalterno non visibile")

            '==================================

            Riga_Data("21 Giugno 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti nuovi enum stampe per piano colturale catasto griglia", "143")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Bug("Elastic Search",
                     "Inserita gestione escaping per log applicativi inviati ad elastic search", "Coldiretti",
                     noteTecniche:="Aggiunto remove escaping pre e post serializzazione",
                     noteTest:="no test")

            Riga_Bug("Piano colturale per catasto griglia",
                     "- Fix bug su lettura dati
                     - WaitFrame non veniva rimosso dove aver cancellato un report")

            Riga_Bug("Stampe Certificati Pomodoro PDF e XLS",
                     "Corretta estrazione nominativo del vettore, per i contatti di tipo 'persona fisica'",
                     "Fruttagel")

            '==================================

            Riga_Data("14 Giugno 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti nuovi enum stampe per piano colturale catasto griglia", "143")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Text("Log Provider",
                      " Modificato il valore di default da logga solo su file a logga solo su DB", noteTest:="Non testabile")

            Riga_Text("Tracciabilità degli appezzamenti esteso",
                      "- convertito l'excel ""Piano Colturale per Catasto"" a kendo Grid
                      - messa la possibilità di molteplici viste su una stessa kendo grid
                      - Aggiunta colonna ISTAT Codice Comune alla griglia
                      - Modificata lettura di tipologia variatale",
                      cliente:="ASIPO", noteTest:="excel rimane accessibile, creata nuova voce ""Piano Colturale per Catasto griglia"" nel menu stampe bootstrap ")

            Riga_Text("Rilascio modifiche UMA schedulate per il 19/04/2024",
                     "Riepilogo interventi: " &
                     "- Introdotta una quarta spunta nella sezione 'Dichiara' della stampa rendicontazione: 'Dichiara che l'azienda dispone delle attrezzature necessarie per esequire le lavorazioni elencate'.",
                     "Regione Umbria",
                     noteTest:="Non testare")

            Riga_Text("Stampa Global Gap e Global Gap Multicentro",
                     "Aggiunto switch 'Visualizza Lotto Impianto/Esercizio'",
                        cliente:="ASIPO")

            Riga_Text("Stampe per Orogel - GLOBALGAP",
                      "Aggiornato il report GLOBALGAP secondo richieste.")

            '==================================

            Riga_Data("03 Giugno 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti nuovi enum stampe", "139")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Bug("Piano colturale per catasto",
                     "Fix lettura sbagliata per biologico")

            '---------------------------------------------------------------------------

            Riga_Data("29 Maggio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti nuovi enum stampe", "139")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Bug("Stampa Global Gap e Scheda di Campagna",
                     "Sistemati arrotondamenti per la stampa dell'Operazione Confusione/Disorientamento Sessuale",
                        idPerforma:=30318, cliente:="Granfruttazani")

            '---------------------------------------------------------------------------
            Riga_Data("27 Maggio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti nuovi enum stampe", "139")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Bug("Export Excel degli Scarichi di Magazzino",
                     "Sistemato Export Semine con stesso prodotto ma Lotti diversi",
                        idPerforma:=30200, cliente:="Genagricola")

            Riga_Bug("Stampa Scheda Colturale BIO",
                     "Non veniva mostrato il codice operatore bio del centro aziendale dell'azienda",
                        idPerforma:=30430, cliente:="Terremerse")


            Riga_Text("Excel Piano Colturale Per Catasto",
                      "-Aggiunta colonna ISTAT Codice comune, concatenazione di codice provincia e comune Istat
                            - Modificato tipologia varietale in lettere", noteTecniche:="tipologia varietale in lettere non contiene piu' la prima lettere del gruppo varietale,
                            ma partendo da Codifica_Varieta_OIPomodorodaIndustriaNordItalia trova il gruppo varietale e poi viene convertito a una lettera basata sul excel ""\\rubino2\DOCUMENTAZIONE\GIAS --- Clienti --- AINPO\materiale inviato da AINPO\7 ALL .7 Format Tracciabilità Mutti 2023.xls""")

            '---------------------------------------------------------------------------

            Riga_Data("17 Maggio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti nuovi enum stampe", "139")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Bug("SQL DataProvider ordinamento orderby",
                     "Fix ordinamento dei risultati di alcune query (come il widget delle colture)")

            Riga_Text("Miglioramento lettura imprese all'apertura di alcune pagine",
                      "Velocizzata la lettura delle imprese prendendo solo i campi necessari, introdotto metodo nuovo per la lettura")

            Riga_Text("Migliorata la parametrizzazione delle query",
                      "Velocizzata la procedura di sostituzione del testo necessaria per la parametrizzazione")

            '---------------------------------------------------------------------------

            Riga_Data("13 Maggio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti nuovi enum stampe", "139")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Bug("Riepilogo Semine/Trapianti",
                      "Sistemata valorizzazione colonna Sup.[Ha]",
                        cliente:="CONSERVE ITALIA",
                        idPerforma:=29569)

            '==================================

            Riga_Data("10 Maggio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti nuovi enum stampe", "139")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Bug("Obiettivo di produzione report",
                      "-Adeguamento report con date aggiornate, modificato logo")

            Riga_Bug("Ordini vivavi report",
                     "modificato logo")


            '==================================
            Riga_Data("08 Maggio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti nuovi enum stampe", "139")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Text("Tracciabilità degli appezzamenti",
                      "Aggiunta settimana trapianto, tipologia variatale in lettere e tecnico rif. al excel che si ottiene dalla stampa ""Piano Colturale per Catasto"" Menu stampe bootstrap")

            '==================================

            Riga_Data("03 Maggio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti nuovi enum stampe", "139")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Text("Orogel stampe nuove",
                      "Effettuate ulteriori correzioni e precisazioni sulle stampe nuove per Orogel ( Caricamento codice B.P., numero telefono socio...)")

            '==================================

            Riga_Data("26 Aprile 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti nuovi enum stampe", "139")

            Riga_Requisiti("CORE API",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("CORE WS",
                           "Per servizio IsAlive", "23/04/2024")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Text("Stampe di Campagna",
                      "Ottimizzazione tempi di stampa",
                        noteTest:="Provare le varie stampe di campagna (Registro Aziendale Unico, Scheda di Campagna,ecc) e verificare che funzionino correttamente")

            Riga_Text("machinekey.config",
                      "aggiunto il richiamo sul web.config")

            Riga_Text("Parametrizzatore DataProvider",
                      "- Aggiunto nuovo parametrizzatore come default per velocizzare l'esecuzione delle query" &
                      "- Corretto caso che mandava in errore la procedura quando veniva utilizzata una funzione aggregata come criterio di order by")

            Riga_Text("Nuova stampa",
                      "- Nuova stampa Obiettivo di Produzione", cliente:="ASIPO", noteTest:="Visibile solo su asipo, si lancia da menu stampe bootstrap, filtra su azienda")

            Riga_Bug("Stampe",
                     "Nell'intestazione delle pagine del sito veniva mostrata sempre la ragione sociale della prima azienda scelta",
                     noteTest:="Segnalazione emersa da test interni. Provare una pagina di filtro di una stampa, tornare sul sito dell'agenda e cambiare azienda, rilanciando la pagina di filtro l'azienda deve aggiornarsi (il bug si verificava anche facendo logout e login su un altro ambiente)" &
                     "rilanciando la pagina di filtro verificare che nella pagina di filtro l'azienda venga correttamente aggiornata",
                     noteTecniche:="[Task TFS 7958] Il bug era dovuto al non aggiornamento di questa informazione nell'oggetto di sessione 'ParametriAgenda', " &
                     "ora questo viene re-inizializzato quando viene chiamata la pagina 'GestioneRichieste' del sito")

            Riga_Text("LOG APPLICATIVI",
                      "Tutti le chiamate alla Scrivi_LOG utilizzano la versione con objParametri",
                      noteTest:="Non testabile da assistenza")

            Riga_Bug("Stampa Registri Cantine Brogliaccio Semplificato",
                      "Correzione lotto mostrato due volte",
                      cliente:="Tenuta Casali", idPerforma:=29711,
                      noteTecniche:="Si verificava per prodotti legati alla tabella linee_preparazioni la cui colonna tipo_integrazione è > 0, generalmente usati nelle riclassificazioni")

            Riga_Text("Servizio IsAlive",
                      "Aggiunto ws per verificare raggiungibilità sito")

            '==================================

            Riga_Data("23 Aprile 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti gli enum delle stampe orogel a enumStampe2010", "136")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Text("Menu Stampe",
                      "Effettuate le aggiunte di dati e correzioni per le nuove stampe per Orogel.")

            '==================================

            Riga_Data("12 Aprile 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti gli enum delle stampe orogel a enumStampe2010", "136")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Text("Menu Stampe",
                      "Effettuate le correzioni per problematiche segnalate per le nuove stampe per Orogel.")

            '==================================

            Riga_Data("10 Aprile 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti gli enum delle stampe orogel a enumStampe2010", "136")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Text("Menu Stampe",
                      "Introdotte tutte le nuove stampe per Orogel e aggiornate le precedenti perché rispettino correttamente i filtri.")

            Riga_Bug("Scheda Aziendale e Autocertificazione",
                     "Corretto il caricamento particelle quando si selezionano degli impianti come filtro in stampa Scheda Aziendale e stampa Autocertificazione.")

            '==================================

            Riga_Data("08 Aprile 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti gli enum delle stampe orogel a enumStampe2010", "136")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Bug("Stampe di Campagna - Semina",
                     "Corretta la colonna 'Sup. [HA]' con la superficie seminata dell'operazione",
                     cliente:="Orogel")

            '==================================

            Riga_Data("05 Aprile 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti gli enum delle stampe orogel a enumStampe2010", "136")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Bug("Stampe Conferimento Pomodoro PDF e XLS",
                     "Correzione errore a causa del quale i report apparivano vuoti",
                     "Asipo", 0,
                     "Problema in join con contatto azienda gias, rag_soc del vettore a null e val_cod dei param_qual numerici del pomodoro a stringa vuota",
                     "Segnalazione emersa dai test interni")

            '==================================

            Riga_Data("04 Aprile 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti gli enum delle stampe orogel a enumStampe2010", "136")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Bug("Menu Stampe",
                     "Rimosse opzioni non previste nel menu stampe Importazioni/Esportazioni OP.")

            Riga_Bug("Gestione Stampe, Autocertificazione",
                     "Corretto il caricamento della data nella stampa Autocertificazione/ImpegnoProduzioneSociDivisoxCentri.")

            Riga_Bug("Stampa Scheda Colturale",
                     "Fix lettura campi Anno Imp.(1° Veget.), Data inizio Fioritura/Data Fioritura e Data Semina --> (dati salvati su esercizio).
                     Ora recupera correttamente il dato salvato sull'esercizio attivo nell'intervallo temporale della stampa. ",
                     "Asipo", 29415)

            '==================================

            Riga_Data("26 Marzo 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "761")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti gli enum stampe a enumStampe2010", "139")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("Rilascio modifiche UMA schedulate per il 23/02/2024",
                     "Riepilogo interventi: " &
                     "- Gestione rimanenze (modificata stampa richiesta, verbale richiesta e verbale rendicontazione)" &
                     "- Corretta lettura fascicolo azienda conto proprio in rendicontazione terzisti",
                     "Regione Umbria",
                     noteTest:="Non testare")

            Riga_Bug("Aggiunto Invio Mail di Log per Segnalazioni Speciali",
                     "Aggiunto invio mail nel caso di Piva = stringa vuota per la funzione AgronicaCoreUtility.CaricaListControl.TutteSpecieColtivate_3_Data_Da_A()",
                     "Regione Umbria",
                     noteTest:="Non testare")

            Riga_Text("Menu stampe bootstrap",
                            "Collegate stampe ImpegnativaColtivazioneConferimento e QuestionarioValutazioneAzienda_Aggiornamento")

            Riga_Bug("Restyle Grafico",
                     "- Veniva erroneamente letta la chiave di configurazione del sito Agenda anziché quella delle Stampe, quindi le pagine si vedevano non volutamente con la grafica nuova")

            '==================================

            Riga_Data("12 Marzo 2024")

            Riga_Requisiti("Migra",
                           "Per aggiunta tabella plateau e colonna plateau su programmazione_entita", "759")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti gli enum delle stampe orogel a enumStampe2010", "136")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("Ordini vivaio stampa",
                       "- Aggiunto campo plateau su stampa ordini vivaio", cliente:="ASIPO")

            Riga_Text("Stampe orogel",
                            " - le stampe orogel sono accessibili direttamente dal menu stampe bootstrap, modifiche per permettere l'apertura diretta")

            Riga_Text("Stampe orogel",
                            "Introdotte le nuove stampe per orogel (Accordo Responsabilita Filiera, Adesione Conad, Adesione Despar, Autocertificazione, Fitoregolatori Kiwi, Modulo Grasp, Nurture Module, Protocollo GLOBALGAP, Scheda Aziendale, Standard Leaf).")

            '==================================

            Riga_Data("29 Febbraio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "754")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti gli enum delle stampe op  a enumStampe2010", "135")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("Modifiche UI Regione Umbria",
                      "- Loghi header/footer, title, favicon " &
                      "- Modifica footer report ")

            '==================================

            Riga_Data("26 Febbraio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "754")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti gli enum delle stampe op  a enumStampe2010", "135")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Stampa Scheda di Campagna",
                     "Ulteriore fix sulla stampa 'Scheda interventi agronomici' se si vuole visualizzare la sezione 'Rilievi Avversità in campo' anche se non ci sono operazioni di questo tipo registrate",
                     cliente:="Orogel", idPerforma:=28527)

            '==================================

            Riga_Data("22 Febbraio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "754")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti gli enum delle stampe op  a enumStampe2010", "135")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Stampa DDT",
                     "Fix stampa DDT in caso di persone fisiche con codice fiscale non fittizio")

            Riga_Bug("Stampa Scheda di Campagna",
                     "Corretta indicazione della Specie Vegetale che si va a stampare nel filtro pre-stampa",
                        noteTecniche:="Se si seleziona una Specie Vegetale nel filtro della Specie delle Operazioni Colturali, questa viene passata anche quando si fa una stampa e viene indicata nella dicitura 'In particolare si è scelto di stampare i dati sulla specie vegetale' nella pagina di filtro pre-stampa." &
                        "Se invece nel filtro della Specie delle Operazioni Colturali viene scelto 'Tutte le Specie', nella pagina di filtro pre-stampa, se si è selezionata una stampa 'multispecie' (esempio Registro Aziendale Unico) la diciatura sarà 'In particolare si è scelto di stampare i dati sulla specie vegetale...' e la stampa avverrà su tutte le Specie." &
                        "Se invece la stampa è 'monospecie' (esempio Global Gap), nella pagina di filtro pre-stampa apparirà il menù a tendina in cui scegliere la Specie da stampare.")

            '==================================

            Riga_Data("14 Febbraio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "754")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunti gli enum delle stampe op  a enumStampe2010", "135")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Stampa Scheda di Campagna",
                     "Fix stampa 'Scheda interventi agronomici'",
                     cliente:="Orogel", idPerforma:=28527)

            Riga_Text("stampe op",
                            " - le stampe op sono accessibili direttamente dal menu stampe bootstrap, modifiche per permettere l'apertura diretta")

            '==================================

            Riga_Data("01 Febbraio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "754")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunte stampe", "131")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Stampa DDT",
                     "Fix sovrascrittura della dicitura Art. 3 (ex Articolo 62) usando l'impostazione superuser che non veniva sentita per nome parametro sbagliato",
                     cliente:="Inalca")

            Riga_Bug("Stampa Estratto Conto Bolle Conferimento",
                     "Correzione dati mostrati in colonna 'punteggio'",
                     "Fruttagel")

            Riga_Text("Stampa Etichetta Conf",
                     "Aggiunta personalizzazione per Suba Sementi")

            '==================================

            Riga_Data("30 Gennaio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "754")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunte stampe", "131")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Esportazione Impianti",
                     "Fix file Excel vuoto durante l'Esportazione degli Impianti",
                     cliente:="Greenyard", idPerforma:=28455)

            '==================================

            Riga_Data("29 Gennaio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "754")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunte stampe", "131")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Stampa Estratto Conto Bolle Conferimento",
                     "Correzione errore per il quale il report non veniva stampato, in caso la colonna punteggio conteneva numeri decimali",
                     "Fruttagel")

            Riga_Text("Stampe Doc Contabili",
                     "Aggiunta impostazione che se attiva, mostra nei report il riferimento alla norma Art. 3 D.Lgs. 08/11/2021, n. 198 (ex Art. 62 D.L. 24/01/2012, n. 1) " &
                     "se il documento contiene prodotti della categoria 'altre risorse' (senza impostazione viene mostrato per altre categorie di prodotto)",
                     "Inalca", noteTecniche:="Impostazione superuser-impresa [1092] 'StampaArticolo62ElemCod' valori 0 (default) / 1")

            Riga_Bug("Stampe Doc Contabili",
                     "Corretto il riferimento alla norma Art. 3 D.Lgs. 08/11/2021, n. 198 nel layout di stampa specifico per le imprese con attivo il modulo F&F " &
                     "perché veniva ancora mostrato il riferimento alla vecchia norma (Art. 62 D.L. 24/01/2012, n. 1)",
                     "Lorenzini")

            '==================================

            Riga_Data("18 Gennaio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "754")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunte stampe", "131")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Richiesta Sementi",
                       " - fix sul report piano colturale preventivo", cliente:="ASIPO")

            '==================================

            Riga_Data("11 Gennaio 2024")

            Riga_Requisiti("Migra",
                           "Nuove colonne tabelle UMA", "754")

            Riga_Requisiti("Aggancio",
                           "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunte stampe", "131")

            Riga_Requisiti("CORE WS",
                           "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                           "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("Rilascio modifiche UMA schedulate per il 15/12/2023",
                      "Riepilogo interventi: " &
                      "- Gestione validità configurazione allevamenti" &
                      "- Gestione litri già decurtati" &
                      "- Gestione biologico",
                      "Regione Umbria")

            Riga_Bug("Stampe aventi sezione 'Patentini'",
                     "Fix caricamento griglia Patentino per i contatti che hanno codice fiscale salvato erroneamente con spazi/tab iniziali/finali", "Coldiretti", 28194)

            '==================================

            Riga_Data("20 Dicembre 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Aggiunte stampe", "131")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("Selezione Scheda Campagna",
                "Sviluppata nuova pagina bootstrap")

            Riga_Text("Report ordini vivaio",
                        "Aggiunto report ordini basato su questo ""\\rubino2\DOCUMENTAZIONE\GIAS --- Clienti --- ASIPO\Materiale ricevuto da ASIPO x config e popolamento\Pianificazione seme\Agronica\Piano colt. preventivo_casoli giovanni.pdf""",
                        cliente:="ASIPO", noteTest:="Per essere richiamata bisogna passare da Gestione Richieste/Ordini in Materiale Vivaistico, selezionare dei ordini e cliccare il pulsante stampe, si puo' fare solo se PersonalizzazioniAsipo e AssociaBudget sono true nel json su Configurazione_Siti chiave Piante2020.")

            Riga_Text("Report Piano Preventivo Colturale WIP",
                            "WIP", cliente:="ASIPO")

            '==================================

            Riga_Data("15 Dicembre 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Stampe Trattamenti Post raccolta",
                      "Fix valorizzazione della Quantità Totale Distribuita quando ci sono più prodotti fitosanitari utilizzati nello stesso Trattamento")

            Riga_Text("Stampe Conferimenti",
                "Completato porting report 'Estratto Conto Bolle Conferimento'",
                "Fruttagel")

            '==================================

            Riga_Data("06 Dicembre 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Stampe Trattamento Post Raccolta",
                      "Fix valorizzazione in stampa del quantitativo di prodotto utilizzato",
                      "Coldiretti Marche",
                      27784)

            Riga_Bug("Errore stampe QDC", "Inserito messaggio di avviso quando un utente tenta di entrare
                      nella sezione stampe dal QDC con un'azienda che non ha impianti (in precedenza restituiva un errore server non chiaro)")

            '==================================

            Riga_Data("22 Novembre 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Stampa Scheda di campagna multicentro",
                      "Fix dovuto a nuovo struttura dataset visite",
                      "Orogel",
                      0,
                      "",
                      "Testare anche il lancio delle altre schede di campagna")

            '==================================

            Riga_Data("20 Novembre 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("Stampa Visite",
                      "Aggiunta stampa nuove visite",
                      "Orogel",
                      0,
                      "",
                      "Testare anche il lancio delle altre schede di campagna")

            '==================================

            Riga_Data("10 Novembre 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("Stampa DDT Emesso",
                      "Aggiunta Impostazione Superuser SUPERUSER_INTESTAZIONE_STAMPA_CENTROAZIENDALEPARTENZA = 877 per stampare l'indirizzo del centro di partenza (e non più la sede operativa) ",
                      "INALCA",
                      0,
                      "",
                      "Fare prove su azienda con più centri, attivare prima l'impostazione su quel DB a mano (da sql) ")

            '==================================

            Riga_Data("31 Ottobre 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("WebConfig",
                      "Aggiunti file separati sessionState e httpCookies", "coldiretti", 0, "", "Nessun Test")

            '==================================

            Riga_Data("26 Ottobre 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Stampa Tracciabilità Vegetale",
                     "Correzione lettura dati campionatura in parametri qualitativi",
                     "Gentili Ercolino", 26799)

            '==================================

            Riga_Data("19 Ottobre 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("Stampe Contabili Fattura",
                     "Aggiornato riferimento a normativa Art. 62 D.L. 24/01/2012, n. 1 abrogata a favore di Art. 3 D.Lgs. 08/11/2021, n. 198",
                     "Lorenzini", 26756)

            Riga_Bug("Stampa Fasi Fenologiche con Multi Specie (Registro Aziendale)",
                     "Fix stampa Fasi Fenologiche con Multi Specie",
                     "Coldiretti Piemonte", 26769,
                     noteTecniche:="Spostata lettura fasi da webservice a metaschema locale")

            '==================================

            Riga_Data("13 Ottobre 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("Stampe generali - unità misura numero",
                "Per le unità di misura intere (ex. 'n. diffusori', 'n. unità') la quantità viene mostrata senza decimali per ottimizzare lo spazio.")

            Riga_Bug("Stampa Scheda Campagna - Dati Catastali",
                     "La stampa mostra ora correttamente solo i dati catastali degli appezzamenti ( o relativi campi, in assenza di particelle su impianto/appezzamento) selezionati.",
                     "Eurovo", 25957,
                     noteTest:="Stampa dati catastali, riepilogo funzionamento:
                     - se esistono particelle su app --> viene mostrata la particella dell'appezzamento
                     - se esistono particelle su campo associato ad appezzamento --> viene mostrata la particella del campo
                     - se esistono particelle sia su app che su campo --> mostro SOLO le particelle dell'APPEZZAMENTO")

            '==================================


            Riga_Data("10 Ottobre 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("Correzioni report 'Lettera con Elenco Conferimenti':",
                "- Aggiornata firma specifica di Fruttagel" &
                "- Corretti margini stampa impostando 4cm in alto e 3 in basso",
                "Fruttagel")

            '==================================

            Riga_Data("29 Settembre 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Stampa Scheda Campagna",
                     "Per le unità di misura 'n. diffusori' e 'n. unità' viene stampato il simbolo 'n.' per essere visualizzato correttamente nello spazio disponibile. ")

            Riga_Bug("Stampa Massiva Certificati Pomodoro",
                     "Correzione errore che impediva la stampa impostando anche il filtro sul primo cessionario", "Fruttagel", 26363)

            Riga_Text("Restyle Grafico",
                     "- Ri-organizzazione file css in multipli file separati <code>styleGiasComponents.css, styleGiasIcons.css, styleGiasPages.css, styleGiasUtils.css</code> anziché l'unico file <code>styleXonneTables.css</code>")

            '==================================

            Riga_Data("20 Settembre 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Stampa Conferimento Pomodoro",
                     "Correzione indicazione unità di misura per i pesi e le tare imballi e veicolo, perché sono espresse in quintali ma le etichette indicavano chilogrammi", "Fruttagel", 26106)

            '==================================

            Riga_Data("05 Settembre 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("Stampa Scheda Colturale / Multicentro / BIO",
                "Aggiunta gestione stampa delle operazioni di DISTRIBUZIONE INSETTI e CONFUSIONE DISORIENTAMENTO SESSUALE",)

            '==================================

            Riga_Data("24 Agosto 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("Correzioni report 'Lettera con Elenco Conferimenti':",
                "- corretti filtri di lancio per i casi in cui non vengono selezionati prodotti e conferenti" &
                "- corretta quantità visualizzata, veniva mostrata la quantità netta al posto del peso netto a pagamento" &
                "- aggiunta ragione sociale del produttore se presente nei singoli conferimenti" &
                "- generalizzato report per gli altri clienti",
                "Fruttagel")

            '==================================

            Riga_Data("02 Agosto 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("Stampe Conferimenti",
                "Aggiunto nuovo report 'Lettera con Elenco Conferimenti'",
                "Fruttagel")

            Riga_Bug("Stampe Conferimenti:",
                "- Correzione in report excel dei trasportatori per calcolo del costo totale se il listino corrispondente ha l'unità di misura numero",
                "Fruttagel")

            '==================================

            Riga_Data("19 Luglio 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Stampa DDT",
                 "- Non veniva più correttamente nascosta la label ""Litri"" che si andava a sovrapporre a ""Peso lordo""",
                 "Inalca")

            '==================================

            Riga_Data("07 Luglio 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Report Brogliaccio Movimenti",
                 "Correzione per mostrare i soli movimenti/prodotti legati al modulo cantine",
                 "Fiorentina di Rensi", 23823)

            Riga_Text("Stampa di Campagna - Distribuzione Insetti",
                      "Aggiunta gestione nuova modalità distribuzione insetti NG (un'avversità per ogni prodotto)")

            Riga_Text("Stampa Piano Colturale Catasto",
                     "Aggiunte colonne 'Cod. Industria OI' e 'Des. Industria OI'",
                     "AINPO", 24856)

            '==================================

            Riga_Data("29 Giugno 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Report Trasportatori Excel",
                     "Fix Calcolo costo trasporto per validita listino ",
                     "Fruttagel")

            '==================================

            Riga_Data("23 Giugno 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("Stampa Bilancio Fertilizzazioni",
                     "NPK MG: se il valore MASSIMO non è stato impostato da anagrafica, comparirà la scritta 'Non Definito'")

            Riga_Text("Report Excel Trasportatori ",
                     "calcolo costo trasporto in base al trasportatore distanza",
                      "Fruttagel")

            '==================================

            Riga_Data("09 Giugno 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Stampa GlobalGap Trattamenti",
                     "Fix stampa Trattamenti con operatori aventi due patentini registrati (con date correttamente non accavallate): ora viene mostrata una sola riga per operatore",
                     "TERREMERSE", 24194)


            Riga_Bug("Stampa Bio Semine",
                     "Fix stampa Bio di semine aventi n volte lo stesso prodotto con lotti diversi",
                     "TERREMERSE", 24397)

            '==================================

            Riga_Data("12 Maggio 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                           "Per chiave LinkGiasBase", "119")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("GiasBase", "Eliminati tutti i riferimenti fissi al GiasBase (/GiasBase/... etc nel codice - lettura parametrizzata tramite chiave in configurazione siti")

            '==================================

            Riga_Data("08 Maggio 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per porting report xls riepilogo semine di Conserve Italia", "118")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Report Biologico - Carichi/Scarichi",
                     "fix mancata visualizzazione colonne 'progetto' e 'matricola' in caso di imputazione costi zootecnici.",
                     "Aboca", 23545)

            '==================================


            Riga_Data("28 Aprile 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per porting report xls riepilogo semine di Conserve Italia", "118")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Bilancio Fertilizzazioni - Controlli Macroelementi_Distribuiti (NPK Mg)",
                     "fix lettura e confronto raccoglitore_cod quando si vanno a leggere i Macroelementi_Distribuiti nelle operazioni",
                     "Genagricola", 23692)

            '==================================

            Riga_Data("18 Aprile 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per porting report xls riepilogo semine di Conserve Italia", "118")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Report XLS Riepilogo Semine Trapianti",
                     "Correzioni al report a seguito dei test.",
                     "Conserve Italia")

            '==================================

            Riga_Data("31 Marzo 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per porting report xls riepilogo semine di Conserve Italia", "118")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report xls riepilogo semine di Conserve Italia", "30/03/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Bug("Report UMA Richieste",
                     "Corretti litri richiesti in caso di richiesta tornata in compilazione proveniente da 'Verifica Intermedia Completata con Riserva'",
                     "P0000000009 - Coldiretti Umbria", 23294)

            Riga_Text("Report XLS Riepilogo Semine",
                     "Porting del report personalizzato da stampe 2003. Il report ora passa dal filtro di ricerca nuovo e permette di selezionare gli impianti dei quali esportare semine e trapianti",
                     "Conserve Italia")

            '==================================

            Riga_Data("17 Marzo 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per porting report impegno produzione soci", "115")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report impegno produzione soci", "27/01/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("Report Analisi Costi e Ricavi",
                        "Aggiunti subtotali anche qualora non si deselezioni 'dettaglio giornaliero'", idPerforma:=22918)

            Riga_Bug("Operazioni di Abbattimento nella stampa Scheda BIO ",
                     "Aggiunte operazioni di abbattimento e defogliazione alle operazioni stampate nella Scheda BIO (selezionando la sezione 'Altre Operazioni Colturali')",
                     "CAB Terra", 23006)

            '==================================


            Riga_Data("06 Marzo 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per porting report impegno produzione soci", "115")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report impegno produzione soci", "27/01/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "06/03/2023")

            Riga_Text("Dashboard ",
                     "- Adeguamento master page per nuova grafica + header")

            '==================================

            Riga_Data("08 Febbraio 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per porting report impegno produzione soci", "115")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report impegno produzione soci", "27/01/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Stampa Registro Aziendale Unico",
                     "fix dicitura 'Colturale'", "Coldiretti Toscana", 22308)

            '==================================

            Riga_Data("07 Febbraio 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per porting report impegno produzione soci", "115")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report impegno produzione soci", "27/01/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Stampe UMA",
                     "Ripristinato il funzionamento delle stampe UMA")

            '==================================

            Riga_Data("27 Gennaio 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per porting report impegno produzione soci", "115")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per report impegno produzione soci", "27/01/2023")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Stampe Registro Trattamenti e Fertilizzazioni",
                     "Correzione errore in generazione delle stampe che ne impediva l'utilizzo")

            Riga_Bug("Stampa  Registro Trattamenti",
                     "Fix lettura centri aziendali", "Tenuta Barbon", 21416)

            Riga_Bug("Stampa Registro Aziendale Unico",
                     "fix dicitura 'Colturale'", "Coldiretti Toscana", 22308)

            Riga_Text("Stampa Impegno Produzione Soci",
                     "- Porting del report e dalla pagina di filtro da sito Stampe precedente",
                     cliente:="AgriBologna", idPerforma:=22241)

            '==================================

            Riga_Data("18 Gennaio 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Bug("Stampe Registro Trattamenti e Fertilizzazioni",
                     "Correzione errore in generazione delle stampe che ne impediva l'utilizzo")

            '==================================

            Riga_Data("17 Gennaio 2023")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("Stampa Doc Vendita",
                     "- Impostata la stampa del lotto sempre di default (anche per trasformati vegetali) " &
                     "a meno che non ci si trova in modulo cantine oppure è proprio stata spenta la configurazione " &
                     "di stampa del lotto per quel prodotto (che viene salvata in Materie_PrimexLotto_Configurazione)",
                     cliente:="Sottali")

            '==================================

            Riga_Data("22 Dicembre 2022")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("GIAS BASE",
                           "Nuova versione 2022", "22/12/2022")

            Riga_Text("Stampa BIO Materie Prime",
                     "- Gestione dei movimenti di reso dei prodotti: mostrato tipo documento prima del numero così come per le altre categorie di documento",
                     cliente:="Agrites", idPerforma:=20070)

            '==================================

            Riga_Data("16 Dicembre 2022")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Bug("Stampe GlobalGAP multicentro",
                     "- Corretta visualizzazione stampa se si selezionano impianti specifici",
                     cliente:="Agribologna", idPerforma:=21918)

            '==================================

            Riga_Data("13 Dicembre 2022")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("Stampe Doc Contabili",
                     "- Sistemazioni per visualizzare correttamente indirizzi esteri con Gestione Geografica di provincie e comuni",
                     cliente:="ENI - Kenia")

            Riga_Text("Stampa BIO Materie Prime",
                     "- Gestione dei movimenti di reso dei prodotti",
                     cliente:="Agrites", idPerforma:=20070)

            Riga_Text("Stampe GlobalGAP multicentro, Scheda di campagna multicentro e Registro aziendale unico",
                     "- Aggiunta possibilità di stampare la sezione 'rilievi avversità' senza le colonne 'numero avversità rilevate' e 'unità di misura'" &
                     "- Cambiata dicitura relativa ai prodotti in miscela da 'stessa data, stessa botte' a 'stessa data, stesso mezzo tecnico'",
                     cliente:="Agrosistemi")

            Riga_Bug("Stampa Registro aziendale unico",
                     "- Corretta gestione per nascondere il logo della regione dalla prima pagina",
                     cliente:="Agrosistemi", idPerforma:=19766)

            '==================================

            Riga_Data("23 Novembre 2022")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Bug("Stampa schede impegnative",
                     "Fix visualizzazione logo",
                     "Orogel")

            '==================================

            Riga_Data("14 Novembre 2022")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("Conferimenti Export Excel Trasportatori",
                     "- Lettura nominativo trasportatore anche da campi cognome-nome oltre a ragione sociale",
                     "Fruttagel")

            '==================================

            Riga_Data("04 Novembre 2022")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("Conferimenti Export Excel Trasportatori",
                     "- Aggiunte colonne partita iva e ragione sociale per prima e seconda cooperativa e colonna netto a pagamento",
                     "Fruttagel", 19916)

            Riga_Text("Report costi CdG",
                     "- Modifica arrotondamento quantità")

            Riga_Text("Stampe Documenti Contabili",
                     "- Aggiunta stampa codice SDI anche nei report di acquisto, condizionata da nuova impostazione",
                     cliente:="Iniziative Biometano",
                     noteTest:="Il codice deve essere salvato nella rubrica del centro aziendale utilizzato nel documento come campo 'social'",
                     noteTecniche:="La nuova impostazione utente, impresa StampaDocAcquisto_CodiceSDI [1069] deve essere attivata su database")

            '==================================

            Riga_Data("25 Ottobre 2022")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Bug("Report Biologico",
                     "- Bugfix raddoppio movimenti",
                     "Aboca", 20952)

            '==================================

            Riga_Data("21 Ottobre 2022")

            Riga_Requisiti("Migra",
                       "Modifiche gestione lotto prodotto", "700")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("Gestione Lotto Prodotto",
                     "- Rimozione scrittura lotto 'indefinito', viene ora scritto lotto vuoto ('')" &
                     "- Differenziazione in ricerche fra lotto vuoto ('') e non filtrare per lotto")

            '==================================

            Riga_Data("05 Ottobre 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Bug("Estrazione catasto affitti",
                     "Fix filtro cau_mov come tipo dato stringa")

            '==================================

            Riga_Data("23 Settembre 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura fabbricati", "23/09/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("leggiTabelle_ws_client.js: ",
                     "Allineamento con versione Agenda necessario per lettura fabbricati")

            '==================================

            Riga_Data("19 Settembre 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura contatti per documenti contabili", "10/06/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Bug("Esporta Gias To SAP (Conserve Italia)",
                     "- cambiata label 'GiasToSap'")

            '==================================

            Riga_Data("01 Settembre 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura contatti per documenti contabili", "10/06/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("GIAS BASE",
                           "Modifiche nuova versione 2022", "01/09/2022")

            Riga_Text("Sito completo",
                      "- Modifiche per gestire nuove versioni di kendo 2022.x")

            Riga_Bug("Intera soluzione",
                     "fix: esistevano due costanti con lo stesso valore (LAV_COD 162), unificate usando solo LAVCOD_ALTRE_OPERAZIONI ")

            Riga_Bug("Stampe documenti contabili",
                     "- Risolta chiamata 19494: ora vengono visualizzati i dati dell'impresa anche se era stata creata da una azienda diversa dal superuser" &
                     "- Risolta chiamata 18962: per le stampe di acquisto, nel riquadro di destinazione, mostrati indirizzi e rubrica del centro aziendale scelto nel documento al posto di quelli dell'impresa" &
                     "- Bolla ricevuta: nascosta visualizzazione nel riquadro del fornitore dell'indirizzo di destinazione, ripetuto erroneamente in caso di mancanza di un secondo soggetto cedente")

            Riga_Bug("Stampa documento accettazione cantine",
                     "ripristinata solo in questo caso la stampa nella sezione note del solo <code>Mov_Desc</code> (in cui è riportata dicitura SQNPI) [chiamata 19688 | Borgoluce]")

            Riga_Bug("Stampa Scheda Colturale Bio",
                     "sistemata la visualizzazione della Quantità 0 quando non c'è un numero di Trappole indicato [chiamata 19688 | Terremerse]")

            '==================================

            Riga_Data("18 Agosto 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura contatti per documenti contabili", "10/06/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Text("Scheda di Campagna",
                      "Fix stampa irrigazione")

            '==================================

            Riga_Data("12 Agosto 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura contatti per documenti contabili", "10/06/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Text("Agenda2010",
                      "Gestione nuovo lav_cod Defogliazione (171) in varie parti del codice e aggiunta uso costanti lav_cod dove mancavano")

            '==================================


            Riga_Data("03 Agosto 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura contatti per documenti contabili", "10/06/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Bug("Scheda Campagna",
                     "- Bugfix visibilità contatti [Genagricola 19164]")

            '==================================

            Riga_Data("22 Luglio 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura contatti per documenti contabili", "10/06/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Text("Esporta_GiasToSap", "Portata stampa su stampe 2010")

            '==================================

            Riga_Data("19 Luglio 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura contatti per documenti contabili", "10/06/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Bug("Stampe Conferimenti e Bolle Ricevute/Emesse: ",
                     "- Correzione perché non venivano più mostrate le note del documento nel report")

            Riga_Bug("Stampa Conferimento Pomodoro: ",
                     "- Correzione errore nella stampa in caso il conferimento abbia delle percentuali sui difetti del prodotto pari a zero")

            '==================================

            Riga_Data("01 Luglio 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura contatti per documenti contabili", "10/06/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Text("Report Statistici Doc Contabili: ",
                     "Verrano ora estratti solo i documenti relativi ai prodotti accessibili all'utente in base alla visibilità dei gruppi merce")

            Riga_Bug("Risolta chiamata nr.18611 necessario aggiornamento Fosforo: ",
                     "La Stampa della Fattura senza riepilogo imballaggi non riporta più la riga di riferimento a ddt se la sezione è vuota")

            '==================================

            Riga_Data("22 Giugno 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura contatti per documenti contabili", "10/06/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Bug("SchedaVendite BIO",
                     "Cessionario diverso non veniva mostrato se il cessionario principale è impresa Gias (nuovo sviluppo chiamata 18198)")

            '==================================

            Riga_Data("17 Giugno 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura contatti per documenti contabili", "10/06/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Bug("SchedaCampagna.aspx",
                     " CaricaDsConcimazioni: bugfix lettura epoca nelle fertilizzazioni registrate con disciplinare BIO (chiamata 18521)")

            Riga_Text("SchedaVendite BIO",
                    "aggiunta indicazione cessionario diverso se presente (nuovo sviluppo chiamata 18198)")

            '==================================

            Riga_Data("10 Giugno 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura contatti per documenti contabili", "10/06/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Bug("Stampa documenti contabili",
                     "- Nella scrittura del log, forzato troncamento ragione sociale a 40 caratteri nel nome del file per evitare errore di percorso troppo lungo")

            Riga_Text("leggiTabelle_ws_client.js: ",
                     "Allineamento con versione Agenda necessario per lettura contatti per documenti contabili")

            '==================================

            Riga_Data("27 Maggio 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuova chiave CompressioneRispostaAjax utilizzata per abilitare compressione risposta standard", "102")

            Riga_Requisiti("CORE WS",
                       "Per lettura prodotti per documenti contabili", "28/03/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Text("RispostaStandard",
                     "- gestione compressione rispostastandard  lato server / decompressione lato client JS")

            '==================================

            Riga_Data("20 Maggio 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per lettura prodotti per documenti contabili", "28/03/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Bug("Stampa DDT:",
                      "Modifica per stampare imballaggi, contenitori e confezioni quando non sono collegati a riga di prodotto")

            '==================================

            Riga_Data("13 Maggio 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per lettura prodotti per documenti contabili", "28/03/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Stampe Campagna:",
                      "testo Regolamento BIO nelle stampe sostituito da una costante per facilitarne future modifiche")

            '==================================

            Riga_Data("09 Maggio 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per lettura prodotti per documenti contabili", "28/03/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Bug("Stampe Campagna:",
                      "fix Regolamento BIO in Scheda Colturale BIO")

            '==================================

            Riga_Data("06 Maggio 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per lettura prodotti per documenti contabili", "28/03/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Bug("Stampe Campagna:",
                      "fix Regolamento BIO in varie stampe")

            '==================================

            Riga_Data("02 Maggio 2022")

            Riga_Requisiti("Migra",
                       "Per Modifiche Vista GiasMVV", "683")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per lettura prodotti per documenti contabili", "28/03/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Bug("ShedaCampagna.aspx (Risolta chiamata nr 17781 - necessario aggiornamento GranfruttaZani):",
                      "Sistemata la visualizzazione delle Avversità nella stampa Scheda di Campagna sezione Trattamenti per le Operazioni che utilizzano degli Insetti.")

            Riga_Text("Magazzini, Stampa la scheda movimenti di magazzino:",
                      "Aggiunta lettura sa_cod da G2G_recode_imprese per azienda Genagricola")

            Riga_Text("MVVE", "MVVE - Pacchetto di modifiche (Zona_Viticola, Lotto, UDM + Capacità imballo)")

            '==================================

            Riga_Data("22 Aprile 2022")

            Riga_Requisiti("Migra",
                       "Per colonna Tipo_Associazione in Mov_Dettagli_Riferimenti", "677")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per lettura prodotti per documenti contabili", "28/03/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("ShedaCampagna.aspx:",
                      "Modificato report scheda campagna (sezione Trattamenti) " &
                      "Aggiunta indicazione % riduzione superficie trattata per nuovo controllo verifica diserbo")

            '==================================

            Riga_Data("06 Aprile 2022")

            Riga_Requisiti("Migra",
                       "Per colonna Tipo_Associazione in Mov_Dettagli_Riferimenti", "677")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per lettura prodotti per documenti contabili", "28/03/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Stampa Bolla Emessa F&F:",
                      "Modificato layout della intestazione per allinearlo alle altre stampe, " &
                      "ora il logo è mostrato sempre sotto alla ragione sociale e cessionario/destinatario e destinazione diversa sono mostrati sulla destra")

            '==================================

            Riga_Data("04 Aprile 2022")

            Riga_Requisiti("Migra",
                       "Per colonna Tipo_Associazione in Mov_Dettagli_Riferimenti", "677")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per lettura prodotti per documenti contabili", "28/03/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Stampa Bolla Emessa:",
                      "Aggiunta gestione per recuperare dati del centro aziendale esterno collegato")

            Riga_Text("Ricerca elenco completo prodotti (singola categoria)",
                     "Allineamento chiamate per aggiunta parametri filtro codice prodotto e filtro codice trappola in pagina Prodotti.asmx")

            '==================================

            Riga_Data("28 Marzo 2022")

            Riga_Requisiti("Migra",
                       "Per colonna Tipo_Associazione in Mov_Dettagli_Riferimenti", "677")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per lettura prodotti per documenti contabili", "28/03/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Ricerca elenco completo prodotti (singola e multicategoria)",
                      "Allineamento chiamate per aggiunta parametro diversificazione descrizione fertilizzanti in pagina Prodotti.asmx")

            '==================================

            Riga_Data("17 Marzo 2022")

            Riga_Requisiti("Migra",
                       "Per colonna Tipo_Associazione in Mov_Dettagli_Riferimenti", "677")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per lettura contatti per documenti contabili", "16/02/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Bug("Risolta chiamata nr.17123 di Cantina Sandrin: ",
                     "Fix estrazione duplicata delle operazioni di trasformazione nei report brogliaccio movimenti semplificato e completo eseguibili da GiasLan")

            '==================================

            Riga_Data("02 Marzo 2022")

            Riga_Requisiti("Migra",
                       "Per colonna Tipo_Associazione in Mov_Dettagli_Riferimenti", "677")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per lettura contatti per documenti contabili", "16/02/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Bug("Risolta chiamata nr.16435 di Tenuta Casali: ",
                     "Fix lettura movimenti di cantina di riclassificazione e di imbottigliamento legati ai report brogliaccio movimenti e registro imbottigliamenti eseguibili da GiasLan")

            '==================================

            Riga_Data("1 Marzo 2022")

            Riga_Requisiti("Migra",
                       "Per colonna Tipo_Associazione in Mov_Dettagli_Riferimenti", "677")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per lettura contatti per documenti contabili", "16/02/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Report CdG: ",
                     "Modificato per passaggio parametri nella lettura tabellone")

            '==================================

            Riga_Data("16 Febbraio 2022")

            Riga_Requisiti("Migra",
                       "Per colonna Tipo_Associazione in Mov_Dettagli_Riferimenti", "677")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per lettura contatti per documenti contabili", "16/02/2022")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("leggiTabelle_ws_client.js: ",
                     "Allineamento con versione Agenda necessario per lettura contatti per documenti contabili")

            '==================================

            Riga_Data("15 Febbraio 2022")

            Riga_Requisiti("Migra",
                       "Per importazione QdC AgriBologna", "665")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per importazione QdC AgriBologna", "13/10/2021")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Bug("Report Uma:",
                     "- Verbale Rendicontazione: azzeramento dei carburanti rendicontati ed ingiustificati in caso di esito negativo dell'istruttoria")

            '==================================

            Riga_Data("14 Febbraio 2022")

            Riga_Requisiti("Migra",
                       "Per importazione QdC AgriBologna", "665")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per importazione QdC AgriBologna", "13/10/2021")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Bug("Report Uma:",
                     "- Nel verbale di rendicontazione correzioni al calcolo del carburante non giustificato: la riduzione di carburante approvato data dall'ispettore " &
                     "non è più considerata come carburante ingiustificato se il carburante approvato è maggiore del carburante da rendicontare minimo; " &
                     "correzione nell'approssimazione dei decimali")

            '==================================

            Riga_Data("04 Febbraio 2022")

            Riga_Requisiti("Migra",
                       "Per importazione QdC AgriBologna", "665")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per importazione QdC AgriBologna", "13/10/2021")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Report Uma:",
                     "- Richiesta: tolta dicitura 'comunità montana', aggiunta dicitura sezione 'ditta attiva e in esercizio e aggiornato pannello firma" &
                     "- Verbale richiesta: tolta dicitura 'comunità montana'" &
                     "- Rendicontazione: aggiornato pannello firma" &
                     "- Verbale rendicontazione: tolta dicitura 'comunità montana', aggiornata dicitura per carburante rimasto in rend. ed aggiunto riepilogo per carburante non giustificato")

            '==================================

            Riga_Data("01 Febbraio 2022")

            Riga_Requisiti("Migra",
                       "Per importazione QdC AgriBologna", "665")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per importazione QdC AgriBologna", "13/10/2021")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Report Biologico:",
                     " Aggiunti dati ZOO , Classificazione Formulati")

            Riga_Text("Report Biologico:",
                     " Gestione selezione solo costi di Campagna o da CDG (MAGAZZINO / EREDITA)")

            Riga_Text("Report Biologico:",
                     " Gestione degli scarichi SAP su movimenti carico/Scarico - Ricalcolo delle qta ")


            '==================================

            Riga_Data("01 Febbraio 2022")

            Riga_Requisiti("Migra",
                       "Per importazione QdC AgriBologna", "665")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per importazione QdC AgriBologna", "13/10/2021")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Bug("Report Statistiche:",
                     "Correzione nel recupero delle note dei documenti di registrazione")

            '==================================

            Riga_Data("25 Gennaio 2022")

            Riga_Requisiti("Migra",
                       "Per importazione QdC AgriBologna", "665")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per importazione QdC AgriBologna", "13/10/2021")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Bug("Scheda Campagna (Risolta chiamata nr. 16127 necessario Aggiornamento Coldiretti):",
                     "Fix stampa Registro Aziendale Unico, sezione Irrigazione quando ci sono delle Specie con l'apostrofo.")

            '==================================

            Riga_Data("20 Gennaio 2022")

            Riga_Requisiti("Migra",
                       "Per importazione QdC AgriBologna", "665")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per importazione QdC AgriBologna", "13/10/2021")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Cache:", "Aggiunto asmx con metodo ClearCache chiamato da esterno")

            '==================================
            Riga_Data("12 Gennaio 2022")

            Riga_Requisiti("Migra",
                       "Per importazione QdC AgriBologna", "665")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per importazione QdC AgriBologna", "13/10/2021")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Report UMA",
                  "Rendicontazione: aggiunta ultima pagina 'dichiarazioni del compilatore' e miglioramenti minori")

            '==================================

            Riga_Data("09 Dicembre 2021")

            Riga_Requisiti("Migra",
                       "Per importazione QdC AgriBologna", "665")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per importazione QdC AgriBologna", "13/10/2021")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Bug("Scheda di campagna (Risolta chiamata performa nr. 15491 - Necessario Aggiornamento Genagricola):",
                  "Aggiunto filtro visibilità utenti sui Centri Aziendali nella stampe Scheda di Campagna.")

            '==================================

            Riga_Data("03 Dicembre 2021")

            Riga_Requisiti("Migra",
                       "Per importazione QdC AgriBologna", "665")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per importazione QdC AgriBologna", "13/10/2021")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Bug("Scheda di campagna (Risolta chiamata performa nr. 15007 - Necessario Aggiornamento Coldiretti):",
                  "Aggiunti decimali in base alla opzione di arrotondamento scelta nella sezione di stampa dei Trattamenti.")

            '==================================


            Riga_Data("24 Novembre 2021")

            Riga_Requisiti("Migra",
                       "Per importazione QdC AgriBologna", "665")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 export 'Centri'.", "92")

            Riga_Requisiti("CORE WS",
                       "Per importazione QdC AgriBologna", "13/10/2021")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Bug("Esportatore_Universale_2:",
                  "- Export Impianti: nella colonna 'Metodo di Produzione' ora viene mostrata la descrizione e non il codice." &
                  "- Export Imprese,Centri: rinominate le colonne ind_des,frz_des, com_des, pro_cod in 'Indirizzo','Frazione','Comune','Provincia'.")

            '==================================

            Riga_Data("16 Novembre 2021")

            Riga_Requisiti("Migra",
                       "Per importazione QdC AgriBologna", "665")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per enumStampe2010 dei reports riepilogo superfici", "91")

            Riga_Requisiti("CORE WS",
                       "Per importazione QdC AgriBologna", "13/10/2021")

            Riga_Requisiti("Agenda",
                       "per reports riepilogo superfici", "16/11/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Esportatore_Universale_2:",
                  "Fix Vari e completato porting di alcune parti mancanti dalle stampe 2003.")

            Riga_Text("Reports Riepilogo Impiego Superfici Mono e Multi Azienda:",
                  "Completato porting da stampe 2003")

            '==================================

            Riga_Data("08 Novembre 2021")

            Riga_Requisiti("Migra",
                       "Per importazione QdC AgriBologna", "665")

            Riga_Requisiti("Aggancio",
                       "Per importazione QdC AgriBologna", "94")

            Riga_Requisiti("GiasBase",
                       "per versione Kendo 2021.1", "22 Febbraio 2021")

            Riga_Requisiti("Configurazione_Siti",
                       "Per nuovi enumStampe2010 dei certificati di pomodoro", "86")

            Riga_Requisiti("CORE WS",
                       "Per importazione QdC AgriBologna", "13/10/2021")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Bug("Importazione QDC Agribologna:",
                  "Inserito Dispose e Close Report")

            '==================================

            Riga_Data("04 Novembre 2021")

            Riga_Requisiti("Migra '665' + Aggancio '94': ",
                       "Per importazione QdC AgriBologna")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '86':",
                       "Per nuovi enumStampe2010 dei certificati di pomodoro.")

            Riga_Requisiti("CORE WS 13/10/2021:",
                       "Per importazione QdC AgriBologna")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Visibilità lotto prodotto:",
                  "Aggiornati report per utilizzare configurazione da database solo se il modulo cantine è disattivato")

            Riga_Bug("SchedaCampagna.aspx:",
                  "Fix stampa Registro Trattamenti Lombardia Veneto.")

            '==================================

            Riga_Data("28 Ottobre 2021")

            Riga_Requisiti("Migra '665' + Aggancio '94': ",
                       "Per importazione QdC AgriBologna")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '86':",
                       "Per nuovi enumStampe2010 dei certificati di pomodoro.")

            Riga_Requisiti("CORE WS 13/10/2021:",
                       "Per importazione QdC AgriBologna")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Estratto Conto Contatti",
                  "Bugfix su filtro contatto se si stampa da impresa che non è quella proprietaria del contatto")

            Riga_Text("Report Statistiche",
                  "Aggiornamento per modifiche alla query di lettura dati")

            Riga_Text("Traduzioni Stampe",
                  "Aggiunta voce 'Sede Operativa'")

            '==================================

            Riga_Data("20 Ottobre 2021")

            Riga_Requisiti("Migra '665' + Aggancio '94': ",
                       "per UMA")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '86':",
                       "Per nuovi enumStampe2010 dei certificati di pomodoro.")

            Riga_Requisiti("CORE WS 05/07/2021:",
                       "Per UMA")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("SalvaReportPdf",
                  "Bugfix tolto Close e Dispose da dentro alla funzione SalvaReportPdf perché andava in conflitto con la maggior parte delle stampe")

            '==================================

            Riga_Data("19 Ottobre 2021")

            Riga_Requisiti("Migra '665' + Aggancio '94': ",
                       "per UMA")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '86':",
                       "Per nuovi enumStampe2010 dei certificati di pomodoro.")

            Riga_Requisiti("CORE WS 05/07/2021:",
                       "Per UMA")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("SchedaCampagna",
                  "- Bugfix caricamento dei dati delle piogge (modificato tipo dati numerici del dataset da single a double)." &
                  "- Bugfix Superficie Trattata nella Stampa Scheda di Campagna Trattamenti Antiparassitari.")

            '==================================

            Riga_Data("13 Ottobre 2021")

            Riga_Requisiti("Migra '665' + Aggancio '94': ",
                       "per UMA")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '86':",
                       "Per nuovi enumStampe2010 dei certificati di pomodoro.")

            Riga_Requisiti("CORE WS 05/07/2021:",
                       "Per UMA")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Documentale:",
                  " Importazione Report QDC in Documentale ")



            '==================================

            Riga_Data("05 Ottobre 2021")

            Riga_Requisiti("Migra '652' + Aggancio '94': ",
                       "per UMA")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '86':",
                       "Per nuovi enumStampe2010 dei certificati di pomodoro.")

            Riga_Requisiti("CORE WS 05/07/2021:",
                       "Per UMA")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Report Uma",
                  "- Richiesta Carburante: migliorato layout pagina documenti allegati")

            '==================================

            Riga_Data("28 Settembre 2021")

            Riga_Requisiti("Migra '652' + Aggancio '94': ",
                       "per UMA")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '86':",
                       "Per nuovi enumStampe2010 dei certificati di pomodoro.")

            Riga_Requisiti("CORE WS 05/07/2021:",
                       "Per UMA")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Report Uma",
                  "- Salvate più informazioni nel file di log in caso di eccezione" &
                  "- Aggiunto controllo per valori null su somma carburante da lavorazioni e allevamenti")

            '==================================

            Riga_Data("24 Settembre 2021")

            Riga_Requisiti("Migra '652' + Aggancio '94': ",
                       "per UMA")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '86':",
                       "Per nuovi enumStampe2010 dei certificati di pomodoro.")

            Riga_Requisiti("CORE WS 05/07/2021:",
                       "Per UMA")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("SchedaCampagna",
                  "Fix rallentamenti su stampa Scheda Campagna (segnalazione Imm. Dante) " &
                  "[dovuti alle precedenti modifiche di ottimizzazione, quello che era stato fatto per COLDIRETTI (SQL 2017) non andava bene per i database che stanno su SQL2012 a causa di comportamenti diversi su delle join a seconda delle 2 versioni]")

            '==================================

            Riga_Data("14 Settembre 2021")

            Riga_Requisiti("Migra '652' + Aggancio '94': ",
                       "per UMA")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '86':",
                       "Per nuovi enumStampe2010 dei certificati di pomodoro.")

            Riga_Requisiti("CORE WS 05/07/2021:",
                       "Per UMA")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Report Uma",
                  "- Istruttorie: migliorato layout nascondendo sezione assegnazione/rigetto in base all'esito del verbale")

            Riga_Text("AnteprimaEtichette.Aspx",
                  "- Bug fix query costruita male in anteprima stampa etichette")

            '==================================

            Riga_Data("09 Settembre 2021")

            Riga_Requisiti("Migra '652' + Aggancio '94': ",
                       "per UMA")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '86':",
                       "Per nuovi enumStampe2010 dei certificati di pomodoro.")

            Riga_Requisiti("CORE WS 05/07/2021:",
                       "Per UMA")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Report Uma",
                  "- Richiesta: individuazione se richiesta iniziale od integrativa attraverso colonna apposita" &
                  "- Istruttoria Rendicontazione: miglioramento ricerca prima richiesta anno" &
                  "- Richiesta, Istruttorie Richiesta e Rend: aggiornata descrizione ente istruttore" &
                  "- allevamenti: riportata descrizione generica per la quantità di animali" &
                  "- lavorazioni e allevamenti: uniformati i litri di carburante mostrati visualizzando le quantità decurtate e non quelle richieste" &
                  "- tutti i report: tolti i decimali per i litri di carburante perché non gestiti")

            Riga_Text("Stampa Scheda Campagna",
                  "- Ottimizzazioni performance ( modifiche a query lente , creazione indici + sostituzione xFiltroAggiuntivo enorme con join tabella temporanea)")

            '==================================

            Riga_Data("26 Agosto 2021")

            Riga_Requisiti("Migra '652' + Aggancio '94': ",
                       "per UMA")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '86':",
                       "Per nuovi enumStampe2010 dei certificati di pomodoro.")

            Riga_Requisiti("CORE WS 05/07/2021:",
                       "Per UMA")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Report Uma",
                  "Istruttoria Rendicontazione: rimanenza carburante anno corrente prelevata dalla rendicontazione e non calcolata")

            '==================================

            Riga_Data("18 Agosto 2021")

            Riga_Requisiti("Migra '652' + Aggancio '94': ",
                       "per UMA")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '86':",
                       "Per nuovi enumStampe2010 dei certificati di pomodoro.")

            Riga_Requisiti("CORE WS 05/07/2021:",
                       "Per UMA")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Report Uma",
                  "- Richiesta: ora per il calcolo del carburante assegnabile tiene conto di carburante assegnato se presente, in sostituzione al richiesto." &
                  "  Dati del compilatore corrispondono ora sempre all'utente che ha creato la richiesta, non all'utente di lancio della stampa" &
                  "- Rendicontazione: correzione visualizzazione carburante lavorazioni terzisti")

            '==================================

            Riga_Data("15 Luglio 2021")

            Riga_Requisiti("Migra '652' + Aggancio '94': ",
                       "per UMA")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '86':",
                       "Per nuovi enumStampe2010 dei certificati di pomodoro.")

            Riga_Requisiti("CORE WS 05/07/2021:",
                       "Per UMA")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Pagina Filtro Stampe Conferimento:",
                  "Aggiunte estrazioni in lavorazione dei Certificati Pomodoro: 'Esportazione Massiva' ed 'Esportazione Excel'")

            Riga_Text("CertificatiPomodoro_XLS:",
                  "Rilascio estrazione Excel dei certificati di pomodoro.")

            Riga_Text("CertificatoPomodoro:",
                  "Rilascio stampa PDF dei certificati di pomodoro.")

            '==================================

            Riga_Data("12 Luglio 2021")

            Riga_Requisiti("Migra '652' + Aggancio '94': ",
                       "per UMA")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '85':",
                       "Per nuovi enumStampe2010 dell'UMA.")

            Riga_Requisiti("CORE WS 05/07/2021:",
                       "Per UMA")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Report Biologico:",
                  "Fix mancata estrazione Progetto, OP + miglioramento performance")

            Riga_Text("Report Bolla DDT Trombin:",
                  "Eliminato codice GGN dalla stampa")

            '==================================


            Riga_Data("08 Luglio 2021")

            Riga_Requisiti("Migra '652' + Aggancio '94': ",
                       "per UMA")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '85':",
                       "Per nuovi enumStampe2010 dell'UMA.")

            Riga_Requisiti("CORE WS 05/07/2021:",
                       "Per UMA")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Report UMA:",
                  "Gestito carburante assegnato per le richieste terzisti nel report di istruttoria della richiesta")

            '==================================

            Riga_Data("05 Luglio 2021")

            Riga_Requisiti("Migra '652' + Aggancio '94': ",
                       "per UMA")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '85':",
                       "Per nuovi enumStampe2010 dell'UMA.")

            Riga_Requisiti("CORE WS 05/07/2021:",
                       "Per UMA")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Report UMA:",
                  "- Gestiti dati dei fascicoli in richiesta e rendicontazione" &
                  "- Gestito carburante assegnato per le richieste terzisti tramite colonne apposite" &
                  "- Correzione sul calcolo delle rimanenze nell'istruttoria della rendicontazione" &
                  "- Gestite richieste di integrazione")

            '==================================

            Riga_Data("01 Luglio 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '85':",
                       "Per nuovi enumStampe2010 dell'UMA.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("SchedaCampagna.aspx:",
                  "Fix stampa 'REGISTRO DEI TRATTAMENTI'.")

            '==================================

            Riga_Data("18 Giugno 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '85':",
                       "Per nuovi enumStampe2010 dell'UMA.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Report UMA:",
                  "- Aggiunti anno e numero pratica nel nome del file pdf creato dai report" &
                  "- Aggiornata ultima pagina del report di Richiesta Carburanti" &
                  "- Gestiti dati di obbligatorietà e fase degli allegati nei report di richiesta e rendicontazione")

            Riga_Text("Esportazione Excel Anagrafica Contatti:",
                  "Bugfix per colonna listino di vendita associato")

            Riga_Text("Report Scheda di Campagna:",
                  "Aggiunta la sezione 'Trattamenti Post Raccolta' nella stampa del 'Registro Aziendale Unico'.")

            '==================================

            Riga_Data("11 Giugno 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '85':",
                       "Per nuovi enumStampe2010 dell'UMA.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Piano Colturale XLS:", "Fix per considerare data semina/raccolta prevista")

            Riga_Text("Report Scheda di Campagna:",
                  "Aggiunta la sezione 'Visite Ispettive' nella stampa del 'Registro Aziendale Unico'.")

            Riga_Text("Report UMA:",
                  "- Gestiti report terzisti per la richiesta, la rendicontazione e le istruttorie" &
                  "- Creazione report istruttoria per la rendicontazione")

            '==================================

            Riga_Data("07 Giugno 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '85':",
                       "Per nuovi enumStampe2010 dell'UMA.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("AgronicaCoreStampeDAL.FreshAndFood.Bolle_XLS (chiamata 11776 di Garezzo):",
                  "modificata la join per leggere Regione_Conferente (non più dal Cod_indirizzorisUm ma lettura diretta su impresexindirizzi)")

            Riga_Text("Report UMA:",
                  "- Wip Richiesta Carburanti" &
                  "- Wip Verbale Istruttoria per la richiesta, raggiunta versione stabile del report" &
                  "- Creazione Rendicontazione Carburanti")

            '==================================

            Riga_Data("1 Giugno 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Stampa Conferimenti Trasportatori (excel):",
                  "Aggiornamento per filtri aggiuntivi sugli articoli e sui conferenti")

            '==================================

            Riga_Data("31 Maggio 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO")

            Riga_Text("Stampa Riepilogo Conferimenti per Articolo:",
                  "Fix estrazione colonna Punteggio per gestire valori con decimali")

            Riga_Text("Piano Colturale Catasto XLS:",
                  "Fix per impostare data semina/raccolta prevista se non sono presenti semine o raccolte")

            '==================================

            Riga_Data("18 Maggio 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Stampe Biologico:",
                  "Estrazione Carichi/Scarichi, aggiunti numero e data ddt")

            Riga_Text("Report Scheda di Campagna:",
                  "- BugFix visualizzazione nella stampa del 'Registro Aziendale Unico' le Rotazioni Colturali inserite negli Appezzamenti." &
                  "- Aggiunte le sezioni 'Trappole Installate' , 'Rilievi Avversità nelle Trappole' , 'Rilievi Avversità in Campo' e 'Indici di Maturità' nella stampa del 'Registro Aziendale Unico'.")

            Riga_Text("StampeBootstrap.Master:",
                  "Fix per nuova gestione di inclusione della versione di JQuery")

            Riga_Text("Stampa BollaAccettazione:",
                  "Gestita lettura del valore del grado tenderometrico")

            '==================================

            Riga_Data("30 Aprile 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Stampe Statistiche in pdf",
                  " Inclusione corrispettivi")

            '==================================

            Riga_Data("29 Aprile 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Stampe Statistiche",
                  " Gestiti raggruppamenti ragione sociale azienda e provenienza/destinazione")

            '==================================

            Riga_Data("27 Aprile 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("SchedaCampagna.aspx",
                  " CaricaDsConcimazioni: modificata lettura epoche delle concimazioni (ora via WS)")

            Riga_Text("Rpt_SchedaCampagna_EUREP_GAP_Semplificata.rpt e Rpt_SchedaCampagna_EUREP_GAP_Multicentro.rpt:",
                  "Impostato il testo delle colonne Colture Precedenti per andare a capo.")

            '==================================

            Riga_Data("26 Aprile 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Mastrino:",
                  "Bugfix Aggiunto Progr_Registrazione nell'ordinamento altrimenti in alcuni casi non rimanevano vicine le righe della stessa registrazione")

            '==================================

            Riga_Data("16 Aprile 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Tutti i report:",
                  "Eliminati altri caratteri problematici dal nome del file pdf generato ( , ' : + )")

            Riga_Text("RegistriBio.vb:",
                  "Sistemata estrazione doc_numero nelle query")

            Riga_Text("Rpt_SchedaMateriePrimeBiologico:",
                  "Centrati margini della stampa")

            '==================================

            Riga_Data("07 Aprile 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Report Scheda di Campagna:",
                  "Aggiunta sezione 'Osservazioni Fasi Fenologiche' nella stampa del 'Registro Aziendale Unico'.")

            Riga_Text("Filtro_Stampe_Conf:",
                  "- Abilitata estrazione 'Esportazione Massiva Bolle di Conferimento' e aggiunto parametro di filtro sul centro aziendale alla stessa" &
                  "- In lavorazione nuova esportazione 'Estratto Conto Bolle', disabilitata")

            '==================================

            Riga_Data("24 Marzo 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Schede_OP",
                  "- Rpt_AttoNotorio.rpt: modificato logo" &
                  "- Rpt_AttoNotorio.rpt: modificata indicazione cooperativa in OP TERREMERSE SEZIONE ORTOFRUTTA (solo per terremerse)" &
                  "- Rpt_AttoNotorio.rpt: modificata indicazione associata all' OP. PEMPACORER")

            '==================================

            Riga_Data("15 Marzo 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("SqlClient:",
                  "Impostato uso SqlClient di default al posto dell'OleDb")

            '==================================

            Riga_Data("11 Marzo 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("BollaAccettazione.aspx (stampa Fruttagel):",
                  "nella stampa del prezzo unitario, modificata formattazione per mostrare da 2 a 5 decimali (in base a quanti valorizzati).")

            '==================================

            Riga_Data("05 Marzo 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '22 Febbraio 2021' :",
                       "per versione Kendo 2021.1")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 05/03/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("File Temporanei:",
                  "Modificata funzione che genera il nome file temp dentro a SitoStampe/File_Temporanei per evitare sovrapposizioni sul millisecondo")

            Riga_Text("Vivaismo/StampaPassaporto.aspx:",
                  "Integrazione QR-Code in etichetta registro passaporti")

            Riga_Text("Esportazione_OP_Catasto, Esportazione_OP_Produttori:",
                  "modificato nome e formato (da xls a xlsx) delle estrazioni excel")

            Riga_Text("Visualizzatore Report:",
                  "Aggiunta possibilità di indicare il nome del file che deve apparire sul visualizzatore pdf di chrome quando si scarica il file")

            Riga_Text("Registro Corrispettivi:",
                  "fix NullReference se non ci sono dati + scrittura file log se ci sono errori + passaggio del nome file al Visualizzatore")

            Riga_Text("Report vari contabilità:",
                  "passaggio del nome file al VisualizzatoreReport: E/C clienti/fornitori, E/C Contatti, Liquidazione, Mastrino, Registri Iva, " & vbCrLf &
                  "Bilancio, Piano dei conti, Libro Giornale, Fattura, DTT, Ordine, Ricevuta A4")

            Riga_Text("Elenco Report (libro giornale e scheda campagna):",
                  "Corretta visualizzazione e download di allegati archiviati in stampa definitiva")

            Riga_Text("Esportatore Universale Imprese:",
                  "Gestito baco su particelle e rappresentante legale")

            '==================================

            Riga_Data("18 Febbraio 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 27/01/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("PianoColturaleCatasto_XLS:",
                  "- gestita la data fioritura prevista in caso di assenza di registrazione della 'Data Fioritura' in agenda" &
                  "- reintrodotta visualizzazione sup intersezione catasto con il campo (in caso di assenza di intersezione con l'appezzamento)")

            Riga_Text("Bolle_Conf_XLS:",
                  "Aggiunta tabella riepilogativa dei prodotti per i totali netti a pagamento")

            Riga_Text("Documenti contabili F&F:",
                  "Corretto errore nella ricerca parametri qualitativi con filtro per varietà")

            Riga_Text("Filtro_ElaboratiContabili - EstrattoConto_ClientiFornitori:",
                  "rename scadenziario in scadenzario")

            Riga_Text("BollaAccettazione Fruttagel - CRBuonoAccettazione:",
                  "sostituito il peso netto con il netto a pagamento sulla riga di bolla + rename intestazione colonna.")

            Riga_Text("Filtro_Stampe_Conf:",
                  "Aggiunta estrazione disabilitata 'Esportazione Massiva Bolle di Conferimento'")

            '==================================

            Riga_Data("4 Febbraio 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 27/01/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("BollaAccettazione:",
                  "- introdotta la stampa del prezzo unitario netto sull'articolo;" &
                  "- modifica su stampa 'trasporto a cura del';" &
                  "- stampa simbolo euro nelle note.")

            Riga_Text("SaldoImballi_Conf:", "Report Saldo imballi: introdotto filtro per più conferenti;")

            Riga_Text("EC_Imballi_Conf:", "Report E/C Beni Confezionamento: introdotto filtro per più conferenti;")

            Riga_Text("Bolle_Conf_XLS:",
                  "- aggiunte colonne con default zero per ogni sigla di calibro, compresa colonna per prodotti senza calibro" &
                  "- gestita visualizzazione peso netto a pagamento sulla relativa colonna del calibro (class. qual.)" &
                  "- cambiato charset in unicode (utf-8) per consentire all'excel di visualizzare caratteri speciali")

            Riga_Text("Filtro_Stampe_Conf:",
                  "- Per i report 'Estratto Conto Beni Confezionamento' e 'Saldo Imballi' viene abilitata la Dropdown Prodotto al posto della griglia." &
                  "- BugFix bottone 'Filtro Prodotti' che si accavalla col Footer della pagina." &
                  "- Intestazione delle griglie della pagina fissate in alto in caso di scorrimento")

            Riga_Text("PianoColturaleCatasto_XLS:", "aggiunte colonne 'Data Fioritura' e 'Data Raccolta' all'estrazione excel")

            '==================================

            Riga_Data("27 Gennaio 2021 - Versione B")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 27/01/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Bolle_Conf_XLS:",
                  "- nuovo calcolo del peso lordo;" &
                  "- nuovo calcolo della tara imballi;")

            Riga_Text("Riepilogo_Conf_Articolo:",
                  "- gestito il filtro multi conferente;" &
                  "- gestito il filtro multi prodotto;" &
                  "- nuovo calcolo della tara imballi;")

            '==================================

            Riga_Data("27 Gennaio 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 27/01/2021:",
                       "Per ricerca Contatti-Rapporti Contabili")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Stampa Global:",
                  "modificata lettura carenza via web service (x SILVIA AGRIBOLOGNA che utilizza il LAN il quale NON salva nell'operazione la carenza) per gestire l'eventuale salvataggio della riga (for_veg_cod) selezionata.")

            Riga_Text("Bolle_Conf_XLS:",
                  "- gestito il filtro multi conferente;" &
                  "- gestito il filtro multi prodotto;" &
                  "- aggiunta estrazione Cod. fiscale e Regione del conferente;" &
                  "- aggiunta estrazione Cod. fiscale e Regione del produttore." &
                  "- aggiunte colonne personalizzate per Fruttagel: Descrizione JDE e Varietà FRG.")

            Riga_Text("Filtro_Stampe_Conf:",
                  "- Aggiunta griglia di visualizzazione e scelta dei Prodotti." &
                  "- Sostituzione di tendina di scelta singola conferenti con griglia a scelta multipla. Aggiornamento delle ricerche dei cessionari e dei produttori")

            '==================================

            Riga_Data("21 Gennaio 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 31 dicembre 2020:",
                       "per ricerca prodotti")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("BollaAccettazione.aspx (layout personalizzato Fruttagel):",
                  "ampliato sottoreport imballi entrata per visualizzare nel riepilogo più imballi")

            Riga_Text("Bolla_FF.aspx (layout per tutti):",
                  "- gestiti nuovi casi di trasformati animali + beni confezionamento animale + conferimento a numero " & vbCrLf &
                  "- corretto specchietto imballi in entrata che non riportava più il numero")

            '==================================

            Riga_Data("17 Gennaio 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 31 dicembre 2020:",
                       "per ricerca prodotti")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Bolle_Conf_XLS:",
                  "- sistemata e debuggata per primo rilascio" &
                    "- commentato il codice (ancora in sviluppo) sul riepilogo in fondo")

            Riga_Text("BollaAccettazione.aspx (layout personalizzato Fruttagel):",
                  "- arrotondato all'unità (e formattato graficamente) il peso netto sulla riga " &
                  "- sistemato riquadro vettore (quando piva negativa e provincia non valorizzata)")

            '==================================

            Riga_Data("14 Gennaio 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 31 dicembre 2020:",
                       "per ricerca prodotti")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("leggiTabelle_ws_client.js:",
                  "Allineamento alla versione dell'Agenda")

            Riga_Text("Conferimenti FF:",
                  "ripristinata stampa visto che ora non viene più salvato il modulo sull'Agenda")

            '==================================

            Riga_Data("08 Gennaio 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 31 dicembre 2020:",
                       "per ricerca prodotti")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("GestioneRichieste: ",
                  "bugfix errore per htVariabiliStampe=Nothing su stampe Campagna")

            '==================================

            Riga_Data("5 Gennaio 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 31 dicembre 2020:",
                       "per ricerca prodotti")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Filtro_Stampe_Conf.js:",
                    "- Sostituito il menu a tendina del magazzino in favore del centro aziendale." &
                    "- menù a tendina report: rimosso doppione su Esportazione_BolleFF_XLS" &
                    "- EseguiEstrazione: corretto link x redirect al report Conf_EsportazionexTrasportatori_XLS " &
                    "- EseguiEstrazione: sistemati parametri passati al report Conf_EsportazionexTrasportatori_XLS " &
                    "- EseguiEstrazione: messaggio di manutenzione sul report Conf_Esportazione_BolleFF_XLS ")

            Riga_Text("AccettazioneDaDiversi.vb:",
                    "Carica_DSImballaggi_StampaBollaECertificato: sezione imballi in uscita, eliminata la gestione Ricava_NumeroDocumento_Con_Sequenza" &
                    "(patch su riepilogo imballi in uscita su stampa bolla di conferimento).")

            Riga_Text("Trasportatori_XLS:",
                  "- gestito filtro mat_cod, veg_cod, cul_cod, codice_conferente" &
                  "- rimossa colonna ordine_det")

            '==================================

            Riga_Data("4 Gennaio 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 31 dicembre 2020:",
                       "per ricerca prodotti")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Filtro_Stampe_Conf.js:",
                    "- Migliorata la gestione del filtro utente sui menu a tendina " &
                    "- Possibilità di effettuare la ricerca dei prodotti con un filtro di due soli caratteri " &
                    "- Fix sul codice prodotto fornito alla funzione di stampa dei report")

            Riga_Text("SaldoImballi_Conf e EC_Imballi_Conf:",
                    "patch sul filtro aggiuntivo passato alla query del report")


            '==================================

            Riga_Data("2 Gennaio 2021 - Ver.B")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 31 dicembre 2020:",
                       "per ricerca prodotti")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Filtro_Stampe_Conf.js:",
                    "Fix per filtro su imballi e per errori nel lancio stampa se non sono stati scelti prodotti")

            Riga_Text("SaldoImballi_Conf e EC_Imballi_Conf:",
                  "svincolati report dalla stampante + modifiche grafiche")

            Riga_Text("Riepilogo_Conf_Articolo:",
                  "sistemata grafica report + math.abs sul mat_cod (temporaneo)")

            '==================================

            Riga_Data("2 Gennaio 2021")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 31 dicembre 2020:",
                       "per ricerca prodotti")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Filtro_Stampe_Conf.js:",
                  "rename 'dettaglio Stabilimento' in 'dettaglio Destinazione'")

            Riga_Text("SaldoImballi_Conf e EC_Imballi_Conf:",
                  "aggiornata grafica dei report e stampato il codice conferente al posto della piva")

            '==================================

            Riga_Data("31 Dicembre 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("CORE WS 31 dicembre 2020:",
                       "per ricerca prodotti")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Filtro_ReportBiologico.js e Filtro_Stampe_Conf.js:",
                  "ricerca prodotti cambio tipo dato e default filtroProdottiValorizzati")

            Riga_Text("SaldoImballi_Conf e EC_Imballi_Conf:",
                  "ToShortDateString su data_da e data_a")

            '==================================

            Riga_Data("28 Dicembre 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '78':",
                       "x esportazione Excel listino.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Conferimenti :",
                  "Filtro_Stampe_Conf: nuovi sviluppi per Fruttagel.")

            Riga_Text("Riepilogo_Conf_Articolo :",
                  "Porting report di Fruttagel da stampe 2003 + adeguamento x conferimento versione WEB.")

            Riga_Text("Contabilita - Listini:",
                  "Listini_XLS: porting da stampe 2003 dell'esportazione excel del listino.")

            '==================================

            Riga_Data("23 Novembre 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("master :",
                  " gestione versione bootstrap in maniera configurabile.")


            Riga_Text("SchedaCampagna - Rpt_SchedaCampagna_ConserveItalia:",
                  " eliminato check DATASET: DS_Arboree.xsd, DS_Erbacee.xsd, DS_UsoNonAgricolo.xsd: i campi N_Campo e N_appezza impostati a string (per evitare errore di cast a integer).")

            Riga_Text("SchedaCampagna - Rpt_SchedaCampagna_ConserveItalia:",
                  "- eliminata selezione dei check visite e dichiarazioni " &
                  "- settato il default lav_cod nella chiamata alla funzione CaricaDsDichiarazioni")


            '==================================

            Riga_Data("13 Novembre 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("CodexUtility :",
                  "Fix bug selectCommand dopo passaggio a doppio data provider")

            '==================================

            Riga_Data("3 Novembre 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Parametri qualitativi :",
                       "Gestione parametri qualitativi data e stringa")

            '==================================

            Riga_Data("23 Ottobre 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Stampa Global",
                  "introdotta lettura carenza via web service x SILVIA AGRIBOLOGNA che utilizza il LAN il quale NON salva nell'operazione la carenza")

            '==================================

            Riga_Data("21 Ottobre 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Data Provider:",
                      "ricompilate per avere come default OleDB")

            '==================================

            Riga_Data("19 Ottobre 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Fattura_NotaAccredito :",
                      "spostata la funzione che ottiene la lingua in cui stampare il documento su DocumentiContab.vb")

            Riga_Text("DDT_BolleConf :",
                      "applicato lo stesso algoritmo del DDT per l'ottenimento della lingua in cui stampare il documento (chiamata a OttieniLingua_ReportContabilita)")

            '==================================

            Riga_Data("7 Ottobre 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Biologico - ModelloTerzistiBio:",
                      "nuovo report x ABOCA by Giacomo Casadei")

            Riga_Text("Filtro_ReportBiologico:",
                      "link nuovo report x ABOCA by Giacomo Casadei")

            Riga_Text("Report Biologico:",
                      "modifiche all'estrazione")

            '==================================

            Riga_Data("7 Ottobre 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("StampeBootstrap.Master:",
                      "Gestito titolo pagina per nuovo menu")

            Riga_Text("DDT_BolleConf:",
                   "- sistemato log_errori mettendo la parte delle traduzioni in un try catch separato (non quello dell'aggancio dataset)" &
                    "- OttieniLInguaReport: nuovo algoritmo di recupero lingua in cui stampare il documento")

            '==================================

            Riga_Data("23 Settembre 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("Stampe Statistiche:",
                      " fix sul caricamento per anno introdotto con la versione precedente")

            Riga_Text("Stampe Statistiche:",
                      " fix query string troppo lunga in caso di selezione tutte le nazioni come filtro")

            Riga_Text("Stampe Statistiche:",
                      " tolta dai report il codice referenza e allargato campo descrittivo del dettaglio")

            Riga_Text("Stampe Statistiche:",
                      " bug fix vari come da documento analisi bug di Borgoluce")

            Riga_Text("Stampe Statistiche:",
                      " Introdotto filtro per Dava Evasione Prevista per Ordini di Vendita e Report Vendite (Ordini da evadere)")

            Riga_Text("Biologico:",
                      " integrata la nuova pagina di filtro di lancio per report biologici ")

            Riga_Text("Stampe Statistiche:",
                      " Introdotta possibilità scelta decimali da stampare per tipo valore QTA")


            '==================================

            Riga_Data("21 Agosto 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO")

            Riga_Text("SchedaCampagna:",
                      " referente su intestazine scheda: utilizzata la stessa funzione della scheda multicentro anche sulle altre stampe (reg trattamenti veneto, qdc lombardia-veneto, ...)")

            '==================================

            Riga_Data("19 Agosto 2020 versione B")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche SI: " &
                         "- introdotto file appsettings.config con link su web.config" &
                         "- modifica versione crystal")

            Riga_Text("OPERAZIONE CRYSTAL REPORT:",
                      "- Nella pagina VisualizzatoreReport.aspx modificata la versione crystal")

            '==================================

            Riga_Data("19 Agosto 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                    "patch lettura classi tossicologiche")

            '==================================

            Riga_Data("10 Agosto 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("StampeStatistiche",
                  "Aggiunte colonne x terna confezionamento + tipi imballi")

            '==================================


            Riga_Data("5 Agosto 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("StampeStatistiche",
                  "Aggiunte colonne Data_Consegna, Data Ultima Consegna / Nuova Gestione per visualizzazione numero decimali nei reports ")

            Riga_Text("Etichette",
                  "bug fix su ricerca parametri qualitativi (verificatosi in Mini Frutta ")

            '==================================

            Riga_Data("23 Luglio 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("StampeStatistiche",
                  "bug fix raggruppamento referenza senza codice (PALAZZONA DI MAGGIO) ")

            Riga_Text("FreshAndFood - bolla",
                  "Riepilogo Imballi: avvicinati i dettagli (su Mini Frutta si era raggiunto il limite del riquadro e non si vedeva un imballo nel riepilogo)")

            '==================================

            Riga_Data("20 Luglio 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("EtichetteVascheEnologiche",
                  "sviluppo x gestione stampa di più etichette di vasca per la stessa vasca (stesso identificativo ma vini/lotti diversi)")

            '==================================

            Riga_Data("30 Giugno 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampa MVV",
                  "spostato Response.Redirect fuori dal catch altrimenti venivano generati una marea di file di log inutili per thread interrotto")

            '==================================

            Riga_Data("29 Giugno 2020")

            Riga_Requisiti("Migra '604' + Aggancio '83':",
                       "Modifica Vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("RegistroPassaporti",
                          "Corretto baco per mancata lettura testo in lingua")

            Riga_Text("Export XML Impianti",
                          "Controllo su presenza metodo produzione")

            Riga_Text("Stampa MVV",
                "Aggiunta Lotto su MVV")

            '==================================

            Riga_Data("25 Giugno 2020")

            Riga_Requisiti("Migra '589' + Aggancio '83':",
                       "per gestione piva estera, lunghezza campo piva portata a 25")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Borgoluce",
                  "Report")

            '==================================

            Riga_Data("19 Giugno 2020")

            Riga_Requisiti("Migra '589' + Aggancio '83':",
                       "per gestione piva estera, lunghezza campo piva portata a 25")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("PianoColturaleCatasto_XLS",
                  "patch nel caso di impianto con semina/trapianto ma senza catasto")

            Riga_Text("Stampe Statistiche",
                  "Nuova stampa confronto anno / 2 valori")

            '==================================

            Riga_Data("15 Giugno 2020")

            Riga_Requisiti("Migra '589' + Aggancio '83':",
                       "per gestione piva estera, lunghezza campo piva portata a 25")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("BollaAccettazione.aspx",
                  "gestito punteggio non valorizzato")

            Riga_Text("EC_Imballi_Conf e SaldoImballi_Conf",
               "gestito cod_rapporto -11 personalizzato di fruttagel")

            Riga_Text("Riepilogo_Conf E Tracciabilita_Conf",
               "- gestito cod_rapporto -11 personalizzato di fruttagel" &
                "- corretto bug sul join con le vasche (mancava il sa_cod)")

            Riga_Text("LogProvider.Scrivi_LOG",
                "DefaultDirectoryLOG impostata = Path.GetTempPath() per ovviare al fatto che su molti server non c'è la cartella C:\GIASLAN\Log")

            '==================================

            Riga_Data("11 Giugno 2020")

            Riga_Requisiti("Migra '589' + Aggancio '83':",
                       "per gestione piva estera, lunghezza campo piva portata a 25")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("PagatiSuConferito.aspx",
                  "Aggiunte Colonne degrado % e degrado Kg. Modifiche a Peso Lordo e modifca labels")

            Riga_Text("BollaAccettazione (bolla conferimento personalizzata Fruttagel)",
                    "- Fix sul peso netto (gestione stampa bolla con più dettagli)." &
                    "- Query di lettura: letto ocalibro e opunteggio.")

            '==================================

            Riga_Data("8 Giugno 2020")

            Riga_Requisiti("Migra '589' + Aggancio '83':",
                       "per gestione piva estera, lunghezza campo piva portata a 25")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("StampaStat12Mesi.aspx",
                  "Aggiunta di filtri alla query di caricamento dei report" & vbCrLf &
                  "Possibilità di filtrare per mese inizio / fine del periodo di riferimento")

            Riga_Text("SchedaCampagna.aspx",
                    "sezione semina/trapianto, nelle note modificata udm della distanza su fila/tra fila in 'Distanza (m)'")

            '==================================

            Riga_Data("26 Maggio 2020")

            Riga_Requisiti("Migra '589' + Aggancio '83':",
                       "per gestione piva estera, lunghezza campo piva portata a 25")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Esportatore_Universale_2.aspx, BilancioFertilizzazioni_Dettagliato_XLS.aspx, CatastoOIPomodoroIndustriaNordItalia_XLS.aspx, PianoColturale_XLS.aspx, PianoColturaleCatasto_XLS.aspx",
                "adeguamento per GESTIONE PIVA ESTERA con modifica colonna PIVA su database a nvarchar(25): modificata creazione della tabella temporanea")

            Riga_Text("DDT_BolleConf",
                  "Traduzione in lingua Stampa DDT. Modificati crbolla.rpt, crbolla2016.rpt, crbolla2016_lb.rpt, DocumentiContab.vb")

            Riga_Text("Filtro_StampeCantine.aspx",
                "Nel menù a tendina 'tipologia semilavorato' aggiunto il vino atto a divenire arricchito, codice generazione 780")

            '==================================

            Riga_Data("29 aprile 2020")

            Riga_Requisiti("Migra '588' + Aggancio '83':",
                       "aggiunta di campi sulle tabelle scadenze / documentale")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Scheda_OP_Tipo_1.aspx - Rpt_Scheda_OP_Tipo_1.rpt",
               "- aggiornato testo sulla section18 - ADESIONE AI DPI come da richiesta" & vbCrLf &
               "- patch sul caricamento dei loghi")


            '==================================

            Riga_Data("28 Aprile 2020")

            Riga_Requisiti("Migra '585' + Aggancio '83':", "Modifiche tabelle liquidazione + Lingue")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.aspx",
                     "riga intestazione 13 nel caso di impostazione 848 (x zespribud): lasciata bianca")

            Riga_Text("BilancioFertilizzazioni_Dettagliato.aspx",
                     " - Gestita unità di misura Qta ")

            '==================================

            Riga_Data("7 Aprile 2020")

            Riga_Requisiti("Migra '585' + Aggancio '83':", "Modifiche tabelle liquidazione + Lingue")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '70' :",
                       "x report bilancio fertilizzazioni dettagliato.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.aspx",
                     "Modifica stampa intestazione azienda in base all'impostazione enum_Impostazioni_Utenti.SUPERUSER_FF_GEST_MATERIALE_VIVAISTICO" &
                        "(dettagli su nota TFS di questa pagina)")

            Riga_Text("BilancioFertilizzazioni_Dettagliato_XLS.aspx",
                     "Report")

            '==================================

            Riga_Data("6 Aprile 2020")

            Riga_Requisiti("Migra '585' + Aggancio '83':", "Modifiche tabelle liquidazione + Lingue")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '68' :",
                       "x esportazione universale agenda.")

            Riga_Requisiti("Altri requisiti :",
                       "")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("DDT Zespri + lingue",
                " ")

            Riga_Text("StampeBootstrap.Master: ",
                  "Aggiunte delle cose che mancavano rispetto ad Agenda Bootstrap Master (ne mancano ancora molte altre)")

            Riga_Text("Filtro_SchedeMagazzino_new: ",
                  "Eliminato uso di varibili in Session per passare valori tra le varie funzioni (sostituite da controlli hidden + var js)")

            '==================================


            Riga_Data("1 Aprile 2020")

            Riga_Requisiti("Migra '584' + Aggancio '83':", "Modifiche tabelle liquidazione")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '68' :",
                       "x esportazione universale agenda.")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Tutti i report DDT",
                "'Fatturato a:' diventa 'Cessionario:' (quando non usato come parametro) e 'Destinatario:' diventa 'Destinazione:' ")

            Riga_Text("DDT_BolleConf.aspx",
             "Gestione impostazione enum_Impostazioni_Utenti.SUPERUSER_FF_GEST_MATERIALE_VIVAISTICO:" &
                "udm visualizzata sempre a numero e dettagli sul peso omessi nella descrizione + stampa 'partenza' sui dati di intestazione")

            '==================================

            Riga_Data("24 Marzo 2020")

            Riga_Requisiti("Migra '584' + Aggancio '83':", "Modifiche tabelle liquidazione")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '68' :",
                       "x esportazione universale agenda.")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampa Etichette Fresh and Food",
                    "agronicacoreStampeDAL - FF_Etitchette.leggiParametriOmniFF_FROM - bug fix aggiungo il join sulla OTabella altrimenti se ci sono prodotti sullo stesso tipo_cod duplica le letture")

            Riga_Text("Report Liquidazione Fresh and Food",
                  "Quando liquidazione, usa principalmente i valori in Movimenti_Dettagli e solo se questi sono 0, va a leggere i valori di Liquid_Mov_campionamentoConferito")

            Riga_Text("Etichetta passaporto",
                "modifica layout - visualizzazione della cultivar nel campo 'A' - Errata Corrige su nome botanico")

            '==================================

            Riga_Data("4 Marzo 2020")

            Riga_Requisiti("Migra '576' + Aggancio '80':", "tabella per passaporto vivai")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '68' :",
                       "x esportazione universale agenda.")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Anteprima Stampe etichette passaporti vivai",
                    "inserite traduzioni")

            Riga_Text("SchedaMateriePrimeBiologico.aspx",
                 "PATCH su parte di lettura giacenze: se si finiva nell'else per giacenza scartata post query, tentava di aggiungere la riga aggiunta al giro precedente e generava eccezione")

            '==================================

            Riga_Data("25 Febbraio 2020")

            Riga_Requisiti("Migra '576' + Aggancio '80':", "tabella per passaporto vivai")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '68' :",
                       "x esportazione universale agenda.")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampe Statistiche",
                    "Ricreato pacchetto stampe per problemi su stampa costi")

            '==================================

            Riga_Data("24 Febbraio 2020 bis")

            Riga_Requisiti("Migra '576' + Aggancio '80':", "tabella per passaporto vivai")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '68' :",
                       "x esportazione universale agenda.")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampe Statistiche",
                    "Fix su formula scostamento percentuale.")

            '==================================

            Riga_Data("24 Febbraio 2020 / 21 Febbraio 2020 / 21 Febbraio 2020 bis")

            Riga_Requisiti("Migra '576' + Aggancio '80':", "tabella per passaporto vivai")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '68' :",
                       "x esportazione universale agenda.")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampe Statistiche",
                    "Fix su formula scostamento percentuale.")

            Riga_Text("Analisi costi / ricavi: ",
                    "Reso opzionale dettaglio attività giornaliere")

            '==================================

            Riga_Data("20 Febbraio 2020")

            Riga_Requisiti("Migra '576' + Aggancio '80':", "tabella per passaporto vivai")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '68' :",
                       "x esportazione universale agenda.")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("RiepilogoProdotti",
                    "correzione su Visualizza Composizione fitofarmaci.")

            Riga_Text("SchedaProdottiFitosanitariMagazzino",
                "ripristinata la visualizzazione composizione fitofarmaci per la SchedaProdottiFitosanitariMagazzino (commentata per sbaglio).")

            '==================================

            Riga_Data("17 Febbraio 2020")

            Riga_Requisiti("Migra '576' + Aggancio '80':", "tabella per passaporto vivai")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '68' :",
                       "x esportazione universale agenda.")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("BollaAccettazione.aspx - CRBuonoAccettazione.rpt",
                    "Gestione dati nuovo conferimento F&F web.")

            '==================================

            Riga_Data("13 Febbraio 2020")

            Riga_Requisiti("Migra '576' + Aggancio '80':", "tabella per passaporto vivai")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '68' :",
                       "x esportazione universale agenda.")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("BollaAccettazione.aspx - CRBuonoAccettazione.rpt",
                    "Gestione dati nuovo conferimento F&F web WORKING PROGRESS.")

            Riga_Text("SchedaColturaleBiologico.aspx",
                    "aggiunta lav_cod 153 Rompicrosta (richiesta Agridelta) + aggiunti altri lav_cod per altre lavorazioni + gestiti altri lav_cod per uniformità a scheda di campagna, ma alcune sezioni probabile che non siano attive nel filtro stampa")

            Riga_Text("SchedaCampagna.aspx",
                 "gestite altre operazioni colturali: 152: Gebiatura,153:Rompicrosta,154:Lavorazione Combinata,159:Manutenzione Impianti,168:Pirodiserbo")

            '==================================

            Riga_Data("12 Febbraio 2020")

            Riga_Requisiti("Migra '576' + Aggancio '80':", "tabella per passaporto vivai")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '68' :",
                       "x esportazione universale agenda.")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna - report",
                    "introdotta 'andata a capo' sulla casella qta tot del sottoreport interno Trattamenti.")

            '==================================

            Riga_Data("3 Febbraio 2020")

            Riga_Requisiti("Migra '576' + Aggancio '80':", "tabella per passaporto vivai")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '68' :",
                       "x esportazione universale agenda.")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Esportatore_Universale",
                    "gestita la lettura dei principi attivi salvati sull'operazione (invece che da metaschema).")

            Riga_Text("Filtro_SchedeMagazzino_new.aspx",
                "Carica_ProdottiWS: gestito il report riepilogo prodotti utilizzati (il caricamento del menù a tendina dei prodotti non funzionava).")

            '==================================

            Riga_Data("30 Gennaio 2020")

            Riga_Requisiti("Migra '576' + Aggancio '80':", "tabella per passaporto vivai")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '68' :",
                       "x esportazione universale agenda.")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("MVV", "Modificata vista GiasMvv + aggiunte condizioni ometti su campi report")

            '==================================

            Riga_Data("29 Gennaio 2019")

            Riga_Requisiti("Migra '575' + Aggancio '80':", "tabella per passaporto vivai")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '68' :",
                       "x esportazione universale agenda.")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCatastoUtilizzi",
                  "Patch su p_ha e resa.")

            Riga_Text("Esportatore_Universale",
                    "- adeguato alle ultime modifiche che erano state fatte sulle stampe 2003 sull'esportazione agenda.")

            Riga_Text("Filtro_SchedeMagazzino_new",
                    "ImgBtn_Stampa_Click: patch su sa_cod e fabbricato_cod.")

            Riga_Text("SchedaColturaleBiologico.aspx",
            " CostiAccessori_Ottimizzata: separata gestione operatori da macchinari x patch bug presente dalla release della revisione patentini (il filtro sulla validità temporale x patentino escludeva però le macchine)")

            Riga_Text(" Passaporti",
            "chiedere a vanni")

            '==================================

            Riga_Data("27 Gennaio 2020 - Versione B")

            Riga_Requisiti("Migra '575' + Aggancio '80':", "tabella per passaporto vivai")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '67' :",
                       "x report Riepilogo Prodotti Utilizzati")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text(" Passaporti",
                    "chiedere a vanni")

            Riga_Text("Filtro_SchedeMagazzino_new.aspx",
                  "- riepilogo prodotti utilizzati: filtro opzionale su sa_cod e fabbricato_cod")

            Riga_Text("Filtro_Stampe_Conf.aspx",
                    "- Carica prodotti: BeniConfezionamentoVegetale e ProdottiConferiti, corretto filtro aggiuntivo sul codice articolo " &
                    "- correzione formato data in italiano (quando si selezionava su calendario).")

            '==================================

            Riga_Data("27 Gennaio 2020")

            Riga_Requisiti("Migra '575' + Aggancio '80':", "tabella per passaporto vivai")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '67' :",
                       "x report Riepilogo Prodotti Utilizzati")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Filtro_SchedeMagazzino_new.aspx",
                  "- modifiche grafiche alla pagina di filtro (spostate opzioni secondarie in fondo e sistemazione eventi su scelta report)" &
                    "- gestione nuovo report riepilogo prodotti utilizzati")

            Riga_Text("RiepilogoProdotti",
                    "aggiunto nuovo report RiepilogoProdottiUtilizzati")

            '==================================

            Riga_Data("24 Gennaio 2020")

            Riga_Requisiti("Migra '575' + Aggancio '80':", "tabella per passaporto vivai")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '66' :",
                       "x etichetta per passaporti vivaisti")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Etichette Passaporti", " - Creata nuova etichetta per passaporti vivaisti")

            '==================================

            Riga_Data("22 Gennaio 2020")

            Riga_Requisiti("Migra '574' + Aggancio '80':", "modifca vista GiasMvv")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '65' :",
                       "x stampa massiva reg trattamenti e reg fertilizzazioni")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Etichette Principe di Puglia", " - Creata nuova etichetta con QRCode")

            '==================================

            Riga_Data("20 Gennaio 2020")

            Riga_Requisiti("Migra '574' + Aggancio '80':", "modifca vista GiasMvv")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '65' :",
                       "x stampa massiva reg trattamenti e reg fertilizzazioni")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("MVV", " - Modifica / Aggiunta titoli alcolometri")


            '==================================

            Riga_Data("16 Gennaio 2020")

            Riga_Requisiti("Migra '572' + Aggancio '80':",
                       "nuove tabelle metaschema SpecieVegetali_XLingue, Cultivar_XLingue, GruppoFinalita_XLingue x SCHEDE QDC")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '65' :",
                       "x stampa massiva reg trattamenti e reg fertilizzazioni")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaColturaleBiologico.aspx",
                " - IRRIGAZIONE: patch, nella modalità raggruppa x intervento non veniva visualizzata la qta tot." &
                "- SEMINA con più lotti: patch, nella modalità raggruppa x intervento raddoppiava la superficie ")

            '==================================

            Riga_Data("3 Gennaio 2020")

            Riga_Requisiti("Migra '572' + Aggancio '80':",
                       "nuove tabelle metaschema SpecieVegetali_XLingue, Cultivar_XLingue, GruppoFinalita_XLingue x SCHEDE QDC")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '65' :",
                       "x stampa massiva reg trattamenti e reg fertilizzazioni")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Fattura_NotaAccredito.aspx - stampa Ordine",
                "sistemata stampa del telefono/cell quando era impostata una destinazione diversa rispetto al cliente")

            '==================================

            Riga_Data("19 dicembre 2019 BIS")

            Riga_Requisiti("Migra '572' + Aggancio '80':",
                       "nuove tabelle metaschema SpecieVegetali_XLingue, Cultivar_XLingue, GruppoFinalita_XLingue x SCHEDE QDC")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '65' :",
                       "x stampa massiva reg trattamenti e reg fertilizzazioni")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Sottoreport fertilizzanti ",
                "Allargamento colonne")

            '==================================

            Riga_Data("19 dicembre 2019")

            Riga_Requisiti("Migra '572' + Aggancio '80':",
                       "nuove tabelle metaschema SpecieVegetali_XLingue, Cultivar_XLingue, GruppoFinalita_XLingue x SCHEDE QDC")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '65' :",
                       "x stampa massiva reg trattamenti e reg fertilizzazioni")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampa etichette ",
                "Correzione sulla query")

            Riga_Text("Sottoreport fertilizzanti ",
                    "Allargamento colonne")

            '==================================

            Riga_Data("18 dicembre 2019")

            Riga_Requisiti("Migra '572' + Aggancio '80':",
                       "nuove tabelle metaschema SpecieVegetali_XLingue, Cultivar_XLingue, GruppoFinalita_XLingue x SCHEDE QDC")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '65' :",
                       "x stampa massiva reg trattamenti e reg fertilizzazioni")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampa etichette ",
                "Correzione sulla query")

            '==================================

            Riga_Data("14 dicembre 2019")

            Riga_Requisiti("Migra '572' + Aggancio '80':",
                       "nuove tabelle metaschema SpecieVegetali_XLingue, Cultivar_XLingue, GruppoFinalita_XLingue x SCHEDE QDC")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '65' :",
                       "x stampa massiva reg trattamenti e reg fertilizzazioni")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("AgronicaCoreStampeDAL.FreshAndFood.StampaBolla",
                  "StampaBolla: modifica per gestire la stampa della bolla di conferimento web:" &
                "AND Mov_Raccolta.Cau_Mov    in ( '4070', '7300')" &
                "AND Mov_Dett_Raccolta.elem_cod=210.")

            Riga_Text("Stampe F&F",
                "AgronicaCoreStampeDAL.ConferimentoAccettazione.vb: modifica query conferimento per convivenza F&F OLD con F&F NEW WEB.")

            Riga_Text("Report costi / ricavi",
                        "Correzione errore su PIVA senza apici")

            '==================================

            Riga_Data("10 Dicembre 2019")

            Riga_Requisiti("Migra '572' + Aggancio '80':",
                       "nuove tabelle metaschema SpecieVegetali_XLingue, Cultivar_XLingue, GruppoFinalita_XLingue x SCHEDE QDC")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '65' :",
                       "x stampa massiva reg trattamenti e reg fertilizzazioni")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampe Statistiche",
                  "Bug fix per imballi (no F&F).")

            '==================================


            '==================================

            Riga_Data("6 Dicembre 2019")

            Riga_Requisiti("Migra '572' + Aggancio '80':",
                       "nuove tabelle metaschema SpecieVegetali_XLingue, Cultivar_XLingue, GruppoFinalita_XLingue x SCHEDE QDC")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '65' :",
                       "x stampa massiva reg trattamenti e reg fertilizzazioni")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                  "Riesumazione reg trattamenti e reg fertilizzazioni massivo con gestione log e migliorie path di salvataggio pdf.")

            Riga_Text("SchedaColturaleBiologico.aspx",
               "gestione stampa del lotto nelle semine e nelle raccolte tramite due impostazioni utente.")

            '==================================

            Riga_Data("5 Dicembre 2019")

            Riga_Requisiti("Migra '572' + Aggancio '80':",
                       "nuove tabelle metaschema SpecieVegetali_XLingue, Cultivar_XLingue, GruppoFinalita_XLingue x SCHEDE QDC")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                  "CaricaDsErbacee, CaricaDsImpianti_Multispecie: introdotta lettura in lingua di specie e cultivar")

            '=================================

            Riga_Data("20 Novembre 2019")

            Riga_Requisiti("Migra '560' :",
                       "Per modifica vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Doc Contabili",
                  "patch su chkfittizio NULL (ad esempio stampa di conferimento ddt uva)")

            Riga_Text("SchedaCampagna.aspx",
                  "CaricaDsSemine: evitato di svuotare cul_des (che in realtà contiene il mat_des se ci sono le materie prime movimentate), così si vedono tutte le varietà di sementi/piantine utilizzate")

            '=================================

            Riga_Data("12 Novembre 2019 Versione B")

            Riga_Requisiti("Migra '560' :",
                       "Per modifica vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaColturaleBiologico.aspx",
                  "- gestita la lettura dell'organismo di controllo con stampa su intestazione" &
                  "- migliorata gestione log degli errori")

            '=================================

            Riga_Data("12 Novembre 2019")

            Riga_Requisiti("Migra '560' :",
                       "Per modifica vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaColturaleBiologico.aspx ",
                    "stampata la Regione selezionata nel filtro stampa (prima stampava la regione dell'indirizzo dell'impresa)")

            Riga_Text("Filtro_SchedeBiologico.aspx",
                    "scheda materie prime e vendite: nei filtri aggiunta anche la selezione della regione di riferimento (con selezione di default da indirizzo del centro)")

            Riga_Text("SchedaMateriePrime e SchedaVendite",
                    "- gestita la lettura dell'organismo di controllo con stampa su intestazione" &
                    "- gestita la stampa della regione da selezione avvenuta su filtro stampa")

            Riga_Text("AnagraficaContatti_XLS.aspx - Filtro_Contatti.aspx",
                    "- Attivata esportazione dei dati del patentino (numero, data rilascio e data scadenza) nuova modalità." &
                    "- Gestito il filtro di visibilità contatti sul centro aziendale.")

            '=================================

            Riga_Data("5 Novembre 2019")

            Riga_Requisiti("Migra '560' :",
                       "Per modifica vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.aspx,DocumentiContab.vb, Fattura_NotaAccredito.aspx, Pagamenti.aspx ",
                    "Gestione CHKFITTIZIO sui contatti (di tutti i tipi) nelle varie sezioni del ddt e fattura." &
                "I clienti esteri non hanno più gestione particolare sulla stampa o meno del codice VAT.")

            '=================================

            Riga_Data("28 Ottobre 2019")

            Riga_Requisiti("Migra '560' :",
                       "Per modifica vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Filtro_StampeCantine.aspx & ConsistenzeEnologiche.aspx",
                    "introdotto filtro sul lotto: contiene/non contiene il testo specificato (richiesta di RUGGERI per filtro SQNPI).")

            '=================================

            Riga_Data("18 Ottobre 2019")

            Riga_Requisiti("Migra '560' :",
                       "Per modifica vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                    "CaricaDsImpianti_Multispecie: patch quando veniva aggiunta la buffer al nome appezza nel caso di arboree e uso non agr.")

            '==================================

            Riga_Data("17 Ottobre 2019")

            Riga_Requisiti("Migra '560' :",
                       "Per modifica vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Registro aziendale unico - Rpt_CopertinaLombardia.rpt",
                    "modifica gestione loghi sulla copertina")

            Riga_Text("Selezione_SchedaCampagna.aspx",
                    "registro aziendale unico: check stampa logo regione selezionato di default")

            Riga_Text("DocumentiContab.vb",
            "Leggi_Indirizzi: aggiunto controllo su IndirizzoTipoDesc e gestito anche nome/cognome del contatto")

            Riga_Text("SchedaColturaleBiologico.aspx",
            "introdotto controllo perché le operazioni create fino ad una certa del 2018 non hanno i nuovi campi (dose_tot_reale, ecc) valorizzati, per cui non venivano stampate le qta")

            '==================================

            Riga_Data("09 Ottobre 2019")

            Riga_Requisiti("Migra '560' :",
                       "Per modifica vista GiasMVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampa MVV",
              "modifiche a descrizione dettaglio prodotti come da richieste")

            '==================================

            Riga_Data("27 Settembre 2019 - Versione B")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
              "modifiche alla GESTIONE COSTI ACCESSORI (manodopera e macchina conseguente a nuova gestione PATENTINO)")

            '==================================

            Riga_Data("27 Settembre 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
              "fix gestione patentino che non mostrava le macchine")

            '==================================

            Riga_Data("24 Settembre 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Esportazione_AnagraficaContatti",
                    "- sistemata esportazione del tipo indirizzo default" &
                    "- patch su Controllo_Selezione_Check" &
                    "- disattivata esportazione del patentino (da attivare se e quando richiesta)")

            Riga_Text("SchedaCampagna.aspx",
              "NUOVA GESTIONE PATENTINI")

            '==================================

            Riga_Data("17 Settembre 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx e SchedaColturaleBiologico.aspx",
                    "NUOVA GESTIONE PATENTINI")

            '==================================

            Riga_Data("13 Settembre 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                    "- RACCOLTE: non utilizzata l'udm_sim del dataset, ma concatena l'udm alla qta" &
                "- TRATTAMENTI: erano state fatte modifiche per utilizzare i nuovi campi salvati dal data entry del trattamento (giasonline)," &
                "ma non si possono usare perché il giaslan non li salva (commentati i pezzi)")

            '==================================

            Riga_Data("10 Settembre 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaColturaleBiologico",
                "- introdotta colonna con la sup. trattata" &
                "- nel caso di catture di massa e install. trappole, utilizzato il campo qta di mov_destinazioni (come per la confusione sessuale), anziché fare la ripartizione della qta tot sull'impianto")

            Riga_Text("PianoColturaleCatasto_XLS.aspx",
                    "patch, dal 27 Ottobre 2017 l'organismo referente è un contatto, quindi ora prende la rag_soc dalla tabella contatti (invece che imprese)")

            '==================================

            Riga_Data("9 Settembre 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaColturaleBiologico",
                "PATCH: corretto raggruppamento perché se c'era lo stesso tipo di operazione nella stessa data, raggruppava insieme e moltiplicava la superficie (all'interno della stessa op di agenda non si deve sommare più volte la sup di uno stesso impianto)")

            '==================================

            Riga_Data("29 Agosto 2019 - Versione B")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("PianoColturaleCatasto_XLS.aspx",
                "gestita l'esportazione degli impianti senza catasto (prima venivano scartati)")

            '==================================

            Riga_Data("29 Agosto 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("PianoColturaleCatasto_XLS.aspx",
                "PianoColturale_Excel_ImpiantiConOperazioni - PRIMA PARTE: IMPIANTI CON SPECIE CON RACCOLTE:" &
                "introdotto LEFT OUTER JOIN Materie_Prime per gestire le RACCOLTE FAST che non hanno il prodotto selezionato")

            '==================================

            Riga_Data("28 Agosto 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampe Statistiche",
                "Aggiunta visualizzazione e raggruppamento per Rapporto Contabile")

            '==================================

            Riga_Data("20 Agosto 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaColturaleBiologico.aspx",
                "- CodiceAppezzamento: N_appezza double per evitare errore di overflow." &
                "- Carica_DSSchedaColturaleBiologico: gestiti i NULL su cul_cod e veg_cod e introdotta la lettura della destinazione d'uso.")

            '==================================

            Riga_Data("19 Agosto 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Filtro_SchedeBiologico.aspx",
                "filtro linee sul report preparazioni bio: escluse le linee la cui anagrafica (selezionata nel menù a tendina) era collegata alla linea ma disattivata.")

            '==================================

            Riga_Data("13 Agosto 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("PianoColturaleCatasto",
                " aggiunte colonne rif. appezzamento, cod. particella, cod. impianto e cod. esercizio")

            '==================================

            Riga_Data("7 Agosto 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaColturaleBiologico",
                " nel caso di 'Raggruppa per Intervento' gestita la stampa della qta totale del prodotto per quell'intervento non come somma delle qta delle righe (come faceva prima) ma leggendo la qta totale salvata su db e facendo l'equivalenza per portarla kg/l")

            '==================================

            Riga_Data("2 Agosto 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("GestioneRichieste.aspx",
                "gestito link stampa enum_CodificaStampe.EsportazionePomodoroIndustriaOINordItalia")

            Riga_Text("CatastoOIPomodoroIndustriaNordItalia_XLS.aspx",
                "nuovo report Excel x AINPO (legge gli impianti e incrocia le codifiche con la tabella Codifica_Varieta_OIPomodorodaIndustriaNordItalia)")

            '==================================

            Riga_Data("30 Luglio 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Filtro_StampeCantine.aspx",
                "Verifica_Operazioni_Pianificate - Leggi_Operazioni_Pianificate: gestita anche la lettura delle operazioni di cantina.")

            Riga_Text("VisualizzatoreReport.aspx",
                "gestita la forzatura dell'apertura dell'anteprima crystal indipendentemente dall'impostazione salvata sull'utente" &
                "(si passa in querystring ForzaAnteprima = true)")

            Riga_Text("Filtro_ElaboratiContabili.aspx",
                 "- cambiata grafica del titolo (blu scuro / bianco)" &
                "- nel caso di stampa con anteprima crystal sostituita querystring con ForzaAnteprima" &
                "- aggiunto controllo sulla chiave  Flag_Stampe_Nuovi_Arrotondamenti  (letta dal config_siti): se è false viene mandato messaggio di allerta.")

            Riga_Text("tutti i report contab",
                "gestita la forzatura dell'apertura dell'anteprima crystal indipendentemente dall'impostazione salvata sull'utente" &
            "(si passa in querystring ForzaAnteprima = true), se dalla pagina di filtro stampe è stata cliccata l'icona della stampante")

            '==================================

            Riga_Data("29 Luglio 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("PianoColturaleCatasto_XLS.aspx",
                "Patch nel caso di doppia data di semina.")

            Riga_Text("SchedaCampagna.aspx",
                "CaricaDsFasiFenologiche: aggiunto un DsFasiFenologiche.FasiFenologiche.AcceptChanges() alla fine della funzione" &
                "(altrimenti, nel caso di fioriture previste, venivano fuori dati sballati).")

            '==================================

            Riga_Data("29 Luglio 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("PianoColturaleCatasto_XLS.aspx",
                "Patch nel caso di doppia data di semina.")

            Riga_Text("SchedaCampagna.aspx",
                "CaricaDsFasiFenologiche: aggiunto un DsFasiFenologiche.FasiFenologiche.AcceptChanges() alla fine della funzione" &
                "(altrimenti, nel caso di fioriture previste, venivano fuori dati sballati).")

            '==================================

            Riga_Data("23 Luglio 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampe F&F",
                "non serve filtrare contemporaneamente cod_contatto e fare il like su rag_soc (il cod_contatto è più importante);" &
                "modificato cmq per evitare il problema di report vuoto nel caso di contatti con lettere accentate nella rag_soc" &
                "(la pagina di pre stampa elimina le lettere accentate nel passaggio via querystring, di conseguenza il filtro per ra_soc non procura risultato).")

            '==================================

            Riga_Data("18 Luglio 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaColturaleBiologico.aspx",
                "- Colonna note: dal secondo dettaglio in poi della stessa operazione (modalità raggruppaXintervento)," &
                "visualizzato '* Attenzione, la presente riga è da intendersi in miscela con la precedente (stessa data, stessa botte)' (x concimazioni e trattamenti)" &
                "e '' per le altre operazioni" &
                "- sistemato layout.")

            '==================================

            Riga_Data("17 Luglio 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Report vendite",
                  "Estrazione MVV")

            Riga_Text("PianoColturaleCatasto_XLS.aspx",
               "sup [ha]: visualizzata la sup_imp solo nel primo record (nelle successive ripetizioni viene messo 0)")


            '==================================

            Riga_Data("16 Luglio 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("PianoColturaleCatasto_XLS.aspx",
                  "- corretto bug che, nel caso di assenza organismo referente, stampava la prima azienda della tabella imprese" &
                    "- nuovo sviluppo: nel caso di assenza catasto sull'appezzamento viene stampato il catasto del campo relativo senza visualizzare la superficie di intersezione")

            '==================================

            Riga_Data("12 Luglio 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("CRBolla_Cofruta.rpt ",
                  "invertito ordine colonne U.M e Qta")

            '==================================

            Riga_Data("11 Luglio 2019")

            Riga_Requisiti("Migra '550' :",
                       "Per stampa DDT")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.aspx - CRBolla_Cofruta.rpt: ",
                  "gestita personalizzazione per MASSEI (che si parametrizza su tabella configurazione_stampe): aggiunta colonna n. colli (campo dataset Extra_Str_5)")

            '==================================

            Riga_Data("05 Luglio 2019")

            Riga_Requisiti("Migra '546' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("MVV: ",
                  "Bug Fix")

            '==================================

            Riga_Data("02 Luglio 2019")

            Riga_Requisiti("Migra '546' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("MVV", "Bug Fix")

            '==================================

            Riga_Data("28 Giugno 2019")

            Riga_Requisiti("Migra '546' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("MVV", "Modifiche come da ultima revisione")

            '==================================

            Riga_Data("27 Giugno 2019")

            Riga_Requisiti("Migra '538' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                    "CaricaDsCatasto: modifica 3° query su CampiXParticelle per richiesta di AINPO," &
                    "nell'incrocio catasto sul campo non visualizzare nome appezzamento, varietà e superficie," &
                    "visualizzare solo nome campo con particella e sup intersecata")

            '==================================

            Riga_Data("25 Giugno 2019")

            Riga_Requisiti("Migra '538' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                    "Aggiunta opzione per mostrare o nascondere i valori non significativi nei rilievi (default mostra tutto)")

            '==================================

            Riga_Data("17 Giugno 2019 - Versione C")

            Riga_Requisiti("Migra '538' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaMateriePrime BIO",
                    "- layout: modificato per allargare le colonne delle qta e sitemare la chiusura della tabella nel fine pagina." &
                    "- gestito il tipo di arrotondamento sulle Quantità." &
                    "- colonna indirizzo: evitata la stampa di '&nbsp;' e 'Non Definita (00)'")

            Riga_Text("Filtro_SchedeBiologico.aspx",
                    "Aggiunta opzione per la selezione del tipo di arrotondamento nella scheda materie prime (default 2 decimali).")

            Riga_Text("Esportazione_AnagraficaContatti",
                "- pagina di avvio stampa: modificati i colori" &
                "- gestita l'esportazione della PEC e del codice SDI")

            '==================================

            Riga_Data("17 Giugno 2019")

            Riga_Requisiti("Migra '538' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Statistiche",
                  "Modifiche per gestione VALORE DA CONSIDERARE 0 Kg/Lt - Pezzi/Nr e per controlli con prodotti che hanno U.M. di vendita diverse a seconda del cliente")

            '==================================

            Riga_Data("10 Giugno 2019")

            Riga_Requisiti("Migra '538' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Reg. Trattamenti Veneto unico (Rpt_RegistroTrattamentiVenetoUnico.rpt)",
                  "Rpt_TrattamentiVenetoExtraAgricolo2.rpt: aggiunta colonna n.ro app. / note (come era nel report SCHEDA C singolo)")

            '==================================

            Riga_Data("07 Giugno 2019 - Versione B")

            Riga_Requisiti("Migra '538' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("DAA",
                  "Stampe DAA - Modifiche")

            '==================================

            Riga_Data("07 Giugno 2019")

            Riga_Requisiti("Migra '538' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("MVV",
                  "Aggiornamento Stampa modello MVV")

            Riga_Text("Rpt_RegistroTrattamentiVenetoUnico.rpt e Rpt_TrattamentiVenetoExtraAgricolo2.rpt",
                "Reg. Trattamenti Veneto unico - Scheda C: rinominata colonna in 'Nome di chi effettua il trattamento (*) / Note'")

            '==================================


            Riga_Data("28 Maggio 2019")

            Riga_Requisiti("Migra '536' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Fattura_NotaAccredito.aspx",
                  "gestita l'aggiunta della dicitura Sconto Merce/campione gratuito/Campione Omaggio senza rivalsa IVA/Campione Omaggio con rivalsa IVA come avveniva già nella stampa DDT")

            Riga_Text("Statistiche",
                "Ordinamento per Valore")

            '==================================

            Riga_Data("27 Maggio 2019")

            Riga_Requisiti("Migra '536' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '62' :",
                       "x stampa statistiche")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna",
                  "CaricaDsDichiarazioni: correzione nel caso di stampa schede multispecie, altrimenti le nuove dichiariazioni salvate su specie/impianto non venivano stampate.")

            Riga_Text("DocumentiContab.vb - RicevutaFiscale_GestioneStampa.vb",
                    "aggiornata dicitura SCONTO MERCE su richiesta di BORGOLUCE")

            Riga_Text("Statistiche",
                    "Stampe Statistiche Tipo1 e 3 - DONE")

            '==================================

            Riga_Data("22 Maggio 2019")

            Riga_Requisiti("Migra '536' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '60' :",
                       "x aggiunta enum_stampe PianoColturale (39) e PianoColturaleCatasto (166)")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Selezione_SchedaCampagna.aspx",
                  "aggiunto flag 'Stampa anno impianto colture pluriennali' (di default selezionato).")

            Riga_Text("SchedaCampagna.aspx",
                "- x tutti i report: aggiornato algoritmo di visualizzazione data inizio impianto arboree" &
                "- CaricaDsConcimazioni: aggiunta dicitura '* Attenzione, la presente riga è da intendersi in miscela con la precedente (stessa data, stessa botte). dalla seconda riga in poi della stessa concimazione (come già avveniva nei trattamenti, richiesta COLDI CUNEO)")

            Riga_Text("Rpt_SchedaCampagna_EUREP_GAP_Multicentro.rpt",
               "sistemato il layout, allineando le larghezze di tutti i sottoreport.")

            '==================================

            Riga_Data("14 Maggio 2019")

            Riga_Requisiti("Migra '536' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '60' :",
                       "x aggiunta enum_stampe PianoColturale (39) e PianoColturaleCatasto (166)")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampa MVV",
                  "Aggiunte ulteriori informazioni")

            '==================================

            Riga_Data("2 Maggio 2019")

            Riga_Requisiti("Migra '532' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '60' :",
                       "x aggiunta enum_stampe PianoColturale (39) e PianoColturaleCatasto (166)")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                  "CaricaDsCatasto: gestita la stampa delle particelle intersecate dal campo qualora i suoi appezzamenti non abbiano catasto.")

            '==================================

            Riga_Data("30 Aprile 2019")

            Riga_Requisiti("Migra '532' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '60' :",
                       "x aggiunta enum_stampe PianoColturale (39) e PianoColturaleCatasto (166)")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Esportazione_OP_Catasto.aspx",
                  "patch nel caso di veg_cod o cul_cod non valorizzati.")

            Riga_Text("PianoColturale_XLS.aspx",
                "porting da AgronicaStampe + patch su sup. intersezione particelle campo (ora non visualizza nulla).")

            Riga_Text("PianoColturaleCatasto_XLS.aspx",
                "porting da AgronicaStampe.")

            Riga_Text("SchedaColturaleBiologico.aspx",
                "- stampato mat_des su SEMILAVORATI_VEGETALI, TRASFORMATI_VEGETALI per gestire la stampa di 'fiori di zucca' nella raccolta dello zucchino" &
                "- nella sezione RACCOLTA: gestito round all'intero nel caso di udm = numero (ad esempio fiori di zucca)")

            '==================================

            Riga_Data("17 Aprile 2019")

            Riga_Requisiti("Migra '532' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '59' :",
                       "x aggiunta enum_stampe ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                  "patch nella preparazione della strFF_Cod_BBCH (la data_fioritura non funzionava nelle schede multispecie)")

            '==================================


            Riga_Data("15 Aprile 2019")

            Riga_Requisiti("Migra '532' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '59' :",
                       "x aggiunta enum_stampe ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaColturaleBiologico.aspx",
                  "RACCOLTA: prima stampava sempre udm = Q.li e sommava la qta solo se l'udm era kg -> sbloccato, nel caso di udm <> kg stampa l'udm selezionata e calcola la qta")

            '==================================

            Riga_Data("12 Aprile 2019")

            Riga_Requisiti("Migra '532' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '59' :",
                       "x aggiunta enum_stampe ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Schede_OP",
                  "Rpt_AttoNotorio.rpt: rimosso OP APOFRUIT")

            Riga_Text("Fattura/DDT",
                  "Stampa dei litri sul dettaglio solo quando ChKLayOut_Litri e udm Litri")

            Riga_Text("SchedaCampagna",
                "CaricaDsCentriAziendali: aggiunto filtro per scartare centri dismessi che non rientrano nel range di stampa (ma il cui sa_cod è presente nella stringa di filtro centri)")

            Riga_Text("StampaMVV.aspx",
              "Fix MVV.rpt - Messo come file contenuto e non risorsa (PATCH)")

            '==================================

            Riga_Data("09 Aprile 2019")

            Riga_Requisiti("Migra '532' :",
                       "Per stampa modello MVV")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '59' :",
                       "x aggiunta enum_stampe ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaTracciabilita.aspx",
                  "CaricaDs_Acquisti: patch nel caso di dati non presenti nell'intervallo e ottimizzazione")

            Riga_Text("StampaMVV.aspx",
                 "Stampa modello MVV")

            Riga_Text("Esportatore_Universale_2.aspx",
                  "Aggiunto centro aziendale per esportazione in XML per Agribologna")

            '==================================

            Riga_Data("05 Aprile 2019")

            Riga_Requisiti("Migra '530' :",
                       "per traduzione fattura")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '57' :",
                       "x aggiunta enum_stampe ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Esportatore_Universale_2.aspx",
                  "Gestito numero piante + gruppo varietale per esportazione in XML per Agribologna")

            '==================================

            Riga_Data("04 Aprile 2019")

            Riga_Requisiti("Migra '530' :",
                       "per traduzione fattura")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '57' :",
                       "x aggiunta enum_stampe ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampe di campagna varie",
                  "Modificato il modulo per le dichiarazioni di non utilizzo includendo specie e appezzamenti, qualora specificati")

            Riga_Text("Rpt_SchedaCampagna_EUREP_GAP_Multicentro.rpt",
            "sistemata dicitura in GLOBALG.A.P..")

            '==================================

            Riga_Data("27 Marzo 2019")

            Riga_Requisiti("Migra '530' :",
                       "per traduzione fattura")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '57' :",
                       "x aggiunta enum_stampe ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Borgoluce",
                  "Riepiloghi Accise con anteprima dati riepilogo.")

            Riga_Text("Rpt_SchedaCampagna_EUREP_GAP_Multicentro.rpt",
                    "sistemata intestazione di pagina dalla seconda in poi.")

            Riga_Text("Fattura_NotaAccredito.aspx",
                    "Gestione IVA SPLIT PAYMENT.")

            Riga_Text("Registri_IVA_3.aspx",
              "Gestione IVA SPLIT PAYMENT.")

            Riga_Text("LiquidazioneIVA_Anteprima_3.aspx",
              "Gestione IVA SPLIT PAYMENT ---> VA FINITA DI GESTIRE NEL RIEPILOGO.")

            '==================================

            Riga_Data("26 Marzo 2019")

            Riga_Requisiti("Migra '530' :",
                       "per traduzione fattura")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '57' :",
                       "x aggiunta enum_stampe ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Global-Gap:",
                  "- nella sezione Trattamenti aggiunte in fondo firma del tecnico e firma dell'operatore (richiesta CICO mazzoni)" &
                "- sistemati graficamente i sottoreport delle concimazioni e dei trattamenti" &
                "- reimportati sottoreport di altre sezioni per sistemazioni grafiche.")

            Riga_Text("Scheda di Campagna MultiCentro",
                    "sistemati graficamente i sottoreport delle concimazioni e dei trattamenti.")

            '==================================

            Riga_Data("25 Marzo 2019")

            Riga_Requisiti("Migra '530' :",
                       "per traduzione fattura")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '57' :",
                       "x aggiunta enum_stampe ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Fattura:",
                  "Sviluppo gestione della traduzione in lingua straniera.")


            '==================================

            Riga_Data("20 Marzo 2019")

            Riga_Requisiti("Migra '519' :",
                       "per buffer nella scheda di campagna")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '57' :",
                       "x aggiunta enum_stampe ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("DAA:",
                  "Report Garanzie Circolanti.")

            '==================================

            Riga_Data("18 Marzo 2019")

            Riga_Requisiti("Migra '519' :",
                       "per buffer nella scheda di campagna")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '57' :",
                       "x aggiunta enum_stampe ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Registro Corrispettivi: ",
                  "Corretta query RegistroCorrispettivi_ReleaseArrotondamenti2019 per ambiguità Data_Movimento.")

            Riga_Text("Fattura / DDT:",
                  "Eliminazione loghi Il Pratello.")

            '==================================

            Riga_Data("15 Marzo 2019 - versione B")

            Riga_Requisiti("Migra '519' :",
                       "per buffer nella scheda di campagna")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '57' :",
                       "x aggiunta enum_stampe ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("RegistroPreparazioniBio:",
                  "sezione B - lavorazioni (operazioni agenda): nelle note stampato l'ingrediente principale con il lotto e i litri/kg impiegati.")

            Riga_Text("Contabilita - varie pagine:",
                    "chiamata la nuova funzione Ricava_Piva_Codicefiscale (per gestire correttamente la stampa della piva o del codice fiscale sulle varie tipologie di contatto) con passaggio dell'id_cf.")

            '==================================

            Riga_Data("15 Marzo 2019")

            Riga_Requisiti("Migra '519' :",
                       "per buffer nella scheda di campagna")

            Riga_Requisiti("GiasBase '14 Marzo 2019' :",
                       "per centralizzazione file bootstrap_AGRONICA.css")

            Riga_Requisiti("Configurazione_Siti '57' :",
                       "x aggiunta enum_stampe ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Filtro_ElaboratiContabili.aspx:",
                  "aggiunta Database_VerificaSezionaleContiVSDocumenti per la verifica del sezionale sui documenti rispetto a quello che è impostato sui suoi conti.")

            Riga_Text("Fattura / Ordine / DDT:",
                  "aggiunta stampa del grado alcolico all'interno della descrizione prodotto, quando attivata l'opzione")

            Riga_Text("StampeBootstrap_Master.css:",
                  "Uniformata larghezza container come in master agenda")

            Riga_Text("Riepiloghi Accise:",
                  "Aggiunto report Garanzie Circolanti")

            '==================================

            Riga_Data("07 Marzo 2019")

            Riga_Requisiti("Migra '519' :",
                       "per buffer nella scheda di campagna")

            Riga_Requisiti("GiasBase '25 Gennaio' :",
                       "per ???")

            Riga_Requisiti("Configurazione_Siti '55' :",
                       "x libro conferimenti excel ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampa fatture / ordini / ddt:", "Aggiunta INCOTERMS  per richiesta Borgoluce")

            '==================================

            Riga_Data("06 Marzo 2019")

            Riga_Requisiti("Migra '519' :",
                       "per buffer nella scheda di campagna")

            Riga_Requisiti("Configurazione_Siti '55' :",
                       "x libro conferimenti excel ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Contabilita - Fattura - Bolla - RicevutaFiscale:",
                  "- aggiornato il testo sulla privacy." &
                  "- modificato il piè di pagina (scritta a fianco al logo Agronica).")

            '==================================

            Riga_Data("28 Febbraio 2019")

            Riga_Requisiti("Migra '519' :",
                       "per buffer nella scheda di campagna")

            Riga_Requisiti("Configurazione_Siti '55' :",
                       "x libro conferimenti excel ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna:",
                  "- fix su finalità e capitolato privato che mostravano dati sbagliati.")

            Riga_Text("BrogliaccioMovimentiTabella:",
                 "Aggiunta colonna del centro aziendale.")

            '==================================

            Riga_Data("25 Febbraio 2019")

            Riga_Requisiti("Migra '519' :",
                       "per buffer nella scheda di campagna")

            Riga_Requisiti("Configurazione_Siti '55' :",
                       "x libro conferimenti excel ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("LibroConferimenti:",
                  "- aggiunta cartella LibroConferimenti con trasloco stampe LibroConferimenti PDF ed Excel (da stampe 2003) + sviluppo richiesto (aggiunta colonna peso effettivo in entrambi i report)." &
                  "(alla fine la stampa PDF non viene linkata perché la colonna nuova la vogliono solo sull'Excel, continua ad essere chiamata la vecchia).")

            '==================================

            Riga_Data("21 Febbraio 2019 - Versione B")

            Riga_Requisiti("Migra '519' :",
                       "per buffer nella scheda di campagna")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Mastrino:",
                  "- modificato il layout al nuovo standard grafica." &
                  "- ripristinata la stampa in intestazione del sezionale selezionato nella pagina di filtro")

            Riga_Text("Filtro_ElaboratiContabili.aspx:",
                    "aggiunto alert post verifica sezionale documenti e sezionale sulla relativa contabilizzazione.")

            '==================================

            Riga_Data("21 Febbraio 2019")

            Riga_Requisiti("Migra '519' :",
                       "per buffer nella scheda di campagna")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("ControlloGestione:",
                    "correzione sul report costi .")

            '==================================

            Riga_Data("18 Febbraio 2019")

            Riga_Requisiti("Migra '519' :",
                       "per buffer nella scheda di campagna")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Registro Trattamenti Veneto (std. condizionalità) :",
                  "rilasciato nuovo report.")

            '==================================

            Riga_Data("12 Febbraio 2019")

            Riga_Requisiti("Migra '519' :",
                       "per buffer nella scheda di campagna")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("FormAggiornaImportoNEW :",
                  "BugFix per colonna iva indetraibile presente solo se si arriva da liquidazione che impediva la stampa corretta dell'iva sul singolo documento")

            '==================================

            Riga_Data("11 Febbraio 2019")

            Riga_Requisiti("Migra '519' :",
                        "per buffer nella scheda di campagna")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("RegistroCorrispettiviNew.aspx e AgronicaCoreStampeDAL.RegistriContab.RegistroCorrispettivi_ReleaseArrotondamenti2019 :",
                  "REMAKE REGISTRO CORRISPETTIVI sulla legge 2019 degli arrotondamenti.")

            '==================================

            Riga_Data("8 Febbraio 2019")

            Riga_Requisiti("Migra '519' :",
                        "per buffer nella scheda di campagna")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("RisultatoAnalisiConformita :",
                  "aggiunta indicazione del centro nelle operazioni e gestito in testata il multi-centro")

            Riga_Text("DDT_BolleConf.aspx e Fattura_NotaAccredito.aspx :",
                  "DocContab.DocumentiContabili: patch sui campi aggiunti nella versione precedente, in alcuni casi sono salvati NULL e quindi generavano errore di cast.")

            Riga_Text("Rpt_SchedaCampagna_EUREP_GAP_Multicentro.rpt:",
                     "- sistemata label tecnico di riferimento." &
                     "- sistemato allineamento label frontespizio" &
                     "- aggiunta andata a capo sulla colonna lotto prodotto")

            '==================================

            Riga_Data("4 Febbraio 2019")

            Riga_Requisiti("Migra '519' :",
                        "per buffer nella scheda di campagna")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.aspx e Fattura_NotaAccredito.aspx :",
                  "modificata gestione del Rif_Ordine (se specificato quello del cliente, prende la precedenza rispetto al numero ordine Gias)")

            '==================================

            Riga_Data("30 Gennaio 2019")

            Riga_Requisiti("Migra '519' :",
                        "per buffer nella scheda di campagna")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.aspx e Fattura_NotaAccredito.aspx :",
                  "modifica alla gestione della destinazione diversa (se il contatto destinazione è diverso dal contatto cliente oltre al tipo indirizzo viene stampata anche la rag_soc del contatto destinazione) -> leggi_indirizzi")

            Riga_Text("AgronicaCoreStampeDAL.RegistriCantina.BrogliaccioMovimenti :",
                "Patch: nelle query interne, nei case dei Confezionati, considerati anche i bag in box (enum_Omni_Tipo_Generazione.BagInBox = 21).")

            '==================================

            Riga_Data("28 Gennaio 2019")

            Riga_Requisiti("Migra '519' :",
                        "per buffer nella scheda di campagna")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Schede_OP:",
            " AgronicaCoreAnagrafeDAL.Imprese.DatiIntestazioneSocio" &
            "- aggiunte le date di validità del legale rappresentante per gestire il cambio del legale sull'azienda Terre da Frutta di Terremerse" &
            "- aggiunta lettura della data_nascita" &
            "- aggiunto ordinamento per visualizzare l'ultimo legale rappresentante attivo")

            Riga_Text("SchedaColturaleBiologico :",
          "- Rilievi Avversita' in Campo: aggiunta gestione erbe infestanti (119)" &
        "- spostati i lav_cod 118,121 da sezione Trappole a sezione Trattamenti")

            '==================================

            Riga_Data("24 gennaio 2019")

            Riga_Requisiti("Migra '519' :",
                       "per buffer nella scheda di campagna")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("RisultatoAnalisiConformita :",
                  "aggiunta gestione dell'opzione di modalità di stampa (diretta su PDF o con anteprima)")

            '==================================

            Riga_Data("23 gennaio 2019")

            Riga_Requisiti("Migra '519' :",
                       "per buffer nella scheda di campagna")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Filtro_ElaboratiContabili.aspx",
                  "controllo sull'imputazione dell'anno contabile: adeguata query mettendo il controllo su chkcoge_manuale = CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA")

            Riga_Text("DDT_BolleConf.aspx e Fattura_NotaAccredito.aspx",
                "gestito IndirizzoTipoDesc (la descrizione della tipologia indirizzo personalizzata).")

            '==================================

            Riga_Data("16 Gennaio 2019")

            Riga_Requisiti("Migra '519' :",
                       "per buffer nella scheda di campagna")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna :",
                  "- nell'elenco appezzamenti aggiunta indicazione se l'appezzamento confina con buffer" & vbCrLf &
                  "- nei trattamenti aggiunta eventuale indicazione sup_ridotta e mitigazione deriva" & vbCrLf &
                  "- allargata colonna appezzamenti nelle schede multicentro")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti.Query_Conti_Patrimoniali_Movimentati: ",
                  "PATCH SULLE VERSIONI DAL 14/01" &
                "Non andava aggiunto il lav_cod fattura professionisti alla 14° parte della Union, " &
                "ma andava patchata la 19° parte (PAGAMENTO FATTURE - GESTIONE RITENUTA ACCONTO ED ENASARCO (CoGe Auto))" &
                "era rimasto x sbaglio il filtro su Agenda.chkcoge_manuale invece che su Pagamenti.ChkCoge_Manuale_Pagamenti")

            '==================================

            Riga_Data("15 Gennaio 2019")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("GestioneRichieste.aspx ",
                  "sezione di lettura impostazione superuser per link a pagina di pre stampa fatture/ddt: gestiti anche note di accredito, ordini, preventivi, ddt contabilizzati. (al momento utilizzato da Maiorano)")

            '==================================

            Riga_Data("14 Gennaio 2019")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaColturaleBiologico.aspx: ",
                  "- nella query di lettura aggiunto NOT EXISTS per escludere le visite che hanno il lav_cod delle altre operazioni e hanno sezione dedicata (copiato dalla scheda di campagna - altre operazioni colturali)" &
                  "- gestita erpicatura rotante e Intervento Antibrina.")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti.Query_Conti_Patrimoniali_Movimentati: ",
              "corretto bug su 14° PARTE: RISCOSSIONE PAGAMENTI - CONTO RIS FINANZIARIE IN AVERE - CON RIFERIMENTO BANCHE E CASSA (CoGe Auto), mancava il lav_cod 1070 (fattura professionisti) " &
              "[in pratica non veniva movimentato il conto in avere della banca per la fattura professionisti]")

            '==================================

            Riga_Data("9 Gennaio 2019")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Fattura/DTT: ",
                  " - Rimosse formule su SuperRag_Soc in rpt versioni 2016" &
                  " - Rimossi loghi interni di aziende non più clienti")

            '==================================

            Riga_Data("7 Gennaio 2019")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Fattura/DTT: ",
                  " - Rimossi loghi interni di Agronica + vecchi loghi Net-Agree + Ancarani + Gentili + Istine + San Polo")

            Riga_Text("Selezione_SchedaCampagna :",
                "chiamata la funzione ReplaceTipoReport: dismesse le stampe semplificate facendole puntare sempre alle multicentro")

            '==================================

            Riga_Data("3 Gennaio 2019")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Fattura/DTT: ",
                  " - Rimossi loghi interni di Tom e Giò + Giovannini + Bini Denny + Galvana + Barone Luigi + Bordona" &
                  " - Rimossi loghi interni di Tenuta Mara + Palazzona di Maggio + Barbon + Villa Papiano " &
                  " - Eliminati da CRBolla2016, CRBolla2016_LB, CRFattura2016 e CRFattura2016_LB i loghi delle sezioni personalizzate per clienti che sono ancora sui vecchi report")

            Riga_Text("BrogliaccioMovimentiTabella: ",
                 "Attivata 'salva personalizzazione griglia'")

            '==================================

            Riga_Data("2 Gennaio 2019")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Fattura/DTT: ",
                  "- Rimossi loghi interni di Valmorri + Gazzola + Martelli + Garuti + Tenuta Casali + Il Mio Casale + La Piana" &
                    "- Rimossi loghi interni di Zucchi + Zanasi + La Spinosa + Il Gallese + Baldetti + Folesano + Tomisa")

            Riga_Text("Filtro_StampeCantine.aspx: ",
                 "lettura del permesso utente enum_Security_Attivita.Registri_Cantina per aggiungere la possibilità di stampare il vecchio registro di commercializzazione (al momento utilizzato da Villa Papiano e Guarini che non inviano al Sian da Gias)")

            '==================================

            Riga_Data("20 Dicembre 2018")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Fattura/DTT: ",
                  "in caso di F&F la Qta avrà al massimo 3 decimali")

            '==================================

            Riga_Data("17 Dicembre 2018 - Versione C")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("scheda materie prime e vendite bio",
                  "eliminata parola 'interno' sul trasferimento di magazzino (richiesta di Zaghi)")

            Riga_Text("Contabilita - Query_Conti_Economici_Movimentati e Query_Conti_Patrimoniali_Movimentati",
                "- sulla parte dei documenti: rename in enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA" &
                "- sulla parte dei documenti sostituito il filtro con il nuovo campo Pagamenti.ChkCoge_Manuale_Pagamenti = enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA")

            Riga_Text("Contabilita - vari report IVA",
              "adeguamenti sul filtro della enum_ChkCoGe (ora legge i documenti con CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA,CoGe_CONTABILIZZAZIONE_MANUALE,CoGe_CONTABILIZZAZIONE_AUTOMATICA)")

            '==================================

            Riga_Data("17 Dicembre 2018 - Versione B ")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                  "corretto baco fioritura")

            '==================================

            Riga_Data("17 Dicembre 2018")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                  "corretta lettura per tare")

            Riga_Text("Fattura",
          "Tutti quanti ora salvano il report temporaneo su cartelle e non passano più da sessione")

            '==================================

            Riga_Data("14 Dicembre 2018")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("RegistriContab.vb e PianoConti.vb",
                  "adeguamento enum chkcoge")

            Riga_Text("ConsistenzeEnologiche",
                "sistemato il caso di 'Stampa anche le vasche con consistenze =0'")

            Riga_Text("SchedaCampagna.aspx",
                  "corretta lettura fase di fioritura per i casi vecchi (ora viene letta via web service la nuova e nel modo precedente la vecchia)")

            '==================================

            Riga_Data("10 Dicembre 2018")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                  "- CaricaDsFasiFenologiche: aggiunta indicazione BBCH" &
                  "- CaricaDsErbacee, CaricaDsConcimazioni, CaricaDsTrattamenti: modificata lettura fase di fioritura (ora viene letta via web service e gestisce sia la vecchia sia la nuova)")

            '==================================

            Riga_Data("7 Dicembre 2018")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCatastoUtilizzi",
                 "primo rilascio del report.")

            Riga_Text("AttoNotorio_2tipo - SchedaAutoCertificazione.aspx",
         "correzione sul com_cod_istat del centro")

            Riga_Text("Registro Aziendale Unico",
                "LeggiOccupazioneDes_su_quintuplaAGEA: eliminato il join sul cul_cod")

            '==================================

            Riga_Data("6 Dicembre 2018")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("AttoNotorio_2tipo - SchedaAutoCertificazione.aspx",
                 "sup_imp formattata a 4 decimali + richiamata la query semplificata AttoNotorio_Semplificato")

            Riga_Text("Fattura_NotaAccredito.aspx.vb",
                 "bugfix non veniva istanziata la classe ConfigurazioneStampe in assenza di record nella tabella")

            '==================================

            Riga_Data("5 Dicembre 2018")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                    "- gestito ordinamento per coltura sulle sezioni gestite dal registro aziendale unico:" &
                    "semine, concimazioni, trattamenti, irrigazione, altre op colturali, raccolte" &
                    "- CaricaDsIrrigazione: sistemata per la modalità multispecie" &
                       "")

            Riga_Text("Rpt_SchedaCampagnaMulti.rpt",
                 "reimport sottoreport irrigazioni")

            '==================================

            Riga_Data("4 Dicembre 2018")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
       "- aggiunte AGEA_LeggiDT_OccupazioneDes e AGEA_Ricava_OccupazioneDes" &
       "- CaricaDsImpianti_Multispecie: " &
       "            - gestita la lettura dell'utilizzo del macrouso (per reg trattamenti veneto lo concatena solo agli impianti di terreno nudo, per gli altri report lo concatena su tutti gli impianti) " &
       "            - per il reg trattamenti veneto gestito errore che si verificava sulla funzione CodiceAppezzamento " &
       "- CaricaDsCatasto: " &
       "            concatenato l'utilizzo del macrouso a specie e varietà (se era stato letto dalla lettura del piano colturale) " &
       "- CaricaDsDichiarazioni: sostituito agenda.Validita_inizio con Movimenti.Data_Movimento " &
       "- TipoReportSchedaCampagnaMulti_QuadernoCampagnaLombardiaVeneto: visualizato il sottoreport delle dichiarazioni di non utilizzo (che era stato commentato)" &
        "")

            Riga_Text("Rpt_SchedaCampagnaMulti.rpt",
                 "reimport sottoreport del frontespizio piano colturale")

            '==================================

            Riga_Data("30 Novembre 2018 (2)")

            Riga_Requisiti("Migra '514' :",
                       "per aggiunta colonne su tabella Configurazione_Stampe x doc contabili")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Fattura/DDT/Ordine",
                  "aggiunta possibilità di stampare o meno singolarmente confezioni / contenitori / imballi in descrizione prodotti" &
                  " (e relative tare) + riepilogo confezioni / contenitori / imballi usato tabella Configurazione_Stampe + default via codice")

            Riga_Text("Fattura",
                  "aggiunto parametro variabili stampa 'preview' che se passato a 0 forza l'ottenimento diretto del pdf " &
                  " ( a prescindere dalle impostazioni utente)")

            '==================================

            Riga_Data("30 Novembre 2018")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Rpt_SchedaCampagna_Multicentro.rpt",
                "riportato il report al 13/11 perché il reimport del sottoreport semine aveva perso le modifiche per Orogel fatte erroneamente aprendo il sottoreport dentro al report (si è generata una versione fantasma del sottoreport). DA GESTIRE")

            '==================================

            Riga_Data("29 Novembre 2018")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("AttoNotorio",
                 "sistemata la visualizzazione o meno del logo di terremerse.")

            Riga_Text("SchedaCampagna.aspx",
                "Registro Aziendale Unico: gestita la sezione delle raccolte." &
                "CaricaDsImpianti_Multispecie e CaricaDsErbacee: corretto bug che si generava sulle imprese con piva fittizia" &
                "CaricaDsCatasto: modifica gestione ordinamento per coltura" &
                "CaricaDsSemine, CaricaDsConcimazioni, CaricaDsAltreOperazioniCulturali, CaricaDsRilievoProduzioneRaccoltaNEW e CaricaDsIrrigazione: modifica gestione ordinamento (ma non funziona)" &
                "CaricaDsTrattamentiPostRaccolta: debuggata (prima funzionava solo se c'era un solo trattamento)" &
                "CaricaDsTrattamenti: introdotte note sul prodotto in miscela + modifica gestione ordinamento (ma non funziona)" &
                "CopertinaLombardia: visualizzati CUAA e Piva" &
                "CaricaDsRilievoProduzioneRaccoltaNEW: sistemata la query (che era in vecchio stile sql server 2000), gestiti anche i terreni nudi con visualizzazione destinazione d'uso" &
                "CaricaDsVisite: adeguamento sul cambiamento del salvataggio dell'orario" &
                "SchedaCampagnaMulticentro: sistemata la stampa delle intestazioni nel caso di orogel" &
                "Tutte le schede: gestita l'impostazione utente sulla visualizzazione del nome del campo solo sul frontespizio e non sulle sezioni delle operazioni" &
                "")

            Riga_Text("Selezione_SchedaCampagna.aspx",
                "RegistroAziendaleUnico: disattivato la possibilità di ordinamento (non funziona) e gestita la sezione delle raccolte")

            '==================================

            Riga_Data("26 Novembre 2018")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCatastoUtilizzi",
                  "creato nuovo report SchedaCatastoUtilizzi (working progress)")

            Riga_Text("AttoNotorio_2tipo",
                 "creata cartella AttoNotorio_2tipo con nuovo report SchedaAutoCertificazione creato a partire dal report dell'Atto Notorio")

            Riga_Text("SchedaCampagna.aspx",
                "Registro Aziendale Unico: gestita la visualizzazione del testo Misura10 in base al flag che arriva dal pre stampa." &
                "CaricaDsCatasto: gestito ordinamento per coltura" &
                "CaricaDsImpianti_Multispecie: letta la destinazione d'uso nella query principale ed adeguamenti collaterali" &
                "CaricaDsSemine e CaricaDsIrrigazione: gestito ordinamento per coltura + utilizzata data_movimento " &
                "CaricaDsConcimazioni e CaricaDsTrattamenti: letta la destinazione d'uso nella query principale ed adeguamenti collaterali + gestito ordinamento per coltura" &
                "CaricaDsRilievoPiogge: utilizzata(data_movimento)")

            Riga_Text("Selezione_SchedaCampagna.aspx",
                "- divRegolamenti: rename dei nomi dei check e aggiunto anche il check per la Misura10 " &
                "- solo nel caso di RegistroAziendaleUnico visualizzata la possibilità di ordinamento per data o per coltura")

            '==================================

            Riga_Data("20 Novembre 2018")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                  "- gestita sezione semine nel Registro Aziendale Unico" &
                "- CaricaDsSemine: sui dati 'indefinito', 'non definito' impostato '' " &
                "- CaricaDsSemine: modificata la query per leggere veg_des oltre al cul_des oppure la destinazione d'uso e gestito il flag_multispecie" &
                "- gestiti in tutti i report la non visualizzazione della piva nel caso sia fittizia (F...)")

            Riga_Text("Rpt_SchedaCampagnaMulti.rpt",
                "aggiunta sezione per le semine")

            Riga_Text("Schede BIO (tutte e tre)",
                "stampato il CUAA e gestita la non visualizzazione della piva nel caso sia fittizia (F...)")

            '==================================

            Riga_Data("13 Novembre 2018")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                  "- CaricaDsSemine: introdotto left join su cultivar e specie altrimenti non venivano lette le semine sui terreni nudi." &
                "- CaricaDsAltreOperazioniCulturali: patch sulla precedente release (join su specie e cultivar che erano stati commentati, ma per alcuni filtri servivano) ")

            Riga_Text("Schede di Campagna RPT",
                "sui sottoreport degli impianti impostato format a 4 cifre decimali e separatore migliaia")

            '==================================

            Riga_Data("12 Novembre 2018 - Versione B")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaVenditeBiologico.",
                  "- gestite altre causali e altri pendenti sullo scarico")

            Riga_Text("SchedaMateriePrime",
              "- gestite altre causali" &
              "- eliminato il cau_mov di scarico nella lettura altrimenti il trasferimento veniva visualizzato due volte")

            '==================================

            Riga_Data("12 Novembre 2018")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaVenditeBiologico.aspx",
                  "nei DDT stampata anche la causale_trasporto")

            Riga_Text("Selezione_SchedaCampagna.aspx:",
                "- visualizzato titolo e rag_soc dell'impresa" &
                "- gestite le sezioni per il registro aziendale unico" &
                "- introdotti flag per la scelta dei regolamenti nella stampa del registro aziendale unico")

            Riga_Text("Schede di Campagna RPT",
                "- visualizzato il CUAA nell'intestazione della QDC semplificata, multicentro e multispecie")

            Riga_Text("SchedaCampagna.aspx",
                  "- Gestione del CUAA anche per le schede in cui mancava." &
                "CaricaDsConcimazioni:" &
                "- le dosi prima venivano sempre arrotondate a 3, ora usano l'arrotondamento scelto nel filtro stampa" &
                "- sostituiti double con decimal" &
                "- usata nuova funzione di round" &
                "- sup_trattata arrotondata sempre a 4" &
                "CaricaDsTrattamenti:" &
                "- sostituiti double con decimal" &
                "- usata nuova funzione di round" &
                "- sostituiti i math.round con AgronicaCoreDataProvider.Agro_Math.ArrotondaVal" &
                "- sup_trattata arrotondata sempre a 4" &
                "CompactDtTrattamentiPostRaccolta_Multispecie" &
                "- sostituiti double con decimal" &
                "- sup_trattata arrotondata sempre a 4" &
                "" &
                "CaricaDsCatasto: veg_des e cul_des unificate e lette anche le destinazioni d'uso " &
                "" &
                "CaricaDsAltreOperazioniCulturali:" &
                "- compattate le righe anche nei report del tipo scheda campagna multispecie" &
                "CaricaDsErbacee e CaricaDsImpianti_Multispecie: gestito il CUAA " &
                "- " &
                "- ")


            '==================================

            Riga_Data("9 Novembre 2018")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Bolla_FF: ",
                  "Eliminato il logo interno fisso di Cofruta, caricando dinamicamente i loghi da file system")

            '==================================

            Riga_Data("5 Novembre 2018")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Fattura_NotaAccredito.aspx",
                  "applicata stessa modifica fatta al ddt: se orario = 00:00 non viene stampato")

            Riga_Text("SchedaColturaleBiologico.aspx",
                "Aggiunte al filtro di lettura anche semina per sovescio - Semina Su Sodo - Trapianto in serra.")

            '==================================

            Riga_Data("02 Novembre 2018 - versione B")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaMovimentiMagazzino",
                  "corretto bug sul filtro della data fine presente dal 23 Agosto 2018")

            '==================================

            Riga_Data("02 Novembre 2018")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Rpt_SchedaCampagnaMulti.rpt",
                  "fix baco per cui il nome dell'appezzamento non andava a capo")

            '==================================

            Riga_Data("25 Ottobre 2018")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Visualizzatore report",
                  "fix baco su visualizzazione pdf da url locale e impostazione di visualizzazione diretta del file pdf")

            '==================================

            Riga_Data("19 Ottobre 2018")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampa Global",
                  "Arrotondate a 4 cifre le superfici trattate")

            '==================================

            Riga_Data("17 Ottobre 2018")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampa scheda Trentino",
                  "Ottimizzazioni per ridurre il numero di pagine")


            '==================================

            Riga_Data("11 Ottobre 2018")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("DDT e Fattura",
                  "Podere dell'Angelo ora ha logo su fileSystem e gestione da tabella Configurazione_Stampe")

            Riga_Text("DDT e Fattura",
                  "Fiammetta ora ha gestione da tabella Configurazione_Stampe + eliminata sua piva in formula che nascondeva la ragione sociale")

            '==================================

            Riga_Data("4 Ottobre 2018")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaMateriePrime",
                "Nella parte giacenze di magazzino gestite tutte le categorie materie prime per il salvataggio dei mat-cod per ricavare il regolamento.")

            '==================================

            Riga_Data("20 Settembre 2018")

            Riga_Requisiti("Migra '506' :",
                       "per aggiunta colonne su tabella DW_CDG_Costi_Ricavi")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 17/09/2018 per passaggio parametri report CdG")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Report CdG",
                "aggiunta gestione distinta poliennale")

            Riga_Text("SchedaMateriePrime",
                "- visualizza anche il tipo documento oltre al numero" &
                "- gestita la lettura delle fatture collegate ai ddt" &
                "- gestita l'opzione di visualizzazione del cod articolo" &
                "- sui formulati aggiunta dicitura num reg" &
                "- sistemata visualizzazione lotto ")

            Riga_Text(" SchedaVendite",
                "- gestita l'opzione di visualizzazione del cod articolo" &
            "- sistemata visualizzazione lotto:" &
            "nel caso di configurazione lotto (usata dalle cantine) visualizzo il lotto nella desc del prodotto (visto che ci sarà l'anno di produzione)" &
            "nel caso di visualizza sempre il lotto (giasonline) visualizzo il lotto nella colonna etichetta.")


            '==================================


            Riga_Data("13 Settembre 2018")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaMateriePrime e SchedaVendite",
                "sistemata la visualizzazione dei dati se il contatto è persona fisica.")

            '==================================

            Riga_Data("11 Settembre 2018")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                  "CaricaDsErbacee: dalla query tolta la lettura del legale/referente aziendale + gestito in modo dedicato la visualizzazione del legale/referente " &
                  "(da priorità al legale e, se ce ne sono più di 1, li visualizza tutti; se valorizzate visualizza anche le date di validità)")

            Riga_Text("AgronicaCoreStampeDAL.Magazzino.SchedaMovimentiMagazzino()",
               "Agenda.sa_cod AS Sa_Cod_Agenda (c'era già un sa_cod nella query)")

            Riga_Text("SchedaMovimentiMagazzino.aspx",
                "Adeguata a Sa_Cod_Agenda della query")

            '==================================

            Riga_Data("3 Settembre 2018")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.aspx",
                  "data consegna: visualizzato l'orario solo se <> 00:00")

            Riga_Text("rptEtichetteConferimento_MiniFrutta.rpt",
                 "Modificata formula sulla specie per gestire Nett.")

            '==================================

            Riga_Data("24 Agosto 2018 Versione B")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Magazzino",
                  "ripristinato core + patch group by giacenze")

            '==================================

            Riga_Data("24 Agosto 2018 - NON USARE")

            Riga_Requisiti("Versione fallata!!!!!!! :",
                       "è stato fatto il rollback di magazzino.vb ma non di SchedaMovimentiMagazzino.aspx")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("magazzino.vb",
                  "rollback per problema su giacenze magazzino")

            '==================================

            Riga_Data("23 Agosto 2018")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx",
                  "CaricaDsTrattamentiPostRaccolta: patch nel caso di stampa su un terreno nudo (es: erbaio)")

            Riga_Text("SchedaTracciabilita",
                "SchedaTracciabilitaVegetale_SezioneVendite: - patch, Mov_Dettaglio_Tecnico_Extra in left join perché il GiasOnline non la salva" &
                "- modificato filtro sulla data_fine perché da quando il GiasOnline ha iniziato a salvare anche l'ora in data_movimento l'ultimo giorno non veniva conteggiato")

            Riga_Text("SchedaMovimentiMagazzino e SchedaGiacenzeMagazzinoSementiConBolle",
                "- modificato filtro sulla data_fine perché da quando il GiasOnline ha iniziato a salvare anche l'ora in data_movimento l'ultimo giorno non veniva conteggiato" &
                "- ")

            Riga_Text("SchedaVendite e SchedaMateriePrime",
                "- LeggiVendite e LeggiMovimentiCarico: modificato filtro sulla data_fine perché da quando il GiasOnline ha iniziato a salvare anche l'ora in data_movimento l'ultimo giorno non veniva conteggiato" &
                "- scheda vendite bio: gestito lo scarico di magazzino con causale smaltimento / perdita di lavorazione")

            '==================================

            Riga_Data("30 Luglio 2018")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("DocumentiConatb",
                  "Aggiunto peso lordo tot in descrizione dettaglio per FF")

            '==================================

            Riga_Data("25 Luglio 2018")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Selezione_SchedaCampagna.aspx",
              "gestito il filtro sulla destinazione d'uso")

            '==================================

            Riga_Data("24 Luglio 2018")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaColturaleBiologico: ",
                  "- Per le trappole prendeva espressamente la qta_tot invece di quella sull'impianto -> sistemato" &
                    "Il problema era presente nella stampa normale e, come conseguenza, anche in quella raggruppata." &
                    "Nel caso di installazione trappole e catture di massa introdotta ripartizione della qta tot sulla superficie dell'impianto," &
                    "in modo da avere un valore su ogni riga (altrimenti per queste operazioni veniva visualizzato il totale su un impianto e 0 negli altri)." &
                    "- su db in data_movimento c'è salvata anche l'ora (probabilmente recente introduzione)" &
                    "il confronto su datetime nella ricerca di un preciso giorno scartava i movimenti di quel giorno a causa dell'orario -> sistemato" &
                    "- sistemato il regolamento della scheda che veniva troncato " &
                    "- sistemato piè di pagina")

            '==================================

            Riga_Data("23 Luglio 2018")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("CRFattura*.rpt: ",
                  "In riepilogo totali documento cambiato 'Totale Documento' in 'Totale' e 'Totale da pagare' in 'Totale Documento'")

            '==================================

            Riga_Data("17 Luglio 2018")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaColturaleBiologico.aspx",
                  "- aggiunta l'andata a capo nella query (per faciliare il debug)" &
                    "- sostituito inner join con left join su Cultivar e SpecieVegetali per poter stampare anche le operazioni dei terreni nudi")

            Riga_Text("SchedaMateriePrimeBiologico.aspx",
                  "chiamata la LeggiProdotto per i prodotti della banca dati diversi da fito e concimi (prima metteva "")")

            '==================================

            Riga_Data("13 Luglio 2018")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '49' :",
                       "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("DDT e Fattura",
                  "Sistemato arrotondamento sotto opzione in FF anche in speso netto di fattura")

            Riga_Text("CRFattura_Cofruta",
                  "Eliminata Stampa di NSBanca in testata documento, lasciato solo in campo a parte, visibile solo su prima pagina")

            Riga_Text("SchedaTracciabilita.aspx",
                "Patch sulla funzione BolleRicevute_from_MateriaPrima.")

            '==================================

            Riga_Data("12 Luglio 2018")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '49' :",
                         "x esportazione excel anagrafica contatti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Esportazione_AnagraficaContatti",
                 "- porting dalle stampe 2003 di Filtro_Contatti e AnagraficaContatti_XLS " &
                 "- nell' esportazione contatti gestito anche il listino di vendita di default.")

            Riga_Text("AgronicaCoreStampeDAL.RegistriContab.RegistriIVA_3",
                    "Patch, la query andava in errore su SQL2008.")

            '==================================

            Riga_Data("10 Luglio 2018")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '46' :",
                       "x scheda tracciabilità vegetale ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaTracciabilita.aspx",
                 "patch nel caso di DSAcquisti vuoto (al momento del reimport delle righe).")

            '==================================

            Riga_Data("28 giugno 2018")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '46' :",
                       "x scheda tracciabilità vegetale ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx e Rpt_RegistroTrattamentiVeneto.rpt",
                 "registro trattamenti veneto: gestita la nuova sezione manutenzione macchinari (viene stampata se selezionato check sul filtro stampa) e introduzione logo cliente.")

            Riga_Text("Selezione_SchedaCampagna.aspx",
                 "Introdotta possibilità di check sezione trattamenti post raccolta.")

            Riga_Text("SchedaCampagna.aspx",
                    "Scheda di campagna multicentro: aggiunto il sottoreport Rpt_TrattamentiPostRaccolta.rpt (creato a partire da quello della scheda Veneto).")

            Riga_Text("SchedaColturaleBiologico.aspx",
                    "Patch nel caso di stampa su impianti non biologici.")


            '==================================

            Riga_Data("18 Giugno 2018")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '46' :",
                       "x scheda tracciabilità vegetale ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Analisi_Progetti.aspx",
                 "Modifiche varie al report costi / ricavi")


            '==================================
            Riga_Data("15 Giugno 2018")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '46' :",
                       "x scheda tracciabilità vegetale ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaColturaleBiologico.aspx",
                 "ottimizzazione su costi accessori e fasi fenologiche per problema segnalato da coldiretti")


            '==================================
            Riga_Data("11 Giugno 2018")

            Riga_Requisiti("Migra '493' :",
                       "per aggiunta colonne Tara_Unit_Conf_Riscontrata, Tara_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata In Mov_Dettaglio_Tecnico_Extra")

            Riga_Requisiti("Configurazione_Siti '46' :",
                       "x scheda tracciabilità vegetale ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("AgronicaCoreStampeDAL.FreshAndFood",
                  "RiepilogoImballi_BollaConferimento: riattivata visualizzazione confezioni x richiesta di Cofruta.")

            Riga_Text("EC_Imballi_Conf",
                 "rename titolo report in 'estratto conto beni confezionamento'")

            Riga_Text("Rpt_AttoNotorio.rpt",
                 "aggiornato titolo e sottotitolo per richiesta di TERREMERSE")

            Riga_Text("Bolla",
                 "Aggiunto layout pesi con valori totalmente riscontrati, se disponibili")

            '==================================

            Riga_Data("5 Giugno 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '46' :",
                       "x scheda tracciabilità vegetale ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Rpt_RegistroImbottigliamento.rpt",
                  "Campo lotto, tolto limite max di due righe.")

            Riga_Text("EC_Imballi_Conf ",
                    "tolto il filtro chkimballaggio=1 ")

            Riga_Text("Filtro_Stampe_Conf.aspx ",
                "modificato caricamento cmb_prodotti nel caso report imballi: carica tutti i beni di confezionamento, non solo imballi")

            '==================================

            Riga_Data("28 Maggio 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '46' :",
                       "x scheda tracciabilità vegetale ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Etichette F&F",
                  "Modifiche.")

            '==================================

            Riga_Data("25 Maggio 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '46' :",
                       "x scheda tracciabilità vegetale ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx e Rpt_SchedaCampagna_Multicentro.rpt ",
                  "SCHEDA MULTICENTRO E INTERVENTI AGRONOMICI: parametrizzate alcune intestazioni di colonna della sezione semina (se cliente è orogel alcune non sono visualizzate)")

            '==================================

            Riga_Data("24 Maggio 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '46' :",
                       "x scheda tracciabilità vegetale ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx e Rpt_SchedaCampagna_Multicentro.rpt ",
                  "Scheda interventi agronomici: titolo, logo e revisione visualizzati in tutte le pagine + eliminata label 'data fioritura' sugli impianti + aggiunta sezione 'parte riservata al tecnico'  " &
                  "Formule sulle sezioni per non distruggere la scheda multicentro + aggiunta colonna principi attivi sulla sezione trattamenti")

            Riga_Text("Rpt_SchedaCampagna_EUREP_GAP_Multicentro.rpt e Rpt_SchedaCampagna_EUREP_GAP_Semplificata.rpt",
                    "aumento di riga sul campo lotto prodotto della sezione semine/trapianti. ")

            Riga_Text(" ConsistenzeEnologiche.aspx",
              "Scartate dall'output le giacenze che sono <> 0 (quindi lette dalla query) ma infinitamente piccole (es. -3,41060513164848E-13).")

            '==================================
            Riga_Data("21 Maggio 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '46' :",
                       "x scheda tracciabilità vegetale ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text(" Master",
                  "  Gestione collegamento GiasBase")

            '==================================

            Riga_Data("18 Maggio 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '46' :",
                       "x scheda tracciabilità vegetale ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text(" Bolla_Campionatura",
                  "  Aggiunta stampa note di testata ")

            '==================================
            Riga_Data("16 Maggio 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '46' :",
                       "x scheda tracciabilità vegetale ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text(" StampaTracciabilitaVegetale",
                  "  fix su stampa Tracciabilita ")

            Riga_Text(" BrogliaccioMovimentiTabella",
                  "  Filtro tabella brogliaccio ")

            '==================================

            Riga_Data("14 Maggio 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '46' :",
                       "x scheda tracciabilità vegetale ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text(" StampaTracciabilitaVegetale",
                  "  aggiunta nuova stampa - cacciavitata ")

            '==================================

            Riga_Data("9 Maggio 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '44' :",
                       "x scheda interventi agronomici ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text(" StampeBootStrap.Master :",
                  "  Rimosso riferimento ad Angular.js (non usato) ")

            '==================================
            Riga_Data("9 Maggio 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '44' :",
                       "x scheda interventi agronomici ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("RegistroCorrispettivi 2:",
                  "Corretta lettura parametro QueryString [sez] anziché [szc] ")

            Riga_Text("Fattura-DDT :",
                  "In FF se prezzo al Kg anche qta e udm devono essere Kg (solo x fattura)")

            Riga_Text("Bolla-Campionatura :",
                  "Orario di inizio e fine lavorazione espresso in formato 24h")


            '==================================
            Riga_Data("3 Maggio 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '44' :",
                       "x scheda interventi agronomici ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Fattura-DDT :",
                  "Eliminato logo di Cà Bruciata da dentro rpt (+ riferimenti codice) perché ora ha logo nuovo con gestione da tab Configurazione_Stampe")

            Riga_Text("RegistroCorrispettivi :",
                   "nuovo report")

            Riga_Text("RegistriIVA :",
               "nuovo layout del report")

            Riga_Text("LiquidazioneIVA :",
                "nuovo layout del report")

            '==================================
            Riga_Data("24 Aprile 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '44' :",
                       "x scheda interventi agronomici ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Selezione_SchedaCampagna.aspx :",
                     "introdotta visualizzazione dell'errore nel page load + corretto bug su inizializzazione check org referente per scheda interventi agronomici")

            Riga_Text("SchedaCampagna.aspx - Rpt_SchedaCampagna_EUREP_GAP_Multicentro.rpt - Rpt_SchedaCampagna_EUREP_GAP_Semplificata.rpt:",
                   "modificata formula per nascondere o meno la sezione della revisione globalgap (prima veniva visualizzata eventualmente dalla seconda pagina in poi, ora solo nella prima e se la revisione è valorizzata)")

            '==================================
            Riga_Data("23 Aprile 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '44' :",
                       "x scheda interventi agronomici ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 20/04/2017 per scheda interventi agronomici" &
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Selezione_SchedaCampagna.aspx :",
                     "- riattivato funzionamento della stampa sezioni vuote" &
                    "- introdotto salvataggio log di errori" &
                    "- gestito salvataggio su database della revisione globalgap" &
                    "- per la scheda interventi agronomici gestito il check per visualizzare l'organismo referente come intestatario")

            Riga_Text("SchedaCampagna.aspx :",
                   "- introdotto salvataggio log di errori su alcune funzioni" &
                  "- stampe global: leggono la revisione da database (non più da variabile di sessione)" &
                  "- gestita scheda interventi agronomici")

            '==================================
            Riga_Data("10 Aprile 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '42' :",
                       "x Flag_Stampe_Nuovi_Arrotondamenti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("EstrattoreGrafica/Filtro_EstrattoreGrafica.aspx.vb :",
                         "Filtro_EstrattoreGrafica: Gestita la differenza di tipologia Layer in filtro ed esportazione")

            '==================================

            Riga_Data("04 Aprile 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '42' :",
                       "x Flag_Stampe_Nuovi_Arrotondamenti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto" &
                       "- GiasBase > 02/02/2018 x uso di Kendo 2018.1.117")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Filtro_ElaboratiContabili.aspx.vb :",
                  "Introdotto controllo per Anno Imputazione-Data Registrazione per Mastrino e Bilancio")

            Riga_Text("Fattura-DDT :",
                  "Eliminato logo di Cavaliera da dentro rpt (+ riferimenti codice) perché ora ha logo nuovo con gestione da tab Configurazione_Stampe")

            Riga_Text("StampeBootstrap.Master : ",
                  "Uso di Kendo 2018.1.117 + corretto logo footer + sistemazione errori inclusione kendo css")

            Riga_Text("Filtro_SchedeMagazzino_New : ",
                  "Visualizzazione dei pulsanti Export Excel e blocco operazioni condizionato in base a permesso, " &
                  "Corretto bug che perdeva le date e il tipo stampa in fase di postback")

            Riga_Fine()

            '==================================

            Riga_Data("21 Marzo 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '42' :",
                       "x Flag_Stampe_Nuovi_Arrotondamenti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("RisultatoAnalisiConformita :",
                    "aggiunta sezione controllo ulteriori criteri disciplinare")

            Riga_Fine()

            '==================================

            Riga_Data("15 Marzo 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '42' :",
                       "x Flag_Stampe_Nuovi_Arrotondamenti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("BrogliaccioMovimenti Pdf :",
                  "commentati molti parametri in qs e adeguata chiamata alla query, venivano passati dalla pagina di filtro anche se erano filtri nascosti.")

            Riga_Text("SchedaCampagna - GLOBAL GAP :",
                "patch su fasi fenologiche e sistemata parte grafica acqua e dosi su trattamenti.rpt")

            Riga_Fine()

            '==================================

            Riga_Data("22 Febbraio 2018")

            Riga_Requisiti("Migra '481' :",
                       "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '42' :",
                       "x Flag_Stampe_Nuovi_Arrotondamenti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna - GLOBAL GAP :",
                  ": inserito codice GGN, CUAA + sistemata graficamente l'intestazione")

            Riga_Fine()

            '==================================

            Riga_Data("19 Febbraio 2018")

            Riga_Requisiti("Migra '481' :",
                     "per aggiunta colonna Flag_Credito_Imposta_Export alla tabella IVA_Aliquote")

            Riga_Requisiti("Configurazione_Siti '42' :",
                       "x Flag_Stampe_Nuovi_Arrotondamenti ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("RegistrContab.vb :",
                  "Filtro data scadenza su RegistroRIBA va applicato sulla scadenza della singola tranches (Pagamenti), non su quella dell'intero documento (Movimenti)")

            Riga_Text("AgentiProvvigioni_XLS:",
                  "Introdotta Nota di Credito Emessa + Uso di Decimal, anziché Double")

            Riga_Text("Documenti Contabili:",
                  "Introduzione Nuova gestione arrotondamenti")

            Riga_Fine()

            '==================================

            Riga_Data("2 Febbraio 2018")

            Riga_Requisiti("Migra '478' :",
                       "per aggiunta colonna tipo_arrotondamenti in OGenerazioni_Moduli_Log")

            Riga_Requisiti("Configurazione_Siti '41' :",
                       "x stampe liquidazione Fresh & Food ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Ricevuta Fiscale :",
                  "Aggiunta riga descrizione libera + corretto bug di stampa su word se ci sono sconti/omaggi")

            Riga_Text("DDT :",
                  "aggiunta opzione per formattazione pesi con/senza decimali per FF")

            Riga_Text("RisultatoAnalisiConformita :",
                    "aggiunte sezioni controllo magazzino ed IAF")

            Riga_Fine()

            '==================================

            Riga_Data("24 Gennaio 2018 ")

            Riga_Requisiti("Migra '477' :",
                       "per aggiunta tabelle liquidazioni FF")

            Riga_Requisiti("Configurazione_Siti '41' :",
                       "x stampe liquidazione Fresh & Food ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Scheda Campagna :",
                  "aggiornata analisi conformità con controllo volumi max acqua dpi e tipo intervento consentito (no diserbo in bio).")

            Riga_Text("Scheda Campagna :",
                "Nelle revisioni macchine, se si seleziona il centro nel prefiltro vengono mostrate solo quelle di quel centro (o pubbiche). Altrimenti vengono mostrate quelle dei centri degli appezzamenti in oggetto (e quelle pubbliche).")

            Riga_Fine()


            '==================================

            Riga_Data("22 Gennaio 2018 ")

            Riga_Requisiti("Migra '477' :",
                       "per aggiunta tabelle liquidazioni FF")

            Riga_Requisiti("Configurazione_Siti '41' :",
                       "x stampe liquidazione Fresh & Food ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Fattura_NotaAccredito.vb:",
                  "Stampa riepilogo imballi anche in fatt. acc. ed esclusione dei relativi record in castelletto iva <br>" &
                  "Correzione errore indice oltre i limiti della matrice quando riga descrizione libera + estrazione di alcuni metodi")

            Riga_Fine()

            '==================================

            Riga_Data("27 Dicembre 2017")

            Riga_Requisiti("Migra '477' :",
                       "per aggiunta tabelle liquidazioni FF")

            Riga_Requisiti("Configurazione_Siti '41' :",
                       "x stampe liquidazione Fresh & Food ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.vb:",
                  "rimessi i decimali sui pesi per il F&F")

            Riga_Text("BrogliaccioMovimenti:",
                  "patch nelle query di exists con le tabelle che hanno id_report (escluso enum_AgroReportistica.PreparazioniBio)")

            Riga_Fine()

            '==================================

            Riga_Data("19 Dicembre 2017")

            Riga_Requisiti("Migra '477' :",
                       "per aggiunta tabelle liquidazioni FF")

            Riga_Requisiti("Configurazione_Siti '41' :",
                       "x stampe liquidazione Fresh & Food ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("RicevutaFiscale_GestioneStampa.vb:",
                  "nel caso degli omaggi visualizzato 'omaggio' nella colonna importo e visualizzato importo totale omaggi")

            Riga_Fine()

            '==================================

            Riga_Data("15 Dicembre 2017 B")

            Riga_Requisiti("Migra '477' :",
                       "per aggiunta tabelle liquidazioni FF")

            Riga_Requisiti("Configurazione_Siti '41' :",
                       "x stampe liquidazione Fresh & Food ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaFertilizzantiMagazzino.vb:",
                  "corretta stampa Fertilizzanti di magazzino per cui i titoli npk apparivano solo se erano associati ad una ditta")

            Riga_Fine()

            '==================================

            Riga_Data("15 Dicembre 2017")

            Riga_Requisiti("Migra '477' :",
                       "per aggiunta tabelle liquidazioni FF")

            Riga_Requisiti("Configurazione_Siti '41' :",
                       "x stampe liquidazione Fresh & Food ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.vb:",
                  "corretta stampa global multicentro per cui se ci sono 2 note, vengono raddoppiati i mm di acqua e gli altri valori di conseguenza")

            Riga_Text("DDT e Fattura:",
               "personalizzazioni stampa ddt e fatture per Trombin Mattia")


            Riga_Fine()

            '==================================

            Riga_Data("13 Dicembre 2017")

            Riga_Requisiti("Migra '477' :",
                       "per aggiunta tabelle liquidazioni FF")

            Riga_Requisiti("Configurazione_Siti '41' :",
                       "x stampe liquidazione Fresh & Food ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampa fattura e riepilogo liquidazione:",
                  "corretto bug su prezzo ivato calcolato male")

            Riga_Text("Agro_Math.RoundNumber:",
                  "corretto arrotondamento sbagliato se il numero di input è negativo")

            Riga_Fine()

            '==================================

            Riga_Data("07 Dicembre 2017")

            Riga_Requisiti("Migra '477' :",
                       "per aggiunta tabelle liquidazioni FF")

            Riga_Requisiti("Configurazione_Siti '41' :",
                       "x stampe liquidazione Fresh & Food ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 07/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampe Liquidazioni FF:",
                  "Aggiunte autofatture, riepilogo liquidazioni e pagati su campionato")

            Riga_Fine()

            '==================================

            Riga_Data("06 Dicembre 2017")

            Riga_Requisiti("Migra '477' :",
                       "per aggiunta tabelle liquidazioni FF")

            Riga_Requisiti("Configurazione_Siti '40' :",
                       "x stampe liquidazione Fresh & Food ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 05/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("ConsistenzeEnologiche:",
                  "eliminato il join sul sa_cod perché altrimenti non venivano letti i traferimenti da un ICQRF (SA_COD) all'altro (ALTRO SA_COD) es: podere palazzo")

            Riga_Fine()

            '==================================

            Riga_Data("05 Dicembre 2017")

            Riga_Requisiti("Migra '477' :",
                       "per aggiunta tabelle liquidazioni FF")

            Riga_Requisiti("Configurazione_Siti '40' :",
                       "x stampe liquidazione Fresh & Food ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 05/12/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("VisualizzatoreReport.aspx:",
                  "in caso di passagio diretto del pdf come parametro viene valorizzato anche il nome file, così salvando appare il nome reale e non il generico VisualizzatoreReport")

            Riga_Text("FF Liquidazione:",
                  "Aggiunte stampe Fattura Liquidazione Soci e Pagati su conferito")

            Riga_Text("AgronicaCoreParametri:",
                  "Corretto Recupera_NomeDB che si impiantava perché la stringaConnessione termina con ;")

            Riga_Fine()

            '==================================

            Riga_Data("01 Dicembre 2017")

            Riga_Requisiti("Migra '470' :",
                       "per script che aggiorna in automatico il salvataggio del magazzino conferimento su impianto")

            Riga_Requisiti("Configurazione_Siti '33' :",
                       "x stampa analisi conformita ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampe Campagna:",
                  "Fix problema su stampe campagna con irrigazioni con qta con la virgola")

            Riga_Fine()

            '==================================

            Riga_Data("29 Novembre 2017")

            Riga_Requisiti("Migra '470' :",
                       "per script che aggiorna in automatico il salvataggio del magazzino conferimento su impianto")

            Riga_Requisiti("Configurazione_Siti '33' :",
                       "x stampa analisi conformita ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Selezione_schedaCampagna:",
                  "Attivato il patentino nella stampa multicentro")

            Riga_Text("CRBolla_Trombin.rpt e CRFattura_Trombin.rpt:",
                  "Aggiunto logo Trombin Mattia.")

            Riga_Fine()

            '==================================

            Riga_Data("24 Novembre 2017")

            Riga_Requisiti("Migra '470' :",
                       "per script che aggiorna in automatico il salvataggio del magazzino conferimento su impianto")

            Riga_Requisiti("Configurazione_Siti '33' :",
                       "x stampa analisi conformita ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Schede_OP - MandatoTrasmissioneDati:",
                  "modifiche chieste da Saragoni per il 2018")

            Riga_Fine()

            '==================================

            Riga_Data("16 Novembre 2017")

            Riga_Requisiti("Migra '470' :",
                       "per script che aggiorna in automatico il salvataggio del magazzino conferimento su impianto")

            Riga_Requisiti("Configurazione_Siti '33' :",
                       "x stampa analisi conformita ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Stampe Campagna:",
                  "Corretto baco in irrigazioni per cui se i mm sono con la virgola, viene stampato 0.")

            Riga_Text("Stampe Campagna:",
                  "Generalizzata la stampa di campagna della lombardia con logo che cambia dinamicamente insieme alla regione")

            Riga_Text("RegistriContab:",
                  "Corretto baco che includeva anche l'autoconsumo del giorno successivo rispetto al periodo in registri IVA vendite")

            Riga_Fine()

            '==================================

            Riga_Data("07 Novembre 2017")

            Riga_Requisiti("Migra '470' :",
                       "per script che aggiorna in automatico il salvataggio del magazzino conferimento su impianto")

            Riga_Requisiti("Configurazione_Siti '33' :",
                       "x stampa analisi conformita ")

            Riga_Requisiti("Altri requisiti :",
                       "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF" &
                       "- GiasOnline 2003 > 3 nov 2017 x modifica magazzino di conferimento su impianto")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("DDT con dettagli economici:",
                  "corretto bug per cui i non FF che passano ancora da fattura, ma usano già la configurazione con tabella," &
                  "ora istanziano il report della fattura, anziché quello del DDT")

            Riga_Text("Documenti Contab:",
                  "Fixbug x Trombin che usa descrizione breve e non vedeva la riga descrizione libera")

            Riga_Text("Stampe Campagna:",
                  "Fix grafici di font e linee griglia")

            Riga_Text("Esportatore_Universale:",
                "adeguata ai nuovi sviluppi la lettura del magazzino di conferimento + Gestione delle marachelle di Drudi nel porting da 2003 a 2010.")

            Riga_Fine()

            '==================================

            Riga_Data("24 Ottobre 2017 B")

            Riga_Requisiti("Migra '466' :",
                  "")

            Riga_Requisiti("Configurazione_Siti '33' :",
                  "x stampa analisi conformita ")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DDT e Fattura Trombin:",
                "Sistemazione layout per Mattia")

            Riga_Fine()

            '==================================

            Riga_Data("24 Ottobre 2017")

            Riga_Requisiti("Migra '466' :",
                  "")

            Riga_Requisiti("Configurazione_Siti '33' :",
                  "x stampa analisi conformita ")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DDT e Fattura :",
                "Eliminazione loghi ed if nel codice per Villa Venti, che ora usa il logo su File System e tabella di configurazione")

            Riga_Fine()

            '==================================

            Riga_Data("19 Ottobre 2017 B")

            Riga_Requisiti("Migra '466' :",
                  "")

            Riga_Requisiti("Configurazione_Siti '33' :",
                  "x stampa analisi conformita ")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Esportatore universale :",
                "Stampe: corretto baco in esportazione impianti (esportatore universale) in caso non si metta la ragione sociale + sblocco checkbox")

            Riga_Fine()

            '==================================

            Riga_Data("19 Ottobre 2017")

            Riga_Requisiti("Migra '466' :",
                  "")

            Riga_Requisiti("Configurazione_Siti '33' :",
                  "x stampa analisi conformita ")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Esportatore universale :",
                "corretto baco in export operazioni d'agenda")

            Riga_Text("scheda campagna :",
                "Tolto if specifico per stampa diretta PDF per APOT")

            Riga_Text("scheda campagna :",
                  "sistemata indicazione nome appezzamento nella sezione trappole per gli appezzamenti giustificati")

            Riga_Fine()

            '==================================

            Riga_Data("18 Ottobre 2017")

            Riga_Requisiti("Migra '466' :",
                  "x stampa analisi conformita ")

            Riga_Requisiti("Configurazione_Siti '33' :",
                  "x stampa analisi conformita ")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Fattura e DDT Trombin :",
                "aggiunta dicitura intestazione per Trombin Mattia")

            Riga_Fine()

            '==================================

            Riga_Data("16 Ottobre 2017")

            Riga_Requisiti("Migra '466' :",
                  "x stampa analisi conformita ")

            Riga_Requisiti("Configurazione_Siti '33' :",
                  "x stampa analisi conformita ")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Stampa Risultato Analisi Conformità :",
                "rilasciata.")

            Riga_Fine()

            '==================================

            Riga_Data("12 Ottobre 2017")

            Riga_Requisiti("Migra '459' :",
                  "---------------- ")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("ConsistenzeEnologiche.aspx:",
                  "nel caso di data futura, eseguita la stessa query" &
                    "prima usava la vecchia query record statico (era stata lasciata come 'trucchetto' per verificare che il record statico fosse salvato bene, con la giacenza = alla giacenza dinamica" &
                    "su Ruggeri winematch ci sono operazioni future da considerare")

            Riga_Text("BrogliaccioMovimenti :",
                "Griglia: visualizzato anche il lav_cod e l'id_agenda." &
                "- Stile boostrap_AGRONICA.css per formattazione più larga" &
                "- Modifiche per non dover ricaricare la pagina quando si modificano js e css")

            Riga_Text("Scheda Campagna :",
                "aggiornata analisi conformità.")

            Riga_Text("DocumentiContab :",
                "in caso di associazione deve comparire il CF, anche se in realtà rispetta le regole della PIva")


            Riga_Fine()


            '==================================

            Riga_Data("6 Ottobre 2017")

            Riga_Requisiti("Migra '459' :",
                  "---------------- ")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("visualizzatore report.aspx :",
                  "aggiunta gestione dell'opzione di modalità di stampa (diretta su PDF o con anteprima)")

            Riga_Text("Filtro_SchedeMagazzino_new :",
                 "fix errore per cui all'avvio se anche c'era un solo centro ed un solo magazzino, questi non venivano selezionati")

            Riga_Text("DocumentiContab :",
                 "fixbug nota di credito con descrizione prodotto altri beni strumentali doppio")

            Riga_Text("DDT_BolleConf :",
                 "fixbug importo stampato al posto del peso netto in DDT no FF")

            Riga_Text("BrogliaccioMovimenti :",
                "Griglia: introdotta la paginazione per migliorare le prestazioni.")


            Riga_Fine()


            '==================================

            Riga_Data("26 Settembre 2017")

            Riga_Requisiti("Migra '459' :",
                  "---------------- ")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("EsportatoreUniversale_2.aspx :",
                  "Esportatore Imprese e Esportatore Impianti")


            Riga_Fine()


            '==================================

            '==================================

            Riga_Data("18 Settembre 2017")

            Riga_Requisiti("Migra '459' :",
                  "---------------- ")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx :",
                  "CaricaDsTrattamentiPostRaccolta: patch nella creazione della query")

            Riga_Fine()


            '==================================

            Riga_Data("15 Settembre 2017")

            Riga_Requisiti("Migra '459' :",
                  "---------------- ")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Fattura_NotaAccredito.aspx.vb :",
                  "Introduzione modalità liquidazione fattura acconto soci")

            Riga_Text("Fatture e DDT :",
                  "- eliminazione CRFatturaNew.rpt e CRBollaNew.rpt" & vbCrLf &
                  "- eliminazione loghi vecchi di Valmori e Valpiani da crystal" & vbCrLf &
                  "- se usati i report nuovi con logo su file system e il logo non viene trovato, non va più in errore")

            Riga_Fine()


            '==================================

            Riga_Data("11 settembre 2017")

            Riga_Requisiti("Migra '459' :",
                  "---------------- ")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 11/09/2017 per stampe FF")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Stampa bolla di accettazione :",
                    "Modificata per non aggregare le righe per cliente Cofruta")

            Riga_Text("Stampa buono di campionatura :",
                           "Modifiche richieste da Cofruta")

            Riga_Fine()


            '==================================

            Riga_Data("5 settembre 2017")

            Riga_Requisiti("Migra '458' :",
                  "aggiunte colonne tabelle liquidazioni per stampa bolla di campionamento ")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche al 5/09/2017 per stampa bolla di campionamento")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Selezione_SchedaCampagna :",
                    "Tolte le opzioni di magazzino per giacenze e movimenti. Messo come default i Fitosanitari")

            Riga_Fine()


            '==================================

            Riga_Data("1 Settembre 2017")

            Riga_Requisiti("Migra '456' :",
                  "aggiunta colonna ChkLayOut_Riscontrato in Movimenti (in stampe contabili) ")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("LeggiProdottoStampeContab :",
                  "In caso di alias prodotto corretto bug sottoquery restituisce più risultati" &
                  "In caso di alias quando va a cercare info sul lotto deve fare riferimento al prodotto principale, non all'alias")

            Riga_Text("Selezione_SchedaCampagna :",
                    "Aggiunto pulsante per stampe magazzino")

            Riga_Text("Filtro_StampeCantine.aspx :",
                  "aggiornato controllo sul filtro stampe cantine (movimenti che non movimentano il magazzino, escluse Casd e Usd)")

            Riga_Fine()

            '==================================

            Riga_Data("25 Agosto 2017")

            Riga_Requisiti("Migra '456' :",
                  "aggiunta colonna ChkLayOut_Riscontrato in Movimenti (in stampe contabili) ")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DDT e Fatture:",
                  "abilitata la lettura della tab Configurazione_Stampe per tutti (non solo FF). " &
                  "Se non sono presenti voci fallback su vecchio giro con test del cliente su codice")

            Riga_Text("ConferimentoAccettazione:",
                  "Aggiunto distinct in riepilogo conferimenti per evitare doppioni")

            Riga_Fine()

            '==================================

            Riga_Data("24 Agosto 2017")

            Riga_Requisiti("Migra '454' :",
                  "per Agenda")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")


            Riga_Text("Esportatore Universale:",
                  "Esportazione XML e EXcel di Impianti e Imprese")

            Riga_Fine()

            '==================================

            Riga_Data("17 Agosto 2017 (2)")

            Riga_Requisiti("Migra '456' :",
                  "aggiunta colonna ChkLayOut_Riscontrato in Movimenti (in stampe contabili) ")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DDT - Cofruta:",
                  "aggiunto layout pesi + valori riscontrati (utilizza il layout pesi, modificando alcuni parametri)")

            Riga_Fine()

            '==================================

            Riga_Data("17 Agosto 2017")

            Riga_Requisiti("Migra '454' :",
                  "per Agenda")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("ConsistenzeEnologiche",
                  "- gestito il riepilogo per linea e annata (prima era solo per linea)" &
                "- nuova pagina prima del riepilogo" &
                "- sistemata estensione del file di log")

            Riga_Text("RegistroImbottigliamento",
                "modificato titolo in Riepilogo Imbottigliamenti e visualizzato il filtro sul centro aziendale e l'intervallo temporale")

            Riga_Text("BrogliaccioMovimenti",
                "Query: nella parte non ti cantina mancava exists su Materie_PrimexReport" &
                "report: visualizzato il numero dei confezionati")

            Riga_Fine()

            '==================================

            Riga_Data("16 Agosto 2017")

            Riga_Requisiti("Migra '454' :",
                  "per Agenda")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Gestione richieste:",
                  "rimesso come prima, senza utilizzare la sessione perché sennò il LAN si inluppa")

            Riga_Fine()

            '==================================

            Riga_Data("14 Agosto 2017")

            Riga_Requisiti("Migra '454' :",
                  "per Agenda")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Cantine:",
                  "- Prima release del Brogliaccio Movimenti Kendo e Pdf semplificato" &
                  "- Modificato Filtro_StampeCantine.aspx (vedi dettagli su TFS)")

            Riga_Fine()

            '==================================

            Riga_Data("08bis Agosto 2017")

            Riga_Requisiti("Migra '454' :",
                  "per Agenda")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")


            Riga_Text("SchedaCampagna:",
                  "iserite info aggiuntive semina anche per semine previste")

            Riga_Fine()
            '==================================

            Riga_Data("08 Agosto 2017")

            Riga_Requisiti("Migra '454' :",
                  "per Agenda")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")


            Riga_Text("SchedaCampagna:",
                  "corretto bug fasi se specie vegetali non popolate nelle fasi nuove")

            Riga_Fine()

            '==================================

            Riga_Data("07 Agosto 2017")

            Riga_Requisiti("Migra '454' :",
                  "per Agenda")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")


            Riga_Text("SchedaCampagna:",
                  "inserite info aggiuntive semina in note presenti in CaricaDsSemina2 su schede Global se presenti")

            Riga_Fine()
            '==================================

            Riga_Data("04 Agosto 2017")

            Riga_Requisiti("Migra '454' :",
                  "per Agenda")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")


            Riga_Text("CaricaDsRaccolta:",
                  "tolto arrotondamento per superficie raccolta impostabile")

            Riga_Fine()
            '==================================

            Riga_Data("03 Agosto 2017")

            Riga_Requisiti("Migra '454' :",
                  "per Agenda")

            Riga_Requisiti("Altri requisiti :",
                  "Agenda aggiornata almeno al 31/07/2017 per fasi fenologiche")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Report Scheda Campagna:",
                  "modificati per visualizzazione irrigazioni")

            Riga_Text("Rpt_Semina:",
                  "inserito lotto prodotto per GlobalGap")

            Riga_Text("CaricaDSFAsiFeno:",
                  "modificati per nuova gestione dell'operazione")

            Riga_Text("Mastrino:",
                  "corretto baco stampando singolo conto dal LAN")

            Riga_Fine()
            '==================================

            Riga_Data("1 agosto 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.aspx:",
                  "Gestione % uve diraspate (x Ruggeri).")

            Riga_Fine()

            '==================================

            Riga_Data("25 Luglio 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Reg_Impianti.vb:",
                  "modificata query schedaCampagna per inseriemnto campo_nome mancante")

            Riga_Text("schedaCampagna.vb:",
                  "Sistemata per inserimento campo in Raccolta/FaseFeno/Semina PREVISTA (prima non gestita)")

            Riga_Fine()
            '==================================

            Riga_Data("20 Luglio 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Mov_Destinazioni:",
                  "migliorata velocità Leggi_Dettagli_Impianti_2 x export excel mov magazzino")

            Riga_Text(":",
                  "")

            Riga_Fine()

            '==================================

            Riga_Data("17 Luglio 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna:",
                  "sistemato baco mandava errore la stampa del logo se stampavo con date esterne a quelle dell'impianto (che già non andava bene)")

            Riga_Text("SchedaCampagna:",
                  "aggiunta gestione nessuno-nessuno per le avversità dei trattamenti_2")

            Riga_Fine()

            '==================================

            Riga_Data("10 Luglio 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DatatableUtility.vb:",
                  "sistemato baco su ordina = false")

            Riga_Text(" :",
                  "")

            Riga_Fine()


            '==================================

            Riga_Data("6 Luglio 2017")

            Riga_Requisiti("Migra '453' :",
                  "Per bilancio fertilizzazioni core modificato per colcolo rameico")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Stampa DDT Ricevuto :",
                  "corretto bug cedente diverso, inizializzazione stringhe")

            Riga_Text("Liquidazione IVA :",
                  "escluso elem_cod = RIGA_DESCRIZIONE_LIBERA da query")

            Riga_Text("SchedaCampagna.vb :",
                  "modificato per riportare l'elenco degli appezzamenti secondo APP_NOME. modificato core di select_distinct")

            Riga_Text("SchedaCampagna.vb :",
                  "nuova priorità per le precessioni colturali")

            Riga_Text("Documentazione SchedaCampagna:",
                  "inserito file da popolare e da 'coltivare'")

            Riga_Text("Scheda Colturale BIO:",
                  "sistemato il raggruppamento per campo che cancellava un po' troppo")

            Riga_Text("Entrata conferimento:",
                    "Stampe 2010: Modifica alla bolla di conferimento per non stampare ID sulla riga e per non stampare nr e data DDT in caso di auto ddt emesso")

            Riga_Text("SchedaCampagna:",
                    "risolto baco su semine previste")

            Riga_Fine()


            '==================================

            Riga_Data("22 Giugno 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "profilazione per nuove impostazioni stampe sotto")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna Global e APOT:",
                  "Inserita la possibilità di nascondere il campo. (profilabile dall'impostazione dell'utente)")

            Riga_Text("Semine ds_semina e stampe campagna global e apot:",
                  "modificate semine per liberare spazio")

            Riga_Text("scehdaCampagna.vb:",
                  "modificate semine per liberare spazio e gestite varibili profilazione x nascondere il campo")

            Riga_Fine()

            '==================================

            Riga_Data("20 Giugno 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "prof. aggiornata per dose etichetta in stampa")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna Global e APOT:",
                  "Inserita la possibilità di stampare la dose etichetta. (profilabile dall'impostazione dell'utente)")

            Riga_Text("Etichette Trombin e Cofruta:",
                  "Modifiche varie)")

            Riga_Text(" :",
                  "")

            Riga_Fine()

            '==================================

            Riga_Data("8 Giugno 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Patch su stampa documenti contabili :",
                  "AgronicaCoreStampeDAL.DocContab.DocumentiContabili: patch per pezzo di query non supportato su SQL2008.")

            Riga_Text(" :",
                  "")

            Riga_Fine()

            '==================================

            Riga_Data("7 Giugno 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Ds_Semina_Verticale:",
                  "sistemate colonne per visib. Qta seminata")

            Riga_Text("DDT_BolleConf.aspx :",
                  "gestita stampa ddt ricevuto")

            Riga_Fine()

            '==================================

            Riga_Data("5 Giugno 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Bolla accettazione :",
                  "Inserito agente ")
            '==================================


            Riga_Data("31 Maggio 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Report RiBa :",
                  "corretto baco inversione prefisso-suffisso in riferimento documento ")

            Riga_Text("CRFattura, CRBolla :",
                  "rinominata 'causale trasporto' in 'causale' su ddt, ordini, fatture acc")

            Riga_Text("Fattura_NotaAccredito.aspx :",
                  "gestita 'legenda' per gli articoli esclusione iva / non imponibile iva")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "inserito Contrassegno nel filtro dei tipi pagamento")

            Riga_Text("Report Gias F&F :",
                  "modifica emissione DDT per uscita imballaggi: prima non stampava contenitori e confezioni e stampava lordo, tara e netto ")

            Riga_Text("Report Gias F&F :",
                  "modifica etichetta Cofruta per stampare contenitori se non sono presenti imballi")

            Riga_Text("Report Gias F&F :",
          "modifica report riepilogo conferimenti per stampare il peso lordo e i generali totali lordo / netto")

            Riga_Fine()

            '==================================

            Riga_Data("30 Maggio 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DocumentiContab :",
                  "anche con riga descrizione libera si deve portare dietro il riferimento al documento")

            Riga_Text("Report Gias F&F :",
                  "gestione del rapporto contabile 'Fornitore Ortofrutta' i cui movimenti non venivano estratti")

            Riga_Fine()

            '==================================

            Riga_Data("29 Maggio 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("FreshAndFood - Bolla :",
                  "modificata gestione 'stampa a cura del' + patch su vettore + modifica su contatore dettagli")

            Riga_Fine()


            '==================================

            Riga_Data("26 Maggio 2017 - Versione B")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DocumentiContab.vb :",
                  "- Gestito andata a capo e simbolo € anche su 'altri beni strumentali'." &
                  "<br/>- Corretta gestione riga descrizione libera (con l'exit sub rimanevano impostate le variabili sul prodotto precedente)")

            Riga_Text("FreshAndFood - Bolla :",
                  "Vedi TFS")

            Riga_Fine()


            '==================================

            Riga_Data("26 Maggio 2017 - Versione A")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Stampa DDT :",
                  "DocumentiContab.vb: corretto baco stampa tara anche su cantine")

            Riga_Fine()


            '==================================

            Riga_Data("25 Maggio 2017 Versione B")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Stampa DDT :",
                  "DDT_BolleConf.aspx e DocumentiContab.vb: gestita riga descrizione libera + sistemato riquadro vettore")

            Riga_Fine()


            '==================================

            Riga_Data("25 Maggio 2017 Versione A")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Bolla FreshAndFood :",
                  "nuovi sviluppi, vedi TFS")

            Riga_Text("SchedaCampagna:",
                  "corretto baco che non stamapava la portata nelle irrigazioni")

            Riga_Fine()

            '==================================

            Riga_Data("16 Maggio 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Selezione_Quadro_P:",
                  "inserita possibilità di stampare solo se superfici occupate")

            Riga_Text("Quadro_P:",
                  "Modificato per poter stampare solo le particelle occupate")

            Riga_Text("AnagraficaAziendale (core):",
                  "Modificata Query QuadroP per considerare o meno le sole sup. utilizzate")

            Riga_Text("SchedaColturaleBiologico:",
                  "Modificato Resp.Aziendale: ora non prende più la Piva del Superuser ma quello dell'azienda")

            Riga_Fine()

            '==================================

            Riga_Data("12 Maggio 2017")

            Riga_Requisiti("Migra '430' :",
                    "Aggiunta di colonne Cod_RisUm_Aggiuntivo e Cod_Indirizzo_Aggiuntivo in Movimenti")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            'f&f

            Riga_Fine()

            '==================================


            Riga_Data("10 Maggio 2017")

            Riga_Requisiti("Migra '430' :",
                    "Aggiunta di colonne Cod_RisUm_Aggiuntivo e Cod_Indirizzo_Aggiuntivo in Movimenti")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Fattura :",
                  "Cambiato titolo documento da Fattura Immediata a Fattura Accompagnatoria")

            Riga_Text("DDT :",
                  "Cambio dicitura legge per campioni gratuiti" & vbCrLf &
                  "stampa anche dei metodi pagamento in DDT SOLO per DEFAVERI")

            Riga_Text("EstrattoConto_ClientiFornitori :",
                  "- Pagamenti.aspx: visualizzata data del pagamento avvenuto;" &
                  "- Rpt_Pagamenti.rpt: ampliata visualizzazione dettagli pagamento previsto e avvenuto.")

            Riga_Fine()

            '==================================

            Riga_Data("8 Maggio 2017")

            Riga_Requisiti("Migra '430' :",
                    "Aggiunta di colonne Cod_RisUm_Aggiuntivo e Cod_Indirizzo_Aggiuntivo in Movimenti")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Schede magazzino:",
                  "Cambiati Double in Decimal per errori di conversione!!")

            Riga_Text(" :",
                  "")

            Riga_Fine()

            '==================================

            Riga_Data("24 Aprile 2017")

            Riga_Requisiti("Migra '430' :",
                   "Aggiunta di colonne Cod_RisUm_Aggiuntivo e Cod_Indirizzo_Aggiuntivo in Movimenti")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("RegistroImbottigliamento :",
                  "AgronicaCoreStampeDAL.RegistriCantina.RegistroImbottigliamento: modificata query per leggere il colore non da materie_prime ma dalla tabella di log.")

            Riga_Text("EtichetteVascheEnologiche :",
                  "Filtro_StampeCantine.aspx: corretto, non recuperava il fornitore del c/lavoro + gestita stampa del lotto e del semilavorato (alternativo al nome della linea)." &
                  "Rpt_EtichetteVasche.rpt: gestiti nuovi dati e modificato layout per linea e lotto su più righe")

            Riga_Text("Bilancio.vb :",
                  "Corretto layout CE Europeo che considerava due volte il conto E.22.")

            Riga_Fine()

            '==================================

            Riga_Data("11 Aprile 2017")

            Riga_Requisiti("Migra '430' :",
                   "Aggiunta di colonne Cod_RisUm_Aggiuntivo e Cod_Indirizzo_Aggiuntivo in Movimenti")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna.vb:",
                  "Corretto Baco seconda colonna rilievo piogge")

            Riga_Text("SchedaMovimentiMagazzino :",
                  "Migliarato arrotondamento usando decimal e MidpointRounding.AwayFromZero")

            Riga_Text("SchedaGiacenzeMagazzino :",
                  "Possibilità di raggruppamento per prodotto (no divisione per cal_cod) in base a opzione su filtro stampa.")

            Riga_Text("SchedaVenditeBiologico :",
                  "Query LeggiVendite raggruppa sul prodotto (sempre).")

            Riga_Fine()

            '==================================
            Riga_Data("31 Marzo 2017")

            Riga_Requisiti("Migra '430' :",
                    "Aggiunta di colonne Cod_RisUm_Aggiuntivo e Cod_Indirizzo_Aggiuntivo in Movimenti")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna :",
                  "Nelle visite ispettive non appariva il nome se l'ispettore era una persona fisica")

            Riga_Fine()

            '==================================

            Riga_Data("23 Marzo 2017")

            Riga_Requisiti("Migra '430' :",
                    "Aggiunta di colonne Cod_RisUm_Aggiuntivo e Cod_Indirizzo_Aggiuntivo in Movimenti")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("RegistroCommercializzazioneVini e RegistroVinificazione :",
                  "Aumentato timeout per l'esecuzione delle query.")

            Riga_Text("AgronicaCoreDataProvider :",
                  "Aggiunta proprietà TimeoutQuery all'objparametri.")

            Riga_Fine()

            '==================================

            Riga_Data("22 Marzo 2017")

            Riga_Requisiti("Migra '430' :",
                    "Aggiunta di colonne Cod_RisUm_Aggiuntivo e Cod_Indirizzo_Aggiuntivo in Movimenti")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("RPT_Campagna e Global singolo e multiplo:",
                  "aggiornato per gestire le note di semina")

            Riga_Text("SchedaCampagna.aspx.vb:",
                  "inserite le note della semina + gestita operazione di strigliatura")

            Riga_Text("GestioneRichieste.aspx :",
                  "- Log errore su XML_EstraiVariabiliStampe2." &
                  "- nuova opzione di stampa layout ricevuta fiscale.")

            Riga_Text("RicevutaFiscale :",
                  "- Rpt_RicevutaFiscaleA4aCapoAuto.rpt: generato a partire da Rpt_RicevutaFiscaleA4.rpt ma con 'a capo' automatico." &
                  "- RicevutaFiscaleA4ACapoAuto.aspx: pagina di gestione")

            Riga_Text("Filtro_StampeCantine.aspx :",
                  "Recepito mat_des inviato dal giaslan.")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "Introdotto alert sulla data di inizio gestione contabile.")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti.vb:",
                  "Saldo_Conti_Patrimoniali: la data di inizio viene impostata dalla data di inizio gestione contabile.")

            Riga_Text("DocumentiContab.vb:",
                  "Arrotondato a due decimali il peso netto nella descrizione prodotto FF")

            Riga_Fine()


            '==================================

            Riga_Data("8 Marzo 2017")

            Riga_Requisiti("Migra '430' :",
                  "Aggiunta di colonne Cod_RisUm_Aggiuntivo e Cod_Indirizzo_Aggiuntivo in Movimenti")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_SchedeBiologico:",
                  "inserita combo del magazzino")

            Riga_Text("SchedaMateriePrimeBiologico:",
                  "Inserita gestione filtro per Magazzino e cod_Resp_BIO")

            Riga_Text("SchedaVenditeBiologico:",
                  "Inserita gestione filtro per Magazzino e cod_Resp_BIO")

            Riga_Text("RegistroPreparazioniBio:",
                  "Inserita gestione filtro per Magazzino")

            Riga_Text("LeggiProdottoStampeContab :",
                  "Aggiunta lettura di codice articolo da CAC_Codifiche per prodotti generali")

            Riga_Text("CRFattura_Cofruta e CRBolla_Cofruta :",
                  "Creazione report a partire da Trombin")

            Riga_Text("Fattura_NotaAccredito e DDT_BolleConf :",
                  "Aggiunta gestione di Cessionario aggiuntivo")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "Inserito check EscludiIvaIndetraibile che si attiva solo per i conti CE di costo.")

            Riga_Text("Mastrino :",
                  "Gestito il check EscludiIvaIndetraibile." &
                  "MastrinoConti_Economici e Query_Conti_Economici_Movimentati: gestito flag per escludere o meno la lettura dell'iva indetraibile sul conto di costo.")

            Riga_Fine()

            '==================================

            Riga_Data("01 Marzo 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "Profilazione aggiornata per Stampe BIO")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("RtpStampaColturaleBIO:",
                  "revisionata")

            Riga_Text("SchedaColturaleBiologico.aspx:",
                  "inseriti campo appBIO e app impostabile come nella campagna + op di strigliatura")

            Riga_Fine()

            '==================================

            Riga_Data("27 Febbraio 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaGiacenzeMagazzino_2.aspx :",
                  "SchedaGiacenzeMagazzino: gestiti i ricambi e i carburanti aziendali.")

            Riga_Text(" :",
                  "")

            Riga_Fine()

            '==================================

            Riga_Data("20 Febbraio 2017")

            Riga_Requisiti("Migra '428' :",
                  "Aggiunta colonna CertificazioneInDescrizione in Configurazione_Stampe")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Leggi_MovimentoDettaglio_DocContabile :",
                  "cambiata dicitura capacità/peso netto confezione in descrizione prodotto se FF" & vbCrLf &
                  "eliminata parentesi di chiusura orfana in descrizione se prodotto sfuso" & vbCrLf &
                  "aggiunta lettura di certificazione prodotto all'interno della descrizione")

            Riga_Text("CRBolla.rpt :",
                  "rimpicciolito caratteri di Articolo 62, per renderlo uguale agli altri")

            Riga_Text("CRBolla_Trombin.rpt :",
                  "Aggiunta paese origine Italia + altre sistemazioni")

            Riga_Fine()

            '==================================

            Riga_Data("15 Febbraio 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaColturale BIO:",
                  "Revisionata più note")

            Riga_Text(" :",
                  "")

            Riga_Fine()

            '==================================

            Riga_Data("14 Febbraio 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Fattura_NotaAccredito :",
                  "sistemato bug peso complessivo che si sovrappone ad altri dati" & vbCrLf &
                  "corretto bug su nuova versione di stampa rif Bolla in Fattura se c'è raggruppamento dei dettagli")

            Riga_Text("Nota Accredito :",
                  "Corretto bug per cui non stampava più il riferimento alla fattura")

            Riga_Text("Scheda Campagna:",
                  "Corretto bug in CaricaDS_Trattamenti nei prodotti senza principio attivo")

            Riga_Fine()

            '==================================

            Riga_Data("02 Febbraio 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "- Gias_Configurazione_Siti_18 x report agenti provvigioni")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DDT e Fatture :",
                  "riazzerata la stringa dettagli evasione se ho scelto di non stampare il riferimento all'ordine")

            Riga_Text("Filtro_StampeCantine.aspx :",
                  "Controlli_x_RegistriCantina -> Leggi_OpMagazzino_JollyInt1 eliminate le GIIN dalla lettura.")

            Riga_Fine()

            '==================================

            Riga_Data("01 Febbraio 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "- Gias_Configurazione_Siti_18 x report agenti provvigioni")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("AgentiProvvigioni :",
                  "AgentiProvvigioni_XLS.aspx: porting da stampe_2003.")

            Riga_Text("CRBolla_Trombin :",
                  "Modifiche minimali layout grafico report crystal")

            Riga_Text("Bolle varie :",
                  "correzione peso lordo troppo spostato se non ci sono i litri totali")

            Riga_Fine()

            '==================================

            Riga_Data("30 Gennaio 2017")

            Riga_Requisiti("Migra '425' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx :",
                  "Per APOT, anziché avere il visualizzatore report, apre subito un PDF")

            Riga_Text(" :",
                  "")

            Riga_Fine()

            '==================================

            Riga_Data("27 Gennaio 2017")

            Riga_Requisiti("Migra '425' : Prezzo_Livello in Movimenti_Dettagli e nuove colonne in Configurazione_Stampe",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("CRBolla_Trombin.rpt:",
                  "sistemazione documento per cofruta")

            Riga_Text("CRFattura_Trombin.rpt:",
                  "sistemazione documento per cofruta")

            Riga_Text("DocContab.vb:",
                  "aggiunto Livello_Prezzo, QtaDettaglio1 e QtaDettaglio2 in DocumentiContabili")

            Riga_Text("StampeDAL->Utility:",
                  "ampliata struttura ConfigurazioneStampe")

            Riga_Text("DDT_BolleConf:",
                  "modifiche a gestione personalizzazioni su tabella Configurazione_Stampe (per il momento solo F&F)")

            Riga_Text("Fattura_NotaAccredito",
                  "gestione personalizzazioni su tabella Configurazione_Stampe (per il momento solo F&F)")

            Riga_Fine()

            '==================================


            Riga_Data("17 Gennaio 2017")

            Riga_Requisiti("Migra '423' : Ordine_Det in Movimenti_Dettagli e nuove colonne in Configurazione_Stampe",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("CRBolla_Trombin.rpt:",
                  "sistemazione documento per cofruta")

            Riga_Text("DocContab.vb:",
                  "aggiunto ordinamento per Ordine_Det in DocumentiContabili")

            Riga_Text("StampeDAL->Utility:",
                  "aggiunta struttura ConfigurazioneStampe")

            Riga_Text("DDT_BolleConf:",
                  "gestione personalizzazioni su tabella Configurazione_Stampe (per il momento solo F&F)")

            Riga_Text("Gestione_Richieste.aspx:",
                  "ora la stampa con dettagli economici di DDT passa da DDT e non più come la fattura accompagnatoria (per il momento solo F&F)")

            Riga_Fine()

            '==================================

            Riga_Data("10 Gennaio 2017")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_SchedeBiologico:",
                  "Sistemata pagina (non era stata adattata dopo il passaggio del BIO al 2010 per il LAN)")


            Riga_Fine()

            '==================================

            Riga_Data("28 Dicembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "Configurazione Siti 17 per aggiunta chiave CodRisUm_Personalizzazione_Doc_Contabili_1")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Bolle e Fatture :",
                  "aggiunte info agente")

            Riga_Text("CRBolla_Trombin:",
                  "aggiunti doc/ sezioni per SBTF e CoFruTa")

            Riga_Text("CRFattura_Trombin :",
                  "aggiunti doc/ sezioni per SBTF e CoFruTa")

            Riga_Fine()

            '==================================

            Riga_Data("27 Dicembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Mastrino, Piano Conti e Bilancio :",
                  "aggiunta gestione compensazione IVA Estero e sistemazione IVA" & vbCrLf &
                  "aggiunta gestione sezionale per ogni conto come è stato mappato")

            Riga_Text("Liquidazione IVA :",
                  "aggiunto credito di imposta su estero, migliorata stampa per regimi speciali e misti")

            Riga_Text("Registri IVA Vendite, Acquisti e registri corrispettivi :",
                  "aggiornato specchietto riepilogo come la nuova liquidazione")

            Riga_Fine()

            '==================================

            Riga_Data("21 Dicembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna:",
                  "Risolto caricamento AltreOperazioniColturali con sa_cod non valorizzato (lo filtrava con 0)")

            Riga_Text(" :",
                  "")

            Riga_Fine()

            '==================================


            Riga_Data("20 Dicembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_SchedeMagazzino_new:",
                  "Sistemato per la Zucca per esportazione eXcel multi-magazzino.. La sfido a ritrovare un baco su questo")

            Riga_Text(" :",
                  "")

            Riga_Fine()

            '==================================

            Riga_Data("15 Dicembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Seleziona_SchedaCampagna.aspx:",
                  "ott. controllo selezione nel filtro stampa" & vbCrLf &
                  "Inserito check per 'viasualizza finalità' delle stampe")

            Riga_Text("SchedaCampagna:",
                  "Inserite correttamente le note nella raccolta CaricaDsRaccoltaNEW" & vbCrLf &
                  "Gestite le finalità nella testata e nelle erbaceee se selezionate dal filtro")

            Riga_Text("AgendaXNote.aspx:",
                  "inserito core per ricevere l'elenco delle note predefinite (quelle checkable) all'operazione")

            Riga_Text("FiltroSchedaMagazzino.aspx:",
                  "Ottimizzato per selezione centro e stampa solo se ho i dati necessari")


            Riga_Fine()

            '==================================

            Riga_Data("02 Dicembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Giornale Contabile :",
                  "Risolto errore su stampa saldi iniziali anche quando non voluti")

            Riga_Text("Seleziona_SchedaCampagna.aspx:",
                  "non tirava su il check sul fitoregolatore")

            Riga_Text("SchedaCampagna.vb:",
                  "baco sul frontespizio delle multispecie")

            Riga_Fine()

            '==================================

            Riga_Data("01 Dicembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampaqgna.aspx:",
                  "Escluse le reccolte e semine previste con data fuori dall'intervallo della stampa")

            Riga_Text("Scheda di Campagna.rpt Singola e Multicentro:",
                  "Reinserito (questa volta in entrambe le schede) il prog cod - codice associato alla domanda allegata ER")

            Riga_Text("SchedaCampagnaSingola.rpt:",
                  "Risolto errore sul collegamento del dataset delle arboree")

            Riga_Fine()

            '==================================


            Riga_Data("29 Novembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "Script Configurazione Siti ver 16 per Registri BIO")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_ElaboratiContabili : ",
                  "Tolta spunta di default su trimestre in corso per stampa registri IVA vendite ed acquisti")

            Riga_Text("Scheda Campagna :",
                  "- ottimizzata lettura costi accessori" &
                  "- introdotto filtro Piva su lettura costi accessori (id_agenda non è univoco)")

            Riga_Fine()

            '==================================

            Riga_Data("28 Novembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "Script Configurazione Siti ver 16 per Registri BIO")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda campagna :",
                  "Inserito parco macchine revisionato")

            Riga_Text("Rpt_ParcoMacchine e Rpt_ParcoMacchine_Verticale :",
                  "Inserito parco macchine revisionato")

            Riga_Text("Ds_ParcoMacchine :",
                  "Inserito parco macchine revisionato")

            Riga_Text("Scheda campagna :",
                  "Inserito parco macchine revisionato" & vbCrLf &
                  "Inseriti i loghi delle regioni dinamici" & vbCrLf &
                  "Corretto baco date semine e raccolte nel carica impianti multispecie")

            Riga_Text("AB_Immagini :",
                  "Inseiriti i loghi delle regioni")

            Riga_Text("Rpt vari:",
                  "revisionati")

            Riga_Text("Selezione_SchedaCampagna:",
                  "ora disabilita i report che non può stampare in quella scheda.. inserite costanti per report")

            Riga_Text("Schede/Registri Biologico:",
                  "Schede Materie Prime BIO, Schede Vendite BIO e Registri Preparazioni BIO" & vbCrLf &
                  "sono stati portati nelle Stampe 2010 + correzione di qualche bug")

            Riga_Text("RPT_RegistroVuoto:",
                  "corretto bug che tranciava dati impresa a destra in intestazione")

            Riga_Fine()

            '==================================

            Riga_Data("22 Novembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda campagna :",
                  "corretto baco lettura tecnico riferimento.")

            Riga_Fine()

            '==================================

            Riga_Data("21 Novembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Liquidazione IVA :",
                  "Stampando tutti i sezionali metteva sempre l'interesse al 1%, anche se sono tutti mensili")

            Riga_Text("Mastrino e Bilancio :",
                  "Ritenute d'acconto ed Enasarco non erano correttamente visibili")

            Riga_Fine()

            '==================================

            Riga_Data("14 Novembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("CRFattura2016.rpt :",
                  "Aveva smarrito il collegamento ai dettagli del sottoreport sull'IVA")

            Riga_Text("SchedaCampagna:",
                  "- Modificato il cericaDsRilievoPiogge" & vbCrLf &
                  "- Aggiunti i centri aziendali nel DsCentriAzinedali dopo aver caricato i dataset per" &
                  " inserire anche i centri che hanno magazzini movimentati nella stampa")

            Riga_Fine()

            '==================================

            Riga_Data("10 Novembre 2016")

            Riga_Requisiti("Migra '414' : ",
                  "Aggiunta colonna ChkLayOut_Litri in Movimenti per stampa DDT e Fatture")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Fattura / DDT con dett.economici :",
                  "se sconto merce non è il primo, manteneva l'importo della voce precedente")

            Riga_Text("Fatture e DDT :",
                  "Inserito conteggio Litri per prodotto e Totali")

            Riga_Fine()

            '==================================

            Riga_Data("09 Novembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DS_Irrigazione:",
                  "modificato campo data_creazione a String per incompatibilità")

            Riga_Text("DS_RilievoPiogge:",
                  "aggiunti campi per il rif. al centro")

            Riga_Text("Rpt_RilievoPiogge_verticale e Rpt_RilievoIrrigazione_orizzontale:",
                  "Aggiunti per inserire il centro nelle multicentro")

            Riga_Text("Tutte le stampe principali:",
                  "Modificate per inerimento dei nuovi report delle piogge")

            Riga_Fine()

            '==================================

            Riga_Data("07 Novembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Selezione_SchedaCampagna:",
                  "tolta la visualizzazione delle schede vuote nel caso di errore" & vbCrLf &
                  "introdotto utilizzo dei solo valori alfabetici per riferirsi alle schede (cambiati i macchinare come y)")

            Riga_Text("Scheda campagna, rpt_scheda_campagnamulti:",
                  "Inserito report irrigazione + gestione del multi-specie x scheda Campagna Lombardia" & vbCrLf &
                  "cambiati i macchinare come y solo scheda_Campagna")

            Riga_Fine()

            '==================================
            Riga_Data("04 Novembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "Config. siti 15 per StampaLombardia" &
                  "e Agenda aggiornata al 11/2016")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DDT e Fatture:",
                  "Se collegate ad un ordine viene stampato anche quantitativo evaso su totale")

            Riga_Text("Rpt_AltreOperazioniCulturali_Multi.rpt:",
                  "Aggiunto per stampa lombardia")

            Riga_Text("Rpt_CopertinaLombardia.rpt:",
                  "Aggiunto per stampa lombardia")


            Riga_Text("Rpt_SchedaCampagnaMulti.rpt:",
                  "modifica report per scheda campagna lombardia")

            Riga_Text("Selezione_SchedaCampagna:",
                  "aggiunta gestione scheda campagna lombardia")

            Riga_Text("SchedaCampagna:",
                  "aggiunta gestione scheda campagna lombardia")

            Riga_Text("GestioneRichieste:",
                  "aggiunta gestione scheda campagna lombardia")

            Riga_Text("DS_AltreOperazioniColturali:",
                  "aggiunto campo per inserire il dataset nel multispecie")
            Riga_Fine()

            '==================================

            Riga_Data("28 Ottobre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx.vb:",
                  "sistemato il caricamento dei costi accessori per APOT")

            Riga_Text("DocumentiContab.vb :",
                  "Aggiunta lettura indirizzi da CentrixIndirizzi se si tratta dell'impresa stessa ma l'indirizzo non è presente in ImpresaxIndirizzi")

            Riga_Text("cls_SottoReport_IVA.vb :",
                  "L'IVA in compensazione deve essere calcolata anche sulle vendite estere esenti da IVA")

            Riga_Text("Mastrino :",
                  "Corretto bug se si sceglie una banca, vengono su tutte le fatture professionisti (anche delle altre banche/casse)")


            Riga_Fine()

            '==================================

            Riga_Data("26 Ottobre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx.vb:",
                  "sistemato il caricamento delle piogge")

            Riga_Text("  :",
                  "")

            Riga_Fine()

            '==================================

            Riga_Data("25 Ottobre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Mastrino e Estratto Conto Contatto :",
                  "Accorpamento di più dettagli per lo stesso documento in un'unica riga (esclusi pagamenti e incassi)")

            Riga_Text("Mastrino :",
                  "Stampando più conti, i saldi vengono riazzerati ad ogni conto, senza portare dietro i saldi di riporto del precedente")

            Riga_Text("Bilancio :",
                  "Aggiunta possibilità di stampare solo CE e/o SP e miglioramento opzioni stampa preliminari")

            Riga_Fine()

            '==================================
            Riga_Data("21 Ottobre 2016")

            Riga_Requisiti("Migra '409' :",
                  "per Stampa APOT")

            Riga_Requisiti("Altri requisiti :",
                  "Config. Siti 14 per Stampa APOT")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda campagna :",
                  "Gestito Gestione Rifiuti")

            Riga_Text("GestioneRifiuti.rpt:",
                  "Modificato per APOT")
            Riga_Text("DS_GestioneRifiuti.xsd:",
                  "Modificato per APOT")

            Riga_Text("tutti i Sottoreport:",
                  "modificati i bordi")

            Riga_Fine()

            '==================================

            Riga_Data("18 Ottobre 2016")

            Riga_Requisiti("Migra '409':",
                  "Per Apot, versione di passaggio delle stampe nuove. Non definitiva!")

            Riga_Requisiti("Altri requisiti :",
                  "Stampe aggiornate per APOT")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Selezione_SchedaCampagna:",
                  "Aggiunti check per stampa nuova di APOT")

            Riga_Text("SchedaCampagna:",
                  "Aggiunta stampa Prov Aut Trento con i report: formazione, manca caricaDsGestioneRifiuti (Attesa sviluppo Fede)")

            Riga_Text("Rpt_SchedaCampagna_Prov_Aut_Trento:",
                  "aggiunto report per APOT")

            Riga_Text("Ds_Formazione:",
                  "Aggiunto per nuovo report")

            Riga_Text("Rpt_Formazione_Orizzontale.rpt:",
                  "Aggiunto per stampa nuova")

            Riga_Text("Rpt_GestioneRifiuti_Orizzontale.rpt:",
                  "Aggiunto per stampa nuova")

            Riga_Text("Bilancio e Mastrino:",
                  "Gestione corretta di Fattura Professionista che venivano duplicate nel capomastro")

            Riga_Text("SchedaCampagna.aspx.vb",
                  "Modificato il caricaDS impianti multispecie che non funzionava il registro trattamenti a causa del contabilizzato")

            Riga_Text("selezione_SchedaCampagna.aspx.vb",
                  "Deselezionato il check autoamatico della verifica di conformita che non piaceva a Motti e i clienti")

            Riga_Fine()

            '==================================
            Riga_Data("07 Ottobre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Trombin DDT e Fattura:",
                  "Aggiunta di descrizione addizionale sulla descrizione del prodotto")

            Riga_Text("Trombin DDT:",
                  "Correzzione bug layout")

            Riga_Text("Trombin Etichette ingresso Conferimenti:",
                  "Aggiunta di campo note prodotto")

            Riga_Fine()

            '==================================

            Riga_Data("01 Ottobre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "Per le pian (NON BLOCCANTE quindi non obbligatorio richiede agenda)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.aspx:",
                  "Corretto baco in stampa DDT con Layout dettagli raggruppati")

            Riga_Text("SchedaCampagna :",
                  "Inserito filtro su dettaglio_movimenti.contabilizzato che se è < 0 non deve essere pianificato perché" &
                  "è una pianificazione d'agenda (in particolare: modificati i caricaDs)")

            Riga_Fine()

            '==================================

            Riga_Data("30 Settembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna:",
                  "Sistemato baco per terreno nudo nel caricaDsErbacee + Aggiunta eliminazione delle arboree nel caso di terreno nudo")

            Riga_Text("  :",
                  "")

            Riga_Fine()


            '==================================

            Riga_Data("28 Settembre 2016 (2)")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Bilancio.aspx:",
                  "Corretto baco non stampare saldi 0 in bilancio" & vbCrLf &
                  "Aggiunta possibilità di stampare o meno tutti i dettagli dei conti automatici Crediti Clienti, Banche, Casse, Debiti Fornitori in SP")

            Riga_Fine()

            '==================================



            Riga_Data("28 Settembre 2016")

            Riga_Requisiti("Migra '404' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx.vb:",
                  "corretto baco (virgola) in CaricaDsRilieviAvv nelle trappole")

            Riga_Fine()

            '==================================

            Riga_Data("26 Settembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_SchedeMagazzino_new :",
                  "Verifica funzionamento + aggiunta movimenti (carichi e scarichi) + modificate le combo con le ricerche")

            Riga_Text("SchedaMovimentiMagazzino e schedaMovimentiMagazzino_excel:",
                  "Inserita possibilità di stampare solo carichi o solo scarichi")

            Riga_Text("SelezioneSchedaCampagna:",
                  "Inserito check Capitolo Privato")

            Riga_Text("SchedaCampagna:",
                  "Modificato report Erbacee per inserire il Capitolo Privato")

            Riga_Text("DS_Erbacee:",
                  "Inserita colonna Progetto_cod")

            Riga_Text("Bilancio:",
                  "Aggiunti parametri preliminari" & vbCrLf &
                  "Modificati C.E. e S.P perché sommino ed estrapolino tutti i dati")

            Riga_Fine()

            '==================================

            Riga_Data("19 Settembre 2016")

            Riga_Requisiti("Migra '404' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda campagna:",
                  "Sistemato l'arrotondamento personalizzabile" & vbCrLf &
                  "Sistemato: metteva un logo sull'altro nella scheda di campagna nel caso in cui fosse deselezionato il frontespizio")

            Riga_Text("Rpt_SchedaCampagna Semplificata e Multicentro :",
                  "Sistemato report rilievi avversità ")

            Riga_Text("Rpt_RilieviAvversità_Verticale :",
                  "Aggiunto per migliorare le stampe di campagna")

            Riga_Text("Selezione_Scheda_Campagna :",
                  "Sistemato Baco che non prendeva gli appezzamenti filtrati ma tutti quelli della specie")

            Riga_Text("Tutte e 4 le stampe :",
                  "Revisione layout")

            Riga_Text("Scheda campagna (standard + global):",
                  "aggiunta sezione controlli conformita")

            Riga_Fine()

            '==================================

            Riga_Data("12 Settembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Selezione_SchedaCampagna:",
                  "- inserita la possibilità di escludere i fitofarmaci dall'elenco" & vbCrLf &
                  "- inserito il controllo del permesso per visualizzare o meno il check delle pratiche ecologiche" & vbCrLf &
                  "- inseriti menu a tendina scelta centro e specie")

            Riga_Text("SchedaCampagna:",
                  "- in base alla selezione esclude o meno i fitofarmaci dal Ds di tutte le stampe" & vbCrLf &
                  "- in base alla selezione esclude o meno le pratiche ecologiche da tutte le stampe")

            Riga_Text("RegistroVinificazione:",
                  "ridotti i bordi dei report per segnalazione cliente")

            Riga_Text("SchedaCampagna, SchedaCamapagnaMulticentro, GlobalGap, GlobalGapMulticentro:",
                  "inserita la parte delle pratiche ecologiche")

            Riga_Text("DsChecklistDS:",
                  "dataset per pratiche ecologiche")
            Riga_Text("Rpt_Checklist:",
                  "sottoreport per pratiche ecologiche verticale")
            Riga_Text("Rpt_Checklist_Orizzontale2:",
                  "sottoreport per pratiche ecologiche orizzontale (2 perché il primo aveva avuto problemi)")

            Riga_Text("Bolle/Fatture:",
                  "Inseriti i loghi per Fiorentina Di Sopra che ora usa CRFattura2016 e CRBolla2016")

            Riga_Fine()

            '==================================


            Riga_Data("05 Settembre 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Ricevuta Fiscale :",
                  "Stampa di 'Contributo CONAI' anche su ricevute fiscali (A4 e A5)")

            Riga_Text("DocumentiContab.vb :",
                  "Corretto bug in Leggi_Movimenti_DocContabile se targa mezzo è NULL")

            Riga_Fine()

            '==================================


            Riga_Data("29 Agosto 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx.vb :",
                  "Corretto baco che non caricava il campo_des in CaricaDsRilievoIndiciMaturita()")

            Riga_Fine()

            '==================================

            Riga_Data("18 Agosto 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("BilancioFertilizzazioni_XLS  :",
                  "Corretto calcolo N-P-K Totali (calcolati in base alla sup_trattata ora).")

            Riga_Fine()

            '==================================

            Riga_Data("11 Agosto 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx.vb:",
                  "Inserita la mod. per vedere i mc nella scheda delle fertilizz")

            Riga_Text("BilancioFertilizzazioni_XLS  :",
                  "Corretto calcolo apporto digestato.")

            Riga_Fine()

            '==================================


            Riga_Data("10 Agosto 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx.vb :",
                  "Eliminato ultimo riferimento alla doppia riga")

            Riga_Fine()

            '==================================
            Riga_Data("09 Agosto 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda di camapagna singolo centro:",
                  "Inserita modifica per rif campo (non si vede nel singolo centro)")

            Riga_Text("Scheda di camapagna multicentro:",
                  "Inserita modifica per rif campo ")


            Riga_Text("Scheda GLOBAL GAP singolo centro:",
                  "Inserita modifica per rif campo (non si vede nel singolo centro)")

            Riga_Text("Scheda GLOBAL GAP multicentro:",
                  "Inserita modifica per rif campo ")

            Riga_Text("Report Campagna e GLOBAL:",
                  "Allineati alcuni di essi per rendere più uniforme la visualizzazione")

            Riga_Fine()
            '==================================

            Riga_Data("08 Agosto 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Text("AgronicaCoreAnagrafeDAL.Imprese_Codici.vb:",
                  "Modificato tecnicoRif from piva per ottenere anche i contatti pubblici")

            Riga_Text("  :",
                  "")

            Riga_Fine()


            '==================================

            Riga_Data("05 Agosto 2016 ")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Stampa GLOBALGAP :",
                  "Aggiunto Tecnico di riferimento e Codice Socio/Produttore associato alle impstazioni")

            Riga_Text("Stampa GLOBALGAP Multicentro:",
                  "Aggiunto Tecnico di riferimento e Codice Socio/Produttore associato alle impstazioni")

            Riga_Text("Stampa CAMPAGNA Multicentro :",
                  "Aggiunto Tecnico di riferimento e Codice Socio/Produttore associato alle impstazioni")

            Riga_Text("Stampa CAMPAGNA :",
                  "Aggiunto Tecnico di riferimento e Codice Socio/Produttore associato alle impstazioni")

            Riga_Fine()

            '==================================

            Riga_Data("27 Luglio 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DDT: ",
                  " Correzione dati trasportatore ")

            Riga_Text("  :",
                  "")

            Riga_Fine()

            '==================================

            Riga_Data("25 Luglio 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx.vb: ",
                  " -Corretto BACO rilievi avv. e erbe infestanti su Nome_App_Breve per report (Segn. Martini APOT)")

            Riga_Text("  :",
                  "")

            Riga_Fine()

            '==================================

            Riga_Data("22 Luglio 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")


            Riga_Text("Fatture/Bolle :",
                 " Messo opzionale rif. all'ordine in DDT / FATTURA ")

            Riga_Fine()

            '==================================


            Riga_Data("21 Luglio 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna :",
                  "corretto baco errore rilievi in campo con macchine inserite nei costi accessori (aggiunto dettaglio ditta).")

            Riga_Text("Selezione_SchedaCampagna.aspx :",
                  "corretto baco che salvava male le colture precedenti.")

            Riga_Fine()

            '==================================

            Riga_Data("18 Luglio 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")


            Riga_Text("Bolle :",
                 " aggiunti altri dati del trasportatore ")

            Riga_Fine()

            '==================================


            Riga_Data("14 Luglio 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("BilancioFertilizzazioni_XLS :",
                  "- spostata da stampe 2003;" &
                  "- aggiunti campi (campo, anno impianto, finalita, regolamento).")

            Riga_Text("Filtro_SchedeMagazzino_new :",
                 "- sistemato bug su date per esportazione excel + visibilità bottoni")

            Riga_Fine()

            '==================================

            Riga_Data("08 Luglio 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Selezione_SchedaCampagna.aspx :",
                  "rilasciata prima versione verifica conformità (momentaneamente commentata in attesa di gestire problema sessione in passaggio tra siti agenda-stampe-agenda)")

            Riga_Text("Filtro_SchedeMagazzino_new :",
            "- sistemato bug su passaggio client per valori selezionati tramite sessione per stampa magazzino")

            Riga_Text("  :",
                  "")

            Riga_Fine()

            '==================================


            Riga_Data("07 Luglio 2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_SchedeMagazzino_new :",
                  "- sistemato bug su passaggio client per valori selezionati tramite sessione per stampa magazzino" &
                    "- caricamento parametri client tra progetti")

            Riga_Fine()

            '==================================

            Riga_Data("05 Luglio 2016")

            Riga_Requisiti("Migra '395' :",
                        "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Bolle/Fatture",
                " Modificato Test su Piva Colombarda in logo perché cambiata")

            Riga_Text("Stampa Global Gap :",
                  "Aggiunta gesione delle note ricorrenti / Giustificazioni nel report della Global Gap")

            Riga_Fine()


            '==================================


            Riga_Data("30 Giugno 2016")

            Riga_Requisiti("Migra '395' :",
                        "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Etichette:",
                "- Modificata procedura di generazione codice PDF417")

            Riga_Text("Bolla Accettazione Ricevuta:",
                " Creato layout x CO.FRU.TA + modifiche Bolle/Fatture x gestione logo")

            Riga_Text("Bolle/Fatture",
                " Corretta funzione per Caricare Logo (in basso) In Campo Blob di Crystal")

            Riga_Fine()


            '==================================

            '==================================


            Riga_Data("29 Giugno 2016")

            Riga_Requisiti("Migra '395' :",
                        "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Etichette:",
                "- Modifiche letture documenti per cofruta, lettura etichette basate su modulo gias, vari affinamenti")

            Riga_Text("Bolla Accettazione Ricevuta:",
                " Creato layout x CO.FRU.TA + modifiche Bolle/Fatture x gestione logo")

            Riga_Text("Bolle/Fatture",
                " Corretta funzione per Caricare Logo (in basso) In Campo Blob di Crystal")

            Riga_Fine()


            '==================================

            Riga_Data("22 Giugno 2016")

            Riga_Requisiti("Migra '395' :",
                        "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Sistemato aggancio note in irrigazione.rpt con GlobalGAP singolo e multiplo:",
                          "- sistemate le note e l'ordinamento delle irrigazioni")

            Riga_Fine()

            '==================================
            Riga_Data("16/06/2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")



            Riga_Text("Stampe fatture e DDT personalizzate per Maiorano: gestita la prestampa per stampa in PDF o su Carta Intestata",
          "")

            Riga_Fine()

            '==================================


            Riga_Data("13/06/2016")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_SchedeMagazzino_new :",
                  "- Corretto bug su parametri passati via querystring e gestione riempimento controlli")

            Riga_Text("Selezione_ScedaCampagna:",
                  "- Gestione stampe per Movimenti (Id_Agedna)")

            Riga_Text("Corretta anomalia Filtro Stampe cantine : quando si indicava il centro aziendale la stampa veniva fuori vuota.",
          "(segnalata da Poderepalazzo)")

            Riga_Fine()


            '==================================

            Riga_Data("10/06/2016")

            Riga_Requisiti("Migra '395' :",
                        "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Implementato Filtro per fattura :",
                  "Scelta St. fattura Normale o Raggruppata per prodotto")

            Riga_Fine()


            '==================================

            Riga_Data("06/06/2016")

            Riga_Requisiti("Migra '395' :",
                        "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("St. Bilancio :",
                  "Gestita la stampa per esercizio.")

            Riga_Fine()
            '==================================



            Riga_Data("25/05/2016")

            Riga_Requisiti("Migra '395' :",
                        "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda Campagna  :",
                  "Corretto baco che troncava 2 lettere nel macchinario.")

            Riga_Fine()

            '==================================
            Riga_Data("20/05/2016")

            Riga_Requisiti("Migra '395' :",
                        "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda Campagna  :",
                  "Corretto baco che troncava una lettera nel nome operatore.")

            Riga_Fine()

            '==================================


            Riga_Data("19/05/2016")

            Riga_Requisiti("Migra '395' :",
                        "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")


            Riga_Text("Gestito Logo DeFaveri. ",
          " ")

            Riga_Fine()

            '==================================


            Riga_Data("18/05/2016")

            Riga_Requisiti("Migra '395' :",
                       "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")


            Riga_Text("Gestito Logo Bartolini. ",
          " ")

            Riga_Fine()

            '==================================


            Riga_Data("16/05/2016")

            Riga_Requisiti("Migra '395' :",
                       "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")


            Riga_Text("Segnalazione Garuti/Anfil. Corretto il calcolo degli interessi sulla liquidazione generale iva trimestrale con un +. Ha due sezionali: se fa la liquidazione totale, l'interesse viene calcolato in negativo, anziché in positivo. ",
          " ")

            Riga_Text("Segnalazione Garuti. Registri corrispettivi: impostato di default la stampa riepilogo iva. ",
          " ")


            Riga_Fine()

            '==================================

            Riga_Data("10 Maggio 2016")

            Riga_Requisiti("Migra '395' :",
                       "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")


            Riga_Text("Implementazione DDT per Ortofrutta Grosseto: trattato caso particolare del mat_cod_alias. ",
          " ")

            Riga_Text("  :",
                  "")

            Riga_Fine()


            '==================================
            Riga_Data("06 Maggio 2016")

            Riga_Requisiti("Migra '395' :",
                       "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_SchedeMagazzino_new :",
                  "- gestione dei dati Impresa provenienti dalla pagina chiamante, con funzione che legge correttamente gli attributi xml" &
                  "- gestione client alla ricerche del form principale")

            Riga_Text("Correzioni DDT per Ortofrutta Grosseto. ",
          " ")

            Riga_Text("  :",
                  "")

            Riga_Fine()

            '==================================

            Riga_Data("03 Maggio 2016")

            Riga_Requisiti("Migra '395' :",
                       "")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DDT/Fatture F&F : Adeguamento stampe ai parametri di configurazione. ",
                  " ")


            Riga_Fine()


            '==================================

            Riga_Data("20 Aprile 2016 Versione B")

            Riga_Requisiti("Migra '384' :",
                       "Per gestione Etichette")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Fatture/bolle : reinserite modifiche Simon per risolvere problema caricamento report. ",
                  " ")


            Riga_Fine()


            '==================================
            Riga_Data("20 Aprile 2016")

            Riga_Requisiti("Migra '384' :",
                       "Per gestione Etichette")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Fatture/bolle --> implementata gestione loghi su file system per clienti su fosforo:",
                  "Guarini, Fiammetta ,Al Canevon, LaRizzola, Lorenzato, Monticino Zeoli,Podere Palazzo,Randi,Zuffa")


            Riga_Fine()
            '==================================


            Riga_Data("11 Aprile 2016")

            Riga_Requisiti("Migra '384' :",
                       "Per gestione Etichette")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Modifica mastrini per stampa di tutti i mastrini contemporaneamente ",
                  " (era stata erroneamente sovrascritta da altro utente vss). ")


            Riga_Fine()
            '==================================

            Riga_Data("05 Aprile 2016")

            Riga_Requisiti("Migra '384' :",
                       "Per gestione Etichette")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Fatture/ddt: Inserito logo Gazzola. ",
           " ")

            Riga_Text(" Annullata la seguente modifica per problemi di rallentamento:",
                  " Interventi per passaggio del report al visualizzatore su disco effettuando la dispose dei report in modo che non rimangano allocati in memoria:  : " &
                  " Fatture " &
                  " Bolle")


            Riga_Fine()

            '==================================


            Riga_Data("01 Aprile 2016")

            Riga_Requisiti("Migra '384' :",
                       "Per gestione Etichette")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text(" Risolta regressione su Bilancio (non esponeva i dati) e Mastrini (non permetteva più la stampa di tutti i mastrini ma solo uno per volta) .",
                  " Segnalazione di Collina dei poeti")

            Riga_Text("Interventi per passaggio del report al visualizzatore su disco effettuando la dispose dei report in modo che non rimangano allocati in memoria:  :",
                  "- Fatture/Ordini" &
                  "- Registro Vinificazione" &
                  "- Registro Commercializzazione Vini" &
                  "- Registro Imbottigliamento" &
                  "- DDT BolleConf" &
                  "- Ricevuta Fiscale A4" &
                  "- Ricevuta Fiscale A5" &
                  "- Mastrino" &
                  "- Bilancio" &
                  "- Estratto Conto Cli/For Pagamenti" &
                  "- Estratto Conto Contatti" &
                  "- Giornale" &
                  "- Liquidazione IVA" &
                  "- Registro Corrispettivi" &
                  "- Registri IVA" &
                  "- Scheda Prodotti Fitosanitari" &
                  "- Scheda Movimenti Magazzino" &
                  "- Scheda Giacenze Magazzino" &
                  "- Scheda Fertilizzanti Magazzino")

            Riga_Text(" Problema Timeout: impostata la proprietà: executionTimeout. ",
           " ")


            Riga_Fine()


            '==================================

            Riga_Data("23 Marzo 2016")

            Riga_Requisiti("Migra '384' :",
                       "Per gestione Etichette")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "StampaDirettaMastrino: sistemata la chiamata (si era disallineata nella versione precedente, occorrevano nuovi parametri in querystring).")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti :",
               "Query_Conti_Patrimoniali_Movimentati: " &
               "gestita la lettura della ritenuta d'acconto e dell'enasarco nella fattura professionisti.")

            Riga_Text("Scadenziario (EstrattoConto_ClientiFornitori) :",
                  "Fatture professionisti: gestite in modo da non mettere nel totale gli importi dell'enasarco e della ritenuta d'acconto")

            Riga_Text("AgronicaCoreStampeDAL.RegistriCantina :",
                  "RegistroVinificazione e RegistroVinificazioneRiepilogo: nel caso di enum_RegistroContoTerzi.RegistroUnicoDiversificato " &
                  "modificata nella select la parte 'case else' che si riferisce ai semilavorati ceh non sono nè uva nè altre materie prime (leggi commento dettagliato sul codice).")

            Riga_Fine()

            '==================================

            Riga_Data("21 Marzo 2016 - Versione B")

            Riga_Requisiti("Migra '384' :",
                       "Per gestione Etichette")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Bolle/Fatture Aggiunto Logo Zanasi : creati nuovi file CRFatturaNew.rpt e CRBollaNew.rpt ",
                  "copia rispettivamente di CRFattura.rpt e CRBolla.rpt per nuovi clienti")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti :",
               "Query_Conti_Economici_Movimentati: " &
               "- aggiunta parte 5b e 6b per gestire i C.E. attivi o passivi movimentati col pagamento delle fatture." &
               "- parte 5 e 6: riattivato il filtro iniziale che era solo sul lav_cod = 1032.")

            Riga_Text("SchedaMovimentiMagazzinoExcel.aspx  :",
                  "Introdotta indicazione destinazione uso negli scarichi.")

            Riga_Fine()

            '==================================

            Riga_Data("21 Marzo 2016")

            Riga_Requisiti("Migra '384' :",
                       "Per gestione Etichette")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "- Lettura opzioni per inizio gestione contabile e passaggio a tutti i report dei conti;" &
                  "- Cambiata gestione filtri sul report pianodeiconti;" &
                  "- attivato filtro del sezionale per il mastrino;" &
                  "- disattivato filtro anno contabile sul mastrino, estratto conto contatti, giornale;" &
                  "- sostituiti menù a tendina conti con uno unico che è possibile filtrare per report mastrino ed estratto conto contatti." &
                  "- Database_OperazioniPreliminari: commentata la parte che inseriva i conti automatici di Gias.")

            Riga_Text("Bilancio  :",
                  "Gestite opzioni per inizio gestione contabile.")

            Riga_Text("Mastrino  :",
              "- Leggi_Codifica_ContiEconomici e Leggi_Codifica_ContiPatrimoniali: gestito anno minimo." &
              "- eliminato filtro anno contabile. " &
              "- gestito filtro per sezionale. " &
              "- Gestite opzioni per inizio gestione contabile." &
              "- Sostituite le caselle di testo dell'intestazione con i parametri.")

            Riga_Text("EstrattoConto_Contatti  :",
             "- Leggi_Codifica_ContiEconomici e Leggi_Codifica_ContiPatrimoniali: gestito anno minimo." &
             "- eliminato filtro anno contabile. " &
             "- Gestite opzioni per inizio gestione contabile.")

            Riga_Text("PianodeiConti (BilancioVerifica)  :",
                    "- Nella stampa del piano dei conti senza saldi continua ad essere per anno contabile" &
                    "- Nella stampa con saldo alla data o saldo a inizio gestione contabilità usa l'anno dell'inizio gestione contabile.")

            Riga_Text("LibroGiornale  :",
               "- Leggi_Codifica_ContiEconomici e Leggi_Codifica_ContiPatrimoniali: gestito anno minimo." &
             "- eliminato filtro anno contabile. " &
             "- Gestite opzioni per inizio gestione contabile.")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti :",
              "- Nelle query dei saldi, gestite le opzioni sull'inizio gestione contabile e il filtro del sezionale." &
              "- adeguate le varie query per il passaggio dei parametri." &
              "- Query_Conti_Economici_Movimentati e Query_Conti_Patrimoniali_Movimentati: nella lettura dei saldi dei conti non automatici filtra per anno inizio gestione contabile.")

            Riga_Fine()

            '==================================

            Riga_Data("8 Marzo 2016")

            Riga_Requisiti("Migra '384' :",
                     "Per gestione Etichette")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("GestioneRichieste, CrystalHelper, VisualizzatoreReport, Fattura_NotaAccredito :",
                  "Modifica per deallocazione report: ")

            Riga_Text("GestioneRichieste :",
                  "Gestito report etichette vasche.")

            Riga_Text("RegistroCommercializzazioneVini e RegistroVinificazione:",
                    "- Corretta data fine allegati;" &
                    "- rinominato nome file di log.")

            Riga_Text("ConsistenzeEnologiche  :",
                  "Porting del report dalle stampe 2003.")

            Riga_Text("Filtro_StampeCantine.aspx :",
                  "Modifica sul filtro stampa report etichette vasca.")

            Riga_Text("Stampa Bolle/Fatture :",
                  "- Corretta visualizzazione errata del tag <br>" &
                  "- sistemate richieste per Trombin. Correzione UDM/qta su righe altri beni strumentali.")

            Riga_Fine()

            '==================================

            Riga_Data("1 Marzo 2016")

            Riga_Requisiti("Migra '384' :",
                       "Per gestione Etichette")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            'Riga_Text("App_Scripts - App_Styles:", _
            '          "Eliminate cartelle.")

            Riga_Text("Magazzino :",
                  "Filtro_SchedeMagazzino_new.aspx working progress.")

            Riga_Text("Ri.Ba :",
                  "1409 - Costa Vanni - Bug - Sul file di flusso la data di scadenza viene calcolata male, ad esempio sul tipo 30 giorni da fine mese viene impostato il 30 giorni da data fattura. Leggere il dato dalla tabella Pagamenti (colonna DataScadenza_Manuale)")

            Riga_Text("Gestione Etichette :",
                  "working progress x Trombin")

            Riga_Text("RegistroImbottigliamento :",
                  "AgronicaCoreStampeDAL.RegistriCantina.RegistroImbottigliamento: gestita una parte di union per leggere i condizionamenti non collegati a linea.")

            Riga_Text("AgronicaCoreStampeDAL.RegistriCantina  :",
                  "query RegistroVinificazione e RegistroVinificazioneRiepilogo: " &
                 "select nella parte agenda, Case enum_RegistroContoTerzi.RegistroUnicoDiversificato: modificata gestione del cau_scarico del vino (vedi commento dettagliato).")

            Riga_Text("DDT :",
                  "- CRBolla.rpt: nel riquadro vettore c'era una linea di troppo." &
                  "- CRBolla_Trombin.rpt: layout personalizzato per Trombin." &
                  "<br/>- Gestione stampa qta o qta * qta_extra in base a configurazione anagrafica (udm aspetto default) oppure layout pesi o layout Trombin.")

            Riga_Text("Fattura :",
                  "- CRFattura.rpt: layout personalizzato per Trombin." &
                  "- Fattura_NotaAccredito.aspx: Nel caso di piva impresa = cod_contatto stampata AUTOFATTURA solo nel caso della fattura." &
                  "<br/>- Fattura_NotaAccredito.aspx: sezione pagamenti: quando viene stampata per i ddt (che hanno appunto layout con dettagli economici) non stampa la data di scadenza.")

            Riga_Text("DocumentiContab.vb  :",
                  "Introdotta lettura del flag_extra delle materie prime e gestiti pesi extra in base a peso_set.")

            Riga_Fine()

            '==================================

            Riga_Data("22 Febbraio 2016")

            Riga_Requisiti("Migra '384' :",
                       "Per gestione Etichette")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_StampeCantine.aspx :",
                  "- corretta visualizzazione pannello c/terzi;" &
                  "- gestito unico cmb centro aziendale;" &
                  "- gestito unico chk stampa intestazione;" &
                  "- eliminato il background da alcuni controlli;" &
                  "- copertina registro: introdotto controllo sul numero totale di pagine;")

            Riga_Text("Stampe Cantina :",
                  "Imbottigliamento: correzione stampa parametro filtro nel caso di registro vuoto.")

            Riga_Fine()

            '==================================

            Riga_Data("19 Febbraio 2016")

            Riga_Requisiti("Migra '384' :",
                        "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Stampe Cantina :",
             "Imbottigliamento: correzione label parametro C/lavoro.")

            Riga_Text("Stampe Cantina :",
                  "Commercializzazione: modificata impostazione pagina :nessuna stampante associata")

            Riga_Text("Stampe Fattura e DDT :",
                  "Inserito Logo Tenuta Galvana.")

            Riga_Fine()


            '==================================

            Riga_Data("17 Febbraio 2016")

            Riga_Requisiti("Migra '384' :",
                        "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI: aggiunto <sessionState timeout=120/>. ")

            Riga_Text("Filtro_SceltaStampa.aspx :",
                  "Aggiunta pagina, working progress.")

            Riga_Text("GestioneRichieste.aspx  :",
                  "commentata parte del configurazione_stampe.")

            Riga_Text("Selezione_SchedaCampagna :",
                  "Per la stampa GLOBAL multicentro aggiunto il check 'visualizza titolo global gap' (default selezionato) per poter non visualizzare il titolo e l'intestazione pagina GLOBAL (richiesta OPTA).")

            Riga_Text("Scheda GLOBAL Multi-centro :",
                  "Gestito il check 'visualizza titolo global gap' (default selezionato) per poter non visualizzare il titolo e l'intestazione pagina GLOBAL (richiesta OPTA).")

            Riga_Text("Stampe Fattura e DDT :",
                  "Inseriti Intestazione e Footer per Bele Casel")


            Riga_Fine()

            '==================================

            Riga_Data("16 Febbraio 2016")

            Riga_Requisiti("Migra '384' :",
                        "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI: aggiunto <sessionState timeout=120/>. ")

            Riga_Text("RegistroImbottigliamento  :",
                  "- RegistroImbottigliamento.aspx: gestione stampa del cod_contatto e sistemata visualizzazione dei dati del c/lavoro sulla descrizione operazione o sull'intestazione" &
                  "<br/>- AgronicaCoreStampeDAL.RegistriCantina.RegistroImbottigliamento: aggiornata la query per leggere anche il cod_contatto.")

            Riga_Text("CopertinaRegistri:",
                  "CopertinaRegistri.aspx: allineata lettura del nome/cognome del legale rappresentante.")

            Riga_Fine()

            '==================================

            Riga_Data("12 Febbraio 2016")

            Riga_Requisiti("Migra '384' :",
                      "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI: aggiunto <sessionState timeout=120/>. ")

            Riga_Text("Filtro_StampeCantine.aspx :",
                  "Attivato nuovo filtro stampa cantine.")

            Riga_Text("RegistroImbottigliamento  :",
                  "- RegistroImbottigliamento.aspx: gestione del filtro per categoria e per c/lavoro + visualizzazione del nome del c/lavoro nell'operazione." &
                  "<br/>- AgronicaCoreStampeDAL.RegistriCantina.RegistroImbottigliamento: aggiornata la query per gestire filtro per categoria e per c/lavoro.")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti :",
                    "Query_Conti_Economici_Movimentati: corretto bug sulle stampe dei conti (presente dalla versione 07/12/2015). ")

            Riga_Fine()

            '==================================

            Riga_Data("09 Febbraio 2016")

            Riga_Requisiti("Migra '384' :",
                      "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("AgronicaCoreStampeDAL :",
                "- Introdotta AccettazioneDaDiversi_Funzioni.vb che sostituisce il modulo AccettazioneDaDiversi.vb dentro a GestioneStampe -> AccettazioneDaDiversi" &
                "- AccettazioneDaDiversi: spostata CertificatiPomodoro_DistinctAnni_Leggi dal core ContabDAL.AccettazioneDaDiversi")

            Riga_Text("Tutti i report di AccettazioneDaDiversi  :",
                  "Ora chiamano il core AccettazioneDaDiversi_Funzioni.vb")

            Riga_Text("BollaAccettazione.aspx :",
                "Ora chiama il core AccettazioneDaDiversi_Funzioni.vb")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "- Database_OperazioniPreliminari: nella verifica della Cassa sistemato l'insert e introdotto update (non serve più CASSA nel nome della banca);" &
                  "- filtrate le risorse finanziarie in base al conto patrimoniale cassa o banca;")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti.Query_Conti_Patrimoniali_Movimentati()  :",
                  "Gestione multi cassa.")

            Riga_Text("Scheda Campagna  :",
                  "Reintrodotte Firme operatori per tutti i costi associati.")

            Riga_Fine()

            '==================================

            Riga_Data("04 Febbraio 2016")

            Riga_Requisiti("Migra '384' :",
                      "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                        "Modifiche NO. ")

            Riga_Text("Riba :",
                "Correzione baco su Numerazione Riba Maggiore di 10  --- Gestione di società non profit con p.iva che comincia per 9")

            Riga_Text("Stampa Registro Cespiti :",
                "Rilascio interno, la gestione ancora non è stata rilasciata")

            Riga_Text("Filtro_StampeCantine.aspx  :",
                 "working progress.")

            Riga_Fine()

            '==================================

            Riga_Data("03 Febbraio 2016")

            Riga_Requisiti("Migra '384' :",
                      "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Requisiti("Altri requisiti :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                        "Modifiche NO. ")

            Riga_Text("Riba :",
                "Indicazione numerica sul campo Market Place sostituito con spazio bianco per compatibilità con Home Banking Veneto Banca")

            Riga_Fine()

            '==================================

            Riga_Data("01 Febbraio 2016")

            Riga_Requisiti("Migra '384' :",
                      "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Requisiti("Altri requisiti :",
                        "richiede nuova release giaslan ancora da compilare!")

            Riga_Text("Contabilita:  :",
                "- CRBolla.rpt, CRFattura.rpt: inserito il logo di Colombarda, Lorenzato, Bele Casel, Geronazzo (Al Canevon) e attivato quello di Guarini anche per la seconda piva. ")

            Riga_Text("Bilancio :",
                  "File rpt: modificate le formule sugli id_riclassificazione (case sulla lunghezza).")

            Riga_Fine()

            '==================================

            Riga_Data("28 Gennaio 2016")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '384' :",
                   "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Riba CBI :",
                  "Migliorata la visualizzazione degli errori.")

            Riga_Text("Scadenziario - EstrattoConto_ClientiFornitori :",
                  "gestita fattura professionisti che non veniva considerata.")

            Riga_Text("RegitriIVA :",
                  "Gestita lettura dei campi nome e cognome dei contatti (leggeva solo rag_soc).")

            Riga_Text("Filtro_StampeCantine.aspx  :",
                  "working progress.")

            Riga_Text("GestioneRichieste.aspx :",
                  "Nel blocco GESTIONE STAMPE PERSONALIZZATE introdotto controllo aggiuntivo sull'hash per evitare errore generato dalla chiamata dello smart.")

            Riga_Fine()

            '==================================

            Riga_Data("26 Gennaio 2016")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '384' :",
                   "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Schede_OP/Rpt_AttoNotorio.rpt :",
                  "modificata label per loro richiesta: Associata OP. Pempacorer / OP. Apofruit ")


            Riga_Fine()

            '==================================

            Riga_Data("20 Gennaio 2016 - Versione B")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '384' :",
                    "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Documenti Contabilita :",
                  "- DocumentiContab.vb: gestita lettura nome e cognome dei contatti (cliente, destinatario, vettore) " &
                  "e gestita riga di testo libera (elem_cod = 502).")

            Riga_Text("Fattura_NotaAccredito.aspx :",
                  "Gestita la riga di testo libera.")

            Riga_Fine()

            '==================================

            Riga_Data("20 Gennaio 2016 - Versione A")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '384' :",
                    "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("RicevutaFiscale :",
                  "- RicevutaFiscale_GestioneStampa.vb: cambiato format su Importo_Riga" &
                  "<br/>- Rpt_RicevutaFiscaleA5.rpt: corretto allineamento qta seconda riga sezione a sinistra.")

            Riga_Text("Contabilita  :",
                  "DocumentiContab.vb: nel caso dei servizi stampato anche il tipo di servizio selezionato nel menù a tendina.")

            Riga_Text("Bilancio:",
                  "Bilancio.aspx: cambiato il calcolo del saldo con figli (modificato il Like per prendere esattamente solo i figli e se stesso). " &
                  "(Risoluzione del problema che i conti za, zb, zc venivano considerati figli di z e non fratelli).")

            Riga_Text("EC_Imballi_Conf  :",
                  "Commentata lettura sequenza progressivi per correzione query estratto conto imballi" &
                  "AgronicaCoreStampeDAL.ConferimentoAccettazione.EstrattoConto_Imballi: Commentata lettura sequenza progressivi che portava una riga in più nel report.")

            Riga_Fine()


            '==================================

            Riga_Data("11 Gennaio 2016")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '384' :",
                    "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Contabilita:  :",
                  "- DDT_BolleConf.aspx, Fattura_NotaAccredito.aspx: gestita la stampa del numero di telefono della destinazione." &
                  "<br/>- CRBolla.rpt, CRFattura.rpt: inserito il logo di Andreola. " &
                  "<br/>- DocumentiContab.vb: inserita Leggi_Rubrica.")

            Riga_Fine()

            '==================================

            Riga_Data("30 Dicembre 2015 - SOLO X MAIORANO")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '384' :",
                    "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Contabilita:  :",
                "- CRBolla.rpt, CRFattura.rpt: gestita intestazione personalizzata per Maiorano più margine in intestazione e piè di pagina dedicato x Maiorano. ")

            Riga_Fine()

            '==================================

            Riga_Data("23 Dicembre 2015")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '384' :",
                    "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Selezione_SchedaCampagna.aspx :",
                  "corretto baco errore indirizzi.")

            Riga_Fine()

            '==================================

            Riga_Data("11 Dicembre 2015")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '384' :",
                    "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda Colturale Bio  :",
                  "Corretto baco (Mov_Destinazioni.vb) su Leggi_Dati_OsservazioneFasi, Leggi_Dati_RilievoAvversitaCampo, Leggi_Dati_RilievoAvversitaTrappole che dava -Il nome colonna Mov_Dettaglio_Tecnico non è valido-")

            Riga_Fine()


            '==================================

            Riga_Data("10 Dicembre 2015")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '384' :",
                    "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_ElaboratiContabili.aspx  :",
                "- report scadenziario clienti/fornitori: Attivato filtro per tipo pagamento." &
                "<br/>- report piano dei conti: attivata opzione per stampa con solo saldo di riporto.")

            Riga_Text("EstrattoConto_ClientiFornitori :",
                  "Pagamenti.aspx e AgronicaCoreStampeDAL.RegistriContab.Lista_Insoluti_Movimenti: gestito il filtro per tipo pagamento.")

            Riga_Text("BilancioVerifica alias Piano dei Conti  :",
                  "Gestito il caso di lettura solo saldi di riporto.")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti  :",
              "- Query_Conti_Economici_Movimentati e Query_Conti_Patrimoniali_Movimentati: gestione lettura solo saldi riporto;" &
              "- Query_Conti_Economici_Movimentati: parte 5 e 6, gestita la movimentazione dei conti sui pagamenti (non solo su movimenti finanziari);" &
              "- adeguamento su tutte le funzioni che chiamano le query sopra.")

            Riga_Text("EstrattoConto_Contatti, Mastrino, Giornale, Bilancio  :",
                    "Adeguate le chiamate alle funzioni dei conti per l'aggiunta di Flag_SOLOSaldiIniziali.")

            Riga_Text("Schede Campagna singolo centro: ",
                  "introdotta visualizzazione costi accessori nelle fertilizzazioni.")

            Riga_Text("Agrintesa - Etichette  :",
                  "Problema di stampa quando si aggiunge un nuovo dettaglio del piano di lavoro e nel pannello si manda in stampa al posto di un dettaglio già assegnato in precedenza.")

            Riga_Fine()

            '==================================

            Riga_Data("3 Dicembre 2015")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '384' :",
                    "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Stampa scheda colturale biologico: ",
                  "corretto errore su core mov_destinazioni.vb")

            Riga_Text("Fattura, DDT, ecc  :",
             "- DocumentiContab.vb: nel caso F&F se ChkLayOut_Peso = 1 non stampa peso unitario e peso netto in riga  ." &
             "- CRBolla.rpt: aggiunto parametro del contributo CONAI + modificato ordine colonne layout con pesi e prezzo." &
             "- CRFattura.rpt: sezione pagamenti disabilitata per Trombin e modificata sua intestazione personalizzata." &
             "- DDT_BolleConf.aspx: gestione parametro Conai + sistemato testo capo su omaggi e rif. documento." &
             "- Fattura_NotaAccredito.aspx: sistemato testo capo su rif. documento.")

            Riga_Fine()

            '==================================

            Riga_Data("2 Dicembre 2015")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '384' :",
                    "Per gestione Etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("AgronicaCoreDataProvider.DataProvider  :",
                 "FindConnessione_Su_Ini_O_Superserver: ")

            Riga_Text("Bilancio :",
                "Stato Patrimoniale con layout dare/avere working progress.")

            Riga_Text("Fattura, DDT, ecc  :",
                  "- DocumentiContab.vb: gestita la sostituzione del carattere § con <br> per mandare a capo il testo." &
                  "- CRBolla.rpt, CRFattura.rpt: il campo del dataset Descrizione è interpretato ora come HTML." &
                  "- DDT_BolleConf.aspx, Fattura_NotaAccredito.aspx: il riferimento doc allegati e il tipo di sconto/omaggio concatenati con <br>.")

            Riga_Text("Mastrino.aspx :",
                  "Gestito bene il caso in cui la query non ritornava dati (genera report vuoto, anziché errore).")

            Riga_Text("Filtro_ElaboratiContabili.aspx  :",
                "Stampa mastrino: gestita attivazione contatti/risorse finanziarie a seconda del conto selezionato.")

            Riga_Text("AnteprimaEtichette.aspx :",
                  " Ripristinata mancata lettura dati ragione sociale cliente " &
                  " Corretto Baco su generazione barcode in formato CODE128, il programma stampa sempre dei GS1-128 " &
                  " Impostata lingua di default"
                    )

            Riga_Fine()

            '==================================


            Riga_Data("26 Novembre 2015 ")

            Riga_Requisiti("Migra '383' :",
                        "x gestione etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("GestioneRichieste.aspx :",
                     "Separata la chiamata all'esportazione Riba CBI dalla stampa presentazione Riba.")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "- Database_OperazioniPreliminari: aggiornata la procedura verificando anche i conti patrimoniali ErarioRitenuteLavoroAutonomo e DebitiVsEnasarco " &
                  "e inserendo il controllo sulla codifica conti economici (OmaggiAllaClientela e RicavixIVAincompensazione) ." &
                  "<br/>- Nella chiamata alla stampa (elaborati dei conti) verificata la presenza delle codifiche dei conti economici e aggiornata quella dei conti patrimoniali." &
                  "<br/>- Riattivato link al report presentazione Riba.")

            Riga_Text("Bilancio :",
                  "Terminata stampa con layout CE Costi-Ricavi.")

            Riga_Text("RegistroRiBa :",
                  "- Report_PresentazioneRIBA.aspx: introdotta come copia dalle stampe 2003 (viene chiamata dal Filtro_ElaboratiContabili.aspx)." &
                  "<br/>- AgronicaCoreStampeDAL.RegistriContab.RegistroRIBA: corretta la query per gestire anche il caso 'anticipi fatture'.")

            Riga_Text("AnteprimaEtichette :",
                  "Gestione configurazione campi crystal attraverso tabella OModuli_Referenze_Config_Dettagli_Label")

            Riga_Fine()

            '==================================

            Riga_Data("24 Novembre 2015 ")

            Riga_Requisiti("Migra '382' :",
                  "x gestione etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda Campagna  :",
                  "Corretto baco sezione raccolte.")

            Riga_Fine()

            '==================================

            Riga_Data("23 Novembre 2015 --- SOLO X AGRINTESA ")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '382' :",
                  "x gestione etichette")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            '///////////
            ' questa parte era in progress mentre Vanni stava compilando
            Riga_Text("GestioneRichieste.aspx :",
                        "Gestita la stampa diretta del mastrino.")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "- Gestito nuovo report 'Estratto Conto Contatti';" &
                  "<br/>- Stampa mastrino: default selezione sul conto delle banche ed eliminato il filtro sui conti UE (inutile)." &
                  "<br/>- Gestita la stampa diretta del mastrino.")
            '//////////

            Riga_Text("AgronicaCoreStampeDAL.PianoConti  :",
                  "Query_Conti_Patrimoniali_Movimentati: " &
                  "<br/>- nella lettura dei saldi iniziali crediti/debiti evitati filtri sui rapporti contabili. " &
                  "<br/>- gestito filtro per lista cod_risum;" &
                  "<br/>- nella lettura di operazioni p.d. collegate a crediti/debiti letto numero doc e numero prot della fattura collegata (serve per la contabilizzazione manuale fatture)." &
                  "EstrattoConto_Contatti: nuova query.")

            Riga_Text("EstrattoConto_Contatti :",
                    "Nuovo report.")

            Riga_Text("Filtro_Registri.aspx  :",
                  "Alert su registri di cantina (non ancora attivi).")

            Riga_Text("Etichette Agrintesa ed altri  :",
                  "Gestione Etichette di confezione con barcode Code128 in maniera configurabile (Per rimuovere primitive crystal di Azalea). Attenzione al fatto che i barcode fatti con le primitive crystal di azalea non sono stampabili se il processo che stampa gira a 64 bit.")

            Riga_Fine()

            '==================================

            Riga_Data("19 Novembre 2015 ")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '381' :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI/NO. ")

            Riga_Text("Agrintesa  :",
                  "Porting della gestione etichetta così come esiteva in Manager, Aggancio e salvataggio come da configurazione in GIASLAN.")

            Riga_Fine()

            '==================================

            Riga_Data("13 Novembre 2015 ")

            Riga_Requisiti("Componenti '' :",
                       "")

            Riga_Requisiti("Migra '380' :",
                     "SOLO per Etichette Fresh And Food" &
                     "- ConfigurazioneSiti_xGiasLan_v20.sql x stampe Fresh&Food.")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Fattura :",
                    "- CRFattura.rpt: inserita intestazione personalizzata per Trombin (con i loghi delle tre ragioni sociali)." &
                    "<br/>- inserito logo di Tomisa." &
                    "<br/>- modificata l'intestazione per Omina Romana.")

            Riga_Text("DDT :",
                  "- CRBolla.rpt: inserita intestazione personalizzata per Trombin (con i loghi delle tre ragioni sociali)." &
                  "<br/>- DDT_BolleConf.aspx: gestito peso_lordo peso_netto e tara totale (al momento visualizzati su intestazione Trombin)." &
                  "<br/>- inserito logo di Tomisa." &
                    "<br/>- modificata l'intestazione per Omina Romana.")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "- Disattivato avvio stampa bilancio per layout C.E. costi e ricavi." &
                   "- Stampa scadenziario clienti/fornitori: attivato filtro sul sezionale (era gestito ma invisibile).")

            Riga_Text("Scadenziario - EstrattoConto_ClientiFornitori  :",
                  "AgronicaCoreStampeDAL.RegistriContab.Lista_Insoluti_Movimenti: se non c'era il filtro agente i corrispettivi non venivano letti (deve fare il contrario).")

            Riga_Text("Agrintesa  :",
                  "Porting della gestione etichetta così come esiteva in Manager.")

            Riga_Text("Nuove stampe conferimenti :",
                  "Introdotto 4 nuovi report eseguibili in GIAS LAN da menu Conferimenti. " &
                  "Saldi imballi, EC Imballi, Riepilogo Conferimenti, Tracciabilità conferimenti ")

            Riga_Text("Filtro_Stampe_Conf.aspx :",
                  "Introdotto nuovo filtro per stampe conferimenti: num 183 ")

            '==================================

            Riga_Data("10 Novembre 2015 - NO X GIASLAN")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '380' :",
                  "SOLO per Etichette Fresh And Food")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_SchedeMagazzino.aspx :",
                  "Introdotto Check 'Blocca Operazioni', visibile solo se si ha il permesso.")

            Riga_Text("SchedaMovimentiMagazzinoExcel.aspx  :",
                  "Introdotto blocco operazioni se si seleziona il check sopra.")

            Riga_Text("Scheda Campagna Veneto :",
                  "- Sezione A : corretta lettura Responsabile Aziendale per contatti legati al centro aziendale." &
                  "- Seziona A : eliminata intestazione pagina per le pagine successive alla prima." &
                  "- Sezione C : aggiunta colonna N.App/Note." &
                  "- Sezione C : visualizzato l'anno.")

            Riga_Text("DDT_BolleConf.aspx :",
                  "- Corretto errore nella stampa dei ddt ricevuti con accettazione dell'uva." &
                  "<br/>- Gestito parametro LblPrezzoUdmExtra per l'intestazione della modalità layout stampa pesi" &
                  "<br/>- Nel caso degli imballaggi e contenitori non stampati i dettagli di pesi e prezzi.")

            Riga_Text("Bilancio :",
                "Layout CE Costi-Ricavi working progress.")

            Riga_Fine()

            '==================================

            Riga_Data("4 Novembre 2015 - NO X GIASLAN")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '380' :",
                  "SOLO per Etichette Fresh And Food")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti:",
                  "Query_Conti_Patrimoniali_Movimentati: corrette le sezioni per il pagamento/incasso delle note di accredito (lav_cod da invertire)" &
                  "- 11a° PARTE: RISCOSSIONE PAGAMENTI - CONTO CREDITI IN AVERE - CON RIFERIMENTO CONTATTI" &
                  "- 12° PARTE: RISCOSSIONE PAGAMENTI - CONTO RIS FINANZIARIE IN DARE - CON RIFERIMENTO BANCHE " &
                  "- 13° PARTE: PAGAMENTO DEBITI - CONTO DEBITI IN DARE - CON RIFERIMENTO CONTATTI")

            Riga_Text("Filtro_ElaboratiContabili.aspx:",
                  "- Database_OperazioniPreliminari: introdotte altre query di verifica/update sui pagamenti delle note di accredito ricevute ed emesse." &
                  "- Disattivato filtro layout CE (serve solo x bilancio).")

            Riga_Text("Bilancio :",
                "Layout CE Costi-Ricavi working progress.")

            Riga_Text("AnteprimaEtichette.aspx:",
                          "- Evoluzione gestione etichette con invio a servizio di stampa massiva." &
                          "- Configurazione di stampanti lette da database." &
                          "- Layout di etichette letti in maniera configurabile da database via reflection.")

            Riga_Text("SchedaCampagna.aspx:",
                    "- Sezione Raccolta : introdotta Sup. Raccolta sulle 2 schede multi-centro." &
                    "- Sezione Raccolta : corretto baco nella colonna resa veniva visualizzata l'unità di misura." &
                    "- Sezione Raccolta : introdotti gli arrotondamenti in base alla schelta del filtro pre-stampa." &
                    "- Sezione Arboree : modificato l'anno impianto (se è stato registrato il trapianto o la data trapianto prevista si visualizza quella).")

            Riga_Fine()

            '==================================

            Riga_Data("27 Ottobre 2015 - NO X GIASLAN")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti:",
                  "Saldo_Conti_Economici(): gestito ordinamento per layput Costi-Ricavi.")

            Riga_Text("Bilancio :",
                  "Layout CE Costi-Ricavi working progress.")

            Riga_Text("SchedaMovimentiMagazzinoExcel  :",
                  "Corretto baco che moltiplicava le righe delle raccolte fatte con i semilavorati.")

            Riga_Fine()

            '==================================

            Riga_Data("21 Ottobre 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("FreshAndFood :",
                  "Riattivazione stampe che per sbaglio erano state escluse dal progetto.")

            Riga_Fine()

            '==================================


            Riga_Data("20 Ottobre 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("AgronicaCoreContabHLP.Contabilita.vb :",
                  "- class Contabilita: Aggiornate le funzioni sui conti codificati economici: gestiti gli oamggi alla clientela." &
                  "- class GiasLan_Round: allineata la FormAggiornaImporto alle modifiche del giaslan.")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti  :",
                  "Query_Conti_Economici_Movimentati: " &
                  "<br/>- aggiornate le chiamate alle funzioni di lettura dei conti codificati" &
                  "<br/>- aggiunta 12° parte alla union, che legge gli omaggi alla clientela." &
                  "<br/>Query_Conti_Patrimoniali_Movimentati: gestiti crediti/vclienti nel caso degli omaggi. "
                  )

            Riga_Text("Fattura, DDT, Ricevuta :",
                  "Aggiornate descrizioni degli omaggi.")

            Riga_Text("SchedaMovimentiMagazzinoExcel  :",
                  "Corretto baco che moltiplicava le righe in base alle distinte.")

            Riga_Fine()

            '==================================

            Riga_Data("14 Ottobre 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("LiquidazioneIVA_Anteprima.aspx :",
                  "Sostituito controllo su cod_iva con aliquota.")

            Riga_Text("DDT_BolleConf.aspx - CRBolla.rpt  :",
                  "Corretto bug nella stampa della bolla di conferimento.")

            Riga_Text("BilancioMatrino :",
                  "Mastrino.aspx: gestito saldo iniziale come riga a parte.")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti.Query_Conti_Patrimoniali_Movimentati()  :",
                  "Nella lettura dei saldi iniziali gestitito filtro su cod_risum e cod_liquidita.")

            Riga_Text("Esportazione_OP_Catasto.aspx  :",
                  "Introdotto storico codici varietà.")

            Riga_Fine()

            '==================================

            Riga_Data("6 Ottobre 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.aspx, Fattura_NotaAccredito.aspx :",
                  "Nel caso del fresh&food, gestito il caso di Tabella_Cod_Base non valorizzato (generava eccezione).")

            Riga_Text("Esportazione_OP_Catasto.aspx  :",
                  "Modificato caricamento dati. Eliminato join con specie per evitare doppioni (es. pesco mappato con 3 codici OP).")

            Riga_Text("Schede Magazzino  :",
                  "Rilasciate (tipi enumerativi 9,10,11,12,178).")

            Riga_Text("Schede Campagna :",
                  "Nella sezione erbacee/orticole introdotta postilla x copertura (la copertura viene indicata se presente tra parentesi accanto alla varietà).")

            Riga_Text("Scheda Veneto B :",
                  "Eliminata la colonna Copertura. Introdotta postilla come per le altre (la copertura viene indicata se presente tra parentesi accanto alla varietà).")


            Riga_Fine()

            '==================================

            Riga_Data("25 Settembre 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Fattura - DDT :",
                  "CRFattura.rpt, CRBolla.rpt, Fattura_NotaAccredito.aspx, CRBolla.rpt:" &
                  "<br/>- gestita intestazione personalizzata per Omina Romana. " &
                  "<br/>- finita la parametrizzazione dell'intestazione. ")

            Riga_Text("FreshAndFood  :",
                  "Stampe imballi working progress.")

            Riga_Fine()

            '==================================

            Riga_Data("4 Settembre 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Contabilita :",
               "- DocumentiContab.vb: Leggi_Intestazione_Impresa_2: gestito il il codice operatore bio ." &
               "<br/>- AgronicaCoreStampeDAL.DocContab.Intestazione_Documento_2: introdotto il codice operatore bio.")

            Riga_Text("Fattura :",
                  "CRFattura.rpt: inserito logo di tenuta mara.")

            Riga_Text("DDT_BolleConf  :",
                  "- DDT_BolleConf.aspx: gestione della nuova intestazione." &
                  "<br/>- CRBolla.rpt: adeguata l'intestazione (come la fattura) e inserito logo di Tenuta Mara.")

            Riga_Text("RicevutaFiscale  :",
                    "- RicevutaFiscale_GestioneStampa.vb: stampata sede operativa (al posto di quella legale) se valorizzata." &
                    "<br/>- Rpt_RicevutaFiscaleA5.rpt: strette le caselle della qta e udm, allargata la descrizione.")

            Riga_Fine()

            '==================================

            Riga_Data("1 Settembre 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda Campagna :",
                  "Corretto baco rilievi indici maturità (nome appezzamento seconda colonna errato).")

            Riga_Text("RegistroCorrispettivi :",
                  "Registro_Corrispettivi_2.aspx: spostato il setdatasource del sottoreport, per evitare di generare errore quando non ci sono dati.")

            Riga_Text("Contabilita :",
                  "- DocumentiContab.vb: Leggi_Intestazione_Impresa_2: nuova gestione intestazione documenti" &
                  "<br/>- AgronicaCoreStampeDAL.DocContab.Intestazione_Documento_2: nuova query per l'intestazione dei documenti.")

            Riga_Text("Fattura  :",
                  "- CRFattura.rpt: nuova intestazione e parametri." &
                  "<br/>- Fattura_NotaAccredito.aspx: gestione nuova intestazione e parametri.")

            Riga_Fine()

            '==================================

            Riga_Data("28 Agosto 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "Introdotti maggiori controlli sulla data inizio e data fine.")

            Riga_Text("Bilancio:",
                  "- Rpt_Bilancio.rpt: aggiunta sezione con riport utile/perdita e totali a pareggio." &
                  "<br/>- Bilancio.aspx: cambiata gestione utile/perdita e introdotto totale a pareggio.")

            Riga_Text("DDT_BolleConf.aspx, Fattura_NotaAccredito.aspx :",
                  "- Lettura del modulo (cantine, fresh&food,...) " &
                  "<br/>- se modulo fresh&food legge l'udm default del calcolo e utilizza quella come udm principale." &
                  "<br/>- nella fattura, nel caso di dettaglio di riepilogo beni di confezionamento, non stmpa i dettagli economici. ")

            Riga_Text("DocumentiContab.vb :",
                  "Leggi_MovimentoDettaglio_DocContabile: lettura chkcontenitore e chkimballi per i beni di confezionamento + passati dati della funzione.")

            Riga_Fine()

            '==================================

            Riga_Data("25 Agosto 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DDT :",
                  "DDT_BolleConf.aspx e CRBolla.rpt: fine gestione stampa auto-ddt.")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti - Query_Conti_Economici_Movimentati e Query_Conti_Patrimoniali_Movimentati :",
                  "- Le parti di lettura della partita doppia filtrate ora su data di registrazione;" &
                  "<br/>- Gestito CONTO_UE_NOFILTRO e CE_CONTO_IMPUTABILE_NOFILTRO." &
                  "<br/>- .")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "- Nella stampa del bilancio tolto filtro conti UE e aggiunto filtro bilancio sintetico o analitico. " &
                  "<br/>- Introdotto controllo sulle codifiche dei conti economici solo nel caso di aziende che hanno regime iva in forfettario.")

            Riga_Text("LibroGiornale.aspx  :",
                  "Nelle operazioni di partita doppia, stampata nella colonna riferimento documento la data del movimento della partita doppia.")

            Riga_Text("Bilancio :",
                  "- Gestito bilancio sintetico e analitico; " &
                  "<br/>- Corretto il calcolo dei saldi dei mastri." &
                  "<br/>- Corretto il calcolo del saldo del conto C.002.d (fratelli -bis e -ter).")

            Riga_Text("AgronicaCoreStampeDAL.DocContab.DocumentiContabili :",
                  "Passato il codice report e ib base a quello fa il filtro sul cau_mov.")

            Riga_Text("DocumentiContab.vb, RicevutaFiscale_GestioneStampa.vb, Fattura_NotaAccredito.aspx, Gandini_DocContabile.aspx  :",
                  "Adeguamenti alla funzione di lettura dati.")

            Riga_Text("DDT_BolleConf.aspx :",
                  "Per le bolle di conferimento uva, gestita la stampa del dettaglio ceh contiene la qta di uva totale conferita.")

            Riga_Text("GestioneRichieste.aspx :",
                  "- Passato il codice report alle stampe contab;" &
                  "<br/>- Passato parametro random alle varie chiamate.")

            Riga_Fine()

            '==================================

            Riga_Data("14 Agosto 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti :",
                  "- Query_Conti_Patrimoniali_Movimentati: nella lettura dei pagamenti, impostata data_pagamento su data_registrazione " &
                  "<br/>- Libro_Giornale_Contabile: corretto order by")

            Riga_Fine()

            '==================================

            Riga_Data("11 Agosto 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("StampePersonalizzate - GandiniValTrebbia:",
                  "Gandini_DocContabile.aspx: inserita inizializzazione a tutti i parametri.")

            Riga_Text("LibroGiornale, Mastrino, Bilancio, Pianoconti :",
                  "Gestita cattura dell'eccezione nella lettura dei conti codificati.")

            Riga_Text("AgronicaCoreContabDAL.RicxConti_R.Leggi_Codifica_ContiEconomici():",
                  "Tolto lancio dell'eccezione x dt vuoto.")

            Riga_Text("AgronicaCoreContabHLP.Contabilita.vb  :",
                  "Tolti i riferimenti al conto costi x iva indetraibile (che non era da gestire).")

            Riga_Text("LibroGiornale :",
                  "Gestito lo split dell'operazione non solo per id_agenda ma anche per des_lib (i pagamenti hanno lo stesso id_agenda)")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti.Query_Conti_Patrimoniali_Movimentati:",
                  "- gestiti i corrispettivi di vendita a parte, sia per accendere il conto patrimoniale, sia per chiduerlo con il pagamento." &
                  "- Gestita correttamente la movimentazione della cassa." &
                  "<br/>- nella sezione dei pagamenti aggiunto filtro su coge_manuale.")

            Riga_Fine()

            '==================================

            Riga_Data("7 Agosto 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DDT :",
                  "- CRBolla.rpt: casella peso dinamica in base alla selezione sul data entry." &
                  "<br/>- DocumentiContab.Leggi_MovimentoDettaglio_DocContabile_NEW: modifiche sulla gestione della qta_extra (sia per cantine che fresh&Food)." &
                  "<br/>- DDT_BolleConf.aspx: modifiche alla gestione del peso totale del documento e della qta extra.")

            Riga_Fine()

            '==================================

            Riga_Data("5 Agosto 2015 Versione B")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "- Rinominati 'estratto conto' in 'scadenziario' clienti/fornitori." &
                  "<br/>- Per il registro dei corrispettivi, introdotto check per scelta di stampa riepilogo iva.")

            Riga_Text("RegistroCorrispettivi:",
                  "Registro_Corrispettivi_2.aspx e Rpt_RegistroCorrispettivi_2.rpt: gestione stampa riepilogo iva (sottoreport iva).")

            Riga_Text("Pagamenti:",
                  "Pagamenti.aspx e Rpt_Pagamenti.rpt: rinominato titolo in 'scadenziario', corrette date nel nome del file di log e pdf e corretta gestione categoria documento.")

            Riga_Fine()

            '==================================

            Riga_Data("5 Agosto 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda Campagna :",
                  "Corretto baco caricamento sezioni piogge.")

            Riga_Fine()

            '==================================

            Riga_Data("4 Agosto 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda Campagna :",
                  "Gestita opzione utente per nascondere la fase fenologica/epoca dell'etichetta nelle note.")

            Riga_Text("RegistroCorrispettivi  :",
                  "AgronicaCoreStampeDAL.RegistriContab.RegistroCorrispettivi_Importi_2: tolto join inutile su Movimenti_dettagli.Cod_IvaIndetraibile che, dati le recenti modifiche al salvataggio by giaslan, ora non dava risultato alla query.")

            Riga_Text("RegistriIVA  :",
              "cls_SottoReport_IVA, Carica_DSSottoReportIVA_daDtIVAGenerale_VENDITE: Corretto bug presente dalla versione precedente sui totali iva in compensazione e non in compensazione nel caso in cui nel riepilogo c'erano anche imponibili con esclusione iva.")

            Riga_Fine()

            '==================================

            Riga_Data("3 Agosto 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Contabilita:",
                    "CRFattura.rpt: allargata sezione dei pagamenti. " &
                    "Fattura_NotaAccredito.aspx: nelle note sostituito ? con €.")

            Riga_Text("DDT_BolleConf.aspx  :",
                    "- nelle note sostituito ? con €" &
                    "<br/>- Introdotta stampa auto-ddt (con e senza accettazione).")

            Riga_Text("DocumentiContab.vb :",
                  "Leggi_Pagamenti: gestita stampa della scadenza manuale.")

            Riga_Fine()

            '==================================

            Riga_Data("31 Luglio 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaCampagna Veneto - Lombardia :",
                  "- Modificata l'indicazione del regolamento in testata con aggiunta di quello aggiornato." &
                  "- Introdotta la stampa del logo regione selezionato dall'utente (non più fisso regione veneto).")

            Riga_Text("Selezione_SchedaCampagna :",
                  "- Anche per la stampa registro trattamenti veneto (ora + lombardia) possibilità di scegliere quale logo stampare.")

            Riga_Fine()

            '==================================

            Riga_Data("30 Luglio 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")


            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Contabilita:",
                  "CRFattura.rpt: modificato layout degli ordini e preventivi (tolto riquadro fiscale e aggiunto riquadro per firma). " &
                  "Fattura_NotaAccredito.aspx: gestiti riquadri firme per ordini e preventivi (e corretto problema subentrato dalla versione precedente).")

            Riga_Text("DDT_BolleConf.aspx  :",
                  "- Aggiunta estensione al file di log." &
                  "<br/>- Nel caso di ddt con accettazione, gestita la stampa del numero ddt e non del numero accettazione." &
                  "<BR/>- Gestita stampa auto-ddt.")

            Riga_Text("RicevutaFiscale :",
                  "Modificato nome file pdf salvato (aggiunta contatto).")

            Riga_Text("LiquidazioneIVA, RegistriIVA, RegistroCorrispettivi  :",
                  "Corretto nome file pdf salvato (date).")

            Riga_Text("RegistriIVA  :",
                 "Corretto bug presente dalla versione precedente sul totale documento (metteva solo il totale di imponibile e iva dell'ultimo dettaglio del documento).")

            Riga_Fine()

            '==================================

            Riga_Data("29 Luglio 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("AgronicaCoreContabHLP.Contabilita.vb :",
                  "Nuovo funzioni sui conti economici codificati.")

            Riga_Text("Bilancio - BilancioVerifica - BilancioMatrino - LibroGiornale  :",
                  "Adeguamenti alle chiamate alle query dei conti economici (passato dT_Codifiche).")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti :",
                    "Query_Conti_Economici_Movimentati e Query_Conti_Patrimoniali_Movimentati: modificate per gestire correttamente i nuovi dati salvati per iva in compensazione.")

            Riga_Text("RegistriIVA  :",
                    "- Dataset_InserisciRighe: modificato il totale documento (si calcola imponibile + iva, non si usa più l'output Riepilogo_importo dell'algoritmo di round, per via degli omaggi." &
                    "<br/>- Rpt_RegistriIVA_2.rpt: reimportato sottoreport iva vendite (alias duplicato).")

            Riga_Text("LiquidazioneIVA :",
                  "Rpt_LiquidazioneIVA_2.rpt: reimportato sottoreport iva vendite (alias duplicato).")

            Riga_Text("Sottoreport_IVA  :",
                  "- Rpt_SottoReportIVA_Duplicato.rpt: nei totali finali, tolti imponibile non in compensazione e imponibile in compensazione (non hanno senso)." &
                  "<br/>- Carica_DSSottoReportIVA_daDtIVAGenerale_VENDITE (alias DUPLICATO): corretto calcolo iva in compensazione.")

            Riga_Text("Fattura  :",
                    "- CRFattura.rpt: modificato riepilogo, introdotto totale fattura e totale da pagare + spostato contributo Conai." &
                    "<br/>- Fattura_NotaAccredito.aspx: gestiti i parametri Conai e totale fattura." &
                    "<br/>- Modificato il nome del file pdf.")

            Riga_Text("DDT :",
                  "DDT_BolleConf.aspx: Modificato il nome del file pdf.")

            Riga_Fine()

            '==================================

            Riga_Data("3 Luglio 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                    "- Modificato per stampa bilancio." &
                    "<br/>- gestita la stampa del piano dei conti con e senza saldo.")

            Riga_Text("Bilancio :",
                  "Rilasciata nuova versione.")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti :",
                "- Query_Conti_Patrimoniali_Movimentati: inseriti record nella sezione saldi dei conti padre dei crediti/debiti/banche.")

            Riga_Text("BilancioVerifica - PianoConti  :",
                  "BilancioVerifica.aspx e Rpt_BilancioVerifica.rpt: gestita la stampa del solo piano dei conti (senza saldo).")

            Riga_Fine()

            '==================================

            Riga_Data("25 Giugno 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("CRFattura.rpt, CRBolla.rpt :",
                    "Inserito logo x La Buona Romagna.")

            Riga_Text("Mastrino e LibroGiornale :",
                  "- Corretta chiamata alla query (disallineata dalla versione precedente)." &
                  "<br/>- Sistemato ordinamento data di registrazione (per data e non alfanumerico).")

            Riga_Text("StampePersonalizzate - GandiniValTrebbia :",
                  "- Gandini_DocContabile.aspx: nelle fatture, per gli imballaggi nascosti i valori delle colonne prezzo e iva, altrimenti veniva stampato 0 e non ivabile." &
                  " e gestiti entrambi i report (con e senza layout)." &
                  "<br/>- Rpt_Gandini_DocContabile_ConLayout.rpt: creato nuovo report.")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "Stampa bilancio disattivata.")

            Riga_Text("Bilancio :",
                  "- DS_Bilancio.xsd: eliminato il datatable del Bilancio di verifica." &
                  "<br/>- Rpt_Bilancio.rpt: eliminate sezione bilancio di verifica e sistemato layout." &
                  "<br/>- Bilancio.aspx: nuova versione." &
                  "<br/>- " &
                  "<br/>- ")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti :",
                  "- Saldo_Conti_Economici(): aggiunta la data di inizio." &
                  "<br/>- Query_Conti_Economici_Movimentati e Query_Conti_Patrimoniali_Movimentati: convert(date) della data di registrazione.")

            Riga_Fine()

            '==================================

            Riga_Data("12 Giugno 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "Gestiti i filtri e la chiamata alla nuova stampa del Bilancio di Verifica.")

            Riga_Text("BilancioVerifica :",
                  "Nuova stampa.")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti :",
                  "- Query_Conti_Economici_Movimentati e Query_Conti_Patrimoniali_Movimentati: gestito il filtro sul saldo iniziale <>0. " &
                  "<br/>- Saldo_Conti_Patrimoniali: aggiungo filtro sul saldo <> 0 e inserito order by." &
                  "<br/>- Saldo_Conti_Economici: aggiungo filtro sul saldo <> 0.")

            Riga_Fine()

            '==================================

            Riga_Data("11 Giugno 2015")

            Riga_Requisiti("Componenti '2015-04-14' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "Database_OperazioniPreliminari: introdotto update del campo CodificaContoPat.")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti  :",
                  "- Query_Conti_Patrimoniali_Movimentati(): corretta parte di lettura dei saldi iniziali" &
                  "- Query_Conti_Economici_Movimentati(): corretta parte di lettura dei saldi iniziali")

            Riga_Text("Mastrino.aspx  :",
                  "Correzione sezioen dei saldi sui conti economici.")

            Riga_Fine()

            '==================================


            Riga_Data("8 Giugno 2015")

            Riga_Requisiti("Componenti '' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalizzate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Rpt_SchedaCampagna_Multicentro.rpt, Rpt_SchedaCampagna_Semplificata.rpt :",
                  "Allargata la colonna Qta nella sezione semine.")

            Riga_Text("Schede Campagna:",
                  "Introdotta stampa logo in alto a destra (deve essere presente il file immagine nella cartella linkata in configurazione_siti 'Path_Directory_Loghi_Cliente' e deve avere nome PivaSuperUser_Logo_SchedaCampagna.jpg o .bmp)")

            Riga_Text("Scheda Campagna Semplificata e Global (singolo centro):",
                  "Abbassate le righe del layout nelle sezioni impianti.")

            Riga_Fine()

            '==================================


            Riga_Data("27 Maggio 2015")

            Riga_Requisiti("Componenti '' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalzizate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("StampePersonalizzate - GandiniValTrebbia :",
                "- Aggiunto campo num_colli al dataset." &
                "<br/>- descrizione dettaglio: tolto confezionamento;" &
                "<br/>- eliminato totale contenitori in fondo;" &
                "<br/>- gestita colonna separata del numero dei colli (num contenitori);" &
                "<br/>- gestita tipologia documento: fattura differita e fattura da bolla;" &
                "<br/>- sezione destinatario: usata stessa interlinea tra caselle di testo uguale alla sezione destinazione diversa." &
                "<br/>- ")

            Riga_Fine()

            '==================================

            Riga_Data("22 Maggio 2015 versione B")

            Riga_Requisiti("Componenti '' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalzizate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Requisiti("WEB.CONFIG :",
                  "Aggiunto in system.webServer, handlers e validation per poter vedere le immagini anche in anteprima del crystal ")

            Riga_Text("Scheda Campagna :",
                    "corretto baco che non visualizzava le altre lavorazioni.")

            Riga_Fine()

            '==================================

            Riga_Data("22 Maggio 2015 ")

            Riga_Requisiti("Componenti '' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalzizate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Requisiti("WEB.CONFIG :",
                  "Aggiunto in system.webServer, handlers e validation per poter vedere le immagini anche in anteprima del crystal ")

            Riga_Text("StampePersonalizzate - GandiniValTrebbia :",
                  "Modificato il layout. Aggiunto campo lav_cod al dataset.")

            Riga_Text("CRFattura.rpt, CRBolla.rpt :",
                    "Inserito logo x Garuti.")

            Riga_Fine()

            '==================================

            Riga_Data("11 Maggio 2015 ")

            Riga_Requisiti("Componenti '' :",
                       "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalzizate)")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("VisualizzatoreReport.aspx :",
                  "- Corretto baco sull'impossibilità di passare dalla pagina 2 alla 3." &
                  "")

            Riga_Fine()

            '==================================

            Riga_Data("8 Maggio 2015 versione B")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalzizate)")

            Riga_Requisiti("Migra '369' e Aggancio '55' :",
                      "Migra tabella RicXConti e RicXConti_Patrimonio  Movimenti_Dettagli: aggiunti Qta_Dettaglio1 Qta_Dettaglio2 e tabella Mov_Destinazioni: Qta_Dest1, Qta_Dest2")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda Colturale Bio :",
                  "- Corretto baco sul caricamento avversità." &
                  "- Modificato layout report.")

            Riga_Fine()

            '==================================

            Riga_Data("8 Maggio 2015")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalzizate)")

            Riga_Requisiti("Migra '369' e Aggancio '55' :",
                      "Migra tabella RicXConti e RicXConti_Patrimonio  Movimenti_Dettagli: aggiunti Qta_Dettaglio1 Qta_Dettaglio2 e tabella Mov_Destinazioni: Qta_Dest1, Qta_Dest2")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda Campagna / Scheda Campagna Bio :",
                  "aggiunta nella sezione 'Altre lavorazioni' la lettura delle operazioni registrate come 'Altre Lavorazioni' con eventualmente attivita inclusa.")

            Riga_Fine()

            '==================================


            Riga_Data("28 Aprile 2015")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '373' :",
                     "tabella configurazione_Stampe (per stampe personalzizate)")

            Riga_Requisiti("Migra '369' e Aggancio '55' :",
                      "Migra tabella RicXConti e RicXConti_Patrimonio  Movimenti_Dettagli: aggiunti Qta_Dettaglio1 Qta_Dettaglio2 e tabella Mov_Destinazioni: Qta_Dest1, Qta_Dest2")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda Campagna :",
                  "modificata label sezione semina in 'Lotto Impianto' x tutte le schede.")

            Riga_Text("StampePersonalizzate - GandiniValTrebbia  :",
                   "Gandini_DocContabile.aspx e Rpt_Gandini_DocContabile.rpt: primo rilascio.")

            Riga_Text("GestioneRichieste.aspx  :",
                  "Attivata lettura configurazione_stampe (nelle precedenti release avevo commentato la lettura).")

            Riga_Fine()

            '==================================

            Riga_Data("27 Aprile 2015 - Nuova Gestione ARROTONDAMENTI")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '369' e Aggancio '55' :",
                      "Migra tabella RicXConti e RicXConti_Patrimonio  Movimenti_Dettagli: aggiunti Qta_Dettaglio1 Qta_Dettaglio2 e tabella Mov_Destinazioni: Qta_Dest1, Qta_Dest2")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Contabilita :",
                  "DocumentiContab.vb: introdotto divisorio tra i dati del confezionamento.")

            Riga_Text("FreshAndFood - Bolla:",
                  "- Rpt_BollaFF.rpt: Introdotto articolo DPR e data e ora ingresso. " &
                  "<br/>- Bolla_FF.aspx: correzione bug trasporto, gestita data e ora ingresso, gestiti correttamente tutti i alv_cod accettazione." &
                  "<br/>- AgronicaCoreStampeDAL.FreshAndFood.StampaBolla: adeguata la query per gestire bene il caso di distinta di carico e sistemato filtro lav_cod.")

            Riga_Text("CRFattura.rpt, CRBolla.rpt :",
                 "Inserito logo x Ortofrutta Grosseto.")

            Riga_Text("StampePersonalizzate - GandiniValTrebbia  :",
                    "Gandini_DocContabile.aspx e Rpt_Gandini_DocContabile.rpt: working progress")

            Riga_Text("DDT_BolleConf.aspx, Fattura_NotaAccredito.aspx :",
                  "- Nel caso di opzione su stampa codice articolo, visualizzato davanti alla descrizione." &
                  "<br/>- Gestito correttamente il peso in base a tipo_peso." &
                  "<br/>- Nella data di spedizione stampata anche l'ora.")

            Riga_Text("SchedaCampagna :",
                  "modificato il caricamento del referente aziendale per i contatti pubblici (segnalazione AgriBologna).")

            Riga_Fine()

            '==================================

            Riga_Data("17 Aprile 2015 - Nuova Gestione ARROTONDAMENTI")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '369' e Aggancio '55' :",
                      "Migra tabella RicXConti e RicXConti_Patrimonio  Movimenti_Dettagli: aggiunti Qta_Dettaglio1 Qta_Dettaglio2 e tabella Mov_Destinazioni: Qta_Dest1, Qta_Dest2")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("StampePersonalizzate :",
                  "Creati Rpt_Gandini_DocContabile.aspx e Rpt_Gandini_DocContabile.rpt: working progress.")

            Riga_Text("FreshAndFood  :",
                  "Bolla_FF.aspx e Rpt_BollaFF.rpt: primo rilascio.")

            Riga_Text("CRFattura.rpt, CRBolla.rpt :",
                "Inserito logo x Istine.")

            Riga_Fine()

            '==================================

            Riga_Data("14 Aprile 2015 - Nuova Gestione ARROTONDAMENTI")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '369' e Aggancio '55' :",
                      "Migra tabella RicXConti e RicXConti_Patrimonio  Movimenti_Dettagli: aggiunti Qta_Dettaglio1 Qta_Dettaglio2 e tabella Mov_Destinazioni: Qta_Dest1, Qta_Dest2")

            Riga_Requisiti("Migra 'da fare' :",
                    "tabella configurazione_Stampe... in progress")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("AgronicaCoreStampeDAL.Configurazione_Stampe_R :",
                  "Nuovo core che legge le personalizzazioni delle stampe.")

            Riga_Text("GestioneRichieste.aspx  :",
                  "Legge configurazione_stampe per il codice report richiesto e verifica se è configurata una stampa personalizzata, in tal caso invia l'url alla funzione Gestione_Redirect.")

            Riga_Text("Etichette :",
                  "AnteprimaEtichette.aspx e FF_Etichette.vb: gestiti tutti i lav_cod.")

            Riga_Text("AgronicaCoreStampeDAL.FreshAndFood  :",
                  "StampaBolla: nuova query. ")

            Riga_Text("EstrattoreGrafica_XLS  :",
                  "Gestito errore di stampa per campionamenti su layer sbagliato. ")


            Riga_Fine()

            '==================================

            Riga_Data("10 Aprile 2015 - Nuova Gestione ARROTONDAMENTI")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '369' e Aggancio '55' :",
                            "Migra tabella RicXConti e RicXConti_Patrimonio  Movimenti_Dettagli: aggiunti Qta_Dettaglio1 Qta_Dettaglio2 e tabella Mov_Destinazioni: Qta_Dest1, Qta_Dest2")

            Riga_Requisiti("WEB.CONFIG :",
                        "Modifiche NO. ")

            Riga_Text("Fattura_NotaAccredito, RicevutaFiscale_GestioneStampa :",
                  "Gestito il flag sulla stampa del numero di vasca.")

            Riga_Text("CRFattura.rpt, CRBolla.rpt :",
                   "rimpicciolito il logo x Istine.")

            Riga_Fine()

            '==================================


            Riga_Data("31 Marzo 2015 - Nuova Gestione ARROTONDAMENTI")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '369' e Aggancio '55' :",
                            "Migra tabella RicXConti e RicXConti_Patrimonio  Movimenti_Dettagli: aggiunti Qta_Dettaglio1 Qta_Dettaglio2 e tabella Mov_Destinazioni: Qta_Dest1, Qta_Dest2")

            Riga_Requisiti("WEB.CONFIG :",
                        "Modifiche NO. ")

            Riga_Text("EstrattoConto_ClientiFornitori :",
                  "Pagamenti.aspx: nella stampa per singolo contatto, visualizzati anche piva e codice fiscale.")

            Riga_Text("CRFattura.rpt, CRBolla.rpt :",
                    "- rimpicciolito all'80% logo x Il Quarticello." &
                    "<br/>- Inserito il logo di Il Gallese.")

            Riga_Text("AgronicaCoreContabHLP.Contabilita.vb  :",
                  "GiasLan_Round: modifiche x nuova gestione arrotondamento (tiene conto della modalità con cui sono stati isneriti gli importi).")

            Riga_Text("Fattura_NotaAccredito.aspx, DDT_BolleConf.aspx, RicevutaFiscaleA4.aspx, RicevutaFiscaleA5.aspx, RicevutaWord.aspx, Registri_IVA_2.aspx, Registro_Corrispettivi_2.aspx, LiquidazioneIVA_Anteprima.aspx, RicevutaFiscale_GestioneStampa.vb, DocumentiContab.vb:",
                    "adeguamenti per le modifiche fatte alla classe di round.")

            Riga_Fine()

            '==================================

            Riga_Data("25 Marzo 2015")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '369' e Aggancio '55' :",
                    "Migra tabella RicXConti e RicXConti_Patrimonio  Movimenti_Dettagli: aggiunti Qta_Dettaglio1 Qta_Dettaglio2 e tabella Mov_Destinazioni: Qta_Dest1, Qta_Dest2")

            Riga_Requisiti("WEB.CONFIG :",
                        "Modifiche NO. ")

            Riga_Text("Global.asax :",
                "Modifiche per corretta gestione dell'errore GRILLO DOCET")

            Riga_Text("Custom500.aspx :",
                    "Modifiche per corretta gestione dell'errore GRILLO DOCET")

            Riga_Text("GestioneRichieste.aspx  :",
                  "stampa bolle di conferimento (1050,1052): andavano nel layout fattura se chkprezzo = 1 (forzato su layout ddt).")

            Riga_Text("AgronicaCoreContabHLP.Contabilita.vb:",
             "class GiasLan_Round: ripristinata alla versione prima del 9 febbraio (le modifiche fatte sono in conflitto con arrotondamento valori da importo totale di riga).")

            Riga_Text("RicevutaFiscale_GestioneStampa.vb :",
                  "Modifica sul totale documento: utilizzato l'importo generato dall'algoritmo di round e non num_protocollo salvato.")

            Riga_Text("Riba  :",
                  "nel record di tipo 14 il campo tipo codice non ha valore fisso 4 .")

            Riga_Text("Scheda Campagna :",
                  "- Introdotta sezione 'Personale in possesso del patentino' (era solo nel registro trattamenti)." &
                  "- Corretto baco responsabili sbagliati (quelli legati ai centri aziendali)." &
                  "- Corretto baco (+ infinito nel dosaggio Hl delle operazioni senza acqua)." &
                  "- Corretto baco (Non visualizzava le varietà nella sezione raccolta in caso di raccolte previste)." &
                  "- Aggiunta l'indicazione Terzista." &
                  "- Aggiunta l'indicazione della marca della macchina (se presente)." &
                  "- Introdotta opzione per stampare il nome appezzamento (necessita Profilazione versione 24-03-2015)." &
                  "- La dicitura Data/Firma_______________ diventa Firma Terzista _____ ed appare solo se è stato associato un terzista all'intervento e se non c'è l'opzione nascondi firma nell'utente.")

            Riga_Fine()

            '==================================

            Riga_Data("27 Febbraio 2015")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '369' e Aggancio '55' :",
                    "Migra tabella RicXConti e RicXConti_Patrimonio  Movimenti_Dettagli: aggiunti Qta_Dettaglio1 Qta_Dettaglio2 e tabella Mov_Destinazioni: Qta_Dest1, Qta_Dest2")

            Riga_Requisiti("WEB.CONFIG :",
                        "Modifiche NO. ")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "- caricati i contatti in base al filtro sul cod_rapporto (non sulla configurazione del rapporto contabile)" &
                  "<br/>- Database_OperazioniPreliminari: effettuate le insert solo se si usano i conti Gias." &
                  "<br/>- Stampa: introdotto controllo sulle codifiche conti Gias (x tutti).")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti:",
                "Query_Conti_Patrimoniali_Movimentati: modificata per gestire la codifica dei conti automatizzati con i conti Gias.")

            Riga_Text("Mastrino e LibroGiornale  :",
                  "Introdotta lettura ai conti codificati, prima della query di lettura.")


            Riga_Fine()

            '==================================

            Riga_Data("11 Febbraio 2015")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '368' e Aggancio '55' x Fresh&Food :",
                    "Movimenti_Dettagli: aggiunti Qta_Dettaglio1 Qta_Dettaglio2 e tabella Mov_Destinazioni: Qta_Dest1, Qta_Dest2")

            Riga_Requisiti("Migra '364' :",
                      "tabella Utenti_Visibilita_Appoggio")

            Riga_Requisiti("WEB.CONFIG :",
                        "Modifiche NO. ")

            Riga_Text("Scheda Campagna :",
                  "- Corretto baco acqua." &
                  "- Corretto baco che visualizzava semine,raccolte e fioriture previste anche di anni fuori dalla data filtrata per la stampa.")

            Riga_Fine()

            '==================================

            Riga_Data("9 Febbraio 2015")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '368' e Aggancio '55' x Fresh&Food :",
                    "Movimenti_Dettagli: aggiunti Qta_Dettaglio1 Qta_Dettaglio2 e tabella Mov_Destinazioni: Qta_Dest1, Qta_Dest2")

            Riga_Requisiti("Migra '364' :",
                      "tabella Utenti_Visibilita_Appoggio")

            Riga_Requisiti("WEB.CONFIG :",
                        "Modifiche NO. ")

            Riga_Text("Ortofrutta, stampa etichette :",
                  "stampa etichette con NUOVI CAMPI float IN MOVIMENTI_DETTAGLI: Qta_Dettaglio_1, Qta_Dettaglio_2")

            Riga_Text("LiquidazioneIVA :",
                  "- LiquidazioneIVA_Anteprima.aspx: nel caso di filtro su tutti sezionali, inviata alla stampa il RegimeIva (se uguali per tutti)." &
                  "<br/>- LiquidazioneIVA_2.aspx: nel caso non venga impostato il regimeiva, salvato log nel file di log degli errori.")

            Riga_Text("AgronicaCoreContabHLP.Contabilita.vb:",
          "class GiasLan_Round: allineata FormImpostaArrotondamenti al Giaslan (modifica su segnalazione della Fiorini su arrotondamento nota di accredito).")

            Riga_Fine()

            '==================================

            Riga_Data("5 Febbraio 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '364' e Aggancio '55' x Fresh&Food :",
                  "")

            Riga_Requisiti("Migra '364' :",
                      "tabella Utenti_Visibilita_Appoggio")

            Riga_Text("Atto Notorio :",
                  "Modificato ordinamento (se un'appezzamento cadeva su due particelle di comuni diversi le sue righe venivano separate).")

            Riga_Fine()

            '==================================

            Riga_Data("2 Febbraio 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '364' e Aggancio '55' x Fresh&Food :",
                  "")

            Riga_Requisiti("Migra '364' :",
                      "tabella Utenti_Visibilita_Appoggio")

            Riga_Text("Scheda Campagna :",
                  "Reintrodotto Server.ScriptTimeout = 900 (spreafico andava in timeout).")

            Riga_Fine()

            '==================================

            Riga_Data("30 Gennaio 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '364' e Aggancio '55' x Fresh&Food :",
                  "")

            Riga_Requisiti("Migra '364' :",
                      "tabella Utenti_Visibilita_Appoggio")

            Riga_Text("Scheda Campagna :",
                  "- Modificato il caricamento delle fertilizzazioni per velocizzarlo.")

            Riga_Fine()

            '==================================

            Riga_Data("28 Gennaio 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '364' e Aggancio '55' x Fresh&Food :",
                  "")

            Riga_Requisiti("Migra '364' :",
                      "tabella Utenti_Visibilita_Appoggio")

            Riga_Text("Quadro_P :",
                  "- Corretto baco che non stampava il quadro P di un singolo centro." &
                  "- Introdotta la possibilità di visualizzare il centro (flag nel filtro pre-stampa)." &
                  "- Aggiunte colonne possesso, sup condotta, sup residua.")

            Riga_Text("Scheda Campagna :",
                  "- Modificato il caricamento dei trattamenti per velocizzarlo.")

            Riga_Fine()

            '==================================

            Riga_Data("26 Gennaio 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '364' e Aggancio '55' x Fresh&Food :",
                  "")

            Riga_Requisiti("Migra '364' :",
                      "tabella Utenti_Visibilita_Appoggio")

            Riga_Text("Esportatore Dati Grafici :",
                    "- Corretti bug.")

            Riga_Fine()

            '==================================

            Riga_Data("22 Gennaio 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '364' e Aggancio '55' x Fresh&Food :",
                  "")

            Riga_Requisiti("Migra '364' :",
                      "tabella Utenti_Visibilita_Appoggio")

            Riga_Text("CRFattura.rpt, CRBolla.rpt :",
                    "- Inseriti i loghi di Quarticello e Valmori Esmeraldo.")

            Riga_Text("Esportazioni OP Produttori:",
                      "- aggiunte 4 colonne con descrizioni prov e com." &
                      "- corretto baco sui terreni fuori regione che davano errore se si indicava la regione con 2 caratteri.")

            Riga_Fine()

            '==================================

            Riga_Data("15 Gennaio 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '364' e Aggancio '55' x Fresh&Food :",
                  "")

            Riga_Requisiti("Migra '364' :",
                      "tabella Utenti_Visibilita_Appoggio")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Esportazioni OP Produttori:",
                "- aggiunte 4 colonne con descrizioni prov e com." &
                "- corretto baco sui terreni fuori regione che davano errore se si indicava la regione con 2 caratteri.")

            Riga_Fine()

            '==================================

            Riga_Data("12 Gennaio 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '364' e Aggancio '55' x Fresh&Food :",
                    "")

            Riga_Requisiti("Migra '364' :",
                      "tabella Utenti_Visibilita_Appoggio")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI/NO. ")

            Riga_Text("Adesione DPI:",
                "- tolto campo HTML, inserito campo statico")

            Riga_Fine()



            '==================================

            Riga_Data("9 Gennaio 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '364' e Aggancio '55' x Fresh&Food :",
                    "")

            Riga_Requisiti("Migra '364' :",
                      "tabella Utenti_Visibilita_Appoggio")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI/NO. ")

            Riga_Text("allegato catasto :",
                "- aggiunto data inizio, ordinato aggiunte sommatorie")

            Riga_Text("atto notorio:",
                "- aggiunto data inizio, ordinato aggiunte sommatorie")

            Riga_Text("filtro ricerca (atto notorio /catasto ecc):",
                    "- aggiunta data inizio, ordinato")

            Riga_Text("CRFattura.rpt, CRBolla.rpt :",
                "- centrato logo x La Rizzola." &
                "<br/>- Inserito il logo di Istine e Baldetti.")

            Riga_Text("Registro_RiBa.aspx :",
                  "- Gestito il filtro dello stato nel report;" &
                  "<br/>- introdotto controllo sulla selezione dei documenti e sulla selezione della banca in fase di esportazione.")

            Riga_Fine()

            '==================================

            Riga_Data("30 Dicembre 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '364' e Aggancio '55' x Fresh&Food :",
                    "")

            Riga_Requisiti("Migra '364' :",
                      "tabella Utenti_Visibilita_Appoggio")

            Riga_Requisiti("WEB.CONFIG :",
                      "Modifiche SI. " &
                      "Introdotta chiave <httpRuntime requestValidationMode='2.0' /> necessaria x stampa scheda OP (utilizzo parametro con stringa html).")

            Riga_Text("Stampe OP :",
                  "implementate.")

            Riga_Text("Esportazioni OP Produttori e Catasto :",
                  "implementate.")

            Riga_Text("Fresh and Food :",
                  "prima versione etichetta di sotto confezione.")

            Riga_Fine()

            '==================================

            Riga_Data("5 Dicembre 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '364' e Aggancio '55' x Fresh&Food :",
                    "")

            Riga_Requisiti("Migra '364' :",
                      "tabella Utenti_Visibilita_Appoggio")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("RegistroCorrispettivi :",
                  "cls_SottoReport_IVA.vb: " &
                  "modifiche a DtIVAGenerale_ValorizzaDa_DtRiepilogoIVA e a DtIVAGenerale_ValorizzaDa_DtRiepilogoIVA_Inner " &
                  "per gestire il caso di DT_IVA_Round senza le colonne sulla % iva indetraibile/compensazione.")

            Riga_Text("CRFattura.rpt, CRBolla.rpt :",
                    "- sistemata la visibilità della ragione sociale di La Rizzola nelle fatture." &
                    "<br/>- Inserito il logo di Tenuta Godenza e Gallo Nero.")

            Riga_Fine()

            '==================================

            Riga_Data("27 Novembre 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '364' e Aggancio '55' x Fresh&Food :",
                    "")

            Riga_Requisiti("Migra '364' :",
                      "tabella Utenti_Visibilita_Appoggio")

            Riga_Requisiti("WEB.CONFIG :",
                        "Modifiche NO. ")

            Riga_Text("Filtro centri aziendali :",
                       ".")

            '==================================

            Riga_Data("26 Novembre 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '346' x Giaslan :",
                        "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("Migra '364' e Aggancio '55' x Fresh&Food :",
                    "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda campagna  2colonna raccolta :",
                  "non si vedeva CodiceAppezzamento().")

            '==================================

            Riga_Data("25 Novembre 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '346' x Giaslan :",
                        "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("Migra '364' e Aggancio '55' x Fresh&Food :",
                    "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda campagna :",
                  "raccolta, rilievo fasi e semina prevista aggiunta parte per leggere codice appezzamento con nuova funzione CodiceAppezzamento().")

            '==================================

            Riga_Data("24 Novembre 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '346' x Giaslan :",
                        "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("Migra '364' e Aggancio '55' x Fresh&Food :",
                    "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("CRFattura.rpt :",
                  "Introdotto S.E.&O.")

            Riga_Text("GestioneRichieste.aspx  :",
                    "Introdotto controllo.")

            Riga_Text("scheda di campagna global (ma anche altre probabilmente):",
                  "la data raccolta prevista era del primo impianto, probabilmente anche altri parametri, la parte dove faceva assegnazioni defaults(Counter).Data_Raccolta() aveva 0 invece di j nella DtImpianti.Rows(j).Item")

            Riga_Text("Fresh and Food:",
                  "Prima versione in BETA per la stampa di etichette.")

            Riga_Text("RegistroRiBa:",
                  "corretto baco per codice abi diverso fra record IB e EF.")

            Riga_Fine()

            '==================================

            Riga_Data("13 Novembre 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Sheda Campagna.aspx:",
                  "Introdotta nelle note l'indicazione dell'epoca/fase fenologica salvata nel dosaggio utilizzato. (escluse le schede mono-centro Rpt_SchedaCampagna_EUREP_GAP_Semplificata.rpt e Rpt_SchedaCampagna_Semplificata.rpt).")

            Riga_Fine()

            '==================================

            Riga_Data("12 Novembre 2014 - Versione B")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("CRFattura.rpt, CRBolla.rpt :",
                "Modificato il logo di Poggio Regini.")

            Riga_Text("LiquidazioneIVA_2.aspx :",
                  "Corretto errore nel caso in cui i dataset dell'iva fossero vuoti.")

            Riga_Fine()

            '==================================

            Riga_Data("12 Novembre 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("LiquidazioneIVA :",
                 "- LiquidazioneIVA_Anteprima.aspx: se si selezionano tutti i sezionali, introdotto controllo sul regime iva, " &
                 "<br/>visualizzato regime iva ed eliminato controllo su iva= 0 se % indetraibile = 100." &
                 "<br/>- LiquidazioneIVA_2.aspx: gestita la liquidazione in base al regime iva e ricalcolati i totali in base all'iva detraibile e all'iva non in compensazione." &
                 "<br/>- Rpt_LiquidazioneIVA_2.rpt: gestiti nuovi sottoreport iva acquisti e vendite.")

            Riga_Text("RegistriIVA :",
               "Finita gestione iva in compensazione e iva indetraibile.")

            Riga_Text("RegistroCorrispettivi :",
                  "Adeguate le chiamate alle funzioni di GiaslanRound.")

            Riga_Text("Sottoreport_IVA :",
               "Finita gestione iva in compensazione e iva indetraibile.")

            Riga_Text("DocumentiContab.vb :",
              "- Adeguate le chiamate alle funzioni di GiaslanRound." &
              "<br/>- Leggi_Indirizzi: sistemata la gestione dello stato, per capire se è stato membro o no.")

            Riga_Text("Fattura_NotaAccredito.aspx :",
               "Gestito ordine di acquisto.")

            Riga_Text("GestioneRichieste.aspx  :",
                  "Link alla stampa dell'ordine di acquisto.")

            Riga_Text("Mastrino / giornale  :",
                  "AgronicaCoreStampeDAL.PianoConti: Conti_Economici_Movimentati e Query_Conti_Patrimoniali_Movimentati: gestita iva in compensazione.")

            Riga_Text("Sheda Campagna.aspx:",
                  "- Corretto baco che dimezzava l'acqua in caso di miscela di più prodotti e per le operazioni salvate con il LAN (non salvano la sup_trattata in qta2 di mov_destinazioni). (SOLO in alcuni casi con cifre anomale salvate da SQL!!!!!!!)" &
                  "- Per le sezioni fertilizzazioni e trattamenti, spostata l'indicazione del N.App. nella colonna note. (escluse le schede mono-centro Rpt_SchedaCampagna_EUREP_GAP_Semplificata.rpt e Rpt_SchedaCampagna_Semplificata.rpt).")

            Riga_Fine()

            '==================================

            Riga_Data("28 Ottobre 2014 - NO X GiasLan")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.aspx, Fattura_NotaAccredito.aspx :",
                "- Inserito il logo di Ancarani, La Rizzola e modificato quello di Campanacci.")

            Riga_Text("RegistriIVA :",
                    "- Registri_IVA_2.aspx: Modifiche per gestione iva indetraibile." &
                    "<br/>- Rpt_RegistriIVA_2.rpt: gestiti entrambi i sottoreport dell'iva (prima ce n'era uno che sia usava in un caso e poi nell'altro).")

            Riga_Text("Sottoreport_IVA, AgronicaCoreContabHLP.Contabilita.vb  :",
                  "Modifiche per gestione iva indetraibile.")

            Riga_Text("LiquidazioneIVA :",
                  "- LiquidazioneIVA_2.aspx: per l'iva a credito, utilzizata ora quella detraibile (e non la totale);" &
                  "<br/>- ridotta l'intestazione e modificato font in Arial;" &
                  "<br/>- Rpt_LiquidazioneIVA_2.rpt: reimportato sottoreport iva a credito.")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "- modifica layout;" &
                  "- introdotta verifica presenza conti patrimoniali iva (se non presenti li inserisce).")

            Riga_Text("Contabilita  :",
                  "AgronicaCoreContabDAL.Contabilita_R.LeggiProdottoStampeContab: tolto 'consistenza zootecnica' per i trasformati animali.")

            Riga_Text("RicevutaFiscaleA4.aspx :",
                  "Corretto bug: errato il nome della descrizione della riga22. ")

            Riga_Text("Registro_RiBa.aspx  :",
                  "- Corretto caricamento degli istituti di credito;" &
                  "<br/>- Quando viene esportato il file delle riba, viene anche visualizzato a video.")

            Riga_Text("SchedaCampagna.aspx  :",
                  "Modificata funzione che valorizza l'App_Nome." &
                  "Se è settata l'impostazione utente viene utilizzata la scelta dell'utente " &
                  "altrimenti l'ordine di priorità è il seguente :" &
                  "  1. RiferimentoAlfanumerico nell'appezzamento" &
                  "  2. Numeri nell'App_Nome" &
                  "  3. Appezza - BaseCode")

            Riga_Text("Selezione_SchedaCampagna.aspx  :",
                  "- Aggiunto check sezione 'Informazioni/Dichiarazioni' (visualizzata poi nei registri multi-coltura e multi-centro)." &
                  "- Introdotto il filtro Fascicolo da stampare (x Umbria - viene visualizzato se impostato nelle impostazioni utente).")

            Riga_Text("SchedaCampagna.aspx  :",
                  "- Gestito il caricamento del dataset 'Informazioni/Dichiarazioni' (visualizzato poi nei registri multi-coltura e multi-centro)." &
                  "- Introdotto il filtro Fascicolo (x Umbria - viene visualizzato se impostato nelle impostazioni utente).")

            Riga_Text("Rpt_SchedaCampagnaMulti.rpt, Rpt_SchedaCampagna_EUREP_GAP_Multicentro.rpt, Rpt_SchedaCampagna_Multicentro.rpt  :",
                  "Aggiunta sezione 'informazioni/Dichiarazioni' (al momento visualizza le operazioni di NON Utilizzo fitofarmaci e fertilizzanti solo su centro).")

            Riga_Text("Report schede campagna vari:",
                  "Sistemato un pò il layout.")

            Riga_Fine()

            '==================================

            Riga_Data("3 Ottobre 2014 - SOLO X Gentili")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Filtro_SchedeBiologico.aspx :",
                  "Sistemato link nell'apertura pop-up.")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "- Corretta visibilità risorsa finanziaria;" &
                  "- passato numero di pagina al libro giornale." &
                  "- filtro mastrino e libro giornale: impostato filtro sezionale obbligatorio." &
                  "- reimpostato blu su sfondo titolo." &
                  "- introdotta scelta stampa di prova e definitiva e link ad elenco pdf archiviati.")

            Riga_Text("GestioneRichieste.aspx  :",
                  "Sistemato link alla stampa libro giornale.")

            Riga_Text("LibroGiornale :",
                  "- query di lettura: veniva passato filtro risorsa finanziaria su cassa -> sistemato" &
                  "<br/>- gestito numero di pagina dal filtro elaborati;" &
                  "<br/>- completato sviluppo.")

            Riga_Text("DocumentiContab.vb  :",
                  "Leggi_MovimentoDettaglio_DocContabile_NEW: nel caso di raggruppamento articoli non vengono letti i dettagli del confezionamento.")

            Riga_Text("ElencoReport.aspx :",
                  "Parametrizzato la categoria documento (e relativa gestione) per non gestire solo la scheda di campagna.")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti.Conti_Economici_Movimentati()  :",
                  "Gestita la lettura dell'iva indetraibile.")

            Riga_Fine()

            '==================================

            Riga_Data("22 Settembre 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("RegistroPreparazioniBio :",
                  "Rpt_RegistroPreparazioniBio.rpt: aggiornato il regolamento Bio.")

            Riga_Text("Stampa Scheda GLOBAL :",
                  "Corretto baco errore rilievo indici maturità.")

            Riga_Text("Stampa Scheda Campagna MultiCentro :",
                  "Corretto baco che non visualizzava i dati delle irrigazioni.")

            Riga_Text("Selezione_SchedaCampagna :",
                  "Introdotta lettura impostazione utente annata agraria.")

            Riga_Fine()

            '==================================

            Riga_Data("16 Settembre 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.aspx :",
                 "- in data 25/07/14 era stata disattivata la gestione modalità di pagamento (ma in realtà la sezione continuava ad essere stampata, ora non più visibile).")

            Riga_Text("Fattura_NotaAccredito.aspx :",
                  "- nel caso di stampa ddt e ddt corrispettivi sistemata la stampa delle note (sono salvate diversamente)" &
                  "<br/>- coordinate bancarie: nel caso della nota di accredito il dare/avere è ivertito, allineata la stampa vs/ns banca.")

            Riga_Fine()

            '==================================

            Riga_Data("2 Settembre 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Sheda Campagna.aspx:",
                  "- Corretto baco che sbagliava nome del centro aziendale nel N.App. nelle sezioni con più colonne (ES.raccolta, rilievi etc)." &
                  "- Corretto baco errore fasi fenologiche.")

            Riga_Text("Scheda Campagna.aspx:",
                  "Aggiunta indicazione DOSE HL nelle schede non multi-centro (semplificate usate ancora dal LAN).")

            Riga_Fine()

            '==================================

            Riga_Data("1 Settembre 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Sheda Campagna.aspx:",
                  "- Corretto baco che dava errore se si selezionava nel filtro una data secca." &
                    "- Aggiunta indicazione DOSE HL nelle schede multi-centro e nel registro trattamenti.")

            Riga_Fine()

            '==================================

            Riga_Data("28 Agosto 2014")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("GestioneRichieste.aspx :",
                  "Modifica su gestione stampe bio.")

            Riga_Text("Filtro_SchedeBiologico.aspx  :",
                  "working progress.")

            Riga_Text("Sheda Campagna.aspx:",
                  "Modificato algoritmo per creazione riferimento appezzamento. Necessita nuova profilazione (compilazione successivo 22/08/2014)" &
                  "- Corretto baco che dimezzava l'acqua in caso di miscela di più prodotti." &
                  "- Corretto baco che non visualizzava le fasi fenologiche (visualizzava le righe vuote).")

            Riga_Fine()

            '==================================

            Riga_Data("21 Agosto 2014")

            Riga_Requisiti("Componenti '2014-04-16' :",
                         "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                        "Modifiche NO. ")

            Riga_Text("File_Temporanei :",
                  "Aggiunta.")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "- Gestita eccezione se non ci sono risorse finanziarie sull'azienda." &
                  "<br/>- Spostato qui il controllo sul database (operazioni preliminari per i report dei conti).")

            Riga_Text("Contabilita - tutte le stampe  :",
                  "Sistemata dicitura 'stampato con software Gias di' e sistemato salvataggio log.")

            Riga_Text("RegistriIVA  :",
                    "Gestione delle fatture non contabilizzate (vengono scartate dalla query).")

            Riga_Text("LiquidazioneIVA :",
                  "Gestione delle fatture non contabilizzate (vengono scartate dalla query).")

            Riga_Text("Mastrino.aspx :",
                  "AgronicaCoreStampeDAL.PianoConti.Query_Conti_Patrimoniali_Movimentati(): gestita iva ns credito e iva ns debito.")

            Riga_Text("DDT_BolleConf.aspx e DocumentiContab.vb:",
                  "Gestita la lettura dell'opzione stampa numero vasca (per stampare o meno il numero di vasca).")

            Riga_Text("LibroGiornale  :",
                  "Nuovo report (in costruzione).")

            Riga_Fine()

            '==================================

            Riga_Data("12 Agosto 2014")

            Riga_Requisiti("Componenti '2014-04-16' :",
                         "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                    "Modifiche NO. ")

            Riga_Text("SchedaCampagna :",
                  "- DATASET: DS_Arboree.xsd, DS_Erbacee.xsd, DS_UsoNonAgricolo.xsd: i campi N_Campo e N_appezza impostati a string (per evitare errore di cast a integer)." &
                  "<br/>- SOTTOREPORT: creato Rpt_Erbacee.rpt utilizzato da Rpt_SchedaCampagnaMulti.rpt." &
                  "<br/>- REPORT: " &
                  "<br/>    a) Rpt_SchedaCampagnaMulti.rpt eliminato sottoreport interno Erbacee ed importato nuovo sottoreport esterno." &
                  "<br/>    b) Rpt_SchedaCampagna_Semplificata.rpt, Rpt_SchedaCampagna_Multicentro.rpt, Rpt_SchedaCampagna_EUREP_GAP_Semplificata.rpt, Rpt_SchedaCampagna_EUREP_GAP_Multicentro.rpt, Rpt_SchedaCampagna_ConserveItalia.rpt: sistemate formule dei sottoreport Arboree e Erbacee su N_Campo e N_Appezza." &
                  "<br/>- SchedaCampagna.aspx: " &
                  "<br/>    a) su CaricaDsImpianti_Multispecie corrette varie eccezioni generate quando nel nome dell'appezzamento ci sono molte cifre (vedi commenti nel codice)." &
                  "<br/>    b) scheda A Veneto: sistemata lettura resp. aziendale." &
                  "<br/>    c) CaricaDsErbacee: nella query sistemati id efault delle date, altrimenti dava errore la formula su di esse dentro al sottoreport." &
                    "<br/>    d) CaricaDsRilievoProduzioneRaccolta: corretto errore sull'ordinamento delle date di raccolta (vedi commento nel codice). ")

            Riga_Fine()

            '==================================


            Riga_Data("06 Agosto 2014")

            Riga_Requisiti("Componenti '2014-04-16' :",
                         "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                    "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx :",
                  "- Corrette dichiarazioni di Num_Appezzamento da integer a double." &
                  "<br/>- CaricaDsImpianti_Multispecie: cambiata select di n_campo e n_appezza.")

            Riga_Text("Rpt_SchedaCampagnaMulti.rpt  :",
                  "aumentato il numero righe del num appezzamento.")

            Riga_Text("DDT_BolleConf.aspx, Fattura_NotaAccredito.aspx :",
            "- Modificati i loghi di Podere dell'Angelo, Podere Vecciano. " &
            "<br/>- Gestione loghi di Campanacci, Gandolfi.")

            Riga_Fine()

            '==================================

            Riga_Data("04 Agosto 2014")

            Riga_Requisiti("Componenti '2014-04-16' :",
                         "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
               "Modifiche NO. ")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "Sistemato menù a tendina risorse finanziarie.")

            Riga_Text("Mastrino.aspx  :",
                  "- Query di update per risolvere bug del giaslan sui dati salvati." &
                  "- AgronicaCoreStampeDAL.PianoConti.MastrinoConti_Patrimoniali(): gestiti pagamenti e filtro contatti/risorse finanziarie." &
                  "- AgronicaCoreStampeDAL.PianoConti.MastrinoConti_Economici(): gestito filtro contatti.")

            Riga_Fine()

            '==================================

            Riga_Data("01 Agosto 2014 --- NO PER CLIENTI GIASLAN CONTAB")
            Riga_Data(" (Mastrino Contabile aperto a cozza) ")

            Riga_Requisiti("Componenti '2014-04-16' :",
                         "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("SchedaCampagna.aspx:",
                  "- Corretto baco del non caricamento del nome dei tecnici delle fertilizzazioni (caricava la stringa 'NOME')")

            Riga_Text("SchedaCampagna.aspx:",
                  "- Corretto baco : I rilievi piogge venivano ordinati in ordine alfabetico sul campo validita_inizio ")

            Riga_Fine()


            '==================================

            Riga_Data("25 Luglio 2014 --- NO PER CLIENTI GIASLAN CONTAB")
            Riga_Data(" (Mastrino Contabile aperto a cozza) ")

            Riga_Requisiti("Componenti '2014-04-16' :",
                        "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Filtro_ElaboratiContabili.aspx:",
                  "- Nell'ordinamento dell'estratto conto clienti/fornitori sostituito 'cliente' con 'contatto'." &
                  "<br/>- Mastrino contabile: attivati filtri sezionale, contatto, risorsa finanziaria.")

            Riga_Text("DDT_BolleConf.aspx :",
                  "Disattivata la stampa delle modalità di pagamento.")

            Riga_Text("Fattura_NotaAccredito.aspx, CRFattura.rpt :",
                  "Gestita la stampa anche del ddt normale, ma con visualzizazione dettagli economici.")

            Riga_Text("GestioneRichieste.aspx  :",
                  "Nel caso di ddt normale, ma con check 'stampa prezzi e sconti' linkato report della fattura.")

            Riga_Text("SchedaCampagna.aspx :",
                  "riga 1277, chiamata Contatti_Contatto_Leggi, passato flag_pubblico = false.")

            Riga_Fine()

            '==================================

            Riga_Data("30 Giugno 2014")

            Riga_Requisiti("Componenti '2014-04-16' :",
                        "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("AgronicaCoreStampeDAL.DocContab.DocumentiContabili :",
                  "Modificato join con Contatti (Contatti.Piva verificato che sia superuser).")

            Riga_Text("DDT_BolleConf.aspx, Fattura_NotaAccredito.aspx :",
             "- Inserito il logo di Tenuta Barbon, Poggio Regini, De Riz, Palazzona di Maggio.")

            Riga_Fine()

            '==================================

            Riga_Data("13 Giugno 2014")

            Riga_Requisiti("Componenti '2014-04-16' :",
                        "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Scheda Campagna :",
                  "- Corretto baco che non visualizza le raccolte.")

            Riga_Text("Scheda Campagna :",
                  "- Corretto baco che da errore nella sezione 'altre lavorazioni'." &
                  "- Corretto baco che non visualizza le piogge.")

            Riga_Text("Scheda Campagna :",
                  "Corretto baco che da errore in caso di stampa con logo ed il logo non esiste.")

            Riga_Fine()

            '==================================

            Riga_Data("10 Giugno 2014")

            Riga_Requisiti("Componenti '2014-04-16' :",
                        "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("GestioneRichieste.aspx :",
                  "Gestita stampa DDt Ricevuto (conferimento uva).")

            Riga_Text("DDT_BolleConf.aspx  :",
                  "Gestita stampa DDT Ricevuto (conferimento uva).")

            Riga_Text("EstrattoreGrafica_XLS.aspx  :",
                  "Prima versione estrattore grafica porting da stampe 2003.")

            Riga_Fine()

            '==================================

            Riga_Data("3 Giugno 2014")

            Riga_Requisiti("Componenti '2014-04-16' :",
                        "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("AgronicaCoreContabHLP.Contabilita.vb :",
                  "GiasLan_Round.FormAggiornaImporto: corretto bug nei campioni omaggio.")

            Riga_Text("Stampe Contabilita  :",
                  "- DocumentiContab.vb: corretto errore (dalla precedente versione), se non c'erano doc allegati scriveva rif. Doc. n." &
                  "- AgronicaCoreStampeDAL.DocContab.DocumentiContabili: aggiunta clausola di join, altrimenti per le aziende non superuser (ma contatti del superuser), venivano sdoppiati gli articoli.")

            Riga_Fine()

            '==================================

            Riga_Data("26 Maggio 2014")

            Riga_Requisiti("Componenti '2014-04-16' :",
                        "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Default.aspx :",
              "Nuovo  GiasVersioneCorrente.txt.")

            Riga_Text("RegistriIVA :",
                  "Rpt_RegistriIVA_2.rpt: il campo numero può andare su più righe.")

            Riga_Text("DDT_BolleConf.aspx  :",
                  "Corretto controllo sul campo ora (errore quando la query non restituisce dati).")

            Riga_Text("DocumentiContab.vb  :",
                  "Gestiti nei riferimenti doc allegati anche daa e mvv.")

            Riga_Text("Pagamenti.aspx  :",
                  "Visualizzato l'IBAN tutto attaccato.")

            Riga_Text("Filtro_ElaboratiContabili.aspx  :",
                  "Gestita la scelta dell'ordinamento dei registri iva.")

            Riga_Text("Registri_IVA_2.aspx :",
                  "Gestita la scelta dell'ordinamento dei registri iva." &
                  "AgronicaCoreStampeDAL.RegistriContab.RegistriIVA_2: adeguata la query")

            Riga_Text("DDT_BolleConf.aspx, Fattura_NotaAccredito.aspx :",
                "- Corretta la piva nel logo di Fragola De Bosc." &
                "- Inserito il logo di Tenuta Neri, Collemattoni, La Castellana, Fiorini e Tenuta dei Nanni.")

            Riga_Fine()

            '==================================

            Riga_Data("5 Maggio 2014")

            Riga_Requisiti("Componenti '2014-04-16' :",
                        "")

            Riga_Requisiti("Migra '346' :",
                       "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche NO. ")

            Riga_Text("Selezione scheda campagna :",
                  "Aggiunto bottone per aprire l'elenco delle schede di campagna degli impianti selezionati")

            Riga_Text("SchedaCampagna :",
                    "Corretto baco che replicava i principi attivi del primo prodotto anche per gli altri prodotti della miscela.")

            Riga_Text("SchedaCampagna Veneto :",
                  "- Aggiunto N.App nella scheda B (se c'è il campo mette anche il nome del campo)." &
                  "- Corretto l'arrotondamento delle sup app a 4 cifre decimali (troncava a 2).")

            Riga_Text("Filtro SchedaCampagna :",
                  "Sostituita la checklist con una radiobuttonlist x la stampa veneto (stampava comunque solo l'ultima selezionata) ---> da sistemare!!!.")

            Riga_Fine()

            '==================================

            Riga_Data("17 Aprile 2014 ")

            Riga_Requisiti("Componenti '2014-04-16' :",
                        "")

            Riga_Requisiti("Migra '346' :",
                    "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SUPER SERVER :",
                  "Inviate data inizio e fine alla query su tabella Connessioni (x risoluzione bug stampe su archivio storicizzazto).")

            Riga_Fine()

            '==================================

            Riga_Data("16 Aprile 2014 ")

            Riga_Requisiti("Componenti '2014-04-16' :",
                        "")

            Riga_Requisiti("Migra '346' :",
                    "tabella Imprese_Sezionali: aggiunto campo InteresseDebitoIva_Perc")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda Campagna :",
                  "Modificata la dicitura del 'Registro Trattamenti' con il nuovo decreto (150/12).")

            Riga_Text("LiquidazioneIVA  :",
                  "Gestita la % di interesse sull'iva a debito.")

            Riga_Text("SchedaRilievi.aspx :",
                  "modifiche per gestione report pivot.. nascoste colonne come da indicazione di Merlo")

            Riga_Text("Filtro_ElaboratiContabili.aspx  :",
                    "Se da querystring arriva anno non valorizzato, impostato su anno corrente.")

            Riga_Text("AgronicaCoreStampeDAL.PianoConti :",
                     "Query_Conti_Economici_Movimentati e Query_Conti_Patrimoniali_Movimentati: gestito il enum_ChkCoGe.CoGe_AUTOMATICO. ")

            Riga_Fine()

            '==================================

            Riga_Data("15 Aprile 2014 ")

            Riga_Requisiti("Componenti '2013-04-14' :",
                  "")

            Riga_Requisiti("Migra '345' :",
                        "tabella MisuraxAvversita: aggiunto campo ordine")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaRilievi.aspx :",
                  "modifiche per gestione report pivot.. gestione in stampa dell'anagrafica di misura x avversità (dato testuale associato a dato numerico) ")

            Riga_Fine()

            '==================================

            Riga_Data("11 Aprile 2014 ")

            Riga_Requisiti("Componenti '2013-11-22' :",
                         "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO. ")

            Riga_Text("SchedaRilievi.aspx :",
                  "modifiche per lettura progetto.")

            Riga_Fine()

            '==================================

            Riga_Data("01 Aprile 2014 ")

            Riga_Requisiti("Componenti '2013-11-22' :",
                         "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("SchedaRilievi.aspx :",
                  "corretto baco nella query che legge i dati per la scheda rilievi")

            Riga_Fine()

            '==================================

            Riga_Data("28 Marzo 2014")

            Riga_Requisiti("Componenti '2013-11-22' :",
                         "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.aspx:",
                  "- CRBolla.rpt: inserita sezione per i pagamenti." &
                  "<br/>- DDT_BolleConf.aspx: gestita lettura dei pagamenti.")

            Riga_Fine()

            '==================================

            Riga_Data("20 Marzo 2014")

            Riga_Requisiti("Componenti '2013-11-22' :",
                         "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO. ")

            Riga_Text("SchedaRilievi.aspx :",
                  "nuova stampa per monitoraggio prosecco valdobbiadene.")

            Riga_Fine()

            '==================================

            Riga_Data("27 Febbraio 2014")

            Riga_Requisiti("Componenti '2013-11-22' :",
                         "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO. ")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "Corretto bug che calcolava male le date del range temporale per i registri iva (cambiato il CaricaCombo dei mesi).")

            Riga_Fine()

            '==================================

            Riga_Data("26 Febbraio 2014")

            Riga_Requisiti("Componenti '2013-11-22' :",
                         "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO. ")

            Riga_Text("App_Class :",
                  "GestioneLog.vb: sostituito percorso di log da AgroWebconfig a objParametri.")

            Riga_Fine()

            '==================================

            Riga_Data("25 Febbraio 2014")

            Riga_Requisiti("Componenti '2013-11-22' :",
                         "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO. ")

            Riga_Text("LiquidazioneIVA_Anteprima.aspx :",
                  "- Sistemata grafica e calendario (erano cambiati i percorsi del CSS e delle librerie jquery)." &
                  "<br/>- Sbloccata casella iva da liquidazione precedente (disattivato il calcolo automatico che non è corretto e va adeguato)." &
                  "<br/>- Chiama il Calcola_Saldo prima di avviare la stampa." &
                  "<br/>- introdotti controlli sulle caselle manuali dell'iva.")

            Riga_Text("Rpt_LiquidazioneIVA_2.rpt  :",
                  "Sistemate le sezioni del report (erano tutte intestazione di pagina): se sforava la pagina veniva generato report vuoto.")

            Riga_Text("Registro_Corrispettivi_2.aspx :",
                  "Possibilità di filtro stampa note.")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "Filtro registro corrrispettivi: introdotto check per stampare o meno le note.")

            Riga_Fine()

            '==================================

            Riga_Data("24 Febbraio 2014")

            Riga_Requisiti("Componenti '2013-11-22' :",
                         "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.aspx, Fattura_NotaAccredito.aspx :",
                "Inserito logo di La Piana e Cavaliera.")

            Riga_Text("Magazzino - Giacenze :",
             "SchedaGiacenzeMagazzino_2.aspx: Attivata la gestione stampa lotto per tutte le categorie di magazzino.")

            Riga_Fine()

            '==================================

            Riga_Data("18 Febbraio 2014 --- SUPER SERVER MULTIDB UTENTI")

            Riga_Requisiti("Componenti '2013-11-22' :",
                         "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.aspx, Fattura_NotaAccredito.aspx :",
                    "Reinserito logo di La Spinosa e inserito logo di Fragola De Bosch.")

            Riga_Text("GESTIONE SUPER SERVER :",
                  "Gestito caso di più database utenti.")

            Riga_Fine()

            '==================================

            Riga_Data("10 Febbraio 2014")

            Riga_Requisiti("Componenti '2013-11-22' :",
                         "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO. ")

            Riga_Text("GestioneRichieste.aspx:",
                  "- introdotto controllo sull'objparametri, genera eccezione se non è completamente valorizzato." &
                  "<br/>- valorizzati proprietà mancanti sugli objparametri.")

            Riga_Text("Biologico - Contabilita - Magazzino:",
                  "- Modificati gli rpt, nelle impostazioni di pagina tolto il check sull'utilizzo dei margini della stampante." &
                  "<br/>- sistemata la grafica dei report.")

            Riga_Fine()

            '==================================

            Riga_Data("31 Gennaio 2014")

            Riga_Requisiti("Componenti '2013-11-22' :",
                         "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                         "Modifiche NO. ")

            Riga_Text("Non si sa la data in cui sono state fatte le modifiche ai core per gestione super server.  :",
                  "")

            '==================================

            Riga_Data("28 Gennaio 2014")

            Riga_Requisiti("Componenti '2013-11-22' :",
                  "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Scheda Campagna Veneto:",
                  "Implementata la scheda D (trattamenti derrate).")

            Riga_Text("SchedaCampagna :",
                  "Aggiunto il Responsabile Aziendale nell'intestazione delle schede (assieme al rappresentante legale).")

            Riga_Text("SchedaCampagna :",
                  "Corretto baco che a volte (in presenza anche di registrazione di trappole) non leggeva i principi attivi.")

            Riga_Text("RegistriIVA :",
                  "Rpt_RegistriIVA_2.rpt: sistemato layout (da alcuni veniva troncata la parte destra).")

            Riga_Fine()

            '==================================

            Riga_Data("15 Gennaio 2014")

            Riga_Requisiti("Componenti '2013-11-22' :",
                  "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("BilancioMastrino :",
                  "- sistemato layout (saldo troncato)" &
                  "<br/>- gestione scheda compensi" &
                  "<br/>- corretta lettura saldi conti patrimoniali con riferimento banca e contatto " &
                  "<br/>- corretto bug su gestione saldi iniziali contri patrimoniali")

            Riga_Fine()

            '==================================

            Riga_Data("10 Gennaio 2014")

            Riga_Requisiti("Componenti '2013-11-22' :",
                  "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("GestioneLogStampe :",
                  "Gestione_LogErrori_Stampe: aggiunti controlli sul nome del file e sottocartella.")

            Riga_Text("AgronicaCoreVarieBIZ.GestioneFile.vb :",
                  "SalvaReportPdf: aggiunti controlli sul nome del file e sottocartella.")

            Riga_Text("CRFattura.rpt, Iva.rpt :",
                  "Modificato layout del sottoreport Iva e reimportato su Fattura.")

            Riga_Fine()

            '==================================

            Riga_Data("08 Gennaio 2014")

            Riga_Requisiti("Componenti '2013-11-22' :",
                  "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("DDT_BolleConf.aspx, Fattura_NotaAccredito.aspx :",
                  "- Gestione loghi di Al Canevon, Collina Dei Poeti, Paolo Bea e Tenuta Folesano" &
                  "<br/>- Sistemata gestione log di errori." &
                  "<br/>- corretto bug nel sitomail.")

            Riga_Text("CRBolla.rpt, CRFattura.rpt  :",
                  "Inseriti loghi di Al Canevon, Collina Dei Poeti, Paolo Bea e Tenuta Folesano.")

            Riga_Text("RicevutaFiscaleA5.aspx, RicevutaFiscaleA4.aspx:",
                    "Sistemata gestione log di errori.")

            Riga_Text("BilancioConfronto:",
                 "Attivato.")

            Riga_Text("Filtro_ElaboratiContabili.aspx:",
                "sistemata configurazione pannelli per BilancioConfronto.")

            Riga_Fine()

            '==================================

            Riga_Data("20 Dicembre 2013")

            Riga_Requisiti("Componenti '2013-11-22' :",
                  "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("BilancioMatrino  :",
                "- Modificata la query, gestito l'incrocio del conto patrimoniale con il contatto anche nella parte di elttura dei documenti.")

            Riga_Text("Filtro_ElaboratiContabili.aspx :",
                  "sistemata grafica.")

            Riga_Fine()

            '==================================

            Riga_Data("19 Dicembre 2013")

            Riga_Requisiti("Componenti '2013-11-22' :",
                  "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Fatture, DDT, Ricevute, Note Accredito :",
                  "AgronicaCoreContabDAL.Materie_prime_Campion_R.Leggi_MateriaPrima_Campionature(): allineata a quella del 2003 " &
                  "<br/>(se nel dettaglio c'era vino sfuso, veniva stampata la docitura 'Campionatura' + voce di riepilogo registri).")

            Riga_Text("RegistroRiBa  :",
                  "- ripristinata la pagina (dopo le ultime mdoifiche si era disallineata)." &
                  "<br/>- attivato il pulsante di exit.")

            Riga_Text("BilancioMatrino  :",
                  "- Corretto bug nella query, sezione che legge il join con le risorse finanziarie." &
                  "<br/>- corretto bug nella paret di union che legeg i documenti: leggeva solo imponibile_netto, senza somamre l'iva.")

            Riga_Text("SchedaColturale BIO  :",
                  "working progress")

            Riga_Fine()

            '==================================

            Riga_Data("12 Dicembre 2013")

            Riga_Requisiti("Componenti '2013-11-22' :",
                  "")

            Riga_Requisiti("Migra '325' :",
                        "nuovi record in CategorieDocumenti.")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Contabilita:",
                  "Spostata tutta la sezione dalle stampe 2003.")

            Riga_Text("Biologico:",
                  "Spostate RegistroPreparazioniBio, SchedaMateriePrime, SchedaVendite (NON ATTIVE).")

            Riga_Text("Magazzino:",
                  "spostate SchedaGiacenzeMagazzino e SchedaMovimentiMagazzino (NON ATTIVE).")

            Riga_Text("SchedaCampagna  :",
                  "working progress.")

            Riga_Text("BilancioMatrino :",
                  "Nuovo report del mastrino contabile, creato a partire dal vecchio report movimenti mastro.")

            Riga_Fine()

            '==================================

            Riga_Data("19 Novembre 2013 - Versione B")

            Riga_Requisiti("Componenti '2013-11-18' :",
                  "")

            Riga_Requisiti("Migra '323' :",
                        "CategorieDocumenti insert Preventivo Emesso (23) e Bilancio (24).")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Bilancio :",
                "stampato prima lo stato patrimoniale del conto economico.")

            Riga_Fine()

            '==================================

            Riga_Data("19 Novembre 2013")

            Riga_Requisiti("Componenti '2013-11-18' :",
                  "")

            Riga_Requisiti("Migra '323' :",
                        "CategorieDocumenti insert Preventivo Emesso (23) e Bilancio (24).")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("GestioneRichieste.aspx :",
                  "- x le Riba viene chiamata direttamente la pagina Registro_RiBa.aspx (non si passa più da Filtro_ElaboratiContabili.aspx che faceva da ponte). " &
                  "<br/>- Gestione stampa preventivo.")

            Riga_Text("Fattura_NotaAccredito.aspx  :",
                  "- Gestione stampa preventivo.")

            Riga_Text("Bilancio :",
                  "- spostato report da stampe 2003." &
                  "- gestione stampa stato patrimoniale.")

            Riga_Text("BilancioConfronto  :",
                  "spostato report da stampe 2003.")

            Riga_Text("GestioneLog.vb :",
                  "Inserita classe per salvataggio log.")

            Riga_Fine()

            '==================================

            Riga_Data("21 Ottobre 2103 - NON VANNO PER GIASLAN")

            Riga_Requisiti("Componenti '2013-08-14' :",
                  "")

            Riga_Requisiti("Migra '318' :",
                       "- Aggiunta tabella CBI_Causali." &
                       "- migra dal 316 x tabella IVA_Aliquote: aggiunto campo Tipologia (necessario per registro corrispettivi).")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text("Fattura_NotaAccredito.aspx :",
                 "Gestiti gli omaggi con rivalsa dell'iva.")

            Riga_Text("GestioneStampe - Contabilita:",
                  "- DocumentiContab.vb: gestita nota integrativa nei dettagli." &
                  "<br/>- Allineate Fattura_NotaAccredito.aspx, DDT_BolleConf.aspx e RicevutaFiscale_GestioneStampa.vb.")

            Riga_Text("Quadro P  :",
                  "Gestito aggancio a online")

            Riga_Fine()

            '==================================

            Riga_Data("16 Ottobre 2013")

            Riga_Requisiti("ultima versione funzionante su Giaslan:",
            "")


            Riga_Requisiti("Componenti '2013-08-14' :",
                  "")

            Riga_Requisiti("Migra '318' :",
                       "- Aggiunta tabella CBI_Causali." &
                       "- migra dal 316 x tabella IVA_Aliquote: aggiunto campo Tipologia (necessario per registro corrispettivi).")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text(" :",
                  "")

            Riga_Text("  :",
                  "")

            Riga_Fine()

            '==================================

            Riga_Data("3 Ottobre 2010")

            Riga_Requisiti("Componenti '2013-08-14' :",
                  "")

            Riga_Requisiti("Migra '318' :",
                       "- Aggiunta tabella CBI_Causali." &
                       "- migra dal 316 x tabella IVA_Aliquote: aggiunto campo Tipologia (necessario per registro corrispettivi).")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche NO. ")

            Riga_Text(" :",
                  "")

            Riga_Text("  :",
                  "")

            Riga_Fine()

            '==================================

            Riga_Data("10 Settembre 2010")

            Riga_Requisiti("Componenti '' :",
                  "")

            Riga_Requisiti("Migra '' :",
                  "")

            Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche SI/NO. ")

            Riga_Text(" :",
                  "")

            Riga_Text("  :",
                  "")

            Riga_Fine()

            '==================================

            '##############################################
            '##############################################
            '##############################################

        End Sub


        '#################################################################################################
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            '
            Me.TabellaVersione.Rows.Clear()
            '
        End Sub



        '#################################################################################################
        Private Sub ImgBtn_Codice_Ins_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Codice_Ins.Click

            Dim chiaveSistema As String = "frigoriferoconcubetti"

            Dim chiaveUtente As String = Me.Txt_ChiaveAccesso.Text

            If chiaveUtente = "" Then
                'AgroMsgBox("Non sono ammessi valori nulli per la PASSWORD !!!", Page)
                Exit Sub
            End If

            If chiaveUtente <> chiaveSistema Then
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
        Private Sub Riga_Data(ByVal dataAggiornamento As String)

            If _objChangeSito Is Nothing Then
                Dim riga As New HtmlTableRow

                riga.Cells.Add(New HtmlTableCell)

                riga.Cells(0).BgColor = "#00bfff"
                riga.Cells(0).Height = "25px"
                riga.Cells(0).Attributes.Add("class", "Testo_08_Nero_Bold")
                riga.Cells(0).InnerText = dataAggiornamento

                Me.TabellaVersione.Rows.Add(riga)
            Else
                'Inizializzo tutto qui, visto che sono certa che sia il primo elemento del blocco
                _objChangeSito.versioni.Add(New Versione_Obj(dataAggiornamento))
            End If

        End Sub

        '#################################################################################################
        Private Sub Riga_Requisiti(ByVal titolo As String, ByVal testo As String, Optional ByVal ver As String = "")

            If _objChangeSito Is Nothing Then
                Dim riga As New HtmlTableRow

                riga.Cells.Add(New HtmlTableCell)

                riga.Cells(0).BgColor = "#FFFFC0"
                riga.Cells(0).Height = "20px"
                riga.Cells(0).Attributes.Add("class", "Testo_08_Rosso")

                riga.Cells(0).InnerHtml = "<b>" & titolo & If(Not String.IsNullOrEmpty(ver), " [" & ver & "]", "") & "</b><br>" & testo

                Me.TabellaVersione.Rows.Add(riga)
            Else
                _objChangeSito.versioni.Last().requisiti.Add(New Requisito_Obj(titolo, testo, ver))
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
        Private Sub Riga_Text(ByVal titolo As String, ByVal testo As String,
                          Optional ByVal cliente As String = "",
                          Optional ByVal idPerforma As Integer = 0,
                          Optional ByVal noteTecniche As String = "",
                          Optional ByVal noteTest As String = ""
                          )

            If _objChangeSito Is Nothing Then
                Dim riga As New HtmlTableRow

                riga.Cells.Add(New HtmlTableCell)

                riga.Cells(0).BgColor = "#c0ffc0"
                riga.Cells(0).Height = "20px"
                riga.Cells(0).Attributes.Add("class", "Testo_08_Nero")

                riga.Cells(0).InnerHtml = "<b>" & titolo & "</b><br>" & testo

                Me.TabellaVersione.Rows.Add(riga)
            Else
                _objChangeSito.versioni.Last().changelog.feature.Add(New Feature_Obj(True, titolo, testo, cliente, idPerforma, noteTecniche, noteTest))
            End If

        End Sub

        '#################################################################################################
        <Obsolete("Usare la Funzione Riga_Changelog() con primo parametro = enum_Tipo_Changelog.Bug")>
        Private Sub Riga_Bug(ByVal titolo As String, ByVal testo As String,
                         Optional ByVal cliente As String = "",
                         Optional ByVal idPerforma As Integer = 0,
                         Optional ByVal noteTecniche As String = "",
                         Optional ByVal noteTest As String = ""
                         )

            If _objChangeSito Is Nothing Then
                Dim riga As New HtmlTableRow

                riga.Cells.Add(New HtmlTableCell)

                riga.Cells(0).BgColor = "#c0ffc0"
                riga.Cells(0).Height = "20px"
                riga.Cells(0).Attributes.Add("class", "Testo_08_Nero")

                riga.Cells(0).InnerHtml = "<b>" & titolo & "</b><br>" & testo

                Me.TabellaVersione.Rows.Add(riga)
            Else
                _objChangeSito.versioni.Last().changelog.bugfix.Add(New Bugfix_Obj(True, titolo, testo, cliente, idPerforma, noteTecniche, noteTest))
            End If

        End Sub



        '#################################################################################################
        Private Sub Riga_Fine()

            If _objChangeSito Is Nothing Then
                Dim riga As New HtmlTableRow

                riga.Cells.Add(New HtmlTableCell)

                riga.Cells(0).BgColor = "whitesmoke"
                riga.Cells(0).Height = "20px"
                riga.Cells(0).Attributes.Add("class", "Testo_08_Nero")

                riga.Cells(0).InnerHtml = "&nbsp;"

                Me.TabellaVersione.Rows.Add(riga)
            End If

        End Sub

    End Class
End Namespace
