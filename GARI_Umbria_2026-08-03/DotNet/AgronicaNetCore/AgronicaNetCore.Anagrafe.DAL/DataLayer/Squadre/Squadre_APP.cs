using System.Globalization;
using System.Text;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Squadre
{
    /// <summary>
    /// Ref: DS08-BL sections 3.1, 3.1.4 and 7.1.
    /// Esegue la lettura atomica delle squadre aziendali e mappa i campi pipe-separated.
    /// </summary>
    public class Squadre_APP : BaseDALAnagrafe, ISquadre_APP
    {
        public Squadre_APP(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        /// <summary>
        /// Ref: DS08-BL sections 3.1.3 and 7.1.
        /// Legge le squadre per partita IVA e restituisce entità già tipizzate.
        /// </summary>
        public async Task<List<SquadreEntity>> LeggiAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti)
        {
            _ = objParametriUtenti;

            if (string.IsNullOrWhiteSpace(piva))
                throw new ArgumentException("Specificare la partita iva.", nameof(piva));

            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery
                .AppendLine("SELECT")
                .AppendLine("    id_squadra AS codiceSquadra,")
                .AppendLine("    des_squadra AS descrizione,")
                .AppendLine("    cod_risum_list AS caposquadra_raw,")
                .AppendLine("    cod_risum_caposquadra_list AS membri_raw,")
                .AppendLine("    Validita_Inizio AS validoDal,")
                .AppendLine("    Validita_Fine AS validoAL")
                .AppendLine("FROM SquadreXAttivita")
                .AppendLine("WHERE piva = @piva");

            parSql.Add("@piva", piva);

            try
            {
                var dt = await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), parSql);

                var result = new List<SquadreEntity>(dt.Rows.Count);
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    result.Add(
                        new SquadreEntity
                        {
                            codiceSquadra = Convert.ToString(row["codiceSquadra"], CultureInfo.InvariantCulture)?.Trim() ?? string.Empty,
                            descrizione = Convert.ToString(row["descrizione"], CultureInfo.InvariantCulture)?.Trim() ?? string.Empty,
                            caposquadra = ParsePipeSeparatedList(row["caposquadra_raw"]),
                            membri = ParsePipeSeparatedList(row["membri_raw"]),
                            validoDal = ParseNullableDateTime(row["validoDal"]),
                            validoAL = ParseNullableDateTime(row["validoAL"]),
                        }
                    );
                }

                return result;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        private static List<string> ParsePipeSeparatedList(object value)
        {
            if (value == DBNull.Value)
                return new List<string>();

            var rawValue = Convert.ToString(value, CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(rawValue))
                return new List<string>();

            return rawValue
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToList();
        }

        private static DateTime? ParseNullableDateTime(object value)
        {
            if (value == DBNull.Value)
                return null;

            var rawValue = Convert.ToString(value, CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(rawValue))
                return null;

            return DateTime.TryParse(rawValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed)
                ? parsed
                : null;
        }
    }
}