using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.PrincipiAttivi
{
    public interface IPrincipiAttivi
    {
        Task<DataTable> LeggiPrincipiAttiviAsync(AgronicaCoreParametriServer objParametriServer);
    }
}
