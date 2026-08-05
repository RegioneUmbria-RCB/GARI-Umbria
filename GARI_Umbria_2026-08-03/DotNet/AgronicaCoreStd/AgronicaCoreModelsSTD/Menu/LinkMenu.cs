using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.menu
{
    public class LinkMenu
    {
        private List<LinkMenu> _figli = null;

        public List<LinkMenu> Figli 
        {
            get { return _figli; }
            set { _figli = value; }
        }

        public string colore { get; set; }
        public string testo { get; set; }
        public string idHtml { get; set; }
        public int idSezione { get; set; }
        public int sitoRichiesto { get; set; }
        public int paginaRichiesta { get; set; }
        public int richiedeAziendaSelezionata { get; set; }
        public string redirectUrl { get; set; }
        public bool preferito { get; set; }
        public string classeCssIcona { get; set; }
        public bool presetIniziale { get; set; }
        public string coloreAlternativo { get; set; }
        
        public int enum_TipoAperturaPagina { get; set; }
        public LinkMenu() 
        {
            _figli = new List<LinkMenu>();
        }
    }
}
