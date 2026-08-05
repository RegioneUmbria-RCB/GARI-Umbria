using System;

namespace InData.FoodMetaVerse
{
    /// <summary>
    /// Write DTO for Lookup_Sost_H20_Aziendale_Chiavi.
    /// See DS08-BL and Database schema section lookup_sost_h2o_aziendale_chiavi.
    /// </summary>
    public class WriteLookupSostH2OAziendaleChiavi
    {
        public int Id { get; set; }

        public string Id_Invocazione { get; set; }

        public DateTime Data_Calcolo { get; set; }

        public int Anno { get; set; }

        public string CuaaFiliera { get; set; }

        public string CuaaAzienda { get; set; }

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

        public WriteLookupSostH2OAziendaleChiavi()
        {
        }

        public WriteLookupSostH2OAziendaleChiavi(string idInvocazione, string filiera, string azienda, int anno)
        {
            Id_Invocazione = idInvocazione;
            CuaaFiliera = filiera;
            CuaaAzienda = azienda;
            Anno = anno;
            Data_Calcolo = DateTime.UtcNow;
        }
    }
}
