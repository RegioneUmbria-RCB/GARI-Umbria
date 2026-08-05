using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SmartTractor.DAL.Attivita;

public interface IPrescription
{
    Task<DataTable> GetPrescriptionTypeAsync(int ricettaOperazioneCod, AgronicaCoreParametriServer serverParams);

    Task<DataTable> GetPrescriptionDetailsAsync(int ricettaOperazioneCod, int? elemCod, AgronicaCoreParametriServer serverParams);

    Task<DataTable> GetPrescriptionDestinationsAsync(int ricettaOperazioneCod, int? tipoDestinazione, AgronicaCoreParametriServer serverParams);
}

