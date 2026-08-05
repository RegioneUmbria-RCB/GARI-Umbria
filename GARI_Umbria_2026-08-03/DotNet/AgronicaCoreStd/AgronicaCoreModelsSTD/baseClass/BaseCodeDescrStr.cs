using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.baseClass
{
    public class BaseCodeDescrStr
    {
        public string codice { get; set; }
        public string descrizione { get; set; }
        public BaseCodeDescrStr(string code, string description)
        {
            this.codice = code;
            this.descrizione = description;
        }

        public BaseCodeDescrStr()
        {
            this.codice = "";
            this.descrizione = "";
        }

    }
}
