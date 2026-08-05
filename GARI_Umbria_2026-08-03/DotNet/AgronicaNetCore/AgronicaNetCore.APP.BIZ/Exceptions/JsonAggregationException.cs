namespace AgronicaNetCore.APP.BIZ.Exceptions;

/// <summary>
/// Thrown when the JSON aggregation fails because one or more
/// <c>json_content</c> values cannot be parsed as valid JSON.
/// Ref: DS09-BL – Eccezioni: JsonAggregationException.
/// </summary>
public class JsonAggregationException : Exception
{
    public JsonAggregationException(string message) : base(message) { }

    public JsonAggregationException(string message, Exception innerException) : base(message, innerException) { }
}
