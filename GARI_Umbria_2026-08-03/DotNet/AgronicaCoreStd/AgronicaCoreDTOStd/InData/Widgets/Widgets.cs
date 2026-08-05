using AgronicaCoreModelsSTD.Widgets;
using System;
using System.Collections.Generic;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreDTOStd.InData.Widgets
{
    public class Widgets_In
    {
        public string UserName { get; set; }
        public string Codice { get; set; }
        public string Titolo { get; set; }
        public string Descrizione { get; set; }
        public bool? Visibile { get; set; }
        public bool? Abilitato { get; set; }
        public bool? PresetIniziale { get; set; }
    }

    public class Aggiorna_Widgets_In
    { 
        public List<Widget> widgets { get; set; }
    }

    public class Widget_MovimentiMagazziono_In
    { 
        public int NumeroMovimenti { get; set; }
        public string Piva { get; set; }
    }

    public class Widget_Operazioni_IN
    {
        public int? NumeroMovimenti { get; set; }
        public string Piva { get; set; }
        public string Filtro_Lav_Cod { get; set; }
    }

    public class Widget_Culture_IN
    {
        public int? NumeroMovimenti { get; set; }
        public string Piva { get; set; }
    }

    public class Widget_LinkGestione_IN
    { 
        public string Piva { get; set; }
        public Enum_Codice_Widget CodiceWidget { get; set; }
    }

    public class Widget_Modelli_Previsionali_Indicatori_IN
    {
        public string Piva { get; set; }
        public Widget_Modelli_Previsionali_Indicatori_ParamExtra ParamExtra { get; set; }
    }

    public class Widget_Modelli_Previsionali_Indicatori_ParamExtra
    { 
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }
    }

    public class Widget_Acquisti_IN
    {
        public int? NumeroMovimenti { get; set; }
        public string Piva { get; set; }
    }

    public class Widget_GHGColture_IN
    {
        public int? NumeroMovimenti { get; set; }
        public string Piva { get; set; }
    }

    public class Widget_StimeProduzioneColture_IN
    {
        public int? NumeroMovimenti { get; set; }
        public string Piva { get; set; }
    }
    public class Widget_Statistics_IN
    {
        public int Year { get; set; }
        public string Country { get; set; }
    }

    public class Widget_PrevisioniAI_IN
    {
        public string Piva { get; set; }
        public DateTime DataStats { get; set; }
    }

    public class Widget_Zoo_IN
    {
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Sta_Num { get; set; }
        public DateTime timeStart { get; set; }
    }
}
