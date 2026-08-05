import { Component, Input, OnChanges } from '@angular/core';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { finalize, Observable, of, startWith, Subject, switchMap, tap } from 'rxjs';
import { MacchinaDettaglio, ProfilazioneImpreseService } from '../../services/profilazione-imprese.service';
import { editToolsIcon } from '@progress/kendo-svg-icons';
import { catchError } from 'rxjs';
import { TranslocoService } from '@jsverse/transloco';
import { CaratteristicheMacchina_In } from 'app/Service/net-core6-api.service';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-macchine-dialog',
  templateUrl: './profilazione-imprese-macchine-dialog.component.html',
  styleUrls: ['./profilazione-imprese-macchine-dialog.component.scss', '../../profilazione-imprese.component.scss']
})
export class ProfilazioneImpreseMacchineDialogComponent implements OnChanges {

  editToolsIcon = editToolsIcon;

  @Input() idProfilazione: number;

  macchine$: Observable<MacchinaDettaglio[]> | null = null;
  loading: boolean = false;
  modality: 'edit' | 'info' | null = null;
  details: MachineDetail[] = [];
  reloadSubject = new Subject();

  constructor(
    private service: ProfilazioneImpreseService,
    private giasMessageService: GiasMessageService,
    private translocoService: TranslocoService
  ) { }

  ngOnChanges(): void {
    if (this.idProfilazione == null) {
      return;
    }

    this.loading = true;
    this.macchine$ = this.reloadSubject
      .pipe(
        startWith(null),
        switchMap(() => this.service.leggiMacchine(this.idProfilazione)),
        catchError(() => of([] as MacchinaDettaglio[])),
        tap(() => this.loading = false)
      );
  }

  info(dataItem: MacchinaDettaglio): void {
    if (dataItem.elenco_mac_car.trim() == '') {
      this.giasMessageService.infoMessagge(this.translocoService.translate('profImprese.NessunDettaglioPerQuestaMacchina'));
      this.modality = null;
      return;
    }

    this.modality = 'info';
    this.details = this.getDetails(dataItem);

  }

  edit(dataItem: MacchinaDettaglio): void {
    if (dataItem.elenco_mac_car.trim() == '') {
      this.giasMessageService.infoMessagge(this.translocoService.translate('profImprese.NessunDettaglioPerQuestaMacchina'));
      this.modality = null;
      return;
    }

    this.modality = 'edit';
    this.details = this.getDetails(dataItem);
  }

  save(): void {
    const payload = {
      IdProfilazione: this.idProfilazione,
      Caratteristiche: this.details.map(x => ({
        MacCod: x.mac_cod,
        MacCarCod: x.mac_car_cod,
        Valore: x.valore
      }))
    } as CaratteristicheMacchina_In;
    this.service
      .aggiornaMacchinaCaratteristiche(payload)
      .pipe(catchError(() => of(false)))
      .subscribe(res => {
        this.modality = null;

        if (res) {
          this.giasMessageService.successMessage(this.translocoService.translate('profImprese.AggiornatoCorrettamente'));
          this.reloadSubject.next(null);
          return;
        }

        this.giasMessageService.errorMessage(this.translocoService.translate('profImprese.ErroreAggiornamento'));
      })
  }

  private getDetails(dataItem: MacchinaDettaglio): MachineDetail[] {
    const result = [];
    const chars = dataItem.elenco_mac_car.split('|');
    for (const char of chars) {
      const charArray = char.split('=');
      if (charArray.length > 1) {
        const elem = {
          mac_cod: dataItem.mac_cod,
          mac_car_cod: +charArray[0],
          mac_car_des: charArray[1].split(':')[0],
          valore: charArray[1].split(':')[1],
        } as MachineDetail;

        result.push(elem)
      }
    }

    return result;
  }
}

interface MachineDetail {
  mac_cod: number;
  mac_car_cod: number;
  mac_car_des: string;
  valore: string;
}
