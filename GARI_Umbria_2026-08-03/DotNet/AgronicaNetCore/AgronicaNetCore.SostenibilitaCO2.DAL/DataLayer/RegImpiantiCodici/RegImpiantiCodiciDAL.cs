using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.RegImpiantiCodici
{
    /// <summary>
    /// Implementazione dell'accesso alla tabella <c>Reg_Impianti_Codici</c>
    /// per il recupero dell'anno di impianto nel contesto del calcolo CO2.
    /// Riferimento spec: DS04-BL AssemblyPayloadM4AppezzamentoImpianto — Regola 5 (Anno Impianto).
    /// </summary>
    public class RegImpiantiCodiciDAL : BaseDALSostenibilitaCO2, IRegImpiantiCodiciDAL
    {
        // id_cod = 1362 identifica il codice anno impianto in Reg_Impianti_Codici

        public RegImpiantiCodiciDAL(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
        }

        /// <inheritdoc/>
        public async Task<int?> GetAnnoImpiantoAsync(
            string piva,
            int saCod,
            int appezza,
            int idReg,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(piva)) throw new ArgumentException("Specificare la partita IVA.", nameof(piva));
            if (saCod == 0)  throw new ArgumentException("Specificare il codice Sa_Cod.",  nameof(saCod));
            if (appezza < 0) throw new ArgumentException("Specificare il codice Appezza.", nameof(appezza));

            const string sql = @"
                SELECT TOP 1 ric.val_cod
                FROM Reg_Impianti_Codici ric
                WHERE ric.piva    = @piva
                  AND ric.sa_cod  = @saCod
                  AND ric.appezza = @appezza
                  AND ric.Id_Reg  = @idReg
                  AND ric.id_cod  = @idCod";

            var parametriSql = new Dictionary<string, object>
            {
                { "@piva",    piva              },
                { "@saCod",   saCod             },
                { "@appezza", appezza           },
                { "@idReg",   idReg             },
                { "@idCod",   REG_IMPIANTI_CODICI.ANNO_IMPIANTO }
            };

            DataTable? dt;
            try
            {
                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            if (dt == null || dt.Rows.Count == 0)
                return null;

            var valCod = dt.Rows[0]["val_cod"];
            if (valCod == DBNull.Value)
                return null;

            return int.TryParse(valCod.ToString(), out var anno) ? anno : null;
        }

        /// <inheritdoc/>
        public async Task<int?> GetIdDestinazioneUsoAsync(
            string piva,
            int saCod,
            int appezza,
            int idReg,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(piva)) throw new ArgumentException("Specificare la partita IVA.", nameof(piva));
            if (saCod == 0) throw new ArgumentException("Specificare il codice Sa_Cod.", nameof(saCod));
            if (appezza < 0) throw new ArgumentException("Specificare il codice Appezza.", nameof(appezza));

            const string sql = @"
                SELECT TOP 1 ric.id_cod
                FROM Reg_Impianti_Codici ric
                WHERE ric.piva    = @piva
                  AND ric.sa_cod  = @saCod
                  AND ric.appezza = @appezza
                  AND ric.Id_Reg  = @idReg
                  AND ric.id_cod BETWEEN @minIdCod AND @maxIdCod
                ORDER BY ric.id_cod";

            var parametriSql = new Dictionary<string, object>
            {
                { "@piva", piva },
                { "@saCod", saCod },
                { "@appezza", appezza },
                { "@idReg", idReg },
                { "@minIdCod", 3000 },
                { "@maxIdCod", 4000 }
            };

            DataTable? dt;
            try
            {
                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            if (dt == null || dt.Rows.Count == 0)
                return null;

            var idCod = dt.Rows[0]["id_cod"];
            if (idCod == DBNull.Value)
                return null;

            return int.TryParse(idCod.ToString(), out var destinazioneUso) ? destinazioneUso : null;
        }
    }
}
