/* eslint-disable */
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { AsyncValidatorFn, FormGroup, FormGroupDirective } from '@angular/forms';
import { map, of } from 'rxjs';
import { AGRODATAFINE, AGRODATAINIZIO } from '../utils/models';

@Component({
  standalone: false,
  selector: 'gias-validita',
  templateUrl: './gias-validita.component.html',
  styleUrls: ['./gias-validita.component.css']
})
export class GiasValiditaComponent implements OnInit {
  /** Nome del controllo di riferimento all'interno del FormGroup padre.
   * Deve far riferimento a un FormGroup contenente i controlli `inizio` e `fine`.
   */
  @Input() formGroupName: string;

  /** Nome del controllo di riferimento per l'inizio dell'intervallo di validità
   *  all'interno del FormGroup padre. Usato solo se {@link formGroupName} non è valorizzato.
   */
  @Input() inizioFormControlName: string;

  /** Nome del controllo di riferimento per la fine dell'intervallo di validità
   *  all'interno del FormGroup padre. Usato solo se {@link formGroupName} non è valorizzato.
   */
  @Input() fineFormControlName: string;

  /**
   * Disattiva il css di default.
   *  @default `false`
   */
  @Input() enableAltRender: boolean = false;

  /**
   * If `true` shows the labels inline.
   * @default `false`
   */
  @Input() inlineLabels: Boolean = false;

  @Output('onBlur') onBlurEvent = new EventEmitter<string>();
  form: FormGroup;

  AGRODATA_INIZIO: Date = AGRODATAINIZIO;
  AGRODATA_FINE: Date = AGRODATAFINE;

  constructor(private rootFormGroup: FormGroupDirective) { }

  /** @returns il nome del FormControl che governa l'inizio dell'intervallo. */
  get inizio(): string {
    return !this.formGroupName ? this.inizioFormControlName : "inizio";
  }

  /** @returns il nome del FormControl che governa la fine dell'intervallo. */
  get fine(): string {
    return !this.formGroupName ? this.fineFormControlName : "fine";
  }

  get controlClass(): string {
    return this.enableAltRender ? "" : "col";
  }

  ngOnInit(): void {
    this.valorizeInnerForm();
  }

  handleBlur(field: string) {
    this.onBlurEvent.emit(field);
  }

  private valorizeInnerForm() {
    if (!this.formGroupName) {
      this.form = this.rootFormGroup.control;
    } else {
      this.form = this.rootFormGroup.control.get(this.formGroupName) as FormGroup;
    }
  }
}

export function validityValidator(fcStartName: string = 'inizio', fcEndName: string = 'fine'): AsyncValidatorFn {
  return (group: FormGroup) => {
    const start: Date = group.get(fcStartName).value;
    const end: Date = group.get(fcEndName).value;

    return of(start > end).pipe(
      map(r => {
        return r ?  { 'validity': true } : null;
      })
    );
  };
}
