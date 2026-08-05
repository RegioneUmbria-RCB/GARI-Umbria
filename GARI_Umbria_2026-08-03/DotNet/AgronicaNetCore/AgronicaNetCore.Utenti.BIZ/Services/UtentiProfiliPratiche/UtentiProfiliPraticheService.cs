using AgronicaCoreVisibilitaStd;
using AgronicaCoreDTOStd.InData.Pratiche;
using AgronicaCoreDTOStd.OutData.Pratiche;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Logging.BIZ.Services.AgronicaLogInvio.AgronicaLogInvioChiamate;
using AgronicaNetCore.Utenti.BIZ.Services;
using AgronicaNetCore.Utenti.BIZ.Services.VisibilitaCalcolo;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiProfili;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiProfiliPratiche;
using InData.Log.AgronicaLogInvio;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Data;

namespace AgronicaNetCore.Utenti.BIZ.Services.UtentiProfiliPratiche
{
    /// <summary>
    /// Servizio BIZ per la gestione del profilo utente con filtri pratiche.
    /// DS02-BL – GestioneProfiloUtenteFiltriPratiche.
    /// </summary>
    public class UtentiProfiliPraticheService : BaseServiceUtentiBIZ, IUtentiProfiliPraticheService
    {
        private readonly IUtentiProfili _utentiProfiliDAL;
        private readonly IUtentiProfiliPratiche _utentiProfiliPraticheDAL;
        private readonly IVisibilitaCalcoloService _visibilitaCalcoloService;
        private readonly IAgronicaLogInvioChiamateService _auditService;

        private readonly int _chunkSize = 10_000;

        public UtentiProfiliPraticheService(IServiceProvider provider, IStringLocalizer<Resources.Messages> localizer) : base(provider, localizer)
        {
            _utentiProfiliDAL = provider.GetRequiredService<IUtentiProfili>();
            _utentiProfiliPraticheDAL = provider.GetRequiredService<IUtentiProfiliPratiche>();
            _visibilitaCalcoloService = provider.GetRequiredService<IVisibilitaCalcoloService>();
            _auditService = provider.GetRequiredService<IAgronicaLogInvioChiamateService>();
        }

        /// <inheritdoc/>
        public async Task<SalvaProfiloUtenteFiltriPratiche_OUT> LeggiAsync(string idUtente, AgronicaCoreParametriDouble objParametriDouble)
        {
            // Legge FiltroPraticheAttivo e OperatoreFiltri dal profilo utente (database_utenti)
            var dtProfilo = await _utentiProfiliDAL.LeggiProfiloFiltriAsync(idUtente, objParametriDouble.ObjParametriUtenti);

            bool filtroPraticheAttivo = false;
            var operatoreFiltri = PraticheFilterOperator.Or;

            if (dtProfilo != null && dtProfilo.Rows.Count > 0)
            {
                var row = dtProfilo.Rows[0];
                filtroPraticheAttivo = row["Filtro_Pratiche_Attivo"] != DBNull.Value &&
                                       Convert.ToBoolean(row["Filtro_Pratiche_Attivo"]);
                if (row["Operatore_Filtri"] != DBNull.Value &&
                    Enum.TryParse<PraticheFilterOperator>(Convert.ToString(row["Operatore_Filtri"]), ignoreCase: true, out var op))
                {
                    operatoreFiltri = op;
                }
            }

            // Legge le pratiche filtrate (database_utenti) e le arricchisce con i dati
            // del catalogo dalla vista Servizi_Pratiche (database_utenti → MetaschemaDB)
            var dtPratiche = await _utentiProfiliPraticheDAL.LeggiAsync(idUtente, objParametriDouble.ObjParametriUtenti);
            var pratiche = new List<PraticaFiltrataDettaglio_OUT>();

            if (dtPratiche != null && dtPratiche.Rows.Count > 0)
            {
                var codici = dtPratiche.AsEnumerable()
                    .Select(r => Convert.ToInt32(r["Servizio_Cod"]))
                    .ToList();

                var dtServizi = await _utentiProfiliPraticheDAL.LeggiServiziDettaglioAsync(codici, objParametriDouble.ObjParametriUtenti);

                var serviziMap = dtServizi?.AsEnumerable()
                    .ToDictionary(r => Convert.ToInt32(r["Servizio_Cod"]))
                    ?? new Dictionary<int, DataRow>();

                pratiche = dtPratiche.AsEnumerable().Select(r =>
                {
                    var cod = Convert.ToInt32(r["Servizio_Cod"]);
                    serviziMap.TryGetValue(cod, out var sRow);
                    return new PraticaFiltrataDettaglio_OUT
                    {
                        Servizio_Cod = cod,
                        ServizioDescrizione = sRow?["Servizio_Des"] != null && sRow["Servizio_Des"] != DBNull.Value
                            ? Convert.ToString(sRow["Servizio_Des"]) ?? string.Empty
                            : string.Empty,
                        ConsideraValiditaTemporale = Convert.ToBoolean(r["ConsideraValiditaTemporale"]),
                        DataValiditaInizio = sRow?["Validita_Inizio"] != null && sRow["Validita_Inizio"] != DBNull.Value
                            ? (DateTime?)Convert.ToDateTime(sRow["Validita_Inizio"])
                            : null,
                        DataValiditaFine = sRow?["Validita_Fine"] != null && sRow["Validita_Fine"] != DBNull.Value
                            ? (DateTime?)Convert.ToDateTime(sRow["Validita_Fine"])
                            : null,
                        IsValida = sRow?["IsValida"] != null && sRow["IsValida"] != DBNull.Value
                            && Convert.ToBoolean(sRow["IsValida"])
                    };
                }).ToList();
            }

            bool visibilitaTotale = await _utentiProfiliDAL.VisibilitaTotale(
                idUtente,
                objParametriDouble.ObjParametriUtenti,
                objParametriDouble.ObjParametriServer,
                (int)AgronicaNetCore.Base.Constants.TipiEnumerativi.Enum_Id_Servizio.GiasOnline);

            return new SalvaProfiloUtenteFiltriPratiche_OUT
            {
                Success = true,
                IdUtente = idUtente,
                FiltroPraticheAttivo = filtroPraticheAttivo,
                OperatoreFiltri = operatoreFiltri.ToString(),
                NumPraticheFiltrate = pratiche.Count,
                PraticheFiltrate = pratiche,
                VisibilitaTotale = visibilitaTotale,
                TimestampModifica = DateTime.UtcNow
            };
        }

        /// <inheritdoc/>
        public async Task<SalvaProfiloUtenteFiltriPratiche_OUT> SalvaAsync(SalvaProfiloUtenteFiltriPratiche_IN input, AgronicaCoreParametriDouble objParametriDouble)
        {
            // DS02-BL – Regola 2: OperatoreFiltri default OR, validazione valori ammessi
            if (!Enum.TryParse<PraticheFilterOperator>(input.OperatoreFiltri, ignoreCase: true, out var operatoreFiltriEnum))
            {
                if (string.IsNullOrWhiteSpace(input.OperatoreFiltri))
                    operatoreFiltriEnum = PraticheFilterOperator.Or;
                else
                    throw new ArgumentException($"OperatoreFiltri non valido: '{input.OperatoreFiltri}'. Valori ammessi: AND, OR.");
            }

            // DS02-BL – Regola 8: validazione Servizio_Cod pre-transazione (database_server)
            var serviziCod = input.PraticheFiltrate?.Select(p => p.Servizio_Cod).ToList() ?? new List<int>();
            if (serviziCod.Count > 0)
            {
                bool validi = await _utentiProfiliPraticheDAL.ServiziSonoValidiAsync(serviziCod, objParametriDouble.ObjParametriServer);
                if (!validi)
                    throw new ArgumentException("Uno o più Servizio_Cod non sono validi o sono stati eliminati (Inviato=0 richiesto).");
            }

            // DS02-BL – Regola 1: verifica esistenza utente (database_utenti)
            bool esisteUtente = await _utentiProfiliDAL.EsisteUtenteAsync(input.IdUtente, objParametriDouble.ObjParametriUtenti);
            if (!esisteUtente)
                throw new KeyNotFoundException($"Utente '{input.IdUtente}' non trovato in utenti_profili per il SuperUser corrente.");

            bool filtroPraticheAttivo = input.FiltroPraticheAttivo;

            // DS02-BL – Regola 3: avviso AND con zero pratiche (non-blocking)
            var messaggiAvviso = new List<MessaggioAvviso_OUT>();
            if (operatoreFiltriEnum == PraticheFilterOperator.And && !filtroPraticheAttivo)
                messaggiAvviso.Add(new MessaggioAvviso_OUT
                {
                    Tipo = "AND_ZERO_PRATICHE",
                    Messaggio = "Attenzione: combinazione AND con zero pratiche risulterà in nessuna azienda visibile via pratiche (solo gerarchia applicherà)."
                });

            // DS02-BL – Regola 6: transazione atomica UPDATE + DELETE + INSERT (database_utenti)
            bool connectionOpened = false;
            try
            {
                await OpenConnectionAsync(objParametriDouble.ObjParametriUtenti);
                connectionOpened = true;

                // OperatoreFiltri int NOT NULL: 0 = AND, 1 = OR
                await _utentiProfiliDAL.AggiornaFiltroPraticheAsync(input.IdUtente, filtroPraticheAttivo, operatoreFiltriEnum, objParametriDouble.ObjParametriUtenti);

                await _utentiProfiliPraticheDAL.EliminaTutteAsync(input.IdUtente, objParametriDouble.ObjParametriUtenti);

                if (filtroPraticheAttivo)
                    await _utentiProfiliPraticheDAL.InserisciAsync(input.IdUtente, input.PraticheFiltrate!, objParametriDouble.ObjParametriUtenti);

                CloseTransaction(objParametriDouble.ObjParametriUtenti, Rollback: false);
            }
            catch (Exception ex)
            {
                if (connectionOpened) CloseTransaction(objParametriDouble.ObjParametriUtenti, Rollback: true);
                LogError(ex.Message, objParametriDouble.ObjParametriUtenti, ex);
                throw;
            }
            finally
            {
                if (connectionOpened) CloseConnection(objParametriDouble.ObjParametriUtenti);
            }

            bool visibilitaTotale = await _utentiProfiliDAL.VisibilitaTotale(
                input.IdUtente,
                objParametriDouble.ObjParametriUtenti,
                objParametriDouble.ObjParametriServer,
                (int)AgronicaNetCore.Base.Constants.TipiEnumerativi.Enum_Id_Servizio.GiasOnline);

            var output = new SalvaProfiloUtenteFiltriPratiche_OUT
            {
                Success = true,
                IdUtente = input.IdUtente,
                FiltroPraticheAttivo = filtroPraticheAttivo,
                OperatoreFiltri = operatoreFiltriEnum.ToString(),
                NumPraticheFiltrate = input.PraticheFiltrate?.Count ?? 0,
                MessaggiAvviso = messaggiAvviso,
                // PraticheFiltrate non restituita nel PATCH (solo nella GET)
                PraticheFiltrate = new List<PraticaFiltrataDettaglio_OUT>(),
                VisibilitaTotale = visibilitaTotale,
                TimestampModifica = DateTime.UtcNow
            };

            // DS02-BL – Regola 7: audit trail non-bloccante
            _ = ScriviAuditAsync(input, output, objParametriDouble.ObjParametriServer);

            return output;
        }

        /// <inheritdoc/>
        public async Task<PreviewProfiloUtenteFiltriPratiche_OUT> PreviewAsync(string idUtente, SalvaProfiloUtenteFiltriPratiche_IN input, AgronicaCoreParametriDouble objParametriDouble)
        {
            var operatore = string.IsNullOrWhiteSpace(input.OperatoreFiltri) ? "OR" : input.OperatoreFiltri;
            if (!Enum.TryParse<PraticheFilterOperator>(operatore, ignoreCase: true, out _))
                throw new ArgumentException($"OperatoreFiltri non valido: '{operatore}'. Valori ammessi: AND, OR.");

            var dtProfilo = await _utentiProfiliDAL.LeggiProfiloFiltriAsync(idUtente, objParametriDouble.ObjParametriUtenti);
            if (dtProfilo == null || dtProfilo.Rows.Count == 0)
                throw new KeyNotFoundException($"Utente '{idUtente}' non trovato in utenti_profili per il SuperUser corrente.");

            var descrizione1 = dtProfilo.Columns.Contains("Descrizione_1") && dtProfilo.Rows[0]["Descrizione_1"] != DBNull.Value
                ? Convert.ToString(dtProfilo.Rows[0]["Descrizione_1"]) ?? string.Empty
                : string.Empty;

            var descrizione2 = dtProfilo.Rows[0]["Descrizione_2"] != DBNull.Value
                ? Convert.ToString(dtProfilo.Rows[0]["Descrizione_2"]) ?? string.Empty
                : string.Empty;

            var pratiche = input.PraticheFiltrate ?? new List<PraticaFiltrata_IN>();
            var filtroPraticheAttivo = pratiche.Count > 0;
            var messaggiAvviso = new List<MessaggioAvviso_OUT>();

            if (string.Equals(operatore, "AND", StringComparison.OrdinalIgnoreCase) && !filtroPraticheAttivo)
                messaggiAvviso.Add(new MessaggioAvviso_OUT
                {
                    Tipo = "AND_ZERO_PRATICHE",
                    Messaggio = "Attenzione: combinazione AND con zero pratiche risulterà in nessuna azienda visibile via pratiche."
                });

            var visibilita = await _visibilitaCalcoloService.CalcolaPreviewAsync(
                idUtente,
                descrizione1,
                descrizione2,
                filtroPraticheAttivo,
                operatore,
                pratiche,
                objParametriDouble);

            return new PreviewProfiloUtenteFiltriPratiche_OUT
            {
                Success = true,
                IdUtente = idUtente,
                OperatoreFiltri = operatore,
                NumPraticheFiltrate = pratiche.Count,
                NumAziendeVisibili = visibilita.NumPivaVisibili,
                NumAziendeTotali = visibilita.NumPivaTotali,
                VisibilitaTotale = visibilita.VisibilitaTotale,
                MessaggiAvviso = messaggiAvviso,
                TimestampPreview = visibilita.TimestampCalcolo
            };
        }

        private async Task ScriviAuditAsync(SalvaProfiloUtenteFiltriPratiche_IN input, SalvaProfiloUtenteFiltriPratiche_OUT output, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                var auditEntry = new WriteAgronicaLogInvioChiamate
                {
                    Tipo_Operazione = "MODIFICA_FILTRI_PRATICHE",
                    Dati_Inviati = JsonConvert.SerializeObject(input),
                    Data_Invio = output.TimestampModifica,
                    Esito = "OK",
                    Dati_Ricevuti = JsonConvert.SerializeObject(output),
                    Dettaglio1 = input.IdUtente,
                    Dettaglio2 = input.OperatoreRichiesta,
                    Dettaglio3 = "source=API"
                };

                await _auditService.WriteAsync(auditEntry, objParametriServer);
                output.AuditId = auditEntry.ID.ToString();
            }
            catch (Exception ex)
            {
                // DS02-BL – Regola 7: AuditLogException non-blocking, logga warning
                LogWarning(ex.Message, objParametriServer);
            }
        }

        /// <inheritdoc/>
        public async Task<ConfigurazionePraticheUtente> LeggiConfigurazioneAsync(string username, AgronicaCoreParametriDouble objParametriDouble)
        {
            var dtProfilo = await _utentiProfiliDAL.LeggiProfiloFiltriAsync(username, objParametriDouble.ObjParametriUtenti);

            bool filtroPraticheAttivo = false;
            var operatoreFiltri = PraticheFilterOperator.Or;

            if (dtProfilo != null && dtProfilo.Rows.Count > 0)
            {
                var row = dtProfilo.Rows[0];
                filtroPraticheAttivo = row["Filtro_Pratiche_Attivo"] != DBNull.Value &&
                                       Convert.ToBoolean(row["Filtro_Pratiche_Attivo"]);
                if (row["Operatore_Filtri"] != DBNull.Value &&
                    Enum.TryParse<PraticheFilterOperator>(Convert.ToString(row["Operatore_Filtri"]), ignoreCase: true, out var op))
                {
                    operatoreFiltri = op;
                }
            }

            var dtPratiche = await _utentiProfiliPraticheDAL.LeggiAsync(username, objParametriDouble.ObjParametriUtenti);
            var pratiche = new List<PraticaConfigurataUtente>();

            if (dtPratiche != null && dtPratiche.Rows.Count > 0)
            {
                var codici = dtPratiche.AsEnumerable()
                    .Select(r => Convert.ToInt32(r["Servizio_Cod"]))
                    .ToList();

                var dtServizi = await _utentiProfiliPraticheDAL.LeggiServiziDettaglioAsync(codici, objParametriDouble.ObjParametriUtenti);

                var serviziMap = dtServizi?.AsEnumerable()
                    .ToDictionary(r => Convert.ToInt32(r["Servizio_Cod"]))
                    ?? new Dictionary<int, DataRow>();

                pratiche = dtPratiche.AsEnumerable().Select(r =>
                {
                    var cod = Convert.ToInt32(r["Servizio_Cod"]);
                    serviziMap.TryGetValue(cod, out var sRow);
                    return new PraticaConfigurataUtente
                    {
                        Servizio_Cod = cod,
                        ServizioDescrizione = sRow?["Servizio_Des"] != null && sRow["Servizio_Des"] != DBNull.Value
                            ? Convert.ToString(sRow["Servizio_Des"]) ?? string.Empty
                            : string.Empty,
                        ConsideraValiditaTemporale = Convert.ToBoolean(r["Considera_Validita_Temporale"]),
                        DataValiditaInizio = sRow?["Validita_Inizio"] != null && sRow["Validita_Inizio"] != DBNull.Value
                            ? (DateTime?)Convert.ToDateTime(sRow["Validita_Inizio"])
                            : null,
                        DataValiditaFine = sRow?["Validita_Fine"] != null && sRow["Validita_Fine"] != DBNull.Value
                            ? (DateTime?)Convert.ToDateTime(sRow["Validita_Fine"])
                            : null,
                        IsValida = sRow?["IsValida"] != null && sRow["IsValida"] != DBNull.Value
                            && Convert.ToBoolean(sRow["IsValida"]),
                        Selected = true
                    };
                }).ToList();
            }

            return new ConfigurazionePraticheUtente
            {
                Username = username,
                OperatoreFiltri = operatoreFiltri.ToString(),
                FiltroPraticheAttivo = filtroPraticheAttivo,
                Pratiche = pratiche
            };
        }

        public Task<SalvaProfiloUtenteFiltriPratiche_OUT> SalvaConfigurazioneAsync(ConfigurazionePraticheUtente input, AgronicaCoreParametriDouble objParametriDouble)
        {
            var mapped = new SalvaProfiloUtenteFiltriPratiche_IN
            {
                IdUtente = input.Username,
                FiltroPraticheAttivo = input.FiltroPraticheAttivo,
                OperatoreFiltri = input.OperatoreFiltri,
                PraticheFiltrate = input.Pratiche
                    .Select(p => new PraticaFiltrata_IN { Servizio_Cod = p.Servizio_Cod, ConsideraValiditaTemporale = p.ConsideraValiditaTemporale })
                    .ToList(),
                OperatoreRichiesta = objParametriDouble.ObjParametriUtenti.UtenteUsername
            };

            return SalvaAsync(mapped, objParametriDouble);
        }

        public async Task CopiaVisibilita(string template, IEnumerable<string> targets, bool copyHierarchy, bool copyProcedures, AgronicaCoreParametriDouble objParametriDouble)
        {
            await VerifyParamsForCopy(template, targets, copyHierarchy, copyProcedures, objParametriDouble);

            var innerTargets = targets.Distinct().ToHashSet();
            innerTargets.Remove(template);
            var batches = innerTargets.Chunk(_chunkSize);

            try
            {
                await OpenConnectionAsync(objParametriDouble.ObjParametriUtenti);

                foreach (var batch in batches)
                {
                    await _utentiProfiliDAL.CopyAsync(template, batch, copyHierarchy, copyProcedures, objParametriDouble.ObjParametriUtenti);
                    if (copyProcedures)
                    {
                        await _utentiProfiliPraticheDAL.CopiaAsync(template, batch, objParametriDouble.ObjParametriUtenti);
                    }
                }

                CloseConnection(objParametriDouble.ObjParametriUtenti);
            }
            catch (Exception ex)
            {
                CloseTransaction(objParametriDouble.ObjParametriUtenti, true);
                LogError(ex.Message, objParametriDouble.ObjParametriUtenti, ex);
                throw;
            }
        }

        private async Task VerifyParamsForCopy(string template, IEnumerable<string> targets, bool copyHierarchy, bool copyPratiche, AgronicaCoreParametriDouble objParametriDouble)
        {
            if (!copyHierarchy && !copyPratiche)
                throw new ArgumentException("At least one of copyHierarchy or copyProcedures must be true and template must be provided.");

            var verified = await _utentiProfiliDAL.EsisteUtenteAsync(template, objParametriDouble.ObjParametriUtenti);
            if (!verified)
                throw new ArgumentOutOfRangeException(template, "Template user does not exist.");

            verified = await _utentiProfiliDAL.EsisteUtenteAsync(targets, objParametriDouble.ObjParametriUtenti);
            if (!verified)
                throw new ArgumentOutOfRangeException("targets", "One or more target users do not exist.");
        }
    }
}
