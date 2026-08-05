using AgronicaNetCore.Base.Models;
using InData.Log.AgronicaLogInvio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogInvio.AgronicaLogInvioAnagrafe
{
    public interface IAgronicaLogInvioAnagrafe
    {
        Task<bool> WriteAsync(WriteAgronicaLogInvioAnagrafe writeModel, AgronicaCoreParametriServer objParametriServer);
    }
}
