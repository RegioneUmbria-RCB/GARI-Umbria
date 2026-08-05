using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.baseClass
{
    public interface IBaseCodeDescr
    {
        int codice { get; set; }
        string descrizione { get; set; }
    }

    public interface IBaseCodiceDescr
    {
        string codice { get; set; }
        string descrizione { get; set; }
    }

    public class BaseCodeDescr: IBaseCodeDescr
    {
        public int codice { get; set; }
        public string descrizione { get; set; }

        public BaseCodeDescr(int code, string descr)
        {
            codice = code;
            descrizione = descr;
        }

        public BaseCodeDescr()
        {
            codice = 0;
            descrizione = "";
        }

    }

    public class BaseCodiceDescr : IBaseCodiceDescr
    {
        public string codice { get; set; }
        public string descrizione { get; set; }

        public BaseCodiceDescr(string code, string descr)
        {
            codice = code;
            descrizione = descr;
        }

        public BaseCodiceDescr()
        {
            codice = string.Empty;
            descrizione = string.Empty;
        }

    }
}
