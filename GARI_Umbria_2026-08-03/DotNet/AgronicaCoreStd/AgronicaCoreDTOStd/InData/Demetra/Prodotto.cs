using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.attivita.risorse;
using System;

namespace AgronicaCoreDTOStd.InData.Demetra
{
    public class Prodotto
    {
        public int codice { get; set; }

        public decimal quantita { get; set; }
        public string udm { get; set; }

        public Magazzino magazzino { get; set; }
        public Avversita avversita { get; set; }

        public decimal Cu { get; set; }
        public decimal N { get; set; }
        public decimal P { get; set; }
        public decimal K { get; set; }

    }

    public class UdmProdotto
    {
        public const String chilogrammi = "KG";
        public const String litri = "LT";
        public const String numero = "NR";
        public const String millimetri = "MM";
        public const String metriCubiEttaro = "M3HA";
        public const String metriCubi = "M3";
        public const String quintali = "Q";
    }
}
