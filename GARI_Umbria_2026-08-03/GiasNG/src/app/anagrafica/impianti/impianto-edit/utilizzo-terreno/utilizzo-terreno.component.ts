import { Component, EventEmitter, Inject, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormGroupDirective, Validators } from '@angular/forms';
import { Impianto } from 'app/Model/anagrafiche/Impianto';
import { DestinazioneUso } from 'app/Model/metaschema/utilizzi/DestinazioneUso';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { UtilizzoTerreno } from 'app/Model/metaschema/utilizzi/UtilizzoTerreno';
import { Varieta } from 'app/Model/metaschema/utilizzi/Varieta';
import { DestinazioneUsoService } from 'app/Service/Metaschema/destinazioneUso.service';
import { SpecieVegetaliService } from 'app/Service/Metaschema/specie-vegetali.service';
import { VarietaService } from 'app/Service/Metaschema/varieta.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { ImpiantiFactoryService, IMPIANTI_SERVICE_TOKEN } from 'app/Service/ServiceFactory/impianti.factory.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import { Subject, takeUntil } from 'rxjs';
import { SharedDataService } from '../../../../GIS/services/shared-data.service';
import { AppezzamentoEditService } from "../../../appezzamenti/appezzamenti-edit/appezzamento-edit.service";
import { GiasFormErrorVisualizerService } from 'gias-ui-kit';
import { SementieriService } from 'app/Service/sementieri.service';


@Component({
    standalone: false,
    selector: 'app-utilizzo-terreno',
    templateUrl: './utilizzo-terreno.component.html',
    styleUrls: ['./utilizzo-terreno.component.css'],
    providers: [GiasDropDownTemplateService]
})
export class UtilizzoTerrenoComponent implements OnInit, OnDestroy {
    @Input() formGroupName: string;
    form: FormGroup;
    @Output() specieChanged = new EventEmitter<Specie>();

    public signal$: Subject<void> = new Subject();

    changeInside: boolean;
    completeFormControlName = '';
    name = "Utilizzo Terreno";

    UtilizzoTerrenoForm = this.fb.group({
        terrenoNudo: [false, Validators.required],
        destinazioneUso: new FormControl({ codice: 0, descrizione: '' }),
        specie: new FormControl({ codice: 0, descrizione: '' }),
        varieta: new FormControl({ codice: 0, descrizione: '' })
    });

    UtilizzoTerrenoObj = {
        terrenoNudo: true,
        destinazioneUso: { codice: 0, descrizione: '' },
        specie: { codice: 0, descrizione: '' },
        varieta: { codice: 0, descrizione: '' }
    };

    isSementieri$ = this.sementieriService.isSementieriSportello();

    constructor(private rootFormGroup: FormGroupDirective,
        private specieService: SpecieVegetaliService,
        private varietaService: VarietaService,
        private destinazioniService: DestinazioneUsoService,
        private objParametriAgendaService: ObjParametriAgendaService,
        @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
        private formErrorVisualizerService: GiasFormErrorVisualizerService,
        private fb: FormBuilder,
        private ddlservice: GiasDropDownTemplateService,
        private sharedDataService: SharedDataService,
        private appezzamentoEditService: AppezzamentoEditService,
        private sementieriService: SementieriService
    ) { }


    ngOnDestroy(): void {
        this.signal$.next();
        this.signal$.complete();
    }

    async ngOnInit() {
        this.form = this.rootFormGroup.control.get(this.formGroupName) as FormGroup;
        this.completeFormControlName = UtilityFunctions.recursiveParentName(this.form);
        this.parentFormUpdated(this.form.value);
        this.form.valueChanges.pipe(takeUntil(this.signal$)).subscribe((newValue) => {
            if (!this.form.enabled) {
                this.UtilizzoTerrenoForm.disable({ emitEvent: false });
            }
            if (!this.changeInside) {
                this.parentFormUpdated(newValue);
            }
            this.changeInside = true;
        });

        if (this.form.enabled) {
            this.UtilizzoTerrenoForm.enable({ emitEvent: false });
        } else {
            this.UtilizzoTerrenoForm.disable({ emitEvent: false });
        }

        this.form.statusChanges.pipe(takeUntil(this.signal$)).subscribe((val) => {
            if (this.form.enabled) {
                this.UtilizzoTerrenoForm.enable({ emitEvent: false });
            } else {
                this.UtilizzoTerrenoForm.disable({ emitEvent: false });
            }
            this.updateFormErrorVisualizer();
        });

        this.form.valueChanges.pipe(takeUntil(this.signal$)).subscribe((val) => {
            this.updateFormErrorVisualizer();
        });

        this.UtilizzoTerrenoForm.valueChanges.pipe(takeUntil(this.signal$)).subscribe(el => {
            this.onChangeFormGroup(<any>el);
        });

        this.ddlservice.currentDropDownValueObject.pipe(takeUntil(this.signal$)).subscribe(async (el) => {
            switch (el.FormControlName) {
                case 'specie':
                    this.onChangeSpecie(<Specie>el.Value);
                    this.UtilizzoTerrenoForm.controls['varieta'].setValue({
                        codice: 0,
                        descrizione: ''
                    });
                    break;
                case 'varieta':
                    break;
                case 'destinazioneUso':
                    break;
            }
        });

        const objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        if (objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read) {
            setTimeout(() => {
                this.UtilizzoTerrenoForm.disable();
            }, 200);
        }

        if (objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Update && (<Impianto>this.rootFormGroup.value).primaryKey.codice != 0) {
            this.appezzamentiService.verifica_OperazioniAgenda(this.rootFormGroup.value).pipe(takeUntil(this.signal$)).subscribe(r => {

                if (r.RispostaStringa['movimentazioni']) {
                    this.UtilizzoTerrenoForm.controls['specie'].disable();
                    this.UtilizzoTerrenoForm.controls['terrenoNudo'].disable();
                    this.UtilizzoTerrenoForm.controls['destinazioneUso'].disable();
                    if (r.RispostaStringa['trattamentiConcimazioni']) {
                        this.rootFormGroup.form.controls['gruppoFinalita'].disable();
                    }
                }
            });
        }

        this.updateFormErrorVisualizer();

    }

    updateFormErrorVisualizer() {
        this.formErrorVisualizerService.handleFormControl(this.name, this.completeFormControlName, this.form, this.appezzamentoEditService.savings.getValue());
    }

    async parentFormUpdated(formUpd: UtilizzoTerreno) {
        const Proms_Arr = new Array();
        switch (formUpd.classType) {
            case 'Varieta':
                const formUpdV = <Varieta>formUpd;
                this.UtilizzoTerrenoObj.terrenoNudo = false;
                this.UtilizzoTerrenoObj.destinazioneUso = { codice: 0, descrizione: '' };
                this.UtilizzoTerrenoObj.specie = {
                    codice: formUpdV.specie.codice,
                    descrizione: formUpdV.specie.descrizione
                };
                this.UtilizzoTerrenoObj.varieta = {
                    codice: formUpdV.codice,
                    descrizione: formUpdV.descrizione
                };

                this.UtilizzoTerrenoForm.controls['specie'].addValidators([UtilityFunctions.forbiddenDdlValidator]);
                this.UtilizzoTerrenoForm.controls['varieta'].addValidators([UtilityFunctions.forbiddenDdlValidator]);
                this.UtilizzoTerrenoForm.controls['destinazioneUso'].removeValidators([UtilityFunctions.forbiddenDdlValidator]);

                break;
            case 'DestinazioneUso':
                const formUpdTU = <DestinazioneUso>formUpd;
                this.UtilizzoTerrenoObj.terrenoNudo = true;
                this.UtilizzoTerrenoObj.destinazioneUso = {
                    codice: formUpdTU.codice,
                    descrizione: formUpdTU.descrizione
                };
                this.UtilizzoTerrenoObj.specie = { codice: 0, descrizione: '' };
                this.UtilizzoTerrenoObj.varieta = { codice: 0, descrizione: '' };

                this.UtilizzoTerrenoForm.controls['specie'].removeValidators([UtilityFunctions.forbiddenDdlValidator]);
                this.UtilizzoTerrenoForm.controls['varieta'].removeValidators([UtilityFunctions.forbiddenDdlValidator]);
                this.UtilizzoTerrenoForm.controls['destinazioneUso'].addValidators([UtilityFunctions.forbiddenDdlValidator]);
                this.UtilizzoTerrenoForm.patchValue(this.UtilizzoTerrenoObj)
                break;
        }

        //this.UtilizzoTerrenoForm.markAllAsTouched();
        //this.UtilizzoTerrenoForm.updateValueAndValidity({ onlySelf: true, emitEvent: false });
        this.UtilizzoTerrenoForm.patchValue(this.UtilizzoTerrenoObj);
    }

    onChangeFormGroup(newValue: FormGroup) {
        this.changeInside = true;
        this.UtilizzoTerrenoObj = this.UtilizzoTerrenoForm.getRawValue();
        if (this.UtilizzoTerrenoObj.destinazioneUso.codice == 0 && this.UtilizzoTerrenoObj.varieta.codice == 0) {
            if (this.UtilizzoTerrenoObj.terrenoNudo) {
                this.form.setValue({
                    codice: 0,
                    descrizione: '',
                    classType: 'DestinazioneUso'
                }, { emitEvent: true });
            } else {
                this.form.setValue({
                    codice: 0,
                    descrizione: '',
                    specie: {
                        codice: 0,
                        descrizione: ''
                    },
                    classType: 'Varieta'
                }, { emitEvent: true });
            }
        } else {
            if (this.UtilizzoTerrenoObj.destinazioneUso.codice != 0) {
                this.form.setValue({
                    codice: this.UtilizzoTerrenoObj.destinazioneUso.codice,
                    descrizione: this.UtilizzoTerrenoObj.destinazioneUso.descrizione,
                    classType: 'DestinazioneUso'
                }, { emitEvent: true });
            } else {
                this.form.setValue({
                    codice: this.UtilizzoTerrenoObj.varieta.codice,
                    descrizione: this.UtilizzoTerrenoObj.varieta.descrizione,
                    specie: {
                        codice: this.UtilizzoTerrenoObj.specie.codice,
                        descrizione: this.UtilizzoTerrenoObj.specie.descrizione
                    },
                    classType: 'Varieta'
                }, { emitEvent: true });
            }
        }
    }

    async onChangeChkTerrenoNudo(val: boolean) {
        this.UtilizzoTerrenoObj.destinazioneUso = { codice: 0, descrizione: '' };
        this.UtilizzoTerrenoObj.specie = { codice: 0, descrizione: '' };
        this.UtilizzoTerrenoObj.varieta = { codice: 0, descrizione: '' };
        if (val) {
            this.UtilizzoTerrenoForm.controls['terrenoNudo'].setValue(true);
            // this.DestinazioniUso = await this.destinazioniService.leggi();
            this.UtilizzoTerrenoForm.controls['specie'].setValue({ codice: 0, descrizione: '' })
            this.UtilizzoTerrenoForm.controls['varieta'].setValue({ codice: 0, descrizione: '' })

            this.UtilizzoTerrenoForm.controls['specie'].removeValidators([UtilityFunctions.forbiddenDdlValidator]);
            this.UtilizzoTerrenoForm.controls['varieta'].removeValidators([UtilityFunctions.forbiddenDdlValidator]);
            this.UtilizzoTerrenoForm.controls['destinazioneUso'].addValidators([UtilityFunctions.forbiddenDdlValidator]);

        } else {
            this.UtilizzoTerrenoForm.controls['terrenoNudo'].setValue(false);
            this.UtilizzoTerrenoForm.controls['destinazioneUso'].setValue({ codice: 0, descrizione: '' })

            this.UtilizzoTerrenoForm.controls['specie'].addValidators([UtilityFunctions.forbiddenDdlValidator]);
            this.UtilizzoTerrenoForm.controls['varieta'].addValidators([UtilityFunctions.forbiddenDdlValidator]);
            this.UtilizzoTerrenoForm.controls['destinazioneUso'].removeValidators([UtilityFunctions.forbiddenDdlValidator]);

            // this.SpecieVegetali = await this.specieService.leggi_FiltroUtente();
            // this.Cultivar = new Array();
        }

        this.UtilizzoTerrenoForm.markAllAsTouched();
        this.UtilizzoTerrenoForm.updateValueAndValidity({ onlySelf: true, emitEvent: false });
        this.UtilizzoTerrenoForm.patchValue(this.UtilizzoTerrenoObj);
    }

    onChangeSpecie(specie: Specie) {
        this.varietaService.leggiVarietaAltreAsObs(specie).subscribe(cultivarAltre => {
            if (cultivarAltre) {
                this.UtilizzoTerrenoObj.varieta = {
                    codice: cultivarAltre.codice,
                    descrizione: cultivarAltre.descrizione
                };
                this.UtilizzoTerrenoForm.patchValue(this.UtilizzoTerrenoObj);
            }
            this.specieChanged.emit(specie);
        });
    }

    async openDdl(ddlEl: GiasDropDownTemplateSComponent, formName: string) {
        switch (formName) {
            case 'destinazioneUso':
                UtilityFunctions.loadDropDownItems(ddlEl, this.destinazioniService.leggi());
                break;
            case 'specie':
                UtilityFunctions.loadDropDownItems(ddlEl, this.specieService.leggi_FiltroUtente(this.sharedDataService?.getCfgSementiAsValue()));
                break;
            case 'varieta':
                UtilityFunctions.loadDropDownItemsObs(ddlEl, this.varietaService.leggiAsObs(this.UtilizzoTerrenoForm.getRawValue().specie));
                break;
        }
    }

}
