namespace AgronicaNetCore.RischiMeteo.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando <c>poligonoWkt</c> o <c>centroideWkt</c> non rispettano
    /// il formato WKT valido richiesto (rispettivamente <c>POLYGON(...)</c> e <c>POINT(...)</c>).
    /// Il payload per l'Esercizio viene saltato.
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Validazione formato WKT, InvalidWktFormatException.
    /// </summary>
    public class InvalidWktFormatException : Exception
    {
        /// <summary>Nome del campo con formato WKT non valido.</summary>
        public string Campo { get; }

        /// <summary>Valore che ha causato il fallimento della validazione.</summary>
        public string? ValoreInvalido { get; }

        public InvalidWktFormatException(string campo, string? valoreInvalido)
            : base($"Il campo '{campo}' non rispetta il formato WKT valido. Valore ricevuto: '{valoreInvalido}'.")
        {
            Campo = campo;
            ValoreInvalido = valoreInvalido;
        }
    }
}
