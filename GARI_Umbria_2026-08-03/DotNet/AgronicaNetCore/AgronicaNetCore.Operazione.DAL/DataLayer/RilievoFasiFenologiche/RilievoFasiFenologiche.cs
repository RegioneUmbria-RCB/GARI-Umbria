using System.Data;
using System.Text;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Operazione.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.RilievoFasiFenologiche
{
    /// <summary>
    /// Implementazione del DAL per l'estrazione cronologica delle fasi fenologiche per appezzamento.
    /// Esegue la query descritta in DS20-BL — Query Movimenti Fenologici con deduplicazione e ordinamento.
    /// </summary>
    /// <remarks>
    /// Design Specification: DS20-BL GetPhenologicalPhasesChronological — Dettaglio Procedurale.
    /// Persistenze coinvolte (R): Movimenti_dettagli, Movimenti, Mov_Destinazioni, Mov_Dettaglio_Tecnico,
    /// Agenda, Reg_Impianti, SpecieVegetaliXStadiCrescita.
    /// </remarks>
    public sealed class RilievoFasiFenologiche : BaseDALOperazione, IRilievoFasiFenologiche
    {
        public RilievoFasiFenologiche(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer,
            bool securityBypass = false)
            : base(provider, localizer, securityBypass)
        {
        }

        /// <inheritdoc/>
        public async Task<DataTable> LeggiAsync(
            string piva,
            int saCod,
            int appezza,
            int idReg,
            DateTime dataInizio,
            DateTime dataFine,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(piva))
                throw new ArgumentException("La PIVA è obbligatoria.", nameof(piva));

            var sqlParams = new Dictionary<string, object>
            {
                ["@piva"]       = piva,
                ["@saCod"]      = saCod,
                ["@appezza"]    = appezza,
                ["@idReg"]      = idReg,
                ["@dataInizio"] = dataInizio,
                ["@dataFine"]   = dataFine,
                ["@lavCod"]     = LAV_COD.LAVCOD_FASI_FENOLOGICHE,
                ["@cauMov"]     = CAU_MOV.CAU_RILIEVO_CAMPO
            };

            var sql = new StringBuilder();

            // ── CTE: costruisce il set grezzo con ROW_NUMBER per la deduplica ──────────────
            // Design Specification: DS20-BL — Ordinamento e Deduplica.
            // A parità di (ff_classe, Validita_Inizio) viene mantenuta la riga con
            // Data_Creazione più recente (timestamp di inserimento record).
            sql.AppendLine(";WITH CTE_FasiFenologiche AS (");
            sql.AppendLine("    SELECT");
            sql.AppendLine("        CONCAT(scb.Stadio_Principale, scb.Seconda_Cifra, scb.Terza_Cifra) AS CodBbch,");
            sql.AppendLine("        ISNULL(svsc.Descrizione, '')         AS DescrizioneBbch,");
            sql.AppendLine("        md.Validita_Inizio,");
            sql.AppendLine("        md.Id_Agenda,");
            sql.AppendLine("        md.Id_Mov,");
            sql.AppendLine("        md.Id_Mov_Det");
            sql.AppendLine("    FROM Mov_Destinazioni md");

            // ── Join: Reg_Impianti — validazione appezzamento (DS20-BL § Filtraggio per appezzamento) ──
            sql.AppendLine("    INNER JOIN Reg_Impianti ri");
            sql.AppendLine("        ON  md.PIVA             = ri.PIVA");
            sql.AppendLine("        AND md.Sa_Cod           = ri.SA_COD");
            sql.AppendLine("        AND md.Appezza          = ri.APPEZZA");
            sql.AppendLine("        AND md.Id_Destinazione  = ri.ID_REG");

            // ── Join: Movimenti — testata movimento (data, causalità) ────────────────────
            sql.AppendLine("    INNER JOIN Movimenti mov");
            sql.AppendLine("        ON  md.PIVA         = mov.PIVA");
            sql.AppendLine("        AND md.Sa_Cod       = mov.Sa_Cod");
            sql.AppendLine("        AND md.Id_Agenda    = mov.Id_Agenda");
            sql.AppendLine("        AND md.Id_Mov       = mov.Id_Mov");

            // ── Join: Movimenti_dettagli — riga movimento (fornisce Data_Creazione per la deduplica) ──
            sql.AppendLine("    INNER JOIN Movimenti_dettagli mdet");
            sql.AppendLine("        ON  md.PIVA         = mdet.PIVA");
            sql.AppendLine("        AND md.Sa_Cod       = mdet.Sa_Cod");
            sql.AppendLine("        AND md.Id_Agenda    = mdet.Id_Agenda");
            sql.AppendLine("        AND md.Id_Mov       = mdet.Id_Mov");
            sql.AppendLine("        AND md.Id_Mov_Det   = mdet.Id_Mov_Det");

            // ── Join: Agenda — filtro tipo lavoro = 79 (Rilievo Fasi Fenologiche) ────────
            sql.AppendLine("    INNER JOIN Agenda ag");
            sql.AppendLine("        ON  mov.PIVA        = ag.PIVA");
            sql.AppendLine("        AND mov.Id_Agenda   = ag.Id_Agenda");

            // ── Join: Mov_Dettaglio_Tecnico — contiene ff_classe (codice BBCH) ──────────
            // INNER JOIN: le righe senza ff_classe non sono fasi fenologiche.
            sql.AppendLine("    INNER JOIN Mov_Dettaglio_Tecnico mdt");
            sql.AppendLine("        ON  md.PIVA         = mdt.PIVA");
            sql.AppendLine("        AND md.Sa_Cod       = mdt.Sa_Cod");
            sql.AppendLine("        AND md.Id_Agenda    = mdt.Id_Agenda");
            sql.AppendLine("        AND md.Id_Mov       = mdt.Id_Mov");
            sql.AppendLine("        AND md.Id_Mov_Det   = mdt.Id_Mov_Det");

            // ── Join: SpecieVegetaliXStadiCrescita — lookup descrizione BBCH ─────────────
            // Design Specification: DS20-BL — Descrizione BBCH; join su Cod_SS.
            sql.AppendLine("    LEFT JOIN SpecieVegetaliXStadiCrescita svsc");
            sql.AppendLine("        ON  mdt.ff_classe   = svsc.Cod_SS");

            // ── Join: Stadi_Crescita_BBCH — codice BBCH (concatenazione Stadio_Principale, Seconda_Cifra, Terza_Cifra) ──
            // Design Specification: DS20-BL — recuperare BbchCod tramite join su ID_BBCH.
            sql.AppendLine("    LEFT JOIN Stadi_Crescita_BBCH scb");
            sql.AppendLine("        ON  svsc.ID_BBCH   = scb.ID_BBCH");

            // ── Filtri ────────────────────────────────────────────────────────────────────
            // Design Specification: DS20-BL — Regole di Business.
            sql.AppendLine("    WHERE");
            sql.AppendLine("        ag.Lav_Cod              = @lavCod");      // 79 = LAVCOD_FASI_FENOLOGICHE
            sql.AppendLine("        AND mov.Cau_Mov         = @cauMov");      // '2100' = CAU_RILIEVO_CAMPO
            sql.AppendLine("        AND md.PIVA             = @piva");
            sql.AppendLine("        AND md.Sa_Cod           = @saCod");
            sql.AppendLine("        AND md.Appezza          = @appezza");
            sql.AppendLine("        AND md.Id_Destinazione  = @idReg");
            sql.AppendLine("        AND mov.Validita_Inizio  >= @dataInizio"); // 01/01/anno
            sql.AppendLine("        AND mov.Validita_Inizio  <= @dataFine");   // 31/12/anno
            sql.AppendLine("        AND mdt.ff_classe       IS NOT NULL");
            sql.AppendLine("        AND mdt.ff_classe       > 0");
            sql.AppendLine(")");

            // ── Proiezione finale: applica deduplica e ordina cronologicamente ────────────
            // Design Specification: DS20-BL — Composizione Output; Ordinamento e Deduplica.
            sql.AppendLine("SELECT");
            sql.AppendLine("    CodBbch,");
            sql.AppendLine("    DescrizioneBbch,");
            sql.AppendLine("    Validita_Inizio,");
            sql.AppendLine("    Id_Agenda,");
            sql.AppendLine("    Id_Mov,");
            sql.AppendLine("    Id_Mov_Det");
            sql.AppendLine("FROM CTE_FasiFenologiche");
            sql.AppendLine("ORDER BY Validita_Inizio ASC, CodBbch ASC;");

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(sql.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
