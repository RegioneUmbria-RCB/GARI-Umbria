using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class ApportoMacroelementi
    {

        public RegolamentoConcimazione pianoConcimazione { get; set; }
        public FinalitaPianoConcimazione tipologia { get; set; }

        /// <summary>
        /// creare classe: capire da dove arriva per trovare nomenclaura per classe
        /// </summary>
        public FaseCicloColturale fase { get; set; }
        public double? n { get; set; }
        public double? p2o5 { get; set; }
        public double? k2o { get; set; }
        public double? mgo { get; set; }


        public ApportoMacroelementi()
        {
        }
    }
}
