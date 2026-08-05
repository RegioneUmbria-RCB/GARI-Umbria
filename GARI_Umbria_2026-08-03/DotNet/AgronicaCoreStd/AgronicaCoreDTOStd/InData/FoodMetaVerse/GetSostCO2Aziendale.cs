namespace InData.FoodMetaVerse
{
    public class GetSostCO2Aziendale
    {
        /// <summary>Chiave primaria surrogata (valore generato applicativamente). Null = nessun filtro.</summary>
        public int? Id { get; set; }

        /// <summary>Identificativo univoco dell'invocazione M4. Null = nessun filtro.</summary>
        public string IdInvocazione { get; set; }

        /// <summary>Anno di riferimento della campagna, es. 2025</summary>
        public int Anno { get; set; }

        /// <summary>Identificativo Filiera agroalimentare</summary>
        public string Filiera { get; set; }

        /// <summary>ID Azienda (PIVA), da Gerarchia Imprese</summary>
        public string Azienda { get; set; }

        /// <summary>Filtro per flag invio: 0 = non inviato, 1 = inviato. Null = nessun filtro.</summary>
        public short? Inviato { get; set; }
    }
}
