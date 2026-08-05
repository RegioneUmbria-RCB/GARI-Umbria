using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.analisi
{
    public class Campione : BaseCodeDescr
    {
        public double longitude { get; set; }
        public double latitude { get; set; }

        public double quantita { get; set; }
        public UnitaDiMisura unitaMisura { get; set; }

        public double profondita { get; set; }
        public double profonditaMin { get; set; }
        public double profonditaMax { get; set; }

        public string riferimento1 { get; set; }
        public string riferimento2 { get; set; }
        public string riferimento3 { get; set; }
        public string riferimento4 { get; set; }
        public string riferimento5 { get; set; }

        public string note { get; set; }
        public DateTime dataPrelievo { get; set; }


        public string KeyPiva { get; set; }
        public int KeySaCod { get; set; }
        public string KeyGrafica { get; set; }

        public ParticelleCatastali particella { get; set; }

        public Campione(int codice, string descrizione) : base(codice, descrizione) { }
        public Campione() : base() { }

    }
}
