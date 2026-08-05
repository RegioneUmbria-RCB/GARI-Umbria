using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class CreaProdottiResult
    {
        public int sementiCreati {get;set;} = 0;
        public int sementiEsistenti {get;set;} = 0;
        public int trasformatiCreati {get;set;} = 0;
        public int trasformatiEsistenti { get; set; } = 0;

        public CreaProdottiResult() { }

        public CreaProdottiResult(int sementiCreati, int sementiEsistenti, int trasformatiCreati, int trasformatiEsistenti)
        {
            this.sementiCreati = sementiCreati;
            this.sementiEsistenti = sementiEsistenti;
            this.trasformatiCreati = trasformatiCreati;
            this.trasformatiEsistenti = trasformatiEsistenti;
        }

    }
}
