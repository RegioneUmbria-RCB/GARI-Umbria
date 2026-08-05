using AgronicaCoreDTOStd.InData;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Utenti.BIZ.Services.UtentiPermessi
{
    public interface IUtentiPermessiService
    {
        Task<bool> ReadAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer, int idService, enum_Security_Attivita idActivity, int idOperation, int id = 1);
        
        Task<List<Utente_Permesso>> ReadAllAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);
    }
}
