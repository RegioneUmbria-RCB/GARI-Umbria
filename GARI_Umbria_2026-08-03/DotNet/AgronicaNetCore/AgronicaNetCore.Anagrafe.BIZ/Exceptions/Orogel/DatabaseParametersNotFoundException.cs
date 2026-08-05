namespace AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel
{
    public class DatabaseParametersNotFoundException : Exception
    {
        public int IdDbServer { get; }

        public DatabaseParametersNotFoundException(int idDbServer)
            : base($"Impossibile recuperare i parametri di connessione per il database con id '{idDbServer}'.")
        {
            IdDbServer = idDbServer;
        }
    }
}
