using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class Rubrica
    {

        public int codice { get; set; }
        public string tipologia { get; set; }
        public bool flag_cancellazione { get; set; }

        public Rubrica(int codice)
        {
            this.codice = codice;
            this.flag_cancellazione = false;
        }

        public Rubrica()
        {

        }
    }
}
