using System;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class LeggiLink_Operazione
    {
        public string id_agenda { get; set; }
        public string codiceRicetta { get; set; }
        public string codiceOperazioneRicetta { get; set; }
        public int tipo_operazione { get; set; }
        public Impresa impresa { get; set; }
        public CentroAziendale centroaziendale { get; set; }
        public Specie specie { get; set; }
        public DateTime data { get; set; }
        public Lavorazione lavorazione { get; set; }
        public Attivita.Tipo_Attivita tipo { get; set; }
        public Attivita.Tipo_Ricetta tiporicetta { get; set; }
        public Attivita.Stati stato { get; set; }
        public VariabiliInSessione_NG variabiliInSessione_NG { get; set; }

    }
}