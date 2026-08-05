using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgronicaNetCore.Base.Constants;

namespace AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogInvio.AgronicaLogInvioMateriePrime
{
    /// <summary>
    /// Implementazione del DAL per la sincronizzazione delle Materie Prime.
    /// Segue il pattern aziendale ereditando da DAL_Base.
    /// </summary>
    public class AgronicaLogInvioMateriePrime : DAL_Base, IAgronicaLogInvioMateriePrime
    {
        public AgronicaLogInvioMateriePrime(IServiceProvider provider) : base(provider) { }

        public async Task<DataTable> GetAnagraficheDaSincronizzareAsync(
        string tipoAnagrafica,
        int tipoEsportazione,
        int? saCodFilter,
        IEnumerable<int> elemCodFilter,
        AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            stbQuery.AppendLine(@"
            -- Pulisce le tabelle se esistono da sessioni precedenti
            IF OBJECT_ID('tempdb..#UltimoLogPerMateriaPrima') IS NOT NULL DROP TABLE #UltimoLogPerMateriaPrima;
            IF OBJECT_ID('tempdb..#EsitoUltimoInvio') IS NOT NULL DROP TABLE #EsitoUltimoInvio;

            -- Step 1: Crea la prima tabella temporanea
            SELECT
                Mat_Cod,
                MAX(ID_Log_Invio) as LastLogId
            INTO #UltimoLogPerMateriaPrima
            FROM Agronica_Log_Invio_Anagrafe
            WHERE
                Tipo_Esportazione = @TipoEsportazione
                AND Tipo = @TipoAnagrafica
                AND Mat_Cod IS NOT NULL
            GROUP BY Mat_Cod;

            -- Step 2: Crea la seconda tabella temporanea
            SELECT
                u.Mat_Cod,
                c.Esito, 
                c.ID AS ID_Chiamata, 
                c.Data_Invio
            INTO #EsitoUltimoInvio
            FROM #UltimoLogPerMateriaPrima u
            JOIN Agronica_Log_Invio_Chiamate c ON u.LastLogId = c.ID;

            -- Step 3: Query finale di selezione
            SELECT 
                mp.Mat_Cod,
                'UPD' AS TipoOperazione,
                ISNULL(e.Esito, '') AS Esito,
                ISNULL(e.ID_Chiamata, 0) AS ID_Chiamata
            FROM Materie_Prime mp
            LEFT JOIN #EsitoUltimoInvio e ON mp.Mat_Cod = e.Mat_Cod
            WHERE 1=1 
            ");


            // --- FILTRI DINAMICI ---
            if (saCodFilter.HasValue)
            {
                stbQuery.AppendLine(" AND mp.Sa_Cod = @SaCodFilter");
                sqlParams.Add("@SaCodFilter", saCodFilter.Value);
            }
            if (elemCodFilter != null && elemCodFilter.Any())
            {
                //stbQuery.AppendLine(" AND mp.Elem_Cod IN @ElemCodFilter");
                //sqlParams.Add("@ElemCodFilter", elemCodFilter);
                // Trasforma la lista di int in una stringa "1,2,3"
                string ids = string.Join(",", elemCodFilter);

                // Inietta la stringa direttamente nella query (perché sono int)
                stbQuery.AppendLine($" AND mp.Elem_Cod IN ({ids})");
            }
            stbQuery.AppendLine(@"
                AND (
                    e.ID_Chiamata IS NULL 
                    OR e.Esito = @EsitoFallimento
                    OR mp.data_modifica > e.Data_Invio
                );
            ");

            stbQuery.AppendLine("DROP TABLE #UltimoLogPerMateriaPrima;");
            stbQuery.AppendLine("DROP TABLE #EsitoUltimoInvio;");

            sqlParams.Add("@TipoEsportazione", tipoEsportazione);
            sqlParams.Add("@TipoAnagrafica", tipoAnagrafica);
            sqlParams.Add("@EsitoFallimento", TipiEsito.Fallimento);

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
    }
}
