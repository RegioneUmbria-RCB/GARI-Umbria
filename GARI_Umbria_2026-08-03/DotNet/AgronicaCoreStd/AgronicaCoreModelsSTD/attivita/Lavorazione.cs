using AgronicaCoreModelsSTD.attivita.categorie;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita
{
    public class Lavorazione : Job
    {
        public CategoriaOperazione categoriaOperazione { get; set; }

        public Lavorazione()
        {
            tipo = TipiJob.LAVORAZIONE;
            primaryKey = new PK(costanti.ClassType.Lavorazione, "0");
        }

        public Lavorazione(int codice)
        {
            tipo = TipiJob.LAVORAZIONE;
            primaryKey = new PK(costanti.ClassType.Lavorazione, codice.ToString());
        }

        public Lavorazione(int codice, string descrizione)
        {
            tipo = TipiJob.LAVORAZIONE;
            this.primaryKey = new PK(costanti.ClassType.Lavorazione, codice.ToString());
            this.descrizione = descrizione;
        }

    }
}
