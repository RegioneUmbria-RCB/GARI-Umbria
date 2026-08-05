using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class PermessiUtenteEntitaGIS
    {
        public Permesso centriAziendali { get; set; }
        public Permesso fabbricati { get; set; }
        public Permesso campi { get; set; }
        public Permesso appezzamenti { get; set; }
        public Permesso impianti { get; set; }

        public PermessiUtenteEntitaGIS()
        {
            centriAziendali = new Permesso() { Lettura=false , Scrittura=false };
            fabbricati = new Permesso() { Lettura = false, Scrittura = false };
            campi = new Permesso() { Lettura = false, Scrittura = false };
            appezzamenti = new Permesso() { Lettura = false, Scrittura = false };
            impianti = new Permesso() { Lettura = false, Scrittura = false };
        }
    }

    public class Permesso
    {
        public bool Lettura { get; set; }
        public bool Scrittura { get; set; }
    }
}
