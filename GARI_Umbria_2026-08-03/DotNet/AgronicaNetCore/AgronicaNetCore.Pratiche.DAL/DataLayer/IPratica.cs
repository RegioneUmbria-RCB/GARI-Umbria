using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Pratiche.DAL.DataLayer
{
    public interface IPratica
    {
        Task<bool> IsServizioBluArancioAsync(int codiceServizio, DateTime dataInizio, DateTime dataFine, AgronicaCoreParametri objP);
        Task<DataTable> ReadPraticheSottoscrizioneQdCAsync(int codiceServizio, int codiceStato, DateTime dataInizio, DateTime dataFine, AgronicaCoreParametri objP);
        Task<DataTable> ReadStatoAttualePraticheAsync(int codiceServizio, DateTime dataInizio, DateTime dataFine, AgronicaCoreParametri objP);

        Task<DataTable> ReadPraticheStatisticheAsync(int codiceServizio, int codiceStato, DateTime dataInizio, DateTime dataFine, AgronicaCoreParametri objP);

    }
}
