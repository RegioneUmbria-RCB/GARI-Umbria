using System;

namespace InData.FoodMetaVerse
{
    public class WriteLookupSostCO2ColtureChiavi
    {
        /// <summary>Chiave primaria surrogata (identity)</summary>
        public int Id { get; set; }

        /// <summary>Identificativo univoco dell'invocazione M4</summary>
        public string Id_Invocazione { get; set; }

        /// <summary>Data/ora dell'invocazione M4, UTC</summary>
        public DateTime Data_Invocazione { get; set; }

        /// <summary>Anno di riferimento della campagna colturale, es. 2025</summary>
        public int Anno { get; set; }

        /// <summary>Identificativo Filiera agroalimentare (PIVA filiera)</summary>
        public string Piva_Filiera { get; set; }

        /// <summary>ID Azienda (PIVA), da Gerarchia Imprese</summary>
        public string Piva_Azienda { get; set; }

        /// <summary>ID Appezzamento (PIVA|Sa_Cod|Appezza)</summary>
        public int Appezzamento { get; set; }

        /// <summary>Codice Nazione ISO 3166-1 alpha-2, es. IT</summary>
        public string Nazione { get; set; }

        /// <summary>Codice ISTAT regione geografica (max 5 caratteri), es. 01. Opzionale.</summary>
        public string Regione { get; set; }

        /// <summary>Array JSON di ID Esercizi (cicli colturali) inclusi nell'invocazione, es. ["E32","E33"]</summary>
        public int Progetto_Cod { get; set; }

        /// <summary>Codice specie colturale selezionata</summary>
        public int Veg_Cod { get; set; }

        /// <summary>Codice varietà della specie. Opzionale.</summary>
        public int? Cul_Cod { get; set; }

        /// <summary>Codice elemento GIS (GIS_ElementiGrafici)</summary>
        public int? Elem_Cod { get; set; }

        /// <summary>Codice FMP del prodotto raccolto. Opzionale.</summary>
        public int? Mat_Cod { get; set; }

        /// <summary>Codice FMP del lotto raccolto. Opzionale.</summary>
        public string Lotto { get; set; }

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

        public WriteLookupSostCO2ColtureChiavi() { }

        public WriteLookupSostCO2ColtureChiavi(string idInvocazione, string pivaFiliera, string pivaAzienda, int anno, int appezzamento, string nazione)
        {
            Id_Invocazione = idInvocazione;
            Piva_Filiera = pivaFiliera;
            Piva_Azienda = pivaAzienda;
            Anno = anno;
            Appezzamento = appezzamento;
            Nazione = nazione;
        }
    }
}
