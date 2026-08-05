import { APICallsBasic } from "./APICallsBasic";

export class CoreWS_Gis<T> extends APICallsBasic{
    public InData: T

    constructor (objP_super_server: string, objP_server: string, objP_utenti: string, inGisDataReadParam: T) {
        super(objP_super_server, objP_server, objP_utenti);
        this.InData = inGisDataReadParam;
    }
}
