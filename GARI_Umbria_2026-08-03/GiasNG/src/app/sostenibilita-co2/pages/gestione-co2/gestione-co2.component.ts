import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { EMPTY } from 'rxjs';
import { finalize, switchMap, takeUntil } from 'rxjs/operators';
import { TranslocoService } from '@jsverse/transloco';
import { GiasMessageService } from 'gias-kendo-grid';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { Co2DataStoreService } from '../../services/co2-data-store.service';
import { M4SubmissionService } from '../../services/m4-submission.service';
import { CarburanteM4, EnergiaM4, EsercizioM4, PerimetroM4 } from '../../models/sostenibilita-co2.model';

@Component({
  standalone: false,
  selector: 'app-gestione-co2',
  templateUrl: './gestione-co2.component.html',
  styleUrls: ['./gestione-co2.component.scss']
})
export class GestioneCO2PageComponent implements OnInit, OnDestroy {

  private destroy$ = new Subject<void>();

  isProcessing = false;
  riepilogoLabel = '';

  constructor(
    private co2DataStore: Co2DataStoreService,
    private m4Service: M4SubmissionService,
    private translocoService: TranslocoService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (!this.co2DataStore.currentFilters) {
      this.router.navigate(['SostenibitaCO2', 'SelezionePerimetro']);
      return;
    }

    const filters = this.co2DataStore.currentFilters;
    const colture = (filters.coltura && filters.coltura.length > 0)
      ? filters.coltura.join(', ')
      : this.translocoService.translate('sco2_label_tutte_colture');

    this.riepilogoLabel =
      `${this.translocoService.translate('gco2_label_filiera')}: ${filters.filiera} | ` +
      `${this.translocoService.translate('gco2_label_anno')}: ${filters.anno} | ` +
      `${this.translocoService.translate('gco2_label_colture')}: ${colture}`;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onProsegui(): void {
    const title = this.translocoService.translate('gco2_dialog_conferma_title');
    const message = this.translocoService.translate('gco2_dialog_conferma_message');

    this.giasDialogService.dialogMessageObs_Result(title, message)
      .pipe(takeUntil(this.destroy$))
      .subscribe((result: any) => {
        if (result && result.returnObj) {
          this.eseguiInvio();
        }
      });
  }

  private eseguiInvio(): void {
    const selectedRows = this.co2DataStore.selectedRows;
    const filters = this.co2DataStore.currentFilters;

    const perimetro: PerimetroM4 = {
      Aziende: [...new Set(selectedRows.map(r => r.piva))],
      Filiera: filters?.filiera ?? '',
      Anno: filters?.anno ?? 0,
      Colture: filters?.coltura ?? [],
      Modalita: filters?.modalita ?? 'Colture'
    };

    const esercizi: EsercizioM4[] = selectedRows.map(r => ({
      id_esercizio: parseInt(r.esercizio, 10),
      nazione: r.nazione,
      istat_reg: r.istat_reg
    }));

    const rawCarburanti: CarburanteM4[] = this.co2DataStore.carburanti.map(c => ({
      Azienda: c.azienda_piva,
      TipoCarburante: c.tipo_carburante,
      Quantita: c.quantita,
      UnitaMisura: c.unita_misura
    }));

    const rawEnergia: EnergiaM4[] = this.co2DataStore.energia.map(e => ({
      Azienda: e.azienda_piva,
      DataInizio: e.data_inizio ? e.data_inizio.toISOString() : '',
      DataFine: e.data_fine ? e.data_fine.toISOString() : '',
      ConsumoKwh: e.consumo_kwh,
      PercentualeRinnovabili: e.percentuale_rinnovabili
    }));

    this.isProcessing = true;

    this.m4Service.validaEComponi(rawCarburanti, rawEnergia, perimetro, esercizi)
      .pipe(
        takeUntil(this.destroy$),
        switchMap(result => {
          if (!result.ValidazioneEsito) {
            this.giasMessageService.errorMessage(result.Errori.join('\n'));
            return EMPTY;
          }
          return this.m4Service.inviaDatiAM4();
        }),
        finalize(() => { this.isProcessing = false; })
      )
      .subscribe({
        next: () => {
          this.giasMessageService.successMessage(
            this.translocoService.translate('gco2_submit_success')
          );
          this.co2DataStore.reset();
          this.router.navigate(['SostenibitaCO2', 'SelezionePerimetro']);
        },
        error: () => {
          this.giasMessageService.errorMessage(
            this.translocoService.translate('gco2_error_invio')
          );
        }
      });
  }

  onIndietro(): void {
    this.router.navigate(['SostenibitaCO2', 'SelezionePerimetro']);
  }
}
