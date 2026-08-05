namespace AgronicaCoreModelsSTD.App
{
    public class ImpostazioniAPP
    {
        // Token per aprire Gias da app
        public string Token { get; set; }

        // Se true attiva modalità master app
        public bool Master { get; set; }

        // 0 = Default, 1 = Manuale, 2 = Automatico
        public int TipoSincro { get; set; }

        // 0 = Nessuna (solo frontiera), 1 = Parziale (AgroGSB), 2 = Completa
        public string Importazioni { get; set; }

        // "" = Nessuno, "0" = Tutti, "lista lav_cod separati da virgola" x import su agenda
        public string ImportAgenda { get; set; }

        // Sincronizza piano colturale ultimi n anni agrari
        public int SincroAnni { get; set; }

        // "" o "0" = No, "1" = Tutti, "lista tipi dati app da ripristinare separati da virgola"
        public string RestoreDati { get; set; }
        public int RestoreGiorni { get; set; }

        // USATE SOLO DALL'APP
        // Default bozza (true = In Corso, false = Definitiva)
        public bool Bozza { get; set; } = true;

        // Se true abilita lettura tag NFC
        public bool NFC { get; set; }

        // Esclude lavorazioni da gestione costi
        public string FiltroLavorazioni { get; set; }

        // Se true mostra solo magazzini del centro selezionato nelle attività
        public bool FiltroMagazzini { get; set; }

        // Default descrizione visite
        public string VisiteDescrizione { get; set; }

        // Gestione visite-rilievi
        public bool VisiteRilievi { get; set; }

        // Gestione visita con specie senza impianti
        public bool VisiteSpecie { get; set; } = true;

        // Se true cambia label "ricetta" in "proposta di acquisto"
        public bool PropostaAcquisto { get; set; }

        // Versione custom dell'app (es: "1" per ENI Kenya)
        public string Custom { get; set; }

        // Lista lingue utente gestite (es: "en")
        public string Lingua { get; set; }

        // Gestione piano colturale offline
        public bool Offline { get; set; } = true;

        // Forza sincro anagrafiche prodotti (Es: "10" per sementi)
        public string Prodotti { get; set; }

        // Se > 0 attiva gestione posizione nel disegno poligono
        public int Posizione { get; set; }

        // Se true attiva lettura acquisti da qr code
        public int QRCode { get; set; }

        // Link termini di utilizzo
        public string Termini { get; set; }

        // Link termini della privacy
        public string Privacy { get; set; }

        // Per forzare il refresh del token dopo x minuti
        public int RefreshToken { get; set; }

        // Per forzare la validità login a x minuti
        public int ValiditaLogin { get; set; }

        // Forza gestione per agenda NG
        public string GestioneAgenda { get; set; }

        // Sincronizza dati al login se cambia l'utente
        public bool SincroLogin { get; set; } = true;

        // Se true controlla se è presente versione aggiornata dell'app
        public bool CheckUpdate { get; set; } = true;

        // Se true la superficie non è modificabile se ricavata dal poligono
        public bool BloccoArea { get; set; } = false;

        // Distanza minima in metri tra nuovo punto e l'ultimo aggiunto del poligono
        public int DistanzaMinima { get; set; } = 0;

        // Distanza massima in metri tra nuovo punto e l'ultimo aggiunto del poligono
        public int DistanzaMassima { get; set; } = 0;

        // Periodo validità nuovo impianto (0 = Default, 1 = AnnoAgrario, 2 = Manuale)
        public int TipoValidita { get; set; } = 0;

        // Se true attiva gestione gerarchia aziende per gruppo utente
        public bool GerarchiaImprese { get; set; } = false;

        // Attiva gestione risposta compressa su app ("" per disattivare)
        public string Encoding { get; set; } = "gzip";

        // Gestione invio dati aziende APP->WEB
        // 0: non sincronizzare dati aziende non scelte e non visibili
        // 1: sincronizza dati delle aziende non più scelte
        // 2: invio dati delle aziende non più più visibili
        // 3: invio dati delle aziende non più scelte e non più più visibili
        public int InviaDati { get; set; } = 0;

        // Se 1 attiva cache dati comuni per velocizzare la sincro (0 = disattivata)
        public int CacheDati { get; set; } = 1;

        // Se true esegue anche lo speed test oltre al solito controllo di connessione
        public bool TestConn { get; set; } = false;

        // Se true attiva la comunicazione con la centralina BTM per il tracking dei trattori
        public bool BTMConn { get; set; } = false;

        // Se true esegue il controllo della presenza del poligono nell'impianto
        public bool CheckPoligono { get; set; } = false;

        // Se true attiva la gestione SAT sul GIS (deve essere abilitato anche l'utente)
        public bool PrecisionFarming { get; set; } = false;

        // Se 1 possono essere aggiunti più prodotti alle raccolte (se 0 al massimo uno)
        public int ProdottiRaccolta { get; set; } = 1;

        // 0 = nessuna gestione, 1 = solo attività, 2 = attività e ricette
        public int MagazziniEsterni { get; set; } = 0;
    }
}
