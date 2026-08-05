namespace AgronicaNetCore.Base.Models
{
    public class FmisContextDataBase
    {
        public int IdDb { get; set; }
        public int IdTestata { get; set; }
        public WebhookTipo Tipo { get; set; }

        public FmisContextDataBase(int idDb, int idTestata, WebhookTipo tipo = WebhookTipo.Base)
        {
            IdDb = idDb;
            IdTestata = idTestata;
            Tipo = tipo;
        }
    }

    public enum WebhookTipo : short
    {
        Base = 1,
        EsitoConsiglioNutrizione = 2
    }
}
