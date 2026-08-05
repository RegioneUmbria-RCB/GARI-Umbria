using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.Identity
{
    public sealed class TokenUserPwdRequest
    {
        /// <summary>
        /// URL assoluto dell'endpoint token (es. .../protocol/openid-connect/token).
        /// </summary>
        public Uri TokenEndpoint { get; set; }

        /// <summary>
        /// client_id obbligatorio per la maggior parte dei provider.
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// client_secret opzionale (dipende dalla configurazione del client).
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// username dell'utente.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// password dell'utente.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Scope opzionale, spazio-separato (es. "openid profile api.read").
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Audience/Resource opzionale (per provider che la supportano).
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Tipo di grant (es. "password", "client_credentials").
        /// </summary>
        public string GrantType { get; set; } = "password";  // Default è "password"

        /// <summary>
        /// Se true, invia client_id/client_secret in Authorization: Basic anziché nel form.
        /// Default: false.
        /// </summary>
        public bool UseBasicAuthForClient { get; set; } = false;

        /// <summary>
        /// Header HTTP aggiuntivi da inviare (es. User-Agent personalizzato).
        /// </summary>
        public IReadOnlyDictionary<string, string> ExtraHeaders { get; set; }

        /// <summary>
        /// Campi form aggiuntivi da inviare (in aggiunta a grant_type, username, password, client_id, scope...).
        /// </summary>
        public IReadOnlyDictionary<string, string> ExtraFormFields { get; set; }
    }
}
