using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazioni
{
    public interface IOperazioniCausali_APP
    {
        Task<object> LeggiAsync(AgronicaCoreParametriServer objParametriServer);
    }
}
