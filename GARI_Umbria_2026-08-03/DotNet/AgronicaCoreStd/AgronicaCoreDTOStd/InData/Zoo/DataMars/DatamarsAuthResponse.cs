using Newtonsoft.Json;

namespace InData.Zoo.DataMars
{
    /// <summary>
    /// Rappresenta la risposta dell'endpoint OAuth2 di Datamars.
    /// <para>
    /// Endpoint: <c>POST https://test-account.livestock.datamars.com/oauth2/token</c>
    /// </para>
    /// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI â€” Autenticazione DataMars API.</para>
    /// </summary>
    public sealed class DatamarsAuthResponse
    {
        /// <summary>Bearer token da usare nelle successive chiamate API Datamars.</summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>Durata di validitÃ  del token in secondi (tipicamente 3599).</summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>Tipo di token (tipicamente "Bearer").</summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; } = string.Empty;
    }
}
