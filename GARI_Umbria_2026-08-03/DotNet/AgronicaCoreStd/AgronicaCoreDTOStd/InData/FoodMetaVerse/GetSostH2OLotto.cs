namespace InData.FoodMetaVerse
{
    /// <summary>
    /// Query filter for reading H2O sustainability lookup rows in lot/per-product mode.
    /// See DS10-API Endpoint GET /sostenibilita_h2o/per_prodotto_di_filiera.
    /// </summary>
    public class GetSostH2OLotto
    {
        /// <summary>Identificativo filiera richiesto dall'endpoint.</summary>
        public string Filiera { get; set; } = string.Empty;

        /// <summary>Identificativo azienda opzionale.</summary>
        public string Azienda { get; set; } = string.Empty;

        /// <summary>
        /// Codice prodotto FMP (segmento primario).
        /// Mappato su colonna <c>varieta</c> della lookup table.
        /// </summary>
        public string CodProdottoFmp { get; set; } = string.Empty;
    }
}