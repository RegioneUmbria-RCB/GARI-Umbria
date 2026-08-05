using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.documenti;
using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;
using static AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino;

namespace AgronicaCoreModelsSTD.attivita
{
    public class MovimentoDiMagazzino
    {
        public Fabbricato Magazzino { get; set; }
        public Prodotto Prodotto { get; set; }
        public decimal Qta { get; set; }
        public UnitaDiMisura UdM { get; set; }
        public string Lotto { get; set; }
        public RilevamentoMagazzinoTipo Tipo { get; set; }
        public DateTime Data { get; set; }
        public string Codice { get; set; }
        // public string Descrizione { get; set; }
        public decimal Prezzo { get; set; }
        public string Note { get; set; }
        public string guid { get; set; }
        public string versione { get; set; }
        public string origine { get; set; }
        public bool definitivo { get; set; }
        public bool cancellato { get; set; }
        public int codice_progetto { get; set; }
        public List<Documento> documenti { get; set; }
    }
}
