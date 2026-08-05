Public Class ImpostazioniAPP

    ' token per aprire Gias da app
    Public Property Token As String

    ' se true attiva modalità master app
    Public Property Master As Boolean

    ' 0 = Default, 1 = Manuale, 2 = Automatico
    Public Property TipoSincro As Integer

    ' 0 = Nessuna (solo frontiera), 1 = Parziale (AgroGSB), 2 = Completa
    Public Property Importazioni As String

    ' "" = Nessuno, "0" = Tutti, "lista lav_cod separati da virgola" x import su agenda
    Public Property ImportAgenda As String

    ' sincronizza piano colturale ultimi n anni agrari
    Public Property SincroAnni As Integer

    ' "" o "0" = No, "1" = Tutti, "lista tipi dati app da ripristinare separati da virgola"
    Public Property RestoreDati As String
    Public Property RestoreGiorni As Integer

    ' USATE SOLO DALL'APP
    ' default bozza (true = In Corso, false = Definitiva)
    Public Property Bozza As Boolean = True
    ' se true abilita lettura tag NFC
    Public Property NFC As Boolean
    ' esclude lavorazioni da gestione costi
    Public Property FiltroLavorazioni As String
    ' se true mostra solo magazzini del centro selezionato nelle attività
    Public Property FiltroMagazzini As Boolean
    ' default descrizione visite
    Public Property VisiteDescrizione As String
    ' gestione visite-rilievi
    Public Property VisiteRilievi As Boolean
    ' gestione visita con specie senza impianti
    Public Property VisiteSpecie As Boolean = True
    ' se true cambia label "ricetta" in "proposta di acquisto"
    Public Property PropostaAcquisto As Boolean
    ' versione custom dell'app (es: "1" per ENI Kenya)
    Public Property Custom As String
    ' lista lingue utente gestite (es: "en")
    Public Property Lingua As String
    ' gestione piano colturale offline
    Public Property Offline As Boolean = True
    ' forza sincro anagrafiche prodotti (Es: "10" per sementi)
    Public Property Prodotti As String
    ' se > 0 attiva gestione posizione nel disegno poligono
    Public Property Posizione As Integer
    ' se true attiva lettura acquisti da qr code
    Public Property QRCode As Integer
    ' link termini di utilizzo
    Public Property Termini As String
    ' link termini della privacy
    Public Property Privacy As String
    ' per forzare il refresh del token dopo x minuti
    Public Property RefreshToken As Integer
    ' per forzare la validità login a x minuti
    Public Property ValiditaLogin As Integer
    ' forza gestione per agenda NG
    Public Property GestioneAgenda As String
    ' sincronizza dati al login se cambia l'utente
    Public Property SincroLogin As Boolean = True
    ' se true controlla se è presente versione aggiornata dell'app
    Public Property CheckUpdate As Boolean = True
    ' se true la superficie non è modificabile se ricavata dal poligono
    Public Property BloccoArea As Boolean = False
    ' distanza minima in metri tra nuovo punto e l'ultimo aggiunto del poligono
    Public Property DistanzaMinima As Integer = 0
    ' distanza massima in metri tra nuovo punto e l'ultimo aggiunto del poligono
    Public Property DistanzaMassima As Integer = 0

    ' periodo validità nuovo impianto (0 = Default, 1 = AnnoAgrario, 2 = Manuale)
    Public Property TipoValidita As Integer = 0
    ' se true attiva gestione gerarchia aziende per gruppo utente
    Public Property GerarchiaImprese As Boolean = False
    ' attiva gestione risposta compressa su app ("" per disattivare)
    Public Property Encoding As String = "gzip"
    ' gestione invio dati aziende APP->WEB
    ' 0: non sincronizzare dati aziende non scelte e non visibili
    ' 1: sincronizza dati delle aziende non più scelte
    ' 2: invio dati delle aziende non più più visibili
    ' 3: invio dati delle aziende non più scelte e non più più visibili
    Public Property InviaDati As Integer = 0
    ' se 1 attiva cache dati comuni per velocizzare la sincro (0 = disattivata)
    Public Property CacheDati As Integer = 1
    ' se true esegue anche lo speed test oltre al solito controllo di connessione
    Public Property TestConn As Boolean = False
    ' se true attiva la comunicazione con la centralina BTM per il tracking dei trattori
    Public Property BTMConn As Boolean = False
    ' se true esegue il controllo della presenza del poligono nell'impianto
    Public Property CheckPoligono As Boolean = False
    ' se true attiva la gestione SAT sul GIS (deve essere abilitato anche l'utente)
    Public Property PrecisionFarming As Boolean = False
    ' se 1 possono essere aggiunti più prodotti alle raccolte (se 0 al massimo uno)
    Public Property ProdottiRaccolta As Integer = 1
    ' 0 = nessuna gestione, 1 = solo attività, 2 = attivà e ricette
    Public Property MagazziniEsterni As Integer = 0

    ' lista specie per sincro prodotti offline separati da "|"
    Public Property SpecieProdotti As String = ""
    ' se true fa mostrare le giacenze a 0 nell'app
    Public Property ShowZeroStock As Boolean = False
    Public Property MaxAziende As Integer = 50
    Public Property AutoSync As Boolean = False
    ' se false movimenti, consistenze e prelievi nascono in stato eseguito
    Public Property Bozza_Zoo As Boolean = False
    ' determina la percentuale bloccante di sovrapposizione dei poligoni
    Public Property BloccoSovrapposizione As Integer = 0
    ' attiva il menù degli overlay nella mappa
    Public Property VisualizzaOverlay As Boolean = False
    ' massimo livello di zoom nella mappa per la selezione delle aziende da sincronizzare
    Public Property MaxZoomMappaSync As Integer = 16
    ' minimo livello di zoom nella mappa per la selezione delle aziende da sincronizzare
    Public Property MinZoomMappaSync As Integer = 12
    Public Property LeggiStorico As Boolean = False
    ' intervallo temporale in minuti tra un salvataggio temporaneo e l'altro (0.25 min = 15 secondi)
    Public Property SavingInterval As Double = 0.25
    ' soglia del test della banda (in kb)
    Public Property TestConnectionValue As Integer = 400
    ' massima dimensione dell'allegato che può essere spedito da APP a WEB
    Public Property DocumentSizeLimit As Integer = 20
    Public Property StazioniMeteo As Boolean = True
    Public Property TipologiaDocumentoMeteo As Integer = -30

End Class
