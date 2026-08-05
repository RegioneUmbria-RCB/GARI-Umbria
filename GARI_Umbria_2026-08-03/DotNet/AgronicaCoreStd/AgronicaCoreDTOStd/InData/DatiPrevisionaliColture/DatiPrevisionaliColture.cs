using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace InData.DatiPrevisionaliColture
{
    #region Request
    public class DatiPrevisionaliColtureRequest
    {
        public BaseCodeDescr vegCod { get; set; }
        public BaseCodeDescr culCod { get; set; }
        public BaseCodeDescr grvaCod { get; set; }
        public BaseCodeDescr grfiCod { get; set; }
        public BaseCodeDescr statoCod { get; set; }
        public BaseCodeDescr portCod { get; set; }
        public BaseCodeDescr foralCod { get; set; }
        public BaseCodeDescrStr dettSpeciePersonalizzatoCod { get; set; }
        public BaseCodeDescr regCod { get; set; }
        public BaseCodeDescr parametroCod { get; set; }
        public BaseCodeDescrStr reg { get; set; }
        public BaseCodeDescrStr prov { get; set; }
        public BaseCodeDescrStr codiceStato { get; set; }
        public DateTime validitaInizio { get; set; }
        public DateTime validitaFine { get; set; }

        public BaseCodeValue<string, string> getFieldValueString(string field)
        {
            switch (field)
            {
                case "vegCod":
                    return new BaseCodeValue<string, string>("veg_cod", this.vegCod.codice.ToString());
                case "culCod":
                    return new BaseCodeValue<string, string>("cul_cod", this.culCod.codice.ToString());
                case "grvaCod":
                    return new BaseCodeValue<string, string>("grva_cod", this.grvaCod.codice.ToString());
                case "grfiCod":
                    return new BaseCodeValue<string, string>("grfi_cod", this.grfiCod.codice.ToString());
                case "statoCod":
                    return new BaseCodeValue<string, string>("stato_cod", this.statoCod.codice.ToString());
                case "portCod":
                    return new BaseCodeValue<string, string>("port_cod", this.portCod.codice.ToString());
                case "foralCod":
                    return new BaseCodeValue<string, string>("foral_cod", this.foralCod.codice.ToString());
                case "dettSpeciePersonalizzatoCod":
                    return new BaseCodeValue<string, string>("dettSpeciePersonalizzatoCod", "'" + this.dettSpeciePersonalizzatoCod.codice.ToString() + "'");
                case "regCod":
                    return new BaseCodeValue<string, string>("reg_cod", this.regCod.codice.ToString());
                case "parametroCod":
                    return new BaseCodeValue<string, string>("parametro_cod", this.parametroCod.codice.ToString());
                case "reg":
                    return new BaseCodeValue<string, string>("reg", "'" + this.reg.codice.ToString() + "'");
                case "prov":
                    return new BaseCodeValue<string, string>("prov", "'" + this.prov.codice.ToString() + "'");
                case "codiceStato":
                    return new BaseCodeValue<string, string>("codice_stato", "'" + this.codiceStato.codice.ToString() + "'");
                case "validitaInizio":
                    return new BaseCodeValue<string, string>("Validita_Inizio", this.validitaInizio.ToString());
                case "validitaFine":
                    return new BaseCodeValue<string, string>("Validita_Fine", this.validitaFine.ToString());
                default:
                    return new BaseCodeValue<string, string>("", "");
            }
        }
    }

    public class RequestDataPriority
    {
        public uint vegCod { get; set; }
        public uint culCod { get; set; }
        public uint grvaCod { get; set; }
        public uint grfiCod { get; set; }
        public uint statoCod { get; set; }
        public uint portCod { get; set; }
        public uint foralCod { get; set; }
        public uint dettSpeciePersonalizzatoCod { get; set; }
        public uint regCod { get; set; }
        public uint reg { get; set; }
        public uint prov { get; set; }
        public uint codiceStato { get; set; }
        public uint validitaInizio { get; set; }
        public uint validitaFine { get; set; }

        public List<BaseCodeValue<string, uint>> priorityList { get; private set; }

        public RequestDataPriority(
            uint vegCod = 0,
            uint culCod = 0,
            uint grvaCod = 0,
            uint grfiCod = 0,
            uint statoCod = 0,
            uint portCod = 0,
            uint foralCod = 0,
            uint dettSpeciePersonalizzatoCod = 0,
            uint regCod = 0,
            uint reg = 0,
            uint prov = 0,
            uint codiceStato = 0,
            uint validitaInizio = 0,
            uint validitaFine = 0
        )
        {
            this.vegCod = vegCod;
            this.culCod  = culCod;
            this.grvaCod = grvaCod;
            this.grfiCod = grfiCod;
            this.statoCod = statoCod;
            this.portCod = portCod;
            this.foralCod = foralCod;
            this.dettSpeciePersonalizzatoCod = dettSpeciePersonalizzatoCod;
            this.regCod = regCod;
            this.reg = reg;
            this.prov = prov;
            this.codiceStato = codiceStato;
            this.validitaInizio = validitaInizio;
            this.validitaFine = validitaFine;

            this.priorityList = new List<BaseCodeValue<string, uint>>();
            priorityList.Add(new BaseCodeValue<string, uint>("vegCod", this.vegCod));
            priorityList.Add(new BaseCodeValue<string, uint>("culCod", this.culCod));
            priorityList.Add(new BaseCodeValue<string, uint>("grvaCod", this.grvaCod));
            priorityList.Add(new BaseCodeValue<string, uint>("grfiCod", this.grfiCod));
            priorityList.Add(new BaseCodeValue<string, uint>("statoCod", this.statoCod));
            priorityList.Add(new BaseCodeValue<string, uint>("portCod", this.portCod));
            priorityList.Add(new BaseCodeValue<string, uint>("foralCod", this.foralCod));
            priorityList.Add(new BaseCodeValue<string, uint>("dettSpeciePersonalizzatoCod", this.dettSpeciePersonalizzatoCod));
            priorityList.Add(new BaseCodeValue<string, uint>("regCod", this.regCod));
            priorityList.Add(new BaseCodeValue<string, uint>("reg", this.reg));
            priorityList.Add(new BaseCodeValue<string, uint>("prov", this.prov));
            priorityList.Add(new BaseCodeValue<string, uint>("codiceStato", this.codiceStato));
            priorityList.Add(new BaseCodeValue<string, uint>("validitaInizio", this.validitaInizio));
            priorityList.Add(new BaseCodeValue<string, uint>("validitaFine", this.validitaFine));

            priorityList.Sort((a, b) => ((int)a.value) - ((int)b.value));
        }

        public List<BaseCodeValue<string, uint>> getPriorityList()
        {
            this.priorityList = new List<BaseCodeValue<string, uint>>();
            priorityList.Add(new BaseCodeValue<string, uint>("vegCod", this.vegCod));
            priorityList.Add(new BaseCodeValue<string, uint>("culCod", this.culCod));
            priorityList.Add(new BaseCodeValue<string, uint>("grvaCod", this.grvaCod));
            priorityList.Add(new BaseCodeValue<string, uint>("grfiCod", this.grfiCod));
            priorityList.Add(new BaseCodeValue<string, uint>("statoCod", this.statoCod));
            priorityList.Add(new BaseCodeValue<string, uint>("portCod", this.portCod));
            priorityList.Add(new BaseCodeValue<string, uint>("foralCod", this.foralCod));
            priorityList.Add(new BaseCodeValue<string, uint>("dettSpeciePersonalizzatoCod", this.dettSpeciePersonalizzatoCod));
            priorityList.Add(new BaseCodeValue<string, uint>("regCod", this.regCod));
            priorityList.Add(new BaseCodeValue<string, uint>("reg", this.reg));
            priorityList.Add(new BaseCodeValue<string, uint>("prov", this.prov));
            priorityList.Add(new BaseCodeValue<string, uint>("codiceStato", this.codiceStato));
            priorityList.Add(new BaseCodeValue<string, uint>("year", this.validitaInizio));
            priorityList.Add(new BaseCodeValue<string, uint>("year", this.validitaFine));

            priorityList.Sort((a, b) => ((int)a.value) - ((int)b.value));

            return this.priorityList;
        }

    }
    #endregion

    #region DataStructure
    public class DatiPrevisionaliColture
    {
        public int parametroCod { get; set; }
        public UnitaDiMisura udm { get; set; }
        public float valore { get; set; }

        public DatiPrevisionaliColture()
        {
            this.parametroCod = 0;
            this.udm = new UnitaDiMisura(0);
            this.valore = 0;
        }
    }
    
    public class DatiPrevisionaliColtureComplete
    {
        public string piva { get; set; }
        public int id { get; set; }
        public int parametroCod { get; set; }
        public int udm { get; set; }
        public float valore { get; set; }
        public int vegCod { get; set; }
        public int culCod { get; set; }
        public int grvaCod { get; set; }
        public int grfiCod { get; set; }
        public int statoCod { get; set; }
        public int portCod { get; set; }
        public int foralCod { get; set; }
        public string dettSpeciePersonalizzatoCod { get; set; }
        public int regCod { get; set; }
        public string reg { get; set; }
        public string prov { get; set; }
        public string codiceStato { get; set; }
        public DateTime validitaInizio { get; set; }
        public DateTime validitaFine { get; set; }

        public DatiPrevisionaliColtureComplete()
        {
            this.parametroCod = 0;
            this.udm = 0;
            this.valore = 0;
        }
    }
    #endregion
}
