using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Models;
using InData.Log.AgronicaLogInvio;
using System.Dynamic;
using System.Text;
using AgronicaNetCore.Base.Constants;
using Microsoft.Extensions.DependencyInjection;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;

namespace AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogInvio.AgronicaLogInvioChiamate
{
    public class AgronicaLogInvioChiamate: DAL_Base,IAgronicaLogInvioChiamate
    {
        private readonly IAgro_Sequence _agroSequences;

        public AgronicaLogInvioChiamate(IServiceProvider provider) : base(provider)
        {
            _agroSequences = provider.GetRequiredService<IAgro_Sequence>();
        }

        public async Task<bool> WriteAsync(WriteAgronicaLogInvioChiamate writeAgronicaLogInvioChiamate, AgronicaCoreParametriServer objParametriServer)
        {

            if (writeAgronicaLogInvioChiamate.ID == 0)
            {

                writeAgronicaLogInvioChiamate.ID = await _agroSequences.NuovoId_TabellaAsync("Agronica_Log_Invio_Chiamate", 0, int.MaxValue, objParametriServer);
            }

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Agronica_Log_Invio_Chiamate (PivaSuperUser, ID, Tipo_Esportazione,")
                .AppendLine("    Dati_Inviati, Data_Invio, Esito, Dati_Ricevuti, Controllata, Tipo_Operazione, inviato, datainvio,")
                .AppendLine("    Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine,")
                .AppendLine("    Dettaglio1, Dettaglio2, Dettaglio3) ");
            stbQuery.AppendLine("VALUES (@PivaSuperUser, @ID, @Tipo_Esportazione, @Dati_Inviati, @Data_Invio,  ")
                .AppendLine("    @Esito, @Dati_Ricevuti, @Controllata, @Tipo_Operazione, @inviato, @datainvio, ")
                .AppendLine("    @Data_Creazione, @Data_Modifica, @Username_Creazione, @Username_Modifica, @Validita_Inizio, @Validita_Fine, ")
                .AppendLine("    @Dettaglio1, @Dettaglio2, @Dettaglio3) ");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@PivaSuperUser", objParametriServer.PivaSuperUser);
            expandoObj.TryAdd("@ID", writeAgronicaLogInvioChiamate.ID);
            expandoObj.TryAdd("@Tipo_Esportazione", writeAgronicaLogInvioChiamate.Tipo_Esportazione);
            expandoObj.TryAdd("@Dati_Inviati", writeAgronicaLogInvioChiamate.Dati_Inviati);
            expandoObj.TryAdd("@Data_Invio", writeAgronicaLogInvioChiamate.Data_Invio);
            expandoObj.TryAdd("@Esito", writeAgronicaLogInvioChiamate.Esito);
            expandoObj.TryAdd("@Dati_Ricevuti", writeAgronicaLogInvioChiamate.Dati_Ricevuti);
            expandoObj.TryAdd("@Controllata", writeAgronicaLogInvioChiamate.Controllata == null ? 0 : writeAgronicaLogInvioChiamate.Controllata);
            expandoObj.TryAdd("@Tipo_Operazione", writeAgronicaLogInvioChiamate.Tipo_Operazione);
            expandoObj.TryAdd("@inviato", 0);
            expandoObj.TryAdd("@datainvio", DBNull.Value);

            expandoObj.TryAdd("@Data_Creazione", DateTime.Now);
            expandoObj.TryAdd("@Data_Modifica", DateTime.Now);
            expandoObj.TryAdd("@Username_Creazione", objParametriServer.UsernameOperazione);
            expandoObj.TryAdd("@Username_Modifica", objParametriServer.UsernameOperazione);
            expandoObj.TryAdd("@Validita_Inizio", writeAgronicaLogInvioChiamate.Validita_Inizio == null ? CostantiPersonalizzate.AGRODATAINIZIO : writeAgronicaLogInvioChiamate.Validita_Inizio);
            expandoObj.TryAdd("@Validita_Fine", writeAgronicaLogInvioChiamate.Validita_Fine == null ? CostantiPersonalizzate.AGRODATAFINE : writeAgronicaLogInvioChiamate.Validita_Fine);

            expandoObj.TryAdd("@Dettaglio1", writeAgronicaLogInvioChiamate.Dettaglio1);
            expandoObj.TryAdd("@Dettaglio2", writeAgronicaLogInvioChiamate.Dettaglio2);
            expandoObj.TryAdd("@Dettaglio3", writeAgronicaLogInvioChiamate.Dettaglio3);

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


        /// <summary>
        /// Aggiorna un record di log esistente nella tabella Agronica_Log_Invio_Chiamate.
        /// Questo metodo replica la logica della funzione "Modifica" del DAL VB.NET,
        /// costruendo dinamicamente la query UPDATE per modificare solo i campi forniti.
        /// </summary>
        /// <param name="updateModel">L'oggetto DTO che contiene i dati per l'aggiornamento.</param>
        /// <param name="objParametriServer">I parametri di contesto del server.</param>
        /// <returns>True se l'operazione ha successo, altrimenti lancia un'eccezione.</returns>
        public async Task<bool> UpdateAsync(UpdateAgronicaLogInvioChiamate updateModel, AgronicaCoreParametriServer objParametriServer)
        {
            // Controlla che l'ID del record da aggiornare sia stato fornito. Senza di esso,
            // l'operazione non può procedere in modo sicuro.
            if (updateModel.ID == 0)
            {
                throw new ArgumentException("Il parametro ID è obbligatorio per l'operazione di aggiornamento.");
            }

            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject(); // Oggetto dinamico per contenere i parametri della query in modo sicuro (previene SQL Injection).
            var expandoDict = (IDictionary<string, object?>)expandoObj; // Vista dell'ExpandoObject come dizionario per aggiungere facilmente i parametri.
            var updates = new List<string>(); // Lista che conterrà le parti della clausola SET (es. "Esito = @Esito").

            stbQuery.AppendLine("UPDATE Agronica_Log_Invio_Chiamate SET ");

            // Questi campi vengono sempre aggiornati ad ogni chiamata.
            updates.Add("Data_Modifica = @Data_Modifica");
            expandoDict["@Data_Modifica"] = updateModel.Data_Invio ?? DateTime.Now;

            updates.Add("Username_Modifica = @Username_Modifica");
            expandoDict["@Username_Modifica"] = objParametriServer.UsernameOperazione;

            // Se nel modello è stata fornita una Data_Invio, usa quella. Altrimenti, usa la data e ora correnti.
            updates.Add("Data_Invio = @Data_Invio");
            expandoDict["@Data_Invio"] = updateModel.Data_Invio ?? DateTime.Now;

            // Questi campi vengono aggiunti alla query UPDATE solo se hanno un valore nel modello in input.
            // Questo impedisce di sovrascrivere dati esistenti nel DB con valori null o vuoti.
            if (updateModel.Dati_Inviati != null)
            {
                updates.Add("Dati_Inviati = @Dati_Inviati");
                expandoDict["@Dati_Inviati"] = updateModel.Dati_Inviati;
            }
            if (!string.IsNullOrEmpty(updateModel.Esito))
            {
                updates.Add("Esito = @Esito");
                expandoDict["@Esito"] = updateModel.Esito;
            }
            if (updateModel.Dati_Ricevuti != null)
            {
                updates.Add("Dati_Ricevuti = @Dati_Ricevuti");
                expandoDict["@Dati_Ricevuti"] = updateModel.Dati_Ricevuti;
            }
            if (!string.IsNullOrWhiteSpace(updateModel.Tipo_Operazione))
            {
                updates.Add("Tipo_Operazione = @Tipo_Operazione");
                expandoDict["@Tipo_Operazione"] = updateModel.Tipo_Operazione;
            }

            stbQuery.AppendLine(string.Join(", ", updates));

            // Definisce quale record specifico deve essere aggiornato, usando la chiave composta.
            stbQuery.AppendLine(" WHERE ID = @ID AND Tipo_Esportazione = @Tipo_Esportazione");
            expandoDict["@ID"] = updateModel.ID;
            expandoDict["@Tipo_Esportazione"] = updateModel.Tipo_Esportazione;

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
