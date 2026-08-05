using System.Data;
using System.Globalization;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DefaultPianiColturali.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.SpecieVegetali;
using AgronicaNetCore.SpecieVegetaliDefault.DAL.DataLayer.SpecieVegetaliDefault;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using InData.DefaultPianiColturali;
using InData.SpecieVegetali;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OutData.DefaultPianiColturali;

namespace AgronicaNetCore.DefaultPianiColturali.BIZ.Services;

public class DefaultPianiColturaliService : BaseServiceDefaultPianiColturaliBIZ, IDefaultPianiColturaliService
{
    const string SeminaDt = "semina_";
    const string FiorituraDt = "fioritura_";
    const string RaccoltaDt = "raccolta_";

    private readonly ISpecieVegetaliDefault _specieVegetaliDefault;
    private readonly IAgro_Sequence _sequenceDal;
    private readonly ISpecieVegetali _specieVegetali;

    public DefaultPianiColturaliService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
        _specieVegetali = provider.GetRequiredService<ISpecieVegetali>();
        _specieVegetaliDefault = provider.GetRequiredService<ISpecieVegetaliDefault>();
        _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
    }

    public async Task<DefaultDistintaProduzione_Out> LeggiDefaultDistintaDiProduzioneAsync(string piva, int vegCod,
        AgronicaCoreParametriServer objParametriServer)
    {
        var result = new DefaultDistintaProduzione_Out
        {
            GruCod = 0,
            NCicli = 0,
            Data = new DataTable(),
        };

        var dt = await _specieVegetali.LeggiCompletaAsync(vegCod, 0, objParametriServer);
        if (dt.Rows.Count <= 0)
        {
            return result;
        }

        result.GruCod = (int)dt.Rows[0]["gru_cod"];
        result.NCicli = await _specieVegetaliDefault.CicliCulturaliAsync(piva, true, vegCod,
            DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
            DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE), "", objParametriServer);
        if (result.NCicli == 0)
        {
            result.NCicli = 1;
        }

        if (result.NCicli > 0)
        {
            var data = await _specieVegetaliDefault.LeggiAsync(piva, vegCod, 0, 0, 0,
                DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE), "", objParametriServer);

            result.Data = CreaDistintaDataTable(data, result.NCicli, result.GruCod);
        }

        return result;
    }

    public async Task<DataTable> LeggiDefaultGeneraleSpecieAsync(string piva, AgronicaCoreParametriServer objParametriServer)
    {
        try
        {
            var result = new DataTable();
            result.Columns.Add(new DataColumn("Veg_Cod", typeof(int)));
            result.Columns.Add(new DataColumn("Cul_Cod", typeof(int)));
            result.Columns.Add(new DataColumn("Veg_Des", typeof(string)));
            result.Columns.Add(new DataColumn("Cul_des", typeof(string)));
            result.Columns.Add(new DataColumn("tipo_maturazione", typeof(string)));
            result.Columns.Add(new DataColumn("Soglia_Minima", typeof(string)));
            result.Columns.Add(new DataColumn("resa_stabilimento", typeof(string)));
            result.Columns.Add(new DataColumn("peso_sgocciolato", typeof(string)));

            var filter = " SpecieVegetali_Default.Cul_cod = 0 AND SpecieVegetali_Default.Codice in (" +
                         (int)TipiEnumerativi.EnumCodiciDefault.PesoSgocciolato + "," +
                         (int)TipiEnumerativi.EnumCodiciDefault.ResaStabilimento + "," +
                         (int)TipiEnumerativi.EnumCodiciDefault.SogliaMinima + "," +
                         (int)TipiEnumerativi.EnumCodiciDefault.TipoMaturazione + ")";

            var dtCarica = await _specieVegetaliDefault.LeggiAsync(piva, 0, 0, 0, 0,
                DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE), filter, objParametriServer);

            if (dtCarica.Rows.Count <= 0)
            {
                return result;
            }

            // 'identifico il numero di righe  (Veg_Cod cul_cod differenti)
            var vegCods = new HashSet<string>();
            for (var i = 0; i < dtCarica.Rows.Count; i++)
            {
                var vegCod = dtCarica.Rows[i]["Veg_Cod"].ToString();
                if (vegCod != null)
                {
                    vegCods.Add(vegCod);
                }
            }

            // 'scorro tutte le specie e varietà presenti nel db
            foreach (var vegCod in vegCods)
            {
                // 'filtro 
                var dr = dtCarica.Select(" Veg_Cod =" + vegCod.Split("_")[0]);
                var drNew = result.NewRow();

                drNew["Veg_Cod"] = dr[0]["Veg_Cod"];
                drNew["Cul_Cod"] = 0;

                drNew["Veg_Des"] = dr[0]["Veg_Des"];
                if (dr[0]["Cul_des"] != DBNull.Value)
                {
                    drNew["Cul_des"] = dr[0]["Cul_des"];
                }
                else
                {
                    drNew["Cul_des"] = "Tutte le Varietà";
                }

                foreach (var row in dr)
                {
                    // to string and to int because otherwise we read int16 instead of int32
                    var nomeColonna = int.Parse(row["Codice"].ToString()!) switch
                    {
                        (int)TipiEnumerativi.EnumCodiciDefault.TipoMaturazione => "tipo_maturazione",
                        (int)TipiEnumerativi.EnumCodiciDefault.PesoSgocciolato => "peso_sgocciolato",
                        (int)TipiEnumerativi.EnumCodiciDefault.ResaStabilimento => "resa_stabilimento",
                        (int)TipiEnumerativi.EnumCodiciDefault.SogliaMinima => "Soglia_Minima",
                        _ => ""
                    };

                    if (!string.IsNullOrEmpty(nomeColonna))
                    {
                        drNew[nomeColonna] = row["valore"];
                    }
                }

                result.Rows.Add(drNew);
            }

            return result;
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<bool> ScriviSpecieVegetaleAsync(SpecieVegetali_In body, AgronicaCoreParametriServer objParametriServer)
    {
        try
        {
            var dtInitio = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO);
            var dtFine = DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE);
            await OpenConnectionAsync(objParametriServer);
            foreach (var data in body.Data)
            {
                //OpenTransaction(objParams);
                await _specieVegetaliDefault.CancellaAsync(body.Piva, data.VegCod, 0,
                    " Codice in (" + (int)TipiEnumerativi.EnumCodiciDefault.TipoMaturazione +
                    " , " + (int)TipiEnumerativi.EnumCodiciDefault.SogliaMinima +
                    " , " + (int)TipiEnumerativi.EnumCodiciDefault.ResaStabilimento +
                    " , " + (int)TipiEnumerativi.EnumCodiciDefault.PesoSgocciolato + ")", objParametriServer);


                await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, 0,
                    (int)TipiEnumerativi.EnumCodiciDefault.TipoMaturazione,
                    data.TipoMaturazione.ToString(CultureInfo.InvariantCulture), 0, dtInitio, dtFine, objParametriServer);

                await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, 0,
                    (int)TipiEnumerativi.EnumCodiciDefault.SogliaMinima,
                    data.SogliaMinima.ToString(CultureInfo.InvariantCulture), 0, dtInitio, dtFine, objParametriServer);

                await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, 0,
                    (int)TipiEnumerativi.EnumCodiciDefault.ResaStabilimento,
                    data.ResaStabilimento.ToString(CultureInfo.InvariantCulture), 0, dtInitio, dtFine, objParametriServer);

                await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, 0,
                    (int)TipiEnumerativi.EnumCodiciDefault.PesoSgocciolato,
                    data.PesoSgocciolato.ToString(CultureInfo.InvariantCulture), 0, dtInitio, dtFine, objParametriServer);
            }
        }
        catch (Exception ex)
        {
            CloseTransaction(objParametriServer, true);
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
        finally
        {
            CloseConnection(objParametriServer);
        }

        return true;
    }

    public async Task<bool> CancellaSpecieVegetaleAsync(string piva, int vegCod, AgronicaCoreParametriServer objParametriServer)
    {
        try
        {
            await _specieVegetaliDefault.CancellaAsync(piva, vegCod, 0,
                " Codice in (" + (int)TipiEnumerativi.EnumCodiciDefault.TipoMaturazione +
                " , " + (int)TipiEnumerativi.EnumCodiciDefault.SogliaMinima +
                " , " + (int)TipiEnumerativi.EnumCodiciDefault.ResaStabilimento +
                " , " + (int)TipiEnumerativi.EnumCodiciDefault.PesoSgocciolato + ")", objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return true;
    }

    public async Task<bool> ScriviDefaultGeneraliInizialiAsync(DefaultGeneraliColtura_In body,
        AgronicaCoreParametriServer objParametriServer)
    {
        if (body.CulCod == 0)
        {
            return false;
        }

        try
        {
            var dt = await _specieVegetaliDefault.LeggiAsync(body.Piva, body.VegCod, body.CulCod, 0, 0,
                DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE), " SpecieVegetali_Default.Cul_Cod <> 0 ",
                objParametriServer);

            if (dt.Rows.Count > 0)
            {
                return true;
            }

            await ScriviSpecieDefaultOrThrowAsync(body.Piva, body.VegCod, 0, (int)TipiEnumerativi.EnumCodiciDefault.Gg,
                "0.0", 0, DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE), objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return true;
    }

    public async Task<bool> ScriviDefaultSpecieInizialiAsync(DefaultGeneraliColtura_In body,
        AgronicaCoreParametriServer objParametriServer)
    {
        try
        {
            var filter = " SpecieVegetali_Default.Codice in (" +
                         (int)TipiEnumerativi.EnumCodiciDefault.PesoSgocciolato + "," +
                         (int)TipiEnumerativi.EnumCodiciDefault.ResaStabilimento + "," +
                         (int)TipiEnumerativi.EnumCodiciDefault.SogliaMinima + "," +
                         (int)TipiEnumerativi.EnumCodiciDefault.TipoMaturazione + ")";
            var dt = await _specieVegetaliDefault.LeggiAsync(body.Piva, body.VegCod, 0, 0, 0,
                DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE), filter, objParametriServer);

            if (dt.Rows.Count > 0)
            {
                return true;
            }

            await ScriviSpecieDefaultOrThrowAsync(body.Piva, body.VegCod, 0, (int)TipiEnumerativi.EnumCodiciDefault.SogliaMinima,
                "0.0", 0, DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE), objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return true;
    }

    public async Task<DataTable> LeggiDefaultGeneraliAsync(string piva, AgronicaCoreParametriServer objParametriServer)
    {
        try
        {
            var result = await GeneraDtDefaultGeneraleAsync(piva, objParametriServer);
            return result;
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<bool> ScriviDefaultGeneraliAsync(DefaultGenerali_In body, AgronicaCoreParametriServer objParametriServer)
    {
        try
        {
            var dtInitio = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO);
            var dtFine = DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE);
            await OpenConnectionAsync(objParametriServer);
            foreach (var data in body.Data)
            {
                //OpenTransaction(objParams);
                await _specieVegetaliDefault.CancellaAsync(body.Piva, data.VegCod, data.CulCod,
                    " Codice in (" + (int)TipiEnumerativi.EnumCodiciDefault.Cal1 +
                    " , " + (int)TipiEnumerativi.EnumCodiciDefault.Cal2 +
                    " , " + (int)TipiEnumerativi.EnumCodiciDefault.Cal3 +
                    " , " + (int)TipiEnumerativi.EnumCodiciDefault.Cal4 +
                    " , " + (int)TipiEnumerativi.EnumCodiciDefault.Cal5 +
                    " , " + (int)TipiEnumerativi.EnumCodiciDefault.Cal6 +
                    " , " + (int)TipiEnumerativi.EnumCodiciDefault.Gg +
                    " , " + (int)TipiEnumerativi.EnumCodiciDefault.Resa +
                    " , " + (int)TipiEnumerativi.EnumCodiciDefault.UnitaCalore +
                    " , " + (int)TipiEnumerativi.EnumCodiciDefault.DosiHa +
                    " , " + (int)TipiEnumerativi.EnumCodiciDefault.DosiHaUdm + ")", objParametriServer);


                await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, data.CulCod,
                    (int)TipiEnumerativi.EnumCodiciDefault.UnitaCalore,
                    data.UnitaCalore.ToString(CultureInfo.InvariantCulture), 0, dtInitio, dtFine, objParametriServer);

                await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, data.CulCod,
                    (int)TipiEnumerativi.EnumCodiciDefault.Gg,
                    data.Gg.ToString(CultureInfo.InvariantCulture), 0, dtInitio, dtFine, objParametriServer);

                await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, data.CulCod,
                    (int)TipiEnumerativi.EnumCodiciDefault.Resa,
                    data.Resa.ToString(CultureInfo.InvariantCulture), 0, dtInitio, dtFine, objParametriServer);

                await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, data.CulCod,
                    (int)TipiEnumerativi.EnumCodiciDefault.DosiHa,
                    data.DosiHa.ToString(CultureInfo.InvariantCulture), 0, dtInitio, dtFine, objParametriServer);

                await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, data.CulCod,
                    (int)TipiEnumerativi.EnumCodiciDefault.DosiHaUdm,
                    data.DosiHaUdm.ToString(CultureInfo.InvariantCulture), 0, dtInitio, dtFine, objParametriServer);

                await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, data.CulCod,
                    (int)TipiEnumerativi.EnumCodiciDefault.Cal1,
                    data.Cal1.ToString(CultureInfo.InvariantCulture), 0, dtInitio, dtFine, objParametriServer);

                await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, data.CulCod,
                    (int)TipiEnumerativi.EnumCodiciDefault.Cal2,
                    data.Cal2.ToString(CultureInfo.InvariantCulture), 0, dtInitio, dtFine, objParametriServer);

                await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, data.CulCod,
                    (int)TipiEnumerativi.EnumCodiciDefault.Cal3,
                    data.Cal3.ToString(CultureInfo.InvariantCulture), 0, dtInitio, dtFine, objParametriServer);

                await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, data.CulCod,
                    (int)TipiEnumerativi.EnumCodiciDefault.Cal4,
                    data.Cal4.ToString(CultureInfo.InvariantCulture), 0, dtInitio, dtFine, objParametriServer);

                await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, data.CulCod,
                    (int)TipiEnumerativi.EnumCodiciDefault.Cal5,
                    data.Cal5.ToString(CultureInfo.InvariantCulture), 0, dtInitio, dtFine, objParametriServer);

                await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, data.CulCod,
                    (int)TipiEnumerativi.EnumCodiciDefault.Cal6,
                    data.Cal6.ToString(CultureInfo.InvariantCulture), 0, dtInitio, dtFine, objParametriServer);
            }
        }
        catch (Exception ex)
        {
            CloseTransaction(objParametriServer, true);
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
        finally
        {
            CloseConnection(objParametriServer);
        }

        return true;
    }

    public async Task<bool> CancellaDefaultGeneraleAsync(string piva, int vegCod, int culCod,
        AgronicaCoreParametriServer objParametriServer)
    {
        try
        {
            await _specieVegetaliDefault.CancellaAsync(piva, vegCod, culCod,
                " Codice in (" + (int)TipiEnumerativi.EnumCodiciDefault.Cal1 +
                " , " + (int)TipiEnumerativi.EnumCodiciDefault.Cal2 +
                " , " + (int)TipiEnumerativi.EnumCodiciDefault.Cal3 +
                " , " + (int)TipiEnumerativi.EnumCodiciDefault.Cal4 +
                " , " + (int)TipiEnumerativi.EnumCodiciDefault.Cal5 +
                " , " + (int)TipiEnumerativi.EnumCodiciDefault.Cal6 +
                " , " + (int)TipiEnumerativi.EnumCodiciDefault.Gg +
                " , " + (int)TipiEnumerativi.EnumCodiciDefault.Resa +
                " , " + (int)TipiEnumerativi.EnumCodiciDefault.DosiHa +
                " , " + (int)TipiEnumerativi.EnumCodiciDefault.DosiHaUdm +
                " , " + (int)TipiEnumerativi.EnumCodiciDefault.UnitaCalore + ")", objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return true;
    }

    public async Task<bool> SalvaDefaultDistintaDiProduzioneAsync(DistintaProduzione_In body, AgronicaCoreParametriServer objParametriServer)
    {
        try
        {
            var dtInitio = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO);
            var dtFine = DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE);
            await OpenConnectionAsync(objParametriServer);

            if (!await _specieVegetaliDefault.CancellaAsync(body.Piva, body.VegCod, -1, "", objParametriServer))
            {
                throw new Exception("Impossibile cancellare defalt vegetali");
            }

            foreach (var data in body.Data)
            {
                foreach (var value in data.Values)
                {
                    if (!string.IsNullOrEmpty(value.DataFioritura))
                    {
                        await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, data.CulCod,
                            (int)TipiEnumerativi.EnumCodiciDefault.Fioritura,
                            value.DataFioritura, value.NCiclo, dtInitio, dtFine, objParametriServer);
                    }

                    if (!string.IsNullOrEmpty(value.DataSemina))
                    {
                        await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, data.CulCod,
                            (int)TipiEnumerativi.EnumCodiciDefault.Semina,
                            value.DataSemina, value.NCiclo, dtInitio, dtFine, objParametriServer);
                    }

                    if (!string.IsNullOrEmpty(value.DataRaccolta))
                    {
                        await ScriviSpecieDefaultOrThrowAsync(body.Piva, data.VegCod, data.CulCod,
                            (int)TipiEnumerativi.EnumCodiciDefault.Raccolta,
                            value.DataRaccolta, value.NCiclo, dtInitio, dtFine, objParametriServer);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            CloseTransaction(objParametriServer, true);
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
        finally
        {
            CloseConnection(objParametriServer);
        }

        return true;
    }

    private async Task ScriviSpecieDefaultOrThrowAsync(string piva, int vegCod, int culCod, int codice, string valore, int nCiclo,
        DateTime dtInitio, DateTime dtFine, AgronicaCoreParametriServer objParametriServer)
    {
        var id = await _sequenceDal.NuovoId_TabellaAsync("SpecieVegetali_Default", 0, 2000000000, objParametriServer);
        var ok = await _specieVegetaliDefault.ScriviAsync(id, piva, vegCod, culCod, codice,
            valore, nCiclo, dtInitio, dtFine, objParametriServer);
        if (!ok)
        {
            throw new Exception("Errore nell'inserimento di un nuovo default");
        }
    }

    private async Task<DataTable> GeneraDtDefaultGeneraleAsync(string piva, AgronicaCoreParametriServer objParametriServer)
    {
        var result = new DataTable();
        result.Columns.Add(new DataColumn("Veg_Cod", typeof(int)));
        result.Columns.Add(new DataColumn("Cul_Cod", typeof(int)));
        result.Columns.Add(new DataColumn("Veg_Des", typeof(string)));
        result.Columns.Add(new DataColumn("Cul_des", typeof(string)));
        result.Columns.Add(new DataColumn("unita_calore", typeof(string)));
        result.Columns.Add(new DataColumn("gg", typeof(string)));
        result.Columns.Add(new DataColumn("resa", typeof(string)));
        result.Columns.Add(new DataColumn("dosi_ha", typeof(string)));
        result.Columns.Add(new DataColumn("dosi_ha_udm", typeof(string)));
        result.Columns.Add(new DataColumn("cal_1", typeof(string)));
        result.Columns.Add(new DataColumn("cal_2", typeof(string)));
        result.Columns.Add(new DataColumn("cal_3", typeof(string)));
        result.Columns.Add(new DataColumn("cal_4", typeof(string)));
        result.Columns.Add(new DataColumn("cal_5", typeof(string)));
        result.Columns.Add(new DataColumn("cal_6", typeof(string)));

        var dtCarica = await _specieVegetaliDefault.LeggiAsync(piva, 0, 0, 0, 0,
            DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
            DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE),
            " SpecieVegetali_Default.Cul_Cod <> 0 ", objParametriServer);
        if (dtCarica.Rows.Count <= 0)
        {
            return result;
        }

        var vegCulDistinct = new HashSet<string>();
        for (var i = 0; i < dtCarica.Rows.Count; i++)
        {
            var vegCod = dtCarica.Rows[i]["Veg_Cod"];
            var culCod = dtCarica.Rows[i]["Cul_Cod"];
            vegCulDistinct.Add($"{vegCod}_{culCod}");
        }

        // scorro tutte le specie e varietà presenti nel db
        foreach (var element in vegCulDistinct)
        {
            // filtro
            var cods = element.Split("_");
            var dr = dtCarica.Select($" Veg_Cod = {cods[0]} AND Cul_Cod = {cods[1]}");
            var drNew = result.NewRow();

            drNew["Veg_Cod"] = dr[0]["Veg_Cod"];
            drNew["Cul_Cod"] = dr[0]["Cul_Cod"];
            drNew["Veg_Des"] = dr[0]["Veg_Des"];
            if (dr[0]["Cul_des"] != DBNull.Value)
            {
                drNew["Cul_des"] = dr[0]["Cul_des"];
            }
            else
            {
                drNew["Cul_des"] = "Tutte le Varietà";
            }


            foreach (var row in dr)
            {
                // to string and to int because otherwise we read int16 instead of int32
                var columnName = int.Parse(row["Codice"].ToString()!) switch
                {
                    (int)TipiEnumerativi.EnumCodiciDefault.UnitaCalore => "Unita_Calore",
                    (int)TipiEnumerativi.EnumCodiciDefault.Gg => "gg",
                    (int)TipiEnumerativi.EnumCodiciDefault.Resa => "Resa",
                    (int)TipiEnumerativi.EnumCodiciDefault.DosiHa => "dosi_ha",
                    (int)TipiEnumerativi.EnumCodiciDefault.DosiHaUdm => "dosi_ha_udm",
                    (int)TipiEnumerativi.EnumCodiciDefault.Cal1 => "Cal_1",
                    (int)TipiEnumerativi.EnumCodiciDefault.Cal2 => "Cal_2",
                    (int)TipiEnumerativi.EnumCodiciDefault.Cal3 => "Cal_3",
                    (int)TipiEnumerativi.EnumCodiciDefault.Cal4 => "Cal_4",
                    (int)TipiEnumerativi.EnumCodiciDefault.Cal5 => "Cal_5",
                    (int)TipiEnumerativi.EnumCodiciDefault.Cal6 => "Cal_6",
                    _ => ""
                };

                if (columnName != "")
                {
                    drNew[columnName] = row["valore"];
                }
            }

            result.Rows.Add(drNew);
        }

        return result;
    }

    private DataTable CreaDistintaDataTable(DataTable data, int nCicli, int gruCod)
    {
        var result = new DataTable();
        result.Columns.Add(new DataColumn("Veg_Cod", typeof(int)));
        result.Columns.Add(new DataColumn("Cul_Cod", typeof(int)));
        result.Columns.Add(new DataColumn("Veg_Des", typeof(string)));
        result.Columns.Add(new DataColumn("Cul_des", typeof(string)));

        for (var i = 1; i <= nCicli; i++)
        {
            if (gruCod == 3)
            {
                result.Columns.Add(new DataColumn($"{SeminaDt}{i}", typeof(string)));
            }

            result.Columns.Add(new DataColumn($"{FiorituraDt}{i}", typeof(string)));
            result.Columns.Add(new DataColumn($"{RaccoltaDt}{i}", typeof(string)));
        }

        if (data.Rows.Count <= 0)
        {
            return result;
        }

        var culCods = new List<int> { (int)data.Rows[0]["Cul_Cod"] };
        for (var i = 1; i < data.Rows.Count; i++)
        {
            if ((int)data.Rows[i]["Cul_Cod"] != culCods[^1])
            {
                culCods.Add((int)data.Rows[i]["Cul_Cod"]);
            }
        }

        foreach (var culCod in culCods)
        {
            var dr = data.Select($" Cul_Cod = {culCod}");

            var drNew = result.NewRow();

            drNew["Veg_Cod"] = dr[0]["Veg_Cod"];
            drNew["Cul_Cod"] = dr[0]["Cul_Cod"];

            drNew["Veg_Des"] = dr[0]["Veg_Des"];
            if (dr[0]["Cul_des"] != DBNull.Value)
            {
                drNew["Cul_des"] = dr[0]["Cul_des"];
            }
            else
            {
                drNew["Cul_des"] = "Tutte le Varietà";
            }

            foreach (var row in dr)
            {
                var nCiclo = (int)row["Numero_Ciclo"];

                // to string and to int because otherwise we read int16 instead of int32
                var nomeColonna = int.Parse(row["Codice"].ToString()!) switch
                {
                    (int)TipiEnumerativi.EnumCodiciDefault.Fioritura => FiorituraDt + nCiclo,
                    (int)TipiEnumerativi.EnumCodiciDefault.Raccolta => RaccoltaDt + nCiclo,
                    (int)TipiEnumerativi.EnumCodiciDefault.Semina => SeminaDt + nCiclo,
                    _ => ""
                };

                if (!string.IsNullOrEmpty(nomeColonna))
                {
                    drNew[nomeColonna] = row["valore"];
                }
            }

            result.Rows.Add(drNew);
        }

        return result;
    }
}