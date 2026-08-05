using AgronicaCoreModelsSTD.analisi;
using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.pianiDiCampionamento
{
    public class pdcDettaglio<T> : BaseCodeDescr
    {
        public List<pdcCampione> campioni { get; set; }
        public T elementoAnagrafico { get; set; }
        public pdcLFO lfo { get; set; }
        public pdcDettaglio(): base() { }
    }
}
