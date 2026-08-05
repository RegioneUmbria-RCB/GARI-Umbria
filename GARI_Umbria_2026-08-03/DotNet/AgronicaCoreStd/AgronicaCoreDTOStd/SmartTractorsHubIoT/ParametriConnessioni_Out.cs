using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.SmartTractors_HubIoT
{
    public class ParametriConnessioni_Out
    {
        public List<ConnectionConfig> pars { get; set; } = new List<ConnectionConfig>();

        public ParametriConnessioni_Out()
        {
            pars = new List<ConnectionConfig>();
        }
    }
}
