using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using InData.Log.AgronicaLogInvio;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogInvio.AgronicaLogInvioAnagrafe
{
    /// <summary>
    /// Implementazione del DAL per scrivere nella tabella di collegamento Agronica_Log_Invio_Anagrafe.
    /// </summary>
    public class AgronicaLogInvioAnagrafe : DAL_Base, IAgronicaLogInvioAnagrafe
    {
        public AgronicaLogInvioAnagrafe(IServiceProvider provider) : base(provider) { }

        public async Task<bool> WriteAsync(WriteAgronicaLogInvioAnagrafe writeModel, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Agronica_Log_Invio_Anagrafe (Tipo_Esportazione, ID_Log_Invio, Tipo, Chiave, Piva, Sa_Cod, Mat_Cod, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine)");
            stbQuery.AppendLine("VALUES (@Tipo_Esportazione, @ID_Log_Invio, @Tipo, @Chiave, @Piva, @Sa_Cod, @Mat_Cod, @Data_Creazione, @Data_Modifica, @Username_Creazione, @Username_Modifica, @Validita_Inizio, @Validita_Fine)");


            var expandoObj = new ExpandoObject();
            var expandoDict = (IDictionary<string, object?>)expandoObj;


            expandoDict["@Tipo_Esportazione"] = writeModel.Tipo_Esportazione;
            expandoDict["@ID_Log_Invio"] = writeModel.ID_Log_Invio;
            expandoDict["@Tipo"] = writeModel.Tipo ?? string.Empty; // Se Tipo è null, usa stringa vuota
            expandoDict["@Chiave"] = writeModel.Chiave ?? string.Empty; // Se Chiave è null, usa stringa vuota
            expandoDict["@Piva"] = writeModel.Piva ?? string.Empty; // Se Piva è null, usa stringa vuota
            expandoDict["@Sa_Cod"] = writeModel.Sa_Cod;

            // Per i tipi valore nullable (int?, DateTime?), il cast a (object) è il modo corretto
            // per farli diventare DBNull.Value se sono null.
            expandoDict["@Mat_Cod"] = (object?)writeModel.Mat_Cod ?? DBNull.Value;

            // Campi di audit
            expandoDict["@Data_Creazione"] = DateTime.Now;
            expandoDict["@Data_Modifica"] = DateTime.Now;
            expandoDict["@Username_Creazione"] = objParametriServer.UsernameOperazione;
            expandoDict["@Username_Modifica"] = objParametriServer.UsernameOperazione;
            expandoDict["@Validita_Inizio"] = (object)CostantiPersonalizzate.AGRODATAINIZIO ?? DBNull.Value;
            expandoDict["@Validita_Fine"] = (object)CostantiPersonalizzate.AGRODATAFINE ?? DBNull.Value;

            try
            {
                // Passa l'oggetto ExpandoObject originale
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
