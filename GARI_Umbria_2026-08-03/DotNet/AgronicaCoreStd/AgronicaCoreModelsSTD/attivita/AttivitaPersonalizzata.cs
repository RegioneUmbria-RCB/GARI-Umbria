using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.baseClass;
using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.attivita
{

    /// <summary>
    /// La classe AttivitaPersonalizzata si riferisce alla tabella Attivita sul db server
    /// </summary>
    public class AttivitaPersonalizzata : BaseCodeDescr
    {

        public string sigla { get; set; }

        public List<Lavorazione> operazioni { get; set; }

        public AttivitaPersonalizzata(int codice) : base(codice, "")
        {
            operazioni = new List<Lavorazione>();
        }


        public AttivitaPersonalizzata() : base() {
            operazioni = new List<Lavorazione>();
        }

    }

}
