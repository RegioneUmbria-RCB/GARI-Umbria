using System;

namespace AgronicaCoreDTOStd.InData.Demetra
{
    public class Avversita
    {
        public string tipo { get; set; }

        public int codice { get; set; }
    }

    public class TipoAvversita
    {
        public const String avversita = "Avversita";
        public const String gruppoAvversita = "GruppoAvversita";
    }

}
