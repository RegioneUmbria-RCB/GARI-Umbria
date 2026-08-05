
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Documentale.DAL.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OutData.DataExchange;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Numerics;
using System.Text;
using static AgronicaNetCore.Base.Constants.CostantiPersonalizzate;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AgronicaNetCore.Documentale.DAL.DataLayer
{
    public class ExportDocumenti : BaseDALExportDocumenti, IExportDocumenti
    {
        public ExportDocumenti(IServiceProvider serviceProvider, IStringLocalizer<Messages> localizer) : base(serviceProvider, localizer)
        {
        }

        public async Task<DataTable> LeggiDocumentiExportAsync(string CUAA, int Tipologia_Cod, DateTime DataRiferimento, AgronicaCoreParametriServer objParametriServer)
        {

            Dictionary<string, object> parSql = new();
            var stbQuery = new StringBuilder();

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     Alert_Elenco.ID_Elenco ");
            stbQuery.AppendLine("   , Alert_Tipologia.ID_Tipologia ");
            stbQuery.AppendLine("   , CASE WHEN Alert_Elenco.Data_Scadenza IS NULL THEN CAST('2100-12-31' AS Date) ELSE Alert_Elenco.Data_Scadenza END AS Data_Scadenza");
            stbQuery.AppendLine("   , Allegati_Documenti.Allegati_Documenti_NomeFile ");
            stbQuery.AppendLine("   , Alert_Elenco.Descrizione_Scadenza AS Descrizione ");
            stbQuery.AppendLine("   , Allegati_Documenti.File_Allegato_DB ");
            stbQuery.AppendLine("   , ISNULL(CategorieDocumenti.Sottocartella, '') AS Sottocartella ");
            stbQuery.AppendLine("   , 0 AS Cancellato ");
            stbQuery.AppendLine(" FROM Alert_Elenco ");
            stbQuery.AppendLine(" JOIN Alert_Entita ON Alert_Elenco.ID_Alert_Entita = Alert_Entita.Id_Alert_Entita ");
            stbQuery.AppendLine(" JOIN Allegati_Documenti ON ");
            stbQuery.AppendLine("     Alert_Entita.Piva = Allegati_Documenti.Allegati_Documenti_Piva ");
            stbQuery.AppendLine(" AND Allegati_Documenti.Allegati_Documenti_Cod = Alert_Entita.Allegati_Documenti_Cod ");
            stbQuery.AppendLine(" JOIN Imprese_Codici ON ");
            stbQuery.AppendLine("     Allegati_Documenti.Allegati_Documenti_Piva = Imprese_Codici.Piva ");
            stbQuery.AppendLine($" AND id_cod = {((int)Enum_CodiciAnagrafe.CodiceCUAA).ToString()} AND val_cod = @CUAA ");
            stbQuery.AppendLine(" INNER JOIN Alert_Tipologia ON ");
            stbQuery.AppendLine("     Alert_Tipologia.ID_Tipologia = Alert_Elenco.ID_Tipologia ");
            stbQuery.AppendLine(" LEFT JOIN CategorieDocumenti ON ");
            stbQuery.AppendLine("     Alert_Tipologia.Cat_Cod = CategorieDocumenti.Cat_Cod ");

            stbQuery.AppendLine(" WHERE 1 = 1 ");

            if (Tipologia_Cod != 0)
            {
                stbQuery.AppendLine(" AND Alert_Elenco.ID_Tipologia = @Tipologia_Cod");
            }

            if (DataRiferimento != DateTime.Parse(AGRODATAINIZIO))
            {
                stbQuery.AppendLine(" AND Alert_Elenco.Data_Modifica >= @DataRiferimento ");
            }
 
            stbQuery.AppendLine("");

            stbQuery.AppendLine("UNION ");

            stbQuery.AppendLine("");

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("    IsNull(ID_Elenco,0) As ID_Elenco ");
            stbQuery.AppendLine("   ,IsNull(ID_Tipologia,0) as ID_Tipologia");
            stbQuery.AppendLine("   , NULL AS Data_Scadenza ");
            stbQuery.AppendLine("   , '' AS Allegati_Documenti_NomeFile ");
            stbQuery.AppendLine("   , '' AS Descrizione ");
            stbQuery.AppendLine("   , NULL AS File_Allegato_DB ");
            stbQuery.AppendLine("   , '' AS Sottocartella ");
            stbQuery.AppendLine("   , 1 AS Cancellato ");
            stbQuery.AppendLine(" FROM Alert_Log ");
            stbQuery.AppendLine(" JOIN Imprese_Codici ON ");
            stbQuery.AppendLine("     Alert_Log.Piva = Imprese_Codici.Piva ");
            stbQuery.AppendLine($" AND id_cod = {((int)Enum_CodiciAnagrafe.CodiceCUAA).ToString()} AND val_cod = @CUAA ");

            stbQuery.AppendLine($"WHERE Alert_Log.Tipo_Operazione = {((int)enum_TipoOperazioneDB.Cancellazione).ToString() } ");

            if (Tipologia_Cod != 0)
            {
                stbQuery.AppendLine(" AND Alert_Log.ID_Tipologia = @Tipologia_Cod");
            }

            if (DataRiferimento != DateTime.Parse(AGRODATAINIZIO))
            {
                stbQuery.AppendLine(" AND Alert_Log.Data_Creazione >= @DataRiferimento ");
            }


            if (Tipologia_Cod != 0)
            {
                parSql.Add("@Tipologia_Cod", Tipologia_Cod);
            }

            if (DataRiferimento != DateTime.Parse(AGRODATAINIZIO))
            {
                parSql.Add("@DataRiferimento", DataRiferimento);
            }

            parSql.Add("@CUAA", CUAA);


            return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql);

        }

        public async Task<DataTable> LeggiMetadatiExportAsync(List<int> IdDocumentList, AgronicaCoreParametriServer objParametriServer)
        {

            Dictionary<string, object> parSql = new();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();
            var stbQuery = new StringBuilder();


            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     Alert_Indice.TitoloIndice, Alert_Entitaxindici.Valore_Des, Alert_Elenco.ID_Elenco ");           
            stbQuery.AppendLine(" FROM Alert_Elenco ");
            stbQuery.AppendLine(" JOIN Alert_Entita ON Alert_Elenco.ID_Alert_Entita = Alert_Entita.Id_Alert_Entita ");
            stbQuery.AppendLine(" JOIN Alert_EntitaxIndici ON ");
            stbQuery.AppendLine("     Alert_Entita.ID_Alert_Entita = Alert_EntitaxIndici.ID_Alert_Entita ");
            stbQuery.AppendLine(" JOIN Alert_Indice ON ");
            stbQuery.AppendLine("     Alert_Indice.ID_Indice = Alert_EntitaxIndici.ID_Indice ");
            stbQuery.AppendLine($" AND TipoCampo = {((int)enum_TipoIndiceDocumentale.Libera_Imputazione).ToString()} --per ora prendiamo solo gli indici che non hanno bisogno di decodifica ");
            stbQuery.AppendLine(" AND Alert_EntitaxIndici.ID_Indice > 0 -- Escludiamo gli indici riservati");

            stbQuery.AppendLine(" WHERE 1 = 1");

            if (IdDocumentList.Any())
            {
                parSqlIn.Add("@DocumentList", FormatClauseIn(IdDocumentList));
                stbQuery.AppendLine(" AND Alert_Elenco.ID_Elenco IN (@DocumentList) ");
            }            
       
            return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);

        }

        public async Task<DataTable> LeggiDocumentiAnalisiPDCExportAsync(string CUAA, DateTime DataRiferimento, AgronicaCoreParametriServer objParametriServer)
        {
            Dictionary<string, object> parSql = new();
            var stbQuery = new StringBuilder();

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     Allegati_Documenti.Allegati_Documenti_Cod AS ID_Documento ");
            stbQuery.AppendLine($"   , {CostantiPersonalizzate.TIPOLOGIA_ANALISI_PDC} AS ID_Tipologia ");
            stbQuery.AppendLine("   , CAST('2100-12-31' AS Date) AS Data_Scadenza");
            stbQuery.AppendLine("   , Allegati_Documenti.Allegati_Documenti_NomeFile ");
            stbQuery.AppendLine("   , Analisi_Testata.Analisi_Testata_Des AS Descrizione ");
            stbQuery.AppendLine("   , Allegati_Documenti.File_Allegato_DB ");
            stbQuery.AppendLine($"   , '{CostantiPersonalizzate.CARTELLA_ANALISI_PDC}' AS Sottocartella ");
            stbQuery.AppendLine("   , Analisi_Testata.Analisi_Testata_Des AS Codice_Analisi ");
            stbQuery.AppendLine("   , CASE WHEN Analisi_Testata.Analisi_Testata_Data_Fine IS NULL THEN CAST('2100-12-31' AS Date) ELSE Analisi_Testata.Analisi_Testata_Data_Fine END AS Data_Fine_Analisi ");
            stbQuery.AppendLine("   , PDC_Analisi.Analisi_Testata_Cod AS ID_Analisi ");
            stbQuery.AppendLine("   , PDC_Analisi.PDC_Stato_Pubblicazione AS Stato_Pubblicazione ");
            stbQuery.AppendLine("   , Imprese_Codici.val_cod AS CUAA ");
            // stbQuery.AppendLine("   , PDC_Testata.PDC_Testata_Des ");
            // stbQuery.AppendLine("   , PDC_Dettagli.Piva ");
            // stbQuery.AppendLine("   , PDC_Dettagli.Rag_Soc ");
            // stbQuery.AppendLine("   , PDC_Dettagli.App_Nome ");
            // stbQuery.AppendLine("   , PDC_Dettagli.Veg_Cod ");
            // stbQuery.AppendLine("   , PDC_Dettagli.Cul_Cod ");
            stbQuery.AppendLine("   , ISNULL(SpecieVegetali.Veg_Des, '') AS Specie ");
            stbQuery.AppendLine("   , ISNULL(Cultivar.Cul_Des, '') AS Varieta ");
            stbQuery.AppendLine("   , 0 AS Cancellato ");
            stbQuery.AppendLine(" FROM Allegati_EntitaxDocumenti ");
            stbQuery.AppendLine(" JOIN Allegati_Documenti ON Allegati_Documenti.Allegati_Documenti_Cod = Allegati_EntitaxDocumenti.Allegati_Documenti_Cod ");
            stbQuery.AppendLine(" JOIN Analisi_Testata ON Analisi_Testata.Analisi_Testata_Cod = Allegati_EntitaxDocumenti.Analisi_Testata_Cod ");
            stbQuery.AppendLine(" JOIN PDC_Analisi ON PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod ");
            stbQuery.AppendLine(" JOIN PDC_Campioni ON PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione ");
            stbQuery.AppendLine(" JOIN PDC_Dettagli ON PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli ");
            stbQuery.AppendLine(" JOIN PDC_Testata ON PDC_Testata.Id_PDC_Testata = PDC_Dettagli.Id_PDC_Testata ");
            stbQuery.AppendLine(" JOIN Imprese_Codici ON PDC_Dettagli.Piva = Imprese_Codici.Piva ");
            stbQuery.AppendLine($" AND id_cod = {((int)Enum_CodiciAnagrafe.CodiceCUAA).ToString()} AND val_cod = @CUAA ");
            stbQuery.AppendLine(" LEFT JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = PDC_Dettagli.Veg_Cod ");
            stbQuery.AppendLine(" LEFT JOIN Cultivar ON Cultivar.Cul_Cod = PDC_Dettagli.Cul_Cod ");
            
            stbQuery.AppendLine(" WHERE PDC_Analisi.PDC_Stato_Pubblicazione IN (1,2,3) ");
        
            if (DataRiferimento != DateTime.Parse(AGRODATAINIZIO))
            {
                stbQuery.AppendLine(" AND (Allegati_Documenti.Data_Modifica >= @DataRiferimento ");
                stbQuery.AppendLine(" OR PDC_Analisi.Data_Modifica >= @DataRiferimento) ");
            }

            stbQuery.AppendLine("");
            stbQuery.AppendLine("UNION ");
            stbQuery.AppendLine("");
            stbQuery.AppendLine(" SELECT DISTINCT ID_Documento ");
            stbQuery.AppendLine($"   , {CostantiPersonalizzate.TIPOLOGIA_ANALISI_PDC} AS ID_Tipologia ");
            stbQuery.AppendLine(" , NULL AS Data_Scadenza ");
            stbQuery.AppendLine(" , '' AS Allegati_Documenti_NomeFile ");
            stbQuery.AppendLine(" , '' AS Descrizione ");
            stbQuery.AppendLine(" , NULL AS File_Allegato_DB ");
            stbQuery.AppendLine(" , '' AS Sottocartella ");
            stbQuery.AppendLine(" , '' AS Codice_Analisi ");
            stbQuery.AppendLine(" , NULL AS Data_Fine_Analisi ");
            stbQuery.AppendLine(" , ID_Analisi ");
            stbQuery.AppendLine(" , Stato AS Stato_Pubblicazione ");
            stbQuery.AppendLine(" , CUAA ");
            stbQuery.AppendLine(" , '' AS Specie ");
            stbQuery.AppendLine(" , '' AS Varieta ");
            stbQuery.AppendLine(" , 1 AS Cancellato ");
            stbQuery.AppendLine(" FROM DocComplianceMarketAccessInviatiAlPortale a ");
            stbQuery.AppendLine(" LEFT JOIN Allegati_EntitaxDocumenti b ON a.ID_Documento = b.Allegati_Documenti_Cod AND a.ID_Analisi = b.Analisi_Testata_Cod ");
            stbQuery.AppendLine(" LEFT JOIN Analisi_Testata c ON a.ID_Analisi = c.Analisi_Testata_Cod ");
            stbQuery.AppendLine($" WHERE a.stato={(int)Enum_PDC_Stato_Pubblicazione.Pubblicata} ");
            stbQuery.AppendLine(" AND a.CUAA = @CUAA AND a.Data_Modifica >= @DataRiferimento ");
            stbQuery.AppendLine(" AND (b.Allegati_Documenti_Cod IS NULL OR c.Analisi_Testata_Cod IS NULL) ");

            if (DataRiferimento != DateTime.Parse(AGRODATAINIZIO))
            {
                parSql.Add("@DataRiferimento", DataRiferimento);
            }

            parSql.Add("@CUAA", CUAA);

            return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql);
        }


        public async Task<bool> AggiornaPubblicazioneAnalisiPDCAsync(int IdAnalisi, int Stato, AgronicaCoreParametriServer objParametriServer)
        {
            var expandoObj = new ExpandoObject();
            var stbQuery = new StringBuilder();

            stbQuery.AppendLine(" UPDATE PDC_Analisi ");
            stbQuery.AppendLine(" SET ");
            stbQuery.AppendLine("     PDC_Stato_Pubblicazione = @Stato ");
            stbQuery.AppendLine("   , Data_Modifica =  @Data_Modifica ");
            stbQuery.AppendLine("   , Username_Modifica =  @Username_Modifica ");
            stbQuery.AppendLine(" WHERE Analisi_Testata_Cod = @Id_Analisi ");

            expandoObj.TryAdd("@Id_Analisi", IdAnalisi);
            expandoObj.TryAdd("@Stato", Stato);
            expandoObj.TryAdd("@Data_Modifica", DateTime.Now);
            expandoObj.TryAdd("@Username_Modifica", objParametriServer.UsernameOperazione);

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> AggiornaPubblicazioneAnalisiPDCAsync(string CUAA, AgronicaCoreParametriServer objParametriServer)
        {
            var expandoObj = new ExpandoObject();
            var stbQuery = new StringBuilder();

            stbQuery.AppendLine(" UPDATE PDC_Analisi ");
            stbQuery.AppendLine(" SET ");
            stbQuery.AppendLine("     PDC_Analisi.PDC_Stato_Pubblicazione = a.Stato ");
            stbQuery.AppendLine("   , Data_Modifica =  @Data_Modifica ");
            stbQuery.AppendLine("   , Username_Modifica =  @Username_Modifica ");
            stbQuery.AppendLine(" FROM PDC_Analisi ");
            stbQuery.AppendLine(" INNER JOIN ( ");
            stbQuery.AppendLine("   SELECT ID_Analisi, MAX(Stato) AS Stato ");
            stbQuery.AppendLine("   FROM DocComplianceMarketAccessInviatiAlPortale ");
            stbQuery.AppendLine("   WHERE CUAA = @CUAA GROUP BY ID_Analisi) a ");
            stbQuery.AppendLine(" ON PDC_Analisi.Analisi_Testata_Cod = a.ID_Analisi ");

            expandoObj.TryAdd("@CUAA", CUAA);
            expandoObj.TryAdd("@Data_Modifica", DateTime.Now);
            expandoObj.TryAdd("@Username_Modifica", objParametriServer.UsernameOperazione);

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> ScriviExportDocumentiAnalisiPDCAsync(string CUAA, int IdAnalisi, int IdDocumento, int Stato, AgronicaCoreParametriServer objParametriServer)
        {
            var expandoObj = new ExpandoObject();
            var stbQuery = new StringBuilder();
            
            stbQuery.AppendLine("IF EXISTS(SELECT 1")
                .AppendLine(" FROM DocComplianceMarketAccessInviatiAlPortale")
                .AppendLine(" WHERE CUAA = @CUAA")
                .AppendLine("   AND (@ID_Analisi = 0 OR ID_Analisi = @ID_Analisi)")
                .AppendLine("   AND ID_Documento = @ID_Documento)")
                .AppendLine("BEGIN")
                .AppendLine(" UPDATE DocComplianceMarketAccessInviatiAlPortale")
                .AppendLine(" SET Stato = @Stato ")
                .AppendLine("   , Data_Modifica =  @Data_Modifica ")
                .AppendLine("   , Username_Modifica =  @Username_Modifica ")
                .AppendLine(" WHERE CUAA = @CUAA")
                .AppendLine("   AND (@ID_Analisi = 0 OR ID_Analisi = @ID_Analisi)")
                .AppendLine("   AND ID_Documento = @ID_Documento")
                .AppendLine("END")
                .AppendLine("ELSE")
                .AppendLine("BEGIN")
                .AppendLine(" INSERT INTO DocComplianceMarketAccessInviatiAlPortale ( ")
                .AppendLine("     [CUAA] ")
                .AppendLine("   , [ID_Analisi] ")
                .AppendLine("   , [ID_Documento] ")
                .AppendLine("   , [Stato] ")
                .AppendLine("   , [inviato] ")
                .AppendLine("   , [datainvio] ")
                .AppendLine("   , [Data_Creazione] ")
                .AppendLine("   , [Data_Modifica] ")
                .AppendLine("   , [Username_Creazione] ")
                .AppendLine("   , [Username_Modifica] ")
                .AppendLine("   , [Validita_Inizio] ")
                .AppendLine("   , [Validita_Fine] ")
                .AppendLine(" )")
                .AppendLine(" VALUES (")
                .AppendLine("     @CUAA ")
                .AppendLine("   , @ID_Analisi ")
                .AppendLine("   , @ID_Documento ")
                .AppendLine("   , @Stato ")
                .AppendLine("   , 0 ")
                .AppendLine("   , NULL ")
                .AppendLine("   , @Data_Creazione ")
                .AppendLine("   , @Data_Modifica ")
                .AppendLine("   , @Username_Creazione ")
                .AppendLine("   , @Username_Modifica ")
                .AppendLine("   , @Validita_Inizio ")
                .AppendLine("   , @Validita_Fine ")
                .AppendLine(" ) ")
                .AppendLine("END");

            expandoObj.TryAdd("@CUAA", CUAA);
            expandoObj.TryAdd("@ID_Analisi", IdAnalisi);
            expandoObj.TryAdd("@ID_Documento", IdDocumento);
            expandoObj.TryAdd("@Stato", Stato);
            expandoObj.TryAdd("@Data_Creazione", DateTime.Now);
            expandoObj.TryAdd("@Data_Modifica", DateTime.Now);
            expandoObj.TryAdd("@Username_Creazione", objParametriServer.UsernameOperazione);
            expandoObj.TryAdd("@Username_Modifica", objParametriServer.UsernameOperazione);
            expandoObj.TryAdd("@Validita_Inizio", DateTime.Parse(AGRODATAINIZIO));
            expandoObj.TryAdd("@Validita_Fine", DateTime.Parse(AGRODATAFINE));

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> AggiornaExportDocumentiAnalisiPDCAsync(string CUAA, List<int> IdDocumenti, int Stato, AgronicaCoreParametriServer objParametriServer)
        {
            var expandoObj = new ExpandoObject();
            var stbQuery = new StringBuilder();
            var paramNames = IdDocumenti.Select((id, i) => $"@id{i}").ToList();
            
            stbQuery
                .AppendLine(" UPDATE DocComplianceMarketAccessInviatiAlPortale")
                .AppendLine(" SET Stato = @Stato ")
                .AppendLine("   , Data_Modifica =  @Data_Modifica ")
                .AppendLine("   , Username_Modifica =  @Username_Modifica ")
                .AppendLine(" WHERE CUAA = @CUAA")
                .AppendLine($"   AND ID_Documento IN ({string.Join(",", paramNames)})");

            expandoObj.TryAdd("@CUAA", CUAA);
            for (int i = 0; i < IdDocumenti.Count; i++) expandoObj.TryAdd(paramNames[i], IdDocumenti[i]);
            expandoObj.TryAdd("@Stato", Stato);
            expandoObj.TryAdd("@Data_Modifica", DateTime.Now);
            expandoObj.TryAdd("@Username_Modifica", objParametriServer.UsernameOperazione);
            
            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
