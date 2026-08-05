
import { RisorseUmane } from '../../anagrafiche/RisorseUmane';
import { RisorsaTimeSheet } from './RisorsaTimeSheet';

export class RisorsaPersona extends RisorsaTimeSheet {
    risorsaUmana: RisorseUmane;

    constructor() {
        super();
        this.classType = 'RisorsaPersona';
    }
}
