using System.Data;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Ditte;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.MacchineCaratteristiche;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.ParcoMacchine;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.UnitaMisura;
using AgronicaNetCore.ProfilazioneImprese.DAL.DataLayer.ProfilazioneImprese;
using AgronicaNetCore.ProfilazioneMacchine.BIZ.Resources;
using AgronicaNetCore.ProfilazioneMacchine.DAL.DataLayer.ProfilazioneMacchine;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio;
using InData.ProfilazioneMacchina;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.ProfilazioneMacchine.BIZ.Services;

public class ProfilazioneMacchineService : BaseServiceProfilazioneMacchineBIZ, IProfilazioneMacchineService
{
    private readonly IProfilazioneImprese _profilazioneImpreseDal;
    private readonly IParcoMacchine _parcoMacchineDal;
    private readonly IProfilazioneMacchine _profilazioneMacchineDal;
    private readonly IMacchineCaratteristiche _macchineCaratteristicheDal;
    private readonly IDitte _ditteDal;
    private readonly IUnitaMisura _unitaMisuraDal;
    private readonly IUtentiVisibilitaAppoggio _utentiVisibilitaAppoggioDAL;

    public ProfilazioneMacchineService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
        _profilazioneImpreseDal = provider.GetRequiredService<IProfilazioneImprese>();
        _parcoMacchineDal = provider.GetRequiredService<IParcoMacchine>();
        _profilazioneMacchineDal = provider.GetRequiredService<IProfilazioneMacchine>();
        _macchineCaratteristicheDal = provider.GetRequiredService<IMacchineCaratteristiche>();
        _ditteDal = provider.GetRequiredService<IDitte>();
        _unitaMisuraDal = provider.GetRequiredService<IUnitaMisura>();
    }

    public async Task<DataTable> LeggiAsync(int idProfilazione, AgronicaCoreParametriServer objParametriServer)
    {
        var result = new DataTable();

        result.Columns.Add("Id_Profilo_Dati", typeof(int));
        result.Columns.Add("mac_cod", typeof(int));
        result.Columns.Add("class_code", typeof(string));
        result.Columns.Add("mac_des", typeof(string));
        result.Columns.Add("dettagli", typeof(string));
        result.Columns.Add("caratteristiche", typeof(string));
        result.Columns.Add("elenco_mac_car", typeof(string));

        result.PrimaryKey = new[] { result.Columns["mac_cod"]! };

        try
        {
            var dt = await _profilazioneImpreseDal.LeggiProfilazioneImpreseAsync("0", idProfilazione, "", "macXlav", "", 0,
                0, false, objParametriServer);
            if (dt.Rows.Count == 0)
            {
                return result;
            }

            var dati = dt.Rows[0]["Valore_Salvato"].ToString()?.Split("|");
            if (dati == null || dati.Length == 0)
            {
                return result;
            }

            var strMacCod = dati[1].Split("=")[1];
            strMacCod = strMacCod.Replace("{", "");
            strMacCod = strMacCod.Replace("}", "");

            await Carica_MacchineAsync(result, idProfilazione, strMacCod, objParametriServer);
            return result;
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<bool> UpdateCaratteristicheMacchinaAsync(CaratteristicheMacchina_In body, AgronicaCoreParametriServer objParametriServer)
    {
        try
        {
            var primoRecord = true;
            await OpenConnectionAsync(objParametriServer);

            foreach (var caratteristica in body.Caratteristiche)
            {
                if (string.IsNullOrEmpty(caratteristica.Valore))
                {
                    continue;
                }

                if (primoRecord)
                {
                    await _profilazioneMacchineDal.CancellaAsync(body.IdProfilazione,
                        caratteristica.MacCod, 0, "", objParametriServer);
                    primoRecord = false;
                }

                await _profilazioneMacchineDal.ScriviAsync(body.IdProfilazione,
                    caratteristica.MacCod,
                    caratteristica.MacCarCod,
                    caratteristica.Valore,
                    DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                    DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE), objParametriServer);
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

    private async Task Carica_MacchineAsync(DataTable result, int idProfiloDati, string strMacCods, AgronicaCoreParametriServer objParametriServer)
    {
        // leggo ottengo i macchinari
        var dtCentriVisibili = await _utentiVisibilitaAppoggioDAL.ReadAsync((int)TipiEnumerativi.Enum_TipoEntita.Centro,  objParametriServer, piva: objParametriServer.PivaSuperUser);
        var dtM = await _parcoMacchineDal.LeggiParcoMacchinexSuperUserAsync(objParametriServer.PivaSuperUser, 0, 0, true, $" Parco_Macchine.Mac_Cod IN ({strMacCods}) ", "mac_des", dtCentriVisibili, objParametriServer);
        if (dtM is not { Rows.Count: > 0 })
        {
            return;
        }

        // leggo ottengo i macchinari
        var dtMp = await _profilazioneMacchineDal.LeggiAsync(idProfiloDati, 0, 0,
            $" Mac_Cod IN ({strMacCods}) ", "", objParametriServer);

        // CARATTERISTICHE MACCHINE

        var classCodes = dtM.DefaultView.ToTable(true, "class_code");
        var strClassCode = "";
        if (classCodes is { Rows.Count: > 0 })
        {
            foreach (DataRow row in classCodes.Rows)
            {
                strClassCode += $"'{row["class_code"]}',";
            }
        }

        DataTable? dtCar = null;
        if (strClassCode != "")
        {
            dtCar = await _macchineCaratteristicheDal.LeggiAsync(0, "", $" CLASS_CODE IN ({strClassCode[..^1]}) ", "", objParametriServer);
        }

        for (var i = 0; i < dtM.Rows.Count; i++)
        {
            var dettagli = "";

            var macCod = (int)dtM.Rows[i]["mac_cod"];
            var macDes = dtM.Rows[i]["mac_des"].ToString();
            var classCode = dtM.Rows[i]["class_code"].ToString();

            // // stringa con le info....
            // var info As String = "<b>" + des + "</b><br/>"
            // ditta..
            if (int.TryParse(dtM.Rows[i]["ditta_cod"].ToString(), out var dittaCod) && dittaCod > 0)
            {
                var dtDitta = await _ditteDal.LeggiAsync(dittaCod, "", "", "", objParametriServer);
                if (dtDitta.Rows.Count > 0)
                {
                    dettagli += $"Ditta: {dtDitta.Rows[0]["ditta_des"]}<br/>";
                }
            }

            // modello
            if (!string.IsNullOrEmpty(dtM.Rows[i]["modello"].ToString()))
            {
                dettagli += $"Modello: {dtM.Rows[i]["modello"]}<br/>";
            }

            // potenza
            if (!string.IsNullOrEmpty(dtM.Rows[i]["potenza"].ToString()))
            {
                dettagli += $"Potenza: {dtM.Rows[i]["potenza"]}<br/>";
            }

            // udm...
            if (int.TryParse(dtM.Rows[i]["potenza_udm_cod"].ToString(), out var potenzaUdmCod) && potenzaUdmCod > 0)
            {
                var udm = await _unitaMisuraDal.LeggiAsync(potenzaUdmCod, 0, objParametriServer);
                dettagli += $"Potenza udm: {udm.Rows[0]["udm_des"]}<br/>";
            }

            // targa
            if (!string.IsNullOrEmpty(dtM.Rows[i]["targa"].ToString()))
            {
                dettagli += $"Targa: {dtM.Rows[i]["targa"]}<br/>";
            }

            // date...
            var ultimaManutenzione = (DateTime)dtM.Rows[i]["ultima_manutenzione"];
            if (ultimaManutenzione != DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO))
            {
                dettagli += $"Ultima manutenzione: {ultimaManutenzione.ToShortDateString()}<br/>";
            }

            var ultimaRevisione = (DateTime)dtM.Rows[i]["ultima_revisione"];
            if (ultimaRevisione != DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO))
            {
                dettagli += $"Ultima revisione: {ultimaRevisione.ToShortDateString()}<br/>";
            }

            var elencoMacCar = "";
            var caratteristiche = "";

            var drCar = dtCar?.Select($"class_code='{classCode}'");
            if (drCar is { Length: > 0 })
            {
                foreach (var row in drCar)
                {
                    var macCarDes = row["Mac_Car_Des"];
                    var macCarCod = row["Mac_Car_Cod"];
                    var caratteristicaValore = "";

                    if (dtMp is { Rows.Count: > 0 })
                    {
                        var drMp = dtMp.Select("PivaSuperUser='" + objParametriServer.PivaSuperUser + "'" +
                                               " AND Id_Profilo_Dati=" + idProfiloDati +
                                               " AND mac_cod=" + macCod +
                                               " AND Mac_Car_Cod=" + macCarCod);
                        if (drMp is { Length: > 0 })
                        {
                            caratteristicaValore = drMp[0]["valore"].ToString();
                        }
                    }

                    caratteristiche += $"{macCarDes}:{caratteristicaValore}<br/>";
                    elencoMacCar += $"{macCarCod}={macCarDes}:{caratteristicaValore}|";
                }
            }

            if (elencoMacCar != "")
            {
                elencoMacCar = elencoMacCar[..^1];
            }

            var newRow = result.NewRow();
            newRow["Id_Profilo_Dati"] = idProfiloDati;
            newRow["mac_cod"] = macCod;
            newRow["class_code"] = classCode;
            newRow["mac_des"] = macDes;
            newRow["dettagli"] = dettagli;
            newRow["caratteristiche"] = caratteristiche;
            newRow["elenco_mac_car"] = elencoMacCar;

            result.Rows.Add(newRow);
        }
    }
}