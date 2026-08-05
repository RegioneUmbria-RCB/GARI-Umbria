using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Lista_Razze_Animali;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.MetaSchema.BIZ.Services.Lista_Razze_Animali
{
    public class ListaRazzeAnimaliService : BaseServiceMetaschemaBIZ, IListaRazzeAnimaliService
    {
        private readonly ILista_Razze_Animali _dal;

        public ListaRazzeAnimaliService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _dal = provider.GetRequiredService<ILista_Razze_Animali>();
        }

        public async Task<DataTable> LeggiRazzeAsync(AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                return await _dal.LeggiRazzeAsync(objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
