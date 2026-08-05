using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.GerarchiaImprese
{
    public interface IGerarchiaImprese
    {
        /// <summary>
        /// DS-15 §Modifiche.1 — aggiunto parametro opzionale <c>livello</c>.
        /// Quando valorizzato aggiunge il filtro <c>AND GerarchiaImprese.Livello = @livello</c>.
        /// </summary>
        Task<DataTable> LeggixGerarchiaAlberoImprese_VisibilitaAsync(bool applicaVisibilita, AgronicaCoreParametriServer objParametriServer, int? livello = null);
        Task<DataTable> LeggiGerarchiaImpreseFiglieAsync(string piva, AgronicaCoreParametriServer objParametriServer);
        Task<List<string>> LeggiElencoGerarchiaImpreseFiglieAsync(string piva, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Recupera il cono di visibilità di un Utente specifico interrogando la gerarchia aziendale GIAS.
        /// Restituisce un <see cref="DataTable"/> con colonne <c>PivaPadre</c>, <c>PivaFiglio</c>,
        /// <c>Livello</c> e <c>Foglia</c> ordinate per livello e ragione sociale.
        /// </summary>
        /// <param name="idUtente">Username dell'Utente di cui recuperare la visibilità.</param>
        /// <param name="applicaFiltroVisibilita">
        /// <c>true</c> per filtrare tramite <c>Utenti_Visibilita_Appoggio</c>;
        /// <c>false</c> per visibilità totale (utente con <c>Descrizione_2</c> vuoto in <c>Utenti_Profili</c>).
        /// </param>
        /// <param name="objParametriServer">Parametri di connessione al DB Server operativo.</param>
        /// <remarks>
        /// Design Specification DS01-BL: RecuperoConoVisibilitaUtente - Persistenze Coinvolte.
        /// </remarks>
        Task<DataTable> LeggiConoVisibilitaAsync(string idUtente, bool applicaFiltroVisibilita, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Recupera la struttura organizzativa completa di tutte le Filiere e Aziende presenti nel
        /// database GIAS, inclusi i dati di geolocalizzazione della sede, senza filtri di autorizzazione.
        /// Restituisce sia le aziende figlio (con sede) sia le capofiliere root (con sede),
        /// tramite UNION che aggiunge le capofiliere come righe con <c>Livello = 0</c>.
        /// </summary>
        /// <param name="objParametriServer">Parametri di connessione al DB Server operativo.</param>
        /// <returns>
        /// <see cref="DataTable"/> con colonne: <c>PivaPadreDirecto</c>, <c>NomePadreDirecto</c>,
        /// <c>PivaAzienda</c>, <c>NomeAzienda</c>, <c>Livello</c>,
        /// <c>Stato_ISO</c>, <c>Regione</c>, <c>Citta</c>.
        /// Le righe con <c>Livello = 0</c> sono le Capofiliere (PivaPadreDirecto = null, PivaAzienda = root).
        /// </returns>
        /// <remarks>
        /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Persistenze Coinvolte.
        /// </remarks>
        Task<DataTable> LeggiStrutturaFiliereAsync(AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Recupera l'elenco degli utenti con profilo "Filiera Admin" dal database Utenti,
        /// inclusa la PIVA della Filiera di appartenenza per l'associazione.
        /// </summary>
        /// <param name="objParametriUtenti">Parametri di connessione al DB Utenti.</param>
        /// <returns>
        /// <see cref="DataTable"/> con colonne: <c>UserName</c>, <c>Cognome</c>, <c>Nome</c>,
        /// <c>Email</c>, <c>PivaFiliera</c>.
        /// </returns>
        /// <remarks>
        /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Mapping Admin di Filiera.
        /// </remarks>
        Task<DataTable> LeggiFilieraAdminAsync(AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Recupera la rubrica degli admin di filiera direttamente da DB, indicizzati per PIVA filiera.
        /// Utilizza <c>Utenti_Profili.Descrizione_2</c> come chiave di associazione alla filiera.
        /// </summary>
        /// <param name="objParametriServer">Parametri di connessione al DB Server operativo.</param>
        Task<Dictionary<string, AgronicaCoreDTOStd.InData.Anagrafica.FilieraAdminDto>> LeggiAdminFiliereAsync(AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Risale ricorsivamente la gerarchia a partire da <paramref name="figlio"/> fino alla radice,
        /// restituendo una riga per ogni livello percorso (colonne: <c>Padre</c>, <c>Figlio</c>, <c>Livello</c>).
        /// La ricorsione si ferma quando il nodo corrente non ha un padre nella tabella, oppure il
        /// campo <c>Padre</c> è stringa vuota.
        /// </summary>
        /// <param name="figlio">PIVA del nodo di partenza (Organismo_Referente dell'esercizio).</param>
        /// <param name="objParametriServer">Parametri di connessione al DB archivio.</param>
        Task<DataTable> LeggiPadriRicorsivaAsync(string figlio, AgronicaCoreParametriServer objParametriServer);
        /// <summary>
        /// Carica in un unico round-trip tutta la tabella <c>GerarchiaImprese</c> con la
        /// ragione sociale del figlio da <c>Imprese</c>. Il chiamante usa il DataTable per
        /// navigare la gerarchia in memoria senza ulteriori query.
        /// </summary>
        /// <param name="objParametriServer">Parametri di connessione al DB Server operativo.</param>
        /// <returns>
        /// <see cref="DataTable"/> con colonne: <c>Padre</c>, <c>PIVA</c>, <c>rag_soc</c>, <c>Foglia</c>.
        /// </returns>
        Task<DataTable> LeggiTuttaGerarchiaAsync(AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Restituisce la PIVA dell'azienda padre di gerarchia configurata per il gruppo utente
        /// corrente nel servizio GiasAPP (Id_Servizio = 100), oppure null se non configurata.
        /// </summary>
        Task<string?> LeggiPadreGerarchiaPerUtenteAsync(AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Restituisce i dati di base di un'azienda (Piva, Rag_Soc, TipoImpresaGerarchia, Padre, Livello, Foglia)
        /// senza filtro di visibilità e senza dati indirizzo. Usato per aggiungere l'azienda padre con dettagli ridotti.
        /// </summary>
        Task<DataRow?> LeggiImpreseBasePivaAsync(string piva, AgronicaCoreParametriServer objParametriServer);

    }
}
