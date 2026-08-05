import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';

@Injectable({ providedIn: 'root'})
export class Form_CampiEdit_Service {

    constructor(
        private fb: FormBuilder
    ) {  }

    getFormCodiciCampo() {
        return this.fb.group({
            codiceAnagrafe: this.getFormCodiceAnagrafe(),
            valore: [0],
            validita: this.fb.group({
                inizio: [AGRODATAINIZIO],
                fine: [AGRODATAFINE]
            }),
        });
    }

    getFormCodiceAnagrafe() {
        return this.fb.group({
            codice: [0],
            descrizione: ['']
        });
    }

    getFormAppezzamentoCampo() {
        return this.fb.group({
            piva: [''],
            sa_cod: [0],
            appezza: [0],
            campo_cod: [0],
            sup_app: [0],
            app_nome: [0],
            validita: this.fb.group({
                inizio: [AGRODATAINIZIO],
                fine: [AGRODATAFINE]
            }),
            id_reg: [0],
            validita_impianto: this.fb.group({
                inizio: [AGRODATAINIZIO],
                fine: [AGRODATAFINE]
            }),
            cul_cod: [0],
            cul_des: [''],
            veg_des: ['']
        });
    }

    getFormCatastoCampo(): FormGroup{
        return this.fb.group({
            particella: this.getFormParticella(),
            area: [0, Validators.required]
        });
    }

    getFormParticella(): FormGroup{
        return this.fb.group({
            primaryKey: this.getFormParticellaPK(),
            Area: [0, Validators.required]
        });
    }

    getFormParticellaPK(): FormGroup {
        return this.fb.group({
            Prov: ['', Validators.required],
            Com: ['', Validators.required],
            Sezione: [''],
            Foglio: [, Validators.required],
            Numero: [, Validators.required],
            Subalterno: ['']
        });
    }
}
