import { Inject, Injectable } from '@angular/core';
import { Campo } from 'app/Model/anagrafiche/Campo';
import { CodiceAnagrafe } from 'app/Model/anagrafiche/CodiceAnagrafe';
import { CodiciAnagrafeValori, CodiciAnagrafeValoriChiave } from 'app/Model/anagrafiche/CodiciAnagrafeValori';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { MasterService } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { CampiFactoryService, CAMPI_SERVICE_TOKEN } from 'app/Service/ServiceFactory/campi.factory.service';
import { ICodiciTemplateService } from 'app/Utility/Template/codici-template/services/codici-template.service';
import { HttpAction } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { EMPTY, from, Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { CaricaDatiDto as CampoEditDto } from './campi-codici-metadata';
import { ObjParametriAgenda } from 'gias-ui-kit';

const defaultIndex = -1;

const itemIndex = (item: CodiciAnagrafeValoriChiave, data: CodiciAnagrafeValoriChiave[]): number => {
  for (let idx = 0; idx < data.length; idx++) {
    if (data[idx].chiave === item.chiave) {
      return idx;
    }
  }
  return defaultIndex;
};

@Injectable()
export class CampiEditService implements ICodiciTemplateService {
  gridId = "CampiCodici";
  public gridPublicService: GridPublicService;
  public form: Campo;
  public codici: CodiciAnagrafeValoriChiave[] = [];


  constructor(
    @Inject(CAMPI_SERVICE_TOKEN) private campiService: CampiFactoryService,
    private parametriAgenda: ObjParametriAgendaService,
    private masterService: MasterService
  ) {  }

  public updateForm(form: Campo) {
    this.form = form;
    this.form.codici = <CodiciAnagrafeValori[]>this.codici;
  }

  public updateTable(action: HttpAction, row: CodiciAnagrafeValoriChiave) {

    if (this.codici == undefined) {
      this.codici = new Array<CodiciAnagrafeValoriChiave>()
    }

    switch(action) {
      case HttpAction.CREATE:
        if (this.codici != undefined && this.codici.length != 0){
          const chiave = Math.max.apply(Math, this.codici.map(function (o) {
            return o.chiave;
          }));
          row.chiave = chiave + 1;
        } else {
          const chiave = 0;
          row.chiave = chiave;
        }
        this.codici.push(row);
        break;
      case HttpAction.UPDATE:
        const index = itemIndex(row, this.codici);
        this.codici.splice(index, 1, row);
        break;
      case HttpAction.REMOVE:
        const _index = itemIndex(row, this.codici);
        this.codici.splice(_index, 1);
        break;
    }
    this.overwriteCodes();
    this.gridPublicService.refresh(true);
  }

  private overwriteCodes(){
    this.form.codici = this.codici;
  }

  private setRowsUniqueId(rows: CodiciAnagrafeValoriChiave[]) {
    let i = 0;
    rows.forEach((row) => {
      row.chiave = i++;
    });
  }

  public async postForm() {
    const parametri: CoreWS_Generic<Campo> = new CoreWS_Generic(
      this.masterService.getCoreWSGenericObjP(),
      this.form
    );
  }

  private setParametriAgenda(agenda: ObjParametriAgenda): ObjParametriAgenda {
    return agenda;
  }

  private agendaIsValid(agenda: ObjParametriAgenda) {
    return agenda.Piva && agenda.Piva !== '';
  }

  leggiDropdowns(): Observable<CodiceAnagrafe[]> {
    const agenda = this.setParametriAgenda(this.parametriAgenda.getObjParamValue());

    if (this.agendaIsValid(agenda)) {
      return this.campiService.leggiCampiCodici(agenda);
    }

    return of([]);
  }

  leggiDatiTabella(): Observable<CodiciAnagrafeValoriChiave[]> {
    return of(this.codici);
  }

  public caricaDati(): Observable<CampoEditDto> {
    this.masterService.set_isLoading({ isLoading: true,
      message: 'Caricamento in corso' });

    const agenda = this.parametriAgenda.getObjParamValue();

    const data = this.provaCaricareIDati(agenda).pipe(map((data) => {
      data.ObjParamAgenda = agenda;
      return data;
    }));

    this.masterService.set_isLoading({ isLoading: false });
    return data;
  }

  private provaCaricareIDati(agenda) {
    const data = from(Promise.all([
      this.campiService.leggiCampo(agenda)
    ])).pipe(map((dati): CampoEditDto => {
      const campo: Campo = dati[0].RispostaStringa;
      this.form = campo;
      this.codici = campo.codici as CodiciAnagrafeValoriChiave[];
      if (this.codici == undefined) {
        this.codici = Array<CodiciAnagrafeValoriChiave>();
      }
      this.setRowsUniqueId(this.codici);

      return {
        Campo: this.form,
        ObjParamAgenda: null
      };
    }, catchError((err) => {
      console.log(err);
      return EMPTY;
    })));
    return data;
  }
}
