using System.Data;
using System.Dynamic;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SpecieVegetaliDefault.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SpecieVegetaliDefault.DAL.DataLayer.SpecieVegetaliDefault;

public class SpecieVegetaliDefault : BaseDALSpecieVegetaliDefault, ISpecieVegetaliDefault
{
    public SpecieVegetaliDefault(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
    }

    public async Task<int> CicliCulturaliAsync(string piva, bool verificaPiva, int vegCod, DateTime dtInizio, DateTime dtFine,
        string additionalFilter, AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();
        stbQuery.AppendLine(" SELECT MAX (numero_ciclo) AS 'n_cicli' ");
        stbQuery.AppendLine(" FROM SpecieVegetali_Default ");
        stbQuery.AppendLine(" WHERE SpecieVegetali_Default.PivaSuperUser = @pivaSuperUser ");
        stbQuery.AppendLine(" AND   SpecieVegetali_Default.Validita_Inizio <= @dtFine ");
        stbQuery.AppendLine(" AND   SpecieVegetali_Default.Validita_Fine >= @dtInizio ");

        sqlParams.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        sqlParams.TryAdd("@dtFine", dtFine);
        sqlParams.TryAdd("@dtInizio", dtInizio);

        if (verificaPiva)
        {
            stbQuery.AppendLine(" AND SpecieVegetali_Default.Piva = @piva ");
            sqlParams.TryAdd("@piva", piva);
        }

        if (vegCod != 0)
        {
            stbQuery.AppendLine(" AND SpecieVegetali_Default.Veg_Cod = @vegCod ");
            sqlParams.TryAdd("@vegCod", vegCod);
        }

        switch (objParametriServer.FlagVisibilita)
        {
            case AgronicaCoreParametri.enumVisibilita.visibilita_SoloNonInviati:
                stbQuery.AppendLine(" AND Inviato = 0 ");
                break;
            case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati:
                stbQuery.AppendLine(" AND Inviato =-1 ");
                break;
            case AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti:
                break;
            default:
                throw new Exception("Parametro non corretto nella query (FlagVisibilita)");
        }

        if (!string.IsNullOrEmpty(additionalFilter))
        {
            stbQuery.AppendLine($" AND {additionalFilter}");
        }

        try
        {
            var result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            return result.Rows[0]["n_cicli"] == DBNull.Value ? 0 : (int)result.Rows[0]["n_cicli"];
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<DataTable> LeggiAsync(string piva, int vegCod, int culCod, int codice, int nCiclo, DateTime dtInizio,
        DateTime dtFine,
        string additionalFilter, AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();
        stbQuery.AppendLine(
            " SELECT     SpecieVegetali_Default.Veg_Cod, SpecieVegetali_Default.Cul_Cod, SpecieVegetali_Default.Codice, SpecieVegetali_Default.Valore,  ");
        stbQuery.AppendLine(
            "         SpecieVegetali_Default.Numero_Ciclo, SpecieVegetali.Veg_Des, Cultivar.Cul_Des, SpecieVegetali.Gru_Cod ");
        stbQuery.AppendLine(" FROM         SpecieVegetali_Default LEFT OUTER JOIN ");
        stbQuery.AppendLine(
            "         Cultivar ON SpecieVegetali_Default.Veg_Cod = Cultivar.Veg_Cod AND SpecieVegetali_Default.Cul_Cod = Cultivar.Cul_Cod INNER JOIN ");
        stbQuery.AppendLine("         SpecieVegetali ON SpecieVegetali_Default.Veg_Cod = SpecieVegetali.Veg_Cod ");
        stbQuery.AppendLine(" AND SpecieVegetali_Default.Piva = @piva ");
        stbQuery.AppendLine(" AND SpecieVegetali_Default.Validita_Inizio <= @dtFine ");
        stbQuery.AppendLine(" AND SpecieVegetali_Default.Validita_Fine >= @dtInizio ");

        sqlParams.TryAdd("@piva", piva);
        sqlParams.TryAdd("@dtFine", dtFine);
        sqlParams.TryAdd("@dtInizio", dtInizio);

        if (vegCod != 0)
        {
            stbQuery.AppendLine(" AND SpecieVegetali_Default.Veg_Cod = @vegCod ");
            sqlParams.TryAdd("@vegCod", vegCod);
        }

        if (culCod != 0)
        {
            stbQuery.AppendLine(" AND SpecieVegetali_Default.Cul_Cod = @culCod ");
            sqlParams.TryAdd("@culCod", culCod);
        }

        if (codice != 0)
        {
            stbQuery.AppendLine(" AND SpecieVegetali_Default.Codice = @codice ");
            sqlParams.TryAdd("@codice", codice);
        }

        if (!string.IsNullOrWhiteSpace(additionalFilter))
        {
            stbQuery.AppendLine($" AND {additionalFilter} ");
        }

        switch (objParametriServer.FlagVisibilita)
        {
            case AgronicaCoreParametri.enumVisibilita.visibilita_SoloNonInviati:
                stbQuery.AppendLine(" AND SpecieVegetali_Default.Inviato = 0 ");
                break;
            case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati:
                stbQuery.AppendLine(" AND SpecieVegetali_Default.Inviato =-1 ");
                break;
            case AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti:
                break;
            default:
                throw new Exception("Parametro non corretto nella query (FlagVisibilita)");
        }

        stbQuery.AppendLine(" ORDER BY SpecieVegetali.Veg_Des, Cultivar.Cul_Des, SpecieVegetali_Default.Numero_Ciclo ");

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

    public async Task<bool> ScriviAsync(int id, string piva, int vegCod, int culCod, int codice, string valore, int nCiclo,
        DateTime dtInizio, DateTime dtFine, AgronicaCoreParametriServer objParametriServer)
    {
        const string sqlString = @"
                    INSERT INTO SpecieVegetali_Default
                        (PivaSuperUser, Id, Piva, Veg_Cod, Cul_Cod, Codice, Valore, Numero_Ciclo, Inviato, DataInvio,
                         Data_Creazione, Data_Modifica, UserName_Creazione, UserName_Modifica, Validita_Inizio, Validita_Fine)
                     VALUES (@pivaSuperUser, @id, @piva, @vegCod, @culCod, @codice, @valore, @nCiclo, @inviato, @dataInvio,
                         @dtCreazione, @dtModifica, @userNameCreazione, @userNameModifica, @dtInizio, @dtFine);";

        var expandoObj = new ExpandoObject();
        expandoObj.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        expandoObj.TryAdd("@id", id);
        expandoObj.TryAdd("@piva", piva);
        expandoObj.TryAdd("@vegCod", vegCod);
        expandoObj.TryAdd("@culCod", culCod);
        expandoObj.TryAdd("@codice", codice);
        expandoObj.TryAdd("@valore", valore);
        expandoObj.TryAdd("@nCiclo", nCiclo);
        expandoObj.TryAdd("@inviato", 0);
        expandoObj.TryAdd("@dataInvio", DBNull.Value);
        expandoObj.TryAdd("@dtCreazione", DateTime.Now);
        expandoObj.TryAdd("@dtModifica", DateTime.Now);
        expandoObj.TryAdd("@userNameCreazione", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@userNameModifica", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@dtInizio", dtInizio);
        expandoObj.TryAdd("@dtFine", dtFine);

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

    public async Task<bool> CancellaAsync(string piva, int vegCod, int culCod, string additionalFilter, AgronicaCoreParametriServer objParametriServer)
    {
        var builder = new StringBuilder();
        var expandoObj = new ExpandoObject();

        if (objParametriServer.FlagCancellazioneLogica == AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica)
        {
            builder.AppendLine(" UPDATE SpecieVegetali_Default ");
            builder.AppendLine(" SET ");
            builder.AppendLine("   Username_Modifica = @usernameModifica ");
            builder.AppendLine("   ,Inviato = -1 ");
            builder.AppendLine(" WHERE  PivaSuperUser = @pivaSuperUser ");
            builder.AppendLine(" AND Inviato >= 0 ");

            expandoObj.TryAdd("@usernameModifica", objParametriServer.UsernameOperazione);
        }
        else
        {
            builder.AppendLine(" DELETE ");
            builder.AppendLine(" FROM SpecieVegetali_Default ");
            builder.AppendLine(" WHERE  PivaSuperUser = @pivaSuperUser ");
        }

        builder.AppendLine(" AND SpecieVegetali_Default.Piva = @piva ");
        expandoObj.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        expandoObj.TryAdd("@piva", piva);

        if (vegCod != 0)
        {
            builder.AppendLine(" AND SpecieVegetali_Default.Veg_Cod = @vegCod ");
            expandoObj.TryAdd("@vegCod", vegCod);
        }

        if (culCod != 0)
        {
            builder.AppendLine(" AND SpecieVegetali_Default.Cul_Cod = @culCod ");
            expandoObj.TryAdd("@culCod", culCod);
        }

        if (!string.IsNullOrEmpty(additionalFilter))
        {
            builder.AppendLine(" AND " + additionalFilter);
        }

        try
        {
            return await GetDataProvider(objParametriServer).Execute_WriteAsync(builder.ToString(), expandoObj);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }
}