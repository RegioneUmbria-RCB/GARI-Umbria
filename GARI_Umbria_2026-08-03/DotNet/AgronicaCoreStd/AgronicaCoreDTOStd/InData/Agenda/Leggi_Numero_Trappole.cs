using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.metaschema.avversita;
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class Leggi_Numero_Trappole
    {
        public List<EsercizioCDC> eserciziCDC { get; set; }

        public AvversitaGruppo avversitaGruppo { get; set; }
    }
}
