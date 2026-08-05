using System;
using System.Collections.Generic;
using System.Text;

namespace OutData.Engine.MeteoSuite
{
    public class StazioneMeteoAliasDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Lat {  get; set; }
        public decimal Lng { get; set; }
    }

    public class StazioneMeteoAliasNewDto : StazioneMeteoAliasDto
    {
        public string Fornitore { get; set; }
        public double Dist { get; set; }
        public bool FlagNew { get; set; }
        public string AliasName { get; set; }
    }
}
