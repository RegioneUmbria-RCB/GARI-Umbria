using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{

    /// <summary>
    /// Lettura delle proprietà di un oggetto grafico
    /// </summary>
    public class GetProprieta_In
    {
        /// <summary>
        /// Codice Entita
        /// </summary>
        /// <example>31344</example>
        public string Entita_Cod { get; set; }

        /// <summary>
        /// Tipologia di lettura proprietà: 1 = caricamento della modifica appezzamento, 2 = in altri casi
        /// </summary>
        /// <example>1</example>
        public int Tipo_GetProp { get; set; }
    }
}
