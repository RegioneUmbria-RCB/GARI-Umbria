import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from "@angular/core";
import { FormArray, FormBuilder, Validators } from "@angular/forms";
import { faPlus } from "@fortawesome/free-solid-svg-icons";
import { TranslocoService } from "@jsverse/transloco";
import { Enum_ColonnaData_FiltroRicerca, Enum_TipoConfronto_FiltroRicerca } from "app/Model/TipiEnumerativi";
import { UtentiClient } from "app/Service/net-core6-api.service";
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { Subscription, skip } from "rxjs";

@Component({
    standalone: false,
    selector: 'app-filtri-temporali',
    templateUrl: './filtri-temporali.component.html',
    styleUrls: ['./filtri-temporali.component.scss']
})
export class FiltriTemporaliComponent implements OnInit, OnDestroy, OnChanges {

    @Input() formGroupArrayInput: FormArray;
    @Input() ListaEntita: any[];
    @Input() ListaValidita: any[];
    @Input() ListaTipoConfronto: any[];

    @Input() showSetDefaultFiltersButton: boolean;
    @Output() clickAddEsAttivi = new EventEmitter<boolean>();

    DDLObs = new Subscription();

    faPlus = faPlus;

    inizioCampagna;
    fineCampagna;
    showLastDateArray: boolean[] = new Array();
    showTodayOrAnnata: boolean[] = new Array();

    isVoid: boolean = false;

    constructor(
        private fb: FormBuilder,
        private ddlService: GiasDropDownTemplateService,
        private transloco: TranslocoService,
        private annataAgrariaService: UtentiClient
    ) {

        this.DDLObs.add(this.ddlService.currentDropDownValueObject.pipe(skip(1)).subscribe(async ddlElem => {
            let index = +(ddlElem.Id.split('-')).pop();
            switch (ddlElem.FormControlName) {
                case 'ColonnaData':
                    if (ddlElem.Value.id == Enum_ColonnaData_FiltroRicerca.IntervalloValidita) {
                        this.formGroupArrayInput.controls[index].get('TipoConfronto').patchValue(this.ListaTipoConfronto.filter(item => item.id == Enum_TipoConfronto_FiltroRicerca.CompresoFra)[0]);
                        this.showLastDateArray[index] = true;
                        this.showTodayOrAnnata[index] = false;
                    }
                break;

                case 'TipoConfronto':
                    if (ddlElem.Value.id == Enum_TipoConfronto_FiltroRicerca.CompresoFra) {
                        this.showLastDateArray[index] = true;
                        this.showTodayOrAnnata[index] = false;
                    } else {
                        this.showLastDateArray[index] = false;
                        this.showTodayOrAnnata[index] = true;
                    }

                    this.formGroupArrayInput.controls[index].get('ModalitaFiltroData').patchValue(false);
                    this.onCheckboxChange(index);
                break;
            }
        }));

        this.annataAgrariaService.utentiGetAnnataAgraria().subscribe(r => {
            let objRisp = JSON.parse(r.RispostaStringa);
            this.inizioCampagna = objRisp.inizioCampagna;
            this.fineCampagna = objRisp.fineCampagna;
        });
    }

    ngOnInit() {
        this.initializeFiltriTemporali();
    }

    ngOnChanges(changes: SimpleChanges): void {
        const change = changes['showSetDefaultFiltersButton'];
        if (change && !change.currentValue && change.previousValue)      //sono stati applicati i filtri, pensato per quando cambio tab e torno su filtri temporali
            this.initializeFiltriTemporali();
    }

    initializeFiltriTemporali() {

        this.showLastDateArray.splice(0, this.showLastDateArray.length);
        this.showTodayOrAnnata.splice(0, this.showTodayOrAnnata.length);

        this.formGroupArrayInput.value.forEach((element, index) => {

            if (element.TipoConfronto && element.TipoConfronto.id == Enum_TipoConfronto_FiltroRicerca.CompresoFra) {
                this.showLastDateArray.push(true);
                this.showTodayOrAnnata.push(false);
            } else {
                this.showLastDateArray.push(false);
                this.showTodayOrAnnata.push(true);
            }

            if (element.ModalitaFiltroData) {
                this.formGroupArrayInput.controls[index].get('Date').get('inizio').disable();
                this.formGroupArrayInput.controls[index].get('Date').get('fine').disable();
            }
        });
    }

    ngOnDestroy(): void {
        this.DDLObs.unsubscribe();
    }

    onClickAddFilters() {

        let lastItem = this.formGroupArrayInput.value.slice(-1)[0];

        if (lastItem) {
            if (!lastItem.Entita && !lastItem.ColonnaData && !lastItem.TipoConfronto) {
                this.formGroupArrayInput.controls[this.formGroupArrayInput.value.length - 1].get('Entita').setValidators([Validators.required]);
                this.formGroupArrayInput.controls[this.formGroupArrayInput.value.length - 1].get('Entita').markAsTouched();
                this.formGroupArrayInput.controls[this.formGroupArrayInput.value.length - 1].get('ColonnaData').setValidators([Validators.required]);
                this.formGroupArrayInput.controls[this.formGroupArrayInput.value.length - 1].get('ColonnaData').markAsTouched();
                this.formGroupArrayInput.controls[this.formGroupArrayInput.value.length - 1].get('TipoConfronto').setValidators([Validators.required]);
                this.formGroupArrayInput.controls[this.formGroupArrayInput.value.length - 1].get('TipoConfronto').markAsTouched();
                return;
            }
        }

        const filterGroup = this.fb.group({
            Entita: [null],
            ColonnaData: [null],
            TipoConfronto: [null],
            ModalitaFiltroData: [false],
            Date: this.fb.group({
                inizio: [null],
                fine: [null]
            })
        });

        this.formGroupArrayInput.push(filterGroup);

        this.showLastDateArray.push(false);
        this.showTodayOrAnnata.push(true);
    }

    onClickAddFiltersEserciziAttivi() {
        this.clickAddEsAttivi.emit(true);
    }

    removeFilter(index: number): void {
        this.formGroupArrayInput.removeAt(index);
        this.showLastDateArray.splice(index, 1);
        this.showTodayOrAnnata.splice(index, 1);
    }

    getListaTipoConfronto(ddlEl: GiasDropDownTemplateSComponent, index: number) {
        ddlEl.loading = true;

        ddlEl.listItems = this.ListaTipoConfronto;

        let valOfColonnaData = this.formGroupArrayInput.controls[index].get('ColonnaData').value;

        if (valOfColonnaData)
            if (valOfColonnaData.id == Enum_ColonnaData_FiltroRicerca.IntervalloValidita) {
                ddlEl.listItems = this.ListaTipoConfronto.filter(item => item.id == Enum_TipoConfronto_FiltroRicerca.CompresoFra);
                if (ddlEl.listItems.length > 0) {
                    ddlEl.value = ddlEl.listItems[0];
                    this.showLastDateArray[index] = true;
                    this.showTodayOrAnnata[index] = false;

                    this.formGroupArrayInput.controls[index].get('ModalitaFiltroData').patchValue(false);
                    this.onCheckboxChange(index);
                }
            }

        ddlEl.loading = false;
    }

    onCheckboxChange(index: number) {
        if (this.showTodayOrAnnata[index]) { //TODAY
            if (this.formGroupArrayInput.controls[index].get('ModalitaFiltroData').value) {
                let todayDate = new Date();
                this.formGroupArrayInput.controls[index].get('Date').get('inizio').patchValue(todayDate);
                this.formGroupArrayInput.controls[index].get('Date').get('inizio').disable();
            } else {
                this.formGroupArrayInput.controls[index].get('Date').get('inizio').patchValue(null);
                this.formGroupArrayInput.controls[index].get('Date').get('inizio').enable();
            }
        } else {
            if (this.formGroupArrayInput.controls[index].get('ModalitaFiltroData').value) { //ANNATA AGRARIA
                this.formGroupArrayInput.controls[index].get('Date').get('inizio').patchValue(new Date(this.inizioCampagna));
                this.formGroupArrayInput.controls[index].get('Date').get('fine').patchValue(new Date(this.fineCampagna));
                //rendo non editabili i calendari
                this.formGroupArrayInput.controls[index].get('Date').get('inizio').disable();
                this.formGroupArrayInput.controls[index].get('Date').get('fine').disable();
            } else {
                this.formGroupArrayInput.controls[index].get('Date').get('inizio').patchValue(null);
                this.formGroupArrayInput.controls[index].get('Date').get('fine').patchValue(null);
                this.formGroupArrayInput.controls[index].get('Date').get('inizio').enable();
                this.formGroupArrayInput.controls[index].get('Date').get('fine').enable();
            }
        }

    }

    getName(nomeDaTradurre: string, index: number): string {
        if (index == 0)
            return this.transloco.translate(nomeDaTradurre);
        else
            return '';
    }
}
