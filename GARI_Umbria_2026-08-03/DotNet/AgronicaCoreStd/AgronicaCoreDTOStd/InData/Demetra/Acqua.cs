using System;

namespace AgronicaCoreDTOStd.InData.Demetra
{

    public class Acqua
    {
        public decimal quantita { get; set; }

        public int tipo { get; set; }
    }

    public class TipoAcqua
    {
        public const int hl_totale = 0;
        public const int hl_per_ha = 1;
    }

}
