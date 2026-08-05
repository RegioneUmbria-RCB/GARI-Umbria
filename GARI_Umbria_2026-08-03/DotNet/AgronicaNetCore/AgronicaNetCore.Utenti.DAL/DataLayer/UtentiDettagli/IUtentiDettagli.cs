using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.UtentiDettagli
{
    public interface IUtentiDettagli
    {
        Task<DataTable> LeggiAsync(int idServizio, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiPrimoEdUltimoAccessoUtentiAsync(AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
