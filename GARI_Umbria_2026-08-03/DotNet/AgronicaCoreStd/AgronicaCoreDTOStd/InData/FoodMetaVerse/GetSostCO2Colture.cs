namespace InData.FoodMetaVerse
{
    public class GetSostCO2Colture
    {
        /// <summary>Chiave primaria surrogata (identity). Null = nessun filtro.</summary>
        public int? Id { get; set; }

        /// <summary>Identificativo univoco dell'invocazione M4. Null = nessun filtro.</summary>
        public string IdInvocazione { get; set; }

        /// <summary>Anno di riferimento della campagna colturale, es. 2025</summary>
        public int Anno { get; set; }

        /// <summary>Identificativo Filiera agroalimentare (PIVA filiera)</summary>
        public string PivaFiliera { get; set; }

        /// <summary>ID Azienda (PIVA), da Gerarchia Imprese</summary>
        public string PivaAzienda { get; set; }

        /// <summary>ID Appezzamento. Null = nessun filtro.</summary>
        public int? Appezzamento { get; set; }

        /// <summary>Codice FMP del prodotto raccolto. Null = nessun filtro.</summary>
        public int? MatCod { get; set; }

        /// <summary>Codice FMP del lotto raccolto. Null = nessun filtro.</summary>
        public string Lotto { get; set; }

        /// <summary>Filtro per flag invio: 0 = non inviato, 1 = inviato. Null = nessun filtro.</summary>
        public short? Inviato { get; set; }
    }
}
