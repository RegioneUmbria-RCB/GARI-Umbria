using AgronicaCoreModelsSTD.attivita.categorie;

namespace AgronicaCoreModelsSTD.attivita
{
    public class Registrazione : Job
    {
        public CategoriaOperazione categoriaOperazione { get; set; }

        public Registrazione()
        {
            tipo = TipiJob.REGISTRAZIONE;
            primaryKey = new PK(costanti.ClassType.Registrazione, "0");
        }

        public Registrazione(int codice)
        {
            tipo = TipiJob.REGISTRAZIONE;
            primaryKey = new PK(costanti.ClassType.Registrazione, codice.ToString());
        }

        public Registrazione(int codice, string descrizione)
        {
            tipo = TipiJob.REGISTRAZIONE;
            this.primaryKey = new PK(costanti.ClassType.Registrazione, codice.ToString());
            this.descrizione = descrizione;
        }

    }
}
