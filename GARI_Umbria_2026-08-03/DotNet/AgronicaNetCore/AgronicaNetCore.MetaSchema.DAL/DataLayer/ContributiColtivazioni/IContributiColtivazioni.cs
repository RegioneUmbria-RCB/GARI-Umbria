using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.ContributiColtivazioni
{
    public interface IContributiColtivazioni
    {
        Task<DataTable> LeggiDescrizioniAsync(List<string> codici, AgronicaCoreParametriServer objParametriServer);
    }
}
