using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Resources;
using OutData.FoodMetaverse;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Aziendale_Chiavi;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Aziendale_Payload;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Colture_Chiavi;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Colture_Payload;
using InData.FoodMetaVerse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Data;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.IndicatoriSostenibilita;

/// <summary>
/// Implements sustainability indicator retrieval per company, aggregated by year.
/// Queries <c>Lookup_Sost_CO2_Aziendale_Chiavi</c> and <c>Lookup_Sost_CO2_Aziendale_Payload</c>
/// lookup tables populated by the M4 persistence logic (DS08-BL, 01KHXAFA2T3Y2SVH2TYF1TA671).
/// See DS08-API: Design Specification 01KHXAFCB1651AWBC2Y884DNTR.
/// </summary>
public class IndicatoriSostenibilitaService : BaseServiceSostenibilitaCO2Biz, IIndicatoriSostenibilitaService
{
    private readonly ILookup_Sost_CO2_Aziendale_Chiavi _chiaviDal;
    private readonly ILookup_Sost_CO2_Aziendale_Payload _payloadDal;
    private readonly ILookup_Sost_CO2_Colture_Chiavi _coltureChiaviDal;
    private readonly ILookup_Sost_CO2_Colture_Payload _colturePayloadDal;

    /// <summary>Initializes a new instance, resolving DAL dependencies from the DI container.</summary>
    public IndicatoriSostenibilitaService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer)
    {
        _chiaviDal = provider.GetRequiredService<ILookup_Sost_CO2_Aziendale_Chiavi>();
        _payloadDal = provider.GetRequiredService<ILookup_Sost_CO2_Aziendale_Payload>();
        _coltureChiaviDal = provider.GetRequiredService<ILookup_Sost_CO2_Colture_Chiavi>();
        _colturePayloadDal = provider.GetRequiredService<ILookup_Sost_CO2_Colture_Payload>();
    }

    /// <inheritdoc/>
    public async Task<IndicatoriAziendaAnnualeResponse?> GetIndicatoriAziendaAnnualeAsync(
        string idAzienda,
        AgronicaCoreParametriServer objP)
    {
        // Step 1 - Retrieve all chiavi records for this azienda, ordered by data_invocazione DESC
        DataTable chiaviTable = await _chiaviDal.ReadAsync(
            new GetSostCO2Aziendale { Azienda = idAzienda },
            objP);

        if (chiaviTable.Rows.Count == 0)
        {
            LogInformation($"No lookup records found for azienda '{idAzienda}'.", objP);
            return null; // Caller returns HTTP 404
        }

        // Step 3 - Group by anno; take the row with the most recent data_invocazione per year
        // (DataTable is already sorted by data_invocazione DESC, so First() gives the latest)
        var latestByAnno = chiaviTable.AsEnumerable()
            .GroupBy(row => row.Field<int>("anno"))
            .Select(g => new
            {
                Anno = g.Key,
                IdInvocazione = ReadColumnAsString(g.First(), "id_invocazione")
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.IdInvocazione))
            .OrderByDescending(x => x.Anno)
            .ToList();

        var response = new IndicatoriAziendaAnnualeResponse { IdAzienda = idAzienda };

        // Steps 4-6 - For each year: load payload, deserialize M4 response, extract indicators.
        // Best-effort: if any year fails, log a warning and continue (year is omitted from response).
        foreach (var entry in latestByAnno)
        {
            try
            {
                DataTable payloadTable = await _payloadDal.ReadAsync(entry.IdInvocazione, null, objP);
                if (payloadTable.Rows.Count == 0)
                {
                    LogWarning($"No payload row found for id_invocazione={entry.IdInvocazione}, anno={entry.Anno}. Year omitted.", objP);
                    continue;
                }

                var jsonRisposta = payloadTable.Rows[0].Field<string>("json_risposta");
                if (string.IsNullOrWhiteSpace(jsonRisposta))
                {
                    LogWarning($"Empty json_risposta for id_invocazione={entry.IdInvocazione}, anno={entry.Anno}. Year omitted.", objP);
                    continue;
                }

                // Step 4 - Deserialize M4 response JSON
                var m4Response = JsonConvert.DeserializeObject<M4RispostaDto>(jsonRisposta);

                // Step 5 - Locate the azienda entry by PIVA
                var aziendaData = m4Response?.Aziende?.FirstOrDefault(a =>
                    string.Equals(a.IdAzienda, idAzienda, StringComparison.OrdinalIgnoreCase));

                if (aziendaData == null)
                {
                    LogWarning($"Azienda '{idAzienda}' not found in M4 json_risposta for id_invocazione={entry.IdInvocazione}, anno={entry.Anno}. Year omitted.", objP);
                    continue;
                }

                // Step 6 - Extract indicators; superficie_coltivata_ha = sum of appezzamento area_ha
                var superficieHa = aziendaData.Appezzamenti?.Sum(a => a.AreaHa ?? 0m) ?? 0m;
                var totali = aziendaData.Totali?.FirstOrDefault();

                response.Anni.Add(new IndicatoriAnnoDto
                {
                    Anno = entry.Anno,
                    GhgScope1KgCo2Equiv = totali?.Scope1 ?? aziendaData.Scope1,
                    GhgScope2KgCo2Equiv = totali?.Scope2LocationBased ?? aziendaData.Scope2LocationBased,
                    GhgScope3KgCo2Equiv = totali?.Scope3 ?? aziendaData.Scope3,
                    GhgBiogenicCarbonKgCo2Equiv = totali?.BiogenicCarbon ?? aziendaData.BiogenicCarbon,
                    VarSocSoilBiogenicCarbonKgCo2Equiv = totali?.VarSocSoilBiogenicCarbon ?? aziendaData.VarSocSoilBiogenicCarbon,
                    VarBiomassBiogenicCarbonKgCo2Equiv = totali?.VarBiomassBiogenicCarbon ?? aziendaData.VarBiomassBiogenicCarbon,
                    SuperficieColtivataHa = superficieHa
                });
            }
            catch (JsonException ex)
            {
                // Best-effort: malformed JSON for this year - log and omit year from response
                LogWarning(
                    $"JSON deserialization failed for id_invocazione={entry.IdInvocazione}, anno={entry.Anno}. Year omitted. Error: {ex.Message}",
                    objP, ex);
            }
        }

        return response;
    }

    /// <inheritdoc/>
    public async Task<IndicatoriProdottoFilieraResponse?> GetIndicatoriProdottoFilieraAsync(
        string idFiliera,
        string codProdotto,
        AgronicaCoreParametriServer objP)
    {
        var (elemCod, matCod, normalizedProductCode) = ParseCompositeProductCode(codProdotto, nameof(codProdotto));

        // Step 1 - Query all chiavi for this filiera, ordered by data_invocazione DESC.
        // Filter on cod_prodotto (elem_cod + mat_cod) is applied in-memory after fetch.
        DataTable chiaviTable = await _coltureChiaviDal.ReadAsync(
            new GetSostCO2Colture { PivaFiliera = idFiliera },
            objP);

        // In-memory filter: keep only rows where both elem_cod and mat_cod match the request.
        var filteredRows = chiaviTable.AsEnumerable()
            .Where(row => ElemAndMatMatchProductCode(row, elemCod, matCod))
            .ToList();

        if (filteredRows.Count == 0)
        {
            LogInformation($"No lookup records found for filiera='{idFiliera}', cod_prodotto='{normalizedProductCode}'.", objP);
            return null; // Caller returns HTTP 404
        }

        // Step 3 - Group by (piva_azienda, anno); take the row with the latest data_invocazione per group.
        // The DataTable is already ordered by data_invocazione DESC, so First() gives the most recent.
        var latestByAziendaAnno = filteredRows
            .GroupBy(row => new
            {
                Azienda = row.Field<string>("cuaa_azienda") ?? string.Empty,
                Anno = row.Field<int>("anno")
            })
            .Select(g => new
            {
                g.Key.Azienda,
                g.Key.Anno,
                IdInvocazione = ReadColumnAsString(g.First(), "id_invocazione"),
                EsercizioCode = ReadColumnAsInt(g.First(), "progetto_cod")
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.IdInvocazione))
            .OrderBy(x => x.Azienda)
            .ThenByDescending(x => x.Anno)
            .ToList();

        var response = new IndicatoriProdottoFilieraResponse
        {
            IdFiliera = idFiliera,
            CodProdotto = normalizedProductCode
        };

        // Step 4â€“7 - For each unique azienda/anno pair: load payload, deserialize, extract indicators.
        // Best-effort: on any failure the year for that azienda is omitted (spec step 7).
        var aziendaGroups = latestByAziendaAnno.GroupBy(x => x.Azienda);

        foreach (var aziendaGroup in aziendaGroups)
        {
            var aziendaDto = new IndicatoriAziendaFilieraDto { IdAzienda = aziendaGroup.Key };

            foreach (var entry in aziendaGroup)
            {
                try
                {
                    // Step 4 - Load and deserialize the M4 JSON response (colture mode).
                    DataTable payloadTable = await _colturePayloadDal.ReadAsync(entry.IdInvocazione, null, objP);
                    if (payloadTable.Rows.Count == 0)
                    {
                        LogWarning($"No payload row for id_invocazione={entry.IdInvocazione}, azienda='{entry.Azienda}', anno={entry.Anno}. Year omitted.", objP);
                        continue;
                    }

                    var jsonRisposta = payloadTable.Rows[0].Field<string>("json_risposta");
                    if (string.IsNullOrWhiteSpace(jsonRisposta))
                    {
                        LogWarning($"Empty json_risposta for id_invocazione={entry.IdInvocazione}, azienda='{entry.Azienda}', anno={entry.Anno}. Year omitted.", objP);
                        continue;
                    }

                    var m4Response = JsonConvert.DeserializeObject<M4ColtureRispostaDto>(jsonRisposta);

                    // Step 5 - Locate azienda in M4 response.
                    var aziendaData = m4Response?.Aziende?.FirstOrDefault(a =>
                        string.Equals(a.IdAzienda, entry.Azienda, StringComparison.OrdinalIgnoreCase));

                    if (aziendaData == null)
                    {
                        LogWarning($"Azienda '{entry.Azienda}' not found in M4 json_risposta for id_invocazione={entry.IdInvocazione}, anno={entry.Anno}. Year omitted.", objP);
                        continue;
                    }

                    if (!entry.EsercizioCode.HasValue)
                    {
                        LogWarning($"Missing progetto_cod for id_invocazione={entry.IdInvocazione}, azienda='{entry.Azienda}', anno={entry.Anno}. Year omitted.", objP);
                        continue;
                    }

                    // Select the impianto mapped by progetto_cod (unique esercizio code).
                    var impianti = aziendaData.Appezzamenti?
                        .SelectMany(a => a.Impianti ?? new List<M4ColtureImpiiantoDto>())
                        .Where(i => ParseEsercizioCodeFromIdImpianto(i.IdImpianto) == entry.EsercizioCode)
                        .ToList() ?? new List<M4ColtureImpiiantoDto>();

                    if (impianti.Count == 0)
                    {
                        LogWarning($"No impianti in scope for id_invocazione={entry.IdInvocazione}, azienda='{entry.Azienda}', anno={entry.Anno}. Year omitted.", objP);
                        continue;
                    }

                    // Step 6 - Compute weighted-average indicators per hectare (DS08-API spec "Elaborazione" step 6).
                    var totalAreaHa = impianti.Sum(i => i.AreaHa ?? 0m);
                    if (totalAreaHa == 0m)
                    {
                        LogWarning($"Total area_ha is zero for id_invocazione={entry.IdInvocazione}, azienda='{entry.Azienda}', anno={entry.Anno}. Year omitted.", objP);
                        continue;
                    }

                    var totalGhg = impianti.Sum(i => (i.Scope1 ?? 0m) + (i.Scope2LocationBased ?? 0m) + (i.Scope3 ?? 0m));
                    var totalVarBiomass = impianti.Sum(i => i.VarBiomassBiogenicCarbon ?? 0m);

                    aziendaDto.Anni.Add(new IndicatoriAnnoColtureDto
                    {
                        Anno = entry.Anno,
                        GhgKgCo2EquivPerHa = totalGhg / totalAreaHa,
                        VarBiomassBiogenicCarbonKgCo2EquivPerHa = totalVarBiomass / totalAreaHa
                    });
                }
                catch (JsonException ex)
                {
                    // Best-effort: malformed JSON - log and omit year from response (spec step 7).
                    LogWarning(
                        $"JSON deserialization failed for id_invocazione={entry.IdInvocazione}, azienda='{entry.Azienda}', anno={entry.Anno}. Year omitted. Error: {ex.Message}",
                        objP, ex);
                }
            }

            // Best-effort: omit azienda entirely when no valid years remain (spec step 7).
            if (aziendaDto.Anni.Count > 0)
                response.Aziende.Add(aziendaDto);
        }

        return response;
    }

    /// <inheritdoc/>
    public async Task<IndicatoriLottoRaccoltaResponse?> GetIndicatoriLottoRaccoltaAsync(
        string idFiliera,
        string idAzienda,
        string codProdottoFmp,
        string codLottoFmp,
        AgronicaCoreParametriServer objP)
    {
        var (elemCod, matCod, normalizedProductCode) = ParseCompositeProductCode(codProdottoFmp, nameof(codProdottoFmp));

        // Step 1 - Query most recent chiavi row for (filiera, azienda, lotto), ordered DESC.
        // cod_prodotto maps to (elem_cod, mat_cod), filtered in-memory after fetch.
        DataTable chiaviTable = await _coltureChiaviDal.ReadAsync(
            new GetSostCO2Colture
            {
                PivaFiliera = idFiliera,
                PivaAzienda = idAzienda,
                Lotto = codLottoFmp
            }, objP);

        // Filter in-memory on both elem_cod and mat_cod to match cod_prodotto.
        var filteredRows = chiaviTable.AsEnumerable()
            .Where(row => ElemAndMatMatchProductCode(row, elemCod, matCod))
            .ToList();

        if (filteredRows.Count == 0)
        {
            LogInformation(
                $"No lookup records found for filiera='{idFiliera}', azienda='{idAzienda}', " +
                $"cod_prodotto='{normalizedProductCode}', cod_lotto='{codLottoFmp}'.", objP);
            return null; // Caller returns HTTP 404
        }

        // Take the most recent row (DataTable already sorted by data_invocazione DESC).
        var targetRow = filteredRows.First();
        var idInvocazione = ReadColumnAsString(targetRow, "id_invocazione");
        if (string.IsNullOrWhiteSpace(idInvocazione))
        {
            LogWarning($"Missing id_invocazione for lotto='{codLottoFmp}'. Returning 404.", objP);
            return null;
        }
        var targetProgettoCod = ReadColumnAsInt(targetRow, "progetto_cod");
        if (!targetProgettoCod.HasValue)
        {
            LogWarning($"Missing progetto_cod for id_invocazione={idInvocazione}, lotto='{codLottoFmp}'. Returning 404.", objP);
            return null;
        }

        // Step 3 - Join with Payload: fetch json_risposta for this invocazione.
        DataTable payloadTable = await _colturePayloadDal.ReadAsync(idInvocazione, null, objP);
        if (payloadTable.Rows.Count == 0)
        {
            LogWarning($"No payload row found for id_invocazione={idInvocazione}, lotto='{codLottoFmp}'. Returning 404.", objP);
            return null;
        }

        var jsonRisposta = payloadTable.Rows[0].Field<string>("json_risposta");
        if (string.IsNullOrWhiteSpace(jsonRisposta))
        {
            LogWarning($"Empty json_risposta for id_invocazione={idInvocazione}, lotto='{codLottoFmp}'. Returning 404.", objP);
            return null;
        }

        // Step 4 - Deserialize M4 response JSON.
        var m4Response = JsonConvert.DeserializeObject<M4ColtureRispostaDto>(jsonRisposta);

        // Step 5 - Locate azienda in M4 response.
        var aziendaData = m4Response?.Aziende?.FirstOrDefault(a =>
            string.Equals(a.IdAzienda, idAzienda, StringComparison.OrdinalIgnoreCase));

        if (aziendaData == null)
        {
            LogWarning(
                $"Azienda '{idAzienda}' not found in M4 json_risposta for id_invocazione={idInvocazione}, " +
                $"lotto='{codLottoFmp}'. Returning 404.", objP);
            return null;
        }

        var targetEserciziId = targetProgettoCod.Value;

        // Locate the impianto in the M4 response that matches the target esercizio ID.
        var rispEsercizio = aziendaData.Appezzamenti?
            .SelectMany(a => a.Impianti ?? new List<M4ColtureImpiiantoDto>())
            .FirstOrDefault(i => ParseEsercizioCodeFromIdImpianto(i.IdImpianto) == targetEserciziId);

        if (rispEsercizio == null)
        {
            LogWarning(
                $"Impianto '{targetEserciziId}' not found in M4 json_risposta for id_invocazione={idInvocazione}, " +
                $"lotto='{codLottoFmp}'. Returning 404.", objP);
            return null;
        }

        var areaHa = rispEsercizio.AreaHa ?? 0m;

        if (areaHa == 0m)
        {
            // Division by zero guard for per-hectare calculations.
            throw new DivideByZeroException("area_ha = 0");
        }

        var scope1 = rispEsercizio.Scope1 ?? 0m;
        var scope3 = rispEsercizio.Scope3 ?? 0m;
        var varBiomass = rispEsercizio.VarBiomassBiogenicCarbon ?? 0m;

        // Step 7 - Compute indicators per hectare (DS08-API spec "Elaborazione" step 7).
        return new IndicatoriLottoRaccoltaResponse
        {
            IdFiliera = idFiliera,
            IdAzienda = idAzienda,
            CodProdotto = normalizedProductCode,
            CodLotto = codLottoFmp,
            GhgScope1KgCo2EquivPerHa = scope1 / areaHa,
            GhgScope3KgCo2EquivPerHa = scope3 / areaHa,
            VarBiomassBiogenicCarbonKgCo2EquivPerHa = varBiomass / areaHa
        };
    }

    // â”€â”€ Private DTOs for M4 json_risposta deserialization â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    private class M4RispostaDto
    {
        [JsonProperty("aziende")]
        public List<M4AziendaDto>? Aziende { get; set; }
    }

    private class M4AziendaDto
    {
        [JsonProperty("id_azienda")]
        public string? IdAzienda { get; set; }

        /// <summary>GHG Scope 1 emissions (kg CO2 equiv).</summary>
        [JsonProperty("scope_1")]
        public decimal? Scope1 { get; set; }

        /// <summary>GHG Scope 2 emissions, location-based (kg CO2 equiv).</summary>
        [JsonProperty("scope_2_location_based")]
        public decimal? Scope2LocationBased { get; set; }

        /// <summary>GHG Scope 3 emissions (kg CO2 equiv).</summary>
        [JsonProperty("scope_3")]
        public decimal? Scope3 { get; set; }

        /// <summary>Total biogenic carbon (kg CO2 equiv).</summary>
        [JsonProperty("biogenic_carbon")]
        public decimal? BiogenicCarbon { get; set; }

        /// <summary>Variation in soil organic carbon, biogenic (kg CO2 equiv).</summary>
        [JsonProperty("var_soc_soil_biogenic_carbon")]
        public decimal? VarSocSoilBiogenicCarbon { get; set; }

        /// <summary>Variation in biomass biogenic carbon (kg CO2 equiv).</summary>
        [JsonProperty("var_biomass_biogenic_carbon")]
        public decimal? VarBiomassBiogenicCarbon { get; set; }

        /// <summary>
        /// Totals block returned by M4 (canonical source in persisted json_risposta).
        /// </summary>
        [JsonProperty("totali")]
        public List<M4TotaliDto>? Totali { get; set; }

        [JsonProperty("appezzamenti")]
        public List<M4AppezzamentoDto>? Appezzamenti { get; set; }
    }

    private class M4TotaliDto
    {
        [JsonProperty("CO2eq_tot_scope_1")]
        public decimal? Scope1 { get; set; }

        [JsonProperty("CO2eq_tot_scope_2_location_based")]
        public decimal? Scope2LocationBased { get; set; }

        [JsonProperty("CO2eq_tot_scope_3")]
        public decimal? Scope3 { get; set; }

        [JsonProperty("CO2eq_tot_biogenic")]
        public decimal? BiogenicCarbon { get; set; }

        [JsonProperty("CO2eq_tot_var_SOC")]
        public decimal? VarSocSoilBiogenicCarbon { get; set; }

        [JsonProperty("CO2eq_tot_var_biomass")]
        public decimal? VarBiomassBiogenicCarbon { get; set; }
    }

    private class M4AppezzamentoDto
    {
        /// <summary>Area of the plot in hectares.</summary>
        [JsonProperty("area_ha")]
        public decimal? AreaHa { get; set; }
    }

    // â”€â”€ Private DTOs for M4 colture json_risposta deserialization (DS08-API spec "Elaborazione") â”€â”€

    /// <summary>Top-level M4 response DTO for "Per Colture" mode.</summary>
    private class M4ColtureRispostaDto
    {
        [JsonProperty("aziende")]
        public List<M4ColtureAziendaDto>? Aziende { get; set; }
    }

    private class M4ColtureAziendaDto
    {
        [JsonProperty("id_azienda")]
        public string? IdAzienda { get; set; }

        [JsonProperty("appezzamenti")]
        public List<M4ColtureAppezzamentoDto>? Appezzamenti { get; set; }
    }

    private class M4ColtureAppezzamentoDto
    {
        [JsonProperty("impianti")]
        public List<M4ColtureImpiiantoDto>? Impianti { get; set; }
    }

    /// <summary>Per-impianto (esercizio/ciclo colturale) indicators inside a colture M4 response.</summary>
    private class M4ColtureImpiiantoDto
    {
        /// <summary>Exercise/plant identifier; matched against progetto_cod.</summary>
        [JsonProperty("id_impianto")]
        public string? IdImpianto { get; set; }

        [JsonProperty("scope_1")]
        public decimal? Scope1 { get; set; }

        [JsonProperty("scope_2_location_based")]
        public decimal? Scope2LocationBased { get; set; }

        [JsonProperty("scope_3")]
        public decimal? Scope3 { get; set; }

        [JsonProperty("var_biomass_biogenic_carbon")]
        public decimal? VarBiomassBiogenicCarbon { get; set; }

        [JsonProperty("area_ha")]
        public decimal? AreaHa { get; set; }
    }

    // â”€â”€ Private helpers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>
    /// Returns <c>true</c> when both <c>elem_cod</c> and <c>mat_cod</c> in the DataRow
    /// match the requested composite product code segments.
    /// </summary>
    private static bool ElemAndMatMatchProductCode(DataRow row, int expectedElemCod, int expectedMatCod)
    {
        int? elemCod = ReadColumnAsInt(row, "elem_cod");
        if (!elemCod.HasValue)
            return false;

        int? matCod = ReadColumnAsInt(row, "mat_cod");
        if (!matCod.HasValue)
            return false;

        return elemCod.Value == expectedElemCod && matCod.Value == expectedMatCod;
    }

    private static (int ElemCod, int MatCod, string NormalizedCode) ParseCompositeProductCode(string code, string paramName)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Il parametro deve rispettare il formato '{elem_cod}_{mat_cod}'.", paramName);

        var parts = code.Trim().Split('_', StringSplitOptions.None);
        if (parts.Length != 2
            || !int.TryParse(parts[0], out var elemCod)
            || !int.TryParse(parts[1], out var matCod))
        {
            throw new ArgumentException("Il parametro deve rispettare il formato '{elem_cod}_{mat_cod}'.", paramName);
        }

        return (elemCod, matCod, $"{elemCod}_{matCod}");
    }

    /// <summary>
    /// Reads a DataRow column as string without assuming the underlying CLR type.
    /// </summary>
    private static string? ReadColumnAsString(DataRow row, string columnName)
    {
        var raw = row[columnName];
        if (raw == null || raw == DBNull.Value)
            return null;

        return raw.ToString();
    }

    /// <summary>
    /// Reads a DataRow numeric column as int.
    /// Returns null on DBNull/null/non-numeric values.
    /// </summary>
    private static int? ReadColumnAsInt(DataRow row, string columnName)
    {
        var raw = row[columnName];
        if (raw == null || raw == DBNull.Value)
            return null;

        if (raw is int valueAsInt)
            return valueAsInt;

        if (raw is IConvertible convertible)
        {
            try
            {
                return convertible.ToInt32(System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        return ParseNullableInt(raw.ToString());
    }

    /// <summary>
    /// Parses an integer from text using invariant culture; returns null if parsing fails.
    /// </summary>
    private static int? ParseNullableInt(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : null;
    }

    private static int? ParseEsercizioCodeFromIdImpianto(string? idImpianto)
    {
        if (string.IsNullOrWhiteSpace(idImpianto))
            return null;

        var normalized = idImpianto.Trim();
        var lastSeparatorIndex = normalized.LastIndexOf('|');
        var lastSegment = lastSeparatorIndex >= 0
            ? normalized[(lastSeparatorIndex + 1)..]
            : normalized;

        return ParseNullableInt(lastSegment);
    }

    /// <summary>
    /// Reads a DataRow numeric column as decimal, supporting SQL float/double and numeric strings.
    /// </summary>
    private static decimal? ReadColumnAsDecimal(DataRow row, string columnName)
    {
        var raw = row[columnName];
        if (raw == null || raw == DBNull.Value)
            return null;

        if (raw is decimal valueAsDecimal)
            return valueAsDecimal;

        if (raw is double valueAsDouble)
            return Convert.ToDecimal(valueAsDouble);

        if (raw is float valueAsFloat)
            return Convert.ToDecimal(valueAsFloat);

        if (raw is IConvertible convertible)
            return convertible.ToDecimal(System.Globalization.CultureInfo.InvariantCulture);

        if (decimal.TryParse(raw.ToString(), out var parsed))
            return parsed;

        throw new InvalidCastException($"Column '{columnName}' cannot be converted to decimal.");
    }
}

