using AgronicaCoreDTOStd.Identity;
using AgronicaCoreModelsSTD.attivita;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.Carichi
{
    public interface ICarichiService
    {
        Task<(List<MovimentoDiMagazzino> movimenti, List<Acquisto> acquisti)> LeggiCarichiEAcquistiPerAppAsync(string piva, DateTime dataRiferimento, DateTime dataUltimaSincro, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti);
        Task<string> LeggiGiacenzeAsync(string piva, DateTime data, ObjParametri objParametri, AgronicaCoreParametriTriple tripleParams, string bearerToken, string coreWsUrl);
        Task<string> LeggiProdottiAsync(string piva, DateTime data, ObjParametri objParametri, AgronicaCoreParametriTriple tripleParams, string bearerToken, string coreWsUrl, EnumCategorieMagazzino categoriaMagazzino, bool metaschema, bool giacenza = true);
    }
}
