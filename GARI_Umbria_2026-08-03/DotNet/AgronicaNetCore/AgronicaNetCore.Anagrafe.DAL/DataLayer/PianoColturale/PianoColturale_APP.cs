using System.Data;
using System.Text;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.PianoColturale
{
    public class PianoColturale_APP : BaseDALAnagrafe, IPianoColturale_APP
    {
        private const int CodiceKPin = 1287;
        private const int CodiceBlockName = 1288;
        private const int CodiceLayerElementiGrafici = 19;
        private const int CodiceDistintaChiusa = (int)Enum_CodiciAnagrafe.Distinta_Chiusa;
        private const int CodiceRiferimentoAlfanumericoAppezzamento = (int)
            Enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento;
        private const int CodiceCodiceImpianto = (int)Enum_CodiciAnagrafe.Codice_Impianto;

        public PianoColturale_APP(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        public async Task<DataTable> ReadAsync(
            string piva,
            DateTime data,
            AgronicaCoreParametriServer objParametriServer,
            bool leggiSoloAttivi = false,
            bool leggiAncheBloccati = false,
            bool generaStaticMap = false,
            bool modalitaDemetra = false,
            bool leggiCartografia = false
        )
        {
            if (string.IsNullOrEmpty(piva))
                throw new ArgumentException("Specificare la partita iva.");

            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery
                .AppendLine("SELECT DISTINCT")
                .AppendLine("    Reg_Impianti.PIVA")
                .AppendLine("    , Reg_Impianti.SA_COD")
                .AppendLine("    , Reg_Impianti.APPEZZA")
                .AppendLine("    , Reg_Impianti.ID_REG")
                .AppendLine("    , Reg_Impianti.Validita_Inizio")
                .AppendLine("    , Reg_Impianti.Validita_Fine")
                .AppendLine("    , Reg_Impianti.Sup_Imp")
                .AppendLine("    , ISNULL(Reg_Impianti.COP_COD, 0) AS Cop_Cod")
                .AppendLine("    , ISNULL(Copertura.Cop_Des, '') AS Copertura")
                .AppendLine("    , COALESCE(Reg_Impianti.COVER, 0) AS Cover")
                .AppendLine("    , ISNULL(Cultivar.Veg_Cod, 0) AS Veg_Cod")
                .AppendLine("    , Reg_Impianti.CUL_COD")
                .AppendLine("    , ISNULL(Cultivar.Cul_Des, '') AS Cul_Des")
                .AppendLine("    , ISNULL(Reg_Impianti.GRFI_COD, 0) AS Grfi_Cod")
                .AppendLine("    , ISNULL(GruppoFinalita.Grfi_Des, '') AS Grfi_Des")
                .AppendLine("    , COALESCE(Appezzamento.Blk_Flag, 0) AS Blk_Flag")
                .AppendLine(
                    "    , COALESCE(Appezzamento.Blk_Inizio_Data, CONVERT(DateTime, '1900/01/01', 120)) AS Blk_Inizio_Data"
                )
                .AppendLine(
                    "    , COALESCE(Appezzamento.Blk_Fine_Data, CONVERT(DateTime, '2100/12/31', 120)) AS Blk_Fine_Data"
                )
                .AppendLine(
                    "    , COALESCE(Appezzamento.Blk_Inizio_Username, '') AS Blk_Inizio_Username"
                )
                .AppendLine(
                    "    , COALESCE(Appezzamento.Blk_Fine_Username, '') AS Blk_Fine_Username"
                )
                .AppendLine("    , COALESCE(Appezzamento.Blk_Inizio_Note, '') AS Blk_Inizio_Note")
                .AppendLine("    , COALESCE(Appezzamento.Blk_Fine_Note, '') AS Blk_Fine_Note")
                .AppendLine("    , Appezzamento.Campo_Cod")
                .AppendLine("    , Appezzamento.SUP_APP")
                .AppendLine("    , Appezzamento.APP_NOME")
                .AppendLine(
                    "      + CASE WHEN ISNULL(Reg_Impianti_Codici_KPIN.val_cod, '') = '' THEN '' ELSE '  KPin: ' + Reg_Impianti_Codici_KPIN.val_cod END"
                )
                .AppendLine(
                    "      + CASE WHEN ISNULL(Reg_Impianti_Codici_BlockName.val_cod, '') = '' THEN '' ELSE '  BlockName: ' + Reg_Impianti_Codici_BlockName.val_cod END"
                )
                .AppendLine(
                    $"      + CASE WHEN ISNULL(Imprese_Progetti.data_inizio_prevista, '{CostantiPersonalizzate.AGRODATAINIZIO}') = '{CostantiPersonalizzate.AGRODATAINIZIO}' THEN '' ELSE '  Start: ' + Convert(VARCHAR, Imprese_Progetti.data_inizio_prevista, 103) END AS APP_NOME"
                )
                .AppendLine("    , Imprese_Progetti.Progetto_Cod")
                .AppendLine("    , Imprese_Progetti.Progetto_Nome AS Progetto")
                .AppendLine("    , Imprese_Progetti.Regolamento_Cod AS Regolamento")
                .AppendLine("    , ISNULL(Regolamenti.Reg_Des, '') AS Reg_Des")
                .AppendLine("    , Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Distinta")
                .AppendLine("    , Imprese_Progetti.Validita_Fine AS Validita_Fine_Distinta")
                .AppendLine("    , Imprese_Progetti.Produzione_Prevista")
                .AppendLine("    , Reg_Impianti.Validita_Inizio AS Validita_Inizio_Impianto")
                .AppendLine("    , Reg_Impianti.Validita_Fine AS Validita_Fine_Impianto")
                .AppendLine("    , Appezzamento.Validita_Inizio AS Validita_Inizio_Appezza")
                .AppendLine("    , Appezzamento.Validita_Fine AS Validita_Fine_Appezza")
                .AppendLine("    , ISNULL(Codici_Anagrafe.descrizione, '') AS Codici_Anagrafe_Des")
                .AppendLine("    , ISNULL(Codici_Anagrafe.codice, '') AS id_Cod")
                .AppendLine("    , ISNULL((SELECT campo_des FROM Campi")
                .AppendLine(
                    "        WHERE Appezzamento.PIVA = Campi.PIVA AND Appezzamento.sa_cod = Campi.sa_cod"
                )
                .AppendLine(
                    "        AND Appezzamento.Campo_Cod = Campi.Campo_Cod), '') AS Campo_Des"
                )
                .AppendLine("    , Imprese.rag_soc, Centri_Aziendali.sa_nome")
                .AppendLine("    , ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des")
                .AppendLine("    , ISNULL(ImpiantiIrrigazioni.Imp_Cod, 0) AS Imp_Cod")
                .AppendLine("    , ISNULL(ImpiantiIrrigazioni.Imp_Des, '') AS Imp_Des")
                .AppendLine(
                    "    , ISNULL(Reg_Impianti_Codici_Codice_Impianto.Val_Cod, '') AS Codici_Anagrafe_Impianto"
                )
                .AppendLine(
                    "    , ISNULL(Appezzamento_Codici.Val_Cod, '') AS Codici_Anagrafe_Appezzamento"
                )
                .AppendLine("    , ISNULL(Reg_Impianti.Agea_idColt, '') AS Agea_idColt");

            if (leggiCartografia)
                stbQuery.AppendLine(
                    "    , COALESCE(g.Poligono_GeoEntity_WKT, g.Poligono_GeoEntity.STAsText(), '') AS cartografia"
                );

            if (leggiCartografia || generaStaticMap)
                stbQuery
                    .AppendLine("    , (")
                    .AppendLine("         SELECT * FROM (SELECT StaticMap AS '*') Tbl")
                    .AppendLine("         FOR XML PATH('')")
                    .AppendLine("    ) AS StaticMapBase64String");

            if (generaStaticMap)
                stbQuery
                    .AppendLine("    , g.Entita_Cod")
                    .AppendLine("    , g.poligono_geoEntity.STAsText() AS geo");

            stbQuery
                .AppendLine("FROM Reg_Impianti")
                .AppendLine("    INNER JOIN Imprese ON Reg_Impianti.PIVA = Imprese.PIVA")
                .AppendLine("    INNER JOIN Centri_Aziendali")
                .AppendLine("        ON Reg_Impianti.PIVA = Centri_Aziendali.PIVA")
                .AppendLine("        AND Reg_Impianti.SA_COD = Centri_Aziendali.sa_cod")
                .AppendLine("    INNER JOIN Appezzamento")
                .AppendLine("        ON Reg_Impianti.PIVA = Appezzamento.PIVA")
                .AppendLine("        AND Reg_Impianti.SA_COD = Appezzamento.SA_COD")
                .AppendLine("        AND Reg_Impianti.APPEZZA = Appezzamento.APPEZZA")
                .AppendLine("    INNER JOIN Imprese_Progetti")
                .AppendLine("        ON Reg_Impianti.PIVA = Imprese_Progetti.Piva")
                .AppendLine("        AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod")
                .AppendLine("        AND Reg_Impianti.APPEZZA = Imprese_Progetti.Appezza")
                .AppendLine("        AND Reg_Impianti.ID_REG = Imprese_Progetti.Id_Reg")
                .AppendLine("    LEFT JOIN Reg_Impianti_Codici AS Reg_Impianti_Distinta")
                .AppendLine("        ON Reg_Impianti_Distinta.PIVA = Imprese_Progetti.PIVA")
                .AppendLine("        AND Reg_Impianti_Distinta.SA_COD = Imprese_Progetti.SA_COD")
                .AppendLine("        AND Reg_Impianti_Distinta.APPEZZA = Imprese_Progetti.APPEZZA")
                .AppendLine("        AND Reg_Impianti_Distinta.id_reg = Imprese_Progetti.id_reg")
                .AppendLine(
                    "        AND Reg_Impianti_Distinta.Progetto_Cod = Imprese_Progetti.Progetto_Cod"
                )
                .AppendLine($"        AND Reg_Impianti_Distinta.id_cod = {CodiceDistintaChiusa}")
                .AppendLine("    LEFT JOIN Appezzamento_Codici")
                .AppendLine("        ON Appezzamento_Codici.PIVA = Reg_Impianti.PIVA")
                .AppendLine("        AND Appezzamento_Codici.SA_COD = Reg_Impianti.SA_COD")
                .AppendLine("        AND Appezzamento_Codici.APPEZZA = Reg_Impianti.APPEZZA")
                .AppendLine(
                    $"        AND Appezzamento_Codici.id_cod = {CodiceRiferimentoAlfanumericoAppezzamento}"
                )
                .AppendLine("    LEFT OUTER JOIN Regolamenti")
                .AppendLine("        ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod")
                .AppendLine("    LEFT OUTER JOIN Cultivar")
                .AppendLine("        ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod")
                .AppendLine("    LEFT OUTER JOIN GruppoFinalita")
                .AppendLine("        ON Reg_Impianti.Grfi_Cod = GruppoFinalita.GRFI_COD")
                .AppendLine("    LEFT OUTER JOIN Copertura")
                .AppendLine("        ON Reg_Impianti.COP_COD = Copertura.Cop_Cod")
                .AppendLine("    LEFT OUTER JOIN SpecieVegetali")
                .AppendLine("        ON Cultivar.Veg_Cod = SpecieVegetali.VEG_COD")
                .AppendLine("    LEFT OUTER JOIN ImpiantiIrrigazioni")
                .AppendLine("        ON Reg_Impianti.IMP_COD = ImpiantiIrrigazioni.Imp_Cod")
                .AppendLine("    LEFT OUTER JOIN Reg_Impianti_Codici")
                .AppendLine("        ON Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA")
                .AppendLine("        AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.SA_COD")
                .AppendLine("        AND Reg_Impianti.APPEZZA = Reg_Impianti_Codici.APPEZZA")
                .AppendLine("        AND Reg_Impianti.id_reg = Reg_Impianti_Codici.id_reg")
                .AppendLine("        AND Reg_Impianti_Codici.id_cod > 2999")
                .AppendLine("        AND Reg_Impianti_Codici.id_cod < 4000")
                .AppendLine("    LEFT OUTER JOIN Codici_Anagrafe")
                .AppendLine("        ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice")
                .AppendLine(
                    "    LEFT OUTER JOIN Reg_Impianti_Codici AS Reg_Impianti_Codici_Codice_Impianto"
                )
                .AppendLine(
                    "        ON Reg_Impianti.PIVA = Reg_Impianti_Codici_Codice_Impianto.PIVA"
                )
                .AppendLine(
                    "        AND Reg_Impianti.SA_COD = Reg_Impianti_Codici_Codice_Impianto.SA_COD"
                )
                .AppendLine(
                    "        AND Reg_Impianti.APPEZZA = Reg_Impianti_Codici_Codice_Impianto.APPEZZA"
                )
                .AppendLine(
                    "        AND Reg_Impianti.id_reg = Reg_Impianti_Codici_Codice_Impianto.id_reg"
                )
                .AppendLine(
                    $"        AND Reg_Impianti_Codici_Codice_Impianto.id_cod = {CodiceCodiceImpianto}"
                )
                .AppendLine("    LEFT OUTER JOIN Reg_Impianti_Codici Reg_Impianti_Codici_KPIN")
                .AppendLine("        ON Reg_Impianti.PIVA = Reg_Impianti_Codici_KPIN.PIVA")
                .AppendLine("        AND Reg_Impianti.SA_COD = Reg_Impianti_Codici_KPIN.SA_COD")
                .AppendLine("        AND Reg_Impianti.APPEZZA = Reg_Impianti_Codici_KPIN.APPEZZA")
                .AppendLine("        AND Reg_Impianti.id_reg = Reg_Impianti_Codici_KPIN.id_reg")
                .AppendLine(
                    "        AND Imprese_Progetti.progetto_cod = Reg_Impianti_Codici_KPIN.progetto_cod"
                )
                .AppendLine($"        AND Reg_Impianti_Codici_KPIN.id_cod = {CodiceKPin}")
                .AppendLine("    LEFT OUTER JOIN Reg_Impianti_Codici Reg_Impianti_Codici_BlockName")
                .AppendLine("        ON Reg_Impianti.PIVA = Reg_Impianti_Codici_BlockName.PIVA")
                .AppendLine(
                    "        AND Reg_Impianti.SA_COD = Reg_Impianti_Codici_BlockName.SA_COD"
                )
                .AppendLine(
                    "        AND Reg_Impianti.APPEZZA = Reg_Impianti_Codici_BlockName.APPEZZA"
                )
                .AppendLine(
                    "        AND Reg_Impianti.id_reg = Reg_Impianti_Codici_BlockName.id_reg"
                )
                .AppendLine(
                    "        AND Imprese_Progetti.progetto_cod = Reg_Impianti_Codici_BlockName.progetto_cod"
                )
                .AppendLine(
                    $"        AND Reg_Impianti_Codici_BlockName.id_cod = {CodiceBlockName}"
                );

            if (leggiCartografia || generaStaticMap)
                stbQuery
                    .AppendLine("    LEFT JOIN gis_entita e")
                    .AppendLine("        ON Reg_Impianti.piva = e.piva")
                    .AppendLine("        AND Reg_Impianti.sa_cod = e.sa_cod")
                    .AppendLine("        AND Reg_Impianti.appezza = e.appezza")
                    .AppendLine("        AND Reg_Impianti.ID_REG = e.Id_Imp")
                    .AppendLine("        AND e.ID_Agenda = 0")
                    .AppendLine("    LEFT JOIN gis_elementigrafici g")
                    .AppendLine("        ON e.PivaSuperUser = g.PivaSuperUser")
                    .AppendLine("        AND e.entita_cod = g.Entita_Cod")
                    .AppendLine(
                        $"        AND g.LayerElementiGrafici_Cod = {CodiceLayerElementiGrafici}"
                    );

            stbQuery
                .AppendLine("WHERE")
                .AppendLine("    Reg_Impianti.Piva = @piva")
                .AppendLine(
                    "    AND (Reg_Impianti_Distinta.id_cod IS NULL OR Reg_Impianti_Distinta.val_cod = '0')"
                );

            parSql.Add("@piva", piva);

            if (leggiSoloAttivi)
            {
                stbQuery
                    .AppendLine("    AND Reg_Impianti.Validita_Inizio <= @data")
                    .AppendLine("    AND Reg_Impianti.Validita_Fine >= @data")
                    .AppendLine("    AND Imprese_Progetti.Validita_Inizio <= @data")
                    .AppendLine("    AND Imprese_Progetti.Validita_Fine >= @data");
            }
            else
            {
                stbQuery
                    .AppendLine("    AND Reg_Impianti.Validita_Fine >= @data")
                    .AppendLine("    AND Imprese_Progetti.Validita_Fine >= @data");
            }

            if (!leggiAncheBloccati)
                stbQuery.AppendLine("    AND Appezzamento.Blk_Flag <> -1");

            if (modalitaDemetra)
                stbQuery.AppendLine(
                    "    AND Reg_Impianti.Agea_IdColt IS NOT NULL AND Reg_Impianti.Agea_IdColt <> '0' AND Reg_Impianti.Agea_IdColt <> '' "
                );

            //Escludo impianti cessati
            stbQuery.AppendLine(" AND Reg_Impianti.flagCessata <> 1");

            parSql.Add("@data", data);

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
