using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Contatti;

public interface IContatti
{
    public Task<DataTable> LeggiAsync(string visibilityFilter, AgronicaCoreParametriServer objParametriServer);

    public Task<DataTable> LeggiFromCodRisUmAsync(string codRisUm,string piva, AgronicaCoreParametriServer objParametriServer);

    public Task<DataTable> LeggiListaDiContattiAsync(List<int> codRisUmList, AgronicaCoreParametriServer objParametriServer);
}