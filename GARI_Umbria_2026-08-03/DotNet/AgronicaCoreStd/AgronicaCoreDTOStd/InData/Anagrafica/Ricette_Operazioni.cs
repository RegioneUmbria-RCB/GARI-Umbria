using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    public partial class Ricette_Operazioni
    {
        public string Ricetta_SuperUser { get; set; }

        public int Ricetta_Cod { get; set; }

        public int Ricetta_Operazione_Cod { get; set; }

        public string Ricetta_Operazione_Des { get; set; }

        public int Lav_Cod { get; set; }

        public string Note { get; set; }

        public Nullable<double> Num_Protocollo { get; set; }

        public Nullable<int> Id_Rcdpi { get; set; }

        public Nullable<int> Extra_Int { get; set; }

        public Nullable<short> Mezzo { get; set; }

        public Nullable<short> inviato { get; set; }

        public Nullable<DateTime> datainvio { get; set; }

        public Nullable<DateTime> Data_Creazione { get; set; }

        public Nullable<DateTime> Data_Modifica { get; set; }

        public string Username_Creazione { get; set; }

        public string Username_Modifica { get; set; }

        public Nullable<DateTime> Validita_Inizio { get; set; }

        public Nullable<DateTime> Validita_Fine { get; set; }

        public Nullable<int> DataLock { get; set; }

        public Nullable<int> Gru_Op { get; set; }

        public Nullable<double> Costo { get; set; }

        public Nullable<short> Noleggio_Passivo { get; set; }

        public Nullable<int> Id_Tp_Fer { get; set; }

        public Nullable<int> EM_Cod { get; set; }

        public Nullable<double> Eff_Perc { get; set; }

        public Nullable<int> Disciplinare_PubblicoPrivato { get; set; }

        public Nullable<int> W_Anagrafica_Stati_Cod { get; set; }

        public Nullable<int> Ricetta_Operazione_Cod_RIF { get; set; }

        public string APP_Ricetta_Operazione_ID { get; set; }

        public Nullable<int> Raccoglitore_Cod { get; set; }
    }

}
