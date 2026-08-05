using System;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.anagrafiche;
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class ScriviListaAttivita
    {
        public List<Attivita> attivita_list { get; set; }

        public List<Parametri_Aggiuntivi_Attivita> parametri_aggiuntivi_list { get; set; }

        public List<Impianto> impianti_selezionati_list { get; set; }
    }
}