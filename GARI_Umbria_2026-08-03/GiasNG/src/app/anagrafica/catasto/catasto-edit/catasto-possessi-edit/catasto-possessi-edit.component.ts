import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroupDirective, Validators } from '@angular/forms';
import { PossessoParticella } from 'app/Model/anagrafiche/PossessoParticella';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';

import { Subscription } from 'rxjs';
import { PossessiParticelleService } from './catasto-possessi-edit-grid.service';
import { PossessiService, PossessoParticellaId } from './Possessi.service';

@Component({
  standalone: false,
  selector: 'app-catasto-possessi-edit',
  templateUrl: './catasto-possessi-edit.component.html',
  styleUrls: ['./catasto-possessi-edit.component.css'],
  providers: [
    ...generateGridProviders(PossessiParticelleService, CatastoPossessiEditComponent)
  ]
})
export class CatastoPossessiEditComponent implements OnInit, OnDestroy {
  @Input() formGroupName: string;
  form: FormArray;
  formValue: string;
  subscriptions: Subscription[] = new Array<Subscription>();

  constructor(private possessiService: PossessiService,
              private rootFormGroup: FormGroupDirective,
              private funzioniComuniService: FunzioniComuniService,
              private fb: FormBuilder) { }


  ngOnInit(): void {
    this.rootFormGroup.valueChanges.subscribe(
      ((val) => {
        this.possessiService.particellaVal = val;
      })
    )
    this.form = this.rootFormGroup.control.get(this.formGroupName) as FormArray;
    this.possessiService.setPossessoParticella(this.form.value);
    this.formValue = JSON.stringify(this.form.value);
    this.subscriptions.push(this.form.valueChanges.subscribe((el) => {
      this.formValue = JSON.stringify(el);
    }));

    this.subscriptions.push(this.possessiService.possessoParticellaSource.subscribe((newPossessoParticella: PossessoParticellaId[]) => {
      UtilityFunctions.clearFormArray(this.form);
      for (const possesso of newPossessoParticella) {
        const possessoForm = this.getRowPossessiParticella();
        possessoForm.patchValue(possesso);
        this.form.push(possessoForm);
      }
      this.form.patchValue(newPossessoParticella, { emitEvent: true, onlySelf: false });
      this.formValue = JSON.stringify(this.form.value);

    }));
  }


  ngOnDestroy(): void {
    for (const subs of this.subscriptions) {
      subs.unsubscribe();
    }
  }


  getRowPossessiParticella() {
    return this.fb.group({
      Area: [0],
      codice: [0],
      titolo_Di_Possesso: new FormControl({ codice: 0, descrizione: '' }),
      validita: this.fb.group({
        inizio: [AGRODATAINIZIO],
        fine: [AGRODATAFINE]
      }),
      codice_particella: ['']
    });
  }

}

