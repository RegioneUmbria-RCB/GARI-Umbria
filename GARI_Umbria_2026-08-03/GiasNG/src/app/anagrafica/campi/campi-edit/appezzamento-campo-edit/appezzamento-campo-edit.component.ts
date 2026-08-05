import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroupDirective } from '@angular/forms';
import { MasterService } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { GridPublicService } from 'gias-kendo-grid';
import { generateGridProviders } from 'gias-kendo-grid';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import { Subscription } from 'rxjs';
import { Form_CampiEdit_Service } from '../form-campi-edit.service';
import { AppezzamentoCampoEditService } from './appezzamento-campo-edit-grid.service';
import { AppezzamentoCampoId, AppezzamentoCampoService } from './appezzamento-campo.service';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Component({
    standalone: false,
    selector: 'app-appezzamento-campo-edit',
    templateUrl: './appezzamento-campo-edit.component.html',
    styleUrls: ['./appezzamento-campo-edit.component.css'],
    providers: [
        ...generateGridProviders(AppezzamentoCampoEditService, AppezzamentiCampoEditComponent),
    ]
})

export class AppezzamentiCampoEditComponent implements OnInit, OnDestroy {
    @Input() formGroupName: string;
    form: FormArray;
    formValue: string;
    subscriptions: Subscription[] = new Array<Subscription>();

    constructor(
        private appezzamentoCampoService: AppezzamentoCampoService,
        private formCampiEditService: Form_CampiEdit_Service,
        private objParametriAgendaService: ObjParametriAgendaService,
        private masterService: MasterService,
        private rootFormGroup: FormGroupDirective,
        private kendoGridService: GridPublicService,
        private fb: FormBuilder) {
    }

    objParametriAgenda: ObjParametriAgenda;
    editSubscription: Subscription;
    tableDataSub: Subscription;
    ddlSub: Subscription;

    ngOnInit(): void {
        this.editSubscription = this.kendoGridService.changeDetected.subscribe((event: any) => {
          let a = 0; //Commento per funzione vuota SonarQube
        });

        this.form = this.rootFormGroup.control.get(this.formGroupName) as FormArray;
        this.appezzamentoCampoService.setAppezzamentoCampo(this.form.value);

        this.subscriptions.push(this.appezzamentoCampoService.appezzamentoCampoSource.subscribe((newAppezzamentoCampo: AppezzamentoCampoId[]) => {
            UtilityFunctions.clearFormArray(this.form);
            for (const appezzamento of newAppezzamentoCampo) {
                const appezzamentoCampoForm = this.formCampiEditService.getFormAppezzamentoCampo();
                appezzamentoCampoForm.patchValue(<any>appezzamento);
                this.form.push(appezzamentoCampoForm);
            }
            this.form.patchValue(newAppezzamentoCampo, { emitEvent: true, onlySelf: false });
            this.formValue = JSON.stringify(this.form.value);
        }));

        this.masterService.set_isLoading({ isLoading: true, message: 'Caricamento in corso' });
        try {
            this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
            if(this.objParametriAgenda.Piva === null || this.objParametriAgenda.Piva === undefined) {
                this.objParametriAgenda.Piva = '';
            }

        } catch (e) {
            console.log('Error:', e);
        }

        this.masterService.set_isLoading({ isLoading: false, message: '' });
    }

    ngOnDestroy(): void {
        for (const subs of this.subscriptions) {
            subs.unsubscribe();
        }
    }

}

