using System;

namespace InData.Log.AgronicaLogInvio
{
    public class WriteAgronicaLogInvioAgenda
    {

        public int Tipo_Esportazione { get; set; }

        public int ID_Log_Invio { get; set; }

        public int ID_Agenda { get; set; }

        public int? ID_Operazione_Esterna { get; set; }

        public DateTime? Validita_Inizio { get; set; }

        public DateTime? Validita_Fine { get; set; }

        public int Id_Mov { get; set; }

        public int Id_Mov_Det { get; set; }

        public int Id_Mov_Dest { get; set; }

        public int? Causale_Cod { get; set; }

        public string Piva { get; set; }

        public string Chiave_Esterna { get; set; }

        public string Chiave { get; set; }

        public WriteAgronicaLogInvioAgenda(int _ID_Log_Invio = 0,int _ID_Agenda = 0, int _ID_Mov = 0, int _ID_Mov_Det = 0)
        {
            Tipo_Esportazione = 0;
            ID_Log_Invio = _ID_Log_Invio;
            ID_Agenda = _ID_Agenda;
            ID_Operazione_Esterna = 0;
            Validita_Inizio = null;
            Validita_Fine = null;
            Id_Mov = _ID_Mov;
            Id_Mov_Det = _ID_Mov_Det;
            Id_Mov_Dest = 0;
            Causale_Cod = 0;
            Piva = "";
            Chiave_Esterna = "";
            Chiave = "";
        }

    }
}
