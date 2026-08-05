import { Component } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { AgronicaChatGPTClient, PrevisioniChatGPT_IN, Widget_PrevisioniAI, Widget_PrevisioniAI_IN, WidgetsClient } from "app/Service/api.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { CostsRevenuesTot } from "./costi-ricavi-ai-widget.model";
import { ObjParametriAgenda } from 'gias-ui-kit';

@Component({
    standalone: false,
    selector: 'costi-ricavi-ai-widget',
    templateUrl: './costi-ricavi-ai-widget.component.html',
    styleUrls: ['./costi-ricavi-ai-widget.component.scss']
})
export class CostiRicaviAIWidgetComponent {

    loading = false;

    data: CostsRevenuesTot;

    allCosts = 0;
    allRevenues = 0;

    reportBalance = [];

    comDes: string;

    // callChatGPT: boolean = false;

    constructor(private objParamAgendaService: ObjParametriAgendaService,
        private widgetsService: WidgetsClient,
        private chatGPTservice: AgronicaChatGPTClient,
        private transloco: TranslocoService) {

        this.loading = true;

        let objParamAgenda: ObjParametriAgenda = this.objParamAgendaService.getObjParamValue();

        //leggiamoci i dettagli dell'impresa, perchè ci servirà la città da passare
        //a ChatGPT per costruirsi i costi/ricavi

        let today = new Date();

        let param = {
            Piva: objParamAgenda.Piva,
            DataStats: today
        } as Widget_PrevisioniAI_IN;

        this.widgetsService.widgetsReadStatistichePrevisioniAI(param).subscribe(risp => {
            // console.log(risp.RispostaStringa);

            let arrayCrops: Widget_PrevisioniAI[] = risp.RispostaStringa;

            if (arrayCrops.length > 0 /*&& this.callChatGPT*/) {

                let paramForChatGPT = {
                    year: today.getFullYear(),
                    arrayCrops: arrayCrops
                } as PrevisioniChatGPT_IN;

                this.chatGPTservice.agronicaChatGPTPrevisioniAI(paramForChatGPT).subscribe(rispWidget => {

                    if (rispWidget.RispostaStringa !== '') {
                        this.data = JSON.parse(rispWidget.RispostaStringa);

                        this.data.CostsCrops.forEach(item => {
                            this.allCosts += item.costs;
                        });

                        this.data.RevenuesCrops.forEach(item => {
                            this.allRevenues += item.revenues;
                        });

                        this.reportBalance = [
                            { name: this.transloco.translate("Costi"), values: this.allCosts },
                            { name: this.transloco.translate("Ricavi"), values: this.allRevenues },
                            { name: this.transloco.translate("Utile"), values: this.allRevenues - this.allCosts }
                        ];

                    }

                    this.loading = false;

                });

            } else
                this.loading = false;

        });
    }

    formatCurrency = (e: any) => {
        return new Intl.NumberFormat('it-IT', { style: 'currency', currency: 'EUR' }).format(e.value);
    };

}