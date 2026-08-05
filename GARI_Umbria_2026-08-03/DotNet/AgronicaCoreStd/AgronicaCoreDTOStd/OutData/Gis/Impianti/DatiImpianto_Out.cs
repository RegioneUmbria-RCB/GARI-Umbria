using System;

namespace AgronicaCoreDTOStd.OutData.Gis.Impianti
{
    /// <summary>
    /// Dati dell'impianto letti per il flusso delle Mappe di Prescrizione (semina).
    /// </summary>
    public class DatiImpianto_Out
    {

        /// <summary>Partita IVA dell'azienda.</summary>
        public string Piva { get; set; }

        /// <summary>Codice centro aziendale.</summary>
        public int SaCod { get; set; }

        /// <summary>Codice appezzamento.</summary>
        public int Appezza { get; set; }

        /// <summary>Codice impianto.</summary>
        public int IdImp { get; set; }

        /// <summary>Codice cultivar.</summary>
        public int CulCod { get; set; }

        /// <summary>Codice specie vegetale.</summary>
        public int VegCod { get; set; }
        public DateTime ValiditaInizio { get; set; }
        public DateTime ValiditaFine { get; set; }
    }
}
