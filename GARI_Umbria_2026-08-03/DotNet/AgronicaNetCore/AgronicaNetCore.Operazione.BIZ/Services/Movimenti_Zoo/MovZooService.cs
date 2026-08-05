using InData.Agenda;
using System.Transactions;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti_Zoo;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Operazione.BIZ.Resources;

namespace AgronicaNetCore.Operazione.BIZ.Services.Movimenti_Zoo
{
    public class MovZooService : BaseServiceOperazioneBIZ //, IMovZooService
    {
        private readonly IMovimenti_Zoo _movZooDal;

        public MovZooService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _movZooDal = provider.GetRequiredService<IMovimenti_Zoo>();
        }

        public async Task<bool> ScriviModificaAsync(WriteMovimentiZoo dtoMovZoo, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                bool isNew = !await _movZooDal.ExistAsync(dtoMovZoo.Piva, dtoMovZoo.Id_Agenda, dtoMovZoo.Id_Mov, objParametriServer);

                if (isNew)
                    await _movZooDal.CreateAsync(dtoMovZoo, objParametriServer);
                else
                    await _movZooDal.UpdateAsync(dtoMovZoo, objParametriServer);

                return true;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(WriteMovimentiZoo dtoMovZoo, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                return await _movZooDal.DeleteAsync(dtoMovZoo.Piva, dtoMovZoo.Id_Agenda, dtoMovZoo.Id_Mov, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(List<WriteMovimentiZoo> MovZoo, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0)))
            {
                try
                {
                    foreach (var dto in MovZoo)
                    {
                        await _movZooDal.DeleteAsync(dto.Piva, dto.Id_Agenda, dto.Id_Mov, objParametriServer);
                    }
                    ts.Complete();
                }
                catch (Exception ex)
                {
                    LogError(ex.Message, objParametriServer, ex);
                    throw;
                }
                finally
                {
                    ts.Dispose();
                }
            }

            return true;
        }
    }
}
