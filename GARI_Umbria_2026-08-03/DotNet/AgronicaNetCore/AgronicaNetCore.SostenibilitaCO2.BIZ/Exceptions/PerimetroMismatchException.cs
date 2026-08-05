namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando un'azienda immessa nei dati di consumo
    /// non è presente nel perimetro di calcolo selezionato.
    /// Richiede refresh del perimetro o correzione dell'input.
    /// Riferimento spec: DS02-BL ValidazioneDatiConsumoAziendale — Eccezioni.
    /// </summary>
    public class PerimetroMismatchException : Exception
    {
        /// <summary>Partita IVA dell'azienda non trovata nel perimetro.</summary>
        public string Piva { get; }

        public PerimetroMismatchException(string piva)
            : base($"L'azienda con PIVA '{piva}' non è presente nel perimetro di calcolo selezionato.")
        {
            Piva = piva;
        }

        public PerimetroMismatchException(string piva, Exception innerException)
            : base($"L'azienda con PIVA '{piva}' non è presente nel perimetro di calcolo selezionato.", innerException)
        {
            Piva = piva;
        }
    }
}
