using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.MisureIndiciMaturitaAnagrafiche
{
    public class MisureIndiciMaturitaAnagrafiche_APP : BaseDALAnagrafe, IMisureIndiciMaturitaAnagrafiche_APP
    {
        public MisureIndiciMaturitaAnagrafiche_APP(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable> ReadAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();

            stbQuery.AppendLine("SELECT")
                .AppendLine("    SpecieVegetali.Veg_Des,")
                .AppendLine("    MisuraxIndiciMaturita.IND_MAT_COD,")
                .AppendLine("    IndiciMaturita.IND_MAT_DES,")
                .AppendLine("    MisuraxIndiciMaturita.UDM_COD,")
                .AppendLine("    UnitaMisura.UDM_DES,")
                .AppendLine("    MisuraXIndiciMaturita_Anagrafiche.Anag_des,")
                .AppendLine("    MisuraXIndiciMaturita_Anagrafiche.Anag_valore")
                .AppendLine("FROM IndiciMaturitaxSpecieVegetali")
                .AppendLine("LEFT JOIN MisuraxIndiciMaturita")
                .AppendLine("    ON IndiciMaturitaxSpecieVegetali.IND_MAT_COD = MisuraxIndiciMaturita.IND_MAT_COD")
                .AppendLine("LEFT JOIN MisuraXIndiciMaturita_Anagrafiche")
                .AppendLine("    ON MisuraxIndiciMaturita.IND_MAT_COD = MisuraXIndiciMaturita_Anagrafiche.Ind_Mat_Cod")
                .AppendLine("    AND MisuraxIndiciMaturita.UDM_COD = MisuraXIndiciMaturita_Anagrafiche.Udm_Cod")
                .AppendLine("LEFT JOIN IndiciMaturita")
                .AppendLine("    ON MisuraxIndiciMaturita.IND_MAT_COD = IndiciMaturita.IND_MAT_COD")
                .AppendLine("LEFT JOIN UnitaMisura")
                .AppendLine("    ON MisuraxIndiciMaturita.UDM_COD = UnitaMisura.UDM_COD")
                .AppendLine("LEFT JOIN SpecieVegetali")
                .AppendLine("    ON SpecieVegetali.Veg_Cod = IndiciMaturitaxSpecieVegetali.VEG_COD")
                .AppendLine("WHERE 1 = 1")
                .AppendLine("    AND MisuraXIndiciMaturita_Anagrafiche.Anag_valore > 0")
                .AppendLine("    AND MisuraXIndiciMaturita_Anagrafiche.Inviato >= 0")
                .AppendLine("ORDER BY MisuraXIndiciMaturita_Anagrafiche.Anag_des ASC");
            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString());
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}