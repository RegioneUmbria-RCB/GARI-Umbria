using System;

namespace InData.FoodMetaVerse
{
    public class GetLookupRischioMeteo
    {
        /// <summary>CUAA Filiera. Null = nessun filtro.</summary>
        public string Cuaa_Filiera { get; set; }

        /// <summary>CUAA Azienda. Null = nessun filtro.</summary>
        public string Cuaa_Azienda { get; set; }

        /// <summary>Identificativo appezzamento. Null = nessun filtro.</summary>
        public int? Id_Appezzamento { get; set; }

        /// <summary>Identificativo esercizio. Null = nessun filtro.</summary>
        public int? Id_Esercizio { get; set; }

        /// <summary>Anno campagna agraria. Null = nessun filtro.</summary>
        public int? Anno_Esercizio { get; set; }

        /// <summary>Codice specie vegetale. Null = nessun filtro.</summary>
        public int? Cod_Specie { get; set; }

        /// <summary>Codice varieta colturale. Null = nessun filtro.</summary>
        public int? Cod_Varieta { get; set; }

        /// <summary>Codice ISO nazione. Null = nessun filtro.</summary>
        public string Nazione { get; set; }

        /// <summary>Regione. Null = nessun filtro.</summary>
        public string Regione { get; set; }

        /// <summary>Filtro per flag invio: 0 = non inviato, 1 = inviato. Null = nessun filtro.</summary>
        public short? Inviato { get; set; }

        /// <summary>Filtro data invocazione da (inclusa). Null = nessun filtro.</summary>
        public DateTime? Data_Invocazione_Da { get; set; }

        /// <summary>Filtro data invocazione a (inclusa). Null = nessun filtro.</summary>
        public DateTime? Data_Invocazione_A { get; set; }
    }
}