using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.CodificheCAC.DAL.Model;

namespace AgronicaNetCore.CodificheCAC.BIZ.Services
{
    public interface ICodificheCACService
    {
        Task<DataTable> GetAllCodificheAsync(AgronicaCoreParametriServer objParametriServer);
        Task<bool> DeleteCac(CodificaCACModel objCAC, AgronicaCoreParametriServer objParametriServer);
        Task<bool> UpdateCac(CodificaCACModel objCAC, AgronicaCoreParametriServer objParametriServer);
        Task<bool> WriteCac(CodificaCACModel objCAC, AgronicaCoreParametriServer objParametriServer);
    }
}
