import { Component, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { FormGroup, FormControl, FormsModule, ReactiveFormsModule, } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { CustomComponent } from 'gias-kendo-grid';
import { KENDO_INPUTS } from "@progress/kendo-angular-inputs";
import { KENDO_BUTTONS } from "@progress/kendo-angular-buttons";
import { KENDO_ICONS } from "@progress/kendo-angular-icons";
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { map, takeUntil, Subject, debounce, interval } from 'rxjs';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { enum_Azienda_Persona } from 'app/profilazione/models/utente.model';

@Component({
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, KENDO_INPUTS, KENDO_BUTTONS, KENDO_ICONS],
  selector: 'app-cf-input-template',
  templateUrl: './cf-input-template.component.html',
  styleUrl: './cf-input-template.component.css'
})
/**
 * Component for handling a customizable input field, specifically for Codice Fiscale (CF) input.
 * 
 * This component allows for the display and editing of a field within a reactive form,
 * and provides a method to generate a random Codice Fiscale (CF) value via an API call.
 * 
 * @property input - The object containing the field to be edited.
 * @property edit - Boolean flag to enable or disable editing.
 * @property field - The name of the field in the input object and form group.
 * @property formGroup - The reactive form group containing the field.
 * 
 * @method generateCF - Calls the API to generate a random CF and updates both the input object and form group.
 * 
 * @remarks
 * Uses `AjaxAgronicaAPIService` to interact with the backend API.
 * Displays a sparkles icon for the generate CF action.
 * 
 * @implements OnInit, CustomComponent
 */
export class CfInputTemplateComponent implements OnInit, OnDestroy, CustomComponent {
  @Input() input: any;
  @Input() edit: boolean;
  @Input() field: string;
  @Input() formGroup: FormGroup;
  @ViewChild('textBox') textBox;

  private _signal$ = new Subject<void>();

  protected canAutogenCF: boolean;
  protected innerForm = new FormGroup({
    CodFisc: new FormControl('')
  });

  constructor(
    private apiService: AjaxAgronicaAPIService,
    private permissions: PermessiUtenteService
  ) {
    this.canAutogenCF = this.permissions.canWritePermesso(enum_Security_Attivita.UtentiGenerazioneCF);
  }

  private get isCompany(): boolean {
    return this.formGroup.value["Flag_Azienda_Persona"] == enum_Azienda_Persona.Impresa;
  }

  ngOnInit(): void {
    if (!!this.input[this.field]) {
      this.innerForm.get('CodFisc').setValue(this.input[this.field]);
    }

    // this.innerForm.get('CodFisc').valueChanges.pipe(
    //   takeUntil(this._signal$),
    //   debounce(() => interval(200))
    // ).subscribe(x => {
    //   //this.formGroup.get(this.field).setValue(x);
    //   // this.textBox.input.nativeElement.focus();
    //   // this.textBox.input.nativeElement.autofocus = true;
    // });
  }

  inputBlur() {
    const cf = this.innerForm.get('CodFisc').value;
    this.formGroup.get(this.field).setValue(cf);
    this.input[this.field] = cf;
  }

  ngOnDestroy(): void {
    this._signal$.next();
    this._signal$.complete();
  }

  generateCF() {
    this.apiService.ajaxAPIGet('Profilazione/CreateRandomCF', '')
      .pipe(
        map(R => R.RispostaOK ? R.RispostaStringa : ''),
        map((cf: string) => this.isCompany ? cf.substring(5) : cf)
      ).subscribe((cf: string) => {
        this.innerForm.get('CodFisc').setValue(cf);
        this.formGroup.get(this.field).setValue(cf);
        this.input[this.field] = cf;
      });
  }
}
