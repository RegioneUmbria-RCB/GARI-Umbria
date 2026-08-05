using System;
using System.Numerics;

namespace AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente
{
    public class PersonalizzazioniGraficheCliente
    {
        public string Title { get; set; }
        public string Logo_Favicon_PATH { get; set; }
        public string Logo_Login_PATH { get; set; }
        public string Logo_Navbar_PATH { get; set; }
        public string Logo_Footer_PATH { get; set; }
        public string Logo_Small_PATH { get; set; }
        public Boolean NascondiFooter { get; set; }
        public int? MostraLogoStampeFooter { get; set; }
        public string LogoFooterStampe { get; set; }
        public string TestoPreLogoFooterStampe { get; set; }
        public string TestoPostLogoFooterStampe { get; set; }
    }
}