using System;
using System.Collections.Generic;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaCoreModelsSTD.anagrafiche;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class Inizializza_QdC
    {
        public Boolean SportelloAperto { get; set; }

        public Boolean AziendaInVerifica { get; set; }

        public DateTime Data { get; set; }

        public DateTime Data_Min { get; set; }

        public DateTime Data_Max { get; set; }

        public Boolean flagNuovoControlloRiduzioneDiserbo { get; set; }

        public List <string> ListaPivaAgenzie { get; set; }

        public List<Fabbricato> ListaFabbricatiConUsodaTerzi { get; set; }

        public List<GiacenzeXProdotto> ListaGiacenzeXProdotto { get; set; }

        public Pua pua { get; set; }

        public UtilizzoTerreno utilizzoTerreno { get; set; }

        public Boolean PraticaTrovata { get; set; }

        public List<int> ListaLavCodNonGestitiSuAPP { get; set; }

    }
}