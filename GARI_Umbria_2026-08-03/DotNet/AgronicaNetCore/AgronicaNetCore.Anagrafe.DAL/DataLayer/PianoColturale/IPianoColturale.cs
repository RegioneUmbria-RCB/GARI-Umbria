using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.PianoColturale
{
    public interface IPianoColturale
    {
        Task<DataTable?> ReadPlanningsByCompanyAsync(string partitaIva, AgronicaCoreParametriServer objParametriServer);

    }
}
