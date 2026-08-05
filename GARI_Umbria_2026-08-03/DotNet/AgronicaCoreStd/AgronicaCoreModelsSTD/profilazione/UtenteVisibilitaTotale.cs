using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.profilazione
{
    public class UtenteVisibilitaTotale
    {
        public string Username { get; set; }
        public bool VisibilitaTotale { get; set; }

        public UtenteVisibilitaTotale(string username, bool visibilitaTotale)
        {
            this.Username = username;
            this.VisibilitaTotale = visibilitaTotale;
        }
    }
}
