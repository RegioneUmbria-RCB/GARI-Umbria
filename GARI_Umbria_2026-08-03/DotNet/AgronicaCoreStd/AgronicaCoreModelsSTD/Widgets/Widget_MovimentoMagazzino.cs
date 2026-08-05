using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Widgets
{
    public class Widget_MovimentoMagazzino
    {
        
        public string Piva { get; set; }

        public int Sa_cod { get; set; }
        public int Id_Agenda { get; set; }
        public string Des_Lib { get; set; }

        public Decimal Qta { get; set; }

        public Decimal Giacenza { get; set; }

        public DateTime Data_Movimento { get; set; }

        public int Udm { get; set; }

        public string Udm_Des { get; set; }

        public string Udm_Sim { get; set; }

        public int Id_Destinazione { get; set; }

        public string Fabbricato_Des { get; set; }

        public string Cod_Articolo { get; set; }

        public string Descrizione_Prodotto { get; set; }

        public string Lotto { get; set; }

        public int Elem_Cod { get; set; }
        public int Pro_Cod { get; set; }
        public int Mat_Cod { get; set; }


    }

    public class WidgetAcquisto
    {
        public string Piva { get; set; }
        public int Sa_cod { get; set; }
        public int Id_Agenda { get; set; }
        public int Lav_Cod { get; set; }
        public string Des_Lib { get; set; }
        public string Fornitore { get; set; }
        public DateTime DataDoc { get; set; }
        public string NrDoc { get; set; }
        public string Prodotto { get; set; }
        public string Udm_Sim { get; set; }
        public decimal Qta { get; set; }
        public decimal PrezzoNetto { get; set; }
    }

}
