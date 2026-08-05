import {Inject, Injectable} from '@angular/core';
import {GiasMessageService} from 'gias-kendo-grid';
import {catchError, concatMap, from, of, zip} from 'rxjs';
import { AGRODATAFINE, AGRODATAINIZIO, ObjParametriAgenda } from 'gias-ui-kit';
import { AppezzamentoJoinDescrizioni, PKAppezzamento } from 'app/Model/anagrafiche/Appezzamento';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { LinkedMachine } from 'app/Model/anagrafiche/ParcoMacchine';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { IMPIANTI_SERVICE_TOKEN, ImpiantiFactoryService } from 'app/Service/ServiceFactory/impianti.factory.service';
import { Impianto } from 'app/Model/anagrafiche/Impianto';
import {AgriculturalItem} from '../../agricultural-item.model';
import {AgriculturalPlotBaseService} from '../agricultural-plot-base.service';
import {UtilizzoTerreno} from '../../../../../Model/metaschema/utilizzi/UtilizzoTerreno';

@Injectable()
export class AgriculturalPlotsMachinesLinkService extends AgriculturalPlotBaseService {
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
    zip(from(this.getUniqueKeysFromRows()).pipe(
      catchError(() => {
        this.giasMessageService.errorMessage('ErroreLetturaAppezzamenti', false, true);
        return of(null);
      }),
      concatMap(apk => {
        const objP: ObjParametriAgenda = new ObjParametriAgenda();
        objP.Piva = apk.centroAziendalePK.partitaIva;
        objP.Sa_Cod = apk.centroAziendalePK.codice;
        objP.Appezza = apk.codice;
        return this.appezzamentiService.readAgriculturalPlotLight(objP);
      })
    )).subscribe({
      error: () => this.loading$.next(false),
      next: r => {
        this.listViewRows$.next(r.map(this.mapToListViewItems.bind(this)));
      },
      complete: () => this.loading$.next(false)
    });
  }

  compareMachinePlotsValidity(plots: AgriculturalItem[], machine: object): void {
    plots.forEach(p => {
      p.error = !this.fcService.checkSovrapposizioneIntervalli(p.validity, new IntervalloTemporale(machine['Validita_Inizio'], machine['Validita_Fine']));
    });
  }

  private getUniqueKeysFromRows(): PKAppezzamento[] {
    const jsonKeys: string[] = this._checkedRows.map(r => JSON.stringify(
      {
        codice: r['APPEZZA'],
        centroAziendalePK: {
          codice: r['SA_COD'],
          partitaIva: r['PIVA']
        }
      })
    );
    return Array.from<string>(new Set<string>(jsonKeys)).map(i => JSON.parse(i));
  }

  protected override mapToListViewItems(app: AppezzamentoJoinDescrizioni): AgriculturalItem {
    return new AgriculturalItem(
      `${app.primaryKey.centroAziendalePK.partitaIva}_${app.primaryKey.centroAziendalePK.codice}_${app.primaryKey.codice}`,
      this.extractDescription(app),
      app.primaryKey.centroAziendalePK.partitaIva,
      app.primaryKey.centroAziendalePK.codice,
      app.primaryKey.codice,
      0,
      0,
      new IntervalloTemporale(app.validita.inizio, app.validita.fine),
      null,
      false,
      false,
      false
    );
  }

  protected extractDescription(app: AppezzamentoJoinDescrizioni): string {
    const appDes: string = app.descrizione;

    const activePlant: Impianto = this.selectActivePlant(app);

    const specie: string = this.plantSpecies(activePlant);
    const varieta: string = this.plantSpeciesVariety(activePlant);
    const linkedMachines: string = this.linkedMachinesDescriptions(app);

    return `${appDes}: ${specie} ${varieta}${linkedMachines === '' ? '' : ` - ${linkedMachines}`}`;
  }

  private plantSpecies(plant: Impianto): string {
    let ut: string;
    if (plant.utilizzoTerreno.classType.toLowerCase() == 'varieta') {
      ut = plant.utilizzoTerreno['specie']?.descrizione;
    } else {
      ut = plant.utilizzoTerreno.descrizione;
    }
    return ut;
  }

  private plantSpeciesVariety(plant: Impianto): string {
    let v: string = '';
    if (plant.utilizzoTerreno.classType.toLowerCase() == 'varieta') {
      v = plant.utilizzoTerreno.descrizione;
    }
    return v;
  }

  private selectActivePlant(app: AppezzamentoJoinDescrizioni): Impianto {
    const date: Date = this.objParametriService.getObjParamValue().Data;
    return app.impianti.filter(i => date === AGRODATAINIZIO || (i.validita.inizio <= date && i.validita.fine >= date)).pop();
  }

  private linkedMachinesDescriptions(app: AppezzamentoJoinDescrizioni): string {
    return app.linkedMachines?.map(m => {
      const validity: string = this.linkedMachinesValidities(m);
      return `${m.descrizione}${validity === '' ? '' : `: ${validity}`}`;
    }).join(', ') ?? '';
  }

  private linkedMachinesValidities(m: LinkedMachine<PKAppezzamento>): string {
    const start: Date = m.linkValidity.inizio;
    const end: Date = m.linkValidity.fine;

    return `${start.getTime() == AGRODATAINIZIO.getTime() ? '' : start.toLocaleDateString()}${end.getTime() == AGRODATAFINE.getTime() ? '' : ` - ${end.toLocaleDateString()}`}`;
  }
}
