using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.GruppiOperazione;

public interface IGruppiOperazione
{
    Task<DataTable> GruppiOperazione_LeggiAsync(AgronicaCoreParametriServer objParametriServer);
}