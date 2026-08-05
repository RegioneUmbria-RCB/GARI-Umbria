import {Component, forwardRef, Input, OnInit} from '@angular/core';
import {KendoGridRow} from 'gias-kendo-grid';
import {AppBarComponent, AppBarSectionComponent} from '@progress/kendo-angular-navigation';
import {FaIconComponent} from '@fortawesome/angular-fontawesome';
import {NgForOf, NgIf} from '@angular/common';
import {RouterLink, RouterLinkActive} from '@angular/router';
import {TranslocoPipe} from '@jsverse/transloco';
import {UikitModule} from '../../../Utility/uikit.module';
import {PermessiUtenteService} from '../../../Service/permessi-utente.service';
import {enum_TipoPermesso} from '../../../profilazione/models/profilazione.model';
import {AgriculturalPlotSlopeComponent} from './agricultural-plots/agricultural-plot-slope/agricultural-plot-slope.component';
import {AnagraficaModule} from '../../anagrafica.module';
import {AgriculturalPlotWeavingComponent} from './agricultural-plots/agricultural-plot-weaving/agricultural-plot-weaving.component';
import {enum_Security_Attivita} from '../../../Model/TipiEnumerativi';
import {ReplaySubject} from 'rxjs';
import {AnagraficaService} from '../../anagrafica.service';
import {
  AgriculturalExerciseConstrainComponent
} from './exercices/agricultural-exercise-constrain/agricultural-exercise-constrain.component';

export const SpecialEdit = {
  PlotSlope: enum_Security_Attivita.GestioneAppezzamentiPendenza,
  PlotWeaving: enum_Security_Attivita.GestioneAppezzamentiTessitura,
  PlotExerciseConstrain: enum_Security_Attivita.GestioneEserciziVincoli
} as const;
export type SpecialEditKey = typeof SpecialEdit[keyof typeof SpecialEdit];

export class TabstripItem {
  constructor(
    public title: string,
    public id: SpecialEditKey
  ) {}
}

@Component({
  standalone: true,
  selector: 'app-special-edit',
  imports: [
    AppBarComponent,
    AppBarSectionComponent,
    FaIconComponent,
    NgIf,
    RouterLink,
    RouterLinkActive,
    TranslocoPipe,
    UikitModule,
    NgForOf,
    AgriculturalPlotSlopeComponent,
    AgriculturalPlotWeavingComponent,
    AgriculturalExerciseConstrainComponent,
    forwardRef(() => AnagraficaModule)
  ],
  templateUrl: './special-edit.component.html',
  styleUrl: './special-edit.component.css',
  providers: [AnagraficaService]
})
export class SpecialEditComponent implements OnInit{
  @Input() protected windowHeight$: ReplaySubject<number> = new ReplaySubject<number>(1);
  @Input() protected checkedRows$: ReplaySubject<KendoGridRow[]> = new ReplaySubject<KendoGridRow[]>(1);

  protected readonly tabs: TabstripItem[] = [];

  protected readonly SpecialEdit = SpecialEdit;
  private selectedPage: TabstripItem;

  constructor(
    private permessiUtente: PermessiUtenteService
  ) {
    this.tabs = Object.keys(SpecialEdit).map(i => {
      return new TabstripItem(i, SpecialEdit[i]);
    }).filter(ti => {
      return this.permessiUtente.getPermesso(
        ti.id,
        enum_TipoPermesso.LETTURA_SCRITTURA
      );
    });

    this.selectedPage = this.tabs.find(t => this.isUserAllowed(t.id, enum_TipoPermesso.LETTURA_SCRITTURA));
  }

  ngOnInit(): void {
  }

  isUserAllowed(tab: SpecialEditKey, permission: enum_TipoPermesso): boolean {
    return this.permessiUtente.getPermesso(tab.valueOf(), permission);
  }

  canEditWeaving(): boolean {
    return this.permessiUtente.getPermesso(
      enum_Security_Attivita.GestioneAppezzamentiTessitura,
      enum_TipoPermesso.LETTURA_SCRITTURA
    );
  }

  canEditSlope(): boolean {
    return this.permessiUtente.getPermesso(
      enum_Security_Attivita.GestioneAppezzamentiPendenza,
      enum_TipoPermesso.LETTURA_SCRITTURA
    );
  }

  canEditConstrain(): boolean {
    return this.permessiUtente.getPermesso(
      enum_Security_Attivita.GestioneEserciziVincoli,
      enum_TipoPermesso.LETTURA_SCRITTURA
    );
  }

  protected onTabSelect(e) {
    this.selectedPage = this.tabs.find(t => t.id = SpecialEdit[e.title]);
  }

  protected isTabSelected(page: TabstripItem): boolean {
    return page === this.selectedPage;
  }
}
