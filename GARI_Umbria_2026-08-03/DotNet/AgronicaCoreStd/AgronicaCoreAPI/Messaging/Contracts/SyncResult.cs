namespace AgronicaCoreAPI.Messaging.Contracts
{
    /// <summary>Risultato dell'elaborazione di un messaggio RabbitMQ da parte di un handler.</summary>
    public class SyncResult
    {
        public bool Success { get; set; }
        public string Guid { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }

        public static SyncResult Ok(string guid, string message = null) =>
            new SyncResult { Success = true, Guid = guid, Message = message };

        public static SyncResult Fail(string errorMessage) =>
            new SyncResult { Success = false, ErrorMessage = errorMessage };
    }
}
