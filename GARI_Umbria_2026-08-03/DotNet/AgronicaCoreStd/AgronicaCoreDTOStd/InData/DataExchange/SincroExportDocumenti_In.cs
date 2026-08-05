using System;
using System.Collections.Generic;
using System.Text;

namespace InData.DataExchange
{
    public class SincroExportDocumenti_In
    {
        public string CUAA { get; set; }
        public int Id_Tipologia { get; set; }
        public List<int> Id_Documenti { get; set; }
        public bool Cancellato { get; set; }
    }
}
