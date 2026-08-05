using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.valutazioni
{
    public class Valutazione_Piano_Conti
    {
        public PK primaryKey { get; set; }

        public string Valutazione_Piano_Des { get; set; }

        public bool flag_cancellazione { get; set; }


        public Valutazione_Piano_Conti()
        {
            this.flag_cancellazione = false;
            

        }

        public Valutazione_Piano_Conti(PK primaryKey)
        {
            this.primaryKey = primaryKey;
            this.flag_cancellazione = false;
        }

        public class PK
        {
            public string Piva { get; set; }
            public int Valutazione_Piano_Cod { get; set; }

            public PK(string piva, int codice)
            {
                this.Piva = piva;
                this.Valutazione_Piano_Cod = codice;
            }

            public PK()
            {
                this.Piva = "";
                this.Valutazione_Piano_Cod = 0;
            }
        }
    }

}