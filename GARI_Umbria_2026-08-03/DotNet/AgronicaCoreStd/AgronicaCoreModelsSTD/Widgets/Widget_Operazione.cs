using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Widgets
{
    public class Widget_Operazione
    {
        public string Piva { get; set; }

        public string Rag_Soc { get; set; }

        public int IdAgenda { get; set; }

        public int LavCod { get; set; }

        public string Descrizione_Operazione { get; set; }

        public string Specie { get; set; }
        public DateTime Data_Operazione { get; set; }
        public List<string> Impianti { get; set; }
        public List<ImpiantiMappe> ImpiantiMappe { get; set; }

        public List<string> Macchine { get; set; }

        public List<string> Personale { get; set; }

        public string Note { get; set; }

        public List<string> AttivitaSvolte { get; set; }

        public Operazione Operazione { get; set; }

        public Widget_Operazione() 
        {
            this.Impianti = new List<string>();
            this.ImpiantiMappe = new List<ImpiantiMappe>();
            this.Macchine = new List<string>();
            this.Personale = new List<string>();
            this.AttivitaSvolte = new List<string>();
        }

    }

    public class Operazione
    {
        public string Visualizza { get; set; }
        public string Modifica { get; set; }

        public bool PermessoScrittura { get; set; }
    }

    public class ImpiantiMappe
    { 
        public string APP_Nome { get; set; }
        public string StaticMap { get; set; }
    }

    public class Widget_Coltura
    {
        public int Veg_Cod { get; set; }
        public string Veg_Des { get; set; }
        public double Superficie { get; set; }

        public string Udm_Sim { get; set; }

    }

    public class Widget_ProduzioneColtura
    {
        public int Veg_Cod { get; set; }
        public string Veg_Des { get; set; }
        public double Superficie { get; set; }
        public double PercentualeSeminata { get; set; }
        public double PercentualeRaccolta { get; set; }

    }

    public class Widget_GHGColture
    {
        public int Veg_Cod { get; set; }
        public string Veg_Des { get; set; }
        public double Eec_Ha { get; set; }
        public double Eec_Totale { get; set; }

    }

    public class Widget_StimeProduzioneColture
    {
        public int Veg_Cod { get; set; }
        public string Veg_Des { get; set; }
        public double StimaProduzione_Ha { get; set; }
        public double StimaProduzione_Totale { get; set; }

    }

    public class PrevisioniChatGPT_IN
    {
        public int year { get; set; }
        public Widget_PrevisioniAI[] arrayCrops { get; set; }
    }

    public class Widget_PrevisioniAI
    {
        //public string Piva { get; set; }
        //public string Rag_Soc { get; set; }
        public string Localita { get; set; }
        public string Stato { get; set; }
        public string SpecieVegetale { get; set; }
        public string Cultivar { get; set; }
        public string NomeApp { get; set; }
        public string SuperficieApp { get; set; }
        public string AgricolturaBio { get; set; }
    }

    public class Balance { 
        public int year { get; set; }
        public double costs { get; set; }
        public double revenues { get; set; }
        public string explanation { get; set; }
    }

    public class CostsCrop {
        public string id { get; set; }
        public string description { get; set; }
        public double costs { get; set; }
        public string explanation { get; set; }
    }

    public class RevenuesCrop {
        public string id { get; set; }
        public string description { get; set; }
        public double revenues { get; set; }
        public string explanation { get; set; }
    }

    public class CostsRevenuesTot {
        public Balance[] Balances { get; set; }
        public CostsCrop[] CostsCrops { get; set; }
        public RevenuesCrop[] RevenuesCrops { get; set; }
    }

}
