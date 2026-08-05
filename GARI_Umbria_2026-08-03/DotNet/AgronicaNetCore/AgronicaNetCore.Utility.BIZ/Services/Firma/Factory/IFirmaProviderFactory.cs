using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Utility.BIZ.Services.Firma.Factory
{
    public interface IFirmaProviderFactory
    {
        IFirmaProvider Get(string provider);
    }
}
