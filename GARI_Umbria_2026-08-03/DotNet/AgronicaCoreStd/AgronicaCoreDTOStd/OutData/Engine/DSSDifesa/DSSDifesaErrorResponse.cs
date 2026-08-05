using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

namespace OutData.Engine.DSSDifesa
{
    public class DSSDifesaErrorResponse
    {
        /// <summary>
        /// Error type identifier.
        /// </summary>
        //[Required]
        public string Error { get; set; } = string.Empty;

        /// <summary>
        /// Specific error code.
        /// </summary>
        //[Required]
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// Human-readable error message.
        /// </summary>
        //[Required]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Additional error details.
        /// </summary>
        public object Details { get; set; } = null;

        /// <summary>
        /// Timestamp of the error (ISO 8601).
        /// </summary>
        //[Required]
        public string Timestamp { get; set; } = string.Empty;
    }
}
