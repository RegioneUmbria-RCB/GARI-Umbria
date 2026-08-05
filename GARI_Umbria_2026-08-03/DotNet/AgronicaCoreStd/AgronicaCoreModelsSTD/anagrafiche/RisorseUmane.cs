using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class RisorseUmane
    {

        /// <summary>
        /// Cod_Risum su database
        /// </summary>
        public int codice { get; set; }

        public IntervalloTemporale validita { get; set; }

        public string settore { get; set; }
        public string attivita { get; set; }

        public Contatto contatto { get; set; }

        public RapportoContabile rapportoContabile { get; set; }

        public bool flag_cancellazione { get; set; }

        public RisorseUmane(int codice)
        {
            this.codice = codice;
            this.flag_cancellazione = false;
        }

        public RisorseUmane()
        {
            this.flag_cancellazione = false;
        }

    }
}
