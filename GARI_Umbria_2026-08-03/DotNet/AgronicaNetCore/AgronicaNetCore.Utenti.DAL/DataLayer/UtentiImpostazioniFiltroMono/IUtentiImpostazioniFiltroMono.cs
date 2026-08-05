using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioniFiltroMono
{
    public interface IUtentiImpostazioniFiltroMono
    {
        Task<DataTable> LeggiAsync(TipiEnumerativi.Enum_Impostazioni_Utenti Impostazione_Cod, int ID_0, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);
    }
}
