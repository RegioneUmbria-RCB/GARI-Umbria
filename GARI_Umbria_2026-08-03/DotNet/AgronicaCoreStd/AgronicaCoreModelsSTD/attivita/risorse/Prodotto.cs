using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreModelsSTD.attivita.risorse
{
    
    public class Prodotto : BaseCodeDescr
    {
        public int elemCod { get; set; }  // x compatibilità app
        
        public string codice_alfanumerico { get; set; }
        
        public TipoRisorsa tipo { get; set; }
        /// <summary>
        /// VANNI
        /// </summary>
        public UnitaDiMisura unitaDiMisura { get; set; }

        public Specie specie { get; set; }
        public Varieta varieta { get; set; }
        public GruppoFinalita finalita { get; set; }
        public Regolamenti regolamento { get; set; }


        public Prodotto(int codice) : base(codice, "")
        {
        }

        public Prodotto(int codice, int elem_cod) : base(codice,"")
        {
            elemCod = elem_cod;
        }

        public Prodotto() : base()
        {

        }

    }

}
