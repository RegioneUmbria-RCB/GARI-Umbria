using AgronicaNetCore.Base.Models;
using OutData.Zoo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.MetaSchema.BIZ.Services.Farmaci
{
    public interface IFarmaciService
    {
        Task<DataTable> LeggiFarmaciAsync(int Farm_Cod, string Aic, AgronicaCoreParametriServer objParametriServer);
    }
}
