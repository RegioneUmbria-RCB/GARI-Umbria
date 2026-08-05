import { KeyValue } from "@angular/common";
import { Component, OnInit, OnDestroy, Input } from "@angular/core";
import { FormGroup, FormControl } from "@angular/forms";
import { Contribute, ContributeType, LinkedContribute } from "app/Model/metaschema/Contribute";
import { ProjectXContributeService } from "app/Service/Anagrafica/project-x-contribute.service";
import { ContributeService } from "app/Service/Metaschema/contribute.service";
import { ImpiantiServiceProvider } from "app/Service/ServiceFactory/impianti.factory.provider";
import { UtilityFunctions } from "app/Utility/UtilityFunctions";
import { KendoGridRow, GiasMessageService } from "gias-kendo-grid";
import { GiasMultiSelectTemplateService, GiasDropDownTemplateSComponent, GiiasMultiselectTemplateSComponent } from "gias-ui-kit";
import { ReplaySubject, Subject, BehaviorSubject, takeUntil, lastValueFrom, map, of, take } from "rxjs";
import { AgriculturalItem } from "../../agricultural-item.model";
import { AgriculturalExerciceContributeLinkService } from "./agricultural-exercice-contribute-link.service";
import { Esercizio } from "app/Model/anagrafiche/Esercizio";

class MultiselectContributeFG {
  contributes: FormControl<Contribute[]> = new FormControl<Contribute[]>(
    [],
    [(c) => !!c.value ? null : { contribute: 'required' }]
  );

  constructor() { }
}

@Component({
  standalone: false,
  selector: 'app-agricultural-exercise-contribute-link',
  templateUrl: './agricultural-exercise-contribute-link.component.html',
  styleUrls: ['./agricultural-exercise-contribute-link.component.css'],
  providers: [AgriculturalExerciceContributeLinkService, GiasMultiSelectTemplateService, ImpiantiServiceProvider]
})
export class AgriculturalExerciseContributeLinkComponent implements OnInit, OnDestroy {
  @Input() protected listViewHeight$: ReplaySubject<number> = new ReplaySubject<number>(1);
  @Input() private checkedRows$: Subject<KendoGridRow[]> = new Subject<KendoGridRow[]>();
  protected rows$: BehaviorSubject<AgriculturalItem[]> = new BehaviorSubject<AgriculturalItem[]>([]);
  protected removeDataItem$: Subject<{ index: number, deleteCount: number; }> = new Subject<{ index: number; deleteCount: number; }>();
  protected resetStates$: Subject<boolean> = new Subject();
  protected loading$: ReplaySubject<boolean> = this.exerciceContributeLinkService.loading$;

  protected contributeDdlForm: FormGroup;

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private giasMessageService: GiasMessageService,
    private projectXContributeService: ProjectXContributeService,
    private exerciceContributeLinkService: AgriculturalExerciceContributeLinkService,
    private contributeService: ContributeService
  ) {
    this.checkedRows$.pipe(takeUntil(this.destroy$)).subscribe(crs => {
      this.rows$.next([]);
      this.exerciceContributeLinkService.checkedRows = crs;
      this.exerciceContributeLinkService.setListViewRows();
    });

    this.exerciceContributeLinkService.listViewRows$.pipe(takeUntil(this.destroy$)).subscribe(rr => {
      this.rows$.next([...this.rows$.value, ...rr]);
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  ngOnInit(): void {
    this.contributeDdlForm = new FormGroup<MultiselectContributeFG>(new MultiselectContributeFG());

    this.contributeDdlForm.get('contributes').valueChanges.pipe(takeUntil(this.destroy$)).subscribe(v => {
      this.onContributeChange(v);
    });
  }

  openDdl(ddl: GiasDropDownTemplateSComponent | GiiasMultiselectTemplateSComponent): void {
    switch (ddl.giasFormControlName) {
      case 'contributes':
        UtilityFunctions.loadDropDownItems(
          <GiasDropDownTemplateSComponent>ddl,
          lastValueFrom(this.contributeService.read(new Contribute(0, ContributeType.ACA)).pipe(map(r => r.RispostaStringa)))
        );
        break;
      default:
        UtilityFunctions.loadDropDownItems(<GiasDropDownTemplateSComponent>ddl, lastValueFrom(of([]).pipe(take(1))));
        break;
    }
  }

  disableAddBtn(): boolean {
    return this.rows$.value.some(r => r.error);
  }

  submit(): void {
    let exs: Esercizio[] = this.rows$.value.map(r => r.toEsercizio());
    exs.forEach(e => {
      e.acaContributes = (this.contributeDdlForm.get('contributes').value as Contribute[]).map(c => {
        let lc: LinkedContribute<KeyValue<number, string>> = new LinkedContribute<KeyValue<number, string>>(
          c.code,
          c.type,
          { key: e.codice, value: e.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva }
        );
        lc.description = c.description;
        lc.validity = c.validity;

        return lc;
      });
    });

    this.projectXContributeService.addContributes(exs).pipe(take(1)).subscribe(r => {
      if (r) {
        this.giasMessageService.successMessage('SalvataggioAvvenutoConSuccesso', false, true);
      } else {
        this.giasMessageService.errorMessage('ErroreDuranteIlSalvataggio', false, true);
      }
    });
  }

  private onContributeChange(contributes: Contribute[]): void {
    this.resetStates$.next(false);
    this.exerciceContributeLinkService.compareExerciceContributeValidity(this.rows$.value, contributes);
  }
}
