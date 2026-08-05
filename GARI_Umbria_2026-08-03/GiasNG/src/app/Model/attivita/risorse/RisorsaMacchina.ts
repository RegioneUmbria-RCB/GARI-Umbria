import { ParcoMacchine } from '../../anagrafiche/ParcoMacchine';
import { RisorsaTimeSheet } from './RisorsaTimeSheet';

export class RisorsaMacchina extends RisorsaTimeSheet {
    macchina: ParcoMacchine;

    constructor() {
        super();
        this.classType = 'RisorsaMacchina';
    }
}
