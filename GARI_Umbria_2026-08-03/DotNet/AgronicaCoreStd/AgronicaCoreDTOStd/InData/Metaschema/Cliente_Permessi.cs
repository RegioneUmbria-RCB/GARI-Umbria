using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class Cliente_Permesso: AgronicaCoreModelsSTD.utente.IPermesso
    {
        public int Id_Servizio { get; set; }
        public int Id_Attivita { get; set; }
        public int Id_Operazione { get; set; }
        public int Permesso_ID
        {
            get { return Id_Attivita; }
            set { Id_Attivita = value; }
        }
        public int Permesso_Tipo
        {
            get { return Id_Operazione; }
            set { Id_Operazione = value; }
        }

        public Cliente_Permesso() { }
        public Cliente_Permesso(int attivita, int tipoPermesso)
        {
            this.Id_Attivita = attivita;
            this.Id_Operazione = tipoPermesso;
        }
        public Cliente_Permesso(int attivita, int tipoPermesso, int servizio)
        {
            this.Id_Attivita = attivita;
            this.Id_Operazione = tipoPermesso;
            this.Id_Servizio = servizio;
        }
    }

    public class Cliente_PermessiScrivi
    {
        public string UserName { get; set; }
        public List<Cliente_Permesso> permessi { get; set; }
    }
}
