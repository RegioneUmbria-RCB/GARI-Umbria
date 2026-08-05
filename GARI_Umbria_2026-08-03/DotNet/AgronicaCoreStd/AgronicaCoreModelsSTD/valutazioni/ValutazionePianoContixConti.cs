using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.valutazioni
{
    public class Valutazione_Piano_ContixConti
    {
        public PK primaryKey { get; set; }        

        public bool flag_cancellazione { get; set; }
        public int ordine_pc { get; set; }


        public Valutazione_Piano_ContixConti()
        {
            this.flag_cancellazione = false;
            this.ordine_pc = 0;
        }

        public Valutazione_Piano_ContixConti(PK primaryKey)
        {
            this.primaryKey = primaryKey;
            this.flag_cancellazione = false;
            this.ordine_pc = 0;
        }

        public class PK
        {
            public string Piva { get; set; }
            public int Valutazione_Piano_Cod { get; set; }
            public int Valutazione_Conto_Cod { get; set; }

            public PK(string piva, int pianoCod, int contoCod)
            {
                this.Piva = piva;
                this.Valutazione_Piano_Cod = pianoCod;
                this.Valutazione_Conto_Cod = contoCod;
            }

            public PK()
            {
                this.Piva = "";
                this.Valutazione_Piano_Cod = 0;
                this.Valutazione_Conto_Cod = 0;
            }
        }
    }

}