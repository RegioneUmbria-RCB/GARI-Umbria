using System.Collections.Generic;
using System.Data;

namespace OutData.Kendo
{
    public class ResultAndKendoColumns
    {
        public DataTable result { get; set; }
        public KendoColumn[] kendoColumns { get; set; }
    }
}
