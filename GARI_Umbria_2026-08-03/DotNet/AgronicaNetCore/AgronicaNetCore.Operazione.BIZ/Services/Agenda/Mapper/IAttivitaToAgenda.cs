using AgronicaNetCore.Base.Models;
using AgronicaCoreDTOStd.InData.Agenda.OperazioneAgenda_Temp;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Operazione.BIZ.Services.Agenda.Mapper
{
    public interface IAttivitaToAgenda
    {
        Task<OperazioneAgenda> MapAttivitaToAgenda(AgronicaCoreModelsSTD.attivita.Attivita attivita, AgronicaCoreParametriServer objParametriServer);


    }
}
