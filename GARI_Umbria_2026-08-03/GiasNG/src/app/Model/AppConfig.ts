export interface IAppConfig {
    env: {
        name: string;
    };

}

export class CoreWSRequest<T>{
    objP_super_server: string;
    objP_server: string;
    objP_utenti: string;
    InData: T;
}
