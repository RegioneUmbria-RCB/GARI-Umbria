using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class CentroAziendaleLight
    {
        public CentroAziendale.PK primaryKey { get; set; }
        public string nome { get; set; }

        public CentroAziendaleLight(CentroAziendale.PK primaryKey)
        {
            this.primaryKey = primaryKey;
        }
    }
}
