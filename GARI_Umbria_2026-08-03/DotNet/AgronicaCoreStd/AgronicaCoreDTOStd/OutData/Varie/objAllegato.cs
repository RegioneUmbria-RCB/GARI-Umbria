using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace OutData.Varie
{
    public class objAllegato
    {
        public string NomeFile { get; set; }
        public string Estensione {  get; set; }
        public byte[] File {  get; set; }
    }
}
