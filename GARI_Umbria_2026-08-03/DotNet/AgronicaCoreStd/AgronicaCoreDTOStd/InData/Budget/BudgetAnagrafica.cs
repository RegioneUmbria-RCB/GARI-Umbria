using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Budget
{
    public class BudgetAnagrafica<T>
    {
        public int Id_Budget { get; set; }
        public T ElementoAnagrafico { get; set; }
        public bool Delete_Reale_From_Ribaltamento { get; set; }
    }
}
