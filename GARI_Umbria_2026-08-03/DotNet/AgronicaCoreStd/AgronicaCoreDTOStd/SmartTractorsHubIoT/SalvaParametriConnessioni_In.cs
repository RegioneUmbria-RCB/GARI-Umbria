using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.SmartTractors_HubIoT
{
    public class SalvaParametriConnessioni_In
    {
        public List<SalvaParametriConnessioni> parList { get; set; } 
        public SalvaParametriConnessioni_In()
        {
            parList = new List<SalvaParametriConnessioni>();
        }
            
    }

    public class SalvaParametriConnessioni
    {
        public ConnectionConfig pars { get; set; } 
        public Boolean delete { get; set; } 

        public SalvaParametriConnessioni()
        {
            pars = new ConnectionConfig();
            delete = false;
        }
    }
}
