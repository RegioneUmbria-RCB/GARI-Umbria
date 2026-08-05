using AgronicaNetCore.Base.Constants;
using System.Data;
using System.Text;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Widgets.DAL.Resources;

namespace AgronicaNetCore.Widgets.DAL.DataLayer.WidgetsZoo
{
    public class WidgetsZoo : BaseDALWidgets, IWidgetsZoo
    {
        public WidgetsZoo(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, localizer, securityBypass) { }

        public async Task<DataTable?> GetTrattamentiToSend(
            string piva, 
            int saCod, 
            int staNum,
            DateTime? inizio, 
            DateTime? fine, 
            AgronicaCoreParametri objP_Server,
            int codAnimale = 0,
            string matricola = "",
            string xFiltroAggiuntivo = "",
            bool filtroVisibilitaUtente = false
            )
        {
            var parSql = new Dictionary<string, object>();
            DataTable? result;
            try
            {
                var stb = new StringBuilder();
                bool filtraAnimale = codAnimale != 0 || !string.IsNullOrEmpty(matricola);

                parSql.Add("@utenteUsername", objP_Server.UtenteUsername);
                
                // CTE
                stb.AppendLine("WITH gruppoRicetta AS (")
                   .AppendLine("    SELECT rz.Gruppo_Ricetta, ")
                   .AppendLine("        COUNT(*) AS Num_Somm_Gruppo ")
                   .AppendLine("    FROM Ricette_Zoo rz ")
                   .AppendLine("    GROUP BY rz.Gruppo_Ricetta ")
                   .AppendLine(") ");

                // SELECT
                stb.AppendLine("SELECT rzxa.Id_Ricetta, rzxa.Id_RigaRicetta, rzxa.Id_Agenda, ")
                   .AppendLine("    ag.PIVA, imp.rag_soc AS Rag_Soc, ")
                   .AppendLine("    ag.Sa_Cod, centri.sa_nome AS Sa_Des, ")
                   .AppendLine("    ag.Sta_Num, sta.STA_DES AS Sta_Des, ")
                   .AppendLine("    movdett.Pro_Cod, movdett.Mov_Det_Des AS Pro_Des, ")
                   .AppendLine("    movdett.Qta, movdett.Udm_Cod, ")
                   .AppendLine("    COALESCE(movdett.Rif_Esterno, '') AS Tratt_Numero, ")
                   .AppendLine("    COALESCE(movdett.Extra_Str, '') AS Somm_Numero, ")
                   .AppendLine("    COALESCE(rz.Numero, '') AS Pres_Numero, ")
                   .AppendLine("    COALESCE(rza.Numero, '') AS PresRiga_Numero, ")
                   .AppendLine("    rz.DataEmissione AS Data_Prescrizione, ")
                   .AppendLine("    rz.TipoCodice AS Pres_Tipo, ")
                   .AppendLine("    COALESCE(movdett.RegSco_Numero, '') AS RegSco_Numero, ")
                   .AppendLine("    COALESCE(rz.Gruppo_Ricetta, 0) AS Gruppo_Ricetta, ")
                   .AppendLine("    rz.ProtocolloCodice AS Prot_Numero, ")
                   .AppendLine("    COALESCE(rz.Id_Protocollo, 0) AS Id_Protocollo, ")
                   .AppendLine("    rza.Numero_Somm, rza.Note AS Num_Somm_Des, movz.Note, ")
                   .AppendLine("    sta.GEN_COD, sta.SPE_COD, ")
                   .AppendLine("    sta.BDN_Codice_Azienda AS Azienda_Codice, ")
                   .AppendLine("    rz.ProprietarioIdFiscale AS Prop_IdFiscale, ")
                   .AppendLine("    ag.Validita_Inizio AS Data_Inizio, ")
                   .AppendLine("    ag.Validita_Fine AS Data_Fine, ")
                   .AppendLine("    COALESCE(movz.Tipo_Trattamento, 0) AS Somm_Tipo, ")
                   .AppendLine("    COALESCE(movz.Stato_Trattamento, 0) AS Somm_Stato, ")
                   .AppendLine("    gr.Num_Somm_Gruppo ");

                if (filtraAnimale) 
                    stb.AppendLine("    , zoo.Cod_Progetto, zoo.Matricola, zoo.Sesso, zoo.DAT_NASCITA AS Data_Nascita ");

                // JOIN
                stb.AppendLine("FROM Agenda ag ")
                   .AppendLine("INNER JOIN Ricette_ZooxAgenda rzxa ON rzxa.Id_Agenda = ag.Id_Agenda ")
                   .AppendLine("INNER JOIN Ricette_Zoo rz ON rz.IdRicetta = rzxa.Id_Ricetta ")
                   .AppendLine("INNER JOIN Ricette_Zoo_Agenda rza ON rza.IdRicetta = rz.IdRicetta AND rza.IdAgenda = rzxa.Id_RigaRicetta ")
                   .AppendLine("INNER JOIN Movimenti mov ON mov.Id_Agenda = ag.Id_Agenda ")
                   .AppendLine("LEFT JOIN Movimenti_Zoo movz ON movz.Id_Agenda = mov.Id_Agenda AND movz.Id_Mov = mov.Id_Mov ")
                   .AppendLine("INNER JOIN Movimenti_dettagli movdett ON movdett.Id_Agenda = mov.Id_Agenda AND movdett.Id_Mov = mov.Id_Mov ")
                   .AppendLine("INNER JOIN Imprese imp ON imp.piva = ag.PIVA ")
                   .AppendLine("INNER JOIN Centri_Aziendali centri ON centri.piva = ag.PIVA AND centri.sa_cod = ag.Sa_Cod ")
                   .AppendLine("INNER JOIN Stalla sta ON sta.piva = ag.PIVA AND sta.sa_cod = ag.sa_cod AND sta.STA_NUM = ag.Sta_Num ")
                   .AppendLine("LEFT JOIN gruppoRicetta gr ON gr.Gruppo_Ricetta = rz.Gruppo_Ricetta ");

                if (filtraAnimale)
                    stb.AppendLine("INNER JOIN Mov_Destinazioni movdest ON movdest.Id_Agenda = movdett.Id_Agenda ")
                       .AppendLine("    AND movdest.Id_Mov = movdett.Id_Mov AND movdest.Id_Mov_Det = movdett.Id_Mov_Det ")
                       .AppendLine("INNER JOIN Zoo_Animali zoo ON zoo.Cod_Progetto = movdest.Id_Destinazione ");

                if (filtroVisibilitaUtente)
                    stb.AppendLine("INNER JOIN utenti_Visibilita_Appoggio p (NOLOCK) ON ag.Piva = p.piva AND ag.Sa_Cod = p.Sa_Cod")
                       .AppendLine($"    AND p.Username = @utenteUsername ");

                // WHERE
                stb.AppendLine("WHERE 1=1 ")
                   .AppendLine($"    AND movz.Stato_Trattamento IN ({(int)enum_StatoTrattamento.UNDEFINED}, {(int)enum_StatoTrattamento.Aperto}) ")
                   .AppendLine($"    AND ag.Lav_Cod = {LAV_COD.LAVCOD_CUREMEDICAMENTI_ANIMALI} ")
                   .AppendLine($"    AND mov.Cau_Mov = '{CAU_MOV.CAU_TRATTAMENTO_ZOO}' ")
                   .AppendLine($"    AND rz.TipoCodice IN({(int)enum_TipoPrescrizione.Da_Protocollo_GIAS}, {(int)enum_TipoPrescrizione.Veterinaria}, {(int)enum_TipoPrescrizione.Indicazione_Terapeutica}) ");

                if (!string.IsNullOrEmpty(piva))
                {
                    parSql.Add("@piva", piva);
                    stb.AppendLine("    AND ag.Piva = @piva ");
                }
                if (saCod != 0)
                {
                    parSql.Add("@saCod", saCod);
                    stb.AppendLine("    AND ag.Sa_Cod = @saCod "); 
                }
                if (staNum != 0)
                {
                    parSql.Add("@staNum", staNum);
                    stb.AppendLine("    AND ag.Sta_Num = @staNum "); 
                }
                if (inizio != null && inizio > CostantiPersonalizzate.AGRODATAINIZIO_DATE)
                {
                    parSql.Add("@dataInizio", inizio);
                    stb.AppendLine("    AND ag.Validita_Inizio >= @dataInizio "); 
                }
                if (fine != null && fine < CostantiPersonalizzate.AGRODATAFINE_DATE)
                {
                    parSql.Add("@dataFine", fine);
                    stb.AppendLine("    AND ag.Validita_Fine <= @dataFine "); 
                }
                if (codAnimale != 0)
                {
                    parSql.Add("@codAnimale", codAnimale);
                    stb.AppendLine($"    AND zoo.Cod_Animale = @codAnimale "); 
                }
                if (!string.IsNullOrEmpty(matricola))
                {
                    parSql.Add("@matricolaAnimale", matricola);
                    stb.AppendLine($"    AND zoo.Matricola = @matricolaAnimale "); 
                }

                result = await GetDataProvider(objP_Server).ExecuteReadAsync(stb.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP_Server, ex);
                result = null;
            }

            return result;
        }
    }
}
