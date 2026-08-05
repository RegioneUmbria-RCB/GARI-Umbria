using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Parametri di filtro in lettura della lista dei layer e dei tiles
    /// </summary>
    public class AggiornaElencoTipologie_In
    {
        /// <summary>
        /// Tipologia layer selezionato
        /// 1 = Standard, 5 = Specie, 15 = Varietà, ecc.
        /// </summary>
        /// <example>1</example>
        public string Layer_Selezionato { get; set; }

        /// <summary>
        /// Codice di sportello sementieri passato come parametro (-1 valore predefinito)
        /// </summary>
        /// <example>-1</example>
        public string Sementieri_Sportello_Configurazione_cod { get; set; }

        /// <summary>
        /// P.iva selezionata (da objParametriAgenda)
        /// </summary>
        /// <example>01704430519</example>
        public string Piva { get; set; }

        /// <summary>
        /// Codice Fiscale Tecnico come letto da configurazione
        /// </summary>
        /// <example>CF TEC</example>
        public string Codice_Fiscale_Tecnico { get; set; }

        /// <summary>
        /// Indica se leggere anche i layer non visibili
        /// </summary>
        /// <example>false</example>
        public bool leggiLayerNonVisibili { get; set; } = false;

        /// <summary>
        /// Indica se leggere anche i layer non attivi
        /// </summary>
        /// <example>false</example>
        public bool leggiLayerNonAttivi { get; set; } = false;
    }
}
