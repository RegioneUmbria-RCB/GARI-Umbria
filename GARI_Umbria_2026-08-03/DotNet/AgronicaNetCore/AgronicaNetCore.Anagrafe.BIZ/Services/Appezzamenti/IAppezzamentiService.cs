using AgronicaCoreModelsSTD.metaschema;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Appezzamenti
{
    public interface IAppezzamentiService
    {
        Task<List<(string, List<Comune>)>> GetAppezzamentixParticelleAsync(List<(string, int, int)> chiaviAppezzamento, AgronicaCoreParametriServer objParametriServer);    }

}
