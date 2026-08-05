using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.DomandaIrrigua
{
    public class DomandaIrrigua
    {
        public int id { get; set; }
        public string piva { get; set; }
        public string n_protocollo { get; set; }
        public DateTime data_protocollo { get; set; }
        public int TipoContratto { get; set; }
        public int Stato { get; set; }
        public DateTime ValiditaInizio { get; set; }
        public DateTime ValiditaFine { get; set; }
        public List<DomandaIrriguaRighe> dettaglio { get; set; }
        public DatiAzienda datiAzienda { get; set; }
    }

    public class DatiAzienda{
        public string PivaReale { get; set; }
        public string RagioneSociale { get; set; }
        public string CodiceFiscale { get; set; }
        public string CUAA { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public IndirizzoAzienda indirizzo { get; set; }
        public DatiAmministrativi datiAmministrativi { get; set; }
    }

    public class IndirizzoAzienda
    {
        public string Via { get; set; }
        public string Frazione { get; set; }
        public string Provincia { get; set; }
        public string Comune { get; set; }
        public string CAP { get; set; }
        public string Stato { get; set; }
    }

    public class DatiAmministrativi
    {
        public string Telefono { get; set; }
        public string PEC { get; set; }
        public string SDI { get; set; }
    }

    public class DomandaIrriguaRighe
    {
        public int riga { get; set; }
        public string piva { get; set; }
        public int sa_cod { get; set; }
        public int appezza { get; set; }
        public int id_reg { get; set; }
        public Boolean Selezionato { get; set; }
        public string app_nome { get; set; }

        public decimal Superficie { get; set; }
        public int veg_cod { get; set; }
        public string veg_des { get; set; }
        public int cul_cod { get; set; }
        public string cul_des { get; set; }
        public int gruppo_consegna { get; set; }

        public string PROV { get; set; }
        public string PROV_Des { get; set; }
        public string COM { get; set; }
        public string COM_Des { get; set; }
        public string Sezione { get; set; }
        public string Foglio { get; set; }
        public string Numero { get; set; }

    }
}

