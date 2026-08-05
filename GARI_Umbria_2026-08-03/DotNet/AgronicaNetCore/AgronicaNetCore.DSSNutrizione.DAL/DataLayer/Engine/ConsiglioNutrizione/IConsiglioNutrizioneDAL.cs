using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.ConsiglioNutrizione.Models;

namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.ConsiglioNutrizione
{
    /// <summary>
    /// Contratto DAL per la persistenza del consiglio nutrizionale e il polling del webhook.
    /// Riferimento: DS04-BL Richiesta Consiglio Nutrizione Appezzamenti — Persistenze Coinvolte.
    /// DS15-BL Polling WebHook Consiglio Nutrizione — Persistenze Coinvolte.
    /// </summary>
    public interface IConsiglioNutrizioneDAL
    {

        /// <summary>
        /// Inserisce un elemento del consiglio nutrizionale in Consigli_Nutrizione_Engine.
        /// Restituisce l'ID IDENTITY generato dal database.
        /// Riferimento: DS04-BL — Persistenza in Consigli_Nutrizione_Engine.
        /// </summary>
        Task<int> InsertConsiglioNutrizioneAsync(
            InsertConsiglioNutrizioneInput input,
            AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Inserisce un record di audit in Input_Consigli_Nutrizione_Engine.
        /// Riferimento: DS04-BL — Persistenza in Input_Consigli_Nutrizione_Engine.
        /// </summary>
        Task InsertInputConsiglioNutrizioneAsync(
            InsertInputConsiglioNutrizioneInput input,
            AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Verifica se esiste già un record in Consigli_Nutrizione_Engine per la combinazione
        /// (PIVA, SA_COD, APPEZZA, ID_REG, Data_Consiglio) al fine di evitare duplicati.
        /// Riferimento: DS07-BL Salvataggio Consiglio Nutrizione — Regole di Business (Verifica duplicati).
        /// </summary>
        /// <returns><c>true</c> se almeno un record corrisponde alla chiave; altrimenti <c>false</c>.</returns>
        Task<bool> ExistsConsiglioNutrizioneAsync(
            string piva,
            int saCod,
            int appezza,
            int idReg,
            DateTime dataConsiglio,
            AgronicaCoreParametriServer objParametriServer);
    }
}
