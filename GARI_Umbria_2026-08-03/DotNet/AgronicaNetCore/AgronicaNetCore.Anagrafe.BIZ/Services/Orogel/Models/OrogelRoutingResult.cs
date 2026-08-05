using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models
{
    public record OrogelRoutingResult(
        int AnnoValidato,
        int IdDbServer,
        AgronicaCoreParametriTriple ObjParametriTriple
    );
}
