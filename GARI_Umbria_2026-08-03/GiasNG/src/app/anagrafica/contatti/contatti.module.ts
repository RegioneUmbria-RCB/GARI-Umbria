import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UikitModule } from 'app/Utility/uikit.module';
import { ContattiAssociaUtenteComponent } from './contatti-associa-utente/contatti-associa-utente.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { IconsModule } from '@progress/kendo-angular-icons';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { LabelModule } from '@progress/kendo-angular-label';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';



@NgModule({
    declarations: [
    
    ContattiAssociaUtenteComponent
  ],
    imports: [
        CommonModule,
        FormsModule,
        UikitModule,
        ReactiveFormsModule,
        LayoutModule,
        IconsModule,
        TranslocoRootModule,
        FontAwesomeModule,
        ButtonsModule,
        InputsModule,
        LabelModule,
        GiasKendoGridModule,
        GiasUikitModule
    ]
})
export class ContattiModule { }
