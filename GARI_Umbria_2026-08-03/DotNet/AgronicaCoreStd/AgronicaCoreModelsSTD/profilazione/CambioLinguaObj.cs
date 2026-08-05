using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.profilazione
{
    public class CambioLinguaObj
    {
        /// <summary>
        /// Username dell'utente per cui eseguire la modifica.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Codice della lingua da impostare.
        /// </summary>
        public int LinguaCod { get; set; }
    }
}
