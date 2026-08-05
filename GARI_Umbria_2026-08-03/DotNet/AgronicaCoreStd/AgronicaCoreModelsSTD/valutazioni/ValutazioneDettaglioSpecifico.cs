using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.valutazioni
{
    public class Valutazione_Dettaglio_Specifico
    {
        public string Piva { get; set; }
        public int Id_Testata { get; set; }
        public int Valutazione_Conto_Cod { get; set; }
        public int Anno { get; set; }
        public string Dettaglio_Key { get; set; }
        public string Descrizione { get; set; }
        public decimal Valore_Unitario { get; set; }
        public decimal Valore_Totale { get; set; }
        public decimal Valore_Ha { get; set; }
        public decimal Valore_Peso { get; set; }
        public int TipoOperazioneDB { get; set; }


        public Valutazione_Dettaglio_Specifico() { }

    }



    public class Valutazione_Dettaglio_Specifico_Arete
    {
        public string Piva { get; set; }
        public int Id_Testata { get; set; }
        public int Valutazione_Conto_Cod { get; set; }
        public int Anno { get; set; }
        public string Dettaglio_Key { get; set; }
        public string Descrizione { get; set; }
        public decimal Valore_Unitario { get; set; }
        public decimal Valore_Totale { get; set; }
        public decimal Valore_Ha { get; set; }
        public decimal Valore_Peso { get; set; }
        public int TipoOperazioneDB { get; set; }
        public string jSon_Arete { get; set; }


        public Valutazione_Dettaglio_Specifico_Arete() { }

    }

}
