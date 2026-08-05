using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.attivita.risorse;
using InData.Demetra;
using System;

namespace AgronicaCoreDTOStd.InData.Demetra
{
    public class Raccolto : IWithUDM
    {
        public string codice { get; set; }

        public string codice_esterno { get; set; }

        public decimal quantita { get; set; }
        public string udm { get; set; }

        public Magazzino magazzino { get; set; }

        public string plot_id { get; set; }

    }

}
