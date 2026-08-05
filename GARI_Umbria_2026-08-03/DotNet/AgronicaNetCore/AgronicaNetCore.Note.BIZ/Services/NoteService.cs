using System.Data;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazione;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.SpecieVegetali;
using AgronicaNetCore.Note.BIZ.Resources;
using AgronicaNetCore.Note.DAL.DataLayer.NoteInterventoUtilizzo;
using AgronicaNetCore.ProfilazioneImprese.DAL.DataLayer.ProfilazioneImprese;
using InData.Note;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Note.BIZ.Services;

public class NoteService : BaseServiceNoteBIZ, INoteService
{
    private readonly INote _noteDal;
    private readonly IProfilazioneImprese _profilazioneImpreseDal;
    private readonly IOperazione _operazioneDal;
    private readonly ISpecieVegetali _specieVegetaliDal;

    public NoteService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
        _noteDal = provider.GetRequiredService<INote>();
        _profilazioneImpreseDal = provider.GetRequiredService<IProfilazioneImprese>();
        _operazioneDal = provider.GetRequiredService<IOperazione>();
        _specieVegetaliDal = provider.GetRequiredService<ISpecieVegetali>();
    }

    public async Task<DataTable> LeggiNoteInterventoUtilizzoAsync(int notaUtilizzoCod, AgronicaCoreParametriServer objParametriServer)
    {
        DataTable dt;
        try
        {
            dt = await _noteDal.LeggiNoteInterventoUtilizzoAsync(notaUtilizzoCod, objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
        return dt;
    }

    public async Task<DataTable> LeggiProfilazioneNoteAsync(int notaUtilizzoCod, int notaGruppoCod, bool soloNoteConUtilizzo,
        string filtroAggiuntivo, AgronicaCoreParametriServer objParametriServer)
    {
        DataTable dt;
        try
        {
            dt = await _noteDal.LeggiProfilazioneNoteAsync(notaUtilizzoCod, notaGruppoCod, soloNoteConUtilizzo,
                Visibilita.Visibili, filtroAggiuntivo, objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
        return dt;
    }

    public async Task<DataTable> LeggiNoteAsync(string piva, int inputLavCod, int inputVegCod,
        AgronicaCoreParametriServer objParametriServer)
    {
        var result = new DataTable();
        var c1 = new DataColumn("notautilizzo_des", typeof(string));
        c1.DefaultValue = "";
        result.Columns.Add(c1);
        var c2 = new DataColumn("nota_des", typeof(string));
        c2.DefaultValue = "";
        result.Columns.Add(c2);
        result.Columns.Add(new DataColumn("notautilizzo_cod", typeof(int)));
        result.Columns.Add(new DataColumn("Lav_cod", typeof(int)));
        result.Columns.Add(new DataColumn("Lav_des", typeof(string)));
        result.Columns.Add(new DataColumn("Veg_cod", typeof(int)));
        result.Columns.Add(new DataColumn("Veg_des", typeof(string)));
        result.Columns.Add(new DataColumn("nota_cods", typeof(string)));
        result.DefaultView.Sort = "notautilizzo_cod";

        var dtNote = await LeggiProfilazioneNoteAsync(piva, "", inputLavCod, inputVegCod, false, false, objParametriServer);
        foreach (DataRow drDati in dtNote.Rows)
        {
            var notaCod = (int)drDati["notautilizzo_cod"];
            var lavCod = (int)drDati["Lav_cod"];
            var vegCod = (int)drDati["Veg_cod"];

            // controllo se esiste già la riga nel dt per l'utilizzo
            var drRes = result.AsEnumerable()
                .Where(note => note.Field<int>("notautilizzo_cod") == notaCod
                               && note.Field<int>("Lav_cod") == lavCod
                               && note.Field<int>("Veg_cod") == vegCod)
                .ToArray();

            var drModIns = drRes.Any() ? drRes[0] : result.NewRow();
            drModIns["nota_des"] += $"° {drDati["nota_des"]}<br />";
            drModIns["nota_cods"] += $"{drDati["nota_cod"]}|";

            // 'controllo se devo inserire la riga come nuova
            if (drRes.Length == 0)
            {
                drModIns["notautilizzo_cod"] = drDati["notautilizzo_cod"];
                drModIns["notautilizzo_des"] =
                    await _noteDal.LeggiNotaUtilizzoDesFromNotaUtilizzoCodAsync((int)drDati["notautilizzo_cod"], objParametriServer);

                drModIns["Lav_cod"] = drDati["Lav_cod"];
                if (int.TryParse(drDati["Lav_cod"].ToString(), out var dbLavCod) && dbLavCod != 0)
                {
                    drModIns["Lav_des"] =
                        await _operazioneDal.LavorazioneDesFromLavorazioneCodAsync((int)drDati["Lav_cod"], objParametriServer);
                }
                else
                {
                    drModIns["Lav_des"] = "Tutte le lavorazioni";
                }

                drModIns["Veg_cod"] = drDati["Veg_cod"];
                if (int.TryParse(drDati["Veg_cod"].ToString(), out var dbVegCod) && dbVegCod != 0)
                {
                    // var objSpec As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                    drModIns["Veg_des"] = await _specieVegetaliDal.VegDesFromVegCodAsync(dbVegCod, objParametriServer);
                }
                else
                {
                    drModIns["Veg_des"] = "Tutte le Specie";
                }

                result.Rows.Add(drModIns);
            }
        }
        return result;
    }

    public async Task<DataTable> LeggiGruppoNoteAsync(AgronicaCoreParametriServer objParametriServer)
    {
        DataTable dt;
        try
        {
            dt = await _noteDal.LeggiGruppoNoteAsync(0, "", "NotaGruppo_Cod ASC", objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
        return dt;
    }

    public async Task<DataTable> LeggiProfilazioneNoteAsync(string piva, string codiceChiave, int lavCod, int vegCod,
        bool filtraVegCodAncheSeZero, bool leggiSoloNoteConLavorazioneNullSeLavCodZero,
        AgronicaCoreParametriServer objParametriServer)
    {
        var filtro = "";
        if (filtraVegCodAncheSeZero)
        {
            filtro = $" Veg_Cod = {vegCod}";
        }

        var dt = await _profilazioneImpreseDal.LeggiProfilazioneImpreseAsync(piva, 0, codiceChiave, "note", filtro, lavCod, 0,
            leggiSoloNoteConLavorazioneNullSeLavCodZero, objParametriServer);

        var dtNote = new DataTable("note");
        dtNote.Columns.Add("notautilizzo_cod", typeof(int));
        dtNote.Columns.Add("nota_cod", typeof(int));
        dtNote.Columns.Add("nota_des", typeof(string));
        dtNote.Columns.Add("Lav_cod", typeof(int));
        dtNote.Columns.Add("Veg_Cod", typeof(int));

        if (dt.Rows.Count <= 0)
        {
            return dtNote;
        }

        // per ogni riga della tabella cerco di ottenere i codici
        foreach (DataRow dr in dt.Rows)
        {
            // PATH DI SALVATAGGIO es: 
            // notautilizzo_cod=107|nota_cod={15,4}
            var dati = dr["Valore_Salvato"].ToString()?.Split("|");
            if (dati is not { Length: > 0 } || !int.TryParse(dati[0].Split("=")[1], out var notaUtilCod))
            {
                continue;
            }

            // cero ci valorizzare gli array
            // al primo posto ho sempre il codice utilizzo, che ho già preso
            foreach (var dato in dati)
            {
                var codici = dato[(dato.IndexOf("{", StringComparison.Ordinal) + 1)..]
                    .Replace("}", "")
                    .Split(",");
                // 'elenco delle note
                foreach (var codice in codici)
                {
                    if (!int.TryParse(codice, out var intCod))
                    {
                        continue;
                    }

                    var drNota = dtNote.NewRow();
                    drNota["notautilizzo_cod"] = notaUtilCod;
                    drNota["nota_cod"] = intCod;
                    drNota["nota_des"] = await _noteDal.LeggiNotaDesFromNotaCodAsync(intCod, objParametriServer);

                    if (dr["Lav_cod"] == DBNull.Value)
                    {
                        drNota["Lav_cod"] = 0;
                    }
                    else
                    {
                        drNota["Lav_cod"] = dr["Lav_cod"];
                    }

                    drNota["Veg_cod"] = dr["Veg_cod"];
                    dtNote.Rows.Add(drNota);
                }
            }
        }

        return dtNote;
    }

    public async Task<bool> SalvaGruppoNoteAsync(SalvaGruppoNote_In body, AgronicaCoreParametriServer objParametriServer)
    {
        bool result;
        try
        {
            var id = body.NotaGruppoCod;
            if (id == 0)
            {
                id = await _noteDal.NoteInterventoGruppiNewIdAsync(objParametriServer);
            }

            result = await _noteDal.SalvaGruppoNoteAsync(id, body.NotaGruppoDes,
                DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE), objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return result;
    }

    public async Task<bool> AggiornaGruppoNoteAsync(SalvaGruppoNote_In body, AgronicaCoreParametriServer objParametriServer)
    {
        if (body.NotaGruppoCod <= 0)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(body.NotaGruppoDes))
        {
            return false;
        }

        bool result;
        try
        {
            result = await _noteDal.AggiornaGruppoNoteAsync(body.NotaGruppoCod, body.NotaGruppoDes,
                DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE), true, objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return result;
    }

    public async Task<bool> CancellaGruppoNoteAsync(int gruppoNoteCod, AgronicaCoreParametriServer objParametriServer)
    {
        if (gruppoNoteCod <= 0)
        {
            return false;
        }

        bool result;
        try
        {
            result = await _noteDal.CancellaGruppoNoteAsync(gruppoNoteCod, objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return result;
    }

    public async Task<DataTable> LeggiNoteXUtilizzoAsync(int notaUtilizzoCod, int notaGruppoCod,
        AgronicaCoreParametriServer objParametriServer)
    {
        DataTable dt;
        try
        {
            dt = await _noteDal.LeggiProfilazioneNoteAsync(notaUtilizzoCod, notaGruppoCod, false, Visibilita.Tutte, "", objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return dt;
    }

    public async Task<bool> AggiungiNotaAsync(string notaDes, int notaGruppoCod, AgronicaCoreParametriServer objParametriServer)
    {
        bool result;
        try
        {
            var notaCod = await _noteDal.NoteNewIdAsync(objParametriServer);
            result = await _noteDal.SalvaNotaAsync(notaCod, notaDes, notaGruppoCod,
                DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE),
                objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return result;
    }

    public async Task<DataTable> LeggiNoteInterventoUtilizzoGruppiAsync(int notaGruppoCod, int notaUtilizzoCod,
        AgronicaCoreParametriServer objParametriServer)
    {
        DataTable result;
        try
        {
            result = await _noteDal.LeggiNoteInterventoUtilizzoGruppiAsync(notaGruppoCod, notaUtilizzoCod, objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
        return result;
    }

    public async Task<bool> SalvaGruppoNoteCompletoAsync(SalvaGruppoNoteCompleto_In body, AgronicaCoreParametriServer objParametriServer)
    {
        try
        {
            await OpenConnectionAsync(objParametriServer);
            if (!await _noteDal.CancellaGruppoNoteUtilizzoAsync(body.NotaGruppoCod, 0, objParametriServer))
            {
                throw new Exception("Impossibile aggiornare i dati relativi all'utilizzo");
            }

            foreach (var utilizzoCod in body.NoteUtilizzoCod)
            {
                if (!await _noteDal.ScriviNoteUtilizzoAsync(body.NotaGruppoCod, utilizzoCod,
                        DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                        DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE), objParametriServer))
                {
                    throw new Exception("Impossibile aggiornare i dati relativi all'utilizzo");
                }
            }

            // SALVO VISIBILITa GRUPPO
            if (!await _noteDal.ModificaVisibilitaGruppoAsync(body.NotaGruppoCod, body.Visible, objParametriServer))
            {
                throw new Exception("Impossibile aggiornare la visibilità del gruppo scelto");
            }

            // SALVO VISIBILITà Singole note
            foreach (var nota in body.NoteVisibili)
            {
                if (!await _noteDal.ModificaVisibilitaNotaAsync(nota.NotaCod, nota.Visible, objParametriServer))
                {
                    throw new Exception("Errore durante l'assegnazione della visibilità di una nota");
                }
            }
        }
        catch (Exception ex)
        {
            CloseTransaction(objParametriServer, true);
            LogError(ex.Message, objParametriServer, ex);
            throw; //'rilancio l'eccezione
        }
        finally
        {
            CloseConnection(objParametriServer);
        }

        return true;
    }

    public async Task<bool> CancellaNotaAsync(int notaCod, AgronicaCoreParametriServer objParametriServer)
    {
        if (notaCod <= 0)
        {
            return false;
        }

        bool result;
        try
        {
            result = await _noteDal.CancellaNotaAsync(notaCod, objParametriServer);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        return result;
    }
}