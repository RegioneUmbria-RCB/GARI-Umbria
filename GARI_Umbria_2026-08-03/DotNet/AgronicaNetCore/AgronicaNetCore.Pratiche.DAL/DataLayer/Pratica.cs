using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Models;
using System.Collections.Concurrent;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace AgronicaNetCore.Pratiche.DAL.DataLayer
{
    public class Pratica : DAL_Base, IPratica
    {
        public Pratica(IServiceProvider provider, bool securityBypass = false) : base(provider, securityBypass)
        {
        }

        public async Task<bool> IsServizioBluArancioAsync(int codiceServizio,
            DateTime dataInizio, DateTime dataFine, AgronicaCoreParametri objP)
        {

            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@codiceServizio", codiceServizio);
            sqlParams.TryAdd("@dataValiditaInizio", dataInizio);
            sqlParams.TryAdd("@dataValiditaFine", dataFine);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT TOP(1) 1 ")
                .AppendLine("FROM Pratiche WITH (NOLOCK)")
                .AppendLine("INNER JOIN Servizi WITH (NOLOCK)")
                .AppendLine("ON Pratiche.Servizio_Cod = Servizi.Servizio_Cod")
                .AppendLine("INNER JOIN Imprese WITH (NOLOCK)")
                .AppendLine("ON Pratiche.Piva = Imprese.PIVA")
                .AppendLine("WHERE Pratiche.Servizio_Cod = @codiceServizio")
                .AppendLine("AND Pratiche.Validita_inizio <= @dataValiditaFine")
                .AppendLine("AND Pratiche.Validita_Fine >= @dataValiditaInizio");

            try
            {
                DataTable dt = await GetDataProvider(objP).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
                bool isServizioBluArancio = (dt != null) && (dt.Rows.Count > 0);
                return isServizioBluArancio;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        public async Task<DataTable> ReadPraticheSottoscrizioneQdCAsync(int codiceServizio, int codiceStato,
            DateTime dataInizioPratiche, DateTime dataFinePreatiche, AgronicaCoreParametri objP)
        {
            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@codiceServizio", codiceServizio);
            sqlParams.TryAdd("@codiceStato", codiceStato);
            sqlParams.TryAdd("@dataValiditaInizio", dataInizioPratiche);
            sqlParams.TryAdd("@dataValiditaFine", dataFinePreatiche);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT Imprese.rag_soc,")
                .AppendLine("Servizi.Servizio_Des,")
                .AppendLine("ISNULL(Pratiche_Stati.Stato_Cod, 0) AS Stato_Cod,")
                .AppendLine("ISNULL(st.WAnagraficaStati_Des, '') AS Stato_Des,")
                .AppendLine("st.Ordine,")
                .AppendLine("Pratiche_Stati.Validita_Inizio AS Validita_Inizio_Stato,")
                .AppendLine("Pratiche_Stati.Validita_Fine AS Validita_Fine_Stato,")
                .AppendLine("Pratiche_Stati.stato_Origine_cod,")
                .AppendLine("Pratiche_Stati.PassaggioDiStato_cod,")
                .AppendLine("Pratiche_Stati.note,")
                .AppendLine("st.colore,")
                .AppendLine("st.WWorkFlow_cod,")
                .AppendLine("Pratiche.*,")
                .AppendLine("REPLACE( REPLACE( statoAttuale.stati, '&gt;', '>'), '&lt;', '<') as StatoAttuale_DES,")
                .AppendLine("statoAttuale.StatoFinale_Cod,")
                .AppendLine("statoAttuale.StatoFinale_Des")
                .AppendLine("FROM Pratiche WITH (NOLOCK)")
                .AppendLine("INNER JOIN (")
                .AppendLine("   SELECT p.pratica_cod,")
                .AppendLine("       (")
                .AppendLine("           SELECT TOP 1 '<font color=' + s.colore + '>' + s.WAnagraficaStati_Des + '</font><br /> (' + replace(convert(varchar(100), at.Validita_Inizio, 105), '-', '/') + ') <br />' as [text()]")
                .AppendLine("           FROM Pratiche_Stati_Attuali at WITH (NOLOCK)")
                .AppendLine("           INNER JOIN WAnagraficaStati s WITH (NOLOCK)")
                .AppendLine("           ON at.Stato_Cod = s.WAnagraficaStati_Cod")
                .AppendLine("           WHERE at.Pratica_Cod = p.Pratica_Cod")
                .AppendLine("           ORDER BY at.Validita_Inizio DESC")
                .AppendLine("           FOR XML PATH('')")
                .AppendLine("       ) as stati,")
                .AppendLine("       (")
                .AppendLine("           SELECT TOP 1 s.WAnagraficaStati_Des")
                .AppendLine("           FROM Pratiche_Stati_Attuali at WITH (NOLOCK)")
                .AppendLine("           INNER JOIN WAnagraficaStati s WITH (NOLOCK)")
                .AppendLine("           ON at.Stato_Cod = s.WAnagraficaStati_Cod")
                .AppendLine("           WHERE at.Pratica_Cod = p.Pratica_Cod")
                .AppendLine("           ORDER BY at.Validita_Inizio DESC")
                .AppendLine("       ) as StatoFinale_Des,")
                .AppendLine("       (")
                .AppendLine("           SELECT TOP 1 s.WAnagraficaStati_Cod")
                .AppendLine("           FROM Pratiche_Stati_Attuali at WITH (NOLOCK)")
                .AppendLine("           INNER JOIN WAnagraficaStati s WITH (NOLOCK)")
                .AppendLine("           ON at.Stato_Cod = s.WAnagraficaStati_Cod")
                .AppendLine("           WHERE at.Pratica_Cod = p.Pratica_Cod")
                .AppendLine("           ORDER BY at.Validita_Inizio DESC  ")
                .AppendLine("       ) as StatoFinale_Cod")
                .AppendLine("   FROM pratiche p WITH (NOLOCK)")
                .AppendLine("   GROUP BY p.Pratica_Cod")
                .AppendLine(") AS statoAttuale")
                .AppendLine("ON statoAttuale.Pratica_Cod = Pratiche.Pratica_Cod")
                .AppendLine("INNER JOIN Pratiche_Stati WITH(NOLOCK)")
                .AppendLine("ON Pratiche_Stati.Piva_SuperUser = Pratiche.Piva_SuperUser")
                .AppendLine("AND Pratiche_Stati.Pratica_Cod = Pratiche.Pratica_Cod")
                .AppendLine("INNER JOIN Imprese WITH(NOLOCK)")
                .AppendLine("ON Pratiche.Piva = Imprese.PIVA")
                .AppendLine("INNER JOIN Servizi WITH(NOLOCK)")
                .AppendLine("ON Pratiche.Servizio_Cod = Servizi.Servizio_Cod")
                .AppendLine("INNER JOIN WAnagraficaStati st WITH(NOLOCK)")
                .AppendLine("ON st.WAnagraficaStati_Cod = Pratiche_Stati.Stato_Cod")
                .AppendLine("WHERE Pratiche.Servizio_Cod = @codiceServizio")
                .AppendLine("AND Pratiche_Stati.Stato_Cod = @codiceStato")
                .AppendLine("AND Pratiche.Validita_inizio <= @dataValiditaFine")
                .AppendLine("AND Pratiche.Validita_Fine >= @dataValiditaInizio");

            try
            {
                return await GetDataProvider(objP).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        public async Task<DataTable> ReadStatoAttualePraticheAsync(int codiceServizio,
            DateTime dataInizioPratiche, DateTime dataFinePreatiche, AgronicaCoreParametri objP)
        {
            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@codiceServizio", codiceServizio);
            sqlParams.TryAdd("@dataValiditaInizio", dataInizioPratiche);
            sqlParams.TryAdd("@dataValiditaFine", dataFinePreatiche);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT Imprese.rag_soc,")
                .AppendLine("Servizi.Servizio_Des,")
                .AppendLine("ISNULL(Pratiche_Stati.Stato_Cod, 0) AS Stato_Cod,")
                .AppendLine("ISNULL(st.WAnagraficaStati_Des, '') AS Stato_Des,")
                .AppendLine("st.Ordine,")
                .AppendLine("Pratiche_Stati.Validita_Inizio AS Validita_Inizio_Stato,")
                .AppendLine("Pratiche_Stati.Validita_Fine AS Validita_Fine_Stato,")
                .AppendLine("st.colore,")
                .AppendLine("Pratiche.*")
                .AppendLine("FROM Pratiche WITH (NOLOCK)")
                .AppendLine("INNER JOIN Pratiche_Stati WITH(NOLOCK)")
                .AppendLine("ON Pratiche_Stati.Piva_SuperUser = Pratiche.Piva_SuperUser")
                .AppendLine("AND Pratiche_Stati.Pratica_Cod = Pratiche.Pratica_Cod")
                .AppendLine("INNER JOIN Imprese WITH(NOLOCK)")
                .AppendLine("ON Pratiche.Piva = Imprese.PIVA")
                .AppendLine("INNER JOIN Servizi WITH(NOLOCK)")
                .AppendLine("ON Pratiche.Servizio_Cod = Servizi.Servizio_Cod")
                .AppendLine("INNER JOIN WAnagraficaStati st WITH(NOLOCK)")
                .AppendLine("ON st.WAnagraficaStati_Cod = Pratiche_Stati.Stato_Cod")
                .AppendLine("WHERE Pratiche.Servizio_Cod = @codiceServizio")
                .AppendLine("AND Pratiche.Validita_inizio <= @dataValiditaFine")
                .AppendLine("AND Pratiche.Validita_Fine >= @dataValiditaInizio");

            try
            {
                return await GetDataProvider(objP).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
        }

        public async Task<DataTable> ReadPraticheStatisticheAsync(int codiceServizio, int codiceStato,
            DateTime dataInizioPratiche, DateTime dataFinePreatiche, AgronicaCoreParametri objP)
        {
            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@codiceServizio", codiceServizio);
            sqlParams.TryAdd("@codiceStato", codiceStato);
            sqlParams.TryAdd("@dataValiditaInizio", dataInizioPratiche);
            sqlParams.TryAdd("@dataValiditaFine", dataFinePreatiche);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT DISTINCT Pratiche.Piva,")
                .AppendLine("   sottosrizione.DataInizio,")
                .AppendLine("   statoAttuale.Stato_Des")
                .AppendLine("FROM Pratiche")
                .AppendLine("LEFT JOIN(")
                .AppendLine("   SELECT Pratiche.Piva, MAX(Pratiche_Stati.Validita_Inizio) AS DataInizio ")
                .AppendLine("       FROM Pratiche")
                .AppendLine("       INNER JOIN Pratiche_Stati")
                .AppendLine("       ON Pratiche_Stati.Piva_SuperUser = Pratiche.Piva_SuperUser")
                .AppendLine("       AND  Pratiche_Stati.Pratica_Cod = Pratiche.Pratica_Cod")
                .AppendLine("       INNER JOIN Imprese")
                .AppendLine("       ON Pratiche.Piva = Imprese.PIVA")
                .AppendLine("       INNER JOIN Servizi")
                .AppendLine("       ON Pratiche.Servizio_Cod = Servizi.Servizio_Cod")
                .AppendLine("       INNER JOIN WAnagraficaStati st")
                .AppendLine("       ON Pratiche.Servizio_Cod = Servizi.Servizio_Cod")
                .AppendLine("       WHERE Pratiche.Servizio_Cod = @codiceServizio")
                .AppendLine("       AND Pratiche_Stati.Stato_Cod = @codiceStato")
                .AppendLine("       AND Pratiche.Validita_inizio <= @dataValiditaFine")
                .AppendLine("       AND Pratiche.Validita_Fine >= @dataValiditaInizio")
                .AppendLine("       GROUP BY Pratiche.Piva")
                .AppendLine(") sottosrizione ")
                .AppendLine("ON Pratiche.Piva = sottosrizione.Piva")
                .AppendLine("LEFT JOIN(")
                .AppendLine("   SELECT *")
                .AppendLine("   FROM(")
                .AppendLine("       SELECT Pratiche.Piva,")
                .AppendLine("           CASE WHEN Pratiche_Stati.Validita_Inizio = MAX(Pratiche_Stati.Validita_Inizio)")
                .AppendLine("               OVER(PARTITION BY Pratiche.Piva) THEN")
                .AppendLine("                   ISNULL(st.WAnagraficaStati_Des, '')")
                .AppendLine("           ELSE")
                .AppendLine("               NULL")
                .AppendLine("           END AS Stato_Des")
                .AppendLine("       FROM Pratiche")
                .AppendLine("       INNER JOIN pratiche_stati_attuali Pratiche_Stati")
                .AppendLine("       ON Pratiche_Stati.Piva_SuperUser = Pratiche.Piva_SuperUser")
                .AppendLine("       AND Pratiche_Stati.Pratica_Cod = Pratiche.Pratica_Cod")
                .AppendLine("       INNER JOIN Imprese")
                .AppendLine("       ON Pratiche.Piva = Imprese.PIVA")
                .AppendLine("       INNER JOIN Servizi")
                .AppendLine("       ON Pratiche.Servizio_Cod = Servizi.Servizio_Cod")
                .AppendLine("       INNER JOIN WAnagraficaStati st")
                .AppendLine("       ON st.WAnagraficaStati_Cod = Pratiche_Stati.Stato_Cod")
                .AppendLine("       WHERE Pratiche.Servizio_Cod = @codiceServizio")
                .AppendLine("       AND Pratiche.Validita_inizio <= @dataValiditaFine")
                .AppendLine("       AND Pratiche.Validita_Fine >= @dataValiditaInizio")
                .AppendLine("       AND Pratiche.Inviato >= 0")
                .AppendLine("   ) statiAttualiTotali")
                .AppendLine("   WHERE NOT statiAttualiTotali.Stato_Des IS NULL")
                .AppendLine(") statoAttuale")
                .AppendLine("ON Pratiche.Piva = statoAttuale.Piva")
                .AppendLine("GROUP BY Pratiche.Piva,")
                .AppendLine("sottosrizione.DataInizio,")
                .AppendLine("statoAttuale.Stato_Des");

            try
            {
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
