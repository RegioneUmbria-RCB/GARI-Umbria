using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP
{
    public sealed record LetturaTabellaComuneAppParameters(
        AgronicaCoreParametriTriple ObjParametriTriple,
        string? BearerToken = null,
        string? coreWsUrl = null
    );
}
