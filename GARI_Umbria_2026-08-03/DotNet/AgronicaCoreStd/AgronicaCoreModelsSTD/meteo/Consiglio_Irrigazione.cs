using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema;
using System;

namespace AgronicaCoreModelsSTD.meteo
{
    public class Consiglio_Irrigazione: BaseCodeDescr
    {
        public DateTime dataEsecuzione { get; set; }

        public DateTime dataConsiglio { get; set; }

        public DateTime? minDataTurno { get; set; }
        public DateTime? maxDataTurno { get; set; }

        public decimal qtaAcqua { get; set; }

        public UnitaDiMisura unitaDiMisura { get; set; }

        public Provider_DSS_Irrigazione providerConsiglio { get; set; }

        public Modello_DSS_Irrigazione modelloConsiglio { get; set; }

        public Consiglio_Irrigazione(int codice, string descrizione) : base(codice, descrizione)
        {
        }

        public enum Modello_DSS_Irrigazione
        {
            Irriframe = 1
        }

        public enum Provider_DSS_Irrigazione
        {
            GIAS_Irriframe = 1
        }
    }
}
