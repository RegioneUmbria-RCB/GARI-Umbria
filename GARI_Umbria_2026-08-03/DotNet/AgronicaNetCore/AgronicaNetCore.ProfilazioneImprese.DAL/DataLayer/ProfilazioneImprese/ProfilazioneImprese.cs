using System.Data;
using System.Dynamic;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.ProfilazioneImprese.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.ProfilazioneImprese.DAL.DataLayer.ProfilazioneImprese;

public class ProfilazioneImprese : BaseDALProfilazioneImprese, IProfilazioneImprese
{
    public ProfilazioneImprese(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
    }

    public async Task<DataTable> LeggiProfilazioneImpreseAsync(string piva,
        int idProfiloDati,
        string codiceChiave,
        string idGruppo,
        string filtroAggiuntivo,
        int lavCod,
        int vegCod,
        bool leggiSoloNoteConLavorazioneNullSeLavCodZero,
        AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();

        stbQuery.AppendLine(
            " SELECT PIVA,Id_Profilo_Dati,Codice_Chiave,Quesito,Id_Gruppo,Valore_salvato, Veg_Cod,PivaSuperUser, ISNULL(Lav_Cod, 0) AS Lav_Cod, ");
        stbQuery.AppendLine(
            " inviato,datainvio,Data_Creazione,Data_Modifica,Username_Creazione,Username_Modifica,Validita_Inizio,Validita_Fine ");
        stbQuery.AppendLine(" FROM Profilazione_Dati (NOLOCK) ");
        stbQuery.AppendLine(" WHERE 1=1 ");

        if (piva != "0")
        {
            stbQuery.AppendLine(" AND PIVA = @piva ");
            sqlParams.Add("@piva", piva);
        }

        if (idProfiloDati != 0)
        {
            stbQuery.AppendLine(" AND Id_Profilo_Dati = @idProfiloDati ");
            sqlParams.Add("@idProfiloDati", idProfiloDati);
        }

        if (codiceChiave != "" && codiceChiave != "0")
        {
            stbQuery.AppendLine(" AND Codice_chiave = @codiceChiave ");
            sqlParams.Add("@codiceChiave", codiceChiave);
        }

        if (idGruppo != "")
        {
            stbQuery.AppendLine(" AND Id_Gruppo = @idGruppo");
            sqlParams.Add("@idGruppo", idGruppo);
        }

        if (leggiSoloNoteConLavorazioneNullSeLavCodZero)
        {
            if (lavCod != 0)
            {
                stbQuery.AppendLine(" AND Lav_Cod = @lavCod ");
                sqlParams.Add("@lavCod", lavCod);
            }
            else
            {
                stbQuery.AppendLine(" AND Lav_Cod  is null  ");
            }

            if (vegCod != 0)
            {
                stbQuery.AppendLine(" AND Veg_Cod = @vegCod ");
                sqlParams.Add("@vegCod", vegCod);
            }
        }
        else
        {
            if (lavCod != 0)
            {
                stbQuery.AppendLine(" AND Lav_Cod = @lavCod ");
                sqlParams.Add("@lavCod", lavCod);
            }

            if (vegCod != 0)
            {
                stbQuery.AppendLine(" AND Veg_Cod = @vegCod ");
                sqlParams.Add("@vegCod", vegCod);
            }
        }

        switch (objParametriServer.FlagVisibilita)
        {
            case AgronicaCoreParametri.enumVisibilita.visibilita_SoloNonInviati:
                stbQuery.AppendLine(" AND Inviato = 0 ");
                break;
            case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati:
                stbQuery.AppendLine(" AND Inviato = -1 ");
                break;
            case AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (!string.IsNullOrEmpty(filtroAggiuntivo))
        {
            stbQuery.AppendLine($" AND {filtroAggiuntivo}");
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

    public async Task<bool> ScriviAsync(string piva, int idProfiloDati, string codiceChiave, string quesito, string idGruppo,
        string valoreSalvato, DateTime validitaInizio, DateTime validitaFine, int lavCod, int vegCod,
        AgronicaCoreParametriServer objParametriServer)
    {
        const string sqlString = @"
                    INSERT INTO Profilazione_Dati( PIVA, Id_Profilo_Dati, Codice_chiave, Quesito, Id_Gruppo, 
                                                  Valore_salvato, PivaSuperUser,inviato, datainvio, Data_Creazione, 
                                                  Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio,
                                                  Validita_Fine, Lav_Cod, Veg_Cod) 
                     VALUES ( @piva, @idProfiloDati, @codiceChiave, @quesito, @idGruppo, @valoreSalvato,
                             @pivaSuperUser, @inviato, @dataInvio, @dataCreazione, @dataModifica, 
                             @usernameCreazione, @usernameModifica, @validitaInizio, @validitaFine,
                             @lavCod, @vegCod )
                        ";

        var expandoObj = new ExpandoObject();
        expandoObj.TryAdd("@piva", piva);
        expandoObj.TryAdd("@idProfiloDati", idProfiloDati);
        expandoObj.TryAdd("@codiceChiave", codiceChiave);
        expandoObj.TryAdd("@quesito", quesito);
        expandoObj.TryAdd("@idGruppo", idGruppo);
        expandoObj.TryAdd("@valoreSalvato", valoreSalvato);
        expandoObj.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        expandoObj.TryAdd("@inviato", 0);
        expandoObj.TryAdd("@dataInvio", DBNull.Value);
        expandoObj.TryAdd("@dataCreazione", DateTime.Now);
        expandoObj.TryAdd("@dataModifica", DateTime.Now);
        expandoObj.TryAdd("@usernameCreazione", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@usernameModifica", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@validitaInizio", validitaInizio);
        expandoObj.TryAdd("@validitaFine", validitaFine);
        expandoObj.TryAdd("@validitaFine", validitaFine);
        expandoObj.TryAdd("@lavCod", lavCod != 0 ? lavCod : DBNull.Value);
        expandoObj.TryAdd("@vegCod", vegCod);

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

    public async Task<bool> ModificaAsync(string quesito, string valoreSalvato, string piva, int idProfiloDati, string codiceChiave,
        string idGruppo, DateTime validitaInizio, DateTime validitaFine, int lavCod, int vegCod,
        AgronicaCoreParametriServer objParametriServer)
    {
        const string sqlString = @"
                    UPDATE Profilazione_Dati
                    SET  Quesito = @quesito
                        ,Valore_salvato = @valoreSalvato
                        ,Data_Modifica = @dataModifica
                        ,UserName_Modifica = @userNameModifica
                        ,Validita_Inizio = @validitaInizio
                        ,Validita_Fine = @validitaFine
                        ,inviato = 0
                        ,Lav_Cod = @lavCod
                        ,Veg_Cod = @vegCod
                    WHERE PIVA = @piva
                        AND Id_Profilo_Dati = @idProfiloDati
                        AND Codice_chiave = @codiceChiave
                        AND Id_Gruppo = @idGruppo";

        var expandoObj = new ExpandoObject();
        expandoObj.TryAdd("@quesito", quesito);
        expandoObj.TryAdd("@valoreSalvato", valoreSalvato);
        expandoObj.TryAdd("@dataModifica", DateTime.Now);
        expandoObj.TryAdd("@userNameModifica", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@validitaInizio", validitaInizio);
        expandoObj.TryAdd("@validitaFine", validitaFine);
        expandoObj.TryAdd("@lavCod", lavCod != 0 ? lavCod : DBNull.Value);
        expandoObj.TryAdd("@vegCod", vegCod);
        expandoObj.TryAdd("@piva", piva);
        expandoObj.TryAdd("@idProfiloDati", idProfiloDati);
        expandoObj.TryAdd("@codiceChiave", codiceChiave);
        expandoObj.TryAdd("@idGruppo", idGruppo);

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

    public async Task<bool> CancellaAsync(string piva, string codiceChiave, string idGruppo, int lavCod, int vegCod,
        AgronicaCoreParametriServer objParametriServer)
    {
        var builder = new StringBuilder();
        var expandoObj = new ExpandoObject();
        if (objParametriServer.FlagCancellazioneLogica == AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica)
        {
            builder.AppendLine(" UPDATE Profilazione_Dati ");
            builder.AppendLine(" SET ");
            builder.AppendLine("  Validita_Fine = @dtFine ");
            builder.AppendLine(" ,Username_Modifica = @usernameOperazione ");
            builder.AppendLine(" ,Inviato = -1 ");
            builder.AppendLine(" WHERE PIVA = @piva");
            builder.AppendLine("  AND Id_Gruppo = @idGruppo");
            builder.AppendLine("  AND     Inviato >= 0");

            expandoObj.TryAdd("@dtFine", new DateTime(1900, 1, 1));
            expandoObj.TryAdd("@usernameOperazione", objParametriServer.UsernameOperazione);
            expandoObj.TryAdd("@piva", piva);
            expandoObj.TryAdd("@idGruppo", idGruppo);
        }
        else
        {
            builder.AppendLine(" DELETE ");
            builder.AppendLine(" FROM    Profilazione_Dati ");
            builder.AppendLine(" WHERE PIVA = @piva");
            builder.AppendLine("  AND Id_Gruppo = @idGruppo");
            expandoObj.TryAdd("@piva", piva);
            expandoObj.TryAdd("@idGruppo", idGruppo);
        }

        if (codiceChiave != "")
        {
            builder.AppendLine(" AND Codice_chiave = @codiceChiave ");
            expandoObj.TryAdd("@codiceChiave", codiceChiave);
        }


        if (lavCod != 0)
        {
            builder.AppendLine(" AND Lav_Cod = @lavCod ");
            expandoObj.TryAdd("@lavCod", lavCod);
        }
        else
        {
            builder.AppendLine(" AND Lav_Cod  is null  ");
        }

        builder.AppendLine(" AND Veg_Cod = @vegCod ");
        expandoObj.TryAdd("@vegCod", vegCod);

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

    public async Task<bool> CancellaTutteLavorazioniAsync(string piva, string idGruppo, int vegCod, AgronicaCoreParametriServer objParametriServer)
    {
        var builder = new StringBuilder();
        var expandoObj = new ExpandoObject();
        if (objParametriServer.FlagCancellazioneLogica == AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica)
        {
            builder.AppendLine(" UPDATE Profilazione_Dati ");
            builder.AppendLine(" SET ");
            builder.AppendLine("  Validita_Fine = @dtFine ");
            builder.AppendLine(" ,Username_Modifica = @usernameOperazione ");
            builder.AppendLine(" ,Inviato = -1 ");
            builder.AppendLine(" WHERE PIVA = @piva");
            builder.AppendLine("  AND Id_Gruppo = @idGruppo");
            builder.AppendLine("  AND     Inviato >= 0");

            expandoObj.TryAdd("@dtFine", new DateTime(1900, 1, 1));
            expandoObj.TryAdd("@usernameOperazione", objParametriServer.UsernameOperazione);
            expandoObj.TryAdd("@piva", piva);
            expandoObj.TryAdd("@idGruppo", idGruppo);
        }
        else
        {
            builder.AppendLine(" DELETE ");
            builder.AppendLine(" FROM    Profilazione_Dati ");
            builder.AppendLine(" WHERE PIVA = @piva");
            builder.AppendLine("  AND Id_Gruppo = @idGruppo");
            expandoObj.TryAdd("@piva", piva);
            expandoObj.TryAdd("@idGruppo", idGruppo);
        }

        builder.AppendLine(" AND Veg_Cod = @vegCod ");
        expandoObj.TryAdd("@vegCod", vegCod);

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

    #region Note
    private async Task<DataTable> LeggiNote(
        string piva, int idProfiloDati, string codiceChiave, string idGruppo,
        int lavCod, int vegCod, bool leggiSoloNoteConLavorazioneNullSeLavCodZero,   
        AgronicaCoreParametriServer objParametriServer,
        string xFiltroAggiuntivo = "", string xOrderBy = ""
    )
    {
        var builder = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();

        builder.AppendLine(" SELECT * FROM Profilazione_Dati (NOLOCK)");
        builder.AppendLine(" WHERE 1=1");
        if (piva != "0")
        {
            builder.AppendLine(" AND PIVA = @piva");
            sqlParams.TryAdd("@piva", piva);
        }
        if (idProfiloDati != 0)
        {
            builder.AppendLine(" AND Id_Profilo_Dati = @idProfiloDati");
            sqlParams.TryAdd("@idProfiloDati", idProfiloDati);
        }
        if (codiceChiave != "" && codiceChiave != "0")
        {
            builder.AppendLine(" AND Codice_chiave = @codiceChiave");
            sqlParams.TryAdd("@codiceChiave", codiceChiave);
        }
        if (idGruppo != "")
        {
            builder.AppendLine(" AND Id_Gruppo = @idGruppo");
            sqlParams.TryAdd("@idGruppo", idGruppo);
        }
        if (vegCod != 0)
        {
            builder.AppendLine(" AND Veg_Cod = @vegCod");
            sqlParams.TryAdd("@vegCod", vegCod);
        }
        if (lavCod != 0)
        {
            builder.AppendLine(" AND Lav_Cod = @lavCod");
            sqlParams.TryAdd("@lavCod", lavCod);
        }
        if (leggiSoloNoteConLavorazioneNullSeLavCodZero && lavCod == 0)
        {
            builder.AppendLine(" AND Lav_Cod is null");
        }
        if (xFiltroAggiuntivo != "")
        {
            builder.AppendLine($" AND {xFiltroAggiuntivo}");
        }
        if (xOrderBy != "")
        {
            builder.AppendLine($" ORDER BY {xOrderBy}");
        }
        try
        {
            return await GetDataProvider(objParametriServer).ExecuteReadAsync(builder.ToString(), sqlParams);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }
    #endregion
}