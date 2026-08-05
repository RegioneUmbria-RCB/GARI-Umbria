using System;
using System.Data;

namespace InData.Zoo
{
    public class WriteRicetteZoo
    {
        public int IdRicetta { get; set; }
        public string Numero { get; set; }
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        public int? Sta_Num { get; set; }
        
        public string Note { get; set; }
        public string Pin { get; set; }
        public DateTime? DataEmissione { get; set; }
        public int? StatoCodice { get; set; }
        public int? TipoCodice { get; set; }
        public string ProprietarioIdFiscale { get; set; }
        public string DetentoreIdFiscale { get; set; }
        public string VeterinarioIdFiscale { get; set; }
        public string StrutturaCodice { get; set; }
        public string StrutturaDenominazione { get; set; }
        public int? RigaCardinalita { get; set; }
        public int? Id_Protocollo { get; set; }
        public string ProtocolloCodice { get; set; }
        public int? Gruppo_Ricetta { get; set; }

        public int? Inviato { get; set; }
        public DateTime? DataInvio { get; set; }
        public int? Blocco_Flag { get; set; }
        public DateTime? Blocco_Data { get; set; }
        public string Blocco_Username { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteRicetteZoo() { }

        public WriteRicetteZoo(string piva, int saCod, int staNum, int idRicetta)
        {
            this.Piva = piva;
            this.Sa_Cod = saCod;
            this.Sta_Num = staNum;
            this.IdRicetta = idRicetta;
        }

        public WriteRicetteZoo(string piva, int saCod, int staNum, int idRicetta, string numero)
        {
            this.Piva = piva;
            this.Sa_Cod = saCod;
            this.Sta_Num = staNum;
            this.IdRicetta = idRicetta;
            this.Numero = numero;
        }
    }

    public class RicetteZooRow : DataRowWrapper
    {
        public RicetteZooRow(DataRow row) : base(row) { }
    }
}
