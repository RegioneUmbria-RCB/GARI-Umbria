using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Data;

namespace OutData.Zoo
{
    public class ProtocolloInCorso
    {
        public int IdProt { get; set; }
        public string Numero { get; set; }

        public string Piva { get; set; }
        public string Impresa { get; set; }
        public int SaCod { get; set; }
        public string SaDes { get; set; }
        public int StaNum { get; set; }
        public string StaDes { get; set; }
        public int RaggrCod { get; set; }
        public string RaggrDes { get; set; }

        public string FamigliaAic { get; set; }
        public string FarmacoDes { get; set; }

        public CapoAnimale Capo { get; set; }
        public double PesoStimato { get; set; }

        public ProtocolloInCorso()
        {
            Capo = new CapoAnimale();
        }

        public ProtocolloInCorso(int idProt, string numero, string piva, string impresa, int saCod, string saDes, int staNum, string staDes, int raggrCod, string raggrDes, string famigliaAic, string farmacoDes, CapoAnimale capo, double pesoStimato)
        {
            this.IdProt = idProt;
            this.Numero = numero;
            this.Piva = piva;
            this.Impresa = impresa;
            this.SaCod = saCod;
            this.SaDes = saDes;
            this.StaNum = staNum;
            this.StaDes = staDes;
            this.RaggrCod = raggrCod;
            this.RaggrDes = raggrDes;
            this.FamigliaAic = famigliaAic;
            this.FarmacoDes = farmacoDes;
            this.Capo = capo;
            this.PesoStimato = pesoStimato;
        }

        //public DataRow ToDataRow()
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("IdProt", typeof(int));
        //    dt.Columns.Add("Numero", typeof(string));
        //    dt.Columns.Add("Piva", typeof(string));
        //    dt.Columns.Add("Impresa", typeof(string));
        //    dt.Columns.Add("SaCod", typeof(int));
        //    dt.Columns.Add("SaDes", typeof(string));
        //    dt.Columns.Add("StaNum", typeof(int));
        //    dt.Columns.Add("StaDes", typeof(string));
        //    dt.Columns.Add("RaggrCod", typeof(int));
        //    dt.Columns.Add("RaggrDes", typeof(string));
        //    dt.Columns.Add("FamigliaAic", typeof(string));
        //    dt.Columns.Add("FarmacoDes", typeof(string));
        //    dt.Columns.Add("CapoId", typeof(int));
        //    dt.Columns.Add("CapoCodice", typeof(string));
        //    dt.Columns.Add("CapoSpecie", typeof(string));
        //    dt.Columns.Add("CapoRazza", typeof(string));
        //    dt.Columns.Add("CapoSesso", typeof(string));
        //    dt.Columns.Add("CapoDataNascita", typeof(DateTime));
        //    dt.Columns.Add("PesoStimato", typeof(double));
        //    DataRow row = dt.NewRow();
        //    row["IdProt"] = this.IdProt;
        //    row["Numero"] = this.Numero;
        //    row["Piva"] = this.Piva;
        //    row["Impresa"] = this.Impresa;
        //    row["SaCod"] = this.SaCod;
        //    row["SaDes"] = this.SaDes;
        //    row["StaNum"] = this.StaNum;
        //    row["StaDes"] = this.StaDes;
        //    row["RaggrCod"] = this.RaggrCod;
        //    row["RaggrDes"] = this.RaggrDes;
        //    row["FamigliaAic"] = this.FamigliaAic;
        //    row["FarmacoDes"] = this.FarmacoDes;
        //    row["CapoId"] = this.Capo.Id;
        //    row["CapoCodice"] = this.Capo.Codice;
        //    row["CapoSpecie"] = this.Capo.Specie;
        //    row["CapoRazza"] = this.Capo.Razza;
        //    row["CapoSesso"] = this.Capo.Sesso;
        //    row["CapoDataNascita"] = this.Capo.DataNascita;
        //    row["PesoStimato"] = this.PesoStimato;
        //    return row;
        //}
    }
}
