using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Ditte;

public interface IDitte
{
    public Task<DataTable> LeggiAsync(int dittaCod, string tipo, string filtroAggiuntivo, string orderBy,
        AgronicaCoreParametriServer objParametriServer);
}