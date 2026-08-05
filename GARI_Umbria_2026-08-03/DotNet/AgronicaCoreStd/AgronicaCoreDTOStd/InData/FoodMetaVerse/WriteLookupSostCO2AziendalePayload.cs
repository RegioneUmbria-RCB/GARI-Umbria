using System;

namespace InData.FoodMetaVerse
{
    public class WriteLookupSostCO2AziendalePayload
    {
        /// <summary>FK a Lookup_Sost_CO2_Aziendale_Chiavi.id_invocazione (anche PK di questa tabella)</summary>
        public string Id_Invocazione { get; set; }

        /// <summary>Payload JSON della richiesta inviata a M4 (Request Body)</summary>
        public string Json_Richiesta { get; set; }

        /// <summary>Payload JSON della risposta ricevuta da M4 (Response Body)</summary>
        public string Json_Risposta { get; set; }

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

        public WriteLookupSostCO2AziendalePayload() { }

        public WriteLookupSostCO2AziendalePayload(string idInvocazione, string jsonRichiesta, string jsonRisposta)
        {
            Id_Invocazione = idInvocazione;
            Json_Richiesta = jsonRichiesta;
            Json_Risposta = jsonRisposta;
        }
    }
}
