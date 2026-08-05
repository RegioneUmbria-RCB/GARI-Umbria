using System;
using System.Collections.Generic;
using System.Text;

namespace OutData.Infragri
{
    public class Dispositivo
    {
        public string codiceContratto {  get; set; }
        public string codiceAzienda { get; set; }
        public string comuneInstallazione { get; set; }
        public string codiceModelloDispositivo { get; set; }
        public string noteInstallazione { get; set; }
        public DateTimeOffset dataOraInstallazione { get; set; }
        public double latitudineInstallazione { get; set; }
        public double longitudineInstallazione { get; set; }
        public string serialNumberDispositivo { get; set; }
    }
}
