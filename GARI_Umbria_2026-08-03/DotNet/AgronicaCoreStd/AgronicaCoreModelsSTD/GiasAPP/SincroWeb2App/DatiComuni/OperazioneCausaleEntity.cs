using System;

namespace AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni
{
    public class OperazioneCausaleEntity
    {
        public int id { get; set; }
        public string causale { get; set; }
        public int lavCod { get; set; }
        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }
    }
}
