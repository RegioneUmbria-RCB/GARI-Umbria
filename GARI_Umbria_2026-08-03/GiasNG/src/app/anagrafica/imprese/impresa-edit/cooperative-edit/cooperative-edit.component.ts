import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormGroup, FormGroupDirective } from '@angular/forms';
import { GRID_HTTP_TOKEN, KendoServerResult } from 'gias-kendo-grid';
import { AbstractGridConfigService } from 'gias-kendo-grid';
import { generateGridProviders } from 'gias-kendo-grid';
import { Subject, takeUntil } from 'rxjs';
import { Comune, Provincia } from 'app/Model/MetaschemaModel';
import { CodiciNazioniISO3166 } from 'app/Model/metaschema/CodiciNazioniISO3166';
import { CooperativeConfigService } from './cooperative-edit-config.service';

@Component({
  standalone: false,
  selector: 'cooperative-edit',
  templateUrl: './cooperative-edit.component.html',
  styleUrls: ['./cooperative-edit.component.scss'],
  providers: [...generateGridProviders(CooperativeConfigService, CooperativeComponent)]
})
export class CooperativeComponent implements OnInit, OnDestroy {

  @Input() isFormDisabled: boolean;
  @Input() padri: any;
  signal: Subject<void> = new Subject();

  cooperativeFormGroup: FormGroup;
  istatComune: FormGroup;
  indirizzo: FormGroup;

  provincie: Provincia[];
  comune: Comune[];
  stati: CodiciNazioniISO3166[];

  get indirizzi() {
    return this.cooperativeFormGroup.controls['indirizzi'] as FormArray;
  }

  constructor(
    private rootFormGroup: FormGroupDirective,
    @Inject(GRID_HTTP_TOKEN) public conf: AbstractGridConfigService<KendoServerResult>,
    @Inject(GRID_HTTP_TOKEN) private cooperativeConfigService: CooperativeConfigService
  ) {
  }

  ngOnInit(): void {
    this.cooperativeFormGroup = this.rootFormGroup.form as FormGroup;

    this.indirizzo = (this.cooperativeFormGroup.get('indirizzi') as FormArray).at(0).get('indirizzo') as FormGroup;
    this.istatComune = this.indirizzo.get('istatComune') as FormGroup;

    this.indirizzo.valueChanges.pipe(takeUntil(this.signal)).subscribe();

    this.cooperativeConfigService.setPadri(this.padri);
  }

  ngOnDestroy(): void {
    this.signal.next();
    this.signal.complete();
  }
}
