import {Component, OnInit} from '@angular/core';
import {enum_LAVCOD, enum_PagineGiasNG} from 'app/Model/TipiEnumerativi';
import {GestioneRichiesteService} from 'app/Service/gestione-richieste.service';
import {
  ObjParametriAgendaService
} from 'app/Service/obj-parametri-agenda.service';
import {QdCService} from '../../service/qdc.service';
import {Gias2010Redirector} from '../../../../menu-agenda/components/grid-qdc/Gias2010Redirector.service';
import {TranslocoService} from '@jsverse/transloco';
import {GridImpiantoSelezionatoModel} from "../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {RibaltamentoTypes} from "../../../../menu-agenda/components/utils";
import {CentroAziendale} from "../../../../Model/anagrafiche/CentroAziendale";
import { ObjParametriAgenda, ImpiantiAgendaNG } from 'gias-ui-kit';

@Component({
    standalone: false,
    selector: 'btn-rileva-fase-epoca',
    templateUrl: './rileva-fase-epoca.component.html',
    styleUrls: ['./rileva-fase-epoca.component.css']
})
export class RilevaFaseEpocaComponent implements OnInit {

    constructor(private gestioneRichiesteService: GestioneRichiesteService,
            private giasRedirector: Gias2010Redirector,
            private objParametriAgendaService: ObjParametriAgendaService,
            private qdcservice: QdCService,
            private transloco: TranslocoService
    ) {  }

    ngOnInit(): void {
    }

    ApriRilievo() {
      this.setNewObjParametri();

      this.gestioneRichiesteService.gestionePassaggioStessoSito_Aperto_in_Iframe(enum_PagineGiasNG.Pagina_Edit_Attivita,
        [],-1,this.transloco.translate('RilievoFiorituraFasiFenologiche')).then(GiasIFrameWindowService=>{

        GiasIFrameWindowService.window.window.onDestroy(()=>{
          this.handelWindowClose();
        });
      });
    }

    private handelWindowClose() {
        this.gestioneRichiesteService.ObjPageToMemorize = null;
        this.qdcservice.RicaricaGridImpianti();
    }

    private setNewObjParametri() {

        const newobjParametriAgenda: ObjParametriAgenda = JSON.parse(JSON.stringify(this.objParametriAgendaService.getObjParamValue()));

        newobjParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
        newobjParametriAgenda.Lav_Cod = enum_LAVCOD.FASI_FENOLOGICHE;
        newobjParametriAgenda.Lav_Des = this.transloco.translate('RilievoFiorituraFasiFenologiche');
        newobjParametriAgenda.Veg_Cod = this.qdcservice.GetSpeciefromUtilizzoTerreno().codice;
        newobjParametriAgenda.Veg_Des = this.qdcservice.GetSpeciefromUtilizzoTerreno().descrizione;
        newobjParametriAgenda.Sa_Cod = (this.qdcservice.TestataForm.get("Centro_Aziendale").getRawValue() as CentroAziendale).primaryKey.codice;
        newobjParametriAgenda.SaNome = (this.qdcservice.TestataForm.get("Centro_Aziendale").getRawValue() as CentroAziendale).nome;

        newobjParametriAgenda.Pagina_Provenienza = this.qdcservice.masterService.getCurrentPageAsValue();
        newobjParametriAgenda.Impianti = [];

        if(this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue() && this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue().length > 0){
            newobjParametriAgenda.Impianti = this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue().map((el: GridImpiantoSelezionatoModel) => {
                return <ImpiantiAgendaNG>{
                    Piva: el.PIVA,
                    Sa_Cod: el.SA_COD,
                    Appezza: el.APPEZZA,
                    Id_Reg: el.ID_REG,
                    Progetto_Cod: el.Progetto_Cod,
                    Sup_Imp: el.Sup_Imp,
                    Sup_Imp_help: el.Sup_Imp_help,
                    Sup_Riduzione_BufferZone: el.Sup_Riduzione_BufferZone,
                    Perc_Riduzione_Deriva: el.Perc_Riduzione_Deriva
                }
            });
        }
        newobjParametriAgenda.Data = this.qdcservice.TestataForm.value.Data;

        let obj = this.giasRedirector.setQdCFormModelforRedirect(newobjParametriAgenda,RibaltamentoTypes.Nessuno);

        newobjParametriAgenda.GenericObj_string = JSON.stringify(obj);

        this.objParametriAgendaService.changeObjParametriAgenda(newobjParametriAgenda);
    }

    MostraBtnRilevaFase(){
        let mostraBtn=false;
        let Operazioni = this.qdcservice.TestataForm.get('Operazioni').getRawValue();
        let flag_visita = this.qdcservice.TestataForm.get('flagVisita').getRawValue();

        if(Operazioni && Operazioni.length > 0 && !flag_visita){
            let index = Operazioni.findIndex(o=>
            {
                let lav_cod = + o.primaryKey.codice;

                if(lav_cod === enum_LAVCOD.SEMINA ||
                   lav_cod === enum_LAVCOD.FASI_FENOLOGICHE)
                    return o;
            });

            if(index === -1)
                mostraBtn = true;
        }

        return mostraBtn;
    }

}
