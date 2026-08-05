using AgronicaCoreModelsSTD.anagrafiche;
using System.Collections.Generic;

namespace InData.Agea
{
    public class ExportQdCtoAgea
    {
        public List<string> erroriDaBypassare { get; set; }

        public Impresa impresa { get; set; }

        public int anno { get; set; }

    }

    public class ExportQdCtoAgeaPaginated : ExportQdCtoAgea
    {
        public int Skip { get; set; }
        public int Top { get; set; }
    }
}
