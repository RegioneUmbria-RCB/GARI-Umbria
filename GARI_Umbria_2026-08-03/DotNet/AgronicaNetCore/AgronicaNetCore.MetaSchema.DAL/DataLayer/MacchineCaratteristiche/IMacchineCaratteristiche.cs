using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.MacchineCaratteristiche;

public interface IMacchineCaratteristiche
{
    public Task<DataTable> LeggiAsync(int macCarCod, string classCode, string filtroAggiuntivo, string orderBy,
        AgronicaCoreParametriServer objParametriServer);
}