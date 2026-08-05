using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace OutData.Authentication
{

    public sealed class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("token_type")]
        public string TokenType { get; set; } = "Bearer";

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_expires_in")]
        public int RefreshExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("session_state")]
        public string SessionState { get; set; }

        // Nome JSON con trattini -> serve JsonProperty
        [JsonProperty("not-before-policy")]
        public int NotBeforePolicy { get; set; }

        /// <summary>
        /// Campi non mappati (se il provider ne aggiunge).
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> Extra { get; set; }

        /// <summary>
        /// Comodità per l'header Authorization.
        /// </summary>
        [JsonIgnore]
        public string Authorization => string.IsNullOrEmpty(AccessToken)
            ? string.Empty
            : $"{TokenType} {AccessToken}";


        /// <summary>
        /// Quando scade il token (in base a expires_in).
        /// </summary>
        public DateTimeOffset ExpiresAt => DateTimeOffset.UtcNow.AddSeconds(ExpiresIn);

        public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
    }
}
