using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Operazione.BIZ.Services.ActivityImport
{
    public interface IActivityImportService
    {
        Task<OutData.ActivityImport.ActivityImportResult> ImportExternalActivity(
            InData.ActivityImport.ActivityImportJsonObject importJson,
            enum_Esportazioni_Sistema_Cod sysCod,
            AgronicaCoreParametriServer objParametriServer);
    }
}
