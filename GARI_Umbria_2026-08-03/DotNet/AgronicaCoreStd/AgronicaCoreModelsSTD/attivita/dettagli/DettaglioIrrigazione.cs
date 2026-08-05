using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.meteo;
using System;

namespace AgronicaCoreModelsSTD.attivita.dettagli
{
    public class DettaglioIrrigazione : Risorsa
    {
        public decimal QtaRilevata { get; set; }

        public decimal QtaTotale { get; set; }
        public UnitaDiMisura unitaDiMisura { get; set; }
        public decimal Ore { get; set; }

        public int Portata { get; set; }
        public decimal Efficienza { get; set; }
        public DateTime DataInizio { get; set; }

        public DateTime DataFine { get; set; }

        public int Frequenza { get; set; }

        public TipoIrrigazione tipoIrrigazione { get; set; }

        public EsercizioCDC esercizioCDC { get; set; }

        public Consiglio_Irrigazione consiglioIrrigazione { get; set; }

        public ParcoMacchine macchina { get; set; }

        public DettaglioIrrigazione()
        {
            classType = costanti.ClassType.DettaglioIrrigazione;
        }

    }

}
