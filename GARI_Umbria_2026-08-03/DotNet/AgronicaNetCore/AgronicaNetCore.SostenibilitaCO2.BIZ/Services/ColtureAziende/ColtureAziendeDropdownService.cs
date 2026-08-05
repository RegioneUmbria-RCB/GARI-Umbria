using AgronicaNetCore.SostenibilitaCO2.BIZ.Resources;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.GerarchiaImprese;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impianti;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.ColtureAziende
{
    /// <summary>
    /// Returns the distinct crop species (colture) present across all companies
    /// that belong to the requested filiere.
    /// </summary>
    /// <remarks>
    /// DS-16 — <c>GET v1/sostenibilita/colture-aziende</c>.<br/>
    /// For each supplied filiera PIVA the company hierarchy is resolved recursively;
    /// then a single batched query retrieves the distinct species.
    /// </remarks>
    public class ColtureAziendeDropdownService : BaseServiceSostenibilitaCO2Biz, IColtureAziendeDropdownService
    {
        private readonly IGerarchiaImprese _gerarchiaImprese;
        private readonly IImpianti _impianti;

        /// <inheritdoc/>
        public ColtureAziendeDropdownService( IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _gerarchiaImprese = _serviceProvider.GetRequiredService<IGerarchiaImprese>();
            _impianti = _serviceProvider.GetRequiredService<IImpianti>();
        }

        /// <inheritdoc/>
        public async Task<ColtureAziendeDropdownResult> GetColtureAsync(IEnumerable<string> piveFiliere, AgronicaCoreParametriServer objParametriServer)
        {
            if (piveFiliere is null) throw new ArgumentNullException(nameof(piveFiliere));

            var filiereList = piveFiliere.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().ToList();
            if (!filiereList.Any())
                return new ColtureAziendeDropdownResult();

            // DS-16 §Logica — resolve all child company PIVAs for every supplied filiera.
            var allAziendePivas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var pivaFiliera in filiereList)
            {
                var figlie = await _gerarchiaImprese.LeggiElencoGerarchiaImpreseFiglieAsync(pivaFiliera, objParametriServer);
                foreach (var piva in figlie)
                    allAziendePivas.Add(piva);
            }

            if (!allAziendePivas.Any())
                return new ColtureAziendeDropdownResult();

            // Batch query: distinct species across all resolved companies.
            DataTable coltureDt = await _impianti.LeggiColturexPivaAsync(allAziendePivas, objParametriServer);

            var colture = coltureDt.AsEnumerable()
                .Select(row => new ColturaDropdownItem
                {
                    VegCod = row.Field<int>("Veg_Cod"),
                    VegDes = row.Field<string>("Veg_Des") ?? string.Empty
                })
                .OrderBy(c => c.VegDes)
                .ToList();

            return new ColtureAziendeDropdownResult { Colture = colture };
        }
    }
}
