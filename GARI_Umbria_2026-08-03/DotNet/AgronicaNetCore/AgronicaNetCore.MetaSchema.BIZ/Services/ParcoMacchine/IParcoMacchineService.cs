using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.BIZ.Services.ParcoMacchine;

public interface IParcoMacchineService
{
    Task<DataTable> ParcoMacchine_LeggiAsync(string piva, AgronicaCoreParametriServer objParametriServer,
        AgronicaCoreParametriUtenti objParametriUtenti);
}