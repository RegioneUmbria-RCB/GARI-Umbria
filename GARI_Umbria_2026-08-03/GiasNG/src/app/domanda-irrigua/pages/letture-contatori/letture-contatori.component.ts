import { Component, OnInit } from '@angular/core';
import {FormControl, FormGroup} from "@angular/forms";
import { IntervalloTemporale } from 'app/Service/net-core6-api.service';
import {Subject} from 'rxjs';
import { DateUtility } from 'gias-ui-kit';
import { IntervalloTemporaleForm } from 'gias-ui-kit';

export class LettureContatoriFilters {
  constructor(
    public dateInterval: IntervalloTemporale
  ) { }
}

interface LettureContatoriFiltersForm {
  dateInterval: FormGroup<IntervalloTemporaleForm>;
}

@Component({
  standalone: false,
  selector: 'app-letture-contatori',
  templateUrl: './letture-contatori.component.html',
  styleUrls: ['./letture-contatori.component.css']
})
export class LettureContatoriComponent implements OnInit {
  protected filters = new FormGroup<LettureContatoriFiltersForm>({
    dateInterval: new FormGroup<IntervalloTemporaleForm>(({
      inizio: new FormControl(new Date(DateUtility.getCurrentYear(), DateUtility.months.JANUARY, 1)),
      fine: new FormControl(new Date(DateUtility.getCurrentYear(), DateUtility.months.DECEMBER, 31)),
    }))
  });
  protected searchFilters$ = new Subject<LettureContatoriFilters>();

  constructor() { }

  protected get sideFilters(): FormGroup<IntervalloTemporaleForm> {
    return this.filters.controls.dateInterval;
  }

  ngOnInit(): void {  }

  public applyFilters() {
    this.searchFilters$.next((this.filters.value as LettureContatoriFilters));
  }

}
