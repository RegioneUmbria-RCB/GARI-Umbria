import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { generateGridProviders } from 'gias-kendo-grid';
import { OperationEditService } from '../operation-edit.service';
import { GridAddMacchineService } from './grid-add-macchine.service';
import { BehaviorSubject, Subject, takeUntil, tap } from 'rxjs';
import { GridPublicService } from 'gias-kendo-grid';

@Component({
  standalone: false,
  selector: 'app-grid-add-macchine',
  templateUrl: './grid-add-macchine.component.html',
  styleUrls: ['./grid-add-macchine.component.css'],
  providers: [
    ...generateGridProviders(GridAddMacchineService, GridAddMacchineComponent),
]
})
export class GridAddMacchineComponent implements OnInit {
  public signal$ = new Subject<void>();

  constructor(private fb: FormBuilder,
      private datashare: OperationEditService,
      private gridPublicService: GridPublicService
    ) { }

  public get settings(): FormGroup {
    return this.datashare.editForm.get('macchine') as FormGroup;
  };

  ngOnInit(): void {

    this.datashare.editForm.get('macchine').get('mostraSoloAziendali').valueChanges.pipe(
      takeUntil(this.signal$),
      tap(() => this.gridPublicService.refresh(true))
    ).subscribe();

  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }


}
