using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.UtilityDB.DAL.DataLayer.Utenti_CodiciGiasPro
{
    public interface IUtenti_CodiciGiasPro
    {
        public Task <int> LeggiProgressivoGIASAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);
    }
}
