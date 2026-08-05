import { Risorsa } from "./Risorsa";

export class RisorsaCausale extends Risorsa {
    
    id: number;
    causale: string;

    constructor() {
        super();
        this.classType = 'RisorsaCausale';
    }

}