namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Exceptions
{
    /// <summary>
    /// Exception thrown when global concurrent collection of all model executions exceeds the configured timeout.
    /// Referenced in DS05-BL_ Lancio Modelli Paralleli Calcolo Rischio.
    /// </summary>
    public class GlobalCollectionTimeoutException : Exception
    {
        public GlobalCollectionTimeoutException(TimeSpan globalTimeout)
            : base($"Global collection timeout exceeded: {globalTimeout.TotalSeconds} seconds")
        {
        }
    }
}
