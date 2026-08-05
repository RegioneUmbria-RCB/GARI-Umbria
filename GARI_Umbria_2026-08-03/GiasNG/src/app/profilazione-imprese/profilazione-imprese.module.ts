import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { IconsModule } from '@progress/kendo-angular-icons';
import { LOCALIZATION_LANGUAGES } from 'app/Model/CostantiPersonalizzate';
import { ProfilazioneImpreseComponent } from './profilazione-imprese.component';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { LabelModule } from '@progress/kendo-angular-label';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { UikitModule } from 'app/Utility/uikit.module';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { ProfilazioneImpreseRoutingModule } from './profilazione-imprese-routing.module';
import { ProfilazioneImpreseService } from './services/profilazione-imprese.service';
import { ProfilazioneImpreseMacchineOperatoriComponent } from './pages/profilazione-imprese-macchine-operatori/profilazione-imprese-macchine-operatori.component';
import { ProfilazioneImpreseDefaultPianiColturaliComponent } from './pages/profilazione-imprese-default-piani-colturali/profilazione-imprese-default-piani-colturali.component';
import { ProfilazioneImpreseMacchineDialogComponent } from './components/profilazione-imprese-macchine-dialog/profilazione-imprese-macchine-dialog.component';
import { ProfilazioneImpreseNoteComponent } from './pages/profilazione-imprese-note/profilazione-imprese-note.component';
import { ProfilazioneImpreseGlobalComponent } from './components/profilazione-imprese-global/profilazione-imprese-global.component';
import { ProfilazioneImpreseSpecieComponent } from './components/profilazione-imprese-specie/profilazione-imprese-specie.component';
import { ProfilazioneImpreseGruppoOperazioneComponent } from './components/profilazione-imprese-gruppo-operazione/profilazione-imprese-gruppo-operazione.component';
import { ProfilazioneImpreseLavorazioneComponent } from './components/profilazione-imprese-lavorazione/profilazione-imprese-lavorazione.component';
import { ProfilazioneImpreseMacchineComponent } from './components/profilazione-imprese-macchine/profilazione-imprese-macchine.component';
import { ProfilazioneImpreseContattiComponent } from './components/profilazione-imprese-contatti/profilazione-imprese-contatti.component';
import { ProfilazioneImpreseCampoApplicativoComponent } from './components/profilazione-imprese-campo-applicativo/profilazione-imprese-campo-applicativo.component';
import { ProfilazioneImpreseNoteListComponent } from './components/profilazione-imprese-note-list/profilazione-imprese-note-list.component';
import { ProfilazioneImpreseGruppoNoteDialogComponent } from './components/profilazione-imprese-gruppo-note-dialog/profilazione-imprese-gruppo-note-dialog.component';
import { ProfilazioneImpreseVarietaComponent } from './components/profilazione-imprese-varieta/profilazione-imprese-varieta.component';
import { ProfilazioneImpreseDefaultSpecieComponent } from './components/profilazione-imprese-default-specie/profilazione-imprese-default-specie.component';
import { ProfilazioneImpreseParametriGeneraliColturaComponent } from './components/profilazione-imprese-parametri-generali-coltura/profilazione-imprese-parametri-generali-coltura.component';
import { ProfilazioneImpreseDefaultDistintaProduzioneComponent } from './components/profilazione-imprese-default-distinta-produzione/profilazione-imprese-default-distinta-produzione.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
  acc[lang] = () => import(`./i18n/${lang}.json`);
  return acc;
}, {});

@NgModule({
  declarations: [
    ProfilazioneImpreseComponent,
    ProfilazioneImpreseMacchineOperatoriComponent,
    ProfilazioneImpreseNoteComponent,
    ProfilazioneImpreseDefaultPianiColturaliComponent,
    ProfilazioneImpreseMacchineDialogComponent,
    ProfilazioneImpreseGlobalComponent,
    ProfilazioneImpreseSpecieComponent,
    ProfilazioneImpreseGruppoOperazioneComponent,
    ProfilazioneImpreseLavorazioneComponent,
    ProfilazioneImpreseMacchineComponent,
    ProfilazioneImpreseContattiComponent,
    ProfilazioneImpreseCampoApplicativoComponent,
    ProfilazioneImpreseNoteListComponent,
    ProfilazioneImpreseGruppoNoteDialogComponent,
    ProfilazioneImpreseVarietaComponent,
    ProfilazioneImpreseDefaultSpecieComponent,
    ProfilazioneImpreseParametriGeneraliColturaComponent,
    ProfilazioneImpreseDefaultDistintaProduzioneComponent
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    IconsModule,
    ButtonsModule,
    InputsModule,
    LabelModule,
    LayoutModule,
    UikitModule,
    FontAwesomeModule,
    TranslocoRootModule,
    ProfilazioneImpreseRoutingModule,
    GiasKendoGridModule,
    GiasUikitModule
  ],
  providers: [
    ProfilazioneImpreseService,
    {
      provide: TRANSLOCO_SCOPE,
      useValue: {
        scope: 'profImprese',
        loader
      }
    },
  ]
})
export class ProfilazioneImpreseModule {
}
