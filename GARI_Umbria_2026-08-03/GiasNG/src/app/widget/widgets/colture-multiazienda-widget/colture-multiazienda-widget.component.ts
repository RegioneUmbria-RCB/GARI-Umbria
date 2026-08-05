import { Component, Input, OnDestroy, OnInit } from "@angular/core";
import { SeriesLabelsContentArgs } from "@progress/kendo-angular-charts";
import { IntlService } from "@progress/kendo-angular-intl";
import { WidgetStatisticheClient, Widget_Statistics_IN } from "app/Service/net-core6-api.service";
import { GestioneMultiAziendaService } from "app/widget-config/gestione-multiazienda.service";
import { combineLatest, finalize, Subject } from "rxjs";

@Component({
    standalone: false,
    selector: 'app-colture-multiazienda-widget',
    templateUrl: './colture-multiazienda-widget.component.html',
    styleUrls: ['./colture-multiazienda-widget.component.scss']
})
export class ColtureMultiAziendaWidget implements OnInit, OnDestroy {

    @Input() codice: string | null = null;
    data = [];
    loading = true;
    chartPie = true;

    private signal$ = new Subject<void>();

    constructor(
        private widgetsClient: WidgetStatisticheClient,
        private gestioneMultiAziendaService: GestioneMultiAziendaService,
        private intl: IntlService
    ) {
        this.labelContent = this.labelContent.bind(this);
    }

    ngOnInit(): void {
        combineLatest({
            nazione: this.gestioneMultiAziendaService.nazioneMultiAzienda$,
            anno: this.gestioneMultiAziendaService.annoMultiAzienda$
        }).subscribe(({ nazione, anno }) => {
            this.loadData(nazione, anno);
        });
    }

    ngOnDestroy(): void {
        this.signal$.next();
        this.signal$.complete();
    }

    loadData(nazione: any, anno: any) {
        this.loading = true;

        if (nazione?.code != "") {

            const payload = {
                Year: anno?.text,
                Country: nazione?.code
            } as Widget_Statistics_IN;

            this.widgetsClient
                .widgetStatisticheGetCropMap(payload)
                .pipe(finalize(() => this.loading = false))
                .subscribe(data => {
                    const dataRisposta = JSON.parse(data.RispostaStringa);
                    dataRisposta.forEach((val) => {
                        if (val["Tot_Ha_xUsage"] != -1)
                            this.data.unshift({ 'category': val["Usage_Des"], 'totHaUsage': val["Tot_Ha_xUsage"], 'perc': val["Perc"], 'value': val["Perc"] });
                    });
                });

        }
        this.loading = false;
    }

    public labelContent(args: SeriesLabelsContentArgs): string {
        return `${this.intl.formatNumber(args.dataItem.totHaUsage, "n1")} Ha ${this.intl.formatNumber(args.dataItem.value, "n1")} %`;
    }

}
