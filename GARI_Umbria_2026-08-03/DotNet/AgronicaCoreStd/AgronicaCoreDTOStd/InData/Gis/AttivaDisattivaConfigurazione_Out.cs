namespace InData.Gis
{
    public class AttivaDisattivaConfigurazione_Out
    {
        public AttivaDisattivaConfigurazioneEnum Risultato { get; set; }
    }

    public enum AttivaDisattivaConfigurazioneEnum
    {
        AttivatoCorrettamente = 0,
        GiaAttivato = 1,
        ErroreAttivazione = 2,
        NonAutorizzato = 3,
    }
}
