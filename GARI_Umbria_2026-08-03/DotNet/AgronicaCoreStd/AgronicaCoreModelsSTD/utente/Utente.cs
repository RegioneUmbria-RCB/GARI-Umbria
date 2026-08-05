using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.utente
{
    public class Utente
    {
        public String Username { get; set; }
        public String Nome { get; set; }
        public String Cognome { get; set; }
        public String Rag_Soc { get; set; }
        public String Cod_Fisc { get; set; }
        public String UsernameCommerciale { get; set; }
        public List<Utente_Permesso> Permessi { get; set; }
        public List<Utente_Impostazioni> Impostazioni { get; set; }
        public Messaggio_Utente_Permessi ValiditaUtentePermessiLicenza { get; set; }
    }
}
