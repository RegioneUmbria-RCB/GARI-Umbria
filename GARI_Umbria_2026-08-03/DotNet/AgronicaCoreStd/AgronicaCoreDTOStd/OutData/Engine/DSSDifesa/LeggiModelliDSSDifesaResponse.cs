using AgronicaCoreModelsSTD.Engine;
using System;
using System.Collections.Generic;

namespace OutData.Engine.DSSDifesa
{
    public class LeggiModelliDSSDifesaResponse
    {
        /// <summary>
        /// List of pest models.
        /// </summary>
        public List<DSSModelloEngine> Modelli { get; set; } = new List<DSSModelloEngine>();

        /// <summary>
        /// Status of the operation ("OK" or "ERROR").
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp of the fetch in ISO 8601 format.
        /// </summary>
        public string FetchTimestamp { get; set; } = string.Empty;
    }
}
