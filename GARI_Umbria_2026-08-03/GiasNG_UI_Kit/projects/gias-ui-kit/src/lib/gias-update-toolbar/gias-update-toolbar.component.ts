/* eslint-disable */
import {
  Component,
  DestroyRef,
  EventEmitter,
  inject,
  Inject,
  Input,
  OnDestroy,
  OnInit,
  Output,
  SkipSelf
} from '@angular/core';
import { ControlContainer, FormGroup } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { UpdateToolbarService } from './gias-update-toolbar.service';
import { AppBarPosition, AppBarPositionMode } from "@progress/kendo-angular-navigation";
import { Enum_DBTypeOperation } from '../utils/models';
import { GIAS_PARAMETRI_AGENDA_TOKEN, IObjParametriAgendaService } from '../utils/obj-parametri-agenda.service';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';

@Component({
  standalone: false,
  selector: 'gias-update-toolbar',
  templateUrl: './gias-update-toolbar.component.html',
  styleUrls: ['./gias-update-toolbar.component.css'],
  providers: [UpdateToolbarService],
  viewProviders: [{
    provide: ControlContainer,
    useFactory: (container: ControlContainer) => container,
    deps: [[new SkipSelf(), ControlContainer]],
  }]
})
export class GiasUpdateToolbarComponent implements OnInit, OnDestroy {
  @Input() TipoOperazioneDB: number;
  @Input() disabled: boolean;
  @Input() submit: boolean;
  @Input() salvaNuovo?: boolean = false;
  @Input() salva?: boolean = true;
  @Input() formGroup?: FormGroup;
  @Input() trackChanges?: boolean;
  @Input() showCompanyName?: boolean;
  @Input() openInIFrame?: boolean;
  @Input() position: AppBarPosition = "top";
  @Input() positionMode: AppBarPositionMode = "sticky";
  @Output() clickEvent = new EventEmitter<string>();
  @Output() clickSalvaNuovoEvent = new EventEmitter<string>();

  public signal$: Subject<void> = new Subject();

  UndoEnable: boolean;
  RedoEnable: boolean;

  protected companyRagSoc: string;
  protected companyDescription: string;

  private _destroyRef = inject(DestroyRef);

  constructor(
    private updateToolbarService: UpdateToolbarService,
    @Inject(GIAS_PARAMETRI_AGENDA_TOKEN) private objParametriAgendaService: IObjParametriAgendaService
  ) { }

  ngOnInit(): void {
    if (this.trackChanges && this.formGroup != null) {
      this.updateToolbarService.initializeChanges(this.formGroup);
      this.updateToolbarService.currentAvailableChangeOperation.subscribe((el) => {
        this.UndoEnable = el.canUndo;
        this.RedoEnable = el.canRedo;
      });
    }

    this.objParametriAgendaService.currentObjParametriAgenda.pipe(
      takeUntil(this.signal$)
    ).subscribe(val => {
      if (val.TipoOperazioneDB == Enum_DBTypeOperation.Read) {
        this.disabled = true;
      } else {
        this.disabled = false;
      }
    })

    this.objParametriAgendaService.currentObjParametriAgenda.pipe(takeUntilDestroyed(this._destroyRef)).subscribe((objSelected) => {
      this.companyDescription = objSelected?.Cod_Contatto ?? '';
      this.companyRagSoc = objSelected?.RagSoc ?? '';
    });
  }

  SalvaEvent() {
    this.clickEvent.emit('Salva');
  }

  SalvaNuovoEvent() {
    this.clickSalvaNuovoEvent.emit('SalvaNuovo');
  }

  undo() {
    this.updateToolbarService.undo();
  }

  redo() {
    this.updateToolbarService.redo();
  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }
}
