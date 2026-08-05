using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Operazione.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Fertilizzazioni
{
    /// <summary>
    /// Implementazione DAL per la lettura delle fertilizzazioni minerali precedenti.
    /// Porta la logica di <c>Leggi_Macroelementi_Distribuiti_List_Id_Agenda_Esclusi()</c>
    /// (AgronicaCoreContabDAL) con l'aggiunta della join con <c>Impianto_Fasi_Fenologiche_Engine</c>
    /// per ricavare la fase BBCH attiva al momento di ciascuna applicazione.
    /// Espande ogni evento di fertilizzazione in tante righe quanti sono gli elementi non nulli (N, P, K).
    /// Riferimento: DS04-BL — Regole di Business (Fertilizzazioni considerate).
    /// </summary>
    public sealed class FertilizzazioniDAL : BaseDALOperazione, IFertilizzazioniDAL
    {
        public FertilizzazioniDAL(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
        }

        /// <inheritdoc/>
        public async Task<DataTable> LeggiMacroelementiDistribuitiAsync(
            IReadOnlyList<int> lavCodList,
            string piva,
            int saCod,
            int appezza,
            int idReg,
            DateTime dataAnalisi,
            DateTime dataConsiglio,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(piva))
                throw new ArgumentException("PIVA è obbligatoria.", nameof(piva));
            if (lavCodList is null || lavCodList.Count == 0)
                throw new ArgumentException("La lista lavCodList non può essere vuota.", nameof(lavCodList));

            // Lav_Cod di operazioni di fertilizzazione passati dal chiamante.
            var sql = $@"
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

WITH FertilizzazioniBase AS (
    SELECT
        md.Id_Agenda,
        md.Id_Mov,
        md.Id_Mov_Det,
        mov.Data_Movimento,
        ISNULL(mdt.N,          0) AS N_Tecnico,
        ISNULL(mdt.P,          0) AS P_Tecnico,
        ISNULL(mdt.K,          0) AS K_Tecnico,
        ISNULL(mdt.Efficienza, 1) AS Efficienza,
        mdest.Qta                 AS Qta,
        CASE
            WHEN ISNULL(mdest.Qta2, 0) = 0 THEN ri.Sup_Imp
            ELSE mdest.Qta2
        END                       AS Sup_Tratt,
        fase.BBCH_Cod          AS Fase_BBCH
    FROM Agenda ag
    INNER JOIN Movimenti mov
        ON  mov.PIVA      = ag.PIVA
        AND mov.Sa_Cod    = ag.Sa_Cod
        AND mov.Id_Agenda = ag.Id_Agenda
    INNER JOIN Movimenti_dettagli md
        ON  md.PIVA      = mov.PIVA
        AND md.Sa_Cod    = mov.Sa_Cod
        AND md.Id_Agenda = mov.Id_Agenda
        AND md.Id_Mov    = mov.Id_Mov
    INNER JOIN Fertilizzanti f ON f.Fer_Cod = md.Pro_Cod
    INNER JOIN Mov_Destinazioni mdest
        ON  mdest.PIVA           = md.PIVA
        AND mdest.Sa_Cod         = md.Sa_Cod
        AND mdest.Id_Agenda      = md.Id_Agenda
        AND mdest.Id_Mov         = md.Id_Mov
        AND mdest.Id_Mov_Det     = md.Id_Mov_Det
    INNER JOIN Reg_Impianti ri
        ON  ri.PIVA    = mdest.Piva
        AND ri.SA_COD  = mdest.Sa_Cod
        AND ri.APPEZZA = mdest.Appezza
        AND ri.ID_REG  = mdest.Id_Destinazione
    INNER JOIN Imprese_Progetti ip
        ON  ip.Piva    = ri.PIVA
        AND ip.Sa_Cod  = ri.SA_COD
        AND ip.Appezza = ri.APPEZZA
        AND ip.Id_Reg  = ri.ID_REG
    LEFT JOIN Mov_Dettaglio_Tecnico mdt
        ON  mdt.Piva       = md.PIVA
        AND mdt.Sa_Cod     = md.Sa_Cod
        AND mdt.Id_Agenda  = md.Id_Agenda
        AND mdt.Id_Mov     = md.Id_Mov
        AND mdt.Id_Mov_Det = md.Id_Mov_Det
    -- Fase BBCH più recente tra quelle stimate <= data movimento (OUTER APPLY → NULL se assente)
    OUTER APPLY (
        SELECT TOP 1 CONCAT(scb.Stadio_Principale, scb.Seconda_Cifra, scb.Terza_Cifra) AS BBCH_Cod
        FROM Mov_Destinazioni mddestFF
        INNER JOIN Movimenti_dettagli mdtgFF
            ON  mdtgFF.PIVA       = mddestFF.PIVA  
            AND mdtgFF.Sa_Cod     = mddestFF.Sa_Cod
            AND mdtgFF.Id_Agenda  = mddestFF.Id_Agenda
            AND mdtgFF.Id_Mov     = mddestFF.Id_Mov
            AND mdtgFF.Id_Mov_Det = mddestFF.Id_Mov_Det
        INNER JOIN Mov_Dettaglio_Tecnico mdtFF
            ON  mdtFF.Piva       = mdtgFF.PIVA
            AND mdtFF.Sa_Cod     = mdtgFF.Sa_Cod
            AND mdtFF.Id_Agenda  = mdtgFF.Id_Agenda
            AND mdtFF.Id_Mov     = mdtgFF.Id_Mov
            AND mdtFF.Id_Mov_Det = mdtgFF.Id_Mov_Det
        INNER JOIN Movimenti movFF
            ON  movFF.PIVA      = mdtFF.PIVA
            AND movFF.Sa_Cod    = mdtFF.Sa_Cod
            AND movFF.Id_Agenda = mdtFF.Id_Agenda
        INNER JOIN Agenda agFF
            ON  agFF.PIVA      = movFF.PIVA
            AND agFF.Sa_Cod    = movFF.Sa_Cod
            AND agFF.Id_Agenda = movFF.Id_Agenda
        LEFT JOIN SpecieVegetaliXStadiCrescita svsc
            ON  svsc.Cod_SS    = mdtFF.ff_classe
        LEFT JOIN Stadi_Crescita_BBCH scb
            ON  scb.ID_BBCH    = svsc.ID_BBCH
        WHERE agFF.Lav_Cod = {LAV_COD.LAVCOD_FASI_FENOLOGICHE}
            AND mddestFF.PIVA           = ri.PIVA
            AND mddestFF.Sa_Cod         = ri.Sa_Cod
            AND mddestFF.APPEZZA         = ri.APPEZZA
            AND mddestFF.ID_Destinazione         = ri.ID_REG
            AND mddestFF.Validita_Inizio         <= mov.Data_Movimento
        ORDER BY CONCAT(scb.Stadio_Principale, scb.Seconda_Cifra, scb.Terza_Cifra) DESC,
                 mddestFF.Validita_Inizio DESC
    ) fase
    WHERE ag.Lav_Cod IN ({string.Join(", ", lavCodList)})
      AND mdest.PIVA              = @piva
      AND mdest.Sa_Cod            = @saCod
      AND mdest.Appezza           = @appezza
      AND mdest.Id_Destinazione   = @idReg
      AND mov.Data_Movimento      >= @dataAnalisi
      AND mov.Data_Movimento      <= @dataConsiglio
      AND ip.Validita_Fine        >= @dataAnalisi
      AND ip.Validita_Inizio      <= @dataConsiglio
)
-- Espansione per elemento: una riga per ogni macroelemento non nullo (N, P, K)
SELECT
    fb.Id_Agenda,
    fb.Id_Mov,
    fb.Id_Mov_Det,
    fb.Data_Movimento,
    el.Elemento,
    ISNULL(
        fb.Qta * el.Valore_Tecnico * fb.Efficienza / 100.0
        / NULLIF(fb.Sup_Tratt, 0),
        0
    )                            AS Quantitativo,
    ISNULL(fb.Fase_BBCH, '')     AS Fase_BBCH
FROM FertilizzazioniBase fb
CROSS APPLY (
    VALUES ('N', fb.N_Tecnico),
           ('P', fb.P_Tecnico),
           ('K', fb.K_Tecnico)
) AS el(Elemento, Valore_Tecnico)
WHERE el.Valore_Tecnico > 0
ORDER BY fb.Data_Movimento;";

            var parameters = new Dictionary<string, object>
            {
                ["@piva"]          = piva,
                ["@saCod"]         = saCod,
                ["@appezza"]       = appezza,
                ["@idReg"]         = idReg,
                ["@dataAnalisi"]   = dataAnalisi,
                ["@dataConsiglio"] = dataConsiglio
            };

            try
            {
                DataTable dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, parameters);
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
