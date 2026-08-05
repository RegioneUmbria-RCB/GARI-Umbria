import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { WindowModule } from '@progress/kendo-angular-dialog';
import { IconsModule } from '@progress/kendo-angular-icons';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { NavigationModule } from '@progress/kendo-angular-navigation';
import { LOCALIZATION_LANGUAGES } from 'app/Model/CostantiPersonalizzate';
import { TranslocoRootModule } from 'app/transloco/transloco-root.module';
import { UikitModule } from 'app/Utility/uikit.module';
import { BudgetRoutingModule } from './budget-routing.module';
import { RouterModule } from '@angular/router';
import { BudgetComponent } from './budget.component';
import { BudgetLoadedGuard } from './guards/transloco-budget-guard.service';
import { GiasUikitModule } from 'gias-ui-kit';

export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
    acc[lang] =  () => import(`./i18n/${lang}.json`);
    return acc;
}, {});

@NgModule({
    declarations: [
        BudgetComponent
    ],
    exports: [
        NavigationModule,
    ],
    providers: [
        BudgetLoadedGuard,
        {
            provide: TRANSLOCO_SCOPE,
            useValue: {
                scope: 'budget',
                loader
            }
        }],
    imports: [CommonModule,
        HttpClientModule,
        ReactiveFormsModule,
        UikitModule,
        TranslocoRootModule,
        FontAwesomeModule,
        NavigationModule,
        IconsModule,
        ButtonsModule,
        WindowModule,
        RouterModule,
        LayoutModule,
        BudgetRoutingModule,
        GiasUikitModule
    ]
})

export class BudgetModule {
}
