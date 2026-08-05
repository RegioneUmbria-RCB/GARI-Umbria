using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita.centri_di_costo
{
    public class Progetto : CentroDiCosto
    {
        public string partitaIva { get; set; }
        public string descrizione { get; set; }

        public Progetto()
        {
            classType = costanti.ClassType.Progetto;
            tipo = Tipo.Progetto;
        }
    }
}
