using System;

namespace InData.FoodMetaVerse
{
    /// <summary>
    /// Write DTO for a single row in Lookup_Sost_H20_Lotto.
    /// Corresponds to one combination (filiera, azienda, anno, appezzamento, varieta, esercizio, lotto_raccolta).
    /// See DS08-BL PersistenzaRisultatiPerColture — Database schema table lookup_sost_h2o_lotto.
    /// </summary>
    public class WriteLookupSostH2OLotto
    {
        /// <summary>Surrogate PK — NEWID() (UUID4) generato all'inserimento.</summary>
        public int Id { get; set; }

        /// <summary>Timestamp di calcolo — server time (GETDATE()).</summary>
        public DateTime DataCalcolo { get; set; }

        /// <summary>Identificativo della filiera (CUAA).</summary>
        public string CuaaFiliera { get; set; }

        /// <summary>Identificativo/PIVA dell'azienda (CUAA).</summary>
        public string CuaaAzienda { get; set; }

        /// <summary>Anno di riferimento del calcolo.</summary>
        public int Anno { get; set; }

        /// <summary>Identificativo dell'appezzamento.</summary>
        public int Appezzamento { get; set; }

        /// <summary>Codice cultivar GIAS (Cul_Cod). NOT NULL.</summary>
        public int CulCod { get; set; }

        /// <summary>Codice specie vegetale GIAS (Veg_Cod). NULL se non disponibile.</summary>
        public int? VegCod { get; set; }

        /// <summary>Identificativo dell'esercizio.</summary>
        public string Esercizio { get; set; }

        /// <summary>Codice del lotto di raccolta (opzionale).</summary>
        public string LottoRaccolta { get; set; }

        /// <summary>Codice nazione ISO Alpha-3 (es. "ITA").</summary>
        public string Nazione { get; set; }

        /// <summary>Codice regione (opzionale).</summary>
        public string Regione { get; set; }

        /// <summary>Quantità raccolta in KG.</summary>
        public decimal QuantitaRaccoltaKg { get; set; }

        /// <summary>Superficie di riferimento in ettari (ha).</summary>
        public decimal SuperficieRiferimento { get; set; }

        /// <summary>Payload JSON assoluto : {fabbisognoM3PerHa, consumataM3PerHa, daMeteoM3PerHa, deltaM3PerHa}.</summary>
        public string PayloadJson { get; set; }

        /// <summary>Payload firmato (BLOB, opzionale).</summary>
        public byte[] JsonFirmato { get; set; }

        /// <summary>Flag invio token: 0 = non inviato, 1 = inviato</summary>
        public short? Inviato { get; set; }

        /// <summary>Data/ora di invio del token</summary>
        public DateTime? DataInvio { get; set; }

        /// <summary>Data/ora di creazione del record, UTC</summary>
        public DateTime Data_Creazione { get; set; }

        /// <summary>Data/ora di ultima modifica del record, UTC</summary>
        public DateTime Data_Modifica { get; set; }

        /// <summary>Username che ha creato il record</summary>
        public string Username_Creazione { get; set; }

        /// <summary>Username che ha effettuato l'ultima modifica</summary>
        public string Username_Modifica { get; set; }

        /// <summary>Data inizio validità del record</summary>
        public DateTime Validita_Inizio { get; set; }

        /// <summary>Data fine validità del record</summary>
        public DateTime Validita_Fine { get; set; }
    }
}
