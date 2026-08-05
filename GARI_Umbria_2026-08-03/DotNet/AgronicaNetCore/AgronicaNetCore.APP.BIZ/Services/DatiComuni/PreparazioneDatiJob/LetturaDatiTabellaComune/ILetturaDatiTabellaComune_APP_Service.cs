using AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.Enums;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;

namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune
{
    public interface ILetturaDatiTabellaComuneAPPService
    {
        Task<object> LeggiAsync(TabellaComune tabella, LetturaTabellaComuneAppParameters parameters);
    }
}
