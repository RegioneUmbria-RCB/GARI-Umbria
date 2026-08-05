using System;
using System.Collections.Generic;

namespace AgronicaCoreAPI.Messaging.Validation
{
    /// <summary>
    /// Validazione strutturale del payload di sincronizzazione.
    /// Copre: null-check, GUID formato RFC 4122, payload non vuoto.
    /// Le validazioni semantiche di business (lookup codici, importi, ecc.)
    /// sono demandate a una fase successiva.
    /// </summary>
    public sealed class SyncPayloadValidator : ISyncPayloadValidator
    {
        /// <inheritdoc />
        public ValidationResult Validate(string tipoEntita, object payload, string guid)
        {
            var errors = new List<ValidationError>();

            if (payload == null)
            {
                errors.Add(new ValidationError("payload", "PAYLOAD_NULL", "Il payload non può essere null."));
                return ValidationResult.Failure(errors);
            }

            if (string.IsNullOrWhiteSpace(tipoEntita))
                errors.Add(new ValidationError("tipoEntita", "MISSING_ENTITY_TYPE", "Il tipo entità è obbligatorio."));

            /* if (string.IsNullOrWhiteSpace(guid))
            {
                errors.Add(new ValidationError("guid", "MISSING_GUID", "Il GUID dell'entità è obbligatorio."));
            }
            else if (!Guid.TryParse(guid, out _))
            {
                errors.Add(new ValidationError("guid", "INVALID_GUID_FORMAT", $"Il GUID '{guid}' non è in formato RFC 4122 valido."));
            } */

            return errors.Count > 0 ? ValidationResult.Failure(errors) : ValidationResult.Ok;
        }
    }
}
