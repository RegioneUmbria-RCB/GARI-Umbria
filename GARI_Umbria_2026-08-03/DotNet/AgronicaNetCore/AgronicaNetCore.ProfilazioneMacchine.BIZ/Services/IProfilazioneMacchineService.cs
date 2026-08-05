
using System.Data;
using AgronicaNetCore.Base.Models;
using InData.ProfilazioneMacchina;

namespace AgronicaNetCore.ProfilazioneMacchine.BIZ.Services;

public interface IProfilazioneMacchineService
{

    public Task<DataTable> LeggiAsync(int idProfilazione, AgronicaCoreParametriServer objParametriServer);
    public Task<bool> UpdateCaratteristicheMacchinaAsync(CaratteristicheMacchina_In body, AgronicaCoreParametriServer objParametriServer);
}