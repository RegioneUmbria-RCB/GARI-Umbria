using AgronicaNetCore.Anagrafe.DAL.DataLayer.GerarchiaImprese;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiProfili;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Resources;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.FiliereAziende
{
    /// <summary>
    /// Returns the list of filiere (hierarchy level 2) visible to the calling user,
    /// used to populate the dropdown on the CO2 sustainability calculation page.
    /// </summary>
    /// <remarks>
    /// DS-15 — <c>GET v1/sostenibilita/filiere-aziende</c>.<br/>
    /// Visibility is determined dynamically on every call (no caching) by reading
    /// the user profile and the company hierarchy.
    /// </remarks>
    public class FiliereAziendeDropdownService : BaseServiceSostenibilitaCO2Biz, IFiliereAziendeDropdownService
    {
        private readonly IUtentiProfili _utentiProfili;
        private readonly IGerarchiaImprese _gerarchiaImprese;


        public FiliereAziendeDropdownService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _utentiProfili = _serviceProvider.GetRequiredService<IUtentiProfili>();
            _gerarchiaImprese = _serviceProvider.GetRequiredService<IGerarchiaImprese>();
        }

        public async Task<FiliereAziendeDropdownResult> GetFiliereAsync(AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            var username = objParametriUtenti.UtenteUsername;
            // DS-15 §Logica.1 — determine visibility filter from user profile (profilo 5).
            // DS-15 §Logica.2 — Descrizione_2 empty/null → regular user → apply filter.
            //                    Descrizione_2 valorizzata → superuser → no filter.
            bool filtroVisibilitaUtente = !await _utentiProfili.VisibilitaTotale(
                username, objParametriUtenti, objParametriServer, (int)Enum_Id_Servizio.GiasOnline);
            DataTable gerarchieDt;

            try
            {
                gerarchieDt = await _gerarchiaImprese.LeggixGerarchiaAlberoImprese_VisibilitaAsync(filtroVisibilitaUtente, objParametriServer, 2);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }


            if (gerarchieDt.Rows.Count == 0)
                return new FiliereAziendeDropdownResult();


            var filiere = gerarchieDt.AsEnumerable()
                .Select(row => new FilieraDropdownItem
                {
                    Piva = row.Field<string>("PIVA") ?? string.Empty,
                    RagioneSociale = row.Field<string>("rag_soc") ?? string.Empty,

                })
                .OrderBy(f => f.RagioneSociale)
                .ToList();

            return new FiliereAziendeDropdownResult { Filiere = filiere };
        }
    }
}
