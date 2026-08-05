using AgronicaCoreDTOStd.InData.Anagrafica;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System.Data;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.CodiciAnagrafe
{
    public interface ICodiciAnagrafeService
    {
        Task<List<CodiceAnagrafeBase>> LeggiCodiciUsatixEntitaAsync(LeggiCodiciUsatixEntitaAnagrafe_IN LeggiCodiciUsatixEntitaAnagrafe, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> LeggiCodiciUsatixEntitaDestinazioniDUsoAsync(AgronicaCoreParametriServer objParametriServer);

        Task<List<CodiciAnagrafeValori>> LeggiTuttiCodiciAnagrafexEntita(LeggiCodiciAnagrafe parametri, List<int> idCods, AgronicaCoreParametriServer objParametriServer);
    }
}
