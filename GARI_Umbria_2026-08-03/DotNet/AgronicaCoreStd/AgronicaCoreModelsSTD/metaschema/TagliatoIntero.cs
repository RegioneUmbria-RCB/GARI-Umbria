using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class TagliatoIntero : BaseCodeDescrStr
    {

        //public string codice { get; set; }

        public enum TIPO
        {
            tagliato = 0,
            intero = 1
        }

        public TagliatoIntero(string codice) : base(codice,"")
        {
        }

        public TagliatoIntero() : base()
        {

        }
    }
}
