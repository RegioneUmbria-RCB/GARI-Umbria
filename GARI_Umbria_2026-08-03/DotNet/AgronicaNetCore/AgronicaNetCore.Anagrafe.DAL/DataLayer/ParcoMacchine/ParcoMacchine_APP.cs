using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.ParcoMacchine
{
    public class ParcoMacchine_APP : BaseDALAnagrafe, IParcoMacchine_APP
    {
        public ParcoMacchine_APP(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> ReadAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT")
                .AppendLine("    Parco_Macchine.Piva")
                .AppendLine("    , Parco_Macchine.Sa_Cod")
                .AppendLine("    , COALESCE(Parco_Macchine.Mac_Cod, 0) AS Mac_Cod")
                .AppendLine("    , Parco_Macchine.Mac_Des")
                .AppendLine("    , Parco_Macchine.Modello")
                .AppendLine("    , Parco_Macchine.Validita_Inizio")
                .AppendLine("    , Parco_Macchine.Validita_Fine")
                .AppendLine("    , Parco_Macchine.Class_Code")
                .AppendLine("    , COALESCE(Parco_Macchine.TitoloPossesso, 0) AS TitoloPossesso")
                .AppendLine("    , COALESCE(Parco_Macchine.Tipo, 0) AS Tipo")
                .AppendLine("    , Parco_Macchine.Denominazione_Proprietario")
                .AppendLine("    , Parco_Macchine.Targa")
                .AppendLine("    , Parco_Macchine.N_Immatricolazione")
                .AppendLine("    , Parco_Macchine.Data_Immatricolazione")
                .AppendLine("    , Parco_Macchine.Codice")
                .AppendLine("    , Parco_Macchine.BTM_Serial")
                .AppendLine("    , Parco_Macchine.VIN")
                .AppendLine("    , COALESCE(Parco_Macchine.Img_Thumbnail, '') as Img_Thumbnail")
                .AppendLine("    , COALESCE(Parco_Macchine.Img_Thumbnail_FileName, '') as Img_Thumbnail_FileName")
                .AppendLine("    , COALESCE(Parco_Macchine.Img_Thumbnail_Extension, '') as Img_Thumbnail_Extension")
                .AppendLine("    , COALESCE(Parco_Macchine.Img_Large, '') as Img_Large")
                .AppendLine("    , COALESCE(Parco_Macchine.Img_Large_FileName, '') as Img_Large_FileName")
                .AppendLine("    , COALESCE(Parco_Macchine.Img_Large_Extension, '') as Img_Large_Extension")
                .AppendLine("    , Parco_Macchine.Distinta_Installazione")
                .AppendLine("    , Parco_Macchine.Contratto_Installazione")
                .AppendLine("    , Parco_Macchine.Tipologia_Installazione")
                .AppendLine("    , Parco_Macchine.Data_Inizio_Installazione")
                .AppendLine("    , Parco_Macchine.Data_Fine_Installazione")
                .AppendLine("    , Parco_Macchine.Stato_Installazione")
                .AppendLine("    , Parco_Macchine.Provincia_Istat_Installazione")
                .AppendLine("    , Parco_Macchine.Comune_Istat_Installazione")
                .AppendLine("    , Parco_Macchine.Indirizzo_Installazione")
                .AppendLine("    , COALESCE(Parco_Macchine.Latitudine_Installazione, 0) as Latitudine_Installazione")
                .AppendLine("    , COALESCE(Parco_Macchine.Longitudine_Installazione, 0) as Longitudine_Installazione")
                .AppendLine("    , Parco_Macchine.Cod_Contatto")
                .AppendLine("    , COALESCE(Parco_Macchine.Visibile_ctrl_gestione, 0) AS Visibile_ctrl_gestione")
                .AppendLine("    , CASE")
                .AppendLine("        WHEN Parco_Macchine.HubIot_PlatformDestination = 0 THEN 'Non Definito'")
                .AppendLine("        WHEN Parco_Macchine.HubIot_PlatformDestination = 1 THEN 'JohnDeere'")
                .AppendLine("        WHEN Parco_Macchine.HubIot_PlatformDestination = 2 THEN 'Agrirouter'")
                .AppendLine("        WHEN Parco_Macchine.HubIot_PlatformDestination = 3 THEN 'AGCO_Trimble'")
                .AppendLine("        WHEN Parco_Macchine.HubIot_PlatformDestination = 4 THEN 'CNH1'")
                .AppendLine("    END AS HubIot_PlatformDestinationDes")
                .AppendLine("    , agea.AGEA_Des")
                .AppendLine("    , Macchine.CLASS_DESC")
                .AppendLine("    , Imprese.Rag_Soc AS Impresa")
                .AppendLine("    , ISNULL((SELECT class_desc FROM Macchine WHERE CLASS_CODE = SUBSTRING(Parco_Macchine.Class_Code, 1, 2) AND LEN(Class_Code) = 2), '') AS tipo_desc")
                .AppendLine("    , ISNULL((SELECT class_desc FROM Macchine WHERE CLASS_CODE = SUBSTRING(Parco_Macchine.Class_Code, 1, 5) AND LEN(Class_Code) = 5), '') AS dettaglio_1_desc")
                .AppendLine("    , ISNULL((SELECT class_desc FROM Macchine WHERE CLASS_CODE = SUBSTRING(Parco_Macchine.Class_Code, 1, 7) AND LEN(Class_Code) = 7), '') AS dettaglio_2_desc")
                .AppendLine("    , COALESCE(D.Ditta_Des, '') AS Ditta_Des")
                .AppendLine("    , COALESCE(pcm.val_cod, '') AS numero_certificato")
                .AppendLine("FROM")
                .AppendLine("    Parco_Macchine WITH(NOLOCK)")
                .AppendLine("    INNER JOIN UtentiXImprese WITH(NOLOCK)")
                .AppendLine("        ON Parco_Macchine.Piva = UtentiXImprese.PIVA")
                .AppendLine("    LEFT JOIN Macchine WITH(NOLOCK)")
                .AppendLine("        ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE")
                .AppendLine("    INNER JOIN Imprese WITH(NOLOCK)")
                .AppendLine("        ON Imprese.Piva = Parco_Macchine.Piva")
                .AppendLine("    LEFT JOIN Ditte D WITH(NOLOCK)")
                .AppendLine("        ON D.Ditta_cod = Parco_Macchine.Ditta_Cod")
                .AppendLine("    LEFT JOIN Codifica_Macchine_Agea agea WITH(NOLOCK)")
                .AppendLine("        ON Parco_Macchine.Agea_Cod = agea.AGEA_Cod")
                .AppendLine("    LEFT JOIN Parco_Macchine_Codici pcm WITH(NOLOCK)")
                .AppendLine("        ON Parco_Macchine.Mac_Cod = pcm.Mac_cod")
                .AppendLine("        AND Parco_Macchine.Sa_Cod = pcm.sa_cod")
                .AppendLine("        AND Parco_Macchine.Piva = pcm.PIVA")
                .AppendLine("WHERE")
                .AppendLine("    UtentiXImprese.[USER] = @pivaSuperUser")
                .AppendLine("    AND Parco_Macchine.Inviato >= 0")
                .AppendLine("    AND Parco_Macchine.Mac_Cod_Origine = 0")
                .AppendLine("    AND Parco_Macchine.Inviato >= 0")
                .AppendLine("    AND (Parco_Macchine.Piva = @piva OR Parco_Macchine.Sa_Cod = -1)");

            parSql.Add("@pivaSuperUser", objParametriServer.PivaSuperUser.Trim());
            parSql.Add("@piva", piva);


            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
