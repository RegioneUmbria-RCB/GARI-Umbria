using AgronicaCoreDTOStd.InData.ConfrontoCatasto;
using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.PianoColturale
{
    public interface IPianoColturaleService
    {
        Task<DataTable?> GetPlanningsAsync(String piva, AgronicaCoreParametriServer objParametriServer);
    }
}
