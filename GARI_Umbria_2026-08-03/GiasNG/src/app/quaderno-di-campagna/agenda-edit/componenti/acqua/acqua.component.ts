import { Component, OnInit } from '@angular/core';
import { FormGroupDirective } from '@angular/forms';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { enum_LAVCOD, SIMBOLO_M3_HA } from 'app/Model/TipiEnumerativi';
import { QdCService } from '../../service/qdc.service';
import {MisceleService} from "../../service/miscele.service";
import {ObjParametriAgendaService} from "../../../../Service/obj-parametri-agenda.service";
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {Acqua} from "../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import {TranslocoService} from "@jsverse/transloco";
import {RibaltamentoTypes} from "../../../../menu-agenda/components/utils";
import { Tipo } from 'app/Model/attivita/centri_di_costo/CentroDiCosto';
import {FunzioniComuniService} from "../../../../Service/FunzioniComuni.service";

@Component({
    standalone: false,
    selector: 'app-acqua',
    templateUrl: './acqua.component.html',
    styleUrls: ['./acqua.component.scss']
})
export class AcquaComponent implements OnInit {
    constructor(public parent: FormGroupDirective,
                public qdcservice: QdCService,
                private misceleservice: MisceleService,
                private objParametriAgendaService: ObjParametriAgendaService,
                private translocoService: TranslocoService,
                private funzionicomuniservice: FunzioniComuniService) { }

    ngOnInit(): void {

        //Imposto in automatico l'acqua dalla taratura ugello della macchina appena vengono mostrate le textbox e sono in scrittura ma non durante un ribaltamento
        if(this.qdcservice.mostraAcqua() && this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB === Enum_DBTypeOperation.Write && this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Nessuno){

            this.qdcservice.Imposta_VolumiAcqua(0,null,"").then(risp=>{
                //Triggero il ricalcolo dell'acqua
                if(risp){
                    this.misceleservice.calcoloMiscele('changeAcqua_Ha',null,0);
                }
            });

        }

    }

    changeAcqua_Ha(value){
        this.misceleservice.calcoloMiscele('changeAcqua_Ha',null,0);
    }

    changeAcqua_Tot(value){
        this.misceleservice.calcoloMiscele('changeAcqua_Tot',null,0);
    }

    componi_descrizione_Acqua(){
        let descrizione = ""

        if(this.qdcservice.EsisteProdottoPolverulento())
            descrizione += this.translocoService.translate("TrattamentoPolverulento");

        return descrizione;
    }

    getUdM(): string {
        return this.qdcservice.getCentroDiCostoTipo() == Tipo.ProdottoDaTrattare ? 'q' : 'Ha';
    }

    getProvenienza(): string {
        if (this.qdcservice.getCentroDiCostoTipo() == Tipo.ProdottoDaTrattare) {
            return this.qdcservice.AcquaForm.get('Acqua_Ha').value > 0
                ? this.qdcservice.obj_Acqua_Provenienza.Descrizione
                : '';
        }

        return this.qdcservice.obj_Acqua_Provenienza.Descrizione;
    }

    public ShowLblAcquaM3(): boolean{

      let show = false;

      let Operazioni: Array<Lavorazione> = this.qdcservice.TestataForm.get("Operazioni").getRawValue();

      if(Operazioni && Operazioni.findIndex(o=>+o.primaryKey.codice === enum_LAVCOD.FERTIRRIGAZIONE) > -1)
        show = true;

      return show;
    }

    public GetDescrizioneAcquaHaM3(): string{
      let descrizione: string = this.translocoService.translate("Quantita")+" 0 " + SIMBOLO_M3_HA;

      let Acqua_Ha_Hl: number = this.qdcservice.AcquaForm.get("Acqua_Ha").getRawValue();

      if(Acqua_Ha_Hl > 0){

        let Acqua_Ha_M3: number = this.funzionicomuniservice.roundNumber(Acqua_Ha_Hl / 10, this.qdcservice.get4DecimalNumericSettings().decimals);

        let Acqua_Ha_M3_Str: string = this.qdcservice.decimalpipe.transform(Acqua_Ha_M3, this.qdcservice.digitsInfo_QdC_4_Decimal, this.qdcservice.locale_id);

        descrizione = this.translocoService.translate("Quantita")+" "+Acqua_Ha_M3_Str+" " + SIMBOLO_M3_HA;
      }


      return descrizione;
    }

    public GetDescrizioneAcquaTotM3(): string{
      let descrizione: string = this.translocoService.translate("qdc.rblAcqua_TotResource1.Text")+" 0 m3";

      let Acqua_Acqua_Tot_Hl: number = this.qdcservice.AcquaForm.get("Acqua_Tot").getRawValue();

      if(Acqua_Acqua_Tot_Hl > 0){

        let Acqua_Tot_M3: number = this.funzionicomuniservice.roundNumber(Acqua_Acqua_Tot_Hl / 10, this.qdcservice.get4DecimalNumericSettings().decimals);

        let Acqua_Tot_M3_Str: string = this.qdcservice.decimalpipe.transform(Acqua_Tot_M3, this.qdcservice.digitsInfo_QdC_4_Decimal, this.qdcservice.locale_id);

        descrizione = this.translocoService.translate("qdc.rblAcqua_TotResource1.Text")+" "+Acqua_Tot_M3_Str+" m3";
      }


      return descrizione;
    }
}
