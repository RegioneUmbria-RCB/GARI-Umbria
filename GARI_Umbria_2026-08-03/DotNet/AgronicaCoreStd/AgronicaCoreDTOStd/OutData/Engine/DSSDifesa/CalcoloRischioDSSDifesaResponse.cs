using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

namespace OutData.Engine.DSSDifesa
{
    /// <summary>
    /// Response model for POST /v1/rischio/calcolo API endpoint.
    /// Based on DS08-API: POST _v1_rischio_calcolo.
    /// </summary>
    public class CalcoloRischioDSSDifesaResponse
    {
        /// <summary>
        /// Calculation result data.
        /// </summary>
        public CalcoloRischioResult Result { get; set; } = null;

        /// <summary>
        /// Response status (OK for success).
        /// </summary>
        public string Status { get; set; } = "OK";
    }

    /// <summary>
    /// Result data for risk calculation.
    /// </summary>
    public class CalcoloRischioResult
    {
        /// <summary>
        /// List of model results.
        /// </summary>
        public List<RisultatoModello> RisultatiModelli { get; set; } = new List<RisultatoModello>();

        /// <summary>
        /// Timestamp when results were collected (ISO 8601).
        /// </summary>
        public string TimestampRaccolta { get; set; } = string.Empty;

        /// <summary>
        /// Whether the global timeout was reached.
        /// </summary>
        public bool TimeoutGlobaleRaggiunto { get; set; } = false;
    }

    /// <summary>
    /// Individual model result.
    /// </summary>
    public class RisultatoModello
    {
        /// <summary>
        /// Model identifier.
        /// </summary>
        public int ModelId { get; set; }

        /// <summary>
        /// Model code.
        /// </summary>
        public string ModelCode { get; set; } = string.Empty;

        /// <summary>
        /// Model description.
        /// </summary>
        public string ModelDescription { get; set; } = string.Empty;

        /// <summary>
        /// Model group description.
        /// </summary>
        public string ModelGroup { get; set; } = string.Empty;

        /// <summary>
        /// Synthetic output data.
        /// </summary>
        public RisultatoModelloSynteticOutput OutputSintetico { get; set; } = null;

        /// <summary>
        /// Analytical output data.
        /// </summary>
        public RisultatoModelloAnalyticOutput OutputAnalitico { get; set; } = null;

        /// <summary>
        /// Calculation timestamp (ISO 8601).
        /// </summary>
        //[Required]
        public string TimestampCalcolo { get; set; } = string.Empty;

        /// <summary>
        /// Calculation status (COMPLETATO, NON_DISPONIBILE, ERRORE).
        /// </summary>
        //[Required]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Error message if status is ERRORE.
        /// </summary>
        public string MessaggioErrore { get; set; } = string.Empty;
    }

    public class RisultatoModelloSynteticOutput
    {
        //public string Model { get; set; }
        public int Status { get; set; }
        public string Disease { get; set; }
        public string Message { get; set; }
        public string CropType { get; set; }
        public RisultatoModelloSynteticOutputBand[] Bands { get; set; } = new RisultatoModelloSynteticOutputBand[3];
        public int RiskIndex { get; set; } = 0;
        public string StatusVerbose { get; set; }
        public int ResultProgress { get; set; }
        public string WarningMessage { get; set; }
    }

    public class RisultatoModelloSynteticOutputBand
    {
        public int Max { get; set; }
        public int Min { get; set; }
        public string Name { get; set; } = "";
        public string Color { get; set; } = "#000000";
    }

    public class RisultatoModelloAnalyticOutput
    {
        public string Modello_Tabella1 { get; set; }
    }
}
