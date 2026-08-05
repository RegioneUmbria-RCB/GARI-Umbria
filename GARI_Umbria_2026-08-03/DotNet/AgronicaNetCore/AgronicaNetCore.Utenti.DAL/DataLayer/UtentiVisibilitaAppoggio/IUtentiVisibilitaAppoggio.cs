using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio
{
        public interface IUtentiVisibilitaAppoggio
        {
                Task<DataTable?> ReadAsync(int entity, AgronicaCoreParametriServer objParametriServer, string piva = "");

                Task<DataTable?> ReadForUsernameAsync(int entity, string username, AgronicaCoreParametriServer objParametriServer, string piva = "");

                Task<DataTable?> ReadRowExistsForUsernameAsync(int entity, string username, AgronicaCoreParametriServer parametriServer);

                Task<DataTable?> ReadVisibilitaCentriAsync(string? piva, AgronicaCoreParametriServer objParametriServer);

                /// <summary>
                /// Conta il numero di aziende visibili per l'utente specificato interrogando <c>Utenti_Visibilita_Appoggio</c>.
                /// Utilizzato da DS04-BL per il confronto real-time con il totale aziende del sistema.
                /// </summary>
                /// <param name="username">Username dell'utente di cui contare le aziende visibili (può differire dall'utente corrente in <paramref name="objParametriServer"/>).</param>
                Task<int> ContaAziendeVisibiliAsync(string username, AgronicaCoreParametriServer objParametriServer);

                Task<bool> CancellaPerUtenteAsync(string username, AgronicaCoreParametriServer objParametriServer);

                Task BulkInsertAsync(DataTable dati, AgronicaCoreParametriServer objParametriServer);

                Task<DataTable?> ReadMinimumDataAsync(int entity, AgronicaCoreParametriServer parametriServer, bool readSaCod = true);
        }
}
