using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.CodificaProdotti
{
    public interface ICodificaProdotti_APP
    {
        Task<DataTable> ReadAsync(string piva, Enum_Tipo_CAC_Codifica_ProdottiAziendali tipoCodifica, int elemCod, AgronicaCoreParametriServer objParametriServer, bool soloMappati = false);

    }
}
