using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda
{
    public class AppezzamentoEntity
    {
        public int codice;
        public string nome;
        public int centroAziendaleCod;
        public int campoCod;
        public string partitaIva;
        public string codiceAnagrafe;
        public double superficie;
        public string cartografia;
        public string StaticMapBase64String;
        public string indirizzi;
        public DateTime inizioValidita;
        public DateTime fineValidita;
        public string codici;
        public bool blkFlag;
        public DateTime blkInizioData;
        public string blkInizioUsername;
        public string blkInizioNote;
        public DateTime blkFineData;
        public string blkFineUsername;
        public string blkFineNote;

    }
}
