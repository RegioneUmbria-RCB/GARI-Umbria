using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda
{
    public class RilevamentoDiMagazzinoEntity
    {
        public string partitaIva;
        public int centroaziendaleCod;
        public int fabbricatoCod;
        public int prodottoCod;
        public int elemCod;
        public int tipo;
        public double quantita;
        public string lotto;
        public int cal_cod;
        public int cod_progetto;
        public int unitaDiMisuraCod;
        public string descrizione;
        public double N;
        public double P2O5;
        public double K20;
        public double Cu;
        public int[] LavCodCompatibiliFormulati;
    }
}
