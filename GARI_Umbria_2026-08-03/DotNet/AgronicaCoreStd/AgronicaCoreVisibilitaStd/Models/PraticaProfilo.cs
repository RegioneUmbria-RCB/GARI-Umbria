namespace AgronicaCoreVisibilitaStd.Models
{
    public sealed class PraticaProfilo
    {
        public PraticaProfilo(int servizioCod, bool consideraValiditaTemporale)
        {
            Servizio_Cod = servizioCod;
            ConsideraValiditaTemporale = consideraValiditaTemporale;
        }

        public int Servizio_Cod { get; }

        public bool ConsideraValiditaTemporale { get; }
    }
}
