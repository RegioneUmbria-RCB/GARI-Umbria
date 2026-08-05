



using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.valutazioni
{
    public class Valutazione_Conto
    {
        public PK primaryKey { get; set; }

        public string Valutazione_Conto_Des { get; set; }
        public int Valutazione_Sezione_Cod { get; set; }
        public int Ordine_Default { get; set; }

        public bool flag_cancellazione { get; set; }


        public Valutazione_Conto()
        {
            this.flag_cancellazione = false;

        }

        public Valutazione_Conto(PK primaryKey)
        {
            this.primaryKey = primaryKey;
            this.flag_cancellazione = false;

        }

        public class PK
        {
            public int Valutazione_Conto_Cod { get; set; }

            public PK(int valutazione_conto_cod)
            {
                this.Valutazione_Conto_Cod = valutazione_conto_cod;
            }

            public PK()
            {
                this.Valutazione_Conto_Cod = 0;
            }
        }
    }

}