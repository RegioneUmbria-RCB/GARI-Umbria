using System;

namespace AgronicaCoreModelsSTD.attivita
{
    public class Blocco
    {
        public Tipo_Blocco tipo { get; set; }
        public string utente { get; set; }
        public DateTime data { get; set; }

        public Blocco()
        {
            this.tipo = Tipo_Blocco.Nessuno;
            this.utente = "";
            this.data = new DateTime(1900,1,1); //AGRODATAINIZIO
        }

        public Blocco(Tipo_Blocco tipo, string utente, DateTime data)            
        {
            this.tipo = tipo;
            this.utente = utente;
            this.data = data;
        }

    }

    public enum Tipo_Blocco
    {
        Nessuno = 0,
        QuadernoDiCampagna = 1
    }
}
