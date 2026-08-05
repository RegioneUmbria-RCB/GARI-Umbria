using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita
{
    public class JobComposito : Job
    {
        public Lavorazione lavorazione;
        public AttivitaCDG attivitaCDG;

        public JobComposito()
        {
            tipo = TipiJob.JOB_COMPOSITE;
            primaryKey = new PK(costanti.ClassType.JobComposito, "");
        }

        public JobComposito(string codice)
        {
            tipo = TipiJob.JOB_COMPOSITE;
            primaryKey = new PK(costanti.ClassType.JobComposito, codice);
        }

        public JobComposito(Lavorazione l, AttivitaCDG a)
        {
            tipo = TipiJob.JOB_COMPOSITE;
            primaryKey = new PK(costanti.ClassType.JobComposito, l.getCodice() + "|" + a.getCodice());
            descrizione = l.descrizione + " " + a.descrizione;
            lavorazione = l;
            attivitaCDG = a;
        }
    }
}
