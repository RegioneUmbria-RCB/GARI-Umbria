import {Inject, Injectable} from '@angular/core';
import {IMPIANTI_SERVICE_TOKEN, ImpiantiFactoryService} from '../../../../../Service/ServiceFactory/impianti.factory.service';
import {KendoGridRow} from 'gias-kendo-grid';
import {AgriculturalExerciseBaseService} from '../agricultural-exercise-base.service';

@Injectable()
export class AgriculturalExerciseConstrainService extends AgriculturalExerciseBaseService {
  constructor(
    @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
  ) {
    super();
  }

  protected extractDescription(row: KendoGridRow): string {
    const saNome: string = row['sa_nome'] ?? '';
    const appDes: string = row['app_nome'] ?? '';

    const specie: string = row['veg_des'] ?? '';
    const varieta: string = row['cul_des'] ?? '';
    const useOfLand: string = row['Destinazione_Uso_Des'] ?? '';
    const constrain: string = row['Metodo_Produzione_Des'] ?? '';

    return `${saNome}, ${appDes}: ${useOfLand === '' ? `${specie} ${varieta}` : useOfLand}${constrain === '' ? '' : ` - ${constrain}`}`;
  }
}

