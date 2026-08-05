using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.DomandaIrrigua
{
    public class CalcoloTariffazioneParametri
    {
        public int anno { get; set; }
        public decimal spesatotale { get; set; }
        public decimal spesaquotafissa { get; set; }
        public decimal incrementofascia2 { get; set; }
        public decimal incrementofascia3 { get; set; }
    }

    public class RisultatoTariffazione
    {
        public List<RisultatoTariffazioneAzienda> tariffazioneAzienda { get; set; }
    }

    public class RisultatoTariffazioneAzienda
    {
        public int IdDomanda { get; set; }
        public string RagioneSociale { get; set; }
        public string piva { get; set; }
        public string PivaReale { get; set; }
        public string cuaa { get; set; }
        public string CodiceSDI { get; set; }
        public decimal SuperficieTotale { get; set; }
        public decimal VolumeTotale { get; set; }
        public decimal Incidenza { get; set; }
        public decimal IncidenzaFasciaT1 { get; set; }
        public decimal IncidenzaFasciaT2 { get; set; }
        public decimal IncidenzaFasciaT3 { get; set; }
        public decimal VolumeFasciaT1 { get; set; }
        public decimal VolumeFasciaT2 { get; set; }
        public decimal VolumeFasciaT3 { get; set; }
        public decimal QuotaFissa { get; set; }
        public decimal QuotaVariabileT1 { get; set; }
        public decimal QuotaVariabileT2 { get; set; }
        public decimal QuotaVariabileT3 { get; set; }
        public decimal ImponibileQuotaFissa { get; set; }
        public decimal ImponibileQuotaVariabileT1 { get; set; }
        public decimal ImponibileQuotaVariabileT2 { get; set; }
        public decimal ImponibileQuotaVariabileT3 { get; set; }
        public decimal TotaleImponibile { get; set; }

    }

}
