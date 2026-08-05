import {AfterViewInit, Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { QdCService } from 'app/quaderno-di-campagna/agenda-edit/service/qdc.service';
import { QdCMultiOperazioneService } from 'app/quaderno-di-campagna/agenda-edit/service/testata/multi-operazione.service';
import { QdCVisibilitaControlliTestataService } from 'app/quaderno-di-campagna/agenda-edit/service/testata/visibilita-controlli-testata-service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { OperazioniService, SalvaOperazioniPreferite } from 'app/Service/Metaschema/operazioni.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { GiasMultiSelectTemplateService } from 'gias-ui-kit';
import { MultiSelectFormItem } from 'gias-ui-kit';
import { skip, Subscription } from 'rxjs';
import { QdCTestataService } from '../../../service/testata/testata.service';
import { Tipo_Attivita} from 'gias-ui-kit';
import { GiiasMultiselectTemplateSComponent } from 'gias-ui-kit';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';
import {CodiciXOperazione} from "../../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import {RibaltamentoTypes} from "../../../../../menu-agenda/components/utils";
import { ObjParametriAgenda } from 'gias-ui-kit';


@Component({
  standalone: false,
  selector: 'app-multi-operazione',
  templateUrl: './multi-operazione.component.html',
  styleUrls: ['./multi-operazione.component.scss'],
  providers: [
              GiasMultiSelectTemplateService,
              QdCMultiOperazioneService
            ]
})
export class MultiOperazioneComponent implements OnInit,OnDestroy,AfterViewInit {

  Elenco_Operazioni_da_Escludere: Array<number> = [];

  Subs: Subscription= new Subscription();

  Gestione_Operazioni_PopUp = false;

  showDDLSpecieAnimale = false;

  @ViewChild("MultiSelectOperazioni") MultiSelectOperazioni:GiiasMultiselectTemplateSComponent;

  constructor(public testataservice: QdCTestataService,
              private operazioniservice: OperazioniService,
              private giasmessaggeservice: GiasMessageService,
              private multiService:GiasMultiSelectTemplateService,
              private translocoService: TranslocoService,
              private permessiUtenteService: PermessiUtenteService,
              public visibilitacontrollitestataservice: QdCVisibilitaControlliTestataService,
              public multioperazioniservice: QdCMultiOperazioneService,
              public qdcservice: QdCService,
              private objParametriAgendaService: ObjParametriAgendaService) { }

    ngAfterViewInit() {
      if(!this.qdcservice.TestataForm.get("flagVisita").value) {
        this.multioperazioniservice.MultiSelectOperazioni = this.MultiSelectOperazioni.multiselect;
      }
    }

    ngOnInit(): void {

    if(this.qdcservice.TestataForm){
        this.multioperazioniservice.DisabilitabtnGroupVisualizza_Solo_Operazioni_Preferite();
    }

    this.showDDLSpecieAnimale = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_ZOO_IN_VISITA).Valore === '1';
 
    this.Subs.add(this.multiService.currentMultiSelectValueObject.pipe(skip(1)).subscribe(async (multiElem: MultiSelectFormItem)=>{
        switch(multiElem.FormControlName){
            case 'Operazioni':

                this.multioperazioniservice.changeOperazioni(multiElem);

                break;
        }
    }));

    this.AbilitaDisabilitaMultiOperazione();
  }

  AbilitaDisabilitaMultiOperazione(){

    //Se sono in Info o Modifica di una operazione disabilito la multiselect delle operazioni per aggiungerli o toglierli
    //Anche se sono in modifica di un multicentro lo disabilito oppure se sono in ribaltamento di una ricetta in agenda
    //Disabilito la possibilita di scelta dell'operazione se sto facendo una operazione che ha collegato un rilievo
    let objParametriAgenda: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    if(this.qdcservice.Sola_Lettura_QdCForm() || objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Update || this.qdcservice.TestataForm.getRawValue().MultiCentro ||
        (this.qdcservice.TipoRibaltamento !== RibaltamentoTypes.Nessuno) ||
        (!this.qdcservice.TestataForm.get("flagVisita").getRawValue() &&
          this.qdcservice.TestataForm.get("Codici_Attivita").getRawValue() &&
          this.qdcservice.TestataForm.get("Codici_Attivita").getRawValue().findIndex((c: CodiciXOperazione)=>c.CodiceAttivitaVisita && c.CodiceAttivitaVisita !== "" && + c.CodiceAttivitaVisita > 0))> -1){

        this.qdcservice.TestataForm.get("Operazioni").disable({ emitEvent: false });
    }else{
        this.qdcservice.TestataForm.get("Operazioni").enable({ emitEvent: false });
    }
  }

  ngOnDestroy(): void {
      this.Subs.unsubscribe();
  }

 SalvaOperazioniPreferite(){
    let Operazioni_Scelte: Array<Lavorazione> = this.qdcservice.TestataForm.get("Operazioni").value;

    if(Operazioni_Scelte && Operazioni_Scelte.length > 0){

        const params: SalvaOperazioniPreferite = {
            lista_Lav_Cod: Operazioni_Scelte.map((operazione) => +operazione.primaryKey.codice)
        };

        this.operazioniservice.SalvaOperazioniPreferite(params).then(r=>{
            if(r.RispostaOK)
                this.giasmessaggeservice.successMessage(this.translocoService.translate('qdc.Salvataggio_Operazioni_Preferite_Completato'),false);
        });

    }
  }

  changeVisualizzaOperazioni(e: boolean){
    let a = 0; //Commento per funzione vuota SonarQube
  }

  closePopUp() {
    this.Gestione_Operazioni_PopUp = false;
    this.giasmessaggeservice.setErrorOpen(false);
  }

    mostraBtnGestioneOperazioniPreferite(){
      let mostra = true;

      //Gestione delle operazioni preferite disponibile solo se stiamo facendo un'operazione di agenda

      if(this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB === Enum_DBTypeOperation.Update ||
          this.qdcservice?.TestataForm?.get("Tipo").value !== Tipo_Attivita.QuadernoDiCampagna)
          mostra = false;

      return mostra;
    }

}
