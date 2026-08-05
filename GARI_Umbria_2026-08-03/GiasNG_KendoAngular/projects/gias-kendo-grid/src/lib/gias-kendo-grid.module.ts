import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DateInputsModule, DateRangeModule } from '@progress/kendo-angular-dateinputs';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { LabelModule } from '@progress/kendo-angular-label';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ExcelModule, GridModule, PDFModule } from '@progress/kendo-angular-grid';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { A11yModule } from '@angular/cdk/a11y';
import { ClipboardModule } from '@angular/cdk/clipboard';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { PortalModule } from '@angular/cdk/portal';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { CdkStepperModule } from '@angular/cdk/stepper';
import { CdkTableModule } from '@angular/cdk/table';
import { CdkTreeModule } from '@angular/cdk/tree';
import { GiasKendoGridComponent } from './Template/kendo-grid/kendo-grid.component';
import { MenusModule } from '@progress/kendo-angular-menu';
import { KendoGridCustomColumnDirective } from './Template/kendo-grid/directives/kendo-grid-info-command.directive';
import { GridNumericComponent } from './Template/kendo-grid/components/grid-numeric/grid-numeric.component';
import { CellTypePipe } from './Template/kendo-grid/pipes/get-cell-type.pipe';
import { ToolbarColumnMenuComponent } from './Template/kendo-grid/components/toolbar-column-menu/toolbar-column-menu.component';
import { PopupAnchorDirective } from './Template/kendo-grid/directives/popup.anchor-target.directive';
import { GridDropdownListComponent } from './Template/kendo-grid/components/grid-dropdownlists/grid-dropdownlist.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { MultiCheckFilterComponent } from './Template/kendo-grid/components/multicheck-filter/multicheck-filter.component';
import { ShortenPipe } from './Template/kendo-grid/pipes/shorten.pipe';
import { GridCustomizationsComponent } from './Template/kendo-grid/components/grid-customizations/grid-customizations.component';
import { CellDropdownNamePipe } from './Template/kendo-grid/pipes/get-ddl-cell-name.pipe';
import { CustomizationDropdownComponent } from './Template/kendo-grid/components/grid-customizations/dropdown/customization-dropdown.component';
import { AggregateTypePipe } from './Template/kendo-grid/pipes/aggregateType.pipe';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { FilterTypePipe } from './Template/kendo-grid/pipes/filter-type.pipe';
import { NavigationModule } from '@progress/kendo-angular-navigation';
import { HttpClientJsonpModule, HttpClientModule } from '@angular/common/http';
import { TreeViewModule } from '@progress/kendo-angular-treeview';
import { SafePipe } from './Template/Pipes/safePipe/safe.pipe';
import { CellMultiDropdownNamePipe } from './Template/kendo-grid/pipes/get-multi-ddl-cell-name.pipe';
import { GridMultiDropdownComponent } from './Template/kendo-grid/components/grid-multi-dropdownlist/grid-multi-dropdownlist.component';
import { ScrollingModule as ExperimentalScrollingModule } from '@angular/cdk-experimental/scrolling';
import { GridCustomComponent } from './Template/kendo-grid/components/custom/grid-custom.component';
import { GroupHeaderNamePipe } from './Template/kendo-grid/pipes/groupHeaderName.pipe';
import { DateRangeFilterComponent } from './Template/kendo-grid/components/multicheck-filter/date-range-filter/date-range-filter.component';
import { DynamicInputComponent } from './Template/kendo-grid/components/dynamic-input/dynamic-input.component';
import { FormatNumberPipe } from "./Template/kendo-grid/pipes/formatNumber.pipe";
import { StringToDatePipe } from "./Template/kendo-grid/pipes/stringToDate.pipe";
import { GridBooleanComponent } from './Template/kendo-grid/components/grid-boolean/grid-boolean.component';
import { RouterModule } from '@angular/router';
import { TranslocoModule } from '@jsverse/transloco';
import { itTranslations } from '../assets/i18n/it';
import { enTranslations } from '../assets/i18n/en';
import { ptTranslations } from '../assets/i18n/pt';
import { frTranslations } from '../assets/i18n/fr';
import { GiasUikitModule } from 'gias-ui-kit';


export const giasGridTranslocoLoader = (lang) => {
  // These are not loaded using http because we will be inside the node modules folder
  // and the http client will not be able to find the files
  if (lang === 'it') {
    return itTranslations;
  }

  if (lang === 'en') {
    return enTranslations;
  }

  if (lang === 'pt') {
    return ptTranslations;
  }

  if (lang === 'fr') {
    return frTranslations;
  }

  return {};
}

@NgModule({
  declarations: [
    GiasKendoGridComponent,
    ToolbarColumnMenuComponent,
    KendoGridCustomColumnDirective,
    GridNumericComponent,
    CellTypePipe,
    CellDropdownNamePipe,
    CellMultiDropdownNamePipe,
    GroupHeaderNamePipe,
    GridMultiDropdownComponent,
    GridDropdownListComponent,
    MultiCheckFilterComponent,
    ShortenPipe,
    GridCustomizationsComponent,
    CustomizationDropdownComponent,
    AggregateTypePipe,
    StringToDatePipe,
    FormatNumberPipe,
    FilterTypePipe,
    PopupAnchorDirective,
    SafePipe,
    GridCustomComponent,
    DateRangeFilterComponent,
    DynamicInputComponent,
    GridBooleanComponent
  ],
  exports: [
    GiasKendoGridComponent,
    ToolbarColumnMenuComponent,
    KendoGridCustomColumnDirective,
    GridNumericComponent,
    CellTypePipe,
    CellDropdownNamePipe,
    CellMultiDropdownNamePipe,
    GroupHeaderNamePipe,
    GridMultiDropdownComponent,
    GridDropdownListComponent,
    MultiCheckFilterComponent,
    ShortenPipe,
    GridCustomizationsComponent,
    CustomizationDropdownComponent,
    AggregateTypePipe,
    StringToDatePipe,
    FormatNumberPipe,
    FilterTypePipe,
    PopupAnchorDirective,
    SafePipe,
    GridCustomComponent,
    DateRangeFilterComponent,
    DynamicInputComponent,
    GridBooleanComponent
  ],
  imports: [
    A11yModule,
    ClipboardModule,
    CdkStepperModule,
    CdkTableModule,
    CdkTreeModule,
    DragDropModule,
    CommonModule,
    GridModule,
    PDFModule,
    ExcelModule,
    PDFModule,
    DropDownsModule,
    LabelModule,
    ButtonsModule,
    InputsModule,
    LayoutModule,
    DateInputsModule,
    FormsModule,
    ReactiveFormsModule,
    PortalModule,
    ScrollingModule,
    MenusModule,
    FontAwesomeModule,
    TooltipModule,
    NavigationModule,
    TreeViewModule,
    HttpClientModule,
    HttpClientJsonpModule,
    ScrollingModule,
    ExperimentalScrollingModule,
    DateRangeModule,
    RouterModule,
    TranslocoModule,
    GiasUikitModule
  ],
  providers: []
})
export class GiasKendoGridModule { }
