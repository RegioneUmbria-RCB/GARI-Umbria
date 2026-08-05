import { Injectable} from '@angular/core';
import { CatastoAppezzamento } from 'app/Model/anagrafiche/CatastoAppezzamento';
import { BaseCodeDescrStr } from 'app/Model/baseClass/baseCodeDescrStr';
import { Macrouso } from 'app/Model/metaschema/Macrouso';
import { BehaviorSubject, Observable } from 'rxjs';
import {CaricaCatastoSettings} from '../../../../Service/ServiceFactory/impianti.factory.service';

export class CatastoAppezzamento_Extended extends CatastoAppezzamento {
  macrousi: MacrousiCatastoAppezzamento[]
}

export class MacrousiCatastoAppezzamento {
  macrouso: Macrouso;
  Area: number;
  utilizzi: { utilizzo: BaseCodeDescrStr, Area: number }[]
}

@Injectable()
export class DatiCatastaliService {
  private catastoAppezzamento: CatastoAppezzamento_Extended[] = [];
  public caricaCatastoSettings = new BehaviorSubject<CaricaCatastoSettings>(null);

  private CatastoSource = new BehaviorSubject(this.catastoAppezzamento);
  public currentCatasto: Observable<CatastoAppezzamento_Extended[]> = this.CatastoSource.asObservable();

  public loading = new BehaviorSubject<boolean>(false);

  public setCatastoAppezzamento(catastoAppezzamento: CatastoAppezzamento_Extended[]) {
    this.CatastoSource.next(catastoAppezzamento);
  }

  public getCatastoAppezzamento(): CatastoAppezzamento_Extended[] {
    return this.CatastoSource.getValue();
  }

  public setCaricaCatastoSettings(caricaCatastoSettings: CaricaCatastoSettings) {
    this.caricaCatastoSettings.next(caricaCatastoSettings);
  }

  public getCaricaCatastoSettings(): CaricaCatastoSettings {
    return this.caricaCatastoSettings.getValue();
  }

  ControllaSeSelezionareLaRiga(dataItem: any, catasto: CatastoAppezzamento_Extended[], assegnaSuperficie: boolean): number {
    const settings: CaricaCatastoSettings = this.getCaricaCatastoSettings();
    let index: number = -1;

    if (catasto != undefined && catasto.length > 0) {
      const ProvItem = dataItem.CodiceIstat_Provincia;
      const ComuneItem = dataItem.CodiceIstat_Comune;
      let SezioneItem = dataItem.Sezione;
      const FoglioItem = dataItem.Foglio;
      const NumeroItem = dataItem.Numero;
      let SubalternoItem = dataItem.Subalterno;

      if (SezioneItem == '0') {
        SezioneItem = '';
      }

      if (SubalternoItem == '0') {
        SubalternoItem = '';
      }

      for (let x = 0; x < catasto.length; x++) {
        const ProvCatasto: string = catasto[x].particella['Prov'];
        const ComuneCatasto: string = catasto[x].particella['Com'];
        let SezioneCatasto: string = catasto[x].particella['Sezione'];
        const FoglioCatasto: number = catasto[x].particella['Foglio'];
        const NumeroCatasto: number = catasto[x].particella['Numero'];
        let SubalternoCatasto: string = catasto[x].particella['Subalterno'];

        if (SezioneCatasto == '0') {
          SezioneCatasto = '';
        }

        if (SubalternoCatasto == '0') {
          SubalternoCatasto = '';
        }

        if (
          ProvCatasto === ProvItem &&
          ComuneCatasto === ComuneItem &&
          SezioneCatasto.toUpperCase() === SezioneItem.toUpperCase() &&
          FoglioCatasto === FoglioItem &&
          NumeroCatasto === NumeroItem &&
          SubalternoCatasto.toUpperCase() === SubalternoItem.toUpperCase()
        ) {

          if (settings.chkMacrousi) {
            let findMacrouso = false;
            for (let macIndex = 0; macIndex < catasto[x].macrousi?.length; macIndex++) {
              if (dataItem.Macrouso_Cod == catasto[x].macrousi[macIndex].macrouso.codice) {
                findMacrouso = true;
                if (settings.chkUtilizzi) {
                  let findUtilizzo = false;
                  for (let utilIndex = 0; utilIndex < catasto[x].macrousi[macIndex].utilizzi?.length; utilIndex++) {
                    if (dataItem.Veg_Cod_Agea == catasto[x].macrousi[macIndex].utilizzi[utilIndex].utilizzo.codice) {
                      findUtilizzo = true;
                      dataItem.Selected = true;
                      index = x;
                      if (assegnaSuperficie) {
                        dataItem.SuperficieImpiegata = catasto[x].macrousi[macIndex].utilizzi[utilIndex].Area;
                      }
                    }
                    if (findUtilizzo) {
                      break;
                    }
                  }
                } else {
                  dataItem.Selected = true;
                  index = x;

                  if (assegnaSuperficie) {
                    dataItem.SuperficieImpiegata = catasto[x].macrousi[macIndex].Area;
                  }
                }
              }

              if (findMacrouso) {
                break;
              }
            }
          } else {
            dataItem.Selected = true;
            index = x;

            if (assegnaSuperficie) {
              dataItem.SuperficieImpiegata = catasto[x].area;
            }
          }

          return index;
        }
      }
    }

    return index;
  }
}
