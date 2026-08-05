using AgronicaCoreVisibilitaStd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgronicaCoreVisibilitaStd
{
    /// <summary>
    /// Costruisce le query SQL di calcolo visibilit&#224; gerarchia Imprese / Centri Aziendali,
    /// replicando la logica di <c>Filtrone.CreaStringaQueryPerDTFiltrone</c>
    /// (AgronicaCoreUtility\Filtrone.vb). La variante &#232; controllata da <see cref="ModalitaQuery"/>.
    ///
    /// La query usa i parametri:
    ///   @pivaSuperUser  (UtentiXImprese.[User])
    ///   @finestraInizio (DateTime - Imprese.Validita_Fine &gt;= ...)
    ///   @finestraFine   (DateTime - Imprese.Validita_Inizio &lt;= ...)
    /// Il frammento Descrizione_2 &#232; concatenato come SQL raw (stessa logica legacy).
    /// </summary>
    public static class VisibilitaQueryBuilder
    {
        private const int EnumCodiciAnagrafeCodiceSocio = 1033;
        private const int EnumCodiciAnagrafeCodiceCuaa = 1010;

        /// <summary>
        /// Query su Imprese. <paramref name="modalita"/>:
        /// <see cref="ModalitaQuery.Full"/> = case <c>enum_TipoSelect_FiltroneSuperNova.Imprese</c> (2);
        /// <see cref="ModalitaQuery.UvaLean"/> = case <c>Imprese_Visibilita_Appoggio</c>.
        /// </summary>
        /// <param name="descrizione2">
        /// Contenuto di utenti_profili.Descrizione_2. Pu&#242; essere null/vuoto.
        /// Viene normalizzato via <see cref="DescrizioneFiltroHelper"/>.
        /// </param>
        /// <param name="modalita">Variante Full o UvaLean.</param>
        public static string BuildQueryImprese(string descrizione2, ModalitaQuery modalita)
        {
            return modalita == ModalitaQuery.Full
                ? BuildQueryImpreseFull(descrizione2)
                : BuildQueryImpreseUvaLean(descrizione2);
        }

        /// <summary>
        /// Query su Centri Aziendali. <paramref name="modalita"/>:
        /// <see cref="ModalitaQuery.Full"/> = case <c>CentriAziendali</c>;
        /// <see cref="ModalitaQuery.UvaLean"/> = case <c>CentriAziendali_Visibilita_Appoggio</c>.
        /// </summary>
        public static string BuildQueryCentri(string descrizione2, ModalitaQuery modalita)
        {
            return modalita == ModalitaQuery.Full
                ? BuildQueryCentriFull(descrizione2)
                : BuildQueryCentriUvaLean(descrizione2);
        }

        // ======================================================================
        // IMPRESE - Full (Filtrone case Imprese)
        // ======================================================================
        private static string BuildQueryImpreseFull(string descrizione2)
        {
            var stb = new StringBuilder();

            stb.AppendLine(" SELECT DISTINCT dbo.Imprese.PIVA as chiave, dbo.Imprese.PIVA, dbo.Imprese.rag_soc, Lista_Province.PROVINCIA, Indirizzi.CAP, Istat.Localita, Imprese.TipoImpresaGerarchia, Imprese.Validita_Inizio, Imprese.Validita_Fine, IC.val_cod AS CodiceSocio, ISNULL(IC_Cuaa.val_cod, '') AS CodiceCuaa ");

            var strJoin = new StringBuilder();
            int nParentesi = 0;

            strJoin.AppendLine(" LEFT JOIN GerarchiaImprese  (NOLOCK)  ON Imprese.Piva = GerarchiaImprese.Figlio) ");
            nParentesi++;
            strJoin.AppendLine(" INNER join ImpresexIndirizzi  (NOLOCK)  ON Imprese.PIVA = ImpresexIndirizzi.PIVA) ");
            nParentesi++;
            strJoin.AppendLine(" INNER JOIN Indirizzi  (NOLOCK)  ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo) ");
            nParentesi++;
            strJoin.AppendLine(" INNER JOIN ISTAT  (NOLOCK)  ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM) ");
            nParentesi++;
            strJoin.AppendLine(" INNER JOIN Lista_Province  (NOLOCK)  ON Lista_Province.Sigla = ISTAT.COMUNI_PROV) ");
            nParentesi++;
            strJoin.AppendLine(" LEFT OUTER JOIN Centri_Aziendali  (NOLOCK)  ON Imprese.Piva=Centri_Aziendali.Piva) ");
            nParentesi++;

            // Imprese_Codici (socio + cuaa) - nessuna parentesi aperta
            strJoin.AppendLine(" LEFT OUTER JOIN Imprese_Codici IC  (NOLOCK)  on (IC.Piva = Imprese.Piva) AND IC.id_cod = " + EnumCodiciAnagrafeCodiceSocio + " ");
            strJoin.AppendLine(" LEFT OUTER JOIN Imprese_Codici IC_cuaa  (NOLOCK)  on (IC_cuaa.Piva = Imprese.Piva) AND IC_cuaa.id_cod = " + EnumCodiciAnagrafeCodiceCuaa + " ");

            stb.Append(" FROM ( ").Append(new string('(', nParentesi)).AppendLine();
            stb.AppendLine(" Imprese  (NOLOCK)  INNER JOIN UtentiXImprese  (NOLOCK)  On Imprese.Piva = UtentixImprese.Piva) ");
            stb.AppendLine(strJoin.ToString());

            stb.AppendLine(" Where 1 = 1 ");
            stb.AppendLine(" AND (ImpresexIndirizzi.Tipo_Indirizzo = 1)");
            AppendWhereBase(stb, descrizione2);

            return stb.ToString();
        }

        // ======================================================================
        // IMPRESE - UvaLean (Filtrone case Imprese_Visibilita_Appoggio)
        // Differenze vs Full:
        //   - SELECT solo: chiave, PIVA, rag_soc (Filtrone.vb:102)
        //   - bImpreseXIndirizzi forzato False (Filtrone.vb:261-263) -> niente Indirizzi/ISTAT/Lista_Province/ImpreseCodici
        //   - Niente filtro "Tipo_Indirizzo = 1" (gated su bImpreseXIndirizzi, Filtrone.vb:583-585)
        // ======================================================================
        private static string BuildQueryImpreseUvaLean(string descrizione2)
        {
            var stb = new StringBuilder();

            stb.AppendLine(" SELECT DISTINCT dbo.Imprese.PIVA as chiave, dbo.Imprese.PIVA, dbo.Imprese.rag_soc ");

            var strJoin = new StringBuilder();
            int nParentesi = 0;

            strJoin.AppendLine(" LEFT JOIN GerarchiaImprese  (NOLOCK)  ON Imprese.Piva = GerarchiaImprese.Figlio) ");
            nParentesi++;
            strJoin.AppendLine(" LEFT OUTER JOIN Centri_Aziendali  (NOLOCK)  ON Imprese.Piva=Centri_Aziendali.Piva) ");
            nParentesi++;

            stb.Append(" FROM ( ").Append(new string('(', nParentesi)).AppendLine();
            stb.AppendLine(" Imprese  (NOLOCK)  INNER JOIN UtentiXImprese  (NOLOCK)  On Imprese.Piva = UtentixImprese.Piva) ");
            stb.AppendLine(strJoin.ToString());

            stb.AppendLine(" Where 1 = 1 ");
            AppendWhereBase(stb, descrizione2);

            return stb.ToString();
        }

        // ======================================================================
        // CENTRI - Full (Filtrone case CentriAziendali)
        // ======================================================================
        private static string BuildQueryCentriFull(string descrizione2)
        {
            var stb = new StringBuilder();

            stb.AppendLine(" SELECT DISTINCT (Imprese.PIVA + '_' + cast(Centri_Aziendali.sa_cod as varchar(20))) as chiave, Imprese.PIVA, Imprese.rag_soc, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Centri_Aziendali.Validita_Inizio, Centri_Aziendali.Validita_Fine ");

            var strJoin = new StringBuilder();
            int nParentesi = 0;

            strJoin.AppendLine(" LEFT JOIN GerarchiaImprese  (NOLOCK)  ON Imprese.Piva = GerarchiaImprese.Figlio) ");
            nParentesi++;
            strJoin.AppendLine(" INNER join ImpresexIndirizzi  (NOLOCK)  ON Imprese.PIVA = ImpresexIndirizzi.PIVA) ");
            nParentesi++;
            strJoin.AppendLine(" INNER JOIN Indirizzi  (NOLOCK)  ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo) ");
            nParentesi++;
            strJoin.AppendLine(" INNER JOIN ISTAT  (NOLOCK)  ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM) ");
            nParentesi++;
            strJoin.AppendLine(" INNER JOIN Lista_Province  (NOLOCK)  ON Lista_Province.Sigla = ISTAT.COMUNI_PROV) ");
            nParentesi++;
            strJoin.AppendLine(" LEFT OUTER JOIN Centri_Aziendali  (NOLOCK)  ON Imprese.Piva=Centri_Aziendali.Piva) ");
            nParentesi++;

            stb.Append(" FROM ( ").Append(new string('(', nParentesi)).AppendLine();
            stb.AppendLine(" Imprese  (NOLOCK)  INNER JOIN UtentiXImprese  (NOLOCK)  On Imprese.Piva = UtentixImprese.Piva) ");
            stb.AppendLine(strJoin.ToString());

            stb.AppendLine(" Where 1 = 1 ");
            stb.AppendLine(" AND (ImpresexIndirizzi.Tipo_Indirizzo = 1)");
            AppendWhereBase(stb, descrizione2);

            return stb.ToString();
        }

        // ======================================================================
        // CENTRI - UvaLean (Filtrone case CentriAziendali_Visibilita_Appoggio)
        // Differenze vs Full:
        //   - SELECT IDENTICA (Filtrone.vb:136-137)
        //   - bListaProvince = False (Filtrone.vb:143)
        //   - bImpreseXIndirizzi forzato False (Filtrone.vb:261-263) -> niente Indirizzi/ISTAT/Lista_Province
        //   - Niente filtro "Tipo_Indirizzo = 1"
        // ======================================================================
        private static string BuildQueryCentriUvaLean(string descrizione2)
        {
            var stb = new StringBuilder();

            stb.AppendLine(" SELECT DISTINCT (Imprese.PIVA + '_' + cast(Centri_Aziendali.sa_cod as varchar(20))) as chiave, Imprese.PIVA, Imprese.rag_soc, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Centri_Aziendali.Validita_Inizio, Centri_Aziendali.Validita_Fine ");

            var strJoin = new StringBuilder();
            int nParentesi = 0;

            strJoin.AppendLine(" LEFT JOIN GerarchiaImprese  (NOLOCK)  ON Imprese.Piva = GerarchiaImprese.Figlio) ");
            nParentesi++;
            strJoin.AppendLine(" LEFT OUTER JOIN Centri_Aziendali  (NOLOCK)  ON Imprese.Piva=Centri_Aziendali.Piva) ");
            nParentesi++;

            stb.Append(" FROM ( ").Append(new string('(', nParentesi)).AppendLine();
            stb.AppendLine(" Imprese  (NOLOCK)  INNER JOIN UtentiXImprese  (NOLOCK)  On Imprese.Piva = UtentixImprese.Piva) ");
            stb.AppendLine(strJoin.ToString());

            stb.AppendLine(" Where 1 = 1 ");
            AppendWhereBase(stb, descrizione2);

            return stb.ToString();
        }

        private static void AppendWhereBase(StringBuilder stb, string descrizione2)
        {
            stb.AppendLine(" And   UtentixImprese.[User] = @pivaSuperUser ");
            stb.AppendLine(" AND   Imprese.Validita_inizio <= @finestraFine ");
            stb.AppendLine(" AND   Imprese.Validita_Fine >= @finestraInizio ");

            var filtro = DescrizioneFiltroHelper.Normalizza(descrizione2);
            if (filtro.Length > 0)
                stb.AppendLine(filtro);
        }

        public static BuildPraticheResult BuildQueryPratiche(IEnumerable<PraticaProfilo> pratiche)
        {
            if (pratiche == null)
                return null;

            var praticheList = pratiche
                .Where(p => p != null)
                .GroupBy(p => new { p.Servizio_Cod, p.ConsideraValiditaTemporale })
                .Select(g => g.First())
                .ToList();

            if (praticheList.Count == 0)
                return null;

            var noTime = praticheList.Where(p => !p.ConsideraValiditaTemporale).Select(p => p.Servizio_Cod).ToList();
            var withTime = praticheList.Where(p => p.ConsideraValiditaTemporale).Select(p => p.Servizio_Cod).ToList();
            var parameters = new Dictionary<string, object>();
            var conditions = new List<string>();

            if (noTime.Count > 0)
                conditions.Add("    Servizio_Cod IN (" + BuildInClause("@servCodNoTime_", noTime, parameters) + ")");

            if (withTime.Count > 0)
            {
                parameters["@oggi"] = DateTime.Today;
                conditions.Add("    (Servizio_Cod IN (" + BuildInClause("@servCodWithTime_", withTime, parameters) + ")" + Environment.NewLine +
                               "     AND Validita_Inizio <= @oggi AND Validita_Fine >= @oggi)");
            }

            var stb = new StringBuilder();
            stb.AppendLine(" SELECT DISTINCT Piva, Sa_Cod ");
            stb.AppendLine(" FROM Pratiche (NOLOCK) ");
            stb.AppendLine(" WHERE Piva_SuperUser = @pivaSuperUser ");
            stb.AppendLine("   AND (");
            stb.AppendLine(string.Join(Environment.NewLine + "    OR" + Environment.NewLine, conditions.ToArray()));
            stb.AppendLine("   )");

            return new BuildPraticheResult(stb.ToString(), parameters);
        }

        private static string BuildInClause(string prefix, IList<int> values, IDictionary<string, object> parameters)
        {
            var names = new List<string>();
            for (int i = 0; i < values.Count; i++)
            {
                var name = prefix + i;
                names.Add(name);
                parameters[name] = values[i];
            }

            return string.Join(", ", names.ToArray());
        }
    }
}
