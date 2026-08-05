using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Base.DataLayer.LastCUAA
{
    public interface ILastCUAA
    {
        Task<string> ReadCuaaAsync(AgronicaCoreParametriServer objParametriServer);

        [Obsolete("Usare la versione asincrona del metodo. Questo metodo è presente per essere usato solo dal logger")]
        string ReadCuaaForLog(AgronicaCoreParametriServer objParametriServer);
    }
}
