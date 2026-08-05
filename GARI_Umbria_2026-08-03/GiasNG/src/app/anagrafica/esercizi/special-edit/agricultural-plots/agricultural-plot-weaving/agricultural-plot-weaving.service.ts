import {Inject, Injectable} from '@angular/core';
import {ReplaySubject} from 'rxjs';
import {KendoGridRow} from 'gias-kendo-grid';
import {IMPIANTI_SERVICE_TOKEN, ImpiantiFactoryService} from '../../../../../Service/ServiceFactory/impianti.factory.service';
import {AgriculturalItemBaseService} from '../../agricultural-item-base.service';
import {TranslocoService} from '@jsverse/transloco';
import {AgriculturalItem} from '../../agricultural-item.model';
import {AgriculturalPlotBaseService} from '../agricultural-plot-base.service';

@Injectable()
export class AgriculturalPlotWeavingService extends AgriculturalPlotBaseService {
  listViewRows$: ReplaySubject<AgriculturalItem[]> = new ReplaySubject<AgriculturalItem[]>(1);
  loading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  constructor(
    @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
    private transloco: TranslocoService
  ) {
    super();
  }

  extractDescription(r: KendoGridRow): string {
    return `${r['app_nome']}: ${r['Utilizzo']}, \
    ${this.transloco.translate('Tessitura')}: \
    ${this.transloco.translate('Clay')} ${r['Clay']} % \
    ${this.transloco.translate('Silt')} ${r['Silt']} % \
    ${this.transloco.translate('Sand')} ${r['Sand']} %`;
  }
}
