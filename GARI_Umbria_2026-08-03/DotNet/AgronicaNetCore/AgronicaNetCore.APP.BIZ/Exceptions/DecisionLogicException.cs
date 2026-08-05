namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when the DS03 prioritisation rules produce an ambiguous or unsupported decision.
/// Ref: DS03-BL – Eccezioni: DecisionLogicException.
/// </summary>
public class DecisionLogicException : Exception
{
    public DecisionLogicException(string message) : base(message) { }

    public DecisionLogicException(string message, Exception innerException)
        : base(message, innerException) { }
}