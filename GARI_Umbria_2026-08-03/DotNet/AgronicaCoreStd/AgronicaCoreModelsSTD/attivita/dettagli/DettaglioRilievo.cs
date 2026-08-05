using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.avversita;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita.dettagli
{
    public class DettaglioRilievo : Risorsa
    {
        public FaseFenologica faseFenologica { get; set; }
        public int IndiceMaturita { get; set; }
        public int DannoRaccolta { get; set; }
        public int IndiceResa { get; set; }
        public AvversitaGruppo avversitaGruppo { get; set; }
        public AvversitaGruppo erbaInfestante { get; set; }
        public UnitaDiMisura unitaDiMisura { get; set; }
        public DateTime DataOraRilievo { get; set; }
        public decimal QtaRilevata { get; set; }
        public EsercizioRilievoCDC esercizioCDC { get; set; }

        //Utilizzato nel Rilievo Avversità Trappole
        public RisorsaProdotto risorsaProdotto { get; set; }

        /**
         * Descrizione pura dell'impianto
         * TODO: verificarne l'impiego
         */
        public string Impianto { get; set; }
        public string Descrizione { get; set; }
        public string QtaRilevataString { get; set; }
        public string Note { get; set; }

        public DettaglioRilievo()
        {
            classType = costanti.ClassType.DettaglioRilievo;
        }

    }

}
