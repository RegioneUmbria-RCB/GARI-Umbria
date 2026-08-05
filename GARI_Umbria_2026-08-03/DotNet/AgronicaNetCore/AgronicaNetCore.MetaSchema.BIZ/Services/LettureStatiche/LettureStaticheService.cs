using AgronicaNetCore.MetaSchema.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.LettureStatiche;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.MetaSchema.BIZ.Services.LettureStatiche
{
    public class LettureStaticheService : BaseServiceMetaschemaBIZ, ILettureStaticheService
    {
        private readonly ILettureStatiche _lettureStaticheDAL;

        public LettureStaticheService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _lettureStaticheDAL = _serviceProvider.GetRequiredService<ILettureStatiche>();
        }

        public async Task<DataTable> LeggiMetodiDiProduzioneAsync()
        {
            DataTable dt = _lettureStaticheDAL.LeggiMetodiDiProduzione();
            return await Task.FromResult(dt);
        }
    }
}
