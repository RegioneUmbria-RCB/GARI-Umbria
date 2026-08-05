import {AfterContentInit, AfterViewChecked, AfterViewInit, Component, forwardRef, Inject, Input, OnInit} from '@angular/core';
import { ControlContainer, ControlValueAccessor, FormGroup, FormGroupDirective, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { KendoGridColumn } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { generateGridProviders } from 'gias-kendo-grid';
import { CODICI_TOKEN } from './models/codici.model';
import { CodiciTemplateConfigService } from './services/codici-template-config.service';
import { ICodiciTemplateService } from './services/codici-template.service';
import {CommandsColumnSettings, ToolbarSettings} from 'gias-kendo-grid';

@Component({
    standalone: false,
    selector: 'gias-codici-template',
    templateUrl: './codici-template.component.html',
    styleUrls: ['./codici-template.component.scss'],
    providers:[
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => CodiciTemplateComponent),
            multi: true
        },
        ...generateGridProviders(CodiciTemplateConfigService, CodiciTemplateComponent)
    ],
    viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class CodiciTemplateComponent implements OnInit, ControlValueAccessor, AfterViewInit {
    @Input() codiciForm: FormGroup;
    @Input() codici: Array<any>;
    @Input() lista_codici: Array<any>;
    @Input() text_field: string;
    @Input() value_field: string;
    @Input() dateControl: boolean;
    @Input() edit: boolean;

    codici_columns: Array<KendoGridColumn>;

    constructor(
        @Inject(CODICI_TOKEN) private codiciTemplateService: ICodiciTemplateService,
        private gridPublicService:GridPublicService,
        private translocoService: TranslocoService
    ) {
        this.codiciTemplateService.gridPublicService = this.gridPublicService;
    }

    public onTouched: () => void = () => {
      let a = 0; //Commento per funzione vuota SonarQube
    };

    ngOnInit(): void {
        this.codici_columns = new Array<KendoGridColumn>(
            new KendoGridColumn({ field: 'Descrizione', title: this.translocoService.translate('Codice') }),
            new KendoGridColumn({ field: 'Val_Cod', title: this.translocoService.translate('Valore') })
        );

        if (this.dateControl){
            this.codici_columns.push(
                new KendoGridColumn({ field: 'Validita_Inizio', title: this.translocoService.translate('Dal')})
            );
            this.codici_columns.push(
                new KendoGridColumn({ field: 'Validita_Fine', title: this.translocoService.translate('Al') })
            );
        }

        if(this.edit == false) {
            this.codiciTemplateService.disableCodesGrid();
        }
    }

    writeValue(val: any): void {
        if(val) {
            this.codiciForm.setValue(val, { emitEvent: false });
        }
    }

    registerOnChange(fn: any): void {
        this.codiciForm.valueChanges.subscribe(fn);
    }

    registerOnTouched(fn: any): void {
        this.onTouched = fn;
    }

    setDisabledState?(isDisabled: boolean): void {
        if(isDisabled) {
            this.codiciForm.disable();
        } else {
            this.codiciForm.enable();
        }
    }

    updateCodice(event: any){
      let a = 0; //Commento per funzione vuota SonarQube
    }

    cancelCodice(event: any){
      let a = 0; //Commento per funzione vuota SonarQube
    }

    saveCodice(event: any){
      let a = 0; //Commento per funzione vuota SonarQube
    }

    removeCodice(event: any){
      let a = 0; //Commento per funzione vuota SonarQube
    }

    addCodice(event: any){
      let a = 0; //Commento per funzione vuota SonarQube
    }

    pageChangeEvent(event: any){
      let a = 0; //Commento per funzione vuota SonarQube
    }

    ngAfterViewInit(): void {
        // if(this.edit == false) {
        //     console.log('edit = false -ViewInit !!!')
        //     //this.codiciTemplateService.gridPublicService.formGroup.value.disable();
        // }
      let a = 0; //Commento per funzione vuota SonarQube
    }

}
