using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Utility.BIZ.Services.Firma
{
    public interface IFirmaDigitaleService
    {
        public Task<byte[]?> SignAsync(string jsonContent, bool timeStampLevelEnabled, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer);
    }
}
