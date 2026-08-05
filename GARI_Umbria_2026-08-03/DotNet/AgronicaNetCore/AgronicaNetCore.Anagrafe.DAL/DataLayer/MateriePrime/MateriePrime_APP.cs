using System.Data;
using System.Text;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.MateriePrime
{
    public class MateriePrime_APP : BaseDALAnagrafe, IMateriePrime_APP
    {
        private readonly TempChiaviMassivo _tempChiaviMassivo;
        public MateriePrime_APP(TempChiaviMassivo tempChiaviMassivo, IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _tempChiaviMassivo = tempChiaviMassivo;
        }

        /// <summary>
        /// Legge l'anagrafica materie prime per la categoria indicata.
        /// Corrisponde al ramo CAU_CARICO di MateriePrime_Anagrafica del legacy VB con ChkAlias = 0,
        /// Flag_AncheImportatati = false, senza gruppi merce, senza filtri vegetale/animale.
        /// </summary>
        public async Task<DataTable> ReadAnagraficaAsync(
            string piva,
            int elemCod,
            List<int> joinSpecieUtilizzate,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            if (elemCod == 0)
                throw new ArgumentException("Specificare la categoria prodotto (elemCod).");

            var stb = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            if (joinSpecieUtilizzate != null && joinSpecieUtilizzate.Count > 0)
                await _tempChiaviMassivo.CreaTabellaTemp_FiltroChiaveInt(joinSpecieUtilizzate, objParametriServer);

            stb.AppendLine("SELECT")
                .AppendLine("    Materie_Prime.Mat_Cod,")
                .AppendLine("    Materie_Prime.Mat_Des,")
                .AppendLine("    Materie_Prime.Cod_Articolo,")
                .AppendLine("    Materie_Prime.Udm_Cod,")
                .AppendLine("    Materie_Prime.Extra_Str,")
                .AppendLine("    CategorieMagazzino.NomeComune")
                .AppendLine("FROM Materie_Prime")
                .AppendLine(
                    "INNER JOIN CategorieMagazzino ON Materie_Prime.Elem_Cod = CategorieMagazzino.Elem_Cod"
                )
                .AppendLine("INNER JOIN UtentiXImprese ON Materie_Prime.Piva = UtentiXImprese.PIVA");

            if (joinSpecieUtilizzate != null && joinSpecieUtilizzate.Count > 0)
                stb.AppendLine("INNER JOIN #TempChiaveInt AS temp ON temp.Chiave = Materie_Prime.Veg_Cod");

            stb.AppendLine("WHERE")
                .AppendLine("    UtentiXImprese.[USER] = @pivaSuperUser")
                .AppendLine("    AND Materie_Prime.Validita_inizio <= @finestraFine")
                .AppendLine("    AND Materie_Prime.Validita_Fine >= @finestraInizio")
                .AppendLine(
                    string.IsNullOrEmpty(piva)
                        ? "    AND (Materie_Prime.Sa_Cod = -1)"
                        : "    AND (Materie_Prime.Piva = @piva OR Materie_Prime.Sa_Cod = -1)"
                )
                .AppendLine("    AND Materie_Prime.Elem_Cod = @elemCod")
                .AppendLine("    AND Materie_Prime.Mat_Cod_Origine = 0")
                .AppendLine("    AND Materie_Prime.ChkAlias = 0")
                .AppendLine("    AND Materie_Prime.Inviato >= 0")
                .AppendLine("ORDER BY CategorieMagazzino.NomeComune, Materie_Prime.Mat_Des ASC");

            parSql.Add("@pivaSuperUser", objParametriServer.PivaSuperUser);
            parSql.Add("@finestraFine", objParametriServer.FinestraTemporaleFine);
            parSql.Add("@finestraInizio", objParametriServer.FinestraTemporaleInizio);

            if (!string.IsNullOrEmpty(piva))
                parSql.Add("@piva", piva);

            parSql.Add("@elemCod", elemCod);

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stb.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            finally
            {
                if (joinSpecieUtilizzate != null && joinSpecieUtilizzate.Count > 0)
                    await _tempChiaviMassivo.EliminaTabellaTemp_FiltroChiaveInt(objParametriServer);
            }
        }
    }
}
