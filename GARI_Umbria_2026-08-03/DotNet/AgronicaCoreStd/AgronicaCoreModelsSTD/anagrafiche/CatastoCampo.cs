using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class CatastoCampo
    {

        public double area { get; set; }

        public ParticelleCatastali particella { get; set; }

        public bool flag_cancellazione { get; set; }

        public CatastoCampo()
        {
            flag_cancellazione = false;
        }
    }
}
