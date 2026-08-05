using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Notifiche
{
    public class NotificaCUAA
    {
        public string ID_ExternalSystem { get; set; }
        public List<CUAAObj> elencoCUAA { get; set; }
    }
    public class CUAAObj
    {
        public string CUAA { get; set; }
        public int Campagna { get; set; }
        public string Operazione { get; set; }
        public string id_signal { get; set; }
        public CUAAObj_Detail dettaglio { get; set; }
        public int? Priorita { get; set; }
    }

    public class CUAAObj_Detail
    {
        public CUAAObj_Detail_Notification anagrafica { get; set; }
        public CUAAObj_Detail_Notification catasto { get; set; }
        public CUAAObj_Detail_Notification pcg { get; set; }
        public CUAAObj_Detail_Notification pcg_catasto { get; set; }
        public CUAAObj_Detail_Notification equipaggiamenti { get; set; }
        public CUAAObj_Detail_Notification lavoratori { get; set; }
        public CUAAObj_Detail_Notification gruppi_appezzamenti { get; set; }
    }

    public class CUAAObj_Detail_Notification
    {
        public DateTime DataOraNotifica { get; set; }
    }

}
