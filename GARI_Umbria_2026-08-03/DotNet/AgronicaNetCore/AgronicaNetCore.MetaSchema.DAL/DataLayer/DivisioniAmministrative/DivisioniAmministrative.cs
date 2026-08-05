using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.DivisioniAmministrative
{
    public class DivisioniAmministrative : BaseDALMetaschema, IDivisioniAmministrative
    {
        public DivisioniAmministrative(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> LeggiStatiAsync(string codice, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            var filtroSulCodice = !string.IsNullOrEmpty(codice);
            stbQuery.AppendLine(" SELECT      Codice, Descrizione, inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, ");
            stbQuery.AppendLine("             Validita_Inizio, Validita_Fine, Entrate_Unico_Elenco_paesi_territori_esteri_COD, Codice_Numerico, Codice_Alpha_3, Gestione_Gerarchia_Geografica ");
            stbQuery.AppendLine(" FROM        ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 ");
            stbQuery.AppendLine(" WHERE       1 = 1 ");

            if (filtroSulCodice)
            {
                if (codice.Length >= 3 && codice.Substring(0, 3).ToUpper() == "ITA")
                {
                    codice = "IT";
                }
                stbQuery.AppendLine(" AND         Codice = @codice");
                parametriSql.Add("@codice", codice);
            }

            stbQuery.AppendLine(" ORDER BY    Descrizione");

            try
            {
                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }

        public async Task<DataTable> LeggiRegioniAsync(LeggiRegioni_IN leggiRegioniIN, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            var filtraPerRegione = !string.IsNullOrEmpty(leggiRegioniIN.Regione);
            var filtraPerStato = leggiRegioniIN.Stati_List.Any();

            stbQuery.AppendLine(" SELECT REG, Regione_Des, inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine, Stato_Country ");
            stbQuery.AppendLine(" FROM  Lista_Regioni ");
            stbQuery.AppendLine(" WHERE Validita_inizio <= @dtFine");
            stbQuery.AppendLine(" AND   Validita_Fine >= @dtInizio");

            if (filtraPerRegione)
                stbQuery.AppendLine(" AND   REG = @regione");

            if (filtraPerStato)
            {
                stbQuery.Append(" AND   Stato_Country IN (");

                for (int i = 0; i < leggiRegioniIN.Stati_List.Count; i++)
                {
                    stbQuery.Append($"@stato{i}");

                    if (i != leggiRegioniIN.Stati_List.Count - 1)
                    {
                        //aggiungiamo la virgola per il prossimo parametro
                        stbQuery.Append(",");
                    }
                }

                stbQuery.AppendLine(")");
            }

            if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati)
                stbQuery.AppendLine(" AND   Inviato >= 0 ");
            else if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati)
                stbQuery.AppendLine(" AND   Inviato = -1 ");
            else if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti)
            {
                //nessun filtro da aggiungere
            }
            else
                throw new Exception("Parametro non corretto nella query (FlagVisibilita)");

            stbQuery.AppendLine(" ORDER BY Regione_Des");

            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);
            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);

            if (filtraPerRegione)
                parametriSql.Add("@regione", leggiRegioniIN.Regione);

            if (filtraPerStato)
            {
                for (int i = 0; i < leggiRegioniIN.Stati_List.Count; i++)
                {
                    if (leggiRegioniIN.Stati_List[i].Length >= 3 && leggiRegioniIN.Stati_List[i].Substring(0, 3).ToUpper() == "ITA")
                    {
                        leggiRegioniIN.Stati_List[i] = "IT";
                    }
                    parametriSql.Add($"@stato{i}", leggiRegioniIN.Stati_List[i]);
                }
            }

            try
            {
                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }

        public async Task<DataTable> LeggiProvinceAsync(LeggiProvince_IN leggiProvinceIN, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            var filtraPerReg = leggiProvinceIN.Reg_List.Any();
            var filtraPerStato = !string.IsNullOrEmpty(leggiProvinceIN.Stato_Country);

            stbQuery.AppendLine(" SELECT      Lista_Province.SIGLA, Lista_Province.REG, Lista_Province.PROV, Lista_Province.COM, Lista_Province.PROVINCIA,");
            stbQuery.AppendLine("             Lista_Regioni.Regione_Des, Lista_Regioni.Stato_Country ");
            stbQuery.AppendLine(" FROM        Lista_Province ");
            stbQuery.AppendLine(" INNER JOIN  Lista_Regioni ");
            stbQuery.AppendLine(" ON          Lista_Province.REG = Lista_Regioni.REG ");
            stbQuery.AppendLine(" AND         Lista_Province.Stato_Country = Lista_Regioni.Stato_Country");

            if (filtraPerReg)
            {
                stbQuery.Append(" AND         Lista_Province.REG IN (");

                for (int i = 0; i < leggiProvinceIN.Reg_List.Count; i++)
                {
                    stbQuery.Append($"@regione{i}");

                    if (i != leggiProvinceIN.Reg_List.Count - 1)
                    {
                        //aggiungiamo la virgola per il prossimo parametro
                        stbQuery.Append(",");
                    }
                }

                stbQuery.AppendLine(")");
            }

            if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati)
                stbQuery.AppendLine(" AND         Lista_Province.Inviato >= 0 ");
            else if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati)
                stbQuery.AppendLine(" AND         Lista_Province.Inviato = -1 ");
            else if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti)
            {
                //nessun filtro da aggiungere
            }
            else
                throw new Exception("Parametro non corretto nella query (FlagVisibilita)");

            stbQuery.AppendLine(" WHERE       Lista_Province.Validita_inizio <= @dtFine ");
            stbQuery.AppendLine(" AND         Lista_Province.Validita_Fine >= @dtInizio ");

            if (filtraPerStato)
            {
                if (leggiProvinceIN.Stato_Country.Length >= 3 && leggiProvinceIN.Stato_Country.Substring(0, 3).ToUpper() == "ITA")
                {
                    leggiProvinceIN.Stato_Country = "IT";
                }
                stbQuery.AppendLine(" AND         Upper(Lista_Province.Stato_Country) = @stato ");
            }

            stbQuery.AppendLine(" ORDER BY    Regione_Des , Provincia");

            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);
            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);

            if (filtraPerReg)
            {
                for (int i = 0; i < leggiProvinceIN.Reg_List.Count; i++)
                {
                    parametriSql.Add($"@regione{i}", leggiProvinceIN.Reg_List[i]);
                }
            }
            if (filtraPerStato)
                parametriSql.Add("@stato", leggiProvinceIN.Stato_Country.ToUpper());

            try
            {
                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }

        public async Task<DataTable> LeggiComuniAsync(LeggiComuni_IN leggiComuni_IN, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            DataTable result;

            var filtraPerPROV = leggiComuni_IN.PROV_List.Any();
            var filtraPerCOM_LOCALITA = !string.IsNullOrEmpty(leggiComuni_IN.COM_LOCALITA);
            var filtraPerCOM_PROVINCIA = !string.IsNullOrEmpty(leggiComuni_IN.COM_PROVINCIA);
            var filtraPerSIGLA_PROVINCIA = !string.IsNullOrEmpty(leggiComuni_IN.SIGLA_PROVINCIA);
            var filtraPerCAP = !string.IsNullOrEmpty(leggiComuni_IN.CAP);
            var filtraPerREG = !string.IsNullOrEmpty(leggiComuni_IN.REG);
            var filtraPerStrProvincia = !string.IsNullOrEmpty(leggiComuni_IN.Filtro_StrProvincia);
            var filtraPerStrComune = !string.IsNullOrEmpty(leggiComuni_IN.Filtro_StrComune);


            stbQuery.AppendLine(" SELECT      ISTAT.PROV, ISTAT.COM AS COM_LOCALITA, ISTAT.LOCALITA, ISTAT.CAP, Lista_Province.REG,");
            stbQuery.AppendLine("             Lista_Province.COM AS COM_PROVINCIA,  Lista_Province.PROVINCIA, Lista_Province.SIGLA  ");
            stbQuery.AppendLine(" FROM        ISTAT");
            stbQuery.AppendLine(" INNER JOIN  Lista_Province ON ISTAT.PROV = Lista_Province.PROV ");

            stbQuery.AppendLine(" WHERE       ISTAT.Validita_Inizio <= @dtFine");
            stbQuery.AppendLine(" AND         ISTAT.Validita_Fine >= @dtInizio");
            stbQuery.AppendLine(" AND         Lista_Province.Validita_Inizio <= @dtFine");
            stbQuery.AppendLine(" AND         Lista_Province.Validita_Fine >= @dtInizio");

            if (filtraPerPROV)
            {
                stbQuery.Append(" AND         Lista_Province.PROV IN (");

                for (int i = 0; i < leggiComuni_IN.PROV_List.Count; i++)
                {
                    stbQuery.Append($"@PROV{i}");

                    if (i != leggiComuni_IN.PROV_List.Count - 1)
                    {
                        //aggiungiamo la virgola per il prossimo parametro
                        stbQuery.Append(",");
                    }
                }
                stbQuery.AppendLine(")");
            }

            if (filtraPerCOM_LOCALITA)
                stbQuery.AppendLine(" AND        ISTAT.COM = @COM_LOCALITA");

            if (filtraPerCOM_PROVINCIA)
                stbQuery.AppendLine(" AND        Lista_Province.COM = @COM_PROVINCIA");

            if (filtraPerSIGLA_PROVINCIA)
                stbQuery.AppendLine(" AND        Lista_Province.SIGLA = @SIGLA_PROVINCIA");

            if (filtraPerCAP)
                stbQuery.AppendLine(" AND        ISTAT.CAP = @CAP");

            if (filtraPerREG)
                stbQuery.AppendLine(" AND        Lista_Province.REG = @REG");

            if (filtraPerStrProvincia)
                stbQuery.AppendLine(" AND        Lista_Province.PROVINCIA LIKE CONCAT('%', @filtroStrProvincia, '%')");

            if (filtraPerStrComune)
                stbQuery.AppendLine(" AND        ISTAT.LOCALITA LIKE CONCAT('%', @filtroStrComune, '%')");


            if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati)
                stbQuery.AppendLine(" AND        ISTAT.Inviato >= 0 ");
            else if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati)
                stbQuery.AppendLine(" AND        ISTAT.Inviato = -1 ");
            else if (objParametriServer.FlagVisibilita == AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti)
            {
                //nessun filtro da aggiungere
            }
            else
                throw new Exception("Parametro non corretto nella query (FlagVisibilita)");

            stbQuery.AppendLine(" ORDER BY   PROVINCIA, LOCALITA ");


            parametriSql.Add("@dtInizio", objParametriServer.FinestraTemporaleInizio);
            parametriSql.Add("@dtFine", objParametriServer.FinestraTemporaleFine);

            if (filtraPerPROV)
            {
                for (int i = 0; i < leggiComuni_IN.PROV_List.Count; i++)
                {
                    leggiComuni_IN.PROV_List[i] = leggiComuni_IN.PROV_List[i].Trim();
                    if (leggiComuni_IN.PROV_List[i].Length < 3)
                    {
                        if (leggiComuni_IN.PROV_List[i].Length == 1)
                            leggiComuni_IN.PROV_List[i] = "00" + leggiComuni_IN.PROV_List[i];
                        else if (leggiComuni_IN.PROV_List[i].Length == 2)
                            leggiComuni_IN.PROV_List[i] = "0" + leggiComuni_IN.PROV_List[i];
                    }
                    parametriSql.Add($"@PROV{i}", leggiComuni_IN.PROV_List[i]);
                }
            }

            if (filtraPerCOM_LOCALITA)
                parametriSql.Add("@COM_LOCALITA", leggiComuni_IN.COM_LOCALITA);

            if (filtraPerCOM_PROVINCIA)
                parametriSql.Add("@COM_PROVINCIA", leggiComuni_IN.COM_PROVINCIA);

            if (filtraPerSIGLA_PROVINCIA)
                parametriSql.Add("@SIGLA_PROVINCIA", leggiComuni_IN.SIGLA_PROVINCIA);

            if (filtraPerCAP)
                parametriSql.Add("@CAP", leggiComuni_IN.CAP);

            if (filtraPerREG)
                parametriSql.Add("@REG", leggiComuni_IN.REG);

            if (filtraPerStrProvincia)
                parametriSql.Add("@filtroStrProvincia", leggiComuni_IN.Filtro_StrProvincia);

            if (filtraPerStrComune)
                parametriSql.Add("@filtroStrComune", leggiComuni_IN.Filtro_StrComune);

            try
            {
                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }

    }
}
