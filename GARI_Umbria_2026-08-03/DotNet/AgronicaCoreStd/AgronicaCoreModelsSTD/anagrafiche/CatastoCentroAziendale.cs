using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class CatastoCentroAziendale
    {
        public CentroAziendale.PK centro { get; set; }
        public ParticelleCatastali particella { get; set; }
        public List<PossessoParticella> possessiParticella { get; set; }
        public bool flag_cancellazione { get; set; }
        public CatastoCentroAziendale()
        {
            flag_cancellazione = false;
        }
    }
}
