using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.analisi;
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Demetra
{
    public class AnalisiTerreno
    {
        public string codice { get; set; }
        public string codice_esterno { get; set; }

        public string descrizione { get; set; }

        public double? longitude { get; set; }
        public double? latitude { get; set; }

        public IntervalloTemporale validita { get; set; }

        public string numero_certificato { get; set; }

        public List<AnalisiDettaglio> dettagli { get; set; }

        public bool flag_cancellazione { get; set; }
        public string utente_ultima_modifica { get; set; }

    }
}
