import { TipiJob } from './TipiJob';

export type PKJob = typeof Job.PK.prototype;
export abstract class Job {
    public static PK = class JobPK {
        classType: string;
        codice: string;

        constructor(classType: string, codice: string) {
            this.classType = classType;
            this.codice = codice;
        }
    };

    primaryKey: PKJob;
    descrizione: string;
    jobConseguenti: Job[];

    constructor(primaryKey: PKJob) {
        this.primaryKey = primaryKey;
    }
    abstract getTipo(): TipiJob;

}
