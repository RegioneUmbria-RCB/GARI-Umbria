using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.contabilita
{
    public class Imputazione : baseClass.BaseCodeDescr
    {
        public string Nome { get; set; }
        public baseClass.BaseCodeDescr Tipo { get; set; }
        public baseClass.BaseCodeDescr Classe { get; set; }
    }
}
