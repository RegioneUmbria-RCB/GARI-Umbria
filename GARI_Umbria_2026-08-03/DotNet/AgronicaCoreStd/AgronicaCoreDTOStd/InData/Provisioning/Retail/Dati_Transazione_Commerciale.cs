using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Provisioning.Retail
{
    public class Dati_Transazione_Commerciale
    {
        public string Codice_Transazione { get; set; }
        public string Codice_Servizio { get; set; }
        public string Codice_SDI { get; set; }
        public string PEC { get; set; }
        public int Numero_Licenze { get; set; }
        public string Codice_Coupon { get; set; }
        public string Modalita_Pagamento { get; set; }
        public string Stato_Pagamento { get; set; }
        public double Prezzo { get; set; }
        public int Codice_Prodotto { get; set; }

    }
}
