import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import { enum_PaginaImpostazioni } from '../../impostazioni.model';
import { ImpostazioniFormService } from '../../../services/impostazioni/impostazioni-form.service';
import { ImpostazioniAziendeCentriService } from '../../../services/impostazioni/impostazioni-aziende-centri.service';
import {Subject, takeUntil} from "rxjs";

@Component({
  standalone: false,
  selector: 'app-pagina-impostazioni-massive',
  templateUrl: './pagina-impostazioni-massive.component.html',
  styleUrls: ['./pagina-impostazioni-massive.component.css']
})
export class PaginaImpostazioniMassiveComponent implements OnInit, OnDestroy {

    @ViewChild('Settings') Settings: any;
    public editingSettings = false;
    private onDestroy$ = new Subject<boolean>();

    constructor(
        private utentiSettings: ImpostazioniFormService,
        private aziendeSettings: ImpostazioniAziendeCentriService
    ) { }

    ngOnInit(): void {
        this.utentiSettings.USAGE_AREA = enum_PaginaImpostazioni.AZIENDE_CENTRI;
        this.handleEvents();
    }

    ngOnDestroy(): void {
        this.onDestroy$.next(true);
        this.onDestroy$.complete();
    }

    private handleEvents() {
        this.aziendeSettings.selectionChange$.pipe(takeUntil(this.onDestroy$))
            .subscribe(selected => this.editingSettings = selected.length === 1);
    }

}
