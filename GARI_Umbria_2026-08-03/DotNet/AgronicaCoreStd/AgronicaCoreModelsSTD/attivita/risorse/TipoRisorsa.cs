using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema;

namespace AgronicaCoreModelsSTD.attivita.risorse
{
    public class TipoRisorsa : BaseCodeDescr
    {
           /// <summary>
           /// 
           /// </summary>
           /// <param name="codice"> elem_cod (tabella categoriemagazzino)</param>
        public TipoRisorsa(int codice) : base(codice, "")
        {

        }

        public TipoRisorsa() : base()
        {

        }
    }
}



