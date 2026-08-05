namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando entrambe le coordinate (latitudine e longitudine)
    /// risultano uguali a 0 per un'azienda, indicando che nessuna riga è stata trovata
    /// in <c>Imprese_Codici</c> per gli <c>id_cod</c> 1105 e 1106.
    /// Comportamento non bloccante: registrare warning log e continuare con <c>POINT(0 0)</c>.
    /// Riferimento spec: DS03-BL AssemblyPayloadM4FilieraAzienda — Eccezioni.
    /// </summary>
    public class CentroideInvalidoException : Exception
    {
        public CentroideInvalidoException(string piva)
            : base($"Centroide non disponibile per PIVA '{piva}': coordinate (0, 0). Usato POINT(0 0).")
        {
        }

        public CentroideInvalidoException(string piva, Exception innerException)
            : base($"Centroide non disponibile per PIVA '{piva}': coordinate (0, 0). Usato POINT(0 0).", innerException)
        {
        }
    }
}
