import { AfterViewInit, Component, Inject, TemplateRef, ViewChild } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { StatoPatrimonialeHttpService } from './stato-patrimoniale-config.service';
import { KendoGridMasterDetailService } from 'gias-kendo-grid';
import { GRID_HTTP_TOKEN } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { ValutazioniService } from 'app/valutazioni/valutazioni/service/valutazioni.service';
import { TranslocoService } from '@jsverse/transloco';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { DialogResult } from '@progress/kendo-angular-dialog';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { TestGridMasterService } from 'app/Utility/Template/kendo-grid/test/grid-master-detail/test-grid-master-detail.service';

@Component({
  standalone: false,
  selector: 'app-valutazioni-stato-patrimoniale',
  templateUrl: './valutazioni-stato-patrimoniale.component.html',
  styleUrls: ['./valutazioni-stato-patrimoniale.component.css'],
  providers: [
    ...generateGridProviders(StatoPatrimonialeHttpService, ValutazioniStatoPatrimonialeComponent, KendoGridMasterDetailService)
  ]
})
export class ValutazioniStatoPatrimonialeComponent implements AfterViewInit {

  @ViewChild("TemplateDetail") Tmp: TemplateRef<any> = null;
  permessoEdit: boolean;


  constructor(@Inject(GRID_HTTP_TOKEN) private gridmasterservice: TestGridMasterService,
    private publicservice: GridPublicService,
    private valutazioniService: ValutazioniService,
    private translocoService: TranslocoService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    private objParametriService: ObjParametriAgendaService,
    private permessiUtenteService: PermessiUtenteService
  ) {

    let objParametriAgenda = this.objParametriService.getObjParamValue();
    this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Valutazioni_Rischio, 2) && (objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Update)

  }
  ngAfterViewInit() {
    this.gridmasterservice.masterdetailSettings.templateGridDetail = this.Tmp;
  }

  aggiornatiDatiDettaglioSpecifico(dataItem: any): any {
    return new Promise((resolve, reject) => {
      this.giasDialogService.baseWarning(
        '', this.translocoService.translate('AggiornaDatiDettaglioDomandaSpecifico'), false
      ).then((resp: DialogResult) => {
        if (!resp['returnObj']) return;
        this.aggiornaDatiSpecifico(dataItem);
      });
    });

  }

  aggiornaDatiSpecifico(dataItem: any): any {
    if (dataItem != null) {
      this.valutazioniService.aggiornaDatiDettaglioSingolo(dataItem).subscribe(x => {
        if (x.RispostaOK) {
          console.log(x);
          this.giasMessageService.successMessage(this.translocoService.translate('RecordAggiornatoCorrettamente'));
          this.publicservice.refresh(true);
        }
      });
    }
  }



}
