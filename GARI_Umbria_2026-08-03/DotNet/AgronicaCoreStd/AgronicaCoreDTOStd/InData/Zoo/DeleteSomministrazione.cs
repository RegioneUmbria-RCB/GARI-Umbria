using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Zoo
{
    public class DeleteSomministrazione
    {
        public int? Id_Ricetta { get; set; }
        public int? Id_Riga_Ricetta { get; set; }
        public int? Id_Agenda { get; set; }

        /// <summary>
        /// Indica se si tratta di una Somministrazione futura o confermata
        /// </summary>
        public bool Programmata { get; set; }
    }
}
