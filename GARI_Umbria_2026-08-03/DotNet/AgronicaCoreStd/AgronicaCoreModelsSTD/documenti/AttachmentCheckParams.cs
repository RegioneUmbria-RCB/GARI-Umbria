using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace AgronicaCoreModelsSTD.documenti
{
    public interface IAttachmentCheckParams
    {
        string Piva { get; }
        int IdAgenda { get; }
        int RicettaCod { get; }
        int MacCod { get; }
        int Analisi_Testata_Cod { get; }
    }

    public class AttachmentCheckParams : IAttachmentCheckParams
    {
        public string Piva { get; set; } = "";
        public int IdAgenda { get; set; } = 0;
        public int RicettaCod { get; set; } = 0;
        public int MacCod { get; set; } = 0;
        public int Analisi_Testata_Cod { get; set; } = 0;
    }
}
