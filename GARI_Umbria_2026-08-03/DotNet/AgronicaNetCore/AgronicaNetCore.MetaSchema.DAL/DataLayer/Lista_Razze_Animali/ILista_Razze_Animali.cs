using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Lista_Razze_Animali
{
    public interface ILista_Razze_Animali
    {
        Task<DataTable> LeggiRazzeAsync(AgronicaCoreParametriServer objParametriServer);
    }
}
