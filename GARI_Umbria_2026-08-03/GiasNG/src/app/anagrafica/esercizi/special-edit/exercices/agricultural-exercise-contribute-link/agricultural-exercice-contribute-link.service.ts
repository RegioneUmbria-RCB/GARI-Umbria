import {Inject, Injectable} from '@angular/core';
import {from, ReplaySubject, Subject, zip} from 'rxjs';
import { KendoGridRow } from 'gias-kendo-grid';
import {IMPIANTI_SERVICE_TOKEN, ImpiantiFactoryService} from '../../../../../Service/ServiceFactory/impianti.factory.service';
import {ObjParametriAgendaService} from '../../../../../Service/obj-parametri-agenda.service';
import {GiasMessageService} from '../../../../../Service/gias-message.service';
import {FunzioniComuniService} from '../../../../../Service/FunzioniComuni.service';
import {Contribute} from '../../../../../Model/metaschema/Contribute';
import {IntervalloTemporale} from '../../../../../Model/anagrafiche/IntervalloTemporale';
import {AgriculturalItem} from '../../agricultural-item.model';
import {AgriculturalExerciseBaseService} from '../agricultural-exercise-base.service';

@Injectable()
export class AgriculturalExerciceContributeLinkService extends AgriculturalExerciseBaseService {
  constructor(
    @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
    private objParametriService: ObjParametriAgendaService,
    private giasMessageService: GiasMessageService,
    private fcService: FunzioniComuniService
  ) {
    super();
  }

  override setListViewRows(): void {
    this.loading$.next(true);
    zip(from(this._checkedRows)).subscribe({
      error: () => this.loading$.next(false),
      next: r => this.listViewRows$.next(r.map(this.mapToListViewItems.bind(this))),
      complete: () => this.loading$.next(false)
    });
  }

  compareExerciceContributeValidity(exs: AgriculturalItem[], contributes: Contribute[]): void {
    exs.forEach(p => {
      p.error = contributes.some(c => !this.fcService.checkSovrapposizioneIntervalli(p.validity, c.validity));
    });
  }

  protected override extractDescription(row: KendoGridRow): string {
    const saNome: string = row['sa_nome'] ?? '';
    const appDes: string = row['app_nome'] ?? '';

    const specie: string = row['veg_des'] ?? '';
    const varieta: string = row['cul_des'] ?? '';
    const destUso: string = row['Destinazione_Uso_Des'] ?? '';
    const linkedContributes: string = row['LinkedACAContributes'] ?? '';

    return `${saNome}, ${appDes}: ${destUso === '' ? `${specie} ${varieta}` : destUso}${linkedContributes === '' ? '' : ` - ${linkedContributes}`}`;
  }
}
