using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Imprese
{
    public interface IImpresaService
    {
        Task<AgronicaCoreModelsSTD.anagrafiche.Impresa?> LeggiImpresaAsync(AgronicaCoreParametriServer objParametriServer, string piva);
        Task<DataTable> Leggi2Async(AgronicaCoreParametriServer objParametriServer, string partiIva);
        Task<DataTable> LeggiPadriAsync(AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiImpreseAsync(string piva, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> TestClausolaINAsync(List<string> elencoPiva, List<int> elencoVegCod, int varieta, AgronicaCoreParametriServer objParametriServer);
    }
}
