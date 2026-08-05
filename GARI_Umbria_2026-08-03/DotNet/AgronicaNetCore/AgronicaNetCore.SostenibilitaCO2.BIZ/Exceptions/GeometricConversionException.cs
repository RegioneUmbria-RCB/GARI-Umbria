namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando il centroide dell'azienda non è convertibile in EPSG:4326
    /// (geometria invalida o proiezione sconosciuta).
    /// In questo caso il sistema applica il fallback: usa il centroide originale e logga un warning.
    /// Riferimento spec: DS03-BL AssemblyPayloadM4FilieraAzienda — Eccezioni.
    /// </summary>
    public class GeometricConversionException : Exception
    {
        /// <summary>Partita IVA dell'azienda il cui centroide non è stato convertito.</summary>
        public string Piva { get; }

        /// <summary>Valore originale del centroide che non ha superato la conversione.</summary>
        public string? OriginalValue { get; }

        public GeometricConversionException(string piva, string? originalValue, Exception innerException)
            : base($"Impossibile convertire il centroide dell'azienda '{piva}' in EPSG:4326. Valore originale: '{originalValue}'.", innerException)
        {
            Piva = piva;
            OriginalValue = originalValue;
        }

        public GeometricConversionException(string piva, string? originalValue)
            : base($"Impossibile convertire il centroide dell'azienda '{piva}' in EPSG:4326. Valore originale: '{originalValue}'.")
        {
            Piva = piva;
            OriginalValue = originalValue;
        }
    }
}
