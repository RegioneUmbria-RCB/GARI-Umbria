using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace AgronicaCoreDTOStd.OutData.NewAgri
{
    public class ResponseNDistribuito
    {
        public List<Appezzamento> ListaLimitiMas { get; set; }

    }

    public class Appezzamento : AgronicaCoreModelsSTD.NewAgri.Appezzamento
    {      
        public decimal N_FabbisognoSoddisfatto { get; set; }
        public decimal N_Zootecnico { get; set; }
        public decimal N_Zootecnico_Letame { get; set; }
        public decimal N_Zootecnico_Liquame { get; set; }
        public decimal N_BilancioAzotato_Utile { get; set; }
        public decimal N_TotaleSoddisfatto { get; set; }
        public decimal N_BilancioAzotato_Totale { get; set; }
    }
}

