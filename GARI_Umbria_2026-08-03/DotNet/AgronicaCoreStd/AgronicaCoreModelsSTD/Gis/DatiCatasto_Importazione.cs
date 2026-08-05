using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class Daticatasto_Importazione
    {
        public int ComportamentoImportazione { get; set; }
        public bool CreaLayerTestuale { get; set; }
        public string CodBelfiore { get; set; }
        public string Provincia { get; set; }
        public string Comune { get; set; }
        public string Sezione { get; set; }
        public string Foglio { get; set; }
        public string Particella { get; set; }
        public string Subalterno { get; set; }
        public string IdentificativoEsterno { get; set; }
        public string FiltroParticelleCatastali { get; set; }

        public string ListaLayersDXFAgenziaEntrate { get; set; }

        public TipoFileCatasto fileCatasto { get; set; }
    }

    public enum TipoFileCatasto
    {
        CatastoDXF = 1,
        CatastoSHP = 2
    }
}
