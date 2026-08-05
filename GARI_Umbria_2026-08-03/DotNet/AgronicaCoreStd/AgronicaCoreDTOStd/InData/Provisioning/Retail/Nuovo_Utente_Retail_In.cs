using System;

namespace AgronicaCoreDTOStd.InData.Provisioning.Retail
{
    public class Nuovo_Utente_Retail_In
    {
        public Dati_Utente_Retail Utente { get; set; }

        public Dati_Transazione_Commerciale transazioneCommerciale { get; set; }

        public bool ForzaRegistrazione { get; set; }

        public string CodiceISOLingua { get; set; }

        public DateTime dataFineValiditaUtente { get; set; }

    }

    public class Dati_Utente_Retail
    {
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Password { get; set; }
        public string Ragione_Sociale_Azienda { get; set; }
        public string PIVA_Azienda { get; set; }
        public string CF_Azienda { get; set; }
        public string CUAA { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }

    }
}
