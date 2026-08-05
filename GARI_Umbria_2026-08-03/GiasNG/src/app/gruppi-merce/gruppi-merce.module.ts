import { CommonModule } from '@angular/common'
import { HttpClientModule } from '@angular/common/http'
import { NgModule } from '@angular/core'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { RouterModule } from '@angular/router'
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome'
import { TRANSLOCO_SCOPE } from '@jsverse/transloco'
import { ButtonsModule } from '@progress/kendo-angular-buttons'
import { IconsModule } from '@progress/kendo-angular-icons'
import { LabelModule } from '@progress/kendo-angular-label'
import { LayoutModule } from '@progress/kendo-angular-layout'
import { LOCALIZATION_LANGUAGES } from 'app/Model/CostantiPersonalizzate'

import { TranslocoRootModule } from 'app/transloco/transloco-root.module'
import { UikitModule } from 'app/Utility/uikit.module'
import { EditGruppiMerceComponent } from './gestioneGruppiMerce/editGruppiMerce.component'
import { EditGruppiMerceGridComponent } from './gestioneGruppiMerce/gruppiMerceGrid/editGruppiMerce.component'
import { GruppiMerceMasterComponent } from './gruppi-merce-master.component'
import { GruppiMerceRoutingModule } from './gruppi-merce-routing.module'
import { GruppiMerceGrid2Component } from './gruppiUtentiPerGruppiMerce/gruppiMerceGrid/gruppiMerceGrid2.component'
import { GruppiUtentiGridComponent } from './gruppiUtentiPerGruppiMerce/gruppiUtentiGrid/gruppiUtentiGrid.component'
import { GruppiUtentiPerGruppiMerce } from './gruppiUtentiPerGruppiMerce/gruppiUtentiPerGruppiMerce.component'
import { GruppiUtentixGruppiMerceGridComponent } from './gruppiUtentiPerGruppiMerce/gruppiUtentixGruppiMerceGrid/gruppiUtentixGruppiMerceGrid.component'
import { GiasKendoGridModule } from 'gias-kendo-grid'
import { GiasUikitModule } from 'gias-ui-kit'


export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
  acc[lang] = () => import(`./i18n/${lang}.json`);
  return acc;
}, {});

@NgModule({
  declarations: [
    GruppiMerceMasterComponent,
    GruppiUtentiPerGruppiMerce,
    GruppiUtentiGridComponent,
    GruppiUtentixGruppiMerceGridComponent,
    GruppiMerceGrid2Component,
    
    EditGruppiMerceComponent,
    EditGruppiMerceGridComponent
  ],
  imports: [
    CommonModule,
    GruppiMerceRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    IconsModule,
    ButtonsModule,
    LayoutModule,
    UikitModule,
    FontAwesomeModule,
    TranslocoRootModule,
    FormsModule,
    RouterModule,
    ReactiveFormsModule,
    LabelModule,
    GiasKendoGridModule,
    GiasUikitModule
],
providers:[
  {
    provide: TRANSLOCO_SCOPE,
    useValue: {
        scope: 'grMerci',
        loader
    }
},
]
})
export class GruppiMerceModule { }
