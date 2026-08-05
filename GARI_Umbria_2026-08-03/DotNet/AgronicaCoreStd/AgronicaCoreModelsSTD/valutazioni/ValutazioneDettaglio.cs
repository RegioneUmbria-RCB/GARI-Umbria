using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.valutazioni
{
    public class Valutazione_Dettaglio
    {
        public string Piva { get; set; }
        public int Id_Testata { get; set; }
        public int Valutazione_Conto_Cod { get; set; }
        public int Anno { get; set; }
        public int Ordine { get; set; }
        public decimal Valore { get; set; }
        public List<Valutazione_Dettaglio_Specifico> Valutazione_Dettaglio_Specifico { get; set; }
        public int TipoOperazioneDB { get; set; }


        public Valutazione_Dettaglio()
        {
            Valutazione_Dettaglio_Specifico = new List<Valutazione_Dettaglio_Specifico>();
        }

    }

}