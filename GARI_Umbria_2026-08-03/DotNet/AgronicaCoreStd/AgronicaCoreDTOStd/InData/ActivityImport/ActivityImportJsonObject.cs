using System;
using System.Collections.Generic;
using System.Text;

namespace InData.ActivityImport
{
    public class ActivityImportJsonObject
    {
        /// <summary>
        /// Cuaa impresa a cui fa capo il l’operazione QdC
        /// </summary>
        public string cuaa { get; set; }
        /// <summary>
        /// Dati in formato compresso.
        /// </summary>
        public string dati { get; set; }
    }
}
