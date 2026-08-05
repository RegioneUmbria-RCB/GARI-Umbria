using InData.Zoo.DataMars;
using OutData.Zoo.DataMars;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.ValidazionePayload;

/// <summary>
/// Implementazione della validazione del payload JSON Datamars.
/// Classe stateless, pura logica di business senza accesso a DB o HTTP.
/// <para>
/// Regole applicate:
/// <list type="bullet">
///   <item>Array pesate non vuoto</item>
///   <item>LID obbligatorio (non-null, non-empty)</item>
///   <item>Peso &gt; 0</item>
///   <item>Timestamp ISO 8601 UTC, non nel futuro</item>
///   <item>Deduplicazione: per LID + giorno, mantieni solo il timestamp più recente</item>
/// </list>
/// </para>
/// <para>Riferimento spec: DS09-BL ValidazionePayloadJsonDatamars — Regole di Business.</para>
/// </summary>
public sealed class ValidazionePayloadJsonDatamarsService : IValidazionePayloadJsonDatamarsService
{
    /// <inheritdoc/>
    public ValidazionePayloadResult ValidaPayload(DatamarsSessioneDettaglio sessioneDettaglio, string idSessione)
    {
        if (sessioneDettaglio.SessionAnimals.Count == 0)
        {
            return new ValidazionePayloadResult
            {
                ValidazioneOK = false,
                StatusValidazione = FlagValidazione.ErroreTotale,
                ErrorMessage = "L'array sessionAnimals è vuoto: nessuna pesata da acquisire."
            };
        }

        var pesateValide = new List<DatamarsAnimalePesata>();
        var warnings = new List<ValidazioneWarning>();
        var now = DateTime.UtcNow;
        var erroriCount = 0;

        // Prima passata: valida ogni singola pesata
        for (var i = 0; i < sessioneDettaglio.SessionAnimals.Count; i++)
        {
            var pesata = sessioneDettaglio.SessionAnimals[i];

            // Validazione LID obbligatorio
            if (string.IsNullOrWhiteSpace(pesata.Animal?.LifetimeIdentifierTag?.Value))
            {
                erroriCount++;
                continue; // scarta pesata, non aggiungere né a valide né a warnings
            }

            // Validazione peso > 0
            if (pesata.Weight <= 0)
            {
                erroriCount++;
                continue;
            }

            // Validazione timestamp ISO 8601 e non nel futuro
            if (!DateTime.TryParse(pesata.Timestamp, null, System.Globalization.DateTimeStyles.RoundtripKind, out var ts)
                || ts.Kind == DateTimeKind.Unspecified)
            {
                erroriCount++;
                continue;
            }

            var tsUtc = ts.Kind == DateTimeKind.Local ? ts.ToUniversalTime() : ts;
            if (tsUtc > now)
            {
                erroriCount++;
                continue;
            }

            // Warning non bloccante: EID mancante
            if (string.IsNullOrWhiteSpace(pesata.Animal?.ElectronicIdentifierTag?.Value))
            {
                warnings.Add(new ValidazioneWarning
                {
                    PesataIndex = i,
                    Warning = "EID non valorizzato, sarà usato solo LID."
                });
            }

            pesateValide.Add(pesata);
        }

        // Seconda passata: deduplica per LID + data (mantieni timestamp più recente)
        pesateValide = DeduplicaPerLidEGiorno(pesateValide);

        if (pesateValide.Count == 0)
        {
            return new ValidazionePayloadResult
            {
                ValidazioneOK = false,
                StatusValidazione = FlagValidazione.ErroreTotale,
                NumPesateValidate = 0,
                NumPesateScartate = erroriCount,
                WarningCount = warnings.Count,
                WarningDettagli = warnings,
                ErrorMessage = "Nessuna pesata valida dopo validazione e deduplicazione."
            };
        }

        var status = erroriCount > 0 ? FlagValidazione.ErroriParziali : FlagValidazione.Valida;

        return new ValidazionePayloadResult
        {
            ValidazioneOK = true,
            StatusValidazione = status,
            NumPesateValidate = pesateValide.Count,
            NumPesateScartate = erroriCount,
            PesateValide = pesateValide,
            WarningCount = warnings.Count,
            WarningDettagli = warnings
        };
    }

    /// <summary>
    /// Deduplica la lista di pesate valide: per ogni combinazione LID + data UTC,
    /// mantiene solo la pesata con il timestamp più recente.
    /// <para>Riferimento spec: DS01-BL — Regole di Business "Una pesata per capo per giorno".</para>
    /// </summary>
    private static List<DatamarsAnimalePesata> DeduplicaPerLidEGiorno(List<DatamarsAnimalePesata> pesate)
    {
        return pesate
            .GroupBy(p => (
                Lid: p.Animal!.LifetimeIdentifierTag!.Value,
                Giorno: DateTime.Parse(p.Timestamp, null, System.Globalization.DateTimeStyles.RoundtripKind)
                               .ToUniversalTime()
                               .Date
            ))
            .Select(g => g.OrderByDescending(p =>
                DateTime.Parse(p.Timestamp, null, System.Globalization.DateTimeStyles.RoundtripKind)
                        .ToUniversalTime())
                .First())
            .ToList();
    }

    private static class FlagValidazione
    {
        public const string Valida = "VALIDA";
        public const string ErroriParziali = "ERRORI_PARZIALI";
        public const string ErroreTotale = "ERRORE_TOTALE";
    }
}
