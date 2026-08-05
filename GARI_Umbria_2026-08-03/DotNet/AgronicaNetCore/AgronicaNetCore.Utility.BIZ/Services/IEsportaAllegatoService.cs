using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Options;
using OutData.Varie;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Utility.BIZ.Services
{
    public interface IEsportaAllegatoService
    {
        Task<string?> EsportaExcelPath(DataTable data, string nomefile, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer);

        Task<string?> EsportaExcelPath(DataTable data, string nomefile, bool applyFormating, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer);

        Task<objAllegato?> ImpostaObjAllegato(string pathFile, AgronicaCoreParametri objParametriServer);
    }
}
