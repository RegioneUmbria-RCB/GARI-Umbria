using InData.Agenda;
using System.Transactions;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Operazione.BIZ.Resources;
using InData.Anagrafica;
using AgronicaNetCore.Operazione.BIZ.Services.Movimenti_Dettagli;

namespace AgronicaNetCore.Operazione.BIZ.Services.Movimenti
{
    public class MovimentiService : BaseServiceOperazioneBIZ, IMovimentiService
    {
        private readonly IMovimenti _movimentiDal;
        private readonly IAgro_Sequence _sequenceDal;
        private readonly IMovDettagliService _movDettagliService;

        public MovimentiService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _movimentiDal = provider.GetRequiredService<IMovimenti>();
            _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
            _movDettagliService = provider.GetRequiredService<IMovDettagliService>();
        }

        public async Task ScriviMovimentoAsync(Movimento movimento, AgronicaCoreParametriServer objParametriServer)
        {
            int idMov = movimento.Id_Mov;
            if (idMov <= 0)
            {
                idMov = await _sequenceDal.NuovoId_TabellaAsync("Movimenti", movimento.BaseCode, movimento.TopCode, objParametriServer);
            }
            movimento.Id_Mov = idMov;
            if (movimento.Num_Protocollo_Decimal != movimento.Num_Protocollo && movimento.Num_Protocollo != 0)
            {
                movimento.Num_Protocollo_Decimal = movimento.Num_Protocollo;
            }
            await _movimentiDal.CreateAsync(movimento.ToWriteMovimenti(), objParametriServer);

            // TODO write dettagli tecnici extra

            if (movimento.Movimenti_Dettagli.Any())
            {
                foreach (var dett in movimento.Movimenti_Dettagli)
                {
                    dett.Id_Agenda = movimento.Id_Agenda;
                    dett.Id_Mov = idMov;
                    await _movDettagliService.ScriviMovimentoDettaglioAsync(dett, objParametriServer);

                }
            }

            // TODO write pagamenti
        }

        public async Task<int> ScriviModificaAsync(WriteMovimenti dtoMovimenti, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                bool isNew = false;
                
                if (dtoMovimenti.Id_Mov == 0)
                {
                    dtoMovimenti.Id_Mov = await _sequenceDal.NuovoId_TabellaAsync("movimenti", 0, 2000000000, objParametriServer);
                    isNew = true;
                }
                else
                    isNew = !await _movimentiDal.ExistAsync(dtoMovimenti.Id_Mov, objParametriServer);

                if (isNew)
                    await _movimentiDal.CreateAsync(dtoMovimenti, objParametriServer);
                else
                    await _movimentiDal.UpdateAsync(dtoMovimenti, objParametriServer);

                return dtoMovimenti.Id_Mov;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(WriteMovimenti dtoMovimenti, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                return await _movimentiDal.DeleteAsync(dtoMovimenti.Piva, dtoMovimenti.Id_Agenda, dtoMovimenti.Id_Mov, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(List<WriteMovimenti> Movimenti, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0)))
            {
                try
                {
                    foreach (var dto in Movimenti)
                    {
                        await _movimentiDal.DeleteAsync(dto.Piva, dto.Id_Agenda, dto.Id_Mov, objParametriServer);
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
