using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    /// <summary>
    /// Dati di sede geografica di un'Azienda (stato, regione, città).
    /// I singoli campi sono nullable: se la geolocalizzazione è assente nel database GIAS,
    /// i campi corrispondenti sono <c>null</c>.
    /// </summary>
    /// <remarks>
    /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Output, Geolocalizzazione Completa.
    /// Catena di JOIN: ImpresexIndirizzi → Indirizzi → ISTAT → Lista_Province → Lista_Regioni
    ///                 → ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.
    /// </remarks>
    public class SedeDto
    {
        [JsonProperty("stato")]
        public string Stato { get; set; }

        [JsonProperty("regione")]
        public string Regione { get; set; }

        [JsonProperty("citta")]
        public string Citta { get; set; }
    }

    /// <summary>
    /// Posizione di un'Azienda nell'albero gerarchico della propria Filiera.
    /// </summary>
    /// <remarks>
    /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Output,
    /// Livello Gerarchia Zero per Capofiliera.
    /// </remarks>
    public class GerarchiaDto
    {
        [JsonProperty("id_azienda_parent")]
        public string IdAziendaParent { get; set; }

        [JsonProperty("livello_gerarchia")]
        public int LivelloGerarchia { get; set; }
    }

    /// <summary>
    /// Anagrafica completa di un'Azienda inclusa nella struttura di Filiera.
    /// </summary>
    /// <remarks>
    /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Output, aziende[].
    /// </remarks>
    public class AziendaDto
    {
        [JsonProperty("id_azienda")]
        public string IdAzienda { get; set; } = string.Empty;

        [JsonProperty("ragione_sociale")]
        public string RagioneSociale { get; set; } = string.Empty;

        [JsonProperty("sede")]
        public SedeDto Sede { get; set; } = new SedeDto();

        [JsonProperty("gerarchia")]
        public GerarchiaDto Gerarchia { get; set; } = new GerarchiaDto();
    }

    /// <summary>
    /// Metadati dell'Utente assegnato al profilo "Filiera Admin" per una Filiera specifica.
    /// Presente solo quando un Admin è effettivamente associato alla Filiera; <c>null</c> altrimenti.
    /// </summary>
    /// <remarks>
    /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Regole di Business,
    /// Mapping Admin di Filiera. Dati estratti da <c>Utenti</c>, <c>Utenti_Dettagli</c>,
    /// <c>Utenti_Tipologie</c> (SuperServer).
    /// </remarks>
    public class FilieraAdminDto
    {
        [JsonProperty("id_utente")]
        public string IdUtente { get; set; } = string.Empty;

        [JsonProperty("nome_utente")]
        public string NomeUtente { get; set; } = string.Empty;

        [JsonProperty("email_utente")]
        public string EmailUtente { get; set; } = string.Empty;
    }

    /// <summary>
    /// Struttura completa di una Filiera: anagrafica, admin assegnato e gerarchia aziendale.
    /// </summary>
    /// <remarks>
    /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Output, filiere[].
    /// L'elenco <c>Aziende</c> è ordinato per livello gerarchico ASC, poi per <c>IdAzienda</c> ASC.
    /// </remarks>
    public class FilieraCompletaDto
    {
        [JsonProperty("id_filiera")]
        public string IdFiliera { get; set; } = string.Empty;

        [JsonProperty("nome_filiera")]
        public string NomeFiliera { get; set; } = string.Empty;

        [JsonProperty("filiera_admin")]
        public FilieraAdminDto FilieraAdmin { get; set; }

        [JsonProperty("aziende")]
        public List<AziendaDto> Aziende { get; set; } = new List<AziendaDto>();
    }

    /// <summary>
    /// DTO di risposta per la struttura organizzativa completa di tutte le Filiere e Aziende
    /// presenti in piattaforma.
    /// </summary>
    /// <remarks>
    /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Output.
    /// FS1.02: API Anagrafica | Query Filiere e Aziende.
    /// Endpoint: GET /Anagrafica/filiere
    /// </remarks>
    public class StrutturaFiliereResponseDto
    {
        [JsonProperty("filiere")]
        public List<FilieraCompletaDto> Filiere { get; set; } = new List<FilieraCompletaDto>();
    }
}
