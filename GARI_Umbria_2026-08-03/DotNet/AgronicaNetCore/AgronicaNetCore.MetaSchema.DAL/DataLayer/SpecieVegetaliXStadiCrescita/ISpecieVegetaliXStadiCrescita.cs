using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.SpecieVegetaliXStadiCrescita
{
    /// <summary>
    /// Interfaccia di lettura per la tabella <c>SpecieVegetaliXStadiCrescita</c>.
    /// Traduzione C# della funzione VB.NET <c>SpecieVegetaliXStadiCrescita_R.Leggi()</c>.
    /// Riferimento: DS02-BL ConfrontiFasiFenologicheQDCA — Regole di Business — Confronto per unicità.
    /// </summary>
    public interface ISpecieVegetaliXStadiCrescita
    {
        /// <summary>
        /// Legge le righe di <c>SpecieVegetaliXStadiCrescita</c> con i relativi join a
        /// <c>SpecieVegetali</c> e <c>Stadi_Crescita_BBCH</c>, applicando i filtri opzionali.
        /// Corrisponde alla firma della funzione VB.NET <c>Leggi()</c>.
        /// </summary>
        /// <param name="vegCod">Codice specie vegetale (0 = nessun filtro).</param>
        /// <param name="idBbch">Codice BBCH intero da <c>Stadi_Crescita_BBCH</c> (0 = nessun filtro).</param>
        /// <param name="ffCod">Chiave <c>FF_COD</c> / <c>Cod_SS</c> di <c>SpecieVegetaliXStadiCrescita</c> (0 = nessun filtro).</param>
        /// <param name="soloVisibili">Se <c>true</c>, filtra per <c>ss.Flag_Visibile = 1</c>.</param>
        /// <param name="soloFioritura">Se <c>true</c>, filtra per <c>ss.Flag_Fioritura = 1</c>.</param>
        /// <param name="soloRipresaVegetativa">Se <c>true</c>, filtra per <c>ss.Flag_RipresaVegetativa = 1</c>.</param>
        /// <param name="objParametriServer">Parametri server (connessione, visibilità).</param>
        /// <param name="agrodataFine">Data fine validità (default: <see cref="CostantiPersonalizzate.AGRODATAFINE"/>).</param>
        /// <param name="agrodataInizio">Data inizio validità (default: <see cref="CostantiPersonalizzate.AGRODATAINIZIO"/>).</param>
        Task<DataTable> LeggiAsync(
            int vegCod,
            int idBbch,
            int ffCod,
            bool soloVisibili,
            bool soloFioritura,
            bool soloRipresaVegetativa,
            AgronicaCoreParametriServer objParametriServer,
            DateTime? agrodataFine = null,
            DateTime? agrodataInizio = null);

        /// <summary>
        /// Recupero batch per lista di valori <c>Cod_SS</c> (corrispondenti a <c>FF_Classe</c>
        /// memorizzati in <c>Mov_Dettaglio_Tecnico</c> nel QDCA).
        /// Usato dal servizio <c>ConfrontiFasiFenologicheQDCA</c> per risolvere i codici BBCH
        /// già registrati nel quaderno di campagna.
        /// Riferimento: DS02-BL ConfrontiFasiFenologicheQDCA — Regole di Business — Confronto per unicità.
        /// </summary>
        /// <param name="codSsList">Lista di valori <c>Cod_SS</c> / <c>FF_COD</c> da cercare.</param>
        /// <param name="objParametriServer">Parametri server (connessione, visibilità).</param>
        /// <param name="agrodataFine">Data fine validità (default: <see cref="CostantiPersonalizzate.AGRODATAFINE"/>).</param>
        /// <param name="agrodataInizio">Data inizio validità (default: <see cref="CostantiPersonalizzate.AGRODATAINIZIO"/>).</param>
        Task<DataTable> LeggiPerCodSsListAsync(
            List<int> codSsList,
            AgronicaCoreParametriServer objParametriServer,
            DateTime? agrodataFine = null,
            DateTime? agrodataInizio = null);
    }
}
