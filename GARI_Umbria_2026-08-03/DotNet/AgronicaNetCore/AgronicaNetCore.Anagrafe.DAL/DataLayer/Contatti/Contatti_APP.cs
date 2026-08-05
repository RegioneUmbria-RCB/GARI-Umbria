using System.Data;
using System.Text;
using AgronicaCoreDTOStd.InData.Agea;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Contatti
{
    public class Contatti_APP : BaseDALAnagrafe, IContatti_APP
    {
        private const string PivaPubblica = "99999999999";

        public Contatti_APP(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        public async Task<DataTable> ReadAsync(
            string? piva,
            bool includeContatti,
            bool includeFornitoriMeteo,
            bool includeFornitoriNormali,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();

            stbQuery
                .AppendLine("SELECT")
                .AppendLine(
                    " Contatti.Piva, Contatti.Sa_Cod, Contatti.Cod_Contatto, Rapporti_Contabili.Cod_Rapporto, Contatti.Rag_Soc, Contatti.Convenevoli, "
                )
                .AppendLine(
                    " Contatti.Nome, Contatti.Cognome, Contatti.Sesso, Contatti.Data_Nascita, "
                )
                .AppendLine(
                    " Risorse_Umane.Cod_RisUm, Risorse_Umane.Validita_Inizio, Risorse_Umane.Validita_Fine, "
                )
                .AppendLine(
                    " DatiPatentino.nrPatentino, DatiPatentino.Data_Rilascio_Patentino, DatiPatentino.Data_Scadenza_Patentino,  "
                )
                .AppendLine(
                    " COALESCE(substring(nrbadge, patindex('%[^0]%',Contatti.nrbadge), 10), '') AS NrBadge,   "
                )
                .AppendLine(" COALESCE(Rapporti_Contabili.Piva,'') AS RapportoContabile_Piva,")
                .AppendLine(
                    " COALESCE(Rapporti_Contabili.Cod_Rapporto,0) AS RapportoContabile_Cod_Rapporto,"
                )
                .AppendLine(
                    " COALESCE(Rapporti_Contabili.Dipendente,0) AS RapportoContabile_Dipendente,"
                )
                .AppendLine(" COALESCE(Rapporti_Contabili.Legale,0) AS RapportoContabile_Legale,")
                .AppendLine(
                    " COALESCE(Rapporti_Contabili.Terzista,0) AS RapportoContabile_Terzista"
                )
                .AppendLine("FROM")
                .AppendLine("    Contatti ")
                .AppendLine("    INNER JOIN Risorse_Umane ")
                .AppendLine("        ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto")
                .AppendLine("        AND Contatti.Piva = Risorse_Umane.Piva")
                .AppendLine("    INNER JOIN Rapporti_Contabili ")
                .AppendLine(
                    "        ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto"
                )
                .AppendLine(
                    "OUTER APPLY (SELECT top 1 ISNULL(Allegati_Documenti.Allegati_Documenti_Numero,'N/D') as nrPatentino, Allegati_Documenti.Validazione_Data as Data_Rilascio_Patentino,  Alert_Elenco.Data_Scadenza as Data_Scadenza_Patentino From alert_entita "
                )
                .AppendLine(
                    " INNER JOIN Alert_Elenco ON Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser And Alert_Elenco.ID_Alert_Entita = Alert_Entita.ID_Alert_Entita "
                )
                .AppendLine(
                    " INNER JOIN Allegati_Documenti ON Allegati_Documenti.Allegati_Documenti_SuperUser = Alert_Entita.PivaSuperUser and Allegati_Documenti.Allegati_Documenti_Piva = Alert_Entita.piva  and "
                )
                .AppendLine(
                    " Allegati_Documenti.Allegati_Documenti_Cod = Alert_Entita.Allegati_Documenti_Cod "
                )
                .AppendLine(
                    " WHERE tipoentita_cod = 8 And Allegati_Documenti.Allegati_documenti_CatCod = 2 "
                )
                .AppendLine(
                    " And REPLACE(LTRIM(RTRIM(alert_entita.Cod_Contatto)), char(9), '') = REPLACE(LTRIM(RTRIM(Contatti.Cod_Contatto)), char(9), '') and alert_entita.piva = Contatti.piva "
                )
                .AppendLine(" order by Alert_Elenco.Data_Scadenza desc) AS DatiPatentino")
                .AppendLine("WHERE 1 = 1");

            var codRapporti = new List<int>();

            if (includeContatti)
            {
                codRapporti.AddRange(
                    new[]
                    {
                        (int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Legale_Rappresentante,
                        (int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Dipendente,
                        (int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Tecnico,
                        (int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Terzista,
                    }
                );
            }

            if (includeFornitoriMeteo || includeFornitoriNormali)
            {
                codRapporti.Add((int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Fornitore);
            }

            if (includeFornitoriNormali)
            {
                codRapporti.AddRange(
                    new[]
                    {
                        (int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Vivaio,
                        (int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Fornitore_Agrofarmaci,
                        (int)TipiEnumerativi.Enum_Rapporti_Contabili_Standard.Conferente,
                    }
                );
            }

            if (codRapporti.Count > 0)
            {
                stbQuery.AppendLine("    AND (Rapporti_Contabili.Cod_Rapporto IN (@codRapporti)");
                if (includeContatti)
                {
                    stbQuery.AppendLine("        OR Rapporti_Contabili.Dipendente = 1");
                    stbQuery.AppendLine("        OR Rapporti_Contabili.Legale = 1");
                    stbQuery.AppendLine("        OR Rapporti_Contabili.Terzista = 1");
                }
                stbQuery.AppendLine("    )");

                parSqlIn.Add("@codRapporti", FormatClauseIn(codRapporti.ToList()));
            }

            if (!string.IsNullOrWhiteSpace(piva))
            {
                stbQuery.AppendLine("    AND (Contatti.Piva = @piva");
                stbQuery.AppendLine("    OR Contatti.Sa_Cod = -1)");
                parSql.Add("@piva", piva);
            }

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> ContattiFornitoriMovimentati_APPAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery
                .AppendLine(";WITH cte_FornitoriMovimentati AS (")
                .AppendLine("    SELECT m.Cod_RisUm")
                .AppendLine("    FROM Movimenti m")
                .AppendLine("    INNER JOIN Agenda a ON a.PIVA = m.PIVA AND a.Id_Agenda = m.Id_Agenda")
                .AppendLine($"    WHERE a.Lav_Cod IN ({LAV_COD.LAVCOD_BOLLA_RICEVUTA}, {LAV_COD.LAVCOD_BOLLA_EMESSA}, {LAV_COD.LAVCOD_FATTURA_RICEVUTA}, {LAV_COD.LAVCOD_FATTURA_EMESSA})")
                .AppendLine($"    AND Cau_Mov = '{CAU_MOV.CAU_REGISTRAZIONI}'")
                .AppendLine("    AND a.Piva = @piva")
                .AppendLine(")")
                .AppendLine("SELECT DISTINCT")
                .AppendLine(
                    " Contatti.Piva, Contatti.Sa_Cod, Contatti.Cod_Contatto, Rapporti_Contabili.Cod_Rapporto, Contatti.Rag_Soc, Contatti.Convenevoli,"
                )
                .AppendLine(
                    " Contatti.Nome, Contatti.Cognome, Contatti.Sesso, Contatti.Data_Nascita,"
                )
                .AppendLine(
                    " Risorse_Umane.Cod_RisUm, Risorse_Umane.Validita_Inizio, Risorse_Umane.Validita_Fine,"
                )
                .AppendLine(
                    " COALESCE(substring(nrbadge, patindex('%[^0]%',Contatti.nrbadge), 10), '') AS NrBadge,"
                )
                .AppendLine(" COALESCE(Rapporti_Contabili.Piva,'') AS RapportoContabile_Piva,")
                .AppendLine(
                    " COALESCE(Rapporti_Contabili.Cod_Rapporto,0) AS RapportoContabile_Cod_Rapporto,"
                )
                .AppendLine(
                    " COALESCE(Rapporti_Contabili.Dipendente,0) AS RapportoContabile_Dipendente,"
                )
                .AppendLine(" COALESCE(Rapporti_Contabili.Legale,0) AS RapportoContabile_Legale,")
                .AppendLine(
                    " COALESCE(Rapporti_Contabili.Terzista,0) AS RapportoContabile_Terzista"
                )
                .AppendLine("FROM")
                .AppendLine("    Contatti ")
                .AppendLine("    INNER JOIN Risorse_Umane ")
                .AppendLine("        ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto")
                .AppendLine("        AND Contatti.Piva = Risorse_Umane.Piva")
                .AppendLine("    INNER JOIN Rapporti_Contabili ")
                .AppendLine(
                    "        ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto"
                )
                .AppendLine("    INNER JOIN cte_FornitoriMovimentati cte ON cte.Cod_RisUm = Risorse_Umane.Cod_RisUm")
                .AppendLine("WHERE Rapporti_Contabili.Fornitore = 1");

            parSql.Add("@piva", piva);

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

        public async Task<DataTable> ContattiLavoratoriMovimentati_APPAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery
                .AppendLine(";WITH cte_ContattiMovimentati AS (")
                .AppendLine("    SELECT Mat_Cod AS Cod_Risum")
                .AppendLine("    FROM Agenda A")
                .AppendLine("    INNER JOIN Movimenti m ON m.PIVA = A.PIVA AND m.Id_Agenda = A.Id_Agenda")
                .AppendLine("    INNER JOIN Movimenti_dettagli md ON md.PIVA = A.PIVA AND md.Id_Agenda = A.Id_Agenda AND md.Id_Mov = m.Id_Mov")
                .AppendLine($"    WHERE ELEM_COD = {ELEM_COD.ELEMCOD_MANODOPERA}")
                .AppendLine($"    AND Cau_Mov = '{CAU_MOV.CAU_IMPUTAZIONE_MANODOPERA}'")
                .AppendLine("    AND A.PIVA = @piva")
                .AppendLine(" UNION ALL")
                .AppendLine("    SELECT Mat_Cod AS Cod_Risum")
                .AppendLine("    FROM Ricette r")
                .AppendLine("    INNER JOIN Ricette_Operazioni ro ON ro.Ricetta_Cod = r.Ricetta_Cod")
                .AppendLine("    INNER JOIN Ricette_Dettagli rd ON rd.Ricetta_Cod = ro.Ricetta_Cod AND rd.Ricetta_Operazione_Cod = ro.Ricetta_Operazione_Cod")
                .AppendLine($"    WHERE ELEM_COD = {ELEM_COD.ELEMCOD_MANODOPERA}")
                .AppendLine($"    AND Cau_Mov = '{CAU_MOV.CAU_IMPUTAZIONE_MANODOPERA}'")
                .AppendLine("    AND r.PIVA = @piva")
                .AppendLine(")")
                .AppendLine("SELECT DISTINCT")
                .AppendLine(
                    " Contatti.Piva, Contatti.Sa_Cod, Contatti.Cod_Contatto, Rapporti_Contabili.Cod_Rapporto, Contatti.Rag_Soc, Contatti.Convenevoli,"
                )
                .AppendLine(
                    " Contatti.Nome, Contatti.Cognome, Contatti.Sesso, Contatti.Data_Nascita,"
                )
                .AppendLine(
                    " Risorse_Umane.Cod_RisUm, Risorse_Umane.Validita_Inizio, Risorse_Umane.Validita_Fine,"
                )
                .AppendLine(
                    " COALESCE(substring(nrbadge, patindex('%[^0]%',Contatti.nrbadge), 10), '') AS NrBadge,"
                )
                .AppendLine(" COALESCE(Rapporti_Contabili.Piva,'') AS RapportoContabile_Piva,")
                .AppendLine(
                    " COALESCE(Rapporti_Contabili.Cod_Rapporto,0) AS RapportoContabile_Cod_Rapporto,"
                )
                .AppendLine(
                    " COALESCE(Rapporti_Contabili.Dipendente,0) AS RapportoContabile_Dipendente,"
                )
                .AppendLine(" COALESCE(Rapporti_Contabili.Legale,0) AS RapportoContabile_Legale,")
                .AppendLine(
                    " COALESCE(Rapporti_Contabili.Terzista,0) AS RapportoContabile_Terzista"
                )
                .AppendLine("FROM")
                .AppendLine("    Contatti ")
                .AppendLine("    INNER JOIN Risorse_Umane ")
                .AppendLine("        ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto")
                .AppendLine("        AND Contatti.Piva = Risorse_Umane.Piva")
                .AppendLine("    INNER JOIN Rapporti_Contabili ")
                .AppendLine("        ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto")
                .AppendLine("    INNER JOIN cte_ContattiMovimentati cte ON cte.Cod_Risum = Risorse_Umane.Cod_RisUm");

            parSql.Add("@piva", piva);

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
