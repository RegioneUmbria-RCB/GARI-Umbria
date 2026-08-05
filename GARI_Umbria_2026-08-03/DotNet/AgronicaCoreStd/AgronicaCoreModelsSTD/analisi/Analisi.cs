using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.analisi
{
    public class Analisi : baseClass.BaseCodeDescr
    {
        public IntervalloTemporale validita { get; set; }

        public string note { get; set; }

        public CertificatoAnalisi certificatoAnalisi { get; set; }

        public Contatto laboratorio { get; set; }
        public AnalisiTipologia analisiTipologia { get; set; }


        public List<AnalisiDettaglio> dettagli { get; set; }
        public AnalisiTipo AnalisiTipo { get; set; }

        public Analisi(int code, string descr) : base(code, descr) { }

        public Analisi() { }
    }
}