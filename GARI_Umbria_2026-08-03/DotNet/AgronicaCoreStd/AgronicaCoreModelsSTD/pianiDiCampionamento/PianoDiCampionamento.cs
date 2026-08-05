using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.Zoo;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.pianiDiCampionamento
{
    public class PianoDiCampionamento: BaseCodeDescr
    {
        public IntervalloTemporale validita { get; set; }
        public DateTime data_istantanea { get; set; }
        public RifImpresa impresa { get; set; }
        public RifCentroAziendale centro { get; set; }
        public RifFabbricato stalla { get; set; }
        public BaseCodeDescr stato { get; set; }
        public bool daCampagna { get; set; }
        public bool daZoo { get; set; }
        public List<pdcDettaglio<GiacenzaZoo>> dettagliZoo { get; set; }
        public List<pdcDettaglio<Impianto>> dettagliImpianti { get; set; }
        public List<pdcLFO> lfo { get; set; }
        public string guid { get; set; }
        public bool cancellato { get; set; }
        public PianoDiCampionamento(): base() { }
    }
}
