using System;
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.AuthDispatcher
{
    public class RichiestaSatUrl_In
    {
        public List<RichiestaSatUrl> elencoRichieste { get; set; }
    }

    public class RichiestaSatUrl
    {
        public string Sensor { get; set; }

        /// <summary>
        /// Coords in the format zoom/x_tile/y_tile
        /// e.g. 12/2184/1475
        /// </summary>
        public string Coords { get; set; }
        public String Data { get; set; }
    }
}