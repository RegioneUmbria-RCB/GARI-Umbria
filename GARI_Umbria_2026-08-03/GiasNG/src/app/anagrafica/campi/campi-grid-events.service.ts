import { Inject, Injectable } from '@angular/core';
import { EditEvent } from '@progress/kendo-angular-grid';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { ActivatedRoute, Router } from '@angular/router';
import { MasterService, RispostaStandard } from 'app/Service/master.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { Observable, tap } from 'rxjs';
import { Campo } from 'app/Model/anagrafiche/Campo';
import { enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { toInteger } from 'lodash';
import { KendoGridRow } from 'gias-kendo-grid';
import { InvestimentoCatastaleCampoService } from './investimento-catastale-campo/investimento-catastale-campo.service';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { ParticelleCatastali } from 'app/Model/anagrafiche/ParticelleCatastali';
import { InvestimentoCatastaleCampoComponent } from './investimento-catastale-campo/investimento-catastale-campo.component';
import { GiasWindowsService } from 'gias-ui-kit';
import { CampiFactoryService, CAMPI_SERVICE_TOKEN } from 'app/Service/ServiceFactory/campi.factory.service';
import { TranslocoService } from '@jsverse/transloco';
import { GiasMessageService } from 'app/Service/gias-message.service';
import {GiasDialogService} from "../../Service/gias-dialog.service";
import { ObjParametriAgenda } from 'gias-ui-kit';

@Injectable()
export class CampiGridEventsService {

  objParametriAgenda: ObjParametriAgenda;

  private campiSelezionati : Map<Campo, string> = new Map<Campo, string>();

  constructor(
    private masterService: MasterService,
    @Inject(CAMPI_SERVICE_TOKEN) private campiService: CampiFactoryService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private router: Router,
    private route: ActivatedRoute,
    private windowService: GiasWindowsService,
    private investimentoCatastaleService: InvestimentoCatastaleCampoService,
    protected transloco: TranslocoService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
  ) {

  }

  infoCampoAngular(event: EditEvent) {

    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
    this.objParametriAgenda.Campo_Cod = toInteger(event?.dataItem['Campo_Cod']);
    this.objParametriAgenda.Sa_Cod = toInteger(event?.dataItem['sa_cod']);

    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);

    //this.router.navigate(['/Anagrafica/Campi/Campi-Edit']);
    this.router.navigate(['Campi-Edit'], { relativeTo: this.route });
  }

  modificaCampoAngular(row: KendoGridRow, add_operation: boolean, rowIndex: number) {
    this.objParametriAgendaService.setValiditaInizio(AGRODATAINIZIO);
    this.objParametriAgendaService.setValiditaFine(AGRODATAFINE);

    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    if (add_operation){
      this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
      this.objParametriAgenda.Campo_Cod = 0;
    } else {
      this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
      this.objParametriAgenda.Campo_Cod = toInteger(row['Campo_Cod']);
      this.objParametriAgenda.Sa_Cod = toInteger(row['sa_cod']);
    }

    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);

    //this.router.navigate(['/Anagrafica/Campi/Campi-Edit']);
    this.router.navigate(['Campi-Edit'], { relativeTo: this.route });
  }

  eliminaCampoAngular(item: any, deleteRibaltamento: boolean = false): Observable<RispostaStandard> {
    let campo: Campo = this.prepareCampo(item, enum_TipoOperazioneDB.Cancellazione);
    return this.campiService.aggiornaCampoObs(campo, enum_TipoOperazioneDB.Cancellazione, deleteRibaltamento);
  }

  updateCampo_inLine(item: any, tipo_operazione: enum_TipoOperazioneDB): Observable<any> {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    let campo = this.prepareCampo(item, tipo_operazione);
    return this.campiService.aggiornaCampo_inLine(campo, tipo_operazione, this.objParametriAgenda).pipe(
      tap((resp) => {
        this.masterService.set_isLoading({ isLoading: false, message: '' });
        let objPAgenda = this.objParametriAgendaService.getObjParamValue();
        objPAgenda.Sa_Cod = 0;
        objPAgenda.Campo_Cod = 0;
        this.objParametriAgendaService.changeObjParametriAgenda(objPAgenda);
        if (resp.RispostaOK) {

        }
      })
    )
  }

  prepareCampo(item: any, tipo_operazione: enum_TipoOperazioneDB): Campo {
    let primaryKeyCampo;
    if (tipo_operazione != enum_TipoOperazioneDB.Scrittura) {
      let primaryKeyCentro = new CentroAziendale.PK(toInteger(item.sa_cod), item.PIVA);
      primaryKeyCampo = new Campo.PK(
        toInteger(item.Campo_Cod),
        primaryKeyCentro
      )
    } else {
      let primaryKeyCentro = new CentroAziendale.PK(toInteger(item.sa_cod), this.objParametriAgenda.Piva);
      primaryKeyCampo = new Campo.PK(
        0,
        primaryKeyCentro
      )
    }

    const campo = new Campo(primaryKeyCampo);

    campo.campo_Codice = item.Campo_Cod;
    campo.descrizione = item.Campo;
    campo.orientamento_Colturale = toInteger(item.Gru_Cod);
    campo.specie = new Specie(campo.orientamento_Colturale != 3 ? 0 : toInteger(item.Veg_Cod));
    campo.validita = new IntervalloTemporale(item.Validita_Inizio, item.Validita_Fine);
    campo.campo_Codice = item.rif_alfanumerico;

    return campo;
  }

  onInvestimentoCatastale(dataItem: any) {
    const Piva = dataItem.PIVA;
    const Sa_Cod = dataItem.sa_cod;
    const Campo_Cod = dataItem.Campo_Cod;

    let impresa = new Impresa;
    impresa.partitaIva = Piva

    let centro = new CentroAziendale({ partitaIva: impresa.partitaIva, codice: Sa_Cod })

    let particella = new ParticelleCatastali()
    particella.primaryKey = {
      Prov: '',
      Com: '',
      Sezione: '',
      Foglio: 0,
      Numero: 0,
      Subalterno: '',
    }
    let campo = new Campo({ codice: Campo_Cod, centroAziendalePK: centro.primaryKey });
    let data = AGRODATAINIZIO

    this.investimentoCatastaleService.filtri.next({
      impresa: impresa,
      centro: centro,
      particella: particella,
      campo:campo,
      data: data
    })
    this.windowService.open({
      title: this.transloco.translate('InvestimentoCatastale'),
      content: InvestimentoCatastaleCampoComponent,
      //top: 115,
      left: 10,
      width: window.innerWidth - 30,
      autoFocusedElement: "#investimentocatastalecampocomponent"
    })
  }

  addCampoSelezionato(campo: Campo, error: string) {
    this.campiSelezionati.set(campo, error);
  }

  removeCampoSelezionato(campo: Campo) {
    this.campiSelezionati.delete(campo);
  }

  getCampiSelezionati() {
    return this.campiSelezionati;
  }

  clearSelezionati(): void {
    this.campiSelezionati = new Map<Campo, string>();
  }

  msgWarningDelete() {
    if(this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB == Enum_DBTypeOperation.Delete) {
      let msgFailure = this.transloco.translate('ErroreCancellazioneCampi')
      let msgSuccess = this.transloco.translate('SuccessoCancellazioneCampi')
      let displaySuccess = false;
      let displayFailure = false;
      this.getCampiSelezionati().forEach((error, cam) => {
        if(error == "") {
          msgSuccess = msgSuccess.concat('\n' + cam['Campo_Des']);
          displaySuccess = true;
        } else {
          msgFailure = msgFailure.concat('\n' + cam['Campo_Des'] + ':' + error);
          displayFailure = true;
        }
      })
      if(displaySuccess) {
        this.giasMessageService.successMessage(msgSuccess);
      }
      if(displayFailure) {
        //this.giasMessageService.warningMessage(msgFailure);
        this.giasDialogService.baseError("", msgFailure);
      }
    }
    this.clearSelezionati();
  }

}
