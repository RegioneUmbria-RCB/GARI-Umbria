using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiImpianto
    {
        public Disciplinare disciplinare { get; set; }

        public Disciplinare direttiva_nitrati { get; set; }

        public CentroAziendale centroAziendale { get; set; }

        public Lavorazione[] lavorazioni { get; set; }

        public Campo campo { get; set; }

        public DateTime data { get; set; }

        public bool consideraTerrenoNudo{ get; set; }

        public bool dettagliTerrenoNudo { get; set; }

        public int[] id_agenda_list { get; set; }

        public int[] ricetta_operazione_cod_list { get; set; }

        public int tipo_ricetta { get; set; }

        public int tipo_attivita { get; set; }

        public int stato { get; set; }

        public int tipo_operazione_db { get; set; }

        public int veg_cod { get; set; }

        public int dest_cod { get; set; }

        public Impresa impresa { get; set; }

        public bool? filtra_validita_esercizi { get; set; }
    }
}
