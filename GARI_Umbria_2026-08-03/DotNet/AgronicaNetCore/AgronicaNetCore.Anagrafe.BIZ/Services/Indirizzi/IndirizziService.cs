using System.Data;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Indirizzi;
using AgronicaNetCore.Anagrafe.DAL.HelpersSTD;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Indirizzi
{
    public class IndirizziService : BaseServiceAnagrafeBIZ, IIndirizziService
    {
        private readonly IIndirizzi _indirizzi;

        public IndirizziService(
            IServiceProvider provider,
            IIndirizzi indirizzi,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _indirizzi = indirizzi;
        }

        public async Task<List<IndirizzoAssociato>> LeggiIndirizzixEntitaAsync(
            LeggiIndirizzi parametri,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            List<IndirizzoAssociato> indirizzi = new();
            DataTable dt;

            switch (parametri.ElementoAnagrafico)
            {
                case EntitaAlberoImprese.Impresa:
                    dt = await _indirizzi.LeggiIndirizziImpresaAsync(
                        parametri.ChiaviImprese.Select(k => k.partitaIva).ToList(),
                        parametri.TipoIndirizzo,
                        parametri.IndirizzoCompleto,
                        objParametriServer
                    );
                    break;
                case EntitaAlberoImprese.Centro:
                    dt = await _indirizzi.LeggiIndirizziCentroAsync(
                        parametri
                            .ChiaviCentriAziendali.Select(k => (k.partitaIva, k.codice))
                            .ToList(),
                        parametri.TipoIndirizzo,
                        parametri.IndirizzoCompleto,
                        objParametriServer
                    );
                    break;
                case EntitaAlberoImprese.Appezzamento:
                    dt = await _indirizzi.LeggiIndirizziAppezzamentiAsync(
                        parametri
                            .ChiaviAppezzamenti.Select(k =>
                                (
                                    k.centroAziendalePK.partitaIva,
                                    k.centroAziendalePK.codice,
                                    k.codice
                                )
                            )
                            .ToList(),
                        parametri.TipoIndirizzo,
                        parametri.IndirizzoCompleto,
                        objParametriServer
                    );
                    break;
                case EntitaAlberoImprese.Contatto:
                    dt = await _indirizzi.LeggiIndirizziContattiAsync(
                        parametri
                            .ChiaviContatti.Select(k =>
                                (
                                    k.partitaIva,
                                    int.TryParse(k.codice, out var codContatto) ? codContatto : 0
                                )
                            )
                            .ToList(),
                        parametri.TipoIndirizzo,
                        parametri.IndirizzoCompleto,
                        objParametriServer
                    );
                    break;
                default:
                    throw new NotSupportedException(
                        $"Lettura CodiciAnagrafeValori su elemento anagrafico {parametri.ElementoAnagrafico} non gestito"
                    );
            }
            if (dt.Rows.Count == 0)
                return indirizzi;

            return IndirizzoMapper.MapSTDFromDataTable(dt, parametri.IndirizzoCompleto);
        }
    }
}
