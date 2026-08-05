using System;
using System.Collections.Generic;

namespace OutData.Engine.MeteoSuite
{
    public class MeteoDataPuntualeSensore
    {
        public string Sensore { get; set; }
        public float Valore { get; set; }
    }

    public class MeteoDataPuntuale
    {
        public DateTime DataOra { get; set; } = new DateTime();
        public List<MeteoDataPuntualeSensore> ValoriSensori { get; set; }
    }

    public class MeteoDataSensore
    {
        public int SensoreId { get; set; }
        public string Sensore { get; set; }
        public string Etichetta { get; set; }
        public string TipoSensore { get; set; }
        public string UM { get; set; }
        public string FunAggreg { get; set; }
        public string ProviderReference { get; set; }
    }

    public class MeteoDataRiepilogoDato
    {
        public string Sensore { get; set; }
        public string FunAggreg { get; set; }
        public float Valore { get; set; }
        public string UM { get; set; }
    }

    public class MeteoDataRiepilogoDatoSensore
    {
        public string Sensore { get; set; }
        public string Etichetta { get; set; } = "";
        public string UM { get; set; } = "";
        public DateTime DataOra { get; set; } = new DateTime();
        public float? Val_Last { get; set; }
        public float? Val_Avg { get; set; }
        public float? Val_Min { get; set; }
        public float? Val_Max { get; set; }
        public float? Val_Sum { get; set; }
        public float? Val_Dir { get; set; }
    }
    
    public class AcquisizioneDatiMeteoResponse
    {
        public string Stazione {  get; set; }
        public DateTime UltimoAggiornamento { get; set; }
        public List<MeteoDataPuntuale> Dati {  get; set; }
        public List<MeteoDataSensore> Sensori { get; set; }
        public List<MeteoDataRiepilogoDato> Riepilogo { get; set; }
        public List<MeteoDataRiepilogoDatoSensore> RiepilogoSensori { get; set; }
    }
}
