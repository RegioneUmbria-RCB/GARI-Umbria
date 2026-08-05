namespace AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel
{
    public class InvalidYearFormatException : Exception
    {
        public int Anno { get; }

        public InvalidYearFormatException(int anno)
            : base($"Il valore '{anno}' non è un anno valido nel formato YYYY (4 cifre).")
        {
            Anno = anno;
        }
    }
}
