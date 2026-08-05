using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App
{
    public class ProdottoEntity
    {
        public int codice;
        public int elemCod;
        public int unitaDiMisuraCod;
        public string nomeComune;
        public string descrizione;
        public int specieCod;
        public double N;
        public double P2O5;
        public double K20;
        public double Cu;

        //Lorenzo C. Per ora commentato da rivedere quando si svilupperanno le Trappole 
        //public bool IsTrappolaFormulato;
    }
}
