using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.Gis;

namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Classe scrittura nuovo layer personalizzato
    /// </summary>    

    public class ScriviNuovoLayerPersonalizzato_In
    {

        /// <summary>
        /// Nome layer
        /// </summary>
        /// <example>NuovoLayer</example>
        public string NomeLayer { get; set; }

        /// <summary>
        /// Indica se mostrare la descrizione associata al layer: 1 = Visibile, 0 = Nascosta.
        /// </summary>
        /// <example>0</example>
        public string MostraDescrizioneAssociata { get; set; }

        /// <summary>
        /// Indica il tipo di oggetto disegnabile sul layer: 1 = Punto, 3 = Linea, 5 = Poligono.
        /// </summary>
        /// <example>5</example>
        public int FeatureTypeId { get; set; }

    }
}
