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
import { UikitModule } from 'app/Utility/uikit.module';
import { PianoContiComponent } from './piano-conti.component';
import { PianoContiRoutingModule } from './piano-conti-routing.module';
import { PianoContiEditComponent } from './piano-conti-edit/piano-conti-edit.component';
import { InputMaskModule } from '@ngneat/input-mask';
import { GiasKendoGridModule } from 'gias-kendo-grid';
import { GiasUikitModule } from 'gias-ui-kit';

@NgModule({
    declarations: [
      PianoContiComponent,
      PianoContiEditComponent
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
        UikitModule,
        PianoContiRoutingModule,
        InputMaskModule,
        GiasKendoGridModule,
        GiasUikitModule
    ]
})

export class PianoContiModule {
}
