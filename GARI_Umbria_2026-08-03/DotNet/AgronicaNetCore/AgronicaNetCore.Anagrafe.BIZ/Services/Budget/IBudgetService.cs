using AgronicaCoreDTOStd.InData.Budget;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Budget
{
    public interface IBudgetService
    {
        Task<DataTable> LeggiTestateBudgetAsync(LeggiBudgetTestate leggiBudgetTestate, AgronicaCoreParametriServer objParametriServer);
    }
}
