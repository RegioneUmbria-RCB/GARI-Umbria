using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.valutazioni
{
    public class Valutazione_Testata
    {
        public PK primaryKey { get; set; }

        public string Ragione_Sociale { get; set; }
        public int Valutazione_Piano_Cod { get; set; }
        public string Valutazione_Piano_Des { get; set; }
        public string Data_Redazione { get; set; }
        public string Note { get; set; }
        public List<Valutazione_TestataxAnno> Valutazione_TestataxAnno { get; set; }

        public bool flag_cancellazione { get; set; }


        public Valutazione_Testata()
        {
            this.flag_cancellazione = false;
            Valutazione_TestataxAnno = new List<Valutazione_TestataxAnno>();
        }

        public Valutazione_Testata(PK primaryKey)
        {
            this.primaryKey = primaryKey;
            this.flag_cancellazione = false;
            Valutazione_TestataxAnno = new List<Valutazione_TestataxAnno>();
        }

        public class PK
        {
            public string Piva { get; set; }
            public int Id_Testata { get; set; }

            public PK(string piva, int codice)
            {
                this.Piva = piva;
                this.Id_Testata = codice;
            }

            public PK()
            {
                this.Piva = "";
                this.Id_Testata = 0;
            }
        }
    }

}
