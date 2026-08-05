import {Component, DestroyRef, Inject, inject, OnInit} from '@angular/core';
import {GiasKendoGridModule} from 'gias-kendo-grid';
import {TranslocoPipe} from '@jsverse/transloco';
import {UikitModule} from '../../../../../Utility/uikit.module';
import {FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {ButtonComponent} from '@progress/kendo-angular-buttons';
import {GiasDropDownTemplateSComponent, GiasUikitModule, GiiasMultiselectTemplateSComponent} from 'gias-ui-kit';
import {
  AgriculturalExerciseConstrainService
} from './agricultural-exercise-constrain.service';
import {SpecialEditBaseComponent} from '../../special-edit-base.component';
import {AgriculturalItem} from '../../agricultural-item.model';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {UtilityFunctions} from '../../../../../Utility/UtilityFunctions';
import {catchError, lastValueFrom, of, take} from 'rxjs';
import {NgIf} from '@angular/common';
import {IMPIANTI_SERVICE_TOKEN, ImpiantiFactoryService} from '../../../../../Service/ServiceFactory/impianti.factory.service';
import {VincoliService} from '../../../../../Service/DPI/vincoli.service';
import {IntervalloTemporale} from '../../../../../Model/anagrafiche/IntervalloTemporale';
import {GiasMessageService} from '../../../../../Service/gias-message.service';
import {Appezzamento} from '../../../../../Model/anagrafiche/Appezzamento';
import {Esercizio} from '../../../../../Model/anagrafiche/Esercizio';
import {Vincolo} from '../../../../../Model/metaschema/Vincoli';
import {Impianto} from '../../../../../Model/anagrafiche/Impianto';
import {Varieta} from '../../../../../Model/metaschema/utilizzi/Varieta';
import {ApportoMacroelementi} from '../../../../../Model/metaschema/ApportoMacroelementi';
import {MasterService} from '../../../../../Service/master.service';

class DdlConstrainFG {
  useOfLand: FormControl<object> = new FormControl<object>(
      {},
      [(c) => !!c.value['codice'] ? null : { constrain: 'required' }]
  );

  constrain: FormControl<object> = new FormControl<object>(
      {},
      [(c) => !!c.value['codice'] ? null : { constrain: 'required' }]
  );

  constructor() { }
}

@Component({
  standalone: true,
  selector: 'app-agricultural-exercise-constrain',
  imports: [
    TranslocoPipe,
    UikitModule,
    ReactiveFormsModule,
    ButtonComponent,
    GiasUikitModule,
    GiasKendoGridModule,
    NgIf
  ],
  templateUrl: './agricultural-exercise-constrain.component.html',
  styleUrl: './agricultural-exercise-constrain.component.css',
  providers: [AgriculturalExerciseConstrainService]
})
export class AgriculturalExerciseConstrainComponent extends SpecialEditBaseComponent<AgriculturalItem> implements OnInit {
  protected constrainDdlForm: FormGroup;
  private _destroyRef: DestroyRef = inject(DestroyRef);
  private _selectedConstrain: Vincolo;
  private _isSaving: boolean = false;
  private readonly BIO_CONSTRAIN_CODE = '4';
  private readonly BIO_USE_OF_LAND_CODE = '3';
  private readonly CONVERTING_USE_OF_LAND_CODE = '2';
  private readonly _bioConstrain: Vincolo = {
    disciplinare: {
      disciplinarePubblicoPrivato: 1,
      regolamentoConcimazione: {
        tipo: 0,
        codice: 0,
        descrizione: 'Nessuno'
      },
      raggruppamentiColturaliDPI: null,
      gruppoFinalita: { codice: 0, descrizione: '', specieCod: 0 },
      flagProtetto: 0,
      idTr: 3,
      validita: new IntervalloTemporale(),
      codice: '0',
      descrizione: ''
    },
    regolamento: {
      codice: 4,
      descrizione: 'Bio'
    },
    codice: this.BIO_CONSTRAIN_CODE,
    descrizione: 'Bio'
  };

  constructor(
      @Inject(IMPIANTI_SERVICE_TOKEN) private plotsService: ImpiantiFactoryService,
      private agriculturalExerciseConstrainService: AgriculturalExerciseConstrainService,
      private constrainsService: VincoliService,
      private messageService: GiasMessageService,
      private masterService: MasterService
  ) {
    super();
    this.loading$ = this.agriculturalExerciseConstrainService.loading$;
  }

  ngOnInit(): void {
    this.listViewHeight$.next(0.45 * window.innerHeight);
    this.checkedRows$.pipe(takeUntilDestroyed(this._destroyRef)).subscribe(crs => {
      this.rows$.next([]);
      this.agriculturalExerciseConstrainService.checkedRows = crs;
      this.agriculturalExerciseConstrainService.setListViewRows();
    });

    this.agriculturalExerciseConstrainService.listViewRows$.pipe(takeUntilDestroyed(this._destroyRef)).subscribe(rr => {
      this.rows$.next([...this.rows$.value, ...rr]);
    });

    this.constrainDdlForm = new FormGroup<DdlConstrainFG>(new DdlConstrainFG());

    this.constrainDdlForm.get('useOfLand').valueChanges.pipe(takeUntilDestroyed(this._destroyRef)).subscribe(v => {
      if (v.codice == this.BIO_USE_OF_LAND_CODE || v.codice == this.CONVERTING_USE_OF_LAND_CODE) {
        this.constrainDdlForm.get('constrain').setValue({codice: this.BIO_CONSTRAIN_CODE, descrizione: 'Bio'});
      }
    });

    this.constrainDdlForm.get('constrain').valueChanges.pipe(takeUntilDestroyed(this._destroyRef)).subscribe((v) => {
      if (v.codice === this.BIO_CONSTRAIN_CODE) {
        this._selectedConstrain = this._bioConstrain;
      } else {
        this._selectedConstrain = v;
      }

      if (this.invalidFormValue()) {
        this.messageService.errorMessage('Un appezzamento con metodo produzione \'Biologico\' può avere solo esercizi a loro volta con regolamento biologico.');
      }
    });
  }

  openDdl(ddl: GiasDropDownTemplateSComponent | GiiasMultiselectTemplateSComponent): void {
    switch (ddl.giasFormControlName) {
      case 'constrain':
        UtilityFunctions.loadDropDownItems(
            <GiasDropDownTemplateSComponent>ddl,
            lastValueFrom(this.constrainsService.leggiVincoli(new IntervalloTemporale()).pipe(take(1)))
        );
        break;
      case 'useOfLand':
        UtilityFunctions.loadDropDownItems(
            <GiasDropDownTemplateSComponent>ddl,
            lastValueFrom(of(this.plotsService.getArray_Metodo_Produzione()).pipe(take(1)))
        );
        break;
      default:
        UtilityFunctions.loadDropDownItems(<GiasDropDownTemplateSComponent>ddl, lastValueFrom(of([]).pipe(take(1))));
        break;
    }
  }

  disableSubmitBtn(): boolean {
    return this.invalidFormValue() || this.constrainDdlForm.invalid || this._isSaving;
  }

  submit(): void {
    if (this.constrainDdlForm.valid) {
      let plots: Appezzamento[] = this.rows$.value.map(r => {
        let plot: Appezzamento = r.toAppezzamento();
        let plant: Impianto = r.toImpianto();
        let exercise: Esercizio = r.toEsercizio();

        exercise.vincolo = this._selectedConstrain;
        exercise.regolamento = this._selectedConstrain.regolamento;
        exercise.disciplinare = this._selectedConstrain.disciplinare;
        exercise.apportiMassimiMacroelementi = new ApportoMacroelementi();
        exercise.apportiMassimiMacroelementi.pianoConcimazione = this._selectedConstrain.disciplinare.regolamentoConcimazione;
        plot.metodo_Produzione = this.constrainDdlForm.get('useOfLand').value;
        plant.esercizi = [exercise];
        plot.impianti = [plant];

        return plot;
      });

      this.masterService.set_isLoading({isLoading: true});
      this._isSaving = true;
      this.plotsService.updateConstrain(plots).pipe(
          take(1),
          catchError(() => of(false))
      ).subscribe({
        next: r => {
          if (r) {
            this.messageService.successMessage('SalvataggioAvvenutoConSuccesso', false, true);
          } else {
            this.messageService.errorMessage('ErroreDuranteIlSalvataggio', false, true);
          }
        },
        complete: () => {
          this.masterService.set_isLoading({isLoading: false});
          this._isSaving = false;
        },
        error: () => this.messageService.errorMessage('ErroreDuranteIlSalvataggio', false, true)
      });
    }
  }

  private invalidFormValue(): boolean {
    const useOfLandCode = this.constrainDdlForm.get('useOfLand').value.codice;
    const constrainCode = this.constrainDdlForm.get('constrain').value.codice;

    return (useOfLandCode == this.BIO_USE_OF_LAND_CODE || useOfLandCode == this.CONVERTING_USE_OF_LAND_CODE) && constrainCode != this.BIO_CONSTRAIN_CODE;
  }
}
