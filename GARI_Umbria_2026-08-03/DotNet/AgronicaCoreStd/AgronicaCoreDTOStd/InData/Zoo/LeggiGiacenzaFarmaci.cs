using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Zoo
{
    public class LeggiGiacenzaFarmaci
    {
        public string Piva { get; set; }
        public int SaCod { get; set; }
        public string codiceBDN { get; set; }
        public int UdmCod { get; set; }
        public string codFiscaleProprietario { get; set; }
        public int ProCod { get; set; }
        public string[] codiceAIC { get; set; }
        public DateTime ValiditaFine { get; set; }
    }
}