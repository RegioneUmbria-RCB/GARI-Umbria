import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroupDirective, Validators } from '@angular/forms';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import { Subscription } from 'rxjs';
import { MetodoProduzioneParticellaService } from './catasto-metodo-produzione-edit.service';
import { MetodoProduzioneId, MetodoProduzioneService } from './MetodoProduzioneService';

@Component({
    standalone: false,
    selector: 'app-catasto-metodo-produzione-edit',
    templateUrl: './catasto-metodo-produzione-edit.component.html',
    styleUrls: ['./catasto-metodo-produzione-edit.component.css'],
    providers: [
        ...generateGridProviders(MetodoProduzioneParticellaService, CatastoMetodoProduzioneEditComponent)
    ]
})
export class CatastoMetodoProduzioneEditComponent implements OnInit, OnDestroy {

    @Input() formGroupName: string;
    form: FormArray;
    formValue: string;
    subscriptions: Subscription[] = new Array<Subscription>();

    constructor(private metodoProduzioneService: MetodoProduzioneService,
        private rootFormGroup: FormGroupDirective,
        private funzioniComuniService: FunzioniComuniService,
        private fb: FormBuilder) { }

    ngOnInit(): void {
        this.form = this.rootFormGroup.control.get(this.formGroupName) as FormArray;
        this.metodoProduzioneService.setMetodoProduzioneParticella(this.form.value);
        this.formValue = JSON.stringify(this.form.value);
        this.subscriptions.push(this.form.valueChanges.subscribe((el) => {
            this.formValue = JSON.stringify(el);
        }));


        this.subscriptions.push(this.metodoProduzioneService.metodoProduzioneParticellaSource.subscribe((newMetodoProduzioneParticella: MetodoProduzioneId[]) => {
            UtilityFunctions.clearFormArray(this.form);
            for (const metodoProduzioneParticella of newMetodoProduzioneParticella) {
                const metodoProduzioneParticellaForm = this.getRowMetodoProduzioneParticella();
                metodoProduzioneParticellaForm.patchValue(metodoProduzioneParticella);
                this.form.push(metodoProduzioneParticellaForm);
            }
            this.form.patchValue(newMetodoProduzioneParticella, { emitEvent: true, onlySelf: false });
            this.formValue = JSON.stringify(this.form.value);
        }));
    }

    ngOnDestroy(): void {
        for (const subs of this.subscriptions) {
            subs.unsubscribe();
        }
    }


    getRowMetodoProduzioneParticella() {
        return this.fb.group({
            metodoProduzione: new FormControl({ codice: 0, descrizione: '' }),
            validita: this.fb.group({
                inizio: [AGRODATAINIZIO],
                fine: [AGRODATAFINE]
            })
        });
    }


}
