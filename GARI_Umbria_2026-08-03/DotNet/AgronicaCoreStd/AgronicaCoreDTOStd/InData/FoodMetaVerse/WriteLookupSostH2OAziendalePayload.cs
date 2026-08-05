using System;

namespace InData.FoodMetaVerse
{
    /// <summary>
    /// Write DTO for Lookup_Sost_H20_Aziendale_Payload.
    /// See DS08-BL and Database schema section lookup_sost_h2o_aziendale_payload.
    /// </summary>
    public class WriteLookupSostH2OAziendalePayload
    {
        public string Id_Invocazione { get; set; }

        public string Payload_Json { get; set; }

        public byte[] Json_Firmato { get; set; }

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

        public WriteLookupSostH2OAziendalePayload()
        {
        }

        public WriteLookupSostH2OAziendalePayload(string idInvocazione, string payloadJson, byte[] jsonFirmato, string utente)
        {
            Id_Invocazione = idInvocazione;
            Payload_Json = payloadJson;
            Json_Firmato = jsonFirmato;
            Username_Creazione = utente;
            Username_Modifica = utente;
        }
    }
}
