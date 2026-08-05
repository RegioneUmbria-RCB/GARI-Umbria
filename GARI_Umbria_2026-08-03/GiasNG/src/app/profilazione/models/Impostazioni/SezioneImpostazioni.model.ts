import { FormGroup, FormControl, FormArray } from '@angular/forms';
import {BaseCodeDescr} from '../../../Model/baseClass/baseCodeDescr';
import { Impostazione } from 'gias-ui-kit';

export class SezioneImpostazioni extends BaseCodeDescr {
    espandibile: boolean = false;
    livelloSezione: number;
    note: string;
    children: SezioneImpostazioni[] = [];
    impostazioni: Impostazione[] = [];

    constructor(codice: number, descrizione?: string) {
        super(codice, descrizione);
    }

    public toForm(): FormGroup {
        const childrenArray = new FormArray([]);
        if (this.children.length > 0) {
            this.children.forEach(child => childrenArray.push(child.toForm()));
        }
        const settingsArray = new FormArray([]);
        if (this.impostazioni.length > 0) {
            this.impostazioni.forEach(imp => settingsArray.push(new FormControl(imp)));
        }
        return new FormGroup({
            codice: new FormControl(this.codice),
            descrizione: new FormControl(this.descrizione),
            espandibile: new FormControl(this.espandibile),
            livelloSezione: new FormControl(this.livelloSezione),
            note: new FormControl(this.note),
            children: childrenArray,
            impostazioni: settingsArray
        });
    }
}
