using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.OperazioniZoo.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Text;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.AgendaPesatura;

/// <summary>
/// Implementazione del repository per le operazioni di pesatura su Agenda/Movimenti/Movimenti_Dettagli/Mov_Destinazioni.
/// <para>Riferimento spec: DS02-BL TrasformazionePesateStagingAgenda — Step 3 Prevenzione Duplicati
/// e Step 4 Creazione Operazione Agenda; DS06-BL PrevenzioneDuplicatiOperazioniAgenda.</para>
/// </summary>
public sealed class AgendaPesaturaRepository : BaseDALOperazioniZoo, IAgendaPesaturaRepository
{

    public AgendaPesaturaRepository(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer)
    {
    }

    /// <inheritdoc/>
    public async Task<int> CheckDuplicatoPesaturaPerDataAsync(
        string piva,
        int saCod,
        int staNum,
        IReadOnlyList<int> codProgettiAnimali,
        DateTime dataPesata,
        AgronicaCoreParametriServer objParametriServer)
    {
        var stbQuery = new StringBuilder();
        var parSql = new Dictionary<string, object>();
        Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

        parSqlIn.Add("@codProgettiIn", FormatClauseIn(codProgettiAnimali.ToList()));

        stbQuery.AppendLine("SELECT COUNT(*) AS NumDuplicati");
        stbQuery.AppendLine("FROM Agenda a (NOLOCK)");
        stbQuery.AppendLine("    INNER JOIN Movimenti m (NOLOCK)");
        stbQuery.AppendLine("        ON a.PIVA = m.PIVA AND a.Id_Agenda = m.Id_Agenda");
        stbQuery.AppendLine("    INNER JOIN Movimenti_Dettagli md (NOLOCK)");
        stbQuery.AppendLine("        ON m.PIVA = md.PIVA AND m.Id_Agenda = md.Id_Agenda AND m.Id_Mov = md.Id_Mov");
        stbQuery.AppendLine("WHERE a.PIVA = @piva");
        stbQuery.AppendLine("    AND a.Sa_Cod = @saCod");
        stbQuery.AppendLine("    AND a.Sta_Num = @staNum");
        stbQuery.AppendLine("    AND a.Lav_Cod = @lavCod");
        stbQuery.AppendLine("    AND m.Cau_Mov = @cauMov");
        stbQuery.AppendLine("    AND md.Elem_Cod = @elemCod");
        stbQuery.AppendLine("    AND md.Cod_Progetto IN (@codProgettiIn)");
        stbQuery.AppendLine("    AND CAST(m.Data_Movimento AS DATE) = CAST(@dataPesata AS DATE)");
        parSql.Add("@piva", piva);
        parSql.Add("@saCod", saCod);
        parSql.Add("@staNum", staNum);
        parSql.Add("@lavCod", LAV_COD.LAVCOD_PESATURA_ANIMALI);
        parSql.Add("@cauMov", CAU_MOV.CAU_PESATURA_ANIMALI);
        parSql.Add("@elemCod", ELEM_COD.ZOO_CONSISTENZA);
        parSql.Add("@dataPesata", dataPesata);

        try
        {
            var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["NumDuplicati"]) : 0;
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }
}
