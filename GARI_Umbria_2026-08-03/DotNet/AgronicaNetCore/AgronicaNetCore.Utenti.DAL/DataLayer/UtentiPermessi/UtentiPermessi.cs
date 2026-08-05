using System.Data;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.DAL.Resources;
using Microsoft.Extensions.Localization;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.UtentiPermessi
{
    public class UtentiPermessi : BaseDALUtenti, IUtentiPermessi
    {
        public UtentiPermessi(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        public async Task<bool> ControllaPermessiUtenteAsync(
            string username,
            Enum_Id_Servizio idServizio,
            Enum_Security_Attivita idAttivita,
            Enum_Security_Operazione idOperazione,
            DateTime dataOraControllo,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT * FROM Utenti_Permessi");
            stbQuery.AppendLine("WHERE UserName = @username");
            stbQuery.AppendLine("AND Id_Servizio = @idServizio");
            stbQuery.AppendLine("AND Id_Attivita = @idAttivita");
            stbQuery.AppendLine("AND Id_Operazione = @idOperazione");
            stbQuery.AppendLine("AND Validita_Inizio <= @dataControllo");
            stbQuery.AppendLine("AND Validita_Fine >= @dataControllo");

            parSql.Add("@username", username);
            parSql.Add("@idServizio", (int)idServizio);
            parSql.Add("@idAttivita", (int)idAttivita);
            parSql.Add("@idOperazione", (int)idOperazione);
            parSql.Add("@dataControllo", dataOraControllo.Date);

            try
            {
                DataTable result = await GetDataProvider(objParametriUtenti)
                    .ExecuteReadAsync(stbQuery.ToString(), parSql);
                return result.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<
            IReadOnlyDictionary<Enum_Security_Attivita, IReadOnlySet<Enum_Security_Operazione>>
        > ControllaPermessiUtenteAsync(
            string username,
            Enum_Id_Servizio idServizio,
            IList<Enum_Security_Attivita> attivita,
            IList<Enum_Security_Operazione> operazioni,
            DateTime dataOraControllo,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            if (
                attivita == null
                || attivita.Count == 0
                || operazioni == null
                || operazioni.Count == 0
            )
                return new Dictionary<
                    Enum_Security_Attivita,
                    IReadOnlySet<Enum_Security_Operazione>
                >();

            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>();

            stbQuery.AppendLine("SELECT Id_Attivita, Id_Operazione FROM Utenti_Permessi");
            stbQuery.AppendLine("WHERE UserName = @username");
            stbQuery.AppendLine("AND Id_Servizio = @idServizio");
            stbQuery.AppendLine("AND Id_Attivita IN (@idAttivita)");
            stbQuery.AppendLine("AND Id_Operazione IN (@idOperazione)");
            stbQuery.AppendLine("AND Validita_Inizio <= @dataControllo");
            stbQuery.AppendLine("AND Validita_Fine >= @dataControllo");

            parSql.Add("@username", username);
            parSql.Add("@idServizio", (int)idServizio);
            parSql.Add("@dataControllo", dataOraControllo.Date);
            parSqlIn.Add("@idAttivita", FormatClauseIn(attivita.Select(a => (int)a).ToList()));
            parSqlIn.Add("@idOperazione", FormatClauseIn(operazioni.Select(o => (int)o).ToList()));

            try
            {
                DataTable result = await GetDataProvider(objParametriUtenti)
                    .ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);

                // Initialise all requested activities with an empty granted set.
                var dict = attivita.ToDictionary(
                    a => a,
                    _ =>
                        (IReadOnlySet<Enum_Security_Operazione>)
                            new HashSet<Enum_Security_Operazione>()
                );

                foreach (DataRow row in result.Rows)
                {
                    var act = (Enum_Security_Attivita)Convert.ToInt32(row["Id_Attivita"]);
                    var op = (Enum_Security_Operazione)Convert.ToInt32(row["Id_Operazione"]);
                    if (!dict.ContainsKey(act))
                        dict[act] = new HashSet<Enum_Security_Operazione>();
                    ((HashSet<Enum_Security_Operazione>)dict[act]).Add(op);
                }

                return dict;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable?> ReadAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer, int idService, int idActivity, int idOperation, int id = 1)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            DataTable? result;

            try
            {
                stbQuery.AppendLine(" SELECT * ");
                stbQuery.AppendLine(" FROM Utenti_Permessi (NOLOCK) ");
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                stbQuery.AppendLine(" AND UserName = @Username ");

                stbQuery.AppendLine(" AND Id_Servizio = @idService");
                parSql.Add("@idService", idService);

                stbQuery.AppendLine(" AND Id_Attivita = @idActivity");
                parSql.Add("@idActivity", idActivity);

                stbQuery.AppendLine(" AND Id_Operazione = @idOperation");
                parSql.Add("@idOperation", idOperation);

                stbQuery.AppendLine(" AND ID= @id");
                parSql.Add("@id", id);

                parSql.Add("@Username", objParametriUtenti.UtenteUsername);

                result = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }

            return result;
        }

        public async Task<DataTable?> ReadAllAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            DataTable? result;

            try
            {
                stbQuery.AppendLine("SELECT UserName, Id_Servizio, Id_Operazione, ID, Inizio_Ore, Inizio_Minuti, Fine_Ore, Fine_Minuti");
                stbQuery.AppendLine("FROM Utenti_Permessi (NOLOCK)");
                stbQuery.AppendLine("WHERE 1 = 1");
                stbQuery.AppendLine("AND UserName = @Username");

                parSql.Add("@Username", objParametriUtenti.UtenteUsername);

                result = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }
    }
}