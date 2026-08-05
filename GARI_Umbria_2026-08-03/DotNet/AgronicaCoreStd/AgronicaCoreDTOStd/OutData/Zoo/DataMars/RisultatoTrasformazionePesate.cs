using System;
using System.Collections.Generic;

namespace OutData.Zoo.DataMars
{
    /// <summary>
    /// DTO di output dell'orchestratore di trasformazione pesate staging â†’ agenda.
    /// <para>Riferimento spec: DS02-BL TrasformazionePesateStagingAgenda â€” Output (Parametri_Output).</para>
    /// </summary>
    public sealed class RisultatoTrasformazionePesate
    {
        /// <summary>
        /// Numero di operazioni agenda create con successo.
        /// Ogni operazione corrisponde a un raggruppamento per data (tutte le pesate dello stesso giorno
        /// generano una sola operazione Agenda + Movimenti + N Movimenti_Dettagli).
        /// </summary>
        public int NumOperazioniCreate { get; set; }

        /// <summary>Numero di date scartate totalmente (nessuna pesata valida per quella data).</summary>
        public int NumOperazioniErrore { get; set; }

        /// <summary>Numero totale di pesate estratte dai payload JSON delle sessioni processate.</summary>
        public int NumPesateProcessate { get; set; }

        /// <summary>Numero di record STAGING_PESATE elaborati in questo batch.</summary>
        public int NumSessioniProcessate { get; set; }

        /// <summary>
        /// Numero di operazioni agenda create tramite accorpamento (coincide con NumOperazioniCreate:
        /// ogni data elaborata con successo genera esattamente 1 operazione accorpata).
        /// </summary>
        public int NumOperazioniAccorpate { get; set; }

        /// <summary>
        /// Mappa data â†’ numero di pesate valide accorpate in quella operazione.
        /// Chiave: data in formato "yyyy-MM-dd". Valore: conteggio animali pesati per quella data.
        /// </summary>
        public IReadOnlyDictionary<string, int> NumPesatePerOperazione { get; set; }
            = new Dictionary<string, int>();

        /// <summary>Timestamp UTC di esecuzione del batch.</summary>
        public DateTime DataEsecuzione { get; set; }

        /// <summary>Elenco dettagliato degli errori per singola pesata.</summary>
        public IReadOnlyList<ErroreDettaglioTrasformazione> ErroriDettagli { get; set; }
            = new List<ErroreDettaglioTrasformazione>();
    }
}
