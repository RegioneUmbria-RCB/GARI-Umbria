import { Component, OnDestroy } from "@angular/core";
import { InvestimentoCatastaleServiceProvider } from "app/Service/ServiceFactory/investimento-catastale.factory.provider";
import { generateGridProviders } from 'gias-kendo-grid';
import { Subscription } from "rxjs";
import { InvestimentoCatastaleGridService } from "./investimento-catastale-grid.service";

@Component({
    standalone: false,
    selector: 'app-investimento-catastale',
    templateUrl: './investimento-catastale.component.html',
    providers: [
        ...generateGridProviders(InvestimentoCatastaleGridService, InvestimentoCatastaleComponent), InvestimentoCatastaleServiceProvider
    ]
})
export class InvestimentoCatastaleComponent implements OnDestroy {
    private Sub: Subscription;
    ngOnDestroy(): void {
        if (this.Sub) {
            this.Sub.unsubscribe();
        }
    }

}
