using System;

namespace AgronicaCoreDTOStd.OutData.Gis.Impianti
{
    /// <summary>
    /// Dati dell'impianto dell'appezzamento per un singolo anno agronomico.
    /// </summary>
    public class ImpiantoAnno_Out
    {
        public string piva { get; set; }
        public int sa_cod { get; set; }
        public int appezza { get; set; }
        public int id_imp { get; set; }
        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }
        public int cul_cod { get; set; }
        public int veg_cod { get; set; }
    }
}
