using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    public class ScriviCatasto
    {
        public CatastoCentroAziendale oldValue;

        public CatastoCentroAziendale newValue;

        public ScriviCatasto(CatastoCentroAziendale oldValue, CatastoCentroAziendale newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }
}
