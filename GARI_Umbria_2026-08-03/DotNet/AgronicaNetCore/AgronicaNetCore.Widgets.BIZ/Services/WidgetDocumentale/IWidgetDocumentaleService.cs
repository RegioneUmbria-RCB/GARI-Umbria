using AgronicaCoreDTOStd.InData.ConfrontoCatasto;
using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgronicaCoreDTOStd.InData.Widgets;

namespace AgronicaNetCore.Widgets.BIZ.Services.WidgetStatistiche
{
    public interface IWidgetDocumentaleService
    {
        Task<DataTable?> GetDocumentRecapAsync(DateTime timeStart, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
