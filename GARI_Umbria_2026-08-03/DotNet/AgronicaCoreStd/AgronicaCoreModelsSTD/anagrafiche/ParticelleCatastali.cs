using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class ParticelleCatastali
    {

        public PK primaryKey { get; set; }

        public double Area { get; set; }

        public List<ParticelleCatastaliMacrouso> macrousi { get; set; }

        public List<ParticelleCatastaliZona> zonizzazione { get; set; }

        public List<ParticelleCatastaliClassamento> classamento { get; set; }

        public List<ParticelleCatastaliMetodoProduzione> metodoProduzione { get; set; }

        public string proprietario { get; set; }

        public ParticelleCatastali(PK primaryKey)
        {
            this.primaryKey = primaryKey;
        }

        public ParticelleCatastali()
        {

        }

        public class PK
        {

            public string Prov { get; set; }
            public string Com { get; set; }
            public string Sezione { get; set; }
            public int Foglio { get; set; }
            public int Numero { get; set; }
            public string Subalterno { get; set; }

            public PK(string Prov, string Com, string Sezione, int Foglio, int Numero, string Subalterno)
            {
                this.Prov = Prov;
                this.Com = Com;
                this.Sezione = Sezione;
                this.Foglio = Foglio;
                this.Numero = Numero;
                this.Subalterno = Subalterno;
            }

            public PK()
            {

            }
        }

    }
}
