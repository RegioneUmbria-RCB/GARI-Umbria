using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Anagrafica
{
  public class CentroDropdownLists
    {
        public string CodiceOperatore;
        public AspxDropdown Tipologia;
        public AspxDropdown TitoloPossesso;
        public AspxDropdown Provincie;
        public AspxDropdown Comune;
        public AspxDropdown Stati;
        public AspxDropdown Codici;
        public AspxDropdown TipoAttivita;
        public AspxDropdown OrganismiControllo;
        public AspxDropdown CentroAziendaleEsternoCollegato;
        public AspxDropdown Otes;
    }
    public class AspxDropdown
    {
        public AspxDropdownItem[] Items { get; set; }
    }
    public class AspxDropdownItem
    {
        public Boolean Enabled { get; set; }
        public Boolean Selected { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
    }
}
