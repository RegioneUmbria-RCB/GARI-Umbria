using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Zone
{
    public interface IZone
    {
        Task<DataTable> LeggiZoneAsync(int Zona_Cod, AgronicaCoreParametriServer objParametriServer);

    }
}
