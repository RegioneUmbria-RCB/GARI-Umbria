using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Zoo
{
    public class LeggiPianiCampionamento
    {
        public RifImpresa impresa { get; set; }
        public RifFabbricato stalla { get; set; }
        public DateTime data { get; set; }        
    }
}
