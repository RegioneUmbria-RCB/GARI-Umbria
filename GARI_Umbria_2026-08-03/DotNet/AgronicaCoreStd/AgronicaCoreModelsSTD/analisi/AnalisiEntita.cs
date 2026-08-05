using AgronicaCoreModelsSTD.analisi;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.baseClass;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.analisi
{
    public class AnalisiEntita<T> 
    {
        public T elementoAnagrafico { get; set; }
        public int progressivo { get; set; }
        public Prodotto prodotto { get; set; }
        public string lotto { get; set; }
        public bool stima { get; set; }
        public bool validazione { get; set; }

        public AnalisiEntita() { }

    }
}
