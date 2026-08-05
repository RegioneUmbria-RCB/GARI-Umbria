using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System.Data;
using System.Text;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;
using AgronicaCoreDTOStd.OutData.FiltroRicerca;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.MetaSchema.DAL.Resources;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.SpecieVegetali
{
    public class SpecieVegetali : BaseDALMetaschema, ISpecieVegetali
    {
        private enum TipoQuerySpecieVegetali
        {
            EseguiQueryConGruCod = 1,
            EseguiQueryTutteSpecie = 2,
            EseguiQueryUnionAll = 3,
        }
        private enum TipoQueryCultivar
        {
            UtenteConVisibilitaCompleta = 1,
            QueryUnionAll = 2,
            LeggiVarietaDaListaDiVegCod_SenzaControlloVisibilitaSpecie = 3,
        }

        public SpecieVegetali(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
        }

        public async Task<DtConVisibilita_OUT> SpecieVegetali_GestioneFiltroUtente_LeggiAsync(LeggiSpecieVegetali_IN leggiSpecie_IN, DataTable utentiImpostazioniMonoDt, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            DtConVisibilita_OUT result = new DtConVisibilita_OUT();
            TipoQuerySpecieVegetali tipoQuery;
            List<int> gruCodList = new List<int>();

            var filtraPer_Veg_Cod = leggiSpecie_IN.Veg_Cod != 0;
            var filtraPerLetteraIniziale = !string.IsNullOrEmpty(leggiSpecie_IN.LetteraIniziale);
            var filtraPerStringaCerca = !string.IsNullOrEmpty(leggiSpecie_IN.StringaCerca);

            if (leggiSpecie_IN.Gru_Cod != 0)
            {
                tipoQuery = TipoQuerySpecieVegetali.EseguiQueryConGruCod;
            }
            else
            {
                result.VisibilitaApplicata = utentiImpostazioniMonoDt.Rows.Count > 0;

                if (utentiImpostazioniMonoDt.Rows.Count == 0)
                {
                    tipoQuery = TipoQuerySpecieVegetali.EseguiQueryTutteSpecie;
                }
                else if (utentiImpostazioniMonoDt.Rows.Count == 1)
                {
                    leggiSpecie_IN.Gru_Cod = (int)utentiImpostazioniMonoDt.Rows[0]["ID_0"];
                    tipoQuery = TipoQuerySpecieVegetali.EseguiQueryConGruCod;
                }
                else
                {
                    //Esempio: nel filtro ho selezionato erbacee con 4 specie vegetali
                    //e ho selezionato orticole senza specificare le specie (quindi le voglio tutte)

                    //visto che il gru_cod non è passato, per evitare che nel menù vengano caricate solo le specie delle erbacee,
                    //faccio la query per ogni gru_cod e unifico il risultato

                    foreach (DataRow row in utentiImpostazioniMonoDt.Rows)
                    {
                        gruCodList.Add((int)row["Id_0"]);
                    }
                    tipoQuery = TipoQuerySpecieVegetali.EseguiQueryUnionAll;
                }
            }

            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            bool queryCreata = false;

            if (tipoQuery == TipoQuerySpecieVegetali.EseguiQueryConGruCod)
            {
                //struttura query sottostante:

                //if (filtri a livello di specie per questo gru_cdo)
                //      query per estrarre le specie filtrate appartenenti a questo gru_cod
                //else
                //      if(filtri a livello di gruppo)
                //              query per estrarre le specie appartenenti a questo gru_cod se il filtro è per questo gru_cod
                //      else (non c'è nessun filtro, l'utente può vedere tutto)
                //              query per estrarre le specie appartenenti a questo gru_cod


                //se l'utente ha filtrato le specie vegetali, ovvero nella tabella Utenti_Impostazioni_FiltroMono
                //ci sono dei record per l'impostazione COD_FILTRO_SPECIE_VEGETALI per specie vegetali appartenenti a quel gru_cod
                stbQuery.AppendLine(" IF (  ");
                stbQuery.AppendLine(" SELECT COUNT(1)  ");
                stbQuery.AppendLine(" FROM          SpecieVegetali ");
                stbQuery.AppendLine(" INNER JOIN    Utenti_Impostazioni_FiltroMono ");
                stbQuery.AppendLine("               ON SpecieVegetali.veg_cod = Utenti_Impostazioni_FiltroMono.ID_0 ");
                stbQuery.AppendLine(" WHERE         Utenti_Impostazioni_FiltroMono.Piva_SuperUser = @pivaSuperUser");
                stbQuery.AppendLine(" AND           Utenti_Impostazioni_FiltroMono.UserName = @utenteUserName");
                stbQuery.AppendLine(" AND           Utenti_Impostazioni_FiltroMono.Impostazione_Cod = @impostazioneCodSpecieVegetali");
                stbQuery.AppendLine(" AND           SpecieVegetali.Gru_Cod = @gruCod");
                stbQuery.AppendLine("   ) > 0 ");

                stbQuery.AppendLine("       SELECT  SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, ");
                stbQuery.AppendLine("               SpecieVegetali.Gru_Cod, GruppoVegetale.Gru_Des ");
                stbQuery.AppendLine("       FROM    SpecieVegetali ");
                stbQuery.AppendLine("       JOIN    Utenti_Impostazioni_FiltroMono ");
                stbQuery.AppendLine("       ON      SpecieVegetali.veg_cod = Utenti_Impostazioni_FiltroMono.ID_0 ");
                stbQuery.AppendLine("       JOIN    GruppoVegetale ");
                stbQuery.AppendLine("       ON      SpecieVegetali.Gru_Cod = GruppoVegetale.Gru_Cod ");
                stbQuery.AppendLine("       WHERE   SpecieVegetali.Validita_Inizio <= @dtFine");
                stbQuery.AppendLine("       AND     SpecieVegetali.Validita_Fine >= @dtInizio");
                //le specie vegetali che il Gias non deve caricare hanno gru_cod = -1
                stbQuery.AppendLine("       AND     SpecieVegetali.Gru_Cod <> -1 ");
                stbQuery.AppendLine("       AND     Utenti_Impostazioni_FiltroMono.Piva_SuperUser = @pivaSuperUser");
                stbQuery.AppendLine("       AND     Utenti_Impostazioni_FiltroMono.UserName = @utenteUserName");
                stbQuery.AppendLine("       AND     Utenti_Impostazioni_FiltroMono.Impostazione_Cod = @impostazioneCodSpecieVegetali");
                stbQuery.AppendLine("       AND     SpecieVegetali.Gru_Cod = @gruCod");

                if (filtraPer_Veg_Cod)
                    stbQuery.AppendLine("       AND     SpecieVegetali.Veg_Cod = @vegCod");

                if (filtraPerLetteraIniziale)
                    stbQuery.AppendLine("       AND     SpecieVegetali.Veg_Des LIKE CONCAT(@letteraIniziale, '%')");

                if (filtraPerStringaCerca)
                    stbQuery.AppendLine("       AND     SpecieVegetali.Veg_Des LIKE CONCAT('%', @stringaCerca, '%')");

                stbQuery.AppendLine("       ORDER BY SpecieVegetali.Veg_Des ");

                stbQuery.AppendLine(" ELSE ");

                //se l'utente ha filtrato un gruppo vegetale, ovvero nella tabella Utenti_Impostazioni_FiltroMono
                //ci sono dei record per l'impostazione UTENTE_COD_FILTRO_GRUPPI_VEGETALI
                stbQuery.AppendLine("       IF( ");
                stbQuery.AppendLine("       SELECT COUNT(DISTINCT Gru_Cod)");
                stbQuery.AppendLine("       FROM          SpecieVegetali ");
                stbQuery.AppendLine("       INNER JOIN    Utenti_Impostazioni_FiltroMono ");
                stbQuery.AppendLine("       ON            SpecieVegetali.Gru_Cod = Utenti_Impostazioni_FiltroMono.ID_0 "); //join usando il campo Gru_Cod, perché ci interessano i filtri a livello di gruppo
                stbQuery.AppendLine("       WHERE         Utenti_Impostazioni_FiltroMono.Piva_SuperUser = @pivaSuperUser ");
                stbQuery.AppendLine("       AND           Utenti_Impostazioni_FiltroMono.UserName = @utenteUserName ");
                stbQuery.AppendLine("       AND           Utenti_Impostazioni_FiltroMono.Impostazione_Cod = @impostazioneCodGruppiVegetali) ");
                stbQuery.AppendLine("        > 0 ");

                stbQuery.AppendLine("           SELECT  SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, ");
                stbQuery.AppendLine("                   SpecieVegetali.Gru_Cod, GruppoVegetale.Gru_Des ");
                stbQuery.AppendLine("           FROM    SpecieVegetali ");
                stbQuery.AppendLine("           JOIN    Utenti_Impostazioni_FiltroMono ");
                stbQuery.AppendLine("           ON      SpecieVegetali.Gru_Cod = Utenti_Impostazioni_FiltroMono.ID_0 "); //join usando il campo Gru_Cod, perché ci interessano i filtri a livello di gruppo
                stbQuery.AppendLine("           JOIN    GruppoVegetale ");
                stbQuery.AppendLine("           ON      SpecieVegetali.Gru_Cod = GruppoVegetale.Gru_Cod ");
                stbQuery.AppendLine("           WHERE   SpecieVegetali.Validita_Inizio <= @dtFine");
                stbQuery.AppendLine("           AND     SpecieVegetali.Validita_Fine >= @dtInizio");
                //le specie vegetali che il Gias non deve caricare hanno gru_cod = -1
                stbQuery.AppendLine("           AND     SpecieVegetali.Gru_Cod <> -1 ");
                stbQuery.AppendLine("           AND     Utenti_Impostazioni_FiltroMono.Piva_SuperUser = @pivaSuperUser");
                stbQuery.AppendLine("           AND     Utenti_Impostazioni_FiltroMono.UserName = @utenteUserName");
                stbQuery.AppendLine("           AND     Utenti_Impostazioni_FiltroMono.Impostazione_Cod = @impostazioneCodGruppiVegetali");
                stbQuery.AppendLine("           AND     SpecieVegetali.Gru_Cod = @gruCod");

                if (filtraPer_Veg_Cod)
                    stbQuery.AppendLine("           AND     SpecieVegetali.Veg_Cod = @vegCod");

                if (filtraPerLetteraIniziale)
                    stbQuery.AppendLine("           AND     SpecieVegetali.Veg_Des LIKE CONCAT(@letteraIniziale, '%')");

                if (filtraPerStringaCerca)
                    stbQuery.AppendLine("           AND     SpecieVegetali.Veg_Des LIKE CONCAT('%', @stringaCerca, '%')");

                stbQuery.AppendLine("           ORDER BY SpecieVegetali.Veg_Des ");

                //nessun filtro, possiamo vedere tutto, restituiamo all'utente i dati per quel gru cod
                stbQuery.AppendLine("       ELSE ");
                stbQuery.AppendLine("           SELECT  SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, ");
                stbQuery.AppendLine("                   SpecieVegetali.Gru_Cod, GruppoVegetale.Gru_Des ");
                stbQuery.AppendLine("           FROM    SpecieVegetali ");
                stbQuery.AppendLine("           JOIN    GruppoVegetale ");
                stbQuery.AppendLine("           ON      SpecieVegetali.Gru_Cod = GruppoVegetale.Gru_Cod ");
                stbQuery.AppendLine("           WHERE   SpecieVegetali.Validita_Inizio <=@dtFine");
                stbQuery.AppendLine("           AND     SpecieVegetali.Validita_Fine >=@dtInizio");
                //le specie vegetali che il Gias non deve caricare hanno gru_cod = -1
                stbQuery.AppendLine("           AND     SpecieVegetali.Gru_Cod <> -1 ");
                stbQuery.AppendLine("           AND     SpecieVegetali.Gru_Cod = @gruCod");

                if (filtraPer_Veg_Cod)
                    stbQuery.AppendLine("           AND     Veg_Cod = @vegCod");

                if (filtraPerLetteraIniziale)
                    stbQuery.AppendLine("           AND     Veg_Des LIKE CONCAT(@letteraIniziale, '%')");

                if (filtraPerStringaCerca)
                    stbQuery.AppendLine("           AND     Veg_Des LIKE CONCAT('%', @stringaCerca, '%')");

                stbQuery.AppendLine("           ORDER BY SpecieVegetali.Veg_Des ");

                parametriSql.Add("@pivaSuperUser", objParametriUtenti.PivaSuperUser);
                parametriSql.Add("@utenteUserName", objParametriUtenti.UtenteUsername);
                parametriSql.Add("@impostazioneCodSpecieVegetali", TipiEnumerativi.Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI);
                parametriSql.Add("@impostazioneCodGruppiVegetali", TipiEnumerativi.Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_VEGETALI);
                parametriSql.Add("@gruCod", leggiSpecie_IN.Gru_Cod);
                parametriSql.Add("@dtInizio", objParametriUtenti.FinestraTemporaleInizio);
                parametriSql.Add("@dtFine", objParametriUtenti.FinestraTemporaleFine);

                if (filtraPer_Veg_Cod)
                    parametriSql.Add("@vegCod", leggiSpecie_IN.Veg_Cod);

                if (filtraPerLetteraIniziale)
                    parametriSql.Add("@letteraIniziale", leggiSpecie_IN.LetteraIniziale);

                if (filtraPerStringaCerca)
                    parametriSql.Add("@stringaCerca", leggiSpecie_IN.StringaCerca);

                queryCreata = true;
            }
            else if (tipoQuery == TipoQuerySpecieVegetali.EseguiQueryTutteSpecie)
            {
                var filtraPer_Gru_Cod = leggiSpecie_IN.Gru_Cod != 0;

                stbQuery.AppendLine(" SELECT        SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des,");
                stbQuery.AppendLine("               SpecieVegetali.Gru_Cod, GruppoVegetale.Gru_Des ");
                stbQuery.AppendLine(" FROM          SpecieVegetali");
                stbQuery.AppendLine(" JOIN          GruppoVegetale ");
                stbQuery.AppendLine(" ON            SpecieVegetali.Gru_Cod = GruppoVegetale.Gru_Cod ");
                stbQuery.AppendLine(" WHERE         SpecieVegetali.Validita_Inizio <= @dtFine");
                stbQuery.AppendLine(" AND           SpecieVegetali.Validita_Fine >= @dtInizio");
                stbQuery.AppendLine(" AND           SpecieVegetali.Gru_Cod <> -1 ");

                if (filtraPer_Gru_Cod)
                    stbQuery.AppendLine(" AND           SpecieVegetali.Gru_Cod = @gruCod");

                if (filtraPer_Veg_Cod)
                    stbQuery.AppendLine(" AND           Veg_Cod = @vegCod");

                if (filtraPerLetteraIniziale)
                    stbQuery.AppendLine(" AND           Veg_Des LIKE CONCAT(@letteraIniziale, '%')");

                if (filtraPerStringaCerca)
                    stbQuery.AppendLine(" AND           Veg_Des LIKE CONCAT('%', @stringaCerca, '%')");

                stbQuery.AppendLine(" ORDER BY      SpecieVegetali.Veg_Des");

                parametriSql.Add("@dtInizio", objParametriUtenti.FinestraTemporaleInizio);
                parametriSql.Add("@dtFine", objParametriUtenti.FinestraTemporaleFine);

                if (filtraPer_Gru_Cod)
                    parametriSql.Add("@gruCod", leggiSpecie_IN.Gru_Cod);

                if (filtraPer_Veg_Cod)
                    parametriSql.Add("@vegCod", leggiSpecie_IN.Veg_Cod);

                if (filtraPerLetteraIniziale)
                    parametriSql.Add("@letteraIniziale", leggiSpecie_IN.LetteraIniziale);

                if (filtraPerStringaCerca)
                    parametriSql.Add("@stringaCerca", leggiSpecie_IN.StringaCerca);

                queryCreata = true;
            }

            if (queryCreata)
            {
                try
                {
                    result.DataTable = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
                }
                catch (Exception ex)
                {
                    LogError(ex.Message, objParametriServer, ex);
                    throw;
                }
                return result;
            }

            if (tipoQuery == TipoQuerySpecieVegetali.EseguiQueryUnionAll)
            {
                DataTable DT = new DataTable();
                var primoElemento = true;

                foreach (var gruCodElement in gruCodList)
                {
                    stbQuery.Length = 0;
                    DT.Clear();
                    parametriSql.Clear();

                    stbQuery.AppendLine(" IF (  ");
                    stbQuery.AppendLine(" SELECT COUNT(*)  ");
                    stbQuery.AppendLine(" FROM          SpecieVegetali ");
                    stbQuery.AppendLine(" INNER JOIN    Utenti_Impostazioni_FiltroMono ");
                    stbQuery.AppendLine("               ON SpecieVegetali.veg_cod = Utenti_Impostazioni_FiltroMono.ID_0 ");
                    stbQuery.AppendLine(" WHERE         Utenti_Impostazioni_FiltroMono.Piva_SuperUser = @pivaSuperUser");
                    stbQuery.AppendLine(" AND           Utenti_Impostazioni_FiltroMono.UserName = @utenteUserName");
                    stbQuery.AppendLine(" AND           Utenti_Impostazioni_FiltroMono.Impostazione_Cod = @impostazioneCod");
                    stbQuery.AppendLine(" AND           SpecieVegetali.Gru_Cod = @gruCod"); //per sapere se l'utente ha impostato dei filtri a livello di specie per questo gruppo
                    stbQuery.AppendLine("   ) > 0 ");

                    stbQuery.AppendLine("       SELECT   SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, ");
                    stbQuery.AppendLine("                SpecieVegetali.Gru_Cod, GruppoVegetale.Gru_Des ");
                    stbQuery.AppendLine("       FROM     SpecieVegetali ");
                    stbQuery.AppendLine("       JOIN     Utenti_Impostazioni_FiltroMono ");
                    stbQuery.AppendLine("       ON       SpecieVegetali.veg_cod = Utenti_Impostazioni_FiltroMono.ID_0 ");
                    stbQuery.AppendLine("       JOIN     GruppoVegetale ");
                    stbQuery.AppendLine("       ON       SpecieVegetali.Gru_Cod = GruppoVegetale.Gru_Cod ");
                    stbQuery.AppendLine("       WHERE    SpecieVegetali.Validita_Inizio <= @dtFine");
                    stbQuery.AppendLine("       AND      SpecieVegetali.Validita_Fine >= @dtInizio");
                    //le specie vegetali che il Gias non deve caricare hanno gru_cod = -1
                    stbQuery.AppendLine("       AND      SpecieVegetali.Gru_Cod <> -1 ");
                    stbQuery.AppendLine("       AND      Utenti_Impostazioni_FiltroMono.Piva_SuperUser = @pivaSuperUser");
                    stbQuery.AppendLine("       AND      Utenti_Impostazioni_FiltroMono.UserName = @utenteUserName");
                    stbQuery.AppendLine("       AND      Utenti_Impostazioni_FiltroMono.Impostazione_Cod = @impostazioneCod");
                    stbQuery.AppendLine("       AND      SpecieVegetali.Gru_Cod = @gruCod");

                    if (filtraPer_Veg_Cod)
                        stbQuery.AppendLine("       AND      SpecieVegetali.Veg_Cod = @vegCod");

                    if (filtraPerLetteraIniziale)
                        stbQuery.AppendLine("       AND      SpecieVegetali.Veg_Des LIKE CONCAT(@letteraIniziale, '%')");

                    if (filtraPerStringaCerca)
                        stbQuery.AppendLine("       AND      SpecieVegetali.Veg_Des LIKE CONCAT('%', @stringaCerca, '%')");

                    stbQuery.AppendLine("       ORDER BY SpecieVegetali.Veg_Des ");

                    stbQuery.AppendLine(" ELSE ");

                    stbQuery.AppendLine("       SELECT   SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, ");
                    stbQuery.AppendLine("                SpecieVegetali.Gru_Cod, GruppoVegetale.Gru_Des ");
                    stbQuery.AppendLine("       FROM     SpecieVegetali ");
                    stbQuery.AppendLine("       JOIN     GruppoVegetale ");
                    stbQuery.AppendLine("       ON       SpecieVegetali.Gru_Cod = GruppoVegetale.Gru_Cod ");
                    stbQuery.AppendLine("       WHERE    SpecieVegetali.Validita_Inizio <=@dtFine");
                    stbQuery.AppendLine("       AND      SpecieVegetali.Validita_Fine >= @dtInizio");
                    //le specie vegetali che il Gias non deve caricare hanno gru_cod = -1
                    stbQuery.AppendLine("       AND      SpecieVegetali.Gru_Cod <> -1 ");
                    stbQuery.AppendLine("       AND      SpecieVegetali.Gru_Cod = @gruCod");

                    if (filtraPer_Veg_Cod)
                        stbQuery.AppendLine("       AND      Veg_Cod = @vegCod");

                    if (filtraPerLetteraIniziale)
                        stbQuery.AppendLine("       AND      Veg_Des LIKE CONCAT(@letteraIniziale, '%')");

                    if (filtraPerStringaCerca)
                        stbQuery.AppendLine("       AND      Veg_Des LIKE CONCAT('%', @stringaCerca, '%')");

                    stbQuery.AppendLine("       ORDER BY Veg_Des ");

                    parametriSql.Add("@pivaSuperUser", objParametriUtenti.PivaSuperUser);
                    parametriSql.Add("@utenteUserName", objParametriUtenti.UtenteUsername);
                    parametriSql.Add("@impostazioneCod", TipiEnumerativi.Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI);
                    parametriSql.Add("@gruCod", gruCodElement);
                    parametriSql.Add("@dtInizio", objParametriUtenti.FinestraTemporaleInizio);
                    parametriSql.Add("@dtFine", objParametriUtenti.FinestraTemporaleFine);

                    if (filtraPer_Veg_Cod)
                        parametriSql.Add("@vegCod", leggiSpecie_IN.Veg_Cod);

                    if (filtraPerLetteraIniziale)
                        parametriSql.Add("@letteraIniziale", leggiSpecie_IN.LetteraIniziale);

                    if (filtraPerStringaCerca)
                        parametriSql.Add("@stringaCerca", leggiSpecie_IN.StringaCerca);

                    try
                    {
                        DT = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
                    }
                    catch (Exception ex)
                    {
                        LogError(ex.Message, objParametriServer, ex);
                        throw;
                    }

                    if (primoElemento)
                    {
                        result.DataTable = DT.Clone();
                        primoElemento = false;
                    }

                    foreach (DataRow row in DT.Rows)
                    {
                        result.DataTable.ImportRow(row);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Verifica se l'utente ha un filtro impostato, in tal caso legge le varietà selezionate nel filtro, altrimenti legge tutte le varietà in archivio
        /// Attenzione! Questa query è fatta sul database UTENTI, dall' Aggancio a MetaSchema_11 del 26/05/2009 ci sono infatti anche delle viste nel db utenti sul metaschema
        /// </summary>
        /// <param name="leggiCultivar_IN"></param>
        /// <param name="objParametri"></param>
        /// <returns></returns>
        public async Task<DataTable> Cultivar_GestioneFiltroUtente_LeggiAsync(Cultivar_GestioneFiltroUtente_Leggi_IN leggiCultivar_IN, DataTable utentiImpostazioniMonoDt, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            var gruCodList = new List<int>();
            TipoQueryCultivar tipoQuery = DeterminaIlTipoDiQueryPerLaLetturaDelleVarieta(leggiCultivar_IN, gruCodList, utentiImpostazioniMonoDt);

            DataTable result;

            if (tipoQuery == TipoQueryCultivar.UtenteConVisibilitaCompleta)
            {
                result = await LeggiVarieta_ConVisibilitaCompletaAsync(leggiCultivar_IN, objParametriUtenti, objParametriServer);
            }
            else if (tipoQuery == TipoQueryCultivar.QueryUnionAll)
            {
                result = await EseguiQueryUnionAllPerLetturaVarietaAsync(leggiCultivar_IN, gruCodList, objParametriUtenti, objParametriServer);
            }
            else if (tipoQuery == TipoQueryCultivar.LeggiVarietaDaListaDiVegCod_SenzaControlloVisibilitaSpecie)
            {
                result = await LeggiVarietaDaListaDiVegCodAsync(leggiCultivar_IN.Veg_Cod_List, objParametriUtenti, objParametriServer, leggiCultivar_IN.Cul_Cod, leggiCultivar_IN.Cerca_CulDes);
            }
            else
                throw new NotImplementedException("Tipo query non implementata");

            return result;
        }

        /// <summary>
        /// Verifica se l'utente ha un filtro impostato, in tal caso legge i gruppi vegetali selezionati nel filtro, altrimenti legge tutti i gruppi vegetali 
        /// </summary>
        /// <param name="gru_cod">Codice gruppo, se uguale a zero il metodo legge tutti i gruppi che l'utente può leggere</param>
        /// <param name="objParametriUtenti"></param>
        /// <param name="objParametriServer"></param>
        /// <returns></returns>
        public async Task<DataTable> GruppoVegetale_GestioneFiltroUtente_LeggiAsync(int gru_cod, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            var filtraPerGruCod = gru_cod != 0;

            stbQuery.AppendLine(" IF (  ");
            stbQuery.AppendLine(" SELECT        COUNT(*)  ");
            stbQuery.AppendLine(" FROM          GruppoVegetale ");
            stbQuery.AppendLine(" INNER JOIN    Utenti_Impostazioni_FiltroMono ");
            stbQuery.AppendLine("               ON GruppoVegetale.Gru_Cod = Utenti_Impostazioni_FiltroMono.ID_0 ");
            stbQuery.AppendLine(" WHERE         Utenti_Impostazioni_FiltroMono.Piva_SuperUser = @pivaSuperUser");
            stbQuery.AppendLine(" AND           Utenti_Impostazioni_FiltroMono.UserName = @usernameOperazione");
            stbQuery.AppendLine(" AND           Utenti_Impostazioni_FiltroMono.Impostazione_Cod = @impostazioneCod");
            stbQuery.AppendLine("   ) > 0 ");
            stbQuery.AppendLine("  ");

            stbQuery.AppendLine("       SELECT        Gru_Cod, Gru_Des ");
            stbQuery.AppendLine("       FROM          GruppoVegetale ");
            stbQuery.AppendLine("       INNER JOIN    Utenti_Impostazioni_FiltroMono ");
            stbQuery.AppendLine("       ON            GruppoVegetale.Gru_Cod = Utenti_Impostazioni_FiltroMono.ID_0 ");
            stbQuery.AppendLine("       WHERE         GruppoVegetale.Validita_Inizio <= @dtFine");
            stbQuery.AppendLine("       AND           GruppoVegetale.Validita_Fine >= @dtInizio");
            stbQuery.AppendLine("       AND           Utenti_Impostazioni_FiltroMono.Piva_SuperUser = @pivaSuperUser");
            stbQuery.AppendLine("       AND           Utenti_Impostazioni_FiltroMono.UserName = @usernameOperazione");
            stbQuery.AppendLine("       AND           Utenti_Impostazioni_FiltroMono.Impostazione_Cod = @impostazioneCod");
            if (filtraPerGruCod)
                stbQuery.AppendLine("       AND           GruppoVegetale.Gru_Cod = @gruCod");
            stbQuery.AppendLine("       ORDER BY      GruppoVegetale.Gru_Des");

            stbQuery.AppendLine(" ELSE");
            stbQuery.AppendLine("       SELECT        Gru_Cod, Gru_Des ");
            stbQuery.AppendLine("       FROM          GruppoVegetale ");
            stbQuery.AppendLine("       WHERE         Validita_Inizio <= @dtFine");
            stbQuery.AppendLine("       AND           Validita_Fine >= @dtInizio");
            if (filtraPerGruCod)
                stbQuery.AppendLine("       AND           GruppoVegetale.Gru_Cod = @gruCod");
            stbQuery.AppendLine("       ORDER BY      Gru_Des");

            parametriSql.Add("@pivaSuperUser", objParametriUtenti.PivaSuperUser);
            parametriSql.Add("@usernameOperazione", objParametriUtenti.UtenteUsername);
            parametriSql.Add("@impostazioneCod", Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_VEGETALI);
            parametriSql.Add("@dtInizio", objParametriUtenti.FinestraTemporaleInizio);
            parametriSql.Add("@dtFine", objParametriUtenti.FinestraTemporaleFine);
            if (filtraPerGruCod)
                parametriSql.Add("@gruCod", gru_cod);

            try
            {
                result = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }

        public async Task<DataTable> LeggiGruppiVarietaliAsync(LeggiGruppiVarietali_IN leggiGruppiVarietali_IN, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            var filtraPer_Veg_Cod = leggiGruppiVarietali_IN.Veg_Cod_List.Any();

            stbQuery.AppendLine(" SELECT        GruppoVarietale.Grva_Cod, GruppoVarietale.Grva_Des, SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des");
            stbQuery.AppendLine(" FROM          GruppoVarietale");
            stbQuery.AppendLine(" INNER JOIN    SpecieVegetalixGruppoVarietale");
            stbQuery.AppendLine(" ON            SpecieVegetalixGruppoVarietale.Grva_Cod = GruppoVarietale.Grva_Cod");
            stbQuery.AppendLine(" INNER JOIN    SpecieVegetali");
            stbQuery.AppendLine(" ON            SpecieVegetali.Veg_Cod = SpecieVegetalixGruppoVarietale.Veg_Cod ");
            stbQuery.AppendLine(" WHERE         1 = 1");

            if (filtraPer_Veg_Cod)
            {
                stbQuery.Append(" AND           SpecieVegetali.Veg_Cod IN (");

                for (int i = 0; i < leggiGruppiVarietali_IN.Veg_Cod_List.Count; i++)
                {
                    stbQuery.Append($"@vegCod{i}");
                    parametriSql.Add($"@vegCod{i}", leggiGruppiVarietali_IN.Veg_Cod_List[i]);

                    if (i != leggiGruppiVarietali_IN.Veg_Cod_List.Count - 1)
                    {
                        //aggiungiamo la virgola per il prossimo parametro
                        stbQuery.Append(",");
                    }
                }

                stbQuery.AppendLine(")");
            }

            if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati)
                stbQuery.AppendLine(" AND           GruppoVarietale.Inviato >= 0 ");
            else if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati)
                stbQuery.AppendLine(" AND           GruppoVarietale.Inviato = -1 ");
            else if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti)
            {
                //nessun filtro da aggiungere
            }
            else
                throw new Exception("Parametro non corretto nella query (FlagVisibilita)");

            stbQuery.AppendLine(" ORDER BY      Veg_Des, Grva_Des ");

            try
            {
                var provider = GetDataProvider(objParametriServer);
                result = await provider.ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }

        public async Task<string> VegDesFromVegCodAsync(int vegCod, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            stbQuery.AppendLine(" SELECT veg_cod, veg_des ");
            stbQuery.AppendLine(" FROM  SpecieVegetali ");
            stbQuery.AppendLine(" WHERE Validita_inizio <= @dtFine ");
            stbQuery.AppendLine("   AND Validita_Fine >= @dtInizio ");
            stbQuery.AppendLine("   AND Gru_Cod <> -1 ");
            stbQuery.AppendLine("   AND Veg_Cod = @vegCod ");

            sqlParams.TryAdd("@dtInizio", objParametriServer.FinestraTemporaleInizio);
            sqlParams.TryAdd("@dtFine", objParametriServer.FinestraTemporaleFine);
            sqlParams.TryAdd("@vegCod", vegCod);

            switch (objParametriServer.FlagVisibilita)
            {
                case AgronicaCoreParametri.enumVisibilita.visibilita_SoloNonInviati:
                    stbQuery.AppendLine(" AND Inviato = 0 ");
                    break;
                case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati:
                    stbQuery.AppendLine(" AND Inviato =-1 ");
                    break;
                case AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti:
                    break;
                default:
                    throw new Exception("Parametro non corretto nella query (FlagVisibilita)");
            }

            stbQuery.AppendLine(" ORDER BY Veg_Des ASC ");

            try
            {
                var datatable = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
                return datatable.Rows.Count > 0
                    ? datatable.Rows[0]["Veg_Des"].ToString() ?? string.Empty
                    : string.Empty;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiVarietaAsync(int culCod, int vegCod, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            stbQuery.AppendLine(" SELECT Cul_Cod, Cul_Des ");
            stbQuery.AppendLine(" FROM  Cultivar ");
            stbQuery.AppendLine(" WHERE Validita_inizio <= @dtFine ");
            stbQuery.AppendLine("   AND Validita_Fine >= @dtInizio ");

            if (culCod != 0)
            {
                stbQuery.AppendLine(" AND Cultivar.Cul_Cod = @culCod ");
                sqlParams.TryAdd("@culCod", culCod);
            }

            if (vegCod != 0)
            {
                stbQuery.AppendLine(" AND Cultivar.Veg_Cod = @vegCod ");
                sqlParams.TryAdd("@vegCod", vegCod);
            }

            sqlParams.TryAdd("@dtInizio", objParametriUtenti.FinestraTemporaleInizio);
            sqlParams.TryAdd("@dtFine", objParametriUtenti.FinestraTemporaleFine);
            sqlParams.TryAdd("@vegCod", vegCod);

            switch (objParametriUtenti.FlagVisibilita)
            {
                case AgronicaCoreParametri.enumVisibilita.visibilita_SoloNonInviati:
                    stbQuery.AppendLine(" AND Inviato = 0 ");
                    break;
                case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati:
                    stbQuery.AppendLine(" AND Inviato =-1 ");
                    break;
                case AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti:
                    break;
                default:
                    throw new Exception("Parametro non corretto nella query (FlagVisibilita)");
            }

            stbQuery.AppendLine(" ORDER BY Cul_Des ");

            try
            {
                return await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiSpecieAziendaliAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            stbQuery.AppendLine(" SELECT DISTINCT ");
            stbQuery.AppendLine(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS Veg_Cod, ");
            stbQuery.AppendLine(" ISNULL(SpecieVegetali.Veg_Des, 'Terreno Nudo') AS Veg_Des ");
            stbQuery.AppendLine(" FROM Appezzamento INNER JOIN ");
            stbQuery.AppendLine(
                " Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ");
            stbQuery.AppendLine(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA LEFT OUTER JOIN ");
            stbQuery.AppendLine(" SpecieVegetali INNER JOIN ");
            stbQuery.AppendLine(
                " Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ON Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod ");
            stbQuery.AppendLine(" WHERE 1=1 ");

            if (!string.IsNullOrEmpty(piva) && piva != "0")
            {
                stbQuery.AppendLine(" AND  Reg_Impianti.PIVA = @piva ");
                sqlParams.TryAdd("@piva", piva);
            }

            stbQuery.AppendLine(" ORDER BY VEG_DES ");

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

        public async Task<DataTable> LeggiVarietaFilteredAsync(string piva, int culCod, int vegCod, DataTable dtCentriVisibili, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            stbQuery.AppendLine(" SELECT DISTINCT Cultivar.Cul_Des, Cultivar.Cul_Cod ");
            stbQuery.AppendLine(" FROM    Cultivar INNER JOIN Reg_Impianti ");
            stbQuery.AppendLine("         ON Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod ");
            stbQuery.AppendLine(" WHERE   Cultivar.VEG_COD = @vegCod ");

            if (!string.IsNullOrEmpty(piva))
            {
                stbQuery.AppendLine(" AND     Reg_Impianti.Piva = @piva ");
                sqlParams.TryAdd("@piva", piva);
            }

            if (dtCentriVisibili is { Rows.Count: > 0 })
            {
                var centri = dtCentriVisibili.Rows;
                foreach (DataRow row in dtCentriVisibili.Rows)
                {
                    centri.Add(row["sa_cod"].ToString()!);
                }

                if (centri.Count > 0)
                {
                    stbQuery.AppendLine(" AND Reg_Impianti.sa_cod IN (@filtroCentri) ");
                    sqlParams.TryAdd("@filtroCentri", string.Join(",", centri));
                }
            }

            stbQuery.AppendLine(" AND NOT ( Reg_Impianti.Validita_Inizio > @dtFine) ");
            stbQuery.AppendLine(" AND NOT ( Reg_Impianti.Validita_Fine < @dtInizio) ");
            stbQuery.AppendLine(" ORDER BY Cultivar.Cul_Des");

            sqlParams.TryAdd("@vegCod", vegCod);
            sqlParams.TryAdd("@dtFine", objParametriServer.FinestraTemporaleFine);
            sqlParams.TryAdd("@dtInizio", objParametriServer.FinestraTemporaleInizio);

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

        public async Task<DataTable> LeggiCompletaAsync(int vegCod, int gruCod, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            stbQuery.AppendLine(" SELECT * ");
            stbQuery.AppendLine(" FROM SpecieVegetali ");
            stbQuery.AppendLine(" WHERE Validita_Inizio <= @dtFine ");
            stbQuery.AppendLine(" AND Validita_Fine >= @dtInizio ");
            stbQuery.AppendLine(" AND Gru_Cod <> -1 ");

            sqlParams.TryAdd("@dtFine", objParametriServer.FinestraTemporaleFine);
            sqlParams.TryAdd("@dtInizio", objParametriServer.FinestraTemporaleInizio);

            if (vegCod != 0)
            {
                stbQuery.AppendLine(" AND Veg_Cod = @vegCod ");
                sqlParams.TryAdd("@vegCod", vegCod);
            }

            if (gruCod != 0)
            {
                stbQuery.AppendLine(" AND Gru_Cod = @gruCod ");
                sqlParams.TryAdd("@gruCod", gruCod);
            }

            switch (objParametriServer.FlagVisibilita)
            {
                case AgronicaCoreParametri.enumVisibilita.visibilita_SoloNonInviati:
                    stbQuery.AppendLine(" AND Inviato = 0 ");
                    break;
                case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati:
                    stbQuery.AppendLine(" AND Inviato =-1 ");
                    break;
                case AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti:
                    break;
                default:
                    throw new Exception("Parametro non corretto nella query (FlagVisibilita)");
            }

            stbQuery.AppendLine(" ORDER BY Veg_Des ASC ");

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

        /// <summary>
        /// Determina il tipo di query da effettuare per la lettura delle varietà distinguendo tra il caso senza nessuna limitazione e il caso con limitazioni
        /// </summary>
        /// <param name="gruCodList"></param>
        /// <returns></returns>
        private TipoQueryCultivar DeterminaIlTipoDiQueryPerLaLetturaDelleVarieta(Cultivar_GestioneFiltroUtente_Leggi_IN leggiCultivar_IN, List<int> gruCodList, DataTable utentiImpostazioniMonoDt)
        {
            if (!leggiCultivar_IN.ControllaLaVisibilitaDelleSpecie && leggiCultivar_IN.Veg_Cod_List.Any())
                return TipoQueryCultivar.LeggiVarietaDaListaDiVegCod_SenzaControlloVisibilitaSpecie;

            TipoQueryCultivar tipoQuery;

            if (utentiImpostazioniMonoDt.Rows.Count == 0)
            {
                //l'utente ha visibilità completa
                tipoQuery = TipoQueryCultivar.UtenteConVisibilitaCompleta;
            }
            else
            {
                foreach (DataRow row in utentiImpostazioniMonoDt.Rows)
                {
                    gruCodList.Add((int)row["Id_0"]);
                }
                tipoQuery = TipoQueryCultivar.QueryUnionAll;
            }

            return tipoQuery;
        }

        /// <summary>
        /// Legge le varietà senza controllare se ci sono dei filtri per l'utente a livello di specie
        /// </summary>
        /// <param name="leggiCultivar_IN"></param>
        /// <param name="objParametriUtenti"></param>
        /// <param name="objParametriServer"></param>
        /// <param name="gruCod">Se diverso da zero, </param>
        /// <returns></returns>
        private async Task<DataTable> LeggiVarieta_ConVisibilitaCompletaAsync(Cultivar_GestioneFiltroUtente_Leggi_IN leggiCultivar_IN, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer, int gruCod = 0)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            var filtraPer_Veg_Cod = leggiCultivar_IN.Veg_Cod_List.Any();
            var filtraPer_Cul_Cod = leggiCultivar_IN.Cul_Cod != 0;
            var filtraPer_Cerca_CulDes = !string.IsNullOrEmpty(leggiCultivar_IN.Cerca_CulDes);
            var filtraPerGruCod = gruCod != 0;

            stbQuery.AppendLine(" SELECT       SpecieVegetali.Veg_des, ");
            stbQuery.AppendLine("              Cultivar.Cul_Cod, Cultivar.Cul_Cod_Aux, Cultivar.Cul_Cod_ORACOLO, Cultivar.Cul_Cod_CRPV, Cultivar.Cul_Des, Cultivar.Veg_Cod, ");
            stbQuery.AppendLine("              Cultivar.DATA_AGG, Cultivar.inviato, Cultivar.datainvio, Cultivar.Data_Creazione, Cultivar.Data_Modifica, Cultivar.Username_Creazione, ");
            stbQuery.AppendLine("              Cultivar.Username_Modifica, Cultivar.Validita_Inizio, Cultivar.Validita_Fine, Cultivar.Fatto");
            stbQuery.AppendLine(" FROM         Cultivar ");
            stbQuery.AppendLine(" INNER JOIN   SpecieVegetali ");
            stbQuery.AppendLine(" ON           SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ");
            stbQuery.AppendLine(" WHERE        Cultivar.Validita_Inizio <= @dtFine");
            stbQuery.AppendLine(" AND          Cultivar.Validita_Fine >=@dtInizio");
            stbQuery.AppendLine(" AND          SpecieVegetali.Gru_Cod <> -1");

            if (filtraPerGruCod)
                stbQuery.AppendLine(" AND          SpecieVegetali.Gru_Cod = @gruCod");

            if (filtraPer_Cul_Cod)
                stbQuery.AppendLine(" AND          Cultivar.Cul_Cod = @culCod");

            if (filtraPer_Veg_Cod)
            {
                stbQuery.Append(" AND          Cultivar.Veg_Cod IN (");

                for (int i = 0; i < leggiCultivar_IN.Veg_Cod_List.Count; i++)
                {
                    stbQuery.Append($"@vegCod{i}");

                    if (i != leggiCultivar_IN.Veg_Cod_List.Count - 1)
                    {
                        //aggiungiamo la virgola per il prossimo parametro
                        stbQuery.Append(",");
                    }
                }

                stbQuery.AppendLine(")");
            }

            if (filtraPer_Cerca_CulDes)
                stbQuery.AppendLine(" AND          Cultivar.Cul_Des LIKE CONCAT('%', @cercaCulDes, '%')");

            stbQuery.AppendLine(" ORDER BY     Veg_Des, Cul_Des ");

            parametriSql.Add("@dtInizio", objParametriUtenti.FinestraTemporaleInizio);
            parametriSql.Add("@dtFine", objParametriUtenti.FinestraTemporaleFine);

            if (filtraPer_Veg_Cod)
            {
                for (int i = 0; i < leggiCultivar_IN.Veg_Cod_List.Count; i++)
                {
                    parametriSql.Add($"@vegCod{i}", leggiCultivar_IN.Veg_Cod_List[i]);
                }
            }

            if (filtraPer_Cul_Cod)
                parametriSql.Add("@culCod", leggiCultivar_IN.Cul_Cod);

            if (filtraPer_Cerca_CulDes)
                parametriSql.Add("@cercaCulDes", leggiCultivar_IN.Cerca_CulDes);

            if (filtraPerGruCod)
                parametriSql.Add("@gruCod", gruCod);

            try
            {
                result = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Esegue query union all mettendo insieme i risultati delle varietà per ogni specie
        /// </summary>
        /// <param name="leggiCultivar_IN"></param>
        /// <param name="gruCodList"></param>
        /// <param name="objParametriUtenti"></param>
        /// <param name="objParametriServer"></param>
        /// <returns></returns>
        private async Task<DataTable> EseguiQueryUnionAllPerLetturaVarietaAsync(Cultivar_GestioneFiltroUtente_Leggi_IN leggiCultivar_IN, List<int> gruCodList, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable risultatoFinale = new DataTable();
            DataTable risultatoIntermedio = new DataTable();
            var primoElemento = true;

            foreach (var gruCodElement in gruCodList)
            {
                if (gruCodElement != 0)
                {
                    risultatoIntermedio.Clear();

                    var filtriALivelloDiSpeciePerGruCod = await FiltriALivelloDiSpeciePerGruCodAsync(gruCodElement, objParametriUtenti, objParametriServer);
                    if (filtriALivelloDiSpeciePerGruCod)
                    {
                        var vegCodVisibiliPerGruCod = await GetVegCodVisibiliPerGruCodAsync(gruCodElement, objParametriUtenti, objParametriServer);
                        if (leggiCultivar_IN.Veg_Cod_List.Any())
                        {
                            var vegCodDaUsarePerIlFiltro = leggiCultivar_IN.Veg_Cod_List.Where(v => vegCodVisibiliPerGruCod.Contains(v)).ToList();

                            if (vegCodDaUsarePerIlFiltro.Any())
                            {
                                risultatoIntermedio = await LeggiVarietaDaListaDiVegCodAsync(vegCodDaUsarePerIlFiltro, objParametriUtenti, objParametriServer, leggiCultivar_IN.Cul_Cod, leggiCultivar_IN.Cerca_CulDes);
                            }
                            else
                            {
                                var dummyVegCodList = new List<int>()
                            {
                                -2000
                            };

                                //effettuo una lettura con dati dummy per prendere la struttura della tabella
                                risultatoIntermedio = await LeggiVarietaDaListaDiVegCodAsync(dummyVegCodList, objParametriUtenti, objParametriServer, leggiCultivar_IN.Cul_Cod, leggiCultivar_IN.Cerca_CulDes);
                                risultatoIntermedio.Clear();
                            }
                        }
                        else //leggi le varietà per tutte le specie visibili per questo gruCod
                        {
                            risultatoIntermedio = await LeggiVarietaDaListaDiVegCodAsync(vegCodVisibiliPerGruCod, objParametriUtenti, objParametriServer, leggiCultivar_IN.Cul_Cod, leggiCultivar_IN.Cerca_CulDes);
                        }

                    }
                    else //posso leggere tutte le varietà per questo gru cod
                    {
                        risultatoIntermedio = await LeggiVarieta_ConVisibilitaCompletaAsync(leggiCultivar_IN, objParametriUtenti,objParametriServer, gruCodElement);
                    }

                    if (primoElemento)
                    {
                        risultatoFinale = risultatoIntermedio.Clone();
                        primoElemento = false;
                    }
                    foreach (DataRow row in risultatoIntermedio.Rows)
                    {
                        risultatoFinale.ImportRow(row);
                    }
                }
            }

            return risultatoFinale;
        }

        /// <summary>
        /// Data in input una lista di veg_cod, restituisce le varietà visibili dall'utente per questi veg_cod
        /// </summary>
        /// <param name="vegCodList"></param>
        /// <param name="objParametriUtenti"></param>
        /// <param name="objParametriServer"></param>
        /// <param name="Cul_Cod"></param>
        /// <param name="Cerca_CulDes"></param>
        /// <returns></returns>
        private async Task<DataTable> LeggiVarietaDaListaDiVegCodAsync(List<int> vegCodList, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer, int Cul_Cod, string Cerca_CulDes)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable risultatoFinale = new DataTable();
            DataTable risultatoIntermedio = new DataTable();
            var primoElemento = true;

            var filtraPer_Cul_Cod = Cul_Cod != 0;
            var filtraPer_Cerca_CulDes = !string.IsNullOrEmpty(Cerca_CulDes);

            foreach (var vegCod in vegCodList)
            {
                stbQuery.Length = 0;
                parametriSql.Clear();
                risultatoIntermedio.Clear();

                stbQuery.AppendLine(" IF (  ");
                stbQuery.AppendLine(" SELECT COUNT(*)  ");
                stbQuery.AppendLine(" FROM  Cultivar ");
                stbQuery.AppendLine(" INNER JOIN Utenti_Impostazioni_FiltroMono ");
                stbQuery.AppendLine("               ON Cultivar.Cul_cod = Utenti_Impostazioni_FiltroMono.ID_0 ");
                stbQuery.AppendLine(" WHERE Utenti_Impostazioni_FiltroMono.Piva_SuperUser = @pivaSuperUser");
                stbQuery.AppendLine(" AND   Utenti_Impostazioni_FiltroMono.UserName = @utenteUserName");
                stbQuery.AppendLine(" AND   Utenti_Impostazioni_FiltroMono.Impostazione_Cod = @impostazioneCod");
                stbQuery.AppendLine(" AND   Cultivar.Veg_Cod = @vegCod");
                stbQuery.AppendLine("   ) > 0 ");

                stbQuery.AppendLine("       SELECT      SpecieVegetali.Veg_des,");
                stbQuery.AppendLine("       Cultivar.Cul_Cod, Cultivar.Cul_Cod_Aux, Cultivar.Cul_Cod_ORACOLO, Cultivar.Cul_Cod_CRPV, Cultivar.Cul_Des, Cultivar.Veg_Cod, ");
                stbQuery.AppendLine("       Cultivar.DATA_AGG, Cultivar.inviato, Cultivar.datainvio, Cultivar.Data_Creazione, Cultivar.Data_Modifica, Cultivar.Username_Creazione, ");
                stbQuery.AppendLine("       Cultivar.Username_Modifica, Cultivar.Validita_Inizio, Cultivar.Validita_Fine, Cultivar.Fatto");
                stbQuery.AppendLine("       FROM        Cultivar ");
                stbQuery.AppendLine("       INNER JOIN  SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ");
                stbQuery.AppendLine("       INNER JOIN  Utenti_Impostazioni_FiltroMono ");
                stbQuery.AppendLine("                   ON Cultivar.Cul_cod = Utenti_Impostazioni_FiltroMono.ID_0 ");
                stbQuery.AppendLine("       WHERE   Cultivar.Validita_Inizio <= @dtFine");
                stbQuery.AppendLine("       AND     Cultivar.Validita_Fine >=@dtInizio");
                stbQuery.AppendLine("       AND     Utenti_Impostazioni_FiltroMono.Piva_SuperUser = @pivaSuperUser");
                stbQuery.AppendLine("       AND     Utenti_Impostazioni_FiltroMono.UserName = @utenteUserName");
                stbQuery.AppendLine("       AND     Utenti_Impostazioni_FiltroMono.Impostazione_Cod = @impostazioneCod");
                stbQuery.AppendLine("       AND     Cultivar.Veg_Cod = @vegCod");

                if (filtraPer_Cul_Cod)
                    stbQuery.AppendLine("       AND     Cultivar.Cul_Cod = @culCod");

                if (filtraPer_Cerca_CulDes)
                    stbQuery.AppendLine("       AND     Cultivar.Cul_Des LIKE CONCAT('%', @cercaCulDes, '%')");

                stbQuery.AppendLine("       ORDER BY Veg_Des, Cul_Des ");

                stbQuery.AppendLine(" ELSE ");
                stbQuery.AppendLine(" SELECT       SpecieVegetali.Veg_des, ");
                stbQuery.AppendLine("              Cultivar.Cul_Cod, Cultivar.Cul_Cod_Aux, Cultivar.Cul_Cod_ORACOLO, Cultivar.Cul_Cod_CRPV, Cultivar.Cul_Des, Cultivar.Veg_Cod, ");
                stbQuery.AppendLine("              Cultivar.DATA_AGG, Cultivar.inviato, Cultivar.datainvio, Cultivar.Data_Creazione, Cultivar.Data_Modifica, Cultivar.Username_Creazione, ");
                stbQuery.AppendLine("              Cultivar.Username_Modifica, Cultivar.Validita_Inizio, Cultivar.Validita_Fine, Cultivar.Fatto");
                stbQuery.AppendLine(" FROM         Cultivar ");
                stbQuery.AppendLine(" INNER JOIN   SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ");
                stbQuery.AppendLine(" WHERE        Cultivar.Validita_Inizio <= @dtFine");
                stbQuery.AppendLine(" AND          Cultivar.Validita_Fine >=@dtInizio");
                stbQuery.AppendLine(" AND          Cultivar.Veg_Cod = @vegCod");

                if (filtraPer_Cul_Cod)
                    stbQuery.AppendLine(" AND          Cultivar.Cul_Cod = @culCod");

                if (filtraPer_Cerca_CulDes)
                    stbQuery.AppendLine(" AND          Cultivar.Cul_Des LIKE CONCAT('%', @cercaCulDes, '%')");

                stbQuery.AppendLine(" ORDER BY     Veg_Des, Cul_Des ");

                parametriSql.Add("@pivaSuperUser", objParametriUtenti.PivaSuperUser);
                parametriSql.Add("@utenteUserName", objParametriUtenti.UtenteUsername);
                parametriSql.Add("@impostazioneCod", (int)Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_VARIETA);
                parametriSql.Add("@dtInizio", objParametriUtenti.FinestraTemporaleInizio);
                parametriSql.Add("@dtFine", objParametriUtenti.FinestraTemporaleFine);
                parametriSql.Add("@vegCod", vegCod);

                if (filtraPer_Cul_Cod)
                    parametriSql.Add("@culCod", Cul_Cod);

                if (filtraPer_Cerca_CulDes)
                    parametriSql.Add("@cercaCulDes", Cerca_CulDes);

                try
                {
                    risultatoIntermedio = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
                }
                catch (Exception ex)
                {
                    LogError(ex.Message, objParametriServer, ex);
                    throw;
                }

                if (primoElemento)
                {
                    risultatoFinale = risultatoIntermedio.Clone();
                    primoElemento = false;
                }

                foreach (DataRow row in risultatoIntermedio.Rows)
                {
                    risultatoFinale.ImportRow(row);
                }

            }
            return risultatoFinale;
        }

        /// <summary>
        /// Restituisce true se per il gru cod selezionato ci sono dei filtri a livello di specie
        /// </summary>
        /// <param name="gruCod"></param>
        /// <param name="objParametriUtenti"></param>
        /// <param name="objParametriServer"></param>
        /// <returns></returns>
        private async Task<bool> FiltriALivelloDiSpeciePerGruCodAsync(int gruCod, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            stbQuery.AppendLine(" SELECT COUNT(*)  AS ImpostazioniCount");
            stbQuery.AppendLine(" FROM          SpecieVegetali ");
            stbQuery.AppendLine(" INNER JOIN    Utenti_Impostazioni_FiltroMono ");
            stbQuery.AppendLine("               ON SpecieVegetali.veg_cod = Utenti_Impostazioni_FiltroMono.ID_0 ");
            stbQuery.AppendLine(" WHERE         Utenti_Impostazioni_FiltroMono.Piva_SuperUser = @pivaSuperUser");
            stbQuery.AppendLine(" AND           Utenti_Impostazioni_FiltroMono.UserName = @utenteUserName");
            stbQuery.AppendLine(" AND           Utenti_Impostazioni_FiltroMono.Impostazione_Cod = @impostazioneCod");
            stbQuery.AppendLine(" AND           SpecieVegetali.Gru_Cod = @gruCod");

            parametriSql.Add("@pivaSuperUser", objParametriUtenti.PivaSuperUser);
            parametriSql.Add("@utenteUserName", objParametriUtenti.UtenteUsername);
            parametriSql.Add("@impostazioneCod", TipiEnumerativi.Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI);
            parametriSql.Add("@gruCod", gruCod);

            try
            {
                result = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            var impostazioniCount = (int)result.Rows[0]["ImpostazioniCount"];
            return impostazioniCount > 0;
        }

        /// <summary>
        /// Restituisce una lista di veg_cod visibili dall'utente per il gru_cod selezionato
        /// </summary>
        /// <param name="gruCod"></param>
        /// <param name="objParametriUtenti"></param>
        /// <param name="objParametriServer"></param>
        /// <returns></returns>
        private async Task<List<int>> GetVegCodVisibiliPerGruCodAsync(int gruCod, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;
            var vegCodList = new List<int>();

            //devo leggere le specie che posso vedere per questo gru cod
            stbQuery.AppendLine("       SELECT   SpecieVegetali.Veg_Cod");
            stbQuery.AppendLine("       FROM     SpecieVegetali ");
            stbQuery.AppendLine("                INNER JOIN Utenti_Impostazioni_FiltroMono ");
            stbQuery.AppendLine("                ON SpecieVegetali.veg_cod = Utenti_Impostazioni_FiltroMono.ID_0 ");
            stbQuery.AppendLine("       WHERE    SpecieVegetali.Validita_Inizio <= @dtFine");
            stbQuery.AppendLine("       AND      SpecieVegetali.Validita_Fine >= @dtInizio");
            //le specie vegetali che il Gias non deve caricare hanno gru_cod = -1
            stbQuery.AppendLine("       AND      SpecieVegetali.Gru_Cod <> -1 ");
            stbQuery.AppendLine("       AND      Utenti_Impostazioni_FiltroMono.Piva_SuperUser = @pivaSuperUser");
            stbQuery.AppendLine("       AND      Utenti_Impostazioni_FiltroMono.UserName = @utenteUserName");
            stbQuery.AppendLine("       AND      Utenti_Impostazioni_FiltroMono.Impostazione_Cod = @impostazioneCod");
            stbQuery.AppendLine("       AND      SpecieVegetali.Gru_Cod = @gruCod");

            parametriSql.Add("@pivaSuperUser", objParametriUtenti.PivaSuperUser);
            parametriSql.Add("@utenteUserName", objParametriUtenti.UtenteUsername);
            parametriSql.Add("@impostazioneCod", TipiEnumerativi.Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI);
            parametriSql.Add("@gruCod", gruCod);
            parametriSql.Add("@dtInizio", objParametriUtenti.FinestraTemporaleInizio);
            parametriSql.Add("@dtFine", objParametriUtenti.FinestraTemporaleFine);

            try
            {
                result = await GetDataProvider(objParametriUtenti).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            foreach (DataRow row in result.Rows)
            {
                vegCodList.Add((int)row["Veg_Cod"]);
            }
            return vegCodList;
        }
    }
}
