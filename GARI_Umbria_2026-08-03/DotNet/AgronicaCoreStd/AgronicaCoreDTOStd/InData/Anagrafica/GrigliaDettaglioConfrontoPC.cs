using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using InData.Anagrafica;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    public class PianoColturale : DettaglioEntita
    {
        public int tipo {  get; }

        public PianoColturale(int tipo) : base()
        {
            this.tipo = 1;
        }
    }

    public class Planning : DettaglioEntita
    {
        public int tipo { get; }
        public Planning(int tipo) : base()
        {
            this.tipo = 2;
        }
    }

    public class Budget : DettaglioEntita
    {
        public int tipo { get; }
        public Budget(int tipo) : base()
        {
            this.tipo = 3;
        }
    }

    public class DettaglioEntita
    {
        public ParticelleCatastali.PK particella { get; set; }
        public UtilizzoTerreno utilizzoTerreno { get; set; }
        public double superficie { get; set; }

        public DettaglioEntita()
        {
            utilizzoTerreno = null;
            particella = null;
            superficie = 0;
        }
    }
}
