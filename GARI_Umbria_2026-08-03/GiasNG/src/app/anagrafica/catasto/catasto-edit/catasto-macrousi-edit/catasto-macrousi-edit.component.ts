import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroupDirective } from '@angular/forms';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import { Subscription } from 'rxjs';
import { MacrousiParticellaService } from './catasto-macrousi-edit.service';
import { MacrousiCatastoService, ParticelleCatastaliMacrousoId } from './MacrousiCatasto.service';

@Component({
    standalone: false,
    selector: 'app-catasto-macrousi-edit',
    templateUrl: './catasto-macrousi-edit.component.html',
    styleUrls: ['./catasto-macrousi-edit.component.css'],
    providers: [
        ...generateGridProviders(MacrousiParticellaService, CatastoMacrousiEditComponent)
    ]

})
export class CatastoMacrousiEditComponent implements OnInit {
    @Input() formGroupName: string;
    form: FormArray;
    formValue: string;
    subscriptions: Subscription[] = new Array<Subscription>();

    constructor(private macrousiCatastoService: MacrousiCatastoService,
        private rootFormGroup: FormGroupDirective,
        private funzioniComuniService: FunzioniComuniService,
        private fb: FormBuilder) { }

    ngOnInit(): void {
        this.form = this.rootFormGroup.control.get(this.formGroupName) as FormArray;
        this.macrousiCatastoService.setMacrousiParticella(this.form.value);
        this.formValue = JSON.stringify(this.form.value);
        this.subscriptions.push(this.form.valueChanges.subscribe((el) => {
            this.formValue = JSON.stringify(el);
        }));


        this.subscriptions.push(this.macrousiCatastoService.macrousiParticellaSource.subscribe((newMacrousoParticella: ParticelleCatastaliMacrousoId[]) => {
            UtilityFunctions.clearFormArray(this.form);
            for (const macrousoParticella of newMacrousoParticella) {
                const macrousoParticellaForm = this.getRowMacrousoParticella();
                macrousoParticellaForm.patchValue(<any>macrousoParticella);
                this.form.push(macrousoParticellaForm);
            }
            this.form.patchValue(newMacrousoParticella, { emitEvent: true, onlySelf: false });
            this.formValue = JSON.stringify(this.form.value);
        }));
    }

    ngOnDestroy(): void {
        for (const subs of this.subscriptions) {
            subs.unsubscribe();
        }
    }


    getRowMacrousoParticella() {
        return this.fb.group({
            macrouso: new FormControl({ codice: 0, descrizione: '' }),
            validita: this.fb.group({
                inizio: [AGRODATAINIZIO],
                fine: [AGRODATAFINE]
            }),
            Area: [0],
            Piva: [''],
            NumeroFascicolo: [''],
            DataValidazioneFascicolo: [AGRODATAINIZIO]
        });
    }

}
