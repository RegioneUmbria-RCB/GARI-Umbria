using AgronicaCoreModelsSTD.App;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.UtentiImpostazioni;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioni;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiPermessi;
using AgronicaNetCore.Base.Base;
using Newtonsoft.Json;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.APP.BIZ.Services.ImpostazioniApp;

public sealed class ImpostazioniAppService : IImpostazioniAppService
{
    private readonly IUtentiPermessi _utentiPermessi;
    private readonly IUtentiImpostazioni _utentiImpostazioni;
    private readonly ILoggingService _loggingService;

    public ImpostazioniAppService(
        IUtentiPermessi utentiPermessi,
        IUtentiImpostazioni utentiImpostazioni,
        ILoggingService loggingService
    )
    {
        _utentiPermessi = utentiPermessi;
        _utentiImpostazioni = utentiImpostazioni;
        _loggingService = loggingService;
    }

    public async Task<PermessiUtenteSincronizzazioneEntity> LeggiPermessiAppAsync(
        AgronicaCoreParametriUtenti objParametriUtenti,
        AgronicaCoreParametriServer objParametriServer
    )
    {
        PermessiUtenteSincronizzazioneEntity permessiSicnronizzazione = new();

        bool gestionePermessi =
            await _utentiImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(
                Enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_Permessi,
                (int)ModalitaLeggiImpostazioniUtente.SuperUser,
                objParametriUtenti,
                objParametriServer
            ) != "0";

        bool permessiGiasApp = await _utentiPermessi.ControllaPermessiUtenteAsync(
            objParametriUtenti.UtenteUsername,
            Enum_Id_Servizio.GiasOnline,
            Enum_Security_Attivita.GiasAPP_Permessi,
            Enum_Security_Operazione.Lettura,
            DateTime.Now,
            objParametriUtenti,
            objParametriServer
        );

        if (!permessiGiasApp || !gestionePermessi)
            return permessiSicnronizzazione;

        LeggiPermessiAppResult permessiResult = await LeggiPermessiSincronizzazioneAppAsync(
            objParametriUtenti,
            objParametriServer
        );

        ApplyMappedPermissions(permessiSicnronizzazione, permessiResult.MappedPermissions);
        ApplyUserPermissions(permessiSicnronizzazione, permessiResult.MappedUserPermissions);

        return permessiSicnronizzazione;
    }

    public async Task<ImpostazioniAPP?> LeggiImpostazioniAppAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
    {
        ImpostazioniAPP? impostazioniApp = null;
        var impostazioni = await _utentiImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(Enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI, 2,
            objParametriUtenti, objParametriServer);
        
        if (!string.IsNullOrEmpty(impostazioni))
        {
            impostazioniApp = JsonConvert.DeserializeObject<ImpostazioniAPP>(impostazioni);
        }

        return impostazioniApp;
    }

    private async Task<LeggiPermessiAppResult> LeggiPermessiSincronizzazioneAppAsync(
        AgronicaCoreParametriUtenti objParametriUtenti,
        AgronicaCoreParametriServer objParametriServer
    )
    {
        var permessi = new Dictionary<Enum_Security_Attivita, Enum_Impostazioni_Utenti>
        {
            {
                Enum_Security_Attivita.GiasAPP_NUOVA_RICETTA,
                Enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_NUOVA_RICETTA
            },
            {
                Enum_Security_Attivita.GiasAPP_NUOVO_INTERVENTO,
                Enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_NUOVO_INTERVENTO
            },
            {
                Enum_Security_Attivita.GiasAPP_INTERVENTI_DA_FARE,
                Enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_INTERVENTI_DA_FARE
            },
            {
                Enum_Security_Attivita.GiasAPP_VISITE,
                Enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_VISITE
            },
            {
                Enum_Security_Attivita.GiasAPP_DOCUMENTI,
                Enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_DOCUMENTI
            },
            {
                Enum_Security_Attivita.GiasAPP_RILIEVI,
                Enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_RILIEVI
            },
        };

        var permessiUtente = new List<Enum_Security_Attivita>
        {
            Enum_Security_Attivita.GiasAPP_PianoColturale,
            Enum_Security_Attivita.GiasAPP_Magazzini,
            Enum_Security_Attivita.GiasAPP_Macchine,
            Enum_Security_Attivita.GiasAPP_Manutenzioni,
            Enum_Security_Attivita.GiasAPP_DDT_Movimenti,
            Enum_Security_Attivita.GiasAPP_Gias,
            Enum_Security_Attivita.GiasAPP_Aziende,
            Enum_Security_Attivita.GiasAPP_Centri,
            Enum_Security_Attivita.GiasAPP_Isolamenti,
            Enum_Security_Attivita.GiasAPP_DSS_Difesa,
            Enum_Security_Attivita.GiasAPP_Consiglio_Irriguo,
            Enum_Security_Attivita.GiasAPP_Consiglio_Fertirriguo,
            Enum_Security_Attivita.GiasAPP_Precision_Farming,
            Enum_Security_Attivita.GiasAPP_Monitoraggio_Meteo,
        };

        var operazioniDaControllare = new List<Enum_Security_Operazione>
        {
            Enum_Security_Operazione.Lettura,
            Enum_Security_Operazione.Modifica,
        };

        IReadOnlyDictionary<
            Enum_Security_Attivita,
            IReadOnlySet<Enum_Security_Operazione>
        > risultatiMappati = await _utentiPermessi.ControllaPermessiUtenteAsync(
            objParametriUtenti.UtenteUsername,
            Enum_Id_Servizio.GiasOnline,
            permessi.Keys.ToList(),
            operazioniDaControllare,
            DateTime.Now,
            objParametriUtenti,
            objParametriServer
        );

        var mappedPermissions = new Dictionary<Enum_Impostazioni_Utenti, string>(permessi.Count);
        foreach (var (attivita, impostazioneCod) in permessi)
        {
            IReadOnlySet<Enum_Security_Operazione> permesso = risultatiMappati.TryGetValue(
                attivita,
                out var p
            )
                ? p
                : new HashSet<Enum_Security_Operazione>();
            mappedPermissions[impostazioneCod] =
                permesso.Contains(Enum_Security_Operazione.Modifica) ? "1"
                : permesso.Contains(Enum_Security_Operazione.Lettura) ? "2"
                : "0";
        }

        IReadOnlyDictionary<
            Enum_Security_Attivita,
            IReadOnlySet<Enum_Security_Operazione>
        > risultatiExtra = await _utentiPermessi.ControllaPermessiUtenteAsync(
            objParametriUtenti.UtenteUsername,
            Enum_Id_Servizio.GiasOnline,
            permessiUtente,
            operazioniDaControllare,
            DateTime.Now,
            objParametriUtenti,
            objParametriServer
        );

        var mappedUserPermissions = new Dictionary<Enum_Security_Attivita, string>(
            permessiUtente.Count
        );
        foreach (var attivita in permessiUtente)
        {
            IReadOnlySet<Enum_Security_Operazione> permesso = risultatiExtra.TryGetValue(
                attivita,
                out var p
            )
                ? p
                : new HashSet<Enum_Security_Operazione>();
            mappedUserPermissions[attivita] =
                permesso.Contains(Enum_Security_Operazione.Modifica) ? "1"
                : permesso.Contains(Enum_Security_Operazione.Lettura) ? "2"
                : "0";
        }

        return new LeggiPermessiAppResult(mappedPermissions, mappedUserPermissions);
    }

    private static void ApplyMappedPermissions(
        PermessiUtenteSincronizzazioneEntity entity,
        IReadOnlyDictionary<Enum_Impostazioni_Utenti, string> mapped
    )
    {
        // attivita = ricette OR nuovi interventi
        entity.Attivita =
            IsGranted(mapped, Enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_NUOVO_INTERVENTO)
            || IsGranted(mapped, Enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_INTERVENTI_DA_FARE)
            || IsGranted(mapped, Enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_NUOVA_RICETTA);
        entity.Rilievi = IsGranted(mapped, Enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_RILIEVI);
        entity.Visite = IsGranted(mapped, Enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_VISITE);
        entity.Documenti = IsGranted(mapped, Enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_DOCUMENTI);
    }

    private static void ApplyUserPermissions(
        PermessiUtenteSincronizzazioneEntity entity,
        IReadOnlyDictionary<Enum_Security_Attivita, string> userMapped
    )
    {
        entity.PianoColturale = IsGranted(
            userMapped,
            Enum_Security_Attivita.GiasAPP_PianoColturale
        );
        entity.Magazzini =
            IsGranted(userMapped, Enum_Security_Attivita.GiasAPP_Magazzini)
            || IsGranted(userMapped, Enum_Security_Attivita.GiasAPP_DDT_Movimenti);
        entity.Macchine =
            IsGranted(userMapped, Enum_Security_Attivita.GiasAPP_Macchine)
            || IsGranted(userMapped, Enum_Security_Attivita.GiasAPP_Manutenzioni);
        entity.Aziende =
            IsGranted(userMapped, Enum_Security_Attivita.GiasAPP_Aziende)
            || IsGranted(userMapped, Enum_Security_Attivita.GiasAPP_Centri);
    }

    private static bool IsGranted(
        IReadOnlyDictionary<Enum_Impostazioni_Utenti, string> dict,
        Enum_Impostazioni_Utenti key
    ) => dict.TryGetValue(key, out var v) && v != "0";

    private static bool IsGranted(
        IReadOnlyDictionary<Enum_Security_Attivita, string> dict,
        Enum_Security_Attivita key
    ) => dict.TryGetValue(key, out var v) && v != "0";

    public sealed record LeggiPermessiAppResult(
        IReadOnlyDictionary<Enum_Impostazioni_Utenti, string> MappedPermissions,
        IReadOnlyDictionary<Enum_Security_Attivita, string> MappedUserPermissions
    );
}

