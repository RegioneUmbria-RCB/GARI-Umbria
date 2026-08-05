using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class LeggiTipiOggettoPerElementoGrafico
    {
        /// <summary>
        /// La Partita Iva dell'utente
        /// </summary>
        public string PivaSuperUser { get; set; }

        /// <summary>
        /// ID Elemento Grafico
        /// </summary>
        public string LayerElementiGrafici_Cod { get; set; }

        /// <summary>
        /// Lista di tipi oggetto per l'elemento grafico selezionato
        /// </summary>
        public List<int> ListaTipiOggetto { get; set; }
    }

}
