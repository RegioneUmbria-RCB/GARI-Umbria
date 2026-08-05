using System;
namespace InData.Zoo.DataMars
{
    /// <summary>
    /// DTO per l'inserimento di un record in STAGING_SESSIONI_API.
    /// Contiene i metadati di riepilogo (conteggi, stato) per ogni sessione di acquisizione.
    /// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI â€” Persistenze Coinvolte, STAGING_SESSIONI_API.</para>
    /// </summary>
    public sealed class StagingSessioneApiRecord
    {
        /// <summary>Identificativo univoco della sessionIntegration (FK logica verso STAGING_PESATE.id_sessione).</summary>
        public string IdSessione { get; set; } = string.Empty;

        /// <summary>Timestamp di inizio della sessione di acquisizione.</summary>
        public DateTime TimestampInizio { get; set; }

        /// <summary>Timestamp di fine della sessione di acquisizione.</summary>
        public DateTime TimestampFine { get; set; }

        /// <summary>Numero totale di pesate ricevute dall'API per questa sessione.</summary>
        public int RecordRicevuti { get; set; }

        /// <summary>Numero di pesate che hanno superato la validazione.</summary>
        public int RecordValidati { get; set; }

        /// <summary>Numero di pesate con errore di validazione.</summary>
        public int RecordErrori { get; set; }

        /// <summary>Numero di pesate effettivamente persistite in STAGING_PESATE.</summary>
        public int RecordPersistiti { get; set; }

        /// <summary>
        /// Stato complessivo della sessione.
        /// Valori attesi: <c>IN_CORSO</c>, <c>COMPLETATA</c>, <c>ERRORE</c>.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Descrizione dell'errore se la sessione Ã¨ terminata in errore; null altrimenti.</summary>
        public string ErroreDescrizione { get; set; }

        /// <summary>Numero di tentativi HTTP effettuati verso l'API Datamars per questa sessione.</summary>
        public int NumeroTentativi { get; set; }
    }
}
