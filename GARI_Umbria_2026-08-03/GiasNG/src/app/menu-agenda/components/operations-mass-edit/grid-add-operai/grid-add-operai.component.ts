import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { generateGridProviders } from 'gias-kendo-grid';
import { OperationEditService } from '../operation-edit.service';
import { GridAddOperaiService } from './grid-add-operai.service';
import { Subject, takeUntil, tap } from 'rxjs';
import { GridPublicService } from 'gias-kendo-grid';

@Component({
  standalone: false,
  selector: 'app-grid-add-operai',
  templateUrl: './grid-add-operai.component.html',
  styleUrls: ['./grid-add-operai.component.css'],
  providers: [
    ...generateGridProviders(GridAddOperaiService, GridAddOperaiComponent),
]
})
export class GridAddOperaiComponent implements OnInit, OnDestroy {
  public signal$ = new Subject<void>();



  constructor(private fb: FormBuilder,
    private datashare: OperationEditService,
    private gridPublicService: GridPublicService
    ) { }

  public get settings(): FormGroup {
    return this.datashare.editForm.get('manodopera') as FormGroup;
  };

  ngOnInit(): void {

    this.datashare.editForm.get('manodopera').get('mostraSoloAziendali').valueChanges.pipe(
      takeUntil(this.signal$),
      tap(() => this.gridPublicService.refresh(true))
    ).subscribe();

  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }


}
