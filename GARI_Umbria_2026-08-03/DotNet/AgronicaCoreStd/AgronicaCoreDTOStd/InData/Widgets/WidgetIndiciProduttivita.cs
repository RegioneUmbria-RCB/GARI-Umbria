using System.Collections.Generic;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreDTOStd.InData.Widgets
{
    #region DataStructure
    public class WidgetIndiciProduttivita
    {
        public UtilizzoTerreno utilizzoTerreno { get; set; }
        public List<UtilizzoTerreno> altreColture { get; set; }
        public float sup { get; set; }
        public int indiceProduttivitaAICod { get; set; }
        public List<BaseCodeValue<string, int>> indici { get; set; }
    }

    public class WidgetIndiciProduttivitaXAnno
    {
        public int year { get; set; }
        public List<WidgetIndiciProduttivita> indiciProduttivita { get; set; }
    }

    public class WidgetIndiciProduttivitaXImpresa
    {
        public string piva { get; set; }
        public string ragSoc { get; set; }
        public string cuaa { get; set; }
        public BaseCodeDescrStr regione { get; set; }
        public List<WidgetIndiciProduttivitaXAnno> indiciXAnno { get; set; }
    }

    public class WidgetIndiciProduttivitaGlobal
    {
        public List<WidgetIndiciProduttivitaXImpresa> indiciXImpresa { get; set; }
    }

    public class WidgetKPI
    {
        public int min { get; set; }
        public int max { get; set;}
        public string udm { get; set;}
        public string kpi { get; set; }
        public string des { get; set; }
        public int val{ get; set; }
        public IndicatoreRatingUnico indicator { get; set; }
        public string icon { get; set; }
    }
    #endregion

    #region Request
    public class WidgetRequestIndiciProduttivitaXAnno
    {
        public int year { get; set; }
        public List<BaseCodeDescr> specieVegetale { get; set; }
    }

    public class WidgetRequestIndiciProduttivitaXImpresa
    {
        public string piva { get; set; }
        public List<WidgetRequestIndiciProduttivitaXAnno> specieXYear { get; set; }
    }

    public class WidgetRequestIndiciProduttivita
    {
        public List<WidgetRequestIndiciProduttivitaXImpresa> requestXImpresa { get; set; }
    }

    public class WidgetRequestKpi
    {
        public string piva { get; set; }
        public uint year { get; set; }
    }
    #endregion

    public enum IndicatoreRatingUnico
    {
        Basso, //Rosso
        Medio, //Giallo
        Alto //Verde
    }
}
