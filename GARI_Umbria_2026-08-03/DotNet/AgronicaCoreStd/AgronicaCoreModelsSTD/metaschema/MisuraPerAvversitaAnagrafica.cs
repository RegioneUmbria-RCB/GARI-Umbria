using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class ListaMisurePerAvversitaAnagrafica
    {
        public List<MisuraPerAvversitaAnagrafica> ListaMisure { get; set; }
    }

    public class ListaMisurePerAvversitaAnagraficaExtended
    {
        public List<MisuraPerAvversitaAnagraficaExtended> ListaMisure { get; set; }
    }

    public class MisuraPerAvversitaAnagrafica
    {
        public int CodiceMisura { get; set; }
        public int CodiceAnagrafica { get; set; }
        public string Descrizione { get; set; }
        public int valoreAnagrafica { get; set; }
        public int DPI_FlagPrivatoPubblico { get; set; }
        public int DPI_COD { get; set; }
        public bool modificabile { get; set; }
        public bool cancellabile { get; set; }
    }

    public class MisuraPerAvversitaAnagraficaExtended : MisuraPerAvversitaAnagrafica
    {
        public int Veg_Cod { get; set; }
        public string Veg_Des { get; set; }
        public string DPI_Des { get; set; }
        public int IdRcdpi { get; set; }
        public int Av_Cod { get; set; }
        public string DescrizioneAvversita { get; set; }
    }
}
