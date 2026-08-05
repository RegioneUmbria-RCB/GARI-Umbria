using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.analisi
{
    public class AnalisiTerreno : Analisi
    {
        public double? longitude { get; set; }
        public double? latitude { get; set; }

        public string riferimento1 { get; set; }
        public string riferimento2 { get; set; }
        public string riferimento3 { get; set; }
        public string riferimento4 { get; set; }
        public string riferimento5 { get; set; }
        public ClasseTessitura tessitura { get; set; }

        public List<int> entitaCoinvolte { get; set; }

        public List<AnalisiEntita<Impresa>> entitaImprese { get; set; }
        public List<AnalisiEntita<CentroAziendale>> entitaCentri { get; set; }
        public List<AnalisiEntita<Campo>> entitaCampi { get; set; }
        public List<AnalisiEntita<Appezzamento>> entitaAppezzamenti { get; set; }
        public List<AnalisiEntita<Impianto>> entitaImpianti { get; set; }
        public List<AnalisiEntita<Fabbricato>> entitaFabbricati { get; set; }
        public List<AnalisiEntita<CatastoCentroAziendale>> entitaParticelleCatastali { get; set; }
        //public List<AnalisiEntita<>> entitaGrafiche { get; set; }


        public List<Campione> campioni { get; set; }

        public bool analisiUtilizzata { get; set; }

        public AnalisiTerreno(int codice, string descrizione) : base(codice, descrizione)
        {
            entitaCoinvolte = new List<int>();
        }
        public AnalisiTerreno() : base()
        {
            entitaCoinvolte = new List<int>();
        }


        public bool flag_cancellazione { get; set; }
        public string utente_ultima_modifica { get; set; }

    }
}
