using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Utility.BIZ.Services.Firma.Factory
{
    public class FirmaProviderFactory : IFirmaProviderFactory
    {
        private readonly IEnumerable<IFirmaProvider> _providers;

        public FirmaProviderFactory(IEnumerable<IFirmaProvider> providers)
        {
            _providers = providers;
        }

        public IFirmaProvider Get(string provider)
        {
            var match = _providers.FirstOrDefault(p =>
                p.Name.Equals(provider, StringComparison.OrdinalIgnoreCase));

            if (match == null)
                throw new NotSupportedException($"Provider {provider} non supportato");

            return match;
        }
    }
}
