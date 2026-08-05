using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Lavorazione
{
    public interface ILavorazione
    {
        Task<string> GetLavDes(int lavCod, AgronicaCoreParametriServer objParametriServer);
    }
}
