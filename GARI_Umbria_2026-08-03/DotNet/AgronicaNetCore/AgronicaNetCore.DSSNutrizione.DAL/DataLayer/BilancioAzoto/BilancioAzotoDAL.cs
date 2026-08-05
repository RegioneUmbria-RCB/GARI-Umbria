using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System.Data;
using System.Text;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer.BilancioAzoto
{
    /// <summary>
    /// Implementazione DAL per il caricamento degli eventi della timeline del bilancio azoto.
    /// Restituisce due categorie di eventi nell'arco di validità dell'impianto (Imprese_Progetti):
    ///   - BBCH_CHANGE: operazioni QDCA Lav_Cod=79 (rilievo fasi fenologiche) filtrate per destinazione impianto.
    ///   - ANALISI_TERRENO: nuove analisi terreno da Analisi_EntitaxTestata / Analisi_Testata.
    /// La query per BBCH_CHANGE segue il pattern di FertilizzazioniDAL.LeggiMacroelementiDistribuitiAsync().
    /// La query per ANALISI_TERRENO segue il pattern della CTE UltimaAnalisiTerreno in LeggiEserciziDSSNutrizioneAsync().
    /// Riferimento: DS09-BL — Persistenze Coinvolte.
    /// </summary>
    public sealed class BilancioAzotoDAL : DAL_Base, IBilancioAzotoDAL
    {
        public BilancioAzotoDAL(IServiceProvider provider) : base(provider)
        {
        }

        /// <inheritdoc/>
        public async Task<DataTable> LeggiEventiTimelineAsync(
            string piva,
            int saCod,
            int appezza,
            int idReg,
            int progettoCod,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(piva))
                throw new ArgumentException("PIVA è obbligatoria.", nameof(piva));

            var parameters = new Dictionary<string, object>
            {
                ["@piva"]        = piva,
                ["@saCod"]       = saCod,
                ["@appezza"]     = appezza,
                ["@idReg"]       = idReg,
                ["@progettoCod"] = progettoCod,
                ["@lavCodBbch"]  = LAV_COD.LAVCOD_FASI_FENOLOGICHE
            };

            var sql = new StringBuilder();
            sql.AppendLine("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
            sql.AppendLine();

            // ── CTE: finestra di validità dell'impianto ──────────────────────────────
            // DS09-BL: "Timeline copre la validità dell'impianto corrente
            //           (Validita_Inizio e Validita_Fine dell'Imprese_Progetti)".
            sql.AppendLine("WITH ImpiantoValidita AS (");
            sql.AppendLine("    SELECT ip.Validita_Inizio, ip.Validita_Fine");
            sql.AppendLine("    FROM Imprese_Progetti ip");
            sql.AppendLine("    WHERE ip.Piva         = @piva");
            sql.AppendLine("      AND ip.Sa_Cod       = @saCod");
            sql.AppendLine("      AND ip.Appezza      = @appezza");
            sql.AppendLine("      AND ip.Id_Reg       = @idReg");
            sql.AppendLine("      AND ip.Progetto_Cod = @progettoCod");
            sql.AppendLine("),");

            // ── CTE: eventi BBCH_CHANGE (Lav_Cod=79, Cau_Mov='2100') ─────────────────
            // Pattern: vedi OUTER APPLY in FertilizzazioniDAL.LeggiMacroelementiDistribuitiAsync
            // e CTE UltimaFaseFenologica in LeggiEserciziDSSNutrizioneAsync.
            sql.AppendLine("EventiBbch AS (");
            sql.AppendLine("    SELECT DISTINCT");
            sql.AppendLine("        iv.Validita_Inizio,");
            sql.AppendLine("        iv.Validita_Fine,");
            sql.AppendLine("        'BBCH_CHANGE'                                                          AS Tipo_Evento,");
            sql.AppendLine("        CAST(mov.Data_Movimento AS DATE)                                      AS Data_Evento,");
            sql.AppendLine("        CONCAT(scb.Stadio_Principale, scb.Seconda_Cifra, scb.Terza_Cifra)     AS BBCH_Cod,");
            sql.AppendLine("        mddest.Id_Agenda,");
            sql.AppendLine("        mddest.Id_Mov,");
            sql.AppendLine("        mddest.Id_Mov_Det,");
            sql.AppendLine("        CAST(NULL AS INT)           AS Analisi_Testata_Cod,");
            sql.AppendLine("        CAST(NULL AS NVARCHAR(50))  AS Analisi_SuperUser,");
            sql.AppendLine("        CAST(NULL AS FLOAT)         AS Sabbia_Percentuale,");
            sql.AppendLine("        CAST(NULL AS FLOAT)         AS Limo_Percentuale,");
            sql.AppendLine("        CAST(NULL AS FLOAT)         AS Argilla_Percentuale,");
            sql.AppendLine("        CAST(NULL AS FLOAT)         AS N_Totale");
            sql.AppendLine("    FROM ImpiantoValidita iv");
            sql.AppendLine("    INNER JOIN Agenda ag");
            sql.AppendLine("        ON  ag.PIVA    = @piva");
            sql.AppendLine("        AND ag.Sa_Cod  = @saCod");
            sql.AppendLine("        AND ag.Lav_Cod = @lavCodBbch");
            sql.AppendLine("    INNER JOIN Movimenti mov");
            sql.AppendLine("        ON  mov.PIVA       = ag.PIVA");
            sql.AppendLine("        AND mov.Sa_Cod     = ag.Sa_Cod");
            sql.AppendLine("        AND mov.Id_Agenda  = ag.Id_Agenda");
            sql.AppendLine("        AND mov.Cau_Mov    = '2100'");
            sql.AppendLine("        AND mov.Data_Movimento >= iv.Validita_Inizio");
            sql.AppendLine("        AND mov.Data_Movimento <= iv.Validita_Fine");
            sql.AppendLine("    INNER JOIN Mov_Destinazioni mddest");
            sql.AppendLine("        ON  mddest.PIVA            = mov.PIVA");
            sql.AppendLine("        AND mddest.Sa_Cod          = mov.Sa_Cod");
            sql.AppendLine("        AND mddest.Id_Agenda       = mov.Id_Agenda");
            sql.AppendLine("        AND mddest.Id_Mov          = mov.Id_Mov");
            sql.AppendLine("        AND mddest.APPEZZA         = @appezza");
            sql.AppendLine("        AND mddest.Id_Destinazione = @idReg");
            sql.AppendLine("    INNER JOIN Movimenti_dettagli mdtg");
            sql.AppendLine("        ON  mdtg.PIVA       = mddest.Piva");
            sql.AppendLine("        AND mdtg.Sa_Cod     = mddest.Sa_Cod");
            sql.AppendLine("        AND mdtg.Id_Agenda  = mddest.Id_Agenda");
            sql.AppendLine("        AND mdtg.Id_Mov     = mddest.Id_Mov");
            sql.AppendLine("        AND mdtg.Id_Mov_Det = mddest.Id_Mov_Det");
            sql.AppendLine("    INNER JOIN Mov_Dettaglio_Tecnico mdt");
            sql.AppendLine("        ON  mdt.Piva       = mdtg.PIVA");
            sql.AppendLine("        AND mdt.Sa_Cod     = mdtg.Sa_Cod");
            sql.AppendLine("        AND mdt.Id_Agenda  = mdtg.Id_Agenda");
            sql.AppendLine("        AND mdt.Id_Mov     = mdtg.Id_Mov");
            sql.AppendLine("        AND mdt.Id_Mov_Det = mdtg.Id_Mov_Det");
            sql.AppendLine("    LEFT JOIN SpecieVegetaliXStadiCrescita svsc");
            sql.AppendLine("        ON  svsc.Cod_SS = mdt.ff_classe");
            sql.AppendLine("    LEFT JOIN Stadi_Crescita_BBCH scb");
            sql.AppendLine("        ON  scb.ID_BBCH = svsc.ID_BBCH");
            sql.AppendLine("),");

            // ── CTE: eventi ANALISI_TERRENO (Analisi_EntitaxTestata + Analisi_Testata) ──
            // Pattern: vedi CTE UltimaAnalisiTerreno in Esercizi.LeggiEserciziDSSNutrizioneAsync.
            // Qui vengono restituite TUTTE le analisi nell'arco di validità (non solo l'ultima).
            sql.AppendLine("EventiAnalisi AS (");
            sql.AppendLine("    SELECT");
            sql.AppendLine("        iv.Validita_Inizio,");
            sql.AppendLine("        iv.Validita_Fine,");
            sql.AppendLine("        'ANALISI_TERRENO'                                                      AS Tipo_Evento,");
            sql.AppendLine("        CAST(atesta.Analisi_Testata_Data_Inizio AS DATE)                      AS Data_Evento,");
            sql.AppendLine("        CAST(NULL AS VARCHAR(20))    AS BBCH_Cod,");
            sql.AppendLine("        CAST(NULL AS INT)            AS Id_Agenda,");
            sql.AppendLine("        CAST(NULL AS INT)            AS Id_Mov,");
            sql.AppendLine("        CAST(NULL AS INT)            AS Id_Mov_Det,");
            sql.AppendLine("        aext.Analisi_Testata_Cod,");
            sql.AppendLine("        aext.Analisi_SuperUser,");
            sql.AppendLine($"        MAX(CASE WHEN ad.Analisi_Parametro_Cod = {(int)enum_AnalisiParametri.AnalisiParametri_Sabbia}  THEN ad.Analisi_Dettaglio_Valore_1 ELSE NULL END) AS Sabbia_Percentuale,");
            sql.AppendLine($"        MAX(CASE WHEN ad.Analisi_Parametro_Cod = {(int)enum_AnalisiParametri.AnalisiParametri_Limo}    THEN ad.Analisi_Dettaglio_Valore_1 ELSE NULL END) AS Limo_Percentuale,");
            sql.AppendLine($"        MAX(CASE WHEN ad.Analisi_Parametro_Cod = {(int)enum_AnalisiParametri.AnalisiParametri_Argilla} THEN ad.Analisi_Dettaglio_Valore_1 ELSE NULL END) AS Argilla_Percentuale,");
            sql.AppendLine($"        MAX(CASE WHEN ad.Analisi_Parametro_Cod = {(int)enum_AnalisiParametri.AnalisiParametri_Ntot}    THEN ad.Analisi_Dettaglio_Valore_1 ELSE NULL END) AS N_Totale");
            sql.AppendLine("    FROM ImpiantoValidita iv");
            sql.AppendLine("    INNER JOIN Analisi_EntitaxTestata aext");
            sql.AppendLine("        ON  aext.PIVA    = @piva");
            sql.AppendLine("        AND aext.SA_COD  = @saCod");
            sql.AppendLine("        AND aext.APPEZZA = @appezza");
            sql.AppendLine("        AND aext.Id_Imp  = @idReg");
            sql.AppendLine("    INNER JOIN Analisi_Testata atesta");
            sql.AppendLine("        ON  atesta.Analisi_SuperUser   = aext.Analisi_SuperUser");
            sql.AppendLine("        AND atesta.Analisi_Testata_Cod = aext.Analisi_Testata_Cod");
            sql.AppendLine("        AND atesta.Analisi_Testata_Data_Inizio >= iv.Validita_Inizio");
            sql.AppendLine("        AND atesta.Analisi_Testata_Data_Inizio <= iv.Validita_Fine");
            sql.AppendLine("    LEFT JOIN Analisi_Dettagli ad");
            sql.AppendLine("        ON  ad.Analisi_SuperUser   = aext.Analisi_SuperUser");
            sql.AppendLine("        AND ad.Analisi_Testata_Cod = aext.Analisi_Testata_Cod");
            sql.AppendLine("    GROUP BY");
            sql.AppendLine("        iv.Validita_Inizio, iv.Validita_Fine,");
            sql.AppendLine("        aext.Analisi_Testata_Cod, aext.Analisi_SuperUser,");
            sql.AppendLine("        atesta.Analisi_Testata_Data_Inizio");
            sql.AppendLine(")");

            // ── Query finale: UNION ALL + ORDER BY Data_Evento crescente ─────────────
            // DS09-BL: "eventi ordinati cronologicamente per data crescente".
            sql.AppendLine("SELECT * FROM EventiBbch");
            sql.AppendLine("UNION ALL");
            sql.AppendLine("SELECT * FROM EventiAnalisi");
            sql.AppendLine("ORDER BY Data_Evento;");

            try
            {
                DataTable dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql.ToString(), parameters);
                return dt ?? new DataTable();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
