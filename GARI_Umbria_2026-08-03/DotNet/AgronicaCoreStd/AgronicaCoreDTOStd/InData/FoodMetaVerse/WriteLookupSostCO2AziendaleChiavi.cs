using System;

namespace InData.FoodMetaVerse
{
    public class WriteLookupSostCO2AziendaleChiavi
    {
        /// <summary>Chiave primaria surrogata (valore generato applicativamente)</summary>
        public int Id { get; set; }

        /// <summary>Identificativo univoco dell'invocazione M4</summary>
        public string Id_Invocazione { get; set; }

        /// <summary>Data/ora dell'invocazione M4, UTC</summary>
        public DateTime Data_Invocazione { get; set; }

        /// <summary>Anno di riferimento della campagna, es. 2025</summary>
        public int Anno { get; set; }

        /// <summary>Identificativo Filiera agroalimentare</summary>
        public string Filiera { get; set; }

        /// <summary>ID Azienda (PIVA), da Gerarchia Imprese</summary>
        public string Azienda { get; set; }

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

        public WriteLookupSostCO2AziendaleChiavi() { }

        public WriteLookupSostCO2AziendaleChiavi(string idInvocazione, string azienda, string filiera, int anno)
        {
            Id_Invocazione = idInvocazione;
            Azienda = azienda;
            Filiera = filiera;
            Anno = anno;
        }
    }
}
