namespace InData.FoodMetaVerse
{
    /// <summary>
    /// Query filter for reading H2O sustainability lookups in aziendale mode.
    /// See DS09-API Endpoint GET /sostenibilita_h2o/per_azienda_annuale.
    /// </summary>
    public class GetSostH2OAziendale
    {
        /// <summary>Identificativo invocazione. Null o vuoto = nessun filtro.</summary>
        public string IdInvocazione { get; set; }

        /// <summary>Identificativo filiera richiesto dall'endpoint.</summary>
        public string Filiera { get; set; }

        /// <summary>Identificativo azienda opzionale.</summary>
        public string Azienda { get; set; }

        /// <summary>Anno di riferimento. 0 = nessun filtro.</summary>
        public int Anno { get; set; }
    }
}
