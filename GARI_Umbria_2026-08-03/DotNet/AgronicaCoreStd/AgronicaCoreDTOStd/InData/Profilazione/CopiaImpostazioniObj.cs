using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Profilazione
{
    public class CopiaImpostazioniObj
    {
        /// <summary>
        /// Lista di stringhe identificative degli utenti o imprese-centri per cui si vogliono salvare le impostazioni
        /// </summary>
        public IEnumerable<string> Base { get; set; }
        /// <summary>
        /// Stringa identificativa dell'utente o impresa-centro da cui copiare i valori delle impostazioni
        /// </summary>
        public string Template { get; set; }
    }

    public class CopiaVisibilitaObj : CopiaImpostazioniObj
    {
        public bool CopyHierarchy { get; set; } = false;
        public bool CopyProcedures { get; set; } = false;
    }
}
