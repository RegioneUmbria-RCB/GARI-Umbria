using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.ConsiglioNutrizione.Models;
using System.Dynamic;
using System.Text;

namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.ConsiglioNutrizione
{
    /// <summary>
    /// Implementazione DAL per la persistenza del consiglio nutrizionale e il polling del webhook.
    /// Riferimento: DS04-BL Richiesta Consiglio Nutrizione Appezzamenti — Persistenze Coinvolte.
    /// DS15-BL Polling WebHook Consiglio Nutrizione — Regole di Business.
    /// </summary>
    public sealed class ConsiglioNutrizioneDAL : DAL_Base, IConsiglioNutrizioneDAL
    {

        public ConsiglioNutrizioneDAL(IServiceProvider provider) : base(provider)
        {
        }

        /// <inheritdoc/>
        public async Task<int> InsertConsiglioNutrizioneAsync(
            InsertConsiglioNutrizioneInput input,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));

            const string sql = @"
                INSERT INTO Consigli_Nutrizione_Engine
                    (PIVA, SA_COD, APPEZZA, ID_REG,
                     Data_Consiglio, Elemento,
                     Fabbisogno_Minimo, Fabbisogno_Massimo,
                     Dose_Consigliata_Minima, Dose_Consigliata_Massima,
                     Quantitativo_Presente,
                     Quantitativo_Minimo_Residuo, Quantitativo_Massimo_Residuo,
                     Messaggi, Inviato,
                     Data_Creazione, Data_Modifica,
                     Username_Creazione, Username_Modifica,
                     Validita_Inizio, Validita_Fine)
                OUTPUT INSERTED.ID
                VALUES
                    (@piva, @saCod, @appezza, @idReg,
                     @dataConsiglio, @elemento,
                     @fabbisognoMinimo, @fabbisognoMassimo,
                     @doseConsigliataMiniima, @doseConsigliataMassima,
                     @quantitativoPresente,
                     @quantitativoMinimoResiduo, @quantitativoMassimoResiduo,
                     @messaggi, 0,
                     GETDATE(), GETDATE(),
                     @usernameCreazione, @usernameCreazione,
                     @validitaInizio, @validitaFine);";

            var parameters = new Dictionary<string, object>
            {
                ["@piva"]                      = input.Piva,
                ["@saCod"]                     = input.SaCod,
                ["@appezza"]                   = input.Appezza,
                ["@idReg"]                     = input.IdReg,
                ["@dataConsiglio"]             = input.DataConsiglio,
                ["@elemento"]                  = input.Elemento,
                ["@fabbisognoMinimo"]          = (object?)input.FabbisognoMinimo ?? DBNull.Value,
                ["@fabbisognoMassimo"]         = (object?)input.FabbisognoMassimo ?? DBNull.Value,
                ["@doseConsigliataMiniima"]    = (object?)input.DoseConsigliataMiniima ?? DBNull.Value,
                ["@doseConsigliataMassima"]    = (object?)input.DoseConsigliataMassima ?? DBNull.Value,
                ["@quantitativoPresente"]      = (object?)input.QuantitativoPresente ?? DBNull.Value,
                ["@quantitativoMinimoResiduo"] = (object?)input.QuantitativoMinimoResiduo ?? DBNull.Value,
                ["@quantitativoMassimoResiduo"]= (object?)input.QuantitativoMassimoResiduo ?? DBNull.Value,
                ["@messaggi"]                  = (object?)input.Messaggi ?? DBNull.Value,
                ["@usernameCreazione"]         = input.UsernameCreazione,
                ["@validitaInizio"]            = CostantiPersonalizzate.AGRODATAINIZIO_DATE,
                ["@validitaFine"]              = CostantiPersonalizzate.AGRODATAFINE_DATE
            };

            try
            {
                var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, parameters);
                if (dt == null || dt.Rows.Count == 0)
                    throw new InvalidOperationException("INSERT su Consigli_Nutrizione_Engine non ha restituito l'ID.");

                return Convert.ToInt32(dt.Rows[0]["ID"]);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task InsertInputConsiglioNutrizioneAsync(
            InsertInputConsiglioNutrizioneInput input,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));

            var expando = new ExpandoObject();
            expando.TryAdd("@idConsiglio",         input.IdConsiglio);
            expando.TryAdd("@idAgenda",            (object?)input.IdAgenda ?? DBNull.Value);
            expando.TryAdd("@idMov",               (object?)input.IdMov ?? DBNull.Value);
            expando.TryAdd("@idMovDet",            (object?)input.IdMovDet ?? DBNull.Value);
            expando.TryAdd("@analisiSuperUser",    (object?)input.AnalisiSuperUser ?? DBNull.Value);
            expando.TryAdd("@analisiTestataCod",   (object?)input.AnalisiTestataCod ?? DBNull.Value);
            expando.TryAdd("@analisiDettaglioCod", (object?)input.AnalisiDettaglioCod ?? DBNull.Value);
            expando.TryAdd("@analisiParametroCod", (object?)input.AnalisiParametroCod ?? DBNull.Value);
            expando.TryAdd("@usernameCreazione",   input.UsernameCreazione);
            expando.TryAdd("@validitaInizio",      CostantiPersonalizzate.AGRODATAINIZIO_DATE);
            expando.TryAdd("@validitaFine",        CostantiPersonalizzate.AGRODATAFINE_DATE);

            const string sql = @"
                INSERT INTO Input_Consigli_Nutrizione_Engine
                    (ID_Consiglio,
                     Id_Agenda, Id_Mov, Id_Mov_Det,
                     Analisi_SuperUser, Analisi_Testata_Cod, Analisi_Dettaglio_Cod, Analisi_Parametro_Cod,
                     Inviato, Data_Creazione, Data_Modifica,
                     Username_Creazione, Username_Modifica,
                     Validita_Inizio, Validita_Fine)
                VALUES
                    (@idConsiglio,
                     @idAgenda, @idMov, @idMovDet,
                     @analisiSuperUser, @analisiTestataCod, @analisiDettaglioCod, @analisiParametroCod,
                     0, GETDATE(), GETDATE(),
                     @usernameCreazione, @usernameCreazione,
                     @validitaInizio, @validitaFine);";

            try
            {
                await GetDataProvider(objParametriServer).Execute_WriteAsync(sql, expando);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsConsiglioNutrizioneAsync(
            string piva,
            int saCod,
            int appezza,
            int idReg,
            DateTime dataConsiglio,
            AgronicaCoreParametriServer objParametriServer)
        {
            var parameters = new Dictionary<string, object>
            {
                ["@piva"] = piva,
                ["@saCod"] = saCod,
                ["@appezza"] = appezza,
                ["@idReg"] = idReg
            };
            

            var sql = new StringBuilder();

            sql.AppendLine("SELECT *");
            sql.AppendLine("FROM   Consigli_Nutrizione_Engine");
            sql.AppendLine("WHERE  PIVA           = @piva");
            sql.AppendLine("AND  SA_COD        = @saCod");
            sql.AppendLine("AND  APPEZZA       = @appezza");
            sql.AppendLine("AND  ID_REG        = @idReg");

            if (dataConsiglio != CostantiPersonalizzate.AGRODATAINIZIO_DATE && dataConsiglio != CostantiPersonalizzate.AGRODATAFINE_DATE)
            {
                sql.AppendLine("AND  Data_Consiglio = @dataConsiglio");
                parameters.Add("@dataConsiglio", dataConsiglio);
            }

            try
            {
                var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql.ToString(), parameters);
                return dt != null && dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
