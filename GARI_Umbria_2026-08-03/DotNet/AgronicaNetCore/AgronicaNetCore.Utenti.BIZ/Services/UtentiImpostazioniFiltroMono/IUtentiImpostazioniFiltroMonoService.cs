using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Utenti.BIZ.Services.UtentiImpostazioniFiltroMono
{
    public interface IUtentiImpostazioniFiltroMonoService
    {
        Task<DataTable> LeggiAsync(TipiEnumerativi.Enum_Impostazioni_Utenti Impostazione_Cod, int ID_0, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);
    }
}
