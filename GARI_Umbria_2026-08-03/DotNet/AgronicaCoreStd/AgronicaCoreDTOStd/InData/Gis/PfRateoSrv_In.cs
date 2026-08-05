using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.Gis;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class PfRateoSrv_In
    {
        public ChiaveAlbero ChiaveAlbero { get; set; }
        public string DescrizionePiano { get; set; }
        public string CellSize { get; set; }
        public DateTime DataRiferimento_LetturaDatiSentinel { get; set; }
        public Lista_DatiPrecision_XmlAllegati_Out oCfgLetturaPF { get; set; }
        public string psw_SuperUser { get; set; }
    }
}
