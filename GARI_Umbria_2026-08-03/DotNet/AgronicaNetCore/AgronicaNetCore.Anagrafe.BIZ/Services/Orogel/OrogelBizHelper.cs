namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel
{
    internal static class OrogelBizHelper
    {
        internal static string? NullIfEmpty(string? value) =>
            string.IsNullOrEmpty(value) ? null : value;

        internal static bool IsConnectionError(Exception ex)
        {
            string msg = ex.Message;
            return msg.Contains("connection", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("connessione", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("login", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("network", StringComparison.OrdinalIgnoreCase);
        }
    }
}
