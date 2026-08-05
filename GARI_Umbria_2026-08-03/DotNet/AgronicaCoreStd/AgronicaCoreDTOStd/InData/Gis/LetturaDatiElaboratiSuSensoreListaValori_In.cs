using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class LetturaDatiElaboratiSuSensoreListaValori_In
    {

        //PoligonoWKT As String, Zoom As Integer, Sensore As String, DataInizio As String, DataFine As String

        public String PoligonoWKT { get; set; }

        public int Zoom { get; set; }

        public String Sensore { get; set; }

        public String DataInizio { get; set; }

        public String DataFine { get; set; }

        public int LayerElementiGrafici_Cod { get; set; }

    }
}
