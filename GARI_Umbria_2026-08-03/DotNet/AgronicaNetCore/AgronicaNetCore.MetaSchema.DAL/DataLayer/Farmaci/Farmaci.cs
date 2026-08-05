using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Farmaci
{
    public class Farmaci : BaseDALMetaschema, IFarmaci
    {
        public Farmaci(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
        }

        public async Task<DataTable> LeggiFarmaciAsync(int Farm_Cod, string Aic, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            stbQuery.AppendLine(" SELECT * ");
            stbQuery.AppendLine(" FROM [dbo].[Farmaci] ");
            stbQuery.AppendLine(" WHERE 1 = 1 ");

            if (Farm_Cod != 0)
            {
                stbQuery.AppendLine(" AND Farm_Cod = @farmCod ");
                sqlParams.TryAdd("@farmCod", Farm_Cod);
            }

            if (Aic != "")
            {
                stbQuery.AppendLine(" AND AIC = @aic ");
                sqlParams.TryAdd("@aic", Aic);
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

        public async Task<DataTable> LeggiFarmaciListAICAsync(int Farm_Cod, List<string> Aic, List<string> FamigliaAic, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

            stbQuery.AppendLine(" SELECT * ");
            stbQuery.AppendLine(" FROM [dbo].[Farmaci] ");
            stbQuery.AppendLine(" WHERE 1 = 1 ");

            if (Farm_Cod != 0)
            {
                stbQuery.AppendLine(" AND Farm_Cod = @farmCod ");
                sqlParams.TryAdd("@farmCod", Farm_Cod);
            }

            if (Aic != null && Aic.Count > 0)
            {
                parSqlIn.Add("@AIC", FormatClauseIn(Aic));
                stbQuery.AppendLine(" AND AIC IN (@AIC) ");
            }
            if (FamigliaAic != null && FamigliaAic.Count > 0)
            {
                //SUBSTRING(AIC, 0, 7)
                parSqlIn.Add("@FamAIC", FormatClauseIn(FamigliaAic));
                stbQuery.AppendLine(" AND SUBSTRING(AIC, 0, 7) IN (@FamAIC) ");
            }

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

        }

        public async Task<DataTable> ReadFarmacixCategoriaSemplificataAsync(int Farm_Cod, string Aic, int Farm_Cat, int Farm_Cat_Sim, AgronicaCoreParametri objP)
        {
            try
            {
                var stbQuery = new StringBuilder();
                var sqlParams = new Dictionary<string, object>();


                if (Aic.Length > 6)
                    stbQuery.AppendLine("SELECT ");
                else
                    stbQuery.AppendLine("SELECT DISTINCT ");

                stbQuery.AppendLine("    farm.Farm_Cod, farm.AIC, farm.Denominazione, ");
                
                if (Aic.Length > 6) stbQuery.AppendLine("    farm.Confezione, farm.ModalitaPrescrizione, farm.Codice_GTIN, ");

                stbQuery
                    .AppendLine("    farmCat.Id AS FarmCat_Id, farmCat.Categoria_Codice AS FarmCat_Cod, farmCat.Categoria_Descrizione AS FarmCat_Des, ")
                    .AppendLine("    farmCatS.ID AS FarmCatS_Id, farmCatS.Descrizione AS FarmCatS_Des ")
                    .AppendLine("FROM Farmaci farm ")
                    .AppendLine("INNER JOIN FarmacixCategorie fxc ON farm.Farm_Cod = fxc.Farm_Cod ")
                    .AppendLine("INNER JOIN Farmaci_Categorie farmCat ON fxc.Cat_Cod = farmCat.ID ")
                    .AppendLine("INNER JOIN Farmaci_CategoriexFarmaci_Categorie_Semplificate fcxfcs ON farmCat.ID = fcxfcs.ID_Categoria ")
                    .AppendLine("INNER JOIN Farmaci_Categorie_Semplificate farmCatS ON fcxfcs.ID_Categoria_Semplificata = farmCatS.ID ")
                    .AppendLine("WHERE 1=1 ");

                if (Farm_Cod != 0)
                {
                    sqlParams.TryAdd("@farmCod", Farm_Cod);
                    stbQuery.AppendLine("    AND farm.Farm_Cod = @farmCod ");
                }
                if (!string.IsNullOrEmpty(Aic))
                    // Famiglia AIC
                    if (Aic.Length == 6)
                    {
                        sqlParams.TryAdd("@famigliaAic", Aic + "%");
                        stbQuery.AppendLine("    AND farm.AIC LIKE @famigliaAic ");
                    }
                    // Codice AIC
                    else
                    {
                        sqlParams.TryAdd("@aic", Aic);
                        stbQuery.AppendLine("    AND farm.AIC = @aic ");
                    }
                if (Farm_Cat != 0)
                {
                    sqlParams.TryAdd("@farmCat", Farm_Cat);
                    stbQuery.AppendLine("    AND farmCat.ID = @farmCat ");
                }
                if (Farm_Cat_Sim != 0)
                {
                    sqlParams.TryAdd("@farmCatSim", Farm_Cat_Sim);
                    stbQuery.AppendLine("    AND farmCatS.ID = @farmCatSim ");
                }

                return await GetDataProvider(objP).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }
    }
}
