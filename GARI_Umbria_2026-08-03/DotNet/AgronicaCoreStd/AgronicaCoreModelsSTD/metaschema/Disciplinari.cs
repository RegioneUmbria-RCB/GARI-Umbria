using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class Disciplinare : BaseCodeDescrStr
    {
        //0=non applicabile/nessun disciplinare/reg bio/nessun vincolo normativo
        //1=pubblico
        //2=privato
        public int disciplinarePubblicoPrivato { get; set; }

        public RegolamentoConcimazione regolamentoConcimazione { get; set; }

        public RaggruppamentiColturaliDPI raggruppamentiColturaliDPI { get; set; }

        public GruppoFinalita gruppoFinalita { get; set; }

        public int flagProtetto { get; set; }

        public int idTr { get; set; }
        public IntervalloTemporale validita { get; set; }

        public Disciplinare(string codice) : base(codice,"")
        {

        }

        public Disciplinare() :base() { }

    }
}
