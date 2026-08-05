using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Gis.Shared.Interfaces
{
    public interface IGisElementiGrafici
    {
        Task<bool> AggiornaStaticMapAsync(int entitaCod, byte[] staticMap, AgronicaCoreParametri objParametri);
    }
}
