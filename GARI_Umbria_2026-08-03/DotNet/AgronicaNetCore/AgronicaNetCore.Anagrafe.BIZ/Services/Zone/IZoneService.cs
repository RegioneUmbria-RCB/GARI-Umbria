using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Zone
{
    public interface IZoneService
    {
        Task<DataTable> LeggiZoneAsync(int Zona_Cod, AgronicaCoreParametriServer objParametriServer);
    }
}
