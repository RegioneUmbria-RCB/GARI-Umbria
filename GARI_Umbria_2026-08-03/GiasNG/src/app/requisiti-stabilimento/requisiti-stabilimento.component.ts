import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ObjParametriAgendaService } from '../Service/obj-parametri-agenda.service';
import { ReadRequisitiStabilimento, } from '../Service/RequisitiStabilimento/requisiti-stabilimento.service';
import { IntervalloTemporale } from '../Model/anagrafiche/IntervalloTemporale';
import { RequisitiStabilimentoService } from './requisiti-stabilimento.service';
import { Subject, debounceTime, takeUntil, tap } from 'rxjs';
import { faIndustry, faLeaf } from '@fortawesome/free-solid-svg-icons';
import {Router} from "@angular/router";
import { ObjParametriAgenda } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-requisiti-stabilimento',
  templateUrl: './requisiti-stabilimento.component.html',
  styleUrls: ['./requisiti-stabilimento.scss'],
  providers: [RequisitiStabilimentoService]
})
export class RequisitiStabilimentoComponent implements OnInit, OnDestroy {
  filtriForm: FormGroup;
  objParametriAgenda: ObjParametriAgenda;
  isValid = false;

  protected readonly faIndustry = faIndustry;
  protected readonly faLeaf = faLeaf;
  private signal$ = new Subject<void>();

  constructor(
    private objParametriAgendaService: ObjParametriAgendaService,
    private router: Router,
    private requisitiStabilimentoService: RequisitiStabilimentoService,
  ) { }

  ngOnInit(): void {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.filtriForm = this.requisitiStabilimentoService.getFiltriForm();
    let year = new Date().getFullYear();
    this.filtriForm.get('validita').get('inizio').setValue(new Date(year, 0, 1));
    this.filtriForm.get('validita').get('fine').setValue(new Date(year, 11, 31));
    this.filtriForm
      .valueChanges
      .pipe(
        takeUntil(this.signal$),
        debounceTime(200),
        tap(() => {
          const validita = this.filtriForm.get('validita').getRawValue();
          const matCod = this.filtriForm.get('prodotto').getRawValue().codice;
          const risUm = this.filtriForm.get('cooperativa').getRawValue().Cod_RisUm;

          this.isValid = (validita.inizio != null && validita.fine != null)
            && (matCod != null && matCod != 0)
            && (risUm != null && risUm != 0);
        })
      )
      .subscribe()

    this.requisitiStabilimentoService.reloadGrid$.pipe(takeUntil(this.signal$)).subscribe(() => this.getGridData());
  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }

  getGridData(): void {
    const validita = this.filtriForm.get('validita').getRawValue();
    const matCod = this.filtriForm.get('prodotto').getRawValue().codice;
    const idBudget = this.filtriForm.get('budget').getRawValue().Id_Budget;
    const risUm = this.filtriForm.get('cooperativa').getRawValue().Cod_RisUm;

    const params: ReadRequisitiStabilimento = {
      validita: new IntervalloTemporale(validita.inizio, validita.fine),
      matCod: matCod,
      idBudget: idBudget,
      risUm: risUm,
      mostraAssegnazioni: true
    };

    this.requisitiStabilimentoService.nextRequisitiPayload(params);
  }

  goToDettaglioAzienda() {
    this.router.navigate(['requisiti-stabilimento/VisualizzaDettagli/piano-colturale']);
  }

  public mouseMoved(event: WheelEvent) {
    const direction = event.deltaY;
    if (direction <= 0) {
      this.moveToRight();
    } else {
      this.moveToLeft();
    }
  }

  public moveToLeft() {
    // versione di codice con
    //const tabs = document.getElementById("anag-top-bar-2");
    //this.anagTopTar2.nativeElement.scrollLeft -= 120;
  }

  public moveToRight() {
    // const tabs = document.getElementById("anag-top-bar-2");
    // tabs.scrollLeft += 100;
    //this.anagTopTar2.nativeElement.scrollLeft += 120;
  }
}
