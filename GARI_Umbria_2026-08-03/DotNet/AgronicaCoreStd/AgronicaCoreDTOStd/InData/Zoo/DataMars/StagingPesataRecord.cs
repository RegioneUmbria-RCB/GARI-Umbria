using System;
namespace InData.Zoo.DataMars
{
    /// <summary>
    /// DTO per l'inserimento di un record in STAGING_PESATE.
    /// Ogni record rappresenta il payload JSON completo di una singola sessionIntegration
    /// acquisita dall'API Datamars.
    /// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI â€” Persistenze Coinvolte, STAGING_PESATE.</para>
    /// </summary>
    public sealed class StagingPesataRecord
    {
        /// <summary>Identificativo univoco della sessionIntegration ricevuto dall'API Datamars.</summary>
        public string IdSessione { get; set; } = string.Empty;

        /// <summary>Timestamp di inizio acquisizione della sessione (UTC).</summary>
        public DateTime TimestampInizioSessione { get; set; }

        /// <summary>Payload JSON completo della sessionIntegration, inclusi tutti gli animali pesati e i metadati.</summary>
        public string PayloadJson { get; set; } = string.Empty;

        /// <summary>
        /// Esito di validazione della sessione.
        /// Valori attesi: <c>VALIDA</c>, <c>ERRORI_PARZIALI</c>, <c>ERRORE_TOTALE</c>.
        /// </summary>
        public string FlagValidazione { get; set; } = string.Empty;

        /// <summary>Descrizione dell'errore in caso di validazione fallita; null se sessione valida.</summary>
        public string ErroreDescrizione { get; set; }

        /// <summary>Identificativo del farm Datamars associato a questa sessione.</summary>
        public string IdFarm { get; set; } = string.Empty;
    }
}
