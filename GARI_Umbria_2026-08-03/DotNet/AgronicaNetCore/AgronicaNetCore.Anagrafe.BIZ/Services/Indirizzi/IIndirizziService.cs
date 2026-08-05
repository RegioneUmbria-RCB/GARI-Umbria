using System.Data;
using AgronicaCoreDTOStd.InData.Anagrafica;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Indirizzi
{
    public interface IIndirizziService
    {
        Task<List<IndirizzoAssociato>> LeggiIndirizzixEntitaAsync(
            LeggiIndirizzi parametri,
            AgronicaCoreParametriServer objParametriServer
        );
    }
}
