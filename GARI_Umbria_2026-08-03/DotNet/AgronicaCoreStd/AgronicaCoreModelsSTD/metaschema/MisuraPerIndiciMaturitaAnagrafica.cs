using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class ListaMisuraPerIndiciMaturitaAnagrafica
    {
        public List<MisuraPerIndiciMaturitaAnagrafica> ListaMisure { get; set; }
    }

    public class MisuraPerIndiciMaturitaAnagrafica
    {
        public int Ind_Mat_Cod { get; set; }
        public string Ind_Mat_Des { get; set; }
        public string CodiceCompostoMisura { get; set; }
        public string DescrizioneMisura { get; set; }
        public int Udm_Cod { get; set; }
        public string Udm_Des { get; set; }
        public int Anag_Cod { get; set; }
        public string Anag_Des { get; set; }
        public int Anag_Valore { get; set; }
        public bool Cancellabile { get; set; }
        public bool Modificabile { get; set; }
    }
}
