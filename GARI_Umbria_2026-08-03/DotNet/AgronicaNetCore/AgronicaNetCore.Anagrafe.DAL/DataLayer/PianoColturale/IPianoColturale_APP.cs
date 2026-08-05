using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.PianoColturale
{
    public interface IPianoColturale_APP
    {
        Task<DataTable> ReadAsync(
            string piva,
            DateTime data,
            AgronicaCoreParametriServer objParametriServer,
            bool leggiSoloAttivi = false,
            bool leggiAncheBloccati = false,
            bool leggiStaticMap = false,
            bool modalitaDemetra = false,
            bool leggiCartografia = false
        );
    }
}
