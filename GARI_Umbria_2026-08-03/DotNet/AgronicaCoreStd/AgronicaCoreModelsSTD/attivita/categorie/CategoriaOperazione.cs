using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita.categorie
{
    public class CategoriaOperazione : BaseCodeDescrStr
    {
        
        public CategoriaOperazione(string codice, string descrizione) : base(codice,descrizione)
        {
        
        }
        public CategoriaOperazione() : base()
        {

        }

        public static CategoriaOperazione forCodiceLavorazione(string codiceLavorazione) {
            return new CategoriaOperazione("a", "b");
        }

    }
}
