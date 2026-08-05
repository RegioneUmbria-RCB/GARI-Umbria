using System.Data;
using System.Text;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.AreaTipologie
{
    public class AreaTipologie_APP : BaseDALAnagrafe, IAreaTipologie_APP
    {
        public AreaTipologie_APP(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        public async Task<DataTable> ReadAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery
                .AppendLine(" SELECT")
                .AppendLine(
                    "   Alert_Area.ID_Area, Alert_Tipologia.ID_Tipologia, Alert_Area.Nome As Nome_Area, Alert_Tipologia.Nome As Nome_Tipologia"
                )
                .AppendLine(" FROM Alert_Tipologia")
                .AppendLine(
                    " INNER JOIN Alert_Area ON Alert_Area.Id_Area = Alert_Tipologia.ID_Area"
                )
                .AppendLine(" WHERE Alert_Tipologia.PivaSuperUser = @pivaSuperUser")
                .AppendLine(" AND Alert_Tipologia.Utilizzo_GiasAPP = 1")
                .AppendLine(
                    $" AND (Alert_Area.TipoEntita_Cod = 0 OR Alert_Area.ID_Area IN ({(int)Enum_CategorieDocumento.AgricolturaDiPrecisione},{(int)Enum_CategorieDocumento.Documenti_Contabili},{(int)Enum_CategorieDocumento.Operazioni_Campagna_QDC},{(int)Enum_CategorieDocumento.Carichi_Scarichi_Magazzino}))"
                );

            // Se sono stati inseriti record in CategTipologiaDocumentiXUtenti, l'utente deve essere autorizzato
            // su quella categoria o tipologia specifica. Il SuperUser vede tutto.
            if (
                !string.Equals(
                    objParametriServer.UtenteUsername,
                    objParametriServer.SuperUserUsername,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                stbQuery
                    .AppendLine(
                        " AND (EXISTS (SELECT 1 FROM CategTipologiaDocumentiXUtenti permessi"
                    )
                    .AppendLine(
                        "             WHERE permessi.ID_Categoria = Alert_Tipologia.ID_area"
                    )
                    .AppendLine(
                        "               AND (permessi.ID_Tipologia = Alert_Tipologia.ID_Tipologia OR permessi.ID_Tipologia = 0)"
                    )
                    .AppendLine("               AND permessi.Autorizzato = 1")
                    .AppendLine("               AND permessi.Username = @utenteUsername)")
                    .AppendLine(
                        "      OR (NOT EXISTS (SELECT 1 FROM CategTipologiaDocumentiXUtenti permessi)))"
                    );
                parSql.Add("@utenteUsername", objParametriServer.UtenteUsername);
            }

            parSql.Add("@pivaSuperUser", objParametriServer.PivaSuperUser);

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
