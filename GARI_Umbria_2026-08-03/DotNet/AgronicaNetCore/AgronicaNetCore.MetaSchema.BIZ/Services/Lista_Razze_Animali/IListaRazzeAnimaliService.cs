using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.MetaSchema.BIZ.Services.Lista_Razze_Animali
{
    public interface IListaRazzeAnimaliService
    {
        Task<DataTable> LeggiRazzeAsync(AgronicaCoreParametriServer objParametriServer);
    }
}
