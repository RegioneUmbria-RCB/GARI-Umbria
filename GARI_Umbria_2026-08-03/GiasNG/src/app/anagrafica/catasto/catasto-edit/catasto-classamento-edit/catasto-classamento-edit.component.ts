import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroupDirective } from '@angular/forms';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import { Subscription } from 'rxjs';
import { ClassamentiParticellaService } from './catasto-classamento-edit.service';
import { ClassamentoCatastoService, ParticelleCatastaliClassamentoId } from './ClassamentoCatasto.service';

@Component({
    standalone: false,
    selector: 'app-catasto-classamento-edit',
    templateUrl: './catasto-classamento-edit.component.html',
    styleUrls: ['./catasto-classamento-edit.component.css'],
    providers: [
        ...generateGridProviders(ClassamentiParticellaService, CatastoClassamentoEditComponent)
    ]
})
export class CatastoClassamentoEditComponent implements OnInit, OnDestroy {
    @Input() formGroupName: string;
    form: FormArray;
    formValue: string;
    subscriptions: Subscription[] = new Array<Subscription>();

    constructor(private classamentoCatastoService: ClassamentoCatastoService,
        private rootFormGroup: FormGroupDirective,
        private funzioniComuniService: FunzioniComuniService,
        private fb: FormBuilder) { }

    ngOnInit(): void {
        this.form = this.rootFormGroup.control.get(this.formGroupName) as FormArray;
        this.classamentoCatastoService.setClassamentoParticella(this.form.value);
        this.formValue = JSON.stringify(this.form.value);
        this.subscriptions.push(this.form.valueChanges.subscribe((el) => {
            this.formValue = JSON.stringify(el);
        }));


        this.subscriptions.push(this.classamentoCatastoService.classamentoParticellaSource.subscribe((newClassamentoParticella: ParticelleCatastaliClassamentoId[]) => {
            UtilityFunctions.clearFormArray(this.form);
            for (const classamentoParticella of newClassamentoParticella) {
                const classamentoParticellaForm = this.getRowClassamentoParticella();
                classamentoParticellaForm.patchValue(<any>classamentoParticella);
                this.form.push(classamentoParticellaForm);
            }
            this.form.patchValue(newClassamentoParticella, { emitEvent: true, onlySelf: false });
            this.formValue = JSON.stringify(this.form.value);
        }));
    }

    ngOnDestroy(): void {
        for (const subs of this.subscriptions) {
            subs.unsubscribe();
        }
    }


    getRowClassamentoParticella() {
        return this.fb.group({
            qualita: new FormControl({ codice: 0, descrizione: '' }),
            Area: [0],
            porzione: [''],
            classe: [''],
            redditoDomiciliare: [''],
            redditoAgrario: ['']
        });
    }
}
