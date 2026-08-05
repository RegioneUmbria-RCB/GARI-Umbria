import { ButtonsModule, } from '@progress/kendo-angular-buttons';
import { CommonModule } from '@angular/common';
import { DateInputsModule, DateRangeModule } from '@progress/kendo-angular-dateinputs';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { GiasCheckboxTemplateComponent } from './gias-checkbox-template/gias-checkbox-template.component';
import { GiasCodiceTemplateComponent } from './gias-codice-template/gias-codice-template.component';
import { GiasDataTemplateComponent } from './gias-data-template/gias-data-template.component';
import { GiasDialogComponent } from './gias-dialog/gias-dialog.component';
import { GiasDropDownTemplateComponent } from './gias-drop-down-template/gias-drop-down-template.component';
import { GiasDropDownTemplateSComponent } from './gias-drop-down-template-s/gias-drop-down-template-s.component';
import { GiasFormErrorVisualizerComponent } from './gias-form-error-visualizer/gias-form-error-visualizer.component';
import { GiasInfoTemplateComponent } from './gias-info-template/gias-info-template.component';
import { GiasMasterSpinnerComponent } from './gias-master-spinner/gias-master-spinner.component';
import { GiasNumericTemplateComponent } from './gias-numeric-template/gias-numeric-template.component';
import { GiasPasswordTemplateComponent } from './gias-password-template/gias-password-template.component';
import { GiasRadioButtonTemplateComponent } from './gias-radio-button-template/gias-radio-button-template.component';
import { GiasSwitchTemplateComponent } from './gias-switch-template/gias-switch-template.component';
import { GiasTextTemplateComponent } from './gias-text-template/gias-text-template.component';
import { GiasWaitFrameComponent } from './gias-wait-frame/gias-wait-frame.component';
import { HttpClientJsonpModule, HttpClientModule } from '@angular/common/http';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { LabelModule } from '@progress/kendo-angular-label';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { TranslocoModule } from '@jsverse/transloco';
import { GiasExpansionPanelComponent } from './gias-expansion-panel/gias-expansion-panel.component';
import { GiasUpdateToolbarComponent } from './gias-update-toolbar/gias-update-toolbar.component';
import { NavigationModule } from '@progress/kendo-angular-navigation';
import { GiiasMultiselectTemplateSComponent } from './gias-multiselect-template-s/gias-multiselect-template-s.component';
import { GiasValiditaComponent } from './gias-validita/gias-validita.component';
import { GiasValiditaDateTimeComponent } from './gias-validita-datetime/gias-validita-datetime.component';
import { GiasDateTimePickerTemplateComponent } from './gias-date-time-picker-template/gias-date-time-picker-template.component';
import { GiasBinaryButtonGroupTemplateComponent } from './gias-binary-button-group-template/gias-binary-button-group-template.component';
import { GiasButtonGroupTemplateComponent } from './gias-button-group-template/gias-button-group-template.component';
import { GiasSafePipe } from './pipes/safe/safe.pipe';
import { GiasJoinPipe } from './pipes/join/join.pipe';
import { GiasIFrameWindowComponent } from './gias-iframe-window/gias-iframe-window.component';
import { GiasMultiColumnComboboxTemplateComponent } from './gias-multi-column-combobox-template/gias-multi-column-combobox-template.component';
import { GiasGeneralInputTemplateComponent } from './gias-general-input-template/gias-general-input-template.component';
import { GiasPanelBar } from './gias-panelbar/gias-panelbar.component';
import { GiasKendoWindowTemplates } from './gias-kendo-window-templates/gias-kendo-window-templates.component'; 
import { GiasTextAreaTemplateComponent } from './gias-text-area-template/gias-text-area-template.component';
import { GiasTimePickerTemplateComponent } from './gias-time-picker/gias-time-picker-template.component';
import { GiasSideFiltersTemplateComponent } from './gias-side-filters-template/gias-side-filters-template.component';
import {ListviewComponent} from './components/kendo/listview/listview.component';
import {
  AddCommandDirective, EditCommandDirective,
  HeaderTemplateDirective,
  ItemTemplateDirective,
  ListViewComponent,
  LoaderTemplateDirective, RemoveCommandDirective
} from '@progress/kendo-angular-listview';
import {GiasDropDownButtonComponent} from './components/drop-down-button/gias-drop-down-button.component';

// export const loader = LOCALIZATION_LANGUAGES.reduce((acc, lang) => {
//   acc[lang] = () => import(`./i18n/${lang}.json`);
//   return acc;
// }, {});

@NgModule({
  declarations: [
    GiasBinaryButtonGroupTemplateComponent,
    GiasButtonGroupTemplateComponent,
    GiasCheckboxTemplateComponent,
    GiasCodiceTemplateComponent,
    GiasDataTemplateComponent,
    GiasDateTimePickerTemplateComponent,
    GiasDialogComponent,
    GiasDropDownTemplateComponent,
    GiasDropDownTemplateSComponent,
    GiasExpansionPanelComponent,
    GiasFormErrorVisualizerComponent,
    GiasGeneralInputTemplateComponent,
    GiasIFrameWindowComponent,
    GiasInfoTemplateComponent,
    GiasKendoWindowTemplates,
    GiasMasterSpinnerComponent,
    GiasMultiColumnComboboxTemplateComponent,
    GiiasMultiselectTemplateSComponent,
    GiasNumericTemplateComponent,
    GiasPanelBar,
    GiasPasswordTemplateComponent,
    GiasRadioButtonTemplateComponent,
    GiasSideFiltersTemplateComponent,
    GiasSwitchTemplateComponent,
    GiasTextAreaTemplateComponent,
    GiasTextTemplateComponent,
    GiasTimePickerTemplateComponent,
    GiasUpdateToolbarComponent,
    GiasValiditaComponent,
    GiasValiditaDateTimeComponent,
    GiasWaitFrameComponent,
    GiasJoinPipe,
    GiasSafePipe,
    ListviewComponent,
    GiasDropDownButtonComponent
  ],
  exports: [
    GiasBinaryButtonGroupTemplateComponent,
    GiasButtonGroupTemplateComponent,
    GiasCheckboxTemplateComponent,
    GiasCodiceTemplateComponent,
    GiasDataTemplateComponent,
    GiasDateTimePickerTemplateComponent,
    GiasDialogComponent,
    GiasDropDownTemplateComponent,
    GiasDropDownTemplateSComponent,
    GiasExpansionPanelComponent,
    GiasFormErrorVisualizerComponent,
    GiasGeneralInputTemplateComponent,
    GiasIFrameWindowComponent,
    GiasInfoTemplateComponent,
    GiasKendoWindowTemplates,
    GiasMasterSpinnerComponent,
    GiasMultiColumnComboboxTemplateComponent,
    GiiasMultiselectTemplateSComponent,
    GiasNumericTemplateComponent,
    GiasPanelBar,
    GiasPasswordTemplateComponent,
    GiasRadioButtonTemplateComponent,
    GiasSideFiltersTemplateComponent,
    GiasSwitchTemplateComponent,
    GiasTextAreaTemplateComponent,
    GiasTextTemplateComponent,
    GiasTimePickerTemplateComponent,
    GiasUpdateToolbarComponent,
    GiasValiditaComponent,
    GiasValiditaDateTimeComponent,
    GiasWaitFrameComponent,
    GiasJoinPipe,
    GiasSafePipe,
    ListviewComponent,
    GiasDropDownButtonComponent
  ],
  imports: [
    ButtonsModule,
    CommonModule,
    DateInputsModule,
    DateRangeModule,
    DropDownsModule,
    FontAwesomeModule,
    FormsModule,
    HttpClientJsonpModule,
    HttpClientModule,
    InputsModule,
    LabelModule,
    LayoutModule,
    MatInputModule,
    MatSlideToggleModule,
    NavigationModule,
    ReactiveFormsModule,
    RouterModule,
    TooltipModule,
    TranslocoModule,
    ListViewComponent,
    ItemTemplateDirective,
    LoaderTemplateDirective,
    HeaderTemplateDirective,
    AddCommandDirective,
    RemoveCommandDirective,
    EditCommandDirective
  ],
  providers: [
    // {
    //   provide: TRANSLOCO_SCOPE,
    //   useValue: {
    //     scope: 'uikit',
    //     loader
    //   }
    // }
  ],
})
export class GiasUikitModule {
}
