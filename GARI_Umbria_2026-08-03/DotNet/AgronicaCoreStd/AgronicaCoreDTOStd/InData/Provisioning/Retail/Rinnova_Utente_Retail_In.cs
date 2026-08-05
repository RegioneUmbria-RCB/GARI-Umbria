using System;

namespace AgronicaCoreDTOStd.InData.Provisioning.Retail
{
    public class Rinnova_Utente_Retail_In
    {
        public string EmailUtente { get; set; }
        public string PasswordUtente { get; set; }

        public Dati_Transazione_Commerciale transazioneCommerciale { get; set; }

        public string CodiceISOLingua { get; set; }

        public DateTime dataFineValiditaUtente { get; set; }
    }
}
