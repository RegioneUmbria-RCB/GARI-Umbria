using AgronicaCoreModelsSTD.Gis;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class ElencoPermessiUtenteMaschere
    {
        public List<ElencoPermessiMaschera> elencoPermessiUtente { get; set; }
    }
    public class ModifichePermessiMaschera
    {
        public int Maschera_Cod { get; set; }
        public List<PermessoMaschera> InsertPermessi { get; set; }
        public List<PermessoMaschera> UpdatePermessi { get; set; }
        public List<PermessoMaschera> DeletePermessi { get; set; }
    }
    public class ElencoPermessiMaschera
    {
        public int Maschera_Cod { get; set; }

        public List<PermessoMaschera> elencoPermessiMaschera { get; set; }
    }

}