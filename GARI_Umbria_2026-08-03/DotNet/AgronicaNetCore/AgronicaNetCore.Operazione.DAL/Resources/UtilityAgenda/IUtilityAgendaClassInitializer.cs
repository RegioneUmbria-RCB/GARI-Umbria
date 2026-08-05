using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Operazione.DAL.Resources.UtilityAgenda
{
    public interface IUtilityAgendaClassInitializer
    {
        InfoOperazione GetInfoOperazione(int lavCod, Attivita.Tipo_Attivita tipoAttivita);
    }
}
