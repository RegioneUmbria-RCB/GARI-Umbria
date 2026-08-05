namespace AgronicaCoreDTOStd.InData.Provisioning
{
    /// <summary>
    /// Parametri per l'inserimento di un nuovo token utente nelle tabelle super_server
    /// </summary>
    public class CreaToken_In
    {
        /// <summary>
        /// ID del DB utenti
        /// </summary>
        /// <example>abc123</example>
        public int DB_Utenti { get; set; }

        /// <summary>
        /// ID del DB server
        /// </summary>
        /// <example>abc123</example>
        public int DB_Server { get; set; }


    }
}
