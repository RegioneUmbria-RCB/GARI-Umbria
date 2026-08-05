using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class LeggiAllegatiDaRicettaDestinazione
    {
        /// <summary>
        /// Lista di allegati della ricetta di destinazione selezionata
        /// </summary>
        public List<AllegatiDaRicettaDestinazione> ListaAllegati { get; set; }
    }

    public class AllegatiDaRicettaDestinazione
    {

        /// <summary>
        /// Codice allegato
        /// </summary>
        public int Allegati_Documenti_Cod { get; set; }

        /// <summary>
        /// Descrizione allegato
        /// </summary>
        public string Allegati_Documenti_Des { get; set; }

    }

}
