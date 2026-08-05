using System;

namespace AgronicaCoreAPI.models
{
    public class StatisticheEntity
    {
        public class RilievoProduzione
        {
            public int esercizioCod;
            public int impiantoCod;
            public int appezzamentoCod;
            public int centroAziendaleCod;
            public string partitaIva;
            public double resaRilievo;
            public DateTime dataRilievo;
            public string utenteRilievo;
        }

        public class StimeProduzione
        {
            public int esercizioCod;
            public int impiantoCod;
            public int appezzamentoCod;
            public int centroAziendaleCod;
            public string partitaIva;
            public string ragioneSociale;
            public string centroAziendaleNome;
            public string appezzamentoNome;
            public int specieCod;
            public string specie;
            public int varietaCod;
            public string varieta;
            public string produzione;
            public double superficie;
            public double resaPrevista;
            public double supAbbattuta;
            public double rilievoDanni;
            public double stimaProduzione;
            public DateTime dataPrimaProduzione;
        }
    }
}
