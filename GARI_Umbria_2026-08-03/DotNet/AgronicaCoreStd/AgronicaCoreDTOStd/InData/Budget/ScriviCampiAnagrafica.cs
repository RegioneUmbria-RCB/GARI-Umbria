using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Budget
{
    public class ScriviCampiAnagrafica
    {
        public BudgetAnagrafica<AgronicaCoreModelsSTD.anagrafiche.Campo> InData { get; set; }
        public enum_TipoOperazioneDB tipoOperazione { get; set; }                                       
    }

    public enum enum_TipoOperazioneDB
    {
        Lettura = 0,
        Scrittura = 1,
        Modifica = 2,
        Cancellazione = 3,
        Trasferimento = 4,
        Copia = 10
    }

}
