using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazione;

public interface IOperazione
{
    public Task<string?> LavorazioneDesFromLavorazioneCodAsync(int lavCod,
        AgronicaCoreParametriServer objParametriServer);

    public Task<DataTable> LeggiAsync(int lavCod, string tipo, string xOrderBy, AgronicaCoreParametriServer objParametriServer);
}