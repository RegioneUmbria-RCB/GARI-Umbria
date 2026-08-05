using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;

namespace AgronicaCoreModelsSTD.attivita.risorse
{
    public class RisorsaCausale : Risorsa
    {
        public int id { get; set; }

        //public int lav_cod { get; set; }

        public String causale { get; set; }

        //public String attivitaCod { get; set; }


        public RisorsaCausale()
        {
            //id = -1;
            //lav_cod = -1;
            //causale = "";
            //attivitaCod = "";
            classType = costanti.ClassType.RisorsaCausale;
        }

        public RisorsaCausale(int id, int lavcod, String causale, String attivitaCod) {
            this.id = id;
            //this.lav_cod = lavcod;
            this.causale = causale;
            //this.attivitaCod = attivitaCod;
            classType = costanti.ClassType.RisorsaCausale;
        }

    }
}
