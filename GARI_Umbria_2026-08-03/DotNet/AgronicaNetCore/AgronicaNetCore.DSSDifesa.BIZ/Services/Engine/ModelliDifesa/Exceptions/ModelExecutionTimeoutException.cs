namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Exceptions
{
    /// <summary>
    /// Exception raised when a specific model execution does not complete within its per-model timeout.
    /// Referenced in DS05-BL_ Lancio Modelli Paralleli Calcolo Rischio.
    /// </summary>
    public class ModelExecutionTimeoutException : Exception
    {
        public ModelExecutionTimeoutException(string modelCode)
            : base($"Model execution timed out for model: {modelCode}")
        {
        }
    }
}
