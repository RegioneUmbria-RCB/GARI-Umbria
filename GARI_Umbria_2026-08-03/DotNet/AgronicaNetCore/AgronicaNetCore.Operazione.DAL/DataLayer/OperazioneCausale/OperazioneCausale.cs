using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Operazione.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Dynamic;
using System.Text;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.OperazioneCausale;

public class OperazioneCausale : BaseDALOperazione, IOperazioneCausale
{
    public OperazioneCausale(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
    }

    public async Task<DataTable> LeggiAsync(int lavCod, DateTime? data, AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();

        stbQuery.AppendLine(" SELECT oc.Id, oc.Causale, oc.Lav_Cod, o.LAV_DES, oc.Validita_Inizio, oc.Validita_Fine ");
        stbQuery.AppendLine(" FROM Operazione_Causale oc ");
        stbQuery.AppendLine(" INNER JOIN Operazioni o on oc.Lav_Cod = o.LAV_COD ");
        stbQuery.AppendLine(" WHERE 1 = 1 ");

        if (lavCod != -1)
            stbQuery.AppendLine("AND oc.Lav_Cod = @lavCod ");

        stbQuery.AppendLine(" AND   oc.Validita_Inizio <= @dtFine ");
        stbQuery.AppendLine(" AND   oc.Validita_Fine >= @dtInizio ");

        stbQuery.AppendLine(" ORDER BY oc.Causale ASC ");

        sqlParams.TryAdd("@lavCod", lavCod);

        if (data.HasValue)
        {
            sqlParams.TryAdd("@dtFine", data);
            sqlParams.TryAdd("@dtInizio", data);
        }
        else
        {
            sqlParams.TryAdd("@dtFine", objParametriServer.FinestraTemporaleFine);
            sqlParams.TryAdd("@dtInizio", objParametriServer.FinestraTemporaleInizio);
        }

        try
        {
            return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<DataTable> Leggi_CausaleDes_From_CausaleId_DALAsync(int id, AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();

        stbQuery.AppendLine(" SELECT oc.Id, oc.Causale ");
        stbQuery.AppendLine(" FROM Operazione_Causale oc ");
        stbQuery.AppendLine(" WHERE 1 = 1 ");

        if (id != -1)
            stbQuery.AppendLine("AND oc.Id = @id ");

        stbQuery.AppendLine(" AND   oc.Validita_Inizio <= @dtFine ");
        stbQuery.AppendLine(" AND   oc.Validita_Fine >= @dtInizio ");

        stbQuery.AppendLine(" ORDER BY oc.Causale ASC ");

        sqlParams.TryAdd("@id", id);
        sqlParams.TryAdd("@dtFine", objParametriServer.FinestraTemporaleFine);
        sqlParams.TryAdd("@dtInizio", objParametriServer.FinestraTemporaleInizio);

        try
        {
            return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<bool> ModificaAsync(int id, string causale, int lavCod, int inviato, DateTime validitaInizio, DateTime validitaFine, AgronicaCoreParametriServer objParametriServer)
    {
        const string sqlString = @"
                    UPDATE Operazione_Causale
                        SET Causale = @causale,
                            Lav_Cod = @lavCod,
                            inviato = @inviato,
                            datainvio = @datainvio,
                            Data_Modifica = @dataModifica,
                            Username_Modifica = @usernameModifica, 
                            Validita_Inizio = @validitaInizio, 
                            Validita_Fine = @validitaFine
                     WHERE Id = @id";

        var expandoObj = new ExpandoObject();
        expandoObj.TryAdd("@id", id);
        expandoObj.TryAdd("@causale", causale);
        expandoObj.TryAdd("@lavCod", lavCod);
        expandoObj.TryAdd("@inviato", inviato);
        expandoObj.TryAdd("@datainvio", DBNull.Value);
        expandoObj.TryAdd("@dataModifica", DateTime.Now);
        expandoObj.TryAdd("@usernameModifica", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@validitaInizio", validitaInizio);
        expandoObj.TryAdd("@validitaFine", validitaFine);

        try
        {
            return await GetDataProvider(objParametriServer).Execute_WriteAsync(sqlString, expandoObj);
        }
        catch
            (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<bool> ScriviAsync(int id,
                                        string causale,
                                        int lavCod,
                                        int inviato,
                                        DateTime validitaInizio, DateTime validitaFine,
                                        AgronicaCoreParametriServer objParametriServer)
    {
        const string sqlString = @"
                    INSERT INTO Operazione_Causale( Id, Causale, Lav_Cod, inviato, datainvio, Data_Creazione, Data_Modifica,
                                                    Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) 
                     VALUES ( @id, @causale, @lavCod, @inviato, @datainvio, @dataCreazione, @dataModifica, 
                             @usernameCreazione, @usernameModifica, @validitaInizio, @validitaFine)
                        ";

        var expandoObj = new ExpandoObject();
        expandoObj.TryAdd("@id", id);
        expandoObj.TryAdd("@causale", causale);
        expandoObj.TryAdd("@lavCod", lavCod);
        expandoObj.TryAdd("@inviato", inviato);
        expandoObj.TryAdd("@datainvio", DBNull.Value);
        expandoObj.TryAdd("@dataCreazione", DateTime.Now);
        expandoObj.TryAdd("@dataModifica", DateTime.Now);
        expandoObj.TryAdd("@usernameCreazione", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@usernameModifica", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@validitaInizio", validitaInizio);
        expandoObj.TryAdd("@validitaFine", validitaFine);

        try
        {
            return await GetDataProvider(objParametriServer).Execute_WriteAsync(sqlString, expandoObj);
        }
        catch
            (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<bool> CancellaAsync(int id, int lavCod, AgronicaCoreParametriServer objParametriServer)
    {
        //verifichiamo che la causale non sia in uso
        DataTable dt;
        var stbQuery = new StringBuilder();

        stbQuery.AppendLine(" SELECT * ");
        stbQuery.AppendLine(" FROM agenda a ");
        stbQuery.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico mdt ON a.piva = mdt.piva  and a.Id_Agenda = mdt.Id_Agenda ");
        stbQuery.AppendLine(" WHERE a.Lav_Cod = @lavCod ");
        stbQuery.AppendLine(" AND mdt.Dett_Cod = @id ");

        var sqlParams = new Dictionary<string, object>();
        sqlParams.TryAdd("@id", id);
        sqlParams.TryAdd("@lavCod", lavCod);

        try
        {
            dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
        }
        catch
            (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }

        if (dt.Rows.Count > 0)
        {
            return false;
        }
        else
        {
            const string sqlDeleteString = @"
                                            DELETE Operazione_Causale
                                            WHERE Id = @id 
                                                AND Lav_Cod = @lavCod";

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@id", id);
            expandoObj.TryAdd("@lavCod", lavCod);

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(sqlDeleteString, expandoObj);
            }
            catch
                (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
