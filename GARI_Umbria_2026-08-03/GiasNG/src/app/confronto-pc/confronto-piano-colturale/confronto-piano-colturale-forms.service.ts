import {Injectable} from '@angular/core';
import {AjaxAgronicaNetCore6ApiService} from '../../Service/ajax-agronica-net-core6-api.service';
import {FormBuilder, FormControl, FormGroup} from '@angular/forms';
import {AGRODATAFINE, AGRODATAINIZIO} from '../../Model/CostantiPersonalizzate';

@Injectable()
export class ConfrontoPianoColturaleFormsService {
  constructor(
    private fb: FormBuilder
  ) { }

  public getConfrontoPCForm(): FormGroup {
    return this.fb.group({
      considerCatasto: true,
      considerVarieta: false,
      validita:  this.fb.group({
        inizio: AGRODATAINIZIO,
        fine: AGRODATAFINE
      })
    })
  }
}
