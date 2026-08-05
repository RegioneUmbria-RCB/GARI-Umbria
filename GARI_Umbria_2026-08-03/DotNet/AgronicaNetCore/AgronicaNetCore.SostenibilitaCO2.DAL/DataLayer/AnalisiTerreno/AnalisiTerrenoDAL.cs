using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.AnalisiTerreno
{
    /// <summary>
    /// Implementazione dell'accesso alla tabella <c>Analisi_Terreno</c>
    /// per il recupero del parametro Sostanza Organica (<c>SO</c>).
    /// Riferimento spec: DS04-BL AssemblyPayloadM4AppezzamentoImpianto — Persistenze, Tabella <c>Analisi_Terreno</c>, Regola 3.
    /// </summary>
    public class AnalisiTerrenoDAL : BaseDALSostenibilitaCO2, IAnalisiTerrenoDAL
    {
        // Analisi_Parametro_Cod = 9 → Sostanza Organica
        private const int CodiceParametroSO = 9;
        // Analisi_Entita_Cod = 4 → entità di tipo Appezzamento (enum_Entita_Analisi.Appezzamento)
        private const int CodiceEntitaAppezzamento = 4;

        public AnalisiTerrenoDAL(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
        }

        /// <inheritdoc/>
        public async Task<AnalisiTerrenoEntity?> GetSostanzaOrganicaAsync(
            string piva,
            int saCod,
            int appezza,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(piva))    throw new ArgumentException("Specificare la partita IVA.",    nameof(piva));
            if (saCod == 0)   throw new ArgumentException("Specificare il codice Sa_Cod.",  nameof(saCod));
            if (appezza < 0) throw new ArgumentException("Specificare il codice Appezza.", nameof(appezza));

            // Analisi più recente del parametro SO (cod=9) per l'appezzamento (entità cod=4).
            const string sql = @"
                SELECT TOP 1
                    aet.piva,
                    aet.sa_cod,
                    aet.appezza,
                    ad.Analisi_Dettaglio_Valore_1 AS SostanzaOrganica
                FROM Analisi_Testata at
                INNER JOIN Analisi_EntitaxTestata aet ON aet.Analisi_Testata_Cod = at.Analisi_Testata_Cod
                INNER JOIN Analisi_Dettagli ad        ON ad.Analisi_Testata_Cod  = at.Analisi_Testata_Cod
                                                     AND ad.Analisi_Parametro_Cod = @parametroCod
                WHERE aet.Analisi_Entita_Cod = @entitaCod
                  AND aet.piva    = @piva
                  AND aet.Sa_Cod  = @saCod
                  AND aet.Appezza = @appezza
                ORDER BY at.Validita_Inizio DESC";

            var parametriSql = new Dictionary<string, object>
            {
                { "@piva",         piva                      },
                { "@saCod",        saCod                     },
                { "@appezza",      appezza                   },
                { "@parametroCod", CodiceParametroSO         },
                { "@entitaCod",    CodiceEntitaAppezzamento  }
            };

            DataTable? dt = null;
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

            var row = dt.Rows[0];
            decimal? sostanzaOrganica = null;
            if (row["SostanzaOrganica"] != DBNull.Value
                && decimal.TryParse(row["SostanzaOrganica"]?.ToString(),
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var parsed))
            {
                sostanzaOrganica = parsed;
            }

            return new AnalisiTerrenoEntity
            {
                Piva             = row["Piva"]?.ToString()    ?? string.Empty,
                SaCod            = row["Sa_Cod"]?.ToString()  ?? string.Empty,
                Appezza          = row["Appezza"]?.ToString() ?? string.Empty,
                SostanzaOrganica = sostanzaOrganica
            };
        }
    }
}
