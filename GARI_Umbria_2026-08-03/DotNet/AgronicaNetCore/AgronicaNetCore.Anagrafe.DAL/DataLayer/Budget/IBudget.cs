using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Budget
{
    public interface IBudget
    {
        Task<DataTable> LeggiTestateBudgetAsync(string piva, int idBudget, bool includiPubblici, AgronicaCoreParametriServer objParametriServer);
    }
}
