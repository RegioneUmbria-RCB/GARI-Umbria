using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita
{
    public class AttivitaCDG : Job
    {
        public AttivitaCDG()
        {
            tipo = TipiJob.ATTIVITACDG;
            primaryKey = new PK(costanti.ClassType.AttivitaCDG, "0");
        }

        public AttivitaCDG(int codice, string descrizione)
        {
            tipo = TipiJob.ATTIVITACDG;
            primaryKey = new PK(costanti.ClassType.AttivitaCDG, codice.ToString());
            this.descrizione = descrizione;
        }
    }
}
