using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Operazione.BIZ.Resources;
using AgronicaNetCore.Operazione.BIZ.Services.Mov_Destinazioni;
using AgronicaNetCore.Operazione.BIZ.Services.Mov_Dettaglio_Tecnico;
using AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti_Dettagli;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using InData.Agenda;
using InData.Anagrafica;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Transactions;

namespace AgronicaNetCore.Operazione.BIZ.Services.Movimenti_Dettagli
{
    public class MovDettagliService : BaseServiceOperazioneBIZ, IMovDettagliService
    {
        private readonly IMovimenti_Dettagli _movDettagliDal;
        private readonly IMovDestinazioniService _movDestinazioni;
        private readonly IAgro_Sequence _sequenceDal;
        private readonly IMovDettTecnicoService _movDettTecnico;

        public MovDettagliService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _movDettagliDal = provider.GetRequiredService<IMovimenti_Dettagli>();
            _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
            _movDestinazioni = provider.GetRequiredService<IMovDestinazioniService>();
            _movDettTecnico = provider.GetRequiredService<IMovDettTecnicoService>();
        }


        public async Task ScriviMovimentoDettaglioAsync(Movimento_Dettaglio movDett, AgronicaCoreParametriServer objParametriServer, bool documentoPrevisionale = false, bool usaDataModifica = false)
        {
            // TODO aggiorna materie prime
            int idMovDet = movDett.Id_Mov_Det;
            if (idMovDet <= 0)
            {
                idMovDet = await _sequenceDal.NuovoId_TabellaAsync("Movimenti_Dettagli", movDett.BaseCode, movDett.TopCode, objParametriServer);
            }
            movDett.Id_Mov_Det = idMovDet;

            if (movDett.Data > DateTime.Now && !documentoPrevisionale)
                movDett.Contabilizzato = -Math.Abs(movDett.Contabilizzato);
            movDett.Data_Modifica = usaDataModifica ? movDett.Data_Modifica : new DateTime(1900,1,2);

            await _movDettagliDal.CreateAsync(movDett.ToWriteMovDettagli(), objParametriServer);
        
            // TODO write dettagli riferiti
            // TODO write dettagli riferimenti
            // TODO write dettagli tecnici extra
            // TODO write dettagli conferimento
            // TODO write dettagli destinazione

            if (movDett.Movimenti_Destinazioni.Any())
            {
                foreach (var dest in movDett.Movimenti_Destinazioni)
                {
                    dest.Id_Agenda = movDett.Id_Agenda;
                    dest.Id_Mov = movDett.Id_Mov;
                    dest.Id_Mov_Det = movDett.Id_Mov_Det;
                    await _movDestinazioni.ScriviMovimentoDestinazioneAsync(dest, objParametriServer);
                }
            }

            if (movDett.Movimenti_Dettagli_Tecnici.Any())
            {
                foreach (var tecnico in movDett.Movimenti_Dettagli_Tecnici)
                {
                    tecnico.Piva = movDett.Piva;
                    tecnico.Sa_Cod = movDett.Sa_Cod;
                    tecnico.Id_Agenda = movDett.Id_Agenda;
                    tecnico.Id_Mov = movDett.Id_Mov;
                    tecnico.Id_Mov_Det = movDett.Id_Mov_Det;
                    tecnico.Data = movDett.Data;
                    tecnico.BaseCode = movDett.BaseCode;
                    tecnico.TopCode = movDett.TopCode;
                    await _movDettTecnico.ScriviMovDettTecnicoAsync(tecnico, objParametriServer);
                }

            }
        }

        public async Task<int> ScriviModificaAsync(WriteMovDettagli dtoMovDettagli, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                bool isNew = false;

                if (dtoMovDettagli.Id_Mov_Det == 0)
                {
                    dtoMovDettagli.Id_Mov_Det = await _sequenceDal.NuovoId_TabellaAsync("movimenti_dettagli", 0, 2000000000, objParametriServer);
                    isNew = true;
                }
                else
                    isNew = !await _movDettagliDal.ExistAsync(dtoMovDettagli.Id_Mov_Det, objParametriServer);

                if (isNew)
                    await _movDettagliDal.CreateAsync(dtoMovDettagli, objParametriServer);
                else
                    await _movDettagliDal.UpdateAsync(dtoMovDettagli, objParametriServer);

                return dtoMovDettagli.Id_Mov_Det;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(WriteMovDettagli dtoMovDettagli, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                return await _movDettagliDal.DeleteAsync(dtoMovDettagli.Piva, dtoMovDettagli.Id_Agenda, dtoMovDettagli.Id_Mov, dtoMovDettagli.Id_Mov_Det, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(List<WriteMovDettagli> MovDettagli, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0)))
            {
                try
                {
                    foreach (var dto in MovDettagli)
                    {
                        await _movDettagliDal.DeleteAsync(dto.Piva, dto.Id_Agenda, dto.Id_Mov, dto.Id_Mov_Det, objParametriServer);
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
