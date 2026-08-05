using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.models
{
    public class DatiAziendaRequest
    {
        // impostazioni prodotti
        public bool anagTrasformatiVegetali;
        public bool anagSementi;

        // impostazioni piano colturale
        public bool anagPianoColturale;

        // anagrafiche fornitori
        public bool anagFornitori;

        // gestione sincro in base ai permessi
        public string permessi;
    }
}
