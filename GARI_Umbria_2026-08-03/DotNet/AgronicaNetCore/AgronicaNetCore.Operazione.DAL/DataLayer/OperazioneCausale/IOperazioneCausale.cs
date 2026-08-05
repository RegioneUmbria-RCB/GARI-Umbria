using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.OperazioneCausale;

public interface IOperazioneCausale
{
    public Task<DataTable> LeggiAsync(int lavCod, DateTime? data, AgronicaCoreParametriServer objParametriServer);
    public Task<DataTable> Leggi_CausaleDes_From_CausaleId_DALAsync(int id, AgronicaCoreParametriServer objParametriServer);
    public Task<bool> ScriviAsync(int id, string causale, int lavCod, int inviato, DateTime validitaInizio, DateTime validitaFine, AgronicaCoreParametriServer objParametriServer);
    public Task<bool> ModificaAsync(int id, string causale, int lavCod, int inviato, DateTime validitaInizio, DateTime validitaFine, AgronicaCoreParametriServer objParametriServer);
    public Task<bool> CancellaAsync(int id, int lavCod, AgronicaCoreParametriServer objParametriServer);
}
