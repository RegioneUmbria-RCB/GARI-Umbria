using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class ElencoPermessiUtente
    {
        public List<ElencoPermessiConfigurazione> elencoPermessiUtente { get; set; }
    }
    public class ModifichePermessiConfigurazione
    {
        public int LayerAnalysisConfig_Cod { get; set; }
        public List<PermessoConfigurazione> InsertPermessi { get; set; }
        public List<PermessoConfigurazione> UpdatePermessi { get; set; }
        public List<PermessoConfigurazione> DeletePermessi { get; set; }
    }
    public class ElencoPermessiConfigurazione
    {
        public int LayerAnalysisConfig_Cod { get; set; }

        public List<PermessoConfigurazione> elencoPermessiConfigurazione { get; set; }
    }
    public class PermessoConfigurazione
    {
        public string UserName { get; set; }
        public int Gruppi_Utente_cod { get; set; }
        public string Gruppi_Utente_des { get; set; }
        public int Flag_Inserimento { get; set; }
        public int Flag_Modifica { get; set; }
        public int Flag_Cancellazione { get; set; }
        public int Flag_Informazioni { get; set; }
        public int Flag_Amministrazione { get; set; }
        public int Flag_GestioneInteroLayer { get; set; }
    }
}