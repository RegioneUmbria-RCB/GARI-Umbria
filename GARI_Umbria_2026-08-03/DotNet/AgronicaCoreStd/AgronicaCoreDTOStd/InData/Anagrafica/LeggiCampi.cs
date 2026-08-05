using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiCampi
    {
        public CentroAziendale centro { get; set; }

        public UtilizzoTerreno utilizzoTerreno { get; set; }

        public DateTime data { get; set; }
    }
}
