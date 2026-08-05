using AgronicaCoreDTOStd.OutData;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Utenti.BIZ.Services.UtentiImpostazioni
{
    public interface IUtentiImpostazioniService
    {
        Task<DataTable?> Read_User_Then_SuperUserAsync(int cod, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable?> ReadAsync(int cod, int User1_SuperUser2, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);
        Task<DataInizioEFine> LeggiAnnataAgrariaAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);
    }
}
