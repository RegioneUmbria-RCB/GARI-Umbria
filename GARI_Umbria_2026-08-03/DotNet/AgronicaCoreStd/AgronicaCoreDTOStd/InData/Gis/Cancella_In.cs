using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class Cancella_In
    {

        /// <summary>
        /// Codice Da Eliminare
        /// </summary>
        /// <example>-1</example>
        public string Entita_Cod { get; set; }

        /// <summary>
        /// Indica se ci si trova in modalità sementi, con uno sportello selezioanto, la cancellazione seguirà le logiche di sportello
        /// </summary>
        public string Sementi { get; set; }

        /// <summary>
        /// Indica se ci si trova in modalità mappatura libera nel contesto della modalità sementi
        /// </summary>
        public string SementiMappaturaLibera { get; set; }


        public string DatiPassaggio { get; set; }

        /// <summary>
        /// Flag di eliminazione: Dato cartografico associato
        /// </summary>
        /// <example>true</example>
        public bool Elimina_Grafica { get; set; }

        /// <summary>
        /// Flag di eliminazione: Elimina impianto, appezzamento, distinta se il dato è associato ad un elemento di anagrafica
        /// </summary>
        /// <example>false</example>
        public bool Elimina_Impianto { get; set; }

        /// <summary>
        /// Flag di eliminazione: Elimina i dati di precision farming associati
        /// </summary>
        /// <example>false</example>
        public bool Elimina_PrecisionFarming { get; set; }

        /// <summary>
        /// Flag di eliminazione: Elimina le linee guida AB associate
        /// </summary>
        /// <example>false</example>
        public bool Elimina_PrecisionFarmingABLine { get; set; }

        /// <summary>
        /// Flag di eliminazione: Elimina il dato misurato con il software GIASPALM
        /// </summary>
        /// <example>false</example>
        public bool Elimina_DatoGiasPalm { get; set; }

        /// <summary>
        /// Flag di eliminazione: Elimina anagrafica appezzamento in modalità sementieri
        /// </summary>
        /// <example>false</example>
        public bool Elimina_Dato_Anagrafica { get; set; }
    }
}
