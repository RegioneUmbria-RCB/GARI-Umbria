using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiCentriAziendali
    {
        public Impresa impresa { get; set; }

        public UtilizzoTerreno utilizzoTerreno { get; set; }

        public DateTime data { get; set; }

        public bool? filtra_validita_esercizi { get; set; }
    }
}
