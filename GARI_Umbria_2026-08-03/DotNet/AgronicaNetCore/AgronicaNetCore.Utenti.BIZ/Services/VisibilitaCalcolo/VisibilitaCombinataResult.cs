namespace AgronicaNetCore.Utenti.BIZ.Services.VisibilitaCalcolo
{
    public class VisibilitaCombinataResult
    {
        public int NumPivaVisibili { get; set; }

        public int NumCentriVisibili { get; set; }

        public int NumPivaTotali { get; set; }

        public bool VisibilitaTotale { get; set; }

        public DateTime TimestampCalcolo { get; set; }

        public bool PersistitoSuUVA { get; set; }
    }
}
