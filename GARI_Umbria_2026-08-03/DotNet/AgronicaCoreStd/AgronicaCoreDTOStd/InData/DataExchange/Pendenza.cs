using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using InData.Anagrafica;

namespace AgronicaCoreDTOStd.InData.DataExchange
{
    public class Pendenza
    {
        public List<PendenzaParticella> particelle { get; set; }
        public PendenzaRiepilogo riepilogo { get; set; }
        public PendenzaProperties properties { get; set; }
    }

    public class PendenzaParticella
    {
        public string codNazionale { get; set; }
        public int foglio { get; set; }
        public int tavola { get; set; }
        public string particella { get; set; }
        [JsonProperty("sub")]
        public string subalterno { get; set; }
        public decimal areaIntersecata { get; set; }
        public decimal areaParticella { get; set; }
        public decimal percIntersez { get; set; }
        public decimal pendenzaPct { get; set; }
        public decimal pendenzaGrad { get; set; }
        public decimal direzione { get; set; }
        public decimal quotaAltimetrica { get; set; }
        public decimal percIntersezRipr { get; set; }
        public decimal pendenzaPctRipr { get; set; }
        public decimal pendenzaGradRipr { get; set; }
        public decimal direzioneRipr { get; set; }
        public decimal quotaAltimetricaRipr { get; set; }
    }

    public class PendenzaRiepilogo
    {
        public string codNazionale { get; set; }
        public int foglio { get; set; }
        public int tavola { get; set; }
        public string particella { get; set; }
        [JsonProperty("sub")]
        public string subalterno { get; set; }
        public decimal areaIntersecata { get; set; }
        public decimal areaParticella { get; set; }
        public decimal percIntersez { get; set; }
        public decimal pendenzaPct { get; set; }
        public decimal pendenzaGrad { get; set; }
        public decimal direzione { get; set; }
        public decimal quotaAltimetrica { get; set; }
        public decimal percIntersezRipr { get; set; }
        public decimal pendenzaPctRipr { get; set; }
        public decimal pendenzaGradRipr { get; set; }
        public decimal direzioneRipr { get; set; }
        public decimal quotaAltimetricaRipr { get; set; }
    }

    public class PendenzaProperties
    {
        public string msMethod { get; set; }
        public string msType { get; set; }
        public string dataElab { get; set; }
    }
}
