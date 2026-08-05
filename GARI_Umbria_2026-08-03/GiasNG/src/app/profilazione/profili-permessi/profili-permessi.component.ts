import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import { TipologiaUtente } from '../models/profili-permessi/tipologia-utente.model';
import { TipologieUtentiService } from 'app/profilazione/services/profili-permessi/tipologie-utenti.service';
import {ExpansionPanelComponent} from "@progress/kendo-angular-layout";
import {Subject, takeUntil} from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-profili-permessi',
  templateUrl: './profili-permessi.component.html',
  styleUrls: ['./profili-permessi.component.css'],
  providers: [ TipologieUtentiService ]
})
export class ProfiliPermessiComponent implements OnInit, OnDestroy {
    @ViewChild('PanelPermessi') panelPermessi : ExpansionPanelComponent;
    public profileCod = 0;
    public isEditing = false;
    protected isRecapTableExpanded = false;
    private _onDestroy$ = new Subject<boolean>();

    constructor(
        private profileService: TipologieUtentiService
    ) { }

    public get profiloSelezionato(): TipologiaUtente {
        return this.profileService.selectedProfile$.value;
    }

    ngOnInit(): void {
        this.profileService.isEditing
          .pipe(takeUntil(this._onDestroy$))
          .subscribe(val => this.isEditing = val);
        this.profileService.selectedProfile$
            .pipe(takeUntil(this._onDestroy$))
            .subscribe(p => this.showPermissions(p));
    }

    ngOnDestroy(): void {
        this._onDestroy$.next(true);
        this._onDestroy$.complete();
    }

    showPermissions(profile: TipologiaUtente) {
        if (!profile || (profile.codice === this.profileCod && !this.isEditing)) {
          this.panelPermessi?.toggle(false);
          this.profileCod = 0;
          return;
        }
        if (profile.codice === this.profileCod && this.isEditing)
          return;
        else {
          this.profileCod = profile.codice;
          this.profileService.isEditing.next(false);
          this.panelPermessi.toggle(true);
        }
    }

}
