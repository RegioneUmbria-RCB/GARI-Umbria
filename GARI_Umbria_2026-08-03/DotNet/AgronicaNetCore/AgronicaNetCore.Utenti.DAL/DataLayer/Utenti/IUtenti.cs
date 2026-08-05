using AgronicaCoreDTOStd.Identity;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.profilazione;
using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.Utenti
{
    public interface IUtenti
    {
        Task<DataTable> LeggiAsync(AgronicaCoreParametriUtenti objParametriUtenti);
        Task<DataTable> LeggiUtenteAsync(string username, AgronicaCoreParametriUtenti objParametriUtenti);
        Task<bool> EsisteUtenteCuaaAsync(string cuaa, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiCodiceCuaaAsync(string cuaa, AgronicaCoreParametriServer objParametriServer);
        Task<string> LeggiPivaByCuaaAsync(string cuaa, AgronicaCoreParametriServer objParametriServer);
        Task<string> LeggiNumeroTesseraByCuaaAsync(string username, string cuaa, AgronicaCoreParametriServer objParametriServer);
        Task<LeggiScriviVisibilitaUtente> MapUserToAgronicaUtente(string pivaSuperUser, string piva, string username,
            UtenteColdiretti datiColdiretti, int profiloDefault, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Restituisce gli utenti la cui password scadrà entro il numero di giorni specificato,
        /// aventi email non nulla/vuota.
        /// </summary>
        Task<DataTable> LeggiUtentiInScadenzaPasswordAsync(int giorniScadenza, int giorniPreavviso, AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
