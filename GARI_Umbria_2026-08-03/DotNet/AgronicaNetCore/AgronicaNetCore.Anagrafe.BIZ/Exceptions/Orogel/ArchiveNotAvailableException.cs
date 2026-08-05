namespace AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel
{
    public class ArchiveNotAvailableException : Exception
    {
        public int Anno { get; }
        public IReadOnlyList<string> SupportedYears { get; }

        public ArchiveNotAvailableException(int anno, IEnumerable<string> supportedYears)
            : base($"Nessun archivio disponibile per l'anno '{anno}'. Anni supportati: {string.Join(", ", supportedYears)}.")
        {
            Anno = anno;
            SupportedYears = supportedYears.ToList().AsReadOnly();
        }
    }
}
