using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.pianiDiCampionamento
{
    public class pdcAnalisi: analisi.Analisi
    {
        public BaseCodeDescr stato { get; set; }
        public DateTime dataRichiesta { get; set; }
        public pdcAnalisi() { }
    }
}
