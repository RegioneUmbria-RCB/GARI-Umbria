using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Programmazione
{
    public interface IProgrammazione
    {
        Task<DataTable?> GetPlanningPerConfrontoCatastoAsync(string partitaIva, int programmazioneCod, AgronicaCoreParametriServer objParametriServer, bool origine = true, bool ShowCatasto = false, bool showVarieta = false);
    }
}
