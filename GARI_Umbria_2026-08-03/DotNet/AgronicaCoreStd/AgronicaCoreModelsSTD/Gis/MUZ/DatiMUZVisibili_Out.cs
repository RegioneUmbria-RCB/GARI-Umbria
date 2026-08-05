using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis.MUZ
{
    public class DatiMUZVisibili_Out
    {
        public int Area_Cod { get; set; }
        public List<MUZ_Appezzamento> MUZ_Appezzamenti { get; set; }
        public GruppoAreaOmogenea Gruppo { get; set; }

    }

    public class MUZ_Appezzamento
    {
        public int Sa_Cod { get; set; }
        public string Piva { get; set; }
        public int Appezza { get; set; }
    }

    public class GruppoAreaOmogenea
    {
        public string Piva { get; set; }
        public int Gruppo_Area_Cod { get; set; }
        public string Gruppo_Area_Des { get; set; }
    }
}
