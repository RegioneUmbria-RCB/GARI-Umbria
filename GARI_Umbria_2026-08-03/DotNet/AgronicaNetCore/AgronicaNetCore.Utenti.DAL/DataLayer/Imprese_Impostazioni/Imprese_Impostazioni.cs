using AgronicaCoreDTOStd.Identity;
using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioni;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Imprese_Impostazioni
{
    public class Imprese_Impostazioni : DAL_Base, IImprese_Impostazioni
    {
        private readonly IUtentiImpostazioni _utentiImpostazioni;

        public Imprese_Impostazioni(IServiceProvider provider) : base(provider)
        {
            _utentiImpostazioni = provider.GetRequiredService<IUtentiImpostazioni>();
        }

        public async Task<string> LeggiScalareMulticentroAziendaSuperUser_ElemCod(string piva, 
            List<int> sa_cod_lst, 
            Enum_Impostazioni_Utenti impostazioneCod, 
            int tipoProdotto, 
            string valoreDefault, 
            AgronicaCoreParametriUtenti objParametriUtenti, 
            AgronicaCoreParametriServer objParametriServer)
        {
            string valImpostazione = valoreDefault;

            // DT: in caso di nessun centro indicato, si aggiunge il centro "0" per attivare la scalarità su azienda 
            if (sa_cod_lst == null || sa_cod_lst.Count == 0)
            {
                sa_cod_lst = new List<int>() { 0 };
            }
                


            valImpostazione = await LeggiScalareCentroAziendaSuperUser_ElemCod(piva, sa_cod_lst[0], impostazioneCod, tipoProdotto, valoreDefault, objParametriUtenti, objParametriServer);

            foreach (var sa_cod in sa_cod_lst.Skip(1))
            {
                var valImpostazioneCorrente = await LeggiScalareCentroAziendaSuperUser_ElemCod(piva, sa_cod, impostazioneCod, tipoProdotto, valoreDefault, objParametriUtenti, objParametriServer);
                if (valImpostazioneCorrente != valImpostazione)
                    throw new Exception("ImpostazioneIncompatibileMulticentro " + impostazioneCod);
            }

            return valImpostazione;
        }

        private async Task<string> LeggiScalareCentroAziendaSuperUser_ElemCod(string piva, int sa_cod, Enum_Impostazioni_Utenti impostazioneCod, int tipoProdotto, string valoreDefault, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            string valImpostazione = await LeggiScalareCentroAziendaSuperUser(piva, sa_cod, impostazioneCod, valoreDefault, objParametriUtenti, objParametriServer);

            if (!string.IsNullOrEmpty(valImpostazione))
            {
                var impostazione = valImpostazione.Split("|");
                foreach (string s in impostazione)
                {
                    var elem_cod_impostazione = s.Split("_");
                    if (elem_cod_impostazione[0] == tipoProdotto.ToString())
                    {
                        valImpostazione = elem_cod_impostazione[1];
                        return valImpostazione;
                    }
                }
            }

            return valoreDefault;
        }

        private async Task<string> LeggiScalareCentroAziendaSuperUser(string piva, int sa_cod, Enum_Impostazioni_Utenti impostazioneCod, string valoreDefault, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {

            // TODO_DT: da capire come modificare quando arriverà la tabella nuova delle impostazioni con default e "solo azienda"
            string valImpostazione = valoreDefault;

            var strImpostazioni = await LeggiScalare_Centro_Azienda_SuperUser(piva, sa_cod, impostazioneCod, objParametriUtenti, objParametriServer);

            if (!string.IsNullOrEmpty(strImpostazioni))
                valImpostazione = strImpostazioni;

            return valImpostazione;
        }

        private async Task<string> LeggiScalare_Centro_Azienda_SuperUser(string piva, int sa_cod, Enum_Impostazioni_Utenti impostazione_cod, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            string nomeRoutine = "CoreAnagrafeDAL.Imprese_Impostazioni_R.LeggiScalare_Centro_Azienda_SuperUser()";
            string strImpostazione_Valore;

            try
            {
                strImpostazione_Valore = await LeggiScalare_Centro_Azienda(piva, sa_cod, (int) impostazione_cod, objParametriServer);

                if (string.IsNullOrEmpty(strImpostazione_Valore))
                {
                    var tuttiICentri = 0;
                    strImpostazione_Valore = await LeggiScalare_Centro_Azienda(piva, tuttiICentri, (int) impostazione_cod, objParametriServer);
                }

                if (string.IsNullOrEmpty(strImpostazione_Valore))
                {
                    strImpostazione_Valore = await _utentiImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(impostazione_cod, 2, objParametriUtenti, objParametriServer);
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw new Exception("[" + nomeRoutine + "] : " + ex.Message);
            }

            // DT: ritornando direttamente la stringa, il chiamante non riesce a capire da quale livello di scalarità arrivi. Se necessario fare un altro metodo
            return strImpostazione_Valore;
        }

        private async Task<string> LeggiScalare_Centro_Azienda(string piva, int sa_cod, int impostazione_cod, AgronicaCoreParametriServer objParametriServer)
        {
            string nomeRoutine = "Imprese_Impostazioni.LeggiScalare_Centro_Azienda()";

            string messaggioErrore = "";
            System.Text.StringBuilder stb = new System.Text.StringBuilder();
            DataTable dt;
            
            var parSql = new Dictionary<string, object>();

            if (impostazione_cod == 0)
                throw new Exception("Parametro impostazione_cod obbligatorio");

            try
            {
                stb.Length = 0;

                stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ");

                stb.AppendLine(" SELECT TOP 1 * FROM Imprese_Impostazioni ");

                stb.AppendLine(" WHERE Piva_SuperUser = @Piva_SuperUser ");
                stb.AppendLine(" AND Impostazione_cod = @Impostazione_cod ");

                parSql.Add("@Piva_SuperUser", objParametriServer.PivaSuperUser);
                parSql.Add("@Impostazione_cod", impostazione_cod);

                if (piva != "")
                {
                    stb.AppendLine(" AND Piva = @Piva ");
                    parSql.Add("@Piva", piva);
                }
                    

                // DT: lo 0 deve essere un valore di filtro, solo il -999 ha valore di "nessun filtro"
                if (sa_cod != CONTATTI_VISIBILITA.SACOD_NOFILTRO)
                {
                    stb.AppendLine(" AND (Sa_Cod = @Sa_Cod OR Sa_Cod = 0)");
                    parSql.Add("@Sa_Cod", sa_cod);
                }

                // DT: ordinamento prefissato per garantire la scalarità centro/azienda
                stb.AppendLine(" ORDER BY Sa_Cod DESC");

                // --------------------------------------------------------------------------
                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stb.ToString(), parSql);
            }
            // --------------------------------------------------------------------------

            catch (Exception ex)
            {
                messaggioErrore = ex.Message;
                LogError(ex.Message, objParametriServer, ex);
                dt = null;
                throw new Exception("[" + nomeRoutine + "] : " + messaggioErrore);
            }

            // DT: ritornando direttamente la stringa, il chiamante non riesce a capire da quale livello di scalarità arrivi. Se necessario fare un altro metodo
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["Impostazione_Valore"] != null && !string.IsNullOrEmpty(dt.Rows[0]["Impostazione_Valore"].ToString()))
                return dt.Rows[0]["Impostazione_Valore"].ToString();
            else
                return string.Empty;
        }



    }
}
