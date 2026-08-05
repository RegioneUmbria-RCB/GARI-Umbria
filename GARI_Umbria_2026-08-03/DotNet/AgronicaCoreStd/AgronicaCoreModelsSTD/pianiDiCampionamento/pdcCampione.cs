using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.pianiDiCampionamento
{
    public class pdcCampione: BaseCodeDescr
    {
        public string codice_campione { get; set; }
        public DateTime data_campionamento { get; set; }
        public BaseCodeDescr stato { get; set; }
        public int flag_pdc { get; set; }
        public string note { get; set; }
        public List<pdcAnalisi> analisi { get; set; }
        public pdcCampione() { }
    }
}
