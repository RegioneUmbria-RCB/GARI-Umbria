using AgronicaCoreModelsSTD.metaschema;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Impianti
{
    public interface IImpiantiService
    {
        Task<List<Contribute>?> GetContributiACAAsync(AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Restituisce le specie vegetali distinte (Veg_Cod, Veg_Des) presenti nei
        /// <c>Reg_Impianti</c> delle aziende fornite.
        /// </summary>
        Task<DataTable> LeggiColturexPivaAsync(IEnumerable<string> pivas, AgronicaCoreParametriServer objParametriServer);
    }
}
