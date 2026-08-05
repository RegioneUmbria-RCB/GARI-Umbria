using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Imprese_Impostazioni
{
    public interface IImprese_Impostazioni
    {
        public Task<string> LeggiScalareMulticentroAziendaSuperUser_ElemCod(string piva,
            List<int>? sa_cod_lst,
            Enum_Impostazioni_Utenti impostazioneCod,
            int tipoProdotto,
            string valoreDefault,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer);
    }
}
