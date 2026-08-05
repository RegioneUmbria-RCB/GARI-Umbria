using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioniFiltroMono
{
    public class UtentiImpostazioniFiltroMono : BaseDALUtenti, IUtentiImpostazioniFiltroMono
    {
        public UtentiImpostazioniFiltroMono(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> LeggiAsync(TipiEnumerativi.Enum_Impostazioni_Utenti Impostazione_Cod, int ID_0, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            //corrisponde alla query effettuata dalla classe AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R.Leggi con xSelezioneVariabile = AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
            stbQuery.AppendLine(" SELECT      Utenti_Impostazioni_FiltroMono.Piva_SuperUser, Utenti_Impostazioni_FiltroMono.UserName, Utenti_Impostazioni_FiltroMono.Impostazione_Cod,   ");
            stbQuery.AppendLine("             Utenti_Impostazioni_FiltroMono.ID_0, Utenti_Impostazioni.Impostazione_Valore_1, Utenti_Impostazioni.Impostazione_Valore_2, ");
            stbQuery.AppendLine("             Utenti_Impostazioni.Impostazione_Valore_3, Utenti_Impostazioni.Impostazione_Valore_4,Utenti_Impostazioni_FiltroMono.Str_0  ");
            stbQuery.AppendLine(" FROM        Utenti_Impostazioni_FiltroMono ");
            stbQuery.AppendLine(" INNER JOIN  Utenti_Impostazioni ON ");
            stbQuery.AppendLine("             Utenti_Impostazioni.Piva_SuperUser = Utenti_Impostazioni_FiltroMono.Piva_SuperUser ");
            stbQuery.AppendLine("             AND  Utenti_Impostazioni.UserName = Utenti_Impostazioni_FiltroMono.UserName ");
            stbQuery.AppendLine("             AND Utenti_Impostazioni.Impostazione_Cod = Utenti_Impostazioni_FiltroMono.Impostazione_Cod ");
            stbQuery.AppendLine(" WHERE       1=1  ");

            if (!string.IsNullOrEmpty(objParametriUtenti.PivaSuperUser))
            {
                stbQuery.AppendLine(" AND     Utenti_Impostazioni_FiltroMono.Piva_SuperUser = @pivaSuperUser");
                parametriSql.Add("@pivaSuperUser", objParametriUtenti.PivaSuperUser);
            }

            if (!string.IsNullOrEmpty(objParametriUtenti.UtenteUsername))
            {
                stbQuery.AppendLine(" AND     Utenti_Impostazioni_FiltroMono.Username = @username");
                parametriSql.Add("@username", objParametriUtenti.UtenteUsername);
            }

            if (Impostazione_Cod != 0)
            {
                stbQuery.AppendLine(" AND     Utenti_Impostazioni_FiltroMono.Impostazione_Cod = @impostazioneCod");
                parametriSql.Add("@impostazioneCod", Impostazione_Cod);
            }

            if (ID_0 != 0)
            {
                stbQuery.AppendLine(" AND     Utenti_Impostazioni_FiltroMono.ID_0 = @ID_0");
                parametriSql.Add("@ID_0", ID_0);
            }

            stbQuery.AppendLine(" ORDER BY    Utenti_Impostazioni_FiltroMono.Piva_SuperUser, Utenti_Impostazioni_FiltroMono.Username, Utenti_Impostazioni_FiltroMono.Impostazione_Cod ");

            try
            {
                result = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }
    }
}
