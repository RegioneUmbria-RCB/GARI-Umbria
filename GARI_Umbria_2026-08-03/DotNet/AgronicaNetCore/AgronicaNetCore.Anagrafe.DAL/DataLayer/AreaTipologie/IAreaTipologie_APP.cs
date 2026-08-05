using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.AreaTipologie
{
    public interface IAreaTipologie_APP
    {
        Task<DataTable> ReadAsync(AgronicaCoreParametriServer objParametriServer);
    }
}
