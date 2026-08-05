using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class RubricaVoci
    {

        public Rubrica rubrica { get; set; }

        public string valore { get; set; }
        public bool flag_cancellazione { get; set; }

        public RubricaVoci()
        {
            this.flag_cancellazione = false;
        }
    }
}
