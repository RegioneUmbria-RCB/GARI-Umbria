using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SpecieVegetaliDefault.DAL.DataLayer.SpecieVegetaliDefault;

public interface ISpecieVegetaliDefault
{
    public Task<int> CicliCulturaliAsync(string piva, bool verificaPiva, int vegCod, DateTime dtInizio, DateTime dtFine,
        string additionalFilter, AgronicaCoreParametriServer objParametriServer);

    public Task<DataTable> LeggiAsync(string piva, int vegCod, int culCod, int codice, int nCiclo, DateTime dtInizio,
        DateTime dtFine, string additionalFilter, AgronicaCoreParametriServer objParametriServer);

    public Task<bool> ScriviAsync(int id, string piva, int vegCod, int culCod, int codice, string valore, int nCiclo,
        DateTime dtInizio,
        DateTime dtFine, AgronicaCoreParametriServer objParametriServer);

    public Task<bool> CancellaAsync(string piva, int vegCod, int culCod, string additionalFilter, AgronicaCoreParametriServer objParametriServer);
}