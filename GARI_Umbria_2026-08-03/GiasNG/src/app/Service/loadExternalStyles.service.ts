import {MasterService} from "./master.service";
import {Injectable} from "@angular/core";
import {filter, switchMap, tap} from "rxjs";
import {ConfigurazioneSitiService, Configurazione_Siti} from "./configurazione-siti.service";
import {CssService} from "./css.service";


@Injectable({ providedIn: 'root'  })
export class LoadExternalStylesService {

  constructor(private masterService: MasterService,
              private configurazioneSitiService: ConfigurazioneSitiService,
              private cssService: CssService) {
    this.masterService.initialLoadCompleteSource.pipe(
      filter(val => val),
      switchMap(val => this.configurazioneSitiService.leggiChiave("ListaStiliPersonalizzati_NG")),
      tap((val: Configurazione_Siti) => {
        if (val?.Valore != ''){
          let files = val.Valore.split(";");
          let path = this.masterService.link_GiasBase + '/agronica/Styles/';
          for (let i = 0; i < files.length; i++){
            if (files[i] != ''){
              let id = files[i].replace(".css", "").replace(".", "");
              this.cssService.loadStyle(path + files[i], id);
            }
          }
        }
      })
    ).subscribe()
  }

}

