using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.valutazioni
{
    public class Valutazione_TestataxAnno
    {
        public PK primaryKey { get; set; }

        //public string Piva { get; set; }
        //public string Id_Testata { get; set; }
        //public int Anno { get; set; }
        public int Anno_Tipo { get; set; }
        public string Anno_Tipo_Des { get; set; }

        public bool flag_cancellazione { get; set; }

        public Valutazione_TestataxAnno()
        {
            this.flag_cancellazione = false;
        }

        public Valutazione_TestataxAnno(PK primaryKey)
        {
            this.primaryKey = primaryKey;
            this.flag_cancellazione = false;
        }

        public class PK
        {
            public int Anno { get; set; }
            public Valutazione_Testata.PK valutazioneTestataPK { get; set; }


            public PK(int anno, Valutazione_Testata.PK valutazioneTestataPK)
            {
                this.Anno = anno;
                this.valutazioneTestataPK = valutazioneTestataPK;
            }

            public PK()
            {

            }

        }
    }

}
