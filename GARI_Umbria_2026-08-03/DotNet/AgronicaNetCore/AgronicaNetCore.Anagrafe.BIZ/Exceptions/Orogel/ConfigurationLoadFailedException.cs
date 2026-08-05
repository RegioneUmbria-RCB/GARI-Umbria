namespace AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel
{
    public class ConfigurationLoadFailedException : Exception
    {
        public ConfigurationLoadFailedException(string message)
            : base(message) { }

        public ConfigurationLoadFailedException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
