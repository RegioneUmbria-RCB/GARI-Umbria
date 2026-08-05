using AgronicaCoreModelsSTD.attivita;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.DAL.DataLayer
{
    public interface ICarichiPerAppDal
    {
        Task<List<MovimentoDiMagazzino>> LeggiCarichiPerAppAsync(string piva, DateTime dataLavorazioneMin, DateTime dataUltimaSincro, AgronicaCoreParametriServer objParametriServer);
        Task<List<Acquisto>> LeggiAcquistiPerAppAsync(string piva, DateTime dataLavorazioneMin, DateTime dataUltimaSincro, AgronicaCoreParametriServer objParametriServer);
    }
}
