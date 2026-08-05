using AgronicaNetCore.Base.Models;
using AgronicaNetCore.RischiMeteo.BIZ.Models.RischiMeteo.Response;
using AgronicaNetCore.RischiMeteo.BIZ.Resources;
using AgronicaNetCore.RischiMeteo.BIZ.Services;
using AgronicaNetCore.RischiMeteo.DAL.DataLayer.Lookup_Rischio_Meteo;
using InData.FoodMetaVerse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OutData.FoodMetaverse;
using System.Data;
using System.Globalization;
using System.Text.Json;

namespace AgronicaNetCore.RischiMeteo.BIZ.Services.RischiAggregati;

/// <summary>
/// Implements weather-climate risk geographic aggregation based on Lookup_Rischio_Meteo records.
/// See DS09-API Endpoint Consultazione Rischi Aggregati Geografici, sezione "Descrizione".
/// Depends on DS05-BL Aggregazione Indicatori Geografica per API FS4.08.1.
/// </summary>
public class RischiMeteoService : BaseServiceRischiMeteoBiz, IRischiMeteoService
{
    private readonly ILookup_Rischio_Meteo _lookupRischi;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Initializes the service and resolves required DAL dependencies.
    /// See DS09-API Endpoint Consultazione Rischi Aggregati Geografici, sezione "Dipendenze Business Logic".
    /// </summary>
    public RischiMeteoService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer)
    {
        _lookupRischi = provider.GetRequiredService<ILookup_Rischio_Meteo>();
    }

    /// <inheritdoc/>
    public async Task<RischiMeteoAggregationResult> GetRischiAggregatiAsync(
        string idFiliera,
        string codProdottoFmp,
        int tolleranzaMaxDays,
        AgronicaCoreParametriServer objP)
    {
        if (string.IsNullOrWhiteSpace(idFiliera))
            throw new ArgumentException("Parametro 'cuaa_filiera' obbligatorio.", nameof(idFiliera));

        if (string.IsNullOrWhiteSpace(codProdottoFmp))
            throw new ArgumentException("Parametro 'cod_prodotto' obbligatorio.", nameof(codProdottoFmp));

        if (tolleranzaMaxDays < 0)
            throw new ArgumentOutOfRangeException(nameof(tolleranzaMaxDays), "Il parametro 'tolleranza_max_days' deve essere >= 0.");

        var (codSpecie, codVarieta) = await ResolveSpecieVarietaDaCodProdottoAsync(codProdottoFmp, nameof(codProdottoFmp), objP);

        var filtro = new GetLookupRischioMeteo
        {
            Cuaa_Filiera = idFiliera,
            Cod_Specie = codSpecie,
            Cod_Varieta = codVarieta,
            Data_Invocazione_Da = DateTime.UtcNow.AddDays(-tolleranzaMaxDays)
        };

        DataTable table = await _lookupRischi.ReadAsync(filtro, objP);
        if (table.Rows.Count == 0)
        {
            return new RischiMeteoAggregationResult
            {
                HasData = false,
                Data = new RischiAggregatiData
                {
                    RischioGelo = null,
                    RischioSiccita = null,
                    RischioAllagamento = null,
                    SuperficieTotaleHa = 0,
                    Nazioni = new List<RischiAggregatiNazione>()
                },
                NumeroRigheElaborate = 0
            };
        }

        var rows = table.AsEnumerable()
            .Select(row => ToRiskRow(row, objP))
            .ToList();

        var data = new RischiAggregatiData
        {
            RischioGelo = CalculateWeightedAverage(rows.Select(r => (r.RischioGelo, r.SuperficieHa))),
            RischioSiccita = CalculateWeightedAverage(rows.Select(r => (r.RischioSiccita, r.SuperficieHa))),
            RischioAllagamento = CalculateWeightedAverage(rows.Select(r => (r.RischioAllagamento, r.SuperficieHa))),
            SuperficieTotaleHa = Round2(rows.Sum(r => r.SuperficieHa)),
            Nazioni = BuildNationAggregates(rows)
        };

        return new RischiMeteoAggregationResult
        {
            HasData = true,
            Data = data,
            NumeroRigheElaborate = rows.Count,
            NumeroRigheValide = CountValidRows(rows),
            IsPartialData = IsPartialData(data)
        };
    }

    /// <inheritdoc/>
    public async Task<RischiMeteoDettaglioResult> GetRischiDettaglioAsync(
        string idFiliera,
        string codProdottoFmp,
        int tolleranzaMaxDays,
        string? stato,
        string? regione,
        AgronicaCoreParametriServer objP)
    {
        if (string.IsNullOrWhiteSpace(idFiliera))
            throw new ArgumentException("Parametro 'cuaa_filiera' obbligatorio.", nameof(idFiliera));

        if (string.IsNullOrWhiteSpace(codProdottoFmp))
            throw new ArgumentException("Parametro 'cod_prodotto' obbligatorio.", nameof(codProdottoFmp));

        if (tolleranzaMaxDays < 0)
            throw new ArgumentOutOfRangeException(nameof(tolleranzaMaxDays), "Il parametro 'tolleranza_max_days' deve essere >= 0.");

        var (codSpecie, codVarieta) = await ResolveSpecieVarietaDaCodProdottoAsync(codProdottoFmp, nameof(codProdottoFmp), objP);

        var filtro = new GetLookupRischioMeteo
        {
            Cuaa_Filiera = idFiliera,
            Cod_Specie = codSpecie,
            Cod_Varieta = codVarieta,
            Nazione = NormalizeOptional(stato),
            Regione = NormalizeOptional(regione),
            Data_Invocazione_Da = DateTime.UtcNow.AddDays(-tolleranzaMaxDays)
        };

        DataTable table = await _lookupRischi.ReadAsync(filtro, objP);
        if (table.Rows.Count == 0)
        {
            return new RischiMeteoDettaglioResult
            {
                HasData = false,
                Data = new RischiDettaglioData
                {
                    NumeroEsercizi = 0,
                    Esercizi = new List<RischiDettaglioEsercizio>()
                },
                NumeroRigheElaborate = 0
            };
        }

        var esercizi = table.AsEnumerable()
            .Select(row => ToDettaglioRow(row, objP))
            .Select(MapToDettaglioEsercizio)
            .ToList();

        return new RischiMeteoDettaglioResult
        {
            HasData = true,
            Data = new RischiDettaglioData
            {
                NumeroEsercizi = esercizi.Count,
                Esercizi = esercizi
            },
            NumeroRigheElaborate = esercizi.Count
        };
    }

    private static List<RischiAggregatiNazione> BuildNationAggregates(List<RiskRow> rows)
    {
        return rows
            .GroupBy(r => r.Nazione, StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
            .Select(g => new RischiAggregatiNazione
            {
                CodNazione = g.Key,
                RischioGelo = CalculateWeightedAverage(g.Select(r => (r.RischioGelo, r.SuperficieHa))),
                RischioSiccita = CalculateWeightedAverage(g.Select(r => (r.RischioSiccita, r.SuperficieHa))),
                RischioAllagamento = CalculateWeightedAverage(g.Select(r => (r.RischioAllagamento, r.SuperficieHa))),
                SuperficieTotaleHa = Round2(g.Sum(r => r.SuperficieHa)),
                NumeroEsercizi = g.Count(),
                Regioni = BuildRegionAggregates(g.ToList())
            })
            .ToList();
    }

    private static List<RischiAggregatiRegione> BuildRegionAggregates(List<RiskRow> rows)
    {
        return rows
            .GroupBy(r => r.Regione, StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g.Key == null ? 0 : 1)
            .ThenBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
            .Select(g => new RischiAggregatiRegione
            {
                Regione = g.Key,
                RischioGelo = CalculateWeightedAverage(g.Select(r => (r.RischioGelo, r.SuperficieHa))),
                RischioSiccita = CalculateWeightedAverage(g.Select(r => (r.RischioSiccita, r.SuperficieHa))),
                RischioAllagamento = CalculateWeightedAverage(g.Select(r => (r.RischioAllagamento, r.SuperficieHa))),
                SuperficieTotaleHa = Round2(g.Sum(r => r.SuperficieHa)),
                NumeroEsercizi = g.Count()
            })
            .ToList();
    }

    private static int CountValidRows(List<RiskRow> rows)
    {
        return rows.Count(r =>
            r.SuperficieHa > 0
            && (r.RischioGelo.HasValue || r.RischioSiccita.HasValue || r.RischioAllagamento.HasValue));
    }

    private static bool IsPartialData(RischiAggregatiData data)
    {
        if (IsRiskTripletPartial(data.RischioGelo, data.RischioSiccita, data.RischioAllagamento))
            return true;

        foreach (var nazione in data.Nazioni)
        {
            if (IsRiskTripletPartial(nazione.RischioGelo, nazione.RischioSiccita, nazione.RischioAllagamento))
                return true;

            foreach (var regione in nazione.Regioni)
            {
                if (IsRiskTripletPartial(regione.RischioGelo, regione.RischioSiccita, regione.RischioAllagamento))
                    return true;
            }
        }

        return false;
    }

    private static bool IsRiskTripletPartial(double? rischioGelo, double? rischioSiccita, double? rischioAllagamento)
    {
        int valorizzati = 0;

        if (rischioGelo.HasValue)
            valorizzati++;

        if (rischioSiccita.HasValue)
            valorizzati++;

        if (rischioAllagamento.HasValue)
            valorizzati++;

        return valorizzati > 0 && valorizzati < 3;
    }

    private static double? CalculateWeightedAverage(IEnumerable<(double? Value, double Weight)> values)
    {
        double weightedSum = 0;
        double totalWeight = 0;

        foreach (var (value, weight) in values)
        {
            if (!value.HasValue)
                continue;

            if (weight <= 0)
                continue;

            weightedSum += value.Value * weight;
            totalWeight += weight;
        }

        if (totalWeight <= 0)
            return null;

        return Round2(weightedSum / totalWeight);
    }

    private RiskRow ToRiskRow(DataRow row, AgronicaCoreParametriServer objP)
    {
        string nazione = ReadRequiredString(row, "nazione");
        string? regione = ReadNullableString(row, "regione");
        double superficieHa = ReadRequiredDouble(row, "superficie_ha");

        var (rischioGelo, rischioSiccita, rischioAllagamento) = ResolveRiskValues(row, nazione, regione, objP);

        return new RiskRow(
            Nazione: nazione,
            Regione: string.IsNullOrWhiteSpace(regione) ? null : regione,
            SuperficieHa: superficieHa,
            RischioGelo: rischioGelo,
            RischioSiccita: rischioSiccita,
            RischioAllagamento: rischioAllagamento);
    }

    private double? SanitizeRiskValue(double? value, string riskColumn, string nazione, string? regione, AgronicaCoreParametriServer objP)
    {
        if (!value.HasValue)
            return null;

        if (value.Value >= 0d && value.Value <= 100d)
            return value;

        LogWarning(
            "Valore rischio fuori range escluso dall'aggregazione: colonna={0}, valore={1}, nazione={2}, regione={3}.",
            objP,
            null,
            riskColumn,
            value.Value,
            nazione,
            regione ?? "<null>");

        return null;
    }

    private RischioDettaglioRow ToDettaglioRow(DataRow row, AgronicaCoreParametriServer objP)
    {
        string nazione = ReadRequiredString(row, "nazione");
        string? regione = ReadNullableString(row, "regione");
        var (rischioGelo, rischioSiccita, rischioAllagamento) = ResolveRiskValues(row, nazione, regione, objP);

        return new RischioDettaglioRow(
            IdAzienda: ReadRequiredString(row, "cuaa_azienda"),
            Nazione: nazione,
            Regione: regione,
            CentroideWkt: ReadRequiredString(row, "centroide_wkt"),
            Epsg: ReadRequiredString(row, "epsg"),
            SuperficieHa: ReadRequiredDouble(row, "superficie_ha"),
            RischioGelo: rischioGelo,
            RischioSiccita: rischioSiccita,
            RischioAllagamento: rischioAllagamento);
    }

    private (double? RischioGelo, double? RischioSiccita, double? RischioAllagamento) ResolveRiskValues(
        DataRow row,
        string nazione,
        string? regione,
        AgronicaCoreParametriServer objP)
    {
        double? rischioGeloColumn = SanitizeRiskValue(ReadNullableDouble(row, "rischio_gelo"), "rischio_gelo", nazione, regione, objP);
        double? rischioSiccitaColumn = SanitizeRiskValue(ReadNullableDouble(row, "rischio_siccita"), "rischio_siccita", nazione, regione, objP);
        double? rischioAllagamentoColumn = SanitizeRiskValue(ReadNullableDouble(row, "rischio_allagamento"), "rischio_allagamento", nazione, regione, objP);

        string? jsonRisposta = ReadNullableString(row, "json_risposta");
        if (string.IsNullOrWhiteSpace(jsonRisposta))
            return (rischioGeloColumn, rischioSiccitaColumn, rischioAllagamentoColumn);

        try
        {
            AssessRiskResponse? parsed = JsonSerializer.Deserialize<AssessRiskResponse>(jsonRisposta, JsonOptions);
            if (parsed == null)
                return (rischioGeloColumn, rischioSiccitaColumn, rischioAllagamentoColumn);

            bool hasJsonGelo = parsed.rischioGelo is not null;
            bool hasJsonSiccita = parsed.rischioSiccita is not null;
            bool hasJsonAllagamento = parsed.rischioAllagamento is not null;

            double? rischioGeloJson = hasJsonGelo
                ? SanitizeRiskValue(parsed.rischioGelo!.dannoPopolazionePctComb, "json_risposta.rischioGelo.dannoPopolazionePctComb", nazione, regione, objP)
                : null;

            double? rischioSiccitaJson = hasJsonSiccita
                ? SanitizeRiskValue(parsed.rischioSiccita!.dannoPopolazionePctComb, "json_risposta.rischioSiccita.dannoPopolazionePctComb", nazione, regione, objP)
                : null;

            double? rischioAllagamentoJson = hasJsonAllagamento
                ? SanitizeRiskValue(parsed.rischioAllagamento!.dannoPopolazionePctComb, "json_risposta.rischioAllagamento.dannoPopolazionePctComb", nazione, regione, objP)
                : null;

            return (
                hasJsonGelo ? rischioGeloJson : rischioGeloColumn,
                hasJsonSiccita ? rischioSiccitaJson : rischioSiccitaColumn,
                hasJsonAllagamento ? rischioAllagamentoJson : rischioAllagamentoColumn);
        }
        catch (Exception ex)
        {
            LogWarning(
                "json_risposta non deserializzabile; uso fallback sui campi rischio_*: nazione={0}, regione={1}, errore={2}.",
                objP,
                ex,
                nazione,
                regione ?? "<null>",
                ex.Message);

            return (rischioGeloColumn, rischioSiccitaColumn, rischioAllagamentoColumn);
        }
    }

    private static RischiDettaglioEsercizio MapToDettaglioEsercizio(RischioDettaglioRow row)
    {
        return new RischiDettaglioEsercizio
        {
            IdAzienda = row.IdAzienda,
            Stato = row.Nazione,
            Regione = row.Regione,
            Centroide = new RischiDettaglioCentroide
            {
                Coordinate = row.CentroideWkt,
                Epsg = row.Epsg
            },
            SuperficieHa = Round2(row.SuperficieHa),
            RischioGelo = row.RischioGelo.HasValue ? Round2(row.RischioGelo.Value) : null,
            RischioSiccita = row.RischioSiccita.HasValue ? Round2(row.RischioSiccita.Value) : null,
            RischioAllagamento = row.RischioAllagamento.HasValue ? Round2(row.RischioAllagamento.Value) : null
        };
    }

    private static string? NormalizeOptional(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        return input.Trim();
    }

    private async Task<(int CodSpecie, int CodVarieta)> ResolveSpecieVarietaDaCodProdottoAsync(string productCode, string paramName, AgronicaCoreParametriServer objP)
    {
        var (elemCod, matCod) = ParseCompositeProductCodeElemMat(productCode, paramName);

        var mapped = await _lookupRischi.ResolveCodSpecieVarietaDaProdottoAsync(elemCod, matCod, objP);
        if (mapped.HasValue)
            return mapped.Value;

        // Fallback temporaneo in assenza della query di mapping.
        return (elemCod, matCod);
    }

    private static (int ElemCod, int MatCod) ParseCompositeProductCodeElemMat(string productCode, string paramName)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            throw new ArgumentException("Parametro 'cod_prodotto' obbligatorio.", paramName);

        var parts = productCode.Trim().Replace('|', '_').Split('_', StringSplitOptions.None);
        if (parts.Length != 2
            || !int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var elemCod)
            || !int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var matCod))
        {
            throw new ArgumentException("Parametro 'cod_prodotto' non valido: formato atteso '{elem_cod}_{mat_cod}'.", paramName);
        }

        return (elemCod, matCod);
    }

    private static string ReadRequiredString(DataRow row, string columnName)
    {
        if (!row.Table.Columns.Contains(columnName))
            throw new InvalidOperationException($"Colonna mancante: {columnName}");

        object value = row[columnName];
        if (value == DBNull.Value || value == null)
            throw new InvalidOperationException($"Valore nullo non consentito per colonna: {columnName}");

        string text = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException($"Valore vuoto non consentito per colonna: {columnName}");

        return text.Trim();
    }

    private static string? ReadNullableString(DataRow row, string columnName)
    {
        if (!row.Table.Columns.Contains(columnName))
            throw new InvalidOperationException($"Colonna mancante: {columnName}");

        object value = row[columnName];
        if (value == DBNull.Value || value == null)
            return null;

        string? text = Convert.ToString(value, CultureInfo.InvariantCulture);
        return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
    }

    private static double ReadRequiredDouble(DataRow row, string columnName)
    {
        double? value = ReadNullableDouble(row, columnName);
        if (!value.HasValue)
            throw new InvalidOperationException($"Valore numerico obbligatorio mancante per colonna: {columnName}");

        return value.Value;
    }

    private static double? ReadNullableDouble(DataRow row, string columnName)
    {
        if (!row.Table.Columns.Contains(columnName))
            throw new InvalidOperationException($"Colonna mancante: {columnName}");

        object value = row[columnName];
        if (value == DBNull.Value || value == null)
            return null;

        if (value is double valueDouble)
            return valueDouble;

        if (value is float valueFloat)
            return valueFloat;

        if (value is decimal valueDecimal)
            return Convert.ToDouble(valueDecimal, CultureInfo.InvariantCulture);

        if (value is int valueInt)
            return valueInt;

        if (value is long valueLong)
            return valueLong;

        if (double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed))
            return parsed;

        throw new InvalidOperationException($"Valore non convertibile a double per colonna: {columnName}");
    }

    private static double Round2(double value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    private sealed record RiskRow(
        string Nazione,
        string? Regione,
        double SuperficieHa,
        double? RischioGelo,
        double? RischioSiccita,
        double? RischioAllagamento);

    private sealed record RischioDettaglioRow(
        string IdAzienda,
        string Nazione,
        string? Regione,
        string CentroideWkt,
        string Epsg,
        double SuperficieHa,
        double? RischioGelo,
        double? RischioSiccita,
        double? RischioAllagamento);
}