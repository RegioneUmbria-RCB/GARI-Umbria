using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App
{
    public class ContattoEntity
    {
        public string codice;
        public string partitaIva;
        public int centroAziendaleCod;
        public string codiceFiscale;
        public string nome;
        public string cognome;
        public string nomeBreve;
        public string ragioneSociale;
        public bool isPublic;
        public string badge;
        public DateTime validitaFrom;
        public DateTime validitaTo;
        public string nrPatentino;
        public DateTime? dataRilascioPatentino;
        public DateTime? dataScadenzaPatentino;
        public DateTime? dataNascita;
        public string sesso;
    }
}
