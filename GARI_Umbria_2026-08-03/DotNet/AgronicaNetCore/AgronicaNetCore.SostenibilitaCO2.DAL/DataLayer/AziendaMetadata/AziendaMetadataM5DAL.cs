using AgronicaCoreModelsSTD.metaschema;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.DAL.Resources;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.AziendaMetadata
{
    /// <summary>
    /// Recupera i metadati anagrafici di un'azienda (Ragione Sociale, Città, Regione, Stato ISO)
    /// necessari per la costruzione del payload M5 Blockchain.
    /// Interroga la tabella <c>Aziende</c> per i campi <c>Ragione_Sociale</c>, <c>Citta</c>,
    /// <c>Regione</c>, <c>Stato_ISO3</c>.
    /// Riferimento spec: DS10-BL CreaTokenBlockchain — Regole 5, 6.
    /// </summary>
    public class AziendaMetadataM5DAL : BaseDALSostenibilitaCO2, IAziendaMetadataM5DAL
    {
        public AziendaMetadataM5DAL(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
        }

        /// <inheritdoc/>
        public async Task<DataTable> LeggiMetadatiAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(piva))
                throw new ArgumentException("Specificare la partita IVA.", nameof(piva));

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("SELECT Imprese.rag_soc AS RagioneSociale,");
            stbQuery.AppendLine("   Indirizzi.com_des AS Citta,");
            stbQuery.AppendLine("   lista_reg_prov.Regione_Des AS Regione,");
            stbQuery.AppendLine("   lista_reg_prov.Stato_ISO3");
            stbQuery.AppendLine("FROM Imprese WITH(NOLOCK)");
            stbQuery.AppendLine("JOIN ImpresexIndirizzi WITH(NOLOCK)");
            stbQuery.AppendLine("ON Imprese.PIVA = ImpresexIndirizzi.PIVA");
            stbQuery.AppendLine("JOIN Indirizzi");
            stbQuery.AppendLine("ON Indirizzi.cod_indirizzo = ImpresexIndirizzi.cod_indirizzo");
            stbQuery.AppendLine("JOIN (");
            stbQuery.AppendLine("   SELECT Lista_Regioni.REG,");
            stbQuery.AppendLine("       Lista_Regioni.Regione_Des,");
            stbQuery.AppendLine("       Lista_Province.PROV,");
            stbQuery.AppendLine("       Lista_Province.PROVINCIA,");
            stbQuery.AppendLine("       Lista_Regioni.Stato_Country AS Stato_ISO3");
            stbQuery.AppendLine("   FROM Lista_Regioni WITH(NOLOCK)");
            stbQuery.AppendLine("   JOIN Lista_Province WITH(NOLOCK)");
            stbQuery.AppendLine("   ON Lista_Regioni.reg = Lista_Province.REG");
            stbQuery.AppendLine(") AS lista_reg_prov");
            stbQuery.AppendLine("ON lista_reg_prov.PROV = Indirizzi.pro_cod_istat");
            stbQuery.AppendLine("WHERE Imprese.PIVA = @piva");

            var sqlParams = new Dictionary<string, object> { ["@piva"] = piva };

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
    }
}
