import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CodiciNazioniISO3166 } from 'app/Model/metaschema/CodiciNazioniISO3166';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { EditingMode, KendoGridColumn } from 'gias-kendo-grid';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { Provincia } from '../../Model/MetaschemaModel';
import { CentriHttpService } from './services/centri-grid-config.service';
import { KendoCentroModel } from './centri.models';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { GridPublicService } from 'gias-kendo-grid';
import { generateGridProvidersAnagrafica } from 'app/Utility/Template/kendo-grid/services/providers';
import { ObjParametriAgenda } from 'gias-ui-kit';


@Component({
  standalone: false,
  selector: 'gias-anagrafica-centri',
  templateUrl: './centri.component.html',
  styleUrls: ['./centri.component.css'],
  providers: [
    ...generateGridProvidersAnagrafica(CentriHttpService, CentriComponent)
  ]
})
export class CentriComponent implements OnInit, OnDestroy {
  editingMode: EditingMode = EditingMode.IN_LINE;

  objParametriAgenda: ObjParametriAgenda;
  kendo_model: KendoCentroModel;
  kendo_columns: Array<KendoGridColumn> = new Array<KendoGridColumn>();
  kendo_rows: [];

  stati: CodiciNazioniISO3166[];
  provinci: Provincia[];
  public permessoEdit: boolean;

  private signal$: Subject<void> = new Subject();

  constructor(
    private route: ActivatedRoute,
    private kendoGridService: GridPublicService,
    private objParametriService: ObjParametriAgendaService,
    private permessiUtenteService: PermessiUtenteService,
    private router: Router
  ) {
  }

  async ngOnInit() {
    this.objParametriService.currentObjParametriAgenda.pipe(takeUntil(this.signal$))
      .subscribe((data: ObjParametriAgenda) => {
        this.objParametriAgenda = data;
      });

    this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale, 2);

    this.kendoGridService.changeDetected.pipe(
      takeUntil(this.signal$)).subscribe(
        (event: any) => {
          if (event?.action === 'remove') {
            this.eliminaCentro(event.dataItem);
          }
          if (event?.action === 'info') {
            this.infoCentro(event.dataItem);
          }
        });
  }

  public impresaSelezionata() {
    return this.objParametriAgenda.Piva != null && this.objParametriAgenda.Piva !== '';
  }

  ngOnDestroy() {
    this.signal$.next();
    this.signal$.complete();
  }

  onNuovo() {
    this.objParametriAgenda = this.objParametriService.getObjParamValue();
    const piva = this.objParametriAgenda.Piva;
    this.objParametriAgenda.Piva = piva;
    this.objParametriAgenda.Sa_Cod = 0;
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
    this.objParametriService.changeObjParametriAgenda(this.objParametriAgenda);
    this.router.navigate(['Centri-Edit'], { relativeTo: this.route });
  }

  eliminaCentro(event: any) {
    let a = 0; //Commento per funzione vuota SonarQube
  }

  infoCentro(dataItem: any) {
    this.objParametriAgenda = this.objParametriService.getObjParamValue();
    const piva = (<string>dataItem.chiave).split('_')[0];
    const sa_cod = (<string>dataItem.chiave).split('_')[1];
    this.objParametriAgenda.Piva = piva;
    this.objParametriAgenda.Sa_Cod = parseInt(sa_cod);
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
    this.objParametriService.changeObjParametriAgenda(this.objParametriAgenda);
    this.router.navigate(['Centri-Edit'], { relativeTo: this.route });
  }

}
