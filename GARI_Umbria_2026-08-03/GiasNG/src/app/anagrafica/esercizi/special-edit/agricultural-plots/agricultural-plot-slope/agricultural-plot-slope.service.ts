import {Inject, Injectable} from '@angular/core';
import {AgriculturalItemBaseService} from '../../agricultural-item-base.service';
import {IMPIANTI_SERVICE_TOKEN, ImpiantiFactoryService} from '../../../../../Service/ServiceFactory/impianti.factory.service';
import {ReplaySubject} from 'rxjs';
import {KendoGridRow} from 'gias-kendo-grid';
import {TranslocoService} from '@jsverse/transloco';
import {AgriculturalItem} from '../../agricultural-item.model';
import {IntervalloTemporale} from '../../../../../Model/anagrafiche/IntervalloTemporale';
import {AgriculturalPlotBaseService} from '../agricultural-plot-base.service';

@Injectable()
export class AgriculturalPlotSlopeService extends AgriculturalPlotBaseService {
  listViewRows$: ReplaySubject<AgriculturalItem[]> = new ReplaySubject<AgriculturalItem[]>(1);
  loading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  constructor(
    @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
    private transloco: TranslocoService
  ) {
    super();
  }

  extractDescription(r: KendoGridRow): string {
    return `${r['app_nome']}: ${r['Utilizzo']}, ${this.transloco.translate('Pendenza')}: ${r['Slope']} %`;
  }
}
