using AgronicaCoreModelsSTD.Gis;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class LeggiConfigurazioniGeneraliGis
    {
        public string ParametriFiltroPercorsi { get; set; }
        public string Filtrone { get; set; }
        public List<Impianto2010> Impianti { get; set; }
        public int IDTestataTemp { get; set; }
        //Sementi
        public string Sementi { get; set; }
        public string SementiMappaturaLibera { get; set; }
    }
}
