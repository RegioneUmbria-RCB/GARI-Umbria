import { Component, OnDestroy } from "@angular/core";
import { generateGridProviders } from 'gias-kendo-grid';
import { Subscription } from "rxjs";
import { InvestimentoCatastaleCampoGridService } from "./investimento-catastale-campo-grid.service";
import {CampiServiceProvider} from '../../../Service/ServiceFactory/campi.factory.provider';

@Component({
    standalone: false,
    selector: 'app-investimento-catastale-campo',
    templateUrl: './investimento-catastale-campo.component.html',
    providers: [
        ...generateGridProviders(InvestimentoCatastaleCampoGridService, InvestimentoCatastaleCampoComponent), CampiServiceProvider
    ]
})
export class InvestimentoCatastaleCampoComponent implements OnDestroy {
    private Sub: Subscription;
    ngOnDestroy(): void {
        if (this.Sub) {
            this.Sub.unsubscribe();
        }
    }

}
