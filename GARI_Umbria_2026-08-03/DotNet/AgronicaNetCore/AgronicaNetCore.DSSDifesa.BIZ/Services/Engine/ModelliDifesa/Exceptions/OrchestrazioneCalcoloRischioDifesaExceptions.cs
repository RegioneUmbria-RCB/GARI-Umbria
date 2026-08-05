namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Exceptions
{
    /// <summary>
    /// Indicates invalid input for the orchestrated risk calculation request.
    /// </summary>
    public class CalcoloRischioDifesaRequestValidationException : ArgumentException
    {
        public CalcoloRischioDifesaRequestValidationException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Indicates that an end-to-end or per-stage timeout has been reached.
    /// </summary>
    public class CalcoloRischioDifesaDataReadTimeoutException : TimeoutException
    {
        public CalcoloRischioDifesaDataReadTimeoutException(string message) : base(message)
        {
        }

        public CalcoloRischioDifesaDataReadTimeoutException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Indicates a non-recoverable fatal error in the orchestration flow.
    /// </summary>
    public class CalcoloRischioDifesaDataReadFataleException : Exception
    {
        public CalcoloRischioDifesaDataReadFataleException(string message) : base(message)
        {
        }

        public CalcoloRischioDifesaDataReadFataleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
