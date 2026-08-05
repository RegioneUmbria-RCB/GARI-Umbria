using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaDataProvider6.Extensions;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Apezzamenti
{
    public class Appezzamenti : BaseDALAnagrafe, IAppezzamenti
    {
        private readonly int BLOCK_FLAG_VALUE = -1;

        public Appezzamenti(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, localizer, securityBypass)
        {
        }

        /// <summary>
        /// Permette di blocca/sbloccare un appezzamento.
        /// </summary>
        /// <param name="block">True se l'appezzamento è da bloccare, False altrimenti.</param>
        /// <returns>True se l'operazione è andata a buon fine, False altrimenti.</returns>
        public async Task<bool> BloccaSbloccaAsync(bool block, Appezzamento.PK appezzamento, AgronicaCoreParametriServer objParametriServer, DateTime? blockDate)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            try
            {
                stbQuery.AppendLine("UPDATE Appezzamento SET ");
                stbQuery.AppendLine("  Username_Modifica = @user, ");
                stbQuery.AppendLine("  Data_Modifica = GETDATE(), ");
                if (block)
                {
                    stbQuery.AppendLine("  Blk_Flag = @blkflag, ");
                    stbQuery.AppendLine("  Blk_Inizio_Data = @blockDate, ");
                    stbQuery.AppendLine("  Blk_Inizio_Username = @user, ");
                    stbQuery.AppendLine("  Blk_Inizio_Note = '', ");
                    stbQuery.AppendLine("  Blk_Fine_Data = @agrofine, ");
                    stbQuery.AppendLine("  Blk_Fine_Username = @user, ");
                    stbQuery.AppendLine("  Blk_Fine_Note = '' ");
                }
                else
                {
                    stbQuery.AppendLine("  Blk_Flag = 0, ");
                    stbQuery.AppendLine("  Blk_Inizio_Data = @agroinizio, ");
                    stbQuery.AppendLine("  Blk_Inizio_Username = '', ");
                    stbQuery.AppendLine("  Blk_Inizio_Note = '', ");
                    stbQuery.AppendLine("  Blk_Fine_Data = @agrofine, ");
                    stbQuery.AppendLine("  Blk_Fine_Username = '', ");
                    stbQuery.AppendLine("  Blk_Fine_Note = '' ");
                }
                stbQuery.AppendLine("WHERE");
                stbQuery.AppendLine(" Appezzamento.piva = @piva ");
                stbQuery.AppendLine(" AND Appezzamento.sa_cod = @saCod ");
                stbQuery.AppendLine(" AND Appezzamento.appezza = @appezza ");

                if (appezzamento.centroAziendalePK.partitaIva == "" || appezzamento.centroAziendalePK.codice == 0 || appezzamento.codice == 0)
                {
                    throw new ArgumentException(_localizer.GetString("InvalidParameters").Value);
                }
                if (!blockDate.HasValue)
                {
                    blockDate = DateTime.Now;
                }
                sqlParams.TryAdd("@blkflag", BLOCK_FLAG_VALUE);
                sqlParams.TryAdd("@user", objParametriServer.UsernameOperazione);
                sqlParams.TryAdd("@agrofine", CostantiPersonalizzate.AGRODATAFINE);
                sqlParams.TryAdd("@agroinizio", CostantiPersonalizzate.AGRODATAINIZIO);
                sqlParams.TryAdd("@piva", appezzamento.centroAziendalePK.partitaIva);
                sqlParams.TryAdd("@saCod", appezzamento.centroAziendalePK.codice);
                sqlParams.TryAdd("@appezza", appezzamento.codice);
                sqlParams.TryAdd("@blockDate", blockDate.Value.ToString("yyyy-MM-ddTHH:mm:ss"));

                await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
                return true;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                return false;
            }
        }

        public async Task<bool> IsBlockedAsync(Appezzamento.PK appezzamento, DateTime atDate, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            try
            {
                stbQuery.AppendLine("SELECT ");
                stbQuery.AppendLine(" Blk_Flag, Blk_Inizio_Data, Blk_Fine_Data ");
                stbQuery.AppendLine("FROM Appezzamento ");

                stbQuery.AppendLine("WHERE");
                stbQuery.AppendLine(" Appezzamento.piva = @piva ");
                stbQuery.AppendLine(" AND Appezzamento.sa_cod = @saCod ");
                stbQuery.AppendLine(" AND Appezzamento.appezza = @appezza ");

                if (appezzamento.centroAziendalePK.partitaIva == "" || appezzamento.centroAziendalePK.codice == 0 || appezzamento.codice == 0)
                {
                    throw new ArgumentException(_localizer.GetString("InvalidParameters").Value);
                }
                sqlParams.TryAdd("@piva", appezzamento.centroAziendalePK.partitaIva);
                sqlParams.TryAdd("@saCod", appezzamento.centroAziendalePK.codice);
                sqlParams.TryAdd("@appezza", appezzamento.codice);

                DataTable dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
                if (dt == null || dt.Rows.Count != 1)
                {
                    throw new InvalidOperationException(_localizer.GetString("NoRowsFound").Value);
                }
                DataRow row = dt.Rows[0];
                return (int)row["Blk_Flag"] == BLOCK_FLAG_VALUE &&
                    atDate.IsInRange((DateTime)row["Blk_Inizio_Data"], (DateTime)row["Blk_Fine_Data"]);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> GetAppezzamentixParticelleAsync(bool filtraChiaviTmp, AgronicaCoreParametriServer objParametriServer, bool soloChiavi = false)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            try
            {
                stbQuery.AppendLine("SELECT ");
                stbQuery.AppendLine("    axp.[PIVA] ");
                stbQuery.AppendLine("  , axp.[SA_COD] ");
                stbQuery.AppendLine("  , axp.[APPEZZA] ");
                stbQuery.AppendLine("  , axp.[PROV] ");
                stbQuery.AppendLine("  , axp.[COM] ");

                if (!soloChiavi)
                {
                    stbQuery.AppendLine("  , axp.[SEZIONE] ");
                    stbQuery.AppendLine("  , axp.[FOGLIO] ");
                    stbQuery.AppendLine("  , axp.[NUMERO] ");
                    stbQuery.AppendLine("  , axp.[SUBALTERNO] ");
                    stbQuery.AppendLine("  , axp.[AREA] ");
                    stbQuery.AppendLine("  , axp.[SAU_Convenz_Ettari] ");
                    stbQuery.AppendLine("  , axp.[SAU_Convenz_Are] ");
                    stbQuery.AppendLine("  , axp.[SAU_Convenz_Centiare] ");
                    stbQuery.AppendLine("  , axp.[SAU_Convers_Ettari] ");
                    stbQuery.AppendLine("  , axp.[SAU_Convers_Are] ");
                    stbQuery.AppendLine("  , axp.[SAU_Convers_Centiare] ");
                    stbQuery.AppendLine("  , axp.[SAU_Bio_Ettari] ");
                    stbQuery.AppendLine("  , axp.[SAU_Bio_Are] ");
                    stbQuery.AppendLine("  , axp.[SAU_Bio_Centiare] ");
                }
                stbQuery.AppendLine("FROM AppezzamentixParticelle axp ");

                if (filtraChiaviTmp)
                {
                    stbQuery.AppendLine(" INNER JOIN #TempAppezzamento t ON ");
                    stbQuery.AppendLine("     axp.piva = t.piva ");
                    stbQuery.AppendLine(" AND axp.sa_cod = t.sa_cod ");
                    stbQuery.AppendLine(" AND axp.appezza = t.appezza ");
                }

                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);

            }

            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
