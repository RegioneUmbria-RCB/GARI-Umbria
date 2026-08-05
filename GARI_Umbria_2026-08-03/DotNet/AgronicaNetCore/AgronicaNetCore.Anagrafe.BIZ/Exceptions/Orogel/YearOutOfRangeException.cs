namespace AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel
{
    public class YearOutOfRangeException : Exception
    {
        public int Anno { get; }
        public int CurrentYear { get; }

        public YearOutOfRangeException(int anno, int currentYear)
            : base($"L'anno '{anno}' non è nel range valido [2000, {currentYear}].")
        {
            Anno = anno;
            CurrentYear = currentYear;
        }
    }
}
