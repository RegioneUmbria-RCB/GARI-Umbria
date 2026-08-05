import { Component, Input, OnInit, forwardRef } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { AnniHttpService } from '../../anni-grid-config.service';
import { ControlContainer, ControlValueAccessor, FormGroup, FormGroupDirective, NG_VALUE_ACCESSOR } from '@angular/forms';
import { KendoGridColumn } from 'gias-kendo-grid';
import { ValutazioniService } from 'app/valutazioni/valutazioni/service/valutazioni.service';
import { TranslocoService } from '@jsverse/transloco';
import { GridPublicService } from 'gias-kendo-grid';

@Component({
  standalone: false,
  selector: 'app-anno-valutazioni-edit',
  templateUrl: './anno-valutazioni-edit.component.html',
  styleUrls: ['./anno-valutazioni-edit.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AnnoValutazioniEditComponent),
      multi: true
    },
    ...generateGridProviders(AnniHttpService, AnnoValutazioniEditComponent)
  ],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class AnnoValutazioniEditComponent implements OnInit, ControlValueAccessor {

  @Input() anniForm: FormGroup;
  @Input() anni: Array<any>;
  @Input() lista_anni: Array<any>;
  @Input() text_field: string;
  @Input() value_field: string;
  @Input() dateControl: boolean;
  @Input() edit: boolean;

  anni_columns: Array<KendoGridColumn>;

  constructor(private valutazioniService: ValutazioniService,
    private gridPublicService: GridPublicService,
    private translocoService: TranslocoService
  ) {
    this.valutazioniService.gridPublicService = this.gridPublicService;
  }

  public onTouched: () => void = () => {
    let a = 0; //Commento per funzione vuota SonarQube
  };

  ngOnInit(): void {
    this.anni_columns = new Array<KendoGridColumn>(
      new KendoGridColumn({ field: 'name', title: this.translocoService.translate('Anno') }),
      new KendoGridColumn({ field: 'tipo_', title: this.translocoService.translate('Tipo') })
    );
  }

  writeValue(val: any): void {
    if (val) {
      this.anniForm.setValue(val, { emitEvent: false });
    }
  }

  registerOnChange(fn: any): void {
    this.anniForm.valueChanges.subscribe(fn);
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    if (isDisabled) {
      this.anniForm.disable();
    } else {
      this.anniForm.enable();
    }
  }

  updateCodice(event: any) {
    let a = 0; //Commento per funzione vuota SonarQube
  }

  cancelCodice(event: any) {
    let a = 0; //Commento per funzione vuota SonarQube
  }

  saveCodice(event: any) {
    let a = 0; //Commento per funzione vuota SonarQube
  }

  removeCodice(event: any) {
    let a = 0; //Commento per funzione vuota SonarQube
  }

  addCodice(event: any) {
    let a = 0; //Commento per funzione vuota SonarQube
  }

  pageChangeEvent(event: any) {
    let a = 0; //Commento per funzione vuota SonarQube
  }

}
