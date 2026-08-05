using AgronicaCoreEntityFrameworkSTD_POCO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.models
{
    public class DatiIndiciMaturita
    {
        public List<APP_IndiciMaturita> ListaAPP_IndiciMaturita;
        public List<APP_IndiciMaturitaxSpecieVegetali> ListaAPP_IndiciMaturitaxSpecieVegetali;
        public List<APP_MisuraXIndiciMaturita> ListaAPP_MisuraXIndiciMaturita;
        public List<APP_MisuraXIndiciMaturita_Anagrafiche> ListaAPP_MisuraXIndiciMaturita_Anagrafiche;
    }
}
