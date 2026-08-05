using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Utenti.BIZ.Services.UtentiVisibilitaAppoggio
{
    public interface IUtentiVisibilitaAppoggioService
    {
        Task<DataTable?> ReadAsync(int entity, AgronicaCoreParametriServer objParametriServer);
    }
}
