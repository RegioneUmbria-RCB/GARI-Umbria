using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class UnitaDiMisura_Alternativa : BaseCodeDescr
    {

        public string simbolo { get; set; }
        public double tassoConversione { get; set; }

        public UnitaDiMisura_Alternativa(int codice) : base(codice, "")
        {

        }

        public UnitaDiMisura_Alternativa() : base(-1, "")
        {
        }
    }

}
