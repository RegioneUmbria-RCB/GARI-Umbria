import { Job, PKJob } from './Job';
import { TipiJob } from './TipiJob';

export class AttivitaCDG extends Job {
    constructor(codice: string) {
        super({classType:'AttivitaCDG',codice:codice} as PKJob);
    }

    getTipo(): TipiJob {
        return TipiJob.ATTIVITACDG;
    }


}
