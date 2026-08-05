using InData.Agenda;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Operazione.DAL.DataLayer.Mov_Dettaglio_Tecnico;
using Microsoft.Extensions.DependencyInjection;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Operazione.BIZ.Resources;
using InData.Anagrafica;

namespace AgronicaNetCore.Operazione.BIZ.Services.Mov_Dettaglio_Tecnico
{
    public class MovDettTecnicoService : BaseServiceOperazioneBIZ, IMovDettTecnicoService
    {
        private readonly IMov_Dettaglio_Tecnico _movDettTecDal;
        private readonly IAgro_Sequence _sequenceDal;

        public MovDettTecnicoService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _movDettTecDal = provider.GetRequiredService<IMov_Dettaglio_Tecnico>();
            _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
        }

        public async Task<int> ScriviModificaAsync(WriteMovDettTecnico dtoMovDettTec, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                bool isNew = false;

                if (dtoMovDettTec.Id_Reg_Dettaglio == 0)
                {
                    dtoMovDettTec.Id_Reg_Dettaglio = await _sequenceDal.NuovoId_TabellaAsync("movimenti_dettagli_tecnici", 0, 2000000000, objParametriServer);
                    isNew = true;
                }
                else
                    isNew = !await _movDettTecDal.ExistAsync(dtoMovDettTec.Piva, dtoMovDettTec.Id_Agenda, dtoMovDettTec.Id_Mov, dtoMovDettTec.Id_Mov_Det, dtoMovDettTec.Id_Reg_Dettaglio, objParametriServer);

                if (isNew)
                    await _movDettTecDal.CreateAsync(dtoMovDettTec, objParametriServer);
                else
                    await _movDettTecDal.UpdateAsync(dtoMovDettTec, objParametriServer);

                return dtoMovDettTec.Id_Reg_Dettaglio;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task ScriviMovDettTecnicoAsync(
            Movimento_Dettaglio_Tecnico tecnico, AgronicaCoreParametriServer objParametriServer)
        {
            await ScriviModificaAsync(BuildWriteMovDettTecnico(tecnico), objParametriServer);
        }

        public static WriteMovDettTecnico BuildWriteMovDettTecnico(Movimento_Dettaglio_Tecnico tecnico)
        {
            return new WriteMovDettTecnico
            {
                Piva = tecnico.Piva,
                Sa_Cod = tecnico.Sa_Cod,
                Id_Agenda = tecnico.Id_Agenda,
                Id_Mov = tecnico.Id_Mov,
                Id_Mov_Det = tecnico.Id_Mov_Det,
                Id_Reg_Dettaglio = tecnico.Id_Reg_Dettaglio,
                Qta_Ril = (double?)tecnico.Qta_Ril,
                Av_Cod = tecnico.Av_Cod,
                Av_Gru = tecnico.Av_Gru,
                FF_Classe = tecnico.ff_classe,
                Dett_Cod = tecnico.dett_cod
            };
        }

        public async Task<bool> EliminaAsync(WriteMovDettTecnico dtoMovDettTec, AgronicaCoreParametriServer objParametriServer)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> EliminaAsync(List<WriteMovDettTecnico> MovDettTec, AgronicaCoreParametriServer objParametriServer)
        {
            throw new NotImplementedException();
        }
    }
}
