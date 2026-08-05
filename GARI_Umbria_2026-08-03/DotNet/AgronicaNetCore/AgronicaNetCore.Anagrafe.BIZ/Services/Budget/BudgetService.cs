using AgronicaCoreDTOStd.InData.Budget;
using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Budget;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Budget
{
    public class BudgetService : BaseServiceAnagrafeBIZ, IBudgetService
    {
        private readonly IBudget _budgetDAL;

        public BudgetService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _budgetDAL = _serviceProvider.GetRequiredService<IBudget>();
        }

        public async Task<DataTable> LeggiTestateBudgetAsync(LeggiBudgetTestate leggiBudgetTestate , AgronicaCoreParametriServer objParametriServer)
        {
            DataTable dt;
            try
            {
                dt = await _budgetDAL.LeggiTestateBudgetAsync(leggiBudgetTestate.Piva, leggiBudgetTestate.Id_Budget, leggiBudgetTestate.IncludiPubblici,  objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return dt;
        }

    }
}
