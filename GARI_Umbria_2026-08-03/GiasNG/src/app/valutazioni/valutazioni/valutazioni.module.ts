import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { WindowModule } from '@progress/kendo-angular-dialog';
import { IconsModule } from '@progress/kendo-angular-icons';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { NavigationModule } from '@progress/kendo-angular-navigation';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { ValutazioniComponent } from './valutazioni.component';
import { ValutazioniRoutingModule } from './valutazioni-routing.module';
import { UikitModule } from 'app/Utility/uikit.module';
import { ValutazioniEditComponent } from './main-valutazioni-edit/valutazioni-edit/valutazioni-edit.component';
import { AnnoValutazioniEditComponent } from './main-valutazioni-edit/valutazioni-edit/anno-valutazioni-edit/anno-valutazioni-edit.component';
import { MainValutazioniEditComponent } from './main-valutazioni-edit/main-valutazioni-edit.component';
import { ValutazioneEditDettaglioComponent } from './main-valutazioni-edit/valutazione-edit-dettaglio/valutazione-edit-dettaglio.component';
import { ValutazioniStatoPatrimonialeComponent } from './main-valutazioni-edit/valutazione-edit-dettaglio/valutazioni-stato-patrimoniale/valutazioni-stato-patrimoniale.component';
import { ValutazioniContoEconomicoComponent } from './main-valutazioni-edit/valutazione-edit-dettaglio/valutazioni-conto-economico/valutazioni-conto-economico.component';
import { ValutazioniStatoPatrimonialeDettaglioComponent } from './main-valutazioni-edit/valutazione-edit-dettaglio/valutazioni-stato-patrimoniale/valutazioni-stato-patr-det/valutazioni-stato-patr-det.component';
import { ContoEconomicoDettaglioComponent } from './main-valutazioni-edit/valutazione-edit-dettaglio/valutazioni-conto-economico/conto-economico-dettaglio/conto-economico-dettaglio.component';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';

@NgModule({
    declarations: [
        ValutazioniComponent,
        ValutazioniEditComponent,
        AnnoValutazioniEditComponent,
        MainValutazioniEditComponent,
        ValutazioneEditDettaglioComponent,
        ValutazioniStatoPatrimonialeComponent,
        ValutazioniContoEconomicoComponent,
        ValutazioniStatoPatrimonialeDettaglioComponent,
        ContoEconomicoDettaglioComponent
    ],
    exports: [
        NavigationModule
    ],
    providers: [
        ReactiveFormsModule
    ],
    imports: [
        CommonModule,
        HttpClientModule,
        ReactiveFormsModule,
        TranslocoRootModule,
        FontAwesomeModule,
        FormsModule,
        NavigationModule,
        IconsModule,
        ButtonsModule,
        WindowModule,
        LayoutModule,
        ValutazioniRoutingModule,
        UikitModule,
        GiasKendoGridModule,
        GiasUikitModule
    ]
})

export class ValutazioniModule {
}

