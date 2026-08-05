using AgronicaNetCore.Utility.BIZ.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Utility.BIZ.Services.Firma
{
    public interface IFirmaProvider
    {
        string Name { get; }

        Task<byte[]?> SignAsync(
            string jsonContent,
            bool timeStampLevelEnabled,
            ConfigFirmaDigitale config);
    }
}
