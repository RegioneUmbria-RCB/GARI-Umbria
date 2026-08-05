using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.ParcoMacchine;

public interface IParcoMacchine
{
    Task<DataTable> ParcoMacchine_LeggiAsync(string piva, string visibilityFilter, string orderByField,
                                             AgronicaCoreParametriServer objParametriServer);

    Task<string?> LeggiDesFromMacCodAsync(string piva, int i, DataTable dtCentriVisibili, AgronicaCoreParametriServer objParametriServer);

    public Task<DataTable> LeggiParcoMacchinexSuperUserAsync(string piva, int macCod, int macCodOrigine, bool ancheImportati,
                                                             string filtroAggiuntivo, string orderBy, DataTable dtCentriVisibili, AgronicaCoreParametriServer objParametriServer);
}