import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AmministrazionSistemaRoutingModule } from './amministrazione-sistema-routing.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { UikitModule } from 'app/Utility/uikit.module';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { IconsModule } from '@progress/kendo-angular-icons';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
//import { PermessiTreeComponent } from './menu-profilazione/profili-e-permessi/permessi-tree/permessi-tree.component';
import { LabelModule } from '@progress/kendo-angular-label';
import { LOCALIZATION_LANGUAGES } from 'app/Model/CostantiPersonalizzate';
import { TRANSLOCO_SCOPE } from '@jsverse/transloco';

import { InputsModule } from '@progress/kendo-angular-inputs';
import { LayoutModule } from '@progress/kendo-angular-layout';

import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { ConsultaSincroDatiAppComponent } from './dati-app/consulta-sincro-dati-app/consulta-sincro-dati-app.component';
import { GiasDropDownTemplateService  } from 'gias-ui-kit';
import { GridPublicService, GiasKendoGridModule} from 'gias-kendo-grid';
import { GiasMultiSelectTemplateService } from 'gias-ui-kit';
import { ConsultaSincroLogGridService } from './dati-app/consulta-sincro-dati-app/consulta-sincro-log-grid.service';
import { GiasUikitModule } from 'gias-ui-kit';


export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
  acc[lang] = () => import(`./i18n/${lang}.json`);
  return acc;
}, {});

@NgModule({
  declarations: [
  
    ConsultaSincroDatiAppComponent
  ],
  imports: [
      CommonModule,
      AmministrazionSistemaRoutingModule,
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
      DateInputsModule,
      FormsModule,
      ReactiveFormsModule,
      LabelModule,
      GiasKendoGridModule,
      GiasUikitModule
  ],
  providers:[
    {
      provide: TRANSLOCO_SCOPE,
      useValue: {
          scope: 'amministrazione-sistema',
          loader
      }
  }, GiasMultiSelectTemplateService,
  ConsultaSincroLogGridService,
  GiasDropDownTemplateService 
  ]
})
export class AmministrazioneSistemaModule { }
