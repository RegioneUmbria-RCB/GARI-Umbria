using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Trattamenti
{
    public interface ITrattamenti
    {
        public Task<DataTable> LeggiTrattamentiPerAvversitaAsync(string Piva, int Sa_Cod, int APPEZZA, int ID_REG, int Av_Cod, int Intervallo, AgronicaCoreParametriServer objParametriServer);
    }
}
