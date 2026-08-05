using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Widgets.DAL.DataLayer.WidgetsZoo
{
    public interface IWidgetsZoo
    {
        
        Task<DataTable?> GetTrattamentiToSend(string piva, int saCod, int staNum, DateTime? inizio, DateTime? fine, AgronicaCoreParametri objP_Server, int codAnimale = 0, string matricola = "", string xFiltroAggiuntivo = "", bool filtroVisibilitaUtente = false);

    }
}
