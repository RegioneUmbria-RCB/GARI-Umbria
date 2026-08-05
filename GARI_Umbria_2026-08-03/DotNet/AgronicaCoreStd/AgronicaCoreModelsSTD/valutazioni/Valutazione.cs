using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.valutazioni
{
    public class Valutazione
    {
        public string Piva { get; set; }
        public int Id_Testata { get; set; }
        public int Valutazione_Conto_Cod { get; set; }
        public string Valutazione_Conto_Des { get; set; }
        public int Anno { get; set; }
        public int Anno_Tipo { get; set; }
        public string Anno_Tipo_Des { get; set; }
        public int Ordine { get; set; }
        public int Sezione_Ordine { get; set; }
        public decimal Valore { get; set; }
        public int Valutazione_Sezione_Cod { get; set; }
        public string Valutazione_Sezione_Des { get; set; }
        public int Valutazione_Sezione_Invisibile { get; set; }
        public int Valutazione_Gruppo_Cod { get; set; }
        public string Valutazione_Gruppo_Des { get; set; }
        public int Valutazione_Gruppo_Padre { get; set; }
        public string Valutazione_Gruppo_Padre_Des { get; set; }
        public int Pat_Eco { get; set; }
        public int Attivo_Passivo { get; set; }
        public int Valutazione_Conto_Padre { get; set; }
        public int ChkBypassValore { get; set; }

        
        public List<Valutazione_Dettaglio_Specifico> Valutazione_Dettaglio_Specifico { get; set; }


        public Valutazione()
        {
            Valutazione_Dettaglio_Specifico = new List<Valutazione_Dettaglio_Specifico>();
        }

    }

}