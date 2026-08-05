using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda
{
    public class ImpiantoEntity
    {
        public int codice;
        public int appezzamentoCod;
        public int centroAziendaleCod;
        public string partitaIva;
        public string descrizione;
        public string utilizzoTerrenoClassType;
        public int utilizzoTerrenoCod;
        public int gruppoFinalitaCod;
        public double superficie;
        public string cartografia;
        public string StaticMapBase64String;
        public DateTime inizioValidita;
        public DateTime fineValidita;
        public bool coverCrops;
        public string codiceImpianto;
        public string ageaIdColt;
        public string codiceCatalogoAgea;
    }
}
