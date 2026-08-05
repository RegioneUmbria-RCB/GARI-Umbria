using System.Data;
using System.Dynamic;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.ProfilazioneMacchine.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.ProfilazioneMacchine.DAL.DataLayer.ProfilazioneMacchine;

public class ProfilazioneMacchine : BaseDALProfilazioneMacchine, IProfilazioneMacchine
{
    public ProfilazioneMacchine(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
    }

    public async Task<DataTable> LeggiAsync(int idProfiloDati, int macCod, int macCarCod, string filtroAggiuntivo, string orderBy,
        AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();
        stbQuery.AppendLine(
            " SELECT Profilazione_Dati_MacchinexCaratteristiche.*, Macchine_Caratteristiche.Mac_Car_Des ");
        stbQuery.AppendLine(" FROM Profilazione_Dati_MacchinexCaratteristiche INNER JOIN ");
        stbQuery.AppendLine(
            "      Macchine_Caratteristiche ON Profilazione_Dati_MacchinexCaratteristiche.Mac_Car_Cod = Macchine_Caratteristiche.Mac_Car_Cod ");
        stbQuery.AppendLine(" WHERE PivaSuperUser = @pivaSuperUser");
        sqlParams.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);

        if (idProfiloDati != 0)
        {
            stbQuery.AppendLine(" AND Id_Profilo_Dati = @idProfiloDati ");
            sqlParams.TryAdd("@idProfiloDati", idProfiloDati);
        }

        if (macCod != 0)
        {
            stbQuery.AppendLine(" AND Mac_Cod = @macCod ");
            sqlParams.TryAdd("@macCod", macCod);
        }

        if (macCarCod != 0)
        {
            stbQuery.AppendLine(" AND Mac_Car_Cod = @macCarCod ");
            sqlParams.TryAdd("@macCarCod", macCarCod);
        }

        if (filtroAggiuntivo != "")
        {
            stbQuery.AppendLine(" AND " + filtroAggiuntivo);
        }

        switch (objParametriServer.FlagVisibilita)
        {
            case AgronicaCoreParametri.enumVisibilita.visibilita_SoloNonInviati:
                stbQuery.AppendLine(" AND   Profilazione_Dati_MacchinexCaratteristiche.Inviato = 0 ");
                break;
            case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati:
                stbQuery.AppendLine(" AND   Profilazione_Dati_MacchinexCaratteristiche.Inviato =-1 ");
                break;
            case AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti:
                break;
            default:
                throw new Exception("Parametro non corretto nella query (FlagVisibilita)");
        }

        if (orderBy != "")
        {
            stbQuery.AppendLine(" ORDER BY " + orderBy);
        }

        try
        {
            return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
        }
        catch
            (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<bool> CancellaAsync(int idProfiloDati, int macCod, int macCarCod, string filtroAggiuntivo,
        AgronicaCoreParametriServer objParametriServer)
    {
        var builder = new StringBuilder();
        var expandoObj = new ExpandoObject();
        if (objParametriServer.FlagCancellazioneLogica == AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica)
        {
            builder.AppendLine(" UPDATE Profilazione_Dati_MacchinexCaratteristiche ");
            builder.AppendLine(" SET ");
            builder.AppendLine(" ,Username_Modifica = @usernameOperazione ");
            builder.AppendLine(" ,Inviato = -1 ");
            builder.AppendLine(" WHERE PivaSuperUser = @pivaSuperUser");
            builder.AppendLine("  AND Id_Profilo_Dati = @idProfiloDati");
            builder.AppendLine("  AND     Inviato >= 0");

            expandoObj.TryAdd("@usernameOperazione", objParametriServer.UsernameOperazione);
            expandoObj.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
            expandoObj.TryAdd("@idProfiloDati", idProfiloDati);
        }
        else
        {
            builder.AppendLine(" DELETE ");
            builder.AppendLine(" FROM    Profilazione_Dati_MacchinexCaratteristiche ");
            builder.AppendLine(" WHERE PivaSuperUser = @pivaSuperUser");
            builder.AppendLine("  AND Id_Profilo_Dati = @idProfiloDati");
            expandoObj.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
            expandoObj.TryAdd("@idProfiloDati", idProfiloDati);
        }

        if (macCod != 0)
        {
            builder.AppendLine(" AND   Mac_Cod = @macCod ");
            expandoObj.TryAdd("@macCod", macCod);
        }

        if (macCarCod != 0)
        {
            builder.AppendLine(" AND   Mac_Car_Cod = @macCarCod ");
            expandoObj.TryAdd("@macCarCod", macCarCod);
        }

        if (!string.IsNullOrEmpty(filtroAggiuntivo))
        {
            builder.AppendLine($" AND {filtroAggiuntivo}");
        }

        try
        {
            return await GetDataProvider(objParametriServer).Execute_WriteAsync(builder.ToString(), expandoObj);
        }
        catch
            (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<bool> ScriviAsync(int idProfilazione, int macCod, int macCarCod, string valore, DateTime dtInizio, DateTime dtFine,
        AgronicaCoreParametriServer objParametriServer)
    {
        var expandoObj = new ExpandoObject();
        const string sqlString = @"INSERT INTO Profilazione_Dati_MacchinexCaratteristiche 
                     ( PivaSuperUser, Id_Profilo_Dati, Mac_Cod, Mac_Car_Cod, Valore, 
                       Validita_Inizio, Validita_Fine, 
                       Inviato, DataInvio, 
                       Data_Creazione, Data_Modifica, 
                       UserName_Creazione, UserName_Modifica 
                     ) VALUES (
                        @piva, @idProfiloDati, @macCod, @macCarCod, @valore, @validitaInizio, 
                        @validitaFine, @inviato, @dtInvio, @dataCreazione, @dataModifica,
                        @usernameCreazione, @usernameModifica )";
        expandoObj.TryAdd("@piva", objParametriServer.PivaSuperUser);
        expandoObj.TryAdd("@idProfiloDati", idProfilazione);
        expandoObj.TryAdd("@macCod", macCod);
        expandoObj.TryAdd("@macCarCod", macCarCod);
        expandoObj.TryAdd("@valore", valore);
        expandoObj.TryAdd("@validitaInizio", dtInizio);
        expandoObj.TryAdd("@validitaFine", dtFine);
        expandoObj.TryAdd("@inviato", 0);
        expandoObj.TryAdd("@dtInvio", DBNull.Value);
        expandoObj.TryAdd("@dataCreazione", DateTime.Now);
        expandoObj.TryAdd("@dataModifica", DateTime.Now);
        expandoObj.TryAdd("@usernameCreazione", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@usernameModifica", objParametriServer.UsernameOperazione);

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
}