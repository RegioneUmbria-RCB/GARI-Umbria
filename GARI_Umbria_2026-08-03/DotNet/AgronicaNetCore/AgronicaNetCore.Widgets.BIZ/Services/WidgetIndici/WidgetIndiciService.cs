using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using System.Data;
using AgronicaCoreDTOStd.InData.Widgets;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaNetCore.Widgets.BIZ.Resources;
using Microsoft.Extensions.DependencyInjection;
using AgronicaNetCore.Widgets.DAL.DataLayer.WidgetsIndici;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Widgets.BIZ.Services.WidgetIndici;

public class WidgetIndiciService : BaseServiceWidgetsBIZ, IWidgetIndiciService
{
    private readonly IWidgetIndici _widgetIndici;

    public WidgetIndiciService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer)
    {
        _widgetIndici = provider.GetRequiredService<IWidgetIndici>();
    }

    public async Task<List<WidgetKPI>> GetWidgetKpiAsync(uint year, string piva, AgronicaCoreParametriServer objParametriServer)
    {
        var kpi = new List<WidgetKPI>();
        try
        {
            var dt = await _widgetIndici.GetWidgetKpiAsync(year, piva, objParametriServer);
            if (dt == null)
            {
                return kpi;
            }

            foreach (DataRow row in dt.Rows)
            {
                var co2 = new WidgetKPI
                {
                    des = "<ul><li>" + _localizer["WidgetKPI_Co2_Des"] + "</li></ul>",
                    kpi = _localizer["WidgetKPI_Co2"],
                    val = (double)row["indiceCO2NN"] > 5 ? 5 : Convert.ToInt32((double)row["indiceCO2NN"]),
                    icon = "k-i-xi-widget-carbon",
                    udm = "",
                    max = 5,
                    min = 1,
                    indicator = (double)row["indiceCO2NN"] < 4
                        ? (double)row["indiceCO2NN"] > 1
                            ? IndicatoreRatingUnico.Medio
                            : IndicatoreRatingUnico.Basso
                        : IndicatoreRatingUnico.Alto
                };

                var erosione = new WidgetKPI
                {
                    des = "<ul><li>" + _localizer["WidgetKPI_Erosione_Des"] + "</li></ul>",
                    kpi = _localizer["WidgetKPI_Erosione"],
                    val = (double)row["indiceErosioneNN"] > 5 ? 5 : Convert.ToInt32((double)row["indiceErosioneNN"]),
                    icon = "k-i-xi-widget-erosion",
                    udm = "",
                    max = 5,
                    min = 1,
                    indicator = (double)row["indiceErosioneNN"] < 4
                        ? (double)row["indiceErosioneNN"] > 1
                            ? IndicatoreRatingUnico.Medio
                            : IndicatoreRatingUnico.Alto
                        : IndicatoreRatingUnico.Basso
                };

                var plv = new WidgetKPI
                {
                    des = "<ul><li>" + _localizer["WidgetKPI_PLV_Des"] + "</li></ul>",
                    kpi = _localizer["WidgetKPI_PLV"],
                    val = (double)row["indicePlvNN"] > 5 ? 5 : Convert.ToInt32((double)row["indicePlvNN"]),
                    icon = "k-i-xi-widget-productivity",
                    udm = "",
                    max = 5,
                    min = 1,
                    indicator = (double)row["indicePlvNN"] < 4
                        ? (double)row["indicePlvNN"] > 1
                            ? IndicatoreRatingUnico.Medio
                            : IndicatoreRatingUnico.Basso
                        : IndicatoreRatingUnico.Alto
                };

                var meteo = new WidgetKPI
                {
                    des = "<ul><li>" + _localizer["WidgetKPI_Meteo_Des_Produzione"] + "</li></ul>",
                    kpi = _localizer["WidgetKPI_Meteo"],
                    val = Convert.ToInt32((double)row["indiceRischioMeteoAggregatoNN"]),
                    icon = "k-i-xi-widget-weather",
                    udm = "%",
                    max = 100,
                    min = 0,
                    indicator = (double)row["indiceRischioMeteoAggregatoNN"] < 60
                        ? (double)row["indicePlvNN"] > 20
                            ? IndicatoreRatingUnico.Medio
                            : IndicatoreRatingUnico.Alto
                        : IndicatoreRatingUnico.Basso
                };

                kpi.Add(co2);
                kpi.Add(erosione);
                kpi.Add(plv);
                kpi.Add(meteo);
            }
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
        }

        return kpi;
    }

    public async Task<WidgetIndiciProduttivitaGlobal> GetWidgetProduttivitaAsync(
        WidgetRequestIndiciProduttivita request, AgronicaCoreParametriServer objParametriServer)
    {
        var result = new WidgetIndiciProduttivitaGlobal
        {
            indiciXImpresa = new List<WidgetIndiciProduttivitaXImpresa>()
        };

        try
        {
            foreach (var rImp in request.requestXImpresa)
            {
                var indiceXImpresa = await InitializeWidgetIndiciProduttivitaXImpresaAsync(rImp.piva, objParametriServer);
                var indiciXAnno = new List<WidgetIndiciProduttivitaXAnno>();

                foreach (var rAnno in rImp.specieXYear)
                {
                    var indiceXAnno = new WidgetIndiciProduttivitaXAnno
                    {
                        indiciProduttivita = new List<WidgetIndiciProduttivita>(),
                        year = rAnno.year
                    };

                    if (rAnno.specieVegetale.Count == 0)
                    {
                        var indici = await ReadSpecificIndiciProduttivitaAsync(rImp.piva, rAnno.year, objParametriServer);
                        indiceXAnno.indiciProduttivita = indici;
                    }
                    else
                    {
                        foreach (var rSpecie in rAnno.specieVegetale)
                        {
                            var indici = await ReadSpecificIndiciProduttivitaAsync(rImp.piva, rAnno.year,
                                objParametriServer, rSpecie.codice);
                            indiceXAnno.indiciProduttivita = indici;
                        }
                    }

                    var generalIndices = await ReadGeneralIndiciProduttivitaAsync(rImp.piva,
                        rAnno.year, objParametriServer);
                    indiceXAnno.indiciProduttivita = indiceXAnno.indiciProduttivita.Concat(generalIndices).ToList();
                    indiciXAnno.Add(indiceXAnno);
                }

                indiceXImpresa.indiciXAnno = indiciXAnno;
                result.indiciXImpresa.Add(indiceXImpresa);
            }
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
        }

        return result;
    }

    public async Task<List<int>> GetAvailableYearsIndiciProduttivitaAsync(string piva, AgronicaCoreParametriServer objParametriServer)
    {
        var years = new List<int>();
        var dt = await _widgetIndici.ReadAvailableYearsAsync(piva, objParametriServer);

        foreach (DataRow row in dt.Rows)
        {
            var startDate = (DateTime)row["Validita_Inizio"];
            var endDate = (DateTime)row["Validita_Fine"];

            if (!years.Contains(startDate.Year))
            {
                years.Add(startDate.Year);
            }

            if (!years.Contains(endDate.Year))
            {
                years.Add(endDate.Year);
            }
        }

        return years;
    }

    private async Task<WidgetIndiciProduttivitaXImpresa> InitializeWidgetIndiciProduttivitaXImpresaAsync(string piva,
        AgronicaCoreParametriServer objParametriServer)
    {
        var dt = await _widgetIndici.ReadDatiImpresaIndiceProduttivita(piva, objParametriServer);
        var indiciXImpresa = new WidgetIndiciProduttivitaXImpresa
        {
            piva = piva,
            ragSoc = dt.Rows[0]["rag_soc"].ToString(),
            cuaa = dt.Rows[0]["cuaa"].ToString(),
            regione = new BaseCodeDescrStr(dt.Rows[0]["regioneCod"].ToString(), dt.Rows[0]["regioneDes"].ToString())
        };

        return indiciXImpresa;
    }

    private async Task<List<WidgetIndiciProduttivita>> ReadSpecificIndiciProduttivitaAsync(string piva, int year,
        AgronicaCoreParametriServer objParametriServer, int? vegCod = null)
    {
        var indiciProduttivita = new List<WidgetIndiciProduttivita>();
        var dt = await _widgetIndici.ReadSpecificIndiciProduttivitaAsync(piva, year, vegCod, objParametriServer);

        foreach (DataRow row in dt.Rows)
        {
            var varieta = new Varieta
            {
                codice = (int)row["Cul_Cod"],
                descrizione = row["Cul_Des"].ToString(),
                specie = new Specie((int)row["Veg_Cod"], row["Veg_Des"].ToString()),
                classType = "Varieta"
            };

            var idx = new WidgetIndiciProduttivita
            {
                indici = new List<BaseCodeValue<string, int>>(),
                indiceProduttivitaAICod = (int)row["IndiciProduttivitaAi_COD"],
                sup = Convert.ToSingle((double)row["Sup_Imp"]),
                utilizzoTerreno = varieta
            };

            var colsList = new List<DataColumn>();
            for (var i = 6; i < dt.Columns.Count; i++)
            {
                colsList.Add(dt.Columns[i]);
            }

            foreach (var col in colsList)
            {
                var indice = new BaseCodeValue<string, int>(GetIndiceProduttivitaEnum(col.ColumnName).ToString(),
                    row[col.ColumnName] == DBNull.Value ? 0 : Convert.ToInt32((double)row[col.ColumnName]),
                    col.ColumnName);
                idx.indici.Add(indice);
            }

            indiciProduttivita.Add(idx);
        }

        return indiciProduttivita;
    }

    private async Task<List<WidgetIndiciProduttivita>> ReadGeneralIndiciProduttivitaAsync(string piva, int year,
        AgronicaCoreParametriServer objParametriServer)
    {
        var indiciProduttivita = new List<WidgetIndiciProduttivita>();
        var dt = await _widgetIndici.ReadGeneralIndiciProduttivitaAsync(piva, year, objParametriServer);

        foreach (DataRow row in dt.Rows)
        {
            var idx = new WidgetIndiciProduttivita
            {
                indici = new List<BaseCodeValue<string, int>>(),
                indiceProduttivitaAICod = (int)row["IndiciProduttivitaAi_COD"]
            };

            var colsList = new List<DataColumn>();
            for (var i = 1; i < dt.Columns.Count; i++)
            {
                colsList.Add(dt.Columns[i]);
            }

            foreach (var col in colsList)
            {
                var indice = new BaseCodeValue<string, int>(GetIndiceProduttivitaEnum(col.ColumnName).ToString(),
                    row[col.ColumnName] == DBNull.Value ? 0 : Convert.ToInt32((double)row[col.ColumnName]),
                    col.ColumnName);
                idx.indici.Add(indice);
            }

            indiciProduttivita.Add(idx);
        }

        return indiciProduttivita;
    }

    private static int GetIndiceProduttivitaEnum(string indiceDes)
        => indiceDes switch
        {
            "Produttivita" => 1,
            "PLV" => 2,
            "IndiceErosione" => 3,
            "IndiceCO2" => 4,
            "IndiceRischioMeteoAggregato" => 5,
            "IndiceRischioGelata" => 6,
            "IndiceRischioVentoForte" => 7,
            "IndiceRischioSiccita" => 8,
            "IndiceRischioGrandine" => 9,
            "IndiceRischioAllagamento" => 10,
            _ => 0
        };
}