using AgronicaNetCore.Anagrafe.DAL.DataLayer.Esercizi;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.GerarchiaImprese;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Indirizzi;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.RischiMeteo.BIZ.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.RischiMeteo.BIZ.Services.PerimetroRaccolti
{
    /// <summary>
    /// Recupera il perimetro di calcolo aperto per una filiera:
    /// espande le PIVA figlie, interroga il DAL con una singola query e mappa il risultato.
    /// Riferimento spec: DS01-BL CaricamentoPerimetroFiltrato.
    /// </summary>
    public class PerimetroRaccoltiRischiService : BaseServiceRischiMeteoBiz, IPerimetroRaccoltiRischiService
    {
        private readonly IGerarchiaImprese _gerarchiaImprese;
        private readonly IEsercizi _esercizi;
        private readonly IIndirizzi _indirizzi;

        public PerimetroRaccoltiRischiService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _gerarchiaImprese = _serviceProvider.GetRequiredService<IGerarchiaImprese>();
            _esercizi = _serviceProvider.GetRequiredService<IEsercizi>();
            _indirizzi = _serviceProvider.GetRequiredService<IIndirizzi>();
        }

        /// <inheritdoc/>
        public async Task<PerimetroRaccoltiResult> GetPerimetroAsync(PerimetroRaccoltiRequest request, AgronicaCoreParametriServer objParametriServer)
        {
            // STEP 1: espandi le PIVA figlie della filiera (Cono di Visibilità)
            var pivas = await _gerarchiaImprese.LeggiElencoGerarchiaImpreseFiglieAsync(request.PivaFiliera, objParametriServer);

            if (pivas == null || pivas.Count == 0)
                return new PerimetroRaccoltiResult { StatoCaricamento = "no_data", PivaFiliera = request.PivaFiliera };

            // STEP 2: singola query → tutte le righe con esercizio aperto
            DataTable dt;
            try
            {
                dt = await _esercizi.LeggiEserciziApertixPivasAsync(pivas, objParametriServer, dataAttuale: true);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                return new PerimetroRaccoltiResult { StatoCaricamento = "error", PivaFiliera = request.PivaFiliera };
            }

            if (dt == null || dt.Rows.Count == 0)
                return new PerimetroRaccoltiResult { StatoCaricamento = "no_data", PivaFiliera = request.PivaFiliera };

            // STEP 3: batch lookup geografico tramite IIndirizzi
            var geoKeys = dt.Rows.Cast<DataRow>()
                .Select(r => new AppezzamentoGeoKey(
                    r.Field<string>("piva_azienda") ?? string.Empty,
                    Convert.ToInt32(r["sa_cod"]),
                    Convert.ToInt32(r["appezza"])))
                .Distinct()
                .ToList();

            var geoMap = new Dictionary<AppezzamentoGeoKey, (string Nazione, string Regione, string IstatReg)>();
            if (geoKeys.Count > 0)
            {
                var geoDt = await _indirizzi.LeggiIndirizzoxAppezzamentoAsync(geoKeys, objParametriServer);
                if (geoDt != null)
                {
                    foreach (DataRow geoRow in geoDt.Rows)
                    {
                        var key = new AppezzamentoGeoKey(
                            Convert.ToString(geoRow["PIVA"]) ?? string.Empty,
                            Convert.ToInt32(geoRow["SA_COD"]),
                            Convert.ToInt32(geoRow["APPEZZA"]));

                        geoMap[key] = (
                            Convert.ToString(geoRow["Nazione"]) ?? string.Empty,
                            Convert.ToString(geoRow["Regione"]) ?? string.Empty,
                            Convert.ToString(geoRow["REG"])     ?? string.Empty);
                    }
                }
            }

            // STEP 4: mapping DataTable → modello di output
            var perimetro = dt.Rows.Cast<DataRow>()
                .Select(r =>
                {
                    var geoKey = new AppezzamentoGeoKey(
                        r.Field<string>("piva_azienda") ?? string.Empty,
                        Convert.ToInt32(r["sa_cod"]),
                        Convert.ToInt32(r["appezza"]));

                    geoMap.TryGetValue(geoKey, out var geo);

                    return new PerimetroRigaItem
                    {
                        PivaAzienda = r.Field<string>("piva_azienda") ?? string.Empty,
                        NomeAzienda = r.Field<string>("nome_azienda") ?? string.Empty,
                        IdAppezzamento = Convert.ToInt32(r["id_appezzamento"]),
                        NomeAppezzamento = r.Field<string>("nome_appezzamento") ?? string.Empty,
                        IdEsercizio = Convert.ToInt32(r["id_esercizio"]),
                        IdImpianto = Convert.ToInt32(r["id_impianto"]),
                        sa_cod = Convert.ToInt32(r["sa_cod"]),
                        Nazione  = geo.Nazione  ?? string.Empty,
                        Regione  = geo.Regione  ?? string.Empty,
                        IstatReg = geo.IstatReg ?? string.Empty,
                        SuperficieHa = Convert.ToDecimal(r["superficie_ha"]),
                        CodSpecie = r.IsNull("cod_specie") ? null : (int?)Convert.ToInt32(r["cod_specie"]),
                        NomeSpecie = r.Field<string>("nome_specie") ?? string.Empty,
                        CodVarieta = r.IsNull("cod_varieta") ? null : (int?)Convert.ToInt32(r["cod_varieta"]),
                        NomeVarieta = r.Field<string>("nome_varieta") ?? string.Empty,
                        DataInizioEsercizio = r.IsNull("data_inizio_esercizio") ? null : (DateTime?)Convert.ToDateTime(r["data_inizio_esercizio"]),
                        DataFineEsercizio = r.IsNull("data_fine_esercizio") ? null : (DateTime?)Convert.ToDateTime(r["data_fine_esercizio"]),
                        NomeEsercizio = r.Field<string>("nome_esercizio") ?? string.Empty
                    };
                }).ToList();

            return new PerimetroRaccoltiResult
            {
                PivaFiliera = request.PivaFiliera,
                Perimetro = perimetro
            };
        }
    }
}
