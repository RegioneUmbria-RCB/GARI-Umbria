namespace AgronicaNetCore.Utenti.BIZ.Services.NotificaScadenzaPassword
{
    public class NotificheScadenzaPasswordResult
    {
        public int EmailInviate { get; set; }
        public int EmailFallite { get; set; }
        public List<string> LogMessages { get; set; } = new();
    }
}
