using AgronicaNetCore.Base.Models;
using AgronicaNetCore.RischiMeteo.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.RischiMeteo.DAL.DataLayer.GisGeometria
{
    /// <summary>
    /// Recupera geometrie WKT (poligono e centroide) di un appezzamento dalle tabelle GIS GIAS.
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Mapping geoImpianto.poligonoWkt e centroideWkt.
    /// </summary>
    public class GisGeometriaRischiMeteoDAL : BaseDALRischiMeteo, IGisGeometriaRischiMeteoDAL
    {
        public GisGeometriaRischiMeteoDAL(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        /// <inheritdoc/>
        //public async Task<string?> GetPoligonoWktAsync(
        //    string piva, int saCod, int appezza,
        //    AgronicaCoreParametriServer objParametriServer)
        //{
        //    if (string.IsNullOrWhiteSpace(piva))    throw new ArgumentException("Specificare la partita IVA.",     nameof(piva));
        //    if (saCod <= 0)                         throw new ArgumentException("Specificare il codice Sa_Cod.",   nameof(saCod));
        //    if (appezza <= 0)                       throw new ArgumentException("Specificare il codice Appezza.", nameof(appezza));

        //    const string sql = @"
        //        SELECT TOP 1 COALESCE(g.Poligono_GeoEntity_WKT, g.Poligono_GeoEntity.STAsText()) AS Poligono_GeoEntity_WKT
        //        FROM GIS_ElementiGrafici g
        //        INNER JOIN GIS_Entita e ON e.Entita_Cod = g.Entita_Cod
        //        WHERE e.Piva    = @piva
        //          AND e.Sa_Cod  = @saCod
        //          AND e.Appezza = @appezza";

        //    var sqlParams = new Dictionary<string, object>
        //    {
        //        ["@piva"]    = piva,
        //        ["@saCod"]   = saCod,
        //        ["@appezza"] = appezza
        //    };

        //    try
        //    {
        //        var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, sqlParams);
        //        if (dt.Rows.Count == 0) return null;
        //        var value = dt.Rows[0]["Poligono_GeoEntity_WKT"];
        //        return value == DBNull.Value ? null : value?.ToString();
        //        //var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, sqlParams);
        //        //if (dt.Rows.Count == 0) return "POLYGON ((8.6755004051702542 39.9504270996087, 8.6755030253845824 39.950414493526161, 8.675522665624289 39.950267253962394, 8.6755416464719559 39.950240413268851, 8.6755271214814478 39.950233849061824, 8.6755280599919473 39.950226813183463, 8.675549224046641 39.9499993460666, 8.6756212826290788 39.949981708769506, 8.6755985078991529 39.950226489886973, 8.675598481523906 39.950226773363759, 8.6755976424322139 39.950235791824923, 8.6755976311516747 39.950235913067175, 8.6755875614920956 39.950344140412739, 8.675587550176493 39.950344262031592, 8.6755867111328389 39.950353279943485, 8.6755866836805726 39.9503535749959, 8.6755862961832744 39.950353553639864, 8.6755862426466468 39.950353756926269, 8.6755856952351262 39.950355835527112, 8.6755863324641425 39.950355913982285, 8.6755849104119775 39.950362755648356, 8.675584793650513 39.950363317401958, 8.67558292961769 39.950372285488932, 8.6755754593189867 39.950408225972986, 8.6755735809647678 39.950417262949649, 8.6755734655508849 39.950417818219087, 8.6755715944298011 39.9504268203933, 8.6755715663459654 39.950426955507773, 8.67556474632694 39.950459767367988, 8.6755004051702542 39.9504270996087))";
        //        //var value = dt.Rows[0]["Poligono_GeoEntity_WKT"];
        //        //return value == DBNull.Value ? "POLYGON ((8.6755004051702542 39.9504270996087, 8.6755030253845824 39.950414493526161, 8.675522665624289 39.950267253962394, 8.6755416464719559 39.950240413268851, 8.6755271214814478 39.950233849061824, 8.6755280599919473 39.950226813183463, 8.675549224046641 39.9499993460666, 8.6756212826290788 39.949981708769506, 8.6755985078991529 39.950226489886973, 8.675598481523906 39.950226773363759, 8.6755976424322139 39.950235791824923, 8.6755976311516747 39.950235913067175, 8.6755875614920956 39.950344140412739, 8.675587550176493 39.950344262031592, 8.6755867111328389 39.950353279943485, 8.6755866836805726 39.9503535749959, 8.6755862961832744 39.950353553639864, 8.6755862426466468 39.950353756926269, 8.6755856952351262 39.950355835527112, 8.6755863324641425 39.950355913982285, 8.6755849104119775 39.950362755648356, 8.675584793650513 39.950363317401958, 8.67558292961769 39.950372285488932, 8.6755754593189867 39.950408225972986, 8.6755735809647678 39.950417262949649, 8.6755734655508849 39.950417818219087, 8.6755715944298011 39.9504268203933, 8.6755715663459654 39.950426955507773, 8.67556474632694 39.950459767367988, 8.6755004051702542 39.9504270996087))" : value?.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogError(ex.Message, objParametriServer, ex);
        //        throw;
        //    }
        //}

        /// <inheritdoc/>
        //public async Task<string?> GetCentroideWktAsync(
        //    string piva, int saCod, int appezza,
        //    AgronicaCoreParametriServer objParametriServer)
        //{
        //    if (string.IsNullOrWhiteSpace(piva))    throw new ArgumentException("Specificare la partita IVA.",     nameof(piva));
        //    if (saCod <= 0)                         throw new ArgumentException("Specificare il codice Sa_Cod.",   nameof(saCod));
        //    if (appezza <= 0)                       throw new ArgumentException("Specificare il codice Appezza.", nameof(appezza));

        //    const string sql = @"
        //        SELECT TOP 1 cl.Centroide_GeoEntity_WKT
        //        FROM GIS_ElementiGrafici_Clustering cl
        //        INNER JOIN GIS_ElementiGrafici g ON g.ElementoGrafico_Cod = cl.ElementoGrafico_Cod
        //        INNER JOIN GIS_Entita e           ON e.Entita_Cod          = g.Entita_Cod
        //        WHERE e.Piva    = @piva
        //          AND e.Sa_Cod  = @saCod
        //          AND e.Appezza = @appezza";

        //    var sqlParams = new Dictionary<string, object>
        //    {
        //        ["@piva"]    = piva,
        //        ["@saCod"]   = saCod,
        //        ["@appezza"] = appezza
        //    };

        //    try
        //    {
        //        var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, sqlParams);
        //        if (dt.Rows.Count == 0) return null;
        //        var value = dt.Rows[0]["Centroide_GeoEntity_WKT"];
        //        return value == DBNull.Value ? null : value?.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogError(ex.Message, objParametriServer, ex);
        //        throw;
        //    }
        //}

        //public Task<string?> GetCentroideWktAsync(string poligonoWkt)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(poligonoWkt))
        //            return Task.FromResult<string?>(null);

        //        // Estrae il contenuto tra le prime parentesi tonde interne: "lon lat, lon lat, ..."
        //        var start = poligonoWkt.IndexOf('(');
        //        var end = poligonoWkt.LastIndexOf(')');

        //        if (start < 0 || end < 0 || end <= start)
        //            return Task.FromResult<string?>(null);

        //        // Rimuove le parentesi esterne e prende la prima coppia (prima della virgola)
        //        var inner = poligonoWkt.Substring(start + 1, end - start - 1).Trim().Trim('(', ')');
        //        var primaVoce = inner.Split(',')[0].Trim();

        //        // Valida che la coppia contenga esattamente 2 valori numerici
        //        var parti = primaVoce.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        //        if (parti.Length < 2)
        //            return Task.FromResult<string?>(null);

        //        return Task.FromResult<string?>($"POINT({parti[0]} {parti[1]})");
        //    }
        //    catch (Exception ex)
        //    {
        //        LogError(ex.Message, null, ex);
        //        throw;
        //    }
        //}
    }
}
