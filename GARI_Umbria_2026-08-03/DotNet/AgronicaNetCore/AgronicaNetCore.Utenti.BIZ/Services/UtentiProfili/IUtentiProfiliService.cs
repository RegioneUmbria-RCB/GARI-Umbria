using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Utenti.BIZ.Services.UtentiProfili
{
    public interface IUtentiProfiliService { 
        Task<string?> ReadAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer, int idServizio = 0);
    }
}
