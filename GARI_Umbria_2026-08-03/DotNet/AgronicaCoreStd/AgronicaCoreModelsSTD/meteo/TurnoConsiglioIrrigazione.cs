using System;

namespace AgronicaCoreModelsSTD.meteo
{
    public class TurnoConsiglioIrrigazione
    {
        public int Id_Irrig { get; set; }
        public int Id_Irrig_Turno { get; set; }
        public DateTime Data_Turno { get; set; }
        public decimal Qta_Acqua { get; set; }
        public string Udm_Des { get; set; }
    }
}
