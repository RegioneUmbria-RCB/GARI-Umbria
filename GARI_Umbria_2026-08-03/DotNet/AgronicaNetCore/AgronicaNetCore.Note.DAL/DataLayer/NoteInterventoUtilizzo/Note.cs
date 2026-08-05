using System.Data;
using System.Dynamic;
using System.Text;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Note.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Note.DAL.DataLayer.NoteInterventoUtilizzo;

public class Note : BaseDALNote, INote
{
    public Note(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
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
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<DataTable> LeggiNoteInterventoUtilizzoAsync(int notaUtilizzoCod, AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();
        stbQuery.AppendLine(" SELECT * ");
        stbQuery.AppendLine(" FROM  Note_Intervento_Utilizzo AS NIU ");
        stbQuery.AppendLine(" WHERE NIU.PivaSuperUser = @pivaSuperUser ");
        stbQuery.AppendLine(" AND   NIU.Validita_Inizio <= @dtFine ");
        stbQuery.AppendLine(" AND   NIU.Validita_Fine >= @dtInizio ");

        sqlParams.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        sqlParams.TryAdd("@dtFine", objParametriServer.FinestraTemporaleFine);
        sqlParams.TryAdd("@dtInizio", objParametriServer.FinestraTemporaleInizio);

        if (notaUtilizzoCod != 0)
        {
            stbQuery.AppendLine(" AND   NIU.NotaUtilizzo_cod = @notaUtilizzoCod ");
            sqlParams.TryAdd("@notaUtilizzoCod", notaUtilizzoCod);
        }

        stbQuery.AppendLine(" ORDER BY NIU.NotaUtilizzo_Des");

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

    public async Task<DataTable> LeggiProfilazioneNoteAsync(int notaUtilizzoCod, int notaGruppoCod, bool soloNoteConUtilizzo,
        Visibilita visibilita, string filtroAggiuntivo, AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();

        var joinCondition = soloNoteConUtilizzo ? " INNER JOIN " : " LEFT OUTER JOIN ";

        stbQuery.AppendLine(" SELECT NI.Nota_Cod, NI.Nota_Des, NIG.NotaGruppo_Cod, NIG.NotaGruppo_Des, ");
        stbQuery.AppendLine(
            " ISNULL(NIU.NotaUtilizzo_Cod,0) AS NotaUtilizzo_Cod, ISNULL(NIU.NotaUtilizzo_Des,'') AS NotaUtilizzo_Des, NI.Visibile, NIG.Visibile AS VisibileGruppo ");
        stbQuery.AppendLine(" FROM Note_Intervento AS NI INNER JOIN ");
        stbQuery.AppendLine(" Note_Intervento_Gruppi AS NIG ON NI.NotaGruppo_Cod = NIG.NotaGruppo_Cod ");
        stbQuery.AppendLine($" {joinCondition} Note_Intervento_UtilizzoxGruppi AS NIUG ");
        stbQuery.AppendLine($" ON NIUG.NotaGruppo_Cod = NIG.NotaGruppo_Cod {joinCondition} ");
        stbQuery.AppendLine(" Note_Intervento_Utilizzo AS NIU ON NIU.NotaUtilizzo_Cod = NIUG.NotaUtilizzo_Cod ");
        stbQuery.AppendLine(" WHERE NI.PivaSuperUser = @pivaSuperUser ");
        stbQuery.AppendLine(" AND   NI.Validita_Inizio <= @dtFine ");
        stbQuery.AppendLine(" AND   NI.Validita_Fine >= @dtInizio ");

        sqlParams.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        sqlParams.TryAdd("@dtFine", objParametriServer.FinestraTemporaleFine);
        sqlParams.TryAdd("@dtInizio", objParametriServer.FinestraTemporaleInizio);

        if (notaUtilizzoCod != 0)
        {
            stbQuery.AppendLine(" AND NIU.NotaUtilizzo_cod = @notaUtilizzoCod");
            sqlParams.TryAdd("@notaUtilizzoCod", notaUtilizzoCod);
        }

        if (notaGruppoCod != 0)
        {
            stbQuery.AppendLine(" AND   NIG.NotaGruppo_Cod = @notaGruppoCod");
            sqlParams.TryAdd("@notaGruppoCod", notaGruppoCod);
        }

        if (visibilita != Visibilita.Tutte)
        {
            stbQuery.AppendLine(" AND   NI.Visibile = @visible ");
            stbQuery.AppendLine(" AND   NIG.Visibile = @visible ");
            sqlParams.TryAdd("@visible", (int)visibilita);
        }

        if (!string.IsNullOrEmpty(filtroAggiuntivo))
        {
            stbQuery.AppendLine($" AND {filtroAggiuntivo}");
        }

        stbQuery.AppendLine(" ORDER BY NIU.NotaUtilizzo_Des, NIG.NotaGruppo_Des, NI.Nota_Des");

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

    public async Task<string> LeggiNotaDesFromNotaCodAsync(int notaCod, AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();
        stbQuery.AppendLine(" SELECT [Nota_Des] ");
        stbQuery.AppendLine(" FROM  Note_Intervento ");
        stbQuery.AppendLine(" WHERE PivaSuperUser = @pivaSuperUser ");
        stbQuery.AppendLine(" AND   Validita_Inizio <= @dtFine ");
        stbQuery.AppendLine(" AND   Validita_Fine >= @dtInizio ");

        sqlParams.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        sqlParams.TryAdd("@dtFine", objParametriServer.FinestraTemporaleFine);
        sqlParams.TryAdd("@dtInizio", objParametriServer.FinestraTemporaleInizio);

        if (notaCod != 0)
        {
            stbQuery.AppendLine(" AND Nota_Cod = @notaCod ");
            sqlParams.TryAdd("@notaCod", notaCod);
        }

        try
        {
            var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            return dt.Rows.Count > 0 ? dt.Rows[0]["Nota_Des"].ToString() ?? "" : "";
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<string> LeggiNotaUtilizzoDesFromNotaUtilizzoCodAsync(int notaUtilizzoCod, AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();
        stbQuery.AppendLine(" SELECT [NotaUtilizzo_Des] ");
        stbQuery.AppendLine(" FROM  Note_Intervento_Utilizzo ");
        stbQuery.AppendLine(" WHERE PivaSuperUser = @pivaSuperUser ");
        stbQuery.AppendLine(" AND   Validita_Inizio <= @dtFine ");
        stbQuery.AppendLine(" AND   Validita_Fine >= @dtInizio ");
        stbQuery.AppendLine(" AND   NotaUtilizzo_Cod = @notaUtilizzoCod ");

        sqlParams.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        sqlParams.TryAdd("@dtFine", objParametriServer.FinestraTemporaleFine);
        sqlParams.TryAdd("@dtInizio", objParametriServer.FinestraTemporaleInizio);
        sqlParams.TryAdd("@notaUtilizzoCod", notaUtilizzoCod);

        try
        {
            var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            return dt.Rows.Count > 0 ? dt.Rows[0]["NotaUtilizzo_Des"].ToString() ?? "" : "";
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<DataTable> LeggiGruppoNoteAsync(int notaGruppoCod, string filtroAggiuntivo, string orderBy,
        AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();
        stbQuery.AppendLine(" SELECT [PivaSuperUser] ,[NotaGruppo_Cod] ,[NotaGruppo_Des] ,[visibile] ");
        stbQuery.AppendLine(" FROM  Note_Intervento_Gruppi ");
        stbQuery.AppendLine(" WHERE PivaSuperUser = @pivaSuperUser ");
        stbQuery.AppendLine(" AND   Validita_Inizio <= @dtFine ");
        stbQuery.AppendLine(" AND   Validita_Fine >= @dtInizio ");

        sqlParams.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        sqlParams.TryAdd("@dtFine", objParametriServer.FinestraTemporaleFine);
        sqlParams.TryAdd("@dtInizio", objParametriServer.FinestraTemporaleInizio);

        if (notaGruppoCod != 0)
        {
            stbQuery.AppendLine(" AND NotaGruppo_Cod = @notaGruppoCod ");
            sqlParams.TryAdd("@notaGruppoCod", notaGruppoCod);
        }

        if (!string.IsNullOrEmpty(filtroAggiuntivo))
        {
            stbQuery.AppendLine($" AND {filtroAggiuntivo} ");
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

        if (!string.IsNullOrEmpty(orderBy))
        {
            stbQuery.AppendLine($" ORDER BY {orderBy} ");
        }
        else
        {
            stbQuery.AppendLine(" ORDER BY NotaGruppo_Des ASC ");
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

    public async Task<bool> SalvaGruppoNoteAsync(int notaGruppoCod, string notaGruppoDes, DateTime validitaInizio, DateTime validitaFine,
        AgronicaCoreParametriServer objParametriServer)
    {
        var expandoObj = new ExpandoObject();

        const string sqlString = @"
                    INSERT INTO Note_Intervento_Gruppi (
                        PivaSuperUser, NotaGruppo_Cod, NotaGruppo_Des, Inviato, DataInvio, Data_Creazione,
                        Data_Modifica, UserName_Creazione, UserName_Modifica, Validita_Inizio, Validita_Fine, Visibile )
                        VALUES (@pivaSuperUser, @notaGruppoCod, @notaGruppoDes, @inviato, @dataInvio, @dataCreazione,
                                @dataModifica, @userNameCreazione, @userNameModifica, @validitaInizio, @validitaFine, @visibile);";

        expandoObj.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        expandoObj.TryAdd("@notaGruppoCod", notaGruppoCod);
        expandoObj.TryAdd("@notaGruppoDes", notaGruppoDes);
        expandoObj.TryAdd("@inviato", 0);
        expandoObj.TryAdd("@dataInvio", DBNull.Value);
        expandoObj.TryAdd("@dataCreazione", DateTime.Now);
        expandoObj.TryAdd("@dataModifica", DateTime.Now);
        expandoObj.TryAdd("@userNameCreazione", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@userNameModifica", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@validitaInizio", validitaInizio);
        expandoObj.TryAdd("@validitaFine", validitaFine);
        expandoObj.TryAdd("@visibile", 1);


        try
        {
            return await GetDataProvider(objParametriServer).Execute_WriteAsync(sqlString, expandoObj);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<int> NoteInterventoGruppiNewIdAsync(AgronicaCoreParametriServer objParametriServer)
    {
        const string sqlString = @"SELECT ISNULL (
                                     (CASE(MAX(NotaGruppo_Cod) + 1 )
                                         WHEN 0 THEN 1 
                                         ELSE MAX(NotaGruppo_Cod) +1 END  
                                     ) , 1) AS Cod 
                                    FROM Note_Intervento_Gruppi";

        try
        {
            var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sqlString);
            return (int)dt.Rows[0]["Cod"];
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<bool> AggiornaGruppoNoteAsync(int notaGruppoCod, string notaGruppoDes, DateTime finestraTemporaleInizio,
        DateTime finestraTemporaleFine, bool visibile, AgronicaCoreParametriServer objParametriServer)
    {
        var expandoObj = new ExpandoObject();
        const string sqlString = @"UPDATE Note_Intervento_Gruppi SET
                                    NotaGruppo_Des       = @notaGruppoDes
                                   ,Inviato              = 0
                                   ,DataInvio            = @dtInvio
                                   ,Data_Modifica        = @dtModifica
                                   ,UserName_Modifica    = @usernameModifica
                                   ,Validita_Inizio      = @dtInizio
                                   ,Validita_Fine        = @dtFine
                                   ,Visibile             = @visibile
                                 WHERE   PivaSuperUser   = @pivaSuperUser
                                 AND     NotaGruppo_Cod  =  @notaGruppoCod";

        expandoObj.TryAdd("@notaGruppoDes", notaGruppoDes);
        expandoObj.TryAdd("@dtInvio", DBNull.Value);
        expandoObj.TryAdd("@dtModifica", DateTime.Now);
        expandoObj.TryAdd("@usernameModifica", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@dtInizio", finestraTemporaleInizio);
        expandoObj.TryAdd("@dtFine", finestraTemporaleFine);
        expandoObj.TryAdd("@visibile", visibile);
        expandoObj.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        expandoObj.TryAdd("@notaGruppoCod", notaGruppoCod);

        try
        {
            return await GetDataProvider(objParametriServer).Execute_WriteAsync(sqlString, expandoObj);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<bool> CancellaGruppoNoteAsync(int gruppoNoteCod, AgronicaCoreParametriServer objParametriServer)
    {
        var builder = new StringBuilder();
        var expandoObj = new ExpandoObject();

        if (objParametriServer.FlagCancellazioneLogica == AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica)
        {
            builder.AppendLine(" UPDATE Note_Intervento_Gruppi ");
            builder.AppendLine(" SET ");
            builder.AppendLine("   Username_Modifica = @usernameModifica ");
            builder.AppendLine("   ,Inviato = -1 ");
            builder.AppendLine(" WHERE Inviato >= 0 ");

            expandoObj.TryAdd("@usernameModifica", objParametriServer.UsernameOperazione);
        }
        else
        {
            builder.AppendLine(" DELETE ");
            builder.AppendLine(" FROM Note_Intervento_Gruppi ");
            builder.AppendLine(" WHERE  1=1 ");
        }

        builder.AppendLine(" AND PivaSuperUser = @pivaSuperUser ");
        builder.AppendLine(" AND NotaGruppo_Cod = @gruppoNoteCod ");

        expandoObj.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        expandoObj.TryAdd("@gruppoNoteCod", gruppoNoteCod);

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

    public async Task<bool> SalvaNotaAsync(int notaCod, string notaDes, int notaGruppoCod, DateTime validitaInizio,
        DateTime validitaFine, AgronicaCoreParametriServer objParametriServer)
    {
        var expandoObj = new ExpandoObject();

        const string sqlString = @"
                    INSERT INTO Note_Intervento (
                        PivaSuperUser, Nota_Cod, Nota_Des, NotaGruppo_Cod, Inviato, DataInvio, Data_Creazione,
                        Data_Modifica, UserName_Creazione, UserName_Modifica, Validita_Inizio, Validita_Fine, 
                        Note_Valore_Numerico, Visibile, Note_Valore_Stringa )
                        VALUES (@pivaSuperUser, @notaCod, @notaDes, @notaGruppoCod, @inviato, @dataInvio, @dataCreazione,
                                @dataModifica, @userNameCreazione, @userNameModifica, @validitaInizio, @validitaFine, 
                                @noteValoreNumerico, @visibile, @noteValoreStringa);";

        expandoObj.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        expandoObj.TryAdd("@notaCod", notaCod);
        expandoObj.TryAdd("@notaDes", notaDes);
        expandoObj.TryAdd("@notaGruppoCod", notaGruppoCod);
        expandoObj.TryAdd("@inviato", 0);
        expandoObj.TryAdd("@dataInvio", DBNull.Value);
        expandoObj.TryAdd("@dataCreazione", DateTime.Now);
        expandoObj.TryAdd("@dataModifica", DateTime.Now);
        expandoObj.TryAdd("@userNameCreazione", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@userNameModifica", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@validitaInizio", validitaInizio);
        expandoObj.TryAdd("@validitaFine", validitaFine);
        expandoObj.TryAdd("@noteValoreNumerico", DBNull.Value);
        expandoObj.TryAdd("@visibile", 1);
        expandoObj.TryAdd("@noteValoreStringa", DBNull.Value);


        try
        {
            return await GetDataProvider(objParametriServer).Execute_WriteAsync(sqlString, expandoObj);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<int> NoteNewIdAsync(AgronicaCoreParametriServer objParametriServer)
    {
        const string sqlString = @"SELECT ISNULL (
                                     (CASE(MAX(Nota_Cod) + 1 )
                                         WHEN 0 THEN 1 
                                         ELSE MAX(Nota_Cod) +1 END  
                                     ) , 1) AS Cod 
                                    FROM Note_Intervento";

        try
        {
            var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sqlString);
            return (int)dt.Rows[0]["Cod"];
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<DataTable> LeggiNoteInterventoUtilizzoGruppiAsync(int notaGruppoCod, int notaUtilizzoCod,
        AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var sqlParams = new Dictionary<string, object>();
        stbQuery.AppendLine(" SELECT  * ");
        stbQuery.AppendLine(" FROM  Note_Intervento_UtilizzoXGruppi ");
        stbQuery.AppendLine(" WHERE PivaSuperUser = @pivaSuperUser ");
        stbQuery.AppendLine(" AND   Validita_Inizio <= @dtFine ");
        stbQuery.AppendLine(" AND   Validita_Fine >= @dtInizio ");

        sqlParams.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        sqlParams.TryAdd("@dtFine", objParametriServer.FinestraTemporaleFine);
        sqlParams.TryAdd("@dtInizio", objParametriServer.FinestraTemporaleInizio);

        if (notaGruppoCod != 0)
        {
            stbQuery.AppendLine(" AND NotaGruppo_Cod = @notaGruppoCod ");
            sqlParams.TryAdd("@notaGruppoCod", notaGruppoCod);
        }

        if (notaUtilizzoCod != 0)
        {
            stbQuery.AppendLine(" AND NotaUtilizzo_Cod = @notaUtilizzoCod ");
            sqlParams.TryAdd("@notaUtilizzoCod", notaUtilizzoCod);
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

    public async Task<bool> CancellaGruppoNoteUtilizzoAsync(int notaGruppoCod, int notaUtilizzoCod, AgronicaCoreParametriServer objParametriServer)
    {
        var builder = new StringBuilder();
        var expandoObj = new ExpandoObject();

        if (objParametriServer.FlagCancellazioneLogica == AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica)
        {
            builder.AppendLine(" UPDATE Note_Intervento_UtilizzoxGruppi ");
            builder.AppendLine(" SET ");
            builder.AppendLine("   Username_Modifica = @usernameModifica ");
            builder.AppendLine("   ,Inviato = -1 ");
            builder.AppendLine(" WHERE Inviato >= 0 ");

            expandoObj.TryAdd("@usernameModifica", objParametriServer.UsernameOperazione);
        }
        else
        {
            builder.AppendLine(" DELETE ");
            builder.AppendLine(" FROM Note_Intervento_UtilizzoxGruppi ");
            builder.AppendLine(" WHERE  1=1 ");
        }

        builder.AppendLine(" AND PivaSuperUser = @pivaSuperUser ");
        builder.AppendLine(" AND NotaGruppo_Cod = @gruppoNoteCod ");

        expandoObj.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        expandoObj.TryAdd("@gruppoNoteCod", notaGruppoCod);


        if (notaUtilizzoCod > 0)
        {
            builder.AppendLine(" AND NotaUtilizzo_Cod = @notaUtilizzoCod ");
            expandoObj.TryAdd("@notaUtilizzoCod", notaGruppoCod);
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

    public async Task<bool> ScriviNoteUtilizzoAsync(int notaGruppoCod, int utilizzoCod, DateTime dtInizio, DateTime dtFine,
        AgronicaCoreParametriServer objParametriServer)
    {
        var expandoObj = new ExpandoObject();

        const string sqlString = @"
                    INSERT INTO Note_Intervento_UtilizzoxGruppi (
                        PivaSuperUser, NotaGruppo_Cod, NotaUtilizzo_Cod, Inviato, DataInvio, Data_Creazione,
                        Data_Modifica, UserName_Creazione, UserName_Modifica, Validita_Inizio, Validita_Fine )
                        VALUES (@pivaSuperUser, @notaGruppoCod, @notaUtilizzoCod, @inviato, @dataInvio, @dataCreazione,
                                @dataModifica, @userNameCreazione, @userNameModifica, @validitaInizio, @validitaFine);";

        expandoObj.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        expandoObj.TryAdd("@notaGruppoCod", notaGruppoCod);
        expandoObj.TryAdd("@notaUtilizzoCod", utilizzoCod);
        expandoObj.TryAdd("@inviato", 0);
        expandoObj.TryAdd("@dataInvio", DBNull.Value);
        expandoObj.TryAdd("@dataCreazione", DateTime.Now);
        expandoObj.TryAdd("@dataModifica", DateTime.Now);
        expandoObj.TryAdd("@userNameCreazione", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@userNameModifica", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@validitaInizio", dtInizio);
        expandoObj.TryAdd("@validitaFine", dtFine);

        try
        {
            return await GetDataProvider(objParametriServer).Execute_WriteAsync(sqlString, expandoObj);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<bool> ModificaVisibilitaGruppoAsync(int notaGruppoCod, bool visible, AgronicaCoreParametriServer objParametriServer)
    {
        var builder = new StringBuilder();
        var expandoObj = new ExpandoObject();

        builder.AppendLine(" UPDATE Note_Intervento_Gruppi ");
        builder.AppendLine(" SET ");
        builder.AppendLine("   Data_Modifica = @dataModifica ");
        builder.AppendLine("   ,Username_Modifica = @usernameModifica ");
        builder.AppendLine("   ,Visibile = @visibile ");
        builder.AppendLine(" WHERE PivaSuperUser = @pivaSuperUser ");
        builder.AppendLine(" AND NotaGruppo_Cod = @gruppoNoteCod ");

        expandoObj.TryAdd("@dataModifica", DateTime.Now);
        expandoObj.TryAdd("@usernameModifica", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@visibile", visible);
        expandoObj.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        expandoObj.TryAdd("@gruppoNoteCod", notaGruppoCod);

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

    public async Task<bool> ModificaVisibilitaNotaAsync(int notaCod, bool visible, AgronicaCoreParametriServer objParametriServer)
    {
        var builder = new StringBuilder();
        var expandoObj = new ExpandoObject();

        builder.AppendLine(" UPDATE Note_Intervento ");
        builder.AppendLine(" SET ");
        builder.AppendLine("   Data_Modifica = @dataModifica ");
        builder.AppendLine("   ,Username_Modifica = @usernameModifica ");
        builder.AppendLine("   ,Visibile = @visibile ");
        builder.AppendLine(" WHERE PivaSuperUser = @pivaSuperUser ");
        builder.AppendLine(" AND Nota_Cod = @notaCod ");

        expandoObj.TryAdd("@dataModifica", DateTime.Now);
        expandoObj.TryAdd("@usernameModifica", objParametriServer.UsernameOperazione);
        expandoObj.TryAdd("@visibile", visible);
        expandoObj.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        expandoObj.TryAdd("@notaCod", notaCod);

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

    public async Task<bool> CancellaNotaAsync(int notaCod, AgronicaCoreParametriServer objParametriServer)
    {
        var builder = new StringBuilder();
        var expandoObj = new ExpandoObject();

        if (objParametriServer.FlagCancellazioneLogica == AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica)
        {
            builder.AppendLine(" UPDATE Note_Intervento ");
            builder.AppendLine(" SET ");
            builder.AppendLine("   Username_Modifica = @usernameModifica ");
            builder.AppendLine("   ,Inviato = -1 ");
            builder.AppendLine(" WHERE Inviato >= 0 ");

            expandoObj.TryAdd("@usernameModifica", objParametriServer.UsernameOperazione);
        }
        else
        {
            builder.AppendLine(" DELETE ");
            builder.AppendLine(" FROM Note_Intervento ");
            builder.AppendLine(" WHERE  1=1 ");
        }

        builder.AppendLine(" AND PivaSuperUser = @pivaSuperUser ");
        builder.AppendLine(" AND Nota_Cod = @notaCod ");

        expandoObj.TryAdd("@pivaSuperUser", objParametriServer.PivaSuperUser);
        expandoObj.TryAdd("@notaCod", notaCod);

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