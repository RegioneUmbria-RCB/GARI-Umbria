using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaNetCore.Base.Models;
using InData.Zoo;

namespace AgronicaNetCore.Magazzino.BIZ.Services
{
    public interface IMagazzinoService
    {
        public Task<List<DettaglioRegistroSomministrazioni>> Leggi_Giacenze_Farmaci(
            LeggiGiacenzaFarmaci paramsFarmaci,
            AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita tipoAttivita,
            AgronicaCoreModelsSTD.attivita.Attivita.Stati statoAttivita,
            bool escludiGiacenzeZero,
            List<string> codiciAIC,
            AgronicaCoreParametriSuperServer? objParametriSuperServer, 
            AgronicaCoreParametriServer objParametriServer, 
            AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
