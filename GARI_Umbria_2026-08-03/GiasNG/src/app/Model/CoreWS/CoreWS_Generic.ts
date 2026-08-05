import { CoreWS_GenericObjP } from './CoreWS_GenericObjP';
import { CoreWS_GenericPayload } from './CoreWS_GenericPayload';

export class CoreWS_Generic<T> extends CoreWS_GenericPayload{

    InData: T;

    constructor(objP: CoreWS_GenericObjP, InData: T) {
        super(objP);
        this.InData = InData;
    }
}
