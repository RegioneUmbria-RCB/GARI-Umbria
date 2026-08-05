using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita.dettagli
{
    public class Opzioni_Raccolta
    {
        public enum_Ripartizione_Raccolta Ripartizione { get; set; }

        public enum_Modalita_Raccolta Modalita { get; set; }

        // public AgronicaCoreDataProvider.TipiEnumerativi.enum_Opzioni_Raccolta_Aggiornamento_Anagrafica Chiusura { get; set; }
        public Boolean CarichiMagazzinoAttivi { get; set; }

        public enum_Generazione_Lotto_Raccolta GenerazioneLotto { get; set; }


        public enum enum_Ripartizione_Raccolta
        {
            AUTO_SUPERFICIE,
            AUTO_PIANTE,
            MANUALE
        }

        public enum enum_Modalita_Raccolta
        {
            MECCANICA = 0,
            MANUALE = 1
        }

        public enum enum_Generazione_Lotto_Raccolta
        {
            MANUALE,
            DA_DATA,
            UNIVOCO,
            DA_ESERCIZO
        }

    }
}
