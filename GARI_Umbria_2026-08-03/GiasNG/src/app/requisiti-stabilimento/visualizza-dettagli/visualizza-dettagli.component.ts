import { Component, OnDestroy, OnInit } from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';
import { Subject } from 'rxjs';
import {AGRODATAFINE, AGRODATAINIZIO} from "../../Model/CostantiPersonalizzate";
import { ReadDettaglioAziendale } from "../../Service/RequisitiStabilimento/requisiti-stabilimento.service";
import {IntervalloTemporale} from "../../Model/anagrafiche/IntervalloTemporale";
import {VisualizzaDettagliService} from "./visualizza-dettagli.service";
import {generateGridProviders} from 'gias-kendo-grid';
import {VisualizzaDettagliGridConfigurationService} from "./v-d-piano-colturale/visualizza-dettagli-grid-configuration.service";
import { faIndustry, faLeaf } from '@fortawesome/free-solid-svg-icons';

@Component({
  standalone: false,
  selector: 'app-visualizza-dettagli',
  templateUrl: './visualizza-dettagli.component.html',
  styleUrls: ['visualizza-dettagli.scss'],
  providers: [VisualizzaDettagliService]
})
export class VisualizzaDettagliComponent implements OnInit, OnDestroy {
  filtriForm: FormGroup;
  buttonClicked = false;
  protected readonly faIndustry = faIndustry;
  protected readonly faLeaf = faLeaf;
  private signal$ = new Subject<void>();

  constructor(private fb: FormBuilder,
              private visualizzaDettagliService: VisualizzaDettagliService
  ) { }

  public getFiltriForm(): FormGroup {
    return this.fb.group({
      validita: this.fb.group({
        inizio: [AGRODATAINIZIO],
        fine: [AGRODATAFINE]
      })
    });
  }

  ngOnInit(): void {
    this.filtriForm = this.getFiltriForm();
    let year = new Date().getFullYear();
    this.filtriForm.get('validita').get('inizio').setValue(new Date(year, 0, 1));
    this.filtriForm.get('validita').get('fine').setValue(new Date(year, 11, 31));

    this.getGridData()
  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }

  getGridData(): void {
    const validita = this.filtriForm.get('validita').getRawValue();

    const params: ReadDettaglioAziendale = {
      validita: new IntervalloTemporale(validita.inizio, validita.fine),
      matCod: 0,
      idBudget: 0,
      piva: ''
    };
    this.visualizzaDettagliService.nextRequisitiPayload(params);
    if (this.buttonClicked){
      this.visualizzaDettagliService.nextReloadGrid()
    } else {
      this.buttonClicked = true;
    }
  }

}
