namespace AgronicaNetCore.APP.BIZ.Services.ImpostazioniApp;

public sealed class ImpostazioniAppModel
{
    /// <summary>Token per aprire Gias da app.</summary>
    public string Token { get; set; } = "";

    /// <summary>Se true attiva modalità master app (SDF).</summary>
    public bool Master { get; set; } = false;

    /// <summary>0 = Default, 1 = Manuale, 2 = Automatico.</summary>
    public int TipoSincro { get; set; } = 0;

    /// <summary>
    /// 0 = Nessuna (solo frontiera), 1 = Parziale (AgroGSB), 2 = Completa.
    /// </summary>
    public string Importazioni { get; set; } = "";

    /// <summary>"" = Nessuno, "0" = Tutti, "lista lav_cod separati da virgola" per import su agenda.</summary>
    public string ImportAgenda { get; set; } = "";

    /// <summary>Sincronizza piano colturale ultimi n anni agrari.</summary>
    public int SincroAnni { get; set; } = 0;

    /// <summary>"" o "0" = No, "1" = Tutti, "lista tipi dati app da ripristinare separati da virgola".</summary>
    public string RestoreDati { get; set; } = "";

    public int RestoreGiorni { get; set; } = 0;

    // ── USATE SOLO DALL'APP ──────────────────────────────────────────────────

    /// <summary>Default bozza (true = In Corso, false = Definitiva).</summary>
    public bool Bozza { get; set; } = true;

    /// <summary>Se true abilita lettura tag NFC.</summary>
    public bool NFC { get; set; } = false;

    /// <summary>Esclude lavorazioni da gestione costi.</summary>
    public string FiltroLavorazioni { get; set; } = "";

    /// <summary>Se true mostra solo magazzini del centro selezionato nelle attività.</summary>
    public bool FiltroMagazzini { get; set; } = false;

    /// <summary>Default descrizione visite.</summary>
    public string VisiteDescrizione { get; set; } = "";

    /// <summary>Gestione visite-rilievi.</summary>
    public bool VisiteRilievi { get; set; } = false;

    /// <summary>Gestione visita con specie senza impianti.</summary>
    public bool VisiteSpecie { get; set; } = true;

    /// <summary>Se true cambia label "ricetta" in "proposta di acquisto".</summary>
    public bool PropostaAcquisto { get; set; } = false;

    /// <summary>Versione custom dell'app (es: "1" per ENI Kenya).</summary>
    public string Custom { get; set; } = "";

    /// <summary>Lista lingue utente gestite (es: "en", "it,en").</summary>
    public string Lingua { get; set; } = "";

    /// <summary>Gestione piano colturale offline.</summary>
    public bool Offline { get; set; } = true;

    /// <summary>Forza sincro anagrafiche prodotti (es: "10" per sementi).</summary>
    public string Prodotti { get; set; } = "";

    /// <summary>Se > 0 attiva gestione posizione nel disegno poligono.</summary>
    public int Posizione { get; set; } = 0;

    /// <summary>Se true attiva lettura acquisti da qr code.</summary>
    public int QRCode { get; set; } = 0;

    /// <summary>Link termini di utilizzo.</summary>
    public string Termini { get; set; } = "";

    /// <summary>Link termini della privacy.</summary>
    public string Privacy { get; set; } = "";

    /// <summary>Se > 0 attiva refresh del token dopo x minuti.</summary>
    public int RefreshToken { get; set; } = 0;

    /// <summary>Sincronizza dati al login se cambia l'utente.</summary>
    public bool SincroLogin { get; set; } = true;

    /// <summary>Se true controlla se è presente versione aggiornata dell'app.</summary>
    public bool CheckUpdate { get; set; } = true;

    /// <summary>Se true la superficie non è modificabile se ricavata dal poligono.</summary>
    public bool BloccoArea { get; set; } = false;

    /// <summary>Distanza minima in metri tra nuovo punto e l'ultimo aggiunto del poligono.</summary>
    public int DistanzaMinima { get; set; } = 5;

    /// <summary>Distanza massima in metri tra nuovo punto e l'ultimo aggiunto del poligono.</summary>
    public int DistanzaMassima { get; set; } = 100;

    /// <summary>Se true attiva gestione gerarchia aziende per gruppo utente.</summary>
    public bool GerarchiaImprese { get; set; } = false;

    /// <summary>
    /// Gestione invio dati APP → WEB.
    /// 0: non sincronizzare dati aziende non scelte e non visibili;
    /// 1: sincronizza dati aziende non più scelte;
    /// 2: invio dati aziende non più visibili;
    /// 3: invio dati aziende non più scelte e non più visibili.
    /// </summary>
    public int InviaDati { get; set; } = 0;

    /// <summary>Attiva gestione risposta compressa su app. "" per disattivare.</summary>
    public string Encoding { get; set; } = "gzip";

    /// <summary>Lista specie per sincro prodotti offline separati da "|".</summary>
    public string SpecieProdotti { get; set; } = "";

    /// <summary>Tipologia periodo di validità dell'impianto (0 = Default, 1 = AnnoAgrario, 2 = Manuale).</summary>
    public int TipoValidita { get; set; } = 0;

    /// <summary>Attiva cache dati più comuni per velocizzare sincro.</summary>
    public int CacheDati { get; set; } = 1;

    /// <summary>Se attivo, esegue speed test oltre al solito controllo di connessione.</summary>
    public bool TestConn { get; set; } = false;

    /// <summary>Attiva la comunicazione con la centralina BTM per il tracking dei trattori.</summary>
    public bool BTNConn { get; set; } = false;

    /// <summary>Se attivo, esegue il controllo della presenza del poligono per l'impianto.</summary>
    public bool CheckPoligono { get; set; } = false;

    /// <summary>Attiva la gestione SAT sul GIS (deve essere abilitato anche l'utente).</summary>
    public bool PrecisionFarming { get; set; } = false;

    /// <summary>Se 1 possono essere aggiunti più prodotti alle raccolte (se 0 al massimo 1 prodotto).</summary>
    public int ProdottiRaccolta { get; set; } = 1;

    /// <summary>0 = nessuna gestione, 1 = solo attività, 2 = attività e ricette.</summary>
    public int MagazziniEsterni { get; set; } = 0;

    /// <summary>Se true fa mostrare le giacenze a 0 nell'app.</summary>
    public bool ShowZeroStock { get; set; } = false;

    public int MaxAziende { get; set; } = 50;

    public bool AutoSync { get; set; } = false;

    /// <summary>Se > 0, forza la validità del login a x minuti. Default configurazione siti = 240.</summary>
    public int ValiditaLogin { get; set; } = 600;

    /// <summary>Se impostato a false movimenti, consistenze e prelievi nascono in stato eseguito.</summary>
    public bool Bozza_Zoo { get; set; } = false;

    /// <summary>Determina la percentuale bloccante di sovrapposizione dei poligoni.</summary>
    public int BloccoSovrapposizione { get; set; } = 0;

    /// <summary>Attiva il menù degli overlay nella mappa.</summary>
    public bool VisualizzaOverlay { get; set; } = false;

    /// <summary>Massimo livello di zoom nella mappa per la selezione delle aziende da sincronizzare.</summary>
    public int MaxZoomMappaSync { get; set; } = 16;

    /// <summary>Minimo livello di zoom nella mappa per la selezione delle aziende da sincronizzare.</summary>
    public int MinZoomMappaSync { get; set; } = 12;

    public bool LeggiStorico { get; set; } = false;

    /// <summary>Intervallo temporale in minuti tra un salvataggio temporaneo e l'altro (0.25 min = 15 secondi).</summary>
    public double SavingInterval { get; set; } = 0.25;

    /// <summary>Soglia del test della banda (in kb).</summary>
    public int TestConnectionValue { get; set; } = 400;

    /// <summary>Massima dimensione dell'allegato che può essere spedito da APP a WEB.</summary>
    public int DocumentSizeLimit { get; set; } = 20;

    public bool StazioniMeteo { get; set; } = true;

    public int TipologiaDocumentoMeteo { get; set; } = -30;
}
