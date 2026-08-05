using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Widgets
{

    public interface IWidgetParametri
    { 
        int? CodPermesso { get; set; }
        string ListaCodPermessi { get; set; }
    }

    public class WidgetParametriBase : IWidgetParametri
    {
        public int? CodPermesso { get ; set ; }
        public string ListaCodPermessi { get; set; }

        public WidgetParametriBase() { CodPermesso = -1; ListaCodPermessi = ""; }
    }

    public interface IWidget {

        Int32 IdWidget { get; set; }
        string UserName { get; set; }
        string Codice { get; set; }
        string Titolo { get; set; }
        string Descrizione { get; set; }
        bool? Abilitato { get; set; }
        bool? Visibile { get; set; }

        bool? PresetIniziale { get; set; }

    }

    public class WidgetBase: IWidget
    {
        public Int32 IdWidget { get; set; }
        public string UserName { get; set; }
        public string Codice { get; set; }
        public string Titolo { get; set; }
        public string Descrizione { get; set; }
        public bool? Abilitato { get; set; }
        public bool? Visibile { get; set; }

        public bool? PresetIniziale { get; set; }

        public int? Permesso { get; set; }

        public Boolean MultiAziendale { get; set; }

    }

    public class Widget_Complete_Configuration
    {
        public List<Widget_Configuration> PresetIniziale { get; set; }
        public List<Widget_Configuration> UserWidgets { get; set; }

        public bool WidgetsBloccati { get; set; }
    }

   
    public class Widget_Configuration : WidgetBase
    {
        public Widget_Configuration() { }

        public Widget_Configuration(Widget widget)
        {
            this.Abilitato = widget.Abilitato;
            this.Codice = widget.Codice;
            this.Descrizione = widget.Descrizione;
            this.IdWidget = widget.IdWidget;
            this.PresetIniziale = widget.PresetIniziale;
            this.Titolo = widget.Titolo;
            this.UserName = widget.UserName;
            this.Visibile = widget.Visibile;
            this.MultiAziendale = widget.MultiAziendale;
        }
    }

    public class Widget: WidgetBase
    {
        public string Aspetto { get; set; }
        public string Parametri { get; set; }

        public bool RichiedeAziendaSelezionata { get; set; }

        public string TipoWidget { get; set; }

        public string UserNameCreazione { get; set; }
        public string UserNameModifica { get; set; }

    }

    public static class WidgetExtensions
    {
        public static Widget UserWidgetToCompleteWidget(this Widget userWidget, Widget systemWidget)
        {
            var cw = new Widget
            {
                IdWidget = userWidget.IdWidget,
                UserName = userWidget.UserName,
                Codice = systemWidget.Codice,
                Titolo = systemWidget.Titolo,
                Descrizione = systemWidget.Descrizione,
                TipoWidget = systemWidget.TipoWidget,
                Abilitato = userWidget.Abilitato,
                Visibile = userWidget.Visibile,
                PresetIniziale = systemWidget.PresetIniziale,
                RichiedeAziendaSelezionata = systemWidget.RichiedeAziendaSelezionata,
                Aspetto = (!String.IsNullOrEmpty(userWidget.Aspetto) && !String.IsNullOrWhiteSpace(userWidget.Aspetto)) ? userWidget.Aspetto: systemWidget.Aspetto,
                Parametri = (!String.IsNullOrEmpty(userWidget.Parametri) && !String.IsNullOrWhiteSpace(userWidget.Parametri)) ? userWidget.Parametri : systemWidget.Parametri,
                MultiAziendale = systemWidget.MultiAziendale
            };

            return cw;
        }


    }

    public enum Enum_Codice_Widget
    {
        ProdottiMovimentatiGiacenze = 0,
        Visite = 1,
        Attivita = 2,
        Rilievi = 3,
        Colture = 4,
        ProduzioneColture = 5
    }
}

