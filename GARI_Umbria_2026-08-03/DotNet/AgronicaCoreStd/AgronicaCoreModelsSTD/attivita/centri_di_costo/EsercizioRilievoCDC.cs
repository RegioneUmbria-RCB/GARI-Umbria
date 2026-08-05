using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita.centri_di_costo
{
    public class EsercizioRilievoCDC : EsercizioCDC
    {

        ///// <summary>
        ///// quantità rilevata
        ///// </summary>
        // public double quantita { get; set; }

        /// <summary>
        /// data ed ora del rilievo
        /// </summary>
        public DateTime dataRiferimento { get; set; }

        public EsercizioRilievoCDC()
        {
            classType = costanti.ClassType.EsercizioRilievoCDC;
            tipo = Tipo.Esercizio;
        }

    }
}
