using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Models;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Resources;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.TipiCarburante;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.ConsumoAziendale
{
    /// <summary>
    /// Implementazione della business logic <c>ValidazioneDatiConsumoAziendale</c> (DS02-BL).
    /// Esegue validazione server-side dei dati di consumo aziendale (carburanti ed energia)
    /// rispetto al perimetro selezionato e alle regole di business definite nella spec.
    /// Riferimento spec: DS02-BL ValidazioneDatiConsumoAziendale.
    /// </summary>
    public class ValidazioneConsumiAziendaliService : BaseServiceSostenibilitaCO2Biz, IValidazioneConsumiAziendaliService
    {
        private readonly ITipiCarburanteDAL _tipiCarburanteDAL;

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="ValidazioneConsumiAziendaliService"/>.
        /// </summary>
        public ValidazioneConsumiAziendaliService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _tipiCarburanteDAL = _serviceProvider.GetRequiredService<ITipiCarburanteDAL>();
        }

        /// <inheritdoc/>
        public async Task<ValidazioneConsumiAziendaliResult> ValidaAsync(ValidazioneConsumiAziendaliRequest request, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            ArgumentNullException.ThrowIfNull(request);

            // Regola 7: entrambe le tabelle vuote → scenario zero-consumo valido
            if (request.Carburanti.Count == 0 && request.Energia.Count == 0)
            {
                return new ValidazioneConsumiAziendaliResult
                {
                    ValidazioneEsito = true,
                    ConsumiValidati = new ConsumiValidati()
                };
            }

            // Perimetro aziende come HashSet per lookup O(1), case-insensitive
            var perimetroSet = new HashSet<string>(
                request.PerimetroAziende ?? Enumerable.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);

            // Tipi carburante validi da DB
            var tipiCarburanteEntities = await _tipiCarburanteDAL.GetAllAsync(objParametriServer);
            var tipiCarburanteValidi = new HashSet<string>(tipiCarburanteEntities.Select(t => t.Car_Des), StringComparer.OrdinalIgnoreCase);

            var errori = new List<ErroreValidazione>();

            ValidaCarburanti(request.Carburanti, perimetroSet, tipiCarburanteValidi, errori);
            ValidaEnergia(request.Energia, perimetroSet, errori);

            if (errori.Count > 0)
            {
                return new ValidazioneConsumiAziendaliResult
                {
                    ValidazioneEsito = false,
                    Errori = errori
                };
            }

            // Regola 6: sovrapposizioni temporali per stessa azienda → somma diretta, nessun errore.
            // I dati vengono passati invariati al DS03-BL, che applicherà la somma durante l'assembly.
            return new ValidazioneConsumiAziendaliResult
            {
                ValidazioneEsito = true,
                ConsumiValidati = new ConsumiValidati
                {
                    Carburanti = request.Carburanti,
                    Energia = request.Energia
                }
            };
        }

        // ─── Validazione tabella Carburanti ─────────────────────────────────────────

        private static void ValidaCarburanti(List<CarburanteConsumo> righe,HashSet<string> perimetroSet,HashSet<string> tipiValidi,List<ErroreValidazione> errori)
        {
            for (int i = 0; i < righe.Count; i++)
            {
                var row = righe[i];

                // Regola 1: azienda obbligatoria e nel perimetro
                if (string.IsNullOrWhiteSpace(row.Azienda))
                {
                    errori.Add(Errore("FIELD_REQUIRED", "carburanti", i, "azienda",
                        "Il campo Azienda è obbligatorio.", null));
                }
                else if (!perimetroSet.Contains(row.Azienda))
                {
                    errori.Add(Errore("AZIENDA_NOT_IN_PERIMETRO", "carburanti", i, "azienda",
                        $"L'azienda '{row.Azienda}' non è presente nel perimetro di calcolo.", row.Azienda));
                }

                // Tipo carburante obbligatorio e valido
                if (string.IsNullOrWhiteSpace(row.TipoCarburante))
                {
                    errori.Add(Errore("FIELD_REQUIRED", "carburanti", i, "tipo_carburante",
                        "Il campo Tipo Carburante è obbligatorio.", null));
                }
                else if (tipiValidi.Count > 0 && !tipiValidi.Contains(row.TipoCarburante))
                {
                    errori.Add(Errore("INVALID_FORMAT", "carburanti", i, "tipo_carburante",
                        $"Il tipo carburante '{row.TipoCarburante}' non è valido.", row.TipoCarburante));
                }

                // Regola 2: quantità > 0 (valore = 0 rifiutato)
                if (row.Quantita <= 0)
                {
                    errori.Add(Errore("OUT_OF_RANGE", "carburanti", i, "quantita",
                        "La quantità deve essere maggiore di zero.", row.Quantita.ToString()));
                }

                // Unità di misura obbligatoria (default 'litri' già nel modello)
                if (string.IsNullOrWhiteSpace(row.UnitaMisura))
                {
                    errori.Add(Errore("FIELD_REQUIRED", "carburanti", i, "unita_misura",
                        "Il campo Unità di Misura è obbligatorio.", null));
                }
            }
        }

        // ─── Validazione tabella Energia ────────────────────────────────────────────

        private static void ValidaEnergia(List<EnergiaConsumo> righe,HashSet<string> perimetroSet,List<ErroreValidazione> errori)
        {
            for (int i = 0; i < righe.Count; i++)
            {
                var row = righe[i];

                // Regola 1: azienda obbligatoria e nel perimetro
                if (string.IsNullOrWhiteSpace(row.Azienda))
                {
                    errori.Add(Errore("FIELD_REQUIRED", "energia", i, "azienda","Il campo Azienda è obbligatorio.", null));
                }
                else if (!perimetroSet.Contains(row.Azienda))
                {
                    errori.Add(Errore("AZIENDA_NOT_IN_PERIMETRO", "energia", i, "azienda", $"L'azienda '{row.Azienda}' non è presente nel perimetro di calcolo.", row.Azienda));
                }

                // Regola 5 (prerequisito): data_inizio e data_fine obbligatorie
                if (row.DataInizio == DateTime.MinValue)
                {
                    errori.Add(Errore("FIELD_REQUIRED", "energia", i, "data_inizio", "Il campo Data Inizio è obbligatorio.", null));
                }

                if (row.DataFine == DateTime.MinValue)
                {
                    errori.Add(Errore("FIELD_REQUIRED", "energia", i, "data_fine","Il campo Data Fine è obbligatorio.", null));
                }

                // Regola 5: data_fine >= data_inizio (solo se entrambe le date sono presenti)
                if (row.DataInizio != DateTime.MinValue && row.DataFine != DateTime.MinValue && row.DataFine < row.DataInizio)
                {
                    errori.Add(Errore("INVALID_FORMAT", "energia", i, "data_fine", "La data fine deve essere uguale o successiva alla data inizio.", row.DataFine.ToString("yyyy-MM-dd")));
                }

                // Regola 3: consumo KWh > 0 (valore = 0 rifiutato)
                if (row.ConsumoKwh <= 0)
                {
                    errori.Add(Errore("OUT_OF_RANGE", "energia", i, "consumo_kwh","Il consumo KWh deve essere maggiore di zero.", row.ConsumoKwh.ToString()));
                }

                // Regola 4: % rinnovabili 0-100 inclusi, obbligatorio
                if (row.PercentualeRinnovabili < 0 || row.PercentualeRinnovabili > 100)
                {
                    errori.Add(Errore("OUT_OF_RANGE", "energia", i, "percentuale_rinnovabili","La percentuale di energie rinnovabili deve essere compresa tra 0 e 100.",row.PercentualeRinnovabili.ToString()));
                }
            }
        }

        // ─── Factory helper ─────────────────────────────────────────────────────────

        private static ErroreValidazione Errore(string tipo, string tabella, int riga, string campo, string messaggio, string? valoreAttuale) 
            => new()
            {
                Tipo = tipo,
                Tabella = tabella,
                Riga = riga,
                Campo = campo,
                Messaggio = messaggio,
                ValoreAttuale = valoreAttuale
            };
    }
}
