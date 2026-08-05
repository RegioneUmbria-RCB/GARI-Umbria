using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class ElencoAttivazioniMaschereLayerRaster
    {
        public List<AttivazioneMaschereLayerRaster> elencoAttivazioni { get; set; }
    }

    public class AttivazioneMaschereLayerRaster
    {
        public int Maschera_Cod { get; set; }
        public string Username { get; set; }
        public int Gruppi_Utente_Cod { get; set; }
    }
}
