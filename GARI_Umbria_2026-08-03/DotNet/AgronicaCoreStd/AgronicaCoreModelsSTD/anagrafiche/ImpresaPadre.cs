using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class ImpresaPadre : Impresa 
    {
        public string codice_iscrizione_libro_soci { get; set; }
        public DateTime data_iscrizione_libro_soci { get; set; }

        public ImpresaPadre() : base() { }

        public ImpresaPadre(string piva) : base(piva, "") { }
    }
}
