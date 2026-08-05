import { Component, Inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { generateGridProviders, GRID_HTTP_TOKEN } from 'gias-kendo-grid';
import { GridRilievoPioggeService } from './grid-rilievo-piogge.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { faInfoCircle } from '@fortawesome/free-solid-svg-icons';

@Component({
  standalone: false,
  selector: 'app-rilievo-piogge',
  templateUrl: './rilievo-piogge.component.html',
  styleUrl: './rilievo-piogge.component.css',
  providers: [
    ...generateGridProviders(GridRilievoPioggeService, RilievoPioggeComponent)
  ]
})
export class RilievoPioggeComponent {

  faInfoCircle = faInfoCircle;
  showGridRilievoPiogge: boolean = false;
  inputForm: FormGroup;
  constructor(private fb: FormBuilder,
    @Inject(GRID_HTTP_TOKEN) private gridRilievoPioggeService: GridRilievoPioggeService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private giasMessageService: GiasMessageService) {

    this.inputForm = this.fb.group({
      DataInizio: new FormControl<Date>(null, [Validators.required]),
      DataFine: new FormControl<Date>(null, [Validators.required])
    });
  }

  onSubmit(){
    let dataInizio = this.inputForm.get('DataInizio').value;
    let dataFine = this.inputForm.get('DataFine').value;
    if (dataInizio && dataFine){
      if (dataInizio > dataFine){
        this.giasMessageService.errorMessage("DataInizioNonDeveEssereMaggioreDiDataFine", false, true);
        return;
      }
    } else {
      this.giasMessageService.errorMessage("NecessarioCompilareTuttiICampi", false, true);
      return;
    }
    this.gridRilievoPioggeService.DataDa = dataInizio;
    this.gridRilievoPioggeService.DataA = dataFine;
    this.gridRilievoPioggeService.Piva = this.objParametriAgendaService.getObjParamValue().Piva;

    if (!this.showGridRilievoPiogge){
      this.showGridRilievoPiogge = true;
    } else {
      this.gridRilievoPioggeService.refreshGrid();
    }
  }
}
