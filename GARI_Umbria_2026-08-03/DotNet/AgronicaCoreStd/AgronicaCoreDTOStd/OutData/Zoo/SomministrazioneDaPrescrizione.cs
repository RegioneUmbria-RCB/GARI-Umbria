using System;
using System.Collections.Generic;
using AgronicaCoreModelsSTD.attivita;

namespace OutData.Zoo
{
    public class SomministrazioneDaPrescrizione
    {
        public int Id_Ricetta { get; set; }
        public int Tipo_Prescrizione { get; set; } 

        public List<Attivita> Somministrazioni { get; set; }

        public SomministrazioneDaPrescrizione() 
        {
            Somministrazioni = new List<Attivita>();
        }

        public SomministrazioneDaPrescrizione(int idRicetta, int tipoPres)
        {
            Id_Ricetta = idRicetta;
            Tipo_Prescrizione = tipoPres;
            Somministrazioni = new List<Attivita>();
        }

        public static implicit operator SomministrazioneDaPrescrizione(bool v)
        {
            throw new NotImplementedException();
        }
    }
}
