using AgronicaCoreModelsSTD.attivita;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.ConfrontiFasiFenologicheQDCA.Models
{
    /// <summary>
    /// Rappresenta una singola fase fenologica BBCH da confrontare con il QDCA.
    /// Riferimento: DS02-BL ConfrontiFasiFenologicheQDCA — Input elenco_fasi.
    /// </summary>
    public sealed class FaseFenologicaQDCAInput
    {
        /// <summary>Codice BBCH della fase (es. "65"). Chiave di identificazione univoca per il confronto QDCA.</summary>
        public string BbchCod { get; init; } = string.Empty;

        /// <summary>Descrizione leggibile della fase (es. "Piena fioritura").</summary>
        public string BbchDescrizione { get; init; } = string.Empty;

        /// <summary>Data stimata di raggiungimento della fase.</summary>
        public DateTime DataFase { get; init; }
    }

    /// <summary>
    /// Input per il servizio di confronto e sincronizzazione fasi fenologiche QDCA.
    /// Riferimento: DS02-BL ConfrontiFasiFenologicheQDCA — Input.
    /// </summary>
    public sealed class ConfrontiFasiFenologicheQDCAInput
    {
        /// <summary>Partita IVA dell'azienda (obbligatoria).</summary>
        public string Piva { get; init; } = string.Empty;

        /// <summary>Codice centro aziendale (SA_COD).</summary>
        public int SaCod { get; init; }

        /// <summary>Numero appezzamento (APPEZZA).</summary>
        public int Appezza { get; init; }

        /// <summary>Identificativo registrazione impianto (ID_REG).</summary>
        public int IdReg { get; init; }

        public int VegCod { get; init; }

        /// <summary>
        /// Codice progetto (Imprese_Progetti.Progetto_Cod) dell'esercizio corrente.
        /// Usato come Id_Esercizi per filtrare le fasi storiche nel QDCA.
        /// Deve essere maggiore di zero.
        /// </summary>
        public int ProgettoCod { get; init; }

        public decimal SuperficieImpianto { get; init; }

        /// <summary>Fasi fenologiche ricevute dall'engine (DS01-BL) da confrontare con il QDCA.</summary>
        public IReadOnlyList<FaseFenologicaQDCAInput> ElencoFasi { get; init; } = Array.Empty<FaseFenologicaQDCAInput>();
    }

    /// <summary>
    /// Risultato del confronto tra fasi fenologiche ricevute dall'engine e quelle storiche nel QDCA.
    /// Riferimento: DS02-BL ConfrontiFasiFenologicheQDCA — Output.
    /// </summary>
    public sealed class ConfrontiFasiFenologicheQDCAResult
    {
        /// <summary>
        /// Fasi fenologiche non ancora presenti nel QDCA per questo impianto/progetto.
        /// Identificate tramite il codice BBCH come chiave univoca.
        /// Riferimento: DS02-BL — Regole di Business — Confronto per unicità.
        /// </summary>
        public IReadOnlyList<FaseFenologicaQDCAInput> FasiNuove { get; init; } = Array.Empty<FaseFenologicaQDCAInput>();

        /// <summary>
        /// Fasi fenologiche già presenti nel QDCA (duplicate evitate).
        /// Identificate tramite il codice BBCH come chiave univoca.
        /// Riferimento: DS02-BL — Regole di Business — Confronto per unicità.
        /// </summary>
        public IReadOnlyList<FaseFenologicaQDCAInput> FasiDuplicate { get; init; } = Array.Empty<FaseFenologicaQDCAInput>();

        /// <summary>
        /// Fasi fenologiche il cui codice BBCH non è riconosciuto nella tabella
        /// SpecieVegetaliXStadiCrescita per la specie richiesta. Queste fasi non
        /// possono essere salvate come <see cref="Attivita"/> e vengono escluse da
        /// <see cref="ListaAttivita"/>.
        /// Riferimento: DS02-BL — Regole di Business — Confronto per unicità.
        /// </summary>
        public IReadOnlyList<FaseFenologicaQDCAInput> FasiNonRiconosciute { get; init; } = Array.Empty<FaseFenologicaQDCAInput>();

        /// <summary>
        /// Data dell'operazione QDCA: MAX(data_fase) tra tutte le fasi ricevute (storiche e predittive).
        /// Riferimento: DS02-BL — Regole di Business — Data operazione QDCA.
        /// </summary>
        public DateTime DataOperazioneQDCA { get; init; }

        /// <summary>
        /// Oggetti Attivita pronti per essere passati a MapAttivitaToAgenda e ScriviAttivitaAgendaAsync
        /// (Framework.md — Procedura di Salvataggio Fasi Fenologiche, Step 2-3).
        /// Un oggetto Attivita per impianto, contenente le fasi nuove da registrare nel QDCA.
        /// <para>
        /// NOTA: <c>Attivita.risorse</c> (DettaglioRilievo per ogni fase) deve essere popolata
        /// dall'implementazione di <c>GetMovDettaglioRilievo</c> in <c>MovDettagliFactory</c>
        /// quando il pipeline di scrittura rilievi sarà completato.
        /// </para>
        /// Riferimento: DS02-BL — Output lista_attivita.
        /// </summary>
        public IReadOnlyList<Attivita> ListaAttivita { get; init; } = Array.Empty<Attivita>();
    }
}
