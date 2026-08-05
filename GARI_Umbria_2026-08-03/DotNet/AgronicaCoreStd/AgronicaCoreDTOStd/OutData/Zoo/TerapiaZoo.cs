using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;

namespace OutData.Zoo
{
    public class TerapiaZoo
    {
        public int Id_Terapia { get; set; }
        public string Descrizione { get; set; }
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        public int? Sta_Num { get; set; }

        public List<InterventoZoo> Interventi { get; set; }

        public TerapiaZoo()
        {
            this.Interventi = new List<InterventoZoo>();
        }

        public TerapiaZoo(
            int idTerapia, 
            string piva, 
            int saCod, 
            int staNum, 
            string descrizione, 
            List<InterventoZoo> interventi = null)
        {
            this.Id_Terapia = idTerapia;
            this.Descrizione = descrizione;
            this.Piva = piva;
            this.Sa_Cod = saCod;
            this.Sta_Num = staNum;
            this.Interventi = interventi ?? new List<InterventoZoo>();
        }

        //public TerapiaZoo(
        //    int idTerapia,
        //    BaseCodiceDescr impresa,
        //    BaseCodeDescr centroAziendale,
        //    BaseCodeDescr stalla,
        //    string descrizione,
        //    List<InterventoZoo> interventi = null)
        //{
        //    Id_Terapia = idTerapia;
        //    Impresa = impresa;
        //    CentroAziendale = centroAziendale;
        //    Stalla = stalla;
        //    //Piva = piva;
        //    //Sa_Cod = saCod;
        //    //Sta_Num = staNum;
        //    Descrizione = descrizione;
        //    Interventi = interventi ?? new List<InterventoZoo>();
        //}
    }
}
