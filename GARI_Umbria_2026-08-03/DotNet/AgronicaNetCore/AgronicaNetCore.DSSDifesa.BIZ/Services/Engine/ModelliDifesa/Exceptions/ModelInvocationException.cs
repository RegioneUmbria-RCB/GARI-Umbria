namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Exceptions
{
    /// <summary>
    /// Exception thrown when model invocation fails before polling due to bad request, serialization etc.
    /// Referenced in DS05-BL_ Lancio Modelli Paralleli Calcolo Rischio.
    /// </summary>
    public class ModelInvocationException : Exception
    {
        public ModelInvocationException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
