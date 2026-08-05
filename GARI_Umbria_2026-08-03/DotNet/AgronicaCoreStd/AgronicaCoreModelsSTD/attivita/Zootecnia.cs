using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita
{
    public class Zootecnia : Job
    {
        public Zootecnia()
        {
            tipo = TipiJob.ZOOTECNIA;
            primaryKey = new PK(costanti.ClassType.Zootecnia, "0");
        }

        public Zootecnia(int codice, string descrizione)
        {
            tipo = TipiJob.ZOOTECNIA;
            primaryKey = new PK(costanti.ClassType.Zootecnia, codice.ToString());
            this.descrizione = descrizione;
        }
    }
}
