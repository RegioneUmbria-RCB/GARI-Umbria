using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class Contribute
    {
        public int code { get; set; }
        public int type { get; set; }
        public string description { get; set; }
        public IntervalloTemporale validity { get; set; }

        public Contribute(int code, int type) 
        {
            this.code = code;
            this.type = type;
            this.description = "";
            this.validity = new IntervalloTemporale();
        }

        public Contribute() { }
    }

    public class LinkedContribute<T> : Contribute
    {
        public IntervalloTemporale linkValidity { get; set; }
        public T linkedItemPK { get; set; }

        public LinkedContribute(int code, int type, T linkedItemPK) : base(code, type)
        {
            this.linkedItemPK = linkedItemPK;
            this.linkValidity = new IntervalloTemporale();
        }

        public LinkedContribute() : base() { }
    }
}
