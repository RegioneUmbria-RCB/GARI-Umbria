namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Exceptions
{
    /// <summary>
    /// Exception raised when model execution returns explicit ERROR status.
    /// Referenced in DS05-BL_ Lancio Modelli Paralleli Calcolo Rischio.
    /// </summary>
    public class ModelExecutionErrorException : Exception
    {
        public string ErrorCode { get; }
        public string ErrorMessage { get; }

        public ModelExecutionErrorException(string errorCode, string errorMessage)
            : base($"Model error: {errorCode} - {errorMessage}")
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}
