namespace AgronicaCoreVisibilitaStd
{
    /// <summary>
    /// Variante di query generata da <see cref="VisibilitaQueryBuilder"/>.
    /// Rispecchia i due rami di <c>Filtrone.CreaStringaQueryPerDTFiltrone</c>:
    /// <see cref="Full"/> = <c>enum_TipoSelect_FiltroneSuperNova.Imprese</c> / <c>CentriAziendali</c>,
    /// <see cref="UvaLean"/> = <c>Imprese_Visibilita_Appoggio</c> / <c>CentriAziendali_Visibilita_Appoggio</c>.
    /// </summary>
    public enum ModalitaQuery
    {
        /// <summary>
        /// Query "ricca": include JOIN su ImpresexIndirizzi, Indirizzi, ISTAT, Lista_Province,
        /// e (per Imprese) Imprese_Codici (Codice_Socio + CodiceCUAA). SELECT con colonne anagrafiche
        /// complete. Corrisponde al percorso <c>InizializzaTabellaUtentiVisibilitaAppoggio_EF</c>.
        /// </summary>
        Full = 0,

        /// <summary>
        /// Query "snella": SELECT minimale (chiave/PIVA/rag_soc per Imprese),
        /// nessun JOIN su ImpresexIndirizzi/Indirizzi/ISTAT/Lista_Province/Imprese_Codici.
        /// Corrisponde al percorso <c>InizializzaTabellaUtentiVisibilitaAppoggio_GUID_NoTransaction</c>
        /// (login). Replica la forzatura <c>Join.bImpreseXIndirizzi = False</c> di Filtrone.vb:261.
        /// </summary>
        UvaLean = 1
    }
}
