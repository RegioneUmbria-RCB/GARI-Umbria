using System;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class APP_Ricette_Operazioni
    {
        public int W_Anagrafica_Stati_Cod { get; set; }
        public int Ricetta_Cod { get; set; }
        public int Ricetta_Operazione_Cod { get; set; }
        public int Ricetta_Operazione_Cod_RIF { get; set; }
        public string Ricetta_Operazione_Des { get; set; }
        public string Note { get; set; }
        public int Lav_Cod { get; set; }
        public int Extra_Int { get; set; }
        public int Mezzo { get; set; }
        public int Bozza { get; set; }
        public int Invia_App { get; set; }

        public int Id { get; set; }
        public int inviato { get; set; }
        public DateTime? datainvio { get; set; }
        public DateTime Data_Creazione { get; set; }
        public DateTime Data_Modifica { get; set; }
        public string Username_Creazione { get; set; }
        public string Username_Modifica { get; set; }
        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }
    }
}