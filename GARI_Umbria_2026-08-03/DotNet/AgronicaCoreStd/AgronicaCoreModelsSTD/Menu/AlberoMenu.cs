using AgronicaCoreModelsSTD.menu;
using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.Menu
{
    public class AlberoMenu
    {
        private List<LinkMenu> _menu = null;
        private List<LinkMenu> _menuPreferiti = null;

        public List<LinkMenu> Menus 
        {
            get { return _menu; }
            
            set { _menu = value; }
        }
        public List<LinkMenu> MenusPreferiti
        {
            get { return _menuPreferiti; }
            
            set { _menuPreferiti = value; }
        }

        public AlberoMenu() 
        {
            _menu = new List<LinkMenu>();
            _menuPreferiti = new List<LinkMenu>();    
        }

    }
}
