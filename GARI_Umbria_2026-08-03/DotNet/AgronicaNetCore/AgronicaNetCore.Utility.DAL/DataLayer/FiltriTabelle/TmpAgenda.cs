using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utility.DAL.Resources;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Text;

namespace AgronicaNetCore.Utility.DAL.DataLayer.FiltriTabelle;
public partial class TmpAgenda : BaseDALUtility, ITmpAgenda
{
    readonly IAgro_Sequence _agroSequences;
    public TmpAgenda(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, localizer, securityBypass)
    {
        _agroSequences = provider.GetRequiredService<IAgro_Sequence>();
    }

    public async Task<bool> ScriviAsync(int idTestataTemp, string piva, int idAgenda, int lavCod, AgronicaCoreParametriServer objParametriServer)
    {
        string nomeRoutine = "__tmp_Agenda_W.Scrivi()";

        string messaggioErrore = "";
        var stb = new StringBuilder();
        bool xRisp = false;

        try
        {

            if (idTestataTemp == 0)
            {

                idTestataTemp = await _agroSequences.NuovoId_TabellaAsync("__Tmp_Agenda", 0, int.MaxValue, objParametriServer);
                //int idTestataTempMatricola = await _agroSequences.NuovoId_TabellaAsync("__Tmp_Agenda", 0, int.MaxValue, objParametriServer);
            }

            // ---------------------------------------------           
            stb.Length = 0;

            stb.AppendLine(" INSERT INTO __tmp_Agenda ");
            stb.AppendLine("         ( ");
            stb.AppendLine("          IDTestataTemp, piva,  id_agenda, lav_cod");
            stb.AppendLine("         ) ");

            stb.AppendLine(" VALUES ( ");
            stb.AppendLine($"          {idTestataTemp}");
            stb.AppendLine($"         ,'{piva}'   ");
            stb.AppendLine($"         , {idAgenda}  ");
            stb.AppendLine($"         , {lavCod}  ");
            stb.AppendLine("        ) ");
            // --------------------------------------------------------------------------

            xRisp = await this.GetDataProvider(objParametriServer).Execute_WriteAsync(stb.ToString());
        }
        // --------------------------------------------------------------------------

        catch (Exception ex)
        {
            messaggioErrore = ex.Message;
            LogError(messaggioErrore, objParametriServer, ex);
            xRisp = false;
            throw new Exception("[" + nomeRoutine + "] : " + messaggioErrore);
        }

        return xRisp;

    }
    public async Task<bool> CancellaRecordDaIDTestataTempAsync(int idTestataTemp, AgronicaCoreParametriServer objParametriServer)
    {

        string NomeRoutine = "__tmp_Agenda_W.CancellaRecordDaIDTestataTemp()";

        string messaggioErrore = "";
        var stb = new StringBuilder();
        bool xRisp = false;

        try
        {

            if (idTestataTemp != 0)
            {
                // ---------------------------------------------
                stb.Length = 0;
                stb.AppendLine(" Delete  ");
                stb.AppendLine(" From __tmp_Agenda ");
                stb.AppendLine($" Where IDTestataTemp = {idTestataTemp}");

                // --------------------------------------------------------------------------                
                xRisp = await this.GetDataProvider(objParametriServer).Execute_WriteAsync(stb.ToString());
                // --------------------------------------------------------------------------
            }
        }

        catch (Exception ex)
        {
            messaggioErrore = ex.Message;
            LogError(messaggioErrore, objParametriServer, ex);
            xRisp = false;
            throw new Exception("[" + NomeRoutine + "] : " + messaggioErrore);

        }

        return xRisp;
    }
}
