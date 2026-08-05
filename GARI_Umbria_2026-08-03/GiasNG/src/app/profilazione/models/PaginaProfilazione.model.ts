export enum enum_PagineProfilazione {
    UTENTI = 'users',
    UTENTI_IMPOSTAZIONI = 'userSettings',
    IMPRESE_IMPOSTAZIONI = 'businessSettings',
    GRUPPI = 'groups',
    PROFILI_PERMESSI = 'profilesPermissions',
    VISIBILITA_UTENTI = 'businessVisibility',
    CLIENTE_PERMESSI = 'clientPermissions'
}

export class PaginaProfilazioneItem {
    constructor(
        public titolo: string,
        public pagina: enum_PagineProfilazione,
        public url: string,
    ) {}
}
