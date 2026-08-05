using InData.Agenda;
using System.Transactions;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using AgronicaNetCore.Operazione.DAL.DataLayer.Mov_Destinazioni;
using AgronicaNetCore.Operazione.BIZ.Resources;
using Microsoft.Extensions.Localization;
using InData.Anagrafica;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Apezzamenti;
using AgronicaCoreModelsSTD.anagrafiche;

namespace AgronicaNetCore.Operazione.BIZ.Services.Mov_Destinazioni
{
    public class MovDestinazioniService : BaseServiceOperazioneBIZ, IMovDestinazioniService
    {
        private readonly IMov_Destinazioni _movDestinazioniDal;
        private readonly IAppezzamenti _appezzamenti;

        public MovDestinazioniService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _movDestinazioniDal = provider.GetRequiredService<IMov_Destinazioni>();
            _appezzamenti = provider.GetRequiredService<IAppezzamenti>();
        }

        public async Task ScriviMovimentoDestinazioneAsync(Movimento_Destinazione movDest, AgronicaCoreParametriServer objParametriServer)
        {
            var key = movDest.Piva + "_" + movDest.Sa_Cod + "_" + movDest.Appezza;
            if (await _appezzamenti.IsBlockedAsync(
                new Appezzamento.PK(movDest.Appezza, movDest.Sa_Cod, movDest.Piva),
                movDest.Data,
                objParametriServer
            ))
                throw new InvalidOperationException(_localizer.GetString("AppezzamentoBloccato", key));

            await _movDestinazioniDal.CreateAsync(movDest.ToWriteMovDestinazioni(), objParametriServer);

            // TODO gestire giswkt
        }

        public async Task<int> ScriviModificaAsync(WriteMovDestinazioni dtoMovDest, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                bool isNew = 
                    !await _movDestinazioniDal.ExistAsync(dtoMovDest.Piva, dtoMovDest.Id_Agenda, dtoMovDest.Id_Mov, dtoMovDest.Id_Mov_Det, dtoMovDest.Id_Destinazione, objParametriServer);

                if (isNew)
                    await _movDestinazioniDal.CreateAsync(dtoMovDest, objParametriServer);
                else
                    await _movDestinazioniDal.UpdateAsync(dtoMovDest, objParametriServer);

                return dtoMovDest.Id_Destinazione;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(WriteMovDestinazioni dtoMovDest, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                return await _movDestinazioniDal.DeleteAsync(dtoMovDest.Piva, dtoMovDest.Id_Agenda, dtoMovDest.Id_Mov, dtoMovDest.Id_Mov_Det, dtoMovDest.Id_Destinazione, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(List<WriteMovDestinazioni> MovDestinazioni, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0)))
            {
                try
                {
                    foreach (var dto in MovDestinazioni)
                    {
                        await _movDestinazioniDal.DeleteAsync(dto.Piva, dto.Id_Agenda, dto.Id_Mov, dto.Id_Mov_Det, dto.Id_Destinazione, objParametriServer);
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
