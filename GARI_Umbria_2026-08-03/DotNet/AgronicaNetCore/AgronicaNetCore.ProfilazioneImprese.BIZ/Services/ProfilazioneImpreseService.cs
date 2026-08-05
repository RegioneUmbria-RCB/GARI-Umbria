using System.Data;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Contatti;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazione;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.ParcoMacchine;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.SpecieVegetali;
using AgronicaNetCore.ProfilazioneImprese.BIZ.Resources;
using AgronicaNetCore.ProfilazioneImprese.DAL.DataLayer.ProfilazioneImprese;
using AgronicaNetCore.ProfilazioneMacchine.DAL.DataLayer.ProfilazioneMacchine;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using InData.Note;
using InData.ProfilazioneImprese;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.ProfilazioneImprese.BIZ.Services;

public class ProfilazioneImpreseService : BaseServiceProfilazioneImpreseBIZ, IProfilazioneImpreseService
{
    private readonly IProfilazioneImprese _profilazioneImprese;
    private readonly IContatti _contatti;
    private readonly IParcoMacchine _parcoMacchine;
    private readonly IProfilazioneMacchine _profilazioneMacchine;
    private readonly IOperazione _operazione;
    private readonly ISpecieVegetali _specieVegetali;
    private readonly IAgro_Sequence _sequence;
    private readonly IUtentiVisibilitaAppoggio _utentiVisibilitaAppoggio;

    public ProfilazioneImpreseService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
        _profilazioneImprese = provider.GetRequiredService<IProfilazioneImprese>();
        _contatti = provider.GetRequiredService<IContatti>();
        _parcoMacchine = provider.GetRequiredService<IParcoMacchine>();
        _profilazioneMacchine = provider.GetRequiredService<IProfilazioneMacchine>();
        _operazione = provider.GetRequiredService<IOperazione>();
        _specieVegetali = provider.GetRequiredService<ISpecieVegetali>();
        _sequence = provider.GetRequiredService<IAgro_Sequence>();
        _utentiVisibilitaAppoggio = provider.GetRequiredService<IUtentiVisibilitaAppoggio>();
    }

    public async Task<DataTable> LeggiAsync(string piva, int idProfiloDati, string codiceChiave, string idGruppo, int lavCod,
        int vegCod, bool leggiSoloNoteConLavorazioneNullSeLavCodZero, AgronicaCoreParametriServer objParametriServer)
    {
        DataTable dt;
        try
        {
            dt = await _profilazioneImprese.LeggiProfilazioneImpreseAsync(piva, idProfiloDati, codiceChiave, idGruppo, "",
                lavCod,
                vegCod, leggiSoloNoteConLavorazioneNullSeLavCodZero, objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return dt;
    }

    public async Task<bool> SalvaProfilazioneAsync(SalvaProfilazione_In body, AgronicaCoreParametriServer objParametriServer)
    {
        var val2Save = "";

        val2Save += "lav_cod=" + body.Lavorazione;
        var macchine = body.Macchine.Length == 0 ? "" : "|mac_cod={" + string.Join(",", body.Macchine) + "}"; // ToolTip
        val2Save += macchine;

        var contatti =
            body.Contatti.Length == 0 ? "" : "|cod_cont={" + string.Join(",", body.Contatti) + "}"; // ToolTip
        val2Save += contatti;

        var imprese = await _profilazioneImprese.LeggiProfilazioneImpreseAsync(body.Piva, 0, body.Lavorazione.ToString(),
            "macXlav", "", body.Lavorazione, body.Specie, true, objParametriServer);
        if (imprese.Rows.Count > 0)
        {
            var val = imprese.Rows[0]["valore_salvato"].ToString()?.Split("/");
            if (val?.Length == 2)
            {
                val2Save += $"/{val[1]}";
            }
        }

        return await ScriviInserisceOAggiornaAsync(body.Piva,
            body.Lavorazione.ToString(),
            "macXlav",
            "Macchine/Contatti per operazioni",
            val2Save,
            DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
            DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE),
            body.Lavorazione,
            body.Specie,
            objParametriServer);
    }

    public async Task<bool> SalvaNoteAsync(SalvaNote_In body, AgronicaCoreParametriServer objParametriServer)
    {
        var val2Save = "notautilizzo_cod=" + body.NotaUtilizzoCod + "|nota_cod={";
        var note = string.Join(",", body.Note) + "}";
        val2Save += note;

        return await ScriviInserisceOAggiornaAsync(body.Piva ?? "",
            body.NotaUtilizzoCod.ToString(),
            "note",
            "Note",
            val2Save,
            DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
            DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE),
            body.Lavorazione,
            body.Specie,
            objParametriServer);
    }

    public async Task<bool> ScriviInserisceOAggiornaAsync(
        string piva,
        string codiceChiave,
        string idGruppo,
        string quesito,
        string valoreSalvato,
        DateTime validitaInizio,
        DateTime validitaFine,
        int lavCod,
        int vegCod,
        AgronicaCoreParametriServer objParametriServer)
    {
        try
        {
            await OpenConnectionAsync(objParametriServer);
            var imprese = await _profilazioneImprese.LeggiProfilazioneImpreseAsync(piva, 0, codiceChiave, idGruppo,
                    "", lavCod, vegCod, true, objParametriServer);
            if (imprese.Rows.Count == 0)
            {
                var idProfiloDati = await _sequence.NuovoId_TabellaAsync("Profilazione_Dati", 0, 2000000000, objParametriServer);
                await _profilazioneImprese.ScriviAsync(piva, idProfiloDati, codiceChiave, quesito, idGruppo,
                    valoreSalvato, validitaInizio, validitaFine, lavCod, vegCod, objParametriServer);
            }
            else
            {
                var idProfiloDati = (int)imprese.Rows[0]["Id_Profilo_Dati"];
                await _profilazioneImprese.ModificaAsync(quesito, valoreSalvato, piva, idProfiloDati, codiceChiave,
                    idGruppo, validitaInizio, validitaFine, lavCod, vegCod, objParametriServer);
            }

            return true;
        }
        catch (Exception ex)
        {
            CloseTransaction(objParametriServer, true);
            LogError(ex.Message, objParametriServer, ex);
            return false;
        }
        finally
        {
            CloseConnection(objParametriServer);
        }
    }

    public async Task<DataTable> LeggiProfilazioneMacchineXContattiAsync(string piva, int vegCodInput,
        AgronicaCoreParametriServer objParametriServer)
    {
        var result = new DataTable();
        result.Columns.Add("lav_des", typeof(string));
        result.Columns.Add("veg_des", typeof(string));
        result.Columns.Add("mac_des", typeof(string));
        result.Columns.Add("mac_cods", typeof(string));
        result.Columns.Add("lav_cod", typeof(int));
        result.Columns.Add("veg_cod", typeof(int));
        result.Columns.Add("cont_des", typeof(string));
        result.Columns.Add("cont_cods", typeof(string));
        result.Columns.Add("ore", typeof(int));
        result.Columns.Add("minuti", typeof(int));
        result.Columns.Add("Id_Profilo_Dati", typeof(int));
        var dataset = await ReadMacchineAndContattiAsync(piva, vegCodInput, objParametriServer);

        foreach (DataTable table in dataset.Tables)
        {
            foreach (DataRow row in table.Rows)
            {
                var lavCod = (int)row["lav_cod"];
                var vegCod = (int)row["veg_cod"];
                var existingRows = result.Select($"lav_cod = {lavCod} AND veg_cod = {vegCod}");

                var newRow = existingRows.Length > 0 ? existingRows[0] : result.NewRow();
                newRow["Id_Profilo_Dati"] = row["Id_Profilo_Dati"];
                newRow["ore"] = row["ore"];
                newRow["minuti"] = row["minuti"];

                if (table.TableName == "macXlav")
                {
                    var strCarM = "";
                    var dtMacchine =
                        await _profilazioneMacchine.LeggiAsync((int)row["Id_Profilo_Dati"], (int)row["mac_cod"], 0, "", "",
                            objParametriServer);
                    for (var i = 0; i < dtMacchine.Rows.Count; i++)
                    {
                        strCarM += $"{dtMacchine.Rows[i]["Mac_Car_Des"]}:{dtMacchine.Rows[i]["valore"]},";
                    }

                    if (strCarM != "")
                    {
                        strCarM = $" ({strCarM[..^1]})";
                    }

                    newRow["mac_des"] += "° " + (row["mac_des"].ToString() == ""
                        ? "<descrizione non disponibile>"
                        : row["mac_des"] + strCarM);
                    newRow["mac_des"] += "<br />";
                    newRow["mac_cods"] += $"{row["mac_cod"]}|";
                }
                else if (table.TableName == "contXlav")
                {
                    newRow["cont_des"] +=
                        $"° {row["rag_soc"]}({row["nome"]} {row["cognome"]}) - {row["rapporto_des"]}<br />";
                    newRow["cont_cods"] += $"{row["cod_risum"]}|";
                }


                // Check if I need to add this as a new line
                if (existingRows.Length == 0)
                {
                    newRow["lav_cod"] = row["lav_cod"];
                    newRow["lav_des"] =
                        await _operazione.LavorazioneDesFromLavorazioneCodAsync((int)row["lav_cod"], objParametriServer); // Operazioni_R
                    newRow["veg_cod"] = row["veg_cod"];

                    if (int.TryParse(row["Veg_cod"].ToString(), out var vegCodParsed) && vegCodParsed != 0)
                    {
                        newRow["Veg_des"] = await _specieVegetali.VegDesFromVegCodAsync(vegCodParsed, objParametriServer);
                    }
                    else
                    {
                        newRow["Veg_des"] = "Tutte le Specie";
                    }

                    result.Rows.Add(newRow);
                }
            }
        }

        return result;
    }

    public async Task<bool> CancellaAsync(string piva, int notaUtilizzoCod, string idGruppo, int lavCod, int vegCod,
        AgronicaCoreParametriServer objParametriServer)
    {
        bool result;
        try
        {
            result = await _profilazioneImprese.CancellaAsync(piva, notaUtilizzoCod.ToString(), idGruppo, lavCod, vegCod,
                objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return result;
    }

    public async Task<bool> PropagaSuTutteLeOperazioniAsync(string piva, int lavCod, int vegCod,
        AgronicaCoreParametriServer objParametriServer)
    {
        try
        {
            await OpenConnectionAsync(objParametriServer);
            var dt = await _profilazioneImprese.LeggiProfilazioneImpreseAsync(piva, 0, "", "macXlav", "", lavCod, vegCod, false,
                objParametriServer);

            // 'cancello
            await _profilazioneImprese.CancellaTutteLavorazioniAsync(piva, "macXlav", vegCod, objParametriServer);

            // 'ciclo per i vari lav cod
            var dtLav = await _operazione.LeggiAsync(0, "C", "GruppoOperazioni.GRU_COD,Operazioni.LAV_DES", objParametriServer);

            foreach (DataRow row in dtLav.Rows)
            {
                var valoreSalvato = dt.Rows[0]["Valore_Salvato"];
                var valoreApp = "lav_cod=" + row["Lav_Cod"];

                for (var j = 1; j < valoreSalvato.ToString()!.Split("|").Length; j++)
                {
                    valoreApp = valoreApp + "|" + valoreSalvato.ToString()!.Split("|")[j];
                }

                var idProfiloDati = await _sequence.NuovoId_TabellaAsync("Profilazione_Dati", 0, 2000000000, objParametriServer);
                await _profilazioneImprese.ScriviAsync(piva, idProfiloDati, row["Lav_Cod"].ToString()!,
                    dt.Rows[0]["Quesito"].ToString()!, "macXlav", valoreApp,
                    DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                    DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE), (int)row["Lav_Cod"],
                    vegCod, objParametriServer);
            }

            return true;
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
    }

    public async Task<bool> AggiornaOreMinutiAsync(AggiornaOreMinuti_In body, AgronicaCoreParametriServer objParametriServer)
    {
        bool result;
        try
        {
            var dt = await _profilazioneImprese.LeggiProfilazioneImpreseAsync(body.Piva, 0, "", "macXlav", "",
                body.LavCod, body.VegCod, false, objParametriServer);

            var dr = dt.Select($"Piva = '{body.Piva}' AND Lav_Cod = {body.LavCod} AND Veg_Cod = {body.VegCod}");
            if (dr.Length == 0)
            {
                return false;
            }

            var valore = dr[0]["Valore_Salvato"].ToString();
            if (valore is null)
            {
                return false;
            }

            valore = $"{valore.Split("/")[0]}/{body.Ore}:{body.Minuti}";
            result = await ScriviInserisceOAggiornaAsync(body.Piva, body.LavCod.ToString(), "macXlav",
                "Macchine/Contatti per operazioni", valore,
                DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE), body.LavCod, body.VegCod, objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return result;
    }

    private async Task<DataSet> ReadMacchineAndContattiAsync(string piva, int vegCod, AgronicaCoreParametriServer objParametriServer)
    {
        var dtMacchine = new DataTable("macXlav");
        var dtContatti = new DataTable("contXlav");
        var ds = CreaLeggiProfilazioneMacchineXContattiDataset(dtMacchine, dtContatti);

        var dt = await _profilazioneImprese.LeggiProfilazioneImpreseAsync(piva, 0, "0", "macXlav", "", 0, vegCod, false,
            objParametriServer);
        foreach (DataRow dr in dt.Rows)
        {
            var dati = dr["Valore_Salvato"].ToString()!.Split("|");
            var oremin = dr["Valore_Salvato"].ToString()!.Split("/");
            var lavCod = int.Parse(dati[0].Split("|")[0].Split("=")[1]);
            var idProfiloDati = dr["Id_Profilo_Dati"];

            for (var c = 1; c < dati.Length; c++)
            {
                var codici = dati[c][(dati[c].IndexOf("{", StringComparison.Ordinal) + 1)..].Replace("}", "")
                    .Split(",");

                if (dati[c].StartsWith("mac_cod"))
                {
                    await ProcessaMacchineAsync(codici, dtMacchine, idProfiloDati, lavCod, oremin, dr, piva, objParametriServer);
                }
                else if (dati[c].StartsWith("cod_cont"))
                {
                    await ProcessaContattiAsync(codici, dtContatti, idProfiloDati, oremin, dr, lavCod, objParametriServer);
                }
            }
        }

        return ds;
    }

    private static DataSet CreaLeggiProfilazioneMacchineXContattiDataset(DataTable dtMacchine, DataTable dtContatti)
    {
        dtMacchine.Columns.Add("Id_Profilo_Dati", typeof(int));
        dtMacchine.Columns.Add("lav_cod", typeof(int));
        dtMacchine.Columns.Add("Veg_cod", typeof(int));
        dtMacchine.Columns.Add("mac_cod", typeof(int));
        dtMacchine.Columns.Add("mac_des", typeof(string));
        dtMacchine.Columns.Add("minuti", typeof(int));
        dtMacchine.Columns.Add("ore", typeof(int));

        dtContatti.Columns.Add("Id_Profilo_Dati", typeof(int));
        dtContatti.Columns.Add("lav_cod", typeof(int));
        dtContatti.Columns.Add("veg_cod", typeof(int));
        dtContatti.Columns.Add("cod_risum", typeof(int));
        dtContatti.Columns.Add("rag_soc", typeof(string));
        dtContatti.Columns.Add("nome", typeof(string));
        dtContatti.Columns.Add("cognome", typeof(string));
        dtContatti.Columns.Add("rapporto_des", typeof(string));
        dtContatti.Columns.Add("minuti", typeof(int));
        dtContatti.Columns.Add("ore", typeof(int));

        var ds = new DataSet();
        ds.Tables.Add(dtMacchine);
        ds.Tables.Add(dtContatti);

        return ds;
    }

    private async Task ProcessaContattiAsync(string[] codici, DataTable dtContatti, object idProfiloDati, string[] oremin,
        DataRow dr, int lavCod, AgronicaCoreParametriServer objParametriServer)
    {
        foreach (var codice in codici)
        {
            if (string.IsNullOrEmpty(codice))
            {
                continue;
            }

            var drC = dtContatti.NewRow();
            drC["Id_Profilo_Dati"] = idProfiloDati;

            if (codice.IndexOf(":", StringComparison.Ordinal) > -1)
            {
                var codMin = codice.Split(":");
                drC["cod_risum"] = codMin[0].Split("/")[0];
            }
            else
            {
                if (int.TryParse(codice, out var codeInt))
                {
                    drC["cod_risum"] = codeInt;
                }
                else
                {
                    break;
                }
            }

            drC["Ore"] = 0;
            drC["Minuti"] = 0;
            if (oremin.Length > 1)
            {
                var orMin = oremin[1].Split("/");
                drC["Ore"] = orMin[0].Split(":")[0];
                drC["Minuti"] = orMin[0].Split(":")[1];
            }

            drC["Veg_Cod"] = dr["Veg_Cod"];

            var dtDes = await _contatti.LeggiFromCodRisUmAsync(drC["cod_risum"].ToString()!,"", objParametriServer);
            if (dtDes.Rows.Count <= 0)
            {
                continue;
            }

            drC["lav_cod"] = lavCod;
            drC["nome"] = dtDes.Rows[0]["nome"];
            drC["cognome"] = dtDes.Rows[0]["cognome"];
            drC["rag_soc"] = dtDes.Rows[0]["rag_soc"];
            drC["rapporto_des"] = dtDes.Rows[0]["rapporto_des"];
            dtContatti.Rows.Add(drC);
        }
    }

    private async Task ProcessaMacchineAsync(string[] codici, DataTable dtMacchine, object idProfiloDati, int lavCod,
        string[] oremin, DataRow dr, string piva, AgronicaCoreParametriServer objParametriServer)
    {

        var dtCentriVisibili = await _utentiVisibilitaAppoggio.ReadAsync((int)TipiEnumerativi.Enum_TipoEntita.Centro, objParametriServer, piva: piva);

        foreach (var codice in codici)
        {
            if (string.IsNullOrEmpty(codice))
            {
                continue;
            }

            var drM = dtMacchine.NewRow();
            drM["Id_Profilo_Dati"] = idProfiloDati;
            drM["lav_cod"] = lavCod;

            if (codice.IndexOf("/", StringComparison.Ordinal) > -1)
            {
                var codMin = codice.Split("/");
                drM["mac_cod"] = int.Parse(codMin[0]);
            }
            else
            {
                drM["mac_cod"] = int.Parse(codice);
            }

            drM["Ore"] = 0;
            drM["Minuti"] = 0;
            if (oremin.Length > 1)
            {
                var orMin = oremin[1].Split(":");
                drM["Ore"] = orMin[0];
                drM["Minuti"] = orMin[1];
            }

            drM["Veg_Cod"] = dr["Veg_Cod"];

            drM["mac_des"] = await _parcoMacchine.LeggiDesFromMacCodAsync(piva, (int)drM["mac_cod"], dtCentriVisibili, objParametriServer);
            dtMacchine.Rows.Add(drM);
        }
    }
}