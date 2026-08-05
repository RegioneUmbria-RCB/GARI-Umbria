import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroupDirective } from '@angular/forms';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import { Subscription } from 'rxjs';
import { ZoneParticellaService } from './catasto-zone-edit.service';
import { ParticelleCatastaliZonaId, ZoneCatastoService } from './ZoneCatasto.service';

@Component({
    standalone: false,
    selector: 'app-catasto-zone-edit',
    templateUrl: './catasto-zone-edit.component.html',
    styleUrls: ['./catasto-zone-edit.component.css'],
    providers: [
        ...generateGridProviders(ZoneParticellaService, CatastoZoneEditComponent)
    ]

})
export class CatastoZoneEditComponent implements OnInit {
    @Input() formGroupName: string;
    form: FormArray;
    formValue: string;
    subscriptions: Subscription[] = new Array<Subscription>();

    constructor(
        private zoneCatastoService: ZoneCatastoService,
        private rootFormGroup: FormGroupDirective,
        private funzioniComuniService: FunzioniComuniService,
        private fb: FormBuilder) { }

    ngOnInit(): void {
        this.form = this.rootFormGroup.control.get(this.formGroupName) as FormArray;
        this.zoneCatastoService.setZoneParticella(this.form.value);
        this.formValue = JSON.stringify(this.form.value);
        this.subscriptions.push(this.form.valueChanges.subscribe((el) => {
            this.formValue = JSON.stringify(el);
        }));


        this.subscriptions.push(this.zoneCatastoService.zoneParticellaSource.subscribe((newZonaParticella: ParticelleCatastaliZonaId[]) => {
            UtilityFunctions.clearFormArray(this.form);
            for (const zonaParticella of newZonaParticella) {
                const zonaParticellaForm = this.getRowZonaParticella();
                zonaParticellaForm.patchValue(zonaParticella);
                this.form.push(zonaParticellaForm);
            }
            this.form.patchValue(newZonaParticella, { emitEvent: true, onlySelf: false });
            this.formValue = JSON.stringify(this.form.value);
        }));
    }

    ngOnDestroy(): void {
        for (const subs of this.subscriptions) {
            subs.unsubscribe();
        }
    }


    getRowZonaParticella() {
        return this.fb.group({
            zona: new FormControl({ codice: 0, descrizione: '' }),
            Area: [0],
            validita: this.fb.group({
                inizio: [AGRODATAINIZIO],
                fine: [AGRODATAFINE]
            }),
        });
    }

}
