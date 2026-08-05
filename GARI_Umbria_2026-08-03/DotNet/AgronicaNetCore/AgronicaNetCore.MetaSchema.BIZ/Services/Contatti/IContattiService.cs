using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.BIZ.Services.Contatti;

public interface IContattiService
{
    Task<DataTable> Contatti_LeggiAsync(string piva, AgronicaCoreParametriServer objParametriServer,
        AgronicaCoreParametriUtenti objParametriUtenti);
}