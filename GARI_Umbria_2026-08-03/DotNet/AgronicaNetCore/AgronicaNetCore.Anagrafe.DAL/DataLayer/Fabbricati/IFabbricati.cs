using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Fabbricati
{
    public interface IFabbricati
    {
        public Task<int> ReadFabbCodAsync(string Piva, int Sa_Cod, int Tipo_Fabb, AgronicaCoreParametriServer objParametriServer);
        public Task<DataTable> ReadAsync(string Piva, int Sa_Cod, int Fabbricato_Cod, AgronicaCoreParametriServer objParametriServer);
        public Task<DataTable> LeggiDescrizioneAsync(List<(string, int, int)> chiaviFabbricato, AgronicaCoreParametriServer objParametriServer, bool estraiAzienda = false);
    }
}
