using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.anagrafiche;

namespace AgronicaCoreDTOStd.InData.Zoo
{
    public class LeggiSottogruppiStalla
    {
        public Impresa impresa { get; set; }
        public CentroAziendale centro { get; set; }
        public Fabbricato stalla { get; set; }
        public SottogruppoStalla raggruppamento { get; set; }
    }
}
