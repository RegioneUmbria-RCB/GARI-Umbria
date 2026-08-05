using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    public class CopiaSpostaAppezzamenti
    {
        public List<Appezzamento> Appezzamenti { get; set; }

        public CentroAziendale NuovoCentro { get; set; }

        public bool SpostaEliminaOrigine { get; set; }
        public bool CopiaCatasto { get; set; }
    }
}
